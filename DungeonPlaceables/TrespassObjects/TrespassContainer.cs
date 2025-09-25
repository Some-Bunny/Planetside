using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using System.Collections;


namespace Planetside
{

    public class TrespassContainterInteractable: BraveBehaviour, IPlayerInteractable
    {
        public MajorBreakable self;

        private void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            if (self != null)
            {
                self.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
            }
            this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Round));
            this.m_room.RegisterInteractable(this);
        }

        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
            if (clip.GetFrame(frameIdx).eventInfo.Contains("SpawnPickups"))
            {
                AkSoundEngine.PostEvent("Play_PortalOpen", self.gameObject);
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                tk2dSpriteAnimator objanimator = silencerVFX.GetComponentInChildren<tk2dSpriteAnimator>();
                objanimator.ignoreTimeScale = true;
                objanimator.AlwaysIgnoreTimeScale = true;
                objanimator.AnimateDuringBossIntros = true;
                objanimator.alwaysUpdateOffscreen = true;
                objanimator.playAutomatically = true;
                ParticleSystem objparticles = silencerVFX.GetComponentInChildren<ParticleSystem>();
                var main = objparticles.main;
                main.useUnscaledTime = true;
                GameObject vfx = GameObject.Instantiate(silencerVFX.gameObject, self.sprite.WorldCenter, Quaternion.identity);
                Destroy(vfx, 2);
                for (int i = 0; i < 3; i++)
                {
                    int id = BraveUtility.RandomElement<int>(RobotShopkeeperBoss.Lootdrops);
                    DebrisObject pickups = LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, self.transform.position + new Vector3(0.25f, 0), MathToolbox.GetUnitOnCircle(120 * i, 1), 4f, false, true, false);
                }
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("SpawnItem"))
            {
                LootEngine.SpawnItem(UnityEngine.Random.value > 0.5f ? PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(UnityEngine.Random.Range(1, 100)), new List<int> { }, new PickupObject.ItemQuality[] {  PickupObject.ItemQuality.C, PickupObject.ItemQuality.C, PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S }).gameObject : PickupObjectDatabase.GetRandomPassiveOfQualities(new System.Random(UnityEngine.Random.Range(1, 100)), new List<int> { }, new PickupObject.ItemQuality[] { PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S }).gameObject, self.transform.position + new Vector3(0.25f, 0), Vector2.down, 2f, false, true, false);
            }
        }


        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite) return float.MaxValue;
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
        }




        public void Interact(PlayerController interactor)
        {
            self.spriteAnimator.Play("trespassContainer_break");
            this.m_room.DeregisterInteractable(this);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetOverrideMaxDistance()
        {
            return 2.5f;
        }

        public override void OnDestroy()
        {
            if (this.m_room.GetRoomInteractables().Contains(this))
            {
                this.m_room.DeregisterInteractable(this);
            }
            base.OnDestroy();
        }
        private RoomHandler m_room;
    }


    public class TrespassContainer
    {
        public static void Init()
        {

            var tearObject = Alexandria.PrefabAPI.PrefabBuilder.BuildObject("Void Container");
            var sprite = tearObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.Trespass_Room_Object_Data, "trespassContainer_idle_001");
            sprite.IsPerpendicular = false;

            var animator = tearObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.Trespass_Room_Object_Animation;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.Trespass_Room_Object_Animation.GetClipIdByName("trespassContainer_idle");

            var majorBreakable = tearObject.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;

            tearObject.CreateFastBody(new IntVector2(26, 30), new IntVector2(3, -4), CollisionLayer.HighObstacle);
            tearObject.CreateFastBody(new IntVector2(26, 30), new IntVector2(3, -4), CollisionLayer.BeamBlocker);
            tearObject.CreateFastBody(new IntVector2(26, 30), new IntVector2(3, -4), CollisionLayer.BulletBlocker);
            tearObject.CreateFastBody(new IntVector2(26, 30), new IntVector2(3, -4), CollisionLayer.EnemyBlocker);
            tearObject.CreateFastBody(new IntVector2(26, 30), new IntVector2(3, -4), CollisionLayer.PlayerBlocker);






            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            sprite.usesOverrideMaterial = true;
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 8f);
            mat.SetFloat("_EmissivePower", 4);
            sprite.renderer.material = mat;

            majorBreakable.DamageReduction = 1000;



            majorBreakable.gameObject.AddComponent<TrespassContainterInteractable>();
            majorBreakable.gameObject.AddComponent<DungeonPlaceableBehaviour>();
            majorBreakable.DamageReduction = 1000;
            EnemyToolbox.AddEventTriggersToAnimation(majorBreakable.spriteAnimator, "trespassContainer_break", new Dictionary<int, string> { {4, "SpawnPickups"}, { 6, "SpawnItem" } });

            StaticReferences.StoredRoomObjects.Add("trespassContainer", majorBreakable.gameObject);
        }
    }
}
