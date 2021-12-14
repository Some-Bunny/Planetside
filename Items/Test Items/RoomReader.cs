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

using UnityEngine.Serialization;
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;

using System.IO;
using Planetside;
using FullInspector.Internal;

namespace Planetside
{
    public class RoomReader : PlayerItem
    {
        public static void Init()
        {
            string itemName = "RoomReader";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            RoomReader testActive = obj.AddComponent<RoomReader>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Reads Rooms";
            string longDesc = "Prints The Name Of The Current Room Into The F2 Console";
            testActive.SetupItem(shortDesc, longDesc, "psog");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            testActive.consumable = false;
            ItemBuilder.AddPassiveStatModifier(testActive, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        

        protected override void DoEffect(PlayerController user)
        {

            //AkSoundEngine.PostEvent("Play_BossTheme", user.gameObject);

            ETGModConsole.Log(user.CurrentRoom.GetRoomName());




            Dungeon sewerDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");

            GameObject obj = null;

            PrototypeDungeonRoom asset = null;
            foreach (var bundle in StaticReferences.AssetBundles.Values)
            {
                asset = bundle.LoadAsset<PrototypeDungeonRoom>("ChallengeShrine_Gungeon_002");
                if (asset)
                    break;
            }



            obj = asset.placedObjects[0].nonenemyBehaviour.gameObject;
            ChallengeShrineController controller =  obj.GetOrAddComponent<ChallengeShrineController>();
            //controller.ConfigureOnPlacement(user.CurrentRoom);

            //FieldInfo m_parentRoom = controller.GetType().GetField("m_parentRoom", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_parentRoom.SetValue(controller, user.CurrentRoom);

            //RoomHandler room = ReflectionHelper.ReflectGetField<RoomHandler>(typeof(ChallengeShrineController), "m_parentRoom", controller);
            //room = user.CurrentRoom;



            //user.CurrentRoom.RegisterInteractable(obj.GetComponent<IPlayerInteractable>());
            /*
            int PlaceableInt = 11;
            string RoomName = "blacksmith_testroom";
            foreach (DungeonFlow flows in sewerDungeon.PatternSettings.flows)
            {
                foreach (DungeonFlowNode node in flows.AllNodes)
                {
                    if (node.overrideExactRoom != null)
                    {
                        if (node.overrideExactRoom.name.ToLower().StartsWith(RoomName))
                        {
                            ETGModConsole.Log("Found via node.overrideExactRoom");
                            obj = node.overrideExactRoom.placedObjects[PlaceableInt].nonenemyBehaviour.gameObject;

                        }
                    }
                }
            }

            foreach (WeightedRoom wRoom in sewerDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
            {
                if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                {
                    if (wRoom.room.name.ToLower().StartsWith(RoomName))
                    {
                        ETGModConsole.Log("Found via PatternSettings");
                        obj = wRoom.room.placedObjects[PlaceableInt].nonenemyBehaviour.gameObject;
                    }
                }
            }
            */
            if (obj != null)
             
            {
                GameObject obj1 = DungeonPlaceableUtility.InstantiateDungeonPlaceable(obj, user.CurrentRoom, GameManager.Instance.PrimaryPlayer.CurrentRoom.area.UnitCenter.ToIntVector2(), false);
                obj1.transform.position = user.transform.position;

                Component[] componentsInChildren = obj1.GetComponentsInChildren(typeof(IPlayerInteractable));
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    IPlayerInteractable placeConfigurable = componentsInChildren[i] as IPlayerInteractable;
                    if (placeConfigurable != null)
                    {
                        user.CurrentRoom.RegisterInteractable(placeConfigurable);
                    }
                }

                //UnityEngine.Object.Instantiate<GameObj.poect>(obj, user.specRigidbody.UnitCenter, Quaternion.identity);
            }
        }
    }
}



