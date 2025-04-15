using UnityEditor;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections;


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
        var m = BuildPipeline.BuildAssetBundles(windowsBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Windows = m.GetAllAssetBundles();



        string osxBundleDirectory = assetBundleDirectory+"/macos";
        if(!Directory.Exists(osxBundleDirectory)) {
            Directory.CreateDirectory(osxBundleDirectory);
        }
        m = BuildPipeline.BuildAssetBundles(osxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        MacOS = m.GetAllAssetBundles();

        string linuxBundleDirectory = assetBundleDirectory+"/linux";
        if(!Directory.Exists(linuxBundleDirectory)) {
            Directory.CreateDirectory(linuxBundleDirectory);
        }
        m = BuildPipeline.BuildAssetBundles(linuxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux);
        Linux = m.GetAllAssetBundles();

        DoDelay();
    }
    private static string[] Windows;
    private static string[] Linux;
    private static string[] MacOS;


    public static void DoDelay()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        string windowsBundleDirectory = assetBundleDirectory + "/windows";
        string osxBundleDirectory = assetBundleDirectory + "/macos";
        string linuxBundleDirectory = assetBundleDirectory + "/linux";

        foreach (var win in Windows) 
        {
            UnityEngine.Debug.Log(windowsBundleDirectory + "/" + win);
            UnityEngine.Debug.Log(AssetDatabase.RenameAsset(windowsBundleDirectory+"/"+ win, win + "-windows"));
        }
        foreach (var win in Linux)
        {
            UnityEngine.Debug.Log(linuxBundleDirectory + "/" + win);
            UnityEngine.Debug.Log(AssetDatabase.RenameAsset(linuxBundleDirectory + "/" + win, win + "-linux"));
        }
        foreach (var win in MacOS)
        {
            UnityEngine.Debug.Log(osxBundleDirectory + "/" + win);
            UnityEngine.Debug.Log(AssetDatabase.RenameAsset(osxBundleDirectory + "/" + win, win + "-macos"));
        }
    }

}
