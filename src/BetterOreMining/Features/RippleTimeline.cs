using UnityEngine;

namespace BetterOreMining.Features
{
    public static class RippleTimeline
    {
        public static float Front(float elapsed, float waveSpeed)
        {
            return elapsed * waveSpeed;
        }

        public static float End(float radius, float ringWidth)
        {
            return radius + ringWidth;
        }

        public static bool IsFinished(float front, float radius, float ringWidth)
        {
            return front >= End(radius, ringWidth);
        }

        public static float Fade(float front, float radius, float ringWidth)
        {
            return 1f - Mathf.Clamp01(front / End(radius, ringWidth));
        }
    }
}
