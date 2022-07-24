using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using Brave.BulletScript;
using System.Collections;

namespace Planetside
{

    
    public class AbyssFloorDoor
	{

		public static tk2dSpriteAnimationClip AddAnimation(tk2dSpriteAnimator animator, tk2dSpriteCollectionData collection, List<int> spriteIDs,
			string clipName, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop, float fps = 15)
		{
			if (animator.Library == null)
			{
				animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
				animator.Library.clips = new tk2dSpriteAnimationClip[0];
				animator.Library.enabled = true;

			}
			List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
			for (int i = 0; i < spriteIDs.Count; i++)
			{
				tk2dSpriteDefinition sprite = collection.spriteDefinitions[spriteIDs[i]];
				if (sprite.Valid)
				{
					frames.Add(new tk2dSpriteAnimationFrame()
					{
						spriteCollection = collection,
						spriteId = spriteIDs[i]
					});
				}
			}
			var clip = new tk2dSpriteAnimationClip()
			{
				name = clipName,
				fps = fps,
				wrapMode = wrapMode,
			};
			Array.Resize(ref animator.Library.clips, animator.Library.clips.Length + 1);
			animator.Library.clips[animator.Library.clips.Length - 1] = clip;

			clip.frames = frames.ToArray();
			return clip;
		}

