using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

namespace Planetside
{
	internal class BurningSunProjectile : MonoBehaviour
	{
		public BurningSunProjectile()
		{
		}
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                this.ShockRing(projectile);
                projectile.OnDestruction += this.EndRingEffect;
            }
        }
		private void EndRingEffect(Projectile projectile)
		{
			AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
			GoopDefinition goopDefinition = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDefinition);
			goopManagerForGoopType.TimedAddGoopCircle(projectile.sprite.WorldCenter, 4f, 0.8f, false);
		}
		private HeatIndicatorController m_radialIndicator;

		private void ShockRing(Projectile projectile)
		{
			this.m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
			this.m_radialIndicator.CurrentColor = Color.red.WithAlpha(0f);
			this.m_radialIndicator.IsFire = true;
			this.m_radialIndicator.CurrentRadius = 4f;
		}

		private Projectile projectile;
	}
}

