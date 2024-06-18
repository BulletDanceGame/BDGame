using UnityEngine;

namespace BulletDance.Graphics
{
    [System.Serializable]
    public class ColorName
    {
        public string name;
        public Color color;
    }

    [System.Serializable]
    public class Colors
    {
        //Hit UI
        public static readonly Color perfectHit = new Color(0.5f, 1f, 1f, 1f);
        public static readonly Color goodHit    = new Color(0f, 0.5f, 1f, 1f);
        public static readonly Color badHit     = new Color(0.5f, 0f, 0f, 1f);

        //Bullet
        //public const Color default    = new Color(1f, 1f, 1f, 1f);
        //public const Color unhittable = new Color(1f, 1f, 1f, 1f);

        public static Gradient CreateGradient(Color colorMin, Color colorMax)
        {
            var _gradient = new Gradient();
    
            var _gradientColors = new GradientColorKey[2];
            _gradientColors[0].color = colorMin;
            _gradientColors[1].color = colorMax;
            _gradientColors[0].time = 0f;
            _gradientColors[1].time = 1f;
    
            var _gradientAlpha = new GradientAlphaKey[2];
            _gradientAlpha[0].alpha = colorMin.a;
            _gradientAlpha[0].time  = colorMax.a;
            _gradientAlpha[1].alpha = 1;
            _gradientAlpha[1].time  = 0.5f;
    
            _gradient.SetKeys(_gradientColors, _gradientAlpha);
            return _gradient;
        }
    }
}