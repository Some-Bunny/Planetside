using Alexandria.BreakableAPI;
using Alexandria.PrefabAPI;
using GungeonAPI;
using Planetside.Static_Storage;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;
using BreakAbleAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using static AIActor;
using static Planetside.StaticVFXStorage;
using ChallengeAPI;
using Alexandria.ItemAPI;
using Alexandria;
using Planetside.Toolboxes;
using ItemAPI;
using static Dungeonator.CellVisualData;
namespace Planetside.DungeonPlaceables
{
    public class Idol : MonoBehaviour
    {

        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BlessedIdol");

            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("blessedidol_001"));

            tk2d.SortingOrder = 3;
            tk2d.HeightOffGround = 3;
            tk2d.sprite.usesOverrideMaterial = true;

            var body = obj.CreateFastBody(new IntVector2(16, 33), new IntVector2(23, -4), CollisionLayer.PlayerBlocker);
            obj.CreateFastBody(new IntVector2(28, 16), new IntVector2(2, 0), CollisionLayer.EnemyBlocker);
            obj.CreateFastBody(new IntVector2(28, 16), new IntVector2(2, 0), CollisionLayer.BeamBlocker);
            obj.CreateFastBody(new IntVector2(28, 16), new IntVector2(2, 0), CollisionLayer.BulletBlocker);
            obj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));

            var brekable = obj.AddComponent<MajorBreakable>();
            brekable.HitPoints = 65;
            brekable.destroyedOnBreak = true;
            brekable.ImmuneToBeastMode = true;
            brekable.ScaleWithEnemyHealth = true;

            BreakableAPI_Bundled.GenerateShadow("blessedidol_shadow", "shadow_idol", StaticSpriteDefinitions.RoomObject_Sheet_Data, obj.transform, new Vector3(0.4375f, 0.1875f, 0));

            Material mat_ = new Material(ShaderCache.Acquire("Brave/LitCutoutUber_ColorEmissive"));
            mat_.SetTexture("_MainTex", tk2d.sprite.renderer.material.mainTexture);
            tk2d.renderer.material = mat_;

            brekable.handlesOwnPrebreakFrames = false;
            brekable.prebreakFrames = new BreakFrame[] 
            {
                new BreakFrame(){ healthPercentage = 80, sprite = "blessedidol_002"},
                new BreakFrame(){ healthPercentage = 55, sprite = "blessedidol_003"},
                new BreakFrame(){ healthPercentage = 25, sprite = "blessedidol_004"},
            };

            brekable.damageVfx = (PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            brekable.damageVfxMinTimeBetween = 0.1f;

            brekable.PlayerRollingBreaks = true;
            brekable.spawnShards = true;
            brekable.shardBreakStyle = MinorBreakable.BreakStyle.BURST;
            brekable.distributeShards = true;
            brekable.shardClusters = new ShardCluster[]
            {
                new ShardCluster()
                {
                    forceMultiplier = 0.05f,
                    maxFromCluster = 6,
                    minFromCluster = 4,
                    clusterObjects = new DebrisObject[]
                    {
                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_001", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 5f),
                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_003", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 5f),
                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_004", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 5f),

                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_006", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 2f),
                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_007", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 2f),
                        BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_013", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 240, 60, null, 2f),

                    }
                },
                new ShardCluster()
                {
                    forceMultiplier = 0.1f,
                    maxFromCluster = 1,
                    minFromCluster = 1,
                    clusterObjects = new DebrisObject[]
                    {
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_008", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 120, 30, null, 7f),
                    }
                },
                new ShardCluster()
                {
                    forceMultiplier = 0.1f,
                    maxFromCluster = 1,
                    minFromCluster = 1,
                    clusterObjects = new DebrisObject[]
                    {
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_011", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 120, 30, null, 7f),
                    }
                },
                new ShardCluster()
                {
                    forceMultiplier = 0.1f,
                    maxFromCluster = 1,
                    minFromCluster = 1,
                    clusterObjects = new DebrisObject[]
                    {
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_010", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 120, 30, null, 10f),
                    }
                },
                new ShardCluster()
                {
                    forceMultiplier = 0.1f,
                    maxFromCluster = 5,
                    minFromCluster = 3,
                    clusterObjects = new DebrisObject[]
                    {
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_002", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 180, 30, null, 4.7f, null, null, 1, true, Alexandria.Misc.GoopUtility.BloodDef),
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_005", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 180, 30, null, 4.5f, null, null, 1, true, Alexandria.Misc.GoopUtility.BloodDef),
                         BreakableAPI_Bundled.GenerateDebrisObject("idol_debris_012", StaticSpriteDefinitions.RoomObject_Sheet_Data, true, 0.5f, 1f, 60, 30, null, 5f, null, null, 1, true, Alexandria.Misc.GoopUtility.BloodDef),
                    }
                },
            };

            GameObject obj_line = PrefabBuilder.BuildObject("BlessedIdol_Line");
            obj_line.transform.SetParent(obj.transform);
            obj_line.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            var lineRenderer = obj_line.AddComponent<LineRenderer>();
            lineRenderer.enabled = true;
            lineRenderer.sortingOrder = -10;
            lineRenderer.material = new Material(StaticShaders.TransparencyShader);
            lineRenderer.material.SetTexture("_MainTex", StaticTextures.Gradient_Circle);
            lineRenderer.material.SetFloat("_Fade", 0.95f);

            lineRenderer.startWidth = 1.125f;
            lineRenderer.endWidth = 1.125f;

            /*
            GameObject ring = PrefabBuilder.BuildObject("BlessedIdolRing");
            var tk2d_ring = ring.AddComponent<tk2dSprite>();
            
            tk2d_ring.SetSprite(StaticSpriteDefinitions.DoFastSetup("RingCollection", "rhingthing material.mat"), "blessed_pot_circle_001");
            tk2d_ring.usesOverrideMaterial = true;
            tk2d_ring.HeightOffGround = -5;
            tk2d_ring.SortingOrder = 0;
            tk2d_ring.renderLayer = 0;
            tk2d_ring.transform.localScale *= 0.125f;
            tk2d_ring.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            ring.transform.SetParent(obj.transform, true);
            ring.transform.localPosition += new Vector3(1.25f, - 0.45f, 5.25f);
            Material mat = new Material(StaticShaders.CursePotCircleShader);
            tk2d_ring.renderer.material.SetTexture("_MainTex", StaticTextures.Curse_Ring);
            tk2d_ring.renderer.material.SetTexture("_SubTex", StaticTextures.Curse_Ring);
            tk2d_ring.renderer.material.SetColor("_OverrideColor", Color.white);
            tk2d_ring.renderer.material.SetFloat("_Perpendicular", 1);
            tk2d_ring.renderer.material = mat;
            */

            var goopie = obj.AddComponent<Idol>();
            goopie.MarkAsFakePrefab();

            goopie.sprite = tk2d;
            //goopie.RingSprite = tk2d_ring;
            goopie.majorBreakable = brekable;
            goopie.lineRenderer = lineRenderer;

            Alexandria.ItemAPI.SpriteBuilder.AddSpriteToCollection(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteDefinition("blessedidol_001"), Alexandria.ItemAPI.SpriteBuilder.ammonomiconCollection);

            var encounterTrackable = goopie.gameObject.AddComponent<EncounterTrackable>();
            encounterTrackable.journalData = new JournalEntry();
            encounterTrackable.EncounterGuid = "psog:silver_idol";
            encounterTrackable.prerequisites = new DungeonPrerequisite[0];
            encounterTrackable.journalData.SuppressKnownState = false;
            encounterTrackable.journalData.IsEnemy = true;
            encounterTrackable.journalData.SuppressInAmmonomicon = false;
            encounterTrackable.ProxyEncounterGuid = "";
            encounterTrackable.journalData.AmmonomiconSprite = "blessedidol_001";
            encounterTrackable.journalData.enemyPortraitSprite = PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("silveridolsheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\shamberammonomicoenrtytab.png");
            PlanetsideModule.Strings.Enemies.Set("#SILVER_IDOL_NAME", "Silver Idol");
            PlanetsideModule.Strings.Enemies.Set("#SILVER_IDOL_SHORT", "Heavenly Protection");
            PlanetsideModule.Strings.Enemies.Set("#SILVER_IDOL_LONGDESC", "When a specially valued Gundead nears death, the others create an Idol to link their spirit to, so they can continue to serve the Gungeon even in death.\n\nWhile somewhat resistant to gunfire, they are not resistant to a good, strong push onto the ground.");
            encounterTrackable.journalData.PrimaryDisplayName = "#SILVER_IDOL_NAME";
            encounterTrackable.journalData.NotificationPanelDescription = "#SILVER_IDOL_SHORT";
            encounterTrackable.journalData.AmmonomiconFullEntry = "#SILVER_IDOL_LONGDESC";
            //EnemyBuilder.AddEnemyToDatabase(enemy.gameObject, "psog:shamber");
            EnemyDatabaseEntry item = new EnemyDatabaseEntry
            {
                myGuid = "psog:silver_idol",
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = true,
                path = "psog:silver_idol",
                isInBossTab = false,
                encounterGuid = "psog:silver_idol"
            };
            EnemyDatabase.Instance.Entries.Add(item);
            EncounterDatabaseEntry encounterDatabaseEntry = new EncounterDatabaseEntry(encounterTrackable)
            {
                path = "psog:silver_idol",
                myGuid = "psog:silver_idol"
            };
            EncounterDatabase.Instance.Entries.Add(encounterDatabaseEntry);

            EnemyDatabase.GetEntry("psog:silver_idol").ForcedPositionInAmmonomicon = 100;
            EnemyDatabase.GetEntry("psog:silver_idol").isInBossTab = false;
            EnemyDatabase.GetEntry("psog:silver_idol").isNormalEnemy = true;
            encounterTrackable.DoNotificationOnEncounter = false;
            goopie.trackable = encounterTrackable;


            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_Idol", obj);
            StaticReferences.StoredRoomObjects.Add("PSOG_Idol", obj.gameObject);
            Alexandria.DungeonAPI.RoomFactory.OnCustomProperty += OnAction;
        }
        public static GameObject OnAction(string ObjName, GameObject Original, JObject jObject)
        {
            if (ObjName != "PSOG_Idol") { return Original; }

            var clone = UnityEngine.Object.Instantiate(Alexandria.DungeonAPI.StaticReferences.customObjects["PSOG_Idol"]);
            ItemAPI.FakePrefab.MarkAsFakePrefab(clone);
            DontDestroyOnLoad(clone);

            var tearHolder = clone.GetComponent<Idol>();
            JToken value = null;

            string GUID = jObject.TryGetValue("enemyGUID", out value) ? ((string)value) : Alexandria.EnemyGUIDs.Chaingunner_GUID;

            tearHolder.EnemyGUIDToSpawn = GUID;

            float x = jObject.TryGetValue("offsetx", out value) ? ((float)value) : 0;
            float y = jObject.TryGetValue("offsety", out value) ? ((float)value) : -2;

            float speed = jObject.TryGetValue("SpeedMultEnemy", out value) ? ((float)value) : 0.75f;
            float cooldown = jObject.TryGetValue("CooldownMultEnemy", out value) ? ((float)value) : 1f;

            tearHolder.EnemySpawnOffset = new Vector2(x, y);
            tearHolder.EnemyMovementSpeedMult = speed;
            tearHolder.EnemyAttackSpeedMult = cooldown;
            return clone;
            
        }


        public MajorBreakable majorBreakable;
        public tk2dSprite sprite;
        public tk2dSpriteAnimator animator;
        public EncounterTrackable trackable;
        public void Start()
        {
            MarkCells();
            if (this.RingSprite)
            {
                RingSprite.renderer.material.SetTexture("_MainTex", StaticTextures.Curse_Ring);
                RingSprite.renderer.material.SetTexture("_SubTex", StaticTextures.Curse_Ring);
                RingSprite.renderer.material.SetColor("_OverrideColor", Color.white.WithAlpha(0.63f));
                RingSprite.renderer.material.SetFloat("_Perpendicular", 1);

                Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
                vector = Vector2.Min(vector, StaticTextures.Curse_Ring.texelSize);
                vector2 = Vector2.Max(vector2, StaticTextures.Curse_Ring.texelSize);
                Vector2 vector3 = (vector + vector2) / 2;

                RingSprite.renderer.material.SetVector("_WorldCenter", new Vector4(vector3.x, vector3.y, vector3.x - vector.x, vector3.y - vector.y));
            }
            majorBreakable.InvulnerableToEnemyBullets = true;
            majorBreakable.EnemyDamageOverride = 0;


            room = this.transform.position.GetAbsoluteRoom();
            if (room != null)
            {
                room.OnEnemiesCleared += () =>
                {
                    ExecuteTheGuy();
                };
            }
            majorBreakable.OnBreak += () =>
            {
                room.Entered -= Room_Entered;
                trackable?.HandleEncounter();
                ExecuteTheGuy();
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.BloodDef).TimedAddGoopCircle(this.sprite.WorldBottomCenter, 3.5f, 0.5f);
                AkSoundEngine.PostEvent("Play_RockBreaking", base.gameObject);
                GameObject teleportVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
                teleportVFX.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(this.transform.position + new Vector3(2f, -0.5f), tk2dBaseSprite.Anchor.LowerCenter);
                teleportVFX.transform.position = teleportVFX.transform.position.Quantize(0.0625f);
                teleportVFX.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                Destroy(teleportVFX, 2);
            };
            majorBreakable.OnDamaged += (obj1) =>
            {
                AkSoundEngine.PostEvent("Play_OBJ_pot_shatter_01", base.gameObject);
                for (int i = majorBreakable.prebreakFrames.Length - 1; i >= 0; i--)
                {
                    if (majorBreakable.GetCurrentHealthPercentage() <= majorBreakable.prebreakFrames[i].healthPercentage / 100f)
                    {
                        sprite.SetSprite(majorBreakable.prebreakFrames[i].sprite);
                        var minPosition = sprite.WorldBottomLeft;
                        var maxPosition = sprite.WorldTopRight;
                        for (int e = 0; e < 8; e++)
                        {
                            Vector3 position = new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), UnityEngine.Random.Range(minPosition.y, maxPosition.y), 2);
                            StaticVFXStorage.BloodSplatterParticleSystem.Emit(new ParticleSystem.EmitParams()
                            {
                                position = position,
                                applyShapeToPosition = true,
                                rotation = BraveUtility.RandomAngle(),
                                startLifetime = 1,
                                velocity = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 0.4f)
                            }, 8);
                        }
                        for (int e = 0; e < 4; e++)
                        {
                            Vector3 position = new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), UnityEngine.Random.Range(minPosition.y, maxPosition.y), 2);
                            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), position, Quaternion.identity);
                            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                            component.HeightOffGround = 15f;
                            component.UpdateZDepth();
                            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                            if (component2 != null)
                            {
                                component2.alwaysUpdateOffscreen = true;
                                component2.playAutomatically = true;
                                component2.sprite.usesOverrideMaterial = true;
                                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 1);
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
                                component2.sprite.renderer.material.SetColor("_OverrideColor", Color.gray);
                                component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.gray);
                            }
                        }
                        return;
                    }
                }
            };

            room.Entered += Room_Entered;
            E = BraveUtility.RandomAngle();
        }

        public void MarkCells()
        {
            PixelCollider primaryPixelCollider = this.GetComponent<SpeculativeRigidbody>().PrimaryPixelCollider;
            IntVector2 intVector = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    if (GameManager.Instance.Dungeon.data.cellData[i][j] != null)
                    {
                        var cell = GameManager.Instance.Dungeon.data.cellData[i][j];
                        cell.containsTrap = true;
                        cell.isOccupied = true;
                    }
                }
            }
        }
        private bool hasAlreadySpawned = false;
        private void Room_Entered(PlayerController p)
        {
            this.Invoke("SpawnTheGuy", UnityEngine.Random.Range(0.4f, 0.7f));
        }

        private bool isExecuting = false;

        public void ExecuteTheGuy()
        {
            if (isExecuting == true) { return; }
            if (EnemyBlessed == null) { return; }
            isExecuting = true;
            EnemyBlessed.behaviorSpeculator?.Stun(1000, false);
            EnemyBlessed.MovementSpeed *= 0;

            var mourn = StaticVFXStorage.MourningStarVFXController.SpawnMourningStar(EnemyBlessed.sprite.WorldCenter, -1);
            mourn.DoesSound = true;
            mourn.DoesEmbers = false;
            //"Brave/LitCutoutUber_ColorEmissive"
            var sh = ShaderCache.Acquire("Brave/LitCutoutUber");

            mourn.BurstSprite.usesOverrideMaterial = true;
            var mat = new Material(sh);
            mourn.BurstSprite.SortingOrder = -2;
            mat.EnableKeyword("TINTING_ON");
            mat.mainTexture = mourn.BurstSprite.renderer.material.mainTexture;
            mat.SetColor("_OverrideColor", new Color(5, 5, 5, 1));
            mat.SetTexture("_MainTexture", mourn.BurstSprite.renderer.material.mainTexture);
            mat.SetFloat("_EmissivePower", 5);

            mourn.BurstSprite.renderer.material = mat;

            mourn.sprite.SortingOrder = -2;
            mourn.sprite.usesOverrideMaterial = true;
            var mat_2 = new Material(sh);
            mat_2.mainTexture = mourn.sprite.renderer.material.mainTexture;
            mat_2.EnableKeyword("TINTING_ON");
            mat_2.SetColor("_OverrideColor", new Color(5, 5, 5, 1));
            mat_2.SetTexture("_MainTexture", mourn.sprite.renderer.material.mainTexture);
            mat_2.SetFloat("_EmissivePower", 5);

            mourn.sprite.renderer.material = mat_2;

            //Brave/LitCutoutUber_ColorEmissive (Instance)
            for (int i = 0; i < mourn.BeamSections.Count; i++)
            {
                tk2dSpriteAnimator spriteAnimator = mourn.BeamSections[i].spriteAnimator;
                if (spriteAnimator)
                {
                    spriteAnimator.sprite.SortingOrder = -2;
                    spriteAnimator.sprite.usesOverrideMaterial = true;
                    var mat_3 = new Material(sh);
                    mat_3.mainTexture = spriteAnimator.sprite.renderer.material.mainTexture;
                    mat_3.EnableKeyword("TINTING_ON");
                    mat_3.SetColor("_OverrideColor", new Color(5, 5, 5, 1));
                    mat_3.SetTexture("_MainTexture", spriteAnimator.sprite.renderer.material.mainTexture);
                    mat_3.SetFloat("_EmissivePower", 5);

                    spriteAnimator.sprite.renderer.material = mat_3;
                }
            }
            EnemyBlessed.DeregisterOverrideColor("tint");

            EnemyBlessed.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
            EnemyBlessed.sprite.renderer.material.SetTexture("_EeveeTex", StaticTextures.NebulaTexture);
            EnemyBlessed.sprite.renderer.material.SetFloat("_StencilVal", 0);
            EnemyBlessed.sprite.renderer.material.SetFloat("_FlatColor", 0f);
            EnemyBlessed.sprite.renderer.material.SetFloat("_Perpendicular", 0);
            GameManager.Instance.StartCoroutine(DoKill(mourn));
        }
        private RoomHandler room;
        private IEnumerator DoKill(StaticVFXStorage.MourningStarVFXController mourningStarVFXController)
        {
            float e = 0;
            var enemy = EnemyBlessed;

            if (enemy)
            {
                var vec = enemy.sprite.GetBounds().size;
                var vec1 = enemy.transform.position;
                var vec2 = enemy.sprite.WorldBottomCenter;
                yield return new WaitForSeconds(0.125f);
                while (e < 1)
                {
                    if (enemy != null) 
                    {
                        enemy.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0f, 3f), e);
                        enemy.sprite.SortingOrder = -1000;
                        enemy.sprite.HeightOffGround = Mathf.Lerp(2, 20, e);
                        enemy.transform.position = Vector3.Lerp(vec1, vec2, e);
                        enemy.specRigidbody.Reinitialize();
                    }


                    e += Time.deltaTime * 2f;
                    yield return null;
                }
                if (enemy != null)
                {
                    enemy.EraseFromExistenceWithRewards();
                }
            }
            mourningStarVFXController.Dissipate();
            yield break;
        }

        private void SpawnTheGuy()
        {
            if (hasAlreadySpawned == true) { return; }
            if (this == null) { return; }
            hasAlreadySpawned = true;
            EnemyBlessed = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDToSpawn), new IntVector2(Mathf.RoundToInt(this.transform.position.x + EnemySpawnOffset.x), Mathf.RoundToInt(this.transform.position.y + EnemySpawnOffset.y)), this.transform.position.GetAbsoluteRoom(), false, AIActor.AwakenAnimationType.Default, false);
            EnemyBlessed.reinforceType = ReinforceType.Instant;
            EnemyBlessed.HandleReinforcementFallIntoRoom(0.01f);

 

            AkSoundEngine.PostEvent("Play_OBJ_teleport_arrive_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_wizard_summon_01", base.gameObject);


            //var ring = UnityEngine.Object.Instantiate(this.RingSprite.gameObject, EnemyBlessed.transform.position, Quaternion.identity);
            //ring.transform.SetParent(EnemyBlessed.transform, false);
            //ring.transform.localScale = Vector2.one * 0.125f;
            //ring.transform.localPosition = new Vector3(0, -0.5f);


            //var t = ring.GetComponent<tk2dBaseSprite>();
            /*
            var m = t.renderer.material;
            m.SetTexture("_MainTex", StaticTextures.Curse_Ring);
            m.SetTexture("_SubTex", StaticTextures.Curse_Ring);
            m.SetColor("_OverrideColor", Color.white.WithAlpha(0.4f));
            //m.SetFloat("_Perpendicular", 1);

            t.HeightOffGround = -5;
            t.SortingOrder = 0;
            */
            if (EnemyBlessed.behaviorSpeculator)
            {
                EnemyBlessed.behaviorSpeculator.CooldownScale *= EnemyAttackSpeedMult;
            }
            if (EnemyBlessed.EffectResistances == null)
            {
                EnemyBlessed.EffectResistances = new ActorEffectResistance[0];
            }
            List<ActorEffectResistance> l = EnemyBlessed.EffectResistances.ToList();
            l.Add(new ActorEffectResistance() { resistAmount = 100, resistType = EffectResistanceType.None });
            l.Add(new ActorEffectResistance() { resistAmount = 100, resistType = EffectResistanceType.Poison });
            l.Add(new ActorEffectResistance() { resistAmount = 100, resistType = EffectResistanceType.Fire });
            l.Add(new ActorEffectResistance() { resistAmount = 100, resistType = EffectResistanceType.Freeze });
            l.Add(new ActorEffectResistance() { resistAmount = 100, resistType = EffectResistanceType.Charm });
            EnemyBlessed.EffectResistances = l.ToArray();

           
            EnemyBlessed.IgnoreForRoomClear = true;
            EnemyBlessed.CollisionDamage = 0;
            EnemyBlessed.healthHaver.IsVulnerable = false;
            //EnemyBlessed.healthHaver.AllDamageMultiplier = 0;
            EnemyBlessed.gameObject.AddComponent<TeleportationImmunity>();
            //EnemyBlessed.knockbackDoer?.SetImmobile(true);

            EnemyBlessed.SetIsFlying(true, "spookyghost", true, true);

            EnemyBlessed.specRigidbody.CollideWithOthers = false;

            EnemyBlessed.DeregisterOverrideColor("tint");

            EnemyBlessed.healthHaver.ModifyDamage += MD;
            Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(EnemyBlessed.sprite);
            if (EnemyBlessed.healthHaver != null && outlineMaterial1)
            {
                outlineMaterial1.SetColor("_OverrideColor", Color.white);
            }

            EnemyBlessed.RegisterOverrideColor(new Color(2, 2, 2), "tint");


            GameObject teleportVFX = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.TeleportVFX);
            teleportVFX.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(EnemyBlessed.sprite.WorldBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
            teleportVFX.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            Destroy(teleportVFX, 2);
            this.Invoke("DoRecolor", 0.1f);
            this.Invoke("DoRecolor", 0.2f);
            this.Invoke("DoRecolor", 0.3f);
            this.Invoke("DoRecolor", 0.4f);
            this.Invoke("DoRecolor", 0.5f);

        }

        public void MD(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs modifyDamageEventArgs)
        {
            modifyDamageEventArgs.InitialDamage = 0;
            modifyDamageEventArgs.ModifiedDamage = 0;
            if (isExecuting) { return; }
            if (effectCooldown > 0) { return; }
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", healthHaver.gameObject);
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, healthHaver.sprite.WorldCenter, Quaternion.identity);
            blankObj.transform.localScale /= 2.5f;
            Destroy(blankObj, 2f);
            effectCooldown = 0.4f;
        }
        private float effectCooldown;

        private AIActor EnemyBlessed; 

        public Vector2 EnemySpawnOffset;
        [SerializeField]
        public string EnemyGUIDToSpawn;
        public float EnemyMovementSpeedMult = 0.5f;
        public float EnemyAttackSpeedMult = 0.75f;
        public tk2dBaseSprite RingSprite;
        public LineRenderer lineRenderer;

        private float E = 0;

        private void DoRecolor()
        {
            EnemyBlessed.RegisterOverrideColor(new Color(2, 2, 2), "tint_");
        }

        public void Update()
        {
            if (GameManager.Instance.IsPaused == false && EnemyBlessed != null)
            {

                if (effectCooldown > 0) { effectCooldown -= Time.deltaTime; }

               

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, this.sprite.WorldCenter);
                lineRenderer.SetPosition(1, EnemyBlessed.sprite.WorldCenter);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        public void FixedUpdate()
        {
            if (GameManager.Instance.IsPaused == false && EnemyBlessed != null)
            {
                EnemyBlessed.IgnoreForRoomClear = ShouldCountForRoomProgress();
                E += 90 * Time.fixedDeltaTime;
                var m = this.sprite.WorldBottomCenter + MathToolbox.GetUnitOnCircle(E, 0.75f);
                var m_2 = EnemyBlessed.sprite.WorldBottomCenter + MathToolbox.GetUnitOnCircle(E, 0.75f);
                var m_3 = EnemyBlessed.sprite.WorldBottomCenter + MathToolbox.GetUnitOnCircle(E + 180, 0.75f);

                GlobalSparksDoer.DoRandomParticleBurst(1, m, m, Vector3.up, 1f, 0.05f, 0.05f, 1, Color.white, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                GlobalSparksDoer.DoRandomParticleBurst(1, m_2, m_2, Vector3.up, 1f, 0.05f, 0.05f, 1, Color.white, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                GlobalSparksDoer.DoRandomParticleBurst(1, m_3, m_3, Vector3.up, 1f, 0.05f, 0.05f, 1, Color.white, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }
        }


        private bool ShouldCountForRoomProgress()
        {
            if (room.remainingReinforcementLayers == null) { return false; }

            bool remainingReinforcements = true;
            if (room.remainingReinforcementLayers.Count == 0) { remainingReinforcements = false; }

            List<AIActor> EnemyList = GetTheseActiveEnemies(room, RoomHandler.ActiveEnemyType.All);

            if (EnemyList.Count == 0 && remainingReinforcements == true) {  return false; }
            if (EnemyList.Count > 0 && remainingReinforcements == true) {  return true; }


            if (EnemyList.Count > 0 && remainingReinforcements == false) { return false; }
            if (EnemyList.Count == 0 && remainingReinforcements == false) { return false; }
            return false;
        }
        public List<AIActor> GetTheseActiveEnemies(RoomHandler room, RoomHandler.ActiveEnemyType type)
        {
            var outList = new List<AIActor>();
            if (room.activeEnemies == null)
            {
                return outList;
            }
            if (type == RoomHandler.ActiveEnemyType.RoomClear)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (!room.activeEnemies[i].IgnoreForRoomClear)
                    {
                        outList.Add(room.activeEnemies[i]);
                    }
                }
            }
            else
            {
                outList.AddRange(room.activeEnemies);
            }
            return outList;
        }

        public void OnDestroy()
        {
            room.Entered -= Room_Entered;
        }
    }
}
