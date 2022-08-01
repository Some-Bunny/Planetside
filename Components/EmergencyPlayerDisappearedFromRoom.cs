
using GungeonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static GungeonAPI.ShrineFactory;
using Dungeonator;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Brave.BulletScript;
using System.Collections;
using SaveAPI;
using Gungeon;
using ItemAPI;
using System.Collections.ObjectModel;
using Pathfinding;

namespace Planetside
{
    public class EmergencyPlayerDisappearedFromRoom : MonoBehaviour
    {
        public EmergencyPlayerDisappearedFromRoom(bool ConsidersBoth = false)
        {
            ConsidersBothPlayers = ConsidersBoth;
        }
        public void Update()
        {
            if (GameManager.Instance)
            {
                if (ConsidersBothPlayers == true)
                {
                    PlayerController[] players = GameManager.Instance.AllPlayers;
                    foreach (PlayerController player in players)
                    {
                        if (player.CurrentRoom != roomAssigned && player != null)
                        {
                            if (PlayerSuddenlyDisappearedFromRoom != null)
                            {
                                this.PlayerSuddenlyDisappearedFromRoom(roomAssigned);
                            }
                        }
                    }
                }
                else
                {
                    PlayerController player = GameManager.Instance.PrimaryPlayer;
                    if (player.CurrentRoom != roomAssigned && player != null)
                    {
                        if (PlayerSuddenlyDisappearedFromRoom != null)
                        {
                            this.PlayerSuddenlyDisappearedFromRoom(roomAssigned);
                        }
                    }
                }
            }
        }
        public bool ConsidersBothPlayers;
        public RoomHandler roomAssigned;
        public Action<RoomHandler> PlayerSuddenlyDisappearedFromRoom;
    }
}
