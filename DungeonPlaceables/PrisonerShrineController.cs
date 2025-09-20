using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Dungeonator;
using UnityEngine;
using System.Reflection;

namespace Planetside
{
	public class PrisonerShrineController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
	{
		public PrisonerShrineController()
		{
			this.playerVFXOffset = Vector3.zero;
		}

		public void ConfigureOnPlacement(RoomHandler room)
		{
			tk2dSprite sprite = null;
			foreach (Component item in base.GetComponentsInChildren(typeof(Component)))
			{
				if (item is tk2dSprite && item.name == "ShrineBase")
				{
					sprite = item as tk2dSprite;
				}
			}

			this.m_parentRoom = room;
			this.m_parentRoom.PreventStandardRoomReward = true;
			this.RegisterMinimapIcon();
		}

		private void Update()
		{


		}

		public void RegisterMinimapIcon()
		{
			this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
		}

		public void GetRidOfMinimapIcon()
		{
			if (this.m_instanceMinimapIcon != null)
			{
				Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
				this.m_instanceMinimapIcon = null;
			}
		}
		private void DoShrineEffect(PlayerController player)
		{
			StaticReferenceManager.DestroyAllEnemyProjectiles();
			Minimap.Instance.ToggleMinimap(false, false);
			GameManager.IsBossIntro = true;
			for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
			{
				if (GameManager.Instance.AllPlayers[j])
				{
					GameManager.Instance.AllPlayers[j].SetInputOverride("BossIntro");
				}
			}
			GameManager.Instance.PreventPausing = true;
			GameUIRoot.Instance.HideCoreUI(string.Empty);
			GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
			this.GetRidOfMinimapIcon();
			tk2dSprite sprite = null;
			foreach (Component item in base.GetComponentsInChildren(typeof(Component)))
			{
				if (item is tk2dSprite && item.name == "ShrineBase")
				{
					sprite = item as tk2dSprite;
				}
			}
			if (sprite != null)
			{
				GameManager.Instance.StartCoroutine(DoShrineBreakOpen(player, sprite));
			}
			CameraController m_camera = GameManager.Instance.MainCameraController;
			m_camera.StopTrackingPlayer();
			m_camera.SetManualControl(true, false);
			m_camera.OverridePosition = m_camera.transform.position;
			Minimap.Instance.TemporarilyPreventMinimap = true;

		}

