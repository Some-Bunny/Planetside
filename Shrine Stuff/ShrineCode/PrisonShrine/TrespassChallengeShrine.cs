
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
				RoomIconSpritePath = "Planetside/Resources/Shrines/iconsmiley.png",
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
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Inquisitor.guid}, 0.25f),
				GenerateQuickWeightedString(new List<string>(){Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid, Observant.guid}, 0.875f),
				GenerateQuickWeightedString(new List<string>(){Collective.guid, Collective.guid, Unwilling.guid,  Unwilling.guid}, 0.5f),
				GenerateQuickWeightedString(new List<string>(){Observant.guid, Observant.guid, Observant.guid, Observant.guid, Observant.guid } , 0.5f),
				GenerateQuickWeightedString(new List<string>(){Vessel.guid, Observant.guid, Observant.guid, Creationist.guid, Creationist.guid} , 0.5f),
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Observant.guid, Observant.guid}, 0.626f),
				GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Creationist.guid, Observant.guid}, 0.625f),
				GenerateQuickWeightedString(new List<string>(){Vessel.guid, Observant.guid, Observant.guid,Observant.guid}, 0.375f),
				GenerateQuickWeightedString(new List<string>(){Vessel.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid, Unwilling.guid}, 0.33f),
                GenerateQuickWeightedString(new List<string>(){Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid, Creationist.guid}, 0.5f),
                GenerateQuickWeightedString(new List<string>(){Inquisitor.guid, Vessel.guid}, 0.2f),
                GenerateQuickWeightedString(new List<string>(){ Vessel.guid, Vessel.guid, Creationist.guid, Creationist.guid}, 0.2f),
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
			shrine.GetComponent<CustomShrineController>().numUses++;
			Minimap.Instance.DeregisterRoomIcon(shrine.GetComponent<SimpleShrine>().instanceRoom, shrine.GetComponent<SimpleShrine>().instanceMinimapIcon);
			tk2dSpriteAnimator animator = shrine.GetComponent<tk2dSpriteAnimator>();
			animator.Play("use");
			GameManager.Instance.StartCoroutine(DoDelayStuff(shrine, user));
			shrine.GetComponent<SimpleShrine>().instanceRoom.DeregisterInteractable(shrine.GetComponent<SimpleShrine>());

        }


		public static IEnumerator VoidHoleShrinkage(VoidHoleController voidHoleController)
        {
			float elaWait = 0;
			if (voidHoleController.gameObject == null) { yield break; }
			voidHoleController.CanHurt = false;
			while (elaWait < 2f)
            {
                if (voidHoleController.gameObject == null) { yield break; }
                elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min((elaWait/1.5f), 1);

				voidHoleController.SetScale(Vector3.Lerp(Vector3.zero, Vector3.one * 100f, t));// .transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 6.25f, t);
                yield return null;
			}
			elaWait = 0;
            if (voidHoleController.gameObject == null) { yield break; }
            voidHoleController.CanHurt = true;
			while (elaWait < 15f)
			{
                if (voidHoleController.gameObject == null) { yield break; }
                elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min((elaWait / 20), 1);
				//voidHoleController.Radius = Mathf.Lerp(28.5f, 8f, t);
                voidHoleController.ChangeHoleSize(Mathf.Lerp(0.3f, 0.08f, t));

                //partObj.transform.localScale = Vector3.Lerp(Vector3.one * 6.25f, Vector3.one * 2, t);
                yield return null;
			}
			yield break;
        }

		public static IEnumerator VoidHoleAway(VoidHoleController voidHoleController)
		{
			float elaWait = 0;
			voidHoleController.CanHurt = false;
			while (elaWait < 3f)
			{
				elaWait += BraveTime.DeltaTime;
				float t = Mathf.Min(elaWait/3, 1);
				//voidHoleController.Radius = Mathf.Lerp(10f, 50, t);
				voidHoleController.ChangeHoleSize(Mathf.Lerp(0.1f, 2, t));
                //voidHoleController.transform.localScale = Vector3.Lerp(Vector3.one * 3, Vector3.one * 10, t);
                yield return null;
			}
			UnityEngine.Object.Destroy(voidHoleController.gameObject);
			yield break;
		}


		public static GameObject DoSpawnPortalHole(Vector3 pos)
        {
			GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Portal"));
			MeshRenderer rend = partObj.GetComponent<MeshRenderer>();
			rend.allowOcclusionWhenDynamic = true;
			partObj.transform.position = pos - new Vector3(0, 2);
			partObj.transform.localScale = Vector3.zero;
			partObj.name = "DecorativePortal";
			partObj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
			PortalVisualsController visuals = partObj.AddComponent<PortalVisualsController>();
			visuals.LTS(Vector3.zero, Vector3.one * 2.5f, 1.2f);
			visuals.LSV(0.05f, 0.1f, 0.33f, "_OutlineWidth");
			visuals.LSV(10f, 45f, 0.33f, "_OutlinePower");
			return partObj;	
		}


		


		public static IEnumerator DoDelayStuff(GameObject Shrine, PlayerController user)
        {
			RoomHandler Room = user.CurrentRoom;

			user.CurrentRoom.PreventStandardRoomReward = true;
			yield return new WaitForSeconds(0.25f);

			GameObject coolPortal = DoSpawnPortalHole(Shrine.GetComponent<tk2dBaseSprite>().WorldCenter + new Vector2(0, 1));
			PortalVisualsController portalVisuals = coolPortal.GetComponent<PortalVisualsController>();
			EmergencyPlayerDisappearedFromRoom disappearedFromRoomController = portalVisuals.AddComponent<EmergencyPlayerDisappearedFromRoom>();
			disappearedFromRoomController.roomAssigned = user.CurrentRoom;
			

			StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(coolPortal.transform.position);
			yield return new WaitForSeconds(1);


            /*
			GameObject partObj = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Amogus"));
			MeshRenderer rend = partObj.GetComponentInChildren<MeshRenderer>();
			rend.allowOcclusionWhenDynamic = true;
			partObj.transform.position = Shrine.GetComponent<tk2dBaseSprite>().WorldCenter.ToVector3ZisY().WithZ(50);
			partObj.name = "VoidHole";
			partObj.transform.localScale = Vector3.zero;
			*/

            var voidHoleController = VoidHoleController.SpawnVoidHole(Shrine.GetComponent<tk2dBaseSprite>().WorldCenter.ToVector3ZisY().WithZ(50), Vector2.zero);
            voidHoleController.CanHurt = false;
            voidHoleController.Radius = 30;
            voidHoleController.ChangeHoleSize(0.285f);
            voidHoleController.InitializeHole(null, 8, 2 * Time.deltaTime, 0.00333f * Time.deltaTime, Vector2.zero);
            GameManager.Instance.StartCoroutine(VoidHoleShrinkage(voidHoleController));

			disappearedFromRoomController.PlayerSuddenlyDisappearedFromRoom += (obj) =>
			{
				Room.UnsealRoom();
				Room.EndTerrifyingDarkRoom(0);
				UnityEngine.Object.Destroy(voidHoleController.gameObject);
				UnityEngine.Object.Destroy(coolPortal);
			};


			AkSoundEngine.PostEvent("Play_PortalOpen", coolPortal.gameObject);
			AkSoundEngine.PostEvent("Play_PortalOpen", coolPortal.gameObject);
			Exploder.DoDistortionWave(coolPortal.transform.position, 1.5f, 0.25f, 30, 2f);
			UnityEngine.Object.Destroy(Shrine);
			portalVisuals.ConstantlyPulsates = true;


			int WavesSpawned = 0;
			IntVector2? targetCenter = new IntVector2?(user.CenterPosition.ToIntVector2(VectorConversions.Floor));
			List<string> WAVE = WeightedWaves.SelectByWeight();
			for (int i = 0; i < WAVE.Count; i++)
            {
				DoEnemySpawn(targetCenter, user, WAVE[i]);
				if (user.CurrentRoom.IsDarkAndTerrifying == false)
                {
					GameManager.Instance.DungeonMusicController.SwitchToActiveMusic(null);
					user.CurrentRoom.SealRoom();
					user.CurrentRoom.BecomeTerrifyingDarkRoom(3f, 0.1f, 0.05f, "Play_ENM_darken_world_01");
				}
				yield return new WaitForSeconds(0.75f);
			}
			WavesSpawned++;

			user.CurrentRoom.OnEnemiesCleared += () =>
			{
				if (WavesSpawned == 3)
                {
					portalVisuals.ConstantlyPulsates = false;
                    portalVisuals.LTS(Vector3.one * 2.5f, Vector3.zero, 1.2f);
                    portalVisuals.LSV(0.3f, 0f, 1.25f, "_OutlineWidth");
                    portalVisuals.LSV(105f, 10f, 3f, "_OutlinePower");

                    user.CurrentRoom.UnsealRoom();
					user.CurrentRoom.EndTerrifyingDarkRoom(2f);
					GameManager.Instance.StartCoroutine(VoidHoleAway(voidHoleController));


					GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");

					GameObject vfx = GameObject.Instantiate(silencerVFX.gameObject, coolPortal.transform.position, Quaternion.identity);
					UnityEngine.Object.Destroy(vfx, 2);
					for (int i = 0; i < 5; i++)
					{
						int id = BraveUtility.RandomElement<int>(RobotShopkeeperBoss.Lootdrops);
						DebrisObject pickups = LootEngine.SpawnItem(PickupObjectDatabase.GetById(id).gameObject, coolPortal.transform.position + new Vector3(0.25f, 0), MathToolbox.GetUnitOnCircle(72 * i, 1), 4f, false, true, false);
					}

					LootEngine.SpawnItem(UnityEngine.Random.value > 0.5f ? PickupObjectDatabase.GetRandomGunOfQualities(new System.Random(UnityEngine.Random.Range(1, 100)), new List<int> { }, new PickupObject.ItemQuality[] { PickupObject.ItemQuality.C, PickupObject.ItemQuality.C, PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S }).gameObject : PickupObjectDatabase.GetRandomPassiveOfQualities(new System.Random(UnityEngine.Random.Range(1, 100)), new List<int> { }, new PickupObject.ItemQuality[] { PickupObject.ItemQuality.B, PickupObject.ItemQuality.B, PickupObject.ItemQuality.A, PickupObject.ItemQuality.S }).gameObject, coolPortal.transform.position + new Vector3(0.25f, 0), Vector2.down, 2f, false, true, false);

					UnityEngine.Object.Destroy(coolPortal, 3);
				}
				else
                {
					WavesSpawned++;
					GameManager.Instance.StartCoroutine(SpawnWave(coolPortal, targetCenter, user, true, portalVisuals));
				}
			};
			yield break;
        }
		public static IEnumerator SpawnWave(GameObject portal, IntVector2? targetCenter, PlayerController user, bool DoDelay, PortalVisualsController portalVisualsController)
		{
			if (DoDelay == true)
            {
				portalVisualsController.ConstantlyPulsates = false;
				StaticVFXStorage.HighPriestClapVFXInverse.SpawnAtPosition(portal.transform.position);
				yield return new WaitForSeconds(1);
				AkSoundEngine.PostEvent("Play_PortalOpen", portal.gameObject);
				AkSoundEngine.PostEvent("Play_PortalOpen", portal.gameObject);
				Exploder.DoDistortionWave(portal.transform.position, 1.5f, 0.25f, 30, 2f);
				portalVisualsController.ConstantlyPulsates = true;
				portalVisualsController.TimeBetweenPulses -= 0.3f;

				portalVisualsController.LSV(portalVisualsController.ReturnKeyWordValue("_OutlineWidth"), portalVisualsController.ReturnKeyWordValue("_OutlineWidth")+ 0.1f, 1.25f, "_OutlineWidth");
				portalVisualsController.LSV(portalVisualsController.ReturnKeyWordValue("_OutlinePower"), portalVisualsController.ReturnKeyWordValue("_OutlinePower")+30f, 1f, "_OutlinePower");


			}
			PlanetsideReflectionHelper.ReflectSetField<DungeonFloorMusicController.DungeonMusicState>(typeof(DungeonFloorMusicController), "m_currentState", DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A, GameManager.Instance.DungeonMusicController);
			GameManager.Instance.DungeonMusicController.SwitchToActiveMusic(null);

			List<string> WAVE = WeightedWaves.SelectByWeight().Shuffle();
			if (user.CurrentRoom.IsDarkAndTerrifying == false)
			{
				user.CurrentRoom.SealRoom();
				user.CurrentRoom.BecomeTerrifyingDarkRoom(3f, 0.5f, 0.1f, "Play_ENM_darken_world_01");
			}
			for (int i = 0; i < WAVE.Count; i++)
			{
				DoEnemySpawn(targetCenter, user, WAVE[i]);
				yield return new WaitForSeconds(0.25f);
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


namespace Planetside
{
	public class PortalVisualsController : MonoBehaviour
    {
		public PortalVisualsController()
        {
			ConstantlyPulsates = false;
			TimeBetweenPulses = 1;
		}
		public bool ConstantlyPulsates;
		public float TimeBetweenPulses;
		private float ela;

		public void LSV(float prevSize, float afterSize, float duration, string KeyWord)
		{
			GameManager.Instance.StartCoroutine(LerpShaderValue(prevSize, afterSize, duration, KeyWord));
		}

		public void Update()
        {
			if (gameObject != null)
            {
				ela += BraveTime.DeltaTime;
				if (ela > TimeBetweenPulses)
				{
					ela = 0;
					if (ConstantlyPulsates == true)
					{
						Exploder.DoDistortionWave(gameObject.transform.position, 3f, 0.05f, 20, 0.5f);
					}
				}
			}
        }



		private IEnumerator LerpShaderValue(float prevSize, float afterSize, float duration, string KeyWord)
		{
			float elaWait = 0f;
			float duraWait = duration;
			while (elaWait < duraWait)
			{
				elaWait += BraveTime.DeltaTime;
                if (this == null) { yield break; }
                if (gameObject == null) { yield break; }

                float t = elaWait / duraWait;
				if (gameObject != null)
				{
					gameObject.GetComponent<MeshRenderer>().material.SetFloat(KeyWord, Mathf.Lerp(prevSize, afterSize, t));
				}
				yield return null;
			}
			yield break;
		}

		public void LTS(Vector3 prevSize, Vector3 afterSize, float duration)
		{
			GameManager.Instance.StartCoroutine(LerpToSize(prevSize, afterSize, duration));
        }

		
		public float ReturnKeyWordValue(string KeyWord)
        {
			return gameObject.GetComponent<MeshRenderer>().material.GetFloat(KeyWord);

		}

		private IEnumerator LerpToSize(Vector3 prevSize, Vector3 afterSize, float duration)
		{

            if (this == null) { yield break; }
            if (gameObject == null) { yield break; }
            if (gameObject != null)
			{
				gameObject.transform.localScale = prevSize;
                if (this == null) { yield break; }

                float elaWait = 0f;
				float duraWait = duration;
				while (elaWait < duraWait)
				{
					elaWait += BraveTime.DeltaTime;
					float t = elaWait / duraWait;
                    if (this == null) { yield break; }

                    if (gameObject == null) { yield break; }
					if (gameObject != null)
					{
						gameObject.transform.localScale = Vector3.Lerp(prevSize, afterSize, t);
					}
					yield return null;
				}
			}

			yield break;
		}
	}
}
