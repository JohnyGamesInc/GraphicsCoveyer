Shader "Custom/UnlitTextureMix"
{
    Properties
    {
        _Tex1 ("Texture1", 2D) = "white" {}
        _Tex2 ("Texture2", 2D) = "black" {}
        _MixValue("MixValue", Range(0, 1)) = 0.5
        _Color("Main Color", COLOR) = (1, 1, 1, 1)
        _HeightBend("HeightBend", Range(0, 20)) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Tex1;
            float4 _Tex1_ST;
            sampler2D _Tex2;
            float4 _Tex2_ST;
            float _MixValue;
            float4 _Color;

            float _HeightBend;

            // структура, которая помогает преобразовать данные вершины в данные фрагмента
            struct v2f
            {
                float2 uv : TEXCOORD0; // UV coordinates of vertex
                float4 vertex : SV_POSITION; // coordinates of vertex1
            };

            //здесь происходит обработка вершин
            v2f vert (appdata_full v)
            {
                v2f result;
                v.vertex.xyz += v.normal * _HeightBend;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Tex1);
                return result;
            }

            fixed4 frag(v2f f) : SV_Target
            {
                fixed4 color; // финальным цветом данного фрагмента
                color = tex2D(_Tex1, f.uv) * _MixValue; // получить цвет выбранной точки на текстуре
                color += tex2D(_Tex2, f.uv) * (1 - _MixValue);
                color = color * _Color; // дополнительно окрасить изображение
                return color;
            }
            
            
            ENDCG
        }
    }
}