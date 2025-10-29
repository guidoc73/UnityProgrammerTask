using UnityEngine;

namespace Utils
{
    public static class Vector2Extensions
    {
        public static Vector3 ToFixedVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }
    }
}
