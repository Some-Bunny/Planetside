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
	class DeathWarrantController : MonoBehaviour
    {
		public	void Start()
        {
			Kills = 0;
			State = States.NONE;
			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, base.gameObject.transform.position, Quaternion.identity, false);
			AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", player.gameObject);
			if (gameObject && gameObject.GetComponent<tk2dSprite>())
			{
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.scale *= 0.8f;
				component.HeightOffGround = 5f;
				component.UpdateZDepth();
			}
			base.Invoke("MoveToDifferentTarget", 0.25f);
		}

		private void MoveToDifferentTarget()
        {
			if (player)
            {
				List<AIActor> randomActiveEnemy = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                if (randomActiveEnemy == null | randomActiveEnemy.Count == 0)
                {
                    Destroy(base.gameObject, 0.25f);
                    return;
                }
                else if (RemoveInvalidEnemies(randomActiveEnemy) != null && RemoveInvalidEnemies(randomActiveEnemy).Count > 0)
                { 
					if (State == States.NONE)
                    {
						GameManager.Instance.StartCoroutine(LerpToEnemy(base.transform.position, randomActiveEnemy[UnityEngine.Random.Range(0, randomActiveEnemy.Count)]));
					}
				}
                else
                {
					State = States.NONE;
				}
			}
            else
            {
				Destroy(base.gameObject, 0);
            }
        }

		private List<AIActor> RemoveInvalidEnemies(List<AIActor> aIList)
        {
			for (int i = 0; i < aIList.Count; i++)
			{
				AIActor aI = aIList[i];
				{
					if (aI.IgnoreForRoomClear == true) { aIList.Remove(aI); }
					if (aI.gameObject.GetComponent<MirrorImageController>() != null) { aIList.Remove(aI); }
					if (aI.gameObject.GetComponent<DisplacedImageController>() != null) { aIList.Remove(aI); }
					if (bannedEnemies.Contains(aI.EnemyGuid)) { aIList.Remove(aI); }
					if (StaticInformation.ModderBulletGUIDs.Contains(aI.EnemyGuid)) { aIList.Remove(aI); }
				}
			}
			return aIList;
        }


		public static List<string> bannedEnemies = new List<string>()
		{
			EnemyGuidDatabase.Entries["key_bullet_kin"],
			EnemyGuidDatabase.Entries["chance_bullet_kin"],
			EnemyGuidDatabase.Entries["gummy_spent"],
			EnemyGuidDatabase.Entries["gunreaper"],
			EnemyGuidDatabase.Entries["lead_cube"],
			EnemyGuidDatabase.Entries["brown_chest_mimic"],
			EnemyGuidDatabase.Entries["blue_chest_mimic"],
			EnemyGuidDatabase.Entries["green_chest_mimic"],
			EnemyGuidDatabase.Entries["red_chest_mimic"],
			EnemyGuidDatabase.Entries["black_chest_mimic"],
			EnemyGuidDatabase.Entries["rat_chest_mimic"],
			EnemyGuidDatabase.Entries["pedestal_mimic"],
			EnemyGuidDatabase.Entries["wall_mimic"],
			EnemyGuidDatabase.Entries["mine_flayers_bell"],

			EnemyGuidDatabase.Entries["bullet_kings_toadie"],
			EnemyGuidDatabase.Entries["bullet_kings_toadie_revenge"],
			EnemyGuidDatabase.Entries["old_kings_toadie"],
			EnemyGuidDatabase.Entries["fusebot"],
			EnemyGuidDatabase.Entries["draguns_knife"],
			"deturretleft_enemy",
			"deturret_enemy",
			"fodder_enemy",
			"shamber_psog",		
		};

		public static List<string> targetableButWontIncreaseKillCount = new List<string>()
		{
			EnemyGuidDatabase.Entries["blobulin"],
			EnemyGuidDatabase.Entries["blobuloid"],
			EnemyGuidDatabase.Entries["spent"],
			EnemyGuidDatabase.Entries["poisbulin"],
			EnemyGuidDatabase.Entries["grenade_kin"],
			EnemyGuidDatabase.Entries["dynamite_kin"],
			EnemyGuidDatabase.Entries["bombshee"],
			EnemyGuidDatabase.Entries["m80_kin"],
			EnemyGuidDatabase.Entries["summoned_treadnaughts_bullet_kin"],
			EnemyGuidDatabase.Entries["mouser"],
		};



		private IEnumerator LerpToEnemy(Vector3 oldPos, AIActor newTarget)
        {
			State = States.MOVING_TOWARDS_ENEMY;
			float elapsed = 0f;
			float duration = 0.25f;
			while (elapsed < duration)
			{
				if (base.gameObject == null) { break; }
				if (newTarget == null) { base.Invoke("MoveToDifferentTarget", 0.1f); State = States.NONE; yield break; }
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			AkSoundEngine.PostEvent("Play_BOSS_dragun_throw_01", player.gameObject);

			elapsed = 0f;
			duration = 0.5f;
			while (elapsed < duration)
			{
				if (base.gameObject == null) {break;}
				if (newTarget == null) { base.Invoke("MoveToDifferentTarget", 0.1f); State = States.NONE; yield break; }

				elapsed += BraveTime.DeltaTime;
				float t = elapsed / duration * (elapsed / duration);
				base.gameObject.transform.position = Vector3.Lerp(oldPos, (newTarget.specRigidbody.HitboxPixelCollider == null) ? newTarget.sprite.WorldCenter.ToVector3ZUp(0f) : newTarget.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp(0f) + new Vector3(-0.375f, 1.25f), t);
				yield return null;
			}
			if (newTarget != null)
            {
				State = States.LOCKED_ON;
				newTarget.healthHaver.OnPreDeath += OnPreDeath;
				newTarget.healthHaver.AllDamageMultiplier *= 1.2f;
				Target = newTarget;

				GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, base.gameObject.transform.position, Quaternion.identity, false);
				AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", player.gameObject);
				if (gameObject && gameObject.GetComponent<tk2dSprite>())
				{
					tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
					component.scale *= 0.8f;
					component.HeightOffGround = 5f;
					component.UpdateZDepth();
				}
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(newTarget.sprite);
				if (!newTarget.healthHaver.IsDead && outlineMaterial1 != null)
				{
					if (newTarget.healthHaver != null && newTarget.aiActor != null)
					{
						outlineMaterial1.SetColor("_OverrideColor", new Color(10f, 0f, 0f));
					}
				}
			}
			else
            {
				State = States.NONE;
				yield break;
			}
			yield break;
        }

        private void OnPreDeath(Vector2 obj)
        {
			if (this != null)
            {
				State = States.NONE;
				if (Target != null && !targetableButWontIncreaseKillCount.Contains(Target.EnemyGuid)) { Kills++; }
				Target = null;
                State = States.NONE;
                base.Invoke("MoveToDifferentTarget", 0.1f);
			}
		}

        private void Update()
        {
			if (player)
			{
				AIActor randomActiveEnemy = player.CurrentRoom.GetRandomActiveEnemy(false);
				if (randomActiveEnemy == null)
				{
					Destroy(base.gameObject, 0.5f);
					return;
				}
				else if (Target == null && State == States.NONE)
                {
					base.Invoke("MoveToDifferentTarget", 0.1f);
				}

				if (Target == null && State == States.LOCKED_ON) 
				{
                    State = States.NONE;
                    base.Invoke("MoveToDifferentTarget", 0.1f);
                }
            }
			tk2dSpriteAnimator animator = base.gameObject.GetComponent<tk2dSpriteAnimator>();
			if (Target != null)
            {

				Vector3 a = Target.specRigidbody.HitboxPixelCollider == null ? Target.sprite.WorldCenter.ToVector3ZUp(0f) : Target.specRigidbody.HitboxPixelCollider.UnitCenter.ToVector3ZUp(0f);
				base.gameObject.transform.position = a + new Vector3(-0.375f, 1.25f);
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(Target.sprite);
				if (!Target.healthHaver.IsDead && outlineMaterial1 != null)
				{
					if (Target.healthHaver != null && Target.aiActor != null)
					{
						outlineMaterial1.SetColor("_OverrideColor", new Color(10f, 0f, 0f));
					}
				}
			}
			if (State == States.MOVING_TOWARDS_ENEMY && !animator.IsPlaying("openEye"))
			{animator.Play("openEye");}
			if (State == States.LOCKED_ON && !animator.IsPlaying("target"))
			{animator.Play("target");}
		}

		private GenericLootTable GetRewardValue()
		{
			if (Kills > 13)
            {return DeathWarrant.largeKillsTable;}
			else if (Kills > 8)
			{return DeathWarrant.mediumKillsTable;}
			else if (Kills > 3)
			{return DeathWarrant.smallKillsTable;}
			return null;
		}

		private void OnDestroy()
        {
			State = States.MOVING_TOWARDS_ENEMY;
			GameObject gameObject = SpawnManager.SpawnVFX(StaticVFXStorage.JammedDeathVFX, base.gameObject.transform.position, Quaternion.identity, false);
			AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", player.gameObject);
			GenericLootTable lootTableToUse = GetRewardValue();
			if (lootTableToUse != null)
            {
				PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(PickupObject.ItemQuality.COMMON, lootTableToUse, false);
				LootEngine.SpawnItem(pickupObject.gameObject, base.transform.position, Vector2.up, 0f, true, false, false);
			}		
			if (gameObject && gameObject.GetComponent<tk2dSprite>())
			{
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.scale *= 1.33f;
				component.HeightOffGround = 5f;
				component.UpdateZDepth();
			}
			Destroy(gameObject, 2);
			if (Target != null)
            {Target.healthHaver.OnPreDeath -= OnPreDeath;}
		}
		private AIActor Target;
		private int Kills;
		public PlayerController player;


		private DeathWarrantController.States State;
		private enum States
		{
			MOVING_TOWARDS_ENEMY,
			LOCKED_ON,
			NONE
		};
    }


    public class DeathWarrant : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Death Warrant";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<DeathWarrant>();
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("gundeathwarrant"), data, obj);
            string shortDesc = "Sign It For Everyone";
            string longDesc = "A death warrant granted to you by law." +
                "\n\nOn it are the 10 billion signatures of all the Gundead that roam in the Gungeon.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.C;

			GameObject deathmark = ItemBuilder.AddSpriteToObject("deathmark_vfx", "Planetside/Resources/VFX/DeathMark/markedfordeathvfx_001", null);
			FakePrefab.MarkAsFakePrefab(deathmark);
			UnityEngine.Object.DontDestroyOnLoad(deathmark);
			tk2dSpriteAnimator animator = deathmark.GetOrAddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimation animation = deathmark.AddComponent<tk2dSpriteAnimation>();

			tk2dSpriteCollectionData DeathMarkcollection = SpriteBuilder.ConstructCollection(deathmark, ("DeathMark_Collection"));

			tk2dSpriteAnimationClip SpawnClip = new tk2dSpriteAnimationClip() { name = "spawn", frames = new tk2dSpriteAnimationFrame[0], fps = 7 };
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
			for (int i = 1; i < 8; i++)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_search_00{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
			}
			SpawnClip.frames = frames.ToArray();
			SpawnClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			SpawnClip.loopStart = 5;

			tk2dSpriteAnimationClip TargetClip = new tk2dSpriteAnimationClip() { name = "target", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
			List<tk2dSpriteAnimationFrame> framesTarget = new List<tk2dSpriteAnimationFrame>();
			for (int i = 7; i > 0; i--)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_search_00{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				framesTarget.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
			}
			for (int i = 1; i < 10; i++)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_00{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				framesTarget.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
			}


			tk2dSpriteAnimationClip openEyeClip = new tk2dSpriteAnimationClip() { name = "openEye", frames = new tk2dSpriteAnimationFrame[0], fps = 10 };
			List<tk2dSpriteAnimationFrame> framesEye = new List<tk2dSpriteAnimationFrame>();
			for (int i = 1; i < 8; i++)
			{
				tk2dSpriteCollectionData collection = DeathMarkcollection;
				int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Planetside/Resources/VFX/DeathMark/markedfordeathvfx_search_00{i}", collection);
				tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
				frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerLeft);
				framesEye.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
			}
			openEyeClip.frames = framesEye.ToArray();
			openEyeClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			openEyeClip.loopStart = 5;


			TargetClip.frames = framesTarget.ToArray();
			TargetClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			TargetClip.loopStart = 6;

			animator.Library = animation;
			animator.Library.clips = new tk2dSpriteAnimationClip[] { SpawnClip, TargetClip, openEyeClip };
			animator.DefaultClipId = animator.GetClipIdByName("spawn");
			animator.playAutomatically = true;
			
			DeathMarkPrefab = deathmark;
			DeathWarrant.DeathWarrantID = item.PickupObjectId;
			ItemIDs.AddToList(item.PickupObjectId);

			smallKillsTable = LootTableTools.CreateLootTable();
			smallKillsTable.AddItemsToPool(new Dictionary<int, float>() { { 68, 1 }, { 70, 0.4f } });

			mediumKillsTable = LootTableTools.CreateLootTable();
			mediumKillsTable.AddItemsToPool(new Dictionary<int, float>() {{ 70, 1f }, { 73, 0.4f }, { 120, 0.5f }, { 224, 0.5f } });

			largeKillsTable = LootTableTools.CreateLootTable();
			largeKillsTable.AddItemsToPool(new Dictionary<int, float>() { { 70, 1f }, { 73, 0.7f }, { 120, 0.8f }, { 85, 0.5f }, { 67, 0.5f }, { 224, 0.5f } });
		}

		public static GenericLootTable smallKillsTable;
		public static GenericLootTable mediumKillsTable;
		public static GenericLootTable largeKillsTable;



		public static int DeathWarrantID;
		public static GameObject DeathMarkPrefab;

		private void MarkForDeath()
		{
			List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			bool flag = activeEnemies != null;
			if (flag)
			{
				AIActor randomActiveEnemy;
				randomActiveEnemy = base.Owner.CurrentRoom.GetRandomActiveEnemy(true);
				
				GameObject lightning = SpawnManager.SpawnVFX(DeathMarkPrefab, false);

				lightning.transform.position.WithZ(base.Owner.transform.position.z + 99999);
				lightning.transform.position = base.Owner.transform.position + new Vector3(0, 1.25f);

				ImprovedAfterImage image = lightning.AddComponent<ImprovedAfterImage>();
				image.dashColor = Color.red;
				image.spawnShadows = true;
				image.shadowLifetime = 1;
				image.shadowTimeDelay = 0.025f;

				DeathWarrantController controller = lightning.AddComponent<DeathWarrantController>();
				controller.player = base.Owner;
			}
		}
		public override DebrisObject Drop(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.MarkForDeath));
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.MarkForDeath));
            base.Pickup(player);
		}

		public override void OnDestroy()
        {
			base.OnDestroy();
			if (base.Owner != null)
            {
				base.Owner.OnEnteredCombat = (Action)Delegate.Remove(base.Owner.OnEnteredCombat, new Action(this.MarkForDeath));
			}
		}
	}
}


