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
            if (this.projectile != null)
            {
				AkSoundEngine.PostEvent("Play_Burn", this.projectile.gameObject);
				this.ShockRing(projectile);
                projectile.OnDestruction += this.EndRingEffect;
				lightObject = new GameObject("light");
				AdditionalBraveLight braveLight = lightObject.AddComponent<AdditionalBraveLight>();
				lightObject.transform.position = projectile.sprite.WorldCenter;
				braveLight.LightColor = new Color(1, 0.12f, 0);
				braveLight.LightIntensity = 0f;
				braveLight.LightRadius = 0f;
				lightObject.gameObject.transform.parent = this.projectile.transform;
			}
        }
		private float elapsed;
		public void Update()
        {
			
			if (this.projectile != null)
            {
				if (elapsed <= 1) 
				{ 
					this.elapsed += BraveTime.DeltaTime;
					lightObject.GetComponent<AdditionalBraveLight>().LightIntensity = Mathf.Lerp(0f, 5f, elapsed);
					lightObject.GetComponent<AdditionalBraveLight>().LightRadius = Mathf.Lerp(0f, 4f, elapsed);
				}
				if (!GameManager.Instance.IsPaused) { Exploder.DoDistortionWave(this.projectile.sprite.WorldCenter, 0.025f, 0.25f, 6, 0.5f); }
			}
		}
		private void EndRingEffect(Projectile projectile)
		{
			if (lightObject != null) { Destroy(lightObject); }
			Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 1f, 0.25f, 6, 0.25f);
			AkSoundEngine.PostEvent("Stop_Burn", projectile.gameObject);
			AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", projectile.gameObject);
			DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(EasyGoopDefinitions.FireDef);
			goopManagerForGoopType.TimedAddGoopCircle(projectile.sprite.WorldCenter, 4f, 0.8f, false);
		}
		private HeatIndicatorController m_radialIndicator;

		private void ShockRing(Projectile projectile)
		{
			this.m_radialIndicator = (UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, projectile.sprite.WorldCenter, Quaternion.identity, projectile.transform)).GetComponent<HeatIndicatorController>();
			this.m_radialIndicator.CurrentColor = Color.red.WithAlpha(0f);
			this.m_radialIndicator.IsFire = true;
			this.m_radialIndicator.CurrentRadius = 4f;
		}
		private GameObject lightObject;
		private Projectile projectile;
	}
}

