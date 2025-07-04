using UnityEditor;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections;


public class BuildAssetBundles
{

    [MenuItem("Assets/Build AssetBundles (Windows)")]
    static void B_W()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        string windowsBundleDirectory = assetBundleDirectory + "/windows";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        if (!Directory.Exists(windowsBundleDirectory))
        {
            Directory.CreateDirectory(windowsBundleDirectory);
        }
        var m = BuildPipeline.BuildAssetBundles(windowsBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        Windows = m.GetAllAssetBundles();
        if (!isAll) { DoDelay(); }
    }
    [MenuItem("Assets/Build AssetBundles (Linux)")]
    static void B_L()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        string osxBundleDirectory = assetBundleDirectory + "/macos";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        if (!Directory.Exists(osxBundleDirectory))
        {
            Directory.CreateDirectory(osxBundleDirectory);
        }
        var m = BuildPipeline.BuildAssetBundles(osxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        MacOS = m.GetAllAssetBundles();
        if (!isAll) { DoDelay(); }
    }
    [MenuItem("Assets/Build AssetBundles (MacOS)")]
    static void B_M()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        string linuxBundleDirectory = assetBundleDirectory + "/linux";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        if (!Directory.Exists(linuxBundleDirectory))
        {
            Directory.CreateDirectory(linuxBundleDirectory);
        }
        var m = BuildPipeline.BuildAssetBundles(linuxBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux);
        Linux = m.GetAllAssetBundles();
        if (!isAll) { DoDelay(); }
    }
    public static bool isAll;
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        isAll = true;
        B_W();
        B_L();
        B_M();
        DoDelay();
        isAll = false;
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

        if (Windows != null)
        {
            foreach (var win in Windows)
            {
                UnityEngine.Debug.Log(windowsBundleDirectory + "/" + win);
                UnityEngine.Debug.Log(AssetDatabase.RenameAsset(windowsBundleDirectory + "/" + win, win + "-windows"));
            }
        }
        if (Linux != null)
        {
            foreach (var win in Linux)
            {
                UnityEngine.Debug.Log(linuxBundleDirectory + "/" + win);
                UnityEngine.Debug.Log(AssetDatabase.RenameAsset(linuxBundleDirectory + "/" + win, win + "-linux"));
            }
        }
        if (MacOS != null)
        {
            foreach (var win in MacOS)
            {
                UnityEngine.Debug.Log(osxBundleDirectory + "/" + win);
                UnityEngine.Debug.Log(AssetDatabase.RenameAsset(osxBundleDirectory + "/" + win, win + "-macos"));
            }
        }

    }

}
