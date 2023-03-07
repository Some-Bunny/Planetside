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
using GungeonAPI;
using Pathfinding;
using static tk2dSpriteCollectionDefinition;
using SynergyAPI;

namespace Planetside
{
    public class ImmolationPowder : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Immolation Powder";
            //string resourceName = "Planetside/Resources/immolationpowder2.png";
            GameObject obj = new GameObject(itemName);
            ImmolationPowder activeitem = obj.AddComponent<ImmolationPowder>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("immolationpowder2"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Cannot Live With Me";
            string longDesc = "Passively reduces the rate at which you burn, and grants great power when burning. Immolates the player on use. \n\nA bag of chilli powder made from the hottest Gungeon Peppers. Occasionally used in pagan Gundead rituals where the gunpowder would be replaced with this and set alight.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 350f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);
            ImmolationPowder.ImmolationPowderID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            new Hook(typeof(PlayerController).GetMethod("IncreaseFire", BindingFlags.Instance | BindingFlags.Public), typeof(ImmolationPowder).GetMethod("IncreaseFireHook"));

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:immolation_powder",
                "psog:revenant"
            };   
            CustomSynergies.Add("Ashes To Ashes, To Ashes", mandatoryConsoleIDs, null, false);

            activeitem.AddItemToSynergy(CustomSynergyType.PITCHPERFECT);

        }

        public static void IncreaseFireHook(Action<PlayerController, float> orig, PlayerController self, float amount)
        {
            if (self.HasPickupID(ImmolationPowderID) == true) 
            {
                amount = Mathf.Min(amount, BraveTime.DeltaTime * 0.2f);
            }
            orig(self, amount);
        }

        public static int ImmolationPowderID;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += PostProcessProjectile; ;
            player.PostProcessBeam += PostProcessBeam;

            base.Pickup(player);   
        }

        private void PostProcessBeam(BeamController obj)
        {
            if (PlayerIsOnFire(base.LastOwner) == true)
            {
                obj.projectile.projectile.baseData.damage *= 1.15f;
            }
        }

        private void PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (PlayerIsOnFire(base.LastOwner) == true) 
            {
                arg1.projectile.baseData.damage *= 1.075f;
                arg1.projectile.baseData.damage += 1;
            }
        }

        public override void OnPreDrop(PlayerController user)
        {
            user.PostProcessProjectile -= this.PostProcessProjectile;
            user.PostProcessBeam -= this.PostProcessBeam;
            base.OnPreDrop(user);
        }

        public override void OnDestroy()
        {
            if (base.LastOwner != null) 
            {
                base.LastOwner.PostProcessProjectile -= this.PostProcessProjectile;
                base.LastOwner.PostProcessBeam -= this.PostProcessBeam;
            }
            base.OnDestroy();
        }


        public override void Update()
        {
            base.Update();
            if (base.LastOwner)
            {
                var player = base.LastOwner;
                this.RemoveStat(PlayerStats.StatType.RateOfFire);
                this.RemoveStat(PlayerStats.StatType.Accuracy);
                if (PlayerIsOnFire(player) == true)
                {
                    C++;
                    this.AddStat(PlayerStats.StatType.RateOfFire, .2f, StatModifier.ModifyMethod.ADDITIVE);
                    this.AddStat(PlayerStats.StatType.Accuracy, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    if(C == 12)
                    {
                        C = 0;
                        GlobalSparksDoer.DoRadialParticleBurst(1, player.sprite.WorldBottomLeft, player.sprite.WorldTopRight, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    }
                }
                player.stats.RecalculateStats(player, true, false);
            }
        }

        public int C = 0;

        public bool PlayerIsOnFire(PlayerController p)
        {
            if (p == null) { return false; }
            if (p.IsOnFire == true) { return true; }
            return false;
        }


        public override void DoEffect(PlayerController user)
        {
            var currentRoom = user.CurrentRoom;
            if (currentRoom != null)
            {
                currentRoom.ApplyActionToNearbyEnemies(user.sprite.WorldCenter, 3.5f, new Action<AIActor, float>(this.ProcessEnemy));
            }
            AkSoundEngine.PostEvent("Play_Immolate", user.gameObject);
            GlobalSparksDoer.DoRadialParticleBurst(30, user.sprite.WorldBottomLeft, user.sprite.WorldTopRight, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            var vfx = user.PlayEffectOnActor((PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, new Vector3(1 ,-1.375f));
            vfx.transform.localRotation = Quaternion.Euler(0, 0,Vector2.up.ToAngle());
            vfx.transform.localScale = Vector3.one * 0.7f;
            Destroy(vfx, 1);
            user.IsOnFire = true;
            user.IncreaseFire(1);
        }

        private void ProcessEnemy(AIActor target, float distance)
        {
            target.ApplyEffect(DebuffStatics.hotLeadEffect);
            var vfx = target.PlayEffectOnActor((PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, new Vector3(1, -1.375f));
            vfx.transform.localRotation = Quaternion.Euler(0, 0, Vector2.up.ToAngle());
            vfx.transform.localScale = Vector3.one * 0.7f;
            Destroy(vfx, 1);
        }


        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            bool flag = this.passiveStatModifiers == null;
            if (flag)
            {
                this.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
            }
            else
            {
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
                {
                    statModifier
                }).ToArray<StatModifier>();
            }
        }
        private void RemoveStat(PlayerStats.StatType statType)
        {
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i].statToBoost != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }

    }
}



