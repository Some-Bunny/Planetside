// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:True,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:34411,y:32413,varname:node_4013,prsc:2|normal-6534-OUT,emission-6534-OUT,alpha-4365-OUT;n:type:ShaderForge.SFN_Time,id:2106,x:32192,y:33368,varname:node_2106,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:539,x:32297,y:32645,varname:node_539,prsc:2,tex:b0651448fe30ada41826fe99e5e24059,ntxv:0,isnm:False|UVIN-9446-UVOUT,TEX-9541-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:2273,x:31876,y:33028,ptovrint:False,ptlb:Nebula,ptin:_Nebula,varname:node_2273,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6e3ff783bb8d205488f3af1b4723c401,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Panner,id:3565,x:32192,y:33207,varname:node_3565,prsc:2,spu:0.1,spv:0.1|UVIN-6968-UVOUT,DIST-2106-T;n:type:ShaderForge.SFN_TexCoord,id:2807,x:31379,y:32780,varname:node_2807,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:8491,x:32831,y:33135,varname:node_8491,prsc:2,v1:5;n:type:ShaderForge.SFN_Vector1,id:2365,x:32014,y:33368,varname:node_2365,prsc:2,v1:1;n:type:ShaderForge.SFN_UVTile,id:9446,x:31804,y:32816,varname:node_9446,prsc:2|UVIN-2807-UVOUT,WDT-6076-OUT,HGT-6076-OUT,TILE-2542-OUT;n:type:ShaderForge.SFN_Multiply,id:622,x:32490,y:32645,varname:node_622,prsc:2|A-539-RGB,B-5372-OUT;n:type:ShaderForge.SFN_Vector1,id:5372,x:32490,y:32572,varname:node_5372,prsc:2,v1:4;n:type:ShaderForge.SFN_ComponentMask,id:8941,x:32350,y:32819,varname:node_8941,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-622-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:6042,x:32686,y:32815,varname:node_6042,prsc:2,chbt:1|M-2401-OUT,R-2401-OUT,G-2401-OUT,B-2401-OUT,BTM-8941-OUT;n:type:ShaderForge.SFN_Vector1,id:6076,x:31379,y:32942,varname:node_6076,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Vector1,id:2542,x:31714,y:32990,varname:node_2542,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2dAsset,id:9541,x:31784,y:32407,ptovrint:False,ptlb:Floor_Tex,ptin:_Floor_Tex,varname:node_9541,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b0651448fe30ada41826fe99e5e24059,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4862,x:32064,y:33015,varname:node_4862,prsc:2,tex:6e3ff783bb8d205488f3af1b4723c401,ntxv:0,isnm:False|UVIN-3565-UVOUT,TEX-2273-TEX;n:type:ShaderForge.SFN_UVTile,id:6968,x:32000,y:33217,varname:node_6968,prsc:2|UVIN-3272-UVOUT,WDT-2365-OUT,HGT-2365-OUT,TILE-2365-OUT;n:type:ShaderForge.SFN_TexCoord,id:3272,x:31629,y:33251,varname:node_3272,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ComponentMask,id:9707,x:32321,y:33095,varname:node_9707,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-4862-RGB;n:type:ShaderForge.SFN_Multiply,id:7367,x:32641,y:33167,varname:node_7367,prsc:2|A-9707-B,B-7877-OUT;n:type:ShaderForge.SFN_Multiply,id:6405,x:32676,y:33030,varname:node_6405,prsc:2|A-9950-OUT,B-7367-OUT;n:type:ShaderForge.SFN_Multiply,id:660,x:32859,y:32952,varname:node_660,prsc:2|A-4862-RGB,B-7877-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1547,x:33015,y:33002,varname:node_1547,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-660-OUT;n:type:ShaderForge.SFN_Relay,id:7877,x:32918,y:33223,varname:node_7877,prsc:2|IN-8491-OUT;n:type:ShaderForge.SFN_Desaturate,id:953,x:32900,y:33323,varname:node_953,prsc:2|COL-9707-OUT;n:type:ShaderForge.SFN_Color,id:224,x:32986,y:33526,ptovrint:False,ptlb:C,ptin:_C,varname:node_224,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.2,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:6885,x:33072,y:33309,varname:node_6885,prsc:2|A-953-OUT,B-224-RGB;n:type:ShaderForge.SFN_Multiply,id:8207,x:33242,y:33309,varname:node_8207,prsc:2|A-6885-OUT,B-9508-OUT;n:type:ShaderForge.SFN_ChannelBlend,id:8486,x:33485,y:33284,varname:node_8486,prsc:2,chbt:1|M-8207-OUT,R-8207-OUT,G-8207-OUT,B-8207-OUT,BTM-6405-OUT;n:type:ShaderForge.SFN_Multiply,id:8489,x:33487,y:33013,varname:node_8489,prsc:2|A-8486-OUT,B-9508-OUT,C-1547-OUT;n:type:ShaderForge.SFN_Vector1,id:9508,x:33015,y:32952,varname:node_9508,prsc:2,v1:10;n:type:ShaderForge.SFN_Multiply,id:9950,x:32536,y:33079,varname:node_9950,prsc:2|A-9707-OUT,B-3405-OUT;n:type:ShaderForge.SFN_Vector1,id:3405,x:32496,y:33274,varname:node_3405,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:1162,x:33274,y:32698,ptovrint:False,ptlb:OutlineWidth,ptin:_OutlineWidth,varname:node_1162,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Multiply,id:2401,x:32427,y:32959,varname:node_2401,prsc:2|A-4862-RGB,B-152-OUT;n:type:ShaderForge.SFN_Vector1,id:152,x:32064,y:32888,varname:node_152,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Vector2,id:1413,x:32733,y:32572,varname:node_1413,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Distance,id:9402,x:32906,y:32592,varname:node_9402,prsc:2|A-1413-OUT,B-503-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:503,x:32733,y:32661,varname:node_503,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:2792,x:33062,y:32592,varname:node_2792,prsc:2|IN-9402-OUT;n:type:ShaderForge.SFN_Multiply,id:1885,x:33274,y:32545,varname:node_1885,prsc:2|A-2792-OUT,B-1162-OUT;n:type:ShaderForge.SFN_Vector1,id:1687,x:32744,y:32456,varname:node_1687,prsc:2,v1:1;n:type:ShaderForge.SFN_Blend,id:1387,x:33265,y:32269,varname:node_1387,prsc:2,blmd:18,clmp:True|SRC-1885-OUT,DST-9192-OUT;n:type:ShaderForge.SFN_Multiply,id:9637,x:32937,y:32410,varname:node_9637,prsc:2|A-9402-OUT,B-1687-OUT;n:type:ShaderForge.SFN_OneMinus,id:9192,x:33116,y:32434,varname:node_9192,prsc:2|IN-9637-OUT;n:type:ShaderForge.SFN_OneMinus,id:6053,x:33606,y:32550,varname:node_6053,prsc:2|IN-1387-OUT;n:type:ShaderForge.SFN_Blend,id:4365,x:33930,y:32466,varname:node_4365,prsc:2,blmd:1,clmp:True|SRC-6053-OUT,DST-6053-OUT;n:type:ShaderForge.SFN_Multiply,id:6534,x:32906,y:32781,varname:node_6534,prsc:2|A-6042-OUT,B-3672-OUT;n:type:ShaderForge.SFN_Vector1,id:3672,x:32859,y:32914,varname:node_3672,prsc:2,v1:1.5;proporder:2273-9541-224-1162;pass:END;sub:END;*/

