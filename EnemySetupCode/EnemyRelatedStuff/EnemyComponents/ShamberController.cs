using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.ObjectModel;
using System.Collections;
using Planetside;
using Brave.BulletScript;
using Alexandria;

public class ShamberController : BraveBehaviour
{

	private ParticleSystem particle;
	public void Start()
	{

		base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

		Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
		mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
		mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		mat.SetFloat("_EmissivePower", 40);
		mat.SetFloat("_EmissiveThresholdSensitivity", 1f);
		mat.SetFloat("_EmissiveColorPower", 2f);


		var pso = new GameObject("shamber particle");
		pso.transform.position = base.aiActor.sprite.WorldCenter + new Vector2(0, -0.25f);
		pso.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
		pso.transform.parent = base.aiActor.gameObject.transform;

		var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles"));//this is the name of the object which by default will be "Particle System"
		partObj.transform.position = pso.transform.position;
		partObj.transform.parent = pso.transform;

		particle = partObj.GetComponent<ParticleSystem>();
		var h = particle.main;
		h.simulationSpace = ParticleSystemSimulationSpace.World;
		base.sprite.renderer.material = mat;
		base.healthHaver.OnPreDeath += this.OnPreDeath;
	}

	private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
	{
		if (clip.GetFrame(frameIdx).eventInfo == "turnofftrail")
		{
			if (particle != null)
			{
				particle.Stop();
			}
		}
		if (clip.GetFrame(frameIdx).eventInfo == "turnontrail")
		{
			if (particle != null)
			{
				particle.Play();
			}
		}
	}

