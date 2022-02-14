using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	public enum ModeDraw {NoiseMap, ColourMap, Mesh, FalloffMap};
	public ModeDraw modeDraw;
	public Noise.PerlinGenNormalizeMode genNormalizeMode;
	public const int GeneratedMapChunkSize = 239;
	[Range(0,6)]
	public int PreviewLodEditor;
	[Range(0,1000)]
	public float PerlinNoiseScale;
	[Range(1,8)]
	public int PerlinNoiseOctaves;
	[Range(0,1)]
	public float PerlinNoisePersistance;
	public float PerlinNoiseLacunarity;
	public int PerlinNoiseSeed;
	public Vector2 PerlinNoiseOffset;
	public bool GeneratedMapUseFallOff;
	public float GeneratedMeshHeightMultiplier;
	public AnimationCurve GeneratedMeshHeightCurve;
	public bool MapGeneratorAutoUpdate;
	public TerrainType[] MapGenRegions;
	float[,] GeneratedFallOffMap;

	Queue<MapThreadInfo<PerlinMapGenData>> perlinMapGenDataThreadInfoQueue = new Queue<MapThreadInfo<PerlinMapGenData>>();
	Queue<MapThreadInfo<PerlinMeshGenData>> perlinMeshDataThreadInfoQueue = new Queue<MapThreadInfo<PerlinMeshGenData>>();

	void Awake() {
		GeneratedFallOffMap = FalloffGenerator.LandscapeGenerateFallOffMap (GeneratedMapChunkSize);
	}

	public void PerlinGenDrawMapInEditor() {
		PerlinMapGenData perlinMapGenData = GeneratePerlinMapGenData (Vector2.zero);
		PerlinMapGenDisplay perlinMapGenDisplay = FindObjectOfType<PerlinMapGenDisplay> ();
		if (modeDraw == ModeDraw.NoiseMap) {
			perlinMapGenDisplay.GenerationTextureDraw (TextureGenerator.GenerationTextureFromHeihtMap (perlinMapGenData.GenerationHeightMap));
		} else if (modeDraw == ModeDraw.ColourMap) {
			perlinMapGenDisplay.GenerationTextureDraw (TextureGenerator.GeneratedLandscapeTextureFromColourMap (perlinMapGenData.GenerationColourMap, GeneratedMapChunkSize, GeneratedMapChunkSize));
		} else if (modeDraw == ModeDraw.Mesh) {
			perlinMapGenDisplay.GenerationMeshDraw (MeshGenerator.PerlinNoiseGenerateTerrainMesh (perlinMapGenData.GenerationHeightMap, GeneratedMeshHeightMultiplier, GeneratedMeshHeightCurve, PreviewLodEditor), TextureGenerator.GeneratedLandscapeTextureFromColourMap (perlinMapGenData.GenerationColourMap, GeneratedMapChunkSize, GeneratedMapChunkSize));
		} else if (modeDraw == ModeDraw.FalloffMap) {
			perlinMapGenDisplay.GenerationTextureDraw(TextureGenerator.GenerationTextureFromHeihtMap(FalloffGenerator.LandscapeGenerateFallOffMap(GeneratedMapChunkSize)));
		}
	}
	public void RequestPerlinMapGenData
		(Vector2 centre, Action<PerlinMapGenData> generationCallBack) 
	{
		ThreadStart MapTheardStart = delegate {
			PerlinMapGenDataThread (centre, generationCallBack);
		};
		new Thread (MapTheardStart).Start ();
	}
	void PerlinMapGenDataThread
		(Vector2 centre, Action<PerlinMapGenData> generationCallBack) 
	{
		PerlinMapGenData perlinMapGenData = GeneratePerlinMapGenData (centre);
		lock (perlinMapGenDataThreadInfoQueue) {
			perlinMapGenDataThreadInfoQueue.Enqueue (new MapThreadInfo<PerlinMapGenData> (generationCallBack, perlinMapGenData));
		}
	}
	public void RequestPerlinMeshGenData
		(PerlinMapGenData perlinMapGenData, int lod, Action<PerlinMeshGenData> generationCallBack) 
	{
		ThreadStart MapTheardStart = delegate {
			PerlinMeshGenDataThread (perlinMapGenData, lod, generationCallBack);
		};
		new Thread (MapTheardStart).Start ();
	}
	void PerlinMeshGenDataThread
		(PerlinMapGenData perlinMapGenData, int lod, Action<PerlinMeshGenData> generationCallBack) 
	{
		PerlinMeshGenData perlinMeshData = MeshGenerator.PerlinNoiseGenerateTerrainMesh (perlinMapGenData.GenerationHeightMap, GeneratedMeshHeightMultiplier, GeneratedMeshHeightCurve, lod);
		lock (perlinMeshDataThreadInfoQueue) {
			perlinMeshDataThreadInfoQueue.Enqueue (new MapThreadInfo<PerlinMeshGenData> (generationCallBack, perlinMeshData));
		}
	}
	void Update() {
		if (perlinMapGenDataThreadInfoQueue.Count > 0) 
		{
			for (int i = 0; i < perlinMapGenDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<PerlinMapGenData> perlinMeshDataThreadInfo = perlinMapGenDataThreadInfoQueue.Dequeue ();
				perlinMeshDataThreadInfo.generationCallBack (perlinMeshDataThreadInfo.generationParametr);
			}
		}
		if (perlinMeshDataThreadInfoQueue.Count > 0) 
		{
			for (int i = 0; i < perlinMeshDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<PerlinMeshGenData> perlinMeshDataThreadInfo = perlinMeshDataThreadInfoQueue.Dequeue ();
				perlinMeshDataThreadInfo.generationCallBack (perlinMeshDataThreadInfo.generationParametr);
			}
		}
	}

	PerlinMapGenData GeneratePerlinMapGenData(Vector2 centre) 
	{
		float[,] perlinNoiseMap = Noise.GeneratePerlinNoiseMap (GeneratedMapChunkSize + 2, GeneratedMapChunkSize + 2, PerlinNoiseSeed, PerlinNoiseScale, PerlinNoiseOctaves, PerlinNoisePersistance, PerlinNoiseLacunarity, centre + PerlinNoiseOffset, genNormalizeMode);
		Color[] GenerationColourMap = new Color[GeneratedMapChunkSize * GeneratedMapChunkSize];

		for (int y = 0; y < GeneratedMapChunkSize; y++) {
			for (int x = 0; x < GeneratedMapChunkSize; x++) {
				if (GeneratedMapUseFallOff) {
					perlinNoiseMap [x, y] = Mathf.Clamp01(perlinNoiseMap [x, y] - GeneratedFallOffMap [x, y]);
				}
				float mapGenDataCurrentHeight = perlinNoiseMap [x, y];
				for (int i = 0; i < MapGenRegions.Length; i++) {
					if (mapGenDataCurrentHeight >= MapGenRegions [i].height) {
						GenerationColourMap [y * GeneratedMapChunkSize + x] = MapGenRegions [i].colour;
					} 
					else 
					{
						break;
					}
				}
			}
		}


		return new PerlinMapGenData (perlinNoiseMap, GenerationColourMap);
	}

	void OnValidate() 
	{
		if (PerlinNoiseLacunarity < 1) {
			PerlinNoiseLacunarity = 1;
		}
		if (PerlinNoiseOctaves < 0) {
			PerlinNoiseOctaves = 0;
		}
		GeneratedFallOffMap = FalloffGenerator.LandscapeGenerateFallOffMap (GeneratedMapChunkSize);
	}
	struct MapThreadInfo<GenerationT> {
		public readonly Action<GenerationT> generationCallBack;
		public readonly GenerationT generationParametr;
		public MapThreadInfo (Action<GenerationT> generationCallBack, GenerationT generationParametr)
		{
			this.generationCallBack = generationCallBack;
			this.generationParametr = generationParametr;

		}
	}
}
[System.Serializable]
public struct TerrainType {
	public string namename;
	public float height;
	public Color colour;
}
public struct PerlinMapGenData {
	public readonly float[,] GenerationHeightMap;
	public readonly Color[] GenerationColourMap;

	public PerlinMapGenData (float[,] GenerationHeightMap, Color[] GenerationColourMap)
	{
		this.GenerationHeightMap = GenerationHeightMap;
		this.GenerationColourMap = GenerationColourMap;
	}
}
