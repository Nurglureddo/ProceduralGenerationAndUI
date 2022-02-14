using UnityEngine;
using System.Collections;

public class PerlinMeshGenData
{
	int[] meshGenDatatriangles;
	int[] meshGenDataborderTriangles;
	int meshGenDatatriangleIndex;
	int meshGenDataborderTriangleIndex;
	Vector3[] meshGenDataVertices;
	Vector2[] meshGenDataUvs;
	Vector3[] meshGenDataNormals;
	Vector3[] meshGenDataBorderVertices;
	public PerlinMeshGenData(int mapMeshVerticesPerLine)
	{
		meshGenDataVertices = new Vector3[mapMeshVerticesPerLine * mapMeshVerticesPerLine];
		meshGenDataUvs = new Vector2[mapMeshVerticesPerLine * mapMeshVerticesPerLine];
		meshGenDatatriangles = new int[(mapMeshVerticesPerLine - 1) * (mapMeshVerticesPerLine - 1) * 6];

		meshGenDataBorderVertices = new Vector3[mapMeshVerticesPerLine * 4 + 4];
		meshGenDataborderTriangles = new int[24 * mapMeshVerticesPerLine];
	}
	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int mapVertexIndex)
	{
		if (mapVertexIndex < 0)
		{
			meshGenDataBorderVertices[-mapVertexIndex - 1] = vertexPosition;
		}
		else
		{
			meshGenDataVertices[mapVertexIndex] = vertexPosition;
			meshGenDataUvs[mapVertexIndex] = uv;
		}
	}
	public void AddTriangle(int a, int b, int c)
	{
		if (a < 0 || b < 0 || c < 0)
		{
			meshGenDataborderTriangles[meshGenDataborderTriangleIndex] = a;
			meshGenDataborderTriangles[meshGenDataborderTriangleIndex + 1] = b;
			meshGenDataborderTriangles[meshGenDataborderTriangleIndex + 2] = c;
			meshGenDataborderTriangleIndex += 3;
		}
		else
		{
			meshGenDatatriangles[meshGenDatatriangleIndex] = a;
			meshGenDatatriangles[meshGenDatatriangleIndex + 1] = b;
			meshGenDatatriangles[meshGenDatatriangleIndex + 2] = c;
			meshGenDatatriangleIndex += 3;
		}
	}
	Vector3[] CalculateMapNormals()
	{

		Vector3[] mapVertexNormals = new Vector3[meshGenDataVertices.Length];
		int maptriangleCount = meshGenDatatriangles.Length / 3;
		for (int i = 0; i < maptriangleCount; i++)
		{
			int MapGenerationNormalTriangleIndex = i * 3;
			int mapvertexIndexA = meshGenDatatriangles[MapGenerationNormalTriangleIndex];
			int mapvertexIndexB = meshGenDatatriangles[MapGenerationNormalTriangleIndex + 1];
			int mapvertexIndexC = meshGenDatatriangles[MapGenerationNormalTriangleIndex + 2];

			Vector3 MapGentriangleNormal = SurfaceNormalFromIndices(mapvertexIndexA, mapvertexIndexB, mapvertexIndexC);
			mapVertexNormals[mapvertexIndexA] += MapGentriangleNormal;
			mapVertexNormals[mapvertexIndexB] += MapGentriangleNormal;
			mapVertexNormals[mapvertexIndexC] += MapGentriangleNormal;
		}
		int MapgenBorderTriangleCount = meshGenDataborderTriangles.Length / 3;
		for (int i = 0; i < MapgenBorderTriangleCount; i++)
		{
			int MapGenerationNormalTriangleIndex = i * 3;
			int mapvertexIndexA = meshGenDataborderTriangles[MapGenerationNormalTriangleIndex];
			int mapvertexIndexB = meshGenDataborderTriangles[MapGenerationNormalTriangleIndex + 1];
			int mapvertexIndexC = meshGenDataborderTriangles[MapGenerationNormalTriangleIndex + 2];

			Vector3 MapGentriangleNormal = SurfaceNormalFromIndices(mapvertexIndexA, mapvertexIndexB, mapvertexIndexC);
			if (mapvertexIndexA >= 0)
			{
				mapVertexNormals[mapvertexIndexA] += MapGentriangleNormal;
			}
			if (mapvertexIndexB >= 0)
			{
				mapVertexNormals[mapvertexIndexB] += MapGentriangleNormal;
			}
			if (mapvertexIndexC >= 0)
			{
				mapVertexNormals[mapvertexIndexC] += MapGentriangleNormal;
			}
		}
		for (int i = 0; i < mapVertexNormals.Length; i++)
		{
			mapVertexNormals[i].Normalize();
		}
		return mapVertexNormals;
	}
	Vector3 SurfaceNormalFromIndices(int MeshIndexA, int MeshIndexB, int MeshIndexC)
	{
		Vector3 MeshpointA = (MeshIndexA < 0) ? meshGenDataBorderVertices[-MeshIndexA - 1] : meshGenDataVertices[MeshIndexA];
		Vector3 MeshpointB = (MeshIndexB < 0) ? meshGenDataBorderVertices[-MeshIndexB - 1] : meshGenDataVertices[MeshIndexB];
		Vector3 MeshpointC = (MeshIndexC < 0) ? meshGenDataBorderVertices[-MeshIndexC - 1] : meshGenDataVertices[MeshIndexC];
		Vector3 MeshsideAB = MeshpointB - MeshpointA;
		Vector3 MeshsideAC = MeshpointC - MeshpointA;
		return Vector3.Cross(MeshsideAB, MeshsideAC).normalized;
	}
	public void Normals()
	{
		meshGenDataNormals = CalculateMapNormals();
	}
	public Mesh CreateMeshMap()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = meshGenDataVertices;
		mesh.triangles = meshGenDatatriangles;
		mesh.uv = meshGenDataUvs;
		mesh.normals = meshGenDataNormals;
		return mesh;
	}
}
public static class MeshGenerator {

