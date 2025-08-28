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
using UnityEngine.Video;
using static FullInspector.Internal.fiLateBindings;
using UnityEngine.Audio;
using Alexandria.DungeonAPI;


namespace Planetside
{

   

    public class Flowder : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Flowder";
            string resourceName = "Planetside/Resources/blashshower.png";
            GameObject obj = new GameObject(itemName);
            Flowder testActive = obj.AddComponent<Flowder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Flows Loads";
            string longDesc = "Loads a specifie debug flow";
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
        public override void DoEffect(PlayerController user)
        {
            UnityEngine.Object.Instantiate(LoadHelper.LoadAssetFromAnywhere<GameObject>("npc_lostadventurer"), user.transform.position, Quaternion.identity);

            //GameObject beetle = UnityEngine.Object.Instantiate(PlanetsideModule.ModAssets.LoadAsset<GameObject>("Centaur_Midpoly_Riggeda_0"));
            //beetle.transform.position = user.transform.position;

            /*
            MeshRenderer renderer = beetle.GetComponentInChildren<MeshRenderer>();
            renderer.allowOcclusionWhenDynamic = true;
            beetle.transform.localScale = Vector3.one;
            beetle.name = "ShopPortal";
            beetle.transform.localScale *= 2;
            */
            //beetle.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            //beetle.transform.rotation = Quaternion.Euler(300, 180, 180);// 180, 180);
            //GlobalMessageRadio.BroadcastMessage("eye_shot_1");
            //GameManager.Instance.LoadCustomFlowForDebug("NPCParadise", "Base_Castle", "tt_castle");
        }
    }
}



