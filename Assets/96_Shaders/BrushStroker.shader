Shader "Unlit/BrushStroker"
{
    Properties
    {
        [Header(OutLine)]
        // Stroke Color
        _StrokeColor("Stroke Color", Color) = (0,0,0,1)
        // Noise Map
        _OutlineNoise("Outline Noise Map", 2D) = "white" {}
        // First Outline Width
        _Outline("Outline Width", Range(0, 1)) = 0.1
        // Second Outline Width
        _OutsideNoiseWidth("Outside Noise Width", Range(1, 2)) = 1.3
        _MaxOutlineZOffset("Max Outline Z Offset", Range(0,1)) = 0.5

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _OutlineNoise;
            float _MaxOutlineZOffset;
            float _Outline;

            v2f vert (appdata v)
            {
                // fetch Perlin noise map here to map the vertex
                // add some bias by the normal direction
                float4 burn = tex2Dlod(_OutlineNoise, v.vertex);

                v2f o = (v2f)0;
                float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, normalize(v.normal.xyz));
                scaledir += 0.5;
                scaledir.z = 0.01;
                scaledir = normalize(scaledir);

                // camera space
                float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
                position_cs /= position_cs.w;

                float3 viewDir = normalize(position_cs.xyz);
                float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;

                // y = cos（fov/2）
                float linewidth = -position_cs.z / (unity_CameraProjection[1].y);
                linewidth = sqrt(linewidth);
                position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _Outline;
                position_cs.z = offset_pos_cs.z;
                o.vertex = mul(UNITY_MATRIX_P, position_cs);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
//