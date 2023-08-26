using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MarchingCubesGrid))]
public class MarchingCubesGridEditor : Editor
{

    private SerializedProperty mapChunksText;
    private SerializedProperty mapHeightText;
    private SerializedProperty amplidtudeText;
    private SerializedProperty xScalarText;
    private SerializedProperty yScalarText;
    //private SerializedProperty xOffsetText;
    //private SerializedProperty yOffsetText;
    private SerializedProperty isoLevelText;

    private bool isSculptMode = false;
    private float brushSize = 4.0f;
    private float brushstrength = 0.5f;
    private float brushHardness = 0.5f;


    void OnEnable()
    {
        SceneView.beforeSceneGui += onSceneViewBeforeSceneGui;

        this.mapChunksText = serializedObject.FindProperty("chunks");
        this.mapHeightText = serializedObject.FindProperty("mapHeight");
        this.amplidtudeText = serializedObject.FindProperty("amplitude");
        this.xScalarText = serializedObject.FindProperty("xScalar");
        this.yScalarText = serializedObject.FindProperty("yScalar");
        //this.xOffsetText = "TODO";
        //this.yOffsetText = "TODO";
        this.isoLevelText = serializedObject.FindProperty("isoLevel");
    }


    void OnDisable()
    {
        SceneView.beforeSceneGui -= onSceneViewBeforeSceneGui;

        leftMouseDown = false;
        shiftDown = false;
    }



    bool terrainSizeToggle = false;
    bool perlinSettingsToggle = false;
    bool fallOffSettingsToggle = false;

    public override void OnInspectorGUI()
    {
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
        // EditorGUILayout.TextArea("xOffset", "TODO");
        // EditorGUILayout.TextArea("yOffset", "TODO");
        EditorGUILayout.PropertyField(isoLevelText);
        EditorGUILayout.EndToggleGroup();

        fallOffSettingsToggle = EditorGUILayout.BeginToggleGroup("Fall Off Settings", fallOffSettingsToggle);
        EditorGUILayout.TextArea("TODO");
        EditorGUILayout.EndToggleGroup();

        isSculptMode = EditorGUILayout.BeginToggleGroup("Enable sculpt mode", isSculptMode);
        this.brushSize = EditorGUILayout.FloatField("Brush Size", this.brushSize);
        EditorGUILayout.FloatField("Brush Strength", this.brushstrength);
        EditorGUILayout.FloatField("Brush Hardness", this.brushHardness);
        EditorGUILayout.EndToggleGroup();


        MarchingCubesGrid mcGrid = getTargetGrid();


        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            //MarchingCubesGrid mcGrid = (MarchingCubesGrid)target;
            mcGrid.updateTerrainDataFromInspector();
            //TODO call method to update terrain.
        }

        if (GUILayout.Button("Generate mesh"))
        {
            mcGrid.createGrid();
        }

    }


    private bool leftMouseDown = false;
    private bool shiftDown = false;

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
                        MarchingCubesGrid mcGrid = getTargetGrid(); ;
                        mcGrid.deformTerrain(hit.point, brushSize, !shiftDown);
                    }
                }
            }
        }

    }


    private MarchingCubesGrid getTargetGrid()
    {
        return (MarchingCubesGrid)target;
    }

}
