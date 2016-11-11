﻿Shader "Hidden/ShaderGBuffer2"
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
			
			sampler2D_half _MainTex;
			sampler2D _CameraGBufferTexture2;
			float GammaA;
			float GammaY;

			half4 frag(v2f i) : SV_Target
			{
				half4 col= half4(0.0f, 0.0f, 0.0f, 1.0f);
				col.rgb = tex2D(_CameraGBufferTexture2, i.uv).rgb * 1.0f;
				return col;
			}
			ENDCG
		}
	}
}