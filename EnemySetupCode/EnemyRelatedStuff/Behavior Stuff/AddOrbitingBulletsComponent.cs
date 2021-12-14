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
	public class AddOrbitingBulletsComponent : BraveBehaviour
	{
		public static GameObject shootpoint;
		public void Start()
		{
			base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
			shootpoint = new GameObject("fuck");
			shootpoint.transform.parent = base.aiActor.transform;
			shootpoint.transform.position = base.aiActor.sprite.WorldCenter;
			GameObject m_CachedGunAttachPoint = base.aiActor.transform.Find("fuck").gameObject;
			//ETGModConsole.Log("Added Component");


			//base.aiActor.HasShadow = true;
			//if (base.aiActor.ShadowObject == null)
			{
				//base.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;

			}
			CustomSpinBulletsBehavior yeah = new CustomSpinBulletsBehavior()
			{
				ShootPoint = m_CachedGunAttachPoint,
				OverrideBulletName = "homing",
				NumBullets = 2,
				BulletMinRadius = 2.2f,
				BulletMaxRadius = 2.3f,
				BulletCircleSpeed = 75,
				BulletsIgnoreTiles = true,
				RegenTimer = 0.1f,
				AmountOFLines = 3,
			};
			base.aiActor.behaviorSpeculator.OtherBehaviors.Add(yeah);
		}

	}
}