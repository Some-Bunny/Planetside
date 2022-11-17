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
using UnityEngine.Video;
using static FullInspector.Internal.fiLateBindings;
using UnityEngine.Audio;


namespace Planetside
{

    public class Updater : MonoBehaviour
    {
        public void Update()
        {
           // base.gameObject.transform.Rotate(new Vector3(0f, 1f, 0f), 90f, Space.World);
            base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
        }
    }

    public class RoomReader : PlayerItem
    {
        public static void Init()
        {
            string itemName = "RoomReader";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            RoomReader testActive = obj.AddComponent<RoomReader>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Reads Rooms";
            string longDesc = "Prints The Name Of The Current Room Into The F2 Console";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            testActive.consumable = false;
            ItemBuilder.AddPassiveStatModifier(testActive, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;
            particle = AdvancedDragunPrefab.GetComponentInChildren<ParticleSystem>();


            RoomReader.ball = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Resault/plus16ammosyn", null, false);
            UnityEngine.Object.DontDestroyOnLoad(RoomReader.ball);
            FakePrefab.MarkAsFakePrefab(RoomReader.ball);
            RoomReader.ball.SetActive(false);
        }

        private static GameObject ball;

        private static AssetBundle bundle = ResourceManager.LoadAssetBundle("enemies_base_001");
        private static GameObject AdvancedDragunPrefab = bundle.LoadAsset("assets/data/enemies/bosses/dragun.prefab") as GameObject;
        private static ParticleSystem particle;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }




        protected override void DoEffect(PlayerController user)
        {
            /*
            var portal = PlanetsideModule.ModAssets.LoadAsset<GameObject>("vdeioplayer");
            MeshRenderer rend = portal.GetComponent<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            portal.name = "ReturnPortal";
            portal.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            portal.transform.position = user.transform.position;
            */
            
            //List<Type> t = UnityEngine.Object.FindObjectsOfType<Component>().GetType().GetBaseType() 





            ETGModConsole.Log(user.CurrentRoom.GetRoomName());

            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("864ea5a6a9324efc95a0dd2407f42810");
            IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Spawn, true);


            for (int i = 0; i < 4; i++)
            {
                var pp = UnityEngine.Object.Instantiate<GameObject>(Ophanaim.EyeBallMinion, aiactor.transform.position + MathToolbox.GetUnitOnCircle(90 * i, 2).ToVector3ZisY(), Quaternion.identity, null).GetComponent<AdvancedBodyPartController>();
                pp.MainBody = aiactor;
                pp.gameObject.transform.parent = aiactor.transform;
            }

           

            //var hODC = (PickupObjectDatabase.GetById(515) as Gun).DefaultModule.projectiles[0].bleedEffect.vfxExplosion.GetComponent<HammerOfDawnController>();

            //StaticVFXStorage.MourningStarVFXController.SpawnMourningStar(user.sprite.WorldBottomCenter, 4);


            //var h = UnityEngine.Object.Instantiate<GameObject>(StaticVFXStorage.MourningStarLaser, user.transform.position, Quaternion.identity, user.transform).GetComponent<StaticVFXStorage.MourningStarVFXComponent>();
            //h.Invoke("Dissipate", 5);

            //UnityEngine.Object.Instantiate<GameObject>(hODC.BurstSprite.gameObject, user.transform.position + new Vector3(5,0), Quaternion.identity, user.transform);
            //UnityEngine.Object.Instantiate<GameObject>(hODC.InitialImpactVFX, user.transform.position - new Vector3(5, 0), Quaternion.identity, user.transform);

            //UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(515) as Gun).DefaultModule.projectiles[0].bleedEffect.vfxExplosion, user.transform.position + new Vector3(5, 5), Quaternion.identity, user.transform);

            //if (hODC != null) { ETGModConsole.Log("got"); }




            //partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            /*
            RoomHandler roome =  user.CurrentRoom;
            ETGModConsole.Log(user.CurrentRoom.GetRoomName());
            ETGModConsole.Log("room visual subtype int: " + roome.RoomVisualSubtype.ToString());
            Dungeon currentFloor = GameManager.Instance.Dungeon;
            DungeonMaterial mat = currentFloor.roomMaterialDefinitions[roome.RoomVisualSubtype];
            ItemAPI.Tools.LogPropertiesAndFields(mat.roomFloorBorderGrid, "tile", true);
            */

            /*
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(HardlightNailgun.HardAsNailsID).gameObject, user.transform.position, Quaternion.identity);
            Gun component = gameObject.GetComponent<Gun>();
            if (component != null)
            {
                //LootEngine.PostprocessGunSpawn(component);
                component.DropGun(2f);
            }
            */
            //mat.roomFloorBorderGrid

            /*
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("RobotShopkeeperBoss_friendly");
            IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Spawn, true);
            aiactor.HandleReinforcementFallIntoRoom(0f);
            CompanionController comp = aiactor.gameActor.GetComponent<CompanionController>();
            comp.Initialize(user);

            aiactor.CompanionOwner = user;
            aiactor.specRigidbody.Reinitialize();
            */
            //RuntimePrototypeRoomData hjg =  roome.area.runtimePrototypeData;
            //PrototypeDungeonRoom eee = roome.area.prototypeRoom;

