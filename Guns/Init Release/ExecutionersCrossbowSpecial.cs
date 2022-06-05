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
	internal class ExecutionersCrossbowSpecial : MonoBehaviour
	{

		public static void Init()
		{
			ExecutionersCrossbowSpecial.LockInVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/LockedIn/lockedin", null, false);
			ExecutionersCrossbowSpecial.LockInVFXPrefab.name = "LockInVFX";
			UnityEngine.Object.DontDestroyOnLoad(ExecutionersCrossbowSpecial.LockInVFXPrefab);
			FakePrefab.MarkAsFakePrefab(ExecutionersCrossbowSpecial.LockInVFXPrefab);
			ExecutionersCrossbowSpecial.LockInVFXPrefab.SetActive(false);
		}
		private static GameObject LockInVFXPrefab;
		public void Start()
		{

			this.projectile = base.GetComponent<Projectile>();
			this.player = (this.projectile.Owner as PlayerController);

			Projectile projectile = this.projectile;
			PlayerController playerController = projectile.Owner as PlayerController;
			if (projectile != null)
			{
				AkSoundEngine.PostEvent("Play_WPN_woodbow_shot_02", base.gameObject);
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(208, 255, 223, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 100);
				projectile.sprite.renderer.material = mat;
				projectile.OnDestruction += DestroyChain;
				projectile.OnHitEnemy += this.HandleHit;
			}
		}
		private void HandleHit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			//PlayerController player = GameManager.Instance.PrimaryPlayer;
			if (arg2.aiActor != null && !arg2.healthHaver.IsBoss)
			{
				GameObject original;
				original = ExecutionersCrossbowSpecial.LockInVFXPrefab;
				tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(original, arg2.transform).GetComponent<tk2dSprite>();
				component.name = "pain";
				component.PlaceAtPositionByAnchor(arg2.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.one;
				AkSoundEngine.PostEvent("Play_SND_OBJ_chainpot_drop_01", arg2.gameObject);
				arg2.aiActor.gameActor.ApplyEffect(this.LockIn, 1f, null);
				arg2.aiActor.gameObject.AddComponent<ThefuckOffChainToThePlayer>();
			}
		}

		public void Update()
        {
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			bool flag = this.projectile != null;
			if (flag)
			{
				GameObject nuts = FakePrefab.Clone((PickupObjectDatabase.GetById(29) as Gun).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab);
				if (nuts == null)
				{
					nuts = FakePrefab.Clone((PickupObjectDatabase.GetById(29) as Gun).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab);
				}
				if (player && this.extantLink == null)
				{
					tk2dTiledSprite component = SpawnManager.SpawnVFX(nuts, false).GetComponent<tk2dTiledSprite>();
					this.extantLink = component;
				}
				else if (player && this.extantLink != null && this.projectile != null)
				{
					UpdateLink(player, this.extantLink);
				}
				else if (extantLink != null)
				{
					SpawnManager.Despawn(extantLink.gameObject);
					extantLink = null;
				}
			}
			else
			{
				SpawnManager.Despawn(extantLink.gameObject);
				extantLink = null;
			}
		}
		private void DestroyChain(Projectile projectile)
		{
			SpawnManager.Despawn(extantLink.gameObject);
			extantLink = null;
		}
		private tk2dTiledSprite extantLink;

		private void UpdateLink(PlayerController target, tk2dTiledSprite m_extantLink)
		{
			SpeculativeRigidbody specRigidbody = this.projectile.specRigidbody;
			SpeculativeRigidbody speculativeRigidbody = specRigidbody;
			Vector2 unitCenter = speculativeRigidbody.UnitTopCenter;
			Vector2 unitCenter2 = target.specRigidbody.HitboxPixelCollider.UnitCenter;
			m_extantLink.transform.position = unitCenter;
			Vector2 vector = unitCenter2 - unitCenter;
			float num = BraveMathCollege.Atan2Degrees(vector.normalized);
			int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
			m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
			m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
			m_extantLink.UpdateZDepth();

		}

		public AIActorDebuffEffect LockIn = new AIActorDebuffEffect
		{
			SpeedMultiplier = 0.01f,
			KeepHealthPercentage = true,
			AppliesTint = true,
			TintColor = new Color(0.8f, 1, 0.9f),
			duration = 600f
		};
		private Projectile projectile;
		private PlayerController player;
	}
	internal class ThefuckOffChainToThePlayer : MonoBehaviour
    {
		public void Start()
		{
			this.aiactor = base.GetComponent<AIActor>();
			AIActor aIActor = this.aiactor;
			bool flag = aIActor != null;
			bool flag2 = flag;
			if (flag2)
			{
                aIActor.healthHaver.OnDeath += HealthHaver_OnPreDeath;
			}
		}

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
			SpawnManager.Despawn(extantLink.gameObject);
			extantLink = null;
		}

        public void Update()
		{
			PlayerController player = GameManager.Instance.PrimaryPlayer;
			bool flag = this.aiactor != null;
			if (flag)
			{
				GameObject cunt = FakePrefab.Clone((PickupObjectDatabase.GetById(29) as Gun).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab);
				if (cunt == null)
				{
					cunt = FakePrefab.Clone((PickupObjectDatabase.GetById(29) as Gun).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab);
				}
				if (player && this.extantLink == null)
				{
					tk2dTiledSprite component = SpawnManager.SpawnVFX(cunt, false).GetComponent<tk2dTiledSprite>();
					this.extantLink = component;
				}
				else if (player && this.extantLink != null && this.aiactor != null)
				{
					UpdateLink(player, this.extantLink);
				}
				else if (extantLink != null)
				{
					SpawnManager.Despawn(extantLink.gameObject);
					extantLink = null;
				}
			}
			else
            {
				SpawnManager.Despawn(extantLink.gameObject);
				extantLink = null;
			}

		}
		private void UpdateLink(PlayerController target, tk2dTiledSprite m_extantLink)
		{
			SpeculativeRigidbody specRigidbody = this.aiactor.specRigidbody;
			SpeculativeRigidbody speculativeRigidbody = specRigidbody;
			Vector2 unitCenter = speculativeRigidbody.UnitTopCenter;
			Vector2 unitCenter2 = target.specRigidbody.HitboxPixelCollider.UnitCenter;
			m_extantLink.transform.position = unitCenter;
			Vector2 vector = unitCenter2 - unitCenter;
			float num = BraveMathCollege.Atan2Degrees(vector.normalized);
			int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
			m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
			m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
			m_extantLink.UpdateZDepth();

		}

		private tk2dTiledSprite extantLink;
		private AIActor aiactor;

	}

}

