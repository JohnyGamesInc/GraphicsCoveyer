using UnityEngine;


namespace PerformanceProfile
{
    
    public class FlatFractal : MonoBehaviour
    {
        
        [SerializeField] private Mesh mesh; 
        [SerializeField] private Material material;
        
        [SerializeField, Range(1, 10)] private int depth = 4;
        [SerializeField, Range(1, 360)] private int rotationSpeed; 
        
        private const float PositionOffset = 0.75f; 
        private const float ScaleBias = 0.5f;
        private const int ChildCount = 5;
        
        private static readonly Vector3[] directions = new[]
        {
            Vector3.up, 
            Vector3.left, 
            Vector3.right, 
            Vector3.forward, 
            Vector3.back,
        };
        
        private static readonly Quaternion[] rotations = new[]
        {
            Quaternion.identity, 
            Quaternion.Euler(0f, 0f, 90f), 
            Quaternion.Euler(0f, 0f, -90f), 
            Quaternion.Euler(90f, 0f, 0f), 
            Quaternion.Euler(-90f, 0f, 0f),
        };
        
        private FractalPart[][] parts;

        
        private void OnEnable() 
        { 
            parts = new FractalPart[depth][];
            
            for (int i = 0, length = 1; i < parts.Length; i++, length *= ChildCount)
            {
                parts[i] = new FractalPart[length];
            } 
            
            var scale = 1f;
            parts[0][0] = CreatePart(0, 0, scale);
            
            for (var li = 1; li < parts.Length; li++) 
            { 
                scale *= ScaleBias; 
                var levelParts = parts[li]; 
                
                for (var fpi = 0; fpi < levelParts.Length; fpi += ChildCount) {
                    for (var ci = 0; ci < ChildCount; ci++)
                    {
                        levelParts[fpi + ci] = CreatePart(li, ci, scale);
                    } 
                } 
            } 
        }


        private void Update()
        {
            var deltaRotation = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f); 
            var rootPart = parts[0][0]; 
            rootPart.Rotation *= deltaRotation; 
            rootPart.Transform.localRotation = rootPart.Rotation; 
            parts[0][0] = rootPart;
            
            for (var li = 1; li < parts.Length; li++)
            {
                var parentParts = parts[li - 1]; 
                var levelParts = parts[li];

                for (var fpi = 0; fpi < levelParts.Length; fpi++)
                {
                    var parentTransform = parentParts[fpi / ChildCount].Transform; 
                    var part = levelParts[fpi]; part.Rotation *= deltaRotation; 
                    part.Transform.localRotation = parentTransform.localRotation * part.Rotation; 
                    part.Transform.localPosition = parentTransform.localPosition + parentTransform.localRotation * (PositionOffset * part.Transform.localScale.x * part.Direction); 
                    levelParts[fpi] = part;
                }
            }
        }

        
        private FractalPart CreatePart(int levelIndex, int childIndex, float scale) 
        {
            var go = new GameObject($"Fractal Path L{levelIndex} C{childIndex}"); 
            go.transform.SetParent(transform, false); 
            go.AddComponent<MeshFilter>().mesh = mesh; 
            go.AddComponent<MeshRenderer>().material = material; 
            
            return new FractalPart()
            {
                Direction = directions[childIndex], 
                Rotation = rotations[childIndex], 
                Transform = go.transform
            }; 
        }


        private struct FractalPart
        {
            public Vector3 Direction; 
            public Quaternion Rotation; 
            public Transform Transform;
        }
        
        
    }
}