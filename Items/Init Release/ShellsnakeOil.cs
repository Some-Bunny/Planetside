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
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

//Garbage Code Incoming
namespace Planetside
{
    public class ShellsnakeOil : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shell-snake Oil";
            //string resourceName = "Planetside/Resources/gunsnakeoil.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShellsnakeOil>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("gunsnakeoil"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Works 103%!";
            string longDesc = "Harvested from elusive shell-snakes, This wonderful elixir is SURE to boost your firepower and strength in combat." +
                "\n\nSource: Trust us! We're completely honest!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.KnockbackMultiplier, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.PlayerBulletScale, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
			item.quality = PickupObject.ItemQuality.C;
            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:shell-snake_oil",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "rattler",
                "snakemaker",
                "box",
                "weird_egg"
            };
            CustomSynergies.Add("Gun Snek Good Maybe?", mandatoryConsoleIDs, optionalConsoleIDs, true);
            item.AddToSubShop(ItemBuilder.ShopType.Goopton, 1f);
            item.gameObject.AddComponent<RustyItemPool>();

            ShellsnakeOil.ShellSnakeOilID = item.PickupObjectId;
            ItemIDs.AddToList(item.PickupObjectId);

        }
        public static int ShellSnakeOilID;
        private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
        {
            try
            {
                SpeculativeRigidbody specRigidbody = sourceProjectile.projectile.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));

            }
            catch (Exception ex)
            {
                ETGModConsole.Log(ex.Message, false);
            }
        }
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            string text;
            if (otherRigidbody == null)
            {
                text = null;
            }
            else
            {
                AIActor aiActor = otherRigidbody.aiActor;
                text = ((aiActor != null) ? aiActor.EnemyGuid : null);
            }
            string value = text;
            if (!string.IsNullOrEmpty(value))
            {
                if (otherRigidbody && otherRigidbody.healthHaver)
                {
                    foreach (string text2 in ShellsnakeOil.sneks)
                    {
                        if (text2.Equals(value))
                        {
                            float damage = myRigidbody.projectile.baseData.damage;
                            myRigidbody.projectile.baseData.damage *= 1.5f;
                            GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(myRigidbody.projectile, damage));
                        }
                    }
                }
            }
        }
        private void SpawnBall()
        {
            if (base.Owner.PlayerHasActiveSynergy("Gun Snek Good Maybe?"))
            {
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("f38686671d524feda75261e469f30e0b");
                IntVector2? intVector = new IntVector2?(base.Owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Spawn, true);
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
                aiactor.gameObject.AddComponent<KillOnRoomClear>();
                aiactor.IgnoreForRoomClear = true;
                aiactor.IsHarmlessEnemy = true;
                aiactor.HandleReinforcementFallIntoRoom(0f);
            }
        }
        public static List<string> sneks = new List<string>
        {
            EnemyGuidDatabase.Entries["ammoconda"],
            EnemyGuidDatabase.Entries["ammoconda_ball"],
        };
        private IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            bool flag = bullet != null;
            if (flag)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }
        public override DebrisObject Drop(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.SpawnBall));
            player.PostProcessProjectile -= this.PostProcessProjectile;
            DebrisObject result = base.Drop(player);	
			return result;
		}

		public override void Pickup(PlayerController player)
		{
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.SpawnBall));
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
		}
	}
}