            //Play_MUS_Ending_Pilot_01
            //AkSoundEngine.PostEvent("Stop_MUS_Ending_Pilot", GameManager.Instance.gameObject);

            /*
            PrototypeDungeonRoom eee = LoadHelper.LoadAssetFromAnywhere<PrototypeDungeonRoom>("assets/data/rooms/npc rooms/npc_synergrace_room.asset");
            ETGModConsole.Log("START============");
            if (eee != null)
            {
                if (eee.CustomMusicEvent != null)
                {
                    ETGModConsole.Log("CustomMusicEvent:");
                    ETGModConsole.Log(eee.CustomMusicEvent);
                }
                if (eee.CustomMusicSwitch != null)
                {
                    ETGModConsole.Log("CustomMusicSwitch:");
                    ETGModConsole.Log(eee.CustomMusicSwitch);
                }

            }
            if (eee != null)
            {
                if (eee.OverrideMusicState != null)
                {
                    ETGModConsole.Log("OverrideMusicState:");
                    ETGModConsole.Log(eee.OverrideMusicState.ToString());
                }
                if (eee.UseCustomMusic != null)
                {
                    ETGModConsole.Log("UseCustomMusic:");
                    ETGModConsole.Log(eee.UseCustomMusic.ToString());
                }
                if (eee.UseCustomMusicState != null)
                {
                    ETGModConsole.Log("UseCustomMusicState:");
                    ETGModConsole.Log(eee.UseCustomMusicState.ToString());
                }
                if (eee.UseCustomMusicSwitch != null)
                {
                    ETGModConsole.Log("UseCustomMusicSwitch:");
                    ETGModConsole.Log(eee.UseCustomMusicSwitch.ToString());
                }
            }
            
            ETGModConsole.Log("END============");
            */
            //var partObj2 = UnityEngine.Object.Instantiate(ball); ;//this is the name of the object which by default will be "Particle System"

            /*
            var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal")); ;//this is the name of the object which by default will be "Particle System"
            MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
            
            rend.allowOcclusionWhenDynamic = true;

            partObj.transform.position = user.transform.position;

            partObj.transform.localScale = Vector3.one;
            partObj.name = "yes";
            partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            TrespassPortalController romas= partObj.AddComponent<TrespassPortalController>();
            romas.m_room = user.CurrentRoom;
            */
            /*
            BossManager manager = GameManager.Instance.BossManager;

            foreach (BossFloorEntry cum in manager.BossFloorData)
            {
                foreach (IndividualBossFloorEntry fycjyou in cum.Bosses)
                {
                    ETGModConsole.Log(fycjyou.TargetRoomTable.name);
                }
            }
            */
            /*
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("jammed_guardian");
            IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Spawn, true);
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = false;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
            aiactor.gameObject.AddComponent<KillOnRoomClear>();
            aiactor.IgnoreForRoomClear = true;
            aiactor.IsHarmlessEnemy = true;
            aiactor.HandleReinforcementFallIntoRoom(0f);
            */
            //GameObject shader = PlanetsideModule.ModAssets.LoadAsset<GameObject>("SphereObj");
            //UnityEngine.Object.Instantiate<GameObject>(shader, user.specRigidbody.UnitCenter, Quaternion.identity);
            /*
            GameObject obj = PlanetsideModule.ModAssets.LoadAsset<GameObject>("sphere 1");
            obj.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            Instantiate<GameObject>(obj, user.transform.position, Quaternion.Euler(0, 90f, 0));
            obj.AddComponent<Updater>();
            */

            //obj.GetOrAddComponent<MeshFilter>().mesh = obj.GetComponent<Mesh>();


            //obj.transform.Rotate(new Vector3(0f, 0f, 0f), 90f, Space.Self);

            /*
            Gun gun = PickupObjectDatabase.GetById(483) as Gun;

            foreach (Component item in gun.DefaultModule.projectiles[0].GetComponentsInChildren(typeof(Component)))
            {
                if (item != null)
                {
                    ETGModConsole.Log(item.name+"\n"+item.GetType().ToString()+"\n=========");
                }
            }
            */

            /*
            Vector2 vector = user.transform.PositionVector2();
            ETGModConsole.Log("2");
            Vector2 vector2 = new Vector2();
            ETGModConsole.Log("3");
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
            ETGModConsole.Log("4");
            //CollisionLayer layer2 = (!(user is PlayerController)) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox;
            ETGModConsole.Log("5");
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
            ETGModConsole.Log("6");
            RaycastResult raycastResult2;
            ETGModConsole.Log("7");
            Vector2 Point = MathToolbox.GetUnitOnCircle(user.CurrentGun.CurrentAngle, 1);
            if (PhysicsEngine.Instance.Raycast(user.transform.PositionVector2(), Point, 1000, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
            {
                ETGModConsole.Log("7A");
                vector2 = raycastResult2.Contact;
            }
            ETGModConsole.Log("8");
            RaycastResult.Pool.Free(ref raycastResult2);
            */

