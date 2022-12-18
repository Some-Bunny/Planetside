// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Standard,iptp:1,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:True,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33523,y:32807,varname:node_1873,prsc:2|normal-3042-OUT,emission-3042-OUT,custl-3042-OUT,alpha-9570-A,clip-9570-A;n:type:ShaderForge.SFN_Time,id:8713,x:30264,y:32655,varname:node_8713,prsc:2;n:type:ShaderForge.SFN_Panner,id:8550,x:30731,y:32644,varname:node_8550,prsc:2,spu:1,spv:1|UVIN-8994-UVOUT,DIST-8986-OUT;n:type:ShaderForge.SFN_TexCoord,id:9131,x:30146,y:32932,varname:node_9131,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:7518,x:31578,y:32208,ptovrint:False,ptlb:node_5132,ptin:_node_5132,varname:node_7518,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7991,x:31740,y:32190,ptovrint:False,ptlb:node_8861,ptin:_node_8861,varname:_node_7518_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:4824,x:30731,y:32462,varname:node_4824,prsc:2,spu:-1,spv:-1|UVIN-8994-UVOUT,DIST-8986-OUT;n:type:ShaderForge.SFN_Panner,id:6777,x:30732,y:32804,varname:node_6777,prsc:2,spu:-1,spv:1|UVIN-8994-UVOUT,DIST-8986-OUT;n:type:ShaderForge.SFN_Panner,id:8876,x:30732,y:32943,varname:node_8876,prsc:2,spu:1,spv:-1|UVIN-8994-UVOUT,DIST-8986-OUT;n:type:ShaderForge.SFN_Tex2d,id:1547,x:31587,y:32807,varname:node_1547,prsc:2,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False|UVIN-1750-OUT,TEX-6393-TEX;n:type:ShaderForge.SFN_Tex2d,id:7033,x:31587,y:32964,varname:node_7033,prsc:2,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False|UVIN-3832-OUT,TEX-6393-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3735,x:31904,y:33361,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_3735,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:3,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:3701,x:30732,y:33034,ptovrint:False,ptlb:node_8593,ptin:_node_8593,varname:node_8593,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ValueProperty,id:2887,x:31058,y:33201,ptovrint:False,ptlb:ScaleAdjustment,ptin:_ScaleAdjustment,varname:node_4829,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Slider,id:4721,x:30097,y:32797,ptovrint:False,ptlb:ScrollSpeed,ptin:_ScrollSpeed,varname:node_4721,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:2.045288,max:5;n:type:ShaderForge.SFN_Multiply,id:8986,x:30254,y:32526,varname:node_8986,prsc:2|A-8713-TSL,B-4721-OUT;n:type:ShaderForge.SFN_Rotator,id:8994,x:30388,y:32932,varname:node_8994,prsc:2|UVIN-9131-UVOUT,ANG-2962-OUT,SPD-7622-OUT;n:type:ShaderForge.SFN_Slider,id:2780,x:29989,y:33411,ptovrint:False,ptlb:TextureCellRotation,ptin:_TextureCellRotation,varname:node_2780,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Color,id:8888,x:32613,y:32481,ptovrint:False,ptlb:OverrideColor,ptin:_OverrideColor,varname:node_8888,prsc:2,glob:False,taghide:True,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_RemapRange,id:304,x:32658,y:32221,varname:node_304,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-6550-OUT;n:type:ShaderForge.SFN_Multiply,id:3590,x:33112,y:32233,varname:node_3590,prsc:2|A-304-OUT,B-8888-RGB;n:type:ShaderForge.SFN_ValueProperty,id:2962,x:29769,y:33189,ptovrint:False,ptlb:No,ptin:_No,varname:node_2962,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Divide,id:1950,x:30388,y:33276,varname:node_1950,prsc:2|A-2780-OUT,B-3974-OUT;n:type:ShaderForge.SFN_Vector1,id:3974,x:30146,y:33492,varname:node_3974,prsc:2,v1:0.3181734;n:type:ShaderForge.SFN_Time,id:7248,x:29769,y:33275,varname:node_7248,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7622,x:30388,y:33101,varname:node_7622,prsc:2|A-376-OUT,B-1950-OUT;n:type:ShaderForge.SFN_Tex2d,id:9570,x:31904,y:33172,varname:node_9570,prsc:2,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:0,isnm:False|TEX-3735-TEX;n:type:ShaderForge.SFN_If,id:3042,x:32914,y:32779,varname:node_3042,prsc:2|A-3524-OUT,B-6420-OUT,GT-2777-OUT,EQ-3590-OUT,LT-2777-OUT;n:type:ShaderForge.SFN_Vector1,id:6420,x:32886,y:32598,varname:node_6420,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:3524,x:32902,y:32686,ptovrint:False,ptlb:IsColorOverride,ptin:_IsColorOverride,varname:node_3524,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_If,id:376,x:30146,y:33195,varname:node_376,prsc:2|A-2962-OUT,B-4864-OUT,GT-2962-OUT,EQ-7248-TSL,LT-2962-OUT;n:type:ShaderForge.SFN_Slider,id:4864,x:29742,y:32963,ptovrint:False,ptlb:TextureCellRotationTime,ptin:_TextureCellRotationTime,varname:node_4864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Desaturate,id:6550,x:32370,y:32470,varname:node_6550,prsc:2|COL-2777-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:5691,x:31936,y:32661,varname:node_5691,prsc:2,chbt:1|M-1015-RGB,R-4328-RGB,G-6576-RGB,B-4282-B,BTM-3933-OUT;n:type:ShaderForge.SFN_Add,id:2777,x:32299,y:32790,varname:node_2777,prsc:2|A-5691-OUT,B-9090-OUT;n:type:ShaderForge.SFN_Tex2d,id:1015,x:31595,y:33133,varname:node_1015,prsc:2,tex:ae9befa7c76985041a98c095f99a89d6,ntxv:0,isnm:False|TEX-9767-TEX;n:type:ShaderForge.SFN_Rotator,id:3311,x:31258,y:33453,varname:node_3311,prsc:2|UVIN-6604-UVOUT,ANG-1806-OUT;n:type:ShaderForge.SFN_TexCoord,id:6604,x:31001,y:33450,varname:node_6604,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:9767,x:31243,y:33603,ptovrint:False,ptlb:ColorMap,ptin:_ColorMap,varname:node_9767,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ae9befa7c76985041a98c095f99a89d6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9660,x:31501,y:33564,varname:node_9660,prsc:2,tex:ae9befa7c76985041a98c095f99a89d6,ntxv:0,isnm:False|UVIN-3311-UVOUT,TEX-9767-TEX;n:type:ShaderForge.SFN_ChannelBlend,id:9090,x:31936,y:33000,varname:node_9090,prsc:2,chbt:1|M-9660-RGB,R-1547-RGB,G-7033-RGB,B-4282-B,BTM-8157-OUT;n:type:ShaderForge.SFN_Vector1,id:1806,x:31040,y:33638,varname:node_1806,prsc:2,v1:4.71;n:type:ShaderForge.SFN_Color,id:4282,x:31936,y:32835,ptovrint:False,ptlb:node_4282,ptin:_node_4282,varname:node_4282,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:6393,x:30931,y:32113,ptovrint:False,ptlb:OverlayTexture,ptin:_OverlayTexture,varname:node_6393,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9288,x:30882,y:32943,varname:node_9288,prsc:2|A-8876-UVOUT,B-2887-OUT;n:type:ShaderForge.SFN_Multiply,id:8915,x:30882,y:32804,varname:node_8915,prsc:2|A-6777-UVOUT,B-2887-OUT;n:type:ShaderForge.SFN_Multiply,id:1569,x:30894,y:32644,varname:node_1569,prsc:2|A-8550-UVOUT,B-2887-OUT;n:type:ShaderForge.SFN_Multiply,id:2313,x:30884,y:32462,varname:node_2313,prsc:2|A-4824-UVOUT,B-2887-OUT;n:type:ShaderForge.SFN_Pi,id:2763,x:31199,y:33286,varname:node_2763,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:4328,x:31587,y:32523,varname:node_4328,prsc:2,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False|UVIN-3284-OUT,TEX-6393-TEX;n:type:ShaderForge.SFN_TexCoord,id:5556,x:30931,y:32269,varname:node_5556,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3284,x:31119,y:32462,varname:node_3284,prsc:2|A-2313-OUT,B-5556-UVOUT;n:type:ShaderForge.SFN_Add,id:6917,x:31119,y:32644,varname:node_6917,prsc:2|A-1569-OUT,B-5556-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:6576,x:31587,y:32661,varname:node_6576,prsc:2,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False|UVIN-6917-OUT,TEX-6393-TEX;n:type:ShaderForge.SFN_Add,id:1750,x:31119,y:32804,varname:node_1750,prsc:2|A-8915-OUT,B-5556-UVOUT;n:type:ShaderForge.SFN_Add,id:3832,x:31119,y:32943,varname:node_3832,prsc:2|A-9288-OUT,B-5556-UVOUT;n:type:ShaderForge.SFN_Desaturate,id:502,x:32463,y:33532,varname:node_502,prsc:2;n:type:ShaderForge.SFN_Blend,id:8157,x:31670,y:33737,varname:node_8157,prsc:2,blmd:17,clmp:True|SRC-7029-OUT,DST-8102-OUT;n:type:ShaderForge.SFN_Round,id:8102,x:31482,y:33875,varname:node_8102,prsc:2|IN-6604-U;n:type:ShaderForge.SFN_Round,id:7029,x:31482,y:33737,varname:node_7029,prsc:2|IN-6604-V;n:type:ShaderForge.SFN_Rotator,id:1602,x:32429,y:33651,varname:node_1602,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:3933,x:31857,y:33755,varname:node_3933,prsc:2|IN-8157-OUT;proporder:3735-4721-2780-8888-2962-3524-4864-9767-4282-6393-2887-7518-7991;pass:END;sub:END;*/

