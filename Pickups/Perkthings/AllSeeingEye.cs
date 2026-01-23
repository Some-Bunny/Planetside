using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using SaveAPI;
using static Planetside.PrisonerSecondSubPhaseController;
using static UnityEngine.UI.GridLayoutGroup;
using System.ComponentModel;
using Planetside.Controllers;

namespace Planetside
{

	public class AllSeeingEye : PerkPickupObject, IPlayerInteractable
	{
		public static void Init()
		{
			string name = "All-Seeing Eye";
			GameObject gameObject = new GameObject(name);
			AllSeeingEye item = gameObject.AddComponent<AllSeeingEye>();
			var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            bool Stare = FoolMode.isFoolish | UnityEngine.Random.value < 0.25f;
			ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName(Stare ? "allSeeingEyeFun" : "allSeeingEye"), data, gameObject);
			//ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Eye For Secrets.";
			string longDesc = "A Gungeoneers eye, torn out and blessed.\n\nKnows more than you do.";
			item.SetupItem(shortDesc, longDesc, "psog");
			AllSeeingEye.AllSeeingEyeID = item.PickupObjectId;
			item.quality = PickupObject.ItemQuality.EXCLUDED;
			PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
			particles.ParticleSystemColor = new Color(0.98f, 0.94f, 0.2f);
			particles.ParticleSystemColor2 = new Color(1, 0.2f, 0.4f);
			item.OutlineColor = new Color(0.9f, 0.52f, 0.9f);
			item.encounterTrackable.DoNotificationOnEncounter = false;
			item.encounterTrackable.SuppressInInventory = true;
			item.CanBeDropped = false;


			var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
			var GunIcon = ItemBuilder.AddSpriteToObjectAssetbundle("All Seeing Eye Hint", Collection.GetSpriteIdByName("gunicon"), Collection);
			FakePrefab.MarkAsFakePrefab(GunIcon);
			UnityEngine.Object.DontDestroyOnLoad(GunIcon);
            ASEVFXPrefab = GunIcon;



            lootTable = RoomDropModifier.RoomDropModifierData.CreateDummyTable();
            lootTable.AddItemToPool(PickupObjectDatabase.GetById(AllSeeingEyeMiniPickup.MiniEye), 1);
            /*
            dropModifier = new RoomDropModifier.RoomDropModifierData()
            {
                CanOverrideDropCondition = canOverride,
                IdentificationKey = "A_S_E",
                overridePriority = RoomDropModifier.RoomDropModifierData.OverridePriority.FULL_OVERRIDE,
                OverrideTable = a,
            };
            */
            //item.PerkMonobehavior = typeof(AllSeeingEyeController);
			item.StackPickupNotificationText = "See More.";
			item.InitialPickupNotificationText = "Gain Foresight.";
		}
        //public static RoomDropModifier.RoomDropModifierData dropModifier;

        public static GenericLootTable lootTable;

