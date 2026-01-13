#define MAX_GROUPS 64

struct GroupProperty {
    float4 fillColor;
    float alpha;
    float4 outlineColor;
    float outlineThickness;
    float4 inlineColor;
    float inlineThickness;
    float inOutlineThickness;
    float waveFreq;
    float waveAmp;
    float waveSpeed;
};

struct Shape {
    int type;
    float2 position;
    float rotation;
    float2 size;
    float4 paramsA;
    float4 paramsB;
    int groupIndex;
};

StructuredBuffer<Shape> _AllShapes;
int _ShapeCount;

StructuredBuffer<GroupProperty> _GroupProps;
int _GroupCount;

float _GroupSmoothness;


void OutlineSDF_float(float Distance, float Thickness, out float Dist)
{
    Dist = abs(Distance) - Thickness;
}

void InlineSDF_float(float Distance, float Thickness, out float Dist)
{
    Dist = max(Distance, -Distance - Thickness);
}

void Circle_float(float2 p, float r, out float Dist) {
    Dist = length(p) - r;
}

void ChamferBox(float2 p, float2 b, float chamfer, out float Dist) {
    p = abs(p) - b;
    p = (p.y > p.x) ? p.yx : p.xy;
    p.y += chamfer;
    const float k = 1.0 - sqrt(2.0);
    if (p.y < 0.0 && p.y + p.x * k < 0.0) Dist = p.x;
    else if (p.x < p.y) Dist = (p.x + p.y) * sqrt(0.5);
    else Dist = length(p);
}

void Box_float(float2 UV, float2 Size, float4 CornerRounding, float cornerRounding, out float Dist)
{
    float2 centered = UV;
    float2 q = abs(centered);

    float top_mask = step(0.0, centered.y);
    float right_mask = step(0.0, centered.x);

    float left_side_rounding = lerp(CornerRounding.x, CornerRounding.y, top_mask);
    float right_side_rounding = lerp(CornerRounding.w, CornerRounding.z, top_mask);

    float r_individual = lerp(left_side_rounding, right_side_rounding, right_mask);

    float r = r_individual + cornerRounding;

    float2 d = q - Size + r;
    Dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}

void Segment(float2 p, float2 a, float2 b, out float Dist) {
    float2 pa = p - a;
    float2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    Dist = length(pa - ba * h);
}

void Rhombus(float2 p, float2 b, out float Dist) {
    b.y = -b.y;
    p = abs(p);
    float h = clamp( (dot(b,p) + b.y*b.y) / dot(b,b), 0.0, 1.0 );
    p -= b * float2(h, h-1.0);
    Dist = length(p) * sign(p.x);
}

void Trapezoid(float2 p, float r1, float r2, float he, out float Dist) {
    float2 k1 = float2(r2, he);
    float2 k2 = float2(r2 - r1, 2.0 * he);
    p.x = abs(p.x);
    float2 ca = float2(p.x - min(p.x, p.y < 0.0 ? r1 : r2), abs(p.y) - he);
    float2 cb = p - k1 + k2 * clamp(dot(k1 - p, k2) / dot(k2, k2), 0.0, 1.0);
    float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
    Dist = s * sqrt(min(dot(ca, ca), dot(cb, cb)));
}

void Parallelogram(float2 p, float wi, float he, float sk, out float Dist) {
    float2 e = float2(sk, he);
    p = (p.y < 0.0) ? -p : p;
    float2 w = p - e; w.x -= clamp(w.x, -wi, wi);
    float2 d = float2(dot(w, w), -w.y);
    float s = p.x * e.y - p.y * e.x;
    p = (s < 0.0) ? -p : p;
    float2 v = p - float2(wi, 0);
    v -= e * clamp(dot(v, e) / dot(e, e), -1.0, 1.0);
    d = min(d, float2(dot(v, v), wi * he - abs(s)));
    Dist = sqrt(d.x) * sign(-d.y);
}

void EquilateralTriangle(float2 p, float r, out float Dist) {
    const float k = sqrt(3.0);
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = float2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    Dist = -length(p) * sign(p.y);
}

void TriangleIsosceles(float2 p, float2 q, out float Dist) {
    p.x = abs(p.x);
    float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
    float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
    float s = -sign(q.y);
    float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)),
                   float2(dot(b, b), s * (p.y - q.y)));
    Dist = -sqrt(d.x) * sign(d.y);
}

