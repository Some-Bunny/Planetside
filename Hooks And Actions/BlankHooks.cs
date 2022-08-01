using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using SaveAPI;
using Gungeon;
using ItemAPI;
using AnimationType = ItemAPI.BossBuilder.AnimationType;
using System.Collections;
using Brave.BulletScript;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using System.Collections.ObjectModel;

namespace Planetside
{
    public static class BlankHooks
	{
		public class TrackerComponentThatDestroysItself : MonoBehaviour
        {
			
        }
		

        public static void Init()
        {
            //new Hook(typeof(SilencerInstance).GetMethod("HandleSilence", BindingFlags.Instance | BindingFlags.NonPublic), typeof(BlankHooks).GetMethod("HandleSilenceHook"));
			//new Hook(typeof(SilencerInstance).GetMethod("DestroyBulletsInRange", BindingFlags.Static | BindingFlags.Public),  typeof(BlankHooks).GetMethod("DestroyBulletsInRangeHook", BindingFlags.Static | BindingFlags.Public));
		}
		public static IEnumerator HandleSilenceHook(Func<SilencerInstance, Vector2, Single, Single, Single, PlayerController, bool, IEnumerator> orig, SilencerInstance self, Vector2 centerPoint, float expandSpeed, float maxRadius, float additionalTimeAtMaxRadius, PlayerController user, bool shouldReflectInstead)
		{
			if (user != null)
            {
				if (user.GetComponent<CorruptedWealthController>() != null)
                {
					additionalTimeAtMaxRadius *= 5;
				}
            }
			IEnumerator origEnum = orig(self, centerPoint, expandSpeed, maxRadius, additionalTimeAtMaxRadius, user, shouldReflectInstead);
			while (origEnum.MoveNext())
			{
				object obj = origEnum.Current;
				yield return obj;
			}
			yield break;
		}

