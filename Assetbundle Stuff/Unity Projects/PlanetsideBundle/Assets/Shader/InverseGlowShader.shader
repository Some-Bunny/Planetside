// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:577,x:34153,y:32318,varname:node_577,prsc:2|normal-5288-OUT,emission-5288-OUT,custl-5288-OUT,alpha-5619-A,clip-5619-A;n:type:ShaderForge.SFN_Tex2d,id:5619,x:31526,y:32124,varname:node_5619,prsc:2,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:0,isnm:False|TEX-2511-TEX;n:type:ShaderForge.SFN_Subtract,id:8956,x:32118,y:31950,varname:node_8956,prsc:2|A-7675-OUT,B-668-OUT;n:type:ShaderForge.SFN_OneMinus,id:9717,x:33519,y:32202,varname:node_9717,prsc:2|IN-8354-OUT;n:type:ShaderForge.SFN_Slider,id:6059,x:31961,y:31763,ptovrint:False,ptlb:BlackBullet,ptin:_BlackBullet,varname:node_6059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:0;n:type:ShaderForge.SFN_ValueProperty,id:668,x:32118,y:31856,ptovrint:False,ptlb:fuzziness,ptin:_fuzziness,varname:node_1964,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.4;n:type:ShaderForge.SFN_Desaturate,id:9502,x:31794,y:31950,varname:node_9502,prsc:2|COL-5619-RGB;n:type:ShaderForge.SFN_Floor,id:4673,x:32282,y:31950,varname:node_4673,prsc:2|IN-8956-OUT;n:type:ShaderForge.SFN_Blend,id:8828,x:32824,y:32176,varname:node_8828,prsc:2,blmd:1,clmp:True|SRC-6921-OUT,DST-5619-RGB;n:type:ShaderForge.SFN_OneMinus,id:7675,x:31951,y:31950,varname:node_7675,prsc:2|IN-9502-OUT;n:type:ShaderForge.SFN_If,id:6921,x:32626,y:32176,varname:node_6921,prsc:2|A-6597-OUT,B-6059-OUT,GT-4673-OUT,EQ-5619-RGB,LT-5619-RGB;n:type:ShaderForge.SFN_Vector1,id:6597,x:32118,y:32089,varname:node_6597,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7286,x:33034,y:32121,ptovrint:False,ptlb:node_7286,ptin:_node_7286,varname:node_7286,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Power,id:8354,x:33272,y:32184,varname:node_8354,prsc:2|VAL-8828-OUT,EXP-7286-OUT;n:type:ShaderForge.SFN_Slider,id:2197,x:34167,y:32933,ptovrint:False,ptlb:EmissivePower,ptin:_EmissivePower,varname:node_2197,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-100,cur:10,max:100;n:type:ShaderForge.SFN_Tex2dAsset,id:2511,x:31359,y:32163,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2511,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1249,x:33212,y:32377,varname:node_1249,prsc:2|A-9717-OUT,B-9019-OUT;n:type:ShaderForge.SFN_Slider,id:9019,x:32536,y:32447,ptovrint:False,ptlb:EmissiveColorPower,ptin:_EmissiveColorPower,varname:node_9019,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:100;n:type:ShaderForge.SFN_Vector1,id:2192,x:32548,y:32412,varname:node_2192,prsc:2,v1:3.5;n:type:ShaderForge.SFN_Color,id:3536,x:32776,y:32840,ptovrint:False,ptlb:EmissionColor,ptin:_EmissionColor,varname:node_3536,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ChannelBlend,id:7056,x:33519,y:32357,varname:node_7056,prsc:2,chbt:1|M-8354-OUT,R-1249-OUT,G-1249-OUT,B-1249-OUT,A-8354-OUT,BTM-8354-OUT;n:type:ShaderForge.SFN_Blend,id:3408,x:33090,y:32751,varname:node_3408,prsc:2,blmd:20,clmp:True|SRC-9624-OUT,DST-7056-OUT;n:type:ShaderForge.SFN_Add,id:9624,x:32858,y:32655,varname:node_9624,prsc:2|A-7056-OUT,B-8354-OUT;n:type:ShaderForge.SFN_Multiply,id:5288,x:33740,y:32632,varname:node_5288,prsc:2|A-1041-OUT,B-2197-OUT;n:type:ShaderForge.SFN_OneMinus,id:9652,x:33272,y:32614,varname:node_9652,prsc:2|IN-6921-OUT;n:type:ShaderForge.SFN_Floor,id:625,x:33355,y:32756,varname:node_625,prsc:2|IN-3408-OUT;n:type:ShaderForge.SFN_Add,id:3242,x:33197,y:32958,varname:node_3242,prsc:2|A-9652-OUT,B-625-OUT;n:type:ShaderForge.SFN_Blend,id:1508,x:33355,y:32908,varname:node_1508,prsc:2,blmd:5,clmp:True|SRC-9652-OUT,DST-3242-OUT;n:type:ShaderForge.SFN_Floor,id:8146,x:33566,y:32967,varname:node_8146,prsc:2|IN-1508-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6416,x:33853,y:32969,varname:node_6416,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1;n:type:ShaderForge.SFN_ChannelBlend,id:1041,x:33811,y:32816,varname:node_1041,prsc:2,chbt:1|M-1249-OUT,R-8146-OUT,G-8146-OUT,B-8146-OUT,BTM-8146-OUT;proporder:6059-668-7286-2197-2511-9019-3536;pass:END;sub:END;*/

Shader "Shader Forge/InverseGlowShader" {
    Properties {
        _BlackBullet ("BlackBullet", Range(-1, 0)) = 0
        _fuzziness ("fuzziness", Float ) = -0.4
        [HideInInspector]_node_7286 ("node_7286", Float ) = 10
        _EmissivePower ("EmissivePower", Range(-100, 100)) = 10
        _MainTex ("MainTex", 2D) = "bump" {}
        _EmissiveColorPower ("EmissiveColorPower", Range(0, 100)) = 5
        [HDR]_EmissionColor ("EmissionColor", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _BlackBullet;
            uniform float _fuzziness;
            uniform float _node_7286;
            uniform float _EmissivePower;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _EmissiveColorPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
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
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_6921_if_leA = step(0.0,_BlackBullet);
                float node_6921_if_leB = step(_BlackBullet,0.0);
                float4 node_5619 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_6921 = lerp((node_6921_if_leA*node_5619.rgb)+(node_6921_if_leB*floor(((1.0 - dot(node_5619.rgb,float3(0.3,0.59,0.11)))-_fuzziness))),node_5619.rgb,node_6921_if_leA*node_6921_if_leB);
                float3 node_8354 = pow(saturate((node_6921*node_5619.rgb)),_node_7286);
                float3 node_1249 = ((1.0 - node_8354)*_EmissiveColorPower);
                float3 node_9652 = (1.0 - node_6921);
                float3 node_7056 = (lerp( lerp( lerp( node_8354, node_1249, node_8354.r ), node_1249, node_8354.g ), node_1249, node_8354.b ));
                float3 node_8146 = floor(saturate(max(node_9652,(node_9652+floor(saturate((node_7056/(node_7056+node_8354))))))));
                float3 node_5288 = ((lerp( lerp( lerp( node_8146, node_8146, node_1249.r ), node_8146, node_1249.g ), node_8146, node_1249.b ))*_EmissivePower);
                float3 normalLocal = node_5288;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                clip(node_5619.a - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = node_5288;
                float3 finalColor = emissive + node_5288;
                return fixed4(finalColor,node_5619.a);
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
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 node_5619 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(node_5619.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
