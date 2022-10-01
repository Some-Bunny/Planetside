using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using UnityEngine;
using Gungeon;

namespace Planetside
{
	internal class StaticExplosionDatas
	{
		public static ExplosionData explosiveRoundsExplosion = Game.Items["explosive_rounds"].GetComponent<ComplexProjectileModifier>().ExplosionData;

		public static ExplosionData genericSmallExplosion = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;

		public static ExplosionData genericLargeExplosion = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;

		public static ExplosionData customDynamiteExplosion = new ExplosionData
		{
			effect = StaticExplosionDatas.genericLargeExplosion.effect,
			ignoreList = StaticExplosionDatas.genericLargeExplosion.ignoreList,
			ss = StaticExplosionDatas.genericLargeExplosion.ss,
			damageRadius = 5f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 45f,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 30f,
			preventPlayerForce = true,
			explosionDelay = 0.1f,
			usesComprehensiveDelay = false,
			doScreenShake = true,
			playDefaultSFX = true
		};

		public static ExplosionData CopyFields(ExplosionData explosionToCopy)
		{
			ExplosionData explosionData = new ExplosionData();
			explosionData.CopyFrom(explosionToCopy);
            return explosionData;
        }
    }
}