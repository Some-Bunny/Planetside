using Alexandria.CharacterAPI;
using ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Planetside;

namespace AmmonomiconAPI
{
    public class AmmonomiconPageInitialization
    {
     
        public class AmmonomiconPageKey : MonoBehaviour { public string UniqueKey = "Test"; }
        public class AmmonomiconPageTag
        {
            public virtual void InitializeItemsPageLeft(AmmonomiconPageRenderer self)
            {

                Transform transform = self.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");
                dfPanel component = transform.Find("Guns Panel").GetComponent<dfPanel>();
                dfPanel component3 = component.transform.GetChild(0).GetComponent<dfPanel>();

                /*
                List<KeyValuePair<int, PickupObject>> list = new List<KeyValuePair<int, PickupObject>>();
                for (int i = 0; i < CharacterDatabase.Instance.Objects.Count; i++)
                {
                    PickupObject pickupObject = CharacterDatabase.Instance.Objects[i];
                    if (!(pickupObject is Gun) && pickupObject != null)
                    {
                        EncounterTrackable component2 = pickupObject.GetComponent<EncounterTrackable>();
                        if (!(component2 == null))
                        {
                            if (string.IsNullOrEmpty(component2.ProxyEncounterGuid))
                            {
                                int key = (pickupObject.ForcedPositionInAmmonomicon >= 0) ? pickupObject.ForcedPositionInAmmonomicon : 1000000000;
                                list.Add(new KeyValuePair<int, PickupObject>(key, pickupObject));
                            }
                        }
                    }
                }
                list = (from e in list
                        orderby e.Key
                        select e).ToList<KeyValuePair<int, PickupObject>>();
                List<EncounterDatabaseEntry> list2 = new List<EncounterDatabaseEntry>();
                for (int j = 0; j < list.Count; j++)
                {
                    EncounterTrackable component4 = list[j].Value.GetComponent<EncounterTrackable>();
                    if (!component4.journalData.SuppressInAmmonomicon)
                    {
                        EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(component4.EncounterGuid);
                        if (entry != null)
                        {
                            list2.Add(entry);
                        }
                    }
                }
                */
                //self.StartCoroutine(typeof(AmmonomiconPageRenderer).GetMethod("ConstructRectanglePageLayout", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(self, new object[] { component3, list2, new Vector2(12f, 20f), new Vector2(20f, 20f), false, null }) as IEnumerator);

                //self.StartCoroutine(self.ConstructRectanglePageLayout(component3, list2, new Vector2(12f, 20f), new Vector2(20f, 20f), false, null));
                component3.Anchor = (dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal);
                component.Height = component3.Height;
                component3.Height = component.Height;

            }

        }



