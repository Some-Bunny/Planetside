using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.PrefabAPI;
using ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace Planetside
{
	internal class DamnedGuonStone : IounStoneOrbitalItem
	{
		public static void Init()
		{
			string name = "Damned Guon Stone";
			GameObject gameObject = new GameObject();
			DamnedGuonStone woodGuonStone = gameObject.AddComponent<DamnedGuonStone>();
            var data = StaticSpriteDefinitions.Passive_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(name, data.GetSpriteIdByName("damnedstone"), data, gameObject);

            //ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Laced With Curse";
			string longDesc = "A guon stone, bathed in cursed pots scattered around the Gungeon.\n\nThey reek of curse.";
			woodGuonStone.SetupItem(shortDesc, longDesc, "psog");
			woodGuonStone.quality = PickupObject.ItemQuality.EXCLUDED;
			DamnedGuonStone.BuildPrefab();
			woodGuonStone.OrbitalPrefab = DamnedGuonStone.orbitalPrefab;
			woodGuonStone.Identifier = IounStoneOrbitalItem.IounStoneIdentifier.GENERIC;
			woodGuonStone.CanBeDropped = false;
			woodGuonStone.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1.5f, StatModifier.ModifyMethod.ADDITIVE);
		}

		public static void BuildPrefab()
		{
			bool flag = DamnedGuonStone.orbitalPrefab != null;
			if (!flag)
			{
				GameObject gameObject = PrefabBuilder.BuildObject("CursedGuonStone");

                tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
				sprite.collection = StaticSpriteDefinitions.Guon_Sheet_Data;
				sprite.SetSprite(StaticSpriteDefinitions.Guon_Sheet_Data.GetSpriteIdByName("curseguonfloaty_001"));

                tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
                animator.library = StaticSpriteDefinitions.Guon_Animation_Data;
				animator.defaultClipId = StaticSpriteDefinitions.Guon_Animation_Data.GetClipIdByName("cursedFloat_idle");
				animator.playAutomatically = true;

				sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;

                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 13));
				speculativeRigidbody.CollideWithTileMap = false;
				speculativeRigidbody.CollideWithOthers = true;
				speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
				DamnedGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
				DamnedGuonStone.orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
				DamnedGuonStone.orbitalPrefab.shouldRotate = false;
				DamnedGuonStone.orbitalPrefab.orbitRadius = 3f;
				DamnedGuonStone.orbitalPrefab.orbitDegreesPerSecond = 105f;
				DamnedGuonStone.orbitalPrefab.SetOrbitalTier(0);
				ImprovedAfterImage yeah = gameObject.AddComponent<ImprovedAfterImage>();
				yeah.dashColor = Color.black;
				yeah.spawnShadows = true;
				yeah.shadowTimeDelay = 0.1f;
				yeah.shadowLifetime = 0.5f;
				//UnityEngine.Object.DontDestroyOnLoad(gameObject);
				//FakePrefab.MarkAsFakePrefab(gameObject);
				//gameObject.SetActive(false);
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
		}



		public override DebrisObject Drop(PlayerController player)
		{
			return base.Drop(player);
		}

		public override void OnDestroy()
		{
			if (base.Owner != null)
            {
				base.OnDestroy();
			}
		}	
		public static PlayerOrbital orbitalPrefab;
	}
}
