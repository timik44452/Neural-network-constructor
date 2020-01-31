using UnityEngine;

namespace Core.Service
{
    public static class ViewportService
    {
        public static Vector2 cameraPosition;

        public static Rect GetConnectionEllipse(bool isRight, Rect rect, bool localSpace = true)
        {
            float size = Mathf.Min(rect.width, rect.height) * 0.115F;

            Vector2 center = localSpace ? rect.size * 0.5F : rect.position + rect.size * 0.5F;
            Vector2 delta = new Vector2(rect.width * 0.5F - size, 0);

            Vector2 point = isRight ? center + delta : center - delta;

            return new Rect(point.x - size * 0.5F, point.y - size * 0.5F, size, size);
        }

        public static Rect ToSceneRect(Rect rect)
        {
            rect.position += cameraPosition;

            return rect;
        }

        public static Rect FromSceneRect(Rect rect)
        {
            rect.position -= cameraPosition;

            return rect;
        }

        public static Vector2 FromScreenVector(Vector2 mousePosition)
        {
            mousePosition -= cameraPosition;

            return mousePosition;
        }
    }
}