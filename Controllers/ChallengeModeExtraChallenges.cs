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
using static EnemyBulletBuilder.BulletBuilderFakePrefabHooks;
using ChallengeAPI;
using Brave.BulletScript;
using static HutongGames.PlayMaker.Actions.Teleport;
using Pathfinding;
using UnityEngine.Playables;


namespace Planetside
{
    public class ChallengeModeExtraChallenges : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Starting ChallengeModeExtraChallenges setup...");
            try
            {
                ChallengeBuilder.Init();
                ChallengeBuilder.EnableDebugMode();
                //-------------BUILDING CHALLENGES----------
                //Builds a basic challenge.
                /*
                ChallengeBuilder.BuildChallenge<BulletStormChallengeModifier>(PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("leadStorm"), "Lead Storm", true, new List<ChallengeModifier>
                {
                    ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<KingEnemyChallengeModifier>().challenge,
                    ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<BestForLastChallengeModifier>().challenge,
                }, null, null, true, true);
                
                ChallengeBuilder.BuildChallenge<ShamberBodyguard>(PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("shamberWatchman"), "Bullet Sponge", false, new List<ChallengeModifier> 
                {
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<KingEnemyChallengeModifier>().challenge,
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<BestForLastChallengeModifier>().challenge,

                }, null, null, true, true);
                */
                ChallengeBuilder.BuildChallenge<LandminesAhoy>(PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("Landmines"), "Watch Your Step", false, new List<ChallengeModifier>
                {
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<DarknessChallengeModifier>().challenge,
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<FlameTrapChallengeModifier>().challenge,
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<BooRoomChallengeModifier>().challenge,

                }, null, null, true, true);
                /*
                ChallengeBuilder.BuildChallenge<OopsAllLasers>(PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>("DiamondHeist"), "Diamond Heist", true, new List<ChallengeModifier>
                {
                     

                }, null, null, true, true);
                */
                Debug.Log("Finished ChallengeModeExtraChallenges setup without failure!");

            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ChallengeModeExtraChallenges setup!");
                Debug.Log(e);
            }
        }
    }

    public class OopsAllLasers: ChallengeModifier
    {
        public float ProfessionalChance = 0.25f;
        public void Start()
        {
            ProfessionalChance = UnityEngine.Random.value;
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            if (player.CurrentRoom != null)
            {
                var currentRoom = player.CurrentRoom;
                int num = currentRoom.CellsWithoutExits.Count / 60;
                num = Mathf.Max(5, num);
                CellValidator cellValidator = delegate (IntVector2 pos)
                {
                    for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                    {
                        if (Vector2.Distance(GameManager.Instance.AllPlayers[j].CenterPosition, pos.ToCenterVector2()) < 3f)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                int attempts = 250;
                for (int i = 0; i < num; i++)
                {
                    attempts--;
                    if (attempts == 0)
                        break;

                    IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
                    if (randomAvailableCell != null)
                    {
                        CellData cellData = GameManager.Instance.Dungeon.data[randomAvailableCell.Value];
                        CellData cellDataTop = GameManager.Instance.Dungeon.data[randomAvailableCell.Value + new IntVector2(0, 1)];
                        CellData cellDataLeft = GameManager.Instance.Dungeon.data[randomAvailableCell.Value + new IntVector2(1, 0)];
                        CellData cellDataRight = GameManager.Instance.Dungeon.data[randomAvailableCell.Value + new IntVector2(-1, 0)];


                        if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && cellDataTop.type == CellType.WALL)
                        {
                            cellData.containsTrap = true;

                            var r = Alexandria.DungeonAPI.StaticReferences.customPlaceables[UnityEngine.Random.value < ProfessionalChance ? "psog:professionalTurretFront" : "psog:sniperTurretFront"].InstantiateObject(currentRoom, (cellData.position - currentRoom.area.basePosition) + new IntVector2(0, 1));
                            r.gameObject.SetActive(true);
                            r.transform.SetParent(currentRoom.hierarchyParent, true);
                            continue;
                           
                        }
                        if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && cellDataLeft.type == CellType.WALL)
                        {
                            cellData.containsTrap = true;
                            var r = Alexandria.DungeonAPI.StaticReferences.customPlaceables[UnityEngine.Random.value < ProfessionalChance ? "psog:professionalTurretLeft" : "psog:sniperTurretLeft"].InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition);
                            r.gameObject.SetActive(true);
                            r.transform.SetParent(currentRoom.hierarchyParent, true);
        
                            continue;

                        }
                        if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && cellDataRight.type == CellType.WALL)
                        {
                            cellData.containsTrap = true;
                            var r = Alexandria.DungeonAPI.StaticReferences.customPlaceables[UnityEngine.Random.value < ProfessionalChance ? "psog:professionalTurretRight" : "psog:sniperTurretRight"].InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition);
                            r.gameObject.SetActive(true);
                            r.transform.SetParent(currentRoom.hierarchyParent, true); 
                            continue;

                        }
                        i--;
                    }
                }
            }
        }

        public void Update()
        { }

        public void OnDestroy()
        { }

        public override bool IsValid(RoomHandler room)
        {
            return true;
        }
    }

    public class LandminesAhoy : ChallengeModifier
    {
        public void Start()
        {
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            if (player.CurrentRoom != null)
            {
                var currentRoom = player.CurrentRoom;
                int num = currentRoom.CellsWithoutExits.Count / 150;
                num = Mathf.Min(6, Mathf.Max(1, num));
                CellValidator cellValidator = delegate (IntVector2 pos)
                {
                    for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                    {
                        if (Vector2.Distance(GameManager.Instance.AllPlayers[j].CenterPosition, pos.ToCenterVector2()) < 3f)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                for (int i = 0; i < num; i++)
                {
                    IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
                    if (randomAvailableCell != null)
                    {
                        CellData cellData = GameManager.Instance.Dungeon.data[randomAvailableCell.Value];
                        if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && !cellData.isOccupied && !cellData.containsTrap && !cellData.isOccludedByTopWall)
                        {
                            Alexandria.DungeonAPI.StaticReferences.customPlaceables["psog:randomminefieldsign"].InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition, false, false);
                        }
                    }
                }

                num = currentRoom.CellsWithoutExits.Count / 40;
                num = Mathf.Max(6, num);
                cellValidator = delegate (IntVector2 pos)
                {
                    for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                    {
                        if (Vector2.Distance(GameManager.Instance.AllPlayers[j].CenterPosition, pos.ToCenterVector2()) < 3f)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                for (int i = 0; i < num; i++)
                {
                    IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
                    if (randomAvailableCell != null)
                    {
                        CellData cellData = GameManager.Instance.Dungeon.data[randomAvailableCell.Value];
                        if (!cellData.isNextToWall && !cellData.HasPitNeighbor(GameManager.Instance.Dungeon.data))
                        {
                            if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && !cellData.isOccupied && !cellData.containsTrap && !cellData.isOccludedByTopWall)
                            {
                                cellData.containsTrap = true;
                                Alexandria.DungeonAPI.StaticReferences.customPlaceables["psog:buriedbasiclandmine_100"].InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition, false, false);
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        { }

        public void OnDestroy()
        { }

        public override bool IsValid(RoomHandler room)
        {
            return true;
        }
    }

    public class ShamberBodyguard : ChallengeModifier
    {
        public void Start()
        {
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            if (player.CurrentRoom != null)
            {
                var Enemy = EnemyDatabase.GetOrLoadByGuid("shamber_psog");
                Enemy.healthHaver.SetHealthMaximum(15000f);
                Enemy.reinforceType = AIActor.ReinforceType.SkipVfx;
                Enemy.CollisionDamage = 0;
                AIActor lad = AIActor.Spawn(Enemy.aiActor, player.CurrentRoom.GetRandomAvailableCell().Value, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
                lad.GetComponent<ShamberController>().Start();
                lad.aiActor.HasDonePlayerEnterCheck = true;
            }         
        }

        public void Update()
        {}

        public void OnDestroy()
        { }

        public override bool IsValid(RoomHandler room)
        {          
            return room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) > 2;
        }
    }
    public class BulletStormChallengeModifier : ChallengeModifier
    {
        public void Start(){ Living = new List<AIActor>();  }
        public static List<AIActor> Living;
        public void Update()
        {            
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            if (Living.Count() >= 3) { return; }
            this.elapsed += BraveTime.DeltaTime;

            if (this.elapsed > (3f + Living.Count()))
            {
                this.elapsed = 0;
                IntVector2 intVector2 = player.CurrentRoom.GetRandomAvailableCell().Value;
                GameObject gameObject = new GameObject();
                gameObject.transform.position = new Vector2?(intVector2.ToCenterVector2()).Value;
                BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
                gameObject.AddComponent<BulletSourceKiller>();
                var bulletScriptSelected = new CustomBulletScriptSelector(typeof(Skyfall));
                //(bulletScriptSelected.).bulletStormChallengeModifier = this;
                AIActor aIActor = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                AIBulletBank bulletBank = aIActor.GetComponent<AIBulletBank>();
                bulletBank.CollidesWithEnemies = false;
                source.BulletManager = bulletBank;
                source.BulletScript = bulletScriptSelected;
                source.Initialize();//to fire the script once
                Destroy(gameObject, 4);
            }
        }
      

        public void OnDestroy()
        {
            foreach (var entry in Living)
            {
                entry?.ForceDeath(Vector2.zero);
            }
            Living.Clear();
            //If you want something to happen when the challenge ends, this is the place.
        }

        public override bool IsValid(RoomHandler room)
        {
            return room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) > 2;
        }
        private float elapsed;
        public BulletStormChallengeModifier bulletStormChallengeModifier;
        public class Skyfall : Script
        {
            public override IEnumerator Top()
            {
                base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("big_one"));
                GameObject dragunRocket = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyRocket;
                foreach (Component item in dragunRocket.GetComponentsInChildren(typeof(Component)))
                {
                    if (item is SkyRocket rocket)
                    {
                        GameObject fart = SpawnManager.SpawnVFX(rocket.LandingTargetSprite, base.Position, Quaternion.identity);
                        fart.GetComponentInChildren<tk2dSprite>().UpdateZDepth();
                        tk2dSpriteAnimator componentInChildren = fart.GetComponentInChildren<tk2dSpriteAnimator>();
                        componentInChildren.Play(componentInChildren.DefaultClip, 0f, (float)componentInChildren.DefaultClip.frames.Length / 1, false);
                        Destroy(fart, 1.1f);
                    }
                }
                base.Fire(Offset.OverridePosition(base.Position + new Vector2(0f, 30f)), new Direction(-90f, DirectionType.Absolute, -1f), new Speed(30f, SpeedType.Absolute), new Skyfall.BigBullet());
                yield break;
            }
            private class BigBullet : Bullet
            {
                public BigBullet() : base("big_one", false, false, false)
                {
                }

                public override void Initialize()
                {
                    this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
                    base.Initialize();
                }

                public override IEnumerator Top()
                {
                  
                    base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.CollideWithOthers = false;
                    yield return base.Wait(60);
                    AkSoundEngine.PostEvent("Play_ENM_bulletking_slam_01", this.Projectile.gameObject);
                    this.Speed = 0f;
                    this.Projectile.spriteAnimator.Play();
                    base.Vanish(true);
                    yield break;
                }

                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (!preventSpawningProjectiles)
                    {
                        string guid = BraveUtility.RandomElement<string>(StaticInformation.ModderBulletGUIDs);
                        var Enemy = EnemyDatabase.GetOrLoadByGuid(guid);
                        AIActor das = AIActor.Spawn(Enemy.aiActor, this.Projectile.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.CurrentRoom, true, AIActor.AwakenAnimationType.Default, true);
                        Living.Add(das);
                        das.AddComponent<KillOnRoomClear>();
                        GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                        das.healthHaver.ForceSetCurrentHealth(22.5f * (lastLoadedLevelDefinition != null ? lastLoadedLevelDefinition.enemyHealthMultiplier : 1));
                        float num = base.RandomAngle();
                        for (int i = 0; i < 12; i++)
                        {base.Fire(new Direction(num + (30 * (float)i) + 10, DirectionType.Absolute, -1f), new Speed(3.5f, SpeedType.Absolute), new BurstBullet()); }
                        return;
                    }
                }
                public class BurstBullet : Bullet
                {
                    public BurstBullet() : base("reversible", false, false, false)
                    {
                    }
                    public override IEnumerator Top()
                    {
                        base.ChangeSpeed(new Speed(0f, SpeedType.Absolute), 60);
                        yield return base.Wait(60);
                        base.Vanish(false);

                        yield break;
                    }
                }
            }
        }
        
    }
}
