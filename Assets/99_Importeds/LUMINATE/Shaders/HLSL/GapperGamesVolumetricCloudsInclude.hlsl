#ifndef VolumetricClouds
#define VolumetricClouds

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

float3 mod289(float3 x) {
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 mod289(float4 x) {
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 permute(float4 x) {
    return mod289(((x * 34.0) + 10.0) * x);
}

float4 taylorInvSqrt(float4 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float snoise(float3 v)
{
    const float2  C = float2(1.0 / 6.0, 1.0 / 3.0);
    const float4  D = float4(0.0, 0.5, 1.0, 2.0);

    // First corner
    float3 i = floor(v + dot(v, C.yyy));
    float3 x0 = v - i + dot(i, C.xxx);

    // Other corners
    float3 g = step(x0.yzx, x0.xyz);
    float3 l = 1.0 - g;
    float3 i1 = min(g.xyz, l.zxy);
    float3 i2 = max(g.xyz, l.zxy);

    //   x0 = x0 - 0.0 + 0.0 * C.xxx;
    //   x1 = x0 - i1  + 1.0 * C.xxx;
    //   x2 = x0 - i2  + 2.0 * C.xxx;
    //   x3 = x0 - 1.0 + 3.0 * C.xxx;
    float3 x1 = x0 - i1 + C.xxx;
    float3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
    float3 x3 = x0 - D.yyy;      // -1.0+3.0*C.x = -0.5 = -D.y

  // Permutations
    i = mod289(i);
    float4 p = permute(permute(permute(
        i.z + float4(0.0, i1.z, i2.z, 1.0))
        + i.y + float4(0.0, i1.y, i2.y, 1.0))
        + i.x + float4(0.0, i1.x, i2.x, 1.0));


    float n_ = 0.142857142857;
    float3  ns = n_ * D.wyz - D.xzx;

    float4 j = p - 49.0 * floor(p * ns.z * ns.z);

    float4 x_ = floor(j * ns.z);
    float4 y_ = floor(j - 7.0 * x_);

    float4 x = x_ * ns.x + ns.yyyy;
    float4 y = y_ * ns.x + ns.yyyy;
    float4 h = 1.0 - abs(x) - abs(y);

    float4 b0 = float4(x.xy, y.xy);
    float4 b1 = float4(x.zw, y.zw);

    float4 s0 = floor(b0) * 2.0 + 1.0;
    float4 s1 = floor(b1) * 2.0 + 1.0;
    float4 sh = -step(h, 0);

    float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
    float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

    float3 p0 = float3(a0.xy, h.x);
    float3 p1 = float3(a0.zw, h.y);
    float3 p2 = float3(a1.xy, h.z);
    float3 p3 = float3(a1.zw, h.w);

    //Normalise gradients
    float4 norm = taylorInvSqrt(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;

    // Mix final noise value
    float4 m = max(0.5 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
    m = m * m;
    return 105.0 * dot(m * m, float4(dot(p0, x0), dot(p1, x1),
        dot(p2, x2), dot(p3, x3)));
}

float SnoiseRND(float2 uv)
{
    float angle = dot(uv, float2(12.9898, 78.233));
#if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
    // 'sin()' has bad precision on Mali GPUs for inputs > 10000
    angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
#endif
    return frac(sin(angle) * 43758.5453);
}

float SnoiseInterpolate(float a, float b, float t)
{
    return (1.0 - t) * a + (t * b);
}

float Snoise_Value(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = SnoiseRND(c0);
    float r1 = SnoiseRND(c1);
    float r2 = SnoiseRND(c2);
    float r3 = SnoiseRND(c3);

    float bottomOfGrid = SnoiseInterpolate(r0, r1, f.x);
    float topOfGrid = SnoiseInterpolate(r2, r3, f.x);
    float t = SnoiseInterpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float Snoise_3D(float3 uv)
{
    float xy = Snoise_Value(uv.xz + uv.xy);
    float xz = Snoise_Value(uv.xz);
    float yz = Snoise_Value(uv.xz + uv.yz);

    //float xyz = (xy * yz) + xz;
    float xyz = xz;

    return xyz;
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

float GetDensity(float3 rayPos, float3 boxPos, float3 boxBounds, float heightFalloff, float speed, float coverageScale, float noiseScale, Texture3D _NoiseTexture, sampler sampler_NoiseTexture, bool invertNoise, float thickness, float3 noiseWeights)
{
    //Calculate Height Falloff
    float base = boxPos.y;
    float normalizedHeight = abs(rayPos.y - base) / (boxBounds.y);
    normalizedHeight = pow(normalizedHeight, heightFalloff * 2);
    normalizedHeight = 1 - normalizedHeight;

    //Calculate Noise
    float3 offset = _Time * speed * 20;
    float noise = 0;
    float3 noiseTex = 0;

    float noise1 = Snoise_Value((rayPos.xz + (offset.xz * 0.5)) * coverageScale * 0.0001);
    float noise2 = Snoise_Value((rayPos.xz + (offset.xz * 2)) * coverageScale * 0.00015);
    noiseTex = SAMPLE_TEXTURE3D(_NoiseTexture, sampler_NoiseTexture, (rayPos + offset) * noiseScale * 0.00001 * float3(1, 2, 1));

    if (invertNoise)
    {
        noiseTex = 1 - noiseTex;
        noiseTex.z = 1 - noiseTex.z;
    }

    noiseTex *= noiseWeights;
    noise += noiseTex.x;
    noise += noiseTex.y;
    noise /= (noiseWeights.x + noiseWeights.y);
    
    noise *= (noise1 + noise2) / 2;

    noise = clamp(noise, 0, 1);

    noise -= noiseTex.z * 0.15;

    //Normalize Noise
    noise -= thickness;
    noise = clamp(noise, 0, 1);

    return noise * normalizedHeight;
}

float GetLighting(float3 rayPos, float3 lightDir, float3 boxPos, float3 boxBounds, float heightFalloff, float speed, float coverageScale, float noiseScale, Texture3D _NoiseTexture, sampler sampler_NoiseTexture, bool invertNoise, float thickness, float3 noiseWeights, float lightScattering)
{
    //Initialize Variables
    float lightSteps = 10;

    float3 posBL = boxPos - boxBounds / 2;
    float3 posTR = boxPos + boxBounds / 2;

    if (lightDir.y < 0)
    {
        lightDir.y = -lightDir.y;
    }

    float3 ray = normalize(lightDir);
    float2 boxDist = rayBoxDst(posBL, posTR, rayPos, 1 / ray);

    float distInBox = boxDist.y;

    float size = distInBox / lightSteps;

    float cumulativeDensity = 0;

    //Lightmarch
    UNITY_LOOP
    for (int i = 0; i < lightSteps; i++)
    {
        float noise = GetDensity(rayPos, boxPos, boxBounds, heightFalloff, speed, coverageScale, noiseScale, _NoiseTexture, sampler_NoiseTexture, invertNoise, thickness, noiseWeights);

        cumulativeDensity += noise * size;

        //Increment Raypos
        rayPos += ray * size;
    }

    //Normalize Lighting
    return exp(-cumulativeDensity * pow(lightScattering, 3) * 10);
}

float Phase(float eyeCos, float anisotropy)
{
    float numerator = (1 - pow(anisotropy, 2)) * (pow(eyeCos, 2) + 1);
    float denominator = pow((pow(anisotropy, 2) + 1) - (anisotropy * eyeCos), 1.5) * (pow(anisotropy, 2) + 2);
    float final = (numerator / denominator) * ((8 * PI) / 3);
    return final;
}

float EyeCos(float3 viewDirection, float3 dir)
{
    //Calculations
    float dotProduct = dot(viewDirection, dir);
    dotProduct = (dotProduct + 1) * 0.5;

    return dotProduct;
}

float getFogAmount(float dist, float fogThickness)
{
    return 1.0f - (0.1f + exp(-dist * fogThickness * 0.0005));
}

void RenderClouds_float(
Texture3D _NoiseTexture,
sampler sampler_NoiseTexture,
float4 Albedo,
float brightness,
float density,
float speed,
float anisotropy,
float minDarkness,
float maxDist,
float heightFalloff,
float steps,
float stepSize,
float jitter,
float lightScattering,
float3 boxPos,
float3 boxBounds,
float thickness,
float noiseScale,
bool invertNoise,
float3 noiseWeights,
float coverageScale,
float fogThickness,
float4 screenPos,
float depth,
out float3 cloudColor,
out float alpha,
out float fog)
{
    //Make density curve more responsive
    density = pow(density, 3);

    //Offset position to follow the camera
    boxPos.xz += _WorldSpaceCameraPos.xz;

    //Light Data
    float3 dir = 0;
    float3 lightCol = 1;

#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(float4(0, 0, 0, 0), float3(0, 0, 0), 1);
    dir = mainLight.direction;
    lightCol = mainLight.color;
#endif

    if (dot(dir, float3(0, 1, 0)) == 0)
    {
        dir += float3(0.01, 0.01, 0.01);
    }

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

    //Early Exit Conditions
    if (distInBox == 0)
    {
        //surface.BaseColor = background;
        cloudColor = 0;
        alpha = 0;
        fog = 0;
        return;
    }

    fog = 1 - getFogAmount(distToBox, fogThickness);

    //Initialize Variables
    float totalDensity = 0;
    float3 totalLight = 0;
    float distTravelled = 0;
    float transmittance = 1;
    float tempStepSize = stepSize;
    UNITY_LOOP
    for (int i = 0; i < steps && distTravelled + distToBox < depth && distTravelled < distInBox; i++)
    {
        //Sample Density
        float noise = GetDensity(rayPos, boxPos, boxBounds, heightFalloff, speed, coverageScale, noiseScale, _NoiseTexture, sampler_NoiseTexture, invertNoise, thickness, noiseWeights);

        //Accumulate Density
        totalDensity += density * tempStepSize * noise;

        //Lighting
        float hg = Phase(EyeCos(viewDir, dir), anisotropy);

        totalLight += GetLighting(rayPos, dir, boxPos, boxBounds, heightFalloff, speed, coverageScale, noiseScale, _NoiseTexture, sampler_NoiseTexture, invertNoise, thickness, noiseWeights, lightScattering) * transmittance * noise * stepSize;      
        
        transmittance *= exp(-totalDensity);

        //March Along Ray
        float randomLength = abs(random(frac(rayPos.xz * rayPos.yz)));
        float offset = tempStepSize * ((randomLength * jitter * 2) + (1 - jitter));
        distTravelled += offset;
        rayPos += ray * offset;

        //Early Exit Conditions
        if (exp(-totalDensity) < 0.001 || distTravelled > maxDist)
        {
            break;
        }
    }

    //Normalize Values
    float normalizedDensity = 1 - clamp(exp(-totalDensity), 0, 1);
    float normalizedLight = 1 - exp(-totalLight);
    //normalizedLight = totalLight;

    //Calculate Phase
    float phase = Phase(EyeCos(viewDir, dir), anisotropy);
    //phase = 1;

    float lighting = phase * normalizedLight;
    //lighting *= 1 - minDarkness;
    lighting += minDarkness;

    //Lighting Adjustments
    float timeLighting = dot(dir, float3(0, 1, 0));
    timeLighting = max(timeLighting, 0.05);
    timeLighting = 1 - timeLighting;
    timeLighting = pow(timeLighting, 3);
    timeLighting = 1 - timeLighting;

    cloudColor = (Albedo * normalize(lightCol) * brightness * lighting * timeLighting);
    alpha = normalizedDensity;
    return;
}
#endif