Shader "Shader Forge/TearInReailty" {
    Properties {
        _Nebula ("Nebula", 2D) = "black" {}
        _Floor_Tex ("Floor_Tex", 2D) = "white" {}
        _C ("C", Color) = (0.5,0.2,1,1)
        _OutlineWidth ("OutlineWidth", Float ) = 4
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            AlphaToMask On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _Nebula; uniform float4 _Nebula_ST;
            uniform sampler2D _Floor_Tex; uniform float4 _Floor_Tex_ST;
            uniform float _OutlineWidth;
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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_2106 = _Time;
                float node_2365 = 1.0;
                float2 node_6968_tc_rcp = float2(1.0,1.0)/float2( node_2365, node_2365 );
                float node_6968_ty = floor(node_2365 * node_6968_tc_rcp.x);
                float node_6968_tx = node_2365 - node_2365 * node_6968_ty;
                float2 node_6968 = (i.uv0 + float2(node_6968_tx, node_6968_ty)) * node_6968_tc_rcp;
                float2 node_3565 = (node_6968+node_2106.g*float2(0.1,0.1));
                float4 node_4862 = tex2D(_Nebula,TRANSFORM_TEX(node_3565, _Nebula));
                float3 node_2401 = (node_4862.rgb*1.5);
                float node_6076 = 1.5;
                float node_2542 = 0.0;
                float2 node_9446_tc_rcp = float2(1.0,1.0)/float2( node_6076, node_6076 );
                float node_9446_ty = floor(node_2542 * node_9446_tc_rcp.x);
                float node_9446_tx = node_2542 - node_6076 * node_9446_ty;
                float2 node_9446 = (i.uv0 + float2(node_9446_tx, node_9446_ty)) * node_9446_tc_rcp;
                float4 node_539 = tex2D(_Floor_Tex,TRANSFORM_TEX(node_9446, _Floor_Tex));
                float3 node_6534 = ((lerp( lerp( lerp( (node_539.rgb*4.0).rgb, node_2401, node_2401.r ), node_2401, node_2401.g ), node_2401, node_2401.b ))*1.5);
                float3 normalLocal = node_6534;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float3 emissive = node_6534;
                float3 finalColor = emissive;
                float node_9402 = distance(float2(0.5,0.5),i.uv0);
                float node_6053 = (1.0 - saturate((0.5 - 2.0*(((1.0 - node_9402)*_OutlineWidth)-0.5)*((1.0 - (node_9402*1.0))-0.5))));
                return fixed4(finalColor,saturate((node_6053*node_6053)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
