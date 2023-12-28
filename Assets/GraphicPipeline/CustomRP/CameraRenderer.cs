using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace GraphicPipeline.CustomRP
{
    
    public partial class CameraRenderer
    {

        private static readonly List<ShaderTagId> DrawingShaderTagIds = new List<ShaderTagId> { new ShaderTagId("SRPDefaultUnlit") };
        private readonly CommandBuffer _commandBuffer = new CommandBuffer { name = BufferName };
        private const string BufferName = "Camera Render";

        private ScriptableRenderContext _context;
        private Camera _camera;

        private CullingResults _cullingResults;
        // private ScriptableCullingParameters _cullingParameters;


        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _camera = camera;
            _context = context;
            
            if(!Cull(out var parameters)) return;

            Settings(parameters);
            DrawVisible();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();
        }


        private void DrawVisible()
        {
            var drawingSettings = CreateDrawingSettings(DrawingShaderTagIds, SortingCriteria.CommonOpaque, out var sortingSettings); 
            var filteringSettings = new FilteringSettings(RenderQueueRange.all); 
            
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
            
            _context.DrawSkybox(_camera);
            
            sortingSettings.criteria = SortingCriteria.CommonTransparent; 
            drawingSettings.sortingSettings = sortingSettings; 
            filteringSettings.renderQueueRange = RenderQueueRange.transparent; 
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        }


        private void Settings(ScriptableCullingParameters parameters)
        {
            _cullingResults = _context.Cull(ref parameters);
            _context.SetupCameraProperties(_camera);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);
            _commandBuffer.BeginSample(BufferName);
            ExecuteCommandBuffer();
        }


        private void ExecuteCommandBuffer()
        {
            _context.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();
        }


        private bool Cull(out ScriptableCullingParameters parameters)
        {
            return _camera.TryGetCullingParameters(out parameters);
        }


        private DrawingSettings CreateDrawingSettings(
            List<ShaderTagId> shaderTags, 
            SortingCriteria sortingCriteria,
            out SortingSettings sortingSettings)
        {
            sortingSettings = new SortingSettings(_camera) { criteria = sortingCriteria, };
            var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
            
            for (var i = 1; i < shaderTags.Count; i++)
            {
                drawingSettings.SetShaderPassName(i, shaderTags[i]);
            } 
            
            return drawingSettings;
        }


        private void Submit()
        {
            _commandBuffer.EndSample(BufferName);
            ExecuteCommandBuffer();
            _context.Submit();
        }
        
        
        private void DrawGizmos() 
        {
            if (!Handles.ShouldRenderGizmos()) return; 
            
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects); 
        }

        
    }
}