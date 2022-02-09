
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.OldShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;

namespace Planetside
{

	public static class TooLate
	{

		public static void Add()
		{
			OldShrineFactory iei = new OldShrineFactory
			{
				name = "TooLate",
				modID = "psog",
				text = "The note says \"Shop is closed, come back earlier!\"",
				spritePath = "Planetside/Resources/Shrines/toolatesign.png",
				acceptText = "Kick it!",
				declineText = "Leave.",
				OnAccept = Accept,
				OnDecline = null,

				CanUse = CanUse,
				offset = new Vector3(0, 0, 0),
				talkPointOffset = new Vector3(0, 3, 0),
				isToggle = false,
				isBreachShrine = false,

			};
			iei.Build();
		}
		public static bool CanUse(PlayerController player, GameObject shrine)
		{
			return shrine.GetComponent<CustomShrineController>().numUses == 0;
		}

		public static string GetAnimationState(PlayerController interactor, SpeculativeRigidbody otherBody, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			switch (GetFlipDirection(interactor.specRigidbody, otherBody))
			{
				case DungeonData.Direction.NORTH:
					return "tablekick_up";
				case DungeonData.Direction.EAST:
					return "tablekick_right";
				case DungeonData.Direction.SOUTH:
					return "tablekick_down";
				case DungeonData.Direction.WEST:
					shouldBeFlipped = true;
					return "tablekick_right";
			}
			return "error";
		}

		public static DungeonData.Direction GetFlipDirection(SpeculativeRigidbody flipperRigidbody, SpeculativeRigidbody otherRigidBody)
		{
			bool flag = flipperRigidbody.UnitRight <= otherRigidBody.specRigidbody.UnitLeft;
			bool flag2 = flipperRigidbody.UnitLeft >= otherRigidBody.specRigidbody.UnitRight;
			bool flag3 = flipperRigidbody.UnitBottom >= otherRigidBody.specRigidbody.UnitTop;
			bool flag4 = flipperRigidbody.UnitTop <= otherRigidBody.specRigidbody.UnitBottom;
			if (flag && !flag3 && !flag4)
			{
				return DungeonData.Direction.EAST;
			}
			if (flag2 && !flag3 && !flag4)
			{
				return DungeonData.Direction.WEST;
			}
			if (flag3 && !flag && !flag2)
			{
				return DungeonData.Direction.SOUTH;
			}
			if (flag4 && !flag && !flag2)
			{
				return DungeonData.Direction.NORTH;
			}
			Vector2 a = Vector2.zero;
			Vector2 b = Vector2.zero;
			PlayerController component = flipperRigidbody.GetComponent<PlayerController>();
			bool flag5 = component && component.IsSlidingOverSurface;
			if (flag && flag3)
			{
				a = flipperRigidbody.UnitBottomRight;
				b = otherRigidBody.specRigidbody.UnitTopLeft;
			}
			else if (flag2 && flag3)
			{
				a = flipperRigidbody.UnitBottomLeft;
				b = otherRigidBody.specRigidbody.UnitTopRight;
			}
			else if (flag && flag4)
			{
				a = flipperRigidbody.UnitTopRight;
				b = otherRigidBody.specRigidbody.UnitBottomLeft;
			}
			else if (flag2 && flag4)
			{
				a = flipperRigidbody.UnitTopLeft;
				b = otherRigidBody.specRigidbody.UnitBottomRight;
			}
			else
			{
				Debug.LogError("Something about this table and flipper is TOTALLY WRONG MAN (way #1)");
			}
			Vector2 vector = a - b;
			if (vector == Vector2.zero)
			{
				if (flag4)
				{
					return DungeonData.Direction.NORTH;
				}
				if (flag3)
				{
					return DungeonData.Direction.SOUTH;
				}
			}
			
			Vector2 majorAxis = BraveUtility.GetMajorAxis(vector);
			if (majorAxis.x < 0f)
			{
				return DungeonData.Direction.EAST;
			}
			if (majorAxis.x > 0f)
			{
				return DungeonData.Direction.WEST;
			}
			if (majorAxis.y < 0f)
			{
				return DungeonData.Direction.NORTH;
			}
			if (majorAxis.y > 0f)
			{
				return DungeonData.Direction.SOUTH;
			}
			Debug.LogError("Something about this table and flipper is TOTALLY WRONG MAN (way #2)");
			return DungeonData.Direction.NORTH;
		}

		public static void Accept(PlayerController player, GameObject shrine)
		{
			AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", shrine.gameObject);
			UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(178) as Gun).GetComponent<FireOnReloadSynergyProcessor>().DirectedBurstSettings.ProjectileInterface.SpecifiedProjectile.hitEffects.tileMapHorizontal.effects[0].effects[0].effect, shrine.transform.position, Quaternion.identity);
			shrine.GetComponent<CustomShrineController>().numUses++;
			float RNG = UnityEngine.Random.Range(1, 9);
			float RNG2 = UnityEngine.Random.Range(-180, 180);

			bool flag6;
			string text = GetAnimationState(player, shrine.GetComponent<SpeculativeRigidbody>(), out flag6);
			if (player.IsSlidingOverSurface)
			{
				text = string.Empty;
			}
			if (text != string.Empty)
			{
				FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
				leEnabler.SetValue(player, true);
				string str = (!(player.CurrentGun == null) || player.ForceHandless) ? "_hand" : "_twohands";
				string text2 = (!player.UseArmorlessAnim) ? string.Empty : "_armorless";
				bool renderBodyhand = !player.ForceHandless && player.CurrentSecondaryGun == null && (player.CurrentGun == null || player.CurrentGun.Handedness != GunHandedness.TwoHanded);
				if (renderBodyhand && player.spriteAnimator.GetClipByName(text + str + text2) != null)
				{
					player.spriteAnimator.Play(text + str + text2);
				}
				else if (player.spriteAnimator.GetClipByName(text + text2) != null)
				{
					player.spriteAnimator.Play(text + text2);
				}
			}
			for (int i = 0; i < RNG; i++)
            {
				Vector2 Point = MathToolbox.GetUnitOnCircle(((360/RNG)*i)+RNG2, UnityEngine.Random.Range(0.5f, 3));
				LootEngine.SpawnItem(PickupObjectDatabase.GetById(68).gameObject, shrine.transform.PositionVector2(), Point, UnityEngine.Random.Range(0.5f, 3), false, false, false);
			}
			UnityEngine.GameObject.Destroy(shrine);
		}
	}
}



