using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Alexandria.PrefabAPI;
using Planetside.Static_Storage;
using System.Collections;

namespace Planetside
{

    public class ExpandReticleRiserEffect : MonoBehaviour
    {
        public ExpandReticleRiserEffect()
        {
            this.NumRisers = 4;
            this.RiserHeight = 1f;
            this.RiseTime = 1.5f;
            this.UpdateSpriteDefinitions = false;
            this.CurrentSpriteName = string.Empty;
        }

        private void Start()
        {
            this.m_sprite = base.GetComponent<tk2dSprite>();
            this.m_sprite.usesOverrideMaterial = true;

			Height = this.m_sprite.HeightOffGround;
            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
            //gameObject.GetComponent<tk2dSprite>().renderer.material.shader = StaticShaders.TransparencyShader;//ShaderCache.Acquire("tk2d/BlendVertexColorUnlitTilted");

            //UnityEngine.Object.Destroy(gameObject.GetComponent<ExpandReticleRiserEffect>());


            this.m_risers = new tk2dSprite[this.NumRisers];
            //this.m_risers[0] = gameObject.GetComponent<tk2dSprite>();
            for (int i = 0; i < this.NumRisers; i++)
            {
                var obj = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                UnityEngine.Object.Destroy(obj.GetComponent<tk2dSpriteAnimator>());
                UnityEngine.Object.Destroy(obj.GetComponent<ExpandReticleRiserEffect>());
                obj.GetComponent<tk2dSprite>().renderer.material.shader = StaticShaders.TransparencyShader;//ShaderCache.Acquire("tk2d/BlendVertexColorUnlitTilted");
                this.m_risers[i] = obj.GetComponent<tk2dSprite>();
                this.m_risers[i].renderer.enabled = true;

                this.StartCoroutine(DoLoop(this.m_risers[i], (((float)i / (float)this.NumRisers)) * RiseTime));
            }
            this.OnSpawned();
        }
		private float Height;
        private void OnSpawned()
        {
            this.m_localElapsed = 0f;
            if (this.m_risers != null)
            {
                for (int i = 0; i < this.m_risers.Length; i++)
                {
                    this.m_risers[i].transform.parent = base.transform;
                    this.m_risers[i].transform.localPosition = Vector3.zero;
                    this.m_risers[i].transform.localRotation = Quaternion.identity;
                    this.m_risers[i].usesOverrideMaterial = true;
                    this.m_risers[i].gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
                }
            }
        }

        public void Stop()
        {
            Stopped = true;
            for (int i = 0; i < this.m_risers.Count(); i++)
            {
                var riser = this.m_risers[i];
                if (riser)
                {
                    riser.StartCoroutine(Zap(riser));
                }
            }
        }

        public IEnumerator Zap(tk2dSprite s)
        {
            if (s == null) { yield break; }
            float e = s.renderer.material.GetFloat("_Fade");
            while (e > 0)
            {
                float y = Mathf.Lerp(0f, this.RiserHeight, e);
                s.transform.localPosition = Vector3.zero;
                s.transform.position += Vector3.zero.WithY(y * 0.66f);
                s.renderer.material.SetFloat("_Fade", Mathf.Max(0, 1 - e));
                e += BraveTime.DeltaTime * 1.5f;
                yield return null;
            }
            Destroy(s.gameObject);
            yield break;
        }

        private bool Stopped = false;

		public IEnumerator DoLoop(tk2dSprite Riser, float delay = 0)
		{
            float e = 0;
            if (delay > 0)
			{
			
                while (e < delay)
				{
                    e += Time.deltaTime;
                    yield return null;
                }
				
            }
            e = 0;
            while (e < RiseTime)
			{
				e += Time.deltaTime;
				float t = e / RiseTime;// Mathf.Max(0f, this.m_localElapsed - this.RiseTime / (float)this.NumRisers * (float)i) % this.RiseTime / this.RiseTime;
                float y = Mathf.Lerp(0f, this.RiserHeight, t);
                Riser.transform.localPosition = Vector3.zero;
                Riser.transform.position += Vector3.zero.WithY(y);
				Riser.HeightOffGround = Height + y;
                Riser.SortingOrder = (int)(y * 16);
                Riser.ForceRotationRebuild();
                Riser.UpdateZDepth();
                Riser.sprite.renderer.material.SetFloat("_Fade", Mathf.Max(0, 1 - t));
				yield return null;
            }
			if (Stopped == false)
			{
				this.StartCoroutine(DoLoop(Riser));
			}
			yield break;
		}
		

