using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using GungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Planetside;
using BreakAbleAPI;
using System.Collections;
using SaveAPI;
using Alexandria.PrefabAPI;

namespace Planetside
{

    public class HMPrimeBatteryController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public MajorBreakable self;
        public NoteDoer noteSelf;

        public void Start()
        {
            self = this.gameObject.GetComponent<MajorBreakable>();
            noteSelf = base.gameObject.GetComponent<NoteDoer>();
            noteSelf.Start();

            /*
            base.gameObject.GetComponent<tk2dBaseSprite>().usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = base.gameObject.GetComponent<tk2dBaseSprite>().renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            base.gameObject.GetComponent<tk2dBaseSprite>().renderer.material = mat;
            */
            if (self != null)
            {
                RoomHandler instanceRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                instanceRoom.RegisterInteractable(this);
                instanceRoom.RegisterInteractable(this.GetComponent<NoteDoer>());

                base.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
              
            }
        }

        private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {

            if (clip.GetFrame(frameIdx).eventInfo.Contains("BigBoomSoon"))
            {
                GameManager.Instance.StartCoroutine(SpawnRing(base.sprite.WorldCenter));
            }
            if (clip.GetFrame(frameIdx).eventInfo.Contains("DIE"))
            {
                GameUIRoot.Instance.ShowCoreUI(string.Empty);
                TextBoxManager.ClearTextBox(noteSelf.textboxSpawnPoint);

                GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
                epicwin.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(base.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
                epicwin.transform.position = base.sprite.WorldCenter.Quantize(0.0625f);
                epicwin.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                Destroy(epicwin, 8);
                AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", base.gameObject);

                ExplosionData defaultSmallExplosionData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
                defaultSmallExplosionData.damageRadius = 6;
                defaultSmallExplosionData.damage = 1000;
                Exploder.Explode(base.sprite.WorldCenter, defaultSmallExplosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
                Destroy(base.gameObject);
            }
        }

        private IEnumerator SpawnRing(Vector2 centre)
        {
            float elapsed = 0f;
            float duration = 2f;
            HeatIndicatorController ringObject = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), centre, Quaternion.identity)).GetComponent<HeatIndicatorController>();
            ringObject.IsFire = false;
            ringObject.CurrentRadius = 0;
            ringObject.gameObject.transform.parent = base.gameObject.transform;
            while (elapsed < duration)
            {
                if (ringObject.gameObject == null) { break; }
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                ringObject.CurrentRadius = Mathf.Lerp(0, 5f, throne1);
                ringObject.CurrentColor = Color.green.WithAlpha(Mathf.Lerp(0, 75, throne1));

                yield return null;
            }
            yield break;
        }


        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

        public void OnEnteredRange(PlayerController interactor)
        {
        if (!this)
		{
			return;
		}
            noteSelf.OnEnteredRange(interactor);
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            noteSelf.OnExitRange(interactor);
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public void Interact(PlayerController interactor)
        {
            self.spriteAnimator.Play("break");
            RoomHandler instanceRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
            instanceRoom.DeregisterInteractable(this);
            instanceRoom.DeregisterInteractable(this.GetComponent<NoteDoer>());
            noteSelf.Interact(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }
    }
    public class HMPrimeBattery
    {
        public static void Init()
        {
            /*
            string defaultPath = "Planetside/Resources/DungeonObjects/HMPrimeBattery/";
            string[] idlePaths = new string[]
            {
                defaultPath+"hmprime_idle_001.png",
                defaultPath+"hmprime_idle_002.png",
                defaultPath+"hmprime_idle_003.png",
                defaultPath+"hmprime_idle_004.png",
                defaultPath+"hmprime_idle_005.png",
                defaultPath+"hmprime_idle_006.png",
            };
            string[] breakPaths = new string[]
            {
                defaultPath+"hmprime_use_001.png",
                defaultPath+"hmprime_use_002.png",
                defaultPath+"hmprime_use_003.png",
                defaultPath+"hmprime_use_004.png",
                defaultPath+"hmprime_use_005.png",
                defaultPath+"hmprime_use_006.png",
                defaultPath+"hmprime_use_007.png",
                defaultPath+"hmprime_use_008.png",
                defaultPath+"hmprime_use_009.png",
            };
            */
            ETGMod.Databases.Strings.Core.Set("#HM_PRIME_NOTE_TROLL", 
                "The object left on the ground looks like a large, volatile battery.\n\nMaybe you should stand back...");


            //MajorBreakable note3 = BreakableAPIToolbox.GenerateMajorBreakable("hmPrimeBattery", idlePaths, 2, breakPaths, 2, 15000, true, 10, 16, -1, -4, true, null, null, true, null);


            var Battery = PrefabBuilder.BuildObject("HM Prime Battery");
            Battery.layer = 20;
            var sprite = Battery.AddComponent<tk2dSprite>();
            var animator = Battery.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.playAutomatically = true;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("hmprime_battery_idle");
            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
            mat.SetFloat("_EmissiveColorPower", 12f);
            mat.SetFloat("_EmissivePower", 10);
            sprite.renderer.material = mat;
            Battery.CreateFastBody(new IntVector2(10, 14), new IntVector2(0, -3), CollisionLayer.PlayerBlocker);
            Battery.CreateFastBody(new IntVector2(10, 14), new IntVector2(0, -3), CollisionLayer.EnemyBlocker);
            Battery.CreateFastBody(new IntVector2(10, 14), new IntVector2(0, -3), CollisionLayer.BulletBlocker);

            var majorBreakable = Battery.AddComponent<MajorBreakable>();
            majorBreakable.HitPoints = 15000;
            majorBreakable.sprite = sprite;
            majorBreakable.spriteAnimator = animator;
            majorBreakable.DamageReduction = 1000;


            NoteDoer finishedNote3 = BreakableAPIToolbox.GenerateNoteDoer(majorBreakable, BreakableAPI_Bundled.GenerateTransformObject(majorBreakable.gameObject, new Vector2(0.25f, 0.25f), "noteattachPoint").transform, "#HM_PRIME_NOTE_TROLL", false, NoteDoer.NoteBackgroundType.LETTER);
            EnemyToolbox.AddSoundsToAnimationFrame(majorBreakable.spriteAnimator, "hmprime_battery_boom", new Dictionary<int, string> { { 0, "Play_ENM_hammer_target_01" }, { 2, "Play_ENM_hammer_target_01" }, { 5, "Play_ENM_hammer_target_01" } });
            EnemyToolbox.AddEventTriggersToAnimation(majorBreakable.spriteAnimator, "hmprime_battery_boom", new Dictionary<int, string> { { 0, "BigBoomSoon" },{7, "DIE" } });
            finishedNote3.gameObject.AddComponent<HMPrimeBatteryController>();
            StaticReferences.StoredRoomObjects.Add("hmprimeBattery", finishedNote3.gameObject);

        }
    }
}
