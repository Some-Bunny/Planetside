using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;

using Brave.BulletScript;
using GungeonAPI;

namespace Planetside
{
	public class CurseSpeedEffect : GameActorSpeedEffect
	{
		public override void ApplyTint(GameActor actor)
        {
			
		}
        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			PlayerController playerController = actor as PlayerController;
			if (playerController) 
			{ 
				playerController.CurrentCurseMeterValue += (0.8f * BraveTime.DeltaTime);
				playerController.CurseIsDecaying = false;
				if (playerController.CurrentCurseMeterValue > 1f)
				{
					playerController.CurrentCurseMeterValue = 0f;
					StatModifier statModifier = new StatModifier();
					statModifier.amount = 1f;
					statModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
					statModifier.statToBoost = PlayerStats.StatType.Curse;
					playerController.ownerlessStatModifiers.Add(statModifier);
					playerController.stats.RecalculateStats(playerController, false, false);
				}
			}
		}
		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			PlayerController playerController = actor as PlayerController;
			if (playerController) { playerController.CurseIsDecaying = true; }
		}
	}
}


