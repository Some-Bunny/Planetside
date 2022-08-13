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
using GungeonAPI;
using Pathfinding;
using NpcApi;
using SaveAPI;

namespace Planetside
{
    public class OrbitalInsertion : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Orbital Insertion";
            string resourceName = "Planetside/Resources/orbitalinsertion.png";
            GameObject obj = new GameObject(itemName);
            OrbitalInsertion activeitem = obj.AddComponent<OrbitalInsertion>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I Need Backup!";
            string longDesc = "Request heavy backup for the current floor. Single Use.\n\nDespite the victory of the Gungeon over the invading forces of the Hegemony, the Hegemonies presence still remains, watching...\n\nWaiting...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 10f);
            activeitem.consumable = true;
            activeitem.quality = PickupObject.ItemQuality.S;

            OrbitalInsertion.OrbitalInsertionID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

			activeitem.SetupUnlockOnCustomFlag(CustomDungeonFlags.HM_PRIME_DEFEATED_T4, true);
		}
		public static GenericLootTable shopInABoxPickupTable;

        public static int OrbitalInsertionID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
           
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (!user) { return false; }
            if (user.IsInCombat | user.IsInMinecart | user.InExitCell) { return false; }
            if (user.CurrentRoom != null && user.CurrentRoom.IsSealed) { return false; }
            return true;
        }

		private ExplosionData KineticBomb = new ExplosionData
		{
			damageRadius = 3.75f,
			damageToPlayer = 2f,
			doDamage = true,
			damage = 5000f,
			doExplosionRing = false,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 100f,
			preventPlayerForce = false,
			explosionDelay = 0f,
			usesComprehensiveDelay = false,
			doScreenShake = true,
			playDefaultSFX = false
		};


		protected override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", user.gameObject);
            GameManager.Instance.StartCoroutine(DeployRobot(user.CurrentRoom, user));

        }
        private IEnumerator DeployRobot(RoomHandler room, PlayerController player)
        {
			IntVector2 position = new IntVector2((int)player.transform.position.x, (int)player.transform.position.y);
			
			for (int e = 0; e < 8; e++)
			{
				GameManager.Instance.StartCoroutine(DeployReticle(position.ToCenterVector2(), 45* e, e));
			}
			float elapsed = 0;
			float Time = 5;
			bool Playsound = false;
			while (elapsed < Time)
			{
				elapsed += BraveTime.DeltaTime;
				if (elapsed > 3.75f && Playsound == false)
				{
					Playsound = true;
					AkSoundEngine.PostEvent("Play_BOSS_RatMech_Whistle_01", player.gameObject);
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("RobotShopkeeperBoss_friendly");
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, position- new IntVector2(1, 0), room, true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.HandleReinforcementFallIntoRoom(-1f);
					CompanionController comp = aiactor.gameActor.GetComponent<CompanionController>();
					comp.Initialize(player);
					aiactor.CompanionOwner = player;
					aiactor.specRigidbody.Reinitialize();
				}
				yield return null;
			}
			GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
			epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(position.ToCenterVector2(), tk2dBaseSprite.Anchor.LowerCenter);
			epicwin.transform.position = position.ToCenterVector2().Quantize(0.0625f);
			epicwin.GetComponent<tk2dBaseSprite>().UpdateZDepth();
			Destroy(epicwin, 8);
			AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", player.gameObject);
			
			ExplosionData defaultSmallExplosionData = StaticExplosionDatas.genericSmallExplosion;
			defaultSmallExplosionData.damageRadius = 6;
			defaultSmallExplosionData.damage = 1000;
			Exploder.Explode(position.ToCenterVector3(1), defaultSmallExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
			yield break;
        }
		private IEnumerator DeployReticle(Vector2 startPosition, float aimDir, float ID)
		{
			GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);
			tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
			component2.transform.position = new Vector3(startPosition.x, startPosition.y, 99999);
			component2.transform.localRotation = Quaternion.Euler(0f, 0f, 90);
			component2.UpdateZDepth();
			component2.HeightOffGround = -2;
			Color laser = new Color(1f, 0f, 0f, 1f);
			component2.sprite.usesOverrideMaterial = true;
			component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
			component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
			component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
			component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
			component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
			component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
			float elapsed = 0;
			float Tim = 1;
			while (elapsed < Tim)
			{
				float t = (float)elapsed / (float)Tim;
				if (component2 != null)
				{
                    component2.dimensions = new Vector2(1000f, 1f);
					if (UnityEngine.Random.value < 0.33f) { GlobalSparksDoer.DoSingleParticle(Vector2.Lerp(startPosition, startPosition + MathToolbox.GetUnitOnCircle(aimDir, 4), t), Vector3.up, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING); }
					component2.transform.position = Vector2.Lerp(startPosition, startPosition + MathToolbox.GetUnitOnCircle(aimDir, 4), t);
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20);
					component2.HeightOffGround = -2;
					component2.renderer.gameObject.layer = 23;
					component2.UpdateZDepth();
				}
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			Vector2 savedPos = component2.transform.position;
			elapsed = 0;
			Tim = 4;
			while (elapsed < Tim)
			{
				float t = (float)elapsed / (float)Tim;
				if (component2 != null)
				{
					aimDir++;
					component2.dimensions = new Vector2(1000f, 1f);
					if (elapsed % 0.2f == 0.1f) { GlobalSparksDoer.DoSingleParticle(Vector2.Lerp(startPosition + MathToolbox.GetUnitOnCircle(aimDir, 4), startPosition + MathToolbox.GetUnitOnCircle(aimDir, 0), t), Vector3.up, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING); }
					component2.transform.position = Vector2.Lerp(startPosition + MathToolbox.GetUnitOnCircle(aimDir, 4), startPosition + MathToolbox.GetUnitOnCircle(aimDir, 0), t);
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 50 * (t*10));
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20 * (5*t));
					component2.HeightOffGround = -2;
					component2.renderer.gameObject.layer = 23;
					component2.UpdateZDepth();
				}
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			Destroy(gameObject);
			yield break;
		}
	}
}

