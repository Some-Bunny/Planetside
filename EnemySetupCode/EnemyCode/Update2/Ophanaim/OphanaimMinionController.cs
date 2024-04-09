using System;
using System.Collections.Generic;
using Gungeon;
using ItemAPI;
using UnityEngine;

using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using System.Collections;
using Dungeonator;
using System.Linq;
using Brave.BulletScript;
using DirectionType = Brave.BulletScript.DirectionType;

namespace Planetside
{
    public class OphanaimMinionController : BraveBehaviour
    {
        public void Start()
        {
            bodyPartController = this.GetComponent<AdvancedBodyPartController>();
            bodyPartController.healthHaver.SetHealthMaximum(50);
            bodyPartController.healthHaver.ForceSetCurrentHealth(50);
            bodyPartController.OnBodyPartPreDeath += (obj1, obj2, obj3) =>
            {
                var l = bodyPartController.MainBody.gameObject.GetComponent<Ophanaim.EyeEnemyBehavior>().eyes;
                if (l.Contains(this.gameObject)) { l.Remove(this.gameObject); }
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", base.gameObject);
                var onnn = UnityEngine.Object.Instantiate<GameObject>(Ophanaim.SolarClap, obj1.transform.position, Quaternion.identity, null);
                Destroy(onnn, 1.5f);
            };

            GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "eye_gun_laser", "eye_gun_simple", "eye_gun_predict", "eye_shot_hide", "eye_shot_appear", "eye_gun_laser_blue", "eye_gun_simple_blue", "eye_gun_predict_blue" }, OnMessageRecieved);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, new Color(10f, 10f, 10f), 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

       

