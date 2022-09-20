using System;
using System.IO;
using Ionic.Zip;
using UnityEngine;


namespace Planetside
{
	internal class AssetBundleLoader
	{
		public static AssetBundle LoadAssetBundleFromLiterallyAnywhere(string name, bool logs = false)
		{
			AssetBundle result = null;
			{
				if (File.Exists(PlanetsideModule.FilePathFolder + "/" + name))
				{
					try
					{
						result = AssetBundle.LoadFromFile(Path.Combine(PlanetsideModule.FilePathFolder, name));
						if (logs == true)
                        {
							global::ETGModConsole.Log("Successfully loaded assetbundle!", false);
						}
					}
					catch (Exception ex)
					{
						global::ETGModConsole.Log("Failed loading asset bundle from file.", false);
						global::ETGModConsole.Log(ex.ToString(), false);
					}
				}
				else
				{
					global::ETGModConsole.Log("AssetBundle NOT FOUND!", false);
				}
			}
			return result;
		}
	}
}
