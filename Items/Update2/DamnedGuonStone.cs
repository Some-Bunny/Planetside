using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace Planetside
{
	internal class DamnedGuonStone : IounStoneOrbitalItem
	{
		public static void Init()
		{
			string name = "Damned Guon Stone";
			//string resourcePath = "Planetside/Resources/Guons/DamnedGuon/damnedstone.png";
			GameObject gameObject = new GameObject();
			DamnedGuonStone woodGuonStone = gameObject.AddComponent<DamnedGuonStone>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("damnedstone"), data, gameObject);

            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Laced With Curse";
			string longDesc = "A guon stone bathed in the cursed pots scattered around the Gungeon. They reek of evils.";
			woodGuonStone.SetupItem(shortDesc, longDesc, "psog");
			woodGuonStone.quality = PickupObject.ItemQuality.EXCLUDED;
			DamnedGuonStone.BuildPrefab();
			woodGuonStone.OrbitalPrefab = DamnedGuonStone.orbitalPrefab;
			woodGuonStone.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;
			woodGuonStone.CanBeDropped = false;
			woodGuonStone.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
		}

		public static void BuildPrefab()
		{
			bool flag = DamnedGuonStone.orbitalPrefab != null;
			if (!flag)
			{
				GameObject deathmark = ItemBuilder.AddSpriteToObject("cursed_guon", "Planetside/Resources/Guons/DamnedGuon/curseguonfloaty_001", null);
				FakePrefab.MarkAsFakePrefab(deathmark);
				UnityEngine.Object.DontDestroyOnLoad(deathmark);
				tk2dSpriteAnimator animator = deathmark.AddComponent<tk2dSpriteAnimator>();
				tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
				animationClip.fps = 3;
				animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
				animationClip.name = "start";

				GameObject spriteObject = new GameObject("spriteObject");
				ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Guons/DamnedGuon/curseguonfloaty_001", spriteObject);
				tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
				starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
				starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
				tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
				{
				starterFrame
				};
				animationClip.frames = frameArray;
				for (int i = 2; i < 4; i++)
				{
					GameObject spriteForObject = new GameObject("spriteForObject");
					ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Guons/DamnedGuon/curseguonfloaty_00{i}", spriteForObject);
					tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
					frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
					frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
					animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
				}
				animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
				animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
				animator.DefaultClipId = animator.GetClipIdByName("start");
				animator.playAutomatically = true;



				GameObject gameObject = animator.gameObject;
				gameObject.name = "Damned Guon Orbital";
				SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 13));
				speculativeRigidbody.CollideWithTileMap = false;
				speculativeRigidbody.CollideWithOthers = true;
				speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
				DamnedGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
				DamnedGuonStone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
				DamnedGuonStone.orbitalPrefab.shouldRotate = false;
				DamnedGuonStone.orbitalPrefab.orbitRadius = 3f;
				DamnedGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
				DamnedGuonStone.orbitalPrefab.SetOrbitalTier(0);
				ImprovedAfterImage yeah = gameObject.AddComponent<ImprovedAfterImage>();
				yeah.dashColor = Color.black;
				yeah.spawnShadows = true;
				yeah.shadowTimeDelay = 0.01f;
				yeah.shadowLifetime = 0.5f;

				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				FakePrefab.MarkAsFakePrefab(gameObject);
				gameObject.SetActive(false);
			}
		}

		public override void Pickup(PlayerController player)
		{
			//player.OnHitByProjectile = (Action<Projectile, PlayerController>)Delegate.Combine(player.OnHitByProjectile, new Action<Projectile, PlayerController>(this.OwnerHitByProjectile));
			//player.OnReceivedDamage += this.OnOwnerTookDamage;
			DamnedGuonStone.guonHook = new Hook(typeof(PlayerOrbital).GetMethod("Initialize"), typeof(DamnedGuonStone).GetMethod("GuonInit"));
			base.Pickup(player);
			//base.Invoke("breakThis", 15f);
		}

		private void OnOwnerTookDamage(PlayerController user)
		{
		}

		public override DebrisObject Drop(PlayerController player)
		{
			//player.OnHitByProjectile = (Action<Projectile, PlayerController>)Delegate.Remove(player.OnHitByProjectile, new Action<Projectile, PlayerController>(this.OwnerHitByProjectile));
			//player.OnReceivedDamage -= this.OnOwnerTookDamage;
			DamnedGuonStone.guonHook.Dispose();
			DamnedGuonStone.speedUp = false;
			return base.Drop(player);
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000439C0 File Offset: 0x00041BC0
		protected override void OnDestroy()
		{
			if (base.Owner != null)
            {
				//PlayerController owner = base.Owner;
				//owner.OnHitByProjectile = (Action<Projectile, PlayerController>)Delegate.Remove(owner.OnHitByProjectile, new Action<Projectile, PlayerController>(this.OwnerHitByProjectile));
				base.Owner.OnReceivedDamage -= this.OnOwnerTookDamage;
				DamnedGuonStone.guonHook.Dispose();
				DamnedGuonStone.speedUp = false;
				base.OnDestroy();
			}
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x00043A25 File Offset: 0x00041C25
		public static void GuonInit(Action<PlayerOrbital, PlayerController> orig, PlayerOrbital self, PlayerController player)
		{
			orig(self, player);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x00043A31 File Offset: 0x00041C31
		private void OwnerHitByProjectile(Projectile incomingProjectile, PlayerController arg2)
		{
		}

		private void breakThis()
		{
			base.Owner.RemovePassiveItem(this.PickupObjectId);
		}
		public static Hook guonHook;
		public static bool speedUp = false;
		public static PlayerOrbital orbitalPrefab;
	}
}