        public void DoPoof()
        {
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), base.sprite.WorldCenter, Quaternion.identity);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor(base.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.ignoreTimeScale = true;
                component2.AlwaysIgnoreTimeScale = true;
                component2.AnimateDuringBossIntros = true;
                component2.alwaysUpdateOffscreen = true;
                component2.playAutomatically = true;
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 3f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", Color.white);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.white);
            }
            Destroy(gameObject, 1.5f);
        }
        public void OnMessageRecieved(GameObject obj, string message)
        {
           if (message.Contains("eye_gun_"))
            {
                base.sprite.IsOutlineSprite = false;
                BulletScriptSource bulletScriptSource = gameObject.GetOrAddComponent<BulletScriptSource>();
                bulletScriptSource.BulletManager = base.GetComponent<AIBulletBank>();
                bulletScriptSource.BulletScript = ReturnScript(message);
                bulletScriptSource.Initialize();
            }


            if (message == "eye_shot_hide")
            {
                DoPoof();
                this.gameObject.GetComponent<ImprovedAfterImage>().spawnShadows = false;
                SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, false);
                this.specRigidbody.enabled = false;
                bodyPartController.Render = false;
            }
            if (message == "eye_shot_appear")
            {
                DoPoof();
                this.gameObject.GetComponent<ImprovedAfterImage>().spawnShadows = true;
                SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, true);
                this.specRigidbody.enabled = true;
                bodyPartController.Render = true;

            }
        }

        public static CustomBulletScriptSelector ReturnScript(string s)
        {

            if (s == "eye_gun_laser_blue") { return new CustomBulletScriptSelector(typeof(MinionShotBlue)); }
            if (s == ("eye_gun_simple_blue")) { return new CustomBulletScriptSelector(typeof(ShotBlue)); }
            if (s == ("eye_gun_predict_blue")) { return new CustomBulletScriptSelector(typeof(MinionShotPredictiveBlue)); }

            if (s == ("eye_gun_laser")) { return new CustomBulletScriptSelector(typeof(MinionShot)); }
            if (s == ("eye_gun_simple")) { return  new CustomBulletScriptSelector(typeof(Shot)); }
            if (s == ("eye_gun_predict")) { return new CustomBulletScriptSelector(typeof(MinionShotPredictive)); }

            return new CustomBulletScriptSelector(typeof(MinionShot));
        }
        public AdvancedBodyPartController bodyPartController;
    }


    public class ShotBlue : Shot {
        public override bool IsBlue
        {
            get
            {
                return true;
            }
        }
    }

    public class Shot : Script
    {
        public virtual bool IsBlue
        {
            get
            {
                return false;
            }
        }
        public override IEnumerator Top()
        {
            float r = BraveUtility.RandomAngle();
            for (int i =0; i < 4;i++)
            {
                string s = IsBlue == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "frogger";

                base.Fire(Offset.OverridePosition(this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter), new Direction(r + (90*i), DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet(s, 15, 120));
            }
            yield break;
        }
    }

    public class MinionShotBlue : MinionShot
    {
        public override bool IsBlue
        {
            get
            {
                return true;
            }
        }
    }

    public class MinionShot : Script
    {
        public virtual bool IsBlue
        {
            get
            {
                return false;
            }
        }

        public override IEnumerator Top()
        {
            float Angle = base.AimDirection;
            GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
            component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
            component2.dimensions = new Vector2(1000f, 1f);
            component2.UpdateZDepth();
            component2.HeightOffGround = -2;
            Color laser = IsBlue == true ? new Color(0, 0.25f, 1f) : new Color(1, 0.85f, 0.7f);
            component2.sprite.usesOverrideMaterial = true;
            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
            component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
            component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

            GameManager.Instance.StartCoroutine(FlashReticles(component2, Angle, this));
            yield return this.Wait(75 * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier());
            yield break;
        }
        private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject,float Angle, MinionShot parent)
        {
            tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
            float elapsed = 0;
            float Time = 0.25f;
            while (elapsed < Time)
            {
                float t = (float)elapsed / (float)Time;
                if (this == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break; 
                }
                if (parent.IsEnded || parent.Destroyed)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank.gameObject == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }

                if (tiledspriteObject != null)
                {
                    tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;


                    tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (75 * t));
                    tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));
                    tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
                    tiledsprite.HeightOffGround = -2;
                    tiledsprite.renderer.gameObject.layer = 22;
                    tiledsprite.dimensions = new Vector2(1000f, 1f);
                    tiledsprite.UpdateZDepth();
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            elapsed = 0;
            Time = 0.5f;
            while (elapsed < Time)
            {
                if (this == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (parent.IsEnded || parent.Destroyed)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank.gameObject == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (tiledspriteObject != null)
                {
                    tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;
                    tiledsprite.dimensions = new Vector2(1000f, 1f);
                    tiledsprite.HeightOffGround = -2;
                    tiledsprite.renderer.gameObject.layer = 22;
                    tiledsprite.UpdateZDepth();
                    bool enabled = elapsed % 0.25f > 0.125f;
                    tiledsprite.sprite.renderer.enabled = enabled;
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            UnityEngine.Object.Destroy(tiledspriteObject.gameObject);

            if (base.BulletBank != null)
            {
                string s = IsBlue == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "frogger";

                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter), new Direction(Angle, DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 90));
                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter), new Direction(Angle, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 90));
                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter),new Direction(Angle, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 90));
            }
            yield break;
        }
    }

    public class MinionShotPredictiveBlue : MinionShotPredictive {
        public override bool IsBlue
        {
            get
            {
                return true;
            }
        }
    }

    public class MinionShotPredictive : Script
    {
        public virtual bool IsBlue
        {
            get
            {
                return false;
            }
        }

        public override IEnumerator Top()
        {
            float Angle = base.AimDirection;
            GameObject gameObject = SpawnManager.SpawnVFX(RandomPiecesOfStuffToInitialise.LaserReticle, false);

            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
            component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
            component2.dimensions = new Vector2(1000f, 1f);
            component2.UpdateZDepth();
            component2.HeightOffGround = -2;
            Color laser =  IsBlue == true ? new Color(0, 0.25f, 1f) : new Color(1, 0.85f, 0.7f);
            component2.sprite.usesOverrideMaterial = true;
            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.5f);
            component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
            component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

            GameManager.Instance.StartCoroutine(FlashReticles(component2, this));
            yield return this.Wait(90 * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier());
            yield break;
        }
        private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, MinionShotPredictive parent)
        {
            tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
            float elapsed = 0;
            float Time = 0.75f;
            float f = 0;

            while (elapsed < Time)
            {

                float t = (float)elapsed / (float)Time;
                if (this == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (parent.IsEnded || parent.Destroyed)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank.gameObject == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }

                if (tiledspriteObject != null)
                {
                    tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;
                    tiledsprite.sprite.renderer.material.SetFloat("_EmissivePower", 10 * (35 * t));
                    tiledsprite.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.25f + (10 * t));

                    Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter, 40f);
                    float CentreAngle = (predictedPosition - this.Position).ToAngle();
                    f = CentreAngle;
                    tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, CentreAngle);

                    tiledsprite.HeightOffGround = -2;
                    tiledsprite.renderer.gameObject.layer = 22;
                    tiledsprite.dimensions = new Vector2(1000f, 1f);
                    tiledsprite.UpdateZDepth();
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            elapsed = 0;
            Time = 0.5f;
            while (elapsed < Time)
            {
                if (this == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (parent.IsEnded || parent.Destroyed)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }
                if (this.BulletBank.gameObject == null)
                {
                    UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                    yield break;
                }

                if (tiledspriteObject != null)
                {
                    tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;
                    tiledsprite.dimensions = new Vector2(1000f, 1f);
                    tiledsprite.HeightOffGround = -2;
                    tiledsprite.renderer.gameObject.layer = 22;
                    tiledsprite.UpdateZDepth();
                    bool enabled = elapsed % 0.25f > 0.125f;
                    tiledsprite.sprite.renderer.enabled = enabled;
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            UnityEngine.Object.Destroy(tiledspriteObject.gameObject);

            if (base.BulletBank != null)
            {
                base.PostWwiseEvent("Play_BOSS_doormimic_blast_01", null);
                string s = IsBlue == true ? StaticUndodgeableBulletEntries.UndodgeableFrogger.Name : "frogger";

                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter), new Direction(f, DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 60));
                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter), new Direction(f, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 60));
                base.Fire(Offset.OverridePosition(this.BulletBank.sprite.WorldCenter), new Direction(f, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new SpeedChangingBullet(s, 25, 60));
            }
            yield break;
        }
    }

}