		public static void Init()
        {
			string superBasePath = "Planetside/Resources/DungeonObjects/DeepDoor";
			string BlockerPath1 = superBasePath + "/Blocker/Horizontal/";
			string[] blockerHDownPaths = new string[] 
			{
			BlockerPath1+"deep_door_blocker_horizontal_down_001",
			BlockerPath1+"deep_door_blocker_horizontal_down_002",
			BlockerPath1+"deep_door_blocker_horizontal_down_003",
			BlockerPath1+"deep_door_blocker_horizontal_down_004",
			BlockerPath1+"deep_door_blocker_horizontal_down_005",
			BlockerPath1+"deep_door_blocker_horizontal_down_006"
			};
			string[] blockerHUpPaths = new string[]
			{
			BlockerPath1+"deep_door_blocker_horizontal_up_001",
			BlockerPath1+"deep_door_blocker_horizontal_up_002",
			BlockerPath1+"deep_door_blocker_horizontal_up_003",
			BlockerPath1+"deep_door_blocker_horizontal_up_004",
			BlockerPath1+"deep_door_blocker_horizontal_up_005",
			BlockerPath1+"deep_door_blocker_horizontal_up_006",
			};

			string BlockerPath2 = superBasePath + "/Blocker/Vertical/";
			string[] blockerVDownPaths = new string[] 
			{
			BlockerPath2+"deep_door_block_vertical_down_001",
			BlockerPath2+"deep_door_block_vertical_down_002",
			BlockerPath2+"deep_door_block_vertical_down_003",
			BlockerPath2+"deep_door_block_vertical_down_004",
			BlockerPath2+"deep_door_block_vertical_down_005",
			BlockerPath2+"deep_door_block_vertical_down_006",
			BlockerPath2+"deep_door_block_vertical_down_007",
			BlockerPath2+"deep_door_block_vertical_down_008",
			};
			string[] blockerVUpPaths = new string[]
			{
			BlockerPath2+"deep_door_block_vertical_up_001",
			BlockerPath2+"deep_door_block_vertical_up_002",
			BlockerPath2+"deep_door_block_vertical_up_003",
			BlockerPath2+"deep_door_block_vertical_up_004",
			BlockerPath2+"deep_door_block_vertical_up_005",
			BlockerPath2+"deep_door_block_vertical_up_006",
			BlockerPath2+"deep_door_block_vertical_up_007",
			BlockerPath2+"deep_door_block_vertical_up_008",
			};
			string BlockerPath3 = superBasePath + "/Blocker/Shake/";
			string[] blockerVNoPaths = new string[] 
			{
				BlockerPath3+"deep_door_block_vertical_refuse_001",
				BlockerPath3+"deep_door_block_vertical_refuse_002",
				BlockerPath3+"deep_door_block_vertical_refuse_003",
				BlockerPath3+"deep_door_block_vertical_refuse_004",
				BlockerPath3+"deep_door_block_vertical_refuse_005",
				BlockerPath3+"deep_door_block_vertical_refuse_006",
				BlockerPath3+"deep_door_block_vertical_refuse_007",
				BlockerPath3+"deep_door_block_vertical_refuse_008",
				BlockerPath3+"deep_door_block_vertical_refuse_009",

			};

			string DoorPath1 = superBasePath + "/Door/Horizontal/Top/";
			string[] doorHTopPaths = new string[] 
			{ 
				DoorPath1+"deep_door_horizontal_top_001",
				DoorPath1+"deep_door_horizontal_top_002",
				DoorPath1+"deep_door_horizontal_top_003",
				DoorPath1+"deep_door_horizontal_top_004",
				DoorPath1+"deep_door_horizontal_top_005",
				DoorPath1+"deep_door_horizontal_top_006",
			};
			string DoorPath2 = superBasePath + "/Door/Horizontal/Bottom/";
			string[] doorHBottomPaths = new string[] 
			{
				DoorPath2+"deep_door_horizontal_bottom_001",
				DoorPath2+"deep_door_horizontal_bottom_002",
				DoorPath2+"deep_door_horizontal_bottom_003",
				DoorPath2+"deep_door_horizontal_bottom_004",
				DoorPath2+"deep_door_horizontal_bottom_005",
				DoorPath2+"deep_door_horizontal_bottom_006",
			};
			string DoorPath3 = superBasePath + "/Door/Vertical/";
			string[] doorVRightPaths = new string[] 
			{ 
				DoorPath3+"deep_door_south_right_001",
				DoorPath3+"deep_door_south_right_002",
				DoorPath3+"deep_door_south_right_003",
				DoorPath3+"deep_door_south_right_004",
				DoorPath3+"deep_door_south_right_005",
				DoorPath3+"deep_door_south_right_006",
			};


			string[] doorVLeftPaths = new string[]
						{
				DoorPath3+"deep_door_south_left_001",
				DoorPath3+"deep_door_south_left_002",
				DoorPath3+"deep_door_south_left_003",
				DoorPath3+"deep_door_south_left_004",
				DoorPath3+"deep_door_south_left_005",
				DoorPath3+"deep_door_south_left_006",
						};


			var orLoadByName = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

			var doorH = FakePrefab.Clone(orLoadByName.doorObjects.variantTiers[0].nonDatabasePlaceable);
			var doorV = FakePrefab.Clone(orLoadByName.doorObjects.variantTiers[1].nonDatabasePlaceable);
			orLoadByName = null;

			var doorCollection = doorH.transform.Find("BarsLeft").GetComponent<tk2dSprite>().Collection;
			var doorLibary = doorH.transform.Find("BarsLeft").GetComponent<tk2dSpriteAnimator>().Library;
			var doorAnimator= doorH.transform.Find("BarsLeft").GetComponent<tk2dSpriteAnimator>();

			doorH.SetActive(false);

			doorH.GetComponent<DungeonDoorController>().doorModules[0].openAnimationName = "beyond_door_horizontal_top_open";
			doorH.GetComponent<DungeonDoorController>().doorModules[0].closeAnimationName = "beyond_door_horizontal_top_close";

			doorH.GetComponent<DungeonDoorController>().doorModules[1].openAnimationName = "beyond_door_horizontal_bottom_open";
			doorH.GetComponent<DungeonDoorController>().doorModules[1].closeAnimationName = "beyond_door_horizontal_bottom_close";


			doorH.GetComponent<DungeonDoorController>().sealAnimationName = "door_block_beyond_side_close";
			doorH.GetComponent<DungeonDoorController>().unsealAnimationName = "door_block_beyond_side_open";

			doorV.GetComponent<DungeonDoorController>().doorModules[0].openAnimationName = "beyond_door_vertical_north_left_open";
			doorV.GetComponent<DungeonDoorController>().doorModules[0].closeAnimationName = "beyond_door_vertical_north_left_close";

			doorV.GetComponent<DungeonDoorController>().doorModules[1].openAnimationName = "beyond_door_vertical_north_right_open";
			doorV.GetComponent<DungeonDoorController>().doorModules[1].closeAnimationName = "beyond_door_vertical_north_right_close";


			doorV.GetComponent<DungeonDoorController>().sealAnimationName = "door_block_beyond_close";
			doorV.GetComponent<DungeonDoorController>().unsealAnimationName = "door_block_beyond_open";
			doorV.GetComponent<DungeonDoorController>().playerNearSealedAnimationName = "door_block_beyond_close_headshake";

			var ids = new List<int>();


			Material material2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);


			material2.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
			material2.SetFloat("_EmissiveColorPower", 1.55f);
			material2.SetFloat("_EmissivePower", 70);

			foreach (var path in blockerVDownPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;
			}

