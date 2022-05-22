using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dungeonator;
using UnityEngine;

namespace Planetside
{
	public class MultiCompanionItem : PassiveItem
	{
		public MultiCompanionItem()
		{
			this.SacrificeGunDuration = 30f;
			this.m_lastActiveSynergyTransformation = -1;

		}

		public List<GameObject> ExtantCompanions()
		{
			return extantCompanions;
		}

		public GameObject ExtantCompanionFromList(int positionInList)
		{
			return extantCompanions[positionInList];
		}

		private void CreateCompanion(PlayerController owner)
		{
			if (this.PreventRespawnOnFloorLoad)
			{
				return;
			}
			if (this.BabyGoodMimicOrbitalOverridden)
			{
				GameObject extantCompanion = PlayerOrbitalItem.CreateOrbital(owner, (!this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab) ? this.OverridePlayerOrbitalItem.OrbitalPrefab.gameObject : this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab.gameObject, this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab, null);
				extantCompanions.Add(extantCompanion);
				return;
			}
			List<string> guids = this.CompanionGuids;
			for (int e = 0; e < guids.Count; e++)
			{
				string guid = guids[e];

				this.m_lastActiveSynergyTransformation = -1;
				if (this.UsesAlternatePastPrefab && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
				{
					guid = this.CompanionPastGuid;
				}
				else if (this.Synergies.Length > 0)
				{
					for (int i = 0; i < this.Synergies.Length; i++)
					{
						if (owner.HasActiveBonusSynergy(this.Synergies[i].RequiredSynergy, false))
						{
							guid = this.Synergies[i].SynergyCompanionGuid;
							this.m_lastActiveSynergyTransformation = i;
						}
					}
				}
				AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
				Vector3 vector = owner.transform.position;
				if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
				{
					vector += new Vector3(1.125f, -0.3125f, 0f);
				}
				GameObject extantCompanion2 = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
				CompanionController orAddComponent = extantCompanion2.GetOrAddComponent<CompanionController>();
				orAddComponent.Initialize(owner);
				if (orAddComponent.specRigidbody)
				{
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
				}
				if (orAddComponent.companionID == CompanionController.CompanionIdentifier.BABY_GOOD_MIMIC)
				{
					GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GOT_BABY_MIMIC, true);
				}
				ETGModConsole.Log("1");
				if (SavedCompanionComponents.Count > 0)
				{
					ETGModConsole.Log("2");
					if (SavedCompanionComponents[e] != null)
					{
						ETGModConsole.Log("3");
						Component H = extantCompanion2.GetComponent(SavedCompanionComponents[e].GetType());
						if (H.GetType() == SavedCompanionComponents[e].GetType() && H != null)
						{
							PlanetsideReflectionHelper.ReflectionShallowCopyFields(H, SavedCompanionComponents[e], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
						}
					}
				}
				extantCompanions.Add(extantCompanion2);
			}
		}



		public void ForceCompanionRegeneration(PlayerController owner, Vector2? overridePosition)
		{
			bool flag = false;
			Vector2 vector = Vector2.zero;
			if (extantCompanions.Count != 0 && extantCompanions != null)
			{
				for (int i = 0; i < extantCompanions.Count; i++)
				{
					GameObject companion = extantCompanions[i];
					if (companion)
					{
						flag = true;
						vector = companion.transform.position.XY();
					}
					if (overridePosition != null)
					{
						flag = true;
						vector = overridePosition.Value;
					}
					this.DestroyCompanion();
					this.CreateCompanion(owner);
					if (companion && flag)
					{
						companion.transform.position = vector.ToVector3ZisY(0f);
						SpeculativeRigidbody component = companion.GetComponent<SpeculativeRigidbody>();
						if (component)
						{
							component.Reinitialize();
						}
					}
				}

			}


		}

		public void ForceDisconnectCompanion()
		{
			extantCompanions.Clear();
		}

		private void DestroyCompanion()
		{
			for (int i = 0; i < extantCompanions.Count; i++)
			{
				GameObject companion = extantCompanions[i];
				if (companion != null)
				{

					Component[] components = companion.GetComponents(typeof(Component));
					for (int e = 0; e < components.Length; e++)
                    {
						if (components[e] != null && components[e].GetType().Name.ToLower().Contains("transferrable"))
						{
							Component component = new Component();

							ETGModConsole.Log(component.GetType().Name);

							SavedCompanionComponents.Add(components[e]);
						}
					}

				
					ETGModConsole.Log("u");

					UnityEngine.Object.Destroy(companion);
				}
			}
			extantCompanions.Clear();
		}


		protected override void Update()
		{
			base.Update();
			if (!Dungeon.IsGenerating && this.m_owner && this.Synergies.Length > 0)
			{
				if (!this.UsesAlternatePastPrefab || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST)
				{
					bool flag = false;
					for (int i = this.Synergies.Length - 1; i >= 0; i--)
					{
						if (this.m_owner.HasActiveBonusSynergy(this.Synergies[i].RequiredSynergy, false))
						{
							if (this.m_lastActiveSynergyTransformation != i)
							{
								this.DestroyCompanion();
								this.CreateCompanion(this.m_owner);
							}
							flag = true;
							break;
						}
					}
					if (!flag && this.m_lastActiveSynergyTransformation != -1)
					{
						this.DestroyCompanion();
						this.CreateCompanion(this.m_owner);
					}
				}
			}
		}

		public override void Pickup(PlayerController player)
		{
			base.Pickup(player);
			player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Combine(player.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
			this.CreateCompanion(player);
		}

		private void HandleNewFloor(PlayerController obj)
		{
			this.DestroyCompanion();
			if (!this.PreventRespawnOnFloorLoad)
			{
				this.CreateCompanion(obj);
			}
		}

		public override DebrisObject Drop(PlayerController player)
		{
			this.DestroyCompanion();
			player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(player.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
			return base.Drop(player);
		}

		protected override void OnDestroy()
		{
			if (this.m_owner != null)
			{
				PlayerController owner = this.m_owner;
				owner.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(owner.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
			}
			this.DestroyCompanion();
			base.OnDestroy();
		}

		//public List<Component> CompanionComponentsToTransfer = new List<Component>();

		private List<Component> SavedCompanionComponents = new List<Component>();




		public List<string> CompanionGuids = new List<string>();

		public bool UsesAlternatePastPrefab;


		public string CompanionPastGuid;

		public CompanionTransformSynergy[] Synergies;

		public bool PreventRespawnOnFloorLoad;

		public bool HasGunTransformationSacrificeSynergy;

		public CustomSynergyType GunTransformationSacrificeSynergy;

		public int SacrificeGunID;

		public float SacrificeGunDuration;

		public bool BabyGoodMimicOrbitalOverridden;

		public PlayerOrbitalItem OverridePlayerOrbitalItem;

		private int m_lastActiveSynergyTransformation;

		private List<GameObject> extantCompanions = new List<GameObject>();
	}

}