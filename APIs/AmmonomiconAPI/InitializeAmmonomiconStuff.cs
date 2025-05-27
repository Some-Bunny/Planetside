using AmmonomiconAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Planetside.APIs
{
    public class AmmonomiconModification
    {
        public static void InitializeAmmonomiconStuff()
        {
            AmmonomiconAPI.UIBuilder.BuildBookmark("psog", "Perks", new PerkAmmonomiconPageController(),
                new SpriteContainer(StaticSpriteDefinitions.PlanetsideUIAtlas)
                {
                    AppearFrames = new string[] { "bookmark_psog_001", "bookmark_psog_002", "bookmark_psog_003", "bookmark_psog_004" },
                    SelectFrames = new string[] { "bookmark_psog_select_001", "bookmark_psog_select_002", "bookmark_psog_select_003" },
                    HoverFrame = "bookmark_psog_hover_001",
                    SelectHoverFrame ="bookmark_psog_select_hover_001"
                }, Assembly.GetExecutingAssembly());

            AmmonomiconAPI.CustomActions.OnPreEquipmentPageBuild += BuildEquipmentPage;
            AmmonomiconAPI.CustomActions.OnDeathPageFinalizing += BuilDeathRightPage;

        }
        public static void BuilDeathRightPage(AmmonomiconPageRenderer ammonomiconPageRenderer, List<tk2dBaseSprite> tk2DBaseSprites)
        {
            AmmonomiconDeathPageController component = ammonomiconPageRenderer.guiManager.GetComponent<AmmonomiconDeathPageController>();
            dfScrollPanel component2 = component.transform.Find("Scroll Panel").Find("Footer").Find("ScrollItemsPanel").GetComponent<dfScrollPanel>();
            dfPanel component3 = component2.transform.Find("AllItemsPanel").GetComponent<dfPanel>();

            var player = AmmonomiconAPI.HelperTools.LastPlayerOpenedUI();
            if (player)
            {
                if (CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart > 0) { AddSprites(ammonomiconPageRenderer, player, component3, CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart - 1, Patience.PatienceID, ref tk2DBaseSprites); }
                if (player.GetComponent<GunslingerController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<GunslingerController>().Stack, Gunslinger.GunslingerID, ref tk2DBaseSprites); }
                if (player.GetComponent<GreedController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<GreedController>().Stack, Greedy.GreedyID, ref tk2DBaseSprites); }
                if (player.GetComponent<AllSeeingEyeController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<AllSeeingEyeController>().Stacks, AllSeeingEye.AllSeeingEyeID, ref tk2DBaseSprites); }    
                if (player.GetComponent<BlastProjectilesCheck>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<BlastProjectilesCheck>().Stack, BlastProjectiles.BlastProjectilesID, ref tk2DBaseSprites); }           
                if (player.GetComponent<ContractController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<ContractController>().Boys.Count, Contract.ContractID, ref tk2DBaseSprites); }
                if (player.GetComponent<GlassComponent>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<GlassComponent>().Stack, Glass.GlassID, ref tk2DBaseSprites); }
                if (player.GetComponent<PitLordsPactController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<PitLordsPactController>().Stacks, PitLordsPact.PitLordsPactID, ref tk2DBaseSprites); }
                if (player.GetComponent<ChaoticShiftController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<ChaoticShiftController>().AmountOfModules, ChaoticShift.ChaoticShiftID, ref tk2DBaseSprites); }
                if (player.GetComponent<CorruptedWealthController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<CorruptedWealthController>().StackCount, CorruptedWealth.CorruptedWealthID, ref tk2DBaseSprites); }
                if (player.GetComponent<UnbreakableSpiritController>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<UnbreakableSpiritController>().Stacks, UnbreakableSpirit.UnbreakableSpiritID, ref tk2DBaseSprites); }
                if (player.GetComponent<AllStatsUp.AllStatsTrackable>()) { AddSprites(ammonomiconPageRenderer, player, component3, player.GetComponent<AllStatsUp.AllStatsTrackable>().Stacks, AllStatsUp.AllStatsUpID, ref tk2DBaseSprites); }
            }

        }

        public static void AddSprites(AmmonomiconPageRenderer __instance, PlayerController playerController, dfPanel component3, int amount, int itemID, ref List<tk2dBaseSprite>  tk2DBaseSprites)
        {
            var pickup = PickupObjectDatabase.GetById(itemID);
            for (int i = 0; i < amount; i++)
            {
                tk2dClippedSprite tk2dClippedSprite3 = __instance.AddSpriteToPage<tk2dClippedSprite>(pickup.sprite.collection, pickup.sprite.spriteId);// playerController.passiveItems[m].sprite.spriteId);
                SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>(tk2dClippedSprite3, Color.black, 0.1f, 0.01f);
                tk2dClippedSprite3.transform.parent = component3.transform;
                tk2dClippedSprite3.transform.position = component3.GetCenter();
                tk2DBaseSprites.Add(tk2dClippedSprite3);
            }
        }




        public static bool BuildEquipmentPage(AmmonomiconPageRenderer ammonomiconPageRenderer)
        {
            if (SaveAPI.SaveAPIManager.GetFlag(SaveAPI.CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE))
            {
                var c = ammonomiconPageRenderer.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");
                if (c.Find("PerksTab") == null)
                {
                    var obj = AmmonomiconAPI.StaticData.HeaderObject;
                    var newObject = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ActiveItemsHeader, c);
                    newObject.name = "PerksTab";
                    var label = newObject.GetComponentInChildren<dfLabel>();
                    label.isLocalized = false;
                    label.localizationKey = "";
                    label.Text = "Perks";
                    var translator = label.gameObject.GetComponent<ConditionalTranslator>();
                    translator.enabled = false;

                    var lablePanel = newObject.GetComponent<dfPanel>();
                    lablePanel.ZOrder = 9;

                    var newObjectList = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsPanel, c);
                    newObjectList.name = "PerksPanel";
                    newObjectList.ZOrder = 10;
                }

                List<EncounterDatabaseEntry> list2 = new List<EncounterDatabaseEntry>();
                var player = AmmonomiconAPI.HelperTools.LastPlayerOpenedUI();
                if (player)
                {
                    if (player.GetComponent<GreedController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(Greedy.GreedyID), player.GetComponent<GreedController>().Stack); }
                    if (player.GetComponent<GunslingerController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(Gunslinger.GunslingerID), player.GetComponent<GunslingerController>().Stack); }
                    if (CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart > 0) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(Patience.PatienceID), CrossGameDataStorage.CrossGameStorage.AmountOfPerksToChooseFromOnRunStart - 1); }
                    if (player.GetComponent<AllSeeingEyeController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(AllSeeingEye.AllSeeingEyeID), player.GetComponent<AllSeeingEyeController>().Stacks); }
                    if (player.GetComponent<BlastProjectilesCheck>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(BlastProjectiles.BlastProjectilesID), player.GetComponent<BlastProjectilesCheck>().Stack); }
                    if (player.GetComponent<ContractController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(Contract.ContractID), player.GetComponent<ContractController>().Boys.Count); }
                    if (player.GetComponent<GlassComponent>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(Glass.GlassID), player.GetComponent<GlassComponent>().Stack); }
                    if (player.GetComponent<PitLordsPactController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(PitLordsPact.PitLordsPactID), player.GetComponent<PitLordsPactController>().Stacks); }
                    if (player.GetComponent<ChaoticShiftController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(ChaoticShift.ChaoticShiftID), player.GetComponent<ChaoticShiftController>().AmountOfModules); }
                    if (player.GetComponent<CorruptedWealthController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(CorruptedWealth.CorruptedWealthID), player.GetComponent<CorruptedWealthController>().StackCount); }
                    if (player.GetComponent<UnbreakableSpiritController>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(UnbreakableSpirit.UnbreakableSpiritID), player.GetComponent<UnbreakableSpiritController>().Stacks); }
                    if (player.GetComponent<AllStatsUp.AllStatsTrackable>()) { AddEntryFast(ref list2, PickupObjectDatabase.GetById(AllStatsUp.AllStatsUpID), player.GetComponent<AllStatsUp.AllStatsTrackable>().Stacks); }
                }
                if (list2.Count > 0)
                {
                    var panel = c.Find("PerksPanel").GetComponent<dfPanel>();
                    dfPanel component3 = panel.transform.GetChild(0).GetComponent<dfPanel>();
                    ammonomiconPageRenderer.StartCoroutine(ammonomiconPageRenderer.ConstructRectanglePageLayout(component3, list2, new Vector2(12f, 16f), new Vector2(8f, 8f), true, null));
                    panel.Anchor = (dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal);
                    panel.Height = component3.Height;
                    component3.Height = panel.Height;
                }

            }
            return true;
        }
        private static void AddEntryFast(ref List<EncounterDatabaseEntry> list, PickupObject pickupObject, int Count)
        {
            EncounterTrackable component4 = pickupObject.GetComponent<EncounterTrackable>();
            if (!component4.journalData.SuppressInAmmonomicon)
            {
                EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(component4.EncounterGuid);
                if (entry != null)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        list.Add(entry);
                    }
                }
            }
        }

        

        public class PerkAmmonomiconPageController : CustomAmmonomiconPageController
        {
            public PerkAmmonomiconPageController() : base("PERKS", 7, false, ""){}

            public override List<EncounterDatabaseEntry> GetEntriesForPage(AmmonomiconPageRenderer renderer)
            {
                List<KeyValuePair<int, PickupObject>> list = new List<KeyValuePair<int, PickupObject>>();
                for (int i = 0; i < PickupObjectDatabase.Instance.Objects.Count; i++)
                {
                    PickupObject pickupObject = PickupObjectDatabase.Instance.Objects[i];
                    if (pickupObject != null && (pickupObject is PerkPickupObject))
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
                return list2;
            }
            /*
            public override void InitializeItemsPageLeft(AmmonomiconPageRenderer self)
            {
                Transform transform = self.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");
                dfPanel component = transform.Find("Guns Panel").GetComponent<dfPanel>();
                dfPanel component3 = component.transform.GetChild(0).GetComponent<dfPanel>();

                

                self.StartCoroutine(self.ConstructRectanglePageLayout(component3, list2, new Vector2(12f, 20f), new Vector2(20f, 20f), false, null));
                component3.Anchor = (dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal);
                component.Height = component3.Height;
                component3.Height = component.Height;
            }
            */

            public override bool ShouldBeActive()
            {
                return SaveAPI.SaveAPIManager.GetFlag(SaveAPI.CustomDungeonFlags.TRESPASS_INTO_OTHER_PLACE);
            }
            public override void OnPageOpenedRight(AmmonomiconPageRenderer rightPage)
            {
                rightPage.SetPageDataUnknown(rightPage);
            }
            
        }
    }
}
