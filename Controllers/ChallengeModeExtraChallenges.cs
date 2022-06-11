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
                //ChallengeBuilder.EnableDebugMode();
                //-------------BUILDING CHALLENGES----------
                //Builds a basic challenge.

                ChallengeBuilder.BuildChallenge<BulletStormChallengeModifier>("Planetside/Resources/ChallengeModeIcons/leadStorm.png", "Lead Storm", true, new List<ChallengeModifier>
                {
                    ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<KingEnemyChallengeModifier>().challenge,
                    ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<BestForLastChallengeModifier>().challenge,
                }, null, null, true, true);
                
                ChallengeBuilder.BuildChallenge<ShamberBodyguard>("Planetside/Resources/ChallengeModeIcons/shamberWatchman.png", "Bullet Sponge", false, new List<ChallengeModifier> 
                {
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<KingEnemyChallengeModifier>().challenge,
                     ChallengeBuilder.ChallengeManagerPrefab.FindChallenge<BestForLastChallengeModifier>().challenge,

                }, null, null, true, true);
                
            }
            catch (Exception e)
            {
                Debug.Log("Unable to finish ChallengeModeExtraChallenges setup!");
                Debug.Log(e);
            }
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
                lad.GetComponent<ShamberController>().Invoke("Start", 0);
                lad.aiActor.HasDonePlayerEnterCheck = true;
            }         
        }

        public void Update()
        {}

        public void OnDestroy()
        { }

        public override bool IsValid(RoomHandler room)
        {          
            return room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) < 2 ? false : true;
        }
    }
    public class BulletStormChallengeModifier : ChallengeModifier
    {
        public void Start()
        {



        }
        public void Update()
        {            
            PlayerController player = GameManager.Instance.PrimaryPlayer;
            this.elapsed += BraveTime.DeltaTime;
            if (this.elapsed > 2f)
            {
                this.elapsed = 0;
                IntVector2 intVector2 = player.CurrentRoom.GetRandomAvailableCell().Value;
                GameObject gameObject = new GameObject();
                gameObject.transform.position = new Vector2?(intVector2.ToCenterVector2()).Value;
                BulletScriptSource source = gameObject.GetOrAddComponent<BulletScriptSource>();
                gameObject.AddComponent<BulletSourceKiller>();
                var bulletScriptSelected = new CustomBulletScriptSelector(typeof(Skyfall));
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
            //If you want something to happen when the challenge ends, this is the place.
        }

        public override bool IsValid(RoomHandler room)
        {
            return room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) < 2 ? false : true;
        }
        private float elapsed;

        public class Skyfall : Script
        {
            protected override IEnumerator Top()
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
                ETGModConsole.Log("5");

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

                protected override IEnumerator Top()
                {
                  
                    base.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                    this.Projectile.specRigidbody.CollideWithTileMap = false;
                    this.Projectile.specRigidbody.CollideWithOthers = false;
                    yield return base.Wait(60);
                    base.PostWwiseEvent("Play_ENM_bulletking_slam_01", null);
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
                        das.AddComponent<KillOnRoomClear>();
                        das.healthHaver.ForceSetCurrentHealth(10);
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
                    protected override IEnumerator Top()
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
