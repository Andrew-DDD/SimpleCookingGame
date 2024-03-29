﻿Shader "Custom/MatCap"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MatCap("MatCap (RGB)", 2D) = "white" {}
	}

		Subshader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 cap	: TEXCOORD0;
					UNITY_FOG_COORDS(1)
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					float3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
					worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
					o.cap.xy = worldNorm.xy * 0.5 + 0.5;

					UNITY_TRANSFER_FOG(o, o.pos);

					return o;
				}

				uniform float4 _Color;
				uniform sampler2D _MatCap;

				float4 frag(v2f i) : COLOR
				{
					float4 mc = tex2D(_MatCap, i.cap);
					mc = _Color * mc * unity_ColorSpaceDouble;
					UNITY_APPLY_FOG(i.fogCoord, mc);
					return mc;
				}
			ENDCG
		}
	}

		Fallback "VertexLit"
}