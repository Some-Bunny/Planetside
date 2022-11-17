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
            self.spriteAnimator.Play("break");
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

        protected override void OnDestroy()
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
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassContainer/";
            string[] idlePaths = new string[]
            {
                defaultPath+"trespassContainer_idle_001.png",
                defaultPath+"trespassContainer_idle_002.png",
                defaultPath+"trespassContainer_idle_003.png",
                defaultPath+"trespassContainer_idle_004.png",
                defaultPath+"trespassContainer_idle_005.png",
                defaultPath+"trespassContainer_idle_006.png",
                defaultPath+"trespassContainer_idle_007.png",
                defaultPath+"trespassContainer_idle_008.png",
            };
            string[] breakPaths = new string[]
            {
                defaultPath+"trespassContainer_break_001.png",
                defaultPath+"trespassContainer_break_002.png",
                defaultPath+"trespassContainer_break_003.png",
                defaultPath+"trespassContainer_break_004.png",
                defaultPath+"trespassContainer_break_005.png",
                defaultPath+"trespassContainer_break_006.png",
                defaultPath+"trespassContainer_break_007.png",
                defaultPath+"trespassContainer_break_008.png",
                defaultPath+"trespassContainer_break_009.png",
            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_container", idlePaths, 6, breakPaths, 16, 15000, true, 28, 24, 2, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow(defaultPath + "trespassContainer_shadow.png", "trespass_container_shadow", statue.gameObject.transform, new Vector3(0f, -0.1875f));

            statue.gameObject.AddComponent<TresspassLightController>();
            statue.gameObject.AddComponent<TrespassContainterInteractable>();
            statue.gameObject.AddComponent<DungeonPlaceableBehaviour>();
            statue.DamageReduction = 1000;
            EnemyToolbox.AddEventTriggersToAnimation(statue.spriteAnimator, "break", new Dictionary<int, string> { {4, "SpawnPickups"}, { 7, "SpawnItem" } });


            StaticReferences.StoredRoomObjects.Add("trespassContainer", statue.gameObject);
        }
    }
}
