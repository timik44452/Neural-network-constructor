using UnityEngine;

namespace Core.Service
{
    public static class Extensions
    {
        public static bool IsCollidedCircle(Rect a, Rect b, float radius = 1)
        {
            return a.width * radius > Vector2.Distance(a.center, b.center);
        }
    }
}
