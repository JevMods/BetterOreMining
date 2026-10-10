using System;
using System.IO;
using UnityEditor;
using UnityEngine.Rendering;

public static class BuildBundle
{
    private const string BundleName = "betteroremining";
    private const string ShaderPath = "Assets/Ripple.shader";

    public static void Build()
    {
        var output = Environment.GetEnvironmentVariable("BUNDLE_OUT");
        Directory.CreateDirectory(output);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
        PlayerSettings.SetGraphicsAPIs(
            BuildTarget.StandaloneWindows64,
            new[]
            {
                GraphicsDeviceType.Direct3D11,
                GraphicsDeviceType.Direct3D12,
                GraphicsDeviceType.Vulkan
            });
        var build = new AssetBundleBuild
        {
            assetBundleName = BundleName,
            assetNames = new[] { ShaderPath }
        };
        var manifest = BuildPipeline.BuildAssetBundles(
            output,
            new[] { build },
            BuildAssetBundleOptions.ForceRebuildAssetBundle,
            BuildTarget.StandaloneWindows64);
        if (manifest == null)
        {
            EditorApplication.Exit(1);
        }
    }
}
