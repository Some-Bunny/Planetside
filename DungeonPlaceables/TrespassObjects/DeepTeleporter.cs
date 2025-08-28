using ItemAPI;
using UnityEngine;
using Planetside;
using System.Collections;
using ChallengeAPI;
using HutongGames.PlayMaker.Actions;

namespace Planetside.DungeonPlaceables.TrespassObjects
{
    public class Deep_Teleporter
    {
        public class SetScale : MonoBehaviour 
        {
            public MeshRenderer meshRenderer;
            public void Start()
            {
                this.StartCoroutine(Pain());
            }

            private IEnumerator Pain()
            {
                float e = 0;
                while (e < 1)
                {
                    meshRenderer.transform.rotation = Quaternion.Euler(270, 0, 0);

                    e += Time.deltaTime * 3;
                    meshRenderer.material.SetFloat("_AlphaMult", Mathf.Lerp(0, 1.6f, Easing.DoLerpT(e, Easing.Ease.OUT)));
                    yield return null;
                }
                yield break;
            }
        }


        public static void CreateTeleporter()
        {
            var teleporter = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("DeepTeleporter");
            UnityEngine.Object.DontDestroyOnLoad(teleporter);
            TeleporterController existingTeleporterController = ResourceManager.LoadAssetBundle("brave_resources_001").LoadAsset<GameObject>("Teleporter_Gungeon_01").GetComponentInChildren<TeleporterController>();
            teleporter.layer = 20;

            var sprite = teleporter.gameObject.AddComponent<tk2dSprite>();
            sprite.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "deep_teleporter_001");

            var animator = teleporter.gameObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = PlanetsideModule.SpriteCollectionAssets.LoadAsset<GameObject>("DeepTeleporterAnimation").GetComponent<tk2dSpriteAnimation>();
            //animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("miniteleporter_place");

            TeleporterController teleporterController = teleporter.GetOrAddComponent<TeleporterController>();

            teleporterController.sprite = sprite;
            teleporterController.spriteAnimator = animator;
            teleporterController.sprite.usesOverrideMaterial = true;

            Material mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat_.SetFloat("_EmissiveColorPower", 4f);
            mat_.SetFloat("_EmissivePower", 3);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 0.03f);
            mat_.mainTexture = teleporterController.sprite.renderer.material.mainTexture;
            teleporterController.sprite.renderer.material = mat_;


