
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

namespace Planetside
{
	public class InfectionReplacement : AGDEnemyReplacementTier
    {

		public static void Start(List<AGDEnemyReplacementTier> m_cachedReplacementTiers)
		{
			
			List<GlobalDungeonData.ValidTilesets> targetTilesets = new List<GlobalDungeonData.ValidTilesets>
			{
				GlobalDungeonData.ValidTilesets.CASTLEGEON,
				GlobalDungeonData.ValidTilesets.SEWERGEON,
				GlobalDungeonData.ValidTilesets.JUNGLEGEON,
				GlobalDungeonData.ValidTilesets.GUNGEON,
				GlobalDungeonData.ValidTilesets.CATHEDRALGEON,
				GlobalDungeonData.ValidTilesets.BELLYGEON,
				GlobalDungeonData.ValidTilesets.MINEGEON,
				GlobalDungeonData.ValidTilesets.RATGEON,
				GlobalDungeonData.ValidTilesets.CATACOMBGEON,
				GlobalDungeonData.ValidTilesets.OFFICEGEON,
				GlobalDungeonData.ValidTilesets.WESTGEON,
				GlobalDungeonData.ValidTilesets.PHOBOSGEON,
				GlobalDungeonData.ValidTilesets.FORGEGEON,
				GlobalDungeonData.ValidTilesets.HELLGEON,
				GlobalDungeonData.ValidTilesets.SPACEGEON
			};

			List<string> creationist = new List<string>()
			{
				Creationist.guid,
			};

			List<string> observant = new List<string>()
			{
				Observant.guid,
			};
			List<string> inquisitor = new List<string>()
			{
				Inquisitor.guid,
			};

			List<string> vessel = new List<string>()
			{
				Vessel.guid,
			};

			List<string> collective = new List<string>()
			{
				Collective.guid,
			};
			List<string> stagnant = new List<string>()
			{
				Stagnant.guid,
			};

			List<string> bulletKinEnemies = new List<string>()
			{
			"01972dee89fc4404a5c408d50007dad5",
			"db35531e66ce41cbb81d507a34366dfe",
			"70216cae6c1346309d86d4a0b4603045",
			"88b6b6a93d4b4234a67844ef4728382c",
			"5f3abc2d561b4b9c9e72b879c6f10c7e",
			"1a78cfb776f54641b832e92c44021cf2",
			"8bb5578fba374e8aae8e10b754e61d62",
			"39e6f47a16ab4c86bec4b12984aece4c",
			"f020570a42164e2699dcf57cac8a495c",
			"5861e5a077244905a8c25c2b7b4d6ebb",
			"9eba44a0ea6c4ea386ff02286dd0e6bd",
			"906d71ccc1934c02a6f4ff2e9c07c9ec",
			"d4a9836f8ab14f3fadd0f597438b1f1f"
			};

			List<string> SniperlikeEnemies = new List<string>()
			{
				Glockulus.guid,
				"31a3ea0c54a745e182e22ea54844a82d",
				"c5b11bfc065d417b9c4d03a5e385fe2c"
			};


			List<string> GunnutlikeEnemies = new List<string>()
			{
				ArchGunjurer.guid,
				"da797878d215453abba824ff902e21b4",
				"383175a55879441d90933b5c4e60cf6f",
				"463d16121f884984abe759de38418e48"
			};

			List<string> bufferEnemies = new List<string>()
			{
				"8a9e9bedac014a829a48735da6daf3da",
				"c50a862d19fc4d30baeba54795e8cb93",
				"8b4a938cdbc64e64822e841e482ba3d2",
				"ba657723b2904aa79f9e51bce7d23872",
				"b1540990a4f1480bbcb3bea70d67f60d"
			};
			List<string> strongEnemiesToReplace = new List<string>()
			{

				"98ea2fe181ab4323ab6e9981955a9bca",
				Barretina.guid,
				"cd4a4b7f612a4ba9a720b9f97c52f38c",
				"1a4872dafdb34fd29fe8ac90bd2cea67",
				"981d358ffc69419bac918ca1bdf0c7f7"

			};

			List<string> shotgunEnemies = new List<string>()
			{

				"128db2f0781141bcb505d8f00f9e4d47",
				"b54d89f9e802455cbb2b8a96a31e8259",
				"2752019b770f473193b08b4005dc781f",
				"1bd8e49f93614e76b140077ff2e33f2b",
				"7f665bd7151347e298e4d366f8818284"

			};

			List<string> batEnemies = new List<string>()
			{

				"2feb50a6a40f4f50982e89fd276f6f15",
				"2d4f8b5404614e7d8b235006acde427a",
				"b4666cb6ef4f4b038ba8924fd8adf38f",
				"7ec3e8146f634c559a7d58b19191cd43",
			};


			DungeonPrerequisite[] fuck = new DungeonPrerequisite[]
			
			{
				 new CustomDungeonPrerequisite
					{
						advancedPrerequisiteType = CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_STAT_COMPARISION,
						customStatToCheck = CustomTrackedStats.INFECTION_FLOORS_ACTIVATED,
						useSessionStatValue = true,
						prerequisiteOperation = DungeonPrerequisite.PrerequisiteOperation.EQUAL_TO,
						comparisonValue = 1
					}
			};


			List<List<AGDEnemyReplacementTier>> TheList = new List<List<AGDEnemyReplacementTier>>()
			{
				InfectionReplacement.GenerateEnemyReplacementTiers("creationist", fuck, targetTilesets, bulletKinEnemies, creationist, 0.125f),
				InfectionReplacement.GenerateEnemyReplacementTiers("observant", fuck, targetTilesets, SniperlikeEnemies, observant, 0.4f),
				InfectionReplacement.GenerateEnemyReplacementTiers("inquisitor", fuck, targetTilesets, GunnutlikeEnemies, inquisitor, 0.33f),
				InfectionReplacement.GenerateEnemyReplacementTiers("vessel", fuck, targetTilesets, bufferEnemies, vessel, 0.3f),
				InfectionReplacement.GenerateEnemyReplacementTiers("collective", fuck, targetTilesets, strongEnemiesToReplace, collective, 0.25f),
				InfectionReplacement.GenerateEnemyReplacementTiers("collective2", fuck, targetTilesets, shotgunEnemies, collective, 0.15f),
				InfectionReplacement.GenerateEnemyReplacementTiers("stagnant", fuck, targetTilesets, batEnemies, stagnant, 0.1f),

			};
			
	
			foreach (List<AGDEnemyReplacementTier> itemNest in TheList)
			{
				foreach (AGDEnemyReplacementTier item in itemNest)
                {
					m_cachedReplacementTiers.Add(item);
				}
			}
		}

