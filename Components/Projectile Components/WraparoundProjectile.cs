using Dungeonator;
using Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class WraparoundProjectile : MonoBehaviour
    {
        private bool isIgnoringTillExit = false;
        private bool lastFrameCheck;
        private bool cachedMovementType;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            projectile.baseData.range *= RangeMultiplier;
            this.projectile.BulletScriptSettings = new BulletScriptSettings()
            {
                surviveTileCollisions = true
            };
            SpeculativeRigidbody specRigidbody = this.projectile.specRigidbody;
            specRigidbody.OnPostRigidbodyMovement += (spec, vec, intvec) =>
            {
                if (lastFrameCheck == true)
                {
                    lastFrameCheck = false;
                    isIgnoringTillExit = false;
                    projectile.m_usesNormalMoveRegardless = false;
                    projectile.IgnoreTileCollisionsFor(Mathf.Max(Time.deltaTime, (1f / (projectile.baseData.speed / Time.deltaTime)) * 5));
                }
            };


            specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(delegate (SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
            {


                if (isIgnoringTillExit == false)
                {
                    if (Warps < Cap)
                    {
                        isIgnoringTillExit = true;
                        projectile.UpdateCollisionMask();
                        PhysicsEngine.SkipCollision = true;
                        var dungeonData = GameManager.Instance.Dungeon.data;
                        CellData cell = dungeonData.cellData[tile.X][tile.Y];
                        if (cell.type == CellType.WALL)
                        {
                            if (dungeonData.isLeftSideWall(tile.X, tile.Y))
                            {
                                WoopShoop(this.projectile, Vector2.right);
                                return;
                            }
                            if (dungeonData.isRightSideWall(tile.X, tile.Y))
                            {
                                WoopShoop(this.projectile, Vector2.left);
                                return;
                            }
                            if (dungeonData.isFaceWallLower(tile.X, tile.Y) | dungeonData.isFaceWallHigher(tile.X, tile.Y))
                            {
                                WoopShoop(this.projectile, Vector2.down);
                                return;
                            }
                            if (dungeonData.isFaceWallLower(tile.X, tile.Y) | dungeonData.isWallDownRight(tile.X, tile.Y))
                            {
                                WoopShoop(this.projectile, Vector2.up);
                                return;
                            }
                        }
                    }


                }
                else
                {
                    lastFrameCheck = true;
                    PhysicsEngine.SkipCollision = true;
                }

            }));
        }


        private int Warps = 0;
        private float RangeMultiplier = 4;

        public void WoopShoop(Projectile p, Vector2 direction)
        {
            if (p == null) { return; }
            if (p.specRigidbody == null) { return; }
            Warps++;
            int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle);
            var cast = RaycastToolbox.ReturnRaycast(p.specRigidbody.UnitCenter + (direction * 0.5f), direction, rayMask, 1000, null);
            if (cast != null)
            {
                var Position = cast.Contact;
                if (p != null && Position != null)
                {
                    if (OnWrappedAround != null)
                    {
                        OnWrappedAround(p, p.transform.PositionVector2(), Position);
                    }
                    newPos = Position;

                    p.specRigidbody.transform.position = Position;
                    p.specRigidbody.Reinitialize();
                    p.specRigidbody.PullOutOfWalls(-direction.ToIntVector2());

                    if (p is HelixProjectile hp)
                    {
                        p.SendInDirection(dirVec: p.Direction, resetDistance: true, updateRotation: true);
                        hp.m_privateLastPosition = Position;
                        hp.m_timeElapsed = 0f;
                        hp.m_displacement = 0f;
                        hp.m_yDisplacement = 0f;
                        p.IgnoreTileCollisionsFor(0.1f);
                    }
                    else if (p.OverrideMotionModule is HelixProjectileMotionModule hpm)
                        p.SendInDirection(dirVec: hpm.m_initialRightVector.normalized, resetDistance: true, updateRotation: true);
                    else
                        p.SendInDirection(dirVec: p.Direction, resetDistance: true, updateRotation: true);

                }
            }
        }


        private Vector2 newPos;



        public void Update()
        {
            if (Warps >= Cap && this.projectile)
            {
                if (this.projectile.BulletScriptSettings != null)
                {
                    this.projectile.BulletScriptSettings.surviveTileCollisions = false;
                }
            }
            lastFrameCheck = false;
        }
        public Action<Projectile, Vector2, Vector2> OnWrappedAround;
        public int Cap = 1;
        private Projectile projectile;
    }

}
