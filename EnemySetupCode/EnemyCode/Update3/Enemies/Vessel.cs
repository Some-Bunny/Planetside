using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using Pathfinding;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Reflection;

namespace Planetside
{

	public class VesselMiniomController : BraveBehaviour
    {
		public void Start()
        {
			base.aiActor.MovementSpeed *= 0.3f;
			base.aiActor.behaviorSpeculator.AttackCooldown *= 3f;
			base.aiActor.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
		}

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
			if (vessel != null)
			{
				AkSoundEngine.PostEvent("Play_VesselHurt", base.aiActor.gameObject);
				vessel.healthHaver.ApplyDamage(base.aiActor.healthHaver.GetMaxHealth() * 0.5f, vessel.transform.position, "severed link");
				GameObject gameObjecter = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceBurstVFX, false);
				gameObjecter.transform.position = vessel.sprite.WorldTopCenter- new Vector2(2, 0.5f);
				Destroy(gameObjecter, 2);
			}
			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceBurstVFX, false);
			gameObject.transform.position = base.aiActor.sprite.WorldTopCenter;
			Destroy(gameObject, 2);
		}

        public void RemoveOnPreDeath()
		{
			base.aiActor.MovementSpeed = base.aiActor.BaseMovementSpeed;
			base.aiActor.behaviorSpeculator.AttackCooldown /= 3f;
			base.aiActor.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
		}

		protected override void OnDestroy()
        {
			if (base.aiActor)
            {base.aiActor.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;}
		}


		public AIActor vessel;
    }

	public class VesselController : BraveBehaviour
    {
		public void Start(){ IsUpdating = true;  base.aiActor.healthHaver.OnPreDeath += OnPreDeath; }

        private void OnPreDeath(Vector2 obj)
        {
			IsUpdating = false;
			foreach (AIActor enemy in allEnemies)
			{ClearMods(enemy);}
		}

        public void Update()
        {

			timerFloat += BraveTime.DeltaTime;
			if (base.aiActor && IsUpdating == true)
            {

				RoomHandler currentRoom = base.aiActor.GetAbsoluteParentRoom();

				List<AIActor> enemies = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
				List<AIActor> enemiesTwo = new List<AIActor>();
				if (enemies != null || enemies.Count > 0)
                {
					for (int i = 0; i < enemies.Count; i++)
                    {
						if (EnemyBlacklistCheck(enemies[i]) == false) { enemiesTwo.Add(enemies[i]); }
					}
				}
				allEnemies = enemiesTwo;
				for (int i = 0; i < allEnemies.Count; i++)
                {
					UpdateCables(allEnemies[i]);
				}

				if (timerFloat > 1.25f)
                {
					if (allEnemies.Count > 0 || allEnemies != null)
                    {
						timerFloat = 0;
						for (int i = 0; i < allEnemies.Count; i++)
						{
							GameManager.Instance.StartCoroutine(DoChainBreakAway(allEnemies[i]));
						}
					}			
				}
            }
        }
		private IEnumerator DoChainBreakAway(AIActor enemy)
		{
			Vector2 vector = base.aiActor.sprite.WorldCenter;
			Vector2 vector2 = enemy.sprite.WorldCenter;
			int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1)*2;
			for (int i = 0; i < num2; i++)
			{
				if (enemy == null) { yield break; }
				if (base.aiActor == null) { yield break; }

				float t = (float)i / (float)num2;
				Vector3 vector3 = Vector3.Lerp(enemy.sprite.WorldCenter, base.aiActor.sprite.WorldCenter, t);
				GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
				gameObject.transform.position = vector3;
				gameObject.transform.localScale *= 0.75f - (t*0.5f);
				tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
				if (component2 != null)
				{
					component2.ignoreTimeScale = true;
					component2.AlwaysIgnoreTimeScale = true;
					component2.AnimateDuringBossIntros = true;
					component2.alwaysUpdateOffscreen = true;
					component2.playAutomatically = true;
					component2.sprite.usesOverrideMaterial = true;
					component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
					component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
					component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
					component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 10f);
					component2.sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
					component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);
				}
				Destroy(gameObject, 2);
				yield return new WaitForSeconds(0.02f);
			}

			yield break;
		}


		private float timerFloat;
		public void UpdateCables(AIActor enemy)
		{
			if (enemy) { enemy.ApplyEffect(DebuffLibrary.BrainHostBuff); }
			if (!trackedEnemies.Contains(enemy))
            {
				trackedEnemies.Add(enemy);
				VesselMiniomController vesselCont = enemy.gameObject.AddComponent<VesselMiniomController>();
				vesselCont.vessel = base.aiActor;
			}
		}


		public void ClearMods(AIActor enemy)
		{
			foreach (VesselMiniomController vesselCont in enemy.GetComponents(typeof(VesselMiniomController)))
			{
				if (vesselCont.vessel == base.aiActor) 
				{
					vesselCont.RemoveOnPreDeath();
					Destroy(vesselCont);
				}
			}
			enemy.healthHaver.ApplyDamage(enemy.healthHaver.GetMaxHealth() * 0.33f, enemy.transform.position, "brain break");
			enemy.behaviorSpeculator.Stun(2);
			GameObject gameObjecter = SpawnManager.SpawnVFX(StaticVFXStorage.MachoBraceBurstVFX, false);
			Destroy(gameObjecter, 2);

			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, false);
			gameObject.transform.position = enemy.transform.position;
			gameObject.transform.localScale *= 2;
			Destroy(gameObject, 2);
		}

        public bool EnemyBlacklistCheck(AIActor enemy)
        {
			if (enemy == base.aiActor) { return true; }
			if (enemy.healthHaver.IsBoss) { return true; }
			if (enemy.healthHaver.IsSubboss) { return true; }
			if (StaticInformation.ModderBulletGUIDs.Contains(enemy.EnemyGuid)) { return true; }
			if (AdditionalBlackList.Contains(enemy.EnemyGuid)) { return true; }
			if (enemy.GetComponent<VesselController>() != null) { return true; }
			if (enemy.GetComponentInChildren<VesselController>() != null) { return true; }

			return false;
        }

		public List<string> AdditionalBlackList = new List<string>()
		{
			"fodder_enemy",
			"deturretleft_enemy",
			"deturret_enemy",
			"vessel"
		};
		public List<AIActor> allEnemies = new List<AIActor>();
		public List<AIActor> trackedEnemies = new List<AIActor>();

		private bool IsUpdating;
    }

    public class Vessel : AIActor
	{
		public static GameObject prefab;
		public static readonly string guid = "vessel";
		private static tk2dSpriteCollectionData VesselCollection;

		public static void Init()
		{
			Vessel.BuildPrefab();		
		}

	

		public static void BuildPrefab()
		{
			bool flag = prefab != null || EnemyBuilder.Dictionary.ContainsKey(guid);
			bool flag2 = flag;
			if (!flag2)
			{
				prefab = EnemyBuilder.BuildPrefab("Vessel", guid, spritePaths[0], new IntVector2(0, 0), new IntVector2(8, 9), false, true);
				var companion = prefab.AddComponent<EnemyBehavior>();
				prefab.AddComponent<ForgottenEnemyComponent>();
				prefab.AddComponent<VesselController>();
				companion.aiActor.knockbackDoer.weight = 100;
				companion.aiActor.MovementSpeed = 1f;
				companion.aiActor.healthHaver.PreventAllDamage = false;
				companion.aiActor.CollisionDamage = 1f;
				companion.aiActor.IgnoreForRoomClear = true;
				companion.aiActor.aiAnimator.HitReactChance = 0f;
				companion.aiActor.specRigidbody.CollideWithOthers = true;
				companion.aiActor.specRigidbody.CollideWithTileMap = true;
				companion.aiActor.PreventFallingInPitsEver = true;
				companion.aiActor.healthHaver.ForceSetCurrentHealth(74f);
				companion.aiActor.CollisionKnockbackStrength = 0f;
				companion.aiActor.procedurallyOutlined = true;
				companion.aiActor.CanTargetPlayers = true;
				companion.aiActor.IgnoreForRoomClear = false;
				EnemyToolbox.AddShadowToAIActor(companion.aiActor, StaticEnemyShadows.defaultShadow, new Vector2(0.5f, 0.125f), "shadowPos");

				companion.aiActor.healthHaver.SetHealthMaximum(74f, null, false);
				companion.aiActor.specRigidbody.PixelColliders.Clear();
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{
					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyCollider,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 6,
					ManualOffsetY = 9,
					ManualWidth = 14,
					ManualHeight = 23,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0
				});
				companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
				{

					ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
					CollisionLayer = CollisionLayer.EnemyHitBox,
					IsTrigger = false,
					BagleUseFirstFrameOnly = false,
					SpecifyBagelFrame = string.Empty,
					BagelColliderNumber = 0,
					ManualOffsetX = 6,
					ManualOffsetY = 9,
					ManualWidth = 14,
					ManualHeight = 23,
					ManualDiameter = 0,
					ManualLeftX = 0,
					ManualLeftY = 0,
					ManualRightX = 0,
					ManualRightY = 0,



				});
				companion.aiActor.PreventBlackPhantom = false;
				AIAnimator aiAnimator = companion.aiAnimator;
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "teleportOut", new string[] { "teleportOut" }, new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "teleportIn", new string[] { "teleportIn" }, new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "beat", new string[] { "beat" }, new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "thump", new string[] { "thump" }, new DirectionalAnimation.FlipType[1]);
				EnemyToolbox.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1]);

				companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
				companion.aiActor.reinforceType = ReinforceType.SkipVfx;


				aiAnimator.IdleAnimation = new DirectionalAnimation
				{
					Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
					Flipped = new DirectionalAnimation.FlipType[2],
					AnimNames = new string[]
					{
						"idle",
						"idle"
					}
				};

				

				bool flag3 = VesselCollection == null;
				if (flag3)
				{
					VesselCollection = SpriteBuilder.ConstructCollection(prefab, "VesselCollection");
					UnityEngine.Object.DontDestroyOnLoad(VesselCollection);
					for (int i = 0; i < spritePaths.Length; i++)
					{
						SpriteBuilder.AddSpriteToCollection(spritePaths[i], VesselCollection);
					}
					
					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					0,
					1,
					2,
					3,
					3,
					4,		
					5,
					5,
					6,
					
					}, "idle", tk2dSpriteAnimationClip.WrapMode.Loop).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					0,
					1,
					1,
					2,
					2,
					2,
					3,
					3,
					3,
					3,
					4,
					4,
					}, "beat", tk2dSpriteAnimationClip.WrapMode.Once).fps = 18f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "beat", new Dictionary<int, string> { { 10, "Play_ENM_blobulord_reform_01" } });
					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					5,
					5,
					6,
					}, "thump", tk2dSpriteAnimationClip.WrapMode.Once).fps = 7f;

					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14
					}, "teleportOut", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "teleportOut", new Dictionary<int, string> { { 3, "Warp" } });
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "teleportOut", new Dictionary<int, string> { { 1, "Play_BOSS_doormimic_jump_01" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					14,
					14,
					14,
					14,
					13,
					12,
					11,
					10,
					9,
					8,
					7
					}, "teleportIn", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "teleportIn", new Dictionary<int, string> { { 0, "InverseWarp" } });
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "teleportIn", new Dictionary<int, string> { { 1, "Play_BOSS_doormimic_land_01" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					15,
					16,
					17,
					18,
					19,
					18,
					19,
					18,
					19,
					20,//
					21,
					22,
					23,
					24,
					25
					}, "die", tk2dSpriteAnimationClip.WrapMode.Once).fps = 13f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 0, "Play_VesselDeath" }, { 9, "Play_ENM_Tarnisher_Bite_01" } });
					EnemyToolbox.AddEventTriggersToAnimation(prefab.GetComponent<tk2dSpriteAnimator>(), "die", new Dictionary<int, string> { { 9, "Sploosh" } });

					SpriteBuilder.AddAnimation(companion.spriteAnimator, VesselCollection, new List<int>
					{
					26,
					26,
					26,
					27,
					27,
					27,
					28,
					28,
					29,
					30,
					31,
					32,
					33,
					34,
					35,
					36,
					0,
					1,
					2,
					3,
					3,
					4,
					5,
					5,
					6,
					}, "awaken", tk2dSpriteAnimationClip.WrapMode.Once).fps = 11f;
					EnemyToolbox.AddSoundsToAnimationFrame(prefab.GetComponent<tk2dSpriteAnimator>(), "awaken", new Dictionary<int, string> { { 0, "Play_ENM_beholster_teleport_01" }, { 11, "Play_ENM_blobulord_reform_01" } });


				}
				var bs = prefab.GetComponent<BehaviorSpeculator>();
				prefab.GetComponent<ObjectVisibilityManager>();
				BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("43426a2e39584871b287ac31df04b544").behaviorSpeculator;
				bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
				bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

				GameObject shhotPoint = EnemyToolbox.GenerateShootPoint(companion.aiActor.gameObject, new Vector2(0.5f, 1f), "Core");
				
				bs.TargetBehaviors = new List<TargetBehaviorBase>
				{
				new TargetPlayerBehavior
				{
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,
				}
				};

				bs.MovementBehaviors = new List<MovementBehaviorBase>
				{
				new MoveErraticallyBehavior
				{
				   PointReachedPauseTime = 0.1f,
					PathInterval = 0.2f,
					PreventFiringWhileMoving = false,
					StayOnScreen = false,
					AvoidTarget = true,
				}
				};


				bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>() { 
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 1f,
						Behavior = new ShootBehavior{
						ShootPoint = shhotPoint,
						BulletScript = new CustomBulletScriptSelector(typeof(Thump)),
						LeadAmount = 0f,
						AttackCooldown = 2.25f,
						Cooldown = 2f,
						InitialCooldown = 0.5f,
						ChargeTime = 0.8f,
						RequiresLineOfSight = false,
						MultipleFireEvents = false,
						Uninterruptible = true,
						ChargeAnimation = "beat",
						FireAnimation = "thump",
						}
					},
					new AttackBehaviorGroup.AttackGroupItem()
					{
						Probability = 0.7f,
						Behavior = new TeleportBehavior()
				{
					AttackableDuringAnimation = true,
					AllowCrossRoomTeleportation = false,
					teleportRequiresTransparency = false,
					hasOutlinesDuringAnim = true,
					ManuallyDefineRoom = false,
					MaxHealthThreshold = 1f,
					StayOnScreen = true,
					AvoidWalls = true,
					GoneTime = 0.1f,
					OnlyTeleportIfPlayerUnreachable = false,
					MinDistanceFromPlayer = 4f,
					MaxDistanceFromPlayer = -1f,
					teleportInAnim = "teleportIn",
					teleportOutAnim = "teleportOut",
					AttackCooldown = 1f,
					InitialCooldown = 0f,
					RequiresLineOfSight = false,
					roomMax = new Vector2(0,0),
					roomMin = new Vector2(0,0),
					GlobalCooldown = 0.5f,
					Cooldown = 3f,

					CooldownVariance = 1f,
					InitialCooldownVariance = 0f,
					goneAttackBehavior = null,
					IsBlackPhantom = false,
					GroupName = null,
					GroupCooldown = 0f,
					MinRange = 0,
					Range = 0,
					MinHealthThreshold = 0,
					MaxUsages = 0,
					AccumulateHealthThresholds = true,
					targetAreaStyle = null,
					HealthThresholds = new float[0],
					MinWallDistance = 0,
					}
					},
				};

				/*
				companion.healthHaver.spawnBulletScript = true;
				companion.healthHaver.chanceToSpawnBulletScript = 1f;
				companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
				companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(EatPants));
				*/

				bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
				bs.TickInterval = behaviorSpeculator.TickInterval;
				bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
				bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
				bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
				bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
				bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;
				Game.Enemies.Add("psog:vessel", companion.aiActor);


				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Enemies/Vessel/host_idle_001.png", SpriteBuilder.ammonomiconCollection);
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:vessel";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = ""; 
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Enemies/Vessel/host_idle_001";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\sheetvesselTrespass.png");
				PlanetsideModule.Strings.Enemies.Set("#VESSEL", "Vessel");
				PlanetsideModule.Strings.Enemies.Set("#VESSEL_SHORTDESC", "Carrier");
				PlanetsideModule.Strings.Enemies.Set("#VESSEL_LONGDESC", "An amalgam of many, it psychically connects with all nearby living beings and uses them as vessels to stay out of danger.");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#VESSEL";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#VESSEL_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#VESSEL_LONGDESC";
				EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "psog:vessel");
				EnemyDatabase.GetEntry("psog:vessel").ForcedPositionInAmmonomicon = 80;
				EnemyDatabase.GetEntry("psog:vessel").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:vessel").isNormalEnemy = true;

				companion.aiActor.sprite.usesOverrideMaterial = true;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
				mat.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
				mat.SetFloat("_EmissiveColorPower", 2f);
				mat.SetFloat("_EmissivePower", 45);
				companion.aiActor.sprite.renderer.material = mat;

				//EnemyBuilder.SetupEntry(companion.aiActor, "Hells Bells", "These urns of past Gundead can be seen scattered around the Gungeon, with Gungeonners showing little respect to the contents inside.", "Planetside/Resources/Ammocom/johan", "Planetside/Resources/Fodder/fodder_idle_001", "Fodder");
				/*
				SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Fodder/fodder_idle_001.png", SpriteBuilder.ammonomiconCollection);
				//FOR BOSSES USE BOSS ICONS
				if (companion.GetComponent<EncounterTrackable>() != null)
				{
					UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
				}
				companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
				companion.encounterTrackable.journalData = new JournalEntry();
				companion.encounterTrackable.EncounterGuid = "psog:fodder";
				companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
				companion.encounterTrackable.journalData.SuppressKnownState = false;
				companion.encounterTrackable.journalData.IsEnemy = true;
				companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
				companion.encounterTrackable.ProxyEncounterGuid = "";
				companion.encounterTrackable.journalData.enemyPortraitSprite = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\johan.png");
				companion.encounterTrackable.journalData.AmmonomiconSprite = "Planetside/Resources/Fodder/fodder_idle_001.png";

				PlanetsideModule.Strings.Enemies.Set("#FODDER", "Fodder");
				PlanetsideModule.Strings.Enemies.Set("#FODDER_SHORTDESC", "Hells Bells");
				PlanetsideModule.Strings.Enemies.Set("#FODDER_LONGDESC", "Some Bunny will never add ammonomicon descriptions for enemi- PFFFFFFFFFFT");
				companion.encounterTrackable.journalData.PrimaryDisplayName = "#FODDER";
				companion.encounterTrackable.journalData.NotificationPanelDescription = "#FODDER_SHORTDESC";
				companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#FODDER_LONGDESC";
				//EnemyBuilder.AddEnemyToDatabase2(companion.gameObject, "psog:fodder", true, true);
				EnemyDatabase.GetEntry("psog:fodder").ForcedPositionInAmmonomicon = 10000;
				EnemyDatabase.GetEntry("psog:fodder").isInBossTab = false;
				EnemyDatabase.GetEntry("psog:fodder").isNormalEnemy = true;
				*/

			}
		}



		private static string[] spritePaths = new string[]
		{

			"Planetside/Resources/Enemies/Vessel/host_idle_001.png",//0
			"Planetside/Resources/Enemies/Vessel/host_idle_002.png",
			"Planetside/Resources/Enemies/Vessel/host_idle_003.png",
			"Planetside/Resources/Enemies/Vessel/host_idle_004.png",
			"Planetside/Resources/Enemies/Vessel/host_idle_005.png",
			"Planetside/Resources/Enemies/Vessel/host_idle_006.png",
			"Planetside/Resources/Enemies/Vessel/host_idle_007.png",//6

			"Planetside/Resources/Enemies/Vessel/host_teleport_001.png",//7
			"Planetside/Resources/Enemies/Vessel/host_teleport_002.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_003.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_004.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_005.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_006.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_007.png",
			"Planetside/Resources/Enemies/Vessel/host_teleport_008.png",//14

			"Planetside/Resources/Enemies/Vessel/host_die_001.png",//15
			"Planetside/Resources/Enemies/Vessel/host_die_002.png",
			"Planetside/Resources/Enemies/Vessel/host_die_003.png",
			"Planetside/Resources/Enemies/Vessel/host_die_004.png",
			"Planetside/Resources/Enemies/Vessel/host_die_005.png",
			"Planetside/Resources/Enemies/Vessel/host_die_006.png",
			"Planetside/Resources/Enemies/Vessel/host_die_007.png",
			"Planetside/Resources/Enemies/Vessel/host_die_008.png",
			"Planetside/Resources/Enemies/Vessel/host_die_009.png",
			"Planetside/Resources/Enemies/Vessel/host_die_010.png",
			"Planetside/Resources/Enemies/Vessel/host_die_011.png",//25

			"Planetside/Resources/Enemies/Vessel/host_awaken_001.png",//26
			"Planetside/Resources/Enemies/Vessel/host_awaken_002.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_003.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_004.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_005.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_006.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_007.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_008.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_009.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_010.png",
			"Planetside/Resources/Enemies/Vessel/host_awaken_011.png",//36

		};

		public class EnemyBehavior : BraveBehaviour
		{

			private RoomHandler m_StartRoom;

			public void Update()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				if (!base.aiActor.HasBeenEngaged)
				{
					CheckPlayerRoom();
				}
			}
			private void CheckPlayerRoom()
			{
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					GameManager.Instance.StartCoroutine(LateEngage());
				}
				else
				{
					base.aiActor.HasBeenEngaged = false;
				}
			}
			private IEnumerator LateEngage()
			{
				yield return new WaitForSeconds(0.5f);
				if (GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() != null && GameManager.Instance.PrimaryPlayer.GetAbsoluteParentRoom() == m_StartRoom)
				{
					base.aiActor.HasBeenEngaged = true;
				}
				yield break;
			}
			private void Start()
			{
				m_StartRoom = aiActor.GetAbsoluteParentRoom();
				base.aiActor.healthHaver.OnPreDeath += (obj) =>{ };
				base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

			}
			private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
			{
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Warp"))
                {
					Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 3f, 0.1f, 5, 0.5f);

				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("InverseWarp"))
				{
					GameManager.Instance.StartCoroutine(fuck.DoReverseDistortionWaveLocal(base.aiActor.sprite.WorldCenter, 3, 0.1f, 10, 0.5f));
				}
				if (clip.GetFrame(frameIdx).eventInfo.Contains("Sploosh"))
				{
					SpawnManager.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, base.aiActor.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(Splat)), StringTableManager.GetEnemiesString("#TRAP", -1));
					Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 20f, 0.1f, 10, 0.3f);
					for (int i = 0; i < 7; i++)
					{
						GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.BlueSynergyPoofVFX, false);
						gameObject.transform.position = base.aiActor.sprite.WorldCenter + MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0f, 1f));
						gameObject.transform.localScale *= UnityEngine.Random.Range(0.6f, 1.4f);
						tk2dSpriteAnimator component2 = gameObject.GetComponent<tk2dSpriteAnimator>();
						if (component2 != null)
						{
							component2.ignoreTimeScale = true;
							component2.AlwaysIgnoreTimeScale = true;
							component2.AnimateDuringBossIntros = true;
							component2.alwaysUpdateOffscreen = true;
							component2.playAutomatically = true;
							component2.sprite.usesOverrideMaterial = true;
							component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
							component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
							component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
							component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 100f);
							component2.sprite.renderer.material.SetColor("_OverrideColor", Color.cyan);
							component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.cyan);
						}
						Destroy(gameObject, 2);
					}
				}
			}
		}

		public class Splat : Script
        {

			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);

				base.PostWwiseEvent("Play_ENM_creecher_burst_01");
				for (int e = 0; e < 40; e++)
				{
					this.Fire(new Direction(UnityEngine.Random.Range(-180, 180), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(2f, 5), SpeedType.Absolute), new Spore());
				}
				yield break;
			}

			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				protected override IEnumerator Top()
				{		
					base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), UnityEngine.Random.Range(60, 150));
					yield return this.Wait(600);
					base.Vanish(false);	
					yield break;
				}
			}
		}

		public class Thump : Script 
		{
			protected override IEnumerator Top()
			{
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSmallSpore);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableLargeSpore);
				base.BulletBank.Bullets.Add(StaticUndodgeableBulletEntries.undodgeableSniper);

				VesselController vesselCont = base.BulletBank.aiActor.GetComponentInChildren<VesselController>();

				if (vesselCont == null)
                {
					base.StartTask(Obliterate(base.BulletBank.aiActor));
					yield return this.Wait(180);
					yield break;

				}
				if (vesselCont.allEnemies.Count > 0 && vesselCont.allEnemies != null)
				{
					foreach (AIActor enemy in vesselCont.allEnemies)
					{
						base.StartTask(DoSlappy(enemy));
					}
				}
				else
				{
					base.StartTask(Obliterate(base.BulletBank.aiActor));
					yield return this.Wait(180);
				}
				yield break;
			}

			public IEnumerator Obliterate(AIActor enemy)
            {
				if (enemy == null) { yield break; }
				StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(enemy.sprite.WorldCenter);
				yield return this.Wait(60);
			
				float Angle = base.AimDirection;
				GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

				tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
				component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
				component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
				component2.dimensions = new Vector2(1000f, 1f);
				component2.UpdateZDepth();
				component2.HeightOffGround = -2;
				Color laser = new Color(0f, 1f, 1f, 1f);
				component2.sprite.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
				component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
				GameManager.Instance.StartCoroutine(FlashReticles(component2, false, Angle, this, "directedfire"));
				if (enemy == null) { yield break; }

				for (int e = 0; e < 3; e++)
                {
					base.PostWwiseEvent("Play_ENM_creecher_burst_01");
					if (enemy == null) { yield break; }
					for (int i = 0; i < 8; i++)
					{
						this.Fire(Offset.OverridePosition(enemy.sprite.WorldCenter), new Direction((45 * i), DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 3), SpeedType.Absolute), new Spore());
						this.Fire(Offset.OverridePosition(enemy.sprite.WorldCenter), new Direction((45 * i) - 4, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 3), SpeedType.Absolute), new Spore());
						this.Fire(Offset.OverridePosition(enemy.sprite.WorldCenter), new Direction((45 * i) + 4, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 3), SpeedType.Absolute), new Spore());
					}
					yield return this.Wait(20);
				}
				yield break;

			}

			public IEnumerator DoSlappy(AIActor enemy)
            {
				yield return this.Wait(UnityEngine.Random.Range(0, 60));
				if (enemy == null) { yield break; }
				StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(enemy.sprite.WorldCenter);
				yield return this.Wait(60);
				float a = UnityEngine.Random.Range(0, 60);
				base.PostWwiseEvent("Play_ENM_creecher_burst_01");
				for (int i = 0; i < 12; i++)
				{
					if (enemy == null) { yield break; }
					this.Fire(Offset.OverridePosition(enemy.sprite.WorldCenter), new Direction((30 * i)+a, DirectionType.Aim, -1f), new Speed(UnityEngine.Random.Range(1f, 6), SpeedType.Absolute), new Spore());
				}
				yield break;
            }

			private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, bool isDodgeAble, float Angle, Thump parent, string BulletType)
			{
				tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
				float elapsed = 0;
				float Time = 0.125f;
				while (elapsed < Time)
				{
					float t = (float)elapsed / (float)Time;

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					if (tiledspriteObject != null)
					{
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);


						float math = isDodgeAble == true ? 250 : 25;
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (10 * t));
						tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				elapsed = 0;
				Time = 0.375f;
				base.PostWwiseEvent("Play_FlashTell");
				while (elapsed < Time)
				{

					if (parent.IsEnded || parent.Destroyed)
					{
						Destroy(tiledspriteObject.gameObject);
						yield break;
					}
					float t = (float)elapsed / (float)Time;
					if (tiledspriteObject != null)
					{
						float math = isDodgeAble == true ? 350 : 35;
						tiledsprite.transform.position = new Vector3(this.Position.x, this.Position.y, 0);
						tiledsprite.dimensions = new Vector2(1000f, 1f);
						tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (math * t));
						tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f + (20 * t));
						tiledsprite.HeightOffGround = -2;
						tiledsprite.renderer.gameObject.layer = 23;
						tiledsprite.UpdateZDepth();
					}
					elapsed += BraveTime.DeltaTime;
					yield return null;
				}
				Destroy(tiledspriteObject.gameObject);
				if (isDodgeAble == false)
				{
					base.PostWwiseEvent("Play_ENM_bulletking_skull_01", null);
				}
				if (base.BulletBank.aiActor != null)
				{
					base.PostWwiseEvent("Play_Stomp");
					GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
					tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
					objanimator.ignoreTimeScale = true;
					objanimator.AlwaysIgnoreTimeScale = true;
					objanimator.AnimateDuringBossIntros = true;
					objanimator.alwaysUpdateOffscreen = true;
					objanimator.playAutomatically = true;
					ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
					var main = objparticles.main;
					main.useUnscaledTime = true;
					GameObject.Instantiate(silencerVFX.gameObject, base.BulletBank.aiActor.transform.Find("Core").position, Quaternion.identity);
					Exploder.DoDistortionWave(base.BulletBank.aiActor.transform.Find("Core").position, 10f, 0.4f, 3, 0.066f);
					for (int e = 0; e < 3; e++)
					{
						base.Fire(new Direction(Angle, DirectionType.Absolute, -1f), new Speed(25f + (15 + e), SpeedType.Absolute), new UndodgeableBullshit());
					}

				}
				yield break;
			}


			public class UndodgeableBullshit : Bullet
			{
				public UndodgeableBullshit() : base("sniperUndodgeable", false, false, false)
				{

				}
				protected override IEnumerator Top()
				{
					for (int i = 0; i < 100; i++)
					{
						yield return this.Wait(1f);
					}
					yield break;
				}
			}


			public class Spore : Bullet
			{
				public Spore() : base(UnityEngine.Random.value > 0.33f ? StaticUndodgeableBulletEntries.undodgeableSmallSpore.Name : StaticUndodgeableBulletEntries.undodgeableLargeSpore.Name, false, false, false) { }
				protected override IEnumerator Top()
				{
					base.ChangeSpeed(new Speed(12f, SpeedType.Absolute), 90);
					yield break;
				}
			}
		}
	}
}





