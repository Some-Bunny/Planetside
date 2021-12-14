using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;

namespace Planetside
{
	// Token: 0x02000060 RID: 96
	internal class EasyGoopDefinitions
	{
		// Token: 0x0600026A RID: 618 RVA: 0x00015DC0 File Offset: 0x00013FC0
		public static void DefineDefaultGoops()
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			EasyGoopDefinitions.goopDefs = new List<GoopDefinition>();
			foreach (string text in EasyGoopDefinitions.goops)
			{
				GoopDefinition goopDefinition;
				try
				{
					GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
					goopDefinition = gameObject.GetComponent<GoopDefinition>();
				}
				catch
				{
					goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
				}
				goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
				EasyGoopDefinitions.goopDefs.Add(goopDefinition);
			}
			List<GoopDefinition> list = EasyGoopDefinitions.goopDefs;
			EasyGoopDefinitions.FireDef = EasyGoopDefinitions.goopDefs[0];
			EasyGoopDefinitions.OilDef = EasyGoopDefinitions.goopDefs[1];
			EasyGoopDefinitions.PoisonDef = EasyGoopDefinitions.goopDefs[2];
			EasyGoopDefinitions.BlobulonGoopDef = EasyGoopDefinitions.goopDefs[3];
			EasyGoopDefinitions.WebGoop = EasyGoopDefinitions.goopDefs[4];
			EasyGoopDefinitions.WaterGoop = EasyGoopDefinitions.goopDefs[5];
			EasyGoopDefinitions.FireDef2 = EasyGoopDefinitions.goopDefs[6];

		}

		// Token: 0x0600026C RID: 620 RVA: 0x00015F84 File Offset: 0x00014184
		// Note: this type is marked as 'beforefieldinit'.
		static EasyGoopDefinitions()
		{
			PickupObject byId = PickupObjectDatabase.GetById(310);
			GoopDefinition charmGoopDef;
			if (byId == null)
			{
				charmGoopDef = null;
			}
			else
			{
				WingsItem component = byId.GetComponent<WingsItem>();
				charmGoopDef = ((component != null) ? component.RollGoop : null);
			}
			EasyGoopDefinitions.CharmGoopDef = charmGoopDef;
			EasyGoopDefinitions.GreenFireDef = (PickupObjectDatabase.GetById(698) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
			EasyGoopDefinitions.CheeseDef = (PickupObjectDatabase.GetById(808) as Gun).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
			EasyGoopDefinitions.TripleCrossbow = (ETGMod.Databases.Items["triple_crossbow"] as Gun);
			EasyGoopDefinitions.TripleCrossbowEffect = EasyGoopDefinitions.TripleCrossbow.DefaultModule.projectiles[0].speedEffect;
			
		}

		public static string[] goops = new string[]
		{
			"assets/data/goops/napalmgoopthatworks.asset",
			"assets/data/goops/oil goop.asset",
			"assets/data/goops/poison goop.asset",
			"assets/data/goops/blobulongoop.asset",
			"assets/data/goops/phasewebgoop.asset",
			"assets/data/goops/water goop.asset",
			"assets/data/goops/napalmgoopquickignite.asset"
		};

		private static List<GoopDefinition> goopDefs;

		public static GoopDefinition FireDef;
		public static GoopDefinition FireDef2;

		public static GoopDefinition OilDef;

		public static GoopDefinition PoisonDef;

		public static GoopDefinition BlobulonGoopDef;

		// Token: 0x040000D1 RID: 209
		public static GoopDefinition WebGoop;

		// Token: 0x040000D2 RID: 210
		public static GoopDefinition WaterGoop;

		// Token: 0x040000D3 RID: 211

		// Token: 0x040000D4 RID: 212
		public static GoopDefinition CharmGoopDef;

		// Token: 0x040000D5 RID: 213
		public static GoopDefinition GreenFireDef;

		// Token: 0x040000D6 RID: 214
		public static GoopDefinition CheeseDef;

		// Token: 0x040000D7 RID: 215
		private static Gun TripleCrossbow;

		// Token: 0x040000D8 RID: 216
		private static GameActorSpeedEffect TripleCrossbowEffect;

		// Token: 0x040000D9 RID: 217
	}
}
