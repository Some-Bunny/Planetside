using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace Planetside
{

    public class AllSeeingEye : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "All-Seeing Eye";
            string resourcePath = "Planetside/Resources/PerkThings/allSeeingEye.png";
            GameObject gameObject = new GameObject(name);
            AllSeeingEye item = gameObject.AddComponent<AllSeeingEye>();
          
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "All-Seeing eye.";
            string longDesc = "yep.";
            item.SetupItem(shortDesc, longDesc, "psog");
			AllSeeingEye.AllSeeingEyeID = item.PickupObjectId;
			item.quality = PickupObject.ItemQuality.EXCLUDED;
			PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
			particles.ParticleSystemColor = new Color(230, 230, 250);
			particles.ParticleSystemColor2 = new Color(255, 53, 184);
			OutlineColor = new Color(0.9f, 0.52f, 0.9f);


			AllSeeingEye.gunVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/AllSeeingEye/gunicon.png", null, false);
			AllSeeingEye.itemVFXPrefab = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/AllSeeingEye/itemicon.png", null, false);
			UnityEngine.Object.DontDestroyOnLoad(AllSeeingEye.gunVFXPrefab);
			FakePrefab.MarkAsFakePrefab(AllSeeingEye.gunVFXPrefab);
			AllSeeingEye.gunVFXPrefab.SetActive(false);
			UnityEngine.Object.DontDestroyOnLoad(AllSeeingEye.itemVFXPrefab);
			FakePrefab.MarkAsFakePrefab(AllSeeingEye.itemVFXPrefab);
			AllSeeingEye.itemVFXPrefab.SetActive(false);


            var a = RoomDropModifier.RoomDropModifierData.CreateDummyTable();
            a.AddItemToPool(PickupObjectDatabase.GetById(AllSeeingEyeMiniPickup.MiniEye), 1);


            RoomDropModifier.roomDropModifierDatas.Add(new RoomDropModifier.RoomDropModifierData()
            {
                CanOverrideDropCondition = canOverride,
                IdentificationKey = "A_S_E",
                overridePriority = RoomDropModifier.RoomDropModifierData.OverridePriority.FULL_OVERRIDE,
                OverrideTable = a,
				
            });

        }
		public static GameObject gunVFXPrefab;
		public static GameObject itemVFXPrefab;

		public static int AllSeeingEyeID;
		private static Color OutlineColor;

	

		public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;

            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);

            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }
            AllSeeingEyeController chaos = player.gameObject.GetOrAddComponent<AllSeeingEyeController>();
            //chaos.player = player;
            m_hasBeenPickedUp = true;
            if (chaos.hasBeenPickedup == true)
            { chaos.IncrementStack(); }
            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = chaos.hasBeenPickedup == true ? "See More." : "Gain Foresight.";
            OtherTools.Notify("All Seeing Eye", BlurbText, "Planetside/Resources/PerkThings/allSeeingEye", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);

        }

        public static bool canOverride()
		{
			foreach (PlayerController p in GameManager.Instance.AllPlayers)
			{
				var ase = p.GetComponent<AllSeeingEyeController>();
				ETGModConsole.Log(1);
                if (ase != null)
				{
                    ETGModConsole.Log(2);

                    if (1-1/(1 + 0.17f * ase.Stacks) > UnityEngine.Random.value)
					{
                        ETGModConsole.Log(3);

                        return true;
					}
				}
			}
            ETGModConsole.Log(4);

            return false;
		}

        public float distortionMaxRadius = 30f;
        public float distortionDuration = 2f;
        public float distortionIntensity = 0.7f;
        public float distortionThickness = 0.1f;
        protected void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
				SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			}
			catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, OutlineColor, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			base.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

		public void Interact(PlayerController interactor)
		{
			if (!this)
			{
				return;
			}
			if (RoomHandler.unassignedInteractableObjects.Contains(this))
			{
				RoomHandler.unassignedInteractableObjects.Remove(this);
			}
			SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
			this.Pickup(interactor);
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}

namespace Planetside
{
	public class AllSeeingEyeController : BraveBehaviour
	{
        public AllSeeingEyeController()
        {
            this.Stacks = 1;
            this.hasBeenPickedup = false;
        }
		public bool hasBeenPickedup;

        public void Start()
		{
            PlayerController player = GameManager.Instance.PrimaryPlayer;
			this.RevealSecretRooms();
			GameManager.Instance.OnNewLevelFullyLoaded += this.RevealSecretRooms;
            this.hasBeenPickedup = true;


            player.gameObject.AddComponent<AllSeeingEyeController.AllSeeingEyeChestBehaviour>();
			player.OnRoomClearEvent += Player_OnRoomClearEvent;



        }

		private void Player_OnRoomClearEvent(PlayerController obj)
		{
			if (1 - 1 / (1 + 0.04f * Stacks) > UnityEngine.Random.value)
			{
				IntVector2 bestRewardLocation = obj.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.CameraCenter, true);
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(AllSeeingEyeMiniPickup.MiniEye).gameObject, bestRewardLocation.ToVector3(), Vector2.up, 1f, true, true, false);
			}
		}


        public int Stacks;
        public void IncrementStack()
        {
            Stacks++;
        }

        public void Update()
		{

		}
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
					if (Stacks > 1)
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
		private static string vfxName = "WisperVFX";
		private class AllSeeingEyeChestBehaviour : BraveBehaviour
		{
			private void Start()
			{
				this.player = base.GetComponent<PlayerController>();
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
						bool flag3 = !this.encounteredChests.Contains(chest) && !chest.transform.Find(AllSeeingEyeController.vfxName);
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
								Transform transform2 = transform.Find(AllSeeingEyeController.vfxName);
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
				component.name = AllSeeingEyeController.vfxName;
				component.PlaceAtPositionByAnchor(chest.sprite.WorldTopCenter + this.offset, tk2dBaseSprite.Anchor.LowerCenter);
				component.scale = Vector3.zero;

				if (chest.IsRainbowChest == true | chest.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
				{
					component.usesOverrideMaterial = true;
                    component.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
                }

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
					Transform transform = chest.transform.Find(AllSeeingEyeController.vfxName);
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

}
