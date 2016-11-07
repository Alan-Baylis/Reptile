Shader "Custom/VoxelShader" {
	Properties {
		_MainTex ("RGBA", 2D) = "white" {}
		_DetailTex ("MSEU", 2D) = "black" {}
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Transparent" }
		LOD 200
		Pass{
		ZWrite On
		ColorMask 0

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				return half4 (0, 0, 0, 0);
			}
		ENDCG
	}
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DetailTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			fixed4 d = tex2D (_DetailTex, IN.uv_MainTex);
			o.Metallic = d.r;
			o.Smoothness = d.g;
			o.Emission = c.rgb * d.b;
		}
		ENDCG
	}
	FallBack "Standard"
}
