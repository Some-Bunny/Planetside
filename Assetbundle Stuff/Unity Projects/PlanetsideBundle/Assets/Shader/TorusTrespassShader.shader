// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33640,y:32823,varname:node_1873,prsc:2|normal-6162-OUT,emission-6162-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:31732,y:32740,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,tex:ff30ad2400a3a8842bb170ff2a31bf5d,ntxv:0,isnm:False|UVIN-79-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1086,x:32469,y:32500,cmnt:RGB,varname:node_1086,prsc:2|A-5921-OUT,B-5983-RGB,C-7918-OUT;n:type:ShaderForge.SFN_Color,id:5983,x:32095,y:33128,ptovrint:False,ptlb:ColorMin,ptin:_ColorMin,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:31895,y:33128,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:32969,y:32585,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-7713-OUT,B-8399-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32133,y:32943,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5983-A,C-5376-A;n:type:ShaderForge.SFN_Slider,id:1261,x:33186,y:33352,ptovrint:False,ptlb:OutlineWidth,ptin:_OutlineWidth,varname:node_1261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.428145,max:10;n:type:ShaderForge.SFN_Vector1,id:2425,x:31502,y:32926,varname:node_2425,prsc:2,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:7918,x:32084,y:32406,ptovrint:False,ptlb:SecondaryPower,ptin:_SecondaryPower,varname:node_7918,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:15;n:type:ShaderForge.SFN_Transform,id:1428,x:31945,y:32733,varname:node_1428,prsc:2,tffrom:1,tfto:1|IN-4805-RGB;n:type:ShaderForge.SFN_Desaturate,id:5921,x:32133,y:32563,varname:node_5921,prsc:2|COL-1428-XYZ;n:type:ShaderForge.SFN_ConstantClamp,id:9755,x:32469,y:32650,varname:node_9755,prsc:2,min:0,max:1|IN-5921-OUT;n:type:ShaderForge.SFN_Multiply,id:1354,x:32716,y:32650,varname:node_1354,prsc:2|A-9755-OUT,B-5983-RGB,C-1711-OUT;n:type:ShaderForge.SFN_Slider,id:1711,x:32503,y:32850,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_1711,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.162527,max:10;n:type:ShaderForge.SFN_Blend,id:8941,x:32469,y:32295,varname:node_8941,prsc:2,blmd:14,clmp:True|SRC-1086-OUT,DST-1354-OUT;n:type:ShaderForge.SFN_Multiply,id:6624,x:32870,y:33003,varname:node_6624,prsc:2|A-8113-OUT,B-3796-OUT,C-8941-OUT;n:type:ShaderForge.SFN_Vector1,id:3796,x:32488,y:33169,varname:node_3796,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Panner,id:9416,x:31288,y:32740,varname:node_9416,prsc:2,spu:0.025,spv:0.025|UVIN-7422-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7422,x:31114,y:32740,varname:node_7422,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:79,x:31502,y:32740,varname:node_79,prsc:2|UVIN-9416-UVOUT,ANG-2425-OUT;n:type:ShaderForge.SFN_OneMinus,id:6487,x:32488,y:33003,varname:node_6487,prsc:2|IN-8941-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:8113,x:32653,y:33003,varname:node_8113,prsc:2,chbt:1|M-6487-OUT,R-5983-RGB,G-5983-B,B-5983-RGB,BTM-6487-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:3254,x:32845,y:33160,varname:node_3254,prsc:2,chbt:0|M-6624-OUT,R-8113-OUT,G-8113-OUT,B-8113-OUT;n:type:ShaderForge.SFN_Blend,id:7713,x:32941,y:32721,varname:node_7713,prsc:2,blmd:8,clmp:False|SRC-8941-OUT,DST-3254-OUT;n:type:ShaderForge.SFN_Multiply,id:6162,x:33019,y:33170,varname:node_6162,prsc:2|A-3254-OUT,B-1280-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8399,x:33063,y:32375,ptovrint:False,ptlb:InnerOrbPower,ptin:_InnerOrbPower,varname:node_8399,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.03;n:type:ShaderForge.SFN_ValueProperty,id:1280,x:32799,y:33418,ptovrint:False,ptlb:OutlinePower,ptin:_OutlinePower,varname:node_1280,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_Tex2d,id:8711,x:33265,y:32500,ptovrint:False,ptlb:node_8711,ptin:_node_8711,varname:node_8711,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ebf0d097488551348b18f0629df47de6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4901,x:33440,y:32125,ptovrint:False,ptlb:EmissionColor_copy,ptin:_EmissionColor_copy,varname:_EmissionColor_copy,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Vector1,id:5926,x:32541,y:33028,varname:node_5926,prsc:2,v1:0;proporder:4805-5983-1261-7918-1711-8399-1280-8711;pass:END;sub:END;*/

Shader "Shader Forge/KillMe" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _ColorMin ("ColorMin", Color) = (0,1,1,1)
        _OutlineWidth ("OutlineWidth", Range(0, 10)) = 2.428145
        _SecondaryPower ("SecondaryPower", Float ) = 15
        _Power ("Power", Range(0, 10)) = 1.162527
        _InnerOrbPower ("InnerOrbPower", Float ) = 0.03
        _OutlinePower ("OutlinePower", Float ) = 0.7
        _node_8711 ("node_8711", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _ColorMin;
            uniform float _SecondaryPower;
            uniform float _Power;
            uniform float _OutlinePower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_79_ang = 0.1;
                float node_79_spd = 1.0;
                float node_79_cos = cos(node_79_spd*node_79_ang);
                float node_79_sin = sin(node_79_spd*node_79_ang);
                float2 node_79_piv = float2(0.5,0.5);
                float4 node_2325 = _Time;
                float2 node_79 = (mul((i.uv0+node_2325.g*float2(0.025,0.025))-node_79_piv,float2x2( node_79_cos, -node_79_sin, node_79_sin, node_79_cos))+node_79_piv);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_79, _MainTex));
                float node_5921 = dot(_MainTex_var.rgb.rgb,float3(0.3,0.59,0.11));
                float3 node_8941 = saturate(( (node_5921*_ColorMin.rgb*_SecondaryPower) > 0.5 ? ((clamp(node_5921,0,1)*_ColorMin.rgb*_Power) + 2.0*(node_5921*_ColorMin.rgb*_SecondaryPower) -1.0) : ((clamp(node_5921,0,1)*_ColorMin.rgb*_Power) + 2.0*((node_5921*_ColorMin.rgb*_SecondaryPower)-0.5))));
                float3 node_6487 = (1.0 - node_8941);
                float3 node_8113 = (lerp( lerp( lerp( node_6487, _ColorMin.rgb, node_6487.r ), _ColorMin.b, node_6487.g ), _ColorMin.rgb, node_6487.b ));
                float3 node_6624 = (node_8113*0.7*node_8941);
                float3 node_3254 = (node_6624.r*node_8113 + node_6624.g*node_8113 + node_6624.b*node_8113);
                float3 node_6162 = (node_3254*_OutlinePower);
                float3 normalLocal = node_6162;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////// Lighting:
////// Emissive:
                float3 emissive = node_6162;
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*_ColorMin.a*i.vertexColor.a));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
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
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
