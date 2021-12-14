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
	public class AddBeamsBehavior : BraveBehaviour
	{
		public static GameObject shootpoint;
		//public static Projectile newbeamprojectile;
		public void Start()
		{
			//base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("465da2bb086a4a88a803f79fe3a27677").bulletBank.GetBullet("homing"));
			shootpoint = new GameObject("fuck");
			shootpoint.transform.parent = base.aiActor.transform;
			shootpoint.transform.position = base.aiActor.sprite.WorldCenter;
			GameObject m_CachedGunAttachPoint = base.aiActor.transform.Find("fuck").gameObject;
			//ETGModConsole.Log("Added Component");


			AIBeamShooter aIBeamShooter = base.aiActor.gameObject.AddComponent<AIBeamShooter>();
			AIActor actor = EnemyDatabase.GetOrLoadByGuid("21dd14e5ca2a4a388adab5b11b69a1e1");
			AIBeamShooter aIBeamShooter2 = actor.GetComponent<AIBeamShooter>();

			//newbeamprojectile = aIBeamShooter2.beamProjectile;
			//Projectile proj = base.aiActor.gameObject.AddComponent<AIBeamShooter>(aIBeamShooter2).beamProjectile;
			AIActor actor1 = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
			BeholsterController aIBeamShooter22 = actor1.GetComponent<BeholsterController>();

			aIBeamShooter.beamTransform = m_CachedGunAttachPoint.transform;
			aIBeamShooter.beamModule = aIBeamShooter22.beamModule;
			aIBeamShooter.beamProjectile = aIBeamShooter22.projectile;
			aIBeamShooter.LaserAngle = aIBeamShooter2.LaserAngle;
			//aIBeamShooter.beamTransform = aIBeamShooter2.beamTransform;
			aIBeamShooter.chargeVfx = aIBeamShooter2.chargeVfx;
			//.shootAnim = null;
			//aIBeamShooter.beamModule = aIBeamShooter2.beamModule;
			aIBeamShooter.heightOffset = aIBeamShooter2.heightOffset;
			aIBeamShooter.PreventBeamContinuation = true;
			aIBeamShooter.TurnDuringDissipation = true;
			//aIBeamShooter.firingEllipseA = aIBeamShooter2.firingEllipseA;
			//aIBeamShooter.firingEllipseB = aIBeamShooter2.firingEllipseB;
			//aIBeamShooter.firingEllipseCenter = aIBeamShooter2.firingEllipseCenter;
			//aIBeamShooter.specifyAnimator = null;

			//base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").);

			ShootBeamBehavior yee = new ShootBeamBehavior()
			{
				
				AttackCooldown = 4,
				unitCatchUpSpeed = 1f,
				degTurnRateAcceleration = 30f,
				beamSelection = ShootBeamBehavior.BeamSelection.Specify,
				firingTime = 3f,
				stopWhileFiring = false,
				restrictBeamLengthToAim = false,
				RequiresLineOfSight = false,
				trackingType = ShootBeamBehavior.TrackingType.Follow,
				initialAimType = ShootBeamBehavior.InitialAimType.Aim,
				specificBeamShooter = aIBeamShooter,
				FireAnimation = null,
				PostFireAnimation = null,
				TellAnimation = null
				
				
				
			};
			
			base.aiActor.behaviorSpeculator.AttackBehaviors.Add(yee);
		}

	}
}
