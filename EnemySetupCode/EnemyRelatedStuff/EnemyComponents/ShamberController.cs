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


public class ShamberController : BraveBehaviour
{

	private ParticleSystem particle;
	public void Start()
	{

		base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;

		base.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
		BulletsEaten = 0;
		Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
		mat.mainTexture = base.aiActor.sprite.renderer.material.mainTexture;
		mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		mat.SetFloat("_EmissivePower", 40);
		mat.SetFloat("_EmissiveThresholdSensitivity", 1f);
		mat.SetFloat("_EmissiveColorPower", 2f);


		var pso = new GameObject("shamebr fart");
		pso.transform.position = base.aiActor.sprite.WorldCenter + new Vector2(0, -0.25f);
		pso.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
		pso.transform.parent = base.aiActor.gameObject.transform;

		var partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("ShamberParticles")); ;//this is the name of the object which by default will be "Particle System"
		partObj.transform.position = pso.transform.position;
		partObj.transform.parent = pso.transform;

		particle = partObj.GetComponent<ParticleSystem>();

		base.sprite.renderer.material = mat;
		CanSucc = true;
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

	protected override void OnDestroy()
	{
		if (base.healthHaver)
		{
			base.healthHaver.OnPreDeath -= this.OnPreDeath;
			if (particle != null)
            {
				particle.Stop();
            }
		}
		base.OnDestroy();
	}

	protected void Update()
	{
		if (CanSucc == true)
        {
			ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
			if (allProjectiles != null && allProjectiles.Count >= 0 && base.gameObject != null)
			{
				for (int i = 0; i < allProjectiles.Count; i++)
                {
					Projectile  proj = allProjectiles[i];
					if (Vector2.Distance(proj.sprite.WorldCenter, base.sprite.WorldCenter) < 3f && proj != null && proj.specRigidbody != null)
					{
						GameManager.Instance.Dungeon.StartCoroutine(this.HandleBulletSuck(proj));
					}
				}
			}
		}
	}
	public List<Projectile> activeBullets;
	private IEnumerator HandleBulletSuck(Projectile target)
	{
		if(BulletsEaten <= 299)
        {
			this.BulletsEaten++;
		}
		
		Transform copySprite = this.CreateEmptySprite(target);
		Destroy(target.gameObject);
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
	private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
	{
		
	}

	private void OnPreDeath(Vector2 obj)
	{
		CanSucc = false;
		if (particle != null)
		{
			particle.Stop();
		}
		for (int j = 0; j < BulletsEaten; j++)
		{
			SpawnManager.SpawnBulletScript(base.aiActor.gameActor, new CustomBulletScriptSelector(typeof(BLLLARGH)));
		}

	}
	private bool CanSucc;
	public int BulletsEaten;
	protected void OnProjCreated(Projectile projectile)
	{
		if (!projectile.Owner.aiActor.CanTargetPlayers && projectile.Owner.aiActor.CanTargetEnemies)
		{
			projectile.collidesWithPlayer = false;
			projectile.collidesWithEnemies = true;

		}
		else
        {
			projectile.collidesWithPlayer = true;
			projectile.collidesWithEnemies = false;
		}
	}
}


public class BLLLARGH : Script
{
	protected override IEnumerator Top()
	{
		float fuckYOUYOUPIECEOFfuckINGSHITIHOPEYOUROTINAfuckINGFREEZER = UnityEngine.Random.Range(-180, 180);
		base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
		base.Fire(new Direction(fuckYOUYOUPIECEOFfuckINGSHITIHOPEYOUROTINAfuckINGFREEZER), new Speed(UnityEngine.Random.Range(2f, 5.5f), SpeedType.Absolute), new BurstBullet());
		yield break;
	}
	public class BurstBullet : Bullet
	{
		public BurstBullet() : base("reversible", false, false, false)
		{
		}
		protected override IEnumerator Top()
		{
			float speed = base.Speed;
			base.ChangeSpeed(new Speed(speed * 3.6f, SpeedType.Absolute), 120);
			yield break;
		}
	}
}
