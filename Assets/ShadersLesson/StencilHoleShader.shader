Shader "Custom/StencilHoleShader"
{
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" //отрисовку объекта надо делать в режиме прозрачности, так как этот шейдер нужен для полностью прозрачного материала
            "Queue" = "Geometry-1" // укажет порядок отрисовки, вданномслучаенепосредственно перед отрисовкой игровой геометрии. Это позволит записать необходимое значение в StencilBufferперед отрисовкой остального игровогомира.
            "ForceNoShadowCasting"="True" //укажет,чтообъектнедолженотбрасыватьтени.Даже прозрачные объекты в Unityмогут отбрасывать тени
        }
        
        LOD 200
        
        Stencil
        {
            Ref 10
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        #pragma surface surf NoLighting alpha
        
        
        struct Input
        {
            float2 uv_MainTex;
        };

        
        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            
        }
        
        ENDCG
    }
    
    FallBack "Diffuse"
}
