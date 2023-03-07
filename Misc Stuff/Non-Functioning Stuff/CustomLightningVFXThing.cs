using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//DOES NOT WORK
//DONT EVEN TRY
//                    *(%%%%%%&&&&&&&&&%%%%##(/**,.                               
//                &@@@@@@@@@@/.%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&#                
//              @@@@@@@@#*@@@/*%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&@@@%           
//            (@@@@@&*@@@,&@/*&@@@@@@@@@@@.@@@@@@@@@@%/#&@@@@@@%,%@@@@@@@@        
//           @@@@@@@@@*&@,@@@@@@@@@@@@@@@@@/@@@@@@@@@@#&@@@@@@@@@@@(@@@@@@@       
//         .@@@@@@@@@@@@@@@*             %@@@@@@@@@@@@(@@@@@@@@@@@@@@@@@@@@,      
//        .,/#&@@@@@#//@%  %,       /%@@@%  /@@@@@@@@@&      .,    @&&@@@@@@@.    
//     ##@@@%,    ,#@@#@@@@@@@@@*.@@@&,     %@@@@@@*     *#@@@@@@@@@@@@@@@&@%,@%  
//   @.@@@  (@@@@@@&.    .(/    &@@@@@@@@@@@@@@@@@@@@* &@@@@@@@@@@@&      ,@#@@,( 
//  @@/@&  @@@@&   &@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&  (@@@@@@     (@@@@@@@@/@## 
//  @@,@% #&     @@%    %@@@@@@&#(//,,@   *%@@@@@@@@@@@@%   @@@@@@@@@% ,@@@@.@@,. 
//  *@(@@, &@@@@  @@@@@@,     ,&@@@@@@@  @@     *@@@@@@@  . %&/,&@@@@    @@@#.@   
//    &@*/@@@@@@%    ,&@@  @@@@&/     ,#@@@@@@@@@@@     &@@@@@@@@&.      &@@@@    
//       *@@@@@@@@/ ,@(      &@@@@@@&  &(*        .,**///*,      #@  @   (@@@.    
//        *@@@@@@@@@, /@@  *       *  @@@@@@@@@  @@@@@. %@@@@@, #@@  #   (@@@     
//          @@@@@@@@@@.   .@@@@@@(.                                      #@@@     
//           #@@@@@@@@@@%  *@@@@@@@  @@@@/.                              @@@@     
//             #@@@@@@@@@@@@   #@@  &@@@@@@@@* &@@@@@/  ...   ,#/ .&  * (@@@@     
//                @@*%@@@*&@@@@%    *&@@@@@@@, @@@@@@, &@@@  @@* (@@,  @@@@@@     
//                   *@@%.&@@&.#@@@@@@%,                          /@@@@@@@@@@.    
//                        &@@&,/@@@%.(@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%*@@@@@@@*    
//                             /@@@@%*,#@@%(*,**/(#%&&@@@@@@@@@%**&@@@@@#*@@@#    
//                                  ,&@@@@@@@@@&#/**/(#%&&@@@@@@@@@&/,&@@@@@@/    
//                                       .(@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      
//                                                 ,(&@@@@@@@@@@@@@@@@@@%.        

public class ModifiedChainLightningModifier : BraveBehaviour
{
	// Token: 0x06008492 RID: 33938 RVA: 0x00369840 File Offset: 0x00367A40
	private void Start(AIActor aiactor)
	{
		this.UpdateLinkToProjectile(aiActor);
		PhysicsEngine.Instance.OnPostRigidbodyMovement += this.PostRigidbodyUpdate;
	}

	// Token: 0x06008493 RID: 33939 RVA: 0x00369858 File Offset: 0x00367A58
	public override void OnDestroy()
	{
		if (PhysicsEngine.Instance != null)
		{
			PhysicsEngine.Instance.OnPostRigidbodyMovement -= this.PostRigidbodyUpdate;
		}
		this.ClearLink();
		if (this.BackLinkProjectile && this.BackLinkProjectile.GetComponent<ModifiedChainLightningModifier>())
		{
			ModifiedChainLightningModifier component = this.BackLinkProjectile.GetComponent<ModifiedChainLightningModifier>();
			component.ClearLink();
			component.ForcedLinkProjectile = null;
		}
		base.OnDestroy();
	}

	// Token: 0x06008494 RID: 33940 RVA: 0x003698D8 File Offset: 0x00367AD8
	private void Update()
	{
		this.m_frameLinkProjectile = null;
	}

