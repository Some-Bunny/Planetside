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
	public class MarkWithColorComponent : BraveBehaviour
	{
		public void Start()
		{
			//base.healthHaver.minimumHealth = this.Mark;
		}
		public void Update()
		{
			if (ai != null)
            {
				Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(ai.sprite);
				if (!ai.healthHaver.IsDead && outlineMaterial1 != null)
                {
					float Scale = Mark;
					if (playa != null)
					{
						Scale = Mark * playa.stats.GetStatValue(PlayerStats.StatType.Damage);
					}
					if (playa == null)
					{
						Scale = Mark;
					}

					if (ai.healthHaver != null && ai.aiActor != null)
					{
						if (ai.healthHaver.GetCurrentHealth() <= Scale)
						{
							outlineMaterial1.SetColor("_OverrideColor", new Color(10f, 10f, 42f));
						}
					}
				}
				else
				{
				}
			}
			else
            {
			}

		}
		protected override void OnDestroy()
		{
			Material outlineMaterial1 = SpriteOutlineManager.GetOutlineMaterial(ai.sprite);
			if (ai.aiActor != null && !ai.healthHaver.IsDead && outlineMaterial1 != null)
            {
				outlineMaterial1.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
				//base.OnDestroy();
			}
			base.OnDestroy();
		}
		public float minimumHealth;
		public float Mark = 15.001f;
		public AIActor ai;
		public PlayerController playa;
	}
}