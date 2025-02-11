Shader "Custom/ScytheWithFixedOutlineAndMainTexture"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "gray" {} // ✅ 기본값을 `gray`로 설정 (흰색 방지)
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 0.03
    }

    SubShader
    {
        Tags { "Queue"="Geometry+1" } // ✅ 벽보다 먼저 렌더링되도록 조정

        // 1️⃣ **테두리 Pass (Outline)**
        Pass
        {
            Name "Outline Pass"
            Cull Front
            ZTest Always  // ✅ 벽 뒤에서도 보이도록 설정
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

                worldPosition += worldNormal * _OutlineThickness;
                o.pos = UnityWorldToClipPos(float4(worldPosition, 1));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor; // ✅ 초록색 테두리 적용
            }
            ENDCG
        }

        // 2️⃣ **기존 머티리얼 유지 (Main Object Pass)**
        Pass
        {
            Name "Main Object Pass"
            Cull Back
            ZWrite On
            ZTest Always // ✅ 벽 밖에서도 항상 보이게 설정

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col; // ✅ 기존 머티리얼 적용 (흰색 방지)
            }
            ENDCG
        }
    }
}
