using BepInEx.Configuration;
using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Config
{
    public sealed class ModSettings : IRippleSettings, IColorSettings
    {
        private readonly ConfigEntry<float> _radius;
        private readonly ConfigEntry<float> _waveSpeed;
        private readonly ConfigEntry<float> _ringWidth;
        private readonly ConfigEntry<float> _opacity;
        private readonly ConfigEntry<Color> _visibleColor;
        private readonly ConfigEntry<Color> _blockedColor;

        public ModSettings(ConfigFile file)
        {
            _radius = file.Bind(
                "Ripple",
                "Radius",
                8f,
                new ConfigDescription(
                    "How far the ripple spreads from where you hit, in meters.",
                    new AcceptableValueRange<float>(2f, 40f)));
            _waveSpeed = file.Bind(
                "Ripple",
                "WaveSpeed",
                4.5f,
                new ConfigDescription(
                    "How fast the ripple spreads, in meters per second.",
                    new AcceptableValueRange<float>(2f, 100f)));
            _ringWidth = file.Bind(
                "Ripple",
                "RingWidth",
                2f,
                new ConfigDescription(
                    "How thick the glowing ring is, in meters.",
                    new AcceptableValueRange<float>(0.5f, 10f)));
            _opacity = file.Bind(
                "Ripple",
                "Opacity",
                0.8f,
                new ConfigDescription(
                    "How strong the ring is at its brightest, from 0 to 1.",
                    new AcceptableValueRange<float>(0.05f, 1f)));
            _visibleColor = file.Bind(
                "Colors",
                "Visible",
                new Color(0.2f, 1f, 0.35f, 1f),
                "Ring color on ore at or above the ground, as RGBA hex (RRGGBBAA).");
            _blockedColor = file.Bind(
                "Colors",
                "Blocked",
                new Color(1f, 0.15f, 0.1f, 1f),
                "Ring color on ore buried 2 meters or deeper, as RGBA hex (RRGGBBAA).");
        }

        public float Radius => _radius.Value;

        public float WaveSpeed => _waveSpeed.Value;

        public float RingWidth => _ringWidth.Value;

        public float Opacity => _opacity.Value;

        public Color VisibleColor => _visibleColor.Value;

        public Color BlockedColor => _blockedColor.Value;
    }
}
