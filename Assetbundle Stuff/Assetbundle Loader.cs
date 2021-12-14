using System;
using System.IO;
using Ionic.Zip;
using UnityEngine;


namespace Planetside
{
	internal class AssetBundleLoader
	{
		public static AssetBundle LoadAssetBundleFromLiterallyAnywhere(string name)
		{
			AssetBundle result = null;
			bool flag = File.Exists(PlanetsideModule.ZipFilePath);
			bool flag2 = flag;
			if (flag2)
			{
				ZipFile zipFile = ZipFile.Read(PlanetsideModule.ZipFilePath);
				bool flag3 = zipFile != null && zipFile.Entries.Count > 0;
				bool flag4 = flag3;
				if (flag4)
				{
					foreach (ZipEntry zipEntry in zipFile.Entries)
					{
						bool flag5 = zipEntry.FileName == name;
						bool flag6 = flag5;
						if (flag6)
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								zipEntry.Extract(memoryStream);
								memoryStream.Seek(0L, SeekOrigin.Begin);
								result = AssetBundle.LoadFromStream(memoryStream);
								global::ETGModConsole.Log("Successfully loaded assetbundle!", false);
								break;
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = File.Exists(PlanetsideModule.FilePath + "/" + name);
				bool flag8 = flag7;
				if (flag8)
				{
					try
					{
						result = AssetBundle.LoadFromFile(Path.Combine(PlanetsideModule.FilePath, name));
						global::ETGModConsole.Log("Successfully loaded assetbundle!", false);
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
