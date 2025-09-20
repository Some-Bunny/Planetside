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
using GungeonAPI;
using SaveAPI;
using HutongGames.PlayMaker.Actions;


namespace Planetside
{
    public class HellShrinesController : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting HellShrinesController setup...");
            try
            {
                DungeonHooks.OnPostDungeonGeneration += this.StartHandleShrineSpawns;
                Debug.Log("Finished HellShrinesController setup without failure!");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish HellShrinesController setup!");
                Debug.Log(e);
            }
        }

        public static List<string> potentialShrines = new List<string>()
        {
            "shrineofdarkness",
            "shrineofcurses",
            "shrineofpetrification",
            "shrineofsomething"
        };


        public static List<Vector2> offsets = new List<Vector2>()
        {
            new Vector2(5.0625f,  5.0625f),
            new Vector2(-6.0625f,  -6.0625f),
            new Vector2(5.0625f,  -6.0625f),
            new Vector2(-6.0625f, 5.0625f),
        };

        public static List<string> blackListedRoomNames = new List<string>()
        {
            "Hell Entrance",
            "Boss Foyer",
            "LichRoom01",
            "LichRoom02",
            "LichRoom03",
            "BigDumbIdiotBossRoom1.room",
        };

        private void StartHandleShrineSpawns()
        {
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH) == false) { return; }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                try
                {
                    potentialShrines.Shuffle();
                    offsets.Shuffle();
                    for (int i = 0; i < 4; i++)
                    {
                        SpawnSpecificCurseShrine(potentialShrines[i], offsets[i]);
                    }
                    PlaceOtherHellShrines();
                }
                catch
                {
                    ETGModConsole.Log("Catastrophic Failure In Placing Curse Shrines! Send A Screenshot of this and associated error in F3 Console.");
                }
            }
        }

        public bool ProcessRoom(RoomHandler room)
        {
            if (room == null) { return false; }
            if (room.GetRoomName() == null) { return false; }
            if (blackListedRoomNames.Contains(room.GetRoomName())) { return false; }
            if (room.IsSecretRoom == true) { return false; }
            //ETGModConsole.Log(room.GetRoomName() + " IS VALID");
            return true;
        }


        private void PlaceOtherHellShrines()
        {
            try
            {
                List<RoomHandler> list = new List<RoomHandler>();
                list.AddRange(GameManager.Instance.Dungeon.data.rooms); 
                for (int i = 0; i < list.Count; i++)
                {
                    if (ProcessRoom(list[i]) == false) { list.Remove(list[i]); }
                }
                list.Shuffle();

                //ETGModConsole.Log("Starting loading this");
                int attempts = 12;
                for (int i = 0; i < 4; i++)
                {
                    attempts--;
                    if (attempts == 0) { Debug.Log("[PSOG] ATTEMPTS HAVE RUN DRY, ABORTING FURTHER PLACEMENT"); return; }

                    if (list.Count == 0 || list == null) { Debug.Log("[PSOG] HELL ROOM LIST IS DRY, ABORTING FURTHER PLACEMENT"); return; }
                    RoomHandler room = list[i];
                    if (room == null) { Debug.Log("[PSOG] ROOM IS NULL, ABORTING FURTHER PLACEMENT"); return; }

                    string name = room.GetRoomName();
                    if (room.IsStandardRoom && name != "Hell Entrance" && name != "Boss Foyer" && name != "LichRoom01" && name != "LichRoom02" && name != "LichRoom03" && name != "BigDumbIdiotBossRoom1.room")
                    {

                        //ETGModConsole.Log("selected room: "+ room.GetRoomName());

                        IntVector2 randomVisibleClearSpot = room.GetRandomVisibleClearSpot(3, 3);
                        if (randomVisibleClearSpot != IntVector2.Zero)
                        {

                            GameObject original;
                            ShrineFactory.registeredShrines.TryGetValue("psog:shrineofpurity", out original);
                            GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3((float)randomVisibleClearSpot.x, (float)randomVisibleClearSpot.y), Quaternion.identity);
                            gObj.gameObject.AddComponent<PurityShrineController>();
                            //ETGModConsole.Log("PLACED SHRINE OF PURITY");
                        }
                    }
                    else
                    {
                        list.Remove(list[i]);
                        //ETGModConsole.Log("Removing room: " + room.GetRoomName()+ " because it is INVALID!");
                        i--;
                    }
                }
            }
            catch
            {
                ETGModConsole.Log("Catastrophic Failure In Placing Purity Shrines! Send A Screenshot of this and associated error in F3 Console.");
            }
        }


        public class PurityShrineController : BraveBehaviour
        {
            public void Start()
            {
                Trigger = false;
                base.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

                GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, new Vector3(base.transform.position.x + 1f, base.transform.position.y, base.transform.position.z + 5f), Quaternion.Euler(0f, 0f, 0f));
                MeshRenderer component3 = gameObject1.GetComponent<MeshRenderer>();
                base.StartCoroutine(this.HoldPortalOpen(component3));

                component3.material.SetTexture("_PortalTex", StaticTextures.NebulaTexture);
                hole = component3;

                tk2dSprite sprite = this.GetComponent<tk2dSprite>();
                sprite.transform.localScale = Vector3.zero;


                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);

            
            }
            private IEnumerator HoldPortalOpen(MeshRenderer component)
            {
                float elapsed = 0f;
                while (component != null)
                {
                    if (Trigger == true) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = Mathf.Clamp01(elapsed / 0.25f);
                    component.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0f, 0.25f, t));
                    yield return null;
                }

                yield break;
            }
            private IEnumerator ClosePortal(MeshRenderer portal) 
            {
                tk2dSprite sprite = this.gameObject.GetComponent<tk2dSprite>();
                AkSoundEngine.PostEvent("Play_BOSS_spacebaby_charge_01", this.gameObject);
                Material material = sprite.renderer.material;

                float elapsed = 0f;
                float duration = 2;
                while (elapsed < duration)
                {
                   
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / duration;
                    sprite.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

                    float t2 = Mathf.Clamp01(elapsed / 1.25f);
                    portal.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.25f - (elapsed / 25f), 0f, t2));
                    yield return null;
                }


                AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", this.gameObject);

                IPlayerInteractable[] interfaces = this.gameObject.GetInterfaces<IPlayerInteractable>();
                for (int j = 0; j < interfaces.Length; j++)
                {
                    this.transform.position.GetAbsoluteRoom().RegisterInteractable(interfaces[j]);
                }
                UnityEngine.Object.Destroy(portal.gameObject);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);

                base.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));
                material.shader = Shader.Find("Brave/PlayerShader");

                yield break;
            }

            public void Update()
            {
                RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (absoluteRoom == player.CurrentRoom && !player.IsInCombat && Trigger == false)
                    {
                        Trigger = true;
                        base.StartCoroutine(this.ClosePortal(hole));
                    }
                }               
            }
            MeshRenderer hole;
            bool Trigger;
        }




        public void SpawnSpecificCurseShrine(string shrineName, Vector2 offset)
        {
            RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
            IntVector2 randomVisibleClearSpot = entrance.GetCenterCell();
            GameObject original;
            ShrineFactory.registeredShrines.TryGetValue("psog:" + shrineName, out original);
            GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(original, ((Vector2)randomVisibleClearSpot) + offset, Quaternion.identity);
            gObj.AddComponent<HellShrineController>();
            IPlayerInteractable[] interfaces = gObj.GetInterfaces<IPlayerInteractable>();
            for (int j = 0; j < interfaces.Length; j++)
            {
                entrance.RegisterInteractable(interfaces[j]);
            }      
        }




        public class HellShrineController : BraveBehaviour
        {
            public void Start()
            {
                roomSelf = this.gameObject.transform.position.GetAbsoluteRoom();
                EmergencyPlayerDisappearedFromRoom emergencyPlayerDisappearedFrom = this.AddComponent<EmergencyPlayerDisappearedFromRoom>();
                emergencyPlayerDisappearedFrom.ConsidersBothPlayers = true;
                emergencyPlayerDisappearedFrom.roomAssigned = roomSelf;
                emergencyPlayerDisappearedFrom.PlayerSuddenlyDisappearedFromRoom += PlayerDisappersFromRoom;
            }

           public void PlayerDisappersFromRoom(RoomHandler room)
           {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (player.IsInCombat)
                    {
                        tk2dSpriteAnimator animator = this.GetComponent<tk2dSpriteAnimator>();
                        animator.Play("use");

                        EmergencyPlayerDisappearedFromRoom emergencyPlayerDisappearedFrom = this.GetComponent<EmergencyPlayerDisappearedFromRoom>();
                        emergencyPlayerDisappearedFrom.PlayerSuddenlyDisappearedFromRoom = null;
                        LootEngine.DoDefaultPurplePoof(this.transform.position, false);
                        try
                        {
                            SimpleShrine shrine = base.gameObject.GetComponent<SimpleShrine>();
                            shrine.text = "The spirits that have once inhabited the shrine have departed.";
                            shrine.OnAccept = null;
                            shrine.OnDecline = null;
                            shrine.acceptText = "Leave, with style.";
                            shrine.declineText = "Leave.";
                            shrine.CanUse = CanUse;
                        }
                        catch
                        {
                            ETGModConsole.Log("Failure in modifying shrines (1)");
                        }
                    }
                }

                
           }       
            public static bool CanUse(PlayerController player, GameObject shrine)
            {
                return false;
            }
            public RoomHandler roomSelf;
        }
    }
}
