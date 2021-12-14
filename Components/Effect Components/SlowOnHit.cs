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



namespace Planetside
{
	// Token: 0x0200009C RID: 156
	public class EasyApplyDirectSlow
	{
		// Token: 0x06000353 RID: 851 RVA: 0x0002058C File Offset: 0x0001E78C
		public static void ApplyDirectSlow(GameActor target, float duration, float speedMultiplier, Color tintColour, Color deathTintColour, EffectResistanceType resistanceType, string identifier, bool tintsEnemy, bool tintsCorpse)
		{
			Gun gun = ETGMod.Databases.Items["triple_crossbow"] as Gun;
			GameActorSpeedEffect speedEffect = gun.DefaultModule.projectiles[0].speedEffect;
			GameActorSpeedEffect effect = new GameActorSpeedEffect
			{
				duration = duration,
				TintColor = tintColour,
				DeathTintColor = deathTintColour,
				effectIdentifier = identifier,
				AppliesTint = tintsEnemy,
				AppliesDeathTint = tintsCorpse,
				resistanceType = resistanceType,
				SpeedMultiplier = speedMultiplier,
				OverheadVFX = speedEffect.OverheadVFX,
				AffectsEnemies = true,
				AffectsPlayers = false,
				AppliesOutlineTint = false,
				OutlineTintColor = tintColour,
				PlaysVFXOnActor = false
			};
			bool flag = target && target.aiActor && target.healthHaver && target.healthHaver.IsAlive;
			if (flag)
			{
				target.ApplyEffect(effect, 1f, null);
			}
		}
	}
}