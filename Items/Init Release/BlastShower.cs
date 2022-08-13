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
    public class BlastShower  : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Blast Shower";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            BlastShower activeitem = obj.AddComponent<BlastShower>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Comes With Auto-Hairdryer.";
            string longDesc = "Grants temporary immunity to debuffs, and applies any currently applied debuffs onto enemies. A quick and easy portable shower, intended for Gungeoneers to stay somewhat clean for their eventual demise.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 250f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.C;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            activeitem.gameObject.AddComponent<RustyItemPool>();

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:blast_shower",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "siren",
                "starpew",
                "teapot",
                "glacier",
                "ice_cube"
            };
            CustomSynergies.Add("Watered Down", mandatoryConsoleIDs, optionalConsoleIDs, true);
            BlastShower.BlastShowerID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int BlastShowerID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {
            SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, user.sprite.WorldBottomCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(user.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
            AkSoundEngine.PostEvent("Play_ENV_water_splash_01", base.gameObject);
            this.EnemyListing(user);
        }
        private void EnemyListing(PlayerController user)
        {
            RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            
            if (activeEnemies != null)
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    this.AffectEnemy(activeEnemies[i], user, user.IsOnFire, user.CurrentPoisonMeterValue > 0 ? true : false);
                }
            }
            user.StartCoroutine(TempImmunity(user));
            user.CurrentStoneGunTimer = 0f;
        }

        private IEnumerator TempImmunity(PlayerController player)
        {
            DamageTypeModifier fire = GenSpecImmunity(CoreDamageTypes.Fire);
            DamageTypeModifier poison = GenSpecImmunity(CoreDamageTypes.Poison);
            player.healthHaver.damageTypeModifiers.AddRange(new List<DamageTypeModifier>() { fire, poison});
            yield return new WaitForSeconds(5f);
            player.healthHaver.damageTypeModifiers.Remove(fire);
            player.healthHaver.damageTypeModifiers.Remove(poison);
            yield break;
        }

        public DamageTypeModifier GenSpecImmunity(CoreDamageTypes damageType)
        {
            DamageTypeModifier immunity = new DamageTypeModifier();
            immunity.damageMultiplier = 0f;
            immunity.damageType = damageType;
            return immunity;
        }

        protected void AffectEnemy(AIActor target, PlayerController user, bool willBurn, bool willPoison)
        {
            bool flag = target.IsNormalEnemy || (target.healthHaver.IsBoss && !target.IsHarmlessEnemy);
            bool flag2 = flag;
            if (flag2)
            {
                if (target != null)
                {
                    float MaxHP = user.PlayerHasActiveSynergy("Watered Down") == true ? target.healthHaver.GetMaxHealth() / 5:0;
                    target.healthHaver.ApplyDamage(30f + MaxHP, Vector2.zero, "Take a bath, nerd.", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

                    if (willBurn)
                    {
                        BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
                        GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
                        target.ApplyEffect(gameActorFire, 5f, null);
                    }
                    if (willPoison)
                    {
                        BulletStatusEffectItem PoisonComponent = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>();
                        GameActorHealthEffect gameActorPOSON = PoisonComponent.HealthModifierEffect;
                        target.ApplyEffect(gameActorPOSON, 5f, null);
                    }
                }
            }
        }
    }
}
