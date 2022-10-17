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
using static Planetside.LightningController;


namespace Planetside
{

    public class LightningController : MonoBehaviour
    {

        public static void Init()
        {
            AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
            var lightningReticlelocal = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

            LightningReticle = UnityEngine.Object.Instantiate<GameObject>(lightningReticlelocal);
            LightningReticle.gameObject.SetActive(false);

            FakePrefab.MarkAsFakePrefab(LightningReticle);
            DontDestroyOnLoad(LightningReticle);

            var component2 = LightningReticle.GetComponent<tk2dTiledSprite>();
            component2.renderer.gameObject.layer = 23;

            component2.usesOverrideMaterial = true;
            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);

            component2.sprite.renderer.material.SetColor("_OverrideColor", new Color(1.4f, 1.7f, 1.7f));
            component2.sprite.renderer.material.SetColor("_EmissiveColor", new Color(1.4f, 1.7f, 1.7f));

            ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = 1;
            yes1.shadowTimeDelay = 0.1f;
            yes1.dashColor = new Color(0.9f, 1, 1).WithAlpha(3.33f);
            yes1.overrideHeight = 23;

            bundle = null;
        }

        public static GameObject LightningReticle;

        public void GenerateLightning(Vector3 startPosition, Vector3 impactPosition)
        {
            StartPosition = startPosition;
            ImpactPosition = impactPosition;
            Nodes = GenerateNodes();
            GameManager.Instance.StartCoroutine(StartLightning());
        }

        public IEnumerator StartLightning()
        {
            if (DoesDelay() == true)
            {
                if (OnPreDelay != null) { OnPreDelay(ImpactPosition); }
                float elapsed = 0;
                while (elapsed < LightningPreDelay)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is LightningMajorNode major)
                {
                    float elapsed = 0;
                    float elapsedTwo = 0;

                    while (elapsed < LightningMajorNodeDelay)
                    {
                        elapsed += BraveTime.DeltaTime;
                        yield return null;
                    }

                    for (int e = 0; e < major.minorNodes.Count - 1; e++)
                    {
                        elapsedTwo = 0;
                        while (elapsed < LightningMinorNodeDelay)
                        {
                            elapsedTwo += BraveTime.DeltaTime;
                            yield return null;
                        }
                        GameObject vfx = GenerateLine(major.minorNodes[e].position, major.minorNodes[e + 1].position);
                        UnityEngine.Object.Destroy(vfx, 0.25f);
                    }
                    if (UnityEngine.Random.value < MajorNodeSplitoffChance)
                    {
                        for (int e = 0; e < major.branchNodes.Count - 1; e++)
                        {
                            GameObject vfx = GenerateLine(major.branchNodes[e].position, major.branchNodes[e + 1].position);
                            UnityEngine.Object.Destroy(vfx, LinePieceLifetime);
                        }
                    }
                }               
            }
           
            if (OnPostStrike != null){OnPostStrike(ImpactPosition); }

