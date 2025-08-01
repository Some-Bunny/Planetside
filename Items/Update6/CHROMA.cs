using Alexandria.PrefabAPI;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using Planetside;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using static Alexandria.Misc.MiscUtility;

using static UnityEngine.UI.GridLayoutGroup;
//using UnityEngine.UIElements;

namespace Planetside
{
    public class RefractionEffect : BraveBehaviour
    {
        private Gun parentObject;
        private Gun _ImitatedGun;
        public void InitSprite(Gun parent, Gun ImitatedGun, Color color, Vector2 Offset)
        {
            parentObject = parent;
            _ImitatedGun = ImitatedGun;
            this.transform.SetParent(parent.gameObject.transform, false);
            this.transform.localPosition = Vector3.zero;
            this.transform.position = parent.transform.position;

            this.sprite.collection = _ImitatedGun.sprite.collection;
            this.spriteAnimator.library = _ImitatedGun.spriteAnimator.library;
            
            this.StartCoroutine(DoLerp(color, Offset));
            this.sprite.SetSprite(_ImitatedGun.sprite.collection, ImitatedGun.DefaultSpriteID);
            this.sprite.color = color;

            this.sprite.usesOverrideMaterial = true;
            this.spriteAnimator.Play(GetPotentialIdle(ImitatedGun));

            parent.spriteAnimator.OnPlayAnimationCalled += (_, __) =>
            {
                var anim = TryAttemptGetAnimation(_ImitatedGun, parentObject, (__ as tk2dSpriteAnimationClip));
                if (anim != string.Empty && anim != "")
                {
                    this.spriteAnimator.Play(anim);
                    AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
                }
            };

           
        }

