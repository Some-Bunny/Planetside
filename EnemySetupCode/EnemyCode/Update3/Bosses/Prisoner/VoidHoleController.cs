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
        public void Start()
        {
            MeshRenderer rend = base.gameObject.GetComponentInChildren<MeshRenderer>();
            rend.allowOcclusionWhenDynamic = true;
            base.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 90f);
            base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
        }
        public void Update()
        {
            if (actorToFollow != null)
            {
                base.gameObject.transform.position = actorToFollow.sprite.WorldCenter;
            }

            if (CanHurt == true)
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (Vector2.Distance(player.sprite.WorldCenter, actorToFollow != null ? actorToFollow.sprite.WorldCenter : trueCenter) > Radius)
                    {   
                        if (PlanetsideReflectionHelper.ReflectGetField<bool>(typeof(HealthHaver), "vulnerable", player.healthHaver) == true && player.healthHaver.spriteAnimator.QueryInvulnerabilityFrame())
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
                        }
                        player.healthHaver.ApplyDamage(0.5f, player.transform.position, "Eye Of The Deep", CoreDamageTypes.Void, DamageCategory.Normal, true, null, true);
                    }
                }
            }
        }


      


        public void ChangeHoleSize(float radius)
        {
            if (base.gameObject == null) { ETGModConsole.Log("object is NULL"); }
           // if (base.gameObject.GetComponent<MeshRenderer>() == null) { ETGModConsole.Log("MeshRenderer is NULL"); }
            foreach (Component a in base.gameObject.GetComponentsInChildren(typeof(Component)))
            {
                if (a)
                {
                    //ETGModConsole.Log(a.name ?? "NULL");
                    //ETGModConsole.Log(a.GetType().ToString() ?? "NULL TYPE");

                }
            }

            //Vector2.Distance(new Vector2(0,0), new Vector2(base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[0].GetFloat("_HoleSize")))

            if (base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader == null) { ETGModConsole.Log("sharedMaterial is NULL"); }
            if (base.gameObject.GetComponentInChildren<MeshRenderer>().materials[0].shader == null) { ETGModConsole.Log("materials is NULL"); }
            if (base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[0].shader == null) { ETGModConsole.Log("sharedMaterials is NULL"); }

            base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.SetFloat("_HoleSize", radius);
            base.gameObject.GetComponentInChildren<MeshRenderer>().materials[0].SetFloat("_HoleSize", radius);
            base.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials[0].SetFloat("_HoleSize", radius);

        }

        public Material voidHoleMaterial;
        public bool CanHurt;
        public Vector2 trueCenter;
        public AIActor actorToFollow;

        public float Radius;
    }
}
