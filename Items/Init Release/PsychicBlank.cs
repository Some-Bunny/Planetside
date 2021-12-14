using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;


namespace Planetside
{
    public class PsychicBlank  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Psychic Blank";
            string resourceName = "Planetside/Resources/psychicblank.png";
            GameObject obj = new GameObject(itemName);
			PsychicBlank activeitem = obj.AddComponent<PsychicBlank>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Astral Rejection";
            string longDesc = "Creates a blank effect on the players cursor.\n\nA forgotten art once used by Wizbangs as a form of defence against intruding Gungeoneers.\n\nSeems relatively easy to learn...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 500f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            activeitem.AddToSubShop(ItemBuilder.ShopType.OldRed, 1f);

            PsychicBlank.PsychicBlankID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int PsychicBlankID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {
            bool flag = BraveInput.GetInstanceForPlayer(user.PlayerIDX).IsKeyboardAndMouse(false);
            if (flag)
            {
                this.aimpoint = user.unadjustedAimPoint.XY();
            }
            else
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(user.PlayerIDX);
                Vector2 a = user.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
                a += instanceForPlayer.ActiveActions.Aim.Vector * 8f * BraveTime.DeltaTime;
                this.m_currentAngle = BraveMathCollege.Atan2Degrees(a - user.CenterPosition);
                this.m_currentDistance = Vector2.Distance(a, user.CenterPosition);
                this.m_currentDistance = Mathf.Min(this.m_currentDistance, 15f);
                this.aimpoint = user.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
            }
            this.DoMicroBlank(aimpoint, 0f);
        }
        private void DoMicroBlank(Vector2 center, float knockbackForce = 30f)
        {
            PlayerController owner = base.LastOwner;
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
            GameObject gameObject = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = 0.25f;
            silencerInstance.TriggerSilencer(center, 25f, 5f, silencerVFX, 0f, 3f, 3f, 3f, 250f, 5f, additionalTimeAtMaxRadius, owner, false, false);
        }
        private float m_currentAngle;
        private float m_currentDistance;
        private Vector2 aimpoint;
    }
}



