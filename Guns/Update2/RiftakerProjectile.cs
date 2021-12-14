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

namespace Planetside
{
	internal class RiftakerProjectile : BraveBehaviour
	{
		public RiftakerProjectile()
		{
		}

        public void Start()
        {
            //GameObject original;
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            PlayerController playerController = projectile.Owner as PlayerController;
            Projectile component = base.gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                component.sprite.usesOverrideMaterial = true;

                Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat1.mainTexture = component.sprite.renderer.material.mainTexture;
                mat1.SetColor("_EmissiveColor", new Color32(215, 225, 255, 255));
                mat1.SetFloat("_EmissiveColorPower", 15.5f);
                mat1.SetFloat("_EmissivePower", 1000);
                mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                component.OnDestruction += this.Zzap;
            }
                        
        }
        private void Zzap(Projectile projectile)
        {
            if (projectile.Owner as PlayerController)
            {
                Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 0.1f, 0.5f, 3, 0.25f);
                RiftakerProjectile component = projectile.gameObject.GetComponent<RiftakerProjectile>();
                RiftakerProjectile.HoleObject = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>();
                component.synergyobject = RiftakerProjectile.HoleObject.objectToSpawn;
                BlackHoleDoer component2 = this.synergyobject.GetComponent<BlackHoleDoer>();

                this.gameObject1 = UnityEngine.Object.Instantiate<GameObject>(component2.HellSynergyVFX, new Vector3(projectile.transform.position.x, projectile.transform.position.y, projectile.Owner.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
                gameObject1.layer = projectile.Owner.gameObject.layer;
                MeshRenderer component3 = this.gameObject1.GetComponent<MeshRenderer>();
                var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
                component3.material.SetTexture("_PortalTex", texture);
                component3.material.SetFloat("_UVDistCutoff", 0f);
                hole = component3;


                WhyYouGottaMakeThingsSoComplicated why = gameObject1.AddComponent<WhyYouGottaMakeThingsSoComplicated>();
                why.AceCourtBot = component3;
                why.player = projectile.PossibleSourceGun.CurrentOwner as PlayerController;
            }
          
         

        }

        MeshRenderer hole;
        private GameObject synergyobject;
        private static SpawnObjectPlayerItem HoleObject;
        private GameObject gameObject1;
        private new Projectile projectile;
	}
}

namespace Planetside
{
    public class WhyYouGottaMakeThingsSoComplicated : BraveBehaviour
    {
        public WhyYouGottaMakeThingsSoComplicated()
        {
            AceCourtBot = null;
            player = GameManager.Instance.PrimaryPlayer;
        }
        public void Start()
        {
            if (AceCourtBot != null)
            {
                base.StartCoroutine(this.HoldPortalOpen(AceCourtBot, player));
            }
        }
        private IEnumerator HoldPortalOpen(MeshRenderer portal, PlayerController player) // this be closing coroutine
        {
            //0.25 is the size of portal, so noteice it for the Mathf.Lerp and here. maybe put it into a variable?
            portal.material.SetFloat("_UVDistCutoff", 0.30f);
            //yield return new WaitForSeconds(1);//time it waits  before it starts closing
            float elapsed = 0f;
            float duration = 3;//time it takes it to close
            float t = 0f;
            while (elapsed < duration)//idk dodgeroll black magic
            {
                elapsed += BraveTime.DeltaTime;
                t = Mathf.Clamp01(elapsed / 1.25f);
                portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.0f + (elapsed/2), 0f, t));
                float Rad = portal.material.GetFloat("_UVDistCutoff");

                float num = (player.stats.GetStatValue(PlayerStats.StatType.Damage));


                List<AIActor> activeEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                Vector2 centerPosition = AceCourtBot.transform.position;
                if (activeEnemies != null)
                {
                    foreach (AIActor aiactor in activeEnemies)
                    {
                        bool ae = Vector2.Distance(aiactor.CenterPosition, centerPosition) < Rad * 20 && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && player != null;
                        if (ae)
                        {
                            aiactor.healthHaver.ApplyDamage((27f* num) * BraveTime.DeltaTime, Vector2.zero, "fwomp", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                            if (player.PlayerHasActiveSynergy("Event Horizon"))
                            {
                                if (!aiactor.healthHaver.IsBoss)
                                {
                                    aiactor.knockbackDoer.weight = 150f;
                                    Vector2 a = aiactor.transform.position - portal.transform.position;
                                    aiactor.knockbackDoer.ApplyKnockback(-a, 1.33f * (Vector2.Distance(portal.transform.position, aiactor.transform.position) + 0.005f), false);
                                }
                            }
                        }
                    }
                }
                        yield return null;
            }
            Destroy(portal.gameObject);
            yield break;
        }

        public MeshRenderer AceCourtBot;
        public PlayerController player;

    }
}