#ifndef VolumetricFog
#define VolumetricFog

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

float random3D(float3 uv)
{
    float Coord = (uv.x + uv.y + uv.z);
    float2 _uv = float2(Coord, Coord);
    float2 noise = (frac(sin(dot(_uv, float2(12.9898, 78.233) * 2.0)) * 43758.5453));
    return (abs(noise.x + noise.y) - 1);
}

float random(float2 uv)
{
    float2 noise = (frac(sin(dot(uv, float2(12.9898, 78.233) * 2.0)) * 43758.5453));
    return (abs(noise.x + noise.y) - 1);
}

float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir)
{
    float3 t0 = (boundsMin - rayOrigin) * invRaydir;
    float3 t1 = (boundsMax - rayOrigin) * invRaydir;
    float3 tmin = min(t0, t1);
    float3 tmax = max(t0, t1);

    float dstA = max(max(tmin.x, tmin.y), tmin.z);
    float dstB = min(tmax.x, min(tmax.y, tmax.z));

    float dstToBox = max(0, dstA);
    float dstInsideBox = max(0, dstB - dstToBox);
    return float2(dstToBox, dstInsideBox);
}

float3 GetRay(float2 screenPos)
{
    float3 viewVector = mul(unity_CameraInvProjection, float4(screenPos * 2 - 1, 0, -1));
    float3 viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));
    float viewLength = length(viewDir);
    float3 ray = viewDir / viewLength;

    return ray;
}

float SceneDepth(float2 UV)
{
    // return Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV), _ZBufferParams);
    return LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV), _ZBufferParams);
    //return SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV), _ZBufferParams;
}

float3 saturation(float3 In, float Saturation)
{
    float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
    return luma.xxx + Saturation.xxx * (In - luma.xxx);
}

float3 GetLighting(float3 rayPos, Texture2D additional, sampler sampler_additional, float additionalLightsCount)
{
    float3 color = 0;

    UNITY_LOOP
    for (int x = 0; x < additionalLightsCount; x++)
    {
        float width = 512;

        float4 lightPosRange = SAMPLE_TEXTURE2D(additional, sampler_additional, float2(x / width, 0.15));
        float3 lightPos = lightPosRange.xyz;
        float lightRange = lightPosRange.w;

        if (distance(lightPos, rayPos) < lightRange)
        {

            float4 lightVectorAngle = SAMPLE_TEXTURE2D(additional, sampler_additional, float2(x / width, 0.25));
            float3 lightVector = normalize(lightVectorAngle.xyz);
            float lightAngle = lightVectorAngle.w;

            float isInCone = 1;
            if (lightAngle > 0)
            {
                float3 rayVector = normalize(rayPos - lightPos);
                float dp = dot(rayVector, lightVector);
                if (acos(dp) * 57.2958 > lightAngle / 2)
                {
                    isInCone = 0;
                }
            }

            if (isInCone > 0)
            {
                float3 lightCol = SAMPLE_TEXTURE2D(additional, sampler_additional, float2(x / width, 0.05)).xyz;
                float falloff = distance(lightPos, rayPos) / lightRange;
                falloff = pow(1 - clamp(falloff, 0, 1), 3);
                falloff += exp(-distance(lightPos, rayPos)) * lightRange;

                color += falloff * lightCol * 0.1;
            }
        }
    }

    return color;
}

float GetShadows(float3 rayPos)
{
    return MainLightRealtimeShadow(TransformWorldToShadowCoord(rayPos));
}

float Phase(float eyeCos, float anisotropy)
{
    float numerator = (1 - pow(anisotropy, 2)) * (pow(eyeCos, 2) + 1);
    float denominator = pow((pow(anisotropy, 2) + 1) - (anisotropy * eyeCos), 1.5) * (pow(anisotropy, 2) + 2);
    float final = (numerator / denominator) * ((8 * PI) / 3);
    return final;
}

float EyeCos(float3 viewDirection)
{
    //Light Data
    float3 dir = 0;

#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(float4(0, 0, 0, 0), 0, 1);
    dir = mainLight.direction;
#endif


    //Calculations
    float dotProduct = dot(viewDirection, dir);
    dotProduct = (dotProduct + 1) * 0.5;

    return dotProduct;
}

