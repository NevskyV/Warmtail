struct Shape {
    float3 position;
    float2 UV;
    float radius;
    float materialID;
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
    float h = clamp(0.5 + 0.5 + (b - a) / k, 0.0, 1.0);
    Out = lerp(b, a, h) - k * h * (1.0 - h);
}

void SmoothIntersection_float(float a, float b, float k, out float Out)
{
    float h = clamp(0.5 + 0.5 + (a - b) / k, 0.0, 1.0);
    Out = lerp(b, a, h) - k * h * (1.0 - h);
}

void SmoothDifference_float(float a, float b, float k, out float Out)
{
    float h = clamp(0.5 + 0.5 + (a - (-b)) / k, 0.0, 1.0);
    Out = lerp(-b, a, h) + k * h * (1.0 - h);
}

void SceneSDF_float(float3 worldPos, float smoothness, out float dist, out float materialID) {
    dist = 1e10;
    materialID = 0;

    [loop]
    for (int i = 0; i < _ShapeCount; i++) {
        Shape s = _Shapes[i];
        float d;
        CircleSDF_float(s.UV, s.radius,  d);

        // Если текущая фигура ближе без blending — обновляем материал
        if (d < dist) {
            materialID = s.materialID;
        }

        // Smooth union
        float temp;
        SmoothUnion_float(dist, d, smoothness, temp);
        dist = temp;
    }

    // Если фигур нет — возвращаем большое расстояние
    if (_ShapeCount == 0) dist = 1e10;
}