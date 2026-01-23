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
using Dungeonator;
using Alexandria.PrefabAPI;
using System.Diagnostics;
using static Planetside.BulletBankMan;

public class UmbraController : BraveBehaviour
{
	public static GameObject UmbralEye;
    public static GameObject UmbralLockOnSmall;
    public static GameObject UmbralLockOnFloor;

    public class LockOnFloor : MonoBehaviour
    {
        public List<tk2dSprite> tk2DSprites_1;
        public List<tk2dSprite> tk2DSprites_2;

        public Transform trans_1;
        public Transform trans_2;

        float e = 0;
        float e_ = 0;

        public void SetState(bool State)
        {
            e_ = State ? 1 : 0;
        }

        public void Update()
        {
            e = Mathf.MoveTowards(e, e_, Time.deltaTime);
            trans_1.Rotate(new Vector3(0, 0, 150 * Time.deltaTime));
            trans_2.Rotate(new Vector3(0, 0, -125 * Time.deltaTime));
            foreach (var entry in tk2DSprites_1)
            {
                entry.renderer.material.SetFloat("_Fade", MathToolbox.EaseOut(e));         
            }
            foreach (var entry in tk2DSprites_2)
            {
                entry.renderer.material.SetFloat("_Fade", MathToolbox.EaseOut(e));
            }
        }

    }

    public class LockOnEffect : MonoBehaviour
	{
		float Offset = 0.25f;
        public tk2dSprite mainSprite;
        public List<tk2dSprite> tk2DSprites;
        public Transform trans;

        float e = 0;
        float e_ = 0;
		public Vector2 LockInPos;

		public void SetState(bool State)
		{
			e_ = State ? 1 : 0;
		}

		public void Update()
		{
			this.transform.position = LockInPos;
            e = Mathf.MoveTowards(e, e_, Time.deltaTime * 5);
            trans.Rotate(new Vector3(0, 0, 270 * Time.deltaTime));
            Offset = Mathf.Lerp(1.25f, 0.5f, MathToolbox.EaseIn(e));
            mainSprite.renderer.material.SetFloat("_Fade", MathToolbox.EaseIn(e));
            int slot = 0;
            foreach (var entry in tk2DSprites)
            {
                entry.renderer.material.SetFloat("_Fade", MathToolbox.EaseIn(e));
                Vector3 vector3 = Vector3.zero;

                switch (slot)
                {
                    case 0:
                        vector3 = new Vector2(-Offset, -Offset);
                        break;
                    case 1:
                        vector3 = new Vector2(-Offset, Offset);
                        break;
                    case 2:
                        vector3 = new Vector2(Offset, -Offset);
                        break;
                    case 3:
                        vector3 = new Vector2(Offset, Offset);
                        break;
                }
                entry.transform.localPosition = vector3;
                slot++;
            }
        }
	}

