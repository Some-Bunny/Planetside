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
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using Brave.BulletScript;
using GungeonAPI;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;

namespace Planetside
{



	public class PickupGuonComponent : BraveBehaviour
	{
		public void Start()
		{
			var str = SynergyToCheck();
            bool HasSynergy = PlayerOwner != null && str != null && PlayerOwner.PlayerHasActiveSynergy(str);
            switch (pickupType)
			{
				case PickupType.HALF_HEART:
					HitsBeforeDeath = HasSynergy ? 60 : 45;
					this.spriteAnimator.Play("halfheartguon_idle");
                    Orbital.orbitRadius = 2;
                    Orbital.orbitDegreesPerSecond = 90;
                    break;
                case PickupType.HEART:
                    HitsBeforeDeath = HasSynergy ? 120 : 90;
					this.spriteAnimator.Play("heartguon_idle");
                    Orbital.orbitRadius = 3;
                    Orbital.orbitDegreesPerSecond = 72;
                    break;

                case PickupType.ARMOR:
                    HitsBeforeDeath = HasSynergy ? 160 : 120;
                    this.spriteAnimator.Play("armorguon_idle");
                    Orbital.orbitRadius = 1.8f;
                    Orbital.orbitDegreesPerSecond = 90;
                    break;

                case PickupType.AMMO:
                    HitsBeforeDeath = HasSynergy ? 120 : 90;
                    this.spriteAnimator.Play("ammoguon_idle");
                    Orbital.orbitRadius = 3;
                    Orbital.orbitDegreesPerSecond = 90;
                    break;
                case PickupType.HALF_AMMO:
                    HitsBeforeDeath = HasSynergy ? 80 : 60;
                    this.spriteAnimator.Play("halfammoguon_idle");
                    Orbital.orbitRadius = 4.5f;
                    Orbital.orbitDegreesPerSecond = 75;
                    break;

                case PickupType.KEY:
                    HitsBeforeDeath = HasSynergy ? 160 : 120;
                    this.spriteAnimator.Play("keyguon_idle");
                    Orbital.orbitRadius = 1.25f;
                    Orbital.orbitDegreesPerSecond = 60;
                    break;
                case PickupType.BLANK:
                    HitsBeforeDeath = HasSynergy ? 100 : 75;
                    this.spriteAnimator.Play("blankguon_idle");
                    Orbital.orbitRadius = 5f;
                    Orbital.orbitDegreesPerSecond = 90;
                    break;
                //creditguon_idle
                case PickupType.CREDIT:
                    HitsBeforeDeath = HasSynergy ? 40 : 30;
                    this.spriteAnimator.Play("creditguon_idle");
                    Orbital.orbitRadius = 8.5f;
                    Orbital.orbitDegreesPerSecond = 20;
                    Orbital.sprite.usesOverrideMaterial = true;
                    Material material = new Material(ShaderCache.Acquire("Brave/Internal/HologramShader"));
                    material.SetFloat("_IsGreen", 1);
                    Orbital.sprite.renderer.material = material;
                    break;
            }
            Orbital.Initialize(PlayerOwner);
            if (DoPoof == true)
            {
				LootEngine.DoDefaultItemPoof(this.transform.position, false, false);
			}
            Orbital.specRigidbody.OnPreRigidbodyCollision += OnPreCollision;
        }

		public string SynergyToCheck()
		{
            switch (pickupType)
            {
                case PickupType.HALF_HEART:
					return "More To Hearts";
                case PickupType.HEART:
					return "More To Hearts";

                case PickupType.AMMO:
                    return "More To Ammo";
                case PickupType.HALF_AMMO:
                    return "More To Ammo";
                case PickupType.KEY:
                    return "More To Keys";

                case PickupType.BLANK:
                    return "More To Blanks";

                case PickupType.ARMOR:
                    return "More To Armor";
                case PickupType.CREDIT:
                    return "More To Greed";

                default:
					return null;
            }
        }


