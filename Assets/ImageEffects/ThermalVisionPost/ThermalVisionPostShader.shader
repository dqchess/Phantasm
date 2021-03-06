﻿Shader "Hidden/ThermalVisionPost"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D ThermalRamp;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 col = tex2D(ThermalRamp, i.uv);

				float luminance = 0.2989 * col.r + 0.5874 * col.g + 0.1137 * col.b;
				
				//float4 thermalRamp = tex2D(ThermalRamp, float2(luminance * 0.8f, 0.0f));
				//float4 thermalRamp = tex2D(ThermalRamp, float2(max(luminance, 0.05f) * 0.925f, 0.0f));
				float4 thermalRamp = tex2D(ThermalRamp, float2(max(luminance, 0.00f) * 1.0f, 0.0f));
				//float4 thermalRamp = tex2D(_ThermalRamp, float2(max(dotProduct, 0.05f) * 0.925f, 0.0f));
				col = thermalRamp;
				
				return col;
			}
			ENDCG
		}
	}
}
