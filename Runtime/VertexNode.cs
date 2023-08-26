using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace McTerrain
{
    [Serializable]
    public class VertexNode
    {

        [SerializeField] private bool IsOutside = false;
        [SerializeField] private Vector3 location;
        [SerializeField] private float weight = 0f;


        public VertexNode(Vector3 location)
        {
            this.location = location;
        }


        //fuck c# properties. I like boilerplate code.
        public void setIsOutside(bool IsOutside)
        {
            this.IsOutside = IsOutside;
        }

        public bool getIsOutside()
        {
            return this.IsOutside;
        }

        public void setLocation(Vector3 location)
        {
            this.location = location;
        }

        public Vector3 getLocation()
        {
            return this.location;
        }

        public void setWeight(float weight)
        {
            this.weight = weight;
        }

        public float getWeight()
        {
            return this.weight;
        }

    }
}