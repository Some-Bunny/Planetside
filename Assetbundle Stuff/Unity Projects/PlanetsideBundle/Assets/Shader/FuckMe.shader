// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:1,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:34225,y:32442,varname:node_1873,prsc:2|emission-2994-OUT,alpha-8699-OUT,clip-8699-OUT,olcol-1775-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32587,y:32890,varname:_MainTex_copy,prsc:2,tex:6e3ff783bb8d205488f3af1b4723c401,ntxv:0,isnm:False|UVIN-447-UVOUT,TEX-3252-TEX;n:type:ShaderForge.SFN_Multiply,id:1086,x:33218,y:32909,cmnt:RGB,varname:node_1086,prsc:2|A-4805-RGB,B-2100-OUT;n:type:ShaderForge.SFN_VertexColor,id:5376,x:33286,y:32456,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:4598,x:32873,y:32419,varname:node_4598,prsc:2|EXP-2590-OUT;n:type:ShaderForge.SFN_Multiply,id:9639,x:33519,y:33127,varname:node_9639,prsc:2|A-9735-OUT,B-5376-A;n:type:ShaderForge.SFN_Tex2dAsset,id:3252,x:32128,y:33088,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_3252,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6e3ff783bb8d205488f3af1b4723c401,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9735,x:33218,y:32778,varname:node_9735,prsc:2|A-4805-A,B-6289-OUT,C-1086-OUT,D-8628-OUT,E-3759-OUT;n:type:ShaderForge.SFN_Multiply,id:979,x:33733,y:32449,varname:node_979,prsc:2|A-4598-OUT,B-6774-OUT;n:type:ShaderForge.SFN_Vector1,id:6774,x:33155,y:32174,varname:node_6774,prsc:2,v1:2;n:type:ShaderForge.SFN_Slider,id:4817,x:32403,y:33144,ptovrint:False,ptlb:Light,ptin:_Light,varname:node_4817,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Subtract,id:55,x:33733,y:32179,varname:node_55,prsc:2|A-8628-OUT,B-5376-A;n:type:ShaderForge.SFN_Slider,id:8338,x:32516,y:32348,ptovrint:False,ptlb:HoleSize,ptin:_HoleSize,varname:node_8338,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:20,max:100;n:type:ShaderForge.SFN_Subtract,id:1775,x:33733,y:33127,varname:node_1775,prsc:2|A-1086-OUT,B-9639-OUT;n:type:ShaderForge.SFN_Ceil,id:3759,x:33733,y:32318,varname:node_3759,prsc:2|IN-55-OUT;n:type:ShaderForge.SFN_UVTile,id:4641,x:32319,y:32674,varname:node_4641,prsc:2|WDT-8405-OUT,HGT-8405-OUT,TILE-8405-OUT;n:type:ShaderForge.SFN_Panner,id:447,x:32319,y:32806,varname:node_447,prsc:2,spu:1,spv:1|UVIN-4641-UVOUT,DIST-558-OUT;n:type:ShaderForge.SFN_Time,id:3036,x:31933,y:32831,varname:node_3036,prsc:2;n:type:ShaderForge.SFN_Slider,id:8519,x:31776,y:32753,ptovrint:False,ptlb:Spinspeed,ptin:_Spinspeed,varname:node_8519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.15,max:1;n:type:ShaderForge.SFN_Multiply,id:558,x:32128,y:32806,varname:node_558,prsc:2|A-8519-OUT,B-3036-T;n:type:ShaderForge.SFN_ValueProperty,id:8494,x:31933,y:32605,ptovrint:False,ptlb:Tiles,ptin:_Tiles,varname:node_8494,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Divide,id:8405,x:32128,y:32675,varname:node_8405,prsc:2|A-5546-OUT,B-8494-OUT;n:type:ShaderForge.SFN_Vector1,id:5546,x:31933,y:32675,varname:node_5546,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:2100,x:32695,y:32753,ptovrint:False,ptlb:ColorMultiplier,ptin:_ColorMultiplier,varname:node_2100,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.700855,max:10;n:type:ShaderForge.SFN_OneMinus,id:6289,x:32506,y:33243,varname:node_6289,prsc:2|IN-4817-OUT;n:type:ShaderForge.SFN_Divide,id:2590,x:32638,y:32456,varname:node_2590,prsc:2|A-8338-OUT,B-3554-OUT;n:type:ShaderForge.SFN_Vector1,id:3554,x:32638,y:32602,varname:node_3554,prsc:2,v1:50;n:type:ShaderForge.SFN_Multiply,id:9567,x:33960,y:33127,varname:node_9567,prsc:2|A-1775-OUT,B-4302-OUT;n:type:ShaderForge.SFN_Slider,id:4302,x:33803,y:33344,ptovrint:False,ptlb:EmissionMultiplier,ptin:_EmissionMultiplier,varname:node_4302,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.86873,max:5;n:type:ShaderForge.SFN_Noise,id:9349,x:32924,y:31907,varname:node_9349,prsc:2|XY-9281-OUT;n:type:ShaderForge.SFN_Add,id:8628,x:33733,y:32048,varname:node_8628,prsc:2|A-5798-OUT,B-4598-OUT;n:type:ShaderForge.SFN_Multiply,id:5798,x:33405,y:32041,varname:node_5798,prsc:2|A-9349-OUT,B-4900-OUT,C-5579-OUT;n:type:ShaderForge.SFN_Divide,id:5579,x:33155,y:32041,varname:node_5579,prsc:2|A-979-OUT,B-6774-OUT;n:type:ShaderForge.SFN_Rotator,id:5446,x:32587,y:31907,varname:node_5446,prsc:2|UVIN-4641-UVOUT,ANG-5386-OUT,SPD-5386-OUT;n:type:ShaderForge.SFN_Vector1,id:5386,x:32557,y:32203,varname:node_5386,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:9281,x:32759,y:31907,varname:node_9281,prsc:2|A-5446-UVOUT,B-5446-UVOUT;n:type:ShaderForge.SFN_Vector1,id:4900,x:33405,y:31961,varname:node_4900,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Add,id:2994,x:33723,y:32638,varname:node_2994,prsc:2|A-3927-OUT,B-9567-OUT;n:type:ShaderForge.SFN_Multiply,id:3927,x:33527,y:32638,varname:node_3927,prsc:2|A-8164-OUT,B-4805-B,C-1787-RGB;n:type:ShaderForge.SFN_Vector1,id:8164,x:33527,y:32553,varname:node_8164,prsc:2,v1:3;n:type:ShaderForge.SFN_Color,id:1787,x:33218,y:32638,ptovrint:False,ptlb:Bluue,ptin:_Bluue,varname:node_1787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:4875,x:33024,y:32323,varname:node_4875,prsc:2|A-4598-OUT,B-5386-OUT;n:type:ShaderForge.SFN_OneMinus,id:2660,x:33179,y:32323,varname:node_2660,prsc:2|IN-4875-OUT;n:type:ShaderForge.SFN_Add,id:8699,x:33890,y:32496,varname:node_8699,prsc:2|A-979-OUT,B-2660-OUT;proporder:3252-4817-8338-8519-8494-2100-4302-1787;pass:END;sub:END;*/

