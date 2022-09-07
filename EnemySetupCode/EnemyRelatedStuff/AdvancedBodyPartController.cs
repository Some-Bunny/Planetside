using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using static Planetside.CoinTosser;

namespace Planetside
{
    public class AdvancedBodyPartController : BraveBehaviour
    {
        private void HealthHaver_OnPreDeathAIActor(Vector2 obj)
        {
            HostBodyIsDead = true;
            if (ownHealthHaver != null)
            {
                ownHealthHaver.ApplyDamage(int.MaxValue, new Vector2(0, 0), "get fucked");
            }
            if (OnHostPreDeath != null)
            {
                OnHostPreDeath(MainBody, obj);
            }
        }


        private void OwnHealthHaver_OnDeathAIActor(Vector2 obj)
        {
            if (OnHostDeath != null)
            {
                OnHostDeath(MainBody, obj);
            }
        }


        private void OwnHealthHaver_OnDamagedAIActor(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (OnHostDamaged != null)
            {
                OnHostDamaged(MainBody, resultValue, maxValue, damageTypes, damageCategory, damageDirection);
            }
        }


        public System.Action<AIActor, Vector2> OnHostPreDeath, OnHostDeath;
        public Action<AIActor, float, float, CoreDamageTypes, DamageCategory, Vector2> OnHostDamaged;

        public virtual void Start()
        {
            this.MainBody = base.transform.parent.GetComponent<AIActor>();
            this.renderer.enabled = Render;

            if (!this.ownBody)
            {
                ownBody = this.gameObject.GetComponent<SpeculativeRigidbody>();
            }
            if (!this.ownHealthHaver)
            {
                ownHealthHaver = this.gameObject.GetComponent<HealthHaver>();
            }


            this.m_heightOffBody = base.sprite.HeightOffGround;
            if (this.hasOutlines)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, base.sprite.HeightOffGround + 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
                if (this.MainBody)
                {
                    ObjectVisibilityManager component = this.MainBody.GetComponent<ObjectVisibilityManager>();
                    if (component)
                    {
                        component.ResetRenderersList();
                    }
                }
            }
            if (!base.specRigidbody)
            {
                base.specRigidbody = this.MainBody.specRigidbody;
            }

            MainBody = this.gameObject.transform.parent.GetComponent<AIActor>();

            if (MainBody != null)
            {
                MainBody.healthHaver.OnPreDeath += HealthHaver_OnPreDeathAIActor;
                MainBody.healthHaver.OnDamaged += OwnHealthHaver_OnDamagedAIActor;
                MainBody.healthHaver.OnDeath += OwnHealthHaver_OnDeathAIActor;
            }


            if (ownBody != null && ownHealthHaver != null)
            {
                ownHealthHaver.OnDamaged += OwnHealthHaver_OnDamaged;
                ownHealthHaver.OnPreDeath += OwnHealthHaver_OnPreDeath;
                ownHealthHaver.OnDeath += OwnHealthHaver_OnDeath;
            }

        }

        private void OwnHealthHaver_OnDeath(Vector2 obj)
        {
            if (OnBodyPartDeath != null)
            {
                OnBodyPartDeath(ownBody, ownHealthHaver, obj);
            }
        }

        private void OwnHealthHaver_OnPreDeath(Vector2 obj)
        {
            if (OnBodyPartPreDeath != null)
            {
                OnBodyPartPreDeath(ownBody, ownHealthHaver, obj);
            }
        }

        private void OwnHealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (OnBodyPartDamaged != null)
            {
                OnBodyPartDamaged(ownHealthHaver, ownBody, resultValue, maxValue, damageTypes, damageCategory, damageDirection);
            }
        }

        public bool Render = true;
        public virtual void Update()
        {
            float num;
            if (ownBody != null)
            {
                ownBody.Reinitialize();
                ownBody.healthHaver.sprite.ForceUpdateMaterial();
            }

            this.renderer.enabled = Render;

            if (!this.OverrideFacingDirection && this.faceTarget && this.TryGetAimAngle(out num))
            {
                if (this.faceTargetTurnSpeed > 0f)
                {
                    float current = (!base.aiAnimator) ? base.transform.eulerAngles.z : base.aiAnimator.FacingDirection;
                    num = Mathf.MoveTowardsAngle(current, num, this.faceTargetTurnSpeed * BraveTime.DeltaTime);
                }
                if (base.aiAnimator)
                {
                    base.aiAnimator.LockFacingDirection = true;
                    base.aiAnimator.FacingDirection = num;
                }
                else
                {
                    base.transform.rotation = Quaternion.Euler(0f, 0f, num);
                }
            }
            if (this.autoDepth && base.aiAnimator)
            {
                float num2 = BraveMathCollege.ClampAngle180(this.MainBody.aiAnimator.FacingDirection);
                float num3 = BraveMathCollege.ClampAngle180(base.aiAnimator.FacingDirection);
                bool flag = num2 <= 155f && num2 >= 25f && num3 <= 155f && num3 >= 25f;
                base.sprite.HeightOffGround = ((!flag) ? this.m_heightOffBody : (-this.m_heightOffBody));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected virtual bool TryGetAimAngle(out float angle)
        {
            angle = 0f;
            if (this.MainBody.TargetRigidbody)
            {
                Vector2 unitCenter = this.MainBody.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                Vector2 b = base.transform.position.XY();
                if (this.aimFrom == BodyPartController.AimFromType.ActorHitBoxCenter)
                {
                    b = this.MainBody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                }
                angle = (unitCenter - b).ToAngle();
                return true;
            }
            if (this.MainBody.aiAnimator)
            {
                angle = this.MainBody.aiAnimator.FacingDirection;
                return true;
            }
            return false;
        }




        public string Name =  "BodyPart";

        public SpeculativeRigidbody ownBody;

        public HealthHaver ownHealthHaver;

        public System.Action<SpeculativeRigidbody, HealthHaver, Vector2> OnBodyPartPreDeath, OnBodyPartDeath;
        public Action<HealthHaver, SpeculativeRigidbody, float, float, CoreDamageTypes, DamageCategory, Vector2> OnBodyPartDamaged;


        public int intPixelCollider;
        public bool hasOutlines;
        public bool faceTarget;
        [ShowInInspectorIf("faceTarget", true)]
        public float faceTargetTurnSpeed = -1f;

        [ShowInInspectorIf("faceTarget", true)]
        public BodyPartController.AimFromType aimFrom = BodyPartController.AimFromType.Transform;
        public bool autoDepth = true;
        public bool redirectHealthHaver;
        public bool independentFlashOnDamage;
        public AIActor MainBody;
        private float m_heightOffBody;
        public bool HostBodyIsDead = false;
        public bool OverrideFacingDirection { get; set; }

        public enum AimFromType
        {
            Transform = 10,
            ActorHitBoxCenter = 20
        }
    }
}
