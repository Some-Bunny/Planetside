using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;


namespace Planetside
{
	// Token: 0x0200008A RID: 138
	public class PlanetsideRainPlaceable : DungeonPlaceableBehaviour, IPlaceConfigurable
	{
		private PlanetsideRainPlaceable()
		{
			this.RainIntensity = 100f;
			this.useCustomIntensity = true;
			this.enableLightning = false;
			this.isSecretFloor = true;
		}

		public void Start()
		{
		}

		public void Update()
		{
		}

		public void ConfigureOnPlacement(RoomHandler room)
		{
			PlanetsideWeatherController expandWeatherController = GameManager.Instance.Dungeon.gameObject.AddComponent<PlanetsideWeatherController>();
			expandWeatherController.RainIntensity = this.RainIntensity;
			expandWeatherController.useCustomIntensity = this.useCustomIntensity;
			expandWeatherController.enableLightning = this.enableLightning;
			expandWeatherController.isSecretFloor = this.isSecretFloor;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}
		public bool useCustomIntensity;
		public float RainIntensity;
		public bool enableLightning;
		public bool isSecretFloor;
	}
}


namespace Planetside
{
	public class PlanetsideWeatherController : BraveBehaviour
	{
	//This is basically all Expand code. Thanks Apache!
		public PlanetsideWeatherController()
		{
			this.isActive = false;
			this.isSecretFloor = true;
			this.MinTimeBetweenLightningStrikes = 5f;
			this.MaxTimeBetweenLightningStrikes = 10f;
			this.AmbientBoost = 1f;
			this.RainIntensity = 250f;
			this.useCustomIntensity = true;
			this.enableLightning = false;
			this.isLocalToRoom = false;
			this.ThunderShake = new ScreenShakeSettings
			{
				magnitude = 0.2f,
				speed = 3f,
				time = 0f,
				falloff = 0.5f,
				direction = new Vector2(1f, 0f),
				vibrationType = ScreenShakeSettings.VibrationType.Auto,
				simpleVibrationTime = Vibration.Time.Normal,
				simpleVibrationStrength = Vibration.Strength.Medium
			};
			this.m_lightningTimer = UnityEngine.Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
		}

		private void Start()
		{
			var hunterPast = DungeonDatabase.GetOrLoadByName("finalscenario_guide");
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(hunterPast.PatternSettings.flows[0].AllNodes[0].overrideExactRoom.placedObjects[0].nonenemyBehaviour.gameObject.transform.Find("Rain").gameObject);
			hunterPast = null;
			gameObject.name = "PlanetsideRain";
			this.m_StormController = gameObject.GetComponent<ThunderstormController>();
			ParticleSystem component = this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>();
			if (this.useCustomIntensity)
			{
				BraveUtility.SetEmissionRate(component, this.RainIntensity);
			}
			this.m_StormController.DecayVertical = this.isLocalToRoom;
			this.m_StormController.DoLighting = false;
			this.LightningRenderers = this.m_StormController.LightningRenderers;
			gameObject.transform.parent = base.gameObject.transform;
			this.isActive = true;
		}

		private void Update()
		{
			if (GameManager.Instance.IsLoadingLevel)
			{
				return;
			}
			if (this.isSecretFloor)
			{
				PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
				if (primaryPlayer)
				{
					this.CheckForWeatherFX(primaryPlayer, this.RainIntensity);
				}
			}
			if (!this.isActive)
			{
				return;
			}
			if (this.enableLightning)
			{
				this.m_lightningTimer -= ((!GameManager.IsBossIntro) ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME);
				if (this.m_lightningTimer <= 0f && this.isActive)
				{
					base.StartCoroutine(this.DoLightningStrike());
					if (this.LightningRenderers != null)
					{
						for (int i = 0; i < this.LightningRenderers.Length; i++)
						{
							base.StartCoroutine(this.ProcessLightningRenderer(this.LightningRenderers[i]));
						}
					}
					base.StartCoroutine(this.HandleLightningAmbientBoost());
					this.m_lightningTimer = UnityEngine.Random.Range(this.MinTimeBetweenLightningStrikes, this.MaxTimeBetweenLightningStrikes);
				}
			}
		}




		public void CheckForWeatherFX(PlayerController player, float RainIntensity)
		{
			try
			{
				GameManager.Instance.StartCoroutine(this.ToggleRainFX(player, RainIntensity));
			}
			catch (Exception exception)
			{
				ETGModConsole.Log(exception.ToString());
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x000B2604 File Offset: 0x000B0804
		private IEnumerator ToggleRainFX(PlayerController player, float cachedRate)
		{
			if (!this.m_StormController)
			{
				yield break;
			}
			bool Active = true;
			if ( player.CurrentRoom.IsShop | player.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
			{
				Active = false;
			}
			yield return null;
			if (!Active)
			{
				if (!this.m_StormController.enabled && !this.isActive)
				{
					yield break;
				}
				AkSoundEngine.PostEvent("Stop_ENV_rain_loop_01", this.m_StormController.gameObject);
				BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), 0f);
				yield return new WaitForSeconds(2f);
				this.m_StormController.enabled = false;
				this.isActive = false;
				yield return null;
				yield break;
			}
			else
			{
				if (this.m_StormController.enabled && this.isActive)
				{
					yield break;
				}
				BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), cachedRate);
				this.m_StormController.enabled = true;
				this.isActive = true;
				yield break;
			}
		}