Shader "Shader Forge/WarpScroll" {
    Properties {
        _MainTex ("MainTex", 2D) = "bump" {}
        _ScrollSpeed ("ScrollSpeed", Range(-5, 5)) = 2.045288
        _TextureCellRotation ("TextureCellRotation", Range(0, 10)) = 0
        [HideInInspector][HDR]_OverrideColor ("OverrideColor", Color) = (1,0,0,1)
        _No ("No", Float ) = 1
        _IsColorOverride ("IsColorOverride", Range(0, 1)) = 0
        _TextureCellRotationTime ("TextureCellRotationTime", Range(0, 1)) = 0
        _ColorMap ("ColorMap", 2D) = "white" {}
        _node_4282 ("node_4282", Color) = (0,0,1,1)
        _OverlayTexture ("OverlayTexture", 2D) = "bump" {}
        _ScaleAdjustment ("ScaleAdjustment", Float ) = 1
        _node_5132 ("node_5132", 2D) = "white" {}
        _node_8861 ("node_8861", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            "PreviewType"="Plane"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
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
            AlphaToMask On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _ScaleAdjustment;
            uniform float _ScrollSpeed;
            uniform float _TextureCellRotation;
            uniform float4 _OverrideColor;
            uniform float _No;
            uniform float _IsColorOverride;
            uniform float _TextureCellRotationTime;
            uniform sampler2D _ColorMap; uniform float4 _ColorMap_ST;
            uniform float4 _node_4282;
            uniform sampler2D _OverlayTexture; uniform float4 _OverlayTexture_ST;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float node_3042_if_leA = step(_IsColorOverride,1.0);
                float node_3042_if_leB = step(1.0,_IsColorOverride);
                float4 node_1015 = tex2D(_ColorMap,TRANSFORM_TEX(i.uv0, _ColorMap));
                float node_8157 = saturate(abs(round(i.uv0.g)-round(i.uv0.r)));
                float node_3933 = (1.0 - node_8157);
                float4 node_8713 = _Time;
                float node_8986 = (node_8713.r*_ScrollSpeed);
                float node_376_if_leA = step(_No,_TextureCellRotationTime);
                float node_376_if_leB = step(_TextureCellRotationTime,_No);
                float4 node_7248 = _Time;
                float node_8994_ang = _No;
                float node_8994_spd = (lerp((node_376_if_leA*_No)+(node_376_if_leB*_No),node_7248.r,node_376_if_leA*node_376_if_leB)*(_TextureCellRotation/0.3181734));
                float node_8994_cos = cos(node_8994_spd*node_8994_ang);
                float node_8994_sin = sin(node_8994_spd*node_8994_ang);
                float2 node_8994_piv = float2(0.5,0.5);
                float2 node_8994 = (mul(i.uv0-node_8994_piv,float2x2( node_8994_cos, -node_8994_sin, node_8994_sin, node_8994_cos))+node_8994_piv);
                float2 node_3284 = (((node_8994+node_8986*float2(-1,-1))*_ScaleAdjustment)+i.uv0);
                float4 node_4328 = tex2D(_OverlayTexture,TRANSFORM_TEX(node_3284, _OverlayTexture));
                float2 node_6917 = (((node_8994+node_8986*float2(1,1))*_ScaleAdjustment)+i.uv0);
                float4 node_6576 = tex2D(_OverlayTexture,TRANSFORM_TEX(node_6917, _OverlayTexture));
                float node_3311_ang = 4.71;
                float node_3311_spd = 1.0;
                float node_3311_cos = cos(node_3311_spd*node_3311_ang);
                float node_3311_sin = sin(node_3311_spd*node_3311_ang);
                float2 node_3311_piv = float2(0.5,0.5);
                float2 node_3311 = (mul(i.uv0-node_3311_piv,float2x2( node_3311_cos, -node_3311_sin, node_3311_sin, node_3311_cos))+node_3311_piv);
                float4 node_9660 = tex2D(_ColorMap,TRANSFORM_TEX(node_3311, _ColorMap));
                float2 node_1750 = (((node_8994+node_8986*float2(-1,1))*_ScaleAdjustment)+i.uv0);
                float4 node_1547 = tex2D(_OverlayTexture,TRANSFORM_TEX(node_1750, _OverlayTexture));
                float2 node_3832 = (((node_8994+node_8986*float2(1,-1))*_ScaleAdjustment)+i.uv0);
                float4 node_7033 = tex2D(_OverlayTexture,TRANSFORM_TEX(node_3832, _OverlayTexture));
                float3 node_2777 = ((lerp( lerp( lerp( float3(node_3933,node_3933,node_3933), node_4328.rgb, node_1015.rgb.r ), node_6576.rgb, node_1015.rgb.g ), _node_4282.b, node_1015.rgb.b ))+(lerp( lerp( lerp( float3(node_8157,node_8157,node_8157), node_1547.rgb, node_9660.rgb.r ), node_7033.rgb, node_9660.rgb.g ), _node_4282.b, node_9660.rgb.b )));
                float3 node_3042 = lerp((node_3042_if_leA*node_2777)+(node_3042_if_leB*node_2777),((dot(node_2777,float3(0.3,0.59,0.11))*-1.0+1.0)*_OverrideColor.rgb),node_3042_if_leA*node_3042_if_leB);
                float3 normalLocal = node_3042;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 node_9570 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(node_9570.a - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = node_3042;
                float3 finalColor = emissive + node_3042;
                return fixed4(finalColor,node_9570.a);
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
                float4 node_9570 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(node_9570.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
