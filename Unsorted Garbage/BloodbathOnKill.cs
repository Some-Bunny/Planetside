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
	public class BloodOnKill : BraveBehaviour
	{
		private void Awake()
		{
			if (this.m_aiactor != null)
			{
				this.m_aiactor = base.GetComponent<AIActor>();
				this.m_aiactor.healthHaver.OnPreDeath += Die;
			}
		}
		private AIActor m_aiactor;
		private void Die(Vector2 obj)
		{
			this.teleporter = PickupObjectDatabase.GetById(449).GetComponent<TeleporterPrototypeItem>();
			UnityEngine.Object.Instantiate<GameObject>(this.teleporter.TelefragVFXPrefab, obj, Quaternion.identity);
		}
		private TeleporterPrototypeItem teleporter;
	}
}