	public static void InitEffect()
	{

        UmbralEye = PrefabBuilder.BuildObject("UmbralEye");
        var sprite_eye = UmbralEye.AddComponent<tk2dSprite>();
        sprite_eye.SetSprite(StaticSpriteDefinitions.VFX_Sheet_Data, "umbral_eye_001");
        tk2dSpriteAnimator animator = UmbralEye.GetOrAddComponent<tk2dSpriteAnimator>();
        animator.library = StaticSpriteDefinitions.VFX_Animation_Data;
        animator.Library = StaticSpriteDefinitions.VFX_Animation_Data;
        animator.defaultClipId = StaticSpriteDefinitions.VFX_Animation_Data.GetClipIdByName("umbraleye_inner_open");

		
        animator.sprite.usesOverrideMaterial = true;
        Material mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
        mat.SetFloat("_EmissivePower", 3f);
        mat.SetFloat("_EmissiveColorPower", 5f);
        mat.SetColor("_EmissiveColor", new Color(255, 0, 0, 255));
        mat.mainTexture = animator.sprite.renderer.material.mainTexture;
        animator.sprite.renderer.material = mat;
		

        var UmbralInner = PrefabBuilder.BuildObject("UmbralLens");
        var sprite_lens = UmbralInner.AddComponent<tk2dSprite>();
        sprite_lens.SetSprite(StaticSpriteDefinitions.VFX_Sheet_Data, "umbral_eyeinner_001");
        var animator_ = UmbralInner.GetOrAddComponent<tk2dSpriteAnimator>();
        animator_.library = StaticSpriteDefinitions.VFX_Animation_Data;
        animator_.Library = StaticSpriteDefinitions.VFX_Animation_Data;
        animator_.defaultClipId = StaticSpriteDefinitions.VFX_Animation_Data.GetClipIdByName("umbraleye_lens_open");

        animator_.sprite.usesOverrideMaterial = true;
        mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
        mat.SetFloat("_EmissivePower", 50);
        mat.SetFloat("_EmissiveColorPower", 1000f);
        mat.SetColor("_EmissiveColor", new Color(255, 151, 0, 255));
        mat.mainTexture = animator_.sprite.renderer.material.mainTexture;
        animator_.sprite.renderer.material = mat;

        sprite_eye.SortingOrder = 7;
        sprite_eye.HeightOffGround = 3;

        sprite_lens.SortingOrder = 10;
        sprite_lens.HeightOffGround = 5;

        UmbralInner.transform.SetParent(UmbralEye.transform, false);

		var umbraVFX = UmbralEye.gameObject.AddComponent<UmbraVFX>();
		umbraVFX.InnerEye = animator;
        umbraVFX.Lens = animator_;


        #region LockOn
        UmbralLockOnSmall = PrefabBuilder.BuildObject("UmbralLockIn");
		var s = UmbralLockOnSmall.AddComponent<tk2dSprite>();
        s.SetSprite(StaticSpriteDefinitions.VFX_Sheet_Data, "lockin_umbral1");
        s.SortingOrder = 10;
        s.HeightOffGround = 5;
        s.sprite.usesOverrideMaterial = true;
        s.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
        var holder = PrefabBuilder.BuildObject("Holder");
        holder.transform.SetParent(UmbralLockOnSmall.transform, false);
        var lockOn = UmbralLockOnSmall.gameObject.AddComponent<LockOnEffect>();
        lockOn.mainSprite = s;
		lockOn.trans = holder.transform;
        lockOn.tk2DSprites = new List<tk2dSprite>
		{	
			CreateEffectSmall(holder, "1", false, false, new Vector2(-0.25f, -0.25f)),
			CreateEffectSmall(holder, "2", false, true, new Vector2(-0.25f, 0.25f)),
			CreateEffectSmall(holder, "3", true, false, new Vector2(0.25f, -0.25f)),
			CreateEffectSmall(holder, "4", true, true, new Vector2(0.25f, 0.25f)),
		};
        #endregion

        #region LockOnFloor
        UmbralLockOnFloor = PrefabBuilder.BuildObject("UmbralLockInFloor");

        var holder_1 = PrefabBuilder.BuildObject("Holder1");
        var holder_2 = PrefabBuilder.BuildObject("Holder2");

        holder_1.transform.SetParent(UmbralLockOnFloor.transform, false);
        holder_2.transform.SetParent(UmbralLockOnFloor.transform, false);

        var bigFloor = UmbralLockOnFloor.AddComponent<LockOnFloor>();
        bigFloor.trans_1 = holder_1.transform;
        bigFloor.trans_2 = holder_2.transform;

        bigFloor.tk2DSprites_1 = new List<tk2dSprite>()
        {
            CreateEffect(holder_1, "lockin_umbral_floor_001","1_Large", false, false, new Vector2(-0.75f, 0.75f)),
            CreateEffect(holder_1, "lockin_umbral_floor_001","2_Large", false, true, new Vector2(-0.75f, -0.75f)),
            CreateEffect(holder_1, "lockin_umbral_floor_001","3_Large", true, false, new Vector2(0.75f, 0.75f)),
            CreateEffect(holder_1, "lockin_umbral_floor_001","4_Large", true, true, new Vector2(0.75f, -0.75f)),
        };
        bigFloor.tk2DSprites_2 = new List<tk2dSprite>()
        {
            CreateEffect(holder_2, "lockin_umbral_floor_002","11_Large", false, false, new Vector2(-0.75f, 0.75f)),
            CreateEffect(holder_2, "lockin_umbral_floor_002","12_Large", false, true, new Vector2(-0.75f, -0.75f)),
            CreateEffect(holder_2, "lockin_umbral_floor_002","13_Large", true, false, new Vector2(0.75f, 0.75f)),
            CreateEffect(holder_2, "lockin_umbral_floor_002","14_Large", true, true, new Vector2(0.75f, -0.75f)),
        };
        #endregion
    }

