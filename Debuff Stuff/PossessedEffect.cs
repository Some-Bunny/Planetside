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
	public class PossessedEffect : GameActorHealthEffect
	{
		public static string vfxNameposessed = "PosessedVFX";
		public static GameObject posessedVFXObject;
		public Color TintColorPosessed = new Color(3f, 2f, 0f, 0.75f);
		public Color ClearUp = new Color(0f, 0f, 0f, 0f);


        public static GameObject BuildVFX()
        {
            var debuffCollection = StaticSpriteDefinitions.VFX_Sheet_Data;
            var BrokenArmorVFXObject = ItemBuilder.AddSpriteToObjectAssetbundle("Possessed", debuffCollection.GetSpriteIdByName("possesedeffect_idle_005"), debuffCollection);//new GameObject("Broken Armor");//SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/Debuffs/brokenarmor", new GameObject("BrokenArmorEffect"));
            FakePrefab.MarkAsFakePrefab(BrokenArmorVFXObject);
            UnityEngine.Object.DontDestroyOnLoad(BrokenArmorVFXObject);
            BrokenArmorVFXObject.GetOrAddComponent<tk2dBaseSprite>();
            tk2dSpriteAnimator animator = BrokenArmorVFXObject.GetOrAddComponent<tk2dSpriteAnimator>();
            var clip = SpriteBuilder.AddAnimation(animator, debuffCollection, new List<int>()
            {
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_001"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_002"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_003"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_004"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_005"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_006"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_007"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_008"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_009"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_010"),
                debuffCollection.GetSpriteIdByName("possesedeffect_idle_011"),

            }, "start", tk2dSpriteAnimationClip.WrapMode.LoopSection, 12);

            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            clip.loopStart = 3;
            posessedVFXObject = BrokenArmorVFXObject;
            return posessedVFXObject;
        }

    
		public override void ApplyTint(GameActor actor)
		{
			actor.RegisterOverrideColor(TintColorPosessed, vfxNameposessed);

            heatIndicatorController = (UnityEngine.Object.Instantiate(StaticVFXStorage.RadialRing, actor.sprite.WorldCenter, Quaternion.identity, actor.transform)).GetComponent<HeatIndicatorController>();
            heatIndicatorController.CurrentColor = Color.yellow.WithAlpha(3f);
            heatIndicatorController.IsFire = false;
            heatIndicatorController.CurrentRadius = QuickRadius(actor);
            heatIndicatorController.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));


        }
        private HeatIndicatorController heatIndicatorController;

        public float QuickDamage(GameActor actor)
        {
            float Damage = actor.healthHaver != null ? actor.healthHaver.maximumHealth / 6.25f : 6.25f;
            Damage = Mathf.Max(3, Damage);
            Damage = Mathf.Min(20, Damage);
            return Damage;
        }

        public float QuickRadius(GameActor actor)
        {
            float Damage = QuickDamage(actor);
            return Damage * 0.625f;
        }

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			List<AIActor> activeEnemies = actor.GetAbsoluteParentRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
			Vector2 centerPosition = actor.sprite.WorldCenter;
            if (heatIndicatorController)
            {
                heatIndicatorController.transform.position = heatIndicatorController.transform.position.WithZ(100);
            }

            if (activeEnemies != null)
			{
				for (int i = activeEnemies.Count - 1; i > -1; i--)
				{
					var aiactor = activeEnemies[i];
					if (Vector2.Distance(aiactor.CenterPosition, centerPosition) < QuickRadius(actor) && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null)
					{
						aiactor.healthHaver.ApplyDamage(QuickDamage(actor) * BraveTime.DeltaTime, Vector2.zero, "oooo spoooooky", CoreDamageTypes.Void, DamageCategory.Normal, false, null, false);
					}
				}
			}
		}


		public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
		{
			base.OnEffectRemoved(actor, effectData);
			actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
            GameManager.Instance.StartCoroutine(KillRing(heatIndicatorController));
		}

        public IEnumerator KillRing(HeatIndicatorController myheatIndicatorController) 
        {
            float t = 0;
            float g = myheatIndicatorController.CurrentRadius;
            heatIndicatorController.transform.SetParent(null, true);
            while (t < 1)
            {
                t += Time.deltaTime * 5;
                myheatIndicatorController.CurrentRadius = Mathf.Lerp(g, 0, t);
               yield return null;
            }
            UnityEngine.Object.Destroy(myheatIndicatorController.gameObject);
            yield break;
        }
	}
}
