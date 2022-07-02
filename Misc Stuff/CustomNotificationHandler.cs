using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using ItemAPI;
using NpcApi;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BotsMod
{
	class CustomNotificationHandler
	{

		private static string[] BundlePrereqs = new string[]
			{
				"brave_resources_001",
				"dungeon_scene_001",
				"encounters_base_001",
				"enemies_base_001",
				"flows_base_001",
				"foyer_001",
				"foyer_002",
				"foyer_003",
				"shared_auto_001",
				"shared_auto_002",
				"shared_base_001",
				"dungeons/base_bullethell",
				"dungeons/base_castle",
				"dungeons/base_catacombs",
				"dungeons/base_cathedral",
				"dungeons/base_forge",
				"dungeons/base_foyer",
				"dungeons/base_gungeon",
				"dungeons/base_mines",
				"dungeons/base_nakatomi",
				"dungeons/base_resourcefulrat",
				"dungeons/base_sewer",
				"dungeons/base_tutorial",
				"dungeons/finalscenario_bullet",
				"dungeons/finalscenario_convict",
				"dungeons/finalscenario_coop",
				"dungeons/finalscenario_guide",
				"dungeons/finalscenario_pilot",
				"dungeons/finalscenario_robot",
				"dungeons/finalscenario_soldier"
			};


		public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
		{
			if (BundlePrereqs == null)
			{
				Init();
			}
			T obj = null;
			foreach (string name in BundlePrereqs)
			{
				try
				{
					obj = ResourceManager.LoadAssetBundle(name).LoadAsset<T>(path);
				}
				catch
				{
				}
				if (obj != null)
				{
					break;
				}
			}
			return obj;
		}

		public static dfAnimationClip beyondClip;

		public static void Init()
		{
			var UIRootPrefab = LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>();
			if (UIRootPrefab.Manager.DefaultAtlas == null)
			{
				//BotsModule.Log("BoxSprite.Atlas is null");
			}

			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_001" + ".png"), "notification_box_beyond_001");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_002" + ".png"), "notification_box_beyond_002");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_003" + ".png"), "notification_box_beyond_003");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_004" + ".png"), "notification_box_beyond_004");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_005" + ".png"), "notification_box_beyond_005");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_006" + ".png"), "notification_box_beyond_006");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_007" + ".png"), "notification_box_beyond_007");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_008" + ".png"), "notification_box_beyond_008");

			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/notification_box_beyond_001" + ".png"), "notification_box_beyondns_001");

			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/crosshair_beyond" + ".png"), "crosshair_beyond");
			UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(ItemAPI.ResourceExtractor.GetTextureFromResource("BotsMod/sprites/NotificationSprites/object_box_beyond_001" + ".png"), "object_box_beyond_001");

			var beyondClipObj = new GameObject("Notification_Box_Shine_Clip_Beyond");
			FakePrefab.MarkAsFakePrefab(beyondClipObj);
			beyondClip = beyondClipObj.AddComponent<dfAnimationClip>();
			beyondClip.Atlas = UIRootPrefab.Manager.DefaultAtlas;

			FieldInfo _sprites = typeof(dfAnimationClip).GetField("sprites", BindingFlags.NonPublic | BindingFlags.Instance);
			_sprites.SetValue(beyondClip, new List<string> { "notification_box_beyond_001", "notification_box_beyond_002", "notification_box_beyond_003", "notification_box_beyond_004", "notification_box_beyond_005", "notification_box_beyond_006", "notification_box_beyond_007",
					"notification_box_beyond_008", "notification_box_beyondns_001" });

			new Hook(typeof(UINotificationController).GetMethod("DoNotificationInternal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(CustomNotificationHandler).GetMethod("DoNotificationInternalHook", BindingFlags.Static | BindingFlags.NonPublic));


		}

		private static void DoNotificationInternalHook(Action<UINotificationController, NotificationParams> orig, UINotificationController self, NotificationParams notifyParams)
		{
			bool flag = false;//!string.IsNullOrEmpty(notifyParams.EncounterGuid) && Tools.BeyondItems.Contains(EncounterDatabase.GetEntry(notifyParams.EncounterGuid).pickupObjectId) && GameStatsManager.Instance.QueryEncounterable(notifyParams.EncounterGuid) != 1;
			if (flag)
			{
				FieldInfo field = typeof(UINotificationController).GetField("m_queuedNotifications", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(UINotificationController).GetField("m_queuedNotificationParams", BindingFlags.Instance | BindingFlags.NonPublic);
				(field.GetValue(self) as List<IEnumerator>).Add(CustomNotificationHandler.HandleBeyondNotification(self, notifyParams));
				(field2.GetValue(self) as List<NotificationParams>).Add(notifyParams);
				self.StartCoroutine((IEnumerator)typeof(UINotificationController).GetMethod("PruneQueuedNotifications", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(self, null));
			}
			else
			{
				orig(self, notifyParams);
			}
		}



		public static IEnumerator HandleBeyondNotification(UINotificationController self, NotificationParams notifyParams)
		{
			yield return null;
			typeof(UINotificationController).GetMethod("SetupSprite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { notifyParams.SpriteCollection, notifyParams.SpriteID });

			FieldInfo _doingNotification = typeof(UINotificationController).GetField("m_doingNotification", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo _panel = typeof(UINotificationController).GetField("m_panel", BindingFlags.NonPublic | BindingFlags.Instance);


			self.DescriptionLabel.ProcessMarkup = true;
			self.DescriptionLabel.ColorizeSymbols = true;
			self.NameLabel.Text = notifyParams.PrimaryTitleString.ToUpperInvariant();
			self.DescriptionLabel.Text = notifyParams.SecondaryDescriptionString;
			self.CenterLabel.Opacity = 1f;
			self.NameLabel.Opacity = 1f;
			self.DescriptionLabel.Opacity = 1f;
			self.CenterLabel.IsVisible = false;
			self.NameLabel.IsVisible = true;
			self.DescriptionLabel.IsVisible = true;
			dfSpriteAnimation component = self.BoxSprite.GetComponent<dfSpriteAnimation>();
			component.Stop();
			dfSpriteAnimation component2 = self.CrosshairSprite.GetComponent<dfSpriteAnimation>();
			component2.Stop();
			dfSpriteAnimation component3 = self.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
			component3.Stop();
			string trackableGuid = notifyParams.EncounterGuid;

			self.CrosshairSprite.SpriteName = "crosshair_beyond";
			self.CrosshairSprite.Size = self.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
			self.BoxSprite.SpriteName = "notification_box_beyondns_001";
			self.ObjectBoxSprite.IsVisible = true;
			self.ObjectBoxSprite.SpriteName = "object_box_beyond_001";
			self.StickerSprite.IsVisible = false;

			bool singleLineSansSprite = notifyParams.isSingleLine;
			if (singleLineSansSprite || notifyParams.SpriteCollection == null)
			{
				self.ObjectBoxSprite.IsVisible = false;
				self.StickerSprite.IsVisible = false;
			}
			if (singleLineSansSprite)
			{
				self.CenterLabel.IsVisible = true;
				self.NameLabel.IsVisible = false;
				self.DescriptionLabel.IsVisible = false;
				self.CenterLabel.Text = self.NameLabel.Text;
			}
			else
			{
				self.NameLabel.IsVisible = true;
				self.DescriptionLabel.IsVisible = true;
				self.CenterLabel.IsVisible = false;
			}
			_doingNotification.SetValue(self, true);
			(_panel.GetValue(self) as dfPanel).IsVisible = false;
			GameUIRoot.Instance.MoveNonCoreGroupOnscreen((_panel.GetValue(self) as dfPanel), false);
			float elapsed = 0f;
			float duration = 5f;
			bool hasPlayedAnim = false;
			if (singleLineSansSprite)
			{
				self.notificationObjectSprite.renderer.enabled = false;
				SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, false);
			}
			while (elapsed < ((!notifyParams.HasAttachedSynergy) ? duration : (duration - 2f)))
			{
				elapsed += BraveTime.DeltaTime;
				if (!hasPlayedAnim && elapsed > 0.75f)
				{
					self.BoxSprite.GetComponent<dfSpriteAnimation>().Clip = beyondClip;

					hasPlayedAnim = true;
					self.ObjectBoxSprite.Parent.GetComponent<dfSpriteAnimation>().Play();
				}
				yield return null;
				(_panel.GetValue(self) as dfPanel).IsVisible = true;
				if (!singleLineSansSprite && notifyParams.SpriteCollection != null)
				{
					self.notificationObjectSprite.renderer.enabled = true;
					SpriteOutlineManager.ToggleOutlineRenderers(self.notificationObjectSprite, true);
				}
			}
			if (notifyParams.HasAttachedSynergy)
			{
				AdvancedSynergyEntry pairedSynergy = notifyParams.AttachedSynergy;
				EncounterDatabaseEntry encounterSource = EncounterDatabase.GetEntry(trackableGuid);
				int pickupObjectId = (encounterSource == null) ? -1 : encounterSource.pickupObjectId;
				PickupObject puo = PickupObjectDatabase.GetById(pickupObjectId);
				if (puo)
				{
					//int pID = self.GetIDOfOwnedSynergizingItem(puo.PickupObjectId, pairedSynergy);
					int pID = (int)typeof(UINotificationController).GetMethod("GetIDOfOwnedSynergizingItem", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { puo.PickupObjectId, pairedSynergy });
					PickupObject puo2 = PickupObjectDatabase.GetById(pID);
					if (puo2 && puo2.sprite)
					{
						typeof(UINotificationController).GetMethod("SetupSynergySprite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { puo2.sprite.Collection, puo2.sprite.spriteId });
						elapsed = 0f;
						duration = 4f;
						self.notificationSynergySprite.renderer.enabled = true;
						SpriteOutlineManager.ToggleOutlineRenderers(self.notificationSynergySprite, true);
						dfSpriteAnimation boxSpriteAnimator = self.BoxSprite.GetComponent<dfSpriteAnimation>();
						boxSpriteAnimator.Clip = self.SynergyTransformClip;
						boxSpriteAnimator.Play();
						dfSpriteAnimation crosshairSpriteAnimator = self.CrosshairSprite.GetComponent<dfSpriteAnimation>();
						crosshairSpriteAnimator.Clip = self.SynergyCrosshairTransformClip;
						crosshairSpriteAnimator.Play();
						dfSpriteAnimation objectSpriteAnimator = self.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
						objectSpriteAnimator.Clip = self.SynergyBoxTransformClip;
						objectSpriteAnimator.Play();
						string synergyName = string.IsNullOrEmpty(pairedSynergy.NameKey) ? string.Empty : StringTableManager.GetSynergyString(pairedSynergy.NameKey, -1);
						bool synergyHasName = !string.IsNullOrEmpty(synergyName);
						if (synergyHasName)
						{
							self.CenterLabel.IsVisible = true;
							self.CenterLabel.Text = synergyName;
						}
						while (elapsed < duration)
						{
							float baseSpriteLocalX = self.notificationObjectSprite.transform.localPosition.x;
							float synSpriteLocalX = self.notificationSynergySprite.transform.localPosition.x;
							self.CrosshairSprite.Size = self.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
							float p2u = self.BoxSprite.PixelsToUnits();
							Vector3 endPosition = self.ObjectBoxSprite.GetCenter();
							Vector3 startPosition = endPosition + new Vector3(0f, -120f * p2u, 0f);
							Vector3 startPosition2 = endPosition;
							Vector3 endPosition2 = endPosition + new Vector3(0f, 12f * p2u, 0f);
							endPosition -= new Vector3(0f, 21f * p2u, 0f);
							float t = elapsed / duration;
							float quickT = elapsed / 1f;
							float smoothT = Mathf.SmoothStep(0f, 1f, quickT);
							if (synergyHasName)
							{
								float num = Mathf.SmoothStep(0f, 1f, elapsed / 0.5f);
								float opacity = Mathf.SmoothStep(0f, 1f, (elapsed - 0.5f) / 0.5f);
								self.NameLabel.Opacity = 1f - num;
								self.DescriptionLabel.Opacity = 1f - num;
								self.CenterLabel.Opacity = opacity;
							}
							Vector3 t2 = Vector3.Lerp(startPosition, endPosition, smoothT).Quantize(p2u * 3f).WithX(startPosition.x);
							Vector3 t3 = Vector3.Lerp(startPosition2, endPosition2, smoothT).Quantize(p2u * 3f).WithX(startPosition2.x);
							t3.y = Mathf.Max(startPosition2.y, t3.y);
							self.notificationSynergySprite.PlaceAtPositionByAnchor(t2, tk2dBaseSprite.Anchor.MiddleCenter);
							self.notificationSynergySprite.transform.position = self.notificationSynergySprite.transform.position + new Vector3(0f, 0f, -0.125f);
							self.notificationObjectSprite.PlaceAtPositionByAnchor(t3, tk2dBaseSprite.Anchor.MiddleCenter);
							self.notificationObjectSprite.transform.localPosition = self.notificationObjectSprite.transform.localPosition.WithX(baseSpriteLocalX);
							self.notificationSynergySprite.transform.localPosition = self.notificationSynergySprite.transform.localPosition.WithX(synSpriteLocalX);
							self.notificationSynergySprite.UpdateZDepth();
							self.notificationObjectSprite.UpdateZDepth();
							elapsed += BraveTime.DeltaTime;
							yield return null;
						}
					}
				}
			}
			GameUIRoot.Instance.MoveNonCoreGroupOnscreen((_panel.GetValue(self) as dfPanel), true);
			elapsed = 0f;
			duration = 0.25f;
			while (elapsed < duration)
			{
				elapsed += BraveTime.DeltaTime;
				yield return null;
			}
			self.CenterLabel.Opacity = 1f;
			self.NameLabel.Opacity = 1f;
			self.DescriptionLabel.Opacity = 1f;
			self.CenterLabel.IsVisible = false;
			self.NameLabel.IsVisible = true;
			self.DescriptionLabel.IsVisible = true;

			typeof(UINotificationController).GetMethod("DisableRenderers", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
			_doingNotification.SetValue(self, false);
			yield break;
		}

		private static void ToggleBeyondStatus(UINotificationController self)
		{
			/*
			if (BotsModule.debugMode) ETGModConsole.Log("0");
			self.CrosshairSprite.SpriteName = "crosshair_beyond";
			self.CrosshairSprite.Size = self.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
			if (BotsModule.debugMode) ETGModConsole.Log("1");
			self.BoxSprite.SpriteName = "notification_box_beyondns_001";
			if (BotsModule.debugMode) ETGModConsole.Log("2");
			self.ObjectBoxSprite.IsVisible = true;
			self.ObjectBoxSprite.SpriteName = "object_box_beyond_001";
			if (BotsModule.debugMode) ETGModConsole.Log("3");
			self.StickerSprite.IsVisible = false;
			if (BotsModule.debugMode) ETGModConsole.Log("4");
			*/

		}

	}
}
