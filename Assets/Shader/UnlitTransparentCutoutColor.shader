// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Unlit/Transparent Cutout Colored" {
Properties {
	_MainColor ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100
	Pass {
		Lighting Off
		Alphatest Greater [_Cutoff]
		SetTexture [_MainTex] {
			constantColor [_MainColor]
			Combine texture * constant, texture * constant 
		} 
		
	}
}
}
