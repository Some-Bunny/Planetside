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
		/*
		ParticleSystem yes = pso.gameObject.AddComponent<ParticleSystem>();
		//yes.CopyFrom<ParticleSystem>(particle);
		yes.Play();
		yes.name = "Shamber Particles";
		yes.transform.position = base.aiActor.sprite.WorldCenter;

		particle = yes;

		var main = yes.main;
		main.maxParticles = 10000;
		main.playOnAwake = false;
		main.duration = 1;
		main.loop = true;
		main.startLifetime = new ParticleSystem.MinMaxCurve(0.33f, 1.2f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 4f);
		main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
		main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255));
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.startRotation = new ParticleSystem.MinMaxCurve(-45, 45);
		main.randomizeRotationDirection = 2;
		main.gravityModifier = -1.2f;
		

		var emm = yes.emission;
		emm.rateOverTime = 28;

		var colorOverLifetime =  yes.colorOverLifetime;
		colorOverLifetime.enabled = true;
		var brightness = UnityEngine.Random.Range(0.2f, 1);
		var gradient = new Gradient();
		gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 0.9f) }, new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
		colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

		var vOL = yes.velocityOverLifetime;
		vOL.enabled = true;
		vOL.speedModifier = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f)));

		var sc = yes.shape;
		sc.shapeType = ParticleSystemShapeType.Circle;
		sc.radius = 0.1f;

		var tsa = yes.textureSheetAnimation;
		tsa.animation = ParticleSystemAnimationType.SingleRow;
		tsa.numTilesX = 5;
		tsa.numTilesY = 1;
		tsa.enabled = true;
		tsa.cycleCount = 1;
		tsa.frameOverTimeMultiplier = 1.3f;

		var vel = yes.inheritVelocity;

		vel.mode = ParticleSystemInheritVelocityMode.Initial;
		vel.curveMultiplier = 0.9f;

		var sizeOverLifetime = yes.sizeOverLifetime;
		sizeOverLifetime.enabled = true;
		sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0.4f, 1f), new Keyframe(1f, 0f)));


		var sbs =  yes.sizeBySpeed;
		sbs.separateAxes = false;
		sbs.sizeMultiplier = 0.9f;
		sbs.size = new ParticleSystem.MinMaxCurve(1, 0);

		var particleRenderer = yes.gameObject.GetComponent<ParticleSystemRenderer>();
		//particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
		particleRenderer.material.mainTexture = Shamber.ShamberParticleTexture;

		Material sharedMaterial = particleRenderer.sharedMaterial;
		//particleRenderer.usesOverrideMaterial = true;
		Material material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
		material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
		material.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		material.SetFloat("_EmissiveColorPower", 5f);
		material.SetFloat("_EmissivePower", 25f);
		particleRenderer.material = material;
		*/
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
			if (allProjectiles != null)
			{
				foreach (Projectile proj in allProjectiles)
				{
					bool ae = Vector2.Distance(proj.sprite.WorldCenter, base.sprite.WorldCenter) < 3f && proj != null && proj.specRigidbody != null;
					if (ae)
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
			this.BulletsEaten += 1;
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
			/*
			GameObject gameObject = new GameObject();
			gameObject.transform.position = base.sprite.WorldCenter;
			BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
			gameObject.AddComponent<BulletSourceKiller>();
			var bulletScriptSelected = new CustomBulletScriptSelector(typeof(BLLLARGH));
			AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
			AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
			bulletBank = OtherTools.CopyAIBulletBank(aIActor.bulletBank);//to prevent our gun from affecting the bulletbank of the enemy
			bulletBank.CollidesWithEnemies = false;
			source.BulletManager = bulletBank;
			source.BulletScript = bulletScriptSelected;
			source.Initialize();//to fire the script once
			*/
			//Destroy(gameObject);
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
