using UnityEngine;
using System.Collections;
public static class TextureGenerator {

	public static Texture2D GenerationTextureFromHeihtMap(float[,] GenerationHeightMap)
	{
		int widthTextureHeightMap = GenerationHeightMap.GetLength(0);
		int heightTextureHeightMap = GenerationHeightMap.GetLength(1);
		Color[] GenerationColourMap = new Color[widthTextureHeightMap * heightTextureHeightMap];
		for (int y = 0; y < heightTextureHeightMap; y++)
		{
			for (int x = 0; x < widthTextureHeightMap; x++)
			{
				GenerationColourMap[y * widthTextureHeightMap + x] = Color.Lerp(Color.black, Color.white, GenerationHeightMap[x, y]);
			}
		}
		return GeneratedLandscapeTextureFromColourMap(GenerationColourMap, widthTextureHeightMap, heightTextureHeightMap);
	}
	public static Texture2D GeneratedLandscapeTextureFromColourMap(Color[] GenerationColourMap, int widthColourMap, int heightColourMap) 
	{
		Texture2D textureGeneratorColour = new Texture2D (widthColourMap, heightColourMap);
		textureGeneratorColour.filterMode = FilterMode.Point;
		textureGeneratorColour.wrapMode = TextureWrapMode.Clamp;
		textureGeneratorColour.SetPixels (GenerationColourMap);
		textureGeneratorColour.Apply ();
		return textureGeneratorColour;
	}
	
}
