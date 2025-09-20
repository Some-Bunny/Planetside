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
using SaveAPI;
using Planetside.Toolboxes;
using HarmonyLib;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Planetside
{
    public class BloodIdol  : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Blood Idol";
            //string resourceName = "Planetside/Resources/bloodidol.png";
            GameObject obj = new GameObject(itemName);
            BloodIdol activeitem = obj.AddComponent<BloodIdol>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("bloodidol"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "The Work Never Stops";
            string longDesc = "An 1dol dedicated to Kaliber, demanding the blood of slain Gundead.\n\nThough you may not complete her quest in this lifetime, she is faithful and will keep watch for many lifetimes.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.Damage, 0.95f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.MovementSpeed, 0.95f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(activeitem, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
            activeitem.gameObject.AddComponent<RustyItemPool>();
            BloodIdol.BloodIdolID = activeitem.PickupObjectId;
            SynergyAPI.SynergyBuilder.AddItemToSynergy(activeitem, CustomSynergyType.BLOOD_LOCKET);
            GameManager.Instance.RainbowRunForceExcludedIDs.Add(activeitem.PickupObjectId);
            activeitem.AddSynergy("Sacrifices Must Be Made", new List<PickupObject>()
            {
                Items.Bullet_Idol
            }, false);

            ItemIDs.AddToList(activeitem.PickupObjectId);
        }
        public static int BloodIdolID;


        [HarmonyPatch]
        private static class OnDamagedPassiveItemPlayerTookDamagePatch
        {
            [HarmonyPatch(typeof(OnDamagedPassiveItem), nameof(OnDamagedPassiveItem.PlayerTookDamage))]
            [HarmonyILManipulator]
            private static void GameUIAmmoControllerUpdateUIGunIL(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchCallvirt<Dungeonator.RoomHandler>("ApplyActionToNearbyEnemies")))
                    return;


                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Call, typeof(OnDamagedPassiveItemPlayerTookDamagePatch).GetMethod(nameof(ModifyNote), BindingFlags.Static | BindingFlags.NonPublic));
            }
            private static void ModifyNote(OnDamagedPassiveItem Passive)
            {
                Passive.Owner.CurrentRoom.ApplyActionToNearbyEnemies(Passive.Owner.CenterPosition, 100f, delegate (AIActor enemy, float dist)
                {
                    if (enemy && enemy.healthHaver)
                    {
                        if (Passive.PickupObjectId == Items.Bullet_Idol.PickupObjectId && Passive.Owner.PlayerHasActiveSynergy("Sacrifices Must Be Made"))
                        {
                            SaveAPIManager.RegisterStatChange(CustomTrackedStats.BLODD_IDOL_KILLS, 2);
                        }
                    }
                });
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
           
            return SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.BLODD_IDOL_KILLS) > 249;
        }
        public override void Pickup(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            base.Pickup(player);
        }
        public override void OnPreDrop(PlayerController user)
        {
            user.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(user.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
        }
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
           if (enemy != null && fatal == true)
           {
                SaveAPIManager.RegisterStatChange(CustomTrackedStats.BLODD_IDOL_KILLS, 1);
           }
        }

        private int kills;

        public override void Update()
        {
            


            if (this.m_pickedUp)
            {
                if (this.LastOwner == null)
                {
                    this.LastOwner = base.GetComponentInParent<PlayerController>();
                }

                this.remainingTimeCooldown = 1f -((float)SaveAPIManager.GetPlayerStatValue(CustomTrackedStats.BLODD_IDOL_KILLS) / 250f);
                if (this.IsCurrentlyActive)
                {
                    this.m_activeElapsed += BraveTime.DeltaTime * this.m_adjustedTimeScale;
                    if (!string.IsNullOrEmpty(this.OnActivatedSprite))
                    {
                        base.sprite.SetSprite(this.OnActivatedSprite);
                    }
                }
            }
            else
            {
                base.HandlePickupCurseParticles();
                if (!this.m_isBeingEyedByRat && Time.frameCount % 47 == 0 && base.ShouldBeTakenByRat(base.sprite.WorldCenter))
                {
                    GameManager.Instance.Dungeon.StartCoroutine(base.HandleRatTheft());
                }
            }
        }
        public override void OnDestroy()
        {
            if (base.LastOwner != null)
            {
                base.LastOwner.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Remove(base.LastOwner.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
            }
            base.OnDestroy();
        }
        public override void DoEffect(PlayerController user)
        {
            SaveAPIManager.RegisterStatChange(CustomTrackedStats.BLODD_IDOL_KILLS, -250);
            GameManager.Instance.StartCoroutine(DoWackyStuff(user));            
        }


        private IEnumerator DoWackyStuff(PlayerController user)
        {
            Vector2 pos = user.sprite.WorldCenter;
            StaticVFXStorage.HighPriestClapVFX.SpawnAtPosition(pos);
            yield return new WaitForSeconds(1f);
            PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(ReturnRandomQuality(), UnityEngine.Random.value > 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable, false);
            LootEngine.SpawnItem(pickupObject.gameObject, pos - new Vector2(0.5f, 0.5f), Vector2.up, 0f, true, false, false);
            yield break;
        }

        public ItemQuality ReturnRandomQuality()
        {
            int i = UnityEngine.Random.Range(1, 9);
            switch (i)
            {
                case 1:
                    return PickupObject.ItemQuality.D;
                case 2:
                    return PickupObject.ItemQuality.D;
                case 3:
                    return PickupObject.ItemQuality.D;
                case 4:
                    return PickupObject.ItemQuality.C;
                case 5:
                    return PickupObject.ItemQuality.C;
                case 6:
                    return PickupObject.ItemQuality.B;
                case 7:
                    return PickupObject.ItemQuality.A;
                case 8:
                    return PickupObject.ItemQuality.S;
                default:
                    return PickupObject.ItemQuality.D;
            }
        }

    }
}



