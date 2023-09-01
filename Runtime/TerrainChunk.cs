using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace McTerrain
{

    [Serializable]
    public class TerrainChunk : MonoBehaviour, ISerializationCallbackReceiver
    {

        public static readonly int CHUNK_SIZE = 16;
        public static readonly int CHUNK_HEIGHT = 10;

        private VertexNode vertexNodePrefab;

        [SerializeField] private int xChunkOffset;
        [SerializeField] private int zchunkOffset;


        private MCTerrainData terrainData;

        private VertexNode[,,] vertexNodes;
        [SerializeField] private List<VertexNode> serializedVerticies = new List<VertexNode>();


        private Mesh mesh;
        [SerializeField] private List<Vector3> verticies = new List<Vector3>();
        [SerializeField] public List<int> triangles = new List<int>();



        // void Start() {

        // }

        // void Update() {

        // }


        public void init(int chunkIndexX, int chunkIndexZ, MCTerrainData terrainData, VertexNode vertexNodePrefab)
        {
            this.xChunkOffset = chunkIndexX * CHUNK_SIZE;
            this.zchunkOffset = chunkIndexZ * CHUNK_SIZE;
            this.terrainData = terrainData;
            this.vertexNodePrefab = vertexNodePrefab;

            createTerrain();
            drawMesh();
        }


        public void updateTerrainData()
        {
            createTerrain();
            drawMesh();
        }


        //TODO JFC fix this duplicate code
        public void makeBombHole(Vector3 location, float radius) {
             bool isTerrainDirty = false;
            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE + 1; z++)
                    {
                        VertexNode vNode = vertexNodes[x, y, z];
                        if ((vNode.getLocation() - location).magnitude > radius)
                        {
                            continue;
                        }
                         if (vNode.getLocation().y == 0)
                        {
                            continue;
                        }
                        if (vNode.getLocation().y >= CHUNK_HEIGHT)
                        {
                            continue;
                        }
                        isTerrainDirty = true;

                        vNode.setWeight(1.0f);
                    }
                }
            }
            if (isTerrainDirty)
            {
                drawMesh();
            }
        }


        public void deformTerrainSphere(Vector3 location, float radius, bool isSubtractTerrain)
        {

            bool isTerrainDirty = false;
            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE + 1; z++)
                    {
                        VertexNode vNode = vertexNodes[x, y, z];
                        if ((vNode.getLocation() - location).magnitude > radius)
                        {
                            continue;
                        }
                        // if(vNode.getIsOutside() == isSubtractTerrain) {
                        //     continue;
                        // }
                        if (vNode.getLocation().y == 0)
                        {
                            continue;
                        }
                        if (vNode.getLocation().y >= CHUNK_HEIGHT)
                        {
                            continue;
                        }


                        isTerrainDirty = true;
                        //vNode.setIsOutside(isSubtractTerrain);

                        if (isSubtractTerrain)
                        {
                            vNode.setWeight(vNode.getWeight() - 0.01f);
                        }
                        else
                        {
                            vNode.setWeight(vNode.getWeight() + 0.01f);
                        }

                    }
                }
            }

            if (isTerrainDirty)
            {
                drawMesh();
            }
        }


        private void createTerrain()
        {


            MeshFilter meshfilter = GetComponent<MeshFilter>();
            meshfilter.mesh = new Mesh();
            this.mesh = meshfilter.sharedMesh;

            this.vertexNodes = createVertexNodes();

            foreach (VertexNode node in vertexNodes)
            {
                float nodeWeight = noiseForLoc(node.getLocation());


                node.setWeight(nodeWeight);
                if (nodeWeight > terrainData.isoLevel)
                {
                    // node.setIsOutside(true);
                }
            }

            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                for (int z = 0; z < CHUNK_SIZE + 1; z++)
                {
                    //vertexNodes[x,0,z].setWeight(1);
                }
            }
        }


        private VertexNode[,,] createVertexNodes()
        {

            int hoizontalNodes = CHUNK_SIZE + 1;
            int veticalNodes = CHUNK_HEIGHT + 1;

            VertexNode[,,] verticies = new VertexNode[hoizontalNodes, veticalNodes, hoizontalNodes];

            for (int x = 0; x < hoizontalNodes; x++)
            {
                for (int z = 0; z < hoizontalNodes; z++)
                {
                    for (int y = 0; y < veticalNodes; y++)
                    {
                        VertexNode node = new VertexNode(new Vector3(x + xChunkOffset, y, z + zchunkOffset));
                        verticies[x, y, z] = node;
                    }
                }
            }

            return verticies;
        }



        public float noiseForLoc(Vector3 vec)
        {
            float returnVal = Mathf.PerlinNoise(
                 (vec.x / terrainData.mapSize) * terrainData.xAmpScale + terrainData.xOffset,
                 (vec.z / terrainData.mapSize) * terrainData.zAmpScale + terrainData.zOffset);

            returnVal += (vec.y / terrainData.mapHeight) * 1.0f;
            return returnVal * this.terrainData.amplitude;
        }


        private void drawMesh()
        {
            // Debug.Log("drawMesh called");
            verticies = new List<Vector3>();
            triangles = new List<int>();


            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        updateCubeTriangles(new Vector3Int(x, y, z));
                    }
                }
            }


            if (null == this.mesh)
            {
                this.mesh = GetComponent<MeshFilter>().sharedMesh;
            }
            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();
            //Debug.Log("verticies: " + verticies.Count + ", tris: " + triangles.Count);
            mesh.RecalculateNormals();
            mesh.Optimize();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
            Resources.UnloadUnusedAssets();
        }

        void updateCubeTriangles(Vector3Int cubeLoc)
        {

            if (cubeLoc == Vector3Int.zero)
            {

            }

            int cubeConfig = calcCubeConfig(cubeLoc);
            int[] edges = MarchingCubeConstants.triTable[cubeConfig];

            // if(cubeLoc == Vector3Int.zero) {
            //     Debug.Log("cubeConfig: " + cubeConfig);

            //     Debug.Log("iso: " + terrainData.isoLevel);
            // }

            //     foreach (int i in edges) {
            //         if(i != -1) {
            //         Debug.Log("edge: " + i);
            //         }
            //     }

            //Debug.Log("Weight " + vertexNodes[1,1,1].getWeight()) ;


            foreach (int edge in edges)
            {
                if (-1 == edge)
                {
                    break;
                }

                int indexA = MarchingCubeConstants.edgeConnections[edge][0];
                int indexB = MarchingCubeConstants.edgeConnections[edge][1];

                //Vector3 vertexPos = (MarchingCubeConstants.cubeCorners[indexA] + MarchingCubeConstants.cubeCorners[indexB]) * 0.5f;
                //vertexPos += cubeLoc;

                Vector3Int v1Loc = cubeLoc + Vector3Int.FloorToInt(MarchingCubeConstants.cubeCorners[indexA]);
                Vector3Int v2Loc = cubeLoc + Vector3Int.FloorToInt(MarchingCubeConstants.cubeCorners[indexB]);

                VertexNode v1 = vertexNodes[v1Loc.x, v1Loc.y, v1Loc.z];
                VertexNode v2 = vertexNodes[v2Loc.x, v2Loc.y, v2Loc.z];

                Vector3 vertexPos = Vector3.zero;
                float mu = (terrainData.isoLevel - v1.getWeight()) / (v2.getWeight() - v1.getWeight());
                vertexPos.x = v1Loc.x + mu * (v2Loc.x - v1Loc.x);
                vertexPos.y = v1Loc.y + mu * (v2Loc.y - v1Loc.y);
                vertexPos.z = v1Loc.z + mu * (v2Loc.z - v1Loc.z);

                verticies.Add(vertexPos + new Vector3(xChunkOffset, 0, zchunkOffset));
                triangles.Add(triangles.Count);
            }
        }


        private int calcCubeConfig(Vector3Int cube)
        {

            VertexNode[] cubeVerts = new VertexNode[8];

            // Debug.Log("AA" + this.vertexNodes.Length);

            cubeVerts[0] = vertexNodes[cube.x, cube.y, cube.z + 1];
            cubeVerts[1] = vertexNodes[cube.x + 1, cube.y, cube.z + 1];
            cubeVerts[2] = vertexNodes[cube.x + 1, cube.y, cube.z];
            cubeVerts[3] = vertexNodes[cube.x, cube.y, cube.z];

            cubeVerts[4] = vertexNodes[cube.x, cube.y + 1, cube.z + 1];
            cubeVerts[5] = vertexNodes[cube.x + 1, cube.y + 1, cube.z + 1];
            cubeVerts[6] = vertexNodes[cube.x + 1, cube.y + 1, cube.z];
            cubeVerts[7] = vertexNodes[cube.x, cube.y + 1, cube.z];

            int cubeIndex = 0;

            if (null == this.terrainData)
            {
                Debug.Log("Terrain data is null");
            }

            for (int i = 0; i < 8; i++)
            {
                if (cubeVerts[i].getWeight() > this.terrainData.isoLevel)
                {
                    cubeIndex |= 1 << i;
                }
            }

            return cubeIndex;
        }




        public void OnAfterDeserialize()
        {
            // Debug.Log("DE SErialize");
            int hoizontalNodes = CHUNK_SIZE + 1;
            int veticalNodes = CHUNK_HEIGHT + 1;


            vertexNodes = new VertexNode[hoizontalNodes, veticalNodes, hoizontalNodes];

            // if(serializedVerticies == null) {
            //     this.serializedVerticies = new VertexNode[0];
            //     return;
            // }
            //Debug.Log("serializedVerticies count:" + serializedVerticies.Count );



            int count = 0;

            for (int x = 0; x < hoizontalNodes; x++)
            {
                for (int y = 0; y < veticalNodes; y++)
                {
                    for (int z = 0; z < hoizontalNodes; z++)
                    {
                        vertexNodes[x, y, z] = serializedVerticies[count];
                        count++;
                    }
                }
            }
        }


        public void OnBeforeSerialize()
        {
            // Debug.Log("OnSerialize");
            int hoizontalNodes = CHUNK_SIZE + 1;
            int veticalNodes = CHUNK_HEIGHT + 1;
            serializedVerticies.Clear();

            for (int x = 0; x < hoizontalNodes; x++)
            {
                for (int y = 0; y < veticalNodes; y++)
                {
                    for (int z = 0; z < hoizontalNodes; z++)
                    {
                        serializedVerticies.Add(vertexNodes[x, y, z]);
                    }
                }
            }

            //Debug.Log("aa serializedVerticies count:" + serializedVerticies.Count );

        }

        public void setTerrainData(MCTerrainData terrainData)
        {
            this.terrainData = terrainData;
        }

    }
}
