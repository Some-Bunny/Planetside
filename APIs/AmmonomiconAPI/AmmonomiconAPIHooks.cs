using ItemAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AmmonomiconAPI
{
    class AmmonomiconAPIHooks
    {
        public static void Init()
        {

            try
            {
                var startHook = new Hook(
                    typeof(AmmonomiconInstanceManager).GetMethod("Open", BindingFlags.Instance | BindingFlags.Public),
                    typeof(AmmonomiconAPIHooks).GetMethod("OpenHook", BindingFlags.Static | BindingFlags.Public));

                //BotsModule.Log("AH: startHook Setup", "#03fc0b");

                /*var ammonomiconInitializeHook = new Hook(
					typeof(AmmonomiconPageRenderer).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public),
					typeof(AmmonomiconHooks).GetMethod("AmmonomiconInitializeHook", BindingFlags.Static | BindingFlags.Public));
				*/
                //BotsModule.Log("AH: ammonomiconInitializeHook Setup", "#03fc0b");

                var toggleHeaderImageHook = new Hook(
                    typeof(AmmonomiconPageRenderer).GetMethod("ToggleHeaderImage", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(AmmonomiconAPIHooks).GetMethod("ToggleHeaderImageHook", BindingFlags.Static | BindingFlags.Public));

                //BotsModule.Log("AH: toggleHeaderImageHook Setup", "#03fc0b");

                var updateOnBecameActiveHook = new Hook(
                    typeof(AmmonomiconPageRenderer).GetMethod("UpdateOnBecameActive", BindingFlags.Instance | BindingFlags.Public),
                    typeof(AmmonomiconAPIHooks).GetMethod("UpdateOnBecameActiveHook", BindingFlags.Static | BindingFlags.Public));
                //BotsModule.Log("AH: updateOnBecameActiveHook Setup", "#03fc0b");


                var checkLanguageFontsHook = new Hook(
                    typeof(AmmonomiconPageRenderer).GetMethod("CheckLanguageFonts", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(AmmonomiconAPIHooks).GetMethod("CheckLanguageFontsHook", BindingFlags.Static | BindingFlags.NonPublic));
                //BotsModule.Log("AH: checkLanguageFontsHook Setup", "#03fc0b");

                var LoadPageUIAtPathHook = new Hook(
                    typeof(AmmonomiconController).GetMethod("LoadPageUIAtPath", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(AmmonomiconAPIHooks).GetMethod("LoadPageUIAtPathHook", BindingFlags.Static | BindingFlags.NonPublic));
                //BotsModule.Log("AH: LoadPageUIAtPathHook Setup", "#03fc0b");
                /*var UpdateEncounterStateHook = new Hook(
					typeof(AmmonomiconPokedexEntry).GetMethod("UpdateEncounterState", BindingFlags.Instance | BindingFlags.Public),
					typeof(AmmonomiconHooks).GetMethod("UpdateEncounterStateHook", BindingFlags.Static | BindingFlags.Public));
				*/
                var SetRightDataPageTextsHook = new Hook(
                    typeof(AmmonomiconPageRenderer).GetMethod("SetRightDataPageTexts", BindingFlags.Instance | BindingFlags.Public),
                    typeof(AmmonomiconAPIHooks).GetMethod("SetRightDataPageTextsHook", BindingFlags.Static | BindingFlags.Public));

                var DelayedBuildPageHook = new Hook(
                    typeof(AmmonomiconPageRenderer).GetMethod("DelayedBuildPage", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(AmmonomiconAPIHooks).GetMethod("DelayedBuildPageHook", BindingFlags.Static | BindingFlags.NonPublic));

                //BotsModule.Log("AH: UpdateEncounterStateHook Setup", "#03fc0b");
                /*return;
				var SetEncounterStateHook = new Hook(
					typeof(AmmonomiconPokedexEntry).GetMethod("SetEncounterState", BindingFlags.Instance | BindingFlags.Public),
					typeof(AmmonomiconHooks).GetMethod("SetEncounterStateHook", BindingFlags.Static | BindingFlags.Public));
				//BotsModule.Log("AH: SetEncounterStateHook Setup", "#03fc0b");*/
            }
            catch (Exception arg)
            {
                Tools.Log("oh no thats not good (AmmonomiconHooks broke): " + arg, "#eb1313");
                //LostItemsMod.Log(string.Format("D:", ), "#eb1313");
            }
        }



        public static void SetRightDataPageTextsHook(Action<AmmonomiconPageRenderer, tk2dBaseSprite, EncounterDatabaseEntry> orig, AmmonomiconPageRenderer self, tk2dBaseSprite sourceSprite, EncounterDatabaseEntry linkedTrackable)
        {
            Debug.Log("SetRightDataPageTextsHook_!");

            JournalEntry journalData = linkedTrackable.journalData;
            AmmonomiconPageRenderer ammonomiconPageRenderer = (!(AmmonomiconController.Instance.ImpendingRightPageRenderer != null)) ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer;
            dfScrollPanel component = ammonomiconPageRenderer.guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>();
            Transform transform = component.transform.Find("Header");
            if (transform)
            {
                dfLabel component2 = transform.Find("Label").GetComponent<dfLabel>();
                component2.Text = journalData.GetPrimaryDisplayName(false);
                if (linkedTrackable.ForceEncounterState)
                {
                    component2.Text = component2.ForceGetLocalizedValue("#AMMONOMICON_UNKNOWN");
                }
                component2.PerformLayout();
                dfSprite component3 = transform.Find("Sprite").GetComponent<dfSprite>();
                if (component3)
                {
                    component3.FillDirection = dfFillDirection.Vertical;
                    component3.FillAmount = ((GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH) ? 0.8f : 1f);
                    component3.InvertFill = true;
                }
            }
            dfLabel component4 = component.transform.Find("Tape Line One").Find("Label").GetComponent<dfLabel>();
            component4.Text = journalData.GetNotificationPanelDescription();
            component4.PerformLayout();
            dfSlicedSprite componentInChildren = component.transform.Find("Tape Line One").GetComponentInChildren<dfSlicedSprite>();
            componentInChildren.Width = component4.GetAutosizeWidth() / 4f + 12f;
            dfLabel component5 = component.transform.Find("Tape Line Two").Find("Label").GetComponent<dfLabel>();
            component5.Text = linkedTrackable.GetSecondTapeDescriptor();
            component5.PerformLayout();
            dfSlicedSprite componentInChildren2 = component.transform.Find("Tape Line Two").GetComponentInChildren<dfSlicedSprite>();
            componentInChildren2.Width = component5.GetAutosizeWidth() / 4f + 12f;
            dfPanel component6 = component.transform.Find("ThePhoto").Find("Photo").Find("tk2dSpriteHolder").GetComponent<dfPanel>();
            dfSprite component7 = component.transform.Find("ThePhoto").Find("Photo").Find("ItemShadow").GetComponent<dfSprite>();
            component7.IsVisible = !journalData.IsEnemy;
            tk2dSprite tk2dSprite = component6.GetComponentInChildren<tk2dSprite>();
            dfTextureSprite componentInChildren3 = component.transform.Find("ThePhoto").GetComponentInChildren<dfTextureSprite>();
            /*
            if (linkedTrackable != null && BotsMod.Tools.BeyondItems.Contains(linkedTrackable.pickupObjectId))
            {
                component.transform.Find("ThePhoto").Find("Photo").gameObject.GetComponent<dfSprite>().SpriteName = "Item_Picture_Beyond_001";

            }
            else
            {
                component.transform.Find("ThePhoto").Find("Photo").gameObject.GetComponent<dfSprite>().SpriteName = "Item_Picture_Gungeon_002";

            }
            */


            if (journalData.IsEnemy && journalData.enemyPortraitSprite != null)
            {
                if (tk2dSprite != null)
                {
                    if (SpriteOutlineManager.HasOutline(tk2dSprite))
                    {
                        SpriteOutlineManager.RemoveOutlineFromSprite(tk2dSprite, true);
                    }
                    tk2dSprite.renderer.enabled = false;
                }
                componentInChildren3.IsVisible = true;
                componentInChildren3.Texture = journalData.enemyPortraitSprite;
            }
            else
            {
                if (componentInChildren3 != null)
                {
                    componentInChildren3.IsVisible = false;
                }
                if (tk2dSprite == null)
                {
                    tk2dSprite = self.AddSpriteToPage(sourceSprite);
                    if (!journalData.IsEnemy)
                    {
                        tk2dSprite.scale *= 2f;
                    }
                    tk2dSprite.transform.parent = component6.transform;
                }
                else
                {
                    tk2dSprite.renderer.enabled = true;
                    tk2dSprite.SetSprite(sourceSprite.Collection, sourceSprite.spriteId);
                }
                if (SpriteOutlineManager.HasOutline(tk2dSprite))
                {
                    SpriteOutlineManager.RemoveOutlineFromSprite(tk2dSprite, true);
                }
                SpriteOutlineManager.AddScaledOutlineToSprite<tk2dSprite>(tk2dSprite, Color.black, 0.1f, 0.05f);
                if (journalData.IsEnemy)
                {
                    tk2dSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
                }
                else
                {
                    tk2dSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.LowerCenter);
                }
                if (Mathf.RoundToInt(sourceSprite.GetCurrentSpriteDef().GetBounds().size.x / 0.0625f) % 2 == 1)
                {
                    tk2dSprite.transform.position = tk2dSprite.transform.position.WithX(tk2dSprite.transform.position.x - 0.03125f * tk2dSprite.scale.x);
                }
                tk2dSprite.usesOverrideMaterial = true;
                tk2dSprite.renderer.material.shader = ShaderCache.Acquire("tk2d/CutoutVertexColorTilted");
            }
            dfLabel component8 = component.transform.Find("Scroll Panel").Find("Panel").Find("Label").GetComponent<dfLabel>();

            typeof(AmmonomiconPageRenderer).GetMethod("CheckLanguageFonts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { component8 });
            component8.Text = linkedTrackable.GetModifiedLongDescription();
            component8.transform.parent.GetComponent<dfPanel>().Height = component8.Height;
            component8.PerformLayout();
            component8.Update();
            dfScrollPanel component9 = component.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>();
            component9.ScrollPosition = Vector2.zero;
            component.PerformLayout();
            component.Update();
        }


        public static void SetEncounterStateHook(AmmonomiconPokedexEntry self, AmmonomiconPokedexEntry.EncounterState st)
        {
            Debug.Log("SetEncounterStateHook_!");

            FieldInfo _childSprite = typeof(AmmonomiconPokedexEntry).GetField("m_childSprite", BindingFlags.NonPublic | BindingFlags.Instance);

            if (self.IsEquipmentPage)
            {
                return;
            }
            if (!self.ForceEncounterState)
            {
                self.encounterState = st;
            }

            (_childSprite.GetValue(self) as tk2dClippedSprite).usesOverrideMaterial = true;
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.shader = ShaderCache.Acquire("Brave/AmmonomiconSpriteListShader");
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.SetFloat("_SpriteScale", (_childSprite.GetValue(self) as tk2dClippedSprite).scale.x);
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.SetFloat("_Saturation", 1f);
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.material.SetColor("_OverrideColor", new Color(0.4f, 0.4f, 0.4f, 0f));
            (_childSprite.GetValue(self) as tk2dClippedSprite).renderer.enabled = true;
            self.questionMarkSprite.IsVisible = false;
        }


        public static void UpdateEncounterStateHook(AmmonomiconPokedexEntry self)
        {

            //Tools.Log("forcefully set encounter state", "#eb1313");
            self.SetEncounterState(AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED);
        }

        public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        private static AmmonomiconPageRenderer LoadPageUIAtPathHook(Func<AmmonomiconController, string, AmmonomiconPageRenderer.PageType, bool, bool, AmmonomiconPageRenderer> orig, AmmonomiconController self, string path, AmmonomiconPageRenderer.PageType pageType, bool isPreCache = false, bool isVictory = false)
        {
            Debug.Log("LoadPageUIAtPathHook_!");

            foreach (var page in AmmonomiconPageInitialization.customPages)
            {
                //Tools.Log(page.Key, "#eb1313");
            }

            FieldInfo _extantPageMap = typeof(AmmonomiconController).GetField("m_extantPageMap", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _AmmonomiconBase = typeof(AmmonomiconController).GetField("m_AmmonomiconBase", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _offsets = typeof(AmmonomiconController).GetField("m_offsets", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _offsetInUse = typeof(AmmonomiconController).GetField("m_offsetInUse", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _LowerRenderTargetPrefab = typeof(AmmonomiconController).GetField("m_LowerRenderTargetPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

            if (AmmonomiconPageInitialization.customPages.ContainsKey(path))
            {
                AmmonomiconPageRenderer ammonomiconPageRenderer;
                if ((_extantPageMap.GetValue(self) as Dictionary<AmmonomiconPageRenderer.PageType, AmmonomiconPageRenderer>).ContainsKey(pageType))
                {
                    ammonomiconPageRenderer = (_extantPageMap.GetValue(self) as Dictionary<AmmonomiconPageRenderer.PageType, AmmonomiconPageRenderer>)[pageType];

                    //Tools.Log("fff", "#eb1313");
                    ammonomiconPageRenderer.EnableRendering();
                    ammonomiconPageRenderer.DoRefreshData();
                    //Tools.Log("hhh", "#eb1313");
                }
                else
                {
                    //Tools.Log("1", "#eb1313");
                    GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(AmmonomiconPageInitialization.customPages[path].gameObject); //(GameObject)UnityEngine.Object.Instantiate(BraveResources.Load(path, ".prefab"));
                    ammonomiconPageRenderer = gameObject.GetComponentInChildren<AmmonomiconPageRenderer>();
                    dfGUIManager component2 = (_AmmonomiconBase.GetValue(self) as GameObject).GetComponent<dfGUIManager>();
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((_LowerRenderTargetPrefab.GetValue(self) as MeshRenderer).gameObject);
                    gameObject2.transform.parent = component2.transform.Find("Core");
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.layer = LayerMask.NameToLayer("SecondaryGUI");
                    MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
                    //Tools.Log("2", "#eb1313");
                    ammonomiconPageRenderer.Initialize(component3);
                    ammonomiconPageRenderer.EnableRendering();
                    //Tools.Log("3", "#eb1313");
                    //List<bool>

                    for (int i = 0; i < (_offsetInUse.GetValue(self) as List<bool>).Count; i++)
                    {
                        if (!(_offsetInUse.GetValue(self) as List<bool>)[i])
                        {
                            (_offsetInUse.GetValue(self) as List<bool>)[i] = true;
                            gameObject.transform.position = (_offsets.GetValue(self) as List<Vector3>)[i];
                            ammonomiconPageRenderer.offsetIndex = i;
                            break;
                        }
                    }
                    //Tools.Log("4", "#eb1313");
                    (_extantPageMap.GetValue(self) as Dictionary<AmmonomiconPageRenderer.PageType, AmmonomiconPageRenderer>).Add(pageType, ammonomiconPageRenderer);
                    if (isPreCache)
                    {
                        ammonomiconPageRenderer.Disable(isPreCache);
                    }
                    else
                    {
                        ammonomiconPageRenderer.transform.parent.parent = (_AmmonomiconBase.GetValue(self) as GameObject).transform.parent;
                    }
                    //Tools.Log("5", "#eb1313");
                }
                return ammonomiconPageRenderer;
            }
            else
            {
                return orig(self, path, pageType, isPreCache, isVictory);
            }

        }


        #region a
        public static void UpdateOnBecameActiveHook(Action<AmmonomiconPageRenderer> orig, AmmonomiconPageRenderer self)
        {
            Debug.Log("UpdateOnBecameActiveHook_!");

            self.ForceUpdateLanguageFonts();
            if (AmmonomiconController.Instance.ImpendingLeftPageRenderer == null || AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget == null)
            {
                switch (self.pageType)
                {
                    case AmmonomiconPageRenderer.PageType.GUNS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetFirstVisibleTexts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
                        break;
                    case AmmonomiconPageRenderer.PageType.ITEMS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetFirstVisibleTexts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
                        break;
                    case AmmonomiconPageRenderer.PageType.ENEMIES_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetFirstVisibleTexts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
                        break;
                    case AmmonomiconPageRenderer.PageType.BOSSES_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetFirstVisibleTexts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
                        break;
                    case (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetFirstVisibleTexts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
                        break;
                }
            }
        }

        private static void CheckLanguageFontsHook(Action<AmmonomiconPageRenderer, dfLabel> orig, AmmonomiconPageRenderer self, dfLabel mainText)
        {
            Debug.Log("CheckLanguageFontsHook_!");

            FieldInfo _englishFont = typeof(AmmonomiconPageRenderer).GetField("EnglishFont", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _otherLanguageFont = typeof(AmmonomiconPageRenderer).GetField("OtherLanguageFont", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _cachedLanguage = typeof(AmmonomiconPageRenderer).GetField("m_cachedLanguage", BindingFlags.NonPublic | BindingFlags.Instance);


            if (_englishFont.GetValue(self) == null)
            {
                _englishFont.SetValue(self, mainText.Font);
                _otherLanguageFont.SetValue(self, GameUIRoot.Instance.Manager.DefaultFont);
            }
            typeof(AmmonomiconPageRenderer).GetMethod("AdjustForChinese", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);
            if ((StringTableManager.GungeonSupportedLanguages)_cachedLanguage.GetValue(self) != GameManager.Options.CurrentLanguage)
            {
                _cachedLanguage.SetValue(self, GameManager.Options.CurrentLanguage);
                switch (self.pageType)
                {
                    case AmmonomiconPageRenderer.PageType.GUNS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                    case AmmonomiconPageRenderer.PageType.ITEMS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                    case AmmonomiconPageRenderer.PageType.ENEMIES_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                    case AmmonomiconPageRenderer.PageType.BOSSES_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                    case (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                }
            }
            if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
            {
                if (mainText.Font != (dfFontBase)_englishFont.GetValue(self))
                {
                    mainText.Atlas = self.guiManager.DefaultAtlas;
                    mainText.Font = (dfFontBase)_englishFont.GetValue(self);
                }
            }
            else if (StringTableManager.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE && StringTableManager.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.KOREAN && StringTableManager.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE && StringTableManager.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.RUSSIAN)
            {
                if (mainText.Font != (dfFontBase)_otherLanguageFont.GetValue(self))
                {
                    mainText.Atlas = GameUIRoot.Instance.Manager.DefaultAtlas;
                    mainText.Font = (dfFontBase)_otherLanguageFont.GetValue(self);
                }
            }
        }

        public static void AmmonomiconInitializeHook(Action<AmmonomiconPageRenderer, MeshRenderer> orig, AmmonomiconPageRenderer self, MeshRenderer ts)
        {
            Debug.Log("AmmonomiconInitializeHook_!");

            FieldInfo _camera = typeof(AmmonomiconPageRenderer).GetField("m_camera", BindingFlags.NonPublic | BindingFlags.Instance);

            FieldInfo _topBezierPropID = typeof(AmmonomiconPageRenderer).GetField("topBezierPropID", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _leftBezierPropID = typeof(AmmonomiconPageRenderer).GetField("leftBezierPropID", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _rightBezierPropID = typeof(AmmonomiconPageRenderer).GetField("rightBezierPropID", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _bottomBezierPropID = typeof(AmmonomiconPageRenderer).GetField("bottomBezierPropID", BindingFlags.NonPublic | BindingFlags.Instance);

            self.targetRenderer = ts;

            _camera.SetValue(self, self.GetComponent<Camera>());
            (_camera.GetValue(self) as Camera).aspect = 0.8888889f;

            self.guiManager = self.transform.parent.GetComponent<dfGUIManager>();
            self.guiManager.UIScale = 1f;
            Transform transform = self.guiManager.transform.Find("Scroll Panel");
            if (transform != null)
            {
                transform.GetComponent<dfScrollPanel>().LockScrollPanelToZero = true;
            }
            typeof(AmmonomiconPageRenderer).GetMethod("RebuildRenderData", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, null);

            _topBezierPropID.SetValue(self, Shader.PropertyToID("_TopBezier"));
            _leftBezierPropID.SetValue(self, Shader.PropertyToID("_LeftBezier"));
            _rightBezierPropID.SetValue(self, Shader.PropertyToID("_RightBezier"));
            _bottomBezierPropID.SetValue(self, Shader.PropertyToID("_BottomBezier"));

            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetRow(0, new Vector4(0f, 0f, 0f, 0f));
            matrix.SetRow(1, new Vector4(0f, 0f, 0f, 0f));
            matrix.SetRow(2, new Vector4(1f, 1f, 1f, 1f));
            matrix.SetRow(3, new Vector4(1f, 1f, 1f, 1f));
            self.SetMatrix(matrix);

            //self.StartCoroutine(DelayedBuildPageHook(self));
        }

        private static IEnumerator DelayedBuildPageHook(Func<AmmonomiconPageRenderer, IEnumerator> orig, AmmonomiconPageRenderer self)
        {

            Debug.Log("DelayedBuildPageHook_!");

            IEnumerator origEnum = orig(self);
            while (origEnum.MoveNext())
            {
                switch (self.pageType)
                {
                    case (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_LEFT:
                        if (AmmonomiconPageInitialization.customTags.ContainsKey(self))
                        {
                            AmmonomiconPageInitialization.customTags[self].InitializeItemsPageLeft(self);
                        }
                        else
                        {
                            Debug.Log("oh no");
                            new AmmonomiconPageInitialization.AmmonomiconPageTag().InitializeItemsPageLeft(self);
                        }
                        //AmmonomiconPageInitialization.InitializeItemsPageLeft(self);
                        //self.InitializeItemsPageLeft();
                        //InitializeModsPageLeft(self);
                        break;

                    case (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_RIGHT:
                        typeof(AmmonomiconPageRenderer).GetMethod("SetPageDataUnknown", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { self });
                        break;
                    default:
 
                        break;
                }
                object obj = origEnum.Current;
                yield return obj;
            }


            yield break;
        }

        public static void ToggleHeaderImageHook(Action<AmmonomiconPageRenderer> orig, AmmonomiconPageRenderer self)
        {
            Debug.Log("ToggleHeaderImageHook_!");

            if (self.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT || self.pageType == AmmonomiconPageRenderer.PageType.GUNS_LEFT || self.pageType == AmmonomiconPageRenderer.PageType.ITEMS_LEFT || self.pageType == AmmonomiconPageRenderer.PageType.ENEMIES_LEFT || self.pageType == AmmonomiconPageRenderer.PageType.BOSSES_LEFT || self.pageType == (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_LEFT)
            {
                if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH && self.HeaderBGSprite != null)
                {
                    self.HeaderBGSprite.IsVisible = false;
                }
                else if (self.HeaderBGSprite != null)
                {
                    self.HeaderBGSprite.IsVisible = true;
                }
            }
        }


        public static void InitializeModsPageLeft(AmmonomiconPageRenderer stupidFuckingDumbCunt)
        {
            Debug.Log("InitializeModsPageLeft_!");

            /*
            Transform transform = stupidFuckingDumbCunt.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");
            dfPanel component = transform.Find("Guns Panel").GetComponent<dfPanel>();
            List<KeyValuePair<string, Texture2D>> list = new List<KeyValuePair<string, Texture2D>>();

            for (int i = 0; i < ETGMod.GameMods.Count; i += 1)
            {
                ETGModuleMetadata metadata = ETGMod.GameMods[i].Metadata;

                list.Add(new KeyValuePair<string, Texture2D>(metadata.Name, metadata.Icon));
            }

            list = (from e in list orderby e.Key select e).ToList<KeyValuePair<string, Texture2D>>();
            List<EncounterDatabaseEntry> list2 = new List<EncounterDatabaseEntry>();
            dfPanel component3 = component.transform.GetChild(0).GetComponent<dfPanel>();

            //stupidFuckingDumbCunt.StartCoroutine(stupidFuckingDumbCunt.ConstructRectanglePageLayout(component3, list2, new Vector2(12f, 20f), new Vector2(20f, 20f), false, null));
            component3.Anchor = (dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal);
            component.Height = component3.Height;
            component3.Height = component.Height;
            */
        }
        #endregion
        public static void OpenHook(Action<AmmonomiconInstanceManager> orig, AmmonomiconInstanceManager self)
        {
            try
            {
                Debug.Log("OpenHook_!");
                //Ammonomicon.BuildBookmark("Mods", "BotsMod/sprites/wip", "BotsMod/sprites/wip");
                List<AmmonomiconBookmarkController> ammonomiconBookmarks = self.bookmarks.ToList();

                foreach (var bookmark in AmmonomiconPageInitialization.customBookmarks)
                {

                    if (!ammonomiconBookmarks.Contains(bookmark))
                    {
                        //Tools.Log(bookmark.gameObject.name);
                        ammonomiconBookmarks.Insert(ammonomiconBookmarks.Count - 2, bookmark);
                    }
                }
                bool dosetup = true;
                bool dosetup1 = true;

                foreach (var mark in self.bookmarks)
                {
                    if (mark.name.Contains("Spells"))
                    {

                        //dosetup = false;
                        break;
                    }

                }
                //dosetup = false;
                
                if(dosetup == true)
                {
                    foreach (var entry in AmmonomiconPageInitialization.customBookmarks)
                    {
                        var dumbObj = UnityEngine.Object.Instantiate(entry);
                        AmmonomiconBookmarkController tabController2 = dumbObj.GetComponent<AmmonomiconBookmarkController>();
                        dumbObj.transform.parent = self.bookmarks.Last().gameObject.transform.parent;
                        dumbObj.transform.position = self.bookmarks.Last().gameObject.transform.position;
                        dumbObj.transform.localPosition = new Vector3(0, -1.2f, 0);
                    }
                }

                if (dosetup1 == false)
                {
                    var dumbObj = FakePrefab.Clone(self.bookmarks[ammonomiconBookmarks.Count - 1].gameObject);


                    AmmonomiconBookmarkController tabController2 = dumbObj.GetComponent<AmmonomiconBookmarkController>();

                    //Tools.Log("9");
                    dumbObj.transform.parent = self.bookmarks[2].gameObject.transform.parent;
                    dumbObj.transform.position = self.bookmarks[2].gameObject.transform.position;
                    dumbObj.transform.localPosition = new Vector3(0, -1.2f, 0);

                    tabController2.gameObject.name = "Spells";
                    //1967693681992645534
                    //tabController2.DeselectSelectedSpriteName = "bookmark_beyond_select_hover_001";
                    //tabController2.SelectSpriteName = "bookmark_beyond_hover_001";

                    FieldInfo _sprites = typeof(dfAnimationClip).GetField("sprites", BindingFlags.NonPublic | BindingFlags.Instance);

                    var beyondClipObj = new GameObject("AmmonomiconBookmarkBeyondHover");
                    FakePrefab.MarkAsFakePrefab(beyondClipObj);
                    var beyondClip = beyondClipObj.AddComponent<dfAnimationClip>();
                    beyondClip.Atlas = self.bookmarks[2].AppearClip.Atlas;

                    _sprites.SetValue(beyondClip, new List<string> { "bookmark_beyond_001", "bookmark_beyond_002", "bookmark_beyond_003", "bookmark_beyond_004" });

                    var beyondClipObj2 = new GameObject("AmmonomiconBookmarkBeyondSelectHover");
                    FakePrefab.MarkAsFakePrefab(beyondClipObj2);
                    var beyondClip2 = beyondClipObj.AddComponent<dfAnimationClip>();
                    beyondClip2.Atlas = self.bookmarks[2].AppearClip.Atlas;

                    _sprites.SetValue(beyondClip2, new List<string> { "bookmark_beyond_select_001", "bookmark_beyond_select_002", "bookmark_beyond_select_003" });

                    tabController2.TargetNewPageLeft = "Global Prefabs/Ammonomicon Pages/Beyond Page Left";
                    tabController2.TargetNewPageRight = "Global Prefabs/Ammonomicon Pages/Info Page Right";
                    tabController2.RightPageType = (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_RIGHT;
                    //tabController2.RightPageType = AmmonomiconPageRenderer.PageType.ITEMS_RIGHT;
                    tabController2.LeftPageType = (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_LEFT;
                    //tabController2.LeftPageType = AmmonomiconPageRenderer.PageType.ITEMS_LEFT;
                    tabController2.AppearClip = beyondClip;
                    tabController2.SelectClip = beyondClip2;
                    //Tools.Log("9.5");

                    /*FieldInfo m_sprite = typeof(AmmonomiconBookmarkController).GetField("m_sprite", BindingFlags.NonPublic | BindingFlags.Instance);
					m_sprite.SetValue(tabController2, m_sprite.GetValue(self.bookmarks[2]) as dfButton);

					FieldInfo m_animator = typeof(AmmonomiconBookmarkController).GetField("m_animator", BindingFlags.NonPublic | BindingFlags.Instance);
					m_animator.SetValue(tabController2, m_animator.GetValue(self.bookmarks[2]) as dfSpriteAnimation);
					*/
                    //Tools.Log("10");

                    if (dumbObj.GetComponent<dfButton>() == null)
                    {
                        Tools.Log("dfButton nulled :(");
                    }

                    dumbObj.GetComponent<dfButton>().BackgroundSprite = "bookmark_beyond_004";

                    ammonomiconBookmarks.Insert(ammonomiconBookmarks.Count - 1, tabController2);

                    //ammonomiconBookmarks.Add(deathBookmark);

                    //Tools.Log("11");
                    //self.bookmarks = ammonomiconBookmarks.ToArray();

                    //Tools.Log("12");

                    dumbObj.SetActive(true);
                    //ItemAPI.FakePrefab.MarkAsFakePrefab(dumbObj);*/
                    self.bookmarks = ammonomiconBookmarks.ToArray();
                    //Tools.Log("13");




                    foreach (Transform bookmark in tabController2.transform.parent)
                    {
                        //Tools.Log(bookmark.gameObject.name);
                    }
                }
                
                orig(self);
                //Tools.Log("8");

            }

            catch (Exception e)
            {
                Tools.Log("Ammonomicon broken :(", "#eb1313");
                Tools.Log(string.Format(e + ""), "#eb1313");
            }
        }
        public enum CustomPageType
        {
            NONE = 0,
            EQUIPMENT_LEFT = 1,
            EQUIPMENT_RIGHT = 2,
            GUNS_LEFT = 3,
            GUNS_RIGHT = 4,
            ITEMS_LEFT = 5,
            ITEMS_RIGHT = 6,
            ENEMIES_LEFT = 7,
            ENEMIES_RIGHT = 8,
            BOSSES_LEFT = 9,
            BOSSES_RIGHT = 10,
            DEATH_LEFT = 11,
            DEATH_RIGHT = 12,
            MODS_LEFT = 13,
            MODS_RIGHT = 14
        }

    }
}