            var icon = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("DeepTeleporterIcon");
            var icon_ = icon.gameObject.AddComponent<tk2dSprite>();
            icon_.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "deep_teleporter_icon_001");

            teleporterController.teleporterIcon = icon_.gameObject;


            var t = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Holder");

            UnityEngine.Object.DontDestroyOnLoad(t);
            t.transform.SetParent(teleporter.transform);
            t.transform.rotation = Quaternion.Euler(0, 0, 0);
            t.SetActive(false);
            var scaler = t.gameObject.AddComponent<SetScale>();

            var t_2 = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("VoidHole"));
            t_2.transform.SetParent(t.transform);
            t_2.transform.localPosition = new Vector3(1.5f, 1.75f, 0);
            t_2.transform.localScale = Vector3.one * 0.333f;

            scaler.meshRenderer = t_2.GetComponent<MeshRenderer>();


            var _ = t.gameObject.AddComponent<tk2dSprite>();
            _.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "deep_teleporter_icon_001");
            var __ = t.gameObject.AddComponent<tk2dSpriteAnimator>();
            _.scale = Vector3.zero;


            teleporterController.portalVFX = __;

            var extant = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("ExtantEffect");
            UnityEngine.Object.DontDestroyOnLoad(extant);

            sprite = extant.gameObject.AddComponent<tk2dSprite>();
            sprite.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "deep_teleporter_decor_001");

            var cool = extant.gameObject.AddComponent<tk2dSpriteAnimator>();
            cool.library = animator.library;
            cool.playAutomatically = true;
            cool.defaultClipId = cool.library.GetClipIdByName("teleporter_sexy");
            cool.transform.SetParent(teleporter.transform);
            cool.gameObject.SetActive(false);

            sprite.usesOverrideMaterial = true;
            mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat_.SetFloat("_EmissiveColorPower", 4f);
            mat_.SetFloat("_EmissivePower", 3);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 0.03f);
            mat_.mainTexture = sprite.renderer.material.mainTexture;
            sprite.renderer.material = mat_;

            teleporterController.extantActiveVFX = extant;

            //teleporter_sexy

            //GameObject clonedextantActiveVFX = FakePrefab.Clone(existingTeleporterController.extantActiveVFX);
            /*
            var VFX = PrefabBuilder.BuildObject("MiniTeleporter_Arrival");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            VFX.transform.SetParent(teleporter.transform);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("smallteleporter_glow_001"));
            tk2d.renderer.enabled = false;

            tk2d.usesOverrideMaterial = true;
            tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

            ExpandReticleRiserEffect rRE = VFX.gameObject.AddComponent<ExpandReticleRiserEffect>();
            rRE.RiserHeight = 0.75f;
            rRE.RiseTime = 2;
            rRE.NumRisers = 3;
            rRE.UpdateSpriteDefinitions = true;
            rRE.CurrentSpriteName = "smallteleporter_glow_001";
            */



            //

            //teleportArrive_
            /*
            var VFX_Arrival = PrefabBuilder.BuildObject("MiniTeleporter_ArrivalProper");
            FakePrefab.MarkAsFakePrefab(VFX_Arrival);
            UnityEngine.Object.DontDestroyOnLoad(VFX_Arrival);
            VFX_Arrival.SetActive(false);
            VFX_Arrival.transform.SetParent(teleporter.transform);
            var tk2d_arrival = VFX_Arrival.AddComponent<tk2dSprite>();
            tk2d_arrival.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d_arrival.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("smallteleporter_vfxarrive_001"));

            tk2d_arrival.usesOverrideMaterial = true;
            tk2d_arrival.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

            var animator_arrival = VFX_Arrival.AddComponent<tk2dSpriteAnimator>();
            animator_arrival.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator_arrival.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("teleportArrive_");
            animator_arrival.playAutomatically = true;
            var killer = VFX_Arrival.AddComponent<SpriteAnimatorKiller>();
            killer.animator = animator_arrival;
            killer.delayDestructionTime = 0.6363f;
            killer.fadeTime = 0;

            
            mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(162, 233, 195, 255));
            mat_.SetFloat("_EmissiveColorPower", 15f);
            mat_.SetFloat("_EmissivePower", 10);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 1f);
            mat_.mainTexture = tk2d_arrival.sprite.renderer.material.mainTexture;
            tk2d_arrival.sprite.renderer.material = mat_;
            */

            var f = FakePrefab.Clone(existingTeleporterController.teleportArrivalVFX);
            f.transform.localScale *= 0.5f;
            teleporterController.teleportArrivalVFX = f;

            GameObject clonedteleportDepartureVFX = FakePrefab.Clone(existingTeleporterController.teleportDepartureVFX);
            teleporterController.teleportDepartureVFX = clonedteleportDepartureVFX;

            //GameObject clonedportalVFX = FakePrefab.Clone(existingTeleporterController.portalVFX.gameObject);
            //teleporterController.portalVFX = clonedportalVFX.GetComponent<tk2dSpriteAnimator>();


            var abbeyAnim = animator.library.GetClipByName("teleport_pad_activate");

            abbeyAnim.frames[17].triggerEvent = true;
            abbeyAnim.frames[17].eventAudio = "Play_OBJ_teleport_activate_01";

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("DeepTeleporter", teleporter);
            GungeonAPI.StaticReferences.StoredRoomObjects.Add("DeepTeleporter", teleporter);
            //return teleporter;
        }
    }
}