		public static void InitSpecialMods()
		{
			if (!GameManager.Instance | !GameManager.Instance.Dungeon)
			{
				return;
			}
			List<AGDEnemyReplacementTier> enemyReplacementTiers = GameManager.Instance.EnemyReplacementTiers;
			if (enemyReplacementTiers != null)
			{
				InfectionReplacement.Start(enemyReplacementTiers);
			}
		}

		public static List<AGDEnemyReplacementTier> GenerateEnemyReplacementTiers(string m_name, DungeonPrerequisite[] m_Prereqs, List<GlobalDungeonData.ValidTilesets> m_TargetTilesets, List<string> m_TargetGuids, List<string> m_ReplacementGUIDs, float m_ChanceToReplace = 1f)
		{
			List<AGDEnemyReplacementTier> list = new List<AGDEnemyReplacementTier>();
			foreach (GlobalDungeonData.ValidTilesets targetTileset in m_TargetTilesets)
			{
				list.Add(new AGDEnemyReplacementTier
				{
					Name = m_name + "_" + targetTileset.ToString(),
					Prereqs = m_Prereqs,
					TargetTileset = targetTileset,
					ChanceToReplace = m_ChanceToReplace,
					MaxPerFloor = -1,
					MaxPerRun = -1,
					TargetAllNonSignatureEnemies = false,
					TargetAllSignatureEnemies = false,
					TargetGuids = m_TargetGuids,
					ReplacementGuids = m_ReplacementGUIDs,
					RoomMustHaveColumns = false,
					RoomMinEnemyCount = -1,
					RoomMaxEnemyCount = -1,
					RoomMinSize = -1,
					RemoveAllOtherEnemies = false,
					RoomCantContain = new List<string>()
				});
			}
			return list;
		}
	}


	public static class PrisonShrine
	{




