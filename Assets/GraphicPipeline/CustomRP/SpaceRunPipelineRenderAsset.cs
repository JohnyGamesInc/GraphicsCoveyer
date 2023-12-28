using UnityEngine;
using UnityEngine.Rendering;


namespace GraphicPipeline.CustomRP
{
    
    [CreateAssetMenu(fileName = "SpaceRunPipelineRenderAsset", menuName = "Rendering/SpaceRunPipelineRenderAsset")]
    public class SpaceRunPipelineRenderAsset : RenderPipelineAsset
    {
        
        protected override RenderPipeline CreatePipeline()
        {
            return new SpaceRunPipelineRenderer();
        }
        
        
    }
}