        public override CustomDungeonFlags FlagToSetOnStack => CustomDungeonFlags.ALLSEEINGEYE_FLAG_CHEST_STACK;

        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Gain Foresight.\"",
                    UnlockedString = "\"Gain Foresight.\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Reveals Secrets"),
                    UnlockedString = "Reveals Secret Rooms And Boss Rooms, and potential chest contents.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 4,
                    LockedString = AlphabetController.ConvertString("Gain Pickup Foresight"),
                    UnlockedString = "Room drops can occasionally be Foresight pickups.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.ALLSEEINGEYE_FLAG_ROOMDROP
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 7,
                    LockedString = AlphabetController.ConvertString("Stack Increases Foresight"),
                    UnlockedString = "Stacking Grants More Vision, and pickups.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.ALLSEEINGEYE_FLAG_CHEST_STACK
                },
        };

        public static GameObject ASEVFXPrefab;
		//public static GameObject itemVFXPrefab;

		public static int AllSeeingEyeID;

        public override void OnInitialPickup(PlayerController playerController)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += this.RevealSecretRooms;
			//RoomDropModifier.roomDropModifierDatas.Add(dropModifier);
            //Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents += DetermineRoomDrop;
            playerController.OnRoomClearEvent += Player_OnRoomClearEvent;
        }
        public override void OnStack(PlayerController playerController)
        {
            this.RevealSecretRooms();
        }


        public override void OnDestroy()
        {
			base.OnDestroy();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.RevealSecretRooms;
            //RoomDropModifier.roomDropModifierDatas.Remove(dropModifier);

            Owner.OnRoomClearEvent -= Player_OnRoomClearEvent;
            //Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents -= DetermineRoomDrop;

        }

        private void Player_OnRoomClearEvent(PlayerController obj)
        {
            if (1 - 1 / (1 + 0.04f * CurrentStack) > UnityEngine.Random.value)
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.ALLSEEINGEYE_FLAG_ROOMDROP, true);
                IntVector2 bestRewardLocation = obj.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(AllSeeingEyeMiniPickup.MiniEye).gameObject, bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
            }
        }

        public void DetermineRoomDrop(RoomHandler roomHandler, Alexandria.RoomRewardAPI.ValidRoomRewardContents validRoomRewardContents, float a)
        {
            int amount = PerkHelper.GetGlobalStacksFromAllPlayers(AllSeeingEyeID);
            Debug.Log(1);
            if (1 - 1 / (1 + 0.17f * amount) > UnityEngine.Random.value)
            {
                validRoomRewardContents.overrideItemPool = new List<Tuple<float, int>>() 
                {
                    new Tuple<float, int>(1, AllSeeingEyeMiniPickup.MiniEye),
                    new Tuple<float, int>(1, AllSeeingEyeMiniPickup.MiniEye),
                    new Tuple<float, int>(1, AllSeeingEyeMiniPickup.MiniEye),
                    new Tuple<float, int>(1, AllSeeingEyeMiniPickup.MiniEye),
                    new Tuple<float, int>(1, AllSeeingEyeMiniPickup.MiniEye),

                };
                
            }
        }
        //SelectOverrideContents
        private void RevealSecretRooms()
        {
            for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
            {
                RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
                if (roomHandler.connectedRooms.Count != 0)
                {
                    if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
                    {
                        roomHandler.RevealedOnMap = true;
                        Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, roomHandler == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                    }
                    if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
                    {
                        roomHandler.RevealedOnMap = true;
                        Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, roomHandler == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                    }
                    if (CurrentStack > 1)
                    {
                        if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
                        {
                            roomHandler.RevealedOnMap = true;
                            Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, roomHandler == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                        }
                        if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL)
                        {
                            roomHandler.RevealedOnMap = true;
                            Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, roomHandler == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                        }

                    }
                }
            }
        }


        public static bool canOverride()
		{
            int amount = PerkHelper.GetGlobalStacksFromAllPlayers(AllSeeingEyeID);
            if (1 - 1 / (1 + 0.17f * amount) > UnityEngine.Random.value)
            {
                return true;
            }
            return false;
		}

        private Dictionary<Chest, ChestEffectLerper> ChestWithEffect = new Dictionary<Chest, ChestEffectLerper>();

        public override void Update()
        {
			if (Owner != null && Owner.CurrentRoom != null)
			{
                IPlayerInteractable nearestInteractable = this.Owner.CurrentRoom.GetNearestInteractable(this.Owner.sprite.WorldCenter, 1.5f, this.Owner);
                if (nearestInteractable != null && nearestInteractable is Chest chest)
                {
                    if (!ChestWithEffect.ContainsKey(chest))
                    {
                        this.InitializeChest(chest);
                    }
                    ChestWithEffect[chest].UpdateChestEffect(Owner);
                }
            }
        }

        private void InitializeChest(Chest chest)
        {
            int guess = this.GetGuess(chest);
            tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(ASEVFXPrefab, chest.transform).GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor(chest.sprite.WorldTopCenter + new Vector2(0, 0.25f), tk2dBaseSprite.Anchor.LowerCenter);
            component.scale = Vector3.zero;
            if (chest.IsRainbowChest == true || chest.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
            {
                component.usesOverrideMaterial = true;
                component.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
            }
            if (chest.IsGlitched == true)
            {
                component.usesOverrideMaterial = true;
                Material mate = component.renderer.material;
                mate.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                mate.SetFloat("_GlitchInterval", 0.25f);
                mate.SetFloat("_DispProbability", 0.6f);
                mate.SetFloat("_DispIntensity", 0.3f);
                mate.SetFloat("_ColorProbability", 0.7f);
                mate.SetFloat("_ColorIntensity", 0.1f);

            }
            switch (guess)
            {
                case 0:
                    component.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("itemicon")); break;
                case 1:
                    component.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("gunicon")); break;
            }
            var effectHolder = chest.AddComponent<ChestEffectLerper>();
            effectHolder.Initialize(component, chest);
            ChestWithEffect.Add(chest, effectHolder);
        }




        private int GetGuess(Chest chest)
        {
            Chest.GeneralChestType chestType = chest.ChestType;
            int result = 0;
            switch (chestType)
            {
                case Chest.GeneralChestType.ITEM:
                    result = 0; break;
                case Chest.GeneralChestType.WEAPON:
                    result = 1; break;
                case Chest.GeneralChestType.UNSPECIFIED:
                    result = UnityEngine.Random.Range(0, 2); break;
            }
            return result;
        }

        public class ChestEffectLerper : MonoBehaviour
        {
            private tk2dSprite _Effect;
            private Chest _Chest;

            public void Initialize(tk2dSprite effect, Chest Owner)
            {
                _Effect = effect;
                _Chest = Owner;
                UpdateEffect();
            }

            public void UpdateChestEffect(PlayerController playerController) 
            {
                if (Vector2.Distance(_Chest.sprite.WorldCenter, playerController.sprite.WorldCenter) < 1.25f)
                {
                    InRange = 1;
                    return;
                }
                InRange = 0;
            }
            public int InRange = 0;
            private float Lerp;

            public void Update()
            {
                if (_Chest.IsOpen || _Chest.IsBroken == true)
                {
                    if (_Effect)
                    {
                        _Effect.scale = Vector3.zero;
                    }
                    return;
                }

                if (Lerp != InRange)
                {
                    Lerp = Mathf.MoveTowards(Lerp, InRange, Time.deltaTime * 7.5f);
                    UpdateEffect();
                }
                if (_Chest.m_registeredIconRoom == null && _Effect != null)
                {
                    _Effect.scale = Vector3.zero;
                }
            }


            public void UpdateEffect()
            {
                if (_Effect != null)
                {
                    _Effect.scale = Vector3.one * Lerp;
                }
            }
        }
    }
}