    private static tk2dSprite CreateEffectSmall(GameObject parent, string name, bool flipX, bool flipY, Vector2 offset)
	{
        var UmbralLockEdge_BL = PrefabBuilder.BuildObject("UmbralLockIn"+ name);
        var s = UmbralLockEdge_BL.AddComponent<tk2dSprite>();
        s.SetSprite(StaticSpriteDefinitions.VFX_Sheet_Data, "lockin_umbral2");
        s.SortingOrder = 10;
        s.HeightOffGround = 5;
		s.FlipX = flipX;
		s.FlipY = flipY;
        s.gameObject.transform.SetParent(parent.transform, false);
        s.gameObject.transform.localPosition = offset;
        s.sprite.usesOverrideMaterial = true;
        s.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
		return s;
        //s.sprite.renderer.material.SetFloat("_Fade", 0.2f);
    }

    private static tk2dSprite CreateEffect(GameObject parent, string p, string name, bool flipX, bool flipY, Vector2 offset)
    {
        var UmbralLockEdge_BL = PrefabBuilder.BuildObject("UmbralLockIn" + name);
        var s = UmbralLockEdge_BL.AddComponent<tk2dSprite>();
        s.SetSprite(StaticSpriteDefinitions.VFX_Sheet_Data, p);
        s.SortingOrder = 1;
        s.HeightOffGround = -1;
        s.FlipX = flipX;
        s.FlipY = flipY;
        s.gameObject.transform.SetParent(parent.transform, false);
        s.gameObject.transform.localPosition = offset;
        s.sprite.usesOverrideMaterial = true;
        s.sprite.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
        return s;
    }


    public class UmbraVFX : MonoBehaviour
	{
		public tk2dSpriteAnimator InnerEye;
        public tk2dSpriteAnimator Lens;
		public AIActor Owner;
        float radius; 
        public void StartUp()
		{
			InnerEye.Play();
            Lens.Play();
            if (Owner)
            {
                radius = Mathf.Max(2.25f, Mathf.Min(9.5f, Mathf.Sqrt(Owner.healthHaver.GetMaxHealth()) * 0.5f));
            }
            //SpriteOutlineManager.AddOutlineToSprite(InnerEye.sprite, Color.black);
        }
        public void Kill()
        {
			IsDying = true;
            InnerEye.PlayAndDestroyObject("umbraleye_inner_close");
            Lens.PlayAndDisableObject("umbraleye_lens_close");
        }
        private float E = 0;
		public bool InRange = false;
        public LockOnEffect lockOnInst;
        public bool isActive = false;
        public void Update()
		{
			E += Time.deltaTime;
			if (IsDying)
			{
				Lens.transform.position = InnerEye.transform.position + new Vector3(UnityEngine.Random.Range(-0.0625f, 0.0625f), UnityEngine.Random.Range(-0.0625f, 0.0625f));
                Lens.transform.position = Lens.transform.position.WithZ(Lens.transform.position.z - 10);
                return;
            }
			bool player = true;
			var p = GetLookAt(ref player);
			if (player == true)
			{
				var direction = (p - InnerEye.transform.position).normalized * 0.12f;
                Lens.transform.position = InnerEye.transform.position + direction + new Vector3(UnityEngine.Random.Range(-0.0625f, 0.0625f), UnityEngine.Random.Range(-0.0625f, 0.0625f));
                Lens.transform.position = Lens.transform.position.WithZ(Lens.transform.position.z - 10);

            }
            else
			{
				Lens.transform.position = InnerEye.transform.position + new Vector3(UnityEngine.Random.Range(-0.0625f, 0.0625f), UnityEngine.Random.Range(-0.0625f, 0.0625f));
				Lens.transform.position = Lens.transform.position.WithZ(Lens.transform.position.z - 10);
            }
			if (lockOnInst)
			{
				lockOnInst.LockInPos = p + new Vector3(0.625f, 0.625f);
            }
            if (isActive == false) {return;}

			if (Vector2.Distance(p, Owner.sprite.WorldCenter.ToVector3ZisY()) < radius)
			{
                if (InRange == false)
                {
                    InRange = true;
                    if (lockOnInst == null)
                    {
                        lockOnInst = UnityEngine.Object.Instantiate(UmbralLockOnSmall).GetComponent<LockOnEffect>();
                        lockOnInst.LockInPos = p + new Vector3(0.625f, 0.625f);
                    }
                    AkSoundEngine.PostEvent("Play_ENM_iceslime_charge_01", lockOnInst.gameObject);
                    lockOnInst.SetState(true);
                }
            }
            else 
            {
                if (InRange == true)
				{
					if (lockOnInst != null)
					{
                        lockOnInst.SetState(false);
                        AkSoundEngine.PostEvent("Play_ENM_iceslime_charge_01", lockOnInst.gameObject);
                    }
                    InRange = false;
                }
            }



            
        }