			material2.SetTexture("_MainTex", doorCollection.spriteDefinitions[ids[0]].material.GetTexture("_MainTex"));
			foreach (var sprite in doorV.GetComponentsInChildren<tk2dSprite>().Where(sprite => !sprite.gameObject.name.Contains("AO", false)))
			{
				sprite.renderer.material = material2;
				sprite.usesOverrideMaterial = true;
			}

			foreach (var sprite in doorH.GetComponentsInChildren<tk2dSprite>().Where(sprite => !sprite.gameObject.name.Contains("AO", false)))
			{
				sprite.renderer.material = material2;
				sprite.usesOverrideMaterial = true;
			}

			foreach (var animator in doorH.GetComponent<DungeonDoorController>().sealAnimators)
			{
				animator.renderer.material = material2;
				animator.sprite.usesOverrideMaterial = true;
			}

			foreach (var animator in doorV.GetComponent<DungeonDoorController>().sealAnimators)
			{
				animator.renderer.material = material2;
				animator.sprite.usesOverrideMaterial = true;
			}

			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_close", tk2dSpriteAnimationClip.WrapMode.Once, 14);
			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_open", tk2dSpriteAnimationClip.WrapMode.Once, 14);
			ids.Clear();



			foreach (var path in blockerHDownPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;
			}

			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_side_close", tk2dSpriteAnimationClip.WrapMode.Once, 14);
			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_side_open", tk2dSpriteAnimationClip.WrapMode.Once, 14);
			ids.Clear();

			foreach (var path in blockerVNoPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;
			}


			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_close_headshake", tk2dSpriteAnimationClip.WrapMode.Once, 14);
			ids.Clear();

			foreach (var path in doorVRightPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;
			}
			doorCollection.spriteDefinitions[ids[0]].colliderVertices = new Vector3[] { new Vector3(1.5f, 1.03125f, 0), new Vector3(0.5f, 0.34375f, 1) };
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_north_right_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_south_right_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);

			foreach (var id in ids)
			{
				var baseID = doorLibary.GetClipByName("iron_wood_door_vertical_north_right_open").frames[0].spriteId;
				doorCollection.spriteDefinitions[id].position0 = doorCollection.spriteDefinitions[baseID].position0;
				doorCollection.spriteDefinitions[id].position1 = doorCollection.spriteDefinitions[baseID].position1;
				doorCollection.spriteDefinitions[id].position2 = doorCollection.spriteDefinitions[baseID].position2;
				doorCollection.spriteDefinitions[id].position3 = doorCollection.spriteDefinitions[baseID].position3;
			}

			doorV.transform.Find("DoorRight").GetComponent<tk2dSprite>().spriteId = ids[0];
			doorV.transform.Find("DoorRight").GetComponent<tk2dSpriteAnimator>().DefaultClipId = doorLibary.GetClipIdByName("beyond_door_vertical_north_right_open");

			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_north_right_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_south_right_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			ids.Clear();


