void SpriteOutline_float(
float dist,
float2 UV,
float OutlineThickness,
float InnerOutlineThickness,
float WaveAmp,
float WaveFreq,
float WaveSpeed,
float GlobalTime,
out float OutlineMask,
out float FillMask)
{
    float angle = atan2(UV.y - 0.5, UV.x - 0.5);
    float wave = sin(angle * WaveFreq + GlobalTime * WaveSpeed) * WaveAmp;
    float perturbedDist = wave -dist;

    float innerMask = saturate(perturbedDist / InnerOutlineThickness);

    float outerMask = saturate(-perturbedDist / OutlineThickness);
    OutlineMask = max(outerMask, innerMask);
    FillMask = saturate(perturbedDist);
    OutlineMask = saturate(OutlineMask - FillMask);
}

void StrokeOutline_float(
float dist,
float2 UV,
float OutlineThickness,
float InnerOutlineThickness,
float WaveAmp,
float WaveFreq,
float WaveSpeed,
float GlobalTime,
out float OutlineMask,
out float FillMask)
{
    float angle = atan2(UV.y - 0.5, UV.x - 0.5);
    float wave = sin(angle * WaveFreq + GlobalTime * WaveSpeed) * WaveAmp;
    FillMask = 0;
    float perturbedDist = wave -dist;
    OutlineMask = perturbedDist;
    
    float bias = WaveAmp;

    OutlineMask = abs(perturbedDist + bias) - (OutlineThickness + bias);
    OutlineMask = smoothstep(OutlineThickness * 0.5, 0.0, wave);
    OutlineMask *= step(0.0, perturbedDist + bias);

    float innerMask = saturate(perturbedDist / InnerOutlineThickness);
    FillMask = innerMask;
    // if (perturbedDist)
    //     float outerMask = saturate(-perturbedDist / OutlineThickness);
    // OutlineMask = max(outerMask, innerMask);
    // FillMask = saturate(perturbedDist);
    // OutlineMask = saturate(OutlineMask - FillMask);
}