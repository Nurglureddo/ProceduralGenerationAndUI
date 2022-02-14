using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour 
{
	const float MapviewerMoveThresholdForChunkUpdate = 25f;
	const float MapsqrViewerMoveThresholdForChunkUpdate = MapviewerMoveThresholdForChunkUpdate * MapviewerMoveThresholdForChunkUpdate;
	const float scale = 2.5f;

	public MapLODInfo[] LevelDetail;
	public static float maxMapGenViewDst;
	public Transform GeneratedMapViewer;
	public Material mapGenMaterial;

	public static Vector2 genMapviewerPosition;

	Vector2 LandscapeMapviewerPositionOld;

	static MapGenerator EndlessTerrainmMapGenerator;

	int mapGeneratedLandscapeChunkSize;
	int GeneratedchunksVisibleInViewDst;

	Dictionary<Vector2, LandScapeTerrainChunk> MapGenterrainChunkDictionary = new Dictionary<Vector2, LandScapeTerrainChunk>();

	static List<LandScapeTerrainChunk> GeneratedterrainChunksVisibleLastUpdate = new List<LandScapeTerrainChunk>();

	void Start()
	{
		EndlessTerrainmMapGenerator = FindObjectOfType<MapGenerator>();

		maxMapGenViewDst = LevelDetail [LevelDetail.Length - 1].visibleDstThreshold;
		mapGeneratedLandscapeChunkSize = MapGenerator.GeneratedMapChunkSize - 1;
		GeneratedchunksVisibleInViewDst = Mathf.RoundToInt(maxMapGenViewDst / mapGeneratedLandscapeChunkSize);

		UpdateVisibleGeneratedChunks ();
	}

	void Update()
	{
		genMapviewerPosition = new Vector2 (GeneratedMapViewer.position.x, GeneratedMapViewer.position.z) / scale;

		if ((LandscapeMapviewerPositionOld - genMapviewerPosition).sqrMagnitude > MapsqrViewerMoveThresholdForChunkUpdate) {
			LandscapeMapviewerPositionOld = genMapviewerPosition;
			UpdateVisibleGeneratedChunks ();
		}
	}
		
	void UpdateVisibleGeneratedChunks()
	{

		for (int i = 0; i < GeneratedterrainChunksVisibleLastUpdate.Count; i++) 
		{
			GeneratedterrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		GeneratedterrainChunksVisibleLastUpdate.Clear ();
			
		int MapcurrentChunkCoordX = Mathf.RoundToInt (genMapviewerPosition.x / mapGeneratedLandscapeChunkSize);
		int MapcurrentChunkCoordY = Mathf.RoundToInt (genMapviewerPosition.y / mapGeneratedLandscapeChunkSize);

		for (int ChunkyOffset = -GeneratedchunksVisibleInViewDst; ChunkyOffset <= GeneratedchunksVisibleInViewDst; ChunkyOffset++) 
		{
			for (int ChunkxOffset = -GeneratedchunksVisibleInViewDst; ChunkxOffset <= GeneratedchunksVisibleInViewDst; ChunkxOffset++)
			{
				Vector2 viewedMapChunkCoord = new Vector2 (MapcurrentChunkCoordX + ChunkxOffset, MapcurrentChunkCoordY + ChunkyOffset);

				if (MapGenterrainChunkDictionary.ContainsKey (viewedMapChunkCoord)) 
				{
					MapGenterrainChunkDictionary [viewedMapChunkCoord].UpdateTerrainChunk ();
				} 
				else 
				{
					MapGenterrainChunkDictionary.Add (viewedMapChunkCoord, new LandScapeTerrainChunk (viewedMapChunkCoord, mapGeneratedLandscapeChunkSize, LevelDetail, transform, mapGenMaterial));
				}

			}
		}
	}

	public class LandScapeTerrainChunk 
	{

		GameObject MapGeneratemeshObject;
		Vector2 position;
		Bounds bounds;

		MeshRenderer mapDispayMeshRenderer;
		MeshFilter mapDispayMeshFilter;
		MeshCollider meshCollider;

		MapLODInfo[] LevelDetail;
		MapLODMesh[] lodMeshes;
		MapLODMesh collisionLODMesh;

		PerlinMapGenData perlinMapGenData;
		bool perlinMapGenDataReceived;
		int previousLODIndex = -1;

		void OnPerlinMapGenDataReceived(PerlinMapGenData perlinMapGenData)
		{
			this.perlinMapGenData = perlinMapGenData;
			perlinMapGenDataReceived = true;

			Texture2D texture = TextureGenerator.GeneratedLandscapeTextureFromColourMap(perlinMapGenData.GenerationColourMap, MapGenerator.GeneratedMapChunkSize, MapGenerator.GeneratedMapChunkSize);
			mapDispayMeshRenderer.material.mainTexture = texture;

			UpdateTerrainChunk();
		}



		public void UpdateTerrainChunk()
		{
			if (perlinMapGenDataReceived)
			{
				float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(genMapviewerPosition));
				bool visible = viewerDstFromNearestEdge <= maxMapGenViewDst;

				if (visible)
				{
					int lodIndex = 0;

					for (int i = 0; i < LevelDetail.Length - 1; i++)
					{
						if (viewerDstFromNearestEdge > LevelDetail[i].visibleDstThreshold)
						{
							lodIndex = i + 1;
						}
						else
						{
							break;
						}
					}

					if (lodIndex != previousLODIndex)
					{
						MapLODMesh lodMesh = lodMeshes[lodIndex];
						if (lodMesh.hasMapGenMesh)
						{
							previousLODIndex = lodIndex;
							mapDispayMeshFilter.mesh = lodMesh.mesh;
						}
						else if (!lodMesh.hasMapGenRequestedMesh)
						{
							lodMesh.GenerateRequestMesh(perlinMapGenData);
						}
					}

					if (lodIndex == 0)
					{
						if (collisionLODMesh.hasMapGenMesh)
						{
							meshCollider.sharedMesh = collisionLODMesh.mesh;
						}
						else if (!collisionLODMesh.hasMapGenRequestedMesh)
						{
							collisionLODMesh.GenerateRequestMesh(perlinMapGenData);
						}
					}

					GeneratedterrainChunksVisibleLastUpdate.Add(this);
				}

				SetVisible(visible);
			}
		}

		public void SetVisible(bool visible)
		{
			MapGeneratemeshObject.SetActive(visible);
		}

		public bool IsVisible()
		{
			return MapGeneratemeshObject.activeSelf;
		}

		public LandScapeTerrainChunk(Vector2 mapcoord, int size, MapLODInfo[] LevelDetail, Transform parent, Material material) 
		{
			this.LevelDetail = LevelDetail;

			position = mapcoord * size;
			bounds = new Bounds(position,Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x,0,position.y);

			MapGeneratemeshObject = new GameObject("Chunk");
			mapDispayMeshRenderer = MapGeneratemeshObject.AddComponent<MeshRenderer>();
			mapDispayMeshFilter = MapGeneratemeshObject.AddComponent<MeshFilter>();
			meshCollider = MapGeneratemeshObject.AddComponent<MeshCollider>();
			mapDispayMeshRenderer.material = material;

			MapGeneratemeshObject.transform.position = positionV3 * scale;
			MapGeneratemeshObject.transform.parent = parent;
			MapGeneratemeshObject.transform.localScale = Vector3.one * scale;
			SetVisible(false);

			lodMeshes = new MapLODMesh[LevelDetail.Length];
			for (int i = 0; i < LevelDetail.Length; i++) {
				lodMeshes[i] = new MapLODMesh(LevelDetail[i].lod, UpdateTerrainChunk);
				if (LevelDetail[i].useForCollider) {
					collisionLODMesh = lodMeshes[i];
				}
			}

			EndlessTerrainmMapGenerator.RequestPerlinMapGenData(position,OnPerlinMapGenDataReceived);
		}

		

	}
	[System.Serializable]
	public struct MapLODInfo
	{
		public int lod;
		public float visibleDstThreshold;
		public bool useForCollider;
	}

	class MapLODMesh
	{

		
		public bool hasMapGenRequestedMesh;
		public bool hasMapGenMesh;
		public Mesh mesh;
		int lod;
		System.Action updateCallback;

		public MapLODMesh(int lod, System.Action updateCallback) 
		{
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		void OnPerlinMeshGenDataReceived(PerlinMeshGenData perlinMeshData) 
		{
			mesh = perlinMeshData.CreateMeshMap ();
			
			hasMapGenMesh = true;

			updateCallback ();
		}
		public void GenerateRequestMesh(PerlinMapGenData perlinMapGenData) 
		{
			hasMapGenRequestedMesh = true;
			EndlessTerrainmMapGenerator.RequestPerlinMeshGenData (perlinMapGenData, lod, OnPerlinMeshGenDataReceived);
		}

	}
}
