using System.Collections.Generic;
using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    public sealed class GroundTextures
    {
        private readonly Dictionary<GroundMap, Texture2D> _textures =
            new Dictionary<GroundMap, Texture2D>();
        private readonly List<GroundMap> _stale = new List<GroundMap>();

        public Texture2D Get(GroundMap ground)
        {
            if (!_textures.TryGetValue(ground, out var texture))
            {
                texture = Upload(ground);
                _textures[ground] = texture;
            }

            return texture;
        }

        public void KeepOnly(IReadOnlyList<RippleWave> waves)
        {
            _stale.Clear();
            foreach (var ground in _textures.Keys)
            {
                if (!IsUsed(ground, waves))
                {
                    _stale.Add(ground);
                }
            }

            foreach (var ground in _stale)
            {
                Object.Destroy(_textures[ground]);
                _textures.Remove(ground);
            }
        }

        private static bool IsUsed(GroundMap ground, IReadOnlyList<RippleWave> waves)
        {
            foreach (var wave in waves)
            {
                if (wave.Ground == ground)
                {
                    return true;
                }
            }

            return false;
        }

        private static Texture2D Upload(GroundMap ground)
        {
            var texture = new Texture2D(
                GroundMap.Resolution,
                GroundMap.Resolution,
                TextureFormat.RFloat,
                false,
                true);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.SetPixelData(ground.Heights, 0);
            texture.Apply(false, true);
            return texture;
        }
    }
}
