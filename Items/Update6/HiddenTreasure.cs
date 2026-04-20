using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;
using SaveAPI;
using Brave.BulletScript;
using Alexandria.Misc;
using HarmonyLib;
using Alexandria.PrefabAPI;
using System.ComponentModel;
using HutongGames.PlayMaker.Actions;
using BreakAbleAPI;
using Alexandria.ItemAPI;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using InControl;
using static Dungeonator.CellVisualData;
using tk2dRuntime.TileMap;
using static SpawnEnemyOnDeath;
using Planetside.Static_Storage;
using static Planetside.PrisonerSecondSubPhaseController;
using Planetside.DungeonPlaceables;
using static Planetside.DungeonPlaceables.BuriedObject;
using Planetside.Toolboxes;
using SynergyAPI;

namespace Planetside
{
	public class HiddenTreasure : PassiveItem
	{
		public static void Init()
		{
			string name = "Hidden Gem";
			GameObject gameObject = new GameObject(name);
            HiddenTreasure item = gameObject.AddComponent<HiddenTreasure>();

            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemAPI.ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("hiddengem"), data, gameObject); 
			string shortDesc = "Discover Something New";
			string longDesc = "All Brown Chests come unlocked. Find a buried chest on every floor with free loot.\n\nBrass Gemstones, while commonly overlooked, hold great potential if given a chance.";
            ItemAPI.ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;
            GenerateLockboxAndBuriedTreasure();
            item.AddSynergy("Loot Crate", new List<PickupObject> { Items.Ring_Of_Chest_Friendship });
            item.AddItemToSynergy(CustomSynergyType.ALTERNATIVE_ROCK);
        }

        public static void GenerateLockboxAndBuriedTreasure()
        {
            var lockbox = PrefabBuilder.BuildObject("Lockbox");
            var sprite = lockbox.AddComponent<tk2dSprite>();
            var animator = lockbox.AddComponent<tk2dSpriteAnimator>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("HiddenLockbox_open_001"));
            animator.playAutomatically = false;
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            var lockboxController = lockbox.AddComponent<LockboxController>();
            lockboxController.MinimapIcon = GameManager.Instance.RewardManager.D_Chest.MinimapIconPrefab;
            DontDestroyOnLoad(lockbox);

            lockbox.layer = Layers.BG_Critical;
            sprite.SortingOrder = 0;

            LockBoxPrefab = lockboxController;

