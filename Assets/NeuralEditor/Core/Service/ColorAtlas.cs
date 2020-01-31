using UnityEngine;

namespace Core.Service
{
    public static class ColorAtlas
    {
        public static Color orange { get => GetColor(255, 171, 59); }
        public static Color blue { get => GetColor(51, 173, 255); }
        public static Color green { get => GetColor(60, 255, 122); }
        public static Color gray { get => GetColor(156, 166, 166); }

        private static Color GetColor(int r, int g, int b, int a = 255)
        {
            return new Color(r / 255F, g / 255F, b / 255F, a / 255F);
        }
    }
}
