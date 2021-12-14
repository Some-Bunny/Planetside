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


namespace Planetside
{
    public class DiasukesPolymorphine  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Daisukes Polymorphine";
            string resourceName = "Planetside/Resources/daisukespolymorph.png";
            GameObject obj = new GameObject(itemName);
            DiasukesPolymorphine activeitem = obj.AddComponent<DiasukesPolymorphine>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Chaos! Chaos!";
            string longDesc = "An incredibly volatile potion capable of transmogrifying enemies on a whim, with incredibly unpredictable results. A note reads on the back: 'Getting any on your skin will result in near guaranteed death.'";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 200f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

			DiasukesPolymorphine.PolyFailVFXPrefab = SpriteBuilder.SpriteFromResource(DiasukesPolymorphine.PolyFailVFX, null, false);
			DiasukesPolymorphine.PolyFailVFXPrefab.name = DiasukesPolymorphine.vfxName;
			UnityEngine.Object.DontDestroyOnLoad(DiasukesPolymorphine.PolyFailVFXPrefab);
			FakePrefab.MarkAsFakePrefab(DiasukesPolymorphine.PolyFailVFXPrefab);
			DiasukesPolymorphine.PolyFailVFXPrefab.SetActive(false);
			DiasukesPolymorphine.DiasukesPolymorphineID = activeitem.PickupObjectId;
			ItemIDs.AddToList(activeitem.PickupObjectId);

		}
		public static int DiasukesPolymorphineID;
		public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {			
			try
			{
				List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
				bool flag = activeEnemies != null;
				if (flag)
				{
					RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
					AIActor randomActiveEnemy;
					

					randomActiveEnemy = user.CurrentRoom.GetRandomActiveEnemy(true);
					int num = 5;
					do
					{
						randomActiveEnemy = user.CurrentRoom.GetRandomActiveEnemy(true);
						num--;
					}
					while (num > 0 && (this.enemyBlacklist.Contains(randomActiveEnemy.EnemyGuid)));
					bool ee = num == 0;
					if (ee)
					{
						AkSoundEngine.PostEvent("Play_OBJ_metronome_fail_01", base.gameObject);
						GameObject original;
						original = DiasukesPolymorphine.PolyFailVFXPrefab;
						tk2dSprite ahfuck = original.GetComponent<tk2dSprite>();
						user.BloopItemAboveHead(ahfuck, "");
						//ETGModConsole.Log("Daisukes Polymorph couldn't choose an appropriate enemy from you current room, oops!", false);
						if (randomActiveEnemy != null)
                        {
							randomActiveEnemy.healthHaver.ApplyDamage(25f, Vector2.zero, "Epic Polymorph Fail", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
						}
					}
					else
                    {
						DiasukesPolymorphine.enemiesToReRoll.Clear();
						DiasukesPolymorphine.enemiesToPostMogModify.Clear();
						for (int i = 0; i < activeEnemies.Count; i++)
						{
							AIActor item = activeEnemies[i];
							DiasukesPolymorphine.enemiesToReRoll.Add(item);
						}
						foreach (AIActor aiactor in DiasukesPolymorphine.enemiesToReRoll)
						{
							if (aiactor != randomActiveEnemy && aiactor.encounterTrackable.EncounterGuid != randomActiveEnemy.encounterTrackable.EncounterGuid && !aiactor.healthHaver.IsBoss)
                            {

								bool flag3 = aiactor.gameObject.GetComponent<ExplodeOnDeath>();
								if (flag3)
								{
									UnityEngine.Object.Destroy(aiactor.gameObject.GetComponent<ExplodeOnDeath>());
								}
								bool flag4 = !aiactor.healthHaver.IsVulnerable;
								if (flag4)
								{
									aiactor.healthHaver.IsVulnerable = true;
								}
								{
									aiactor.Transmogrify(randomActiveEnemy, (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
								}
							}
							else if (aiactor != randomActiveEnemy && aiactor.encounterTrackable.EncounterGuid == randomActiveEnemy.encounterTrackable.EncounterGuid && !aiactor.healthHaver.IsBoss)
							{
								LootEngine.DoDefaultItemPoof(aiactor.sprite.WorldCenter, false, true);
								aiactor.behaviorSpeculator.Stun(3, true);

							}

						}
						foreach (AIActor enemy in DiasukesPolymorphine.enemiesToPostMogModify)
						{
							this.HandlePostTransmogLootEnemies(enemy);
							enemy.behaviorSpeculator.Stun(3, true);

						}
					}
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log(ex.StackTrace, false);
			}
			
		}
		public void HandlePostTransmogLootEnemies(AIActor enemy)
		{
			try
			{
				bool isTransmogrified = enemy.IsTransmogrified;
				if (isTransmogrified)
				{
					enemy.IsTransmogrified = false;
				}
			}
			catch (Exception ex)
			{
				ETGModConsole.Log(ex.Message, false);
				ETGModConsole.Log(ex.StackTrace, false);
			}
		}
		public override bool CanBeUsed(PlayerController user)
		{
			List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			float count = activeEnemies.Count;
			return count >= 2;
		}
		public List<string> enemyBlacklist = new List<string>
		{
			"39de9bd6a863451a97906d949c103538",
			"fa6a7ac20a0e4083a4c2ee0d86f8bbf7",
			"47bdfec22e8e4568a619130a267eab5b",
			"ea40fcc863d34b0088f490f4e57f8913",
			"c00390483f394a849c36143eb878998f",
			"ec6b674e0acd4553b47ee94493d66422",
			"ffca09398635467da3b1f4a54bcfda80",
			"1b5810fafbec445d89921a4efb4e42b7",
			"4b992de5b4274168a8878ef9bf7ea36b",
			"c367f00240a64d5d9f3c26484dc35833",
			"da797878d215453abba824ff902e21b4",
			"5729c8b5ffa7415bb3d01205663a33ef",
			"fa76c8cfdf1c4a88b55173666b4bc7fb",
			"8b0dd96e2fe74ec7bebc1bc689c0008a",
			"5e0af7f7d9de4755a68d2fd3bbc15df4",
			"9189f46c47564ed588b9108965f975c9",
			"6868795625bd46f3ae3e4377adce288b",
			"4d164ba3f62648809a4a82c90fc22cae",
			"6c43fddfd401456c916089fdd1c99b1c",
			"3f11bbbc439c4086a180eb0fb9990cb4",
			"f3b04a067a65492f8b279130323b41f0",
			"41ee1c8538e8474a82a74c4aff99c712",
			"465da2bb086a4a88a803f79fe3a27677",
			"05b8afe0b6cc4fffa9dc6036fa24c8ec",
			"cd88c3ce60c442e9aa5b3904d31652bc",
			"68a238ed6a82467ea85474c595c49c6e",
			"7c5d5f09911e49b78ae644d2b50ff3bf",
			"76bc43539fc24648bff4568c75c686d1",
			"0ff278534abb4fbaaa65d3f638003648",
			"6ad1cafc268f4214a101dca7af61bc91",
			"14ea47ff46b54bb4a98f91ffcffb656d",
			"shellrax",
			"fodder_enemy",
			"deturret_enemy",
			"deturretleft_enemy",
			"Bullet_Banker",
			"Fungannon",
			"Ophanaim",
			"annihichamber",
			"shamber_psog"
		};

		public static List<AIActor> enemiesToReRoll = new List<AIActor>();
		public static List<AIActor> enemiesToPostMogModify = new List<AIActor>();

		private static string PolyFailVFX = "Planetside/Resources/VFX/PolymorphFail/daisukespolymorphfailvfx";

		private static GameObject PolyFailVFXPrefab;

		private static string vfxName = "PolyFailVFX";
	}
}



