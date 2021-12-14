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
	public class AddBeholsterBeamComponent : BraveBehaviour
	{
		public AddBeholsterBeamComponent()
        {
			//this.ShootSFX = "Play_ENM_deathray_shot_01";
			//this.LoopSFXBeamStop = "Stop_ENM_deathray_loop_01";
			//this.ChargeEntity = "Play_ENM_beholster_charging_01";
			//this.ChargeBeam = "Play_ENM_deathray_charge_01";
			this.AddsBaseBeamBehavior = false;
			//this.FiresDirectlyTowardsPlayer = false;
			//this.CustomAngleValue = 0;
			//this.UsesCustomAngle = false;
		}
		public static GameObject shootpoint;
		public static GameObject shootpoin1;

		public void Start()
		{
			shootpoint = new GameObject("fuck");
			shootpoint.transform.parent = base.aiActor.transform;
			shootpoint.transform.position = base.aiActor.sprite.WorldCenter + new Vector2(0, 0);
			GameObject m_CachedGunAttachPoint = base.aiActor.transform.Find("fuck").gameObject;

			shootpoin1 = new GameObject("piss");
			shootpoin1.transform.parent = base.aiActor.transform;
			shootpoin1.transform.position = base.aiActor.sprite.WorldCenter + new Vector2(3, 0);
			GameObject m_CachedGunAttachPoint1 = base.aiActor.transform.Find("piss").gameObject;






			AIBeamShooter2 yeet = base.aiActor.gameObject.AddComponent<AIBeamShooter2>();
			AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
			BeholsterController aIBeamShooter22 = actor.GetComponent<BeholsterController>();

			//AIActor actor = EnemyDatabase.GetOrLoadByGuid("4b992de5b4274168a8878ef9bf7ea36b");
			//BeholsterController aIBeamShooter2 = actor.GetComponent<BeholsterController>();

			yeet.beamTransform = m_CachedGunAttachPoint.transform;
			yeet.beamModule = aIBeamShooter22.beamModule;
			yeet.beamProjectile = aIBeamShooter22.beamModule.GetCurrentProjectile();
			yeet.beamProjectile = aIBeamShooter22.projectile;
			//yeet.LaserAngle = 90;
			yeet.SetLaserAngle(0);
			yeet.firingEllipseCenter = m_CachedGunAttachPoint.transform.position;
			yeet.name = "dont overrwite pls :)";

			
			AIBeamShooter2 yeetfm = base.aiActor.gameObject.AddComponent<AIBeamShooter2>();
			BeholsterController aIBeamShooter2 = actor.GetComponent<BeholsterController>();

			yeetfm.beamTransform = m_CachedGunAttachPoint1.transform;
			yeetfm.beamModule = aIBeamShooter2.beamModule;
			yeetfm.beamProjectile = aIBeamShooter2.beamModule.GetCurrentProjectile();
			yeetfm.beamProjectile = aIBeamShooter2.projectile;
			yeetfm.SetLaserAngle(180);
			yeetfm.firingEllipseCenter = m_CachedGunAttachPoint1.transform.position;

			yeetfm.name = "dsadadsa";


			AIBeamShooter2 yeetfm1 = base.aiActor.gameObject.AddComponent<AIBeamShooter2>();
			yeetfm1.beamTransform = m_CachedGunAttachPoint1.transform;
			yeetfm1.beamModule = aIBeamShooter2.beamModule;
			yeetfm1.beamProjectile = aIBeamShooter2.beamModule.GetCurrentProjectile();
			yeetfm1.beamProjectile = aIBeamShooter2.projectile;
			yeetfm1.SetLaserAngle(270);
			yeetfm1.firingEllipseCenter = m_CachedGunAttachPoint1.transform.position;

			yeetfm1.name = "dsadadsa";

			AIBeamShooter2 yeetfm2 = base.aiActor.gameObject.AddComponent<AIBeamShooter2>();
			yeetfm2.beamTransform = m_CachedGunAttachPoint.transform;
			yeetfm2.beamModule = aIBeamShooter2.beamModule;
			yeetfm2.beamProjectile = aIBeamShooter2.beamModule.GetCurrentProjectile();
			yeetfm2.beamProjectile = aIBeamShooter2.projectile;
			yeetfm2.SetLaserAngle(90);
			yeetfm2.firingEllipseCenter = m_CachedGunAttachPoint.transform.position;

			yeetfm2.name = "dsadadsa";
			//Tools.LogPropertiesAndFields(yeet);
			//Tools.LogPropertiesAndFields(yeetfm);

			//Tools.LogPropertiesAndFields(base.aiActor.gameObject.GetComponents<AIBeamShooter2>()[0]);
			//Tools.LogPropertiesAndFields(base.aiActor.gameObject.GetComponents<AIBeamShooter2>()[1]);

			if (this.AddsBaseBeamBehavior == true)
			{
				CustomBeholsterLaserBehavior yee = new CustomBeholsterLaserBehavior()
				{

					AttackCooldown = 4,
					InitialCooldown = 3,
					//degTurnRateAcceleration = 30f,
					//beamSelection = ShootBeamBehavior.BeamSelection.Specify,
					firingTime = 5f,
					//stopWhileFiring = false,
					//restrictBeamLengthToAim = false,
					RequiresLineOfSight = false,
					trackingType = CustomBeholsterLaserBehavior.TrackingType.Follow,
					//initialAimType = ShootBeamBehavior.InitialAimType.Aim,
					//specificBeamShooter = aIBeamShooter,
					//FireAnimation = null,
					//PostFireAnimation = null,
					//TellAnimation = null
					
					unitCatchUpSpeed = 8f,
					maxTurnRate = 10,
					turnRateAcceleration = 24,
					useDegreeCatchUp = transform,
					minDegreesForCatchUp = 15,
					degreeCatchUpSpeed = 180,
					useUnitCatchUp = true,
					minUnitForCatchUp = 2,
					maxUnitForCatchUp = 10,
					useUnitOvershoot = true,
					minUnitForOvershoot = 1,
					unitOvershootTime = 0.25f,
					unitOvershootSpeed = 10,
					
					
					chargeTime = 1f,
					ShootPoint = shootpoint.transform,
					FiresDirectlyTowardsPlayer = true,
					//specificBeamShooter = yeet,
					//UsesCustomAngle = true,
					//FiresDirectlyTowardsPlayer = true,
					beamSelection = ShootBeamBehavior.BeamSelection.All,
					//UsesBaseSounds = false,
					
					//BulletScript = new CustomBulletScriptSelector(typeof(Wailer.Wail)),
				};

				base.aiActor.behaviorSpeculator.AttackBehaviors.Add(yee);
			}
		}

		public bool AddsBaseBeamBehavior;

	}
}