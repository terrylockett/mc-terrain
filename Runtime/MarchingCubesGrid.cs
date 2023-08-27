using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace McTerrain
{
    public class MarchingCubesGrid : MonoBehaviour
    {
        [SerializeField] private VertexNode VertexNodePrefab;
        [SerializeField] private int mapSize;
        [SerializeField] private int chunks = 4;
        [SerializeField] private int mapHeight = 10;
        [SerializeField] float amplitude = 1f;
        [SerializeField] private float xScalar = 1f;
        [SerializeField] float yScalar = 1f;

        [SerializeField] float xOffset = 0f;
        [SerializeField] float zOffset = 0f;

        [SerializeField] float isoLevel = 0.6f;

        [SerializeField] private MCTerrainData terrainData;
        [SerializeField] private List<TerrainChunk> terrainChunks;


        //TODO tmp ball for deforming terrain
        public GameObject ball;


        private GameObject terrainChunksEmptyObject;


        void Start()
        {
            //tmp
            ball = GameObject.Find("BrushSphere");

            //reset refs to Terrain Chunks
            getTerrainChunks();
        }


        void Update()
        {
            //TODO handle this better lol
            // foreach (TerrainChunk tChunk in getTerrainChunks())
            // {
            //     tChunk.deformTerrainSphere(ball.transform.position, 4, false);
            // }

        }


        public void editModeDestoryChunks() {
            foreach (TerrainChunk tChunk in terrainChunks) {
                if(null != tChunk) {
                    DestroyImmediate(tChunk.gameObject);
                }
            }
            this.terrainData = null;
        }


        public void createGrid()
        {

            if (this.terrainData == null)
            {
                this.terrainData = new MCTerrainData(amplitude, mapSize, mapHeight, xScalar, yScalar, xOffset, zOffset, isoLevel);
            }

            updateTerrainDataFromInspector();

            this.mapSize = this.chunks * TerrainChunk.CHUNK_SIZE;
            this.terrainChunks = new List<TerrainChunk>();

            //create terain data
            if (null == this.terrainData)
            {
                this.terrainData = new MCTerrainData(amplitude, mapSize, mapHeight, xScalar, yScalar, xOffset, zOffset, isoLevel);
            }

            for (int x = 0; x < chunks; x++)
            {
                for (int z = 0; z < chunks; z++)
                {
                    TerrainChunk tc = makeChunk(x, z);
                    this.terrainChunks.Add(tc);
                }
            }
        }





        public void deformTerrain(Vector3 point, float radius, bool isSubtractTerrain)
        {
            //TODO calc which chunks this is in and only update those.

            // synchTerrainDataToChunks();


            foreach (TerrainChunk tChunk in getTerrainChunks())
            {
                tChunk.setTerrainData(this.terrainData);
                tChunk.deformTerrainSphere(point, radius, isSubtractTerrain);
            }
        }


        public void updateTerrainDataFromInspector()
        {
            this.mapSize = this.chunks * TerrainChunk.CHUNK_SIZE;

            this.terrainData.amplitude = this.amplitude;
            this.terrainData.mapSize = this.mapSize;
            this.terrainData.mapHeight = this.mapHeight;
            this.terrainData.xAmpScale = this.xScalar;
            this.terrainData.zAmpScale = this.yScalar;
            this.terrainData.xOffset = this.xOffset;
            this.terrainData.zOffset = this.zOffset;
            this.terrainData.isoLevel = this.isoLevel;

        }




        private TerrainChunk makeChunk(int x, int y)
        {
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
            tc2.init(x, y, terrainData, VertexNodePrefab);
            return tc2;

        }


        void OnDrawGizmosSelected()
        {
            // Draw a yellow cube at the transform position
            Gizmos.color = Color.yellow;
            Vector3 gizmoPos = transform.position;
            gizmoPos.x += (TerrainChunk.CHUNK_SIZE * chunks) / 2;
            gizmoPos.z += (TerrainChunk.CHUNK_SIZE * chunks) / 2;
            gizmoPos.y += (TerrainChunk.CHUNK_HEIGHT) / 2;

            Gizmos.DrawWireCube(gizmoPos, new Vector3(chunks * TerrainChunk.CHUNK_SIZE, TerrainChunk.CHUNK_HEIGHT, chunks * TerrainChunk.CHUNK_SIZE));
        }



        /**
         * the refs to the TerrainChunk objects gets lost when coming back from play mode.
         * As a terrible hack use this method when refrencing this.terrainChunks;
         *
        */
        private List<TerrainChunk> getTerrainChunks()
        {

            if (null != this.terrainChunks && this.terrainChunks.Count > 0)
            {
                if (null != this.terrainChunks[0])
                {
                    return this.terrainChunks;
                }
            }

            this.terrainChunks.Clear();
            Transform chunksParent = getTerrainChunksParent();
            foreach (TerrainChunk tChunk in chunksParent.GetComponentsInChildren<TerrainChunk>())
            {
                Debug.Log("Adding tChunks");
                this.terrainChunks.Add(tChunk);
                tChunk.setTerrainData(this.terrainData);
            }

            return this.terrainChunks;
        }


        private Transform getTerrainChunksParent()
        {
            if (this.terrainChunksEmptyObject == null)
            {
                this.terrainChunksEmptyObject = this.transform.Find("TerrainChunks").gameObject;
            }

            return this.terrainChunksEmptyObject.transform;
        }
    }
}
