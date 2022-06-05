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
using System.Collections.ObjectModel;

using UnityEngine.Serialization;
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

using System.IO;
using Planetside;
using FullInspector.Internal;

//Garbage Code Incoming
namespace Planetside
{
    public class TestActiveItem : PlayerItem
    {
        public static void Init()
        {
            string itemName = "PSOGTest Active Item";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            TestActiveItem testActive = obj.AddComponent<TestActiveItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used for testing";
            string longDesc = "Test Active For Testing 'On Active' Items.";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            testActive.consumable = false;
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;
            //particle = AdvancedDragunPrefab.GetComponentInChildren<ParticleSystem>();

            // particleTexture = ResourceExtractor.GetTextureFromResource("Planetside/Resources/blashshower.png");


            //AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
            //LaserReticle = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;
            //bundle = null;
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        // ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
        public static Shader RainbowMat = ShaderCache.Acquire("Brave/Internal/StarNest_Derivative");
        public static Shader fuckyou = ShaderCache.Acquire("Brave/Internal/WorldDecay");

        // new Material(ShaderCache.Acquire("Brave/Effects/SimplicityDerivativeShader"));
        private static AssetBundle bundle = ResourceManager.LoadAssetBundle("enemies_base_001");
        private GameObject ShipPrefab = bundle.LoadAsset("assets/data/enemies/bosses/bossfinalrogue.prefab") as GameObject;
        //private BasicBeamController mean = LaserReticle.get

        private static GameObject AdvancedDragunPrefab = bundle.LoadAsset("assets/data/enemies/bosses/dragun.prefab") as GameObject;
        //private static ParticleSystem particle;


        //WORKS
        //            material.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");

        public GameObject spawnedPlayerObject;
        public GameObject objectToSpawn;
        //GoopDefinition goop;
        private Projectile beamProjectile = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueGunController>().GetComponent<BossFinalRogueLaserGun>().beamProjectile;


        //private static GameObject LaserReticle;
        //private tk2dTiledSprite ExtantLaserReticle;


        public float Hell;
        public override void Update()
        {
            if (base.LastOwner != null)
            {
              
                /*
                RoomHandler absoluteRoom = base.LastOwner.transform.position.GetAbsoluteRoom();
                List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                bool flag = activeEnemies != null;
                if (flag)
                {
                    foreach (AIActor enemy in activeEnemies)
                    {
                        enemy.sprite.transform.localRotation = Quaternion.Euler(0f, 0f, Hell);
                    }
                }
                */
            }

            base.Update();
        }
        
      


        protected override void DoEffect(PlayerController user)
        {

            GameObject fart;
            StaticReferences.StoredRoomObjects.TryGetValue("note1", out fart);
            GameObject yes = UnityEngine.Object.Instantiate(fart, user.transform.position, Quaternion.identity);
            user.CurrentRoom.RegisterInteractable(yes.GetComponent<NoteDoer>());

            //AkSoundEngine.PostEvent("Play_BossTheme", base.gameObject);

            //this.MyBallsHaveBeenPlaneted();

            /*
            Dungeon sewerDungeon = DungeonDatabase.GetOrLoadByName("Base_Sewer");
            foreach (WeightedRoom wRoom in sewerDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
            {
                if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                {
                    if (wRoom.room.name.ToLower().StartsWith("sewer_trash_compactor_001"))
                    {
                        GameObject HorizontalCrusher = wRoom.room.placedObjects[0].nonenemyBehaviour.gameObject;
                        UnityEngine.Object.Instantiate<GameObject>(HorizontalCrusher, user.specRigidbody.UnitCenter, Quaternion.identity);
                    }
                }
            }
            */
            /*
            PrototypeDungeonRoom asset = null;
            foreach (var bundle in StaticReferences.AssetBundles.Values)
            {
                asset = bundle.LoadAsset<PrototypeDungeonRoom>("ChallengeShrine_Gungeon_002");
                if (asset)
                    break;
            }
            int Count = 0;
            foreach (PrototypePlacedObjectData obj in asset.placedObjects)
            {
                if (obj != null && obj.nonenemyBehaviour != null)
                {
                    Debug.Log("\n=================");
                    Debug.Log(obj.nonenemyBehaviour.gameObject.name);
                    Debug.Log(Count.ToString() + "\n^ nonenemyBehaviour =========");
                }

                if (obj != null && obj.enemyBehaviourGuid != null)
                {
                    Debug.Log("\n=================");
                    Debug.Log(obj.enemyBehaviourGuid);
                    Debug.Log(Count.ToString() + "\n^ enemyBehaviourGuid=========");
                }

                if (obj != null && obj.placeableContents != null)
                {
                    int Count1 = 0;
                    foreach (DungeonPlaceableVariant obj1 in obj.placeableContents.variantTiers)
                    {
                        if (obj1.nonDatabasePlaceable != null)
                        {
                            Debug.Log("\n=================");
                            Debug.Log(obj1.nonDatabasePlaceable.name);
                            Debug.Log(Count1.ToString() + "\n^ placeableContents.nonDatabasePlaceable =========");
                        }
                        if (obj1.enemyPlaceableGuid != null)
                        {
                            Debug.Log("\n=================");
                            Debug.Log(obj1.enemyPlaceableGuid);
                            Debug.Log(Count1.ToString() + "\n^ placeableContents.enemyPlaceableGuid =========");
                        }

                        Count1++;
                    }
                    Debug.Log(Count.ToString() + "\n=========");
                }
                Count++;
            }
            */


            //PrototypeDungeonRoom room = LoadHelper.LoadAssetFromAnywhere<PrototypeDungeonRoom>("assets/data/rooms/shrine rooms/challenege shrine rooms/challengeshrine_gungeon_002");

            //if (asset != null) //{ obj = asset.placedObjects[0].nonenemyBehaviour.gameObject; }


            
            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");
            string RoomString = "boss foyer (final)";
            foreach (DungeonFlow flows in forgeDungeon.PatternSettings.flows)
            {
                foreach (DungeonFlowNode node in flows.AllNodes)
                {
                    if (node.overrideExactRoom != null)
                    {
                        int Count = 0;
                        if (node.overrideExactRoom.name.ToLower().StartsWith(RoomString))
                        {
                            Debug.Log("Found Room! (Via Node)");
                            foreach (PrototypePlacedObjectData obj in node.overrideExactRoom.placedObjects)
                            {
                                if (obj != null && obj.nonenemyBehaviour != null)
                                {
                                    Debug.Log("\n=================");
                                    Debug.Log(obj.nonenemyBehaviour.gameObject.name);
                                    Debug.Log(Count.ToString() + "\n^ nonenemyBehaviour =========");
                                }

                                if (obj != null && obj.enemyBehaviourGuid != null)
                                {
                                    Debug.Log("\n=================");
                                    Debug.Log(obj.enemyBehaviourGuid);
                                    Debug.Log(Count.ToString() + "\n^ enemyBehaviourGuid=========");
                                }

                                if (obj != null && obj.placeableContents != null)
                                {
                                    int Count1 = 0;
                                    foreach (DungeonPlaceableVariant obj1 in obj.placeableContents.variantTiers)
                                    {
                                        if (obj1.nonDatabasePlaceable != null)
                                        {
                                            Debug.Log("\n=================");
                                            Debug.Log(obj1.nonDatabasePlaceable.name);
                                            Debug.Log(Count1.ToString() + "\n^ placeableContents.nonDatabasePlaceable =========");
                                        }
                                        if (obj1.enemyPlaceableGuid != null)
                                        {
                                            Debug.Log("\n=================");
                                            Debug.Log(obj1.enemyPlaceableGuid);
                                            Debug.Log(Count1.ToString() + "\n^ placeableContents.enemyPlaceableGuid =========");
                                        }

                                        Count1++;
                                    }
                                    Debug.Log(Count.ToString() + "\n=========");
                                }
                                Count++;
                            }
                        }
                    }
                }
                foreach (WeightedRoom wRoom in flows.fallbackRoomTable.includedRooms.elements)
                {
                    if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                    {
                        if (wRoom.room.name.ToLower().StartsWith(RoomString))
                        {
                            int Count = 0;
                            Debug.Log("Found Room! (Via Roomtable)");
                            foreach (PrototypePlacedObjectData obj in wRoom.room.placedObjects)
                            {
                                if (obj != null && obj.nonenemyBehaviour != null)
                                {
                                    Debug.Log("\n=================");
                                    Debug.Log(obj.nonenemyBehaviour.gameObject.name);
                                    Debug.Log(Count.ToString() + "\n^ nonenemyBehaviour =========");
                                }

                                if (obj != null && obj.enemyBehaviourGuid != null)
                                {
                                    Debug.Log("\n=================");
                                    Debug.Log(obj.enemyBehaviourGuid);
                                    Debug.Log(Count.ToString() + "\n^ enemyBehaviourGuid=========");
                                }

                                if (obj != null && obj.placeableContents != null)
                                {
                                    int Count1 = 0;
                                    foreach (DungeonPlaceableVariant obj1 in obj.placeableContents.variantTiers)
                                    {
                                        if (obj1.nonDatabasePlaceable != null)
                                        {
                                            Debug.Log("\n=================");
                                            Debug.Log(obj1.nonDatabasePlaceable.name);
                                            Debug.Log(Count1.ToString() + "\n^ placeableContents.nonDatabasePlaceable =========");
                                        }
                                        if (obj1.enemyPlaceableGuid != null)
                                        {
                                            Debug.Log("\n=================");
                                            Debug.Log(obj1.enemyPlaceableGuid);
                                            Debug.Log(Count1.ToString() + "\n^ placeableContents.enemyPlaceableGuid =========");
                                        }

                                        Count1++;
                                    }
                                    Debug.Log(Count.ToString() + "\n=========");
                                }
                                Count++;
                            }
                        }
                    }
                }
            }
            forgeDungeon = null;


            //ETGModConsole.Log(BraveTime.DeltaTime.ToString());







            /*
            Vector2 vector = ReflectionHelper.ReflectGetField<Vector2>(typeof(Gun), "m_localAimPoint", user.CurrentGun) -user.specRigidbody.HitboxPixelCollider.UnitCenter;
            if (this.ExtantLaserReticle == null)
            {
                string path = "Global VFX/VFX_LaserSight";
                if (!(user is PlayerController))
                {
                    path = ((!this.ExtantLaserReticle) ? "Global VFX/VFX_LaserSight_Enemy" : "Global VFX/VFX_LaserSight_Enemy_Green");
                }
                this.ExtantLaserReticle = SpawnManager.SpawnVFX(LaserReticle, false).GetComponent<tk2dTiledSprite>();
                this.ExtantLaserReticle.IsPerpendicular = false;
                this.ExtantLaserReticle.HeightOffGround = 22;
                this.ExtantLaserReticle.renderer.enabled = true;
                //this.ExtantLaserReticle.transform.parent = user.CurrentGun.barrelOffset;
            }
            */
            //this.ExtantLaserReticle.transform.localPosition = Vector3.zero;
            //this.ExtantLaserReticle.transform.rotation = Quaternion.Euler(0f, 0f, user.CurrentGun.CurrentAngle);


            /*
            if (this.ExtantLaserReticle.renderer.enabled)
            {
                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                bool flag2 = false;
                float num9 = float.MaxValue;
                {
                    CollisionLayer layer2 = (!(user is PlayerController)) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox;
                    int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, layer2, CollisionLayer.BulletBreakable);
                    RaycastResult raycastResult2;
                    if (PhysicsEngine.Instance.Raycast(user.CurrentGun.barrelOffset.position.XY(), vector, 1000, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
                    {
                        flag2 = true;
                        num9 = raycastResult2.Distance;
                        
                        if (raycastResult2.SpeculativeRigidbody && raycastResult2.SpeculativeRigidbody.aiActor)
                        {
                            user.CurrentGun.HandleEnemyHitByLaserSight(raycastResult2.SpeculativeRigidbody.aiActor);
                        }
                        
                    }
                    RaycastResult.Pool.Free(ref raycastResult2);
                }
                this.ExtantLaserReticle.dimensions = new Vector2((!flag2) ? 480f : (num9 / 0.0625f), 1f);
                this.ExtantLaserReticle.ForceRotationRebuild();
                this.ExtantLaserReticle.UpdateZDepth();
            }

            */




            /*
            SpawnObjectPlayerItem Molotv = PickupObjectDatabase.GetById(366).GetComponent<SpawnObjectPlayerItem>();
            Vector3 vector = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
            Vector3 vector2 = user.specRigidbody.UnitCenter;
            if (vector.y > 0f)
            {
                vector2 += Vector3.up * 0.25f;
            }
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Molotv.objectToSpawn, vector2, Quaternion.identity);
            tk2dBaseSprite component4 = gameObject2.GetComponent<tk2dBaseSprite>();
            if (component4)
            {
                component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
            }
            this.spawnedPlayerObject = gameObject2;
            Vector2 vector3 = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
            vector3 = Quaternion.Euler(0f, 0f, 0) * vector3;
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject2, gameObject2.transform.position, vector3, Molotv.tossForce, false, false, true, false);
            if (gameObject2.GetComponent<BlackHoleDoer>())
            {
                debrisObject.PreventFallingInPits = true;
                debrisObject.PreventAbsorption = true;
            }
            if (vector.y > 0f && debrisObject)
            {
                debrisObject.additionalHeightBoost = -1f;
                if (debrisObject.sprite)
                {
                    debrisObject.sprite.UpdateZDepth();
                }
            }
            debrisObject.IsAccurateDebris = true;
            debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
            debrisObject.bounceCount = ((!Molotv.canBounce) ? 0 : 1);
        */

            base.DoEffect(user);

            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(TrailRenderer.gameObject, user.specRigidbody.UnitCenter, Quaternion.identity);
            //CustomTrailRenderer yes = gameObject.GetComponent<CustomTrailRenderer>();
            //yes.gameObject.transform.position = user.transform.position;
            //CustomTrailRenderer red =  user.gameObject.AddComponent<CustomTrailRenderer>();
            //red.CopyFrom<CustomTrailRenderer>(TrailRendererobj);

            //GameObject reaper = UnityEngine.Object.Instantiate<GameObject>(SomethingWickedEnemy.SomethingWickedObject, user.gameObject.transform.position - new Vector3(10, 0), Quaternion.identity);

            //GameObject original;
            //original = RandomPiecesOfStuffToInitialise.SomethingWicked;
            //GameObject component = UnityEngine.Object.Instantiate<GameObject>(RandomPiecesOfStuffToInitialise.SomethingWicked, user.specRigidbody.UnitCenter, Quaternion.identity);
            //component.transform.position.WithZ(transform.position.z + 99999);
            //component.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(user.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
            //component.PlaceAtPositionByAnchor(base.LastOwner.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
            //component.GetComponent<tk2dSprite>().scale = Vector3.one;
            //SomethingWicked wispCont = component.gameObject.AddComponent<SomethingWicked>();


            /*
            var enemy = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec");
            if (!enemy)
            {
                ETGModConsole.Log("enemy null");
            }

            ETGModConsole.Log("Room name: " + user.CurrentRoom.GetRoomName()+ "\nLength: " + user.CurrentRoom.area.dimensions.x.ToString() + "\nWidth: " + user.CurrentRoom.area.dimensions.y.ToString());
            */
            /*
            WeightedRoomCollection ballin = GungeonAPI.OfficialFlows.GetDungeonPrefab("base_resourcefulrat").PatternSettings.flows[0].AllNodes[18].overrideRoomTable.includedRooms;
            List<WeightedRoom> yes = ballin.elements;
            foreach(WeightedRoom fuck in yes)
            {
                List<PrototypePlacedObjectData> place = fuck.room.placedObjects;
                foreach(PrototypePlacedObjectData prototypePlacedObjectData in place)
                {
                    ETGModConsole.Log(prototypePlacedObjectData.GetType().ToString());
                }
                ETGModConsole.Log("================");
            }
            */

            //GameObject obj = AdvancedDragunPrefab.GetComponentInChildren<CustomTrailRenderer>().gameObject;
            //user.gameObject.AddComponent<TrailRendereryeah>();

            //Projectile beam = null;
            //ParticleSystem yes = user.gameObject.AddComponent<ParticleSystem>();
            //yes.CopyFrom<ParticleSystem>(particle);
            //yes.Play();
            //yes.SetCustomParticleData(new List<Vector4> { }, ParticleSystemCustomData.Custom1);
            //yes.SetFields(particle, true, true);
            //yes.name = "fucka you lol";




            /*
            ParticleSystem yes = user.gameObject.AddComponent<ParticleSystem>();
            //yes.CopyFrom<ParticleSystem>(particle);
            yes.Play();
            yes.name = "fucka you lol";
            var sc = yes.shape;
            sc.shapeType = ParticleSystemShapeType.Circle;
            sc.radius = 0.1f;

            var tsa = yes.textureSheetAnimation;
            tsa.animation = ParticleSystemAnimationType.SingleRow;
            tsa.numTilesX = 4;
            tsa.numTilesY = 1;
            tsa.enabled = true;
            tsa.cycleCount = 10;
            tsa.frameOverTimeMultiplier = 1.3f;
            


            var particleRenderer = yes.gameObject.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
            particleRenderer.material.mainTexture = particleTexture;
            */
            //particleRenderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");

            /*
            foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
            {


                if (item is ParticleSystemRenderer laser)
                {
                    if (laser.name == "VFX_Gold_Dragun_Eye_Fire_Exhaust")
                    {
                        //ParticleSystem yes = user.gameObject.AddComponent<ParticleSystem>();
                        ParticleSystemRenderer yes = user.gameObject.AddComponent<ParticleSystemRenderer>();
                        yes.CopyFrom<ParticleSystemRenderer>(laser);
                        
                        break;
                    }
                }
                if (item is ParticleSystem particle)
                {
                    if (particle.name == "VFX_Gold_Dragun_Eye_Fire_Exhaust")
                    {
                        ParticleSystem yes = user.gameObject.AddComponent<ParticleSystem>();
                        //yes.CopyFrom<ParticleSystem>(particle);
                        yes.Play();
                        yes.name = "fucka you lol";
                        var sc = yes.shape;//.shapeType = ParticleSystemShapeType.Circle;
                        sc.shapeType = ParticleSystemShapeType.Circle;
                        sc.radius = 0.1f;
                        //var ts = yes.textureSheetAnimation;
                        

                        var particleRenderer = yes.gameObject.GetComponent<ParticleSystemRenderer>();
                        particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        particleRenderer.material.mainTexture = particleTexture;
                        break;
                    }
                }
                
                //ETGModConsole.Log("====");
                //ETGModConsole.Log(item.name);
                //ETGModConsole.Log(item.GetType().Name.ToString());
            }
            */
            //Fire(beam);

            /*
            float RNG = UnityEngine.Random.Range(5.5f, 15f);
            ETGModConsole.Log("As /= ");
            float As = RNG /= 5.5f;
            ETGModConsole.Log(As.ToString());
            ETGModConsole.Log("================");
            float Is = RNG / 5.5f;
            ETGModConsole.Log(Is.ToString());
            ETGModConsole.Log("As /");
            */
            /*
            float Loop = SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.TIMES_LOOPED);
           // UIToolbox.TextBox(Color.white, "Ouroborous Level: " + Loop.ToString() ,user.gameObject,dfPivotPoint.TopCenter ,new Vector3(user.specRigidbody.UnitCenter.x - user.transform.position.x, 1.25f, 0f), 5, 1.5f);
            //List<AssetBundle> assetBundle = new List<AssetBundle> { ResourceManager.LoadAssetBundle("shared_auto_001"), ResourceManager.LoadAssetBundle("shared_auto_002"), ResourceManager.LoadAssetBundle("shared_auto_003"), ResourceManager.LoadAssetBundle("brave_resources_001"), ResourceManager.LoadAssetBundle("dungeon_scene_001"), ResourceManager.LoadAssetBundle("encounters_base_001"), ResourceManager.LoadAssetBundle("enemies_base_001"), ResourceManager.LoadAssetBundle("flows_base_001"), ResourceManager.LoadAssetBundle("foyer_001"), ResourceManager.LoadAssetBundle("foyer_002"), ResourceManager.LoadAssetBundle("foyer_003") };
            ETGModConsole.Log(user.CurrentRoom.GetRoomName());
            this.goop = user.CurrentGoop;
            if (this.goop != null)
            {
                ETGModConsole.Log("=================================");
                ETGModConsole.Log(goop.name);
                Tools.LogPropertiesAndFields(goop);
                //Object t = Resources.LoadAssetAtPath(localPath, typeof(T));
            }
            
            List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            Vector2 centerPosition = user.sprite.WorldCenter;
            if (activeEnemies != null)
            {
                foreach (AIActor aiactor in activeEnemies)
                {
                    bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && user != null;
                    if (ae)
                    {

                        ETGModConsole.Log(aiactor.encounterTrackable.EncounterGuid);
                        ETGModConsole.Log(aiactor.healthHaver.GetMaxHealth().ToString());
                        ETGModConsole.Log("=========================================");

                    }
                }
            }
            */
            //AkSoundEngine.PostEvent("Play_BigSlam", base.gameObject);
            /*
            List<AIActor> activeEnemies = base.LastOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null)
            {
                foreach (AIActor aiactor in activeEnemies)
                {
                    AddBeholsterBeamComponent yee = aiactor.gameObject.AddComponent<AddBeholsterBeamComponent>();
                    yee.AddsBaseBeamBehavior = true;
                }
            }
            */
            //assetBundle.LoadAsset(text);

            //for (int i = 0; i < GoopDefinition.Instance.transform.childCount; i++)
            //{
            //ETGModConsole.Log(Minimap.Instance.transform.GetChild(i).gameObject.name);
            //}

            //this.objectToSpawn = EnemyDatabase.GetOrLoadByGuid(LoaderPylonController.guid).gameObject;
            /*
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LoaderPylonController.Turretprefab, user.specRigidbody.UnitCenter, Quaternion.identity);
            this.spawnedPlayerObject = gameObject;
            tk2dBaseSprite component2 = gameObject.GetComponent<tk2dBaseSprite>();
            if (component2 != null)
            {
                component2.PlaceAtPositionByAnchor(user.specRigidbody.UnitCenter.ToVector3ZUp(component2.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
                if (component2.specRigidbody != null)
                {
                    component2.specRigidbody.RegisterGhostCollisionException(user.specRigidbody);
                }
            }
            SpawnObjectItem componentInChildren2 = this.spawnedPlayerObject.GetComponentInChildren<SpawnObjectItem>();
            if (componentInChildren2)
            {
                componentInChildren2.SpawningPlayer = this.LastOwner;
            }
            LoaderPylonController yah = gameObject.AddComponent<LoaderPylonController>();
            if (yah != null)
            {
                yah.maxDuration = 120f;
            }
            */
            /*
            string guid;
            guid = "01972dee89fc4404a5c408d50007dad5";

            PlayerController owner = base.LastOwner;
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
            IntVector2? intVector = new IntVector2?(base.LastOwner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
            aiactor.CanTargetPlayers = true;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
            aiactor.IgnoreForRoomClear = true;
            aiactor.IsHarmlessEnemy = false;
            aiactor.HandleReinforcementFallIntoRoom(0f);
            */
            //aiactor.gameObject.AddComponent<UmbraController>();

            /*
            Chest rainbow_Chest = GameManager.Instance.RewardManager.S_Chest;
            Chest chest2 = Chest.Spawn(rainbow_Chest, user.CurrentRoom.GetRandomVisibleClearSpot(1, 1));
            chest2.sprite.usesOverrideMaterial = true;

            var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\plating.png");

            chest2.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
            chest2.sprite.renderer.material.SetTexture("_EeveeTex", texture);
            //chest2.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            //chest2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
            chest2.sprite.renderer.material.SetFloat("_StencilVal", 100000);
            chest2.sprite.renderer.material.SetFloat("_FlatColor", 0f);
            chest2.sprite.renderer.material.SetFloat("_Perpendicular", 0);


            chest2.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            chest2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            chest2.lootTable.S_Chance = 0.2f;
            chest2.lootTable.A_Chance = 0.2f;
            chest2.lootTable.B_Chance = 0.22f;
            chest2.lootTable.C_Chance = 0.22f;
            chest2.lootTable.D_Chance = 0.16f;
            chest2.lootTable.Common_Chance = 0f;
            chest2.lootTable.canDropMultipleItems = true;
            chest2.lootTable.multipleItemDropChances = new WeightedIntCollection();
            chest2.lootTable.multipleItemDropChances.elements = new WeightedInt[1];
            chest2.lootTable.overrideItemLootTables = new List<GenericLootTable>();
            chest2.lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
            chest2.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
            WeightedInt weightedInt = new WeightedInt();
            weightedInt.value = 24;
            weightedInt.weight = 1f;
            weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
            chest2.lootTable.multipleItemDropChances.elements[0] = weightedInt;
            chest2.lootTable.onlyOneGunCanDrop = false;
            chest2.RegisterChestOnMinimap(chest2.GetAbsoluteParentRoom());
            */

            /*
            GameObject gameObject = new GameObject();
            gameObject.transform.position = user.sprite.WorldCenter;
            BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
            gameObject.AddComponent<BulletSourceKiller>();
            var bulletScriptSelected = new CustomBulletScriptSelector(typeof(AngerGodsScript));
            AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
            AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
            bulletBank.CollidesWithEnemies = true;
            source.BulletManager = bulletBank;
            source.BulletScript = bulletScriptSelected;
            source.Initialize();//to fire the script once
            */
            /*
            string guid;
            guid = "c4cf0620f71c4678bb8d77929fd4feff";
            PlayerController owner = user;
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
            IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
            //aiactor.gameObject.AddComponent<PlayerController>();
            //Chest aaa = aiactor.gameObject.AddComponent<Chest>();
            //var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
            var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");

            aiactor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/StarNest_Derivative");
            */
            //aiactor.sprite.renderer.material.SetTexture("_EeveeTex", texture);

            //     aiactor.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            //   aiactor.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");

            /*
            aiactor.CanTargetEnemies = false;
            aiactor.CanTargetPlayers = true;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
            aiactor.IsHarmlessEnemy = false;
            aiactor.IgnoreForRoomClear = true;
            aiactor.HandleReinforcementFallIntoRoom(-1f);
            */

        }
        private BasicBeamController m_laserBeam;
        public Transform beamTransform;
        private bool m_firingLaser;
        //private float LaserAngle = 90;

        public void Fire(Projectile projectile)
        {
            m_firingLaser = true;
            //this.m_fireTimer = this.fireTime;
            //this.LaserAngle = -90f;
            //if (this.LightToTrigger)
            //{
            //    this.LightToTrigger.ManuallyDoBulletSpawnedFade();
            //}
            beamTransform = GameManager.Instance.PrimaryPlayer.transform;
            base.StartCoroutine(this.FireBeam(projectile));
            //base.StartCoroutine(this.DoGunMotionCR());
            //if (this.doScreenShake)
            //{
            //    GameManager.Instance.MainCameraController.DoContinuousScreenShake(this.screenShake, this, false);
            //}
        }

        public void LateUpdate()
        {
            if (m_firingLaser && this.m_laserBeam)
            {
                this.m_laserBeam.LateUpdatePosition(this.beamTransform.position);
            }
            else if (this.m_laserBeam && this.m_laserBeam.State == BasicBeamController.BeamState.Dissipating)
            {
                this.m_laserBeam.LateUpdatePosition(this.beamTransform.position);
            }
        }
        public IntVector2 colliderOffset;

        public IntVector2 colliderSize;
        protected IEnumerator FireBeam(Projectile projectile)
        {
            GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(projectile.gameObject);
            this.m_laserBeam = beamObject.GetComponent<BasicBeamController>();
            this.m_laserBeam.Owner = LastOwner;
            this.m_laserBeam.HitsPlayers = false;
            this.m_laserBeam.HitsEnemies = true;
            this.m_laserBeam.collisionLength = 100;
            this.m_laserBeam.collisionWidth = 6;
            this.m_laserBeam.collisionRadius = 6;
            this.m_laserBeam.collisionType = BasicBeamController.BeamCollisionType.Default;

            //this.m_laserBeam.projectile.collidesWithEnemies = true;



            this.m_laserBeam.OverrideHitChecks = delegate (SpeculativeRigidbody hitRigidbody, Vector2 dirVec)
            {
                HealthHaver healthHaver = (!hitRigidbody) ? null : hitRigidbody.healthHaver;
                if (hitRigidbody && hitRigidbody.projectile && hitRigidbody.GetComponent<BeholsterBounceRocket>())
                {
                    BounceProjModifier component = hitRigidbody.GetComponent<BounceProjModifier>();
                    if (component)
                    {
                        component.numberOfBounces = 0;
                    }
                    hitRigidbody.projectile.DieInAir(false, true, true, false);
                }
                if (healthHaver != null)
                {
                    if (healthHaver.aiActor)
                    {
                        Projectile currentProjectile = projectile;
                        healthHaver.ApplyDamage(ProjectileData.FixedFallbackDamageToEnemies, dirVec, "Death", currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
                    }
                    else
                    {
                        Projectile currentProjectile = projectile;
                        healthHaver.ApplyDamage(ProjectileData.FixedFallbackDamageToEnemies, dirVec, "Death", currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
                    }
                }
                if (hitRigidbody.majorBreakable)
                {
                    hitRigidbody.majorBreakable.ApplyDamage(26f * BraveTime.DeltaTime, dirVec, false, false, false);
                }
            };

            this.m_laserBeam.ContinueBeamArtToWall = true;
            this.m_laserBeam.projectile.baseData.damage = 200;
            this.m_laserBeam.projectile.collidesWithEnemies = true;


            bool firstFrame = true;
            while (this.m_laserBeam != null && this.m_firingLaser)
            {
                float clampedAngle = BraveMathCollege.ClampAngle360(LastOwner.CurrentGun.CurrentAngle);
                Vector2 dirVec = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f)) * 10f;
                this.m_laserBeam.Origin = this.beamTransform.position;
                this.m_laserBeam.Direction = dirVec;
                if (firstFrame)
                {
                    yield return null;
                    firstFrame = false;
                }
                else
                {
                    yield return null;
                    while (Time.timeScale == 0f)
                    {
                        yield return null;
                    }
                }
            }
            if (!this.m_firingLaser && this.m_laserBeam != null)
            {
                this.m_laserBeam.CeaseAttack();
            }
            if (this.m_laserBeam)
            {
                this.m_laserBeam.SelfUpdate = false;
                while (this.m_laserBeam)
                {
                    this.m_laserBeam.Origin = this.beamTransform.position;
                    yield return null;
                }
            }
            this.m_laserBeam = null;
            yield break;
        }

        /*
        protected IEnumerator FireBeam()
        {
            ETGModConsole.Log("1");
            //this.m_firingLaser = true;
            //GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(projectile.gameObject);
            ETGModConsole.Log("2");

            //BasicBeamController hinge = ShipPrefab.GetComponent("BasicBeamController") as BasicBeamController;
            //List<Transform> proj = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponents<Transform>();


            //BasicBeamController beam = ShipPrefab.GetComponentInChildren<BasicBeamController>();

            BasicBeamController beam = m_laserBeam;
            Transform beamtrans = base.LastOwner.transform;
            //ETGModConsole.Log("2.1");
            var enemy = EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5");
            foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
            {
                //ETGModConsole.Log("2.2");
                if (item is BossFinalRogueLaserGun laser)
                {
                    //ETGModConsole.Log("2.3");
                    if (laser.beamProjectile != null)
                    {
                        //ETGModConsole.Log("2.4");
                        GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(laser.beamProjectile.gameObject);
                        beam = beamObject.GetOrAddComponent<BasicBeamController>();
                        //GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(laser.projectile.gameObject);
                        //this.m_laserBeam = beamObject.GetComponent<BasicBeamController>();
                    }
                    if (laser.beamTransform != null)
                    {
                        beamtrans = laser.beamTransform;
                    }
                }
            }
            
            //ETGModConsole.Log("3");

            if (beam != null)
            {
                //ETGModConsole.Log("4");
                beam.Owner = base.LastOwner;
                beam.HitsPlayers = true;//projectile.collidesWithPlayer;
                beam.HitsEnemies = true;//projectile.collidesWithEnemies;
                beam.ContinueBeamArtToWall = false;
                beam.chargeDelay = 1;
                beam.usesTelegraph = true;
                beam.renderer.enabled = true;
                bool firstFrame = true;
                while (beam != null)// && this.m_firingLaser)
                {
                    tk2dSprite sproot1 = beam.GetComponent<tk2dSprite>();
                    if (sproot1 != null)
                    {
                        sproot1.HeightOffGround = -2;
                    }
                    //ETGModConsole.Log(beam.Origin.ToString());
                    //ETGModConsole.Log(base.LastOwner.transform.position.ToString());
                    float clampedAngle = BraveMathCollege.ClampAngle360(base.LastOwner.CurrentGun.CurrentAngle);
                    Vector2 dirVec = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f)) * 10f;

                    beam.Origin = base.LastOwner.sprite.transform.position + beamtrans.position;// + new Vector3(-15, -5);
                    beam.Direction = dirVec;
                    beam.transform.position.WithZ(beam.transform.position.z + 99999);

                    //beam.gameObject.transform.position = GameManager.Instance.PrimaryPlayer.transform.position;
                    beam.sprite.UpdateZDepth();
                    if (firstFrame)
                    {
                        yield return null;
                        firstFrame = false;
                    }
                    else
                    {
                        yield return null;
                        while (Time.timeScale == 0f)
                        {
                            yield return null;
                        }
                    }
                }
                //if (!this.m_firingLaser && beam != null)
                //{
                    //this.m_firingLaser = false;
                    //beam.CeaseAttack();
                //}
                /*
                if (beam)
                {
                    beam.SelfUpdate = false;
                    while (beam)
                    {
                        beam.Origin = base.LastOwner.sprite.transform.position + beamtrans.position;
                        yield return null;
                    }
                }
                //beam = null;
                */


    }
}



