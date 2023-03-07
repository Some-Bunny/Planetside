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
    public class HitBoxShower : PassiveItem
    {
        public static void Init()
        {
            string itemName = "HitBoxShower";
            string resourceName = "Planetside/Resources/aurabullets.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HitBoxShower>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Shows Hitboxes";
			string longDesc = "reveals hit boxes of hit targets.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
			item.quality = PickupObject.ItemQuality.EXCLUDED;
			
		}


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            ShootBehavior.DrawDebugFiringArea = true;
            player.OnEnteredCombat += OnEnterCombat;
            player.PostProcessProjectile += this.PostProcessProjectile;
            player.PostProcessBeamTick += this.PostProcessBeamTick;
            player.OnRoomClearEvent += this.OnLeaveCombat;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            ShootBehavior.DrawDebugFiringArea = false;
            player.OnRoomClearEvent -= this.OnLeaveCombat;
            player.PostProcessProjectile -= this.PostProcessProjectile;
            player.PostProcessBeamTick -= this.PostProcessBeamTick;
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            base.Owner.OnRoomClearEvent -= this.OnLeaveCombat;
            base.Owner.PostProcessBeamTick -= this.PostProcessBeamTick;
            base.Owner.PostProcessProjectile -= this.PostProcessProjectile;
            base.OnDestroy();
        }

        private void OnEnterCombat()
        {
            //Tools.Print("OnEnteredCombat", "FFFFFF", true);
            //RoomHandler.DrawRandomCellLines = true;
            BraveUtility.DrawDebugSquare(base.Owner.CurrentRoom.area.basePosition.ToVector2(), base.Owner.CurrentRoom.area.basePosition.ToVector2() + base.Owner.CurrentRoom.area.dimensions.ToVector2(), Color.cyan, 1000f);
        }

        private void OnLeaveCombat(PlayerController user)
        {
            targetsHitList = new List<AIActor>();
        }

        private void PostProcessProjectile(Projectile projectile, float Chance)
        {
            PlayerController owner = base.Owner;
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.OnProjectileHitEnemy));

        }

        private void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if (enemy != null)
            {
                AIActor aiActor = enemy.aiActor;
                PlayerController owner = this.Owner;
                if (!targetsHitList.Contains(aiActor))
                {
                    targetsHitList.Add(aiActor);
                    enemy.ShowHitBox();//DrawDebugFiringArea
                }
            }
        }

        private void PostProcessBeamTick(BeamController beam, SpeculativeRigidbody hitRigidBody, float tickrate)
        {
            AIActor aiactor = hitRigidBody.aiActor;
            if (!aiactor)
            {
                return;
            }
            bool fatal = aiactor.healthHaver && !aiactor.healthHaver.IsDead && !targetsHitList.Contains(aiactor);
            if (fatal)
            {
                targetsHitList.Add(aiactor);
                hitRigidBody.ShowHitBox();//DrawDebugFiringArea
            }
        }

        private List<AIActor> targetsHitList = new List<AIActor>();
    }
}


