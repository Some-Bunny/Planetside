using UnityEditor;
using System.IO;

public class BuildAssetBundles {
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles() {
        string assetBundleDirectory = "Assets/AssetBundles";
        if(!Directory.Exists(assetBundleDirectory)) {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        string windowsBundleDirectory = assetBundleDirectory+"/windows";
        if(!Directory.Exists(windowsBundleDirectory)) {
            Directory.CreateDirectory(windowsBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(windowsBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        string osxBundleDirectory = assetBundleDirectory+"/macos";
        if(!Directory.Exists(osxBundleDirectory)) {
            Directory.CreateDirectory(osxBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(osxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);

        string linuxBundleDirectory = assetBundleDirectory+"/linux";
        if(!Directory.Exists(linuxBundleDirectory)) {
            Directory.CreateDirectory(linuxBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(linuxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux);
    }
}
