
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;
using Alexandria.PrefabAPI;
using UnityEngine;
using System.Reflection;

namespace Planetside
{

	public static class BrokenChamberShrine
	{

		public static void Add()
		{
			ShrineFactory iei = new ShrineFactory
			{
				name = "BrokenChamberShrine",
				modID = "psog",
				text = "A shrine with a half-broken chamber on it. It's seems loose...",
				spritePath = "Planetside/Resources/Shrines/brokenchambershrine.png",
				//room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/BrokenChamberRoom.room").room,
				//RoomWeight = 10,
				acceptText = "Lift the remnant.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = "Planetside/Resources/Shrines/defaultShrineShadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.25f,
                AdditionalComponent = typeof(BrokenChamberShrineController)
			};
			GameObject self = iei.Build();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:brokenchambershrine", self);
			SpriteID = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/brokenchambershrinelifted.png", self.GetComponent<tk2dBaseSprite>().Collection);
            SpriteID2 = ItemAPI.SpriteBuilder.AddSpriteToCollection("Planetside/Resources/Shrines/EOEShrine.png", self.GetComponent<tk2dBaseSprite>().Collection);

            var VFX = PrefabBuilder.BuildObject("EndOfEverythingRing");
            ItemAPI.FakePrefab.DontDestroyOnLoad(VFX);
            ItemAPI.FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("EndOfEverythingRing"));
            tk2d.IsPerpendicular = false;
            tk2d.usesOverrideMaterial = true;
            tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

            ExpandReticleRiserEffect rRE = VFX.gameObject.AddComponent<ExpandReticleRiserEffect>();
            rRE.RiserHeight = 0.75f;
            rRE.RiseTime = 5;
            rRE.NumRisers = 5;
            rRE.UpdateSpriteDefinitions = true;
            rRE.CurrentSpriteName = "EndOfEverythingRing";
            SpriteRing = VFX;

            EnemyToolbox.GenerateShootPoint(tk2d.gameObject, new Vector2(0, 2.5f), "__1");
            EnemyToolbox.GenerateShootPoint(tk2d.gameObject, new Vector2(-1.825f, -1.4375f), "__2");
            EnemyToolbox.GenerateShootPoint(tk2d.gameObject, new Vector2(1.825f, -1.4375f), "__3");

            EnemyToolbox.GenerateShootPoint(tk2d.gameObject, new Vector2(0, 0), "__top");
            /*
            foreach (var entry in SortingLayer.layers)
            {
                ETGModConsole.Log($"{entry.id} | {entry.name}");
            }
            */

        }


        private static int SpriteID;
        private static int SpriteID2;

        public static GameObject SpriteRing;


        public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

        public class BrokenChamberShrineController : BraveBehaviour
        {

            public BrokenChamberShrineController()
            {

            }

            public GameObject Ring;

