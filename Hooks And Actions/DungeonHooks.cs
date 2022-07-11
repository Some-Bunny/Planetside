using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Planetside
{ 
    public class MasteryReplacementOub : DungeonDatabase
    {
        public static void InitDungeonHook()
        {
            
            Hook GetOrLoadByNameHook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(MasteryReplacementOub).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );
            

        }
        public static Dungeon GetOrLoadByNameHook(Func<string, Dungeon> orig, string name)
        {
            Dungeon dungeon = null;
            if (name.ToLower() == "base_cathedral")
            {
                dungeon = AbbeyDungeonMods(GetOrLoadByName_Orig(name));
            }
            else if (name.ToLower() == "base_sewer")
            {
                dungeon = SewerDungeonMods(GetOrLoadByName_Orig(name));
            }
            else if (name.ToLower() == "base_nakatomi")
            {
                dungeon = RNGDungeonMods(GetOrLoadByName_Orig(name));
            }
            if (dungeon)
            {
                DebugTime.RecordStartTime();
                DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
                return dungeon;
            }
            else
            {
                return orig(name);
            }
        }

        public static Dungeon GetOrLoadByName_Orig(string name)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
            DebugTime.RecordStartTime();
            Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeon>();
            DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[] { name });
            return component;
        }

        public static Dungeon AbbeyDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            dungeon.BossMasteryTokenItemId = ForgottenRoundAbbey.ForgottenRoundAbbeyID; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
        public static Dungeon SewerDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            var finalMasteryRewardOub = ForgottenRoundOubliette.ForgottenRoundOublietteID;
            dungeon.BossMasteryTokenItemId = finalMasteryRewardOub; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
        public static Dungeon RNGDungeonMods(Dungeon dungeon)
        {
            // Here is where you'll do your mods to existing Dungeon prefab	
            var finalMasteryRewardOub = ForgottenRoundRNG.ForgottenRoundRNGID;
            dungeon.BossMasteryTokenItemId = finalMasteryRewardOub; // Item ID for Third Floor Master Round. Replace with Item ID of your choosing. 
            return dungeon;
        }
    }
}