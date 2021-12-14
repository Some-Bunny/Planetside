    using System;
using System.Collections;
using ItemAPI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using SaveAPI;

namespace Planetside
{
    public class ShelltansBlessing : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shelltans Blessing";
            string resourceName = "Planetside/Resources/shelltansblessing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShelltansBlessing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Defiled";
            string longDesc = "A piece of a long-defiled Shelltan shrine." +
                "\n\nAlthough its power is weak, the power originally found within still lingers, waiting...";
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.C;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:shelltans_blessing",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "ammo_synthesizer",
                "zombie_bullets",
                "bloody_9mm",
                "holey_grail",
                "bullet_idol",
                "sixth_chamber",
                "yellow_chamber"
            };
            CustomSynergies.Add("Invigorated", mandatoryConsoleIDs, optionalConsoleIDs, true);
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED, true);
            item.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
            ShelltansBlessing.ShelltainsBlessingID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int ShelltainsBlessingID;
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            PlayerController player = base.Owner;
            if (base.Owner != null)
            {
                bool deathed = fatal;
                if (deathed)
                {
                    this.random = UnityEngine.Random.Range(0.00f, 1.00f);
                    float Value = 0.05f + (player.CurrentGun.AdjustedMaxAmmo / 2500);
                    if (Value >= 1) { Value = 1; }
                    if (player.CurrentGun.quality == PickupObject.ItemQuality.A | player.CurrentGun.quality == PickupObject.ItemQuality.S)
                    {
                        Value *= 0.75f;
                    }
                    if (random <= Value)
                    {
                        bool flagA = player.PlayerHasActiveSynergy("Invigorated");
                        if (flagA)
                        {
                            for (int i = 0; i < player.inventory.AllGuns.Count; i++)
                            {
                                if (player.inventory.AllGuns[i] && player.CurrentGun != player.inventory.AllGuns[i])
                                {
                                    player.inventory.AllGuns[i].GainAmmo(Mathf.FloorToInt((float)player.inventory.AllGuns[i].AdjustedMaxAmmo * 0.02f));
                                }
                            }
                        }
                        else
                        {
                            player.inventory.CurrentGun.GainAmmo(Mathf.FloorToInt((float)player.inventory.CurrentGun.AdjustedMaxAmmo * 0.04f));
                        }
                        player.CurrentGun.ForceImmediateReload(false);
                    }
                }
            }
            
		}

		public override DebrisObject Drop(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            DebrisObject result = base.Drop(player);
			return result;
		}
        private float random;
        public override void Pickup(PlayerController player)
		{
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            base.Pickup(player);
		}
		protected override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.Owner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
		}
	}
}