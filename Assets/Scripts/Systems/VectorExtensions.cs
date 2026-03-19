namespace Systems
{
    public static class VectorExtensions
    {
        public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 vec) => new UnityEngine.Vector2(vec.X, vec.Y);
        public static System.Numerics.Vector2 ToNumerics(this UnityEngine.Vector2 vec) => new System.Numerics.Vector2(vec.x, vec.y);
        public static UnityEngine.Vector3 ToVector3(this UnityEngine.Vector2 vec) => new UnityEngine.Vector3(vec.x, vec.y, 0);
    
        public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 vec) => new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
        public static System.Numerics.Vector3 ToNumerics(this UnityEngine.Vector3 vec) => new System.Numerics.Vector3(vec.x, vec.y, vec.z);
        public static UnityEngine.Vector2 ToVector2(this UnityEngine.Vector3 vec) => new UnityEngine.Vector2(vec.x, vec.y);
    }
}