	public override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
			if (particle != null)
            {
				particle.Stop();
            }
		}
        for (int i = this.m_bulletPositions.Count - 1; i >= 0; i--)
        {
            Projectile proj = this.m_bulletPositions[i].projectile;
            if (proj)
            {
                proj.ManualControl = false;
                proj.ResetDistance();
                proj.collidesWithEnemies = base.aiActor.CanTargetEnemies;
                proj.specRigidbody.CollideWithTileMap = true;
                proj.collidesWithPlayer = true;
                proj.UpdateCollisionMask();
                proj.Direction = proj.transform.PositionVector2() - this.aiActor.sprite.WorldCenter;
                proj.baseData.speed = Mathf.Min(this.m_bulletPositions[i].speed * 1.66f, 25);
                proj.UpdateSpeed();
                proj.IgnoreTileCollisionsFor(0.5f);
                if (proj.shouldRotate)
                {
                    proj.transform.rotation = Quaternion.Euler(0f, 0f, (proj.transform.PositionVector2() - this.aiActor.sprite.WorldCenter).ToAngle());
                }
            }
        }

        base.OnDestroy();
	}

	public bool CanSuckBullets()
    {
		if (base.aiActor.isActiveAndEnabled == true && base.aiActor.HasDonePlayerEnterCheck == true && base.aiActor.healthHaver.IsDead == false && base.aiActor.HasBeenAwoken == true) { return true; }
		return false;
    }

    private List<ShamberCont> m_bulletPositions = new List<ShamberCont>();

	public class ShamberCont
	{
		public Projectile projectile;
        public float s;
        public float speed;
		public float Radius = 1.5f; 

    }


    public void Update()
	{
		if (base.aiActor != null)
        {
			if (CanSuckBullets() == true)
			{
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				if (allProjectiles != null && allProjectiles.Count > 0 && base.gameObject != null)
				{
					for (int i = 0; i < allProjectiles.Count; i++)
					{
						Projectile proj = allProjectiles[i];
						if (proj && proj.Owner  != null && proj.Owner != this.aiActor)
                        {
							
                            BeamController beamController = proj.GetComponent<BeamController>();
							BasicBeamController basicBeamController = proj.GetComponent<BasicBeamController>();
							bool isNotBeam = basicBeamController == null && beamController == null;

                            Vector2 position = proj.sprite != null ? proj.sprite.WorldCenter : proj.transform.PositionVector2();

                            if (Vector2.Distance(position, base.aiActor.sprite.WorldCenter) < 3f && proj != null && proj.specRigidbody != null && isNotBeam == true)
                            {
                                if (proj.CanBeCaught)
                                {
                                    if (m_bulletPositions.Count() < 300)
                                    {
                                        if (proj != null)
                                        {
                                            if (proj.Owner != null)
                                            {
                                                proj?.specRigidbody?.DeregisterSpecificCollisionException(proj.Owner.specRigidbody);

                                                if (proj.Owner is PlayerController player)
                                                {
                                                    proj.sprite.color = new Color(1f, 0.1f, 0.1f);
                                                    proj.MakeLookLikeEnemyBullet(false);
                                                }

                                                if (proj.Owner is AIActor enemy)
                                                {
                                                    switch (enemy.EnemyGuid)
                                                    {
                                                        case "b1770e0f1c744d9d887cc16122882b4f":
                                                            proj.DieInAir();
                                                            break;
                                                        case "d5a7b95774cd41f080e517bea07bf495":
                                                            proj.DieInAir();
                                                            break;
                                                    }
                                                }
                                            }



                                            proj.Shooter = base.aiActor.specRigidbody;
                                            proj.Owner = base.aiActor;
                                            proj.specRigidbody.Velocity = Vector2.zero;
                                            proj.ManualControl = true;
                                            float s = proj.baseData.speed;
                                            proj.baseData.speed = 0;
                                            proj.UpdateSpeed();
                                            proj.baseData.speed = s;
                                            proj.specRigidbody.CollideWithTileMap = false;
                                            proj.ResetDistance();
                                            proj.collidesWithEnemies = base.aiActor.CanTargetEnemies;
                                            proj.collidesWithPlayer = true;
                                            proj.UpdateCollisionMask();
                                            proj.RemovePlayerOnlyModifiers();
                                            if (proj.BulletScriptSettings != null)
                                            {
                                                proj.BulletScriptSettings.preventPooling = true;

                                            }
                                            proj.RemoveBulletScriptControl();

                                            float second = BraveMathCollege.ClampAngle360((proj.transform.PositionVector2() - base.aiActor.sprite.WorldCenter).ToAngle());
                                            this.m_bulletPositions.Add(new ShamberCont()
                                            {
                                                speed = s,
                                                s = second,
                                                projectile = proj,
                                                Radius = Mathf.Min(3, 1.25f + (m_bulletPositions.Count() == 0f ? 0f : (float)m_bulletPositions.Count() / 100f))
                                            });
                                        }
                                    }
                                    else
                                    {
                                        GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSuck(proj));
                                    }
                                }
                            }
                        }
                    }
				}
			}
            for (int i = this.m_bulletPositions.Count - 1; i >= 0; i--)
            {
                var thing = this.m_bulletPositions[i];
                if (thing != null)
                {
                    Projectile first = thing.projectile;

                    if (first != null)
                    {
                        float num = this.m_bulletPositions[i].s + BraveTime.DeltaTime * Mathf.Max(30, (10 * this.m_bulletPositions[i].speed));
                        this.m_bulletPositions[i].s = num;

                        Vector2 bulletPosition = this.GetBulletPosition(num, first, this.m_bulletPositions[i].Radius);

                        first.transform.position = bulletPosition;
                        first.specRigidbody.Reinitialize();
                        if (first.shouldRotate)
                        {
                            first.transform.rotation = Quaternion.Euler(0f, 0f, 180f + (Quaternion.Euler(0f, 0f, 90f) * (this.aiActor.sprite.WorldCenter - bulletPosition)).XY().ToAngle());
                        }
                        first.ResetDistance();
                    }
                    else
                    {
                        this.m_bulletPositions[i] = null;
                    }
                }    
            }
        }		
	}

    private IEnumerator HandleBulletSuck(Projectile target)
    {
        Transform copySprite = this.CreateEmptySprite(target);
        target.DieInAir(true);
        Vector3 startPosition = copySprite.transform.position;
        float elapsed = 0f;
        float duration = 0.666f;
        while (elapsed < duration)
        {
            elapsed += BraveTime.DeltaTime;
            bool flag3 = copySprite && base.aiActor != null;
            if (flag3)
            {
                Vector3 position = base.sprite.WorldCenter;
                float t = elapsed / duration * (elapsed / duration);
                copySprite.position = Vector3.Lerp(startPosition, position, t);
                copySprite.rotation = Quaternion.Euler(0f, 0f, 60f * BraveTime.DeltaTime) * copySprite.rotation;
                copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                position = default(Vector3);
            }
            yield return null;
        }
        bool flag4 = copySprite;
        if (flag4)
        {
            UnityEngine.Object.Destroy(copySprite.gameObject);
        }
        yield break;
    }
    private Transform CreateEmptySprite(Projectile target)
    {
        GameObject gameObject = new GameObject("suck image");
        gameObject.layer = target.gameObject.layer;
        tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
        gameObject.transform.parent = SpawnManager.Instance.VFX;
        tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
        tk2dSprite.transform.position = target.sprite.transform.position;
        GameObject gameObject2 = new GameObject("image parent");
        gameObject2.transform.position = tk2dSprite.WorldCenter;
        tk2dSprite.transform.parent = gameObject2.transform;
        return gameObject2.transform;
    }

    private Vector2 GetBulletPosition(float angle, Projectile projectile, float radius)
    {
        return Vector2.MoveTowards(projectile.transform.PositionVector2(), this.aiActor.sprite.WorldCenter + new Vector2(Mathf.Cos(angle * 0.017453292f), Mathf.Sin(angle * 0.017453292f)) * radius, 0.15f);
    }


	private void OnPreDeath(Vector2 obj)
	{
		if (particle != null)
		{
			particle.Stop();
		}
		for (int i = this.m_bulletPositions.Count - 1; i > -1; i--)
		{
            Projectile proj = this.m_bulletPositions[i].projectile;
			if (proj)
			{
                proj.ManualControl = false;

                proj.collidesWithEnemies = base.aiActor.CanTargetEnemies;
                proj.specRigidbody.CollideWithTileMap = true;
                proj.collidesWithPlayer = true;
                proj.UpdateCollisionMask();
				proj.Direction = proj.transform.PositionVector2() - this.aiActor.sprite.WorldCenter;
                proj.ResetDistance();
                proj.baseData.UsesCustomAccelerationCurve = true;
                proj.baseData.CustomAccelerationCurveDuration = 3f;
                proj.baseData.range = 1000;
                proj.baseData.AccelerationCurve = new AnimationCurve()
                {
                    postWrapMode = WrapMode.ClampForever,

                    keys = new Keyframe[] {
                new Keyframe(){time = 0.1f, value = 0.3f, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.5f, value = 0f},
                new Keyframe(){time = 0.95f, value = 1.1f, inTangent = 0.75f, outTangent = 0.25f}
                }
                };
                proj.baseData.speed = Mathf.Min(this.m_bulletPositions[i].speed, 25);
                proj.UpdateSpeed();
                proj.IgnoreTileCollisionsFor(0.5f);
            }
        }
    }

}
