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
using Planetside;
using UnityEngine.Serialization;


public class AddDashComponent : BraveBehaviour
{
	public AddDashComponent()
	{
		this.WaitTime = 10f;
	}
	public static GameObject shootpoint;

	public void Start()
	{
		GameObject piss = new GameObject();

		//shootpoint = new GameObject("fuck");
		piss.transform.parent = base.aiActor.transform;
		piss.transform.position = base.aiActor.sprite.WorldCenter;
		GameObject m_CachedGunAttachPoint = piss;

		base.aiActor.gameObject.AddComponent<AfterImageTrailController>().spawnShadows = false;
		base.aiActor.gameObject.AddComponent<tk2dSpriteAttachPoint>();
		base.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();
		base.aiActor.HasShadow = true;
		base.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("4db03291a12144d69fe940d5a01de376").ShadowObject;

		DashBehavior yeah = new DashBehavior()
		{
			ShootPoint = m_CachedGunAttachPoint,
			dashDistance = 7f,
			dashTime = 0.2f,
			doubleDashChance = 0,
			enableShadowTrail = false,
			Cooldown = 3,
			dashDirection = DashBehavior.DashDirection.Random,
			warpDashAnimLength = true,
			hideShadow = true,
			InitialCooldown = 1f,
			chargeAnim = null,
			//dashAnim = null,
			
			//bulletScript = new CustomBulletScriptSelector(typeof(DashAttack)),
			//BulletScript = new CustomBulletScriptSelector(typeof(Wail)),
			//LeadAmount = 0f,
			//AttackCooldown = 5f,
			//InitialCooldown = 4f,
			//TellAnimation = "wail",
			//FireAnimation = "wail",
			RequiresLineOfSight = false,
		};
		base.aiActor.behaviorSpeculator.AttackBehaviors.Add(yeah);
	}


	private void OnPreDeath(Vector2 obj)
	{


	}

	public float distortionMaxRadius = 50f;
	public float distortionDuration = 1f;
	public float distortionIntensity = 0.5f;
	public float distortionThickness = 0.2f;
	public float WaitTime;
}
