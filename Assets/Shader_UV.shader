Shader "Unlit/Shader_UV"
{
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
                float3 normals : NORMAL;
                float4 uv0 : TEXCOORD0;
            };

            struct Interpolators 
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = v.uv0;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                //return float4(i.uv, 0, 1);// 위치 좌표를 색상과 매핑 ( red: x, green: y, blue: z )
                return float4(i.uv.xxx, 1);// 흑백 그라디언트
            }
            
            ENDCG
        }
    }
}

