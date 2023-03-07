using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;
//using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;

namespace Planetside
{
	public class LynceusBullet : AIActor
	{
		public static void Init()
		{
			EnemyToolbox.CreateNewBulletBankerEnemy("lynceus_bullet", "Lynceus", 20, 20, new List<int> { 227, 228, 229, 230 }, new List<int> { 231, 232, 233, 234, 235 }, null, new LIGHTNING(), 3f);
		}
		public class LIGHTNING : Script 
		{
			public override IEnumerator Top() 
			{
				Vector2 playerPos = this.GetPredictedTargetPosition(1, 30);
                LightningController c = new LightningController();
                c.MajorNodesCount = UnityEngine.Random.Range(3, 6);
                c.OnPostStrike += (obj) =>
                {
                    AkSoundEngine.PostEvent("Play_Lightning", this.BulletBank.gameObject);

                    var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                    lightObj.transform.position = obj;
                    Destroy(lightObj, 0.25f);
                    Exploder.Explode(obj, StaticExplosionDatas.genericLargeExplosion, obj);
                };
                c.LightningPreDelay = 1.5f;
                c.OnPreDelay += (obj) =>
                {
                    GameManager.Instance.StartCoroutine(Wavy(obj));
                };
                c.GenerateLightning(playerPos + new Vector2(UnityEngine.Random.Range(-7, 7), 16), playerPos);
                yield break;
			}
            public IEnumerator Wavy(Vector2 pos)
            {
                float elapsed = 0;
                var lightObj = UnityEngine.Object.Instantiate(LightningMaker.lightObject);
                AdditionalBraveLight braveLight = lightObj.GetComponent<AdditionalBraveLight>();
                lightObj.transform.position = pos;

                braveLight.LightIntensity = 0;
                braveLight.LightRadius = 0;
                while (elapsed < 1.5f)
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
}