            public void Start()
            {
                bool Shrine = SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED);
                if (Shrine == true)
                {
                    tk2dSprite sprite = base.gameObject.GetComponent<tk2dSprite>();
                    sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID2);
                    try
                    {
                        Ring = UnityEngine.Object.Instantiate(SpriteRing, this.transform.position+ new Vector3(0.625f, 0.375f), Quaternion.identity);
                        transform_1 = Ring.transform.Find("__1");
                        transform_2 = Ring.transform.Find("__2");
                        transform_3 = Ring.transform.Find("__3");
                        transform_top = Ring.transform.Find("__top");


                        SimpleShrine shrine = base.gameObject.GetComponent<SimpleShrine>();
                        shrine.text = "A shrine with 4 engravings carved onto it. Although the engravings shift, you can slightly make out what they are...";
                        shrine.OnAccept = Accept;
                        shrine.OnDecline = null;
                        shrine.acceptText = "Kneel.";
                        shrine.declineText = "Leave.";
                    }
                    catch
                    {
                        ETGModConsole.Log("Failure in modifying shrines (1)");
                    }
                }
            }

            public void Update()
            {
                if (Ring)
                {
                    Ring.transform.Rotate(0, 0, 22.5f * Time.deltaTime);
                    transform_1.transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform_2.transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform_3.transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform_top.transform.rotation = Quaternion.Euler(0, 0, 0);

                }
            }

            public Transform transform_1;
            public Transform transform_2;
            public Transform transform_3;
            public Transform transform_top;

            public void Accept(PlayerController player, GameObject shrine)
            {

                //EndOfEverythingRing
                /*
                Gun gun = PickupObjectDatabase.GetById(InitialiseGTEE.HOneToShootIt) as Gun;
                int StoredGunID = gun.PickupObjectId;

                //PickupObject Item1 = PickupObjectDatabase.GetByName(InitialiseGTEE.HOneToFireIt);
                int Item1ID = Game.Items[InitialiseGTEE.HOneToFireIt].PickupObjectId;
                int Item2ID = Game.Items[InitialiseGTEE.HOneToPrimeIt].PickupObjectId;
                int Item3ID = Game.Items[InitialiseGTEE.HOneToHoldIt].PickupObjectId;

                string encounterNameOrDisplayName1 = (PickupObjectDatabase.GetById(StoredGunID) as Gun).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName2 = (PickupObjectDatabase.GetById(Item1ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName3 = (PickupObjectDatabase.GetById(Item2ID)).EncounterNameOrDisplayName;
                string encounterNameOrDisplayName4 = (PickupObjectDatabase.GetById(Item3ID)).EncounterNameOrDisplayName;

                string header;
                string text;

                header = encounterNameOrDisplayName1 + " / " + encounterNameOrDisplayName2;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);


                header = encounterNameOrDisplayName3 + " / " + encounterNameOrDisplayName4;
                text = "Filler.";
                BrokenChamberShrineController.Notify(header, text);

                */
                player.StartCoroutine(DoWait());
                shrine.GetComponent<ShrineFactory.CustomShrineController>().numUses++;
                shrine.GetComponent<ShrineFactory.CustomShrineController>().GetRidOfMinimapIcon();
            }

            public IEnumerator DoWait()
            {
                float e = 0;
                while (e < 1)
                {
                    e+= Time.deltaTime;
                    yield return null;
                }
                e = 0;
                GameObject gameObject = new GameObject("Icon_1");
                gameObject.transform.SetParent(transform_1);
                gameObject.transform.position = transform_1.position;

                var sprite = gameObject.AddComponent<tk2dSprite>();
                sprite.renderer.sortingLayerName = "Foreground";
                sprite.SetSprite(Game.Items[InitialiseGTEE.HOneToFireIt].sprite.collection, Game.Items[InitialiseGTEE.HOneToFireIt].sprite.spriteId);
                SpriteOutlineManager.AddOutlineToSprite(sprite, Color.yellow, 2, 1, SpriteOutlineManager.OutlineType.NORMAL);
                ImprovedAfterImage yes = sprite.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 1.5f;
                yes.shadowTimeDelay = 0.025f;
                yes.dashColor = new Color(1, 0.7f, 0f, 1.5f);
                gameObject.transform.localPosition += new Vector3(-sprite.GetBounds().size.x * 0.5f, 0.5f);


                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, sprite.WorldBottomCenter, Quaternion.Euler(0, 0, 0));
                gameObject2.transform.localPosition += new Vector3(-0.75f, 0);
                gameObject2.transform.localScale *= 0.7f;
                Destroy(gameObject2, 2);


                AkSoundEngine.PostEvent("Play_ENM_mummy_cast_01", base.gameObject);
                while (e < 1)
                {
                    e += Time.deltaTime;
                    yield return null;
                }
                e = 0;
                gameObject = new GameObject("Icon_2");
                gameObject.transform.SetParent(transform_2);
                gameObject.transform.position = transform_2.position;

                sprite = gameObject.AddComponent<tk2dSprite>();
                sprite.renderer.sortingLayerName = "Foreground";

                sprite.SetSprite(Game.Items[InitialiseGTEE.HOneToPrimeIt].sprite.collection, Game.Items[InitialiseGTEE.HOneToPrimeIt].sprite.spriteId);
                SpriteOutlineManager.AddOutlineToSprite(sprite, Color.yellow, 2, 1, SpriteOutlineManager.OutlineType.NORMAL);
                gameObject.transform.localPosition += new Vector3(-sprite.GetBounds().size.x * 0.5f, 0.5f);

                gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, sprite.WorldBottomCenter, Quaternion.Euler(0, 0, 0));
                gameObject2.transform.localPosition += new Vector3(-0.75f, 0);
                gameObject2.transform.localScale *= 0.7f;
                Destroy(gameObject2, 2);

                yes = sprite.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 1.5f;
                yes.shadowTimeDelay = 0.025f;
                yes.dashColor = new Color(1, 0.7f, 0f, 1.5f);
                AkSoundEngine.PostEvent("Play_ENM_mummy_cast_01", base.gameObject);
                while (e < 1)
                {
                    e += Time.deltaTime;
                    yield return null;
                }
                e = 0;
                gameObject = new GameObject("Icon_3");
                gameObject.transform.SetParent(transform_3);
                gameObject.transform.position = transform_3.position;


                sprite = gameObject.AddComponent<tk2dSprite>();
                sprite.renderer.sortingLayerName = "Foreground";

                yes = sprite.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 1.5f;
                yes.shadowTimeDelay = 0.025f;
                yes.dashColor = new Color(1, 0.7f, 0f, 1.5f);

                sprite.SetSprite(Game.Items[InitialiseGTEE.HOneToHoldIt].sprite.collection, Game.Items[InitialiseGTEE.HOneToHoldIt].sprite.spriteId);
                SpriteOutlineManager.AddOutlineToSprite(sprite, Color.yellow, 2, 1, SpriteOutlineManager.OutlineType.NORMAL);
                gameObject.transform.localPosition += new Vector3(-sprite.GetBounds().size.x *0.5f, 0.5f);
                
                gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, sprite.WorldBottomCenter, Quaternion.Euler(0, 0, 0));
                gameObject2.transform.localPosition += new Vector3(-0.75f, 0);
                gameObject2.transform.localScale *= 0.7f;
                Destroy(gameObject2, 2);

                AkSoundEngine.PostEvent("Play_ENM_mummy_cast_01", base.gameObject);
                while (e < 1.5f)
                {
                    e += Time.deltaTime;
                    yield return null;
                }
                e = 0;
                gameObject = new GameObject("Icon_4");
                gameObject.transform.SetParent(transform_top, false);
                gameObject.transform.position = transform_top.position;

                sprite = gameObject.AddComponent<tk2dSprite>();
                sprite.renderer.sortingLayerName = "Foreground";
                yes = sprite.gameObject.AddComponent<ImprovedAfterImage>();
                yes.spawnShadows = true;
                yes.shadowLifetime = 1.5f;
                yes.shadowTimeDelay = 0.025f;
                yes.dashColor = new Color(1, 0.7f, 0f, 1.5f);

                var gun = (PickupObjectDatabase.GetById(InitialiseGTEE.HOneToShootIt) as Gun).GetComponent<tk2dSprite>();

                sprite.SetSprite(gun.collection, gun.sprite.spriteId);
                SpriteOutlineManager.AddOutlineToSprite(sprite, Color.yellow, 2, 1, SpriteOutlineManager.OutlineType.NORMAL);
                gameObject.transform.localPosition += new Vector3(-sprite.GetBounds().size.x * 0.5f, 2f);
               
                gameObject2 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, sprite.WorldBottomCenter, Quaternion.Euler(0, 0, 0));
                gameObject2.transform.localPosition += new Vector3(-0.75f, 0);
                gameObject2.transform.localScale *= 0.7f;
                Destroy(gameObject2, 2);
                
                ExpandReticleRiserEffect rRE = sprite.gameObject.AddComponent<ExpandReticleRiserEffect>();
                rRE.RiserHeight = 0.5f;
                rRE.RiseTime = 2;
                rRE.NumRisers = 3;
                rRE.UpdateSpriteDefinitions = true;


                AkSoundEngine.PostEvent("Play_ENM_mummy_cast_01", base.gameObject);

                //m_ENM_mummy_cast_01
                yield break;
            }

        }






        public static void Accept(PlayerController player, GameObject shrine)
		{
			tk2dSprite sprite = shrine.GetComponent<tk2dSprite>();
			sprite.GetComponent<tk2dBaseSprite>().SetSprite(SpriteID);
			AkSoundEngine.PostEvent("Play_ENM_darken_world_01", shrine);
			LootEngine.TryGivePrefabToPlayer(ETGMod.Databases.Items["Broken Chamber"].gameObject, player, true);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
		}
	}
}