			foreach (var path in doorVLeftPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;

			}
			doorCollection.spriteDefinitions[ids[0]].colliderVertices = new Vector3[] { new Vector3(0.46875f, 1.03125f, 0), new Vector3(0.46875f, 0.34375f, 1) };
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_north_left_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_south_left_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);

			foreach (var id in ids)
			{
				var baseID = doorLibary.GetClipByName("iron_wood_door_vertical_north_left_open").frames[0].spriteId;
				doorCollection.spriteDefinitions[id].position0 = doorCollection.spriteDefinitions[baseID].position0;
				doorCollection.spriteDefinitions[id].position1 = doorCollection.spriteDefinitions[baseID].position1;
				doorCollection.spriteDefinitions[id].position2 = doorCollection.spriteDefinitions[baseID].position2;
				doorCollection.spriteDefinitions[id].position3 = doorCollection.spriteDefinitions[baseID].position3;
			}

			doorV.transform.Find("DoorLeft").GetComponent<tk2dSprite>().spriteId = ids[0];
			doorV.transform.Find("DoorLeft").GetComponent<tk2dSpriteAnimator>().DefaultClipId = doorLibary.GetClipIdByName("beyond_door_vertical_north_left_open");

			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_north_left_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_vertical_south_left_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			ids.Clear();


			foreach (var path in doorHBottomPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;

			}
			doorCollection.spriteDefinitions[ids[0]].colliderVertices = new Vector3[] { new Vector3(0, 2.5f, 0), new Vector3(0.1875f, 1.625f, 1) };
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_horizontal_bottom_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);

			foreach (var id in ids)
			{
				var baseID = doorLibary.GetClipByName("iron_wood_door_horizontal_bottom_open").frames[0].spriteId;
				doorCollection.spriteDefinitions[id].position0 = doorCollection.spriteDefinitions[baseID].position0;
				doorCollection.spriteDefinitions[id].position1 = doorCollection.spriteDefinitions[baseID].position1;
				doorCollection.spriteDefinitions[id].position2 = doorCollection.spriteDefinitions[baseID].position2;
				doorCollection.spriteDefinitions[id].position3 = doorCollection.spriteDefinitions[baseID].position3;
			}

			doorH.transform.Find("DoorBottom").GetComponent<tk2dSprite>().spriteId = ids[0];
			doorH.transform.Find("DoorBottom").GetComponent<tk2dSpriteAnimator>().DefaultClipId = doorLibary.GetClipIdByName("beyond_door_horizontal_bottom_open");

			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_horizontal_bottom_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			ids.Clear();


			foreach (var path in doorHTopPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 5;
			}
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_horizontal_top_open", tk2dSpriteAnimationClip.WrapMode.Once, 15);

			foreach (var id in ids)
			{
				var baseID = doorLibary.GetClipByName("iron_wood_door_horizontal_top_open").frames[0].spriteId;
				doorCollection.spriteDefinitions[id].position0 = doorCollection.spriteDefinitions[baseID].position0;
				doorCollection.spriteDefinitions[id].position1 = doorCollection.spriteDefinitions[baseID].position1;
				doorCollection.spriteDefinitions[id].position2 = doorCollection.spriteDefinitions[baseID].position2;
				doorCollection.spriteDefinitions[id].position3 = doorCollection.spriteDefinitions[baseID].position3;
			}

			doorH.transform.Find("DoorTop").GetComponent<tk2dSprite>().spriteId = ids[0];
			doorH.transform.Find("DoorTop").GetComponent<tk2dSpriteAnimator>().DefaultClipId = doorLibary.GetClipIdByName("beyond_door_horizontal_top_open");

			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "beyond_door_horizontal_top_close", tk2dSpriteAnimationClip.WrapMode.Once, 15);
			ids.Clear();

			foreach (var path in blockerHDownPaths)
			{
				var id = SpriteBuilder.AddSpriteToCollection($"{path}.png", doorCollection);
				ids.Add(id);
				doorCollection.spriteDefinitions[id].materialId = 3;
			}
			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_side_close", tk2dSpriteAnimationClip.WrapMode.Once, 11);

			doorH.transform.Find("BarsLeft").GetComponent<tk2dSprite>().spriteId = ids[0];
			doorH.transform.Find("BarsLeft").GetComponent<tk2dSpriteAnimator>().DefaultClipId = doorLibary.GetClipIdByName("door_block_beyond_side_close");

			ids.Reverse();
			AddAnimation(doorAnimator, doorCollection, ids, "door_block_beyond_side_open", tk2dSpriteAnimationClip.WrapMode.Once, 11);
			ids.Clear();

			AbyssDoor = ScriptableObject.CreateInstance<DungeonPlaceable>();
			AbyssDoor.name = "AbyssDoor";
			AbyssDoor.width = 3;
			AbyssDoor.height = 1;
			AbyssDoor.roomSequential = false;
			AbyssDoor.respectsEncounterableDifferentiator = false;
			AbyssDoor.UsePrefabTransformOffset = false;
			AbyssDoor.MarkSpawnedItemsAsRatIgnored = false;
			AbyssDoor.IsAnnexTable = false;
			AbyssDoor.variantTiers = new List<DungeonPlaceableVariant>
			{
				new DungeonPlaceableVariant
				{
					percentChance = 1,
					unitOffset = Vector2.zero,
					nonDatabasePlaceable = doorH,
					enemyPlaceableGuid = "",
					pickupObjectPlaceableId = -1,
					forceBlackPhantom = false,
					addDebrisObject = false,
					prerequisites = new DungeonPrerequisite[0],
					materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
				},
				new DungeonPlaceableVariant
				{
					percentChance = 1,
					unitOffset = Vector2.zero,
					nonDatabasePlaceable = doorV,
					enemyPlaceableGuid = "",
					pickupObjectPlaceableId = -1,
					forceBlackPhantom = false,
					addDebrisObject = false,
					prerequisites = new DungeonPrerequisite[0],
					materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
				}
			};
		}
		public static DungeonPlaceable AbyssDoor;
    }
}


