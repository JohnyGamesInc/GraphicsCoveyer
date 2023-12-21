using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math; 
using float4x4 = Unity.Mathematics.float4x4; 
using quaternion = Unity.Mathematics.quaternion;
using UnityEngine;


namespace PerformanceProfile
{
    
    public class MathJobsProceduralFractal : MonoBehaviour
    {
        
        [SerializeField] private Mesh mesh; 
        [SerializeField] private Material material;
        
        [SerializeField, Range(1, 10)] private int depth = 4;
        [SerializeField, Range(0, 1)] private float rotationSpeed = 0.125f; 
        
        private const float PositionOffset = 1.5f; 
        private const float ScaleBias = 0.5f;
        private const int ChildCount = 5;
        
        private NativeArray<FractalPart>[] parts;
        private NativeArray<Matrix4x4>[] matrices;
        private ComputeBuffer[] matricesBuffers;
        
        private static readonly int matricesId = Shader.PropertyToID("_Matrices"); 
        private static MaterialPropertyBlock propertyBlock;
        
        private static readonly float3[] directions = {
            up(),
            left(),
            right(),
            forward(),
            back()
        };
        
        private static readonly quaternion[] rotations = {
            quaternion.identity, 
            quaternion.RotateZ(0.5f * PI), 
            quaternion.RotateZ(-0.5f * PI), 
            quaternion.RotateX(0.5f * PI),
            quaternion.RotateX(-.5f * PI),
        };
        
        
        private void OnEnable() 
        { 
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<Matrix4x4>[depth];
            matricesBuffers = new ComputeBuffer[depth]; 
            var stride = 16 * 4;
            
            for (int i = 0, length = 1; i < parts.Length; i++, length *= ChildCount)
            {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
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
                parts[i].Dispose();
                matrices[i].Dispose();
            }
        }


        private void OnValidate()
        {
            if (parts is null || !enabled) { return; }
            
            OnDisable(); 
            OnEnable();
        }


        private void Update()
        {
            var spinAngelDelta = rotationSpeed * PI * Time.deltaTime;
            var rootPart = parts[0][0];
            
            rootPart.SpinAngle += spinAngelDelta;
            
            var deltaRotation = Quaternion.Euler(.0f, rootPart.SpinAngle, .0f);
            rootPart.WorldRotation = rootPart.Rotation * deltaRotation; 
            
            parts[0][0] = rootPart; 
            matrices[0][0] = Matrix4x4.TRS(rootPart.WorldPosition, rootPart.WorldRotation, Vector3.one); 
            
            var scale = 1.0f;

            JobHandle jobHandle = default;
            
            for (var li = 1; li < parts.Length; li++) 
            {
                scale *= ScaleBias; 
                
                jobHandle = new UpdateFractalLevelJob
                {
                    SpinAngleDelta = spinAngelDelta, 
                    Scale = scale, 
                    Parents = parts[li - 1], 
                    Parts = parts[li], 
                    Matrices = matrices[li]
                }.Schedule(parts[li].Length, jobHandle);
            }
            
            jobHandle.Complete();

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
        
        
        [BurstCompile(CompileSynchronously = true, FloatPrecision = FloatPrecision.Standard, FloatMode = FloatMode.Fast)]
        private struct UpdateFractalLevelJob : IJobFor 
        { 
            public float SpinAngleDelta; 
            public float Scale; 
            
            [ReadOnly] public NativeArray<FractalPart> Parents;
            public NativeArray<FractalPart> Parts; 
            
            [WriteOnly] public NativeArray<Matrix4x4> Matrices;


            public void Execute(int index)
            {
                var parent = Parents[index / ChildCount];
                var part = Parts[index];
                
                part.SpinAngle += SpinAngleDelta;
                part.WorldRotation = mul(
                    parent.WorldRotation, 
                    mul(
                        part.Rotation, 
                        quaternion.RotateY(part.SpinAngle))); 
                
                part.WorldPosition = parent.WorldPosition + Vector3.one *
                                     mul(parent.WorldPosition, 
                                         PositionOffset * Scale * part.Direction);
                
                Parts[index] = part; 
                Matrices[index] = float4x4.TRS(part.WorldPosition, part.WorldRotation, float3(Scale));
            }
        }

        
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