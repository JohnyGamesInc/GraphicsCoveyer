using UnityEngine;


namespace PerformanceProfile
{
    
    public class Fractal : MonoBehaviour
    {

        [SerializeField, Range(1, 10)] private int depth = 4;
        [SerializeField, Range(1, 360)] private int rotationSpeed; 
        
        private const float PositionOffset = 0.75f; 
        private const float ScaleBias = 0.5f;

        
        private void Start()
        {
            name = "Fractal " + depth;
            
            if (depth <= 1) { return; } 
            
            var childA = CreateChild(Vector3.up, Quaternion.identity); 
            var childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f)); 
            var childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f)); 
            var childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f)); 
            var childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f)); 
            
            childA.transform.SetParent(transform, false); 
            childB.transform.SetParent(transform, false); 
            childC.transform.SetParent(transform, false); 
            childD.transform.SetParent(transform, false); 
            childE.transform.SetParent(transform, false);
        }


        private void Update()
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }


        private Fractal CreateChild(Vector3 direction, Quaternion rotation)
        {
            var child = Instantiate(this); 
            
            child.depth = depth - 1;
            child.transform.localPosition = PositionOffset * direction; 
            child.transform.localRotation = rotation; 
            child.transform.localScale = ScaleBias * Vector3.one; 
            
            return child;
        }
        

    }
}