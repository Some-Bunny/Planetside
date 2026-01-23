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
using Dungeonator;
using HutongGames.PlayMaker.Actions;

public class ShamberController : BraveBehaviour
{

	public ParticleSystem particle;
	public void Start()
	{

		base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
		var h = particle.main;
		h.simulationSpace = ParticleSystemSimulationSpace.World;
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
        ReleaseAllBullets();
        base.OnDestroy();
	}

	public bool CanSuckBullets()
    {
        if (BulletsReleased) {return false;}
		if (base.aiActor.isActiveAndEnabled == true && base.aiActor.HasDonePlayerEnterCheck == true && base.aiActor.healthHaver.IsDead == false && base.aiActor.HasBeenAwoken == true) { return true; }
		return false;
    }

    private List<ShamberCont> m_bulletPositions = new List<ShamberCont>();
    private List<Projectile> m_CaughtBullets = new List<Projectile>();

    public class ShamberCont
	{
		public Projectile projectile;
        public float s;
        public float speed;
		public float Radius = 1.5f; 

    }


    private bool ShouldCountForRoomProgress(RoomHandler handler, ref int amount)
    {
        List<AIActor> EnemyList = GetTheseActiveEnemies(handler, RoomHandler.ActiveEnemyType.RoomClear);
        EnemyList.RemoveAll(self => self.EnemyGuid == this.aiActor.EnemyGuid);
        amount = EnemyList.Count;

        if (handler.remainingReinforcementLayers == null) { return false; }

        bool remainingReinforcements = true;
        if (handler.remainingReinforcementLayers.Count == 0) { remainingReinforcements = false; }




        if (EnemyList.Count == 0 && remainingReinforcements == true) { return false; }
        if (EnemyList.Count > 0 && remainingReinforcements == true) { return true; }


        if (EnemyList.Count > 0 && remainingReinforcements == false) { return false; }
        if (EnemyList.Count == 0 && remainingReinforcements == false) { return false; }
        return false;
    }
    public List<AIActor> GetTheseActiveEnemies(RoomHandler room, RoomHandler.ActiveEnemyType type)
    {
        var outList = new List<AIActor>();
        if (room.activeEnemies == null)
        {
            return outList;
        }
        if (type == RoomHandler.ActiveEnemyType.RoomClear)
        {
            for (int i = 0; i < room.activeEnemies.Count; i++)
            {
                if (!room.activeEnemies[i].IgnoreForRoomClear)
                {
                    outList.Add(room.activeEnemies[i]);
                }
            }
        }
        else
        {
            outList.AddRange(room.activeEnemies);
        }
        return outList;
    }