        private void Update()
        {
			/*
            if (!this.m_sprite || Stopped == true)
            {
                return;
            }
            this.m_localElapsed += BraveTime.DeltaTime;
            this.m_sprite.ForceRotationRebuild();
            this.m_sprite.UpdateZDepth();
            if (this.m_risers != null)
            {
                for (int i = 0; i < this.m_risers.Length; i++)
                {
                    if (this.UpdateSpriteDefinitions && !string.IsNullOrEmpty(this.CurrentSpriteName))
                    {
                        this.m_risers[i].SetSprite(this.CurrentSpriteName);
                    }
                    float t = Mathf.Max(0f, this.m_localElapsed - this.RiseTime / (float)this.NumRisers * (float)i) % this.RiseTime / this.RiseTime;
                    float y = Mathf.Lerp(0f, this.RiserHeight, t);
                    this.m_risers[i].transform.localPosition = Vector3.zero;
                    this.m_risers[i].transform.position += Vector3.zero.WithY(y);
                    this.m_risers[i].ForceRotationRebuild();
                    this.m_risers[i].UpdateZDepth();
                    this.m_risers[i].sprite.renderer.material.SetFloat("_Fade", Mathf.Max(0, 1 - t));
                    if (this.UpdateSpriteDefinitions && !string.IsNullOrEmpty(this.CurrentSpriteName))
                    {
                        this.m_risers[i].SetSprite(this.CurrentSpriteName);
                    }

                }
            }
			*/
        }
        public bool UpdateSpriteDefinitions;
        public string CurrentSpriteName;
        public int NumRisers;
        public float RiserHeight;
        public float RiseTime;
        private tk2dSprite m_sprite;
        private tk2dSprite[] m_risers;
        private float m_localElapsed;
    }


    public static class RandomPiecesOfStuffToInitialise
    {

        public static void BuildPrefab()
        {

			AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
			LaserReticle = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;
			bundle = null;


			BuildSoulGuon();
			BuildMedal();
			BuildTargetReticle();
			BuildCursePoof();
		}
		public static GameObject LaserReticle;


		public static void BuildTargetReticle()
		{

            GameObject gameObject = PrefabBuilder.BuildObject("Kinetic Strike Target Reticle");

            tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("redmarksthespot"));



            //KineticStrikeTargetReticle = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/KineticStrike/redmarksthespot", new GameObject("Kinetic Strike Target Reticle"));
            //KineticStrikeTargetReticle.SetActive(false);
            //FakePrefab.MarkAsFakePrefab(KineticStrikeTargetReticle);
            //UnityEngine.Object.DontDestroyOnLoad(KineticStrikeTargetReticle);

            tk2dSprite bS = gameObject.GetOrAddComponent<tk2dSprite>();
			bS.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;

            bS.sprite.usesOverrideMaterial = true;


            Material mat = bS.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = bS.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 50);
            bS.sprite.renderer.material = mat;