		private IEnumerator DoShrineBreakOpen(PlayerController player, tk2dSprite shrineSprite)
		{
			AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
			AkSoundEngine.PostEvent("Stop_MUS_All", player.gameObject);

			if (PlanetsideModule.PrisonerDebug == false || PlanetsideModule.DebugMode == false)
			{
                yield return new WaitForSeconds(3);
                Vector2 lol = shrineSprite.WorldTopRight + new Vector2(0.25f, 3).RoundToInt();
                float yes = lol.x.RoundToNearest(1);
                for (int j = 0; j < 2; j++)
                {
                    Vector2 pos = BraveUtility.RandomVector2(shrineSprite.WorldBottomLeft + new Vector2(-0.25f, 0), shrineSprite.WorldTopRight + new Vector2(0.25f, 3), new Vector2(0.025f, 0.025f));
                    LootEngine.DoDefaultItemPoof(pos, false, true);
                    AkSoundEngine.PostEvent("Play_RockBreaking", player.gameObject);
                    yield return new WaitForSeconds(2);
                }
                yield return new WaitForSeconds(1);
                AkSoundEngine.PostEvent("Play_OBJ_key_impact_01", player.gameObject);
                Exploder.DoDistortionWave(lol, 1, 0.3f, 15, 0.5f);
                for (int e = 0; e < 15; e++)
                {
                    GameManager.Instance.StartCoroutine(DoLineOfParticles(shrineSprite.WorldCenter + new Vector2(0, 1.125f)));
                    Vector2 pos = BraveUtility.RandomVector2(shrineSprite.WorldBottomLeft + new Vector2(-0.25f, 0), shrineSprite.WorldTopRight + new Vector2(0.25f, 3), new Vector2(0.025f, 0.025f));
                    LootEngine.DoDefaultItemPoof(pos, false, true);
                    AkSoundEngine.PostEvent("Play_RockBreaking", player.gameObject);
                    yield return new WaitForSeconds(0.2f);
                }
            }
			GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
			epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(shrineSprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
			epicwin.transform.position = shrineSprite.WorldCenter.Quantize(0.0625f);
			epicwin.GetComponent<tk2dBaseSprite>().UpdateZDepth();
			AkSoundEngine.PostEvent("Play_RockBreaking", player.gameObject);
			Pixelator.Instance.FadeToColor(0.5f, Color.white, false, 0f);
			yield return new WaitForSeconds(0.5f);
			AkSoundEngine.PostEvent("Play_PrisonerLaugh", player.gameObject);
			MovePlayerToPrisonerRoom(player);
			yield break;
		}
		private void MovePlayerToPrisonerRoom(PlayerController player)
		{
			RoomHandler megalichRoom = null;
			List<RoomHandler> ROOMS = GameManager.Instance.Dungeon.data.rooms;
			foreach (RoomHandler roomHandler in ROOMS)
			{
				if (roomHandler.GetRoomName() != null && roomHandler.GetRoomName().Contains("PrisonerBossRoom"))
				{
					megalichRoom = roomHandler;
				}
			}
			AIActor Prisoner = null;
			foreach (AIActor enemy in megalichRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
			{
				if (enemy.EnemyGuid == "Prisoner_Cloaked") { Prisoner = enemy; }
			}

			Prisoner.gameObject.GetComponent<GenericIntroDoer>().triggerType = GenericIntroDoer.TriggerType.BossTriggerZone;
			Prisoner.visibilityManager.SuppressPlayerEnteredRoom = true;
			Vector2 idealCameraPosition = Prisoner.gameObject.GetComponent<GenericIntroDoer>().BossCenter;
			CameraController camera = GameManager.Instance.MainCameraController;
			camera.SetManualControl(true, false);
			camera.OverridePosition = idealCameraPosition + new Vector2(0f, 0f);
			int numPlayers = GameManager.Instance.AllPlayers.Length;
			for (int k = 0; k < numPlayers; k++)
			{
				GameManager.Instance.AllPlayers[k].SetInputOverride("lich transition");
			}
			Vector2 targetPoint = megalichRoom.area.Center + new Vector2(0f, -12f);
			if (player)
			{
				player.WarpToPoint(targetPoint, false, false);
			}
			if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
			{
				PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
				if (otherPlayer)
				{
					otherPlayer.ReuniteWithOtherPlayer(player, false);
				}
			}
			Prisoner.gameObject.SetActive(true);
			Prisoner.enabled = true;
			Prisoner.HasBeenEngaged = true;
			GameManager.Instance.StartCoroutine(DoPrisonerActivation(Prisoner));

		}
		private IEnumerator DoPrisonerActivation(AIActor prisoner)
		{
			GameManager.Instance.PreventPausing = false;
			int numPlayers = GameManager.Instance.AllPlayers.Length;
			if (GameManager.HasInstance)
			{
				for (int k = 0; k < numPlayers; k++)
				{
					GameManager.Instance.AllPlayers[k].ClearInputOverride("lich transition");
				}
			}
			GenericIntroDoer metalGearIntroDoer = prisoner.GetComponent<GenericIntroDoer>();
			metalGearIntroDoer.enabled = true;
			prisoner.enabled = true;
			prisoner.behaviorSpeculator.enabled = false;
			prisoner.HasBeenEngaged = true;
			prisoner.gameObject.SetActive(true);
			prisoner.visibilityManager.ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT, true);
			prisoner.gameObject.GetComponent<PrisonerPhaseOneIntroController>().SpawnChains();
			prisoner.aiAnimator.PlayUntilCancelled("introidle", false, null, -1f, false);
			Pixelator.Instance.FadeToColor(1f, Color.white, true, 3f);
			yield return new WaitForSeconds(4f);
			if (prisoner && metalGearIntroDoer)
			{
				metalGearIntroDoer.TriggerSequence(GameManager.Instance.BestActivePlayer);
			}
			yield break;
		}
		private IEnumerator DoLineOfParticles(Vector2 shrineCenter)
		{
			float H = UnityEngine.Random.Range(-180, 180);
			Vector2 Point = MathToolbox.GetUnitOnCircle(H, 0) + shrineCenter;
			Vector2 Point2 = MathToolbox.GetUnitOnCircle(H, 6) + shrineCenter;
			int num2 = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(Point, Point2) * 2), 1);
			for (int i = 0; i < num2; i++)
			{
				float t = (float)i / (float)num2;
				Vector3 vector3 = Vector3.Lerp(Point, Point2, t);
				GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>(UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(178) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX, vector3, Quaternion.identity));
				tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
				component.PlaceAtPositionByAnchor(vector3, tk2dBaseSprite.Anchor.MiddleCenter);
				component.HeightOffGround = 35f;
				component.UpdateZDepth();
				tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
				if (component2 != null)
				{
					component2.ignoreTimeScale = true;
					component2.AlwaysIgnoreTimeScale = true;
					component2.AnimateDuringBossIntros = true;
					component2.alwaysUpdateOffscreen = true;
					component2.playAutomatically = true;
				}
				yield return new WaitForSeconds(0.01f);
			}
			yield break;
		}



		public float GetDistanceToPoint(Vector2 point)
		{
			tk2dSprite sprite = null;
			foreach (Component item in base.GetComponentsInChildren(typeof(Component)))
			{
				if (item is tk2dSprite && item.name == "ShrineBase")
				{
					sprite = item as tk2dSprite;
				}
			}
			if (sprite == null)
			{
				return 100f;
			}
			Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.GetComponentInChildren<SpeculativeRigidbody>().UnitBottomLeft, base.GetComponentInChildren<SpeculativeRigidbody>().UnitDimensions);
			return Vector2.Distance(point, v) / 1.5f;
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(this.AlternativeOutlineTarget ?? base.sprite, Color.white);
		}

		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.RemoveOutlineFromSprite(this.AlternativeOutlineTarget ?? base.sprite, false);
		}

		private IEnumerator HandleShrineConversation(PlayerController interactor)
		{
			TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetString(this.displayTextKey), true, false);
			int selectedResponse = -1;
			interactor.SetInputOverride("shrineConversation");
			yield return null;
			GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(this.acceptOptionKey), StringTableManager.GetString(this.declineOptionKey));
			while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
			{
				yield return null;
			}
			interactor.ClearInputOverride("shrineConversation");
			TextBoxManager.ClearTextBox(this.talkPoint);
			if (selectedResponse == 0)
			{
				this.DoShrineEffect(interactor);
			}
			else
			{
				this.m_useCount--;
				this.m_parentRoom.RegisterInteractable(this);
			}
			yield break;
		}

		public void Interact(PlayerController interactor)
		{
			if (this.m_useCount > 0)
			{
				return;
			}
			this.m_useCount++;
			this.m_parentRoom.DeregisterInteractable(this);
			base.StartCoroutine(this.HandleShrineConversation(interactor));
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
		}
		public string displayTextKey;
		public string acceptOptionKey;
		public string declineOptionKey;
		public Transform talkPoint;
		public GameObject onPlayerVFX;
		public Vector3 playerVFXOffset;
		public tk2dBaseSprite AlternativeOutlineTarget;
		private int m_useCount;
		private RoomHandler m_parentRoom;
		private GameObject m_instanceMinimapIcon;

	}

}
