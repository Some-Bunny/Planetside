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
using System.Collections;
using Newtonsoft.Json.Linq;


namespace Planetside
{

    public class RealityTearController : MonoBehaviour
    {
        public void Start()
        {
            AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_Tear", base.gameObject);
            GameManager.Instance.StartCoroutine(PortalDoer(mesh));
        }


        private IEnumerator PortalDoer(MeshRenderer portal)
        {
            float elapsed = 0f;
            AIActor aiactor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(EnemyToSpawn), this.gameObject.transform.PositionVector2().ToIntVector2(), this.gameObject.transform.position.GetAbsoluteRoom(), true, AIActor.AwakenAnimationType.Awaken, true);
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
            aiactor.reinforceType = AIActor.ReinforceType.SkipVfx;
            aiactor.HandleReinforcementFallIntoRoom(-1f);
            aiactor.ApplyEffect(DebuffLibrary.InfectedEnemyEffect);
            bool ti = false;
            while (elapsed < PortalLifeTime)
            {
                if (elapsed > 1 && ti == false)
                {
                    ti = !ti;
                    AkSoundEngine.PostEvent("Stop_Tear", base.gameObject);
                }
                if (GameManager.Instance.IsPaused== true) { yield return null; }
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / PortalLifeTime;
               
                if (portal.gameObject == null) { yield break; }

                Exploder.DoDistortionWave(portal.transform.PositionVector2(), 0.5f, 0.25f, 3, 0.0625f);

                GlobalSparksDoer.DoSingleParticle(portal.transform.PositionVector2()+ MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0, PortalSize * 10f)), Vector3.up * 20, null, null, null, particleType);

