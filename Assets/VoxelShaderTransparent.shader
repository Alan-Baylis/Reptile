Shader "Voxel/Transparent" {
	Properties {
		_MainTex ("RGBA", 2D) = "white" {}
		_DetailTex ("MSEU", 2D) = "black" {}
		_Pattern ("Pattern", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:fade

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DetailTex;
		sampler2D _Pattern;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uvp = 0;
			if (IN.worldNormal.x != 0) {
				uvp = float2(IN.worldPos.z - 0.5, IN.worldPos.y - 0.5);
			}
			else if (IN.worldNormal.y != 0) {
				uvp = float2(IN.worldPos.x - 0.5, IN.worldPos.z - 0.5);
			}
			else if (IN.worldNormal.z != 0) {
				uvp = float2(IN.worldPos.x - 0.5, IN.worldPos.y - 0.5);
			}

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 p = tex2D(_Pattern, uvp);
			o.Albedo = c.rgb * p.rgb;
			o.Alpha = c.a * p.a;

			fixed4 d = tex2D (_DetailTex, IN.uv_MainTex);
			o.Metallic = d.r;
			o.Smoothness = d.g;
			o.Emission = c.rgb * d.b;
		}
		ENDCG
	}
	FallBack "Standard"
}
