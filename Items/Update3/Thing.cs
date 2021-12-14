using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;


namespace Planetside
{
    public class Thing : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Blue Casing";
            string resourceName = "Planetside/Resources/bluecasing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Thing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used For... Something?";
            string longDesc = "An unusually blue casing." +
                "\n\nMaybe something might want it?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;
            BlueCasingID = item.PickupObjectId;

        }
        public static int BlueCasingID;


        public override DebrisObject Drop(PlayerController player)
		{
            GungeonAPI.DungeonHooks.OnPostDungeonGeneration -= this.PlaceColoredShrines;
            GameManager.Instance.OnNewLevelFullyLoaded -= this.EnableSW;
            DebrisObject result = base.Drop(player);
			return result;
		}
		public override void Pickup(PlayerController player)
		{
            GungeonAPI.DungeonHooks.OnPostDungeonGeneration += this.PlaceColoredShrines;
            GameManager.Instance.OnNewLevelFullyLoaded += this.EnableSW;
            base.Pickup(player);
		}
        private void EnableSW()
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.EnableSW;
            TheMeagaSomethingWickedEnabler dark = base.Owner.gameObject.AddComponent<TheMeagaSomethingWickedEnabler>();
            dark.player = base.Owner;
        }
        private void DisableSW()
        {
            ExtantShrines.Clear();
            GameManager.Instance.OnNewLevelFullyLoaded -= this.DisableSW;
            if (base.Owner.gameObject.GetComponent<TheMeagaSomethingWickedEnabler>() && base.Owner != null)
            {
                TheMeagaSomethingWickedEnabler SW = base.Owner.gameObject.GetComponent<TheMeagaSomethingWickedEnabler>();
                Destroy(SW);
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            }
        }
        protected override void OnDestroy()
		{
            GungeonAPI.DungeonHooks.OnPostDungeonGeneration -= this.PlaceColoredShrines;
            GameManager.Instance.OnNewLevelFullyLoaded -= this.EnableSW;
            base.OnDestroy();
		}

        public static List<GameObject> ExtantShrines = new List<GameObject>();
        void PlaceColoredShrines()
        {
            bool flag = GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.RATGEON && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON;
            if (flag)
            {
                bool Enrty = false;
                bool Leavle = false;
                try
                {

                    RoomHandler roomHandlerentry = GameManager.Instance.Dungeon.data.Entrance;
                    RoomHandler roomHandlerexit = GameManager.Instance.Dungeon.data.Exit;
                    if (roomHandlerentry != null && Enrty != true)
                    {
                        IntVector2 randomVisibleClearSpot = roomHandlerentry.GetCenterCell();
                        bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                        if (flag4)
                        {
                            GameObject original;
                            GungeonAPI.OldShrineFactory.builtShrines.TryGetValue("psog:redshrine", out original);
                            GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x - 0.75f, (float)randomVisibleClearSpot.y - 5), Quaternion.identity);
                            IPlayerInteractable[] interfaces1 = gObj.GetInterfaces<IPlayerInteractable>();
                            IPlaceConfigurable[] interfaces21 = gObj.GetInterfaces<IPlaceConfigurable>();
                            RoomHandler roomHandler21 = roomHandlerentry;
                            for (int j = 0; j < interfaces1.Length; j++)
                            {
                                roomHandler21.RegisterInteractable(interfaces1[j]);
                            }
                            for (int k = 0; k < interfaces21.Length; k++)
                            {
                                interfaces21[k].ConfigureOnPlacement(roomHandler21);
                            }

                            Enrty = true;
                        }
                    }
                    if (roomHandlerexit != null && Leavle != true)
                    {
                        IntVector2 randomVisibleClearSpot = roomHandlerexit.GetCenterCell();
                        bool flag4 = randomVisibleClearSpot != IntVector2.Zero;
                        if (flag4)
                        {
                            GameObject original1;
                            GungeonAPI.OldShrineFactory.builtShrines.TryGetValue("psog:blueshrine", out original1);
                            GameObject gObj1 = UnityEngine.Object.Instantiate<GameObject>(original1, new Vector3((float)randomVisibleClearSpot.x - 0.75f, (float)randomVisibleClearSpot.y - 1), Quaternion.identity);
                            IPlayerInteractable[] interfaces1 = gObj1.GetInterfaces<IPlayerInteractable>();
                            IPlaceConfigurable[] interfaces21 = gObj1.GetInterfaces<IPlaceConfigurable>();
                            RoomHandler roomHandler21 = roomHandlerexit;
                            for (int j = 0; j < interfaces1.Length; j++)
                            {
                                roomHandler21.RegisterInteractable(interfaces1[j]);
                            }
                            for (int k = 0; k < interfaces21.Length; k++)
                            {
                                interfaces21[k].ConfigureOnPlacement(roomHandler21);
                            }
                            Leavle = true;
                            ExtantShrines.Add(gObj1);
                        }
                    }
                }
                catch (Exception e)
                {
                    ETGModConsole.Log("Catastrophic Failure In Placing Color Shrines! Send A Screenshot of this and associated error in F3 Console.");
                    ETGModConsole.Log(e.ToString());
                }
            }
        }
    }

}

