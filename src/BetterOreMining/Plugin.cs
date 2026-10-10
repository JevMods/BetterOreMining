using System.IO;
using BepInEx;
using BetterOreMining.Config;
using BetterOreMining.Features;
using BetterOreMining.Infrastructure;
using HarmonyLib;
using UnityEngine;

namespace BetterOreMining
{
    [BepInPlugin(PluginGuid, PluginName, BuildInfo.Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "JevMods.BetterOreMining";
        public const string PluginName = "BetterOreMining";

        private IFeature[] _features;
        private Harmony _harmony;

        private void Awake()
        {
            var events = new GameEvents();
            GameEvents.Instance = events;

            _features = CreateFeatures(events, new ModSettings(Config), LoadShader());
            foreach (var feature in _features)
            {
                feature.Enable();
            }

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Logger.LogInfo("Loaded");
        }

        private void Update()
        {
            GameEvents.Instance?.RaiseFrameUpdated();
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            if (_features != null)
            {
                foreach (var feature in _features)
                {
                    feature.Disable();
                }
            }

            GameEvents.Instance = null;
        }

        private Shader LoadShader()
        {
            var folder = Path.GetDirectoryName(Info.Location);
            return ShaderBundle.Load(Path.Combine(folder, "assets", "betteroremining"));
        }

        private static IFeature[] CreateFeatures(
            IGameEvents events,
            ModSettings settings,
            Shader rippleShader)
        {
            var reach = new OreReachFeature(events, new PhysicsOreScanner());
            if (rippleShader == null)
            {
                return new IFeature[] { reach };
            }

            var ripple = new RippleFeature(
                events,
                settings,
                new PhysicsVeinScanner(),
                new ZoneGroundProbe(),
                new OverlayGlowRenderer(rippleShader, settings, new GroundTextures()));
            return new IFeature[] { ripple, reach };
        }
    }
}