        public static void Init()
        {

            try
            {
                ETGModConsole.Log(1);
                /*
                UIRootPrefab = Tools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>();
                var atlas = Tools.LoadAssetFromAnywhere<GameObject>("Ammonomicon Atlas").GetComponent<dfAtlas>(); //AmmonomiconController.Instance.Ammonomicon.bookmarks[2].AppearClip.Atlas;
                //CollectionDumper.DumpdfAtlas(atlas);
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_001.png"), "bookmark_beyond_001");
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_002.png"), "bookmark_beyond_002");
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_003.png"), "bookmark_beyond_003");
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_004.png"), "bookmark_beyond_004");

                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_hover_001.png"), "bookmark_beyond_hover_001");

                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_select_001.png"), "bookmark_beyond_select_001");
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_select_002.png"), "bookmark_beyond_select_002");
                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_select_003.png"), "bookmark_beyond_select_003");

                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/bookmark_beyond_select_hover_001.png"), "bookmark_beyond_select_hover_001");

                atlas.AddNewItemToAtlas(Tools.GetTextureFromResource("BotsMod/sprites/Ammonomicon/Item_Picture_Beyond_001.png"), "Item_Picture_Beyond_001");
                */

                StringHandler.AddDFStringDefinition("#AMMONOMICON_TEST", "TEST_PAGE");

                var page = FakePrefab.Clone(BraveResources.Load<GameObject>("Global Prefabs/Ammonomicon Pages/Guns Page Left", ".prefab"));
                if (page == null)
                {
                    Tools.Log("Clone is returning a null object", "#eb1313");
                    return;
                }

                page.name = "Test Page Left";
                ETGModConsole.Log(2);



                var renderer = page.GetComponentInChildren<AmmonomiconPageRenderer>();
                ETGModConsole.Log(21);

                //Tools.Log("h", "#eb1313");
                foreach (var child in page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").gameObject.GetComponents<Component>())
                {
                    //Tools.Log(child.ToString());
                }
                ETGModConsole.Log(22);
                /*
                page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").gameObject.GetComponent<dfLabel>().Text = "#AMMONOMICON_BEYOND";
                //Tools.Log("1", "#eb1313");
                ETGModConsole.Log(23);

                page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").Find("Label").gameObject.GetComponent<dfLabel>().Text = "#AMMONOMICON_BEYOND";
                //Tools.Log("2", "#eb1313");
                ETGModConsole.Log(24);

                page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").Find("Label 2").gameObject.GetComponent<dfLabel>().Text = "#AMMONOMICON_BEYOND";
                //Tools.Log("3", "#eb1313");
                ETGModConsole.Log(25);

                page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").Find("Label 3").gameObject.GetComponent<dfLabel>().Text = "#AMMONOMICON_BEYOND";
                //Tools.Log("4", "#eb1313");
                ETGModConsole.Log(26);

                page.transform.Find("Scroll Panel").Find("Header").Find("Label 4").Find("Label 4").gameObject.GetComponent<dfLabel>().Text = "#AMMONOMICON_BEYOND";
                */
                ETGModConsole.Log(27);

                foreach (Transform child in page.transform.Find("Scroll Panel").Find("Scroll Panel").Find("Guns Panel").GetChild(0))
                {
                    //UnityEngine.Object.Destroy(child.gameObject);
                }
                ETGModConsole.Log(28);

                renderer.pageType = (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_LEFT;
                ETGModConsole.Log(29);

                //Tools.Log("a", "#eb1313");


                customPages.Add("Global Prefabs/Ammonomicon Pages/" + page.name, renderer);
                ETGModConsole.Log(3);

                BuildBookmark("Mods");

            }
            catch (Exception e)
            {
                Tools.Log("Ammonomicon broken :(", "#eb1313");
                Tools.Log(string.Format(e + ""), "#eb1313");
            }



        }

        public static string GetItemNamesFromIdList(string baseString, string prefix, List<int> list)
        {
            if (list?.Count > 0)
            {
                prefix += list.Count > 1 ? "s: " : ": ";

                baseString += prefix;
                for (int i = 0; i < list.Count; i++)
                {
                    baseString += PickupObjectDatabase.GetById(list[i]).EncounterNameOrDisplayName + (i == list.Count - 1 ? "." : ", ");
                }
                baseString += "\n";
            }
            return baseString;
        }


        private static IEnumerator DoStartStuff()
        {

            //ETGModConsole.Log(4);
            //yield return  null;

            yield break;
        }

        public static List<AmmonomiconBookmarkController> customBookmarks = new List<AmmonomiconBookmarkController>();
        public static Dictionary<string, AmmonomiconPageRenderer> customPages = new Dictionary<string, AmmonomiconPageRenderer>();
        public static Dictionary<AmmonomiconPageRenderer, AmmonomiconPageTag> customTags = new Dictionary<AmmonomiconPageRenderer, AmmonomiconPageTag>();

        public static void BuildBookmark(string name)
        {

            ETGModConsole.Log(5);

            var obj = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("Ammonomicon Controller", ".prefab"));
            ETGModConsole.Log(51);

            var ji = obj.GetComponent<AmmonomiconInstanceManager>();
            ETGModConsole.Log(511);
            var baseBookmark = ji.bookmarks[2];
            ETGModConsole.Log(512);

            //var selectSprite = UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(Tools.GetTextureFromResource(selectSpritePath + ".png"));
            //var deselectSelectedSprite = UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(Tools.GetTextureFromResource(deselectSelectedSpritePath + ".png"));

            var dumbObj = FakePrefab.Clone(baseBookmark.gameObject);
            ETGModConsole.Log(52);

            var myatlas = StaticSpriteDefinitions.PlanetsideUIAtlas;
            ETGModConsole.Log(53);


            AmmonomiconBookmarkController tabController2 = dumbObj.GetComponent<AmmonomiconBookmarkController>();
            ETGModConsole.Log(54);

            //Tools.Log("9");
            //dumbObj.transform.parent = baseBookmark.gameObject.transform.parent;
            //dumbObj.transform.position = baseBookmark.gameObject.transform.position;
            //dumbObj.transform.localPosition = new Vector3(0, -1.2f, 0);
            ETGModConsole.Log(6);

            tabController2.gameObject.name = name;

            tabController2.SelectSpriteName = "bookmark_psog_hover_001";//selectSprite.name;//1967693681992645534
            tabController2.DeselectSelectedSpriteName = "bookmark_psog_004";//deselectSelectedSprite.name;

            tabController2.TargetNewPageLeft = "Global Prefabs/Ammonomicon Pages/Equipment Page Left";
            tabController2.TargetNewPageRight = "Global Prefabs/Ammonomicon Pages/Equipment Page Right";
            tabController2.RightPageType = (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_RIGHT;
            tabController2.LeftPageType = (AmmonomiconPageRenderer.PageType)CustomPageType.MODS_LEFT;

            tabController2.AppearClip = baseBookmark.AppearClip;
            tabController2.SelectClip = baseBookmark.SelectClip;
            //Tools.Log("9.5");
            FieldInfo m_sprite = typeof(AmmonomiconBookmarkController).GetField("m_sprite", BindingFlags.NonPublic | BindingFlags.Instance);
            //var thing = m_sprite.GetValue(baseBookmark) as dfButton;
            //m_sprite.SetValue(tabController2, thing);

            FieldInfo m_animator = typeof(AmmonomiconBookmarkController).GetField("m_animator", BindingFlags.NonPublic | BindingFlags.Instance);
            //var thing2 = m_animator.GetValue(baseBookmark) as dfSpriteAnimation;
            //m_animator.SetValue(tabController2, thing2);
            ETGModConsole.Log(7);

            dumbObj.SetActive(true);
            customBookmarks.Add(tabController2);
        }

        public static GameUIRoot UIRootPrefab;

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