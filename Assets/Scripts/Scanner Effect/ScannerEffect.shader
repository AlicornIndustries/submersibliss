﻿Shader "Hidden/ScannerEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DetailTex("Texture", 2D) = "white" {}
		_ScanDistance("Scan Distance", float) = 0
		_ScanWidth("Scan Width", float) = 10
		_LeadSharp("Leading Edge Sharpness", float) = 10
		_LeadColor("Leading Edge Color", Color) = (1, 1, 1, 0)
		_MidColor("Mid Color", Color) = (1, 1, 1, 0)
		_TrailColor("Trail Color", Color) = (1, 1, 1, 0)
		_HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)

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

			struct VertIn
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 ray : TEXCOORD1;
			};

			struct VertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_depth : TEXCOORD1;
				float4 interpolatedRay : TEXCOORD2;
			};

			float4 _MainTex_TexelSize;
			float4 _CameraWS;

			VertOut vert(VertIn v)
			{
				VertOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				o.uv_depth = v.uv.xy;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
				#endif				

				o.interpolatedRay = v.ray;

				return o;
			}

			sampler2D _MainTex;
			sampler2D _DetailTex;
			sampler2D_float _CameraDepthTexture;
			// NEW STUFF
			int _numCircleScanners;
			float4 _circleScannersWorldSpacePositions[2]; // we probably need to hardcode it like this, unfortunately. _numCircleScanners must never exceed this value
			float _circleScannings[2]; // 0 if not scanning, 1 if scanning. Shaders can't use bools or ints
			float _circleScanDistances[2];

			//float _ScanDistance;
			float _ScanWidth;
			float _LeadSharp;
			float4 _LeadColor;
			float4 _MidColor;
			float4 _TrailColor;
			float4 _HBarColor;

			float4 horizBars(float2 p)
			{
				return 1 - saturate(round(abs(frac(p.y * 100) * 2)));
			}

			float4 horizTex(float2 p)
			{
				return tex2D(_DetailTex, float2(p.x * 30, p.y * 40));
			}

			half4 frag (VertOut i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);

				float rawDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv_depth));
				float linearDepth = Linear01Depth(rawDepth);
				float4 wsDir = linearDepth * i.interpolatedRay; // direction in worldspace pointing from camera to far plane
				float3 wsPos = _WorldSpaceCameraPos + wsDir;
				half4 scannerCol = half4(0, 0, 0, 0);
				
				// NEW STUFF
				// n instead of i because i is used in horizBars(i.uv);
				for(int n = 0; n < _numCircleScanners; n++)
				{
					float isScanning = _circleScannings[n];
					if(isScanning > 0) // if(_circleScannings[n] > 0) causes an error; apparently it's difficult to unroll
					{
						float dist = distance(wsPos, _circleScannersWorldSpacePositions[n]);
						float _ScanDistance = _circleScanDistances[n];
						if (dist < _ScanDistance && dist > _ScanDistance - _ScanWidth && linearDepth < 1 )
						{
							float diff = 1 - (_ScanDistance - dist) / (_ScanWidth);
							half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
							scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.uv) * _HBarColor;
							scannerCol *= diff;
						}
					}
				}
				return col + scannerCol;
			}
			ENDCG
		}
	}
}
