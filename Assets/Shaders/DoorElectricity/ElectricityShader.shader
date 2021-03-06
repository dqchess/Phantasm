﻿Shader "Unlit/ElectricityBeam"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		timeMult ("timeMult", Float) = 1000.0
		alphaAmount("Alpha", Float) = 0.4
		alphaAdd("Alpha Add", Float) = 0.4
		ColorMult( "Color", Color) = (0.5, 0.5, 0.5, 0.5)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"}
		LOD 100
		ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha One

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float timeMult;
			float alphaAmount;
			float alphaAdd;
			float4 ColorMult;
			float uDistance;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				float timeAdjust = -_Time * timeMult * 1.0f; 
				//float2 p = i.uv.xy * 10.0f * float2(0.01f * uDistance, 1.0f);
				float2 p = i.uv.xy * 10.0f * float2(0.01f, 1.0f);
				for (int i = 1; i < 4; i++)
				{
					float2 newp = p;
					newp.x += (1.15f / float(i) * sin(float(i) * p.y + (timeAdjust *  0.3f) / 20.0f + 0.3f * float(i)				) + 4.0f);
					newp.y += (1.55f / float(i) * cos(float(i) * p.x - (timeAdjust * -0.2f) / 20.0f + 0.3f * float(i) * 10.0f	) - 4.0f);
					p = newp;
				}

				col.a *= alphaAmount * sin(p.x) + 0.8f + alphaAdd;
				return col * (ColorMult * 2.0f);
			}
			ENDCG
		}
	}
}
