Shader "Unlit/Shader_Color"
{
    //Properties는 vert랑 frag 모두에서 접근 가능
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    } 


    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
       
            #include "UnityCG.cginc"

            float4 _Color;

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : TEXCOORD0;
            };

            struct Interpolators 
            {
                float4 vertex : SV_POSITION;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
