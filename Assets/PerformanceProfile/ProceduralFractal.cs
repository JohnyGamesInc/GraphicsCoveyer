using UnityEngine;


namespace PerformanceProfile
{
    
    public class ProceduralFractal : MonoBehaviour
    {
        
        [SerializeField] private Mesh mesh; 
        [SerializeField] private Material material;
        
        [SerializeField, Range(1, 10)] private int depth = 4;
        [SerializeField, Range(1, 360)] private int rotationSpeed = 100; 
        
        private const float PositionOffset = 1.5f; 
        private const float ScaleBias = 0.5f;
        private const int ChildCount = 5;
        
        private FractalPart[][] parts;
        private Matrix4x4[][] matrices;
        private ComputeBuffer[] matricesBuffers;
        
        private static readonly int matricesId = Shader.PropertyToID("_Matrices"); 
        private static MaterialPropertyBlock propertyBlock;
        
        private static readonly Vector3[] directions = {
            Vector3.up, 
            Vector3.left, 
            Vector3.right, 
            Vector3.forward, 
            Vector3.back,
        };
        
        private static readonly Quaternion[] rotations = {
            Quaternion.identity, 
            Quaternion.Euler(0f, 0f, 90f), 
            Quaternion.Euler(0f, 0f, -90f), 
            Quaternion.Euler(90f, 0f, 0f), 
            Quaternion.Euler(-90f, 0f, 0f),
        };
        
        
        private void OnEnable() 
        { 
            parts = new FractalPart[depth][];
            matrices = new Matrix4x4[depth][];
            matricesBuffers = new ComputeBuffer[depth]; 
            var stride = 16 * 4;
            
            for (int i = 0, length = 1; i < parts.Length; i++, length *= ChildCount)
            {
                parts[i] = new FractalPart[length];
                matrices[i] = new Matrix4x4[length];
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            } 
            
            parts[0][0] = CreatePart(0);

            for (var li = 1; li < parts.Length; li++)
            {
                var levelParts = parts[li];

                for (var fpi = 0; fpi < levelParts.Length; fpi += ChildCount)
                {
                    for (var ci = 0; ci < ChildCount; ci++)
                    {
                        levelParts[fpi + ci] = CreatePart(ci);
                    }
                }
            }
            
            propertyBlock ??= new MaterialPropertyBlock();
        }


        private void OnDisable()
        {
            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                matricesBuffers[i].Release();
            } 
            
            parts = null; 
            matrices = null; 
            matricesBuffers = null;
        }


        private void OnValidate()
        {
            if (parts is null || !enabled) { return; }
            
            OnDisable(); 
            OnEnable();
        }


        private void Update()
        {
            var spinAngelDelta = rotationSpeed * Time.deltaTime; 
            var rootPart = parts[0][0]; 
            
            rootPart.SpinAngle += spinAngelDelta; 
            
            var deltaRotation = Quaternion.Euler(.0f, rootPart.SpinAngle, .0f); 
            rootPart.WorldRotation = rootPart.Rotation * deltaRotation; 
            
            parts[0][0] = rootPart; 
            matrices[0][0] = Matrix4x4.TRS(rootPart.WorldPosition, rootPart.WorldRotation, Vector3.one); 
            
            var scale = 1.0f;
            
            for (var li = 1; li < parts.Length; li++) 
            {
                scale *= ScaleBias; 
                var parentParts = parts[li - 1]; 
                var levelParts = parts[li]; 
                var levelMatrices = matrices[li];

                for (var fpi = 0; fpi < levelParts.Length; fpi++)
                {
                    var parent = parentParts[fpi / ChildCount]; 
                    var part = levelParts[fpi]; 
                    part.SpinAngle += spinAngelDelta; 
                    deltaRotation = Quaternion.Euler(.0f, part.SpinAngle, .0f);
                    part.WorldRotation = parent.WorldRotation * part.Rotation * deltaRotation; 
                    part.WorldPosition = parent.WorldPosition + parent.WorldRotation * (PositionOffset * scale * part.Direction); 
                    levelParts[fpi] = part; 
                    levelMatrices[fpi] = Matrix4x4.TRS(part.WorldPosition, part.WorldRotation, scale * Vector3.one);
                }
            } 
            
            var bounds = new Bounds(rootPart.WorldPosition, 3f * Vector3.one);
            
            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                var buffer = matricesBuffers[i]; 
                buffer.SetData(matrices[i]); 
                propertyBlock.SetBuffer(matricesId, buffer); 
                material.SetBuffer(matricesId, buffer); 
                Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
            }
        }


        private FractalPart CreatePart(int childIndex) => new FractalPart
        {
            Direction = directions[childIndex], 
            Rotation = rotations[childIndex],
        };


        private struct FractalPart
        {
            public Vector3 Direction; 
            public Quaternion Rotation;
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;
            public float SpinAngle;
        }
        
        
    }
}