
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;
using ItemAPI;
using System.Collections.ObjectModel;
using Pathfinding;

namespace Planetside
{
	public static class TrespassChallengeShrine
	{
		public static void Add()
		{
			string defaultPath = "Planetside/Resources/DungeonObjects/TrespassObjects/TrespassContainer/";
			ShrineFactory iei = new ShrineFactory
			{
				name = "TrespassChallengeShrine",
				modID = "psog",
				text = "You feel an ominous energy coming from this statue.",
				spritePath = defaultPath + "trespassContainer_idle_001.png",
				acceptText = "Close the vessel.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = defaultPath + "trespassContainer_shadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.125f,		
				usesCustomColliderOffsetAndSize = true,
				colliderSize = new IntVector2(32, 24),
				colliderOffset = new IntVector2(1, 0),
				RoomIconSpritePath = defaultPath + "trespassContainer_idle_001.png",
				AdditionalComponent = typeof(TresspassLightController)
			};
			string[] L = new string[]
			{
				defaultPath+"trespassContainer_idle_001.png",
				defaultPath+"trespassContainer_idle_002.png",
				defaultPath+"trespassContainer_idle_003.png",
				defaultPath+"trespassContainer_idle_004.png",
				defaultPath+"trespassContainer_idle_005.png",
				defaultPath+"trespassContainer_idle_006.png",
				defaultPath+"trespassContainer_idle_007.png",
				defaultPath+"trespassContainer_idle_008.png",
			};
			string[] H = new string[]
			{
				defaultPath+"trespassContainer_break_001.png",
				defaultPath+"trespassContainer_break_002.png",
				defaultPath+"trespassContainer_break_003.png",
				defaultPath+"trespassContainer_break_004.png",
				defaultPath+"trespassContainer_break_005.png",
				defaultPath+"trespassContainer_break_006.png",
				defaultPath+"trespassContainer_break_007.png",
				defaultPath+"trespassContainer_break_008.png",
				defaultPath+"trespassContainer_break_009.png",
			};
			GameObject self = iei.BuildWithAnimations(L, 6, H, 16);


		
			WeightedWaves = new WeightedTypeCollection<List<string>>();
			WeightedWaves.elements = new WeightedType<List<string>>[]
			{
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Inquisitor.guid}, 0.2f),
				GenerateQuickWeightedString(new List<string>(){Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid, Observant.guid}, 1),
				GenerateQuickWeightedString(new List<string>(){Collective.guid, Collective.guid, Unwilling.guid,  Unwilling.guid}, 0.66f),
				GenerateQuickWeightedString(new List<string>(){Observant.guid, Observant.guid, Observant.guid, Observant.guid, Observant.guid, Observant.guid, Observant.guid } , 0.7f),
				GenerateQuickWeightedString(new List<string>(){Vessel.guid, Stagnant.guid, Stagnant.guid, Stagnant.guid, Stagnant.guid,Stagnant.guid} , 0.5f),
				GenerateQuickWeightedString(new List<string>(){Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid } , 0.5f),
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Observant.guid, Observant.guid}, 0.5f),
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Creationist.guid, Observant.guid}, 0.5f),
				GenerateQuickWeightedString(new List<string>(){Stagnant.guid, Stagnant.guid, Stagnant.guid,Stagnant.guid,Stagnant.guid, Observant.guid,Observant.guid}, 0.6f),
				GenerateQuickWeightedString(new List<string>(){Vessel.guid, Observant.guid, Observant.guid,Observant.guid, Observant.guid }, 0.5f),
			};		
		}





		public static WeightedTypeCollection<List<string>> WeightedWaves;
		private static WeightedType<List<string>> GenerateQuickWeightedString(List<string> str, float weight)
        {
			WeightedType<List<string>> weightedType = new WeightedType<List<string>>();
			weightedType.value = str;
			weightedType.weight = weight;
			return weightedType;
		}

		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

	

		public static void Accept(PlayerController user, GameObject shrine)
		{
			tk2dSpriteAnimator animator = shrine.GetComponent<tk2dSpriteAnimator>();
			animator.Play("use");
			GameManager.Instance.StartCoroutine(DoDelayStuff(shrine, user, shrine.GetComponent<SimpleShrine>()));
		}


		public static IEnumerator VoidHoleShrinkage(VoidHoleController voidHoleController, GameObject partObj)
        {
			float elaWait = 0;
			voidHoleController.CanHurt = false;
			while (elaWait < 2f)
            {
				elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min((elaWait/1.5f), 1);
				partObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 6.25f, t);
				yield return null;
			}
			elaWait = 0;
			voidHoleController.CanHurt = true;
			while (elaWait < 15f)
			{
				elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min((elaWait / 20), 1);
				voidHoleController.Radius = Mathf.Lerp(28.5f, 10f, t);
				partObj.transform.localScale = Vector3.Lerp(Vector3.one * 6.25f, Vector3.one * 2.25f, t);
				yield return null;
			}
			yield break;
        }

		public static IEnumerator VoidHoleAway(VoidHoleController voidHoleController, GameObject partObj)
		{
			float elaWait = 0;
			voidHoleController.CanHurt = false;
			while (elaWait < 3f)
			{
				elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min(elaWait/3, 1);
				voidHoleController.Radius = Mathf.Lerp(10f, 50f, t);
				partObj.transform.localScale = Vector3.Lerp(Vector3.one * 3, Vector3.one * 10, t);
				yield return null;
			}
			UnityEngine.Object.Destroy(partObj);
			yield break;
		}


		public static IEnumerator DoDelayStuff(GameObject Shrine, PlayerController user, SimpleShrine shrineSelf)
        {

			user.CurrentRoom.PreventStandardRoomReward = true;


			yield return new WaitForSeconds(0.25f);
			GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Amogus"));
			MeshRenderer rend = partObj.GetComponentInChildren<MeshRenderer>();
			rend.allowOcclusionWhenDynamic = true;
			partObj.transform.position = Shrine.GetComponent<tk2dBaseSprite>().WorldCenter.ToVector3ZisY().WithZ(50);
			partObj.name = "VoidHole";
			partObj.transform.localScale = Vector3.zero;
			VoidHoleController voidHoleController = partObj.AddComponent<VoidHoleController>();
			voidHoleController.trueCenter = Shrine.GetComponent<tk2dBaseSprite>().WorldCenter;
			voidHoleController.CanHurt = false;
			voidHoleController.Radius = 30;
			GameManager.Instance.StartCoroutine(VoidHoleShrinkage(voidHoleController, partObj));

			int WavesSpawned = 0;
			IntVector2? targetCenter = new IntVector2?(user.CenterPosition.ToIntVector2(VectorConversions.Floor));
			List<string> WAVE = WeightedWaves.SelectByWeight();
			WavesSpawned++;
			for (int i = 0; i < WAVE.Count; i++)
            {
				DoEnemySpawn(targetCenter, user, WAVE[i]);
				if (shrineSelf.instanceRoom.IsDarkAndTerrifying == false)
                {
					user.CurrentRoom.SealRoom();
					shrineSelf.instanceRoom.BecomeTerrifyingDarkRoom(3f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
				}
				yield return new WaitForSeconds(0.75f);
			}

			user.CurrentRoom.OnEnemiesCleared += () =>
			{
				if (WavesSpawned == 3)
                {
					shrineSelf.instanceRoom.EndTerrifyingDarkRoom(2f);
					GameManager.Instance.StartCoroutine(VoidHoleAway(voidHoleController, partObj));
				}
				else
                {
					if (WavesSpawned == 2) { MarkAsUnseal(user.CurrentRoom); }
					WavesSpawned++;
					GameManager.Instance.StartCoroutine(SpawnWave(shrineSelf, targetCenter, user));
				}
			};
			yield break;
        }
		public static IEnumerator SpawnWave(SimpleShrine shrineSelf, IntVector2? targetCenter, PlayerController user)
		{
			GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(user.CurrentRoom);
			List<string> WAVE = WeightedWaves.SelectByWeight();
			for (int i = 0; i < WAVE.Count; i++)
			{
				DoEnemySpawn(targetCenter, user, WAVE[i]);
				if (shrineSelf.instanceRoom.IsDarkAndTerrifying == false)
				{
					user.CurrentRoom.SealRoom();
					shrineSelf.instanceRoom.BecomeTerrifyingDarkRoom(3f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
				}
				yield return new WaitForSeconds(0.75f);
			}
			yield break;
		}
		public static void MarkAsUnseal(RoomHandler room)
        {
			room.area.runtimePrototypeData.roomEvents.Add(new RoomEventDefinition(RoomEventTriggerCondition.ON_ENEMIES_CLEARED, RoomEventTriggerAction.UNSEAL_ROOM));
		}

		private static void DoEnemySpawn(IntVector2? targetCenter, PlayerController user, string EnemyToSpawn)
        {
			AIActor enemyPrefab = EnemyDatabase.GetOrLoadByGuid(EnemyToSpawn);
			CellValidator cellValidator = delegate (IntVector2 c)
			{
				for (int j = 0; j < enemyPrefab.Clearance.x; j++)
				{
					for (int k = 0; k < enemyPrefab.Clearance.y; k++)
					{
						if (GameManager.Instance.Dungeon.data.isTopWall(c.x + j, c.y + k))
						{
							return false;
						}
						if (targetCenter != null)
						{
							if (IntVector2.Distance(targetCenter.Value, c.x + j, c.y + k) < 4f)
							{
								return false;
							}
							if (IntVector2.Distance(targetCenter.Value, c.x + j, c.y + k) > 20f)
							{
								return false;
							}
						}
					}
				}
				return true;
			};
			IntVector2? randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2?(enemyPrefab.Clearance), new CellTypes?(enemyPrefab.PathableTiles), false, cellValidator);
			if (randomAvailableCell == null)
			{
				randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2?(enemyPrefab.Clearance), new CellTypes?(enemyPrefab.PathableTiles), false, cellValidator);
			}
			if (randomAvailableCell != null)
			{
				AIActor aiactor = AIActor.Spawn(enemyPrefab, randomAvailableCell.Value, user.CurrentRoom, true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor.HandleReinforcementFallIntoRoom(0f);
				user.CurrentRoom.RegisterEnemy(aiactor);
				user.CurrentRoom.SealRoom();
			}
		}
	}
}