	// Token: 0x06008495 RID: 33941 RVA: 0x003698E4 File Offset: 0x00367AE4
	private void UpdateLinkToProjectile(AIActor targetProjectile)
	{
		if (this.m_extantLink == null)
		{
			this.m_extantLink = SpawnManager.SpawnVFX(this.LinkVFXPrefab, false).GetComponent<tk2dTiledSprite>();
			int num = -1;
			if (this.DamagesPlayers && !this.m_hasSetBlackBullet)
			{
				this.m_hasSetBlackBullet = true;
				Material material = this.m_extantLink.GetComponent<Renderer>().material;
				material.SetFloat("_BlackBullet", 0.995f);
				material.SetFloat("_EmissiveColorPower", 4.9f);
			}
			else if (!this.DamagesPlayers && PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.TESLA_UNBOUND, out num))
			{
				Material material2 = this.m_extantLink.GetComponent<Renderer>().material;
				material2.SetFloat("_BlackBullet", 0.15f);
				material2.SetFloat("_EmissiveColorPower", 0.1f);
			}
		}
		Vector2 unitCenter = base.projectile.specRigidbody.UnitCenter;
		Vector2 unitCenter2 = targetProjectile.specRigidbody.UnitCenter;
		this.m_extantLink.transform.position = unitCenter;
		Vector2 vector = unitCenter2 - unitCenter;
		float z = BraveMathCollege.Atan2Degrees(vector.normalized);
		int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
		this.m_extantLink.dimensions = new Vector2((float)num2, this.m_extantLink.dimensions.y);
		this.m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, z);
		this.m_extantLink.UpdateZDepth();
		bool flag = this.ApplyLinearDamage(unitCenter, unitCenter2);
		if (flag && this.UsesDispersalParticles)
		{
			this.DoDispersalParticles(unitCenter2, unitCenter);
		}
	}

	// Token: 0x06008496 RID: 33942 RVA: 0x00369A98 File Offset: 0x00367C98
	private void DoDispersalParticles(Vector2 posStart, Vector2 posEnd)
	{
		if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
		{

		}
	}

	// Token: 0x06008497 RID: 33943 RVA: 0x00369C10 File Offset: 0x00367E10
	private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
	{
		this.m_damagedEnemies.Add(damagedTarget);
		yield return new WaitForSeconds(this.damageCooldown);
		this.m_damagedEnemies.Remove(damagedTarget);
		yield break;
	}

	// Token: 0x06008498 RID: 33944 RVA: 0x00369C34 File Offset: 0x00367E34
	private bool ApplyLinearDamage(Vector2 p1, Vector2 p2)
	{
		bool result = false;
		if (this.DamagesEnemies)
		{
			for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
			{
				AIActor aiactor = StaticReferenceManager.AllEnemies[i];
				if (!this.m_damagedEnemies.Contains(aiactor))
				{
					if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody)
					{
						Vector2 zero = Vector2.zero;
						bool flag = BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero);
						if (flag)
						{
							aiactor.healthHaver.ApplyDamage(this.damagePerHit, Vector2.zero, "Chain Lightning", this.damageTypes, DamageCategory.Normal, false, null, false);
							result = true;
							GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
						}
					}
				}
			}
		}
		if (this.DamagesPlayers)
		{
			for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
			{
				PlayerController playerController = GameManager.Instance.AllPlayers[j];
				if (playerController && !playerController.IsGhost && playerController.healthHaver && playerController.healthHaver.IsAlive && playerController.healthHaver.IsVulnerable)
				{
					Vector2 zero2 = Vector2.zero;
					bool flag2 = BraveUtility.LineIntersectsAABB(p1, p2, playerController.specRigidbody.HitboxPixelCollider.UnitBottomLeft, playerController.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero2);
					if (flag2)
					{
						playerController.healthHaver.ApplyDamage(0.5f, Vector2.zero, base.projectile.OwnerName, this.damageTypes, DamageCategory.Normal, false, null, false);
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06008499 RID: 33945 RVA: 0x00369E1C File Offset: 0x0036801C
	private void ClearLink()
	{
		if (this.m_extantLink != null)
		{
			SpawnManager.Despawn(this.m_extantLink.gameObject);
			this.m_extantLink = null;
		}
	}

	// Token: 0x0600849A RID: 33946 RVA: 0x00369E48 File Offset: 0x00368048
	private Projectile GetLinkProjectile()
	{
		Projectile projectile = null;
		float num = float.MaxValue;
		float num2 = this.maximumLinkDistance * this.maximumLinkDistance;
		for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
		{
			Projectile projectile2 = StaticReferenceManager.AllProjectiles[i];
			if (projectile2 && projectile2 != base.projectile && (projectile2.Owner == base.projectile.Owner))
			{
				if (this.RequiresSameProjectileClass)
				{
					if (base.projectile.spriteAnimator && projectile2.spriteAnimator)
					{
						if (base.projectile.spriteAnimator.CurrentClip != projectile2.spriteAnimator.CurrentClip)
						{
							goto IL_28C;
						}
					}
					else if (base.projectile.spriteAnimator || projectile2.spriteAnimator)
					{
						goto IL_28C;
					}
					if (base.projectile.sprite && projectile2.sprite)
					{
						if (projectile2.sprite.spriteId != base.projectile.sprite.spriteId || projectile2.sprite.Collection != base.projectile.sprite.Collection)
						{
							goto IL_28C;
						}
					}
					else if (base.projectile.sprite || projectile2.sprite)
					{
						goto IL_28C;
					}
				}
				ModifiedChainLightningModifier component = projectile2.GetComponent<ModifiedChainLightningModifier>();
				if (component && component.m_frameLinkProjectile == null)
				{
					float sqrMagnitude = (component.specRigidbody.UnitCenter - base.specRigidbody.UnitCenter).sqrMagnitude;
					if (sqrMagnitude < num && sqrMagnitude < num2)
					{
						projectile = projectile2;
						num = sqrMagnitude;
					}
				}
				else if (this.CanChainToAnyProjectile && projectile2 && projectile2.specRigidbody && this && base.specRigidbody)
				{
					float sqrMagnitude2 = (projectile2.specRigidbody.UnitCenter - base.specRigidbody.UnitCenter).sqrMagnitude;
					if (sqrMagnitude2 < num && sqrMagnitude2 < num2)
					{
						projectile = projectile2;
						num = sqrMagnitude2;
					}
				}
			}
			IL_28C:;
		}
		if (projectile == null)
		{
			return null;
		}
		return projectile;
	}

	// Token: 0x0600849B RID: 33947 RVA: 0x0036A104 File Offset: 0x00368304
	private void PostRigidbodyUpdate()
	{
		if (base.projectile)
		{
			Projectile projectile = (!this.UseForcedLinkProjectile) ? this.GetLinkProjectile() : this.ForcedLinkProjectile;
			if (projectile)
			{
				this.UpdateLinkToProjectile(aiActor);
			}
			else
			{
				this.ClearLink();
			}
		}
		else
		{
			this.ClearLink();
		}
	}

	// Token: 0x04008839 RID: 34873
	public GameObject LinkVFXPrefab;

	// Token: 0x0400883A RID: 34874
	public CoreDamageTypes damageTypes;

	// Token: 0x0400883B RID: 34875
	public bool RequiresSameProjectileClass;

	// Token: 0x0400883C RID: 34876
	public float maximumLinkDistance = 8f;

	// Token: 0x0400883D RID: 34877
	public float damagePerHit = 5f;

	// Token: 0x0400883E RID: 34878
	public float damageCooldown = 1f;

	// Token: 0x0400883F RID: 34879
	[NonSerialized]
	public bool CanChainToAnyProjectile;

	// Token: 0x04008840 RID: 34880
	[NonSerialized]
	public bool UseForcedLinkProjectile;

	// Token: 0x04008841 RID: 34881
	[NonSerialized]
	public Projectile ForcedLinkProjectile;

	// Token: 0x04008842 RID: 34882
	[NonSerialized]
	public Projectile BackLinkProjectile;

	// Token: 0x04008843 RID: 34883
	[NonSerialized]
	public bool DamagesPlayers;

	// Token: 0x04008844 RID: 34884
	[NonSerialized]
	public bool DamagesEnemies = true;

	// Token: 0x04008845 RID: 34885
	[Header("Dispersal")]
	public bool UsesDispersalParticles;

	// Token: 0x04008846 RID: 34886
	[ShowInInspectorIf("UsesDispersalParticles", false)]
	public float DispersalDensity = 3f;

	// Token: 0x04008847 RID: 34887
	[ShowInInspectorIf("UsesDispersalParticles", false)]
	public float DispersalMinCoherency = 0.2f;

	// Token: 0x04008848 RID: 34888
	[ShowInInspectorIf("UsesDispersalParticles", false)]
	public float DispersalMaxCoherency = 1f;

	// Token: 0x04008849 RID: 34889
	[ShowInInspectorIf("UsesDispersalParticles", false)]
	public GameObject DispersalParticleSystemPrefab;

	// Token: 0x0400884A RID: 34890
	private Projectile m_frameLinkProjectile;

	// Token: 0x0400884B RID: 34891
	private tk2dTiledSprite m_extantLink;

	// Token: 0x0400884C RID: 34892
	private bool m_hasSetBlackBullet;

	// Token: 0x0400884D RID: 34893
	//public ParticleSystem m_dispersalParticles;

	// Token: 0x0400884E RID: 34894
	private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();
	public GameObject ChainLightningVFX;

	// Token: 0x040070E8 RID: 28904
	public CoreDamageTypes ChainLightningDamageTypes;
}