		public void ForceStopRain(bool DestroySelf, string Name = "PlanetsideRain")
        {
			GameManager.Instance.StartCoroutine(DisableRain(DestroySelf, Name));
		}
		private IEnumerator DisableRain(bool DestroySelf, string Name)
		{
			if (!this.m_StormController)
			{
				yield break;
			}
			if (!this.m_StormController.enabled && !this.isActive)
			{
				yield break;
			}
			AkSoundEngine.PostEvent("Stop_ENV_rain_loop_01", this.m_StormController.gameObject);
			BraveUtility.SetEmissionRate(this.m_StormController.RainSystemTransform.GetComponent<ParticleSystem>(), 0f);
			//yield return new WaitForSeconds(0.1f);
			this.m_StormController.enabled = false;
			this.isActive = false;
			if (DestroySelf)
            {
				PlanetsideWeatherController expandWeatherController = GameManager.Instance.Dungeon.gameObject.GetComponent<PlanetsideWeatherController>();
				if (expandWeatherController)
				{
					if (expandWeatherController.name == Name)
                    {
						Destroy(expandWeatherController);
                    }
				}
			}
			yield return null;
			yield break;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x000B2621 File Offset: 0x000B0821
		public IEnumerator HandleLightningAmbientBoost()
		{
			Color cachedAmbient = RenderSettings.ambientLight;
			Color modAmbient = new Color(cachedAmbient.r + this.AmbientBoost, cachedAmbient.g + this.AmbientBoost, cachedAmbient.b + this.AmbientBoost);
			GameManager.Instance.Dungeon.OverrideAmbientLight = true;
			int num;
			for (int i = 0; i < 2; i = num + 1)
			{
				float elapsed = 0f;
				float duration = 0.15f * (float)(i + 1);
				while (elapsed < duration)
				{
					elapsed += GameManager.INVARIANT_DELTA_TIME;
					float t = elapsed / duration;
					GameManager.Instance.Dungeon.OverrideAmbientColor = Color.Lerp(modAmbient, cachedAmbient, t);
					yield return null;
				}
				num = i;
			}
			yield return null;
			GameManager.Instance.Dungeon.OverrideAmbientLight = false;
			yield break;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x000B2630 File Offset: 0x000B0830
		public IEnumerator ProcessLightningRenderer(Renderer target)
		{
			target.enabled = true;
			yield return this.StartCoroutine(this.InvariantWait(0.05f));
			target.enabled = false;
			yield return this.StartCoroutine(this.InvariantWait(0.1f));
			target.enabled = true;
			yield return this.StartCoroutine(this.InvariantWait(0.1f));
			target.enabled = false;
			yield break;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x000B2646 File Offset: 0x000B0846
		public IEnumerator InvariantWait(float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += GameManager.INVARIANT_DELTA_TIME;
				yield return null;
			}
			yield break;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x000B2655 File Offset: 0x000B0855
		public IEnumerator DoLightningStrike()
		{
			AkSoundEngine.PostEvent("Play_ENV_thunder_flash_01", GameManager.Instance.PrimaryPlayer.gameObject);
			PlatformInterface.SetAlienFXColor(new Color(1f, 1f, 1f, 1f), 0.25f);
			yield return new WaitForSeconds(0.25f);
			GameManager.Instance.MainCameraController.DoScreenShake(this.ThunderShake, null, false);
			yield break;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0000C31A File Offset: 0x0000A51A
		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x0400066A RID: 1642
		public bool isActive;

		// Token: 0x0400066B RID: 1643
		public bool isSecretFloor;

		// Token: 0x0400066C RID: 1644
		public float MinTimeBetweenLightningStrikes;

		// Token: 0x0400066D RID: 1645
		public float MaxTimeBetweenLightningStrikes;

		// Token: 0x0400066E RID: 1646
		public float AmbientBoost;

		// Token: 0x0400066F RID: 1647
		public float RainIntensity;

		// Token: 0x04000670 RID: 1648
		public bool useCustomIntensity;

		// Token: 0x04000671 RID: 1649
		public bool enableLightning;

		// Token: 0x04000672 RID: 1650
		public bool isLocalToRoom;

		// Token: 0x04000673 RID: 1651
		public ScreenShakeSettings ThunderShake;

		// Token: 0x04000674 RID: 1652
		public Renderer[] LightningRenderers;

		// Token: 0x04000675 RID: 1653
		private ThunderstormController m_StormController;

		// Token: 0x04000676 RID: 1654
		private float m_lightningTimer;
	}
}
