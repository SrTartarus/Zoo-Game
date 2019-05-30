// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Clothes_Shader"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (1,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_MaskRGB("MaskRGB", 2D) = "white" {}
		_ToonRamp("Toon Ramp", 2D) = "white" {}
		_ColorMangas("ColorMangas", Color) = (1,0,0,0)
		_ColorShort("ColorShort", Color) = (1,0.9724138,0,0)
		_ColorEspalda("ColorEspalda", Color) = (0,1,0.08965516,0)
		_BaseColorRGB("BaseColorRGB", 2D) = "white" {}
		_RimOffset("Rim Offset", Float) = 0.24
		_RimPower("Rim Power", Range( 0 , 10)) = 0.5
		[HDR]_RimColor("Rim Color", Color) = (0,1,0.8745098,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		uniform fixed4 _ASEOutlineColor;
		uniform fixed _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _MaskRGB;
		uniform float4 _MaskRGB_ST;
		uniform float4 _ColorMangas;
		uniform float4 _ColorShort;
		uniform float4 _ColorEspalda;
		uniform sampler2D _BaseColorRGB;
		uniform float4 _BaseColorRGB_ST;
		uniform sampler2D _ToonRamp;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimColor;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#if DIRECTIONAL
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			float2 uv_MaskRGB = i.uv_texcoord * _MaskRGB_ST.xy + _MaskRGB_ST.zw;
			float4 break3 = tex2D( _MaskRGB, uv_MaskRGB );
			float2 uv_BaseColorRGB = i.uv_texcoord * _BaseColorRGB_ST.xy + _BaseColorRGB_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult20 = dot( ase_worldNormal , ase_worldlightDir );
			float2 temp_cast_0 = (saturate( (dotResult20*0.5 + 0.5) )).xx;
			float3 desaturateInitialColor75 = tex2D( _ToonRamp, temp_cast_0 ).rgb;
			float desaturateDot75 = dot( desaturateInitialColor75, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar75 = lerp( desaturateInitialColor75, desaturateDot75.xxx, 0.0 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi31 = gi;
			float3 diffNorm31 = ase_worldNormal;
			gi31 = UnityGI_Base( data, 1, diffNorm31 );
			float3 indirectDiffuse31 = gi31.indirect.diffuse + diffNorm31 * 0.0001;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult24 = dot( ase_worldNormal , ase_worldViewDir );
			c.rgb = ( ( ( ( ( ( ( break3.r * _ColorMangas ) + ( break3.g * _ColorShort ) + ( break3.b * _ColorEspalda ) ) + ( ( 1.0 - break3.r ) * ( 1.0 - break3.g ) * ( 1.0 - break3.b ) ) ) * tex2D( _BaseColorRGB, uv_BaseColorRGB ) ) * float4( desaturateVar75 , 0.0 ) ) * ( ase_lightColor * float4( ( indirectDiffuse31 + ase_lightAtten ) , 0.0 ) ) ) + ( saturate( ( ( ase_lightAtten * dotResult20 ) * pow( ( 1.0 - saturate( ( dotResult24 + _RimOffset ) ) ) , _RimPower ) ) ) * ( _RimColor * ase_lightColor ) ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
7;29;1906;1004;1970.128;2955.229;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;21;-3260.563,-1928.615;Float;False;507.201;385.7996;Comment;3;24;23;22;N . V;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;35;-2541.939,-1578.081;Float;False;1535.111;547.3881;;13;47;46;39;38;42;36;37;40;41;45;43;44;48;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;22;-3212.563,-1880.615;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;23;-3164.563,-1720.615;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;17;-3266.355,-2343.67;Float;False;539.401;343.6003;Comment;3;20;18;19;N . L;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-3411.447,-3428.543;Float;True;Property;_MaskRGB;MaskRGB;0;0;Create;True;0;0;False;0;None;ad482c79df7674f4dad8e94386d737f6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;24;-2908.563,-1800.615;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-2505.336,-1329.822;Float;False;Property;_RimOffset;Rim Offset;6;0;Create;True;0;0;False;0;0.24;0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;18;-3175.355,-2308.67;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;19;-3195.355,-2161.671;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;25;-2463.513,-2434.595;Float;False;723.599;290;Also know as Lambert Wrap or Half Lambert;3;28;27;26;Diffuse Wrap;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-2290.336,-1443.822;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-2985.482,-2840.99;Float;False;Property;_ColorEspalda;ColorEspalda;4;0;Create;True;0;0;False;0;0,1,0.08965516,0;0.9607843,0,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;3;-3043.094,-3419.016;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;13;-2993.382,-3035.296;Float;False;Property;_ColorShort;ColorShort;3;0;Create;True;0;0;False;0;1,0.9724138,0,0;0.2,0.8313726,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-3000.175,-3222.15;Float;False;Property;_ColorMangas;ColorMangas;2;0;Create;True;0;0;False;0;1,0,0,0;0,0.08235294,0.9607843,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;27;-2413.514,-2259.596;Float;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;20;-2866.355,-2231.671;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;38;-2128.773,-1439.85;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;29;-2498.702,-2030.428;Float;False;812;304;Comment;5;34;33;32;31;30;Attenuation and Ambient;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;63;-2489.219,-3662.688;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;61;-2493.734,-3838.524;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2268.267,-3146.937;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-2268.502,-3244.867;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;62;-2487.001,-3751.852;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2275.453,-3362.54;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;28;-2148.114,-2384.595;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2227.773,-1253.85;Float;False;Property;_RimPower;Rim Power;7;0;Create;True;0;0;False;0;0.5;0.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1982.119,-3312.389;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;30;-2451.302,-1837.428;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;39;-1956.773,-1438.85;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;26;-1914.917,-2377.796;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-1975.533,-3703.966;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;41;-1787.773,-1369.85;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1651.619,-3492.003;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1927.773,-1541.85;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-1833.171,-3037.661;Float;True;Property;_BaseColorRGB;BaseColorRGB;5;0;Create;True;0;0;False;0;a270e82532678eb4aa107ff3f8e45aa6;a270e82532678eb4aa107ff3f8e45aa6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;49;-1539.976,-2444.361;Float;True;Property;_ToonRamp;Toon Ramp;1;0;Create;True;0;0;False;0;52e66a9243cdfed44b5e906f5910d35b;52e66a9243cdfed44b5e906f5910d35b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IndirectDiffuseLighting;31;-2241.703,-1902.428;Float;False;Tangent;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;46;-1602.905,-1357.897;Float;False;Property;_RimColor;Rim Color;8;1;[HDR];Create;True;0;0;False;0;0,1,0.8745098,0;0,1,0.8745098,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1396.417,-3177.02;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;47;-1585.905,-1171.897;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;32;-2413.702,-1990.428;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-1991.705,-1871.428;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DesaturateOpNode;75;-1171.681,-2552.821;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1619.286,-1482.865;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1313.905,-1273.897;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;44;-1423.286,-1478.865;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-894.2479,-2907.529;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1842.705,-1982.428;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-522.7363,-2507.036;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1199.905,-1473.897;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-250.3893,-2343.156;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;67.68793,-2486.936;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Clothes_Shader;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;True;0;1,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;37;0;24;0
WireConnection;37;1;36;0
WireConnection;3;0;1;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;38;0;37;0
WireConnection;63;0;3;2
WireConnection;61;0;3;0
WireConnection;15;0;3;2
WireConnection;15;1;16;0
WireConnection;12;0;3;1
WireConnection;12;1;13;0
WireConnection;62;0;3;1
WireConnection;5;0;3;0
WireConnection;5;1;4;0
WireConnection;28;0;20;0
WireConnection;28;1;27;0
WireConnection;28;2;27;0
WireConnection;14;0;5;0
WireConnection;14;1;12;0
WireConnection;14;2;15;0
WireConnection;39;0;38;0
WireConnection;26;0;28;0
WireConnection;68;0;61;0
WireConnection;68;1;62;0
WireConnection;68;2;63;0
WireConnection;41;0;39;0
WireConnection;41;1;40;0
WireConnection;73;0;14;0
WireConnection;73;1;68;0
WireConnection;42;0;30;0
WireConnection;42;1;20;0
WireConnection;49;1;26;0
WireConnection;74;0;73;0
WireConnection;74;1;6;0
WireConnection;33;0;31;0
WireConnection;33;1;30;0
WireConnection;75;0;49;0
WireConnection;43;0;42;0
WireConnection;43;1;41;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;44;0;43;0
WireConnection;50;0;74;0
WireConnection;50;1;75;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;51;0;50;0
WireConnection;51;1;34;0
WireConnection;45;0;44;0
WireConnection;45;1;48;0
WireConnection;52;0;51;0
WireConnection;52;1;45;0
WireConnection;0;13;52;0
ASEEND*/
//CHKSM=F051396F55C64EB020BF7427DC5D4763E85008D1