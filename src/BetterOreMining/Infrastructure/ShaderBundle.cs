using System.IO;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    internal static class ShaderBundle
    {
        public static Shader Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                return null;
            }

            var shaders = bundle.LoadAllAssets<Shader>();
            return shaders.Length > 0 && shaders[0].isSupported ? shaders[0] : null;
        }
    }
}
