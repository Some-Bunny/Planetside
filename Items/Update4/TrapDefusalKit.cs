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
			string resourceName = "Planetside/Resources/trapdefusalkit.png";
			GameObject obj = new GameObject(itemName);
			var item = obj.AddComponent<TrapDefusalKit>();
			ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
			string shortDesc = "Red Or Blue?";
			string longDesc = "Masterfully disables all sorts of traps.\n\nHolds a screwdriver, a pair of pliers, a wire-cutter, a booklet on bomb defusal and a half-full bottle of vodka.";
			ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.B;
			TrapDefusalKit.TrapDefusalKitID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);
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
		}
		public static void ShootWallBulletScriptHook(Action<HighPriestSimpleMergoBehavior> orig, HighPriestSimpleMergoBehavior self)
		{
			if (IsHoldingDefusalItem == true)
			{
				return;
			}
			else { orig(self);}
		}
		public static BehaviorResult UpdateGoopHook(Func<SpawnGoopBehavior, BehaviorResult> orig, SpawnGoopBehavior self)
        {
			AIActor host = PlanetsideReflectionHelper.ReflectGetField<AIActor>(typeof(SpawnGoopBehavior), "m_aiActor", self);
            if (host != null && host.EnemyGuid == "6868795625bd46f3ae3e4377adce288b" && IsHoldingDefusalItem == true)
			{
				return BehaviorResult.Continue;
			}		
			return orig(self);
        }
		public static void HandleRigidbodyCollisionHook(Action<ForgeFlamePipeController, CollisionData> orig, ForgeFlamePipeController self, CollisionData rigidbodyCollision)
		{
			if (IsHoldingDefusalItem == true) { return; }
			else { orig(self, rigidbodyCollision); }
		}
		public static void HandleBeamCollisionHook(Action<ForgeFlamePipeController, BeamController> orig, ForgeFlamePipeController self, BeamController beamController)
		{
			if (IsHoldingDefusalItem == true) { return; }
			else { orig(self, beamController); }
		}
		public static void DamageHook(Action<PathingTrapController, SpeculativeRigidbody, Single, Single> orig, PathingTrapController self, SpeculativeRigidbody rigidbody, float damage, float knockbackStrength)
        {
			if (IsHoldingDefusalItem == true)
            {
				if (knockbackStrength > 0f && rigidbody.knockbackDoer){rigidbody.knockbackDoer.ApplySourcedKnockback(rigidbody.UnitCenter - self.specRigidbody.UnitCenter, knockbackStrength*0.5f, self.gameObject, false);}
				return;
            }
			orig(self, rigidbody, damage, knockbackStrength);
		}
		public static void UpdateSparksHook(Action<PathingTrapController> orig, PathingTrapController self)
        {
			if (IsHoldingDefusalItem == true)
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
				}
				orig(self);
			}
		}
		public static void HandleTriggerHook(Action<ForgeCrushDoorController, SpeculativeRigidbody, SpeculativeRigidbody, CollisionData> orig, ForgeCrushDoorController self, SpeculativeRigidbody specRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
			if (IsHoldingDefusalItem == true){return;}
			else{orig(self, specRigidbody, sourceSpecRigidbody, collisionData);}
		}

		public static void UpdateHookForgeHammerController(Action<ForgeHammerController> orig, ForgeHammerController self)
        {
			if (IsHoldingDefusalItem == true){return;}
			else{orig(self);}
		}

		public static void UpdateHookPathingTrapController(Action<PathMover> orig, PathMover self)
        {
			if (IsHoldingDefusalItem == true && self.name.ToLower().Contains("trap"))
            {
				self.specRigidbody.PathSpeed = 0;
				if (self.spriteAnimator != null) { self.spriteAnimator.Stop(); }
				return;
            }
            else{orig(self);}
        }

		public static void FireHook(Action<CartTurretController> orig, CartTurretController self)
        {
			if (IsHoldingDefusalItem == true){return;}
			orig(self);
        }

		public static void ShootProjectileInDirectionHook(Action<ProjectileTrapController, Vector3, Vector2> orig, ProjectileTrapController self, Vector3 spawnPosition, Vector2 direction)
        {
			if (IsHoldingDefusalItem!=true){orig(self, spawnPosition, direction);}
        }
		public static void UpdateHook(Action<BasicTrapController> orig, BasicTrapController self)
        {
			if (IsHoldingDefusalItem == true)
            {
				if (self.triggerOnBlank || self.triggerOnExplosion)
				{
					self.Trigger();
					return;
				}
			}
			if (IsHoldingDefusalItem == true){return;}
			orig(self);
			
		}
		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 3f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 50f,
			doExplosionRing = true,
			doDestroyProjectiles = false,
			doForce = true,
			debrisForce = 200f,
			preventPlayerForce = true,
			explosionDelay = 0f,
			usesComprehensiveDelay = false,
			doScreenShake = true,
			playDefaultSFX = true,
		};
		public static int TrapDefusalKitID;
		public static bool IsHoldingDefusalItem;
		public List<AIActor> FuckYouDie = new List<AIActor>();
		protected override void Update()
		{
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
		}

		
		public override DebrisObject Drop(PlayerController player)
		{
			IsHoldingDefusalItem = false;

			DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
			IsHoldingDefusalItem = true;
			base.Pickup(player);



			/*
			List<DungeonFlowNode> nodes =  GungeonAPI.OfficialFlows.GetDungeonPrefab("base_resourcefulrat").PatternSettings.flows[0].AllNodes;
			foreach(DungeonFlowNode node in nodes)
            {
				if (node.overrideExactRoom != null)
                {
					if (node.overrideExactRoom.name == "ResourcefulRatRoom01" )
                    {
						List<PrototypePlacedObjectData> help = node.overrideExactRoom.placedObjects;
						if (help != null)
						{
							ETGModConsole.Log("26");
							for (int e = 0; e < help.Count; e++)
							{
								ETGModConsole.Log("q");

								if (help[e].enemyBehaviourGuid != null)
                                {
									ETGModConsole.Log(help[e].enemyBehaviourGuid.ToString());
									ETGModConsole.Log("4");
								}

								if (help[e].placeableContents != null)
								{
									ETGModConsole.Log(help[e].placeableContents.name.ToString());
									ETGModConsole.Log("4");
									/*
									foreach (Component item in help[e].placeableContents.Get(typeof(Component)))
									{
										ETGModConsole.Log("5");
										ETGModConsole.Log(item.GetType().ToString());
										ETGModConsole.Log("6");
										ETGModConsole.Log(item.name + "\n================");
										ETGModConsole.Log("7");

									}
									
								}
								
								if (help[e].nonenemyBehaviour != null)
								{
									ETGModConsole.Log("3");
									ETGModConsole.Log(help[e].nonenemyBehaviour.gameObject.name.ToString());
									ETGModConsole.Log("4");
									foreach (Component item in help[e].nonenemyBehaviour.gameObject.GetComponentsInChildren(typeof(Component)))
									{
										ETGModConsole.Log("5");
										ETGModConsole.Log(item.GetType().ToString());
										ETGModConsole.Log("6");
										ETGModConsole.Log(item.name + "\n================");
										ETGModConsole.Log("7");

									}
									//ETGModConsole.Log(help[i].nonenemyBehaviour.gameObject.name.ToString());

								}

								//ETGModConsole.Log(help[i].name + "\n================");
							}
						}
					}
                }
			}
			*/


			//Dungeon dung = GungeonAPI.OfficialFlows.GetDungeonPrefab("base_resourcefulrat");

			//AssetBundle bundle = ResourceManager.LoadAssetBundle("base_resourcefulrat");
			//RoomHandler fusebombroom01 = bundle.LoadAsset<Ob>("resourcefulratroom01");

			//if (fusebombroom01 == null) { ETGModConsole.Log("FUCK is null"); }
			//bundle = null;
			/*
			List<int> list = Enumerable.Range(0, dung.data.rooms.Count).ToList<int>();
			for (int i = 0; i < list.Count; i++)
			{
				ETGModConsole.Log("2");
				RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[list[i]];
				ETGModConsole.Log("23");
				string name = roomHandler.GetRoomName();
				ETGModConsole.Log("24");
				bool flag3 = name == "ResourcefulRatRoom01" && name != null;
				if (flag3)
				{
					ETGModConsole.Log("25");
					if (roomHandler == null) { ETGModConsole.Log("RoomHandler is NULL"); }
					if (roomHandler.area == null) { ETGModConsole.Log("area is NULL"); }
					if (roomHandler.area.prototypeRoom == null) { ETGModConsole.Log("prototypeRoom is NULL"); }
					if (roomHandler.area.prototypeRoom.placedObjects == null) { ETGModConsole.Log("placedObjects is NULL"); }

					List<PrototypePlacedObjectData> help = roomHandler.area.prototypeRoom.placedObjects;
					if (help != null)
                    {
						ETGModConsole.Log("26");
						for (int e = 0; e < help.Count; e++)
						{
							if (help[e].nonenemyBehaviour != null)
							{
								ETGModConsole.Log("3");
								ETGModConsole.Log(help[i].nonenemyBehaviour.gameObject.name.ToString());
								ETGModConsole.Log("4");
								foreach (Component item in help[i].nonenemyBehaviour.gameObject.GetComponentsInChildren(typeof(Component)))
								{
									ETGModConsole.Log("5");
									ETGModConsole.Log(item.GetType().ToString());
									ETGModConsole.Log("6");
									ETGModConsole.Log(item.name + "\n================");
									ETGModConsole.Log("7");

								}
								//ETGModConsole.Log(help[i].nonenemyBehaviour.gameObject.name.ToString());

							}

							//ETGModConsole.Log(help[i].name + "\n================");
						}
					}
					
					
				}
			}
			dung = null;

			*/

		}

		protected override void OnDestroy()
		{
			IsHoldingDefusalItem = false;
			if (base.Owner != null)
            {
				base.OnDestroy();
			}
		}
	}
}