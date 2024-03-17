Shader "Custom/BrushStrokeShade"
{
	Properties
	{
		[Header(OutLine)]
	// Stroke Color
		_StrokeColor("Stroke Color", Color) = (0,0,0,1)
		// Noise Map
		_OutlineNoise("Outline Noise Map", 2D) = "white" {}
		_Outline("Outline", Range(0, 1)) = 0.1
		// Outside Noise Width
		_OutsideNoiseWidth("Outside Noise Width", Range(1, 2)) = 1.3
		_MaxOutlineZOffset("Max Outline Z Offset", Range(0,1)) = 0.5

		[Header(Interior)]
		_Ramp("Ramp Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
		// Stroke Map
		_StrokeTex("Stroke Noise Tex", 2D) = "white" {}
		_InteriorNoise("Interior Noise Map", 2D) = "white" {}
		// Interior Noise Level
		_InteriorNoiseLevel("Interior Noise Level", Range(0, 1)) = 0.15
		// Guassian Blur
		radius("Guassian Blur Radius", Range(0,60)) = 30
		resolution("Resolution", float) = 800
		hstep("HorizontalStep", Range(0,1)) = 0.5
		vstep("VerticalStep", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"}

		// the first outline pass
		Pass
		{
			NAME "OUTLINE"
			Cull Front
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			half _MaxOutlineZOffset;
			CBUFFER_END

			struct Attributes
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct Varying
			{
				float4 pos : SV_POSITION;
			};

			Varying vert(Attributes v)
			{
				// fetch Perlin noise map here to map the vertex
				// add some bias by the normal direction
				float4 burn = tex2Dlod(_OutlineNoise, v.vertex);

				Varying o = (Varying)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				// camera space
				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;

				// unity_CameraProjection[1].y = fov/2
				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth);
				//linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _Outline;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);

				return o;
			}

			half4 frag(Varying i) : SV_Target
			{
				half4 c = _StrokeColor;
				return c;
			}
			ENDHLSL
		}

		// the second outline pass for random part, a little bit wider than last one
		Pass
		{
			NAME "OUTLINE 2"
			Cull Front
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float _Outline;
			float4 _StrokeColor;
			sampler2D _OutlineNoise;
			float _OutsideNoiseWidth;
			half _MaxOutlineZOffset;
			CBUFFER_END

			struct Attributes
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct Varying
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varying vert(Attributes v)
			{
				// fetch Perlin noise map here to map the vertex
				// add some bias by the normal direction
				float4 burn = tex2Dlod(_OutlineNoise, v.vertex);

				Varying o = (Varying)0;
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * _MaxOutlineZOffset;

				float linewidth = -position_cs.z / unity_CameraProjection[1].y;
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.y * _Outline * 1.1 * _OutsideNoiseWidth;
				position_cs.z = offset_pos_cs.z;

				o.pos = mul(UNITY_MATRIX_P, position_cs);

				o.uv = v.texcoord.xy;

				return o;
			}

			half4 frag(Varying i) : SV_Target
			{
				// clip random outline here
				half4 c = _StrokeColor;
				half3 burn = tex2D(_OutlineNoise, i.uv).rgb;
				if (burn.x > 0.5)
					discard;
				return c;
			}
			ENDHLSL
		}

		// the interior pass
		Pass
		{
			NAME "INTERIOR"
			Tags { "LightMode" = "UniversalForward" }

			Cull Off
			Blend Off
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			/*#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"*/
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			CBUFFER_START(UnityPerMaterial)
			sampler2D _Ramp;
			sampler2D _MainTex;
			float4 _Ramp_ST;
			sampler2D _StrokeTex;
			float4 _StrokeTex_ST;
			float radius;
			float resolution;
			//the direction of our blur
			//hstep (1.0, 0.0) -> x-axis blur
			//vstep(0.0, 1.0) -> y-axis blur
			//for example horizontaly blur equal:
			//float hstep = 1;
			//float vstep = 0;
			float hstep;
			float vstep;
			float _InteriorNoiseLevel;
			sampler2D _InteriorNoise;
			CBUFFER_END

			struct Attributes
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};

			struct Varying
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float2 uv2 : TEXCOORD3;
				float4 shadowCoord : TEXCOORD4;
			};


			Varying vert(Attributes v)
			{
				Varying o;
				o.pos = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _Ramp);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _StrokeTex);
				o.worldNormal = TransformObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TransformWorldToShadowCoord(o.worldPos);
				return o;
			}

			half4 frag(Varying i) : SV_Target
			{
				half3 worldNormal = normalize(i.worldNormal);
				half3 worldLightDir = normalize(GetMainLight().direction);

				// Perlin Noise
				// For the bias of the coordiante
				float4 burn = tex2D(_InteriorNoise, i.uv);
				float4 tex = tex2D(_MainTex, i.uv);
				// a little bit disturbance on normal vector
				half diff = dot(worldNormal, worldLightDir);
				diff = (diff * 0.5 + 0.5);
				float2 k = tex2D(_StrokeTex, i.uv).xy;
				float2 cuv = float2(diff, diff) + k * burn.xy * _InteriorNoiseLevel;

				// This iniminate the bias of the uv movement?
				if (cuv.x > 0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				if (cuv.y > 0.95)
				{
					cuv.x = 0.95;
					cuv.y = 1;
				}
				cuv = clamp(cuv, 0, 1);

				//Guassian Blur
				float4 sum = float4(0.0, 0.0, 0.0, 0.0);
				float2 tc = cuv;
				//blur radius in pixels
				float blur = radius / resolution / 4;
				sum += tex2D(_Ramp, float2(tc.x - 4.0 * blur * hstep, tc.y - 4.0 * blur * vstep)) * 0.0162162162;
				sum += tex2D(_Ramp, float2(tc.x - 3.0 * blur * hstep, tc.y - 3.0 * blur * vstep)) * 0.0540540541;
				sum += tex2D(_Ramp, float2(tc.x - 2.0 * blur * hstep, tc.y - 2.0 * blur * vstep)) * 0.1216216216;
				sum += tex2D(_Ramp, float2(tc.x - 1.0 * blur * hstep, tc.y - 1.0 * blur * vstep)) * 0.1945945946;
				sum += tex2D(_Ramp, float2(tc.x, tc.y)) * 0.2270270270;
				sum += tex2D(_Ramp, float2(tc.x + 1.0 * blur * hstep, tc.y + 1.0 * blur * vstep)) * 0.1945945946;
				sum += tex2D(_Ramp, float2(tc.x + 2.0 * blur * hstep, tc.y + 2.0 * blur * vstep)) * 0.1216216216;
				sum += tex2D(_Ramp, float2(tc.x + 3.0 * blur * hstep, tc.y + 3.0 * blur * vstep)) * 0.0540540541;
				sum += tex2D(_Ramp, float2(tc.x + 4.0 * blur * hstep, tc.y + 4.0 * blur * vstep)) * 0.0162162162;

				return half4(sum.rgb * tex, 1.0);
			}
			ENDHLSL
		}
	}
			FallBack "Diffuse"
}