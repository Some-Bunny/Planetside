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

    public class TrespassDecorativePillarInetractable : BraveBehaviour, IPlayerInteractable
    {
        public MajorBreakable self;

        private void Start()
        {
            self = base.gameObject.GetComponent<MajorBreakable>();
            this.m_room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
            this.m_room.RegisterInteractable(this);
        }


        public void OnEnteredRange(PlayerController interactor)
        {
            //SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            //SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            //SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            //SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
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

        public void Interact(PlayerController interactor)
        {
            //self.spriteAnimator.Play("break");
            //this.m_room.DeregisterInteractable(this);
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


    public class TrespassDecorativePillar
    {
        public static void Init()
        {
            string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassBigObject/";
            string[] idlePaths = new string[]
            {
                defaultPath+"vesselLock_idle_001.png",
                defaultPath+"vesselLock_idle_002.png",
                defaultPath+"vesselLock_idle_003.png",
                defaultPath+"vesselLock_idle_004.png",
                defaultPath+"vesselLock_idle_005.png",
                defaultPath+"vesselLock_idle_006.png",
                defaultPath+"vesselLock_idle_007.png",

            };
            string[] breakPaths = new string[]
            {
                defaultPath+"vesselLock_use_001.png",
                defaultPath+"vesselLock_use_002.png",
                defaultPath+"vesselLock_use_003.png",
                defaultPath+"vesselLock_use_004.png",
                defaultPath+"vesselLock_use_005.png",
                defaultPath+"vesselLock_use_006.png",
                defaultPath+"vesselLock_use_007.png",
                defaultPath+"vesselLock_use_008.png",
                defaultPath+"vesselLock_use_009.png",
                defaultPath+"vesselLock_use_010.png",
                defaultPath+"vesselLock_use_011.png",

            };
            MajorBreakable statue = BreakableAPIToolbox.GenerateMajorBreakable("trespass_decorative_pillar", idlePaths, 6, breakPaths, 10, 15000, true, 24, 24, 4, -4, true, null, null, true, null);
            BreakableAPIToolbox.GenerateShadow("Planetside/Resources/DungeonObjects/TrespassObjects/TrespassContainer/trespassContainer_shadow.png", "trespass_decorative_pillar_shadow", statue.gameObject.transform, new Vector3(0, -0.1875f));

            statue.gameObject.AddComponent<TresspassLightController>();
            statue.gameObject.AddComponent<TrespassDecorativePillarInetractable>();
            statue.gameObject.AddComponent<DungeonPlaceableBehaviour>();
            statue.DamageReduction = 1000;

            StaticReferences.StoredRoomObjects.Add("trespassDecorativePillar", statue.gameObject);
        }
    }
}