		public static void Add()
		{
			string basePath = "Planetside/Resources/Shrines/PrisonShrine/";
			ShrineFactory iei = new ShrineFactory
			{
				name = "PrisonShrine",
				modID = "psog",
				text = "A shrine with an inscryption on it, warning of a prisoner inside. Do you release what is contained within?",
				spritePath = basePath+ "bigBrick_idle_001.png",
				acceptText = "Release.",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,
				shadowPath = basePath+ "bigBrickShadow.png",
				ShadowOffsetX = 0f,
				ShadowOffsetY = -0.25f,		
				usesCustomColliderOffsetAndSize = true,
				colliderSize = new IntVector2(46, 42),
				colliderOffset = new IntVector2(1, 0),
				AdditionalComponent = typeof(TresspassLightController)
			};
			string[] H = new string[]
			{
				basePath +"bigBrick_use_001.png",
				basePath +"bigBrick_use_002.png",
				basePath +"bigBrick_use_003.png",
				basePath +"bigBrick_use_004.png",
				basePath +"bigBrick_use_005.png",
				basePath +"bigBrick_use_006.png",
				basePath +"bigBrick_use_007.png",
				basePath +"bigBrick_use_008.png",
				basePath +"bigBrick_use_009.png",
				basePath +"bigBrick_use_010.png",
				basePath +"bigBrick_use_011.png",
				basePath +"bigBrick_use_012.png",
				basePath +"bigBrick_use_013.png",
				basePath +"bigBrick_use_014.png",
				basePath +"bigBrick_use_015.png",
				basePath +"bigBrick_use_016.png",
				basePath +"bigBrick_use_017.png",
			};
			GameObject self = iei.BuildWithAnimations(new string[] {basePath+ "bigBrick_idle_001.png", basePath + "bigBrick_idle_002.png" }, 2, H, 10);
			SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ShrineIcons/prisonShrineIcon", SpriteBuilder.ammonomiconCollection);

		}

		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{

			//SaveAPIManager.RegisterStatChange(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED, 1);

			tk2dSpriteAnimator animator = shrine.GetComponent<tk2dSpriteAnimator>();
			animator.Play("use");


			ReadOnlyCollection<IPlayerInteractable> yes = player.CurrentRoom.GetRoomInteractables();
			for (int i = 0; i < yes.Count; i++)
			{
				IPlayerInteractable touchy = yes[i];
				if (touchy is TrespassDecorativePillarInetractable interaactableObj)
				{
					tk2dSpriteAnimator animator2 = interaactableObj.GetComponent<tk2dSpriteAnimator>();
					animator2.Play("break");
				}
			}


			AkSoundEngine.PostEvent("Play_PortalOpen", shrine.gameObject);


			SimpleShrine simple = shrine.gameObject.GetComponent<SimpleShrine>();
			simple.text = "The energy that once containted what was inside the shrine has departed.";

			GameManager.Instance.MainCameraController.DoContinuousScreenShake(EnemyDatabase.GetOrLoadByGuid("cd88c3ce60c442e9aa5b3904d31652bc").GetComponent<LichDeathController>().hellDragScreenShake, shrine.GetComponent<TresspassLightController>(), false);
			shrine.GetComponent<CustomShrineController>().numUses++;
			shrine.GetComponent<CustomShrineController>().GetRidOfMinimapIcon();
			GameManager.Instance.StartCoroutine(DoDelayStuff(shrine));

			ContainmentBreachController.CurrentState = ContainmentBreachController.States.ALLOWED;
			SaveAPIManager.RegisterStatChange(CustomTrackedStats.INFECTION_FLOORS_ACTIVATED, 1);

			if (GameManager.Instance != null && GameManager.Instance.BestActivePlayer != null)
			{ AkSoundEngine.PostEvent("Play_OBJ_moondoor_close_01", GameManager.Instance.BestActivePlayer.gameObject); }
		}

		public static IEnumerator DoDelayStuff(GameObject Shrine)
        {
			yield return new WaitForSeconds(3f);
			GameManager.Instance.MainCameraController.StopContinuousScreenShake(Shrine.GetComponent<tk2dBaseSprite>());
			OtherTools.Notify("SOMETHING EMERGES", "DEEP BELOW.", "Planetside/Resources/ShrineIcons/prisonShrineIcon");
			yield break;
        }
	}
}



