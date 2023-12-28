using UnityEngine;
using UnityEngine.Rendering;


namespace GraphicPipeline.CustomRP
{
    
    public partial class CameraRenderer
    {
        
        partial void DrawUnsupportedShaders();

#if UNITY_EDITOR 
        
        private static readonly ShaderTagId[] _legacyShaderTagIds =
        {
            new ShaderTagId("Always"), 
            new ShaderTagId("ForwardBase"), 
            new ShaderTagId("PrepassBase"), 
            new ShaderTagId("Vertex"), 
            new ShaderTagId("VertexLMRGBM"), 
            new ShaderTagId("VertexLM")
        };
        
        
        private static Material _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));

        
        partial void DrawUnsupportedShaders()
        {
            var drawingSettings = new DrawingSettings(_legacyShaderTagIds[0], new SortingSettings(_camera))
            {
                overrideMaterial = _errorMaterial,
            };
            
            for (var i = 1; i < _legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
            }

            var filteringSettings = FilteringSettings.defaultValue;
            
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        }

#endif
    }
}