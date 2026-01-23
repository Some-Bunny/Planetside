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
using Alexandria.PrefabAPI;
using Alexandria;


namespace Planetside
{


    public class KineticStrike : PlayerItem
    {

        public static void Init()
        {
            string itemName = "Kinetic Bombardment";
            //string resourceName = "Planetside/Resources/kinetisstrikeitem.png";
            GameObject obj = new GameObject(itemName);
            KineticStrike activeitem = obj.AddComponent<KineticStrike>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("kinetisstrikeitem"), data, obj);
            //ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "KA-BEWWWWMMM!";
            string longDesc = "Call in an incredibly powerful, yet delayed kinetic strike on your cursor.\n\nHow did one of these end up inside the Gungeon? No one knows.\n\nHow does one of these even *land* inside the Gungeon? No one knows either.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 1250f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.S;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Trorc, 1f);

            activeitem.gameObject.AddComponent<BoomhildrItemPool>();

            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;

            GameObject _TargetReticle = PrefabBuilder.BuildObject("Kinetic Strike Target Reticle");
            tk2dSprite sprite = _TargetReticle.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("redmarksthespot"));
            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            sprite.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 50);
            sprite.sprite.renderer.material = mat;
            ExpandReticleRiserEffect rRE = _TargetReticle.AddComponent<ExpandReticleRiserEffect>();
            rRE.RiserHeight = 2;
            rRE.RiseTime = 1;
            rRE.NumRisers = 3;
            KineticStrikeTargetReticle = _TargetReticle;


            GameObject _Impact = PrefabBuilder.BuildObject("Kinetic Strike Impact");
            sprite = _Impact.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("kineticstrike"));
            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            sprite.sprite.usesOverrideMaterial = true;
            sprite.usesOverrideMaterial = true;
            sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
            sprite.sprite.usesOverrideMaterial = true;
            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
            sprite.renderer.material.EnableKeyword("_BurnAmount");
            ImpactPrefab = _Impact;

            GameObject _Fall = PrefabBuilder.BuildObject("Kinetic Strike Falldown");
            sprite = _Fall.AddComponent<tk2dSprite>();
            sprite.collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.Oddments_Sheet_Data.GetSpriteIdByName("kineticstrikeaerial2"));
            sprite.CachedPerpState = tk2dBaseSprite.PerpendicularState.FLAT;
            sprite.sprite.usesOverrideMaterial = true;
            sprite.usesOverrideMaterial = true;
            sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
            sprite.sprite.usesOverrideMaterial = true;
            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
            sprite.renderer.material.EnableKeyword("_BurnAmount");

            ImprovedAfterImage yeah = _Fall.AddComponent<ImprovedAfterImage>();
            yeah.dashColor = new Color(3f, 1.8f, 0f);
            yeah.spawnShadows = true;
            yeah.shadowTimeDelay = 0.01f;
            yeah.shadowLifetime = 1.5f;
            FalldownPrefab = _Fall;
        }
        public static int KineticBombardmentID;
        public static GameObject KineticStrikeTargetReticle;
        public static GameObject ImpactPrefab;
        public static GameObject FalldownPrefab;





        public override bool CanBeUsed(PlayerController user)
        {
            if (HasTriggeredCrossHair != true)
            {
                return true;
            }
            else if (HasTriggeredCrossHair == true)
            {
                RoomHandler currentRoom = user.CurrentRoom;
                aimpointCanBeUsed = aimpoint;
                IntVector2? vector = (user as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
                if (vector != null)
                {
                    CellData cellAim = currentRoom.GetNearestCellToPosition(aimpointCanBeUsed);
                    CellData cellAimLeft = currentRoom.GetNearestCellToPosition(aimpointCanBeUsed + Vector2.left);
                    CellData cellAimRight = currentRoom.GetNearestCellToPosition(aimpointCanBeUsed + Vector2.right);
                    CellData cellAimUp = currentRoom.GetNearestCellToPosition(aimpointCanBeUsed + Vector2.up);
                    CellData cellAimDown = currentRoom.GetNearestCellToPosition(aimpointCanBeUsed + Vector2.down);
                    if (!cellAim.isNextToWall && !cellAimLeft.isNextToWall && !cellAimRight.isNextToWall && !cellAimUp.isNextToWall && !cellAimDown.isNextToWall)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public override void DoEffect(PlayerController user)
        {
            if (HasTriggeredCrossHair == false)
            {
                GameManager.Instance.StartCoroutine(ClearCooldown());
                CrossHair = UnityEngine.Object.Instantiate<GameObject>(KineticStrikeTargetReticle, user.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>();

                AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", gameObject);
                this.m_currentAngle = BraveMathCollege.Atan2Degrees(user.unadjustedAimPoint.XY() - user.CenterPosition);
                this.m_currentDistance = 5f;
                this.UpdateReticlePosition();
            }
            else 
            {
                HasTriggeredCrossHair = false;
                GameManager.Instance.StartCoroutine(this.DoStrike(user.CurrentRoom, user));
            }
        }

        private IEnumerator ClearCooldown()
        {
            yield return null;
            HasTriggeredCrossHair = true;
            base.ClearCooldowns();
            yield break;
        }
        public override void Update()
        {
            base.Update();
            if (base.LastOwner != null)
            {
                if (CrossHair != null)
                {
                    this.UpdateReticlePosition();
                }
            }
            else if (base.LastOwner == null && CrossHair != null)
            {
                Destroy(CrossHair.gameObject);
            }
        }
        float T = 0;

        private void UpdateReticlePosition()
        {
            if (CrossHair == null) { return; }
            if (BraveInput.GetInstanceForPlayer(base.LastOwner.PlayerIDX).IsKeyboardAndMouse(false))
            {
                Vector2 vector = base.LastOwner.unadjustedAimPoint.XY();
                Vector2 vector2 = vector - CrossHair.GetBounds().extents.XY();
                CrossHair.transform.position = vector2 + new Vector2(1.3125f, 1.3125f);
                aimpoint = vector2 + new Vector2(1.3125f, 1.3125f);
            }
            else
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(base.LastOwner.PlayerIDX);
                var _ = base.LastOwner.CenterPosition + (Quaternion.Euler(0f, 0f, this.m_currentAngle) * Vector2.right).XY() * this.m_currentDistance;
                Vector2 vector3 = _;
                vector3 += instanceForPlayer.ActiveActions.Aim.Vector * 25f * BraveTime.DeltaTime;
                this.m_currentAngle = BraveMathCollege.Atan2Degrees(vector3 - base.LastOwner.CenterPosition);
                this.m_currentDistance = Vector2.Distance(vector3, base.LastOwner.CenterPosition);
                this.m_currentDistance = Mathf.Min(this.m_currentDistance, this.maxDistance);
                vector3 = _;
                CrossHair.transform.position = vector3 + new Vector2(1.3125f, 1.3125f);
                aimpoint = vector3 + new Vector2(1.3125f, 1.3125f);
            }
            T -= Time.deltaTime;
            if (T < 0)
            {
                ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = aimpoint,
                    startSize = 9,
                    rotation = 0,
                    startLifetime = 0.75f,
                    startColor = Color.red.WithAlpha(0.333f)
                });
                T = 0.5f;
                var v = aimpoint + new Vector2(0, 100);
                var sdit = Vector2.Distance(aimpoint, v);
                for (float i = 0; i < sdit; i++)
                {
                    ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = Vector3.Lerp(aimpoint, v, i / sdit),
                        startColor = Color.red,
                        startLifetime = UnityEngine.Random.Range(0.75f, 1) * (1 - (i / sdit)) ,
                        startSize = 0.33f
                    });
                }

            }
        }

        private bool HasTriggeredCrossHair;
        private tk2dBaseSprite CrossHair;
        private IEnumerator DoStrike(RoomHandler room, PlayerController player)
        {
            HasTriggeredCrossHair = false;
            Vector2 HitPoint = aimpoint;


            tk2dBaseSprite _SpriteReticle = CrossHair;
            CrossHair = null;

            AkSoundEngine.PostEvent("Play_WPN_dawnhammer_charge_01", _SpriteReticle.gameObject);

           
            TextMaker text = player.gameObject.AddComponent<TextMaker>();
            text.TextSize = 4;
            text.Color = Color.red;
            text.ExistTime = 11;
            text.FadeInTime = 0f;
            text.FadeOutTime = 3f;
            text.Opacity = 1;
            text.anchor = dfPivotPoint.TopCenter;
            text.offset = new Vector3(-1.25f, 3f);
            text.GameObjectToAttachTo = player.gameObject;

            bool Playsound = false;
            float ela = 0f;
            float Y = 0f;
            float dura = 10f;
            tk2dBaseSprite Falldown = null;
            while (ela < dura)
            {
                if (Y < 0)
                {
                    Y = 0.9f * (1 - Mathf.Min(ela / dura, 0.75f)) ;
                    this.StartCoroutine(DoLaser(Y));
                }



                float Timer = dura - ela;
                text.ChangeText($"Kinetic Impact In:\n\n{Math.Round(Timer, 2)} Seconds");
                if (ela > 9 && Playsound == false)
                {
                    Playsound = true;
                    AkSoundEngine.PostEvent("Play_BOSS_RatMech_Whistle_01", _SpriteReticle.gameObject);

                    Falldown = UnityEngine.Object.Instantiate<GameObject>(FalldownPrefab, HitPoint + new Vector2(0, 100), Quaternion.identity).GetComponent<tk2dBaseSprite>();
                }
                if (Falldown)
                {
                    Falldown.transform.position = Vector3.Lerp(HitPoint + new Vector2(0, 100), HitPoint, ela - 9);
                    GlobalSparksDoer.DoRadialParticleBurst(3, Falldown.WorldBottomLeft, Falldown.WorldTopRight, 30f, 0.2f, 0.1f, null, 3.5f, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                    GlobalSparksDoer.DoRadialParticleBurst(12, Falldown.WorldBottomLeft, Falldown.WorldTopRight, 30f, 0.2f, 0.1f, null, 8f, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }
                ela += BraveTime.DeltaTime;
                Y -= BraveTime.DeltaTime;
                float t = ela / dura * (ela / dura);
                _SpriteReticle.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0f, 0f, 0f), t);
                yield return null;
            }

            Destroy(_SpriteReticle.gameObject);
            if (Falldown)
            {
                Destroy(Falldown.gameObject);
            }
            Pixelator.Instance.FadeToColor(0.01f, new Color(1,1,1, 0.3f), false, 0f);
            Pixelator.Instance.FadeToColor(0.75f, new Color(1, 1, 1, 0.3f), true, 0f);

            tk2dBaseSprite _Impact = UnityEngine.Object.Instantiate<GameObject>(ImpactPrefab, HitPoint, Quaternion.identity).GetComponent<tk2dBaseSprite>();
            GameObject epicwin = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.HM_Absolution_GUID).GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX, HitPoint, Quaternion.identity);
            

            ExplosionData defaultSmallExplosionData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            this.KineticBomb.effect = defaultSmallExplosionData.effect;
            this.KineticBomb.ignoreList = defaultSmallExplosionData.ignoreList;
            this.KineticBomb.ss = defaultSmallExplosionData.ss;
            Exploder.Explode(HitPoint, this.KineticBomb, Vector2.zero, null, false, CoreDamageTypes.None, false);


            AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", _Impact.gameObject);
            text.ChangeText("Kinetic Impact Delivered.\n\n     Reloading Payload.");
            text.ChangeOffset(new Vector3(-2, 3f));

            yield return new WaitForSeconds(5f);
            


            Material targetMaterial = _Impact.renderer.material;
            ela = 0f;
            dura = 5f;
            while (ela < dura)
            {
                ela += BraveTime.DeltaTime;
                float t = ela / dura;
                targetMaterial.SetFloat("_BurnAmount", t);
                yield return null;
            }
            if (text != null)
            {
                Destroy(text);
            }
            Destroy(_Impact.gameObject);
            Destroy(epicwin);

            yield break;
        }

        private IEnumerator DoLaser(float Duration)
        {
            var v = aimpoint + new Vector2(0, 100);
            var sdit = Vector2.Distance(aimpoint, v) * 2f;
            float w = Duration / 500f;

            for (float i = 0; i < sdit; i++)
            {
                ParticleBase.EmitParticles("ShellraxEyeParticle", 1, new ParticleSystem.EmitParams()
                {
                    position = Vector3.Lerp(v, aimpoint, i / sdit),
                    startColor = Color.red,
                    startLifetime = UnityEngine.Random.Range(0.75f, 1) * (1 - (i / sdit)),
                    startSize = 0.4f
                });
                yield return new WaitForSeconds(w);
            }
            yield break;
        }

        private ExplosionData KineticBomb = new ExplosionData
        {
            damageRadius = 4.2f,
            damageToPlayer = 2f,
            doDamage = true,
            damage = 5000f,
            doExplosionRing = false,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 100f,
            preventPlayerForce = false,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = true,
            playDefaultSFX = false
        };

        public override void OnPreDrop(PlayerController user)
        {
            HasTriggeredCrossHair = false;
            base.OnPreDrop(user);
            if (CrossHair != null)
            {
                Destroy(CrossHair.gameObject);
            }
        }

        public override void OnDestroy()
        {
            HasTriggeredCrossHair = false;
            if (CrossHair != null)
            {
                Destroy(CrossHair.gameObject);
            }
            base.OnDestroy();
        }
        public static GameObject StarNuke;

        
        private Vector2 aimpointCanBeUsed;
        private Vector2 aimpoint;
        private float maxDistance = 15;
        private float m_currentAngle;
        private float m_currentDistance;


    }
}


