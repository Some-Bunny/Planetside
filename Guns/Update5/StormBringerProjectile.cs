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
                projectile.OnDestruction += (objH) =>
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

                        for (int i = 0; i < CoinTosser.AllActiveCoins.Count; i++)
                        {
                            var coinLocal = CoinTosser.AllActiveCoins[i];
                            if (coinLocal != null)

                            {
                                var coinSprite = coinLocal.GetComponentInChildren<tk2dBaseSprite>();
                                if (Vector2.Distance(coinSprite.WorldCenter, objH.sprite.WorldCenter) < 3.5f)
                                {
                                    float angle = BraveUtility.RandomAngle();

                                    for (int e = 0; e < 3; e++)
                                    {
                                        LightningController c2 = new LightningController();
                                        c2.MajorNodeMaxAngleSpacing = 20;
                                        c2.MajorNodeMinAngleSpacing = 5;
                                        c2.MajorNodesCount = UnityEngine.Random.Range(3, 5);
                                        c2.MajorNodeSplitoffChance = 0;
                                        c2.Thickness = 2;

                                        c2.OnPostStrike += (objLocal) =>
                                        {

                                            var lightObjlocal = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                                            lightObjlocal.transform.position = objLocal;
                                            Destroy(lightObjlocal, 0.25f);


                                            Exploder.Explode(objLocal, Explosion, objLocal);
                                            AkSoundEngine.PostEvent("Play_Lightning", GameManager.Instance.BestActivePlayer.gameObject);

                                        };

                                        c2.LightningPreDelay = 0f;
                                        int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable);

                                        var result = RaycastToolbox.ReturnRaycast(coinSprite.WorldCenter, MathToolbox.GetUnitOnCircle(angle + (120 * e), 1), rayMask);
                                        //ETGModConsole.Log(result.Distance);
                                        //ETGModConsole.Log(result.Contact);
                                        //ETGModConsole.Log(coinSprite.WorldCenter);


                                        c2.GenerateLightning(coinSprite.WorldCenter, coinSprite.WorldCenter + MathToolbox.GetUnitOnCircle(angle + (120 * e), result.Distance));
                                        RaycastResult.Pool.Free(ref result);

                                    }
                                    AkSoundEngine.PostEvent("Play_perfectshot", GameManager.Instance.BestActivePlayer.gameObject);

                                    var coinProj = coinLocal.GetComponent<Projectile>();

                                    coinProj.DieInAir();
                                }


                            }
                        }




                    };
                    c.LightningPreDelay = 0f;
                    c.GenerateLightning(objH.PossibleSourceGun != null ? objH.PossibleSourceGun.barrelOffset.transform.position : GameManager.Instance.PrimaryPlayer.sprite.WorldCenter.ToVector3XUp(), objH.sprite.WorldCenter);
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

