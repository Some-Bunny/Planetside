using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planetside
{
    class CustomLootTableInitialiser
    {
        public static void InitialiseCustomLootTables()
        {
            InitTimeTraderTable();
        }

        public static void InitTimeTraderTable()
        {
            GenericLootTable timeshopkeepertable = LootTableTools.CreateLootTable();
            //Guns
            timeshopkeepertable.AddItemsToPool(new Dictionary<int, float> { { 107, 1.5f }, { 210, 0.8f }, { 761, 0.8f }, { 516, 1 }, { 748, 1 }, { 395, 0.9f }, { 275, 1.2f }, { 52, 0.8f }, { 31, 1 }, { 6, 0.8f }, { 17, 1 }, { 327, 1 }, { 81, 0.8f }, { 360, 1 }, { 274, 0.7f }, { 198, 1.2f }, { 100, 0.8f }, { 362, 1.2f }, { 186, 1f }, { 363, 1f }, { 339, 1.1f }, { 130, 1 }, { 341, 2 }, { 33, 2 }, { 292, 2 }, { 90, 1.5f }, { 444, 1 }, { 474, 1 }, { 177, 1.3f }, { 3, 2 }, { 329, 1 } });
            //Items
            timeshopkeepertable.AddItemsToPool(new Dictionary<int, float> { { 448, 1 }, { 298, 1 }, { 447, 1 }, { 403, 1 }, { 253, 1 }, { 437, 2 }, { 259, 2 }, { 170, 1.5f }, { 306, 1.5f }, { 465, 1.5f }, { 138, 2 }, { 287, 2 }, { 489, 1 }, { 452, 1 }, { 533, 1.5f }, { 524, 1 }, { 521, 1 }, { 640, 1.2f }, { 630, 1.5f }, { 567, 1.5f }, { 457, 1 }, { 109, 1 }, { 219, 0.8f }, { 222, 0.8f }, { 463, 1 } });
            //Modded Items
            timeshopkeepertable.AddItemsToPool(new Dictionary<int, float> { { EcholocationAmmolet.EcholocationAmmoletID, 1 }, { DeathWarrant.DeathWarrantID, 1 }, { OffWorldMedicine.OffWorldMedicineID, 1 }, { AoEBullets.AuraBulletsID, 1.5f } });
            //TRoll
            //timeshopkeepertable.AddItemsToPool(new Dictionary<int, float> { { 348, 0.3f }, { 351, 0.3f }, { 349, 0.3f }, { 350, 0.3f } });
            TimeShopKeeperTable = timeshopkeepertable;
        }
        public static GenericLootTable TimeShopKeeperTable;
    }
}
