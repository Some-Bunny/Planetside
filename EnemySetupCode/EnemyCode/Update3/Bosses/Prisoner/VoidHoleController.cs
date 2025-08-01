using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using Random = System.Random;
using Pathfinding;
namespace Planetside
{
    public class VoidHoleController : MonoBehaviour
    {
        public static VoidHoleController SpawnVoidHole(Vector2 Position, Vector3 scale)
        {
            GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Amogus"));
            MeshRenderer rend = partObj.GetComponentInChildren<MeshRenderer>();
            VoidHoleController voidHoleController = partObj.AddComponent<VoidHoleController>();
            voidHoleController.gameObject.transform.position = Position;
            voidHoleController.gameObject.transform.localScale = scale;
            return voidHoleController;
        }


        private MeshRenderer meshRenderer;
        public void Awake()
        {
            meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.allowOcclusionWhenDynamic = true;
            meshRenderer.gameObject.name = "VoidHole";
        }

        public void InitializeHole(GameObject followObject, float Tiling, float Rot, float Spin, Vector2 Offset)
        {
            FollowedObject = followObject;
            _Offset = Offset;
            meshRenderer.allowOcclusionWhenDynamic = true;
            base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            meshRenderer.material.SetVector("_Screen", new Vector4()
            {
                x = Screen.currentResolution.width / 1000,
                y = Screen.currentResolution.height / 1000,
                z = 0,
                w = 0
            });
            ChangeHoleTiling(Tiling);
            ChangeHoleRotation(Rot);
            ChangeHolePanning(Spin);
        }
        public Vector3 LockedVector = new Vector3(90, 0, 180);
        public void Update()
        {
            if (FollowedObject != null)
            {
                base.gameObject.transform.position = PositionOfObject;
            }
            base.gameObject.transform.rotation = Quaternion.Euler(LockedVector);

            if (CanHurt == true)
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (Vector2.Distance(player.sprite.WorldCenter, PositionOfObject) >= Radius)
                    {   
                        if (player.healthHaver.vulnerable == true && player.healthHaver.IsDead == false)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                            GameObject vfx = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX);
                            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
                            component.PlaceAtPositionByAnchor(player.transform.position + new Vector3(0.375f, 0.375f), tk2dBaseSprite.Anchor.MiddleCenter);
                            component.HeightOffGround = 35f;
                            component.UpdateZDepth();
                            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                            if (component2 != null)
                            {
                                component2.ignoreTimeScale = true;
                                component2.AlwaysIgnoreTimeScale = true;
                                component2.AnimateDuringBossIntros = true;
                                component2.alwaysUpdateOffscreen = true;
                                component2.playAutomatically = true;
                            }
                            player.healthHaver.ApplyDamage(0.5f, player.transform.position, "Void Fog", CoreDamageTypes.Void, DamageCategory.Unstoppable, true, null, true);
                        }
                    }
                }
            }
        }

        public void ChangeHoleTiling(float Tiles)
        {
            meshRenderer.sharedMaterial.SetFloat("_Tiles", Tiles);
        }
        public void ChangeHolePanning(float PanSpeed)
        {
            meshRenderer.sharedMaterial.SetFloat("_Spinspeed", PanSpeed);
        }
        public void ChangeHoleRotation(float RotationSpeed)
        {
            meshRenderer.sharedMaterial.SetFloat("_RotationSpeed", RotationSpeed);
        }




        public void ChangeHoleSize(float radius)
        {
            if (base.gameObject == null) { ETGModConsole.Log("object is NULL");return; }
            //Vector2.Distance(new Vector2(0,0), new Vector2(base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[0].GetFloat("_HoleSize")))

            //if (meshRenderer.sharedMaterial.shader == null) { ETGModConsole.Log("sharedMaterial is NULL"); }
            //if (meshRenderer.materials[0].shader == null) { ETGModConsole.Log("materials is NULL"); }
            //if (meshRenderer.sharedMaterials[0].shader == null) { ETGModConsole.Log("sharedMaterials is NULL"); }
            Radius = radius * 100;
            meshRenderer.sharedMaterial.SetFloat("_HoleSize", radius);
            meshRenderer.materials[0].SetFloat("_HoleSize", radius);
            meshRenderer.sharedMaterials[0].SetFloat("_HoleSize", radius);

        }

        public void SetScale(Vector3 newScale)
        {
            base.gameObject.transform.localScale = newScale * 0.125f;
        }

        public Vector2 PositionOfObject
        {
            get
            {
                if (FollowedObject == null) { return this.transform.position; }
                return FollowedObject.transform.position + _Offset;
            }
        }

        public bool CanHurt;
        public GameObject FollowedObject;
        public Vector3 _Offset = Vector3.zero;
        public float Radius;
    }
}
