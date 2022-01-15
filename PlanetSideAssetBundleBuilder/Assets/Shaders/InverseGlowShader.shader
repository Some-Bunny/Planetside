// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:577,x:33173,y:32000,varname:node_577,prsc:2|custl-9717-OUT;n:type:ShaderForge.SFN_Tex2d,id:5619,x:31623,y:32152,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_5619,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fe33c54468a51af4b8b70ea84487e1c0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Subtract,id:8956,x:32118,y:31950,varname:node_8956,prsc:2|A-7675-OUT,B-668-OUT;n:type:ShaderForge.SFN_OneMinus,id:9717,x:32969,y:32239,varname:node_9717,prsc:2|IN-8354-OUT;n:type:ShaderForge.SFN_Slider,id:6059,x:32115,y:31806,ptovrint:False,ptlb:BlackBullet,ptin:_BlackBullet,varname:node_6059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:0;n:type:ShaderForge.SFN_ValueProperty,id:668,x:31964,y:32121,ptovrint:False,ptlb:fuzziness,ptin:_fuzziness,varname:node_1964,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.56;n:type:ShaderForge.SFN_Desaturate,id:9502,x:31794,y:31950,varname:node_9502,prsc:2|COL-5619-RGB;n:type:ShaderForge.SFN_Floor,id:4673,x:32272,y:31950,varname:node_4673,prsc:2|IN-8956-OUT;n:type:ShaderForge.SFN_Blend,id:8828,x:32624,y:32239,varname:node_8828,prsc:2,blmd:1,clmp:True|SRC-6921-OUT,DST-5619-RGB;n:type:ShaderForge.SFN_OneMinus,id:7675,x:31951,y:31950,varname:node_7675,prsc:2|IN-9502-OUT;n:type:ShaderForge.SFN_If,id:6921,x:32463,y:32121,varname:node_6921,prsc:2|A-6597-OUT,B-6059-OUT,GT-4673-OUT,EQ-5619-RGB,LT-5619-RGB;n:type:ShaderForge.SFN_Vector1,id:6597,x:32213,y:32121,varname:node_6597,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7286,x:32624,y:32447,ptovrint:False,ptlb:node_7286,ptin:_node_7286,varname:node_7286,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Power,id:8354,x:32795,y:32239,varname:node_8354,prsc:2|VAL-8828-OUT,EXP-7286-OUT;proporder:5619-6059-668-7286;pass:END;sub:END;*/

Shader "Shader Forge/InverseGlowShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _BlackBullet ("BlackBullet", Range(-1, 0)) = 0
        _fuzziness ("fuzziness", Float ) = -0.56
        _node_7286 ("node_7286", Float ) = 0.5
    }
    //"RenderType"="Opaque"
}
Pass {
    Name "FORWARD"
    "LightMode"="ForwardBase"
}


struct VertexInput {
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4 vertex : POSITION;
    float2 texcoord0 : TEXCOORD0;
};
struct VertexOutput {
    float4 pos : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float2 uv0 : TEXCOORD0;
    UNITY_FOG_COORDS(1)
};
VertexOutput vert (VertexInput v) {
    VertexOutput o = (VertexOutput)0;
    UNITY_SETUP_INSTANCE_ID( v );
    UNITY_TRANSFER_INSTANCE_ID( v, o );
    o.uv0 = v.texcoord0;
    o.pos = UnityObjectToClipPos( v.vertex );
    UNITY_TRANSFER_FOG(o,o.pos);
    return o;
}
float4 frag(VertexOutput i) : COLOR {
    UNITY_SETUP_INSTANCE_ID( i );
// Lighting:
    float _BlackBullet_var = UNITY_ACCESS_INSTANCED_PROP( Props, _BlackBullet );
    float node_6921_if_leA = step(0.0,_BlackBullet_var);
    float node_6921_if_leB = step(_BlackBullet_var,0.0);
    float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
    float _fuzziness_var = UNITY_ACCESS_INSTANCED_PROP( Props, _fuzziness );
    float _node_7286_var = UNITY_ACCESS_INSTANCED_PROP( Props, _node_7286 );
    float3 finalColor = (1.0 - pow(saturate((lerp((node_6921_if_leA*_MainTex_var.rgb)+(node_6921_if_leB*floor(((1.0 - dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)))-_fuzziness_var))),_MainTex_var.rgb,node_6921_if_leA*node_6921_if_leB)*_MainTex_var.rgb)),_node_7286_var));
    fixed4 finalRGBA = fixed4(finalColor,1);
    UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
    return finalRGBA;
}
}
}
FallBack "Diffuse"
CustomEditor "ShaderForgeMaterialInspector"
}
