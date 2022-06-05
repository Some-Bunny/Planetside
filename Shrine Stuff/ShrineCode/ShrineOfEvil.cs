
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Gungeon;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;


namespace Planetside
{
	public static class ShrineOfEvil
	{

		public static void Add()
		{
			ShrineFactory aa = new ShrineFactory
			{

				name = "ShrineOfEvil",
				modID = "psog",
				text = "A shrine dedicated to the Gungeons Master. Do you dare...?",
				spritePath = "Planetside/Resources/Shrines/ShrineOfEvil.png",
				//room = RoomFactory.BuildFromResource("Planetside/Resources/ShrineRooms/ShrineOfEvilShrineRoomHell.room").room,
				//RoomWeight = 200f,
				acceptText = "Dare.",
				declineText = "Leave",
				OnAccept = Accept,
				OnDecline = null,
				CanUse = CanUse,
				offset = new Vector3(-1, -1, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,


			};
			aa.Build();


		}
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			if (!player.IsInCombat)
			{
				return shrine.GetComponent<CustomShrineController>().numUses <= 5;
			}
			else
			{
				return false;
			}
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", shrine);
			shrine.GetComponent<CustomShrineController>().numUses++;
			if (shrine.GetComponent<CustomShrineController>().numUses == 0)
			{

				ETGModConsole.Log("0");
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 1)
			{
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave1));
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 10f;
				yee.HPMultiplier = 4f;
				yee.DropsPickups = true;
				yee.PickupAmount = UnityEngine.Random.Range(2, 4);
				yee.CooldownMulitplier = 0.8f;
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 13; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave1));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.75f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(1.1f, 2f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.25f, 1.5f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = (UnityEngine.Random.value > 0.75f) ? false : true;

				}
				ETGModConsole.Log("1");
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 2)
			{
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave2));
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 7f;
				yee.HPMultiplier = 3f;
				yee.DropsPickups = true;
				yee.CooldownMulitplier = 0.6f;
				yee.GainsSkulls = (UnityEngine.Random.value > 0.2f) ? false : true;
				yee.PickupAmount = UnityEngine.Random.Range(3, 6);
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 12; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>((UnityEngine.Random.value > 0.5f) ? ShrineOfEvil.GuidsWave1 : ShrineOfEvil.GuidsWave2));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.5f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(0.9f, 1.8f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.35f, 1.25f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = (UnityEngine.Random.value > 0.5f) ? false : true;
					//h.CanDash = (UnityEngine.Random.value > 0.25f) ? false : true;
				}
				ETGModConsole.Log("2");
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 3)
			{
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave3));
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 8.25f;
				yee.HPMultiplier = 3.25f;
				yee.DropsPickups = true;
				yee.CooldownMulitplier = 0.5f;
				yee.GainsSkulls = (UnityEngine.Random.value > 0.4f) ? false : true;
				yee.PickupAmount = UnityEngine.Random.Range(2, 4);
				yee.DropsChests = (UnityEngine.Random.value > 0.75f) ? false : true;
				yee.ChestAmount = 1;
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 16; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>((UnityEngine.Random.value > 0.5f) ? ShrineOfEvil.GuidsWave2 : ShrineOfEvil.GuidsWave3));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.5f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(0.7f, 1.4f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.4f, 1.25f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = (UnityEngine.Random.value > 0.65f) ? false : true;
					//h.CanDash = (UnityEngine.Random.value > 0.45f) ? false : true;
				}
				ETGModConsole.Log("3");
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 4)
			{
				ETGModConsole.Log("4");
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave4));
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 5f;
				yee.HPMultiplier = 2.5f;
				yee.DropsPickups = true;
				yee.CooldownMulitplier = 0.5f;
				yee.GainsSkulls = (UnityEngine.Random.value > 0.85f) ? false : true;
				yee.PickupAmount = UnityEngine.Random.Range(3, 6);
				yee.DropsChests = true;
				yee.ChestAmount = 1;
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 12; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>((UnityEngine.Random.value > 0.6f) ? ShrineOfEvil.GuidsWave3 : ShrineOfEvil.GuidsWave4));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.35f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(0.6f, 1.2f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.25f, 1.05f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = (UnityEngine.Random.value > 0.85f) ? false : true;
					//h.CanDash = (UnityEngine.Random.value > 0.45f) ? false : true;
				}
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 5)
			{
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>(ShrineOfEvil.GuidsWave5));
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 2.5f;
				yee.HPMultiplier = 1.7f;
				yee.DropsPickups = true;
				yee.CooldownMulitplier = 0.7f;
				yee.GainsSkulls = true;
				yee.PickupAmount = UnityEngine.Random.Range(2, 5);
				yee.DropsChests = true;
				yee.ChestAmount = 2;
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 8; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>((UnityEngine.Random.value > 0.75f) ? ShrineOfEvil.GuidsWave3 : ShrineOfEvil.GuidsWave4));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.3f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(0.6f, 1.2f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.25f, 1.05f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = (UnityEngine.Random.value > 0.85f) ? false : true;
					//h.CanDash = (UnityEngine.Random.value > 0.45f) ? false : true;
				}
				ETGModConsole.Log("5");
			}
			if (shrine.GetComponent<CustomShrineController>().numUses == 6)
			{
				AIActor orLoadByGuid1 = EnemyDatabase.GetOrLoadByGuid("Bullet_Banker");
				IntVector2? intVector1 = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
				AIActor aiactor1 = AIActor.Spawn(orLoadByGuid1.aiActor, intVector1.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector1.Value), true, AIActor.AwakenAnimationType.Awaken, true);
				aiactor1.CanTargetEnemies = false;
				aiactor1.CanTargetPlayers = true;
				PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor1.specRigidbody, null, false);
				aiactor1.IgnoreForRoomClear = false;
				aiactor1.IsHarmlessEnemy = false;
				UmbraMinionsbehavior yee = aiactor1.gameObject.AddComponent<UmbraMinionsbehavior>();
				yee.TimeUntilInvulnerabilityGone = 1f;
				yee.HPMultiplier = 0.8f;
				yee.DropsPickups = true;
				yee.CooldownMulitplier = 0.8f;
				yee.GainsSkulls = true;
				yee.PickupAmount = UnityEngine.Random.Range(3, 7);
				yee.DropsChests = true;
				yee.ChestAmount = 3;
				aiactor1.HandleReinforcementFallIntoRoom(0f);
				for (int i = 0; i < 1; i++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(BraveUtility.RandomElement<string>((UnityEngine.Random.value > 0.75f) ? ShrineOfEvil.GuidsWave3 : ShrineOfEvil.GuidsWave4));
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom((i * 0.25f));
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(1.2f, 1.6f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.25f, 0.5f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = true;
					//h.CanDash = (UnityEngine.Random.value > 0.45f) ? false : true;
				}
				for (int a = 0; a < 1; a++)
				{
					AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("Bullet_Banker");
					IntVector2? intVector = new IntVector2?(player.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
					AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Awaken, true);
					aiactor.CanTargetEnemies = false;
					aiactor.CanTargetPlayers = true;
					PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
					aiactor.IgnoreForRoomClear = true;
					aiactor.IsHarmlessEnemy = false;
					aiactor.HandleReinforcementFallIntoRoom(0);
					aiactor.behaviorSpeculator.CooldownScale *= UnityEngine.Random.Range(1.4f, 2f);
					aiactor.MovementSpeed *= UnityEngine.Random.Range(0.25f, 0.66f);
					aiactor.healthHaver.PreventAllDamage = true;
					UmbraController h = aiactor.gameObject.AddComponent<UmbraController>();
					h.CanTeleport = true;
					//h.CanDash = (UnityEngine.Random.value > 0.45f) ? false : true;
				}
				ETGModConsole.Log("6");
			}
		}


		public static List<string> GuidsWave1 = new List<string>()
		{
			"01972dee89fc4404a5c408d50007dad5",
			"db35531e66ce41cbb81d507a34366dfe",
			"88b6b6a93d4b4234a67844ef4728382c",
			"70216cae6c1346309d86d4a0b4603045",
			"8bb5578fba374e8aae8e10b754e61d62",
			"e5cffcfabfae489da61062ea20539887",
			"1a78cfb776f54641b832e92c44021cf2",
			"d4a9836f8ab14f3fadd0f597438b1f1f",
			"5f3abc2d561b4b9c9e72b879c6f10c7e",
			"05cb719e0178478685dc610f8b3e8bfc",
			"5861e5a077244905a8c25c2b7b4d6ebb",
			"6f818f482a5c47fd8f38cce101f6566c",
			"4db03291a12144d69fe940d5a01de376",
			"336190e29e8a4f75ab7486595b700d4a",
			"95ec774b5a75467a9ab05fa230c0c143",
			"906d71ccc1934c02a6f4ff2e9c07c9ec",
			"9eba44a0ea6c4ea386ff02286dd0e6bd"
		};
		public static List<string> GuidsWave2 = new List<string>()
		{
			"c0ff3744760c4a2eb0bb52ac162056e6",
			"6f22935656c54ccfb89fca30ad663a64",//Books
			"a400523e535f41ac80a43ff6b06dc0bf",

			"206405acad4d4c33aac6717d184dc8d4",//Apprectince Wizard

			"128db2f0781141bcb505d8f00f9e4d47",
			"b54d89f9e802455cbb2b8a96a31e8259",//Red/Blue Shotgunners

			"31a3ea0c54a745e182e22ea54844a82d",//Sniper Shell

			"6e972cd3b11e4b429b888b488e308551",//Gunzookie

			"7b0b1b6d9ce7405b86b75ce648025dd6",//Beadie

			"9d50684ce2c044e880878e86dbada919",//Coaler
			"f905765488874846b7ff257ff81d6d0c",//Fungun
			"coallet_psog",
			"glockulus",
			"wailer"
		};

		public static List<string> GuidsWave3 = new List<string>()
		{
			"2752019b770f473193b08b4005dc781f",
			"1bd8e49f93614e76b140077ff2e33f2b",//more Shutgun kins
			"7f665bd7151347e298e4d366f8818284",

			"1a4872dafdb34fd29fe8ac90bd2cea67",//king bullat

			"ffdc8680bdaa487f8f31995539f74265",//Muzzle Wisps
			"d8a445ea4d944cc1b55a40f22821ae69",

			"56fb939a434140308b8f257f0f447829",
			"9b2cf2949a894599917d4d391a0b7394", //Gunjurers
			"c4fba8def15e47b297865b18e36cbef8",

			"57255ed50ee24794b7aac1ac3cfb8a95", //Gun Cultist
			"skullvenant"

		};

		public static List<string> GuidsWave4 = new List<string>()
		{
			"ec8ea75b557d4e7b8ceeaacdf6f8238c",//Gun_Nut
			"463d16121f884984abe759de38418e48",//ChainGunner
			"cd4a4b7f612a4ba9a720b9f97c52f38c",//Leadmaiden
			"eed5addcc15148179f300cc0d9ee7f94",//Spogre
			"arch_gunjurer",
			"Barretina",
			"3e98ccecf7334ff2800188c417e67c15",
			"98ca70157c364750a60f5e0084f9d3e2",
			"98ea2fe181ab4323ab6e9981955a9bca",
			"d5a7b95774cd41f080e517bea07bf495"
		};
		public static List<string> GuidsWave5 = new List<string>()
		{
			"edc61b105ddd4ce18302b82efdc47178"
		};
	}
}