        public void FixedUpdate()
        {
            E += 360 * Time.fixedDeltaTime;
            var m = Owner.sprite.WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle3(E, radius);
            var m_2 = Owner.sprite.WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle3(E + 120, radius);
            var m_3 = Owner.sprite.WorldCenter.ToVector3ZisY() + MathToolbox.GetUnitOnCircle3(E + 240, radius);

            GlobalSparksDoer.DoSingleParticle(m.WithZ(0), Vector3.up * UnityEngine.Random.Range(0.025f, 0.075f), 0.25f, 0.5f, Color.red, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            GlobalSparksDoer.DoSingleParticle(m_2.WithZ(0), Vector3.up * UnityEngine.Random.Range(0.025f, 0.075f), 0.25f, 0.5f, Color.red, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            GlobalSparksDoer.DoSingleParticle(m_3.WithZ(0), Vector3.up * UnityEngine.Random.Range(0.025f, 0.075f), 0.25f, 0.5f, Color.red, GlobalSparksDoer.SparksType.FLOATY_CHAFF);



            Vector2 vector = InnerEye.transform.position + new Vector3(1.5f, 0.75f);
            Vector2 vector2 = InnerEye.transform.position - new Vector3(1.5f, 0.75f);
            GlobalSparksDoer.DoRandomParticleBurst(2, vector, vector2, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(1, 4)), 0f, 0.5f, 0.125f, 1f, Color.black, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
        }


		public bool IsDying;

		public Vector3 GetLookAt(ref bool PlayerIsHere)
		{
			if (Owner == null)
			{
				return GameManager.Instance.GetActivePlayerClosestToPoint(this.transform.position).transform.position;
			}
			if (Owner.TargetRigidbody == null) { PlayerIsHere = false; return Vector2.zero; }
			return Owner.TargetRigidbody.transform.position;
		}
		public void OnDestroy()
		{

		}
    }

    public void UnBecomeUmbra()
    {
        if (isOomfing) { return; }
        isOomfing = true;
        this.StartCoroutine(DoWaitForKill());
    }
    private bool isOomfing = false;
    public IEnumerator DoWaitForKill()
    {
        while (IsGooning == true)
        {
            yield return null;
        }
        if (base.aiActor)
        {
            base.aiActor.healthHaver.OnPreDeath -= OnDeath;

            if (base.aiActor.bulletBank)
            {
                base.aiActor.bulletBank.OnProjectileCreated -= OnCreatedProjectile;
            }

            this.aiActor.AssignedCurrencyToDrop /= 3;
            base.aiActor.behaviorSpeculator.CooldownScale /= 0.7f;
            base.aiActor.MovementSpeed /= 1.15f;

            if (improvedAfterImage)
            {
                Destroy(improvedAfterImage);
            }

            umbraEffect.gameObject.transform.SetParent(null, true);
            umbraEffect.Kill();
            if (umbraEffect.lockOnInst)
            {
                umbraEffect.lockOnInst.SetState(false);
                Destroy(umbraEffect.lockOnInst, 0.5f);
            }
            UpdateBlackPhantomShaders(aiActor);

            base.aiActor.UnbecomeBlackPhantom();
            base.aiActor.BecomeBlackPhantom();

            Destroy(this);
        }
    }

    public Action<Vector2> OnDeath;
    public Action<Projectile> OnCreatedProjectile;

    private ImprovedAfterImage improvedAfterImage;

    public void Start()
	{
		base.aiActor.sprite.usesOverrideMaterial = true;
		base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
		base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", StaticTextures.NebulaTexture);
		base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
		base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
		base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
		base.aiActor.behaviorSpeculator.CooldownScale *= 0.7f;
        this.aiActor.AssignedCurrencyToDrop *= 3;

        base.aiActor.MovementSpeed *= 1.15f;
        improvedAfterImage = base.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
        improvedAfterImage.dashColor = Color.black;
        improvedAfterImage.spawnShadows = true;
        improvedAfterImage.shadowTimeDelay = 0.025f;
        improvedAfterImage.shadowLifetime = 1.5f;

        umbraEffect = base.aiActor.SmarterPlayEffectOnActor(UmbralEye, new Vector3(0, 1.5f, 3)).GetComponent<UmbraVFX>();
		umbraEffect.Owner = base.aiActor;

        this.StartCoroutine(DoWait());


        OnDeath = (A) =>
        {
            umbraEffect.gameObject.transform.SetParent(null, true);
            umbraEffect.Kill();
            if (umbraEffect.lockOnInst)
            {
                umbraEffect.lockOnInst.SetState(false);
                Destroy(umbraEffect.lockOnInst, 0.5f);
            }
        };
        OnCreatedProjectile = (A) =>
        {
            if (CooldownAttackBullet <= 0 && Ammo > 0)
            {
                if (umbraEffect.IsDying) { return; }
                Ammo--;
                CooldownAttackBullet = 1f;
                bool player = true;
                var start = umbraEffect.transform.position;
                var end = umbraEffect.GetLookAt(ref player);
                GameManager.Instance.StartCoroutine(DoAttackOfDoom(start, end));
            }
        };



        base.aiActor.healthHaver.OnPreDeath += OnDeath;
        if (base.aiActor.bulletBank)
        {
            base.aiActor.bulletBank.OnProjectileCreated += OnCreatedProjectile;
        }
    }


    private void UpdateBlackPhantomShaders(AIActor aIActor)
    {
        if (aIActor.healthHaver.bodySprites.Count != aIActor.m_cachedBodySpriteCount)
        {
            aIActor.m_cachedBodySpriteCount = aIActor.healthHaver.bodySprites.Count;
            for (int i = 0; i < aIActor.healthHaver.bodySprites.Count; i++)
            {
                tk2dBaseSprite tk2DBaseSprite = aIActor.healthHaver.bodySprites[i];
                tk2DBaseSprite.usesOverrideMaterial = true;
                Material material = tk2DBaseSprite.renderer.material;
                if (aIActor.m_cachedBodySpriteShader == null)
                {
                    aIActor.m_cachedBodySpriteShader = material.shader;
                }

                if (aIActor.OverrideBlackPhantomShader != null)
                {
                    material.shader = aIActor.OverrideBlackPhantomShader;
                }
                else
                {
                    material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    material.SetFloat("_PhantomGradientScale", aIActor.BlackPhantomProperties.GradientScale);
                    material.SetFloat("_PhantomContrastPower", aIActor.BlackPhantomProperties.ContrastPower);
                    if (tk2DBaseSprite != aIActor.sprite)
                    {
                        material.SetFloat("_ApplyFade", 0f);
                    }
                }
                tk2DBaseSprite.renderer.material = material;
            }
            if (aIActor.aiShooter && aIActor.aiShooter.CurrentGun)
            {
                tk2dBaseSprite sprite = aIActor.aiShooter.CurrentGun.GetSprite();
                sprite.usesOverrideMaterial = true;
                Material material2 = sprite.renderer.material;
                if (aIActor.m_cachedGunSpriteShader == null)
                {
                    aIActor.m_cachedGunSpriteShader = material2.shader;
                }
                if (aIActor.OverrideBlackPhantomShader != null)
                {
                    material2.shader = aIActor.OverrideBlackPhantomShader;
                }
                else
                {

                    material2.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    material2.SetFloat("_PhantomGradientScale", aIActor.BlackPhantomProperties.GradientScale);
                    material2.SetFloat("_PhantomContrastPower", aIActor.BlackPhantomProperties.ContrastPower);
                    material2.SetFloat("_ApplyFade", 0.3f);
                }
                sprite.renderer.material = material2;
            }

            base.aiActor.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
            base.aiActor.sprite.renderer.material.SetFloat("_PhantomGradientScale", aIActor.BlackPhantomProperties.GradientScale);
            base.aiActor.sprite.renderer.material.SetFloat("_PhantomContrastPower", aIActor.BlackPhantomProperties.ContrastPower);
            base.aiActor.sprite.renderer.material.SetFloat("_ApplyFade", 0.3f);
        }
    }


    public UmbraVFX umbraEffect;

    private bool IsGooning = true;

    public IEnumerator DoWait()
	{
        this.aiActor.SmarterPlayEffectOnActor(StaticVFXStorage.JammedDeathVFX, Vector2.zero).transform.localScale *= 1.6f;
        this.aiActor?.behaviorSpeculator?.Stun(1f);
        //AkSoundEngine.PostEvent("Play_ENM_cannonarmor_charge_01", this.aiActor.gameObject);
        AkSoundEngine.PostEvent("Play_ENM_cannonball_intro_01", this.aiActor.gameObject);

        this.aiActor.healthHaver.invulnerabilityPeriod = 3.5f;
        float cached = this.aiActor.MovementSpeed;
		this.aiActor.MovementSpeed = 0;
        umbraEffect.StartUp();
		float e = 0;

        float rng = UnityEngine.Random.Range(0.85f, 1.15f);

		while (e < 1)
		{
            e += Time.deltaTime * rng;

            if (base.aiActor && base.aiActor.specRigidbody)
            {
                Vector2 unitDimensions = base.aiActor.specRigidbody.HitboxPixelCollider.UnitDimensions;
                Vector2 a = unitDimensions * 0.5f;
                int num2 = Mathf.RoundToInt((float)12* 0.5f * Mathf.Min(30f, Mathf.Min(new float[]
                {
                unitDimensions.x * unitDimensions.y
                })));
				int num3 = 2;
                Vector2 vector = base.aiActor.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
                Vector2 vector2 = base.aiActor.specRigidbody.HitboxPixelCollider.UnitTopRight;
                PixelCollider pixelCollider = base.aiActor.specRigidbody.GetPixelCollider(ColliderType.Ground);
                if (pixelCollider != null && pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
                {
                    vector = Vector2.Min(vector, pixelCollider.UnitBottomLeft);
                    vector2 = Vector2.Max(vector2, pixelCollider.UnitTopRight);
                }
                vector += Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                vector2 -= Vector2.Min(a * 0.15f, new Vector2(0.25f, 0.25f));
                vector2.y -= Mathf.Min(a.y * 0.1f, 0.1f);
                GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 11f - (e * 10f)), 0f, 0.5f, 0.3f, 1, Color.black, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                GlobalSparksDoer.DoRandomParticleBurst(num3, vector, vector2, Vector2.up * (31f - (e * 30f)), 0f, 0.5f, 0.3f, 1, Color.black, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);

            }
            yield return null;
		}
        AkSoundEngine.PostEvent("Play_ENM_kali_blast_01", this.aiActor.gameObject);
        AkSoundEngine.PostEvent("Play_ENM_kali_blast_01", this.aiActor.gameObject);
        umbraEffect.isActive = true;
        Exploder.DoDistortionWave(this.transform.position, 100, 0.333f, 50f, 0.3f);
        e = 0;
		while (e < 1)
		{
			e += Time.deltaTime * 2;
            this.aiActor.MovementSpeed = Mathf.Lerp(0, cached, e);
            yield return null;
		}
        IsGooning = false;
        yield break;
	}


	public void Update()
    {
		if (!base.aiActor.IsBlackPhantom && base.aiActor != null && isOomfing == false)
        {
			base.aiActor.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
			base.aiActor.sprite.renderer.material.SetTexture("_EeveeTex", StaticTextures.NebulaTexture);
			base.aiActor.sprite.renderer.material.SetFloat("_StencilVal", 0);
			base.aiActor.sprite.renderer.material.SetFloat("_FlatColor", 0f);
			base.aiActor.sprite.renderer.material.SetFloat("_Perpendicular", 0);
			base.aiActor.BecomeBlackPhantom();
		}
        if (umbraEffect == null) { return; }
        if (umbraEffect.IsDying) { return; }
        if (CooldownAttackBullet > 0) { CooldownAttackBullet -= Time.deltaTime; }
        if (CooldownAttackRadius > 0) { CooldownAttackRadius -= Time.deltaTime; }

        if (Ammo < 6)
        {
            Restore += Time.deltaTime;
            if (Restore >= 0.5f)
            {
                Restore = 0;
                Ammo++;
            }
        }
        if (umbraEffect.InRange)
        {
            if (CooldownAttackRadius <= 0 && Ammo > 0)
            {
                Ammo--;
                CooldownAttackRadius = 0.25f;
                Restore -= 0.25f;
                bool player = true;
                var start = umbraEffect.transform.position;
                var end = umbraEffect.GetLookAt(ref player);
                GameManager.Instance.StartCoroutine(DoAttackOfDoom(start, end));
            }
        }
    }

    int Ammo = 0;
    public float Restore = 0;

    public float CooldownAttackRadius = 1.25f;
    public float CooldownAttackBullet = 0.5f;


    public override void OnDestroy()
	{
		if (base.aiActor)
		{
            GameObject effect = UnityEngine.Object.Instantiate(StaticVFXStorage.DragunBoulderLandVFX, this.transform.position, Quaternion.identity);
            Destroy(effect, 2.5f);
            for (int i = 0; i < 64; i++)
            {
                GlobalSparksDoer.DoSingleParticle(this.transform.position, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2.2f, 6.1f)), 0.25f, 4f, Color.red, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }
            base.aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, false, false, false);
            EnemyToolbox.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, OuroborosController.BulletBankDummy.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(Baboomer)), "Reflection");

        }

        base.OnDestroy();
	}

    public IEnumerator DoAttackOfDoom(Vector3 start, Vector3 end)
    {
        var t = UnityEngine.Object.Instantiate(UmbralLockOnFloor, end, Quaternion.identity).GetComponent<LockOnFloor>();
        t.SetState(true);
        var m = MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).normalized * 8;
        AkSoundEngine.PostEvent("Play_ENM_cannonarmor_charge_01", t.gameObject);


        yield return new WaitForSeconds(0.5f);

        AkSoundEngine.PostEvent("Play_ENM_creecher_burst_01", t.gameObject);

        for (int i = 0; i < 16; i++)
        {
            GlobalSparksDoer.DoSingleParticle(start, MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2.2f, 6.1f)),  0.5f, 0.35f, Color.red * 2, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
        }
        float e = 0;
        while (e < 1)
        {
            if (this == null)
            {
                t.SetState(false);
                Destroy(t.gameObject, 0.5f);
                yield break; }
            e += Time.deltaTime;
            var newPosition = Vector3.Lerp(start, end, e) + (m.ToVector3ZUp() * MathToolbox.EaseInAndBack(e));
            GlobalSparksDoer.DoSingleParticle(newPosition, Vector3.zero, (MathToolbox.EaseInAndBack(e) + 0.25f) * 0.5f, 2f, Color.red * 2, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            yield return null;
        }


        if (base.aiActor)
        {
            GameObject effect = UnityEngine.Object.Instantiate(StaticVFXStorage.DragunBoulderLandVFX, end, Quaternion.identity);
            Destroy(effect, 2.5f);
            AkSoundEngine.PostEvent("Play_ENM_bulletking_skull_01", t.gameObject);
            EnemyToolbox.SpawnBulletScript(base.aiActor, base.aiActor.sprite.WorldCenter, OuroborosController.BulletBankDummy.GetComponent<AIBulletBank>(), new CustomBulletScriptSelector(typeof(SmallSlam)), "Reflection");
        }


        t.SetState(false);
        Destroy(t.gameObject, 0.5f);
        yield break;
    }

	public class Baboomer : Script
	{
		public override IEnumerator Top()
		{
			base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);


            base.Fire(Offset.OverridePosition(base.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new Baboomer.BigBullet());

			yield break;
		}
		private class BigBullet : Bullet
		{
			public BigBullet() : base("big_one", false, false, true)
			{
			}

			public override void Initialize()
			{
				this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
				base.Initialize();
			}

			public override IEnumerator Top()
			{
				this.Projectile.specRigidbody.CollideWithTileMap = false;
				this.Projectile.specRigidbody.CollideWithOthers = false;
				yield return base.Wait(60);
				base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
				this.Speed = 0f;
				this.Projectile.spriteAnimator.Play();
				base.Vanish(true);
				yield break;
			}

			public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
			{
				if (!preventSpawningProjectiles)
				{
					
					float num = base.RandomAngle();
					float Amount = 16;
					float Angle = 360 / Amount;
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction((Angle * i) + num, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SmallBullet());
                    }
					num = base.RandomAngle();
					for (int i = 0; i < Amount; i++)
					{
						base.Fire(new Direction((Angle * i) + num, DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SmallBullet());
					}
					num = base.RandomAngle();
					base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
					return;
				}
			}
            private class SmallBullet : Bullet
			{
                public SmallBullet() : base("reversible", false, false, true)
                {
					
                }
				public override IEnumerator Top()
				{
					this.ChangeSpeed(new Brave.BulletScript.Speed(12,SpeedType.Absolute), 120);
					yield break;
				}
			}
        }
	}
    public class SmallSlam : Script
    {
        public override IEnumerator Top()
        {
            base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);

            float Aim = base.AimDirection;
            float delta = 30f;
            base.Fire(new Direction(0, DirectionType.Absolute), new Speed(0, SpeedType.Absolute), new SmallSlam.OopsANull("bigBullet", this, -1));
            
            delta = 60f;
            for (int j = 0; j < 6; j++)
            {
                base.Fire(new Direction(-(float)j * delta, DirectionType.Absolute), new Speed(2.25f, SpeedType.Absolute), new SmallSlam.OopsANull("sweep", this, 1));
            }

            for (int j = 0; j < 6; j++)
            {
                base.Fire(new Direction(-(float) j * delta, DirectionType.Absolute), new Speed(2.25f, SpeedType.Absolute), new SmallSlam.OopsANull("sweep", this, -1));
            }
            yield break;
        }
        public class OopsANull : Bullet
        {
            public OopsANull(string BulletType, SmallSlam parent, float angle = 0f, float aradius = 0) : base(BulletType, false, false, false)
            {
                this.m_parent = parent;
                this.m_angle = angle;
                this.m_radius = aradius;
                this.m_bulletype = BulletType;
                this.SuppressVfx = true;
            }

            public override IEnumerator Top()
            {
                this.ChangeDirection(new Brave.BulletScript.Direction(180 * m_angle, DirectionType.Relative), 60);
                yield return this.Wait(60);
                this.ChangeDirection(new Brave.BulletScript.Direction(180 * m_angle, DirectionType.Relative), 60);

                yield return this.Wait(60);
                base.Vanish(false);
                yield break;
            }

            private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
            {
                float delta = (newSpinSpeed - this.m_spinSpeed) / (float)term;
                for (int i = 0; i < term; i++)
                {
                    this.m_spinSpeed += delta;
                    yield return base.Wait(1);
                }
                yield break;
            }
            private const float ExpandSpeed = 4.5f;
            private const float SpinSpeed = 40f;
            private SmallSlam m_parent;
            private float m_angle;
            private float m_spinSpeed;
            private float m_radius;
            private string m_bulletype;

        }

    }


    public bool CanTeleport;
	public bool CanDash;
}