            //int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector, vector2)), 1);
            //ETGModConsole.Log("ahfuck before" + num2.ToString(), false);
            //ETGModConsole.Log("ahfuck " + num2.ToString(), false);
            //for (int i = 0; i < num2; i++)
            {
                //float t = (float)i / (float)num2;
                //Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
                //GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                //AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
                //GameObject gameObject = new GameObject("silencer");
                //SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                //float additionalTimeAtMaxRadius = 0.25f;
                //silencerInstance.TriggerSilencer(vector3, 25f, 5f, silencerVFX, 0f, 3f, 3f, 3f, 250f, 5f, additionalTimeAtMaxRadius, user, false, false);
                /*
                ETGModConsole.Log("10");
                float t = (float)i / (float)num2;
                ETGModConsole.Log("11");
                Vector3 vector3 = Vector3.Lerp(vector, vector2.ToCenterVector2(), t);
                ETGModConsole.Log("12");
                vector3 += Vector3.back;
                ETGModConsole.Log("13");
                float num3 = Mathf.PerlinNoise(vector3.x / 3f, vector3.y / 3f);
                ETGModConsole.Log("14");
                Vector3 a = Quaternion.Euler(0f, 0f, num3 * 360f) * Vector3.right;
                ETGModConsole.Log("15");
                Vector3 a2 = Vector3.Lerp(a, UnityEngine.Random.insideUnitSphere, UnityEngine.Random.Range(1, 3));
                ETGModConsole.Log("15");
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = vector3,
                    velocity = a2 * 2,
                    startSize = 1,
                    startLifetime = 0.3f,
                    startColor = Color.blue
                };
                ETGModConsole.Log("16");

                particle.Emit(emitParams, 1);
                ETGModConsole.Log("17");
                */
            }

            //AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
            //AkSoundEngine.PostEvent("Stop_MUS_PrisonerTheme", GameManager.Instance.gameObject);


            //AkSoundEngine.PostEvent("Play_BossTheme", user.gameObject);





            /*
            Dungeon sewerDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");

            GameObject obj = null;

            PrototypeDungeonRoom asset = null;
            foreach (var bundle in StaticReferences.AssetBundles.Values)
            {
                asset = bundle.LoadAsset<PrototypeDungeonRoom>("ChallengeShrine_Gungeon_002");
                if (asset)
                    break;
            }



            obj = asset.placedObjects[0].nonenemyBehaviour.gameObject;
            ChallengeShrineController controller =  obj.GetOrAddComponent<ChallengeShrineController>();
            */
            //controller.ConfigureOnPlacement(user.CurrentRoom);

            //FieldInfo m_parentRoom = controller.GetType().GetField("m_parentRoom", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_parentRoom.SetValue(controller, user.CurrentRoom);

            //RoomHandler room = ReflectionHelper.ReflectGetField<RoomHandler>(typeof(ChallengeShrineController), "m_parentRoom", controller);
            //room = user.CurrentRoom;



            //user.CurrentRoom.RegisterInteractable(obj.GetComponent<IPlayerInteractable>());
            /*
            int PlaceableInt = 11;
            string RoomName = "blacksmith_testroom";
            foreach (DungeonFlow flows in sewerDungeon.PatternSettings.flows)
            {
                foreach (DungeonFlowNode node in flows.AllNodes)
                {
                    if (node.overrideExactRoom != null)
                    {
                        if (node.overrideExactRoom.name.ToLower().StartsWith(RoomName))
                        {
                            ETGModConsole.Log("Found via node.overrideExactRoom");
                            obj = node.overrideExactRoom.placedObjects[PlaceableInt].nonenemyBehaviour.gameObject;

                        }
                    }
                }
            }

            foreach (WeightedRoom wRoom in sewerDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
            {
                if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                {
                    if (wRoom.room.name.ToLower().StartsWith(RoomName))
                    {
                        ETGModConsole.Log("Found via PatternSettings");
                        obj = wRoom.room.placedObjects[PlaceableInt].nonenemyBehaviour.gameObject;
                    }
                }
            }
            */
            /*
            if (obj != null)
             
            {
                GameObject obj1 = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, user.CurrentRoom, GameManager.Instance.PrimaryPlayer.CurrentRoom.area.UnitCenter.ToIntVector2(), false);
                obj1.transform.position = user.transform.position;

                Component[] componentsInChildren = obj1.GetComponentsInChildren(typeof(IPlayerInteractable));
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    IPlayerInteractable placeConfigurable = componentsInChildren[i] as IPlayerInteractable;
                    if (placeConfigurable != null)
                    {
                        user.CurrentRoom.RegisterInteractable(placeConfigurable);
                    }
                }

                //UnityEngine.Object.Instantiate<GameObj.poect>(obj, user.specRigidbody.UnitCenter, Quaternion.identity);
            }
            */
        }
    }
}



