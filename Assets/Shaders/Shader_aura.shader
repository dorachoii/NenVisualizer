Shader "Unlit/Shader_aura"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AuraColor ("Aura Color", Color) = (0.5, 0.8, 1.0, 1.0)
        _AuraRadius ("Aura Radius", Range(0.001, 0.1)) = 0.02
        _AuraSpeed ("Aura Speed", Range(0.1, 5.0)) = 1.0
        _AuraIntensity ("Aura Intensity", Range(0.1, 2.0)) = 1.0
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0.1, 5.0)) = 1.0
        _AuraThickness ("Aura Thickness", Range(0.001, 0.05)) = 0.02
        _EmissionIntensity ("Emission Intensity", Range(0.0, 5.0)) = 1.0
        _EmissionColor ("Emission Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SteamSpeed ("Steam Speed", Range(0.1, 3.0)) = 1.0
        _SteamScale ("Steam Scale", Range(0.5, 10.0)) = 2.0
        _SteamIntensity ("Steam Intensity", Range(0.0, 1.0)) = 0.3
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
       
            #include "UnityCG.cginc"
            #define TAU 6.28318530718

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AuraColor;
            float _AuraRadius;
            float _AuraSpeed;
            float _AuraIntensity;
            float _RimPower;
            float _RimIntensity;
            float _AuraThickness;
            float _EmissionIntensity;
            float4 _EmissionColor;
            float _SteamSpeed;
            float _SteamScale;
            float _SteamIntensity;

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
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                
                // 오오라 효과를 위한 vertex displacement
                float3 worldNormal = UnityObjectToWorldNormal(v.normals);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                // 수증기 같은 노이즈 효과 (더 큰 입자)
                float steamNoise = sin(_Time.y * _SteamSpeed + worldPos.x * _SteamScale + worldPos.y * _SteamScale) * 0.5 + 0.5;
                steamNoise += sin(_Time.y * _SteamSpeed * 0.7 + worldPos.x * _SteamScale * 1.3 + worldPos.z * _SteamScale * 0.8) * 0.3 + 0.3;
                steamNoise += sin(_Time.y * _SteamSpeed * 0.5 + worldPos.x * _SteamScale * 0.7 + worldPos.y * _SteamScale * 1.1) * 0.2 + 0.2;
                steamNoise *= _SteamIntensity;
                
                // 최종 오오라 거리 (수증기 효과 포함, 더 강하게)
                float auraDistance = _AuraThickness * _AuraIntensity + steamNoise * 0.03;
                
                // Vertex를 노멀 방향으로 밀어내기 (오오라 효과)
                worldPos += worldNormal * auraDistance;
                
                o.vertex = UnityWorldToClipPos(worldPos);
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                o.worldPos = worldPos;
                o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                // Rim Lighting 효과 (가장자리 빛나는 효과)
                float rim = 1.0 - saturate(dot(i.viewDir, i.normal));
                rim = pow(rim, _RimPower);
                rim *= _RimIntensity;
                
                // 수증기 같은 투명도 변화 (더 큰 패턴)
                float steamAlpha = sin(_Time.y * _SteamSpeed + i.worldPos.x * _SteamScale * 0.5) * 0.3 + 0.7;
                steamAlpha += sin(_Time.y * _SteamSpeed * 0.8 + i.worldPos.y * _SteamScale * 0.3) * 0.2 + 0.2;
                steamAlpha *= _SteamIntensity;
                
                // 오오라 색상 계산 (수증기 효과 포함)
                float4 auraColor = _AuraColor;
                auraColor.a *= rim * 0.2 * steamAlpha; // 수증기로 투명도 변화
                
                // Emission 효과 추가
                float4 emissionColor = _EmissionColor * _EmissionIntensity * rim * steamAlpha;
                emissionColor.a = auraColor.a; // 같은 투명도 사용
                
                // 최종 색상 (오오라 + emission)
                float4 finalColor = auraColor + emissionColor;
                finalColor.a = auraColor.a; // 투명도 유지
                
                return finalColor;
            }
            
            ENDCG
        }
    }
}