    private IEnumerator DoVomitSequence()
    {
        while (this.aiActor.IsGone)
        {
            yield return null;
        }
        if (particle)
        {
            particle.Stop();
        }
        yield return new WaitForSeconds(0.25f);
        this.aiActor.behaviorSpeculator.InterruptAndDisable();
        this.aiActor.aiAnimator.Play("vomit", AIAnimator.AnimatorState.StateEndType.Duration, 1.25f, -1, true, "");
        this.aiActor.MovementSpeed = 0;
        float e = 0;
        AkSoundEngine.PostEvent("Play_ENM_cult_charge_01", this.gameObject);

        while (e < 0.25f)
        {
            e += Time.deltaTime;
            yield return null;
        }
        for (float i = 0; i < 3; i++)
        {
            ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
            {
                position = this.aiActor.sprite.WorldCenter - new Vector2(0.3125f, 0),
                startLifetime = 0.3333f,
                startColor = new Color(1, 1, 1, 0.1f + (0.05f * i)),
                startSize = 20f,
            });
            AkSoundEngine.PostEvent("Play_ENM_bullet_dash_01", this.gameObject);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.25f);
        AkSoundEngine.PostEvent("Play_BOSS_Rat_Cheese_Burst_01", this.gameObject);
        AkSoundEngine.PostEvent("Play_BOSS_Rat_Cheese_Burst_01", this.gameObject);

        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
        {
            position = this.aiActor.sprite.WorldCenter - new Vector2(0.3125f, 0),
            startLifetime = 0.3333f,
            startColor = new Color(1, 1, 1, 0.3f),
            startSize = 20f,
        });

        ReleaseAllBullets(true);
        this.aiActor.aiAnimator.Play("expel", AIAnimator.AnimatorState.StateEndType.UntilFinished, -1, -1, true, "");
        while (this.aiActor.aiAnimator.CurrentClipProgress < 0.5f)
        {
            yield return null;
        }
        for (int i = 0; i < base.aiAnimator.OtherAnimations.Count; i++)
        {
            if (base.aiAnimator.OtherAnimations[i].name == "death")
            {
                base.aiAnimator.OtherAnimations[i].anim.Type = DirectionalAnimation.DirectionType.Single;
                base.aiAnimator.OtherAnimations[i].anim.Prefix = "fast";
            }
        }
        base.healthHaver.SuppressDeathSounds = true;
        AkSoundEngine.PostEvent("Play_BOSS_doormimic_vanish_01", this.gameObject);
        base.healthHaver.ApplyDamage(100000f, Vector2.zero, "Death on Room Claer", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, false);

        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
        {
            position = this.aiActor.sprite.WorldCenter - new Vector2(0.3125f, 0),
            startLifetime = 0.2f,
            startColor = new Color(1, 1, 1, 0.2f),
            startSize = 20f,
        });
        yield break;
    }
    private Coroutine DoVomit;
    public void Update()
	{
		if (base.aiActor != null)
        {
            var room = base.aiActor.parentRoom;
            if (room != null)
            {
                int amount = 1;
                base.aiActor.IgnoreForRoomClear = ShouldCountForRoomProgress(room, ref amount);
                if (amount == 0 && DoVomit == null)
                {
                    DoVomit = this.StartCoroutine(DoVomitSequence());
                }
            }


            if (CanSuckBullets() == true)
			{
				ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
				if (allProjectiles != null && allProjectiles.Count > 0 && base.gameObject != null)
				{
					for (int i = 0; i < allProjectiles.Count; i++)
					{
						Projectile proj = allProjectiles[i];
						if (proj != null && proj.Owner != this.aiActor)
                        {
							if (m_CaughtBullets.Contains(proj)) { continue; }


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
                                        m_CaughtBullets.Add(proj);
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
                                            Radius = Mathf.Min(3, 1.25f + (m_bulletPositions.Count() == 0f ? 0f : (float)(m_bulletPositions.Count()) * 0.025f))
                                        });
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
                        float num = this.m_bulletPositions[i].s + BraveTime.DeltaTime * Mathf.Min(30, (10 * this.m_bulletPositions[i].speed));
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
            if (copySprite && base.aiActor != null)
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
        if (copySprite != null)
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
        var v = this.aiActor.sprite.WorldCenter + new Vector2(Mathf.Cos(angle * 0.017453292f), Mathf.Sin(angle * 0.017453292f)) * radius;
        var m = Vector2.Distance(v, projectile.transform.PositionVector2()) * 1.5f;
        return Vector2.MoveTowards(projectile.transform.PositionVector2(), v, Mathf.Min(10f, m) * Time.deltaTime);
    }


	private void OnPreDeath(Vector2 obj)
	{
        if (particle != null)
        {
            particle.Stop();
        }
        ReleaseAllBullets();
    }
    private bool BulletsReleased = false;
    public void ReleaseAllBullets(bool isForced = false)
    {
        if (BulletsReleased) { return; }
        BulletsReleased = true;
        for (int i = this.m_bulletPositions.Count - 1; i > -1; i--)
        {
            if (this.m_bulletPositions[i] != null)
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
                    proj.baseData.range = 100;
                    proj.ResetDistance();
                    
                    if (isForced)
                    {                      
                        proj.baseData.speed = Mathf.Min(this.m_bulletPositions[i].speed, 25);
                    }
                    else
                    {
                        proj.baseData.UsesCustomAccelerationCurve = true;
                        proj.baseData.CustomAccelerationCurveDuration = 2.5f;

                        proj.baseData.AccelerationCurve = new AnimationCurve()
                        {
                            postWrapMode = WrapMode.ClampForever,

                            keys = new Keyframe[] {
                        new Keyframe(){time = 0.1f, value = 0.3f, inTangent = 0.5f, outTangent = 0.25f},
                        new Keyframe(){time = 0.5f, value = 0f},
                        new Keyframe(){time = 0.9f, value = 1.1f, inTangent = 0.5f, outTangent = 0.25f}
                        }
                        };
                        proj.baseData.speed = Mathf.Min(this.m_bulletPositions[i].speed, 25);
                    }
                    proj.UpdateSpeed();
                    proj.IgnoreTileCollisionsFor(0.25f);
                }
            }
        }
    }
}
