using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace McTerrain {
	[CustomEditor(typeof(MarchingCubesGrid))]
	public class MarchingCubesGridEditor : Editor {

		private SerializedProperty mapChunksText;
		private SerializedProperty mapHeightText;
		private SerializedProperty amplidtudeText;
		private SerializedProperty xScalarText;
		private SerializedProperty yScalarText;
		private SerializedProperty xOffsetText;
		private SerializedProperty zOffsetText;
		private SerializedProperty isoLevelText;

		bool terrainSizeToggle = false;
		bool perlinSettingsToggle = false;
		bool fallOffSettingsToggle = false;

		private bool isSculptMode = false;
		private float brushSize = 4.0f;
		private float brushStrength = 0.5f;
		private float brushHardness = 0.5f;
		private bool isSmoothMode = false;
		private bool leftMouseDown = false;
		private bool shiftDown = false;


		void OnEnable() {
			SceneView.beforeSceneGui += onSceneViewBeforeSceneGui;

			this.mapChunksText = serializedObject.FindProperty("chunks");
			this.mapHeightText = serializedObject.FindProperty("mapHeight");
			this.amplidtudeText = serializedObject.FindProperty("amplitude");
			this.xScalarText = serializedObject.FindProperty("xScalar");
			this.yScalarText = serializedObject.FindProperty("yScalar");

			this.xOffsetText = serializedObject.FindProperty("xOffset");
			this.zOffsetText = serializedObject.FindProperty("zOffset");

			this.isoLevelText = serializedObject.FindProperty("isoLevel");
		}


		void OnDisable() {
			SceneView.beforeSceneGui -= onSceneViewBeforeSceneGui;

			leftMouseDown = false;
			shiftDown = false;
		}


		public override void OnInspectorGUI() {
			GUI.changed = false;

			EditorGUI.BeginChangeCheck();

			terrainSizeToggle = EditorGUILayout.BeginToggleGroup("Edit Terrain Size", terrainSizeToggle);
			EditorGUILayout.PropertyField(mapChunksText);
			EditorGUILayout.PropertyField(mapHeightText);
			EditorGUILayout.EndToggleGroup();

			perlinSettingsToggle = EditorGUILayout.BeginToggleGroup("Edit Perlin Settings", perlinSettingsToggle);
			EditorGUILayout.PropertyField(amplidtudeText);
			EditorGUILayout.PropertyField(xScalarText);
			EditorGUILayout.PropertyField(yScalarText);
			EditorGUILayout.PropertyField(xOffsetText);
			EditorGUILayout.PropertyField(zOffsetText);
			EditorGUILayout.PropertyField(isoLevelText);
			EditorGUILayout.EndToggleGroup();

			fallOffSettingsToggle = EditorGUILayout.BeginToggleGroup("Fall Off Settings", fallOffSettingsToggle);
			EditorGUILayout.TextArea("TODO");
			EditorGUILayout.EndToggleGroup();


			MarchingCubesGrid mcGrid = getTargetGrid();


			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();

				mcGrid.updateTerrainDataFromInspector();
				mcGrid.editModeDestoryChunks();
				mcGrid.createGrid();
			}

			isSculptMode = EditorGUILayout.BeginToggleGroup("Enable sculpt mode", isSculptMode);
			this.brushSize = EditorGUILayout.FloatField("Brush Size", this.brushSize);
			this.brushStrength = EditorGUILayout.FloatField("Brush Strength", this.brushStrength);
			this.brushHardness = EditorGUILayout.FloatField("Brush Hardness", this.brushHardness);
			this.isSmoothMode = EditorGUILayout.Toggle("Smooth Mode", this.isSmoothMode);
			EditorGUILayout.EndToggleGroup();

			if (GUILayout.Button("Generate mesh")) {
				mcGrid.editModeDestoryChunks();
				mcGrid.createGrid();
			}

		}


		void onSceneViewBeforeSceneGui(SceneView sceneView) {

			if (Application.isEditor) {
				if (isSculptMode) {

					if (Event.current.type == EventType.MouseDown) {
						if (Event.current.button == 0) {
							leftMouseDown = true;
							Event.current.Use();
						}
					}
					if (Event.current.type == EventType.MouseUp) {
						if (Event.current.button == 0) {
							leftMouseDown = false;
							Event.current.Use();
						}
					}
					if (Event.current.type == EventType.KeyDown) {
						if (Event.current.keyCode == KeyCode.LeftShift) {
							shiftDown = true;
							Event.current.Use();
						}
					}
					if (Event.current.type == EventType.KeyUp) {
						if (Event.current.keyCode == KeyCode.LeftShift) {
							shiftDown = false;
							Event.current.Use();
						}
					}


					if (leftMouseDown) {
						Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit, 10000f)) {
							MarchingCubesGrid mcGrid = getTargetGrid();
							if (isSmoothMode) {
								mcGrid.smoothTerrain(hit.point, brushSize, brushStrength);
							}
							else {
								mcGrid.deformTerrain(hit.point, brushSize, brushStrength, !shiftDown);
							}
						}
					}
				}
			}

		}


		private MarchingCubesGrid getTargetGrid() {
			return (MarchingCubesGrid)target;
		}

	}
}
