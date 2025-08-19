Shader "Unlit/Shader_Dissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaterialA ("Material A", 2D) = "white" {}
        _MaterialB ("Material B", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveEdge ("Dissolve Edge", Range(0, 0.1)) = 0.02
        _DissolveColor ("Dissolve Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
       
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaterialA;
            sampler2D _MaterialB;
            float _DissolveAmount;
            float _DissolveEdge;
            float4 _DissolveColor;

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
                // 두 머티리얼 샘플링
                float4 materialA = tex2D(_MaterialA, i.uv);
                float4 materialB = tex2D(_MaterialB, i.uv);
                
                // 노이즈 생성 (dissolve 패턴용)
                float noise = frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453);
                
                // dissolve 마스크 계산
                float dissolveMask = step(noise, _DissolveAmount);
                
                // dissolve 엣지 효과
                float edge = smoothstep(_DissolveAmount - _DissolveEdge, _DissolveAmount, noise);
                float4 edgeColor = _DissolveColor * edge;
                
                // 머티리얼 블렌딩
                float4 finalColor = lerp(materialA, materialB, dissolveMask);
                
                // 엣지 색상 추가
                finalColor = lerp(finalColor, edgeColor, edge);
                
                return finalColor;
            }
            
            ENDCG
        }
    }
}
