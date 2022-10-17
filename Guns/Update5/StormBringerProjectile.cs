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
	internal class StormBringerProjectile : MonoBehaviour
	{
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (this.projectile != null)
            {
                projectile.OnDestruction += (obj) =>
                {
                    LightningController c = new LightningController();
                    c.MajorNodesCount = UnityEngine.Random.Range(MajorNodesMin, MajorNodesMax);
                    c.Thickness = Thickness;
                    c.MajorNodeSplitoffChance = splitoffChance;
                    c.LightningMajorNodeDelay = 0;
                    c.LightningMinorNodeDelay = 0;

                    c.OnPostStrike += (obj1) =>
                    {
                        AkSoundEngine.PostEvent("Play_Lightning", GameManager.Instance.BestActivePlayer.gameObject);
                        var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                        lightObj.transform.position = obj1;
                        Destroy(lightObj, 0.25f);
                        Exploder.Explode(obj1, Explosion, obj1);

                    };
                    c.LightningPreDelay = 0f;
                    c.GenerateLightning(obj.PossibleSourceGun != null ? obj.PossibleSourceGun.barrelOffset.transform.position : GameManager.Instance.PrimaryPlayer.sprite.WorldCenter.ToVector3XUp(), obj.sprite.WorldCenter);
                };
            }
        }

        public int MajorNodesMin = 4;
        public int MajorNodesMax = 7;

        public int Thickness = 1;
        public ExplosionData Explosion = StormBringer.defaultLightningExplosion;
        public float splitoffChance = 0;


        private Projectile projectile;
	}
}

