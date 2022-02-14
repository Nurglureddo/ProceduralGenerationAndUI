using UnityEngine;
using System.Collections;
using Random = System.Random;

public static class Noise {

	public enum PerlinGenNormalizeMode {Local, Global};

	public static float[,] GeneratePerlinNoiseMap(int generationMapWidth, int generationMapHeight, int PerlinNoiseSeed, float scale, int PerlinNoiseOctaves, float PerlinNoisePersistance, float PerlinNoiseLacunarity, Vector2 PerlinNoiseOffset, PerlinGenNormalizeMode genNormalizeMode) 
	{
		float[,] perlinNoiseMap = new float[generationMapWidth,generationMapHeight];
        Random seedprng = new Random(PerlinNoiseSeed);
		Vector2[] noiseOctaveOffsets = new Vector2[PerlinNoiseOctaves];
		float generationMaxHeight = 0;
		float generationAmplitude = 1;
		float generationFrequency = 1;

		for (int i = 0; i < PerlinNoiseOctaves; i++)
		{
			float PerlinNoiseOffsetX = seedprng.Next (-100000, 100000) + PerlinNoiseOffset.x;
			float PerlinNoiseOffsetY = seedprng.Next (-100000, 100000) - PerlinNoiseOffset.y;
			noiseOctaveOffsets [i] = new Vector2 (PerlinNoiseOffsetX, PerlinNoiseOffsetY);

			generationMaxHeight += generationAmplitude;
			generationAmplitude *= PerlinNoisePersistance;
		}
		if (scale <= 0)
		{
			scale = 0.0001f;
		}
		float generationMaxLocalNoiseHeight = float.MinValue;
		float generationMinLocalNoiseHeight = float.MaxValue;
		float moietyWidth = generationMapWidth / 2f;
		float moietyHeight = generationMapHeight / 2f;
		for (int y = 0; y < generationMapHeight; y++)
		{
			for (int x = 0; x < generationMapWidth; x++)
			{

				generationAmplitude = 1;
				generationFrequency = 1;
				float perlinNoiseHeight = 0;

				for (int i = 0; i < PerlinNoiseOctaves; i++) {
					float mapSampleX = (x-moietyWidth + noiseOctaveOffsets[i].x) / scale * generationFrequency;
					float mapSampleY = (y-moietyHeight + noiseOctaveOffsets[i].y) / scale * generationFrequency;

					float perlinNoiseValue = Mathf.PerlinNoise (mapSampleX, mapSampleY) * 2 - 1;
					perlinNoiseHeight += perlinNoiseValue * generationAmplitude;

					generationAmplitude *= PerlinNoisePersistance;
					generationFrequency *= PerlinNoiseLacunarity;
				}

				if (perlinNoiseHeight > generationMaxLocalNoiseHeight) {
					generationMaxLocalNoiseHeight = perlinNoiseHeight;
				} else if (perlinNoiseHeight < generationMinLocalNoiseHeight) {
					generationMinLocalNoiseHeight = perlinNoiseHeight;
				}
				perlinNoiseMap [x, y] = perlinNoiseHeight;
			}
		}
		for (int y = 0; y < generationMapHeight; y++) {
			for (int x = 0; x < generationMapWidth; x++) {
				if (genNormalizeMode == PerlinGenNormalizeMode.Local) {
					perlinNoiseMap [x, y] = Mathf.InverseLerp (generationMinLocalNoiseHeight, generationMaxLocalNoiseHeight, perlinNoiseMap [x, y]);
				} else {
					float normalizedHeightGeneration = (perlinNoiseMap [x, y] + 1) / (generationMaxHeight/0.9f);
					perlinNoiseMap [x, y] = Mathf.Clamp(normalizedHeightGeneration,0, int.MaxValue);
				}
			}
		}
		return perlinNoiseMap;
	}

}
