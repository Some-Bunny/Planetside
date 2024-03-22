// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33644,y:32727,varname:node_1873,prsc:2|normal-9266-RGB,custl-8863-OUT,alpha-4805-A,clip-4805-A;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32599,y:32872,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,tex:99ac56ab4b7a8b648905717d83def091,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:7378,x:32088,y:32654,varname:node_7378,prsc:2,spu:1,spv:1|UVIN-948-UVOUT,DIST-7070-OUT;n:type:ShaderForge.SFN_Rotator,id:948,x:31747,y:32670,varname:node_948,prsc:2|UVIN-4732-UVOUT,ANG-1998-OUT;n:type:ShaderForge.SFN_TexCoord,id:4732,x:31551,y:32670,varname:node_4732,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Tex2dAsset,id:3266,x:32599,y:31983,ptovrint:False,ptlb:WarpTex,ptin:_WarpTex,varname:node_3266,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:2686,x:32088,y:32171,varname:node_2686,prsc:2,spu:-1,spv:1|UVIN-948-UVOUT,DIST-7070-OUT;n:type:ShaderForge.SFN_Panner,id:290,x:32088,y:32334,varname:node_290,prsc:2,spu:1,spv:-1|UVIN-948-UVOUT,DIST-7070-OUT;n:type:ShaderForge.SFN_Panner,id:4209,x:32088,y:32497,varname:node_4209,prsc:2,spu:-1,spv:-1|UVIN-948-UVOUT,DIST-7070-OUT;n:type:ShaderForge.SFN_Tex2d,id:3372,x:32599,y:32557,varname:node_3372,prsc:2,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False|UVIN-3632-OUT,TEX-3266-TEX;n:type:ShaderForge.SFN_Tex2d,id:6174,x:32599,y:32420,varname:node_6174,prsc:2,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False|UVIN-7806-OUT,TEX-3266-TEX;n:type:ShaderForge.SFN_Tex2d,id:3312,x:32599,y:32289,varname:node_3312,prsc:2,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False|UVIN-1592-OUT,TEX-3266-TEX;n:type:ShaderForge.SFN_Tex2d,id:9146,x:32599,y:32166,varname:node_9146,prsc:2,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False|UVIN-7336-OUT,TEX-3266-TEX;n:type:ShaderForge.SFN_Slider,id:1998,x:31344,y:32870,ptovrint:False,ptlb:node_1998,ptin:_node_1998,varname:node_1998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Blend,id:3858,x:32993,y:29811,varname:node_3858,prsc:2,blmd:6,clmp:True|SRC-5599-OUT,DST-4396-OUT;n:type:ShaderForge.SFN_Blend,id:3131,x:32993,y:30139,varname:node_3131,prsc:2,blmd:6,clmp:True|SRC-5599-OUT,DST-6869-OUT;n:type:ShaderForge.SFN_Slider,id:1898,x:31964,y:33166,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_1898,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:184.7946,max:1000;n:type:ShaderForge.SFN_Time,id:7260,x:31501,y:32957,varname:node_7260,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7070,x:31747,y:32852,varname:node_7070,prsc:2|A-7260-TSL,B-9531-OUT;n:type:ShaderForge.SFN_Blend,id:9233,x:32993,y:29973,varname:node_9233,prsc:2,blmd:6,clmp:True|SRC-5599-OUT,DST-2175-OUT;n:type:ShaderForge.SFN_Set,id:3653,x:32987,y:31962,varname:RGBOne,prsc:2|IN-9146-RGB;n:type:ShaderForge.SFN_Set,id:8906,x:32990,y:32024,varname:RGBTwo,prsc:2|IN-3312-RGB;n:type:ShaderForge.SFN_Set,id:5296,x:32987,y:32083,varname:RGBThree,prsc:2|IN-6174-RGB;n:type:ShaderForge.SFN_Set,id:9694,x:32987,y:32141,varname:RGBFour,prsc:2|IN-3372-RGB;n:type:ShaderForge.SFN_Get,id:5599,x:32719,y:29819,varname:node_5599,prsc:2|IN-3653-OUT;n:type:ShaderForge.SFN_Get,id:4396,x:32719,y:29941,varname:node_4396,prsc:2|IN-8906-OUT;n:type:ShaderForge.SFN_Get,id:2175,x:32719,y:30072,varname:node_2175,prsc:2|IN-5296-OUT;n:type:ShaderForge.SFN_Get,id:6869,x:32719,y:30184,varname:node_6869,prsc:2|IN-9694-OUT;n:type:ShaderForge.SFN_Blend,id:9205,x:32993,y:30346,varname:node_9205,prsc:2,blmd:6,clmp:True|SRC-2043-OUT,DST-9320-OUT;n:type:ShaderForge.SFN_Blend,id:3364,x:32993,y:30510,varname:node_3364,prsc:2,blmd:6,clmp:True|SRC-2043-OUT,DST-4454-OUT;n:type:ShaderForge.SFN_Blend,id:6077,x:32993,y:30674,varname:node_6077,prsc:2,blmd:6,clmp:True|SRC-2043-OUT,DST-9559-OUT;n:type:ShaderForge.SFN_Get,id:9320,x:32715,y:30375,varname:node_9320,prsc:2|IN-3653-OUT;n:type:ShaderForge.SFN_Get,id:2043,x:32710,y:30501,varname:node_2043,prsc:2|IN-8906-OUT;n:type:ShaderForge.SFN_Get,id:4454,x:32712,y:30613,varname:node_4454,prsc:2|IN-5296-OUT;n:type:ShaderForge.SFN_Get,id:9559,x:32703,y:30746,varname:node_9559,prsc:2|IN-9694-OUT;n:type:ShaderForge.SFN_Get,id:6373,x:32682,y:30897,varname:node_6373,prsc:2|IN-3653-OUT;n:type:ShaderForge.SFN_Get,id:3390,x:32682,y:30950,varname:node_3390,prsc:2|IN-8906-OUT;n:type:ShaderForge.SFN_Get,id:7295,x:32682,y:31008,varname:node_7295,prsc:2|IN-5296-OUT;n:type:ShaderForge.SFN_Get,id:6703,x:32682,y:31071,varname:node_6703,prsc:2|IN-9694-OUT;n:type:ShaderForge.SFN_Blend,id:1000,x:32976,y:30859,varname:node_1000,prsc:2,blmd:6,clmp:True|SRC-7295-OUT,DST-6373-OUT;n:type:ShaderForge.SFN_Blend,id:9589,x:32962,y:31023,varname:node_9589,prsc:2,blmd:6,clmp:True|SRC-7295-OUT,DST-3390-OUT;n:type:ShaderForge.SFN_Blend,id:503,x:32962,y:31184,varname:node_503,prsc:2,blmd:6,clmp:True|SRC-7295-OUT,DST-6703-OUT;n:type:ShaderForge.SFN_Get,id:6387,x:32763,y:31490,varname:node_6387,prsc:2|IN-3653-OUT;n:type:ShaderForge.SFN_Get,id:4300,x:32760,y:31559,varname:node_4300,prsc:2|IN-8906-OUT;n:type:ShaderForge.SFN_Get,id:351,x:32768,y:31616,varname:node_351,prsc:2|IN-5296-OUT;n:type:ShaderForge.SFN_Get,id:6838,x:32763,y:31674,varname:node_6838,prsc:2|IN-9694-OUT;n:type:ShaderForge.SFN_Blend,id:3943,x:33078,y:31360,varname:node_3943,prsc:2,blmd:6,clmp:True|SRC-6838-OUT,DST-6387-OUT;n:type:ShaderForge.SFN_Blend,id:3371,x:33078,y:31523,varname:node_3371,prsc:2,blmd:6,clmp:True|SRC-6838-OUT,DST-4300-OUT;n:type:ShaderForge.SFN_Blend,id:380,x:33078,y:31691,varname:node_380,prsc:2,blmd:6,clmp:True|SRC-6838-OUT,DST-351-OUT;n:type:ShaderForge.SFN_Set,id:7214,x:34311,y:30677,varname:WhatverTheFuckThisThingIs,prsc:2|IN-3597-OUT;n:type:ShaderForge.SFN_Get,id:3027,x:33169,y:32439,varname:node_3027,prsc:2|IN-7214-OUT;n:type:ShaderForge.SFN_Pi,id:1321,x:31793,y:31771,varname:node_1321,prsc:2;n:type:ShaderForge.SFN_Divide,id:7757,x:32097,y:31707,varname:node_7757,prsc:2|A-1321-OUT,B-7283-OUT;n:type:ShaderForge.SFN_Vector1,id:7283,x:31793,y:31974,varname:node_7283,prsc:2,v1:2;n:type:ShaderForge.SFN_Divide,id:8182,x:32068,y:31878,varname:node_8182,prsc:2|A-1321-OUT;n:type:ShaderForge.SFN_Vector1,id:6483,x:31793,y:31919,varname:node_6483,prsc:2,v1:16;n:type:ShaderForge.SFN_Vector1,id:4612,x:31709,y:32104,varname:node_4612,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:9730,x:31909,y:32070,varname:node_9730,prsc:2;n:type:ShaderForge.SFN_Vector1,id:1803,x:31739,y:33148,varname:node_1803,prsc:2,v1:100;n:type:ShaderForge.SFN_Vector1,id:4065,x:32985,y:32205,varname:node_4065,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Multiply,id:2235,x:33342,y:31373,varname:node_2235,prsc:2|A-3943-OUT,B-3371-OUT,C-380-OUT;n:type:ShaderForge.SFN_Multiply,id:9021,x:33331,y:30863,varname:node_9021,prsc:2|A-1000-OUT,B-9589-OUT,C-503-OUT;n:type:ShaderForge.SFN_Multiply,id:1492,x:33328,y:30058,varname:node_1492,prsc:2|A-3858-OUT,B-9233-OUT,C-3131-OUT;n:type:ShaderForge.SFN_Multiply,id:9656,x:33280,y:30310,varname:node_9656,prsc:2|A-9205-OUT,B-3364-OUT,C-6077-OUT;n:type:ShaderForge.SFN_Desaturate,id:8219,x:33535,y:30453,varname:node_8219,prsc:2|COL-9656-OUT,DES-3416-OUT;n:type:ShaderForge.SFN_Desaturate,id:1097,x:33551,y:30199,varname:node_1097,prsc:2|COL-1492-OUT,DES-3416-OUT;n:type:ShaderForge.SFN_Desaturate,id:6933,x:33573,y:31008,varname:node_6933,prsc:2|COL-2235-OUT,DES-3416-OUT;n:type:ShaderForge.SFN_Desaturate,id:9861,x:33598,y:30725,varname:node_9861,prsc:2|COL-9021-OUT,DES-3416-OUT;n:type:ShaderForge.SFN_Vector1,id:3416,x:33312,y:30638,varname:node_3416,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:3597,x:34152,y:30528,varname:node_3597,prsc:2|A-1013-OUT,B-5440-OUT;n:type:ShaderForge.SFN_Add,id:1013,x:33817,y:30281,varname:node_1013,prsc:2|A-1097-OUT,B-8219-OUT;n:type:ShaderForge.SFN_Add,id:5440,x:33826,y:30725,varname:node_5440,prsc:2|A-9861-OUT,B-6933-OUT;n:type:ShaderForge.SFN_Multiply,id:7336,x:32336,y:32171,varname:node_7336,prsc:2|A-2686-UVOUT,B-4860-OUT;n:type:ShaderForge.SFN_Multiply,id:1592,x:32336,y:32311,varname:node_1592,prsc:2|A-290-UVOUT,B-4860-OUT;n:type:ShaderForge.SFN_Multiply,id:7806,x:32336,y:32486,varname:node_7806,prsc:2|A-4209-UVOUT,B-4860-OUT;n:type:ShaderForge.SFN_Multiply,id:3632,x:32336,y:32618,varname:node_3632,prsc:2|A-7378-UVOUT,B-4860-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4860,x:31560,y:32438,ptovrint:False,ptlb:Tiles,ptin:_Tiles,varname:node_4860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Divide,id:9531,x:31739,y:32983,varname:node_9531,prsc:2|A-1898-OUT,B-1803-OUT;n:type:ShaderForge.SFN_Multiply,id:8863,x:33266,y:32760,varname:node_8863,prsc:2|A-3597-OUT,B-9266-RGB;n:type:ShaderForge.SFN_Tex2d,id:9266,x:32599,y:32695,varname:node_9266,prsc:2,tex:ec6e0e0873659fd4bb7610649cfb1e22,ntxv:0,isnm:False|TEX-3266-TEX;proporder:4805-3266-1998-1898-4860;pass:END;sub:END;*/