        private bool Cooldown = false;
        public void C()
        {
            Cooldown = false;
        }
        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
		{
			GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");

            if (Cooldown == false)
            {
                Hits++;
                Cooldown = true;
                this.Invoke("C", 0.15f);
            }

			switch (pickupType)
			{
				case PickupType.BLANK:
                    if (Hits == HitsBeforeDeath | UnityEngine.Random.value <= 0.025f)
                    {

                        LootEngine.DoDefaultItemPoof(PlayerOwner.sprite.WorldCenter, false, true);
                        UnityEngine.Object.Destroy(base.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
                        GameObject gameObject = new GameObject("silencer");
                        SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                        float additionalTimeAtMaxRadius = 0.25f;
                        silencerInstance.TriggerSilencer(myRigidbody.sprite.WorldCenter, 25f, 3f, silencerVFX, 0f, 3f, 3f, 3f, 250f, 5f, additionalTimeAtMaxRadius, PlayerOwner, false, false);
                    }

                    break;
                case PickupType.AMMO:
                    if (other.projectile != null)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null)
                        {
                            if (!actor.healthHaver.IsBoss)
                            {
                                actor.behaviorSpeculator.CooldownScale += 0.02f;
                            }
                        }
                    }
                    break;
                case PickupType.HALF_AMMO:
                    if (other.projectile != null)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null)
                        {
                            if (!actor.healthHaver.IsBoss)
                            {
                                actor.behaviorSpeculator.CooldownScale += 0.01f;
                            }
                        }
                    }
                    break;
				case PickupType.HEART:
                    if (other.projectile)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null && !actor.healthHaver.IsBoss)
                        {
                            if (UnityEngine.Random.value <= 0.05f)
                            {
                                actor.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                                actor.gameObject.AddComponent<KillOnRoomClear>();
                                actor.IsHarmlessEnemy = true;
                                actor.IgnoreForRoomClear = true;
                                var _ = actor.gameObject.GetComponent<SpawnEnemyOnDeath>();
                                if (_)
                                {
                                    Destroy(_);
                                }
                            }
                        }
                    }
                    break;
                case PickupType.HALF_HEART:
                    if (other.projectile)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null && !actor.healthHaver.IsBoss)
                        {
                            if (UnityEngine.Random.value <= 0.025f)
                            {
                                actor.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                                actor.gameObject.AddComponent<KillOnRoomClear>();
                                actor.IsHarmlessEnemy = true;
                                actor.IgnoreForRoomClear = true;
                                var _ = actor.gameObject.GetComponent<SpawnEnemyOnDeath>();
                                if (_)
                                {
                                    Destroy(_);
                                }
                            }
                        }
                    }
                    break;
                case PickupType.ARMOR:
                    if (other.projectile)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null)
                        {
                            if (UnityEngine.Random.value <= 0.1f)
                            {
                                if (!actor.healthHaver.IsBoss)
                                {
                                    actor.behaviorSpeculator.Stun(4, true);
                                }
                            }
                        }
                    }
                    break;
                case PickupType.CREDIT:
                    if (other.projectile)
                    {
                        AIActor actor = other.projectile.Owner as AIActor;
                        if (actor != null)
                        {
                            if (UnityEngine.Random.value <= 0.025f)
                            {
                                LootEngine.SpawnCurrency(this.sprite.WorldCenter, 1);
                            }
                        }
                    }
                    break;

            }			
		}



		public override void OnDestroy()
        {
			base.OnDestroy();
			if (Hits != HitsBeforeDeath)
            {
				GuonStoneRespawner pick = PlayerOwner.gameObject.AddComponent<GuonStoneRespawner>();
                pick.pickupType = pickupType;
				pick.Hits = this.Hits;
				pick.HitsBeforeDeath = this.HitsBeforeDeath;
				pick.player = this.PlayerOwner;
			}
            PlayerOwner.orbitals.Remove(Orbital);
        }

		public PickupType pickupType;


		public bool DoPoof;
		public int HitsBeforeDeath = 15;

        public PlayerOrbital Orbital;
		public PlayerController PlayerOwner;
		public int Hits;

		public enum PickupType
		{
			HEART,
			HALF_HEART,
			AMMO,
			HALF_AMMO,
			KEY,
			ARMOR,
			BLANK,
			CREDIT
		}

	}
	public class GuonStoneRespawner : MonoBehaviour
    {
		public GuonStoneRespawner()
		{
			this.HitsBeforeDeath = 10;
			this.Hits = 0;
		}
		public void Start()
        {
			GameManager.Instance.OnNewLevelFullyLoaded += this.RespawnStones;
		}
		private void RespawnStones()
		{
		
			GameObject orb = PlayerOrbitalItem.CreateOrbital(player, ResourceGuonMaker.GuonDummy, false);
			PickupGuonComponent pick = orb.GetOrAddComponent<PickupGuonComponent>();
            pick.pickupType = pickupType;
            pick.DoPoof = false;
            
            pick.PlayerOwner = player;
            pick.Hits = this.Hits;
            pick.HitsBeforeDeath = this.HitsBeforeDeath;

            GameManager.Instance.OnNewLevelFullyLoaded -= this.RespawnStones;
			Destroy(this);
		}

		public int Hits;
		public int HitsBeforeDeath = 10;
        public PickupGuonComponent.PickupType pickupType;
		public PlayerController player;

	}
}
