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
	internal class GunknownProjectile : MonoBehaviour
	{
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            PlayerController playerController = projectile.Owner as PlayerController;
            if (this.projectile)
            {

                if (playerController.PlayerHasActiveSynergy(".null"))
                {
                    AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", base.gameObject);
                    GameObject gameObject = new GameObject("silencer");
                    SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();


                    playerController.ForceBlank(50f, 1, false, true, null, true, -1f);

                    AkSoundEngine.PostEvent("Play_ENM_statue_ring_01", base.gameObject);

                    for (int i = 0; i < 12; i++)
                    {
                        GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(playerController, PointNull.PointNullRune, false);
                        GunknownGuonComponent yes = gameobject2.AddComponent<GunknownGuonComponent>();
                        yes.maxDuration = 15;
                        yes.sourcePlayer = playerController;
                        yes.TimeBetweenFiring = 0.6f;
                        yes.MaxHealth = 5;
                        yes.isSynergy = true;
                        yes.fireDelay = 0.05f * i;
                        yes.Orbital = gameobject2.GetComponent<PlayerOrbital>();
                    }
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_ENM_statue_ring_01", base.gameObject);

                    for (int i = 0; i < 12; i++)
                    {
                        GameObject gameobject2 = PlayerOrbitalItem.CreateOrbital(playerController, UnknownGun.GunknownGuon, false);
                        GunknownGuonComponent yes = gameobject2.AddComponent<GunknownGuonComponent>();
                        yes.maxDuration = 15;
                        yes.sourcePlayer = playerController;
                        yes.Orbital = gameobject2.GetComponent<PlayerOrbital>();
                    }
                }
            }
        }
        private Projectile projectile;
	}
}

