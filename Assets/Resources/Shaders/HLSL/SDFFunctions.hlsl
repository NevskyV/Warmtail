struct Shape {
    int type;
    float2 position;
    float rotation;
    float2 size;
    float4 cornerRadius;
    float additionalRounding;
};

StructuredBuffer<Shape> _Shapes;
int _ShapeCount;


void CircleSDF_float(float2 UV, float Radius, out float Dist)
{
    Dist = length(UV) - Radius;
}

void RectangleSDF_float(float2 UV, float2 Size, float4 CornerRounding, float cornerRounding, out float Dist)
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

void OutlineSDF_float(float Distance, float Thickness, out float Dist)
{
    Dist = abs(Distance) - Thickness;
}

void InlineSDF_float(float Distance, float Thickness, out float Dist)
{
    Dist = max(Distance, -Distance - Thickness);
}

void SmoothUnion_float(float a, float b, float k, out float Out)
{
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    Out = lerp(b, a, h) - k * h * (1.0 - h);
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

float2 RotateUV(float2 uv, float angle)
{
    float c = cos(angle);
    float s = sin(angle);
    return float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
}

float GetShapeSDF(float2 uv, Shape s)
{
    float2 offset = (0.5, 0.5);
    float2 localUV = uv - offset - s.position;
    
    localUV = RotateUV(localUV, -s.rotation);
    
    if (s.type == 0)
    {
        float dist;
        CircleSDF_float(localUV, s.size.x, dist);
        return dist;
    }
    else
    {
        float dist;
        RectangleSDF_float(localUV, s.size, s.cornerRadius, s.additionalRounding, dist);
        return dist;
    }
}

void SceneSDF_float(float2 UV, float Smoothness, float InterType, out float Dist)
{
    Dist = 1e10;
    if (InterType == 1) Dist = -1e10;
    if (InterType == 2)
    {
        [loop]
        for (int i = 1; i < _ShapeCount; i++)
        {
            Shape s = _Shapes[i];
            float d = GetShapeSDF(UV, s);
                
            float temp;
            SmoothUnion_float(Dist, d, Smoothness, temp);
            Dist = temp;
        }
        Shape s = _Shapes[0];
        float d = GetShapeSDF(UV, s);
        float temp;
        SmoothDifference_float(Dist, d, Smoothness, temp);
        Dist = temp;
    }
    else
    {
        [loop]
        for (int i = 0; i < _ShapeCount; i++)
        {
            Shape s = _Shapes[i];
            float d = GetShapeSDF(UV, s);
        
            float temp;
            if (InterType == 0)
                SmoothUnion_float(Dist, d, Smoothness, temp);
            else if (InterType == 1)
                SmoothIntersection_float(Dist, d, Smoothness, temp);
            Dist = temp;
        }
    }
    
    if (_ShapeCount == 0)
        Dist = 1e10;
}