        public IEnumerator DoLerp(Color color, Vector2 Offset)
        {
            float e = 0;
            while (e < 1)
            {
                float _ = Easing.DoLerpT(e, Easing.Ease.OUT);
                var t = Vector3.Lerp(Vector2.zero, Offset, _);
                this.transform.localPosition = t;
                e += Time.deltaTime * 0.5f;
                yield return null;
            }
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = parentObject.sprite.WorldCenter,
                startSize = 10,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = color.WithAlpha(0.5f)
            });
            yield break;
        }
        public void Update()
        {
            this.sprite.FlipY = parentObject.sprite.FlipY;
            this.sprite.FlipX = parentObject.sprite.FlipX;
            this.sprite.renderer.enabled = parentObject.sprite.renderer.enabled;
        }

        public string TryAttemptGetAnimation(Gun GunAnimation, Gun Parent, tk2dSpriteAnimationClip dSpriteAnimationClip)
        {
            if (Parent.idleAnimation == dSpriteAnimationClip.name) { return GetPotentialIdle(GunAnimation); } //idle
            if (Parent.shootAnimation == dSpriteAnimationClip.name) { return GunAnimation.shootAnimation != string.Empty ? GunAnimation.shootAnimation : GetPotentialIdle(GunAnimation); } //fire
            if (Parent.reloadAnimation == dSpriteAnimationClip.name) { return GunAnimation.reloadAnimation != string.Empty ? GunAnimation.reloadAnimation : GetPotentialIdle(GunAnimation); } //reload

            if (Parent.alternateIdleAnimation == dSpriteAnimationClip.name) { return GunAnimation.alternateIdleAnimation != string.Empty ? GunAnimation.alternateIdleAnimation : GetPotentialIdle(GunAnimation); } //alt idle
            if (Parent.alternateShootAnimation == dSpriteAnimationClip.name) { return GunAnimation.alternateShootAnimation != string.Empty ? GunAnimation.alternateShootAnimation : GetPotentialIdle(GunAnimation); } //alt fire
            if (Parent.alternateReloadAnimation == dSpriteAnimationClip.name) { return GunAnimation.alternateReloadAnimation != string.Empty ? GunAnimation.alternateReloadAnimation : GetPotentialIdle(GunAnimation); } //alt reload

            if (Parent.emptyAnimation == dSpriteAnimationClip.name) { return GunAnimation.emptyAnimation != string.Empty ? GunAnimation.emptyAnimation : GetPotentialIdle(GunAnimation); ; } //empty
            if (Parent.emptyReloadAnimation == dSpriteAnimationClip.name) { return GunAnimation.emptyReloadAnimation != string.Empty ? GunAnimation.emptyReloadAnimation : GetPotentialIdle(GunAnimation); } // empty reload


            if (Parent.dischargeAnimation == dSpriteAnimationClip.name) { return GunAnimation.dischargeAnimation != string.Empty ? GunAnimation.dischargeAnimation : GetPotentialIdle(GunAnimation); } // discharge???

            if (Parent.introAnimation == dSpriteAnimationClip.name) { return GunAnimation.introAnimation != string.Empty ? GunAnimation.introAnimation : GetPotentialIdle(GunAnimation); }//intro

            if (Parent.finalShootAnimation == dSpriteAnimationClip.name) { return GunAnimation.finalShootAnimation != string.Empty ? GunAnimation.finalShootAnimation : GetPotentialIdle(GunAnimation); ; }//final shoot
            if (Parent.enemyPreFireAnimation == dSpriteAnimationClip.name) { return GunAnimation.enemyPreFireAnimation != string.Empty ? GunAnimation.enemyPreFireAnimation : GetPotentialIdle(GunAnimation); }//enemy prefire
            if (Parent.dodgeAnimation == dSpriteAnimationClip.name) { return GunAnimation.dodgeAnimation != string.Empty ? GunAnimation.dodgeAnimation : GetPotentialIdle(GunAnimation); }//dodge
        

            return GetPotentialIdle(GunAnimation);
        }

        public string GetPotentialIdle(Gun Parent)
        {
            return Parent.idleAnimation != string.Empty ? Parent.idleAnimation : "";
        }

    }


    class CHROMA : PlayerItem
    {
        public static void Init()
        {
            string itemName = "CHROMA";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CHROMA>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("CHROMA"), data, obj);
            string shortDesc = "The Trinity";
            string longDesc = "Splits your gun into 3.\n\nAble to refrect reality, this mineral is formed when pockets of the Aimless Void touch the meta-layers of the Universe.\nThe lives of hundreds of Gundead are used to even risk extracting it from the Void.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "psog");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1);
            item.consumable = true;
            item.quality = PickupObject.ItemQuality.S;
            ItemID = item.PickupObjectId;


            RefractionObject = PrefabBuilder.BuildObject("RefractionObject");
            var sprite = RefractionObject.AddComponent<tk2dSprite>();
            var spriteAnimator = RefractionObject.AddComponent<tk2dSpriteAnimator>();
            var refraction = RefractionObject.AddComponent<RefractionEffect>();
            refraction.sprite = sprite;
            refraction.spriteAnimator = spriteAnimator;
        }
        public static int ItemID;
        public static GameObject RefractionObject;

        public override bool CanBeUsed(PlayerController user)
        {
            if (user == null) { return false; }
            if (user.CurrentGun != null && user.CurrentGun.InfiniteAmmo == false)
            {
                if (user.CurrentGun.PickupObjectId == 546 | user.CurrentGun.PickupObjectId == 251 | user.CurrentGun.PickupObjectId == 734)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override void DoEffect(PlayerController user)
        {
            user.StartCoroutine(DoWaitEffect(user));
        }

        public IEnumerator DoWaitEffect(PlayerController user)
        {
            user.IsGunLocked = true;
            float a = BraveUtility.RandomAngle();


            var Gun = user.CurrentGun;
            bool canBeDropped = Gun.CanBeDropped;
            Gun.CanBeDropped = false;
            user.inventory.GunLocked = new OverridableBool(true);
            Vector2 Angle_R = MathToolbox.GetUnitOnCircle(a, 1);
            Vector2 Angle_G = MathToolbox.GetUnitOnCircle(a + 120, 1);
            Vector2 Angle_B = MathToolbox.GetUnitOnCircle(a + 240, 1);

            bool Trigger_Greening = false;
            bool Trigger_Blueing = false;

            bool Trigger_Spawn = false;
            //m_OBJ_powerstar_loop_01
            AkSoundEngine.PostEvent("Play_ENV_time_shatter_01", user.gameObject);
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Gun.sprite.WorldCenter,
                startSize = 16,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = Color.white.WithAlpha(0.333f)
            });

            yield return new WaitForSeconds(1);
            AkSoundEngine.PostEvent("Play_WPN_Vorpal_Shot_Critical_01", user.gameObject);
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Gun.sprite.WorldCenter,
                startSize = 12,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = Color.red.WithAlpha(0.333f)
            });

            float e = 0;
            while (e < 5)
            {
                GlobalSparksDoer.DoRandomParticleBurst(1, 
                    Gun.sprite.WorldCenter + new Vector2(-0.125f, -0.125f),
                    Gun.sprite.WorldCenter + new Vector2(0.125f, 0.125f),
                    Angle_R * (UnityEngine.Random.Range(16,  32) - (5 * e)), 
                    2f, 
                    1f, 
                    0.125f,
                    0.25f, 
                    Color.red * 3, 
                    GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                if (e > 1)
                {
                    if (Trigger_Greening == false)
                    {
                        AkSoundEngine.PostEvent("Play_WPN_Vorpal_Shot_Critical_01", user.gameObject);
                        Trigger_Greening = true;
                        Gun.sprite.color = Color.green * 3;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = Gun.sprite.WorldCenter,
                            startSize = 12,
                            rotation = 0,
                            startLifetime = 0.333f,
                            startColor = Color.green.WithAlpha(0.333f)
                        });
                    }
                    GlobalSparksDoer.DoRandomParticleBurst(1,
                    Gun.sprite.WorldCenter + new Vector2(-0.125f, -0.125f),
                    Gun.sprite.WorldCenter + new Vector2(0.125f, 0.125f),
                    Angle_G * (UnityEngine.Random.Range(16, 32) - (5 * e)),
                    2f,
                    1f,
                    0.125f,
                    0.25f,
                    Color.green * 3,
                    GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
                if (e > 2)
                {
                    if (Trigger_Blueing == false)
                    {
                        AkSoundEngine.PostEvent("Play_WPN_Vorpal_Shot_Critical_01", user.gameObject);
                        Trigger_Blueing = true;
                        ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                        {
                            position = Gun.sprite.WorldCenter,
                            startSize = 12,
                            rotation = 0,
                            startLifetime = 0.333f,
                            startColor = Color.blue.WithAlpha(0.333f)
                        });
                    }
                    GlobalSparksDoer.DoRandomParticleBurst(1,
                        Gun.sprite.WorldCenter + new Vector2(-0.125f, -0.125f),
                        Gun.sprite.WorldCenter + new Vector2(0.125f, 0.125f),
                        Angle_B * (UnityEngine.Random.Range(16, 32) - (5 * e)),
                        2f,
                        1f,
                        0.125f,
                        0.25f,
                        Color.blue * 3,
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
                if (e > 3 && Trigger_Spawn == false)
                {
                    AkSoundEngine.PostEvent("Play_ENM_deathray_charge_01", user.gameObject);

                    Trigger_Spawn = true;
                    var Top = UnityEngine.Object.Instantiate(RefractionObject, Gun.transform).GetComponent<RefractionEffect>();
                    var __ = AddModule(Gun, user);
                    Top.InitSprite(Gun, __, Color.red * 3, new Vector2(0, __.sprite.GetBounds().size.y * 0.25f));

                    var Bottom = UnityEngine.Object.Instantiate(RefractionObject, Gun.transform).GetComponent<RefractionEffect>();
                    __ = AddModule(Gun, user);
                    Bottom.InitSprite(Gun, __, Color.blue * 3, new Vector2(0, -(__.sprite.GetBounds().size.y * 0.25f)));
                }
                e += Time.deltaTime;
                yield return null;
            }
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Gun.sprite.WorldCenter,
                startSize = 10,
                rotation = 0,
                startLifetime = 0.333f,
                startColor = Color.green.WithAlpha(0.5f)
            });

            int _ = 0;
            for (int i = 0; i < 12; i++)
            {
                _++;
                if (_ >= 3) { _ = 0; }
                Color color = Color.white;
                switch (_)
                {
                    case 0:
                        color = Color.red * 3;
                        break;
                    case 1:
                        color = Color.green * 3;
                        break;
                    case 2:
                        color = Color.blue * 3;
                        break;
                }
                ParticleBase.EmitParticles("WaveParticleInverse", 1, new ParticleSystem.EmitParams()
                {
                    position = Gun.sprite.WorldCenter,
                    startSize = 8,
                    rotation = 0,
                    startLifetime = 0.25f,
                    startColor = color
                });
                yield return new WaitForSeconds(0.0125f);

            }
            
            AkSoundEngine.PostEvent("Play_NPC_faerie_heal_01", user.gameObject);
            
            for (int i = 0; i < 144; i++)
            {
                _++;
                if (_ >= 3) { _ = 0; }
                Color color = Color.white;
                switch (_)
                {
                    case 0:
                        color = Color.red * 3;
                        break;
                    case 1:
                        color = Color.green * 3;
                        break;
                    case 2:
                        color = Color.blue * 3;
                        break;
                }


                GlobalSparksDoer.DoRandomParticleBurst(1,
                Gun.sprite.WorldCenter + new Vector2(-0.125f, -0.125f),
                Gun.sprite.WorldCenter + new Vector2(0.125f, 0.125f),
                MathToolbox.GetUnitOnCircle(a + (5 * i), UnityEngine.Random.Range(2.1f, 4.2f)),
                2f,
                1f,
                0.125f,
                3f,
                color,
                GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }

            //s_WPN_Vorpal_Shot_Critical_01
            ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
            {
                position = Gun.sprite.WorldCenter,
                startSize = 12,
                rotation = 0,
                startLifetime = 0.5f,
                startColor = Color.white.WithAlpha(0.333f)
            });
            AkSoundEngine.PostEvent("Stop_OBJ_powerstar_loop_01", user.gameObject);
            user.IsGunLocked = false;
            user.inventory.GunLocked = new OverridableBool(false);
            Gun.AddStat(PlayerStats.StatType.Damage, 0.66f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            user.stats.RecalculateStats(user, true, false);
            Gun.CanBeDropped = canBeDropped;

            //user.switch
            yield break;
        }

        public Gun AddModule(Gun Parent, PlayerController playerController)
        {
            Gun randomGun = NewModuleExtensions.GetRandomGunOfQualitiesAndShootStyle(new System.Random(UnityEngine.Random.Range(1, 100000)),
                new List<int>() 
                {
                    Parent.PickupObjectId,
                    UnknownGun.GunknownID
                }, 
                Parent.DefaultModule.shootStyle,
                ReturnQualities(Parent.quality));
 
            ProjectileVolleyData projectileVolleyData = NewModuleExtensions.CombineVolleys(Parent, randomGun);

            NewModuleExtensions.ReconfigureVolley(projectileVolleyData);
            Parent.RawSourceVolley = projectileVolleyData;
            Parent.OnPrePlayerChange();
            Parent.SetBaseMaxAmmo(Parent.GetBaseMaxAmmo() + (int)(randomGun.GetBaseMaxAmmo()));
            Parent.GainAmmo((int)(randomGun.CurrentAmmo));
            playerController.inventory.ChangeGun(0, false, false);
            return randomGun;
        }

        public PickupObject.ItemQuality ReturnQualities(PickupObject.ItemQuality itemQuality)
        {
            switch (itemQuality)
            {
                case PickupObject.ItemQuality.D:
                    return DTierWeight.SelectByWeight();
                case PickupObject.ItemQuality.C:
                    return CTierWeight.SelectByWeight();
                case PickupObject.ItemQuality.B:
                    return BTierWeight.SelectByWeight();
                case PickupObject.ItemQuality.A:
                    return ATierWeight.SelectByWeight();
                case PickupObject.ItemQuality.S:
                    return STierWeight.SelectByWeight();
                default:
                    return BTierWeight.SelectByWeight();
            }
        }

        public static WeightedTypeCollection<PickupObject.ItemQuality> DTierWeight = GenerateWeights(new Dictionary<ItemQuality, float>()
        {
            {ItemQuality.D, 1 },
            {ItemQuality.C, 0.5f },
            {ItemQuality.B, 0.15f },
            {ItemQuality.A, 0.065f },
            {ItemQuality.S, 0.015f },
        });
        public static WeightedTypeCollection<PickupObject.ItemQuality> CTierWeight = GenerateWeights(new Dictionary<ItemQuality, float>()
        {
            {ItemQuality.D, 0.4f },
            {ItemQuality.C, 1f },
            {ItemQuality.B, 0.5f },
            {ItemQuality.A, 0.1f },
            {ItemQuality.S, 0.02f },
        });
        public static WeightedTypeCollection<PickupObject.ItemQuality> BTierWeight = GenerateWeights(new Dictionary<ItemQuality, float>()
        {
            {ItemQuality.D, 0.1f },
            {ItemQuality.C, 0.5f },
            {ItemQuality.B, 1f },
            {ItemQuality.A, 0.4f },
            {ItemQuality.S, 0.1f },
        });
        public static WeightedTypeCollection<PickupObject.ItemQuality> ATierWeight = GenerateWeights(new Dictionary<ItemQuality, float>()
        {
            {ItemQuality.D, 0.034f },
            {ItemQuality.C, 0.15f },
            {ItemQuality.B, 0.6f },
            {ItemQuality.A, 1f },
            {ItemQuality.S, 0.5f },
        });
        public static WeightedTypeCollection<PickupObject.ItemQuality> STierWeight = GenerateWeights(new Dictionary<ItemQuality, float>()
        {
            {ItemQuality.D, 0.03f },
            {ItemQuality.C, 0.1f },
            {ItemQuality.B, 0.3f },
            {ItemQuality.A, 0.8f },
            {ItemQuality.S, 1f },
        });


        private static WeightedTypeCollection<ItemQuality> GenerateWeights(Dictionary<ItemQuality, float> keyValuePairs)
        {
            var w = new WeightedTypeCollection<ItemQuality>()
            {

            };
            List<WeightedType<ItemQuality>> p = new List<WeightedType<ItemQuality>>();
            foreach (var entry in keyValuePairs)
            {
                p.Add(new WeightedType<ItemQuality>()
                {
                    value = entry.Key,
                    weight = entry.Value
                });
            }
            w.elements = p.ToArray();
            return w;
        }

    }



}