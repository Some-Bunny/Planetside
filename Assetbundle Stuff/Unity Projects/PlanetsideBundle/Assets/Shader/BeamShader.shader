// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:1,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:35933,y:33746,varname:node_4013,prsc:2|normal-2418-OUT,emission-8084-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32978,y:33215,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3402,x:32767,y:32839,varname:node_3402,prsc:2,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:2,isnm:False|UVIN-965-OUT,TEX-9440-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:9440,x:32767,y:33052,ptovrint:False,ptlb:node_9440,ptin:_node_9440,varname:node_9440,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6552,x:32767,y:33225,varname:node_6552,prsc:2,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False|UVIN-401-OUT,TEX-9440-TEX;n:type:ShaderForge.SFN_UVTile,id:2942,x:32084,y:33074,varname:node_2942,prsc:2|UVIN-9285-UVOUT,WDT-7444-OUT,HGT-7444-OUT,TILE-227-OUT;n:type:ShaderForge.SFN_TexCoord,id:9285,x:31790,y:32919,varname:node_9285,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:7444,x:31790,y:33091,varname:node_7444,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Rotator,id:2755,x:32324,y:33084,varname:node_2755,prsc:2|UVIN-2942-UVOUT,PIV-965-OUT,ANG-7412-OUT,SPD-7412-OUT;n:type:ShaderForge.SFN_Panner,id:828,x:32324,y:33227,varname:node_828,prsc:2,spu:2,spv:2|UVIN-2942-UVOUT,DIST-7412-OUT;n:type:ShaderForge.SFN_Add,id:401,x:32505,y:33227,varname:node_401,prsc:2|A-2755-UVOUT,B-828-UVOUT;n:type:ShaderForge.SFN_Panner,id:2582,x:32324,y:32761,varname:node_2582,prsc:2,spu:1,spv:2|UVIN-8662-UVOUT;n:type:ShaderForge.SFN_Blend,id:356,x:33029,y:32836,varname:node_356,prsc:2,blmd:3,clmp:True|SRC-3402-RGB,DST-6552-RGB;n:type:ShaderForge.SFN_UVTile,id:8662,x:32084,y:32761,varname:node_8662,prsc:2|UVIN-9285-UVOUT,WDT-3745-OUT,HGT-3745-OUT,TILE-3745-OUT;n:type:ShaderForge.SFN_Vector1,id:3745,x:32084,y:32916,varname:node_3745,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7412,x:32084,y:33238,varname:node_7412,prsc:2,v1:3;n:type:ShaderForge.SFN_Rotator,id:3146,x:32324,y:32916,varname:node_3146,prsc:2|UVIN-8662-UVOUT,ANG-3745-OUT;n:type:ShaderForge.SFN_Add,id:965,x:32509,y:32841,varname:node_965,prsc:2|A-3146-UVOUT,B-2582-UVOUT;n:type:ShaderForge.SFN_Vector1,id:227,x:31790,y:33166,varname:node_227,prsc:2,v1:2;n:type:ShaderForge.SFN_ChannelBlend,id:3336,x:33183,y:33223,varname:node_3336,prsc:2,chbt:0|M-356-OUT,R-1304-RGB,G-1304-RGB,B-356-OUT;n:type:ShaderForge.SFN_Noise,id:7271,x:33256,y:33508,varname:node_7271,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2063,x:33578,y:33223,varname:node_2063,prsc:2|A-9435-OUT,B-1937-OUT;n:type:ShaderForge.SFN_Vector1,id:1937,x:33578,y:33162,varname:node_1937,prsc:2,v1:25;n:type:ShaderForge.SFN_Reflect,id:9435,x:33375,y:33223,varname:node_9435,prsc:2|A-356-OUT,B-3336-OUT;n:type:ShaderForge.SFN_Vector1,id:8184,x:33375,y:33369,varname:node_8184,prsc:2,v1:-3;n:type:ShaderForge.SFN_Reflect,id:6926,x:33578,y:33369,varname:node_6926,prsc:2|A-2063-OUT,B-8184-OUT;n:type:ShaderForge.SFN_Color,id:9811,x:33602,y:33582,ptovrint:False,ptlb:node_9811,ptin:_node_9811,varname:node_9811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:6704,x:33602,y:33788,ptovrint:False,ptlb:node_9811_copy,ptin:_node_9811_copy,varname:_node_9811_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:140,x:33840,y:33562,varname:node_140,prsc:2|A-6926-OUT,B-9811-RGB,C-6704-RGB;n:type:ShaderForge.SFN_OneMinus,id:1438,x:34034,y:33562,varname:node_1438,prsc:2|IN-140-OUT;n:type:ShaderForge.SFN_Multiply,id:2966,x:33872,y:33798,varname:node_2966,prsc:2|A-1438-OUT,B-6704-RGB;n:type:ShaderForge.SFN_ChannelBlend,id:920,x:34280,y:33798,varname:node_920,prsc:2,chbt:1|M-5560-OUT,R-2966-OUT,G-2966-OUT,B-5560-OUT,BTM-5560-OUT;n:type:ShaderForge.SFN_Blend,id:5560,x:34042,y:33904,varname:node_5560,prsc:2,blmd:8,clmp:True|SRC-140-OUT,DST-2966-OUT;n:type:ShaderForge.SFN_Blend,id:1116,x:34672,y:33592,varname:node_1116,prsc:2,blmd:3,clmp:True|SRC-920-OUT,DST-140-OUT;n:type:ShaderForge.SFN_Color,id:2524,x:34292,y:34012,ptovrint:False,ptlb:node_2524,ptin:_node_2524,varname:node_2524,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:10,c2:10,c3:10,c4:1;n:type:ShaderForge.SFN_Multiply,id:6407,x:34493,y:33798,varname:node_6407,prsc:2|A-920-OUT,B-2524-RGB;n:type:ShaderForge.SFN_Desaturate,id:3205,x:34562,y:33482,varname:node_3205,prsc:2;n:type:ShaderForge.SFN_Round,id:1199,x:34672,y:33814,varname:node_1199,prsc:2|IN-6407-OUT;n:type:ShaderForge.SFN_Posterize,id:2418,x:34945,y:33903,varname:node_2418,prsc:2|IN-1569-OUT,STPS-8274-OUT;n:type:ShaderForge.SFN_Vector1,id:8274,x:34739,y:34082,varname:node_8274,prsc:2,v1:10;n:type:ShaderForge.SFN_Multiply,id:3233,x:35786,y:34102,varname:node_3233,prsc:2;n:type:ShaderForge.SFN_Blend,id:1569,x:34988,y:33614,varname:node_1569,prsc:2,blmd:18,clmp:True|SRC-1116-OUT,DST-1199-OUT;n:type:ShaderForge.SFN_Divide,id:5751,x:35347,y:33747,varname:node_5751,prsc:2;n:type:ShaderForge.SFN_Desaturate,id:8272,x:35140,y:33923,varname:node_8272,prsc:2|COL-2418-OUT;n:type:ShaderForge.SFN_OneMinus,id:5168,x:35312,y:33924,varname:node_5168,prsc:2|IN-8272-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6717,x:35014,y:34175,ptovrint:False,ptlb:node_6717,ptin:_node_6717,varname:node_6717,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ff30ad2400a3a8842bb170ff2a31bf5d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6902,x:35238,y:34175,varname:node_6902,prsc:2,tex:ff30ad2400a3a8842bb170ff2a31bf5d,ntxv:0,isnm:False|UVIN-6042-UVOUT,TEX-6717-TEX;n:type:ShaderForge.SFN_Panner,id:6042,x:35197,y:34306,varname:node_6042,prsc:2,spu:0.5,spv:0.5|UVIN-3107-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3107,x:34955,y:34324,varname:node_3107,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4046,x:35506,y:34219,varname:node_4046,prsc:2|A-6902-RGB,B-1148-OUT;n:type:ShaderForge.SFN_Vector1,id:1148,x:35506,y:34373,varname:node_1148,prsc:2,v1:10;n:type:ShaderForge.SFN_Multiply,id:5797,x:35808,y:34234,varname:node_5797,prsc:2;n:type:ShaderForge.SFN_Vector1,id:4105,x:35498,y:33560,varname:node_4105,prsc:2,v1:4;n:type:ShaderForge.SFN_ComponentMask,id:4717,x:35797,y:34314,varname:node_4717,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1;n:type:ShaderForge.SFN_Blend,id:8084,x:35560,y:33924,varname:node_8084,prsc:2,blmd:13,clmp:True|SRC-5168-OUT,DST-4046-OUT;proporder:1304-9440-9811-6704-2524-6717;pass:END;sub:END;*/

