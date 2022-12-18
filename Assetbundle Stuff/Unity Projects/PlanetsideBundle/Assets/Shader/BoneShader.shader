// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33166,y:32943,varname:node_2865,prsc:2|emission-2424-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32114,y:32712,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32805,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:69e1494bf9c84a24abf1253a95a6652d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:358,x:32250,y:32780,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32250,y:32882,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_UVTile,id:8565,x:31851,y:33066,varname:node_8565,prsc:2|UVIN-4393-UVOUT,WDT-5678-OUT,HGT-5678-OUT,TILE-5678-OUT;n:type:ShaderForge.SFN_TexCoord,id:4393,x:31662,y:33024,varname:node_4393,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Time,id:5070,x:31373,y:33236,varname:node_5070,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3033,x:31570,y:33161,varname:node_3033,prsc:2|A-9984-OUT,B-5070-TSL;n:type:ShaderForge.SFN_Vector1,id:9984,x:31373,y:33161,varname:node_9984,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:5678,x:31851,y:33226,varname:node_5678,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:5684,x:31887,y:33423,varname:node_5684,prsc:2,v1:1;n:type:ShaderForge.SFN_Panner,id:7557,x:32049,y:33066,varname:node_7557,prsc:2,spu:1,spv:1|UVIN-8565-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:5098,x:32301,y:32931,ptovrint:False,ptlb:node_5098,ptin:_node_5098,varname:node_5098,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2439,x:32301,y:33264,varname:_BumpMap,prsc:2,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:2,isnm:False|UVIN-7040-UVOUT,TEX-5098-TEX;n:type:ShaderForge.SFN_Tex2d,id:8976,x:32301,y:33629,varname:_node_8081_copy,prsc:2,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:2,isnm:False|UVIN-3023-UVOUT,TEX-5098-TEX;n:type:ShaderForge.SFN_Panner,id:7040,x:32049,y:33226,varname:node_7040,prsc:2,spu:1,spv:-1|UVIN-8565-UVOUT;n:type:ShaderForge.SFN_Panner,id:8894,x:32049,y:33404,varname:node_8894,prsc:2,spu:-1,spv:-1|UVIN-8565-UVOUT;n:type:ShaderForge.SFN_Panner,id:3023,x:32049,y:33562,varname:node_3023,prsc:2,spu:-1,spv:1|UVIN-8565-UVOUT;n:type:ShaderForge.SFN_Blend,id:2424,x:32551,y:33431,varname:node_2424,prsc:2,blmd:8,clmp:True|SRC-8976-RGB,DST-2439-RGB;n:type:ShaderForge.SFN_Rotator,id:4955,x:32566,y:33625,varname:node_4955,prsc:2;n:type:ShaderForge.SFN_Rotator,id:5139,x:32523,y:33768,varname:node_5139,prsc:2;proporder:6665-7736-358-1813-5098;pass:END;sub:END;*/

Shader "Shader Forge/BoneShader" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Base Color", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
        _node_5098 ("node_5098", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _node_5098; uniform float4 _node_5098_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float4 node_312 = _Time;
                float node_5678 = 2.0;
                float2 node_8565_tc_rcp = float2(1.0,1.0)/float2( node_5678, node_5678 );
                float node_8565_ty = floor(node_5678 * node_8565_tc_rcp.x);
                float node_8565_tx = node_5678 - node_5678 * node_8565_ty;
                float2 node_8565 = (i.uv1 + float2(node_8565_tx, node_8565_ty)) * node_8565_tc_rcp;
                float2 node_3023 = (node_8565+node_312.g*float2(-1,1));
                float4 _node_8081_copy = tex2D(_node_5098,TRANSFORM_TEX(node_3023, _node_5098));
                float2 node_7040 = (node_8565+node_312.g*float2(1,-1));
                float4 _BumpMap = tex2D(_node_5098,TRANSFORM_TEX(node_7040, _node_5098));
                float3 node_2424 = saturate((_node_8081_copy.rgb+_BumpMap.rgb));
                float3 emissive = node_2424;
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _node_5098; uniform float4 _node_5098_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_3833 = _Time;
                float node_5678 = 2.0;
                float2 node_8565_tc_rcp = float2(1.0,1.0)/float2( node_5678, node_5678 );
                float node_8565_ty = floor(node_5678 * node_8565_tc_rcp.x);
                float node_8565_tx = node_5678 - node_5678 * node_8565_ty;
                float2 node_8565 = (i.uv1 + float2(node_8565_tx, node_8565_ty)) * node_8565_tc_rcp;
                float2 node_3023 = (node_8565+node_3833.g*float2(-1,1));
                float4 _node_8081_copy = tex2D(_node_5098,TRANSFORM_TEX(node_3023, _node_5098));
                float2 node_7040 = (node_8565+node_3833.g*float2(1,-1));
                float4 _BumpMap = tex2D(_node_5098,TRANSFORM_TEX(node_7040, _node_5098));
                float3 node_2424 = saturate((_node_8081_copy.rgb+_BumpMap.rgb));
                o.Emission = node_2424;
                
                float3 diffColor = float3(0,0,0);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0, specColor, specularMonochrome );
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
