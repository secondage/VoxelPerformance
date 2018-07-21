Shader "Custom/InstancedColorSurfaceShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf SimpleLambert2 fullforwardshadows
		// Use Shader model 3.0 target
#pragma target 3.0
		half4 LightingSimpleLambert2(SurfaceOutput s, half3 lightDir, half atten) {
		half4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}
		sampler2D _MainTex;
	struct Input {
		float2 uv_MainTex;
	};

	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		UNITY_INSTANCING_BUFFER_END(Props)
		void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
		o.Albedo = c.rgb;
	
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}