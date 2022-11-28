#pragma shader_feature_local _ USE_BORDER

CBUFFER_START(UnityPerMaterial)               
    float4 _MainTex_ST;
        
    float _Expand = 0;
    float _Softness = 0;
                            
    float _BorderWidth = 0;
    float _BorderSoftness = 0;
    float4 _BorderColor;
    float _BorderColorMulti = 0;
                            
    float _InverseExpand = 0;
    float _InverseSoftness = 0;

    float _FlowStep = 0;
CBUFFER_END

void ProcessUSDF(inout float4 color, float sdf, float inverseSDF)
{
    float clipThreshold = max(.9 - _Expand, 0);
    float clipEnd = min(clipThreshold-_Softness, clipThreshold);
    float body;

    float4 borderColor = _BorderColor;
    if(_BorderColorMulti != 0)
    {
        borderColor = float4(color.rgb*_BorderColorMulti, color.a);
    }
    
    if(_Softness == 0)
    {
        float dist = clipThreshold - sdf;

        float2 ddist = float2(ddx(dist), ddy(dist));
        float pixelDist = dist / length(ddist);

        body = saturate(.5 - pixelDist);
    }
    else
    {        
        body = smoothstep(clipEnd, clipThreshold , sdf);
    }

    float inverseThreshold = min(1-saturate(.9 - _InverseExpand), .99);
    float inverseBody;
    
    if(_InverseSoftness == 0 && _InverseExpand != 1)
    {
        float dist = inverseThreshold - inverseSDF;

        float2 ddist = float2(ddx(dist), ddy(dist));
        float pixelDist = dist / length(ddist);

        inverseBody = saturate(.5 - pixelDist);
    }
    else
    {
        float inverseClipEnd = min(inverseThreshold-_InverseSoftness, inverseThreshold);

        inverseBody = smoothstep(inverseClipEnd, inverseThreshold, inverseSDF);
    }  

    color *= body*inverseBody;

    #ifdef USE_BORDER
        float borderClipStart = saturate(clipEnd - _BorderWidth);
    
        if(_BorderSoftness == 0)
        {
            float dist = borderClipStart - sdf;

            float2 ddist = float2(ddx(dist), ddy(dist));
            float pixelDist = dist / length(ddist);

            color += (saturate(.5 - pixelDist)-body)*borderColor;
        }
        else
        {            
            float borderClipEnd = saturate(borderClipStart - max(_BorderSoftness, 0));
            color += (smoothstep(borderClipEnd, borderClipStart, sdf) - body)*borderColor;
        }   
    #endif
}

void ProcessFlowSDF(inout float4 color, float sdf, float inverseSDF)
{
    float clipThreshold = max(.9 - _Expand, 0);
    float clipEnd = min(clipThreshold-_Softness, clipThreshold);
    float body;

    float4 borderColor = _BorderColor;
    if(_BorderColorMulti != 0)
    {
        borderColor = float4(color.rgb*_BorderColorMulti, color.a);
    }
    
    if(_Softness == 0 || sdf == 0)
    {
        float dist = clipThreshold - sdf;

        float2 ddist = float2(ddx(dist), ddy(dist));
        float pixelDist = dist / length(ddist);

        body = saturate(.5 - pixelDist);
    }
    else
    {        
        body = smoothstep(clipEnd, clipThreshold , sdf);
    }

    float inverseThreshold = max(.9 - _InverseExpand, .1);
    float inverseBody;
    
    if(_InverseSoftness == 0 && _InverseExpand != 1)
    {
        float dist = inverseThreshold - inverseSDF;

        float2 ddist = float2(ddx(dist), ddy(dist));
        float pixelDist = dist / length(ddist);

        inverseBody = saturate(.5 - pixelDist);
    }
    else
    {
        float inverseClipStart= min(inverseThreshold+_InverseSoftness, .9);

        inverseBody = smoothstep(inverseThreshold, inverseClipStart, inverseSDF);
    }  

    

    #ifdef USE_BORDER    
        color = lerp(borderColor, color, body);
    #else
        color *= body;
    #endif
    

    color *= inverseBody;
}

