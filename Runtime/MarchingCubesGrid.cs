using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubesGrid : MonoBehaviour {
    [SerializeField] private VertexNode VertexNodePrefab;
    [SerializeField] private int mapSize;
    [SerializeField] private int chunks = 4;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] float amplitude = 1f;
    [SerializeField] private float xScalar = 1f;
    [SerializeField] float yScalar = 1f;
    [SerializeField] float isoLevel = 0.6f;

    [SerializeField] private MCTerrainData terrainData;
    [SerializeField] private List<TerrainChunk> terrainChunks;


    private string mapSizeText;
    private string mapHeightText;
    private string amplidtudeText;
    private string xScalarText;
    private string yScalarText;
    private string xOffsetText;
    private string yOffsetText;
    private string groundHeightText;


    //TODO tmp ball for deforming terrain
    public GameObject ball;


    private GameObject terrainChunksEmptyObject;


    void Start() {

        //tmp
        ball = GameObject.Find("BrushSphere");

        //reset refs to Terrain Chunks
        this.terrainChunks.Clear();
        Transform chunksParent = getTerrainChunksParent();
        foreach (TerrainChunk tChunk in chunksParent.GetComponentsInChildren<TerrainChunk>()) {
            Debug.Log("Adding tChunks");
            this.terrainChunks.Add(tChunk);
            tChunk.setTerrainData(this.terrainData);
        }


        // this.mapSize = this.chunks * TerrainChunk.CHUNK_SIZE;
        // this.terrainChunks = new List<TerrainChunk>();

        // this.terrainData = new MCTerrainData(amplitude, mapSize, mapHeight, xScalar, yScalar, isoLevel);

        
        // for(int x=0; x< chunks; x++) {
        //     for(int z=0; z< chunks; z++) {
        //         TerrainChunk tc = makeChunk(x,z);
        //         this.terrainChunks.Add(tc);
        //     }
        // }

        //initGuiFields();
    }
    

    void Update() {

        //TODO handle this better lol
        foreach (TerrainChunk tChunk in terrainChunks) {
            tChunk.deformTerrainSphere(ball.transform.position, 4, true);
        }

    }



    public void createGrid() {

        // foreach (TerrainChunk tChunk in terrainChunks) {
        //     Destroy(tChunk.gameObject);
        // }

        if(this.terrainData == null) {
            this.terrainData = new MCTerrainData(amplitude, mapSize, mapHeight, xScalar, yScalar, isoLevel);
        }

        updateTerrainDataFromInspector();

        this.mapSize = this.chunks * TerrainChunk.CHUNK_SIZE;
        this.terrainChunks = new List<TerrainChunk>();

        //create terain data
        if(null == this.terrainData) {
            this.terrainData = new MCTerrainData(amplitude, mapSize, mapHeight, xScalar, yScalar, isoLevel);
        }

        for(int x=0; x< chunks; x++) {
            for(int z=0; z< chunks; z++) {
                TerrainChunk tc = makeChunk(x,z);
                this.terrainChunks.Add(tc);
            }
        }
    }


    // private void synchTerrainDataToChunks() {
    //     if(this.terrainChunks != null) {
    //         foreach(TerrainChunk tc in this.terrainChunks) {
    //             tc.setTerrainDate(this.terrainData);
    //         }
    //     }
    // }


    public void deformTerrain(Vector3 point, float radius, bool isSubtractTerrain) {
         //TODO calc which chunks this is in and only update those.

       // synchTerrainDataToChunks();

         foreach (TerrainChunk tChunk in terrainChunks) {
            tChunk.setTerrainData(this.terrainData);
            tChunk.deformTerrainSphere(point, radius, isSubtractTerrain);
        }
    }


    public void updateTerrainDataFromInspector() {
        this.mapSize = this.chunks * TerrainChunk.CHUNK_SIZE;

        this.terrainData.amplitude = this.amplitude;
        this.terrainData.mapSize = this.mapSize;
        this.terrainData.mapHeight = this.mapHeight;
        this.terrainData.xAmpScale = this.xScalar;
        this.terrainData.zAmpScale = this.yScalar;
        this.terrainData.isoLevel = this.isoLevel;

    }




    void OnGUI() {
        GUI.changed = false;
        GUI.Box(new Rect(10, 10, 140, 260), "Terrain Attributes");

        GUI.Label(new Rect(25, 31, 100, 30), "MapSize:");
        mapSizeText = GUI.TextField(new Rect(100, 30, 30, 25), mapSizeText);

        GUI.Label(new Rect(25, 61, 100, 30), "MapHeight:");
        mapHeightText = GUI.TextField(new Rect(100, 61, 30, 25), mapHeightText);

        GUI.Label(new Rect(25, 91, 100, 30), "Amplitude:");
        amplidtudeText = GUI.TextField(new Rect(100, 91, 30, 25), amplidtudeText);

        GUI.Label(new Rect(25, 121, 100, 30), "xScalar:");
        xScalarText = GUI.TextField(new Rect(100, 121, 30, 25), xScalarText);

        GUI.Label(new Rect(25, 151, 100, 30), "yScalar:");
        yScalarText = GUI.TextField(new Rect(100, 151, 30, 25), yScalarText);

        GUI.Label(new Rect(25, 181, 100, 30), "xOffset:");
        xOffsetText = GUI.TextField(new Rect(100, 181, 45, 25), xOffsetText);

        GUI.Label(new Rect(25, 211, 100, 30), "yOffset:");
        yOffsetText = GUI.TextField(new Rect(100, 211, 45, 25), yOffsetText);

        GUI.Label(new Rect(25, 241, 100, 30), "GroundHeight:");
        groundHeightText = GUI.TextField(new Rect(115, 241, 30, 25), groundHeightText);



        if (GUI.Button (new Rect (120, 25, 100, 30), "remake terrain"))  {
            //TODO
        }
        else if (GUI.changed) {
           // UIValuesToAttributes();
            //TODO update chunks
        }
    }


    private TerrainChunk makeChunk(int x, int y) {
        //TODO fix this. it should be a prefab instead of a bunch of add components.
        GameObject chunk2 = new GameObject("TerrainChunk");
        chunk2.transform.parent = getTerrainChunksParent();
        chunk2.AddComponent<TerrainChunk>();
        chunk2.AddComponent<MeshFilter>();
        chunk2.AddComponent<MeshRenderer>();

        //TODO expose a proper mat setter instead of this peice o poo
        chunk2.GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().sharedMaterials[0];
        chunk2.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        chunk2.AddComponent<MeshCollider>();
        TerrainChunk tc2 = chunk2.GetComponent<TerrainChunk>();
        tc2.init(x,y, terrainData, VertexNodePrefab);
        return tc2;

    }


    void OnDrawGizmosSelected() {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Vector3 gizmoPos = transform.position;
        gizmoPos.x += (TerrainChunk.CHUNK_SIZE * chunks)/2;
        gizmoPos.z += (TerrainChunk.CHUNK_SIZE * chunks)/2;
        gizmoPos.y += (TerrainChunk.CHUNK_HEIGHT)/2;

        Gizmos.DrawWireCube(gizmoPos, new Vector3(chunks*TerrainChunk.CHUNK_SIZE, TerrainChunk.CHUNK_HEIGHT, chunks*TerrainChunk.CHUNK_SIZE));
    }


    private Transform getTerrainChunksParent() {
        if(this.terrainChunksEmptyObject == null) {
            this.terrainChunksEmptyObject = this.transform.Find("TerrainChunks").gameObject;
        }

        return this.terrainChunksEmptyObject.transform;
    }
}