Shader "Shader Forge/FuckMe" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Light ("Light", Range(0, 1)) = 1
        _HoleSize ("HoleSize", Range(0, 100)) = 20
        _Spinspeed ("Spinspeed", Range(-1, 1)) = 0.15
        _Tiles ("Tiles", Float ) = 3
        _ColorMultiplier ("ColorMultiplier", Range(0, 10)) = 4.700855
        _EmissionMultiplier ("EmissionMultiplier", Range(0, 5)) = 1.86873
        _Bluue ("Bluue", Color) = (0,1,1,1)
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
            Cull Front
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
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Light;
            uniform float _HoleSize;
            uniform float _Spinspeed;
            uniform float _Tiles;
            uniform float _ColorMultiplier;
            uniform float _EmissionMultiplier;
            uniform float4 _Bluue;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(-v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_4598 = pow(1.0-max(0,dot(normalDirection, viewDirection)),(_HoleSize/50.0));
                float node_6774 = 2.0;
                float node_979 = (node_4598*node_6774);
                float node_5386 = 0.25;
                float node_8699 = (node_979+(1.0 - step(node_4598,node_5386)));
                clip(node_8699 - 0.5);
////// Lighting:
////// Emissive:
                float4 node_3036 = _Time;
                float node_8405 = (1.0/_Tiles);
                float2 node_4641_tc_rcp = float2(1.0,1.0)/float2( node_8405, node_8405 );
                float node_4641_ty = floor(node_8405 * node_4641_tc_rcp.x);
                float node_4641_tx = node_8405 - node_8405 * node_4641_ty;
                float2 node_4641 = (i.uv0 + float2(node_4641_tx, node_4641_ty)) * node_4641_tc_rcp;
                float2 node_447 = (node_4641+(_Spinspeed*node_3036.g)*float2(1,1));
                float4 _MainTex_copy = tex2D(_Texture,TRANSFORM_TEX(node_447, _Texture));
                float3 node_1086 = (_MainTex_copy.rgb*_ColorMultiplier); // RGB
                float node_5446_ang = node_5386;
                float node_5446_spd = node_5386;
                float node_5446_cos = cos(node_5446_spd*node_5446_ang);
                float node_5446_sin = sin(node_5446_spd*node_5446_ang);
                float2 node_5446_piv = float2(0.5,0.5);
                float2 node_5446 = (mul(node_4641-node_5446_piv,float2x2( node_5446_cos, -node_5446_sin, node_5446_sin, node_5446_cos))+node_5446_piv);
                float2 node_9281 = (node_5446*node_5446);
                float2 node_9349_skew = node_9281 + 0.2127+node_9281.x*0.3713*node_9281.y;
                float2 node_9349_rnd = 4.789*sin(489.123*(node_9349_skew));
                float node_9349 = frac(node_9349_rnd.x*node_9349_rnd.y*(1+node_9349_skew.x));
                float node_8628 = ((node_9349*0.15*(node_979/node_6774))+node_4598);
                float3 node_1775 = (node_1086-((_MainTex_copy.a*(1.0 - _Light)*node_1086*node_8628*ceil((node_8628-i.vertexColor.a)))*i.vertexColor.a));
                float3 emissive = ((3.0*_MainTex_copy.b*_Bluue.rgb)+(node_1775*_EmissionMultiplier));
                float3 finalColor = emissive;
                return fixed4(finalColor,node_8699);
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _HoleSize;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(-v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_4598 = pow(1.0-max(0,dot(normalDirection, viewDirection)),(_HoleSize/50.0));
                float node_6774 = 2.0;
                float node_979 = (node_4598*node_6774);
                float node_5386 = 0.25;
                float node_8699 = (node_979+(1.0 - step(node_4598,node_5386)));
                clip(node_8699 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
