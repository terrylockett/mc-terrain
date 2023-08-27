using UnityEngine;

namespace McTerrain
{

    [System.Serializable]
    public class MCTerrainData
    {
        
        [SerializeField] public float amplitude;
        [SerializeField] public int mapSize;
        [SerializeField] public int mapHeight;
        [SerializeField] public float xAmpScale;
        [SerializeField] public float zAmpScale;
        [SerializeField] public float xOffset;
        [SerializeField] public float zOffset;
        [SerializeField] public float isoLevel;

        public MCTerrainData(float amplitude, int mapSize, int mapHeight, float xAmpScale, float zAmpScale, float xOffset, float zOffset, float isoLevel)
        {
            this.amplitude = amplitude;
            this.mapSize = mapSize;
            this.mapHeight = mapHeight;
            this.xAmpScale = xAmpScale;
            this.zAmpScale = zAmpScale;
            this.xOffset = xOffset;
            this.zOffset = zOffset;
            this.isoLevel = isoLevel;
        }

        public float getAmplitude()
        {
            return this.amplitude;
        }
        public void setAmplitude(float amplitude)
        {
            this.amplitude = amplitude;
        }

        public int getMapSize()
        {
            return this.mapSize;
        }
        public void setMapSize(int mapSize)
        {
            this.mapSize = mapSize;
        }

        public int getMapHeight()
        {
            return this.mapHeight;
        }
        public void setMapHeight(int mapHeight)
        {
            this.mapHeight = mapHeight;
        }

        public float getXAmplitude()
        {
            return this.xAmpScale;
        }
        public void setXAmplitude(float xAmplitude)
        {
            this.xAmpScale = xAmplitude;
        }

        public float getZAmplitude()
        {
            return this.zAmpScale;
        }
        public void setZAmplitude(float zAmplitude)
        {
            this.zAmpScale = zAmplitude;
        }

        public float getIsoLevel()
        {
            return this.isoLevel;
        }
        public void setIsoLevel(float isoLevel)
        {
            this.isoLevel = isoLevel;
        }

    }
}