Shader "Zen/3DScan" {
	Properties {

		//Most of these don't work yet
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Bumpmap", 2D) = "bump" {}
		
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}	
				
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off //!!!IMPORTANT otherwise the effect looks dumb as shit
		LOD 300
		
		CGPROGRAM
		
		#pragma surface surf Scan fullforwardshadows vertex:vert
		
		#include "UnityPBSLighting.cginc"

		#pragma target 3.0	

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_MetallicGlossMap;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			float3 worldPos;
			float3 viewDir;
			float3 localPos;  //Filled below w/ vert
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _ConstructY;
		fixed4 _ConstructColor; //Color to show on the 'in construction' surface areas
		
		int building;  //whether we're in process area or not
		float3 viewDir;

		inline half4 LightingScan(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			// use ConstructColor if we're working on a surface area that's being built
			if (building)
				return _ConstructColor;

			// also use construction color when we're seeing a back face, so it doesn't look hollow
			if (dot(s.Normal, viewDir) < 0)
				return _ConstructColor;
			
			return LightingStandard(s, lightDir, gi); // Unity5 PBR	default		
		}

		//This is useless and just returns default value, has to be here or unity bitches
		inline void LightingScan_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);		
		}
		
		//Grab local position for use in cutoff point
		void vert (inout appdata_full v, out Input o) 
		{
		  UNITY_INITIALIZE_OUTPUT(Input,o);
		  o.localPos = v.vertex.xyz;
		}

		//#pragma target 3.0

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Makes the construction area appear less static and mechanical, really apparent up close
			float s = +sin((IN.localPos.x * IN.localPos.z) * 60 + _Time[3] + o.Normal) / 120;

			//how much of an area to color as if it is "in construction"
			//change this number a couple times and look at the result and you'll see what it does ez
			float _ConstructGap = 0.06;
			viewDir = IN.viewDir;
			
			//Completely trash surfaces above our _ConstructY limit, makes them fully invis
			if (IN.localPos.y > _ConstructY + s + _ConstructGap)
				discard;
			
			//This is the after-oconstruction area, just set everything normally
			if (IN.localPos.y < _ConstructY)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha  = c.a;
		 
				building = 0;
			}
			else //This is the gap area, where we shade it _ConstructColor only
			{
				o.Albedo = _ConstructColor.rgb;
				o.Alpha  = _ConstructColor.a;
		 
				building = 1;
			}
		 
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			//FML WHY DOES THIS NOT WORK
			//o.Normal = UnpackNormal ( tex2D(_BumpMap, IN.uv_BumpMap) );
			//o.Emission = _EmissionMap;
			//o.Occlusion = _OcclusionMap;

		}	

		ENDCG
	}
	FallBack "Diffuse"
}


