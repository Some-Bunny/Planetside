using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace Planetside
{

    class AllSeeingEye : PickupObject, IPlayerInteractable
    {
        public static void Init()
        {
            string name = "All-Seeing Eye Perk";
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

		}
		public static int AllSeeingEyeID;
		private static Color OutlineColor;


		public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            if (!player.CurrentItem)
            {
                return;
            }

			PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
			if (cont != null) { cont.DoBigBurst(player); }

            m_hasBeenPickedUp = true;
			AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);
			player.gameObject.AddComponent<AllSeeingEye>();

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            OtherTools.Notify("All Seeing Eye", "Gain Foresight.", "Planetside/Resources/PerkThings/allSeeingEye", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
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