            ExpandReticleRiserEffect rRE = bS.gameObject.AddComponent<ExpandReticleRiserEffect>();
			rRE.RiserHeight = 2;
			rRE.RiseTime = 1;
			rRE.NumRisers = 3;
			KineticStrikeTargetReticle = gameObject;
        }
		public static GameObject KineticStrikeTargetReticle;


		


		public static void BuildSpeakerObject()
		{
			SpeakerObject = new GameObject();
			SpeakerObject.AddComponent<TextMaker>();
			UnityEngine.Object.DontDestroyOnLoad(SpeakerObject);
		}
		public static GameObject SpeakerObject;


		public static void BuildCursePoof()
        {
			List<string> cursepoofvfxVFXPaths = new List<string>()
			{
				"Planetside/Resources/VFX/CursePoof/cursepoof_001",
				"Planetside/Resources/VFX/CursePoof/cursepoof_002",
				"Planetside/Resources/VFX/CursePoof/cursepoof_003",
				"Planetside/Resources/VFX/CursePoof/cursepoof_004",
				"Planetside/Resources/VFX/CursePoof/cursepoof_005",
			};
			cursepoofvfx = VFXToolbox.CreateVFX("Cursebulon Goop Poof", cursepoofvfxVFXPaths, 14, new IntVector2(12, 12), tk2dBaseSprite.Anchor.LowerCenter, true, 0.18f);
		}
		public static GameObject cursepoofvfx;


		public static void BuildSW()
		{
			GameObject SW = ItemBuilder.AddSpriteToObject("SW", "Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", null);
			tk2dSpriteAnimator animator = SW.AddComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip();
			animationClip.fps = 5;
			animationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			animationClip.name = "idle";
			GameObject spriteObject = new GameObject("spriteObject");
			ItemBuilder.AddSpriteToObject("spriteObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_001", spriteObject);
			tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame();
			starterFrame.spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId;
			starterFrame.spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection;
			tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
			{
				starterFrame
			};
			animationClip.frames = frameArray;
			for (int i = 2; i < 5; i++)
			{
				GameObject spriteForObject = new GameObject("spriteForObject");
				ItemBuilder.AddSpriteToObject("spriteForObject", $"Planetside/Resources/Enemies/SomethingWicked/somethingwicked_idle_00{i}", spriteForObject);
				tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
				frame.spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId;
				frame.spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection;
				animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
			}
			animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
			animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
			animator.DefaultClipId = animator.GetClipIdByName("idle");
			animator.playAutomatically = true;

			SpeculativeRigidbody orAddComponent = animator.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
			PixelCollider pixelCollider = new PixelCollider();
			pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
			pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
			pixelCollider.ManualWidth = 0;
			pixelCollider.ManualHeight = 0;
			pixelCollider.ManualOffsetX =0;
			pixelCollider.ManualOffsetY =0;
			orAddComponent.PixelColliders = new List<PixelCollider>
			{
				pixelCollider
			};

			SomethingWicked = animator.gameObject;

			UnityEngine.Object.DontDestroyOnLoad(SomethingWicked);
			FakePrefab.MarkAsFakePrefab(SomethingWicked);
			SomethingWicked.SetActive(false);

		}
		public static GameObject SomethingWicked;

		public static void BuildSoulGuon()
        {
			GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/Guons/SoulGuon/guoner.png");
			gameObject.name = $"Soul Guon";
			SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
			PlayerOrbital orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
			speculativeRigidbody.CollideWithTileMap = false;
			speculativeRigidbody.CollideWithOthers = true;
			speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
			orbitalPrefab.shouldRotate = false;
			orbitalPrefab.orbitRadius = 4f;
			orbitalPrefab.orbitDegreesPerSecond = 40;
			orbitalPrefab.SetOrbitalTier(0);


			ImprovedAfterImage yeah = gameObject.AddComponent<ImprovedAfterImage>();
			yeah.dashColor = new Color(0, 0.7f, 1);
			yeah.spawnShadows = true;
			yeah.shadowTimeDelay = 0.05f;
			yeah.shadowLifetime = 0.15f;

			PlayerOrbital si = gameObject.GetComponent<PlayerOrbital>();

			si.sprite.usesOverrideMaterial = true;
			si.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
			Material mat = si.sprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = si.sprite.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 100);
			si.sprite.renderer.material = mat;

			SoulSynergyGuon = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(SoulSynergyGuon);
			FakePrefab.MarkAsFakePrefab(SoulSynergyGuon);
			SoulSynergyGuon.SetActive(false);
		}
		public static GameObject SoulSynergyGuon;
		public static string vfxNamefrailty = "FrailtyVFX";
		public static GameObject MedalObject;
		public static void BuildMedal()
		{
			MedalObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/planetsidemedal", new GameObject("Medal"));
			MedalObject.SetActive(false);
			tk2dBaseSprite vfxSprite = MedalObject.GetComponent<tk2dBaseSprite>();
			vfxSprite.GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.LowerCenter, vfxSprite.GetCurrentSpriteDef().position3);
			FakePrefab.MarkAsFakePrefab(MedalObject);
			UnityEngine.Object.DontDestroyOnLoad(MedalObject);
		}







	}
}


/*
public class SomethingWicked : MonoBehaviour
{
	public SomethingWicked()
	{
		this.ShootTimer = 3f;
		this.MinSpeed = 4f;
		this.MaxSpeed = 9f;
		this.MinSpeedDistance = 8f;
		this.MaxSpeedDistance = 40f;
	}

	// Token: 0x17000DE8 RID: 3560
	public static SomethingWicked Instance
	{
		get
		{
			return SomethingWicked.m_instance;
		}
	}


	private void Start()
	{
		HasTPed = false;
		AkSoundEngine.PostEvent("Play_OBJ_chestglitch_loop_01", base.gameObject);
		SomethingWicked.m_instance = this;
		SpeculativeRigidbody specRigidbody = base.gameObject.GetComponent<SpeculativeRigidbody>();
		specRigidbody.OnEnterTrigger = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(specRigidbody.OnEnterTrigger, new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered));
		//base.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
		//base.aiAnimator.PlayUntilFinished("intro", false, null, -1f, false);
		//tk2dSpriteAnimator spriteAnimator = base.spriteAnimator;
		//spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent));
		/*
		for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
		{
			if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#SREAPER_ENCNAME")
			{
				GameStatsManager.Instance.HandleEncounteredObjectRaw(EncounterDatabase.Instance.Entries[i].myGuid);
			}
		}
		this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
		if (base.encounterTrackable)
		{
			GameStatsManager.Instance.HandleEncounteredObject(base.encounterTrackable);
		}
		*/