void RenderFog_float(float density, float anisotropy, float3 boxPos, float3 boxBounds, float steps, float stepSize, float depth, float4 screenPos, float lowResDist, float lowResMultiplier, float maxStepSize, float jitter, Texture2D additional, sampler sampler_additional, float additionalLightsCount, out float3 _color, out float3 _lightColor, out float3 _lightDir, out float _phase, out float _density)
{
    //Initialize Variables
    boxPos.xz += _WorldSpaceCameraPos.xz;
    _color = 0;
    _lightColor = 0;
    _lightDir = 0;
    _density = 0;
    _phase = 0;

    //Light Data
    float3 lightDir = 0;
    float3 lightCol = 0;

#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(float4(0, 0, 0, 0), float3(0, 0, 0), 1);
    lightDir = mainLight.direction;
    lightCol = normalize(mainLight.color);
#endif

    //Intersection Ray
    float3 viewVector = mul(unity_CameraInvProjection, float4(screenPos.xy * 2 - 1, 0, -1));
    float3 viewDir = mul(unity_CameraToWorld, float4(viewVector, 0));
    float viewLength = length(viewDir);

    float3 ray = GetRay(screenPos.xy);

    //Volume Intersection

    float3 posBL = boxPos - boxBounds / 2;
    float3 posTR = boxPos + boxBounds / 2;

    float2 boxDist = rayBoxDst(posBL, posTR, _WorldSpaceCameraPos, 1 / ray);

    float distToBox = boxDist.x;
    float distInBox = boxDist.y;

    //Ray Setup
    float3 entryPoint = _WorldSpaceCameraPos + ray * distToBox;

    //float3 rayPos = entryPoint + ray * viewLength;
    float3 rayPos = entryPoint;

    //Calculate Depth
    //depth = SceneDepth(screenPos.xy / screenPos.w) * viewLength;

    //Early Exit Conditions
    if (distInBox == 0 || distToBox > depth)
    {
        return;
    }

    //Calculate Phase
    float phase = Phase(EyeCos(viewDir), anisotropy);

    //Initialize Variables
    float totalDensity = 0;
    float3 totalLight = 0;
    float distTravelled = 0;
    float tempStepSize = stepSize;
    float totalSteps = 0;
    UNITY_LOOP
    for (int i = 0; i < steps && distTravelled + distToBox < depth && distTravelled < distInBox; i++)
    {
        float rnd = abs(random(rayPos.xz + rayPos.yz)) * jitter;
        rnd += 1;
        tempStepSize *= rnd;

#ifdef MAIN_LIGHT_CALCULATE_SHADOWS
        float3 lighting = GetLighting(rayPos, additional, sampler_additional, additionalLightsCount);
        float shadows = GetShadows(rayPos);

        float shadowsMin = dot(lightDir, float3(0, 1, 0));
        shadowsMin = max(shadowsMin, 0);
        shadowsMin = 1 - shadowsMin;
        shadowsMin = pow(shadowsMin, 6);

        shadows = shadowsMin + (shadows * (1 - shadowsMin));

        shadows = clamp(shadows, 0, 1);
#else
        float3 lighting = 1;
        float shadows = 1;
#endif

        float falloff = distance(_WorldSpaceCameraPos, rayPos);
        falloff = exp(-falloff * 5);

        totalLight += ((lighting * totalDensity * 0.5) + (shadows * lightCol * phase));

        float minDens = length( lighting ) * density * 10;
        minDens = clamp(minDens, 0, 1);

        totalDensity += max(density, minDens) * tempStepSize * ((shadows * 0.5) + 0.5);

        //Early Exit Conditions
        if (exp(-totalDensity) == 0)
        {
            break;
        }

        //March Along Ray
        distTravelled += tempStepSize;
        rayPos += ray * tempStepSize;
        totalSteps += 1;

        if (distTravelled > lowResDist)
        {
            tempStepSize = clamp(distTravelled / lowResMultiplier, 0, maxStepSize);
        }
    }

    //Normalize Density
    float3 normalizedDensity = exp(-totalDensity);

    //Normalize Light
    float3 normalizedLight = totalLight / totalSteps;

    _color = normalizedLight;
    _lightColor = lightCol;
    _lightDir = lightDir;
    _density = (1 - normalizedDensity);
    _phase = phase;
    return;
}
#endif