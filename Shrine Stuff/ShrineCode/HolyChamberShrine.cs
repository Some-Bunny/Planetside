
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.OldShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using System.Collections.ObjectModel;
using ItemAPI;

namespace Planetside
{

	public static class HolyChamberShrine
	{
		public static void Add()
		{
			OldShrineFactory iei = new OldShrineFactory
			{

				name = "HolyChamberShrine",
				modID = "psog",
				text = "The Shrine of the Holy Chamber. Seems like it's missing something...",
				spritePath = "Planetside/Resources/Shrines/HolyChamberShrine.png",
				room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/HolyChamberRoom.room").room,
				RoomWeight = 1.5f,
				acceptText = "Grant a Heart, and some supplies.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = "Planetside/Resources/Shrines/defaultShrineShadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.5f

			};
			iei.Build();
		}
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			int armorInt = Convert.ToInt32(player.healthHaver.Armor);
			if (player.name == "PlayerShade(Clone)")
			{
				return true;
			}
			else
            {
				if (player.characterIdentity == PlayableCharacters.Robot)
				{
					return shrine.GetComponent<CustomShrineController>().numUses == 0 && armorInt > 2;
				}
				else if (player.characterIdentity != PlayableCharacters.Robot)
				{
					return shrine.GetComponent<CustomShrineController>().numUses == 0 && player.stats.GetStatValue(PlayerStats.StatType.Health) > 1;
				}
				else
				{
					return false;
				}
			}
			
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
			gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(shrine.transform.PositionVector2() + new Vector2(0.5f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
			gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
			gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();

			StatModifier item = new StatModifier
			{
				statToBoost = PlayerStats.StatType.Health,
				amount = -1,
				modifyType = StatModifier.ModifyMethod.ADDITIVE
			};
			StatModifier item2 = new StatModifier
			{
				statToBoost = PlayerStats.StatType.AmmoCapacityMultiplier,
				amount = 0.8f,
				modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
			};
			if (player.name != "PlayerShade(Clone)")
            {
				if (player.characterIdentity == PlayableCharacters.Robot)
				{
					player.healthHaver.Armor -= 2;
				}
			}
			if (player.name == "PlayerShade(Clone)")
			{
				StatModifier money = new StatModifier
				{
					statToBoost = PlayerStats.StatType.MoneyMultiplierFromEnemies,
					amount = 0.8f,
					modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
				};
				player.ownerlessStatModifiers.Add(money);

			}
			player.ownerlessStatModifiers.Add(item2);
			player.ownerlessStatModifiers.Add(item);

			List<DebrisObject> perksThatExist = new List<DebrisObject>() { };
			List<int> IDsUsed = new List<int>()
            {
				Contract.ContractID,
				Greedy.GreedyID,
				AllStatsUp.AllStatsUpID,
				AllSeeingEye.AllSeeingEyeID,
				BlastProjectiles.BlastProjectilesID,
				Glass.GlassID,
				ChaoticShift.ChaoticShiftID,
				PitLordsPact.PitLordsPactID,
				UnbreakableSpirit.UnbreakableSpiritID,
				Gunslinger.GunslingerID
			};
			int PerksToSpawn = UnityEngine.Random.Range(2, 5);
			for (int e = 0; e < PerksToSpawn; e++)
            {
				int IDtoUse = BraveUtility.RandomElement<int>(IDsUsed);
				DebrisObject debrisSpawned = LootEngine.SpawnItem(PickupObjectDatabase.GetById(IDtoUse).gameObject, shrine.GetComponent<tk2dBaseSprite>().WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle((360 / PerksToSpawn)*e, 1.25f).ToVector3ZisY() + new Vector3(-0.5f, 0), MathToolbox.GetUnitOnCircle((360 / PerksToSpawn) * e, 5), 3).GetComponent<DebrisObject>();
				perksThatExist.Add(debrisSpawned);
				IDsUsed.Remove(IDtoUse);
			}
			if (perksThatExist.Count >= 0 || perksThatExist != null)
            {
				GameManager.Instance.Dungeon.StartCoroutine(ItemChoiceCoroutine(perksThatExist));
			}
			TextMaker text = shrine.gameObject.AddComponent<TextMaker>();
			text.TextSize = 7;
			text.Color = Color.cyan;
			text.ExistTime = 2;
			text.FadeInTime = 0.25f;
			text.FadeOutTime = 0.5f;
			text.Text = "Choose One.";
			text.Opacity = 1;
			text.anchor = dfPivotPoint.TopCenter;
			text.offset = new Vector3(-1.5f, 4f);
			text.GameObjectToAttachTo = shrine.gameObject;

			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
		}

	

		public static IEnumerator ItemChoiceCoroutine(List<DebrisObject> pickups)
		{
			for (; ; )
			{
				foreach (DebrisObject obj in pickups)
                {
					if (!obj)
                    {
						pickups.Remove(obj);
						foreach (DebrisObject obj2 in pickups)
						{
							GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
							gameObject2.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(obj2.transform.PositionVector2() + new Vector2(0f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
							gameObject2.transform.position = gameObject2.transform.position.Quantize(0.0625f);
							gameObject2.GetComponent<tk2dBaseSprite>().UpdateZDepth();
							UnityEngine.Object.Destroy(obj2.gameObject);
						}
						yield break;
					}
				}
				yield return null;
			}
		}

		public static int Numero = 0;
		private static void Notify(string header, string text)
		{
			tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.Instance.EncounterIconCollection;
			int spriteIdByName = encounterIconCollection.GetSpriteIdByName("Planetside/Resources/shellheart");
			GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, null, spriteIdByName, UINotificationController.NotificationColor.PURPLE, true, true);
		}
		public static List<int> MasterRoundIDs = new List<int>()
		{
			467,
			468,
			469,
			470,
			471
		};
		//Done

		//Done
		public class Greed : BraveBehaviour
		{
			public void Start()
			{
				
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				player.OnKilledEnemy += this.OnKilledEnemy;

			}
			public void Update()
			{

			}
			public void OnKilledEnemy(PlayerController source)
			{
				int RNG = UnityEngine.Random.Range(1, 3);
				if (RNG == 1)
				{
					LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(68).gameObject, source);
				}
			}
		}

		//Done
		public class RepellingRolls : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				player.OnRollStarted += this.onDodgeRoll;
			}
			public void Update()
			{

			}
			private void onDodgeRoll(PlayerController player, Vector2 dirVec)
			{
				float num = 3;
				Exploder.DoRadialKnockback(player.specRigidbody.UnitCenter, 80f, (float)num*3.33f);
				DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(player.sprite.WorldCenter, num);
			}
		}

