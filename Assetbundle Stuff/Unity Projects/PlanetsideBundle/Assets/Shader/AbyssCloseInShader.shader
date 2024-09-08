// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33028,y:33260,varname:node_2865,prsc:2|emission-929-OUT,alpha-2955-OUT,olwid-5791-OUT,olcol-929-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:31938,y:33229,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Relay,id:8397,x:32137,y:33229,cmnt:Refract here,varname:node_8397,prsc:2|IN-4219-UVOUT;n:type:ShaderForge.SFN_Relay,id:4676,x:32558,y:33229,cmnt:Modify color here,varname:node_4676,prsc:2|IN-7542-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31827,y:33370,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ff30ad2400a3a8842bb170ff2a31bf5d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7542,x:32235,y:33282,varname:node_1672,prsc:2,tex:ff30ad2400a3a8842bb170ff2a31bf5d,ntxv:0,isnm:False|UVIN-8397-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Multiply,id:929,x:32528,y:33317,varname:node_929,prsc:2|A-4676-OUT,B-4617-OUT;n:type:ShaderForge.SFN_Vector1,id:4617,x:32499,y:33145,varname:node_4617,prsc:2,v1:2;n:type:ShaderForge.SFN_FragmentPosition,id:9767,x:31798,y:33783,varname:node_9767,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:5804,x:31589,y:33565,varname:node_5804,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:436,x:31985,y:33977,ptovrint:False,ptlb:node_436,ptin:_node_436,varname:node_436,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Add,id:3480,x:32003,y:33845,varname:node_3480,prsc:2|A-9767-X,B-436-OUT;n:type:ShaderForge.SFN_Distance,id:2739,x:32186,y:33795,varname:node_2739,prsc:2|A-9767-X,B-3480-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2955,x:32371,y:33526,ptovrint:False,ptlb:node_2955,ptin:_node_2955,varname:node_2955,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9920,x:32472,y:33469,ptovrint:False,ptlb:node_9920,ptin:_node_9920,varname:node_9920,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_NormalVector,id:3089,x:32371,y:33686,prsc:2,pt:True;n:type:ShaderForge.SFN_Add,id:1088,x:32651,y:33678,varname:node_1088,prsc:2|A-3089-OUT,B-1031-OUT;n:type:ShaderForge.SFN_Slider,id:1031,x:32464,y:33850,ptovrint:False,ptlb:node_1031,ptin:_node_1031,varname:node_1031,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5791,x:32196,y:33625,ptovrint:False,ptlb:node_5791,ptin:_node_5791,varname:node_5791,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:4,cur:4.615385,max:6;proporder:4430-436-2955-9920-1031-5791;pass:END;sub:END;*/

Shader "Shader Forge/AbyssCloseInShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _node_436 ("node_436", Float ) = 3
        _node_2955 ("node_2955", Float ) = 0
        _node_9920 ("node_9920", Float ) = 1
        _node_1031 ("node_1031", Range(0, 1)) = 0
        _node_5791 ("node_5791", Range(4, 6)) = 4.615385
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_5791;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + v.normal*_node_5791,1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_8397 = i.uv0; // Refract here
                float4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_8397, _MainTex));
                float3 node_929 = (node_1672.rgb*2.0);
                return fixed4(node_929,0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_2955;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_8397 = i.uv0; // Refract here
                float4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_8397, _MainTex));
                float3 node_929 = (node_1672.rgb*2.0);
                float3 emissive = node_929;
                float3 finalColor = emissive;
                return fixed4(finalColor,_node_2955);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
