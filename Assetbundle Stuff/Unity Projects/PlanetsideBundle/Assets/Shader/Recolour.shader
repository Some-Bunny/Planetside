// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:577,x:33554,y:32885,varname:node_577,prsc:2|diff-6713-OUT,custl-6713-OUT,clip-5619-A;n:type:ShaderForge.SFN_Tex2d,id:5619,x:31935,y:32343,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_5619,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:1005,x:32130,y:32059,ptovrint:False,ptlb:OldColour,ptin:_OldColour,varname:_NewColour_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3686275,c2:0.1137255,c3:0.09019608,c4:1;n:type:ShaderForge.SFN_Add,id:6713,x:33050,y:32851,varname:node_6713,prsc:2|A-3763-OUT,B-5619-RGB;n:type:ShaderForge.SFN_Distance,id:8916,x:32382,y:32132,varname:node_8916,prsc:2|A-1005-RGB,B-5619-RGB;n:type:ShaderForge.SFN_Subtract,id:8956,x:32565,y:32132,varname:node_8956,prsc:2|A-8916-OUT,B-6538-OUT;n:type:ShaderForge.SFN_Vector1,id:2615,x:32699,y:32042,varname:node_2615,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6538,x:32382,y:32279,ptovrint:False,ptlb:range,ptin:_range,varname:node_6538,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.06;n:type:ShaderForge.SFN_Divide,id:2095,x:32917,y:32179,varname:node_2095,prsc:2|A-8956-OUT,B-1117-OUT;n:type:ShaderForge.SFN_Max,id:1117,x:32747,y:32242,varname:node_1117,prsc:2|A-1964-OUT,B-1425-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1964,x:32552,y:32286,ptovrint:False,ptlb:fuzziness,ptin:_fuzziness,varname:node_1964,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Vector1,id:1425,x:32552,y:32348,varname:node_1425,prsc:2,v1:1E-05;n:type:ShaderForge.SFN_Subtract,id:3909,x:33100,y:32072,varname:node_3909,prsc:2|A-2615-OUT,B-2095-OUT;n:type:ShaderForge.SFN_Multiply,id:3763,x:33558,y:31942,varname:node_3763,prsc:2|A-4837-OUT,B-7868-OUT;n:type:ShaderForge.SFN_Clamp01,id:7868,x:33326,y:32052,varname:node_7868,prsc:2|IN-3909-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:4837,x:33260,y:31807,ptovrint:False,ptlb:Recolour,ptin:_Recolour,varname:node_4837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-1005-RGB,B-7536-RGB;n:type:ShaderForge.SFN_Blend,id:1541,x:33168,y:32629,varname:node_1541,prsc:2,blmd:10,clmp:False|SRC-3763-OUT,DST-5619-RGB;n:type:ShaderForge.SFN_Color,id:7536,x:32967,y:31586,ptovrint:False,ptlb:NColour,ptin:_NColour,varname:node_7536,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4981757,c2:0.1395437,c3:0.6544118,c4:1;proporder:5619-1005-6538-1964-4837-7536;pass:END;sub:END;*/

Shader "Custom/Recolour" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _OldColour ("OldColour", Color) = (0.3686275,0.1137255,0.09019608,1)
        _range ("range", Float ) = 0.06
        _fuzziness ("fuzziness", Float ) = 0
        [MaterialToggle] _Recolour ("Recolour", Float ) = 0.4981757
        _NColour ("NColour", Color) = (0.4981757,0.1395437,0.6544118,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _OldColour;
            uniform float _range;
            uniform float _fuzziness;
            uniform fixed _Recolour;
            uniform float4 _NColour;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(_MainTex_var.a - 0.5);
////// Lighting:
                float3 node_3763 = (lerp( _OldColour.rgb, _NColour.rgb, _Recolour )*saturate((1.0-((distance(_OldColour.rgb,_MainTex_var.rgb)-_range)/max(_fuzziness,0.00001)))));
                float3 node_6713 = (node_3763+_MainTex_var.rgb);
                float3 finalColor = node_6713;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(_MainTex_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
