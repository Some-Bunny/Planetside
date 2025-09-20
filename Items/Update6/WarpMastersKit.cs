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
using GungeonAPI;
using Pathfinding;
using static tk2dSpriteCollectionDefinition;
using SynergyAPI;
using Alexandria.PrefabAPI;
using HutongGames.PlayMaker.Actions;
using static UnityEngine.UI.GridLayoutGroup;
using HarmonyLib;

namespace Planetside
{
    public class WarpMastersKit : PlayerItem
    {

        public class BuffAfterWarpEffect : GameActorDecorationEffect 
        {
            public BuffAfterWarpEffect() 
            {
                this.duration = 5;
                this.AffectsPlayers = true;
                this.AffectsEnemies = false;
                this.effectIdentifier = "WarpBuff";
                this.stackMode = GameActorEffect.EffectStackingMode.Refresh;
            }

            public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
            {
                if (actor is PlayerController player)
                {

                    if (player.IsTemporaryEeveeForUnlock == true) { return; }
                    if (player.characterIdentity == PlayableCharacters.Eevee) { return; }
                    player.portalEeveeTex = (Texture2D)StaticTextures.NebulaTexture;
                    player.IsTemporaryEeveeForUnlock = true;


                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = player.sprite.WorldCenter,
                        startSize = 4,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startColor = new Color(0.15f, 0, 0.6f, 1)
                    });
                }
                base.OnEffectApplied(actor, effectData, partialAmount);
            }

