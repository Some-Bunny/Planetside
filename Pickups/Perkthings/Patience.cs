﻿using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using SaveAPI;
using System.Collections.Generic;
using System.Collections;

namespace Planetside
{
    class Patience : PerkPickupObject, IPlayerInteractable
    {

        class PatienceController : MonoBehaviour
        {
            public PatienceController()
            {
                this.hasBeenPickedup = false;
            }
            public void Start() 
            { this.hasBeenPickedup = true;
                CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart = 2;
                CrossGameDataStorage.UpdateConfiguration();
            }
            public void IncrementStack()
            {
                CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart += 1;
                CrossGameDataStorage.UpdateConfiguration();
            }
            public bool hasBeenPickedup;
        }


        public static void Init()
        {
            string name = "Patience";
           // string resourcePath = "Planetside/Resources/PerkThings/patience.png";
            GameObject gameObject = new GameObject(name);
            Patience item = gameObject.AddComponent<Patience>();


            var data = StaticSpriteDefinitions.Pickup_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("patience"), data, gameObject);
            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Does nothing... But...";
            string longDesc = "Given enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop.\n\nGiven enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop.\n\nGiven enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop.\n\nGiven enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop.\n\nGiven enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop.\n\nGiven enough time, the patient gungeoneer shall be rewarded, as this hourglass slips into the gungeons recursive time-stream and re-appears at the start of the loop...";
            item.SetupItem(shortDesc, longDesc, "psog");
            Patience.PatienceID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            PerkParticleSystemController particles = gameObject.AddComponent<PerkParticleSystemController>();
            particles.ParticleSystemColor = Color.cyan;
            particles.ParticleSystemColor2 = Color.blue;
            item.OutlineColor = new Color(0f, 0f, 0.4f);



            Actions.OnRunStart += OnRunStart;
        }
        public override List<PerkDisplayContainer> perkDisplayContainers => new List<PerkDisplayContainer>()
        {
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 0,
                    LockedString = "\"Does nothing... But...\"",
                    UnlockedString = "\"Does nothing... But...\"",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 1,
                    LockedString = AlphabetController.ConvertString("Patience Is Worthy"),
                    UnlockedString = "Grants a selection of perks to choose from at the start of the next run.",
                    requiresFlag = false
                },
                new PerkDisplayContainer()
                {
                    AmountToBuyBeforeReveal = 3,
                    LockedString = AlphabetController.ConvertString("Larger Selection"),
                    UnlockedString = "Stacking grants more options.",
                    FlagToTrack = SaveAPI.CustomDungeonFlags.PATIENCE_FLAG_STACK
                },
        };
        public override CustomTrackedStats StatToIncreaseOnPickup => SaveAPI.CustomTrackedStats.AMOUNT_BOUGHT_PATIENCE;


        public static void OnRunStart(PlayerController player, PlayerController player2, GameManager.GameMode gameMode)
        {
            int perkCount = CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart;
            if (perkCount > 0)
            {
                GameManager.Instance.StartCoroutine(DelaySpawn(player, gameMode, perkCount));
            }
        }
        public static IEnumerator DelaySpawn(PlayerController player, GameManager.GameMode gameMode, float perkCount)
        {
            float d = 1.75f;
            if (gameMode != GameManager.GameMode.NORMAL) { d = 1f; }
            yield return new WaitForSeconds(d);


            float offset = 0;
            if (gameMode != GameManager.GameMode.NORMAL) { offset = -6; }

            AkSoundEngine.PostEvent("Play_PortalOpen", player.gameObject);


            float RNGOffset = BraveUtility.RandomAngle();
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

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, 30, 0.5f, 50, 1);

            for (int e = 0; e < perkCount; e++)
            {
                if (IDsUsed.Count == 0 | IDsUsed == null)
                {
                    IDsUsed = new List<int>()
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
                }

                int IDtoUse = BraveUtility.RandomElement<int>(IDsUsed);
                IDsUsed.Remove(IDtoUse);
                DebrisObject debrisSpawned = LootEngine.SpawnItem(PickupObjectDatabase.GetById(IDtoUse).gameObject, player.sprite.WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle(((360 / perkCount) * e) + RNGOffset, 1.25f).ToVector3ZisY() + new Vector3(-0.5f, offset), MathToolbox.GetUnitOnCircle(((360 / perkCount) * e) + RNGOffset, 5), 2);
                if (debrisSpawned == null) { ETGModConsole.Log("bastard"); }

                perksThatExist.Add(debrisSpawned);
            }

            if (perksThatExist.Count >= 0 || perksThatExist != null)
            {
                GameManager.Instance.Dungeon.StartCoroutine(ItemChoiceCoroutine(perksThatExist));
            }
            CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart = 0;
            CrossGameDataStorage.UpdateConfiguration();


            yield break;
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


        public static int PatienceID;

        public new bool PrerequisitesMet()
        {
            EncounterTrackable component = base.GetComponent<EncounterTrackable>();
            return component == null || component.PrerequisitesMet();
        }
        public override void Pickup(PlayerController player)
        {
            if (m_hasBeenPickedUp)
                return;
            base.HandleEncounterable(player);

            SaveAPI.AdvancedGameStatsManager.Instance.RegisterStatChange(StatToIncreaseOnPickup, 1);

            m_hasBeenPickedUp = true;
            PerkParticleSystemController cont = base.GetComponent<PerkParticleSystemController>();
            if (cont != null) { cont.DoBigBurst(player); }

            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", player.gameObject);


            PatienceController blast = player.gameObject.GetOrAddComponent<PatienceController>();
            if (blast.hasBeenPickedup == true)
            { blast.IncrementStack(); SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.PATIENCE_FLAG_STACK, true); }

            Exploder.DoDistortionWave(player.sprite.WorldTopCenter, this.distortionIntensity, this.distortionThickness, this.distortionMaxRadius, this.distortionDuration);
            player.BloopItemAboveHead(base.sprite, "");
            string BlurbText = blast.hasBeenPickedup == true ? "But more..." : "Does nothing... But...";
            OtherTools.NotifyCustom("Patience", BlurbText, "patience", StaticSpriteDefinitions.Pickup_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            //OtherTools.Notify("Patience", BlurbText, "Planetside/Resources/PerkThings/patience", UINotificationController.NotificationColor.GOLD);
            UnityEngine.Object.Destroy(base.gameObject);
        }


        public void Start()
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

    

        

        private void Update()
        {
            if (!this.m_hasBeenPickedUp && !this.m_isBeingEyedByRat && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
            {
                GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
            }
        }

        

        private bool m_hasBeenPickedUp;
    }
}
