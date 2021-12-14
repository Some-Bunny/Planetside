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
using Pathfinding;


namespace Planetside
{
	public class Enrage : BraveBehaviour
	{
		public void Start()
		{
			RagePassiveItem rageitem = PickupObjectDatabase.GetById(353).GetComponent<RagePassiveItem>();
			this.RageOverheadVFX = rageitem.OverheadVFX.gameObject;
			this.instanceVFX = base.aiActor.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0f, 1.375f, 0f), true, true, false);
			
			base.aiActor.behaviorSpeculator.AttackCooldown *= 0.70f;
            base.aiActor.RegisterOverrideColor(Rage, "rage");
			base.aiActor.healthHaver.AllDamageMultiplier *= 1.4f;
		}
		public void Update()
		{

		}

        public Color Rage = new Color(7f, 0f, 0f, 0.33f);
		private GameObject instanceVFX;
		public GameObject RageOverheadVFX;
	}
}