            var lockboxDirt = PrefabBuilder.BuildObject("LockboxDirtHint");
            DontDestroyOnLoad(lockboxDirt);
            sprite = lockboxDirt.AddComponent<tk2dSprite>();
            animator = lockboxDirt.AddComponent<tk2dSpriteAnimator>();
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("lockbox_dirt_001"));
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("lockbox_dirt_random");
            lockboxDirt.layer = Layers.BG_Critical;
            sprite.SortingOrder = 0;

            LockboxDirt = lockboxDirt;

            var amorPickup = PrefabBuilder.BuildObject("BuriedLockbox").AddComponent<BuriedLockbox>();
            amorPickup.Tiles = new IntVector2(3, 2);
            amorPickup.gameObject.layer = Layers.BG_Critical;
            amorPickup.RandomObjectsToSpawn = 5;
            amorPickup.RevealednessBeforeFullReveal = 0.5f;
            DontDestroyOnLoad(amorPickup);
            var place2 = BreakableAPI_Bundled.GenerateDungeonPlaceable(new Dictionary<GameObject, float>()
            {
                { amorPickup.gameObject, 1 }
            }, 3, 2);
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("buriedlockbox", place2);
            LockBoxBuriedPrefab = amorPickup;
        }

        public static LockboxController LockBoxPrefab;
        public static GameObject LockboxDirt;
        public static BuriedLockbox LockBoxBuriedPrefab;


        public override void Update()
        {
            base.Update();

        }
        private bool HasSpawnedLockBox = false;
        private bool HasSpawnedSynergyLockbox = false;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            SpawnLockbox(ref HasSpawnedLockBox);
            if (Owner.PlayerHasActiveSynergy("Loot Crate"))
            {
                SpawnLockbox(ref HasSpawnedSynergyLockbox);
            }
            player.OnNewFloorLoaded += ONFL;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnNewFloorLoaded -= ONFL;
            return base.Drop(player);
        }

        public void SpawnLockbox(ref bool Check)
        {
            if (Dungeon.IsGenerating)
                return;

            if (Check)
                return;
            Check = true;
            //GameManager.Instance.Dungeon.PlaceFloorObjectInternal(Alexandria.DungeonAPI.StaticReferences.customPlaceables["buriedlockbox"].variantTiers[0].nonDatabasePlaceable.GetComponent<BuriedObjectController>(), new IntVector2(3, 2), Vector2.zero);


            var data = GameManager.Instance.Dungeon.data;

            List<IntVector2> list = new List<IntVector2>();
            for (int i = 0; i < data.rooms.Count; i++)
            {

                RoomHandler roomHandler = data.rooms[i];

                if (roomHandler != null)
                {



                    if (roomHandler.area != null)
                    {


                        if (!roomHandler.area.IsProceduralRoom && roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL)
                        {
                            for (int j = roomHandler.area.basePosition.x; j < roomHandler.area.basePosition.x + roomHandler.area.dimensions.x; j++)
                            {
                                for (int k = roomHandler.area.basePosition.y; k < roomHandler.area.basePosition.y + roomHandler.area.dimensions.y; k++)
                                {
                                    if (GameManager.Instance.Dungeon.ClearForFloorObject(3, 3, j, k))
                                    {
                                        list.Add(new IntVector2(j, k));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                IntVector2 a = list[BraveRandom.GenerationRandomRange(0, list.Count)];
                RoomHandler absoluteRoom = a.ToVector2().GetAbsoluteRoom();



                var obj =  DungeonPlaceableUtility.InstantiateDungeonPlaceable(LockBoxBuriedPrefab.gameObject, absoluteRoom, a - absoluteRoom.area.basePosition, false, AIActor.AwakenAnimationType.Default, false);


                GameObject gameObject = obj;
                for (int m = 0; m < 3; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        IntVector2 intVector = a + new IntVector2(m, n);
                        if (data.CheckInBoundsAndValid(intVector))
                        {
                           data[intVector].cellVisualData.floorTileOverridden = true;
                        }
                    }
                }
            }

        }

        public void ONFL(PlayerController playerController)
        {
            HasSpawnedLockBox = false;
            HasSpawnedSynergyLockbox = false;
            SpawnLockbox(ref HasSpawnedLockBox);
            if (Owner.PlayerHasActiveSynergy("Loot Crate"))
            {
                SpawnLockbox(ref HasSpawnedSynergyLockbox);
            }
        }

        [HarmonyPatch(typeof(Chest), nameof(Chest.Update))]
        public class ChestUpdate
        {
            [HarmonyPostfix]
            private static void Postfix(Chest __instance)
            {
                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    if (player.HasPassiveItem(ID))
                    {
                        if (__instance.spawnAnimName.StartsWith("wood_"))
                        {
                            if (__instance.IsLocked)
                            {
                                __instance.Unlock();
                            }
                        }
                    }
                }
            }
        }


        public class LockboxController : BraveBehaviour , IPlayerInteractable
        {
            public GameObject MinimapIcon;
            private GameObject MinimapIconInst;


            private List<PickupObject> contents;

            public void Awake()
            {
                roomHandler = this.transform.position.GetAbsoluteRoom();
            }
            private RoomHandler roomHandler;
            public void UncoverLockbox()
            {
                if (roomHandler != null)
                {
                    MinimapIconInst = Minimap.Instance.RegisterRoomIcon(roomHandler, MinimapIcon);
                    roomHandler.RegisterInteractable(this);
                }
            }

            public void Interact(PlayerController interactor)
            {
                if (roomHandler != null)
                {
                    Minimap.Instance.DeregisterRoomIcon(roomHandler, MinimapIconInst);
                    roomHandler.DeregisterInteractable(this);
                }
                GameManager.BroadcastRoomTalkDoerFsmEvent("playerOpenedChest");
                this.spriteAnimator.Play("lockbox_open");
                AkSoundEngine.PostEvent("play_obj_chest_open_01", base.gameObject);
                contents = GameManager.Instance.RewardManager.D_Chest.lootTable.GetItemsForPlayer(interactor, 0, null, null);
                VomitOutLoot();
                interactor.TriggerItemAcquisition();
            }

            public void VomitOutLoot()
            {
                List<DebrisObject> list = new List<DebrisObject>();

                for (int i = 0; i < this.contents.Count; i++)
                {
                    List<DebrisObject> list2 = LootEngine.SpewLoot(
                        new List<GameObject>
                    {
                        this.contents[i].gameObject
                    }, 
                        this.sprite.WorldCenter.ToVector3ZUp(2));
                    list.AddRange(list2);
                    for (int j = 0; j < list2.Count; j++)
                    {
                        if (list2[j])
                        {
                            list2[j].PreventFallingInPits = true;
                        }
                        if (!(list2[j].GetComponent<Gun>() != null))
                        {
                            if (!(list2[j].GetComponent<CurrencyPickup>() != null))
                            {
                                if (list2[j].specRigidbody != null)
                                {
                                    list2[j].specRigidbody.CollideWithOthers = false;
                                    DebrisObject debrisObject = list2[j];
                                    debrisObject.OnTouchedGround = (Action<DebrisObject>)Delegate.Combine(debrisObject.OnTouchedGround, new Action<DebrisObject>(this.BecomeViableItem));
                                }
                            }
                        }
                    }
                }
            }

            protected void BecomeViableItem(DebrisObject debris)
            {
                debris.OnTouchedGround = (Action<DebrisObject>)Delegate.Remove(debris.OnTouchedGround, new Action<DebrisObject>(this.BecomeViableItem));
                debris.OnGrounded = (Action<DebrisObject>)Delegate.Remove(debris.OnGrounded, new Action<DebrisObject>(this.BecomeViableItem));
                debris.specRigidbody.CollideWithOthers = true;
                Vector2 vector = Vector2.zero;
                vector = debris.sprite.WorldCenter - base.sprite.WorldCenter;
                debris.ClearVelocity();
                debris.ApplyVelocity(vector.normalized * 2f);
            }

            public void OnEnteredRange(PlayerController interactor)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
                base.sprite.UpdateZDepth();
            }

            public void OnExitRange(PlayerController interactor)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite);
            }


            public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
            {
                shouldBeFlipped = false;
                return string.Empty;
            }

            public float GetDistanceToPoint(Vector2 point)
            {
                float result;
                if (base.sprite == null)
                {
                    result = 100f;
                }
                else
                {
                    Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.sprite.WorldCenter, base.sprite.GetBounds().size);
                    result = Vector2.Distance(point, v) / 1.5f;
                }
                return result;
            }

            public float GetOverrideMaxDistance()
            {
                return -1f;
            }




        }


        public class BuriedLockbox : BuriedObject.BuriedObjectController
        {
            public LockboxController LockboxInst;

            private int oldLayer;
            private int oldGameObjectLayer;
            public Material BuriedMaterial;


            public override void OnControllerSpawned()
            {
                LockboxInst = UnityEngine.Object.Instantiate(LockBoxPrefab, this.transform.position + new Vector3(0.5f, 0.25f), Quaternion.identity);
                oldLayer = LockboxInst.sprite.renderLayer;
                oldGameObjectLayer = LockboxInst.gameObject.layer;
                BuriedMaterial = LockboxInst.sprite.renderer.material;
                LockboxInst.sprite.renderer.sortingOrder = TileInstances[0].sprite.SortingOrder + 1;
                LockboxInst.sprite.renderLayer = TileInstances[0].sprite.renderLayer;
                LockboxInst.gameObject.layer = TileInstances[0].gameObject.layer;
                LockboxInst.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                LockboxInst.renderer.receiveShadows = false;
                LockboxInst.sprite.usesOverrideMaterial = true;

                var mainTex = LockboxInst.sprite.renderer.material.mainTexture;
                LockboxInst.sprite.renderer.material = new Material(StaticShaders.Default_Shader_Basic);
                LockboxInst.sprite.renderer.material.mainTexture = mainTex;
                //roomHandler.DeregisterInteractable(this);

            }

            public override void OnFullReveal()
            {
                LockboxInst.UncoverLockbox();
                LockboxInst.sprite.renderer.sortingOrder++;
                LockboxInst.sprite.renderLayer = oldLayer;
                LockboxInst.gameObject.layer = oldGameObjectLayer;
                LockboxInst.sprite.renderer.material = BuriedMaterial;
            }

            public override void OnHintObjectRoll(int num)
            {
                var room = this.transform.position.GetAbsoluteRoom();
                if (room != null)
                {
                    var t = room.GetRandomAvailableCellDumb();
                    UnityEngine.Object.Instantiate(LockboxDirt, t.ToVector3(), Quaternion.identity);
                }
            }
        }
        



        public static int ID;
	}
}
