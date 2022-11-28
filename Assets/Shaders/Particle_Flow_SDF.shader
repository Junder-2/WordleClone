Shader "Particles/Unlit_Flow_SDF"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        
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
        }
        Blend SrcAlpha One
        Lighting Off
        ColorMask RGB
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "SDFHelpers.cginc"
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            struct appdata
            {
                float4 vertex : POSITION;       //local vertex position
                float2 texcoord : TEXCOORD0;   //uv coordinates
                float4 color : COLOR; 
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;              //screen clip space position and depth
                float2 uv : TEXCOORD0;                //uv coordinates
                float4 color : TEXCOORD2;
                UNITY_FOG_COORDS(3)                    //this initializes the unity fog
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                half4 color = i.color;
                
                float4 sdfData = tex2D(_MainTex, i.uv);
                float sdf = (((sdfData.r*_FlowStep)+_Time.y)%1);               

                UNITY_APPLY_FOG_COLOR(i.fogCoord, color, fixed4(0,0,0,0)); // fog towards black due to our blend mode

                ProcessFlowSDF(color, sdf, sdfData.r);

                return color;
            }
            ENDCG
        }
    }
}
