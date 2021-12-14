
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
				text = "The Shrine of the Holy Chamber. It's so quiet here it very feels wrong to make any noise. Seems like it's missing something...",
				spritePath = "Planetside/Resources/Shrines/HolyChamberShrine.png",
				room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/HolyChamberRoom.room").room,
				RoomWeight = 2f,
				acceptText = "Grant a Heart, and some supplies.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				//offset = new Vector3(43.8f, 42.4f, 42.9f),
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				
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
			string header;
			string text;
			HolyChamberShrine.Numero = UnityEngine.Random.Range(1, 8);
			switch (HolyChamberShrine.Numero)
			{
				case 1:
					player.gameObject.AddComponent<Greed>();
				    header = "Greed";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Kills Can Grant Money";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 2:
					player.gameObject.AddComponent<BlessedShield>();
					header = "Blessed Shield";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Cleanses Nearby Bullets And Foes.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 3:
					player.gameObject.AddComponent<RepellingRolls>();
					header = "Repelling Rolls";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Forces Away Dangers On Rolling.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 4:
					player.gameObject.AddComponent<AllSeeingEye>();
					header = "All-Seeing Eye";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Grants Foresight.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 5:
					player.gameObject.AddComponent<DanageBoosterWithEnemies>();
					header = "Enemies-To-Damage";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "More Active Enemies, More Damage.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 6:
					player.gameObject.AddComponent<BulletsToFire>();
					header = "Bullets To Gunfire";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Damage Converts Enemy Bullets.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;
				case 7:
					player.gameObject.AddComponent<SoulPuddle>();
					header = "Soul Puddle";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					header = "Kills Leave Soul Puddles.";
					text = "Filler.";
					HolyChamberShrine.Notify(header, text);
					break;

			}
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
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
		public class AllSeeingEye : BraveBehaviour
		{
			public void Start()
			{
				PlayerController player = GameManager.Instance.PrimaryPlayer;
				this.RevealSecretRooms();
				GameManager.Instance.OnNewLevelFullyLoaded += this.RevealSecretRooms;


				player.gameObject.AddComponent<AllSeeingEye.AllSeeingEyeChestBehaviour>();

			}
			public void Update()
			{

			}
			private void RevealSecretRooms()
			{
				for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
				{
					RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
					bool flag = roomHandler.connectedRooms.Count != 0;
					bool flag2 = flag;
					bool flag3 = flag2;
					if (flag3)
					{
						bool flag4 = roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET;
						bool flag5 = flag4;
						bool flag6 = flag5;
						if (flag6)
						{
							roomHandler.RevealedOnMap = true;
							Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, roomHandler == GameManager.Instance.PrimaryPlayer.CurrentRoom);
						}
					}
				}
			}
			private static string gunVFX = "Planetside/Resources/VFX/AllSeeingEye/gunicon.png";
			private static string itemVFX = "Planetside/Resources/VFX/AllSeeingEye/itemicon.png";
			private static string vfxName = "WisperVFX";
			private static GameObject gunVFXPrefab;
			private static GameObject itemVFXPrefab;
			private class AllSeeingEyeChestBehaviour : BraveBehaviour
			{
				private void Start()
				{
					this.player = base.GetComponent<PlayerController>();

					AllSeeingEye.gunVFXPrefab = SpriteBuilder.SpriteFromResource(AllSeeingEye.gunVFX, null, false);
					AllSeeingEye.itemVFXPrefab = SpriteBuilder.SpriteFromResource(AllSeeingEye.itemVFX, null, false);
					AllSeeingEye.gunVFXPrefab.name = AllSeeingEye.vfxName;
					AllSeeingEye.itemVFXPrefab.name = AllSeeingEye.vfxName;
					UnityEngine.Object.DontDestroyOnLoad(AllSeeingEye.gunVFXPrefab);
					FakePrefab.MarkAsFakePrefab(AllSeeingEye.gunVFXPrefab);
					AllSeeingEye.gunVFXPrefab.SetActive(false);
					UnityEngine.Object.DontDestroyOnLoad(AllSeeingEye.itemVFXPrefab);
					FakePrefab.MarkAsFakePrefab(AllSeeingEye.itemVFXPrefab);
					AllSeeingEye.itemVFXPrefab.SetActive(false);
				}

				private void FixedUpdate()
				{
					bool flag = !this.player || this.player.CurrentRoom == null;
					if (!flag)
					{
						IPlayerInteractable nearestInteractable = this.player.CurrentRoom.GetNearestInteractable(this.player.sprite.WorldCenter, 1f, this.player);
						bool flag2 = nearestInteractable != null && nearestInteractable is Chest;
						if (flag2)
						{
							Chest chest = nearestInteractable as Chest;
							bool flag3 = !this.encounteredChests.Contains(chest) && !chest.transform.Find(AllSeeingEye.vfxName);
							if (flag3)
							{
								this.InitializeChest(chest);
							}
							else
							{
								this.nearbyChest = chest;
							}
						}
						else
						{
							this.nearbyChest = null;
						}
						this.HandleChests();
					}
				}

				private void HandleChests()
				{
					foreach (Chest chest in this.encounteredChests)
					{
						bool flag = !chest;
						if (!flag)
						{
							tk2dSprite tk2dSprite;
							if (chest == null)
							{
								tk2dSprite = null;
							}
							else
							{
								Transform transform = chest.transform;
								if (transform == null)
								{
									tk2dSprite = null;
								}
								else
								{
									Transform transform2 = transform.Find(AllSeeingEye.vfxName);
									tk2dSprite = ((transform2 != null) ? transform2.GetComponent<tk2dSprite>() : null);
								}
							}
							tk2dSprite tk2dSprite2 = tk2dSprite;
							bool flag2 = !tk2dSprite2;
							if (!flag2)
							{
								bool flag3 = chest != this.nearbyChest;
								if (flag3)
								{
									tk2dSprite2.scale = Vector3.Lerp(tk2dSprite2.scale, Vector3.zero, 0.25f);
								}
								else
								{
									tk2dSprite2.scale = Vector3.Lerp(tk2dSprite2.scale, Vector3.one, 0.25f);
								}
								bool flag4 = Vector3.Distance(tk2dSprite2.scale, Vector3.zero) < 0.01f;
								if (flag4)
								{
									tk2dSprite2.scale = Vector3.zero;
								}
								tk2dSprite2.PlaceAtPositionByAnchor(chest.sprite.WorldTopCenter + this.offset, tk2dBaseSprite.Anchor.LowerCenter);
							}
						}
					}
				}

				// Token: 0x0600065C RID: 1628 RVA: 0x0003FB34 File Offset: 0x0003DD34
				private void InitializeChest(Chest chest)
				{
					int guess = this.GetGuess(chest);
					bool flag = guess == 0;
					GameObject original;
					if (flag)
					{
						original = AllSeeingEye.gunVFXPrefab;
					}
					else
					{
						original = AllSeeingEye.itemVFXPrefab;
					}
					tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(original, chest.transform).GetComponent<tk2dSprite>();
					component.name = AllSeeingEye.vfxName;
					component.PlaceAtPositionByAnchor(chest.sprite.WorldTopCenter + this.offset, tk2dBaseSprite.Anchor.LowerCenter);
					component.scale = Vector3.zero;
					this.nearbyChest = chest;
					this.encounteredChests.Add(chest);
				}

				private int GetGuess(Chest chest)
				{
					Chest.GeneralChestType chestType = chest.ChestType;
					bool flag = chestType == Chest.GeneralChestType.WEAPON;
					int result;
					if (flag)
					{
						result = 0;
					}
					else
					{
						bool flag2 = chestType == Chest.GeneralChestType.ITEM;
						if (flag2)
						{
							result = 1;
						}
						else
						{
							List<PickupObject> list = chest.PredictContents(this.player);
							foreach (PickupObject pickupObject in list)
							{
								bool flag3 = pickupObject is Gun;
								if (flag3)
								{
									return 0;
								}
								bool flag4 = pickupObject is PlayerItem || pickupObject is PassiveItem;
								if (flag4)
								{
									return 1;
								}
							}
							result = UnityEngine.Random.Range(0, 2);
						}
					}
					return result;
				}

				public void DestroyAllFX()
				{
					foreach (Chest chest in this.encounteredChests)
					{
						Transform transform = chest.transform.Find(AllSeeingEye.vfxName);
						bool flag = transform;
						if (flag)
						{
							UnityEngine.Object.Destroy(transform);
						}
					}
					this.encounteredChests.Clear();
				}
				private List<Chest> encounteredChests = new List<Chest>();
				private PlayerController player;
				private Chest nearbyChest;
				private Vector2 offset = new Vector2(0f, 0.25f);

			}

		}

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



