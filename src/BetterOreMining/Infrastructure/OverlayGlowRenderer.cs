using System;
using System.Collections.Generic;
using BetterOreMining.Features;
using UnityEngine;
using UnityEngine.Rendering;

namespace BetterOreMining.Infrastructure
{
    public sealed class OverlayGlowRenderer : IGlowRenderer
    {
        private const int DepthPass = 0;
        private const int RingPass = 1;
        private const float DepthAboveMain = 0.5f;
        private const float CharacterRange = 40f;
        private const CameraEvent DrawEvent = CameraEvent.AfterForwardAlpha;

        private static readonly int CenterId = Shader.PropertyToID("_Center");
        private static readonly int FrontId = Shader.PropertyToID("_Front");
        private static readonly int WidthId = Shader.PropertyToID("_Width");
        private static readonly int OpacityId = Shader.PropertyToID("_Opacity");
        private static readonly int VisibleId = Shader.PropertyToID("_VisibleColor");
        private static readonly int BlockedId = Shader.PropertyToID("_BlockedColor");
        private static readonly int GroundMapId = Shader.PropertyToID("_GroundMap");
        private static readonly int GroundRectId = Shader.PropertyToID("_GroundRect");

        private readonly Material _material;
        private readonly IColorSettings _colors;
        private readonly GroundTextures _groundTextures;
        private readonly CommandBuffer _commands = new CommandBuffer { name = "BetterOreMining" };
        private readonly List<Renderer> _characterParts = new List<Renderer>();
        private readonly HashSet<Renderer> _depthDrawn = new HashSet<Renderer>();

        private IReadOnlyList<RippleWave> _waves = Array.Empty<RippleWave>();
        private float _ringWidth;
        private bool _subscribed;
        private Camera _overlay;

        public OverlayGlowRenderer(
            Shader shader,
            IColorSettings colors,
            GroundTextures groundTextures)
        {
            _material = new Material(shader);
            _colors = colors;
            _groundTextures = groundTextures;
        }

        public void Draw(IReadOnlyList<RippleWave> waves, float ringWidth)
        {
            _waves = waves;
            _ringWidth = ringWidth;
            _groundTextures.KeepOnly(waves);
            if (!_subscribed)
            {
                Camera.onPreCull += OnPreCull;
                _subscribed = true;
            }

            SyncOverlay();
        }

        public void Clear()
        {
            _waves = Array.Empty<RippleWave>();
            _groundTextures.KeepOnly(_waves);
            if (_subscribed)
            {
                Camera.onPreCull -= OnPreCull;
                _subscribed = false;
            }

            if (_overlay != null)
            {
                _overlay.enabled = false;
            }
        }

        private static bool IsShown(Renderer renderer)
        {
            return renderer != null && renderer.enabled && renderer.gameObject.activeInHierarchy;
        }

        private static bool IsMesh(Renderer renderer)
        {
            return renderer is MeshRenderer || renderer is SkinnedMeshRenderer;
        }

        private void SyncOverlay()
        {
            var main = Camera.main;
            if (main == null)
            {
                return;
            }

            if (_overlay == null)
            {
                _overlay = new GameObject("BetterOreMining.Overlay").AddComponent<Camera>();
            }

            if (_overlay.transform.parent != main.transform)
            {
                _overlay.transform.SetParent(main.transform, false);
            }

            _overlay.CopyFrom(main);
            _overlay.clearFlags = CameraClearFlags.Depth;
            _overlay.cullingMask = 0;
            _overlay.renderingPath = RenderingPath.Forward;
            _overlay.depth = main.depth + DepthAboveMain;
            if (_overlay.commandBufferCount == 0)
            {
                _overlay.AddCommandBuffer(DrawEvent, _commands);
            }

            _overlay.enabled = true;
        }

        private void OnPreCull(Camera camera)
        {
            if (camera != _overlay)
            {
                return;
            }

            _commands.Clear();
            _depthDrawn.Clear();
            AddCharacterDepth(camera.transform.position);
            foreach (var wave in _waves)
            {
                AddOreDepth(wave);
            }

            _commands.SetGlobalColor(VisibleId, _colors.VisibleColor);
            _commands.SetGlobalColor(BlockedId, _colors.BlockedColor);
            _commands.SetGlobalFloat(WidthId, _ringWidth);
            foreach (var wave in _waves)
            {
                AddRing(wave);
            }
        }

        private void AddCharacterDepth(Vector3 viewpoint)
        {
            var rangeSquared = CharacterRange * CharacterRange;
            foreach (var character in Character.GetAllCharacters())
            {
                var near = (character.transform.position - viewpoint).sqrMagnitude < rangeSquared;
                if (!near)
                {
                    continue;
                }

                character.GetComponentsInChildren(false, _characterParts);
                foreach (var part in _characterParts)
                {
                    if (IsMesh(part))
                    {
                        DrawIfShown(part, DepthPass);
                    }
                }
            }
        }

        private void AddOreDepth(RippleWave wave)
        {
            foreach (var part in wave.Ore)
            {
                if (_depthDrawn.Add(part))
                {
                    DrawIfShown(part, DepthPass);
                }
            }
        }

        private void AddRing(RippleWave wave)
        {
            var ground = wave.Ground;
            var scale = 1f / ground.Size;
            _commands.SetGlobalVector(CenterId, wave.Center);
            _commands.SetGlobalFloat(FrontId, wave.Front);
            _commands.SetGlobalFloat(OpacityId, wave.Alpha);
            _commands.SetGlobalTexture(GroundMapId, _groundTextures.Get(ground));
            _commands.SetGlobalVector(
                GroundRectId,
                new Vector4(ground.Origin.x, ground.Origin.y, scale, scale));
            foreach (var part in wave.Ore)
            {
                DrawIfShown(part, RingPass);
            }
        }

        private void DrawIfShown(Renderer renderer, int pass)
        {
            if (!IsShown(renderer))
            {
                return;
            }

            var submeshes = renderer.sharedMaterials.Length;
            for (var submesh = 0; submesh < submeshes; submesh++)
            {
                _commands.DrawRenderer(renderer, _material, submesh, pass);
            }
        }
    }
}
