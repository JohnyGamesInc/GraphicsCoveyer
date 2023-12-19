using UnityEngine;
using UnityEngine.Rendering;


namespace GraphicPipeline.CustomRP
{
    
    
    public class SpaceRunPipelineRenderer : RenderPipeline
    {

        private CameraRenderer _cameraRenderer = new();
        
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            CamerasRender(context, cameras);
        }


        private void CamerasRender(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (var camera in cameras)
            {
                _cameraRenderer.Render(context, camera);
            }
        }


    }
}