Shader "Shader Forge/BeamShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _node_9440 ("node_9440", 2D) = "white" {}
        _node_9811 ("node_9811", Color) = (0,1,1,1)
        _node_9811_copy ("node_9811_copy", Color) = (0,0.5,1,1)
        _node_2524 ("node_2524", Color) = (10,10,10,1)
        _node_6717 ("node_6717", 2D) = "white" {}
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
            Cull Front
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _node_9440; uniform float4 _node_9440_ST;
            uniform float4 _node_9811;
            uniform float4 _node_9811_copy;
            uniform float4 _node_2524;
            uniform sampler2D _node_6717; uniform float4 _node_6717_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(-v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float node_3745 = 1.0;
                float node_3146_ang = node_3745;
                float node_3146_spd = 1.0;
                float node_3146_cos = cos(node_3146_spd*node_3146_ang);
                float node_3146_sin = sin(node_3146_spd*node_3146_ang);
                float2 node_3146_piv = float2(0.5,0.5);
                float2 node_8662_tc_rcp = float2(1.0,1.0)/float2( node_3745, node_3745 );
                float node_8662_ty = floor(node_3745 * node_8662_tc_rcp.x);
                float node_8662_tx = node_3745 - node_3745 * node_8662_ty;
                float2 node_8662 = (i.uv0 + float2(node_8662_tx, node_8662_ty)) * node_8662_tc_rcp;
                float2 node_3146 = (mul(node_8662-node_3146_piv,float2x2( node_3146_cos, -node_3146_sin, node_3146_sin, node_3146_cos))+node_3146_piv);
                float4 node_9141 = _Time;
                float2 node_965 = (node_3146+(node_8662+node_9141.g*float2(1,2)));
                float4 node_3402 = tex2D(_node_9440,TRANSFORM_TEX(node_965, _node_9440));
                float node_7412 = 3.0;
                float node_2755_ang = node_7412;
                float node_2755_spd = node_7412;
                float node_2755_cos = cos(node_2755_spd*node_2755_ang);
                float node_2755_sin = sin(node_2755_spd*node_2755_ang);
                float2 node_2755_piv = node_965;
                float node_7444 = 0.5;
                float node_227 = 2.0;
                float2 node_2942_tc_rcp = float2(1.0,1.0)/float2( node_7444, node_7444 );
                float node_2942_ty = floor(node_227 * node_2942_tc_rcp.x);
                float node_2942_tx = node_227 - node_7444 * node_2942_ty;
                float2 node_2942 = (i.uv0 + float2(node_2942_tx, node_2942_ty)) * node_2942_tc_rcp;
                float2 node_2755 = (mul(node_2942-node_2755_piv,float2x2( node_2755_cos, -node_2755_sin, node_2755_sin, node_2755_cos))+node_2755_piv);
                float2 node_401 = (node_2755+(node_2942+node_7412*float2(2,2)));
                float4 node_6552 = tex2D(_node_9440,TRANSFORM_TEX(node_401, _node_9440));
                float3 node_356 = saturate((node_3402.rgb+node_6552.rgb-1.0));
                float3 node_140 = (reflect((reflect(node_356,(node_356.r*_Color.rgb + node_356.g*_Color.rgb + node_356.b*node_356))*25.0),(-3.0))*_node_9811.rgb*_node_9811_copy.rgb);
                float3 node_2966 = ((1.0 - node_140)*_node_9811_copy.rgb);
                float3 node_5560 = saturate((node_140+node_2966));
                float3 node_920 = (lerp( lerp( lerp( node_5560, node_2966, node_5560.r ), node_2966, node_5560.g ), node_5560, node_5560.b ));
                float node_8274 = 10.0;
                float3 node_2418 = floor(saturate((0.5 - 2.0*(saturate((node_920+node_140-1.0))-0.5)*(round((node_920*_node_2524.rgb))-0.5))) * node_8274) / (node_8274 - 1);
                float3 normalLocal = node_2418;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////// Lighting:
////// Emissive:
                float2 node_6042 = (i.uv0+node_9141.g*float2(0.5,0.5));
                float4 node_6902 = tex2D(_node_6717,TRANSFORM_TEX(node_6042, _node_6717));
                float3 emissive = saturate(( (1.0 - dot(node_2418,float3(0.3,0.59,0.11))) > 0.5 ? ((node_6902.rgb*10.0)/((1.0-(1.0 - dot(node_2418,float3(0.3,0.59,0.11))))*2.0)) : (1.0-(((1.0-(node_6902.rgb*10.0))*0.5)/(1.0 - dot(node_2418,float3(0.3,0.59,0.11)))))));
                float3 finalColor = emissive;
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
            Cull Front
            
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
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
