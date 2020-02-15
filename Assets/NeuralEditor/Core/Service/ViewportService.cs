using UnityEngine;

namespace Core.Service
{
    public static class ViewportService
    {
        public static float Zoom
        {
            get => zoom;
            set => zoom = Mathf.Clamp(value, 0.1F, 10F);
        }
        public static Vector2 cameraPosition { get; set; }

        private static float zoom = 1F;

        public static Rect GetConnectionEllipse(bool isRight, Rect rect, bool localSpace = true)
        {
            float size = Mathf.Min(rect.width, rect.height) * 0.115F;

            Vector2 center = localSpace ? rect.size * 0.5F : rect.position + rect.size * 0.5F;
            Vector2 delta = new Vector2(rect.width * 0.5F - size, 0);

            Vector2 point = isRight ? center + delta : center - delta;

            return new Rect(point.x - size * 0.5F, point.y - size * 0.5F, size, size);
        }

        public static Rect ToScreenRect(Rect rect)
        {
            Matrix4x4 viewMatrix = new Matrix4x4(
                new Vector4(zoom, 0, 0, 0),
                new Vector4(0, zoom, 0, 0),
                new Vector4(0, 0, zoom, 0),
                new Vector4(cameraPosition.x, cameraPosition.y, 0, 1));

            rect.position = viewMatrix.MultiplyPoint(rect.position);
            
            return rect;
        }

        public static Vector2 ToScreenPoint(Vector2 point)
        {
            point += cameraPosition;

            return point;
        }

        public static Vector2 FromScreenPoint(Vector2 point)
        {
            point -= cameraPosition;

            return point;
        }
    }
}