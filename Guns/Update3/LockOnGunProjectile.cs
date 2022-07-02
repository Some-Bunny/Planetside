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

namespace Planetside
{
	public class LockOnGunProjectile : MonoBehaviour
	{
		public LockOnGunProjectile()
		{
            this.AngualrVelocity = 360;

        }

        public float AngualrVelocity;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            PlayerController playerController = this.projectile.Owner as PlayerController;
            if (this.projectile)
            {
                try
                {
                    bool flagA = playerController.PlayerHasActiveSynergy("No Virus Included");
                    if (flagA)
                    {
                        AngualrVelocity = 720;
                    }
                    Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                    mat.mainTexture = this.projectile.sprite.renderer.material.mainTexture;
                    mat.SetColor("_EmissiveColor", new Color32(107, 255, 135, 255));
                    mat.SetFloat("_EmissiveColorPower", 1.55f);
                    mat.SetFloat("_EmissivePower", 100);
                    this.projectile.sprite.renderer.material = mat;

                    ImprovedAfterImage yes = this.projectile.gameObject.AddComponent<ImprovedAfterImage>();
                    yes.spawnShadows = true;
                    yes.shadowLifetime = 0.8f;
                    yes.shadowTimeDelay = 0.01f;
                    yes.dashColor = new Color(0.2f, 1f, 0.55f, 0.01f);
                    yes.name = "Gun Trail";
                    GameManager.Instance.StartCoroutine(this.Speed(this.projectile, this.projectile.baseData.speed));
                }
                catch (Exception ex)
                {
                    ETGModConsole.Log(ex.Message, false);
                }
            }
        }
        public float GetAimDirection(Vector2 position, float leadAmount, float speed)
        {
            Vector2 vector = LockOnGun.LockOnInstance.transform.position;
            Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector, LockOnGun.LockOnInstance.transform.position, position, speed);
            vector = new Vector2(vector.x + (predictedPosition.x - vector.x) * leadAmount, vector.y + (predictedPosition.y - vector.y) * leadAmount);
            return (vector - position).ToAngle();
        }
        public IEnumerator Speed(Projectile projectile, float baseSpeed)
        {
            bool flag = projectile != null;
            bool flag3 = flag;
            if (flag3)
            {
                float speed = baseSpeed / 20;
                for (int i = 0; i < 20; i++)
                {
                    if (projectile)
                    {
                        projectile.baseData.speed -= speed;
                        projectile.UpdateSpeed();
                        yield return new WaitForSeconds(0.02f);
                    }
                    else
                    {
                        yield break;
                    }
                }
                yield return new WaitForSeconds(0.2f);
                if (LockOnGun.LockOnInstance != null && projectile != null)
                {                    
                    projectile.SendInDirection((LockOnGun.LockOnInstance.transform.position - projectile.transform.position), false, true);
                    projectile.ModifyVelocity = (Func<Vector2, Vector2>)Delegate.Combine(projectile.ModifyVelocity, new Func<Vector2, Vector2>(this.ModifyVelocity));
                }
                for (int i = 0; i < 20; i++)
                {
                    if (projectile)
                    {
                        projectile.baseData.speed += speed;
                        projectile.UpdateSpeed();
                        yield return new WaitForSeconds(0.005f);
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            yield break;
        }

        private Vector2 ModifyVelocity(Vector2 inVel)
        {
            Vector2 vector = inVel;
            RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(projectile.LastPosition.IntXY(VectorConversions.Floor));
            if (LockOnGun.LockOnInstance == null || LockOnGun.LockOnInstance.transform.position == null)
            {
                return inVel;
            }
            float num = float.MaxValue;
            Vector2 vector2 = Vector2.zero;
            Vector2 b = (!projectile.sprite) ? projectile.transform.position.XY() : projectile.sprite.WorldCenter;
            Vector2 vector3 = LockOnGun.LockOnInstance.transform.PositionVector2() - b;
            float sqrMagnitude = vector3.sqrMagnitude;
            if (sqrMagnitude < num)
            {
                vector2 = vector3;
                num = sqrMagnitude;
            }
            num = Mathf.Sqrt(num);
            if (LockOnGun.LockOnInstance != null)
            {
                float num2 = 1f - num / 1000;
                float target = vector2.ToAngle();
                float num3 = inVel.ToAngle();
                float maxDelta = AngualrVelocity * num2 * projectile.LocalDeltaTime;
                float num4 = Mathf.MoveTowardsAngle(num3, target, maxDelta);
                if (projectile is HelixProjectile)
                {
                    float angleDiff = num4 - num3;
                    (projectile as HelixProjectile).AdjustRightVector(angleDiff);
                }
                else
                {
                    if (projectile.shouldRotate)
                    {
                        projectile.transform.rotation = Quaternion.Euler(0f, 0f, num4);
                    }
                    vector = BraveMathCollege.DegreesToVector(num4, inVel.magnitude);
                }
                if (projectile.OverrideMotionModule != null)
                {
                    projectile.OverrideMotionModule.AdjustRightVector(num4 - num3);
                }
            }
            if (vector == Vector2.zero || float.IsNaN(vector.x) || float.IsNaN(vector.y))
            {
                return inVel;
            }
            return vector;
        }
        private Projectile projectile;
	}
}