namespace Planetside
{
    public class RedThing : PassiveItem
    {

        public static void Init()
        {
            string itemName = "Red Casing";
            string resourceName = "Planetside/Resources/redcasing.png";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RedThing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Used For... Something?";
            string longDesc = "An unusually red casing." +
                "\n\nYou do not feel comfortable holding it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
            item.CanBeDropped = false;
            RedCasingID = item.PickupObjectId;

        }
        public static int RedCasingID;


        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject result = base.Drop(player);
            return result;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}


namespace Planetside
{
    public class TheMeagaSomethingWickedEnabler : MonoBehaviour
    {
        public TheMeagaSomethingWickedEnabler()
        {


        }
        public static OverridableBool shouldBeDark = new OverridableBool(false);
        public static OverridableBool shouldBeLightOverride = new OverridableBool(false);
        public PlayerController player;
        private bool FloorChanged;
        private bool IsHollow;
        private void Start()
        {
            IsHollow = false;
            bool flag = GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.RATGEON && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON;
            if (flag)
            {
                Minimap.Instance.PreventAllTeleports = true;
                Minimap.Instance.TemporarilyPreventMinimap = true;
                NevernamedsDarknessHandler.EnableDarkness(0, 0);
                GameManager.Instance.StartCoroutine(DelaySW());
                IsHollow = true;
                FloorChanged = false;
                GameManager.Instance.OnNewLevelFullyLoaded += this.DisableSW;
                GameObject ChallengeManagerReference = LoadHelper.LoadAssetFromAnywhere<GameObject>("_ChallengeManager");
                DarknessEffectShader = (ChallengeManagerReference.GetComponent<ChallengeManager>().PossibleChallenges[5].challenge as DarknessChallengeModifier).DarknessEffectShader;
                List<RoomHandler> rooms = GameManager.Instance.Dungeon.data.rooms;
                foreach (RoomHandler roomHandler in rooms)
                {
                    try
                    {
                        roomHandler.ClearReinforcementLayers();
                        List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                        if (activeEnemies != null && activeEnemies.Count > 0)
                        {
                            for (int i = 0; i < activeEnemies.Count; i++)
                            {
                                roomHandler.DeregisterEnemy(activeEnemies[i], false);
                                UnityEngine.Object.Destroy(activeEnemies[i].gameObject);
                            }
                        }
                        ReadOnlyCollection<IPlayerInteractable> yes = roomHandler.GetRoomInteractables();
                        foreach (TeleporterController touchy in yes)
                        {
                            roomHandler.DeregisterInteractable(touchy);
                            touchy.enabled = false;
                            Minimap minimap = Minimap.Instance;
                            Dictionary<RoomHandler, GameObject> rTTIM = ReflectionHelper.ReflectGetField<Dictionary<RoomHandler, GameObject>>(typeof(Minimap), "roomToTeleportIconMap", minimap);
                            rTTIM.Clear();
                            Dictionary<RoomHandler, GameObject> rTIM = ReflectionHelper.ReflectGetField<Dictionary<RoomHandler, GameObject>>(typeof(Minimap), "roomToIconsMap", minimap);
                            rTIM.Clear();
                            minimap.roomsContainingTeleporters.Remove(roomHandler);
                            Minimap.Instance.DeregisterRoomIcon(roomHandler, touchy.gameObject);
                        }

                        BaseShopController[] componentsInChildren = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<BaseShopController>(true);
                        bool flag3 = componentsInChildren != null && componentsInChildren.Length != 0;
                        if (flag3)
                        {
                            foreach (BaseShopController shope in componentsInChildren)
                            {
                                List<ShopItemController> shopitem = ReflectionHelper.ReflectGetField<List<ShopItemController>>(typeof(BaseShopController), "m_itemControllers", shope);
                                for (int i = 0; i < shopitem.Count; i++)
                                {
                                    if (shopitem[i])
                                    {
                                        shopitem[i].CurrentPrice = 0;
                                        Destroy(shopitem[i]);
                                    }
                                }
                                Destroy(shope.gameObject);
                                if (shope.OptionalMinimapIcon)
                                {
                                    Minimap.Instance.DeregisterRoomIcon(roomHandler, shope.OptionalMinimapIcon);
                                }
                            }
                        }

                        GunberMuncherController[] muncher = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<GunberMuncherController>(true);
                        bool muncjyflag = muncher != null && muncher.Length != 0;
                        if (muncjyflag)
                        {
                            foreach (GunberMuncherController shope in muncher)
                            {
                                Destroy(shope.gameObject);
                            }
                        }
                        SellCellController[] sellcreep = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<SellCellController>(true);
                        bool sellcreepflag = sellcreep != null && sellcreep.Length != 0;
                        if (sellcreepflag)
                        {
                            foreach (SellCellController shope in sellcreep)
                            {
                                Destroy(shope.gameObject);
                            }
                        }

                        TalkDoerLite[] talkers = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<TalkDoerLite>(true);
                        bool talkersflag = talkers != null && talkers.Length != 0;
                        if (talkersflag)
                        {
                            foreach (TalkDoerLite shope in talkers)
                            {
                                Destroy(shope.gameObject);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void DisableSW()
        {
            if (this != null)
            {
                NevernamedsDarknessHandler.DisableDarkness(0);
                Minimap.Instance.TemporarilyPreventMinimap = false;
                FloorChanged = true;
                GameManager.Instance.OnNewLevelFullyLoaded -= this.DisableSW;
                if (player.gameObject.GetComponent<TheMeagaSomethingWickedEnabler>() != null && player != null)
                {
                    Pixelator.Instance.AdditionalCoreStackRenderPass = null;
                    this.ResetMusic(GameManager.Instance.Dungeon);
                    AkSoundEngine.PostEvent(this.m_cachedMusicEventCore, GameManager.Instance.gameObject);
                    if (this != null)
                    {
                        Destroy(this);
                    }
                }
            }
        }

        public void ResetMusic(Dungeon d)
        {
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            if (flag)
            {
                this.m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                this.m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
        }
        private string m_cachedMusicEventCore;

        private void OnDestroy()
        {
            if (Pixelator.Instance.AdditionalCoreStackRenderPass != null)
            {
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;
            }
        }

        private IEnumerator DelaySW()
        {
            yield return new WaitForSeconds(10f);
            if (FloorChanged != true)
            {
                Vector2 Point1 = MathToolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), 180f);
                UnityEngine.Object.Instantiate<GameObject>(SomethingWickedEnemy.SomethingWickedObject, player.gameObject.transform.PositionVector2() - Point1, Quaternion.identity);
                AkSoundEngine.PostEvent("Play_SomethingWickedThisWayComes", base.gameObject);
                yield return new WaitForSeconds(5f);
                OtherTools.Notify("Something Wicked", "This Way Comes.", "Planetside/Resources/ShrineIcons/DarknessIcon");
            }
            yield break;
        }

        public void RemoveEnemies(AIActor AIActor)
        {
            bool flag = AIActor && AIActor.aiActor && AIActor.aiActor.EnemyGuid != null;
            if (flag)
            {
                string text;
                if (AIActor == null)
                {
                    text = null;
                }
                else
                {
                    AIActor aiActor = AIActor.aiActor;
                    text = ((aiActor != null) ? aiActor.EnemyGuid : null);
                }
                string text2 = text;
                bool flag2 = !string.IsNullOrEmpty(text2);
                if (flag2)
                {
                    try
                    {
                        if (AIActor.gameObject.GetComponent<SpawnEnemyOnDeath>())
                        {
                            UnityEngine.Object.Destroy(AIActor.gameObject.GetComponent<SpawnEnemyOnDeath>());
                        }
                        Destroy(AIActor.gameObject);
                    }
                    catch (Exception e)
                    {
                        ETGModConsole.Log(e.ToString());
                    }
                }
            }
        }
       
        private void Update()
        {
            if(IsHollow == true)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                if (NevernamedsDarknessHandler.IsItDark() == false){NevernamedsDarknessHandler.EnableDarkness(0, 0);}
            }
        }
        public static bool isDark = false;
        public static Shader DarknessEffectShader;
    }
}