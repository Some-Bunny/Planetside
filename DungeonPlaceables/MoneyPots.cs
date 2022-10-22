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
            "Play_OBJ_pot_shatter_01", shadowPath, 0, -0.125f, true, 14, 18, 1, 0);
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
            "Play_OBJ_pot_shatter_01", shadowPath, 0, -0.125f, true, 14, 18, 1, 0);
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
            "Play_OBJ_pot_shatter_01", shadowPath, 0, -0.125f, true, 14, 18, 1, 0);
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
            MinorBreakable breakable = BreakableAPIToolbox.GenerateMinorBreakable("Silver_Pot", new string[]
            { defaultPath + "copper_pot_idle_001.png"
            }, 10,
            new string[] { defaultPath + "copper_pot_break_001.png" }, 10,
            "Play_OBJ_pot_shatter_01", shadowPath, 0, -0.125f, true, 14, 18, 1, 0);
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
            material.SetFloat("_GlitchInterval", 0.2f);
            material.SetFloat("_DispProbability", 0.5f);
            material.SetFloat("_DispIntensity", 0.1f);
            material.SetFloat("_ColorProbability", 0.5f);
            material.SetFloat("_ColorIntensity", 0.2f);

            StaticReferences.StoredRoomObjects.Add("test", breakable.gameObject);
            return breakable.gameObject;
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


            public void Start()
            {
                AkSoundEngine.PostEvent("Play_OBJ_chestglitch_loop_01", this.gameObject);
                var minorbreakable = this.GetComponent<MinorBreakable>();
                if (minorbreakable != null)
                {

                    minorbreakable.OnBreakContext += (self) =>
                    {
                        IntVector2 intVector = minorbreakable.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                        RoomHandler roomFromPosition2 = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector);
                        int rng = UnityEngine.Random.Range(7, 8);
                        switch (rng)
                        {
                            case 1:

                                PotFairyEngageDoer.InstantSpawn = true;
                                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(GameManager.Instance.Dungeon.sharedSettingsPrefab.PotFairyGuid);
                                var fairy = AIActor.Spawn(orLoadByGuid, intVector, roomFromPosition2, true, AIActor.AwakenAnimationType.Default, true);
                                fairy.BecomeBlackPhantom();
                                fairy.MovementSpeed *= 0.6f;
                                fairy.healthHaver.SetHealthMaximum(45, 45, true);
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
                                PickupObject byId = PickupObjectDatabase.GetById(328);
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
                                chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
                                chest2.sprite.usesOverrideMaterial = true;

                                chest2.IsLocked = false;
                                chest2.spawnAnimName = "redgold_chest_appear";
                                chest2.openAnimName = "redgold_chest_open";
                                chest2.breakAnimName = "redgold_chest_break";
                                chest2.majorBreakable.spriteNameToUseAtZeroHP = "chest_redgold_break_001";
                                chest2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
                                chest2.MaybeBecomeMimic();
                                
                                break;
                            case 8:
                                break;
                            case 9:
                                break;
                            case 10:
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
