using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using UnityEngine.Playables;
using System.Collections;
using System.ComponentModel;
using UnityEngine.Video;
using static ETGMod;
using PathologicalGames;

namespace Planetside
{
    public class MoneyPots
    {
        public static void Init()
        {
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { GenerateCopperpot().gameObject, 1f },
                { GenerateSilverpot().gameObject, 0.1f },
                { GenerateGoldpot().gameObject, 0.01f },
                { GenerateGlitchpot().gameObject, 0.001f },

                //GenerateGlitchpot
            };
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("moneyPotRandom", placeable);

            string defaultPath = "Planetside/Resources/DungeonObjects/TutorialNote/";
            string[] idlePaths = new string[]{defaultPath+"tatterednote.png",};

            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_1", "You Lost The Game.");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_2", "Download Fiend Folio on the Steam Workshop today! (Let me in I want the fiend folio community clout please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please please )");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_3", "Lead Maidens are singlehandedly ruining this game. There's no reason an extremely common random enemy should be tons harder than everything else so far including the floor 2 boss even though Lead Maidens started showing up before that fight. It makes every moment not spent fighting a Lead Maiden pointless because whether or not I win depends 99% on whether or not that bullshit miniboss appears. I've never seen one nonboss singlehandedly ruin a game before but this one is doing it extremely efficiently. Beating one Lead Maiden is harder than beating Rabi-Ribi's True Boss Rush. It's fucking unforgivable to have the game change so radically every time that enemy show up. It kills me in like one fucking hit & it has far more health than everything else I've fought before despite also being bigger & faster than everything else too.\r\n\r\nWhomever came up with that enemy needs to go jack off to medieval torture porn & get it out of their system. Jesus Christ!\r\n\r\nI really wish the creators of this game would've just decided whether to make an amazing game or the worst game ever & stuck with it.");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_4", "AMONG US (vine boom)");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_5", "I Ii\n\nII L");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_6", "JUDGEMENT! DIE! DIE! PREPARE THYSELF! DIE! JUDGEMENT! CRUSH! THY END IS NOW! DIE! JUDGEMENT! CRUSH! DIE! THY END IS NOW! PREPARE THYSELF! JUDGMENT! DIE! JUDGEMENT! PREPARE THYSELF! DIE! JUDGEMENT! CRUSH! THY END IS NOW!  CRUSH! THY END IS NOW! DIE! JUDGEMENT! CRUSH! DIE! THY END IS NOW! PREPARE THYSELF! JUDGMENT! DIE!");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_7", "Fuck it, we ball\nFuck it, we ball\nFuck it, we ball\nFuck it, we ball\nFuck it, we ball");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_8", "i could go for a lasagna right now.");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_9", "Behind you.");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_10", ":rabbit2:\n\n:skateboard:");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_11", "Our mod plans to expand and revamp vanilla content by adding new fun and interesting content! we plan on adding new gimmicks and multiple bosses to flesh out the game. (gif of buzz lightyears in a toy aisle)");
            ETGMod.Databases.Strings.Core.Set("#TROLL_NOTE_12", "Shit thyself.");


            MajorBreakable note1 = BreakableAPIToolbox.GenerateMajorBreakable("trollNote_1", idlePaths, 1, idlePaths, 1, 15000,true, 0, 0, 0, 0, true, null, null, true, null);

            NoteDoer finishedNote1 = BreakableAPIToolbox.GenerateNoteDoer(note1, BreakableAPIToolbox.GenerateTransformObject(note1.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#TROLL_NOTE_01", true);
            StaticReferences.StoredRoomObjects.Add("trollNote", finishedNote1.gameObject);

        }

        public static IEnumerator StartHook(Func<MinorBreakable, IEnumerator> orig, MinorBreakable s)
        {
            IEnumerator origEnum = orig(s);
            ETGModConsole.Log(s.gameObject.layer);
            ETGModConsole.Log(s.sprite.HeightOffGround);
            ETGModConsole.Log("==");

            while (origEnum.MoveNext())
            {
                object obj = origEnum.Current;
                yield return obj;
            }


            yield break;
        }

        public class MoneyPotBehavior : MonoBehaviour
        {
            public GameObject objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.goldCoinPrefab;
            public void Start()
            {
                var minorbreakable = this.GetComponent<MinorBreakable>();
                if (minorbreakable != null)
                {
                    minorbreakable.OnBreakContext += (self) =>
                    {
                        Vector3 vector = Vector3.up;
                        vector *= 2f;
                        GameObject gameObject = SpawnManager.SpawnDebris(objectToSpawn, self.sprite.WorldCenter.ToVector3ZUp(self.transform.position.z), Quaternion.identity);
                        DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                        orAddComponent.shouldUseSRBMotion = true;
                        orAddComponent.angularVelocity = 0f;
                        orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                        orAddComponent.Trigger(vector.WithZ(4f), 0.05f, 1f);
                        orAddComponent.canRotate = false;
                    };
                }
            }
        }

        public static GameObject GenerateGoldpot()
        {
            string shadowPath = "Planetside/Resources/DungeonObjects/MoneyPots/money_pot_shadow.png";
            string defaultPath = "Planetside/Resources/DungeonObjects/MoneyPots/Gold/";

            string[] shardPaths = new string[]
            {
                defaultPath+"Debris/gold_debris_idle_001.png",
                defaultPath+"Debris/gold_debris_idle_002.png",
                defaultPath+"Debris/gold_debris_idle_003.png",
                defaultPath+"Debris/gold_debris_idle_004.png",
                defaultPath+"Debris/gold_debris_idle_005.png",
                defaultPath+"Debris/gold_debris_idle_006.png",
                defaultPath+"Debris/gold_debris_idle_007.png",
            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 3, 6, 720, 540, null, 0.6f, null, null, 0, false);
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Gold_Pot", new string[] 
            { defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_001.png",
            defaultPath + "gold_pot_idle_002.png",
            defaultPath + "gold_pot_idle_003.png",
            defaultPath + "gold_pot_idle_004.png",
            defaultPath + "gold_pot_idle_005.png",
            defaultPath + "gold_pot_idle_006.png",
            defaultPath + "gold_pot_idle_007.png",
            }, 10, 
            new string[] { defaultPath + "gold_pot_break_001.png" }, 10, 
            "Play_OBJ_pot_shatter_01", true, 14, 18, 1, 0);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "moneyPot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));
            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 0;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.gameObject.layer = 22;
            breakable.sprite.HeightOffGround = -1;
            breakable.shardClusters = new ShardCluster[] { potShardCluster };
            var moneyPotvar =  breakable.gameObject.AddComponent<MoneyPotBehavior>();
            moneyPotvar.objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.goldCoinPrefab;

            tk2dSpriteAnimator sprite = breakable.spriteAnimator;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 10);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);

            sprite.renderer.material = mat;

            StaticReferences.StoredRoomObjects.Add("Gold_Pot", breakable.gameObject);

            return breakable.gameObject;
        }

        public static GameObject GenerateSilverpot()
        {
            string shadowPath = "Planetside/Resources/DungeonObjects/MoneyPots/money_pot_shadow.png";
            string defaultPath = "Planetside/Resources/DungeonObjects/MoneyPots/Silver/";

            string[] shardPaths = new string[]
            {
                defaultPath+"Debris/silver_debris_1.png",
                defaultPath+"Debris/silver_debris_2.png",
                defaultPath+"Debris/silver_debris_3.png",
                defaultPath+"Debris/silver_debris_4.png",
                defaultPath+"Debris/silver_debris_5.png",
                defaultPath+"Debris/silver_debris_6.png",
                defaultPath+"Debris/silver_debris_7.png",
            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 3, 6, 720, 540, null, 0.6f, null, null, 0, false);
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Silver_Pot", new string[]
            { defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_001.png",
            defaultPath + "silver_pot_idle_002.png",
            defaultPath + "silver_pot_idle_003.png",
            defaultPath + "silver_pot_idle_004.png",
            defaultPath + "silver_pot_idle_005.png",
            defaultPath + "silver_pot_idle_006.png",
            defaultPath + "silver_pot_idle_007.png",
            }, 10,
            new string[] { defaultPath + "silver_pot_break_001.png" }, 10,
            "Play_OBJ_pot_shatter_01", true, 14, 18, 1, 0);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "moneyPot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));

            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 0;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.gameObject.layer = 22;
            breakable.sprite.HeightOffGround = -1;

            breakable.shardClusters = new ShardCluster[] { potShardCluster };
            var moneyPotvar = breakable.gameObject.AddComponent<MoneyPotBehavior>();
            moneyPotvar.objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.silverCoinPrefab;
            tk2dSpriteAnimator sprite = breakable.spriteAnimator;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 10);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            sprite.renderer.material = mat;
            StaticReferences.StoredRoomObjects.Add("Silver_Pot", breakable.gameObject);

            return breakable.gameObject;
        }

        public static GameObject GenerateCopperpot()
        {
            string shadowPath = "Planetside/Resources/DungeonObjects/MoneyPots/money_pot_shadow.png";
            string defaultPath = "Planetside/Resources/DungeonObjects/MoneyPots/Copper/";

            string[] shardPaths = new string[]
            {
                defaultPath+"Debris/copper_debris_1.png",
                defaultPath+"Debris/copper_debris_2.png",
                defaultPath+"Debris/copper_debris_3.png",
                defaultPath+"Debris/copper_debris_4.png",
                defaultPath+"Debris/copper_debris_5.png",
                defaultPath+"Debris/copper_debris_6.png",
                defaultPath+"Debris/copper_debris_7.png",

            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 3, 6, 720, 540, null, 0.6f, null, null, 0, false);
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Glitch_Pot", new string[]
            { defaultPath + "copper_pot_idle_001.png",
                        defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_001.png",
            defaultPath + "copper_pot_idle_002.png",
            defaultPath + "copper_pot_idle_003.png",
            defaultPath + "copper_pot_idle_004.png",
            defaultPath + "copper_pot_idle_005.png",
            defaultPath + "copper_pot_idle_006.png",
            defaultPath + "copper_pot_idle_007.png",
            }, 10,
            new string[] { defaultPath + "copper_pot_break_001.png" }, 10,
            "Play_OBJ_pot_shatter_01", true, 14, 18, 1, 0);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "moneyPot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));

            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 0;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.gameObject.layer = 22;
            breakable.sprite.HeightOffGround = -1;
            breakable.shardClusters = new ShardCluster[] { potShardCluster };
            var moneyPotvar = breakable.gameObject.AddComponent<MoneyPotBehavior>();
            moneyPotvar.objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.bronzeCoinPrefab;


            tk2dSpriteAnimator sprite = breakable.spriteAnimator;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 10);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            sprite.renderer.material = mat;

            StaticReferences.StoredRoomObjects.Add("Copper_Pot", breakable.gameObject);
            return breakable.gameObject;
        }



        public static GameObject GenerateGlitchpot()
        {
            string shadowPath = "Planetside/Resources/DungeonObjects/MoneyPots/money_pot_shadow.png";
            string defaultPath = "Planetside/Resources/DungeonObjects/MoneyPots/Copper/";

            string[] shardPaths = new string[]
            {
                defaultPath+"Debris/copper_debris_1.png",
                defaultPath+"Debris/copper_debris_2.png",
                defaultPath+"Debris/copper_debris_3.png",
                defaultPath+"Debris/copper_debris_4.png",
                defaultPath+"Debris/copper_debris_5.png",
                defaultPath+"Debris/copper_debris_6.png",
                defaultPath+"Debris/copper_debris_7.png",
            };
            DebrisObject[] shardObjects = BreakableAPIToolbox.GenerateDebrisObjects(shardPaths, true, 3, 6, 720, 540, null, 0.6f, null, null, 0, false);
            foreach (var debris in shardObjects)
            {
                //
                debris.sprite.usesOverrideMaterial = true;
                Material mate = debris.sprite.renderer.material;
                mate.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                mate.SetFloat("_GlitchInterval", 0.25f);
                mate.SetFloat("_DispProbability", 0.6f);
                mate.SetFloat("_DispIntensity", 0.3f);
                mate.SetFloat("_ColorProbability", 0.7f);
                mate.SetFloat("_ColorIntensity", 0.1f);
            }
            ShardCluster potShardCluster = BreakableAPIToolbox.GenerateShardCluster(shardObjects, 0.7f, 1.2f, 6, 9, 0.6f);
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Glitch_Pot", new string[]
            { defaultPath + "copper_pot_idle_001.png"
            }, 10,
            new string[] { defaultPath + "copper_pot_break_001.png" }, 10,
            "Play_OBJ_pot_shatter_01", true, 14, 18, 1, 0);
            BreakableAPIToolbox.GenerateShadow(shadowPath, "moneyPot_shadow", breakable.gameObject.transform, new Vector3(0, -0.125f));

            breakable.stopsBullets = true;
            breakable.OnlyPlayerProjectilesCanBreak = false;
            breakable.OnlyBreaksOnScreen = false;
            breakable.resistsExplosions = false;
            breakable.canSpawnFairy = false;
            breakable.chanceToRain = 0;
            breakable.dropCoins = false;
            breakable.goopsOnBreak = false;
            breakable.gameObject.layer = 22;
            breakable.sprite.HeightOffGround = -1;
            breakable.shardClusters = new ShardCluster[] { potShardCluster };



            var moneyPotvar = breakable.gameObject.AddComponent<GlitchPotBehavior>();
            moneyPotvar.objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.bronzeCoinPrefab;
            
            
            tk2dSpriteAnimator sprite = breakable.spriteAnimator;
            sprite.sprite.usesOverrideMaterial = true;
            Material material = sprite.sprite.renderer.material;
            material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
            material.SetFloat("_GlitchInterval", 0.05f);
            material.SetFloat("_DispProbability", 0.1f);
            material.SetFloat("_DispIntensity", 0.2f);
            material.SetFloat("_ColorProbability", 0.2f);
            material.SetFloat("_ColorIntensity", 0.3f);

            StaticReferences.StoredRoomObjects.Add("glitchPot", breakable.gameObject);
            return breakable.gameObject;
        }

        public class TrollyPickups : MonoBehaviour
        {
            public void Start()
            {

            }
            public void Update()
            {
                T += BraveTime.DeltaTime;

                if (this.gameObject && T > 3)
                {
                    foreach (PlayerController p in GameManager.Instance.AllPlayers)
                    {
                        if (Vector2.Distance(p.sprite.WorldCenter, this.transform.PositionVector2()) < 4)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);

                            T = 0;
                            Vector3 pos = this.gameObject.transform.position.GetAbsoluteRoom().GetRandomAvailableCell().Value.ToCenterVector3(1);
                            var obj1 = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, this.gameObject.transform.position, Quaternion.identity);
                            Destroy(obj1, 2);

                            var obj = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX, pos, Quaternion.identity);
                            Destroy(obj, 2);
                            this.gameObject.transform.position = pos;
                            this.gameObject.GetComponent<SpeculativeRigidbody>().Reinitialize();
                        }
                    }
                }
            }
            public float T = 2;
        }

        public class GlitchPotBehavior : MonoBehaviour
        {
            public GameObject objectToSpawn = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.goldCoinPrefab;

            private static IEnumerator SpawnCoins(RoomHandler room)
            {
                var cells = room.Cells;
                foreach (IntVector2 position in cells)
                {
                    Vector3 vector = Vector3.up;
                    vector *= 2f;
                    GameObject gameObject = SpawnManager.SpawnDebris(GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.bronzeCoinPrefab, position.ToCenterVector3(1), Quaternion.identity);
                    DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                    orAddComponent.shouldUseSRBMotion = true;
                    orAddComponent.angularVelocity = 0f;
                    orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                    orAddComponent.Trigger(vector.WithZ(4f), 0.05f, 1f);
                    orAddComponent.canRotate = false;
                    PickupMover component2 = gameObject.GetComponent<PickupMover>();
                    if (component2)
                    {
                        component2.enabled = false;
                    }
                    GameManager.Instance.Dungeon.StartCoroutine(HandleManualCoinSpawnLifespan(gameObject.GetComponent<CurrencyPickup>(), 5));
                    yield return null;
                }
                yield break;
            }

            private static IEnumerator HandleManualCoinSpawnLifespan(CurrencyPickup coins, float lifeTime)
            {
                float elapsed = 0f;
                while (elapsed < lifeTime * 0.75f)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                float flickerTimer = 0f;
                while (elapsed < lifeTime)
                {
                    elapsed += BraveTime.DeltaTime;
                    flickerTimer += BraveTime.DeltaTime;
                    if (coins != null && coins.renderer)
                    {
                        bool enabled = flickerTimer % 0.2f > 0.15f;
                        coins.renderer.enabled = enabled;
                    }
                    else if (coins == null)
                    {
                        yield break;
                    }
                    yield return null;
                }
                UnityEngine.Object.Destroy(coins.gameObject);
                yield break;
            }

            private static IEnumerator InvariantWait(float delay, PlayerController p)
            {
                float elapsed = 0f;
                while (elapsed < delay)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    p.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    yield return null;
                }
                yield break;
            }

            private IEnumerator HandleTrollDeath(PlayerController p)
            {
                p.healthHaver.IsVulnerable = false;

                bool wasPitFalling = p.IsFalling;
                Pixelator.Instance.DoFinalNonFadedLayer = true;
                if (p.CurrentGun)
                {
                    p.CurrentGun.CeaseAttack(false, null);
                }
                p.CurrentInputState = PlayerInputState.NoInput;
                GameManager.Instance.MainCameraController.SetManualControl(true, false);
                p.ToggleGunRenderers(false, "death");
                p.ToggleHandRenderers(false, "death");
                p.spriteAnimator.Play("death");

                PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ToggleAttachedRenderers", p, new object[] { true });
                Transform cameraTransform = GameManager.Instance.MainCameraController.transform;
                Vector3 cameraStartPosition = cameraTransform.position;
                Vector3 cameraEndPosition = p.CenterPosition;
                GameManager.Instance.MainCameraController.OverridePosition = cameraStartPosition;
                if (p.CurrentGun)
                {
                    p.CurrentGun.DespawnVFX();
                }
                yield return null;
                p.ToggleHandRenderers(false, "death");
                if (p.CurrentGun)
                {
                    p.CurrentGun.DespawnVFX();
                }
                p.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unfaded"));
                GameUIRoot.Instance.ForceClearReload(p.PlayerIDX);
                GameUIRoot.Instance.notificationController.ForceHide();
                float elapsed = 0f;
                float duration = 0.8f;
                tk2dBaseSprite spotlightSprite = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DeathShadow", ".prefab"), p.specRigidbody.UnitCenter, Quaternion.identity)).GetComponent<tk2dBaseSprite>();
                spotlightSprite.spriteAnimator.ignoreTimeScale = true;
                spotlightSprite.spriteAnimator.Play();
                tk2dSpriteAnimator whooshAnimator = spotlightSprite.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
                whooshAnimator.ignoreTimeScale = true;
                whooshAnimator.Play();
                Pixelator.Instance.CustomFade(0.6f, 0f, Color.white, Color.black, 0.1f, 0.5f);
                Pixelator.Instance.LerpToLetterbox(0.35f, 0.8f);
                BraveInput.AllowPausedRumble = true;
                p.DoVibration(Vibration.Time.Normal, Vibration.Strength.Hard);
                
                if (p.OverrideAnimationLibrary != null)
                {
                    p.OverrideAnimationLibrary = null;
                    PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ResetOverrideAnimationLibrary", p, new object[] { });
                    GameObject effect = (GameObject)BraveResources.Load("Global VFX/VFX_BulletArmor_Death", ".prefab");
                    p.PlayEffectOnActor(effect, Vector3.zero, true, false, false);
                }
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    float t = elapsed / duration;
                    GameManager.Instance.MainCameraController.OverridePosition = Vector3.Lerp(cameraStartPosition, cameraEndPosition, t);
                    p.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    spotlightSprite.color = new Color(1f, 1f, 1f, t);
                    Pixelator.Instance.saturation = Mathf.Clamp01(1f - t);
                    yield return null;
                }
                spotlightSprite.color = Color.white;
                yield return base.StartCoroutine(InvariantWait(0.4f, p));
                Transform clockhairTransform = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("Clockhair", ".prefab"))).transform;
                ClockhairController clockhair = clockhairTransform.GetComponent<ClockhairController>();
                elapsed = 0f;
                duration = clockhair.ClockhairInDuration;
                Vector3 clockhairTargetPosition = p.CenterPosition;
                Vector3 clockhairStartPosition = clockhairTargetPosition + new Vector3(-20f, 5f, 0f);
                clockhair.renderer.enabled = false;
                clockhair.spriteAnimator.Play("clockhair_intro");
                clockhair.hourAnimator.Play("hour_hand_intro");
                clockhair.minuteAnimator.Play("minute_hand_intro");
                clockhair.secondAnimator.Play("second_hand_intro");
                bool hasWobbled = false;
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    float t2 = elapsed / duration;
                    float smoothT = Mathf.SmoothStep(0f, 1f, t2);
                    Vector3 currentPosition = Vector3.Slerp(clockhairStartPosition, clockhairTargetPosition, smoothT);
                    clockhairTransform.position = currentPosition.WithZ(0f);
                    if (t2 > 0.5f)
                    {
                        clockhair.renderer.enabled = true;
                        clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                    if (t2 > 0.75f)
                    {
                        clockhair.hourAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.minuteAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.secondAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.hourAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                        clockhair.minuteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                        clockhair.secondAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                    if (!hasWobbled && clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1)
                    {
                        clockhair.spriteAnimator.Play("clockhair_wobble");
                        hasWobbled = true;
                    }
                    clockhair.sprite.UpdateZDepth();
                    p.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    yield return null;
                }
                if (!hasWobbled)
                {
                    clockhair.spriteAnimator.Play("clockhair_wobble");
                }
                clockhair.SpinToSessionStart(clockhair.ClockhairSpinDuration);
                elapsed = 0f;
                duration = clockhair.ClockhairSpinDuration + clockhair.ClockhairPauseBeforeShot;
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    yield return null;
                }
              
                BraveInput.AllowPausedRumble = false;
                UnityEngine.Object.Destroy(spotlightSprite.gameObject);
                UnityEngine.Object.Destroy(clockhair.gameObject);

                Pixelator.Instance.saturation = 1f;
                Pixelator.Instance.FadeToColor(0.25f, Pixelator.Instance.FadeColor, true, 0f);
                Pixelator.Instance.LerpToLetterbox(1f, 0.25f);
                Pixelator.Instance.DoFinalNonFadedLayer = false;

                p.CurrentInputState = PlayerInputState.AllInput;                
                p.IsVisible = true;
                p.ToggleGunRenderers(true, "death");
                p.ToggleHandRenderers(true, "death");
                
                PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ToggleAttachedRenderers", p, new object[] { true });

                p.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
                GameManager.Instance.DungeonMusicController.ResetForNewFloor(GameManager.Instance.Dungeon);
                if (p.CurrentRoom != null)
                {
                    GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(p.CurrentRoom);
                }
                GameManager.Instance.ForceUnpause();
                GameManager.Instance.PreventPausing = false;
                BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                if (wasPitFalling)
                {
                    PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "PitRespawn", p, new object[] { Vector2.zero });
                }
                p.healthHaver.IsVulnerable = true;
                p.healthHaver.TriggerInvulnerabilityPeriod(-1f);
                OtherTools.Notify("Just Kidding :)", "Just Kidding :)", "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns", UINotificationController.NotificationColor.PURPLE, true);


                yield break;
            }

            public IEnumerator BeetleAway(GameObject beetle)
            {
                float elapsed = 0f;
                while (elapsed < 3)
                {
                    if (beetle == null) { break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = Mathf.Min(elapsed / 2, 1);
                    float tRue = MathToolbox.SinLerpTValue(t);
                    beetle.transform.localScale = Vector3.one * tRue;
                    yield return null;
                }
                elapsed = 0f;
                while (elapsed < 3)
                {
                    if (beetle == null) { break; }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                elapsed = 0f;
                while (elapsed < 6)
                {
                    if (beetle == null) { break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = Mathf.Min(elapsed / 3, 1);
                    float tRue = MathToolbox.SinLerpTValue(t);
                    beetle.transform.position = beetle.transform.position + new Vector3(0, tRue / 4);
                    yield return null;
                }
                if (beetle)
                {
                    Destroy(beetle);
                }
                yield break;
            }
            public IEnumerator BeetleSpin(GameObject beetle)
            {
                int i = 0;
                while (beetle != null)
                {
                    i++;
                    beetle.transform.localRotation = Quaternion.Euler(-90, i, 90);
                    yield return null;
                }
                yield break;
            }

            bool p = false;

            public void Update()
            {
                if (p == false)
                {
                    if (GameManager.Instance.IsAnyPlayerInRoom(this.gameObject.transform.position.GetAbsoluteRoom()) == true)
                    {
                        p = !p;
                        AkSoundEngine.PostEvent("Play_Glitch", this.gameObject);
                    }
                }
            }



            public void Start()
            {
                var minorbreakable = this.GetComponent<MinorBreakable>();
                if (minorbreakable != null)
                {
                    minorbreakable.OnBreakContext += (self) =>
                    {
                        GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.DragunBoulderLandVFX, self.transform.position, Quaternion.identity);
                        tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                        component.PlaceAtPositionByAnchor(self.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                        component.HeightOffGround = 35f;
                        component.UpdateZDepth();
                        Destroy(component.gameObject, 2.5f);
                        component.usesOverrideMaterial = true;
                        var boomMat = component.renderer.material;
                        boomMat.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                        boomMat.SetFloat("_GlitchInterval", 0.05f);
                        boomMat.SetFloat("_DispProbability", 0.1f);
                        boomMat.SetFloat("_DispIntensity", 0.2f);
                        boomMat.SetFloat("_ColorProbability", 0.2f);
                        boomMat.SetFloat("_ColorIntensity", 0.3f);

                        AkSoundEngine.PostEvent("Stop_Glitch", this.gameObject);

                        AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);

                        IntVector2 intVector = minorbreakable.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                        RoomHandler roomFromPosition2 = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector);
                        int rng = UnityEngine.Random.Range(1, 11);
                        //Debug.Log("RNG Pot event chosen: " + rng);
                        switch (rng)
                        {
                            case 1:

                                PotFairyEngageDoer.InstantSpawn = true;
                                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(GameManager.Instance.Dungeon.sharedSettingsPrefab.PotFairyGuid);
                                var fairy = AIActor.Spawn(orLoadByGuid, intVector, roomFromPosition2, true, AIActor.AwakenAnimationType.Default, true);
                                fairy.BecomeBlackPhantom();
                                fairy.MovementSpeed *= 0.6f;
                                fairy.healthHaver.SetHealthMaximum(65, 65, true);
                                ExplodeOnDeath explodeVar = fairy.gameObject.AddComponent<ExplodeOnDeath>();
                                explodeVar.deathType = OnDeathBehavior.DeathType.PreDeath;
                                explodeVar.LinearChainExplosion = true;
                                explodeVar.ChainNumExplosions = 10;
                                
                                explodeVar.explosionData = StaticExplosionDatas.customDynamiteExplosion;
                                explodeVar.LinearChainExplosionData = StaticExplosionDatas.customDynamiteExplosion;
                                var fairyMat = fairy.sprite.renderer.material;
                                fairyMat.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                                fairyMat.SetFloat("_GlitchInterval", 0.2f);
                                fairyMat.SetFloat("_DispProbability", 0.5f);
                                fairyMat.SetFloat("_DispIntensity", 0.1f);
                                fairyMat.SetFloat("_ColorProbability", 0.5f);
                                fairyMat.SetFloat("_ColorIntensity", 0.2f);

                                break;
                            case 2:
                                GameManager.Instance.Dungeon.StartCoroutine(SpawnCoins(roomFromPosition2));
                                break;
                            case 3:
                                PickupObject byId = PickupObjectDatabase.GetById(276);
                                PlayerItem playerItem = byId as PlayerItem;
                                playerItem.Use(GameManager.Instance.PrimaryPlayer, out float ImTooTiredToNameVariables);
                                break;
                            case 4:
                                GameManager.Instance.PauseRaw(true);
                                BraveTime.RegisterTimeScaleMultiplier(0f, GameManager.Instance.gameObject);
                                AkSoundEngine.PostEvent("Stop_SND_All", base.gameObject);
                                base.StartCoroutine(HandleTrollDeath(GameManager.Instance.PrimaryPlayer));
                                AkSoundEngine.PostEvent("Play_UI_gameover_start_01", base.gameObject);
                                break;
                            case 5:

                                GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("videoplayer"));
                                MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
                                rend.allowOcclusionWhenDynamic = true;
                                partObj.transform.position = minorbreakable.transform.position;
                                partObj.transform.localScale = Vector3.one;
                                partObj.name = "Ultrakill";
                                VideoPlayer v = partObj.GetComponent<VideoPlayer>();
                                v.controlledAudioTrackCount = 1;

                                v.audioOutputMode = VideoAudioOutputMode.AudioSource;
                                v.EnableAudioTrack(1, true);
                                v.source = VideoSource.VideoClip;

                                v.Play();

                                partObj.transform.localScale *= 8;
                                partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

                                var yyy = partObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
                                break;
                            case 6:
                                GameObject beetle = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Bteele"));
                                MeshRenderer renderer = beetle.GetComponentInChildren<MeshRenderer>();
                                renderer.allowOcclusionWhenDynamic = true;
                                beetle.transform.position = minorbreakable.transform.position;
                                beetle.transform.localScale = Vector3.one;
                                beetle.name = "ShopPortal";
                                beetle.transform.localScale *= 2;
                                beetle.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

                                beetle.transform.localRotation = Quaternion.Euler(0, 180, 90);

                                GameManager.Instance.StartCoroutine(BeetleAway(beetle));
                                GameManager.Instance.StartCoroutine(BeetleSpin(beetle));
                                break;
                            case 7:

                                Chest chest2 = GameManager.Instance.RewardManager.SpawnRewardChestAt(new IntVector2((int)minorbreakable.transform.position.x, (int)minorbreakable.transform.position.y), -1f, PickupObject.ItemQuality.EXCLUDED);
                                chest2.sprite.renderer.enabled = false;
                                chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
                                chest2.sprite.usesOverrideMaterial = true;
                                AkSoundEngine.PostEvent("Play_OBJ_redchest_spawn_01", chest2.gameObject);
                                chest2.IsLocked = false;
                                chest2.spawnAnimName = "redgold_chest_appear";
                                chest2.openAnimName = "redgold_chest_open";
                                chest2.breakAnimName = "redgold_chest_break";
                                chest2.majorBreakable.spriteNameToUseAtZeroHP = "chest_redgold_break_001";
                                chest2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
                                chest2.MaybeBecomeMimic();
                                chest2.sprite.renderer.enabled = true;

                                break;
                            case 8:
                                OtherTools.Notify("Time Shifts A Little...", "", "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns", UINotificationController.NotificationColor.PURPLE, true);
                                TimeTraderSpawnController.ShopAllowedToSpawn = true;
                                break;
                            case 9:


                                for (int i = 0; i < 6; i++)
                                {
                                    int id = BraveUtility.RandomElement<int>(Shellrax.Lootdrops);
                                    var pickup = LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, minorbreakable.sprite.WorldCenter, MathToolbox.GetUnitOnCircle(60 * i, 2), 2.2f, false, true, false);
                                    pickup.gameObject.AddComponent<TrollyPickups>();
                                }

                                break;
                            case 10:

                                string[] array = new string[]
                                {
                                    "Global VFX/Confetti_Blue_001",
                                    "Global VFX/Confetti_Yellow_001",
                                    "Global VFX/Confetti_Green_001"
                                };
                                AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", minorbreakable.gameObject);
                                for (int i = 0; i < 16; i++)
                                {
                                    GameObject original = (GameObject)BraveResources.Load(array[UnityEngine.Random.Range(0, 3)], ".prefab");
                                    WaftingDebrisObject a = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<WaftingDebrisObject>();
                                    a.sprite.PlaceAtPositionByAnchor(minorbreakable.transform.position + new Vector3(0.25f, 0.25f, 2f), tk2dBaseSprite.Anchor.MiddleCenter);
                                    Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                                    insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                                    a.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
                                }
                                LootEngine.DoDefaultItemPoof(minorbreakable.transform.PositionVector2() + new Vector2(0.5f, 0.5f));
                                GameObject bom = new GameObject();
                                StaticReferences.StoredRoomObjects.TryGetValue("trollNote", out bom);
                                var note = DungeonPlaceableUtility.InstantiateDungeonPlaceable(bom, roomFromPosition2, new IntVector2((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y) - roomFromPosition2.area.basePosition, false).GetComponent<NoteDoer>();
                                note.GetComponent<NoteDoer>().stringKey = "#TROLL_NOTE_" + (UnityEngine.Random.Range(1, 13).ToString());
                                roomFromPosition2.RegisterInteractable(note);

                                break;
                        }

                        /*
                        Vector3 vector = Vector3.up;
                        vector *= 2f;
                        GameObject gameObject = SpawnManager.SpawnDebris(objectToSpawn, self.sprite.WorldCenter.ToVector3ZUp(self.transform.position.z), Quaternion.identity);
                        DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                        orAddComponent.shouldUseSRBMotion = true;
                        orAddComponent.angularVelocity = 0f;
                        orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                        orAddComponent.Trigger(vector.WithZ(4f), 0.05f, 1f);
                        orAddComponent.canRotate = false;
                        */
                    };
                }
            }
        }
    }
}
