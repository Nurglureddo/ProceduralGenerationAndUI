using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]

public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {

		MapGenerator perlinNoiseMapGeneration = (MapGenerator)target;

		if (DrawDefaultInspector ()) {
			if (perlinNoiseMapGeneration.MapGeneratorAutoUpdate) {
				perlinNoiseMapGeneration.PerlinGenDrawMapInEditor ();
			}
		}

		if (GUILayout.Button ("Generate landscape")) {
			perlinNoiseMapGeneration.PerlinGenDrawMapInEditor ();
		}
	}
}
