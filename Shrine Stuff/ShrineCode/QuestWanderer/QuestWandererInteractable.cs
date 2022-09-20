using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SaveAPI;
using UnityEngine;
using Planetside;

namespace GungeonAPI
{
	// Token: 0x02000158 RID: 344
	public class QuestWandererInteractable : SimpleInteractable, IPlayerInteractable
	{
		private void Start()
		{
			tk2dSprite fatrd = base.gameObject.GetComponent<tk2dSprite>();
			fatrd.usesOverrideMaterial = true;
			Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
			mat.mainTexture = fatrd.renderer.material.mainTexture;
			mat.SetColor("_EmissiveColor", new Color32(255, 0, 120, 255));
			mat.SetFloat("_EmissiveColorPower", 1.55f);
			mat.SetFloat("_EmissivePower", 400);
			fatrd.sprite.renderer.material = mat;
			this.talkPoint = base.transform.Find("talkpoint");
			this.talkPoint.position += new Vector3(0.4f, 2f, 0f);
			this.m_isToggled = false;
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
			base.gameObject.AddAnimation("idle", "Planetside/Resources/Shrines/Wanderer", 4, NPCBuilder.AnimationType.Idle, DirectionalAnimation.DirectionType.None, DirectionalAnimation.FlipType.None);
			base.spriteAnimator.Play("idle");
			this.m_canUse = true;
			bool Q = SaveAPIManager.GetFlag(CustomDungeonFlags.BROKEN_CHAMBER_RUN_COMPLETED);
			bool W = SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_LOOP_1);
			bool E = SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_A_BOSS_UNDER_A_SECOND);
			bool R = SaveAPIManager.GetFlag(CustomDungeonFlags.BULLETBANK_DEFEATED);
			bool T = SaveAPIManager.GetFlag(CustomDungeonFlags.HIGHER_CURSE_DRAGUN_KILLED);
			bool Y = SaveAPIManager.GetFlag(CustomDungeonFlags.SHELLRAX_DEFEATED);
			bool U = SaveAPIManager.GetFlag(CustomDungeonFlags.JAMMED_GUARD_DEFEATED);
			bool I = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_FUNGANNON);
			bool O = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_OPHANAIM);
			bool P = SaveAPIManager.GetFlag(CustomDungeonFlags.DEFEAT_ANNIHICHAMBER);
			bool A = SaveAPIManager.GetFlag(CustomDungeonFlags.DECURSE_HELL_SHRINE_UNLOCK);
			bool S = SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_COMPLETED_SOMETHING_WICKED);
			if (Q == true && W == true && E == true && R == true && T == true && Y == true && U == true && I == true && O == true && P == true && A == true && S == true)
			{
				GameObject synergyobject = PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn;
				BlackHoleDoer component2 = synergyobject.GetComponent<BlackHoleDoer>();

				this.gameObject1 = UnityEngine.Object.Instantiate<GameObject>(component2.HellSynergyVFX, new Vector3(base.transform.position.x+0.75f, base.transform.position.y, base.transform.position.z + 5f), Quaternion.Euler(0f, 0f, 0f));

				MeshRenderer component3 = this.gameObject1.GetComponent<MeshRenderer>();
				base.StartCoroutine(this.HoldPortalOpen(component3, base.gameObject.transform.position, this.gameObject1));
				component3.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);

				if (SaveAPIManager.GetFlag(CustomDungeonFlags.HASDONEEVRYTHING))
				{
					GameObject original;
					original = RandomPiecesOfStuffToInitialise.MedalObject;
					tk2dSprite faack = GameObject.Instantiate(original, base.gameObject.transform.position, Quaternion.identity, base.gameObject.transform).GetComponent<tk2dSprite>();
					faack.transform.position.WithZ(faack.transform.position.z + 99999);
					faack.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.gameObject.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
					tk2dBaseSprite dsakdslad = base.gameObject.GetComponent<tk2dBaseSprite>();
					dsakdslad.sprite.AttachRenderer(faack.GetComponent<tk2dBaseSprite>());
					faack.name = "medal";
					faack.PlaceAtPositionByAnchor(base.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
					faack.scale = Vector3.one;

					mat.mainTexture = faack.renderer.material.mainTexture;
					mat.SetColor("_EmissiveColor", new Color32(239, 222, 252, 255));
					mat.SetFloat("_EmissiveColorPower", 1.55f);
					mat.SetFloat("_EmissivePower", 100);
					faack.sprite.renderer.material = mat;
					LootEngine.DoDefaultSynergyPoof(faack.transform.position, false);

				}
			}
			else
            {
				Destroy(this.gameObject);
			}
		}

		private IEnumerator HoldPortalOpen(MeshRenderer component, Vector2 vector, GameObject gameObject1)
		{
			float elapsed = 0f;
			while (component != null)
			{
				elapsed += BraveTime.DeltaTime;
				float t = Mathf.Clamp01(elapsed / 0.25f);
				component.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0f, 0.166f, t));
				yield return null;
			}

			yield break;
		}
		public void Interact(PlayerController interactor)
		{
			if (!TextBoxManager.HasTextBox(this.talkPoint))
			{
				this.m_canUse = ((this.CanUse != null) ? this.CanUse(interactor, base.gameObject) : this.m_canUse);
				bool flag5 = !this.m_canUse;
				bool flag6 = flag5;
				bool flag7 = flag6;
				bool flag8 = flag7;
				if (flag8)
				{
					TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "SAoryr mate", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
				}
				else
				{
					base.StartCoroutine(this.HandleConversation(interactor));
				}
			}
		}

		private IEnumerator HandleConversation(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
			interactor.SetInputOverride("npcConversation");
			Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
			yield return null;
			List<string> conversationToUse = this.conversation;
			this.m_allowMeToIntroduceMyself = false;

			bool HasEverTalked = SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_EVER_TALKED);
			if (!HasEverTalked)
			{
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HAS_EVER_TALKED, true);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, "I see you've finished everything there is to get.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, "So...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, "I can give you new things to look out for.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1.5f, "Quests, if you will.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "They will yield no reward, and no reason to do them, other than me saying so.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "Yet, If you wish to ignore them... so be it.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, -1f, "Do you want some guidance..?", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);

				string acceptanceTextToUse = this.acceptText;
				string declineTextToUse = this.declineText;
				GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "Sure.", "I'll be fine.");
			}
			else
			{
				if (SaveAPIManager.GetFlag(CustomDungeonFlags.HASDONEEVRYTHING) == true)
                {
					TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
					GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "<Leave.>", "<Leave.>");

				}
				else
                {
					
					TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 5f, "Want some guidance..?", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
					string acceptanceTextToUse2 = this.acceptText;
					string declineTextToUse2 = this.declineText;
					GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, "Sure.", "I'll be fine.");
				}
			}
			int selectedResponse = -1;
			while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
			{
				yield return null;
			}
			bool flag = selectedResponse == 0;
			this.selfdestrut = false;
			bool flag2 = flag;
			bool flag7 = flag2;
			if (flag7)
			{
				TextBoxManager.ClearTextBox(this.talkPoint);
				Action<PlayerController, GameObject> onAccept = this.OnAccept;
				bool flag3 = onAccept != null;
				bool flag8 = flag3;
				if (flag8)
				{
					onAccept(interactor, base.gameObject);
				}
				onAccept = null;
				onAccept = null;
			}
			else
			{
				Action<PlayerController, GameObject> onDecline = this.OnDecline;
				bool flag4 = onDecline != null;
				bool flag10 = flag4;
				if (flag10)
				{
					onDecline(interactor, base.gameObject);
				}
				TextBoxManager.ClearTextBox(this.talkPoint);
				onDecline = null;
				onDecline = null;
			}
			interactor.ClearInputOverride("npcConversation");
			Pixelator.Instance.LerpToLetterbox(1f, 0.25f);
			yield break;
		}

		public void OnEnteredRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
			base.sprite.UpdateZDepth();
		}

		public void OnExitRange(PlayerController interactor)
		{
			SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
		}

		public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
		{
			shouldBeFlipped = false;
			return string.Empty;
		}

		public float GetDistanceToPoint(Vector2 point)
		{
			bool flag = base.sprite == null;
			bool flag2 = flag;
			bool flag3 = flag2;
			bool flag4 = flag3;
			float result;
			if (flag4)
			{
				result = 100f;
			}
			else
			{
				Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
				result = Vector2.Distance(point, v) / 1.5f;
			}
			return result;
		}

		public float GetOverrideMaxDistance()
		{
			return -1f;
		}

		// Token: 0x040010E5 RID: 4325
		public bool m_allowMeToIntroduceMyself = true;
		public bool selfdestrut;
		private GameObject gameObject1;

		public void Talk(PlayerController interactor)
        {
			base.StartCoroutine(WalkTalk(interactor));
        }

		private IEnumerator WalkTalk(PlayerController interactor)
		{
			bool CheckIfEverythingDone = true;
			TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 1f, "So...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
			yield return new WaitForSeconds(1f);
			bool FIREGTEE = SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_FIRED_GTEE);
			if (!FIREGTEE)
			{
				CheckIfEverythingDone = false;

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "Obtain the weapon to end everything, and fire it...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);
			}
			bool CONFUE = SaveAPIManager.GetFlag(CustomDungeonFlags.CONFUSED_THE_CHAMBER);
			if (!CONFUE)
			{
				CheckIfEverythingDone = false;

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "Confuse a murderous Chamber...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);
			}
			bool HERE = SaveAPIManager.GetFlag(CustomDungeonFlags.HAS_COMMITED_HERESY);
			if (!HERE)
			{
				CheckIfEverythingDone = false;

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "Commit a great heresy against Kaliber...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);
			}
			bool shatter = SaveAPIManager.GetFlag(CustomDungeonFlags.SHATTER_GLASSGUON);
			if (!shatter)
			{
				CheckIfEverythingDone = false;

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "Open a cosmic rift with a Glass Guon Stone...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);
			}
			bool PLATE = SaveAPIManager.GetFlag(CustomDungeonFlags.PLATE_DIAMOND_CHAMBER);
			if (!PLATE)
			{
				CheckIfEverythingDone = false;

				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 3f, "Plate a Chamber Of Diamond...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(3f);
			}
			if (CheckIfEverythingDone == true)
            {
				AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.HASDONEEVRYTHING, true);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "Huh. Wow...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "You really did do...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "Everything.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "I'm impressed.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "...", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "Well... some reward must be due.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				yield return new WaitForSeconds(2f);
				TextBoxManager.ShowTextBox(this.talkPoint.position, this.talkPoint, 2f, "Here... as a symbol of success.", interactor.characterAudioSpeechTag, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
				GameObject original;
				original = RandomPiecesOfStuffToInitialise.MedalObject;
				tk2dSprite faack = GameObject.Instantiate(original, base.gameObject.transform.position, Quaternion.identity, base.gameObject.transform).GetComponent<tk2dSprite>();
				faack.transform.position.WithZ(faack.transform.position.z + 99999);
				faack.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.gameObject.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
				tk2dBaseSprite dsakdslad = base.gameObject.GetComponent<tk2dBaseSprite>();
				dsakdslad.sprite.AttachRenderer(faack.GetComponent<tk2dBaseSprite>());
				faack.name = "medal";
				faack.PlaceAtPositionByAnchor(base.sprite.WorldTopCenter, tk2dBaseSprite.Anchor.LowerCenter);
				faack.scale = Vector3.one;
				Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);

				mat.mainTexture = faack.renderer.material.mainTexture;
				mat.SetColor("_EmissiveColor", new Color32(239, 222, 252, 255));
				mat.SetFloat("_EmissiveColorPower", 1.55f);
				mat.SetFloat("_EmissivePower", 100);
				faack.sprite.renderer.material = mat;
				LootEngine.DoDefaultSynergyPoof(faack.transform.position + new Vector3(0, 1), false);
			}
			yield break;
		}
	}
}

