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
using static UnityEngine.UI.GridLayoutGroup;


namespace Planetside
{
    public class OffWorldMedicine : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Off-World Medicine";
            //string resourceName = "Planetside/Resources/offworldmedicine.png";
            GameObject obj = new GameObject(itemName);
            OffWorldMedicine activeitem = obj.AddComponent<OffWorldMedicine>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("offworldmedicine"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I'll Show You Off-World";
            string longDesc = "An off-world medicine from a high-tech civilzation, capable of curing even the most sturdiest addictions and festering wounds.\nIf wounds are fatal, apply with EXTREME care.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 5f);
            activeitem.consumable = true;
            activeitem.quality = PickupObject.ItemQuality.A;

            activeitem.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            OffWorldMedicine.OffWorldMedicineID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            GameManager.Instance.RainbowRunForceExcludedIDs.Add(activeitem.PickupObjectId);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

        }
        public static int OffWorldMedicineID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
        }

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            IsKilled = true;
            float num = -1f;
            bool flag = this.Use(base.LastOwner, out num);
            for (int i = base.LastOwner.activeItems.Count - 1; i > -1; i--)
            {
                if (base.LastOwner.activeItems[i] == this)
                {
                    UnityEngine.Object.Destroy(base.LastOwner.activeItems[i].gameObject);
                    base.LastOwner.activeItems.RemoveAt(i);
                }
            }

            base.LastOwner.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }
        public bool IsKilled = false;
        public override void OnPreDrop(PlayerController user)
        {
            user.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            base.OnPreDrop(user);
        }
        public void DoHealUp(PlayerController user, bool isFatal = false)
        {
            user.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;

            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", base.gameObject);
            user.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
            user.healthHaver.FullHeal();
            if (user.ForceZeroHealthState == true)
            {
                user.healthHaver.Armor = 6;
            }
            user.spiceCount = 0;
            if (isFatal == true)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, false);
                user.ToggleGunRenderers(true, "non-death");
                user.ToggleHandRenderers(true, "non-death");
                user.CurrentInputState = PlayerInputState.AllInput;
                user.spriteAnimator.enabled = true;
                user.StartCoroutine(DoDelayedBoost(user));

                OtherTools.ApplyStat(user, PlayerStats.StatType.Damage, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                OtherTools.ApplyStat(user, PlayerStats.StatType.MovementSpeed, 0.90f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                user.stats.RecalculateStats(base.LastOwner, false, false);
            }
            if (user.ForceZeroHealthState == true)
            {
                OtherTools.NotifyCustom(isFatal == true ? "Integrity Compromised" : "Administering Repair", isFatal == true ? "Administering Emergency Repair Nanites" : "Continue As Per Protocol", "offworldmedicine", StaticSpriteDefinitions.Active_Item_Sheet_Data, UINotificationController.NotificationColor.GOLD);
            }
            else
            {
                OtherTools.NotifyCustom(isFatal == true ? "Fatal Injuries Detected" : "Administering Aid", isFatal == true ? "Administering Adrenaline" : "Have A Good Day.", "offworldmedicine", StaticSpriteDefinitions.Active_Item_Sheet_Data, UINotificationController.NotificationColor.GOLD);

            }
        }

        public IEnumerator DoDelayedBoost(PlayerController user)
        {
            yield return null;
            yield return null;
            yield return null;

            GameManager.Instance.MainCameraController.SetManualControl(false, false);
            user.ToggleGunRenderers(true, "non-death");
            user.ToggleHandRenderers(true, "non-death");
            user.CurrentInputState = PlayerInputState.AllInput;
            user.spriteAnimator.enabled = true;

            user.ownerlessStatModifiers.Add(new StatModifier()
            {
                statToBoost = PlayerStats.StatType.Damage,
                amount = 1.8f,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                isMeatBunBuff = true,
            });
            user.ownerlessStatModifiers.Add(new StatModifier()
            {
                statToBoost = PlayerStats.StatType.MovementSpeed,
                amount = 1.4f,
                modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                isMeatBunBuff = true,
            });
            user.PlayEffectOnActor(StaticVFXStorage.EnemyZappyTellVFX, Vector3.zero);
            user.stats.RecalculateStats(base.LastOwner, false, false);

            for (int e = 0; e < 3; e++)
            {
                for (int i = 0; i < 20; i++)
                {
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", user.gameObject);
            }

            yield break;
        }


        public override void DoEffect(PlayerController user)
        {
            DoHealUp(user, IsKilled);
        }
    }
}



