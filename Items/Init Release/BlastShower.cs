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
            string longDesc = "Damages all enemies in a room, and applies the players effects onto enemies. A quick and easy portable shower, intended for Gungeoneers to stay somewhat clean for their eventual demise.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 300f);
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
            for (int i = 0; i < 2; i++)
            {
                SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, user.sprite.WorldBottomCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(user.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
            }
            AkSoundEngine.PostEvent("Play_ENV_water_splash_01", base.gameObject);
            this.EnemyListing(user);
        }
        private void EnemyListing(PlayerController user)
        {
            RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            bool flag = activeEnemies != null;
            bool flag2 = flag;
            if (flag2)
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    this.AffectEnemy(activeEnemies[i], user);
                }
            }
        }

        protected void AffectEnemy(AIActor target, PlayerController user)
        {
            bool flag = target.IsNormalEnemy || (target.healthHaver.IsBoss && !target.IsHarmlessEnemy);
            bool flag2 = flag;
            if (flag2)
            {
                if (target != null)
                {
                    float MaxHP;
                    if (user.PlayerHasActiveSynergy("Watered Down"))
                    {
                        MaxHP = target.healthHaver.GetMaxHealth() / 5;
                    }
                    else
                    {
                        MaxHP = 0;
                    }
                    if (target.healthHaver.IsBoss)
                    {
                        target.healthHaver.ApplyDamage(30f, Vector2.zero, "Take a bath, nerd.", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                    }
                    else
                    {
                        target.healthHaver.ApplyDamage(10f + MaxHP, Vector2.zero, "Take a bath, nerd.", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                    }
                    if (user.IsOnFire)
                    {
                        this.CurrentFireMeterValue = 0f;
                        BulletStatusEffectItem Firecomponent = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>();
                        GameActorFireEffect gameActorFire = Firecomponent.FireModifierEffect;
                        target.ApplyEffect(gameActorFire, 5f, null);

                    }
                    if (user.CurrentPoisonMeterValue > 0)
                    {
                        this.CurrentPoisonMeterValue = 0f;
                        BulletStatusEffectItem PoisonComponent = PickupObjectDatabase.GetById(204).GetComponent<BulletStatusEffectItem>();
                        GameActorHealthEffect gameActorPOSON = PoisonComponent.HealthModifierEffect;
                        target.ApplyEffect(gameActorPOSON, 5f, null);
                    }
                    if (user.CurrentStoneGunTimer > 0)
                    {
                        user.CurrentStoneGunTimer = 0f;
                    }
                }
            }
        }
        public float CurrentFireMeterValue;
        public float CurrentPoisonMeterValue;
    }
}
