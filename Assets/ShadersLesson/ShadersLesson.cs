using System;
using UnityEngine;


namespace ShadersLesson
{
    
    public class ShadersLesson : MonoBehaviour
    {

        private Material material;

        
        private void Start()
        {
            material.SetColor("_Color", Color.blue);
            float mixVal = material.GetFloat("_MixValue");
        }
        
        
        
        
        
    }
}