            public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
            {
                if (actor is PlayerController player)
                {
                    if (player.characterIdentity == PlayableCharacters.Eevee) { return; }
                    player.IsTemporaryEeveeForUnlock = false;

                    ParticleBase.EmitParticles("WaveParticle", 1, new ParticleSystem.EmitParams()
                    {
                        position = player.sprite.WorldCenter,
                        startSize = 4,
                        rotation = 0,
                        startLifetime = 0.5f,
                        startColor = new Color(0.15f, 0f, 0.6f, 1)
                    });
                    for (int i = 0; i < 32; i++)
                    {
                        StaticVFXStorage.VoidParticleSystem.Emit(new ParticleSystem.EmitParams()
                        {
                            position = player.sprite.WorldCenter,
                            rotation = BraveUtility.RandomAngle(),
                            velocity = BraveUtility.RandomVector2(new Vector2(-8f, -8), new Vector2(8, 8)),
                            startLifetime = 2
                        }, 1);
                    }
                }
                base.OnEffectRemoved(actor, effectData);
            }

        }

        public static void Init()
        {
            string itemName = "Warp-Techs Kit";
            GameObject obj = new GameObject(itemName);
            WarpMastersKit activeitem = obj.AddComponent<WarpMastersKit>();
            var data = StaticSpriteDefinitions.Active_Item_Sheet_Data;
            ItemBuilder.AddSpriteToObjectAssetbundle(itemName, data.GetSpriteIdByName("warpmasterkit"), data, obj);
            string shortDesc = "Building (Instant) Bridges";
            string longDesc = "Places a teleporter connected to the Gungeons teleporter network. If used with a teleporter already in the room, warps you to it, damaging anything along the way.\n\nAfter the Hegemonys invasion fell through, plans were set up for a more 'subtle' takeover.\n\nGroups of 'Warp Technicians' were kitted with quick-deploy teleporters and were tasked of making a secondary teleporter network in the Gungeon, allowing for quick escapes and transportation of goods undercover.\n\nMost died. Some were unfortunate enough to have their only exit point disconnect from the network, entombing them in some forgotten room. The few that lived to tell the tale recommended it solely for the insanely high wages.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Timed, 12.5f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.D;
            WarpMastersKit.WarpMasterKitID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);
            TeleporterToPlace = CreateTeleporter();

            SynergyAPI.SynergyBuilder.AddItemToSynergy(activeitem, CustomSynergyType.TELEPORTER_ACCIDENT);

            Explosion = new ExplosionData();
            Explosion.breakSecretWalls = true;
            Explosion.damage = 5;
            Explosion.damageRadius = 1.625f;
            Explosion.damageToPlayer = 0;
            Explosion.debrisForce = 10;
            Explosion.doDamage = true;
            Explosion.doDestroyProjectiles = false;
            Explosion.doExplosionRing = false;
            Explosion.doForce = false;
            Explosion.doStickyFriction = false;
            Explosion.doScreenShake = false;
            Explosion.explosionDelay = 0;
            Explosion.force = 1;
            Explosion.comprehensiveDelay = 0;
            Explosion.forceUseThisRadius = true;
            Explosion.useDefaultExplosion = false;
            Explosion.ss = new ScreenShakeSettings() { vibrationType = ScreenShakeSettings.VibrationType.None, direction = Vector2.zero, falloff = 0, speed = 0, time = 0, magnitude = 0, simpleVibrationStrength = Vibration.Strength.UltraLight, simpleVibrationTime = Vibration.Time.Instant };
            Explosion.playDefaultSFX = false;
            Explosion.IsChandelierExplosion = false;
            Explosion.isFreezeExplosion = false;
            Explosion.preventPlayerForce = true;
            Explosion.effect = (PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX;
            Explosion.ignoreList = new List<SpeculativeRigidbody> { };

            SynergyAPI.SynergyBuilder.AddItemToSynergy(activeitem, CustomSynergyType.TELEPORTER_ACCIDENT);

            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:warp-techs_kit",
                "psog:teleporting_gunfire"
            };
            Alexandria.ItemAPI.CustomSynergies.Add("Warp Sickness", mandatoryConsoleIDs, null, true);

        }


        public static GameObject TeleporterToPlace;
        public static ExplosionData Explosion;

        public static GameObject CreateTeleporter()
        {
            var teleporter = PrefabBuilder.BuildObject("MiniTeleporter");

            TeleporterController existingTeleporterController = ResourceManager.LoadAssetBundle("brave_resources_001").LoadAsset<GameObject>("Teleporter_Gungeon_01").GetComponentInChildren<TeleporterController>();
            teleporter.layer = 20;

            var sprite = teleporter.gameObject.AddComponent<tk2dSprite>();
            sprite.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            sprite.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data, "smallteleporter_idle_001");
            
            var animator = teleporter.gameObject.AddComponent<tk2dSpriteAnimator>();
            animator.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("miniteleporter_place");

            TeleporterController teleporterController = teleporter.GetOrAddComponent<TeleporterController>();

            teleporterController.sprite = sprite;
            teleporterController.spriteAnimator = animator;
            teleporterController.sprite.usesOverrideMaterial = true;

            Material mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(162, 233, 195, 255));
            mat_.SetFloat("_EmissiveColorPower", 15f);
            mat_.SetFloat("_EmissivePower", 10);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 1f);
            mat_.mainTexture = teleporterController.sprite.renderer.material.mainTexture;
            teleporterController.sprite.renderer.material = mat_; 

            teleporterController.teleporterIcon = existingTeleporterController.teleporterIcon;


            //GameObject clonedextantActiveVFX = FakePrefab.Clone(existingTeleporterController.extantActiveVFX);
            var VFX = PrefabBuilder.BuildObject("MiniTeleporter_Arrival");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            VFX.transform.SetParent(teleporter.transform);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("smallteleporter_glow_001"));
            tk2d.renderer.enabled = false;
            tk2d.HeightOffGround = 2;
            tk2d.usesOverrideMaterial = true;
            tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

            ExpandReticleRiserEffect rRE = VFX.gameObject.AddComponent<ExpandReticleRiserEffect>();
            rRE.RiserHeight = 0.75f;
            rRE.RiseTime = 2;
            rRE.NumRisers = 3;
            rRE.UpdateSpriteDefinitions = true;
            rRE.CurrentSpriteName = "smallteleporter_glow_001";




            teleporterController.extantActiveVFX = VFX;

            //teleportArrive_
            var VFX_Arrival = PrefabBuilder.BuildObject("MiniTeleporter_ArrivalProper");
            FakePrefab.MarkAsFakePrefab(VFX_Arrival);
            DontDestroyOnLoad(VFX_Arrival);
            VFX_Arrival.SetActive(false);
            VFX_Arrival.transform.SetParent(teleporter.transform);
            var tk2d_arrival = VFX_Arrival.AddComponent<tk2dSprite>();
            tk2d_arrival.Collection = StaticSpriteDefinitions.RoomObject_Sheet_Data;
            tk2d_arrival.SetSprite(StaticSpriteDefinitions.RoomObject_Sheet_Data.GetSpriteIdByName("smallteleporter_vfxarrive_001"));

            tk2d_arrival.usesOverrideMaterial = true;
            tk2d_arrival.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

            var animator_arrival = VFX_Arrival.AddComponent<tk2dSpriteAnimator>();
            animator_arrival.library = StaticSpriteDefinitions.RoomObject_Animation_Data;
            animator_arrival.defaultClipId = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipIdByName("teleportArrive_");
            animator_arrival.playAutomatically = true;
            var killer = VFX_Arrival.AddComponent<SpriteAnimatorKiller>();
            killer.animator = animator_arrival;
            killer.delayDestructionTime = 0.6363f;
            killer.fadeTime = 0;


            mat_ = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_.SetColor("_EmissiveColor", new Color32(162, 233, 195, 255));
            mat_.SetFloat("_EmissiveColorPower", 15f);
            mat_.SetFloat("_EmissivePower", 10);
            mat_.SetFloat("_EmissiveThresholdSensitivity", 1f);
            mat_.mainTexture = tk2d_arrival.sprite.renderer.material.mainTexture;
            tk2d_arrival.sprite.renderer.material = mat_;


            var f = FakePrefab.Clone(existingTeleporterController.teleportArrivalVFX);
            f.transform.localScale *= 0.5f;

            teleporterController.teleportArrivalVFX = f;

            GameObject clonedteleportDepartureVFX = FakePrefab.Clone(existingTeleporterController.teleportDepartureVFX);
            teleporterController.teleportDepartureVFX = clonedteleportDepartureVFX;

            GameObject clonedportalVFX = FakePrefab.Clone(existingTeleporterController.portalVFX.gameObject);
            teleporterController.portalVFX = clonedportalVFX.GetComponent<tk2dSpriteAnimator>();


            var abbeyAnim = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("teleport_pad_activate");
            abbeyAnim.frames[2].triggerEvent = true;
            abbeyAnim.frames[2].eventAudio = "Play_OBJ_chest_shake_01";
            abbeyAnim.frames[9].triggerEvent = true;
            abbeyAnim.frames[9].eventAudio = "Play_OBJ_teleport_activate_01";
            abbeyAnim.frames[12].triggerEvent = true;
            abbeyAnim.frames[12].eventInfo = "playIdle";

            var arrive = StaticSpriteDefinitions.RoomObject_Animation_Data.GetClipByName("teleportArrive_");
            arrive.frames[15].triggerEvent = true;
            arrive.frames[15].eventAudio = "Play_OBJ_teleport_arrive_01";

            return teleporter;
        }

        public static int WarpMasterKitID;
        public override void Pickup(PlayerController player)
        {
            base.sprite.SetSprite("warpmasterkit");
            currentTeleporter = null;
            lastRoom = null;

            base.Pickup(player);   
        }

    
        public override void OnPreDrop(PlayerController user)
        {
            base.sprite.SetSprite("warpmasterkit");
            currentTeleporter = null;
            lastRoom = null;
            base.OnPreDrop(user);
        }

        public override void OnDestroy()
        {
            if (base.LastOwner != null) 
            {

            }
            base.OnDestroy();
        }


        public override void Update()
        {
            base.Update();
            if (LastOwner != null)
            {
                if (LastOwner.CurrentRoom != null)
                {
                    if (lastRoom != LastOwner.CurrentRoom)
                    {
                        lastRoom = LastOwner.CurrentRoom;
                        var t = lastRoom.GetComponentsInRoom<TeleporterController>().FirstOrDefault();
                        if (t != null)
                        {
                            currentTeleporter = t;
                            base.sprite.SetSprite("warpmasterkit_warp");
                        }
                        else
                        {
                            base.sprite.SetSprite("warpmasterkit");
                            currentTeleporter = null;
                        }

                    }
                }
            }
        }//warpmasterkit_warp

        public override bool CanBeUsed(PlayerController user)
        {
            if (currentTeleporter != null)
            {
                return true;
            }
            if (lastRoom == null) { return false; }
            var c = lastRoom.GetNearestCellToPosition(user.transform.position);
            if (c == null) { return false; }
            if (c.isOccupied) { return false; }
            if (c.containsTrap) { return false; }
            if (c.IsTrapZone) { return false; }
            if (c.isNextToWall) { return false; }
            if (c.type == CellType.WALL || c.type == CellType.PIT) { return false; }
            return true;
        }



        private RoomHandler lastRoom;
        private TeleporterController currentTeleporter;
        public override void DoEffect(PlayerController user)
        {
            if (currentTeleporter != null)
            {
                var obj = UnityEngine.Object.Instantiate(StaticVFXStorage.TeleportDistortVFX, user.transform.position, Quaternion.identity);
                Destroy(obj, 3);
                TeleportBack teleportBack = null;
                if (user.IsInCombat)
                {
                    teleportBack = currentTeleporter.GetOrAddComponent<TeleportBack>();
                    teleportBack.Controller = currentTeleporter;
                    teleportBack.cachedPoint = user.transform.position;
                    user.CurrentRoom.RegisterInteractable(teleportBack);
                }
                this.StartCoroutine(DoWarp(user, user.transform.position, currentTeleporter.sprite.WorldCenter.ToVector3ZUp() + new Vector3(-0.5f, 0), teleportBack));

            }
            else
            {
                AkSoundEngine.PostEvent("Play_OBJ_turret_set_01", user.gameObject);
                this.CurrentTimeCooldown -= 7.5f;
                var t = UnityEngine.Object.Instantiate(TeleporterToPlace, LastOwner.transform.position, Quaternion.identity).GetComponent<TeleporterController>();
                currentTeleporter = t;
                currentTeleporter.Activate();
                currentTeleporter.extantActiveVFX.SetActive(false);
                currentTeleporter.ConfigureOnPlacement(user.CurrentRoom);
                Minimap.Instance.roomToTeleportIconMap[user.CurrentRoom].SetActive(true);
                currentTeleporter.spriteAnimator.AnimationEventTriggered += (obj1, obj2, obj3) =>
                {
                    if (obj2.GetFrame(obj3).eventInfo == "playIdle")
                    {
                        currentTeleporter.spriteAnimator.Play("miniteleporter_idle");
                        currentTeleporter.extantActiveVFX.SetActive(true);
                    }
                };
                base.sprite.SetSprite("warpmasterkit_warp");
            }
        }

        public static IEnumerator DoWarp(PlayerController playerController,  Vector2 startPoint, Vector2 endPoint, TeleportBack teleportBack)
        {
            bool isSickWithIt = playerController.PlayerHasActiveSynergy("Warp Sickness");

            var startPos = startPoint;
            var endPos = endPoint;
            AkSoundEngine.PostEvent("Play_OBJ_chestwarp_use_01", playerController.gameObject);

            float dot = Vector2.Dot((endPos - startPos).normalized, MathToolbox.GetUnitOnCircle(playerController.CurrentGun.CurrentAngle, 1));
            playerController.healthHaver.PreventAllDamage = true;
            playerController.SetIsFlying(true, "Warp");
            var inp = playerController.CurrentInputState;

            playerController.CurrentInputState = PlayerInputState.NoInput;

            var m = MathToolbox.GetUnitOnCircle(playerController.CurrentGun.CurrentAngle, 1).normalized * 8;

            var downwellAfterimage = playerController.gameObject.AddComponent<AfterImageTrailController>();
            downwellAfterimage.spawnShadows = true;
            downwellAfterimage.shadowTimeDelay = 0.025f;
            downwellAfterimage.shadowLifetime = 0.75f;
            downwellAfterimage.minTranslation = 0.1f;
            downwellAfterimage.dashColor = Color.green * 2.25f;
            downwellAfterimage.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");
            playerController.knockbackDoer.m_isImmobile = new OverridableBool(true);
            playerController.sprite.renderer.enabled = false;
            playerController.ToggleGunRenderers(false);
            playerController.ToggleHandRenderers(false);
            SpriteOutlineManager.ToggleOutlineRenderers(playerController.sprite, false);
            var v_d = 1 + Vector2.Distance(endPos, startPos) * 0.03125f;
            float duration = Mathf.Min(0.5f, v_d);
            playerController.healthHaver.TriggerInvulnerabilityPeriod(1 + duration);
            Vector3 lastPos = Vector2.zero;
            float e = 0;
            float t = 0;
            Vector3 LastPos_2 = startPos;
            float v = Vector2.Distance(lastPos, endPos);

            int DistTick = 0;

            int failsafe = 2500;
            while (e < duration)
            {
                t = e / duration;
                if (float.IsNaN(t)) { t = 0;}
                var newPosition = Vector3.Lerp(startPos, endPos, t * t) + (m.ToVector3ZUp() * MathToolbox.EaseInAndBack(t));
                if (newPosition.GetAbsoluteRoom() != playerController.CurrentRoom)
                {
                    while (newPosition.GetAbsoluteRoom() != playerController.CurrentRoom)
                    {
                        failsafe--;
                        if (failsafe <= 0) { break; }
                        newPosition = Vector2.MoveTowards(newPosition, endPos, Time.deltaTime * 4);
                    }
                    failsafe = 1000;
                }

                playerController.transform.position = newPosition;
                playerController.specRigidbody.Reinitialize();
                lastPos = playerController.transform.position;

                

                e += Time.deltaTime;
                float v_1 = Vector2.Distance(lastPos, endPos);
                if (v_1 > 0) {v = v_1;}

    

                DistTick += (int)Vector2.Distance(playerController.transform.position, LastPos_2);
                if (DistTick > 0)
                {
                    for (int i = 0; i < DistTick; i++)
                    {
                        float t_ = (float)i / DistTick;
                        Vector3 vector3 = Vector3.Lerp(playerController.transform.position, LastPos_2, t_);
                        Explosion.damage = 11.1f * playerController.stats.GetStatValue(PlayerStats.StatType.Damage) * (isSickWithIt ? 3 : 1);
                        Explosion.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
                        Explosion.damageRadius = 1.625f;
                        Explosion.ignoreList = new List<SpeculativeRigidbody> { playerController.specRigidbody };
                        
                        Exploder.Explode(vector3+ new Vector3(0, 0.625f), Explosion, Vector3.zero, null, true);
                    }
                    LastPos_2 = playerController.transform.position;
                    DistTick = 0;
                }

                yield return null;
            }

            playerController.transform.position = endPos;
            playerController.specRigidbody.Reinitialize();
            playerController.healthHaver.PreventAllDamage = false;
            playerController.SetIsFlying(false, "Warp");
            downwellAfterimage.spawnShadows = false;
            Destroy(downwellAfterimage, 1);
            var obj = UnityEngine.Object.Instantiate(StaticVFXStorage.TeleportDistortVFX, playerController.transform.position, Quaternion.identity);
            Destroy(obj, 3);
            if (teleportBack != null)
            {
                teleportBack.ToggleIsUsable(playerController);
            }
            playerController.CurrentInputState = inp;
            playerController.sprite.renderer.enabled = true;
            playerController.ToggleGunRenderers(true);
            playerController.ToggleHandRenderers(true);
            SpriteOutlineManager.ToggleOutlineRenderers(playerController.sprite, true);
            playerController.knockbackDoer.m_isImmobile = new OverridableBool(false);
            yield return null;
            playerController.knockbackDoer.ApplyKnockback(endPos - new Vector2(lastPos.x, lastPos.y), Mathf.Min(25, Mathf.Max((endPos - new Vector2(lastPos.x, lastPos.y)).magnitude * 10, v * 10)), false);

            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", playerController.gameObject);
            Explosion.damageRadius = 2.5f;
            Explosion.damage = 25f * playerController.stats.GetStatValue(PlayerStats.StatType.Damage) * (isSickWithIt ? 3 : 1);
            Explosion.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
            Exploder.Explode(playerController.sprite.WorldTopCenter, Explosion, Vector3.zero, null, true);

            if (isSickWithIt == true)
            {
                playerController.ApplyEffect(new BuffAfterWarpEffect() 
                {
                    AffectsPlayers = true,
                   
                });
            }

            yield break;
        }
    }

    public class TeleportBack : MonoBehaviour, IPlayerInteractable
    {
        public TeleporterController Controller;
        public bool canBeUsed = false;
        public Vector2 cachedPoint;
        public PlayerController trackingPlayer;

        public void Update()
        {
            if (trackingPlayer != null && trackingPlayer.IsInCombat == false)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(Controller.sprite, false);
                this.transform.position.GetAbsoluteRoom()?.DeregisterInteractable(this);
                Destroy(this);
            }
        }

        public void ToggleIsUsable(PlayerController playerController)
        {
            trackingPlayer = playerController;
            canBeUsed = true;
            Controller.OnEnteredRange(playerController);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return Controller.GetAnimationState(interactor, out shouldBeFlipped);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            //if (Vector2.Distance(point, Controller.sprite.WorldCenter) < 4.5f) { Destroy(this); }
            return Vector2.Distance(point, Controller.sprite.WorldCenter) * 0.4f;
        }

        public float GetOverrideMaxDistance()
        {
            return Controller.GetOverrideMaxDistance();
        }

        public void Interact(PlayerController interactor)
        {
            if (canBeUsed == false) { return; }
            interactor.StartCoroutine(WarpMastersKit.DoWarp(interactor, interactor.transform.position, cachedPoint, null));
            this.transform.position.GetAbsoluteRoom()?.DeregisterInteractable(this);
            Destroy(this);
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(Controller.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(Controller.sprite, false);

        }
    }
}



