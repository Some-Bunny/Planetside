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
using Brave.BulletScript;

namespace Planetside
{
	public class TrapDefusalKit : PassiveItem
	{
		public static void Init()
		{
			string itemName = "Trap Defusal Kit";
			//string resourceName = "Planetside/Resources/trapdefusalkit.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<TrapDefusalKit>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("trapdefusalkit"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Red Or Blue?";
			string longDesc = "Masterfully disables all sorts of traps.\n\nHolds a screwdriver, a pair of pliers, a wire-cutter, a booklet on bomb defusal and a half-full bottle of vodka.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;
			TrapDefusalKit.TrapDefusalKitID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
            item.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Trorc, 1);
            item.AddToSubShop(ItemAPI.ItemBuilder.ShopType.Cursula, 0.5f);

            new Hook(typeof(ProjectileTrapController).GetMethod("ShootProjectileInDirection", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("ShootProjectileInDirectionHook"));
			new Hook(typeof(BasicTrapController).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(TrapDefusalKit).GetMethod("UpdateHook"));
			new Hook(typeof(CartTurretController).GetMethod("Fire", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("FireHook"));		
			new Hook(typeof(PathMover).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(TrapDefusalKit).GetMethod("UpdateHookPathingTrapController"));
			new Hook(typeof(PathingTrapController).GetMethod("UpdateSparks", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("UpdateSparksHook"));
			new Hook(typeof(PathingTrapController).GetMethod("Damage", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("DamageHook"));
			new Hook(typeof(ForgeHammerController).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(TrapDefusalKit).GetMethod("UpdateHookForgeHammerController"));
			new Hook(typeof(ForgeCrushDoorController).GetMethod("HandleTrigger", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("HandleTriggerHook"));
			new Hook(typeof(ForgeFlamePipeController).GetMethod("HandleRigidbodyCollision", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("HandleRigidbodyCollisionHook"));
			new Hook(typeof(ForgeFlamePipeController).GetMethod("HandleBeamCollision", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("HandleBeamCollisionHook"));
			new Hook(typeof(SpawnGoopBehavior).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(TrapDefusalKit).GetMethod("UpdateGoopHook"));
			new Hook(typeof(HighPriestSimpleMergoBehavior).GetMethod("ShootWallBulletScript", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("ShootWallBulletScriptHook"));
            new Hook(typeof(TrapEnemyConfigurator).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), typeof(TrapDefusalKit).GetMethod("UpdateTrapEnemyConfiguratorHook"));
            new Hook(typeof(PowderSkullSpinBulletsBehavior).GetMethod("ContinuousUpdate", BindingFlags.Instance | BindingFlags.Public), typeof(TrapDefusalKit).GetMethod("ContinuousUpdatePowderSkullSpinBulletsBehaviorHook"));

        }
        public static ContinuousBehaviorResult ContinuousUpdatePowderSkullSpinBulletsBehaviorHook(Func<PowderSkullSpinBulletsBehavior, ContinuousBehaviorResult> orig, PowderSkullSpinBulletsBehavior self)
        {
			if (self.m_aiActor == null)
			{
                bool shouldBe = TrapsShouldBeDefused();
                if (shouldBe == true)
                {
                    for (int i = 0; i < self.m_projectiles.Count; i++)
                    {
                        Projectile projectile = self.m_projectiles[i].projectile;
                        if (projectile != null)
                        {
                            projectile.DieInAir(false, true, true, false);
                        }
                    }
                    return ContinuousBehaviorResult.Continue;
                }

            }	
            return orig(self);
        }

        public static void UpdateTrapEnemyConfiguratorHook(Action<TrapEnemyConfigurator> orig, TrapEnemyConfigurator self)
        {
            if (TrapsShouldBeDefused() == true && self.m_isActive == true)
            {
				self.m_isActive = false;
                self.behaviorSpeculator.enabled = false;
                return;
            }
            else 
			{
				if (self.m_isActive == false && GameManager.Instance.IsAnyPlayerInRoom(self.m_parentRoom))
				{
                    self.m_isActive = true;
                    self.behaviorSpeculator.enabled = true;
                }
				orig(self); 
			}
        }

        public static void ShootWallBulletScriptHook(Action<HighPriestSimpleMergoBehavior> orig, HighPriestSimpleMergoBehavior self)
		{
			if (TrapsShouldBeDefused() == true)
			{
				return;
			}
			else { orig(self);}
		}
		public static BehaviorResult UpdateGoopHook(Func<SpawnGoopBehavior, BehaviorResult> orig, SpawnGoopBehavior self)
        {
			AIActor host = PlanetsideReflectionHelper.ReflectGetField<AIActor>(typeof(SpawnGoopBehavior), "m_aiActor", self);
            if (host != null && host.EnemyGuid == "6868795625bd46f3ae3e4377adce288b" && TrapsShouldBeDefused() == true)
			{
				return BehaviorResult.Continue;
			}		
			return orig(self);
        }
		public static void HandleRigidbodyCollisionHook(Action<ForgeFlamePipeController, CollisionData> orig, ForgeFlamePipeController self, CollisionData rigidbodyCollision)
		{
			if (TrapsShouldBeDefused() == true) { return; }
			else { orig(self, rigidbodyCollision); }
		}
		public static void HandleBeamCollisionHook(Action<ForgeFlamePipeController, BeamController> orig, ForgeFlamePipeController self, BeamController beamController)
		{
			if (TrapsShouldBeDefused() == true) { return; }
			else { orig(self, beamController); }
		}
		public static void DamageHook(Action<PathingTrapController, SpeculativeRigidbody, Single, Single> orig, PathingTrapController self, SpeculativeRigidbody rigidbody, float damage, float knockbackStrength)
        {
			if (TrapsShouldBeDefused() == true)
            {
				if (knockbackStrength > 0f && rigidbody.knockbackDoer){rigidbody.knockbackDoer.ApplySourcedKnockback(rigidbody.UnitCenter - self.specRigidbody.UnitCenter, knockbackStrength*0.5f, self.gameObject, false);}
				return;
            }
			orig(self, rigidbody, damage, knockbackStrength);
		}
		public static void UpdateSparksHook(Action<PathingTrapController> orig, PathingTrapController self)
        {
			if (TrapsShouldBeDefused() == true)
			{
				if (self.Sparks_A != null && self.Sparks_A.gameObject.activeSelf == true){self.Sparks_A.gameObject.SetActive(false);}
				if (self.Sparks_B != null && self.Sparks_B.gameObject.activeSelf == true){self.Sparks_B.gameObject.SetActive(false);}
				if (self.spriteAnimator != null){self.spriteAnimator.Stop();}
				self.enabled = false;
				AkSoundEngine.PostEvent("Stop_ENV_trap_active", self.gameObject);
				return;
			}
			else 
			{  if (self.enabled == false)
                {
					self.enabled = true;
					AkSoundEngine.PostEvent("Play_ENV_trap_active", self.gameObject);
                    if (self.spriteAnimator != null) { self.spriteAnimator.Play(); }
                    if (self.Sparks_A != null && self.Sparks_A.gameObject.activeSelf == false) { self.Sparks_A.gameObject.SetActive(true); }
                    if (self.Sparks_B != null && self.Sparks_B.gameObject.activeSelf == false) { self.Sparks_B.gameObject.SetActive(true); }
                }
                orig(self);
			}
		}
		public static void HandleTriggerHook(Action<ForgeCrushDoorController, SpeculativeRigidbody, SpeculativeRigidbody, CollisionData> orig, ForgeCrushDoorController self, SpeculativeRigidbody specRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
			if (TrapsShouldBeDefused() == true){return;}
			else{orig(self, specRigidbody, sourceSpecRigidbody, collisionData);}
		}

		public static void UpdateHookForgeHammerController(Action<ForgeHammerController> orig, ForgeHammerController self)
        {
			if (TrapsShouldBeDefused() == true){return;}
			else{orig(self);}
		}

		public static void UpdateHookPathingTrapController(Action<PathMover> orig, PathMover self)
        {
			if (TrapsShouldBeDefused() == true && self.name.ToLower().Contains("trap"))
            {
				self.specRigidbody.PathSpeed = 0;
				if (self.spriteAnimator != null) { self.spriteAnimator.Stop(); }
				return;
            }
            else{orig(self);}
        }

		public static void FireHook(Action<CartTurretController> orig, CartTurretController self)
        {
			if (TrapsShouldBeDefused() == true){return;}
			orig(self);
        }

		public static void ShootProjectileInDirectionHook(Action<ProjectileTrapController, Vector3, Vector2> orig, ProjectileTrapController self, Vector3 spawnPosition, Vector2 direction)
        {
			if (TrapsShouldBeDefused() != true){orig(self, spawnPosition, direction);}
        }
		public static void UpdateHook(Action<BasicTrapController> orig, BasicTrapController self)
        {
			if (TrapsShouldBeDefused() == true)
            {
				if (self.triggerOnBlank || self.triggerOnExplosion)
				{
					self.Trigger();
					return;
				}
			}
			if (TrapsShouldBeDefused() == true){return;}
			orig(self);

		}


		public static bool TrapsShouldBeDefused()
		{
			if (trapDefuseOverrides.Count > 0) { return true; }
			return false;
		}

        public static void AddTrapDefuseOverride(string overrideName)
        {if (!trapDefuseOverrides.Contains(overrideName)) { trapDefuseOverrides.Add(overrideName); } }

        public static void RemoveTrapDefuseOverride(string overrideName)
        { if (trapDefuseOverrides.Contains(overrideName)) { trapDefuseOverrides.Remove(overrideName); } }

        public static List<string> trapDefuseOverrides = new List<string>();

		
		public static int TrapDefusalKitID;
		public override void Update()
		{
			/*
			if (base.Owner)
			{
				IsHoldingDefusalItem = true;

				List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag5 = activeEnemies != null && activeEnemies.Count >= 0;
				if (flag5)
				{
					foreach (AIActor ai in activeEnemies)
					{
						bool flag8 = ai && ai != null && Vector2.Distance(ai.CenterPosition, base.Owner.sprite.WorldCenter) <= 8f && ai.EnemyGuid.ToLower().Contains("deturret") && ai != null;
						if (flag8)
						{
							ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
							this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
							this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
							this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
							this.smallPlayerSafeExplosion.damage = 30;
							for (int j = 0; j < ai.behaviorSpeculator.OtherBehaviors.Count; j++)
							{
								if (ai.behaviorSpeculator.OtherBehaviors[j] is BehaviorBase && ai.behaviorSpeculator.OtherBehaviors[j] != null)
								{
									if (ai.behaviorSpeculator.OtherBehaviors[j] is CustomSpinBulletsBehavior)
									{
										CustomSpinBulletsBehavior spin = ai.behaviorSpeculator.OtherBehaviors[j] as CustomSpinBulletsBehavior;
										spin.EndContinuousUpdate();
										spin.DestroyProjectiles();
									}
								}
							}
							Exploder.Explode(ai.transform.PositionVector2(), this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
							UnityEngine.Object.Destroy(ai.gameObject);
						}
					}
				}			
			}
			else
			{
				IsHoldingDefusalItem = false;
			}
			*/
		}

		
		public override DebrisObject Drop(PlayerController player)
		{
			//IsHoldingDefusalItem = false;
			RemoveTrapDefuseOverride("defusal_kit");
            DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
            //IsHoldingDefusalItem = true;
            AddTrapDefuseOverride("defusal_kit");

            base.Pickup(player);



			

		}

		public override void OnDestroy()
		{
            RemoveTrapDefuseOverride("defusal_kit");
            if (base.Owner != null)
            {
				base.OnDestroy();
			}
		}
	}
}