		//Done
		public class BlessedShield : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				this.Trailer.spawnShadows = true;
				player.gameObject.AddComponent(this.Trailer);
			}

			public void Update()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;

				if (player != null)
                {
					Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
					outlineMaterial.SetColor("_OverrideColor", new Color(150f, 0f, 0f));
				}
				this.elapsed += BraveTime.DeltaTime;
				bool flag3 = this.elapsed > 0.2f;
				if (flag3)
                {

					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = player.sprite.WorldCenter;
					if (activeEnemies != null)
					{
						foreach(AIActor aiactor in activeEnemies)
                        {
							bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 3 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null && aiactor.IsBlackPhantom;
							if (ae)
							{
								aiactor.UnbecomeBlackPhantom();
								SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, aiactor.sprite.WorldBottomCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(aiactor.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
							}
						}

					}
					ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
					if (allProjectiles != null)
					{
						foreach (Projectile proj in allProjectiles)
						{
							bool ae = Vector2.Distance(proj.sprite.WorldCenter, centerPosition) < 6 && proj != null && proj.specRigidbody != null && player != null && proj.IsBlackBullet && proj.Owner != player;
							if (ae)
							{
								proj.ReturnFromBlackBullet();
								proj.Speed *= 1.2f;
							}
						}
					}
					this.elapsed = 0f;
                }
			}
			private readonly ImprovedAfterImage Trailer = new ImprovedAfterImage
			{
				dashColor = Color.red,
				spawnShadows = true
			};
			public AIActor GetNearestEnemy(List<AIActor> activeEnemies, Vector2 position, out float nearestDistance, string[] filter)
			{
				AIActor aiactor = null;
				nearestDistance = float.MaxValue;
				bool flag = activeEnemies == null;
				bool flag2 = flag;
				bool flag3 = flag2;
				bool flag4 = flag3;
				bool flag5 = flag4;
				AIActor result;
				if (flag5)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < activeEnemies.Count; i++)
					{
						AIActor aiactor2 = activeEnemies[i];
						bool flag6 = !aiactor2.healthHaver.IsDead && aiactor2.healthHaver.IsVulnerable;
						bool flag7 = flag6;
						if (flag7)
						{
							bool flag8 = filter == null || !filter.Contains(aiactor2.EnemyGuid);
							bool flag9 = flag8;
							if (flag9)
							{
								float num = Vector2.Distance(position, aiactor2.CenterPosition);
								bool flag10 = num < nearestDistance;
								bool flag11 = flag10;
								if (flag11)
								{
									nearestDistance = num;
									aiactor = aiactor2;
								}
							}
						}
					}
					result = aiactor;
				}
				return result;
			}
			/*
			public Projectile GetNearestBullet(ReadOnlyCollection<Projectile> activeBullets, Vector2 position, out float nearestDistance, string[] filter)
			{
				Projectile Bullet1 = null;
				nearestDistance = float.MaxValue;
				bool flag = activeBullets == null;
				bool flag2 = flag;
				bool flag3 = flag2;
				bool flag4 = flag3;
				bool flag5 = flag4;
				Projectile result;
				if (flag5)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < activeBullets.Count; i++)
					{
						Projectile Bullet2 = activeBullets[i];
						bool flag8 = filter == null;
						bool flag9 = flag8;
						if (flag9)
						{
							float num = Vector2.Distance(position, Bullet2.sprite.WorldCenter);
							bool flag10 = num < nearestDistance;
							bool flag11 = flag10;
							if (flag11)
							{
								nearestDistance = num;
								Bullet1 = Bullet2;
							}
						}
					}
					result = Bullet1;
				}
				return result;
			}
			*/
			public float TimeBetweenDamageEvents;
			private float elapsed;
		}

		//Done
		public class SoulPuddle : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				player.OnAnyEnemyReceivedDamage += OnEnemyDamaged;
			}
			public void Update()
			{

			}
			private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
			{
				bool deathed = fatal;
				if (deathed)
				{
					DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(DebuffLibrary.PossesedPuddle);
					goopManagerForGoopType.TimedAddGoopCircle(enemy.sprite.WorldCenter, 2f, 0.35f, false);
				}
			}
		}

		//Done
		public class DanageBoosterWithEnemies : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				player.PostProcessProjectile += this.PostProcessProjectile;
			}
			public void Update()
			{

			}
			private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
			{
				try
				{
					PlayerController player = GameManager.Instance.PrimaryPlayer;
					List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
					Vector2 centerPosition = player.sprite.WorldCenter;
					if (activeEnemies != null)
					{
						for (int i = 0; i < activeEnemies.Count; i++)
						{
							sourceProjectile.baseData.damage *= 1.03f;
						}
					}
				}
				catch (Exception ex)
				{
					ETGModConsole.Log(ex.Message, false);
					ETGModConsole.Log("If you see this pop up, write a message in the comment section of ModWorkShop AND include a list of all items/guns you have/had during the run.");
				}
			}
		}

		//Done
		public class BulletsToFire : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				player.healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));

			}
			private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
			{

				PlayerController player = GameManager.Instance.PrimaryPlayer;
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				if (allProjectiles != null)
				{

					for (int i = 0; i < allProjectiles.Count; i++)
					{
						if (allProjectiles[i].Owner != player)
                        {
							Projectile proj = allProjectiles[i];
							DoFireGoop(proj.sprite.WorldCenter);
							this.Soul(proj.sprite.WorldCenter);
							SpawnManager.Despawn(proj.gameObject);
						}
					}
				}
			}
			private void DoFireGoop(Vector2 v)
			{
				AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
				GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/napalmgoopquickignite.asset");
				DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef);
				goopManagerForGoopType.TimedAddGoopCircle(v, 1f, 0.35f, false);
				goopDef.damagesEnemies = false;
			}
			public void Soul(Vector3 position)
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				Projectile projectile = ((Gun)ETGMod.Databases.Items[378]).DefaultModule.projectiles[0];
				GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 359)));
				Projectile component2 = gameObject.GetComponent<Projectile>();
				component2.Owner = player;
				component2.Shooter = player.specRigidbody;
				component2.baseData.damage = 20f;
				component2.baseData.speed = 1f;
				component2.sprite.renderer.enabled = false;
				PierceProjModifier spook = component2.gameObject.AddComponent<PierceProjModifier>();
				spook.penetration = 1;
				HomingModifier homing = component2.gameObject.AddComponent<HomingModifier>();
				homing.HomingRadius = 250f;
				homing.AngularVelocity = 120f;
				TrailRenderer tr;
				var tro = component2.gameObject.AddChild("trail object");
				tro.transform.position = component2.transform.position;
				tro.transform.localPosition = new Vector3(0f, 0f, 0f);
				tr = tro.AddComponent<TrailRenderer>();
				tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				tr.receiveShadows = false;
				var mat = new Material(Shader.Find("Sprites/Default"));
				mat.mainTexture = _gradTexture;
				mat.SetColor("_Color", new Color(3f, 1f, 1f, 0.7f));
				tr.material = mat;
				tr.time = 0.5f;
				tr.minVertexDistance = 0.1f;
				tr.startWidth = 0.15f;
				tr.endWidth = 0f;
				tr.startColor = Color.white;
				tr.endColor = new Color(3f, 0f, 0f, 0f);
				component2.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
				base.StartCoroutine(this.Speed(component2));

			}
			public Texture _gradTexture;
			public IEnumerator Speed(Projectile projectile)
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				bool flag = player != null;
				bool flag3 = flag;
				if (flag3)
				{
					for (int i = 0; i < 15; i++)
					{
						projectile.baseData.speed += 1f;
						projectile.UpdateSpeed();
						yield return new WaitForSeconds(0.1f);
					}
				}
				yield break;
			}
			private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
			{
				bool flag = otherRigidbody && otherRigidbody.healthHaver && otherRigidbody != null;
				if (flag)
				{
					DoFireGoop(myRigidbody.sprite.WorldCenter);
				}
			}
		}
	}
}