		public static List<Projectile> DestroyBulletsInRangeSpecial(Vector2 centerPoint, float radius, bool destroysEnemyBullets, bool destroysPlayerBullets, List<Projectile> projectilesToNotyeet, PlayerController user = null, bool reflectsBullets = false, float? previousRadius = null, bool useCallback = false, System.Action<Projectile> callback = null)
		{
			float num = radius * radius;
			float num2 = (previousRadius == null) ? 0f : (previousRadius.Value * previousRadius.Value);
			List<Projectile> list = new List<Projectile>();
			ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
			for (int i = 0; i < allProjectiles.Count; i++)
			{
				Projectile projectile = allProjectiles[i];
				if (projectile && projectile.sprite)
				{
					float sqrMagnitude = (projectile.sprite.WorldCenter - centerPoint).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						if (!projectile.ImmuneToBlanks)
						{
							if (previousRadius == null || !projectile.ImmuneToSustainedBlanks || sqrMagnitude >= num2)
							{
								if (projectile.Owner != null)
								{
									if (projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable)
									{
										if (!projectilesToNotyeet.Contains(projectile))
                                        {
											if (user != null)
											{
												if (user.GetComponent<CorruptedWealthController>() != null)
												{
													if (UnityEngine.Random.value < 0.25f)
													{

														SpawnManager.PoolManager.Remove(projectile.gameObject.transform);
														if (projectile.BulletScriptSettings != null)
														{
															projectile.BulletScriptSettings.preventPooling = true;
														}
														projectile.GetOrAddComponent<MarkForUndodgeAbleBullet>();
														projectilesToNotyeet.Add(projectile);
													}
													else
													{
														list.Add(projectile);
													}
												}
											}
											else
											{
												list.Add(projectile);
											}
										}
										
									}
									else if (projectile.Owner is PlayerController)
									{
										if (destroysPlayerBullets && projectile.Owner != user)
										{
											list.Add(projectile);
										}
									}
									else
									{
										Debug.LogError("Silencer is trying to process a bullet that is owned by something that is neither man nor beast!");
									}
								}
								else if (destroysEnemyBullets)
								{
									list.Add(projectile);
								}
							}
						}
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (!destroysPlayerBullets && reflectsBullets)
				{
					PassiveReflectItem.ReflectBullet(list[j], true, user, 10f, 1f, 1f, 0f);
				}
				else
				{
					if (list[j] && list[j].GetComponent<ChainLightningModifier>())
					{
						ChainLightningModifier component = list[j].GetComponent<ChainLightningModifier>();
						UnityEngine.Object.Destroy(component);
					}
					if (useCallback && callback != null)
					{
						callback(list[j]);
					}
					list[j].DieInAir(false, true, true, true);
				}
			}
			List<BasicTrapController> allTriggeredTraps = StaticReferenceManager.AllTriggeredTraps;
			for (int k = allTriggeredTraps.Count - 1; k >= 0; k--)
			{
				BasicTrapController basicTrapController = allTriggeredTraps[k];
				if (basicTrapController && basicTrapController.triggerOnBlank)
				{
					float sqrMagnitude2 = (basicTrapController.CenterPoint() - centerPoint).sqrMagnitude;
					if (sqrMagnitude2 < num)
					{
						basicTrapController.Trigger();
					}
				}
			}
			return projectilesToNotyeet;
		}


		public static void DestroyBulletsInRangeHook(Vector2 centerPoint, float radius, bool destroysEnemyBullets, bool destroysPlayerBullets, PlayerController user = null, bool reflectsBullets = false, float? previousRadius = null, bool useCallback = false, System.Action<Projectile> callback = null)
		{
			float num = radius * radius;
			float num2 = (previousRadius == null) ? 0f : (previousRadius.Value * previousRadius.Value);
			List<Projectile> list = new List<Projectile>();
			ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
			for (int i = 0; i < allProjectiles.Count; i++)
			{
				Projectile projectile = allProjectiles[i];
				if (projectile && projectile.sprite)
				{
					float sqrMagnitude = (projectile.sprite.WorldCenter - centerPoint).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						if (!projectile.ImmuneToBlanks)
						{
							if (previousRadius == null || !projectile.ImmuneToSustainedBlanks || sqrMagnitude >= num2)
							{
								if (projectile.Owner != null)
								{
									if (projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable)
									{
										if (destroysEnemyBullets)
										{
											if (user != null)
                                            {
												if (user.GetComponent<CorruptedWealthController>() != null)
												{

												}
											}
											else
                                            {
												list.Add(projectile);
											}
										}
									}
									else if (projectile.Owner is PlayerController)
									{
										if (destroysPlayerBullets && projectile.Owner != user)
										{
											list.Add(projectile);
										}
									}
									else
									{
										Debug.LogError("Silencer is trying to process a bullet that is owned by something that is neither man nor beast!");
									}
								}
								else if (destroysEnemyBullets)
								{
									list.Add(projectile);
								}
							}
						}
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (!destroysPlayerBullets && reflectsBullets)
				{
					PassiveReflectItem.ReflectBullet(list[j], true, user, 10f, 1f, 1f, 0f);
				}
				else
				{
					if (list[j] && list[j].GetComponent<ChainLightningModifier>())
					{
						ChainLightningModifier component = list[j].GetComponent<ChainLightningModifier>();
						UnityEngine.Object.Destroy(component);
					}
					if (useCallback && callback != null)
					{
						callback(list[j]);
					}
					list[j].DieInAir(false, true, true, true);
				}
			}
			List<BasicTrapController> allTriggeredTraps = StaticReferenceManager.AllTriggeredTraps;
			for (int k = allTriggeredTraps.Count - 1; k >= 0; k--)
			{
				BasicTrapController basicTrapController = allTriggeredTraps[k];
				if (basicTrapController && basicTrapController.triggerOnBlank)
				{
					float sqrMagnitude2 = (basicTrapController.CenterPoint() - centerPoint).sqrMagnitude;
					if (sqrMagnitude2 < num)
					{
						basicTrapController.Trigger();
					}
				}
			}
		}
	}
}
    

