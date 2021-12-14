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
using MonoMod.Utils;
using Brave.BulletScript;
using GungeonAPI;
using SaveAPI;
using AnimationType = ItemAPI.EnemyBuilder.AnimationType;
using SpriteBuilder = ItemAPI.SpriteBuilder;
using DirectionType = DirectionalAnimation.DirectionType;
using static DirectionalAnimation;





namespace Planetside
{
    public class ForgiveMePlease : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Never Forgive Me";
            string resourceName = "Planetside/Resources/forgivemeplease.png";
            GameObject obj = new GameObject(itemName);
            ForgiveMePlease activeitem = obj.AddComponent<ForgiveMePlease>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Oh How They Fall!";
            string longDesc = "Imitates a hurt and slain foe, and imitates the users harm on the end of its lifespan.\n\nA sown, battered doll of a bullet kin with several voodoo pins on it. Its warm to the touch, as if it's alive...";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 666f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.B;
            activeitem.AddToSubShop(ItemBuilder.ShopType.Cursula, 1f);

            ForgiveMePlease.NeverForgiveMeID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int NeverForgiveMeID;
        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/ForgiveMePleaseDolls/fmg_testsprite_001", null, true);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            GameObject gameObject2 = new GameObject("Death Dolls");
            tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);

            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_convict_001", tk2dSprite.Collection));
            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_hunter_001", tk2dSprite.Collection));
            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_marine_001", tk2dSprite.Collection));
            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_pilot_001", tk2dSprite.Collection));

            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_bullet_001", tk2dSprite.Collection));
            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_robot_001", tk2dSprite.Collection));
            ForgiveMePlease.spriteIds.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/ForgiveMePleaseDolls/fmg_gunslinger_001", tk2dSprite.Collection));


            tk2dSprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            ForgiveMePlease.spriteIds.Add(tk2dSprite.spriteId);
            gameObject2.SetActive(false);

            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[0]); 
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[1]); //Convict
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[2]); //Hunter
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[3]); //Marine
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[4]); //Pilot

            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[5]); //Bullet
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[6]); //Robot
            tk2dSprite.SetSprite(ForgiveMePlease.spriteIds[7]); //Gunsliger

            FakePrefab.MarkAsFakePrefab(gameObject2);
            UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            ForgiveMePlease.FMGPrefab = gameObject2;
        }
        public static GameObject FMGPrefab;
        public static List<int> spriteIds = new List<int>();
        public GameObject objectToSpawn;
        public GameObject spawnedPlayerObject;

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }


        protected override void DoEffect(PlayerController user)
        {
            base.StartCoroutine(this.HandlePlaceDoll(user.CurrentRoom, user));

            
        }
        private IEnumerator HandlePlaceDoll(RoomHandler room, PlayerController player)
        {

            //This is Hunter levels of weird code
            //BUT it works, so take that i guess...
            bool isHuman;
            LootEngine.DoDefaultItemPoof(player.sprite.WorldCenter, false, true);
            GameObject fuck;
            fuck = UnityEngine.Object.Instantiate<GameObject>(ForgiveMePlease.FMGPrefab, player.CenterPosition, Quaternion.identity);
            fuck.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(player.sprite.WorldBottomCenter + new Vector2(0f, 00f), tk2dBaseSprite.Anchor.LowerCenter);
            tk2dSprite ahfuck = fuck.GetComponent<tk2dSprite>();
            if (this.LastOwner.characterIdentity == PlayableCharacters.Convict)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[1]);
                isHuman = true;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Guide)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[2]);
                isHuman = true;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Soldier)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[3]);
                isHuman = true;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Pilot)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[4]);
                isHuman = true;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Bullet)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[5]);
                isHuman = false;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Robot)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[6]);
                isHuman = false;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Gunslinger)
            {
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[7]);
                isHuman = true;
            }
            else if (this.LastOwner.characterIdentity == PlayableCharacters.Eevee)
            {
                int SpriteID = UnityEngine.Random.Range(1, 8);
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[SpriteID]);
                ahfuck.sprite.usesOverrideMaterial = true;
                var texture = ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\nebula_reducednoise.png");
                ahfuck.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
                ahfuck.sprite.renderer.material.SetTexture("_EeveeTex", texture);
                ahfuck.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
                ahfuck.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
                if (SpriteID == 5 | SpriteID == 6)
                {
                    isHuman = false;
                }
                else
                {
                    isHuman = true;
                }
            }
            else
            {
                int SpriteID = UnityEngine.Random.Range(1, 8);
                fuck.GetComponent<tk2dBaseSprite>().SetSprite(ForgiveMePlease.spriteIds[SpriteID]);
                if (SpriteID == 5 | SpriteID == 6)
                {
                    isHuman = false;
                }
                else
                {
                    isHuman = true;
                }
            }
            string EnemyToUse;
            if (isHuman)
            {
                EnemyToUse = "blobulin";
            }
            else
            {
                EnemyToUse = "mouser";
            }

            SpriteOutlineManager.AddOutlineToSprite(ahfuck.sprite, Color.black);

            string enemyGuid = EnemyGuidDatabase.Entries[EnemyToUse];
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);
            

            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, ahfuck.sprite.WorldBottomCenter, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(ahfuck.sprite.WorldBottomCenter.ToIntVector2()), true, AIActor.AwakenAnimationType.Default, true);
            aiactor.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
            aiactor.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
            aiactor.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
            aiactor.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
            aiactor.healthHaver.ForceSetCurrentHealth(100f);
            aiactor.healthHaver.SetHealthMaximum(100f, null, false);
            aiactor.sprite.renderer.enabled = false;
            //aiactor.aiShooter.ToggleGunAndHandRenderers(false, "its a doll");
            aiactor.procedurallyOutlined = false;
            aiactor.CorpseObject = null;
            aiactor.ToggleShadowVisiblity(false);
            aiactor.HasShadow = false;
            aiactor.ImmuneToAllEffects = true;
            aiactor.behaviorSpeculator.ImmuneToStun = true;
            aiactor.SetIsFlying(true, "SpooPOPP");
            aiactor.CanTargetEnemies = false;
            aiactor.CanTargetPlayers = false;
            aiactor.CompanionOwner = player;
            aiactor.HitByEnemyBullets = false;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);

            aiactor.IsHarmlessEnemy = true;
            aiactor.IgnoreForRoomClear = true;
            aiactor.PreventAutoKillOnBossDeath = true;

            aiactor.ManualKnockbackHandling = true; //dunno if this is useful
            aiactor.knockbackDoer.SetImmobile(true, "j"); // from the TetherBehavior to prevent the companion from being pushed by explosions
            aiactor.PreventFallingInPitsEver = true;

            aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker));

            aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));
            for (int i = 0; i < 8; i++)
            {
                if (player != null)
                {
                    Action<float, bool, HealthHaver> aaa = player.OnAnyEnemyReceivedDamage;
                    for (int e = 0; e < 4; e++)
                    {
                        aaa.Invoke(1f, false, aiactor.healthHaver);
                    }
                    aaa.Invoke(10f, true, aiactor.healthHaver);
                }
                LootEngine.DoDefaultPurplePoof(ahfuck.sprite.WorldCenter, false);
                AkSoundEngine.PostEvent("Play_PET_junk_splat_01", base.gameObject);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.75f);
            LootEngine.DoDefaultPurplePoof(ahfuck.sprite.WorldCenter, false);
            float currentHealth = player.healthHaver.GetCurrentHealth();
            bool nextShotKills = player.healthHaver.NextShotKills;
            if (nextShotKills)
            {
                bool flag22 = player.healthHaver.Armor >= 1f || player.healthHaver.HasCrest;
                if (flag22)
                {
                    bool flag23 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                    if (flag23)
                    {
                        player.healthHaver.Armor += 1f;
                        player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Weak.", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                        GameManager.Instance.StartCoroutine(this.SaveFlawless());
                    }
                    else
                    {
                        player.healthHaver.Armor += 1f;
                        player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Die.", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                    }
                }
                else
                {
                    bool flag24 = currentHealth == 0.5f;
                    if (flag24)
                    {
                        bool flag25 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                        if (flag25)
                        {
                            player.healthHaver.ForceSetCurrentHealth(currentHealth + 0.5f);
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Break Beneath Me", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            GameManager.Instance.StartCoroutine(this.SaveFlawless());
                        }
                        else
                        {
                            player.healthHaver.ForceSetCurrentHealth(currentHealth + 0.5f);
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Return To Dust", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                        }
                    }
                    else
                    {
                        bool flag26 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                        if (flag26)
                        {
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Eat Shit", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            float currentHealth8 = player.healthHaver.GetCurrentHealth();
                            player.healthHaver.ForceSetCurrentHealth(currentHealth8 + 0.5f);
                            GameManager.Instance.StartCoroutine(this.SaveFlawless());
                        }
                        else
                        {
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Become Memories", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            float currentHealth9 = player.healthHaver.GetCurrentHealth();
                            player.healthHaver.ForceSetCurrentHealth(currentHealth9 + 0.5f);
                        }
                    }
                }

            }
            else
            {
                bool tt = player.healthHaver.Armor >= 1f || player.healthHaver.HasCrest;
                if (tt)
                {
                    bool flag23 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                    if (flag23)
                    {
                        player.healthHaver.Armor += 1f;
                        player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                        GameManager.Instance.StartCoroutine(this.SaveFlawless());
                    }
                    else
                    {
                        player.healthHaver.Armor += 1f;
                        player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                    }
                }
                else
                {
                    bool flag24 = currentHealth == 0.5f;
                    if (flag24)
                    {
                        bool flag25 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                        if (flag25)
                        {
                            player.healthHaver.ForceSetCurrentHealth(currentHealth + 0.5f);
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            GameManager.Instance.StartCoroutine(this.SaveFlawless());
                        }
                        else
                        {
                            player.healthHaver.ForceSetCurrentHealth(currentHealth + 0.5f);
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                        }
                    }
                    else
                    {
                        bool flag26 = player.CurrentRoom != null && !player.CurrentRoom.PlayerHasTakenDamageInThisRoom;
                        if (flag26)
                        {
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            float currentHealth8 = player.healthHaver.GetCurrentHealth();
                            player.healthHaver.ForceSetCurrentHealth(currentHealth8 + 0.5f);
                            GameManager.Instance.StartCoroutine(this.SaveFlawless());
                        }
                        else
                        {
                            player.healthHaver.ApplyDamage(0.5f, Vector2.zero, "Invoker... how?", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
                            float currentHealth9 = player.healthHaver.GetCurrentHealth();
                            player.healthHaver.ForceSetCurrentHealth(currentHealth9 + 0.5f);
                        }
                    }
                }
            }
            aiactor.healthHaver.ApplyDamage(1000, Vector2.zero, "fuck off", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

            UnityEngine.Object.Destroy(fuck);
            yield break;

        }

        private IEnumerator SaveFlawless()
        {
            yield return new WaitForSeconds(0.1f);
            PlayerController player = this.LastOwner;
            bool flag = player.CurrentRoom != null;
            if (flag)
            {
                player.CurrentRoom.PlayerHasTakenDamageInThisRoom = false;
            }
            yield break;
        }
    }
}