/*
		Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
		mat.mainTexture = base.gameObject.GetComponent<tk2dSpriteAnimator>().sprite.renderer.material.mainTexture;
		mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
		mat.SetFloat("_EmissiveColorPower", 1.55f);
		mat.SetFloat("_EmissivePower", 100);
		base.gameObject.GetComponent<tk2dSpriteAnimator>().sprite.renderer.material = mat;

		Planetside.ImprovedAfterImage yes = base.gameObject.AddComponent<Planetside.ImprovedAfterImage>();
		yes.spawnShadows = true;
		yes.shadowLifetime = 0.2f;
		yes.shadowTimeDelay = 0.001f;
		yes.dashColor = Color.black;
		yes.name = "SW trail";
	}

	public void OnDestroy()
	{
		SomethingWicked.m_instance = null;
	}

	private void HandleTriggerEntered(SpeculativeRigidbody targetRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
	{
		Projectile projectile = targetRigidbody.projectile;
		if (projectile)
		{
			//projectile.HandleKnockback(base.specRigidbody, targetRigidbody.GetComponent<PlayerController>(), false, false);
		}
	}

	private void Update()
	{
		if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
		{
			return;
		}
		if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
		{
			return;
		}
		if (TimeTubeCreditsController.IsTimeTubing)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.HandleMotion();
		ETGModConsole.Log(base.transform.position.ToString());
		foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
		{
			PlayerController player = proj.Owner as PlayerController;
			bool isBem = proj.GetComponent<BasicBeamController>() != null;
			if (isBem == true && Planetside.BeamToolbox.PosIsNearAnyBoneOnBeam(proj.GetComponent<BasicBeamController>(), base.gameObject.transform.PositionVector2(), 6f) && proj.Owner != null && proj.Owner == player && HasTPed != true)
            {
				AkSoundEngine.PostEvent("Play_VO_gorgun_gasp_01", base.gameObject);
				Vector2 Point = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), 200);
				base.transform.position = base.transform.PositionVector2() + Point;
				MaxSpeed += 0.5f;
				GameManager.Instance.StartCoroutine(this.TPCooldown());
			}
			else if (Vector2.Distance(proj.sprite.WorldCenter, base.gameObject.transform.PositionVector2()) < 6f && proj.Owner != null && proj.Owner == player && HasTPed != true)
            {
				AkSoundEngine.PostEvent("Play_VO_gorgun_gasp_01", base.gameObject);
				Vector2 Point = MathToolbox.GetUnitOnCircle(proj.Direction.ToAngle() + 180, 200);
				base.transform.position = base.transform.PositionVector2() + Point;
				MaxSpeed += 0.5f;
				GameManager.Instance.StartCoroutine(this.TPCooldown());
			}
		}
	}

	private IEnumerator TPCooldown()
    {
		HasTPed = true;
		yield return new WaitForSeconds((MaxSpeed/10)+1.5f);
		HasTPed = false;
		yield break;
    }

	private bool HasTPed;
	private void HandleAttacks()
	{
		/*
		if (base.aiAnimator.IsPlaying("intro"))
		{
			return;
		}
		CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
		if (cellData != null && cellData.type != CellType.WALL)
		{

		}
		
	}
	

/*
	private void HandleMotion()
	{
		//base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity = Vector2.zero;
		if (this.m_currentTargetPlayer == null)
		{
			return;
		}
		if (this.m_currentTargetPlayer.healthHaver.IsDead || this.m_currentTargetPlayer.IsGhost)
		{
			this.m_currentTargetPlayer = GameManager.Instance.GetRandomActivePlayer();
		}
		Vector2 centerPosition = this.m_currentTargetPlayer.CenterPosition;
		Vector2 vector = centerPosition - base.gameObject.GetComponent<SpeculativeRigidbody>().UnitCenter;
		float magnitude = vector.magnitude;
		float d = Mathf.Lerp(this.MinSpeed, this.MaxSpeed, (magnitude - this.MinSpeedDistance) / (this.MaxSpeedDistance - this.MinSpeedDistance));
		base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity = vector.normalized * d;
		//base.gameObject.GetComponent<SpeculativeRigidbody>().Velocity += this.knockbackComponent;
	}

	private static SomethingWicked m_instance;

	// Token: 0x040058CB RID: 22731
	public static bool PreventShooting;

	// Token: 0x040058CC RID: 22732
	public BulletScriptSelector BulletScript;

	public Transform ShootPoint;

	public float ShootTimer;

	public float MinSpeed;

	public float MaxSpeed;

	public float MinSpeedDistance;

	public float MaxSpeedDistance;

	[NonSerialized]
	public Vector2 knockbackComponent;

	// Token: 0x040058D4 RID: 22740
	private PlayerController m_currentTargetPlayer;

}

*/