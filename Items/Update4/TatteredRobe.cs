using System;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Collections;

namespace Planetside
{
	public class TatteredRobe : PassiveItem
	{
		public static void Init()
		{
			string name = "Torn Cloth";
			string resourcePath = "Planetside/Resources/tatteredcloth.png";
			GameObject gameObject = new GameObject(name);
			TatteredRobe warVase = gameObject.AddComponent<TatteredRobe>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Part Of His Power";
			string longDesc = "A piece of the robe the Prisoner wore before escaping deeper.\n\nStrangely empowering...";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.SPECIAL;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
			TatteredRobe.PrisonItemID = warVase.PickupObjectId;
			EncounterDatabase.GetEntry(warVase.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
			ItemBuilder.AddPassiveStatModifier(warVase, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);
			warVase.RolledCount = 0;
		}
		public static int PrisonItemID;

		public override void Pickup(PlayerController player)
		{
			player.OnDodgedProjectile += DodgedBullet;
			if (base.m_pickedUpThisRun == false)
            {
				player.healthHaver.FullHeal();
				player.PlayEffectOnActor(StaticVFXStorage.HealingSparklesVFX, new Vector3(0,0));
				if (player.ForceZeroHealthState)
                {player.healthHaver.Armor += 6;}
            }
			base.Pickup(player);
		}

		protected override void Update()
        {
            base.Update();
			if (base.Owner)
            {
				if (RolledCount > 20 && IsDoingTheWacky == false)
				{
					StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(base.Owner.sprite.WorldCenter, 0);
					IsDoingTheWacky = true;
					RolledCount = 0;
					GameManager.Instance.StartCoroutine(StartWacky(base.Owner.sprite.WorldCenter));
				}
			}
        }

		private IEnumerator StartWacky(Vector2 spawnPos)
        {
			float elaWait = 0f;
			while (elaWait < 1f){elaWait += BraveTime.DeltaTime;yield return null;}

			GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Amogus"));
			MeshRenderer rend = partObj.GetComponentInChildren<MeshRenderer>();
			rend.allowOcclusionWhenDynamic = true;
			partObj.transform.position = spawnPos;
			partObj.name = "VoidHole";
			partObj.transform.localRotation = Quaternion.Euler(0, 90, 90f);
			partObj.transform.localScale = Vector3.zero;
			List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

			if (activeEnemies != null || activeEnemies.Count > 0)
			{
				for (int i = 0; i < activeEnemies.Count; i++)
				{
					AIActor target = activeEnemies[i];
					if (target.IsNormalEnemy || (target.healthHaver.IsBoss && !target.IsHarmlessEnemy))
					{
						if (target != null)
						{
							target.healthHaver.ApplyDamage(60, Vector2.zero, "Take a bath, nerd.", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
						}
					}
				}
			}
			AkSoundEngine.PostEvent("Play_PortalOpen", base.Owner.gameObject);
			AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", base.Owner.gameObject);
			elaWait = 0f;
			while (elaWait < 2f)
			{
				float t = elaWait / 2;
				float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
				partObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 20f, throne1);
				elaWait += BraveTime.DeltaTime;
				yield return null;
			}
			Destroy(partObj);
			IsDoingTheWacky = false;
			yield break;
        }

        private void DodgedBullet(Projectile obj)
        {
            if (IsDoingTheWacky == false && !projectiles.Contains(obj)) { projectiles.Add(obj); RolledCount++; }
        }

		public List<Projectile> projectiles = new List<Projectile>();

		public int RolledCount;
		public bool IsDoingTheWacky;

        public override DebrisObject Drop(PlayerController player)
		{
			DebrisObject result = base.Drop(player);
			return result;
		}		
	}
}