void Triangle(float2 p, float2 p0, float2 p1, float2 p2, out float Dist) {
    float2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
    float2 v0 = p - p0, v1 = p - p1, v2 = p - p2;
    float2 pq0 = v0 - e0 * clamp(dot(v0, e0) / dot(e0, e0), 0.0, 1.0);
    float2 pq1 = v1 - e1 * clamp(dot(v1, e1) / dot(e1, e1), 0.0, 1.0);
    float2 pq2 = v2 - e2 * clamp(dot(v2, e2) / dot(e2, e2), 0.0, 1.0);
    float s = sign(e0.x * e2.y - e0.y * e2.x);
    float2 d = min(min(float2(dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
                       float2(dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
                       float2(dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
    Dist = -sqrt(d.x) * sign(d.y);
}

void UnevenCapsule(float2 p, float r1, float r2, float h, out float Dist) {
    p.x = abs(p.x);
    float b = (r1 - r2) / h;
    float a = sqrt(1.0 - b * b);
    float k = dot(p, float2(-b, a));
    if (k < 0.0) Dist = length(p) - r1;
    else if (k > a * h) Dist = length(p - float2(0.0, h)) - r2;
    else Dist = dot(p, float2(a, b)) - r1;
}

void Pentagon(float2 p, float r, out float Dist) {
    const float3 k = float3(0.809016994, 0.587785252, 0.726542528);
    p.x = abs(p.x);
    p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
    p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
    p -= float2(clamp(p.x, -r * k.z, r * k.z), r);
    Dist = length(p) * sign(p.y);
}

void Hexagon(float2 p, float r, out float Dist) {
    const float3 k = float3(-0.866025404, 0.5, 0.577350269);
    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
    Dist = length(p) * sign(p.y);
}

void Octogon(float2 p, float r, out float Dist) {
    const float3 k = float3(-0.9238795325, 0.3826834323, 0.4142135623);
    p = abs(p);
    p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
    p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
    p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
    Dist = length(p) * sign(p.y);
}

void Hexagram(float2 p, float r, out float Dist) {
    const float4 k = float4(-0.5, 0.8660254038, 0.5773502692, 1.7320508076);
    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= 2.0 * min(dot(k.yx, p), 0.0) * k.yx;
    p -= float2(clamp(p.x, r * k.z, r * k.w), r);
    Dist = length(p) * sign(p.y);
}

void Star5(float2 p, float r, out float Dist) {
    const float k1x = 0.809016994;  
    const float k1y = 0.587785252;  
    const float k2x = 0.309016994;  
    const float k2y = 0.951056516;  
    const float k1z = 0.726542528;  

    const float2 v1 = float2( k1x, -k1y);
    const float2 v2 = float2(-k1x, -k1y);
    const float2 v3 = float2( k2x, -k2y);

    p.x = abs(p.x);
    p -= 2.0 * max(dot(v1, p), 0.0) * v1;
    p -= 2.0 * max(dot(v2, p), 0.0) * v2;
    p.x = abs(p.x);
    p.y -= r;

    Dist = length(p - v3 * clamp(dot(p, v3), 0.0, k1z * r)) * sign(p.y * v3.x - p.x * v3.y);
}

void Pie(float2 p, float angle, float r, out float Dist) {
    float2 c = float2(sin(angle * 0.5), cos(angle * 0.5));
    p.x = abs(p.x);
    float l = length(p) - r;
    float m = length(p - c * clamp(dot(p,c), 0.0, r));
    Dist = max(l, m * sign(c.y * p.x - c.x * p.y));
}

void CutDisk(float2 p, float r, float h, out float Dist) {
    float w = sqrt(r*r - h*h);
    p.x = abs(p.x);
    float s = max((h - r)*p.x*p.x + w*w*(h + r - 2.0*p.y), h*p.x - w*p.y);
    Dist = (s < 0.0) ? length(p) - r :
           (p.x < w) ? h - p.y :
           length(p - float2(w, h));
}

void Arc(float2 p, float angle, float ra, float rb, out float Dist) {
    float2 sc = float2(sin(angle * 0.5), cos(angle * 0.5));
    p.x = abs(p.x);
    Dist = ((sc.y * p.x > sc.x * p.y) ? length(p - sc * ra) : abs(length(p) - ra)) - rb;
}

void Cross(float2 p, float2 b, float r, out float Dist) {
    p = abs(p); if (p.y > p.x) p = p.yx;
    float2 q = p - b;
    float k = max(q.y, q.x);
    float2 w = (k > 0.0) ? q : float2(b.y - p.x, -k);
    Dist = sign(k) * length(max(w, 0.0)) + r;
}

void Heart(float2 p, out float Dist) {
    p.x = abs(p.x);
    if (p.y + p.x > 1.0)
        Dist = sqrt(dot(p - float2(0.25, 0.75), p - float2(0.25, 0.75))) - sqrt(2.0)/4.0;
    Dist = sqrt(min(
        dot(p - float2(0.00, 1.00), p - float2(0.00, 1.00)),
        dot(p - 0.5 * max(p.x + p.y, 0.0), p - 0.5 * max(p.x + p.y, 0.0))
    )) * sign(p.x - p.y);
}

void RoundedX(float2 p, float w, float r, out float Dist) {
    p = abs(p);
    Dist = length(p - min(p.x + p.y, w) * 0.5) - r;
}

void RoundedCross(float2 pos, float he, out float Dist) {
    pos = abs(pos);
    float2 p = float2(abs(pos.x - pos.y), 1.0 - pos.x - pos.y) / sqrt(2.0);
    float pp = (he - p.y - 0.25 / he) / (6.0 * he);
    float qq = p.x / (he * he * 16.0);
    float hh = qq * qq - pp * pp * pp;
    float x;
    if (hh > 0.0) {
        float r = sqrt(hh);
        x = pow(qq + r, 1.0/3.0) - pow(abs(qq - r), 1.0/3.0) * sign(r - qq);
    } else {
        float r = sqrt(pp);
        x = 2.0 * r * cos(acos(qq / (pp * r)) / 3.0);
    }
    x = min(x, sqrt(2.0)/2.0);
    float2 z = float2(x, he * (1.0 - 2.0 * x * x)) - p;
    Dist = length(z) * sign(z.y);
}

void SmoothUnion_float(float a, float b, float k, out float result) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    result = lerp(b, a, h) - k * h * (1.0 - h);
}

void SmoothIntersection_float(float a, float b, float k, out float Out)
{
    float h = clamp(0.5 - 0.5 * (b - a) / k, 0.0, 1.0);
    Out = lerp(b, a, h) + k * h * (1.0 - h);
}


void SmoothDifference_float(float a, float b, float k, out float Out)
{
    float h = clamp(0.5 - 0.5 * (b + a) / k, 0.0, 1.0);
    Out = lerp(b, -a, h) + k * h * (1.0 - h);
}

float2 RotateUV(float2 uv, float angle) {
    float c = cos(angle);
    float s = sin(angle);
    return float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
}

float GetShapeSDF(float2 uv, Shape s) {
    float2 offset = float2(0.5, 0.5);
    float2 localUV = uv - offset - s.position;
    localUV = RotateUV(localUV, -s.rotation);
    float dist;
    
    if (s.type == 0)  Circle_float(localUV, s.size.x, dist);
    else if (s.type == 1)  ChamferBox(localUV, s.size, s.paramsA.x, dist);
    else if (s.type == 2)  Box_float(localUV, s.size, s.paramsA, s.paramsB.x, dist);
    else if (s.type == 3)  Segment(localUV, s.paramsA.xy, s.paramsA.zw, dist);
    else if (s.type == 4)  Rhombus(localUV, s.size, dist);
    else if (s.type == 5)  Trapezoid(localUV, s.paramsA.x, s.paramsA.y, s.paramsA.z, dist);
    else if (s.type == 6)  Parallelogram(localUV, s.size.x, s.size.y, s.paramsA.x, dist);
    else if (s.type == 7)  EquilateralTriangle(localUV, s.size.x, dist);
    else if (s.type == 8) TriangleIsosceles(localUV, s.paramsA.xy, dist);
    else if (s.type == 9) Triangle(localUV, s.paramsA.xy, s.paramsA.zw, s.paramsB.xy, dist);
    else if (s.type == 10) UnevenCapsule(localUV, s.paramsA.x, s.paramsA.y, s.paramsA.z, dist);
    else if (s.type == 11) Pentagon(localUV, s.size.x, dist);
    else if (s.type == 12) Hexagon(localUV, s.size.x, dist);
    else if (s.type == 13) Octogon(localUV, s.size.x, dist);
    else if (s.type == 14) Hexagram(localUV, s.size.x, dist);
    else if (s.type == 15) Star5(localUV, s.size.x, dist);
    else if (s.type == 16) Pie(localUV, s.paramsA.xy, s.size.x, dist);
    else if (s.type == 17) CutDisk(localUV, s.size.x, s.paramsA.x, dist);
    else if (s.type == 18) Arc(localUV, s.paramsA.xy, s.size.x, s.size.y, dist);
    else if (s.type == 19) Cross(localUV, s.size, s.paramsA.x, dist);
    else if (s.type == 20) RoundedCross(localUV, s.size.x, dist);
    else if (s.type == 19) RoundedX(localUV, s.size.x, s.paramsA.x, dist);
    else if (s.type == 20) Heart(localUV, dist);
    else dist = 1e10;
    
    return dist;
}

float GetLocalWavePerturbation(float2 localUV, float GlobalTime, float WaveFreq, float WaveAmp, float WaveSpeed) {
    if (WaveAmp < 0.00001) return 0.0;
    float angle = atan2(localUV.y, localUV.x);
    return sin(angle * WaveFreq + GlobalTime * WaveSpeed) * WaveAmp;
}

/*void SceneSDF_float(
    float2 UV,
    float Smoothness,
    float InterType,
    float GlobalTime,
    float OutlineThickness,
    float InOutlineThickness,
    float WaveFreq,
    float WaveAmp,
    float WaveSpeed,
    out float fillMask,
    out float outlineMask)
{
    float combinedBaseSDF = (InterType == 1) ? -1e10 : 1e10;
    float combinedWavySDF = (InterType == 1) ? -1e10 : 1e10;
    outlineMask = 0.0;
    
    if (_ShapeCount == 0)
    {
        fillMask = 0.0;
        return;
    }

    [loop]
    for (int i = 0; i < _ShapeCount; i++)
    {
        Shape s = _Shapes[i];
        
        float2 offset = float2(0.5, 0.5);
        float2 localUV = UV - offset - s.position;
        localUV = RotateUV(localUV, -s.rotation);
        
        float baseDist = GetShapeSDF(UV, s);
        
        float wavePerturbation = GetLocalWavePerturbation(localUV, GlobalTime, WaveFreq, WaveAmp, WaveSpeed);
        float wavyDist = baseDist - wavePerturbation;
        
        float tempBase;
        if (InterType == 0)
            SmoothUnion_float(combinedBaseSDF, baseDist, Smoothness, tempBase);
        else if (InterType == 1)
            SmoothIntersection_float(combinedBaseSDF, baseDist, Smoothness, tempBase);
        else if (InterType == 2 && i > 0)
            SmoothDifference_float(baseDist, combinedBaseSDF, Smoothness, tempBase);
        else
            tempBase = baseDist;
        
        combinedBaseSDF = tempBase;
        
        float tempWavy;
        if (InterType == 0)
            SmoothUnion_float(combinedWavySDF, wavyDist, Smoothness, tempWavy);
        else if (InterType == 1)
            SmoothIntersection_float(combinedWavySDF, wavyDist, Smoothness, tempWavy);
        else if (InterType == 2 && i > 0)
            SmoothDifference_float(wavyDist, combinedWavySDF, Smoothness, tempWavy);
        else
            tempWavy = wavyDist;
        
        combinedWavySDF = tempWavy;
    }
    
    float bias = WaveAmp * InOutlineThickness;
    
    float outlineDist = abs(combinedWavySDF + bias) - (OutlineThickness + bias);
    
    outlineMask = smoothstep(OutlineThickness * 0.5, 0.0, outlineDist);
    outlineMask *= step(0.0, combinedWavySDF + bias);
    
    fillMask = combinedBaseSDF;
}*/

int _MyGroupIndex;
int _IsBaseGroup;
float _InterGroupType;

void SceneSDF_float(
    float2 UV,
    float Smoothness,
    float InterType,
    float GlobalTime,
    out float fillMask,
    out float outlineMask,
    out float4 fillColor,
    out float alpha,
    out float4 outlineColor,
    out float outlineThickness,
    out float4 inlineColor,
    out float inlineThickness)
{
    float groupFill[MAX_GROUPS];
    float groupWavy[MAX_GROUPS];
    GroupProperty groupProp[MAX_GROUPS];

    for (int g = 0; g < _GroupCount; g++)
    {
        groupFill[g] = (InterType == 1) ? -1e10 : 1e10;
        groupWavy[g] = (InterType == 1) ? -1e10 : 1e10;
        groupProp[g] = _GroupProps[g];
    }

    [loop]
    for (int i = 0; i < _ShapeCount; i++)
    {
        Shape s = _AllShapes[i];
        int g = s.groupIndex;
        if (g < 0 || g >= _GroupCount) continue;

        float2 offset = float2(0.5, 0.5);
        float2 localUV = UV - offset - s.position;
        localUV = RotateUV(localUV, -s.rotation);

        float baseDist = GetShapeSDF(UV, s);

        GroupProperty currentGroupProp = _GroupProps[g];
        float currentWaveFreq = currentGroupProp.waveFreq;
        float currentWaveAmp = currentGroupProp.waveAmp;
        float currentWaveSpeed = currentGroupProp.waveSpeed;

        float wavePerturbation = GetLocalWavePerturbation(localUV, GlobalTime, currentWaveFreq, currentWaveAmp, currentWaveSpeed);
        float wavyDist = baseDist - wavePerturbation;

        float tempBase;
        if (InterType == 0)
            SmoothUnion_float(groupFill[g], baseDist, Smoothness, tempBase);
        else if (InterType == 1)
            SmoothIntersection_float(groupFill[g], baseDist, Smoothness, tempBase);
        else if (InterType == 2 && i > 0)
            SmoothDifference_float(groupFill[g], baseDist, Smoothness, tempBase);
        else
            tempBase = baseDist;

        groupFill[g] = tempBase;

        float tempWavy;
        if (InterType == 0)
            SmoothUnion_float(groupWavy[g], wavyDist, Smoothness, tempWavy);
        else if (InterType == 1)
            SmoothIntersection_float(groupWavy[g], wavyDist, Smoothness, tempWavy);
        else if (InterType == 2 && i > 0)
            SmoothDifference_float(groupWavy[g], wavyDist, Smoothness, tempWavy);
        else
            tempWavy = wavyDist;

        groupWavy[g] = tempWavy;
    }

    float combinedFill = groupFill[0];
    float combinedWavy = groupWavy[0];
    GroupProperty combinedProp = groupProp[0];

    for (int g = 1; g < _GroupCount; g++)
    {
        float h = clamp(0.5 + 0.5 * (groupFill[g] - combinedFill) / _GroupSmoothness, 0.0, 1.0);

        float tempFill;
        float tempWavy;
        if (_InterGroupType == 2.0 && g == 1)
        {
            SmoothDifference_float(combinedFill, groupFill[g], _GroupSmoothness, tempFill);
            SmoothDifference_float(combinedWavy, groupWavy[g], _GroupSmoothness, tempWavy);
        }
        else
        {
            SmoothUnion_float(combinedFill, groupFill[g], _GroupSmoothness, tempFill);
            SmoothUnion_float(combinedWavy, groupWavy[g], _GroupSmoothness, tempWavy);
        }

        combinedFill = tempFill;
        combinedWavy = tempWavy;

        combinedProp.fillColor = lerp(groupProp[g].fillColor, combinedProp.fillColor, h);
        combinedProp.alpha = lerp(groupProp[g].alpha, combinedProp.alpha, h);
        combinedProp.outlineColor = lerp(groupProp[g].outlineColor, combinedProp.outlineColor, h);
        combinedProp.outlineThickness = lerp(groupProp[g].outlineThickness, combinedProp.outlineThickness, h);
        combinedProp.inlineColor = lerp(groupProp[g].inlineColor, combinedProp.inlineColor, h);
        combinedProp.inlineThickness = lerp(groupProp[g].inlineThickness, combinedProp.inlineThickness, h);
        combinedProp.waveFreq = lerp(groupProp[g].waveFreq, combinedProp.waveFreq, h);
        combinedProp.waveAmp = lerp(groupProp[g].waveAmp, combinedProp.waveAmp, h);
        combinedProp.waveSpeed = lerp(groupProp[g].waveSpeed, combinedProp.waveSpeed, h);
    }

    float bias = combinedProp.waveAmp * combinedProp.inOutlineThickness;

    float outlineDist = abs(combinedWavy + bias) - (combinedProp.outlineThickness + bias);

    outlineMask = smoothstep(combinedProp.outlineThickness * 0.5, 0.0, outlineDist);
    outlineMask *= step(0.0, combinedWavy + bias);

    fillMask = combinedFill;
    fillColor = combinedProp.fillColor;
    alpha = combinedProp.alpha;
    outlineColor = combinedProp.outlineColor;
    outlineThickness = combinedProp.outlineThickness;
    inlineColor = combinedProp.inlineColor;
    inlineThickness = combinedProp.inlineThickness;
}