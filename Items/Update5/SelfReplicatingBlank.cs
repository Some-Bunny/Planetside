using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BreakAbleAPI;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Planetside
{
	public class SelfReplicatingBlank : PassiveItem
	{

		public static void Init()
		{
			string name = "Self-Replicating Blank";
			string resourcePath = "Planetside/Resources/selfreplicatingblank.png";
			GameObject gameObject = new GameObject(name);
            SelfReplicatingBlank warVase = gameObject.AddComponent<SelfReplicatingBlank>();
			ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
			string shortDesc = "Osis";
			string longDesc = "Blanks used in combat will be refunded, as long as you don't get hit. Consecutive blanks have a lower chance to be refunded.\n\nThis blank is actually an extremely rare fungus, which is able to spread and multiply as long as its left to its own devices.";
			ItemBuilder.SetupItem(warVase, shortDesc, longDesc, "psog");
			warVase.quality = PickupObject.ItemQuality.A;
			warVase.IgnoredByRat = true;
			warVase.RespawnsIfPitfall = true;
            SelfReplicatingBlank.SelfReplicatingBlankID = warVase.PickupObjectId;
			warVase.BlanksUsed = 0;
            warVase.CanRefundBlanks = false;

            string shardDefaultPath = "Planetside/Resources/RandomDebris/SporeDebris/";
            string[] shardPaths1 = new string[]
            {
                shardDefaultPath+"sporebig.png",

            };
            string[] shardPaths2 = new string[]
            {
                shardDefaultPath+"sporemedium.png",
            };
            string[] shardPaths3 = new string[]
            {
                shardDefaultPath+"sporesmall.png",
            };

            DebrisObject shardObject = BreakableAPIToolbox.GenerateAnimatedWaftingDebrisObject(shardPaths1, Vector2.one, Vector2.zero, new Vector2(0.2f, 1.2f), 1, tk2dSpriteAnimationClip.WrapMode.Once, true, 5, 60, 150, 30, null, 1f);
            shardObject.name = "Spore_Debris_Large";
            DebrisObject shardObject2 = BreakableAPIToolbox.GenerateAnimatedWaftingDebrisObject(shardPaths2, Vector2.one, Vector2.zero, new Vector2(0.2f, 1.2f), 1, tk2dSpriteAnimationClip.WrapMode.Once, true, 5, 60, 120, 60, null, 0.8f);
            shardObject2.name = "Spore_Debris_Medium";
            DebrisObject shardObject3 = BreakableAPIToolbox.GenerateAnimatedWaftingDebrisObject(shardPaths3, Vector2.one, Vector2.zero, new Vector2(0.2f, 1.2f), 1, tk2dSpriteAnimationClip.WrapMode.Once, true, 5, 60, 180, 30, null, 0.5f);
            shardObject3.name = "Spore_Debris_Small";

            cluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] { shardObject, shardObject , shardObject, shardObject2, shardObject3 }, 0.35f, 1.2f, 8, 12, 0.8f);
        }

        public static ShardCluster cluster;

        public static int SelfReplicatingBlankID;

		public override void Pickup(PlayerController player)
		{
            player.OnUsedBlank += Player_OnUsedBlank;
            player.OnReceivedDamage += Player_OnReceivedDamage;
            player.OnRoomClearEvent += Player_OnRoomClearEvent;
            player.OnEnteredCombat += OnEnteredCombat;
            base.Pickup(player);
        }

		public int BlanksUsed;
        public bool CanRefundBlanks;
        public bool Succ = false;

        private void OnEnteredCombat(){CanRefundBlanks = true; }

        private void Player_OnRoomClearEvent(PlayerController obj)
		{
			if (CanRefundBlanks == true)
			{
				for (int i = 1; i < BlanksUsed + 1; i++)
				{
					float C = 1 / (float)i;
                    if (UnityEngine.Random.value < C)
					{
						obj.Blanks++;
					}
				}
			}
            GameManager.Instance.StartCoroutine(SuccMethod(this));
            BlanksUsed = 0;
            CanRefundBlanks = false;
        }
        public static IEnumerator SuccMethod(SelfReplicatingBlank self)
        {
            self.Succ = true;
            yield return new WaitForSeconds(3);
            self.Succ = false;
            yield break;
        }


        private void Player_OnReceivedDamage(PlayerController obj)
		{
            if (CanRefundBlanks == true)
            {
                for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
                {
                    DebrisObject debrisObject = StaticReferenceManager.AllDebris[i];
                    //ETGModConsole.Log(debrisObject.name);
                    if (debrisObject.name.Contains("Spore_Debris", true))
                    {
                        GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), debrisObject.sprite.WorldCenter, Quaternion.identity);
                        tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                        component.HeightOffGround = 35f;
                        component.UpdateZDepth();
                        tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                        if (component2 != null)
                        {
                            component.scale *= 0.5f;
                            component2.ignoreTimeScale = true;
                            component2.AlwaysIgnoreTimeScale = true;
                            component2.AnimateDuringBossIntros = true;
                            component2.alwaysUpdateOffscreen = true;
                            component2.playAutomatically = true;
                        }
                        Destroy(gameObject, 1.5f);
                        Destroy(debrisObject.gameObject, 0.1f);
                    }
                }
            }
            CanRefundBlanks = false;
            BlanksUsed = 0;
        }

		private void Player_OnUsedBlank(PlayerController arg1, int arg2)
		{
			if (CanRefundBlanks == false) { return; }
            SpawnSpores(arg1);
            BlanksUsed++;
        }

		public override DebrisObject Drop(PlayerController player)
		{
            player.OnUsedBlank -= Player_OnUsedBlank;
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            player.OnRoomClearEvent -= Player_OnRoomClearEvent;
            player.OnEnteredCombat -= OnEnteredCombat;

            DebrisObject result = base.Drop(player);
			return result;
		}


        protected override void Update()
        {
            if (base.Owner)
            {
                if (this.Succ == true)
                {
                    for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
                    {
                        this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[i], base.Owner);
                    }
                }
            }
           
        }

        

        private bool AdjustDebrisVelocity(DebrisObject debris, PlayerController player)
        {
            if (debris.IsPickupObject)
            {
                return false;
            }
            if (debris.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            if (!debris.name.Contains("Spore_Debris", true))
            {
                return false;
            }
            Vector2 a = debris.sprite.WorldCenter - player.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            float num2 = Mathf.Sqrt(num);
            if (num2 < 0.2f)
            {
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), debris.sprite.WorldCenter, Quaternion.identity);
                tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                component.HeightOffGround = 35f;
                component.UpdateZDepth();
                tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component.scale *= 0.5f;
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                }
                Destroy(gameObject, 1.5f);
                UnityEngine.Object.Destroy(debris.gameObject);
                return true;
            }
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, 8);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            if (debris.HasBeenTriggered)
            {
                debris.ApplyVelocity(frameAccelerationForRigidbody * d);
            }
            else if (num2 < 100 / 2f)
            {
                debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
            }
            return true;
        }

        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            float num = Mathf.Clamp01(1f - currentDistance / 100);
            float d = g * num * num;
            return ((base.Owner.specRigidbody.UnitCenter - unitCenter).normalized * d)*10;
        }


        public void SpawnSpores(PlayerController p)
        {
            Vector3 position = p.sprite.WorldCenter.ToVector3ZUp(p.sprite.WorldCenter.y);
            ShardCluster shardCluster = cluster;
            int num2 = UnityEngine.Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
            int num3 = UnityEngine.Random.Range(0, shardCluster.clusterObjects.Length);
            for (int j = 0; j < num2; j++)
            {
                float lowDiscrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(1);
                float z = Mathf.Lerp(-180, 180, lowDiscrepancyRandom);
                Vector3 vector = Quaternion.Euler(0f, 0f, z) * (MathToolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).normalized * UnityEngine.Random.Range(1, 10)).ToVector3ZUp(5);
                int num4 = (num3 + j) % shardCluster.clusterObjects.Length;
                GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[num4].gameObject, position, Quaternion.identity);
                tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                if (p.sprite.attachParent != null && component != null)
                {
                    component.attachParent = p.sprite.attachParent;
                    component.HeightOffGround = p.sprite.HeightOffGround;
                }
                DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
                vector = Vector3.Scale(vector, shardCluster.forceAxialMultiplier) * shardCluster.forceMultiplier;
                component2.Trigger(vector, 1, shardCluster.rotationMultiplier);
            }
        }


        protected override void OnDestroy()
		{
            if (base.Owner != null)
            {
                base.Owner.OnUsedBlank -= Player_OnUsedBlank;
                base.Owner.OnReceivedDamage -= Player_OnReceivedDamage;
                base.Owner.OnRoomClearEvent -= Player_OnRoomClearEvent;
                base.Owner.OnEnteredCombat -= OnEnteredCombat;
            }
            base.OnDestroy();
			
		}
	}
}
