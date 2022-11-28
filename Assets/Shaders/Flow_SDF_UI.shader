// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/FlowSDF"
{
    Properties
    {
        [PerRendererData] [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
        
        _Softness ("Inside Softness", Range(0, 1)) = 0
        
        _InverseExpand("Outside", Range(0, .9)) = .9
        _InverseSoftness("Outside Softness", Range(0, 1)) = 0
        
        _FlowStep ("FlowSteps", Integer) = 1
        
        [Toggle(USE_BORDER)] _UseBorder ("Use Background", Float) = 0
        _BorderColor ("Background Color", Color) = (1,1,1,1)
        _BorderColorMulti ("Border Color Multiplier", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "SDFHelpers.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            
        
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _TextureSampleAdd;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color;
                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                half4 color = _TextureSampleAdd + IN.color;
                
                float4 sdfData = tex2D(_MainTex, IN.texcoord);
                float sdf = (((sdfData.r*_FlowStep)+_Time.y)%1);

                ProcessFlowSDF(color, sdf, sdfData.r);

                /*#ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif*/
                
                clip (color.a - 0.001);

                return color;
            }
        ENDCG
        }
    }
}