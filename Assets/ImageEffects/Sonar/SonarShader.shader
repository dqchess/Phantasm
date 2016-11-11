﻿Shader "Hidden/SonarShader"
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
			
			float InverseLerp(float v, float v0, float v1)
			{
				return (v - v0) / (v1 - v0);
			}

			float rand(float2 co)
			{
				return frac(sin(1000.0*dot(co.xy, float2(21.5739, 43.421))) * 617284.3);
			}

			float rand(float co)
			{
				return frac(sin(1000.0 * co) * 617284.3);
			}

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			
			float uAmount;
			float RandomNumber;
			float4 uParameter;
			float uMultiplier = 0.05;
			float4 uColorAdd;
			
			//float uTime = 0.00f;
			
			float4x4 uProjBiasMatrixInverse;

			fixed4 frag (v2f i) : SV_Target
			{
				float depth = tex2D(_CameraDepthTexture, i.uv).r;

				fixed4 col = (tex2D(_MainTex, i.uv));
				

				float2 uvRound = floor(i.uv * 2.0f * 100.0f) / 100.0f / 2.0f;
				uvRound.x = floor(i.uv.x * 2.0f  * 177.7f) / 177.7f / 2.0f;
				uvRound.y = floor(i.uv.y * 2.0f  * 100.0f) / 100.0f / 2.0f;

				//col.r = DECODE_EYEDEPTH(depth) / _ProjectionParams.z;
				//depth = DECODE_EYEDEPTH(depth) / _ProjectionParams.z;
				if (depth < 1.0f)
				{
					float4 fragPos = mul(uProjBiasMatrixInverse, float4(i.uv.xy, depth, 1.0f));
					fragPos.xyz /= fragPos.w;
					//fragPos *= 1.0f;
					//col.rgb = fragPos.xyz * 1.0f;

					float fragDistance = length(fragPos.xyz) * uParameter.x; //uMultiplier;

					float fraction = frac(fragDistance - uColorAdd.a + (rand(uvRound + uColorAdd.aa) * 0.25f + 0.75f) * 0.05f);
					float powFraction;

					if (fraction < 0.05f)
					{
						fraction = lerp(1.0f, 0.05f, InverseLerp(fraction, 0.0f, 0.05f));
						powFraction = fraction;
					}
					else
					{
						powFraction = pow(fraction, 2.0f);
					}


					float3 addition = uColorAdd.rgb * powFraction * (rand(uvRound + uColorAdd.aa) * 0.5f + 0.25f);
					//float3 addition = uColorAdd.rgb * powFraction * (rand(fraction.rr + uvRound) * 0.5f + 0.25f);

					//float3 emissive = tex2D(uEmissiveTex, texcoord).rgb * uParameter.z;
					//float3 color = (tex2D(uSceneTex, texcoord).rgb - (emissive)) * uParameter.y;
					//fixed4 col = tex2D(_MainTex, i.uv);
					col = col * uParameter.y;
					float result = lerp(fraction, 1.0f, uParameter.a);
					col.rgb = (col.rgb + addition) * float3(result, result, result);// * (1.0 - pow(depth, 8.0));
					

				}
				else
				{
					
				}

				col.a = 1.0f;

				//col.rgb = float3(fragPos.xyz);

				//vec4 fragPos = uProjBiasMatrixInverse * vec4(texcoord, depth, 1.0);
				//fragPos /= fragPos.w;
				//
				//float fragDistance = length(fragPos.xyz) * uParameter.x; //uMultiplier;
				//
				//float fraction = fract(fragDistance - uColorAdd.a);
				//float powFraction;
				//
				//if (fraction < 0.05)
				//{
				//	fraction = mix(1.0, 0.05, InverseLerp(fraction, 0.0, 0.05));
				//	powFraction = fraction;
				//}
				//else
				//{
				//	powFraction = pow(fraction, 2.0);
				//}
				//
				//vec3 addition = uColorAdd.rgb * powFraction * (rand(texcoord + vec2(fraction)) * 0.5 + 0.25);
				//
				//vec3 emissive = texture(uEmissiveTex, texcoord).rgb * uParameter.z;
				//vec3 color = (texture(uSceneTex, texcoord).rgb - (emissive)) * uParameter.y;
				//
				//outColor.rgb = emissive + (color + addition) * vec3(mix(fraction, 1.0, uParameter.a));// * (1.0 - pow(depth, 8.0));
				
				
				


				

				return col;

			}
			ENDCG
		}
	}
}