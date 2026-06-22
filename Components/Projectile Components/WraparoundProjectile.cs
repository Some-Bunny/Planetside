using Dungeonator;
using Planetside;
using Planetside.DungeonPlaceables;
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
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            if (projectile == null) { return; }
            if (projectile.specRigidbody == null) { return; }

            projectile.baseData.range *= RangeMultiplier;
            this.projectile.BulletScriptSettings = new BulletScriptSettings()
            {
                surviveTileCollisions = true
            };
            SpeculativeRigidbody specRigidbody = this.projectile.specRigidbody;
            this.projectile.specRigidbody.OnPostRigidbodyMovement += (spec, vec, intvec) =>
            {
                if (lastFrameCheck == true)
                {
                    lastFrameCheck = false;
                    isIgnoringTillExit = false;
                    projectile.m_usesNormalMoveRegardless = false;
                    projectile.IgnoreTileCollisionsFor(Mathf.Max(Time.deltaTime, (1f / (projectile.baseData.speed / Time.deltaTime)) * 5));
                }
            };


            this.projectile.specRigidbody.OnPreTileCollision += (SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider) =>
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
            };

            this.projectile.specRigidbody.OnPreRigidbodyCollision += (SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherBody, PixelCollider tilePixelCollider) =>
            {
                if (otherBody != null)
                {

                    if (isIgnoringTillExit == false)
                    {
                        if (Warps < Cap)
                        {
                            isIgnoringTillExit = true;
                            projectile.UpdateCollisionMask();

                            var p = projectile.LastVelocity.normalized * -1;
                            p.x = Mathf.RoundToInt(p.x);
                            p.y = Mathf.RoundToInt(p.y);

                            WoopShoop(projectile, p);

                            var c = otherBody.GetComponentsInParent(typeof(Component));
                            foreach (var c2 in c)
                            {
                                if (c2 is DungeonDoorController door)
                                {



                                    //RelAngleTo((-tileCollision.Normal).ToAngle(), this._LastVelocity.ToAngle()) > 0;

                                    foreach (var d in door.doorModules)
                                    {
                                        projectile.specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                                    }
                                    foreach (var c_1 in door.GetComponentsInChildren<SpeculativeRigidbody>())
                                    {
                                        projectile.specRigidbody.RegisterTemporaryCollisionException(c_1, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                                    }
                                    PhysicsEngine.SkipCollision = true;
                                    break;
                                }
                            }
                            c = otherBody.GetComponents(typeof(Component));
                            foreach (var c2 in c)
                            {
                                if (c2 is FireplaceController fireplace)
                                {
                                    projectile.specRigidbody.RegisterTemporaryCollisionException(fireplace.specRigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 2);
                                    PhysicsEngine.SkipCollision = true;
                                    break;
                                }

                                if (c2 is MajorBreakable breakable)
                                {
                                    if (breakable.IsSecretDoor)
                                    {
                                        projectile.specRigidbody.RegisterTemporaryCollisionException(breakable.specRigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 2);
                                        PhysicsEngine.SkipCollision = true;
                                        break;

                                    }

                                }

                                if (c2 is DungeonDoorSubsidiaryBlocker blocker)
                                {
                                    projectile.specRigidbody.RegisterTemporaryCollisionException(blocker.parentDoor.specRigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 2);
                                    foreach (var d in blocker.parentDoor.doorModules)
                                    {
                                        projectile.specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 2);
                                    }
                                    PhysicsEngine.SkipCollision = true;
                                    break;

                                }
                                if (c2 is ForgeCrushDoorController crusher)
                                {
                                    if (crusher.m_isCrushing)
                                    {

                                        PhysicsEngine.SkipCollision = true;
                                        projectile.specRigidbody.RegisterTemporaryCollisionException(crusher.specRigidbody, projectile.LastVelocity.magnitude * BraveTime.DeltaTime * 2);
                                        PhysicsEngine.SkipCollision = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        public static float RelAngleTo(float angle, float other)
        {
            return BraveMathCollege.ClampAngle180((other - angle));
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
                    if (cast.SpeculativeRigidbody != null)
                    {
                        //prevent collision with door objects temporarily
                        var c = cast.SpeculativeRigidbody.GetComponentsInParent(typeof(Component));
                        foreach (var c2 in c)
                        {
                            if (c2 is DungeonDoorController door)
                            {
                                foreach (var d in door.doorModules)
                                {
                                    p.specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, p.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                                }
                                foreach (var c_1 in door.GetComponentsInChildren<SpeculativeRigidbody>())
                                {
                                    p.specRigidbody.RegisterTemporaryCollisionException(c_1, p.LastVelocity.magnitude * BraveTime.DeltaTime * 3);
                                }
                                break;
                            }
                        }
                        c = cast.SpeculativeRigidbody.GetComponents(typeof(Component));
                        foreach (var c2 in c)
                        {
                            if (c2 is FireplaceController fireplace)
                            {
                                p.specRigidbody.RegisterTemporaryCollisionException(fireplace.specRigidbody, 0.3f);
                                break;
                            }

                            if (c2 is MajorBreakable breakable)
                            {
                                if (breakable.IsSecretDoor)
                                {
                                    p.specRigidbody.RegisterTemporaryCollisionException(breakable.specRigidbody, 0.3f);
                                    break;

                                }

                            }

                            if (c2 is DungeonDoorSubsidiaryBlocker blocker)
                            {
                                p.specRigidbody.RegisterTemporaryCollisionException(blocker.parentDoor.specRigidbody, 0.3f);
                                foreach (var d in blocker.parentDoor.doorModules)
                                {
                                    p.specRigidbody.RegisterTemporaryCollisionException(d.rigidbody, 0.3f);
                                }
                                break;

                            }
                            if (c2 is ForgeCrushDoorController crusher)
                            {
                                if (crusher.m_isCrushing)
                                {

                                    PhysicsEngine.SkipCollision = true;
                                    p.specRigidbody.RegisterTemporaryCollisionException(crusher.specRigidbody, 0.3f);
                                    break;
                                }
                            }
                        }

                    }

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
