using UnityEngine;
using System.Collections;

public class PerlinMapGenDisplay : MonoBehaviour
{
	public Renderer mapDispayTextureRender;
	public MeshFilter mapDispayMeshFilter;
	public MeshRenderer mapDispayMeshRenderer;
	public void GenerationMeshDraw(PerlinMeshGenData perlinMeshData, Texture2D texture)
	{
		mapDispayMeshFilter.sharedMesh = perlinMeshData.CreateMeshMap();
		mapDispayMeshRenderer.sharedMaterial.mainTexture = texture;
	}
	public void GenerationTextureDraw(Texture2D texture)
	{
		mapDispayTextureRender.sharedMaterial.mainTexture = texture;
		mapDispayTextureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

}