                float throne1 = Mathf.Sin(t * (Mathf.PI));
                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, PortalSize, throne1));
                portal.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, throne1));        
                yield return null;
            }
            Destroy(portal.gameObject);

            yield break;
        }


        public GlobalSparksDoer.SparksType particleType;
        public string EnemyToSpawn;
        public MeshRenderer mesh;
        public float PortalLifeTime = 2;
        public float PortalSize = 0.3f;
    }
    public class TearHolderController : MonoBehaviour
    {
        public MajorBreakable self;
        public RoomHandler parent_room;
        public MeshRenderer Pool;
        public GameObject Orb;

        private MeshRenderer OrbMesh;

        bool T = false;
        public RoomEventTriggerCondition Trigger;

        
        public RealityTearData data;
        public bool IsRandom = true;
        public RealityTearData.Floor assigned;
        public string SetGUID = string.Empty;


        public void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();

            if (IsRandom == true) { data = BraveUtility.RandomElement<RealityTearData>(TearInReality.realityTearDatas); }
            else
            {
                foreach (RealityTearData datum in TearInReality.realityTearDatas)
                {
                    if (datum.assignedFloor == assigned) { data = datum; }
                }
            }
            if (SetGUID != string.Empty)
            {
                data.SetGUID = SetGUID;
            }

            Trigger = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;

            parent_room = self.transform.position.GetAbsoluteRoom();

            Orb = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("RealityTear"));
            Orb.transform.position = self.sprite.WorldTopCenter;
            Orb.transform.localRotation = Quaternion.Euler(90, 90, 90f);
            Orb.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            Orb.transform.localScale = Vector3.one * 2;

            OrbMesh = Orb.GetComponentInChildren<MeshRenderer>();
            OrbMesh.allowOcclusionWhenDynamic = true;
            OrbMesh.material.SetTexture("_Floor_Tex", data.FloorTexture);
            OrbMesh.material.SetFloat("_OutlineWidth", 3f);//This doesnt control the outline lol

            OrbMesh.material.SetTexture("_Nebula", StaticTextures.NebulaTexture);
            if (parent_room != null)
            {
                Actions.OnReinforcementWaveTriggered += DoWacky;
            }
        }
        public void DoWacky(RoomHandler self, RoomEventTriggerCondition roomEventTriggerAction)
        {
            if (self == parent_room && T == false && roomEventTriggerAction == Trigger)
            {
                T = true;
                this.StartCoroutine(DoFuckening());
            }
        }

        public IEnumerator DoFuckening()
        {

            float elaWait = 0f;
            float duraWait = UnityEngine.Random.Range(0.1f, 2f);
            while (elaWait < duraWait)
            {
                elaWait += BraveTime.DeltaTime;         
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_PrisonerCharge", base.gameObject);
            elaWait = 0;
            duraWait = 0.5f;
            while (elaWait < duraWait)
            {
                elaWait += BraveTime.DeltaTime;
                float t = elaWait / duraWait;
                if (Orb.transform.localScale == null) { yield break; }
                if (Orb.transform.localScale != null)
                {
                    Orb.transform.localScale = Vector3.Lerp(Vector3.one *2, Vector3.zero, Mathf.SmoothStep(0, 1, t));
                    //Orb.transform.position = Vector3.Lerp(self.sprite.WorldTopCenter, self.sprite.WorldBottomCenter, MathToolbox.SinLerpTValue(t));
                }
                yield return null;
            }
            GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, self.sprite.WorldBottomCenter, Quaternion.Euler(0f, 0f, 0f));
            portalObj.layer = this.gameObject.layer;
            portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();


            RealityTearController realityTearController = portalObj.AddComponent<RealityTearController>();
            realityTearController.EnemyToSpawn = SetGUID != string.Empty ? SetGUID : data.SelectByWeight(null);
            realityTearController.particleType = data.particleType;
            realityTearController.mesh = mesh;

            mesh.material.SetTexture("_PortalTex", data.SecondaryFloorTexture);
            mesh.material.SetTextureScale("_PortalTex", new Vector2(0,0));

            mesh.material.SetVector("_Magnitudes", new Vector4(0.125f, 0.125f, 0.1f, 0.1f));

            Destroy(Orb);
            fuck.DoReverseDistortionWaveLocal(this.gameObject.transform.PositionVector2(), 1, 0.25f, 6, 1);
            self.spriteAnimator.PlayAndDestroyObject("break");

            //Destroy(this.gameObject);
            yield break;
        }
    }



    public class TearInReality
    {
        public static void Init()
        {

            realityTearDatas = new List<RealityTearData>()
            {
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.KEEP,
                    FloorTexture = StaticTextures.Keep_1_Texture,
                    SecondaryFloorTexture =  StaticTextures.Keep_2_Texture,
                    FloorWaves = new List<SingularEnemy>()
                    {
                        new SingularEnemy() { Enemy = new List<string>() { "ec8ea75b557d4e7b8ceeaacdf6f8238c", "463d16121f884984abe759de38418e48" }, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "01972dee89fc4404a5c408d50007dad5", "88b6b6a93d4b4234a67844ef4728382c", "db35531e66ce41cbb81d507a34366dfe" }},
                        new SingularEnemy() { Enemy = new List<string>() { "128db2f0781141bcb505d8f00f9e4d47", "b54d89f9e802455cbb2b8a96a31e8259" }},
                        new SingularEnemy() { Enemy = new List<string>() { "42be66373a3d4d89b91a35c9ff8adfec", "042edb1dfb614dc385d5ad1b010f2ee3" }},
                    },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.SEWER,
                    FloorTexture = StaticTextures.Sewer_1_Texture,
                    SecondaryFloorTexture =  StaticTextures.Sewer_2_Texture,
                    particleType = GlobalSparksDoer.SparksType.STRAIGHT_UP_GREEN_FIRE,

                    FloorWaves = new List<SingularEnemy>()
                    {
                        new SingularEnemy() { Enemy = new List<string>() { "7f665bd7151347e298e4d366f8818284"}},
                        new SingularEnemy() { Enemy = new List<string>() { "d4a9836f8ab14f3fadd0f597438b1f1f"}},
                        new SingularEnemy() { Enemy = new List<string>() { "e61cab252cfb435db9172adc96ded75f", "fe3fe59d867347839824d5d9ae87f244", "116d09c26e624bca8cca09fc69c714b3"}},
                        new SingularEnemy() { Enemy = new List<string>() { "116d09c26e624bca8cca09fc69c714b3"}},
                        new SingularEnemy() { Enemy = new List<string>() { "f905765488874846b7ff257ff81d6d0c", "9b4fb8a2a60a457f90dcf285d34143ac"}},
                    },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.PROPER,
                    FloorTexture = StaticTextures.Proper_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Proper_2_Texture,
                    FloorWaves = new List<SingularEnemy>()
                    {
                        new SingularEnemy() { Enemy = new List<string>() { "cd4a4b7f612a4ba9a720b9f97c52f38c"} , Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "d4a9836f8ab14f3fadd0f597438b1f1f"}},
                        new SingularEnemy() { Enemy = new List<string>() { "e61cab252cfb435db9172adc96ded75f", "fe3fe59d867347839824d5d9ae87f244", "116d09c26e624bca8cca09fc69c714b3"}},
                        new SingularEnemy() { Enemy = new List<string>() { "116d09c26e624bca8cca09fc69c714b3"}},
                        new SingularEnemy() { Enemy = new List<string>() { "1a4872dafdb34fd29fe8ac90bd2cea67"}},
                        new SingularEnemy() { Enemy = new List<string>() { "864ea5a6a9324efc95a0dd2407f42810", "c5b11bfc065d417b9c4d03a5e385fe2c"}, Weight = 0.75f},
                    },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.ABBEY,
                    FloorTexture = StaticTextures.Abbey_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Abbey_2_Texture,
                    particleType = GlobalSparksDoer.SparksType.BLOODY_BLOOD,

                     FloorWaves = new List<SingularEnemy>()
                     {
                        new SingularEnemy() { Enemy = new List<string>() { "8bb5578fba374e8aae8e10b754e61d62"}},
                        new SingularEnemy() { Enemy = new List<string>() { "062b9b64371e46e195de17b6f10e47c8"}, Weight = 0.66f },
                        new SingularEnemy() { Enemy = new List<string>() { "4db03291a12144d69fe940d5a01de376"}},
                        new SingularEnemy() { Enemy = new List<string>() { ArchGunjurer.guid }},
                        new SingularEnemy() { Enemy = new List<string>() { Glockulus.guid }},
                        new SingularEnemy() { Enemy = new List<string>() { Barretina.guid }, Weight = 0.66f},
                     },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.MINES,
                    FloorTexture = StaticTextures.Mines_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Mines_2_Texture,
                     FloorWaves = new List<SingularEnemy>()
                     {
                        new SingularEnemy() { Enemy = new List<string>() { "e61cab252cfb435db9172adc96ded75f", Cursebulon.guid}},
                        new SingularEnemy() { Enemy = new List<string>() { "9d50684ce2c044e880878e86dbada919", Coallet.guid}},
                        new SingularEnemy() { Enemy = new List<string>() { "98ea2fe181ab4323ab6e9981955a9bca"}, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "98ea2fe181ab4323ab6e9981955a9bca", "3e98ccecf7334ff2800188c417e67c15"}, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "af84951206324e349e1f13f9b7b60c1a"}},

                        new SingularEnemy() { Enemy = new List<string>() { "eed5addcc15148179f300cc0d9ee7f94"}, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "f905765488874846b7ff257ff81d6d0c", "9b4fb8a2a60a457f90dcf285d34143ac"}},


                     },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.HOLLOW,
                    FloorTexture = StaticTextures.Hollow_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Hollow_2_Texture,
                     FloorWaves = new List<SingularEnemy>()
                     {
                        new SingularEnemy() { Enemy = new List<string>() { "022d7c822bc146b58fe3b0287568aaa2"}},
                        new SingularEnemy() { Enemy = new List<string>() { "43426a2e39584871b287ac31df04b544"}},
                        new SingularEnemy() { Enemy = new List<string>() { "3f6d6b0c4a7c4690807435c7b37c35a5", "21dd14e5ca2a4a388adab5b11b69a1e1"}, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { "5288e86d20184fa69c91ceb642d31474"}, Weight = 0.75f},
                        new SingularEnemy() { Enemy = new List<string>() { "95ec774b5a75467a9ab05fa230c0c143", "336190e29e8a4f75ab7486595b700d4a"}},
                     },
                },
                new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.FORGE,
                    FloorTexture = StaticTextures.Forge_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Forge_2_Texture,
                    particleType = GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE,
                     FloorWaves = new List<SingularEnemy>()
                     {
                        new SingularEnemy() { Enemy = new List<string>() { "ccf6d241dad64d989cbcaca2a8477f01", "0b547ac6b6fc4d68876a241a88f5ca6a"}},
                        new SingularEnemy() { Enemy = new List<string>() { "d5a7b95774cd41f080e517bea07bf495"}, Weight = 0.5f},
                        new SingularEnemy() { Enemy = new List<string>() { RevolverSkull.guid}, Weight = 0.75f},
                        new SingularEnemy() { Enemy = new List<string>() { "ffdc8680bdaa487f8f31995539f74265", "d8a445ea4d944cc1b55a40f22821ae69"}},
                        new SingularEnemy() { Enemy = new List<string>() { "1a78cfb776f54641b832e92c44021cf2", "1bd8e49f93614e76b140077ff2e33f2b"}, Weight = 1.25f},
                     },
                },
                 new RealityTearData()
                {
                    assignedFloor = RealityTearData.Floor.HELL,
                    FloorTexture = StaticTextures.Hell_1_Texture,
                    SecondaryFloorTexture = StaticTextures.Hell_2_Texture,
                    particleType = GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE,
                     FloorWaves = new List<SingularEnemy>()
                     {
                        new SingularEnemy() { Enemy = new List<string>() { "5f3abc2d561b4b9c9e72b879c6f10c7e", Wailer.guid}},
                        new SingularEnemy() { Enemy = new List<string>() { "044a9f39712f456597b9762893fbc19c"}, Weight = 0.666f},
                        new SingularEnemy() { Enemy = new List<string>() { "cf27dd464a504a428d87a8b2560ad40a"}},
                        new SingularEnemy() { Enemy = new List<string>() { "b1770e0f1c744d9d887cc16122882b4f"}, Weight = 0.75f},
                     },
                },
            };

            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/PillarOfSalt/";
            string[] idlePaths = new string[]
            {
                defaultPath+"pillarsofsalt.png",
            };
            string[] death = new string[]
            {
                defaultPath+"pillarsofsalt_break_001.png",
                defaultPath+"pillarsofsalt_break_002.png",
                defaultPath+"pillarsofsalt_break_003.png",
                defaultPath+"pillarsofsalt_break_004.png",
                defaultPath+"pillarsofsalt_break_005.png",
                defaultPath+"pillarsofsalt_break_006.png",
                defaultPath+"pillarsofsalt_break_007.png",
                defaultPath+"pillarsofsalt_break_008.png",
                defaultPath+"pillarsofsalt_break_009.png",
                defaultPath+"pillarsofsalt_break_010.png",

            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_big_light", idlePaths, 5, death, 16, 15000, true, 26, 30, 3, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow("Planetside/Resources/DungeonObjects/TrespassObjects/TrespassBigLight/tresspasslightBigShadow.png", "trespassPot_shadow", statue.gameObject.transform, new Vector3(0.125f, -0.25f));

            statue.gameObject.AddComponent<TearHolderController>();
            statue.gameObject.AddComponent<TresspassLightController>();

            statue.DamageReduction = 1000;
            Alexandria.DungeonAPI.StaticReferences.StoredRoomObjects.Add("portal_pillar_random", statue.gameObject);

            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            {
                { statue.gameObject, 0.5f },
            };

            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add("portal_pillar_random", placeable);


            GenerateSeparatePillar(RealityTearData.Floor.ABBEY, "portal_pillar_abbey");
            GenerateSeparatePillar(RealityTearData.Floor.FORGE, "portal_pillar_forge");
            GenerateSeparatePillar(RealityTearData.Floor.HELL, "portal_pillar_hell");
            GenerateSeparatePillar(RealityTearData.Floor.HOLLOW, "portal_pillar_hollow");
            GenerateSeparatePillar(RealityTearData.Floor.KEEP, "portal_pillar_keep");
            GenerateSeparatePillar(RealityTearData.Floor.MINES, "portal_pillar_mines");
            GenerateSeparatePillar(RealityTearData.Floor.PROPER, "portal_pillar_proper");
            GenerateSeparatePillar(RealityTearData.Floor.SEWER, "portal_pillar_sewer");

            Alexandria.DungeonAPI.RoomFactory.OnCustomProperty += OnAction;

        }

        public static GameObject OnAction(string ObjName, GameObject Original, JObject jObject)
        {
            if (ObjName != "portal_pillar_random") { return Original; }
            Original = FakePrefab.Clone(Alexandria.DungeonAPI.StaticReferences.StoredRoomObjects[ObjName]);

            if (Original == null) { ETGModConsole.Log("fuck_a"); }


            var tearHolder = Original.GetComponent<TearHolderController>();
            if (tearHolder == null) { ETGModConsole.Log("fuck_b"); }

            JToken value = null;

            string GUID = jObject.TryGetValue("enemyGUID", out value) ? ((string)value) : "None.";
            if (GUID != "None.")
            {
                tearHolder.SetGUID = GUID;
            }

            string Type_ = jObject.TryGetValue("FloorType", out value) ? ((string)value) : "Any";
            switch (Type_)
            {
                case "Any":
                    tearHolder.IsRandom = true;
                    break;
                case "Keep":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.KEEP;
                    break;
                case "Proper":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.PROPER;
                    break;
                case "Sewer":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.SEWER;
                    break;
                case "Abbey":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.ABBEY;
                    break;
                case "Mines":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.MINES;
                    break;
                case "Hollow":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.HOLLOW;
                    break;
                case "Forge":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.FORGE;
                    break;
                case "Hell":
                    tearHolder.IsRandom = false;
                    tearHolder.assigned = RealityTearData.Floor.HELL;
                    break;
            }
            ETGModConsole.Log(5);

            return Original;
        }

        public static void GenerateSeparatePillar(RealityTearData.Floor f, string name)
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/PillarOfSalt/";
            string[] idlePaths = new string[]
            {
                defaultPath+"pillarsofsalt.png",
            };
            string[] death = new string[]
            {
                defaultPath+"pillarsofsalt_break_001.png",
                defaultPath+"pillarsofsalt_break_002.png",
                defaultPath+"pillarsofsalt_break_003.png",
                defaultPath+"pillarsofsalt_break_004.png",
                defaultPath+"pillarsofsalt_break_005.png",
                defaultPath+"pillarsofsalt_break_006.png",
                defaultPath+"pillarsofsalt_break_007.png",
                defaultPath+"pillarsofsalt_break_008.png",
                defaultPath+"pillarsofsalt_break_009.png",
                defaultPath+"pillarsofsalt_break_010.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_big_light", idlePaths, 5, death, 16, 15000, true, 26, 30, 3, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow("Planetside/Resources/DungeonObjects/TrespassObjects/TrespassBigLight/tresspasslightBigShadow.png", "trespassPot_shadow", statue.transform, new Vector3(0.125f, -0.25f));

            TearHolderController tearHolderController = statue.gameObject.AddComponent<TearHolderController>();
            tearHolderController.IsRandom = false;
            tearHolderController.assigned = f;
       

            statue.gameObject.AddComponent<TresspassLightController>();

            statue.DamageReduction = 1000;
            Dictionary<GameObject, float> dict = new Dictionary<GameObject, float>()
            { { statue.gameObject, 1f }};
            DungeonPlaceable placeable = BreakableAPIToolbox.GenerateDungeonPlaceable(dict);
            StaticReferences.StoredDungeonPlaceables.Add(name, placeable);
        }
        public static List<RealityTearData> realityTearDatas = new List<RealityTearData>();
    }


    


    public class RealityTearData
    {
        public Floor assignedFloor;
        public enum Floor
        {
            KEEP,
            SEWER,
            PROPER,
            ABBEY,
            MINES,
            HOLLOW,
            FORGE,
            HELL
        };
        public GlobalSparksDoer.SparksType particleType = GlobalSparksDoer.SparksType.FLOATY_CHAFF;
        public Texture FloorTexture;
        public Texture SecondaryFloorTexture;
        public List<SingularEnemy> FloorWaves = new List<SingularEnemy>();
        public string SetGUID = string.Empty;

        public string SelectByWeight(System.Random generatorRandom)
        {
            List<SingularEnemy> list = new List<SingularEnemy>();
            float num = 0f;
            for (int i = 0; i < this.FloorWaves.Count; i++)
            {
                SingularEnemy weightedInt = this.FloorWaves[i];
                if (weightedInt.Weight > 0f)
                {
                    list.Add(weightedInt);
                    num += weightedInt.Weight;
                }
            }
            float num2 = ((generatorRandom == null) ? UnityEngine.Random.value : ((float)generatorRandom.NextDouble())) * num;
            float num3 = 0f;
            for (int k = 0; k < list.Count; k++)
            {
                num3 += list[k].Weight;
                if (num3 > num2)
                {
                    return BraveUtility.RandomElement<string>(list[k].Enemy);
                }
            }
            return BraveUtility.RandomElement<string>(list[0].Enemy);
        }
    }
   
    public class SingularEnemy
    {
        public List<string> Enemy = new List<string>();
        public float Weight = 1;
    }
}
