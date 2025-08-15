Shader "Unlit/Shader_Scale"
{
    Properties
    {
        _ColorA ("Color", Color) = (1,1,1,1)
        _Scale ("UV Scale", Float) = 1.0
        _Offset ("UV Offset", Float) = 0
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
            float _Scale;
            float _Offset;

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
                o.uv = (v.uv0 + _Offset) * _Scale; // longer or shorter하게 하는 것임. // frag에서 해도 되지만, vertex에서 해야 더 정확함.
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return float4(i.uv, 0, 1);
            }
            
            ENDCG
        }
    }
}

