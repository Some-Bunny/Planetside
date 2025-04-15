using Alexandria.PrefabAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside.DungeonPlaceables
{
    public class MarkTileAsTrap : MonoBehaviour
    {
        public static void Init()
        {
            GameObject marker = PrefabBuilder.BuildObject("MarkTileAsTrap");
            marker.CreateFastBody(new IntVector2(24, 24), new IntVector2(-4, -4), CollisionLayer.Trap, false, true);
            marker.AddComponent<MarkTileAsTrap>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("PSOG_MarkTileAsTrap", marker);
        }

        public void Start()
        {
            RoomHandler R = this.transform.position.GetAbsoluteRoom();
            //R.ce
            PixelCollider primaryPixelCollider = this.GetComponent<SpeculativeRigidbody>().PrimaryPixelCollider;
            IntVector2 intVector = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2 = PhysicsEngine.PixelToUnitMidpoint(primaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor);
            //Debug.Log(intVector + " : " + intVector2);
            for (int i = intVector.x; i <= intVector2.x; i++)
            {
                for (int j = intVector.y; j <= intVector2.y; j++)
                {
                    if (GameManager.Instance.Dungeon.data.cellData[i][j] != null)
                    {
                        var cell = GameManager.Instance.Dungeon.data.cellData[i][j];
                        cell.containsTrap = true;
                        cell.isOccupied = true;
                        cell.isExitCell = true;
                        //UnityEngine.Object.Instantiate(StaticVFXStorage.FriendlyElectricLinkVFX, new Vector3(i, j), Quaternion.identity);
                    }
                }
            }
            Destroy(this.gameObject);
        }
    }
}