            Destroy(this, 1);
            yield break;
        }

        public GameObject GenerateLine(Vector2 startPosition, Vector2 endPosition)
        {
            GameObject gameObject = SpawnManager.SpawnVFX(LightningReticle, false);
            gameObject.name = "Lightning_Piece";
            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.gameObject.transform.position = startPosition;
            component2.renderer.gameObject.layer = 23;

            component2.transform.localRotation = Quaternion.Euler(0f, 0f, (endPosition - startPosition).ToAngle());
            component2.dimensions = new Vector2((Vector2.Distance(startPosition, endPosition) * 16), Thickness);

            ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = AfterimageLifetime;

            return gameObject;
        }



        public List<LightningNode> GenerateNodes()
        {
            List<LightningNode> List = new List<LightningNode>();
            List.Add(new LightningMajorNode() { position = StartPosition }); 
            float Dist = (Vector3.Distance(ImpactPosition, StartPosition)) / MajorNodesCount;
            Dist *= PreFinalLengthMultiplier;
            for (int i = 1; i < MajorNodesCount + 1; i++)
            {

                Vector2 createdPosition = List[i - 1].position;

                float Offset = BraveUtility.RandomBool() == true ? UnityEngine.Random.Range(-MajorNodeMaxAngleSpacing, -MajorNodeMinAngleSpacing) : UnityEngine.Random.Range(MajorNodeMinAngleSpacing, MajorNodeMaxAngleSpacing);
                float Angle = (ImpactPosition- createdPosition).ToAngle();
                Angle += Offset;

                List.Add(new LightningMajorNode()
                { 
                    position = (createdPosition + MathToolbox.GetUnitOnCircle(Angle, Dist)),
                    minorNodes = GenerateMinorNodes(createdPosition, createdPosition + MathToolbox.GetUnitOnCircle(Angle, Dist), UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax + 1), MinorNodeMaxAngleSpacing, MinorNodeMaxAngleSpacing),
                    branchNodes = GenerateMinorNodes(createdPosition + MathToolbox.GetUnitOnCircle(Angle, Dist), createdPosition + MathToolbox.GetUnitOnCircle(Angle, Dist) + MathToolbox.GetUnitOnCircle(Angle, UnityEngine.Random.Range(RadiusBranchMin, RadiusBranchMax)), UnityEngine.Random.Range(MinorBranchNodesMin, MinorBranchNodesMin + 1), MinorBranchNodeMinAngleSpacing, MinorBranchNodeMaxAngleSpacing),//, UnityEngine.Random.Range(RadiusBranchMin, RadiusBranchMax), Angle)


                });           
            }      
            List.Add(new LightningMajorNode() 
            {
            position = ImpactPosition,
            minorNodes = GenerateMinorNodes(List[List.Count-1].position, ImpactPosition, UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax + 1), MinorNodeMaxAngleSpacing, MinorNodeMaxAngleSpacing)
            });
            return List;
        }


        public List<LightningNode> GenerateMinorNodes(Vector2 startPos, Vector2 endPos, float NodesToGenerate, float offsetLower, float offsetHigher)
        {
            float minorNodesTogenerate = NodesToGenerate;//UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax+1);
            List<LightningNode> List = new List<LightningNode>();
            List.Add(new LightningNode() { position = startPos });
            float Dist = (Vector3.Distance(endPos, startPos)) / minorNodesTogenerate;
            for (int i = 1; i < minorNodesTogenerate + 1; i++)
            {
                Vector2 createdPosition = List[i - 1].position;

                float Offset = BraveUtility.RandomBool() == true ? UnityEngine.Random.Range(-offsetHigher, -offsetLower) : UnityEngine.Random.Range(offsetLower, offsetHigher);
                float Angle = (endPos - createdPosition).ToAngle();
                Angle += Offset;
                List.Add(new LightningNode()
                {
                    position = (createdPosition + MathToolbox.GetUnitOnCircle(Angle, Dist)),
                });

            }
            List.Add(new LightningNode() { position = endPos });
            return List;
        }
      
        private bool DoesDelay()
        {return LightningPreDelay > 0; }


        public float LightningPreDelay = 0f;


        //Major Node Data
        public int MajorNodesCount = 2;
        public float MajorNodeMinAngleSpacing = 20f;
        public float MajorNodeMaxAngleSpacing = 45f;

        //Minor Node Data
        public int MinorNodesMin = 1;
        public int MinorNodesMax = 2;
        public float MinorNodeMinAngleSpacing = 7f;
        public float MinorNodeMaxAngleSpacing = 15f;

        //Minor Branch Node Data
        public int MinorBranchNodesMin = 3;
        public int MinorBranchNodesMax = 4;
        public float MinorBranchNodeMinAngleSpacing = 6f;
        public float MinorBranchNodeMaxAngleSpacing = 14f;
        public float RadiusBranchMin = 2f;
        public float RadiusBranchMax = 4f;
        public float MajorNodeSplitoffChance = 0.5f;

        public float LightningMajorNodeDelay = 0.01f;
        public float LightningMinorNodeDelay = 0.01f;

        public float LinePieceLifetime= 0.20f;
        public float AfterimageLifetime = 0.75f;


        public float PreFinalLengthMultiplier = 0.85f;

        public float Thickness = 1;
        //public float MinimumThickness = 1;

        public Vector2 ImpactPosition;
        public Vector2 StartPosition;

        public Action<Vector2> OnPostStrike;
        public Action<Vector2> OnPreDelay;


        public List<LightningNode> Nodes = new List<LightningNode>();
           
        public class LightningNode
        { public Vector2 position;
        }
        public class LightningMajorNode : LightningNode
        {
            public List<LightningNode> minorNodes = new List<LightningNode>();
            public List<LightningNode> branchNodes = new List<LightningNode>();
        }
    }




    public class LightningMaker : PlayerItem
    {
        public static void Init()
        {
            string itemName = "LightningMaker";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            LightningMaker testActive = obj.AddComponent<LightningMaker>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Lightnings";
            string longDesc = "Makes Lightning.";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 0.25f);
            testActive.consumable = false;
            ItemBuilder.AddPassiveStatModifier(testActive, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;

            lightObject = new GameObject();
            FakePrefab.MarkAsFakePrefab(lightObject);
            DontDestroyOnLoad(lightObject);

            AdditionalBraveLight braveLight = lightObject.AddComponent<AdditionalBraveLight>();
            braveLight.LightColor = Color.white;
            braveLight.LightIntensity = 30f;
            braveLight.LightRadius = 6f;


        }

        public static GameObject lightObject;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        protected override void DoEffect(PlayerController user)
        {
            LightningController c = new LightningController();
            c.MajorNodesCount = UnityEngine.Random.Range(3, 6);
            c.OnPostStrike += (obj) =>
            {
                AkSoundEngine.PostEvent("Play_Lightning", user.gameObject);

                var lightObj = UnityEngine.Object.Instantiate(lightObject);
                lightObj.transform.position = obj;
                Destroy(lightObj, 0.25f);
                Exploder.Explode(obj,StaticExplosionDatas.genericLargeExplosion,obj);
            };
            c.LightningPreDelay = 2.5f;
            c.OnPreDelay += (obj) =>
            {
                GameManager.Instance.StartCoroutine(Wavy(obj));
            };
            c.GenerateLightning(user.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-7, 7), 16),user.sprite.WorldCenter);
        }

        public IEnumerator Wavy(Vector2 pos)
        {
            float elapsed = 0;
            var lightObj = UnityEngine.Object.Instantiate(lightObject);
            AdditionalBraveLight braveLight = lightObj.GetComponent<AdditionalBraveLight>();
            lightObj.transform.position = pos;

            braveLight.LightIntensity = 0;
            braveLight.LightRadius = 0;
            while (elapsed < 2.5f)
            {
                elapsed += BraveTime.DeltaTime;
                float t = Mathf.Min((elapsed / 2.5f), 1);
                braveLight.LightIntensity = Mathf.Lerp(0, 10, t);
                braveLight.LightRadius = Mathf.Lerp(0, 4, t);
                GlobalSparksDoer.DoSingleParticle(pos + MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0, Mathf.Lerp(0, 3, t))), Vector3.up * 3, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                yield return null;
            }
            Destroy(lightObj);
            yield break;
        }
    }
}