Shader "Shader Forge/WarpedShader" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _WarpTex ("WarpTex", 2D) = "white" {}
        _node_1998 ("node_1998", Range(0, 1)) = 0
        _Speed ("Speed", Range(0, 1000)) = 184.7946
        _Tiles ("Tiles", Float ) = 1
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _WarpTex; uniform float4 _WarpTex_ST;
            uniform float _node_1998;
            uniform float _Speed;
            uniform float _Tiles;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float4 node_9266 = tex2D(_WarpTex,TRANSFORM_TEX(i.uv0, _WarpTex));
                float3 normalLocal = node_9266.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(_MainTex_var.a - 0.5);
////// Lighting:
                float4 node_7260 = _Time;
                float node_7070 = (node_7260.r*(_Speed/100.0));
                float node_948_ang = _node_1998;
                float node_948_spd = 1.0;
                float node_948_cos = cos(node_948_spd*node_948_ang);
                float node_948_sin = sin(node_948_spd*node_948_ang);
                float2 node_948_piv = float2(0.5,0.5);
                float2 node_948 = (mul(i.uv0-node_948_piv,float2x2( node_948_cos, -node_948_sin, node_948_sin, node_948_cos))+node_948_piv);
                float2 node_7336 = ((node_948+node_7070*float2(-1,1))*_Tiles);
                float4 node_9146 = tex2D(_WarpTex,TRANSFORM_TEX(node_7336, _WarpTex));
                float3 RGBOne = node_9146.rgb;
                float3 node_5599 = RGBOne;
                float2 node_1592 = ((node_948+node_7070*float2(1,-1))*_Tiles);
                float4 node_3312 = tex2D(_WarpTex,TRANSFORM_TEX(node_1592, _WarpTex));
                float3 RGBTwo = node_3312.rgb;
                float2 node_7806 = ((node_948+node_7070*float2(-1,-1))*_Tiles);
                float4 node_6174 = tex2D(_WarpTex,TRANSFORM_TEX(node_7806, _WarpTex));
                float3 RGBThree = node_6174.rgb;
                float2 node_3632 = ((node_948+node_7070*float2(1,1))*_Tiles);
                float4 node_3372 = tex2D(_WarpTex,TRANSFORM_TEX(node_3632, _WarpTex));
                float3 RGBFour = node_3372.rgb;
                float node_3416 = 0.7;
                float3 node_2043 = RGBTwo;
                float3 node_7295 = RGBThree;
                float3 node_6838 = RGBFour;
                float node_3597 = ((lerp((saturate((1.0-(1.0-node_5599)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_5599)*(1.0-RGBThree)))*saturate((1.0-(1.0-node_5599)*(1.0-RGBFour)))),dot((saturate((1.0-(1.0-node_5599)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_5599)*(1.0-RGBThree)))*saturate((1.0-(1.0-node_5599)*(1.0-RGBFour)))),float3(0.3,0.59,0.11)),node_3416)+lerp((saturate((1.0-(1.0-node_2043)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_2043)*(1.0-RGBThree)))*saturate((1.0-(1.0-node_2043)*(1.0-RGBFour)))),dot((saturate((1.0-(1.0-node_2043)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_2043)*(1.0-RGBThree)))*saturate((1.0-(1.0-node_2043)*(1.0-RGBFour)))),float3(0.3,0.59,0.11)),node_3416))*(lerp((saturate((1.0-(1.0-node_7295)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_7295)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_7295)*(1.0-RGBFour)))),dot((saturate((1.0-(1.0-node_7295)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_7295)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_7295)*(1.0-RGBFour)))),float3(0.3,0.59,0.11)),node_3416)+lerp((saturate((1.0-(1.0-node_6838)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_6838)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_6838)*(1.0-RGBThree)))),dot((saturate((1.0-(1.0-node_6838)*(1.0-RGBOne)))*saturate((1.0-(1.0-node_6838)*(1.0-RGBTwo)))*saturate((1.0-(1.0-node_6838)*(1.0-RGBThree)))),float3(0.3,0.59,0.11)),node_3416)));
                float3 finalColor = (node_3597*node_9266.rgb);
                return fixed4(finalColor,_MainTex_var.a);
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
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
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