	public static PerlinMeshGenData PerlinNoiseGenerateTerrainMesh(float[,] GenerationHeightMap, float MeshGenheightMultiplier, AnimationCurve MeshGen_heightCurve, int MeshGenlevelOfDetail) 
	{
		AnimationCurve meshGenHeightCurve = new AnimationCurve (MeshGen_heightCurve.keys);
		int meshSimplificationGen = (MeshGenlevelOfDetail == 0)?1:MeshGenlevelOfDetail * 2;
		int meshBorderedSize = GenerationHeightMap.GetLength (0);
		int meshGenSize = meshBorderedSize - 2*meshSimplificationGen;
		int meshGenerationSizeUnsimplified = meshBorderedSize - 2;
		
		int mapMeshVerticesPerLine = (meshGenSize - 1) / meshSimplificationGen + 1;
		PerlinMeshGenData perlinMeshData = new PerlinMeshGenData (mapMeshVerticesPerLine);
		int[,] meshMapVertexIndicesMap = new int[meshBorderedSize,meshBorderedSize];
		int meshMapVertexIndex = 0;
		int meshMapBorderVertexIndex = -1;

		float maptopLeftX = (meshGenerationSizeUnsimplified - 1) / -2f;
		float maptopLeftZ = (meshGenerationSizeUnsimplified - 1) / 2f;

		for (int y = 0; y < meshBorderedSize; y += meshSimplificationGen) 
		{
			for (int x = 0; x < meshBorderedSize; x += meshSimplificationGen) {
				bool isMeshMapBorderVertex = y == 0 || y == meshBorderedSize - 1 || x == 0 || x == meshBorderedSize - 1;

				if (isMeshMapBorderVertex) 
				{
					meshMapVertexIndicesMap [x, y] = meshMapBorderVertexIndex;
					meshMapBorderVertexIndex--;
				} 
				else
				{
					meshMapVertexIndicesMap [x, y] = meshMapVertexIndex;
					meshMapVertexIndex++;
				}
			}
		}

		for (int y = 0; y < meshBorderedSize; y += meshSimplificationGen)
		{
			for (int x = 0; x < meshBorderedSize; x += meshSimplificationGen)
			{
				int mapVertexIndex = meshMapVertexIndicesMap[x, y];
				Vector2 percentGen = new Vector2((x - meshSimplificationGen) / (float)meshGenSize, (y - meshSimplificationGen) / (float)meshGenSize);
				float height = meshGenHeightCurve.Evaluate(GenerationHeightMap[x, y]) * MeshGenheightMultiplier;
				Vector3 vertexPosition = new Vector3(maptopLeftX + percentGen.x * meshGenerationSizeUnsimplified, height, maptopLeftZ - percentGen.y * meshGenerationSizeUnsimplified);

				perlinMeshData.AddVertex(vertexPosition, percentGen, mapVertexIndex);

				if (x < meshBorderedSize - 1 && y < meshBorderedSize - 1)
				{
					int a = meshMapVertexIndicesMap[x, y];
					int b = meshMapVertexIndicesMap[x + meshSimplificationGen, y];
					int c = meshMapVertexIndicesMap[x, y + meshSimplificationGen];
					int d = meshMapVertexIndicesMap[x + meshSimplificationGen, y + meshSimplificationGen];
					perlinMeshData.AddTriangle(a, d, c);
					perlinMeshData.AddTriangle(d, a, b);
				}
				mapVertexIndex++;
			}
		}


		perlinMeshData.Normals ();
		return perlinMeshData;
	}
}
