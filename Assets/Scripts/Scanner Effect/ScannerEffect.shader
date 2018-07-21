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
			float4 _WorldSpaceScannerPos1;
			float4 _WorldSpaceScannerPos2;
			float _ScanDistance;
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

				// TODO: focus on this.
				// Replace with distance from closest origin?
				// Hardcode possibility for a set number of simultaneous pings.
				// distFromClosest = min( distance(wsPos,_WorldSpaceScannerPos) for all scanner pos)
				// distFromClosest = min( distance(origin1), distance(origin2), distance(origin3), ... )
				////float dist = distance(wsPos, _WorldSpaceScannerPos);
				float dist1 = distance(wsPos, _WorldSpaceScannerPos1);
				float dist2 = distance(wsPos, _WorldSpaceScannerPos2);
				// We'll need to change ScannerEffect.cs for this.

				// TODO: Make this work for multiple "scan distances" so we support multiple pings
				// This is the crux of the matter
				// We need to test this for multiple distances, so have a for(ping) {if ping.dist < ping._ScanDistance ...}

				// BIG IDEA:
				// Allow for, let's say, 2 simultaneous pings. (in final, much more than that)
				// we will have dist1, dist2
				//if ( ( dist1 < _ScanDistance && dist1 > _ScanDistance - _ScanWidth && linearDepth < 1 ) || ( dist2 < _ScanDistance && dist2 > _ScanDistance - _ScanWidth && linearDepth < 1 ) )
				// What we really want is a foreach(Vector4 scannerPos in scannerPositions) { float dist = ... (if(dist < ...))}
				// Where scannerPositions is a list of origins that can be dynamically resized whenever a new origin is instantiated/object pooled.
				////if (dist < _ScanDistance && dist > _ScanDistance - _ScanWidth && linearDepth < 1)

				if (dist1 < _ScanDistance && dist1 > _ScanDistance - _ScanWidth && linearDepth < 1)
				{
					float diff = 1 - (_ScanDistance - dist1) / (_ScanWidth);
					half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
					scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.uv) * _HBarColor;
					scannerCol *= diff;
				}
				if (dist2 < _ScanDistance && dist2 > _ScanDistance - _ScanWidth && linearDepth < 1)
				{
					float diff = 1 - (_ScanDistance - dist2) / (_ScanWidth);
					half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
					scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.uv) * _HBarColor;
					scannerCol *= diff;
				}

				return col + scannerCol;
			}
			ENDCG
		}
	}
}
