Shader "Custom/mask" {
    SubShader {
        Tags {"Queue" = "Geometry-1" }       
        Lighting Off
		//ZTest LEqual
		ZWrite On
		Blend Off
		ColorMask 0

        Pass { 
		}
    }
}

