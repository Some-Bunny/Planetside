using Dungeonator;
using ItemAPI;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Planetside
{
    class CustomLootTableInitialiser
    {
        public static void InitialiseCustomLootTables()
        {
            InitTimeTraderTable();
            InitTablertTable();

        }

        public static void InitTimeTraderTable()
        {
            
            TimeShopKeeperTable = LootTableTools.CreateLootTable();
            //Guns
            TimeShopKeeperTable.AddItemsToPool(new Dictionary<int, float> { { Guns.Raiden_Coil.PickupObjectId, 1.2f }, { Guns.Gunbow.PickupObjectId, 0.8f }, { Guns.High_Kaliber.PickupObjectId, 0.8f }, { Guns.Triple_Gun.PickupObjectId, 1 }, { Guns.Sunlight_Javelin.PickupObjectId, 1 }, { Guns.Staff_Of_Firepower.PickupObjectId, 0.9f }, { Guns.Flare_Gun.PickupObjectId, 1.2f }, { Guns.Crescent_Crossbow.PickupObjectId, 0.8f }, { Guns.Klobbe.PickupObjectId, 1 }, { Guns.Zorgun.PickupObjectId, 0.8f }, { Guns.Heck_Blaster.PickupObjectId, 1 }, { 327, 1 }, { 81, 0.8f }, { 360, 1 }, { 274, 0.7f }, { 198, 1.2f }, { 100, 0.8f }, { 362, 1.2f }, { 186, 1f }, { 363, 1f }, { 339, 1.1f }, { 130, 1 }, { Guns.Buzzkill.PickupObjectId, 1.5f }, { Guns.Tear_Jerker.PickupObjectId, 1.5f }, { Guns.Molotov_Launcher.PickupObjectId, 1.33f }, { Guns.Eye_Of_The_Beholster.PickupObjectId, 1.33f }, { 444, 1 }, { 474, 1 }, { 177, 1.3f }, { 3, 1.5f }, { 329, 1 } });
            //Items
            TimeShopKeeperTable.AddItemsToPool(new Dictionary<int, float> { { 448, 1 }, { 298, 1 }, { 447, 1 }, { 403, 1 }, { 253, 1 }, { 437, 1.2f }, { 259, 1.2f }, { 170, 1.5f }, { 306, 1.5f }, { 465, 1.5f }, { 138, 2 }, { 287, 2 }, { 489, 1 }, { 452, 1 }, { 533, 1.5f }, { 524, 1 }, { 521, 1 }, { 640, 1.2f }, { 630, 1.5f }, { 567, 1.5f }, { 457, 1 }, { 109, 1 }, { 219, 0.8f }, { 222, 0.8f }, { 463, 1 } });
            //Modded Items
            TimeShopKeeperTable.AddItemsToPool(new Dictionary<int, float> { { DiasukesPolymorphine.DiasukesPolymorphineID, 0.8f }, { CHROMA.ItemID, 0.5f }, { NeutroniumCore.ID, 1.2f }, { EcholocationAmmolet.EcholocationAmmoletID, 1 }, { DeathWarrant.DeathWarrantID, 1 }, { OffWorldMedicine.OffWorldMedicineID, 1 }, { AoEBullets.AuraBulletsID, 1.2f } });
            //TRoll
            //timeshopkeepertable.AddItemsToPool(new Dictionary<int, float> { { 348, 0.3f }, { 351, 0.3f }, { 349, 0.3f }, { 350, 0.3f } });
        }
        
        public static void InitTablertTable()
        {
            TalbertKeeperTable = LootTableTools.CreateLootTable();
            //Guns
            TalbertKeeperTable.AddItemsToPool(new Dictionary<int, float> { { 396, 0.9f }, { 397, 1f }, { 398, 1f }, { 399, 0.8f }, { 400, 0.5f }, { 465, 1f }, { 666, 1f }, { 633, 0.8f } });
            TalbertKeeperTable.AddItemsToPool(new Dictionary<int, float> { { TableTechDevour.TableTechDevourID, 0.66f }, { TableTechNullReferenceException.TableTechNullID, 1 }, { TableTechTelefrag.TableTechTelefragID, 0.8f }});
            TalbertKeeperTable.AddItemsToPool(new Dictionary<int, float> { { 644, 0.5f } });

            TalbertKeeperTable.AddItemsToPool(new Dictionary<int, float> { { 294, 0.6f }, { 293, 0.2f }, { 664, 0.2f } });
        }

		public static GenericLootTable TalbertKeeperTable;
        public static GenericLootTable TimeShopKeeperTable;
    }
}
