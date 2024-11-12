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

//Garbage Code Incoming
namespace Planetside
{
    public class DeadKingsDesparation : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Dead Kings Desparation";
            string resourceName = "Planetside/Resources/deadkingsdesparation.png";
            GameObject obj = new GameObject(itemName);
            DeadKingsDesparation activeitem = obj.AddComponent<DeadKingsDesparation>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Strengths Falsified";
            string longDesc = "Steals the strength and resistances of all foes in the room.\n\nA simple war helmet worn by an old King, who in his final act of desparation stole the empowering trinkets the Challenger brought with them.";
            activeitem.SetupItem(shortDesc, longDesc, "psog");
            activeitem.SetCooldownType(ItemBuilder.CooldownType.Damage, 900f);
            activeitem.consumable = false;
            activeitem.quality = PickupObject.ItemQuality.A;



            var Collection = StaticSpriteDefinitions.Oddments_Sheet_Data;
            var FalseStrengthsFire = ItemBuilder.AddSpriteToObjectAssetbundle("False Strengths Fire", Collection.GetSpriteIdByName("falsestrengthfire"), Collection);
            FakePrefab.MarkAsFakePrefab(FalseStrengthsFire);
            UnityEngine.Object.DontDestroyOnLoad(FalseStrengthsFire);
            var tk2dSprite1 = FalseStrengthsFire.GetComponent<tk2dSprite>();
            Material mat = tk2dSprite1.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2dSprite1.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(104, 182, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            tk2dSprite1.sprite.renderer.material = mat;
            FalseStrengthsPrefabFire = FalseStrengthsFire;

            var FalseStrengthsPoison = ItemBuilder.AddSpriteToObjectAssetbundle("False Strengths Poison", Collection.GetSpriteIdByName("falsestrengthpoison"), Collection);
            FakePrefab.MarkAsFakePrefab(FalseStrengthsPoison);
            UnityEngine.Object.DontDestroyOnLoad(FalseStrengthsPoison);
            var tk2dSprite2 = FalseStrengthsPoison.GetComponent<tk2dSprite>();
            Material mat1 = tk2dSprite2.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2dSprite2.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(104, 182, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            tk2dSprite1.sprite.renderer.material = mat1;
            FalseStrengthsPrefabPoison = FalseStrengthsPoison;


            List<string> mandatoryConsoleIDs = new List<string>
            {
                "psog:dead_kings_desparation",
            };
            List<string> optionalConsoleIDs = new List<string>
            {
                "psog:executioners_crossbow",
                "shotgun_full_of_hate",
                "psog:death_warrant"
            };
            CustomSynergies.Add("Vermincide.", mandatoryConsoleIDs, optionalConsoleIDs, true);
            DeadKingsDesparation.DeadKingsDesparationID = activeitem.PickupObjectId;
            ItemIDs.AddToList(activeitem.PickupObjectId);

        }
        public static int DeadKingsDesparationID;
        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Planetside/Resources/VFX/MithrixCalldown/mithrixfalling_001", null, true);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            GameObject gameObject2 = new GameObject("Mithrix Calldown");
            tk2dSprite tk2dSprite = gameObject2.AddComponent<tk2dSprite>();
            tk2dSprite.SetSprite(gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId);

            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixfalling_001", tk2dSprite.Collection));

            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_001", tk2dSprite.Collection));
            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_002", tk2dSprite.Collection));
            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_003", tk2dSprite.Collection));
            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_004", tk2dSprite.Collection));
            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_005", tk2dSprite.Collection));
            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixland_006", tk2dSprite.Collection));

            DeadKingsDesparation.spriteIds1.Add(SpriteBuilder.AddSpriteToCollection("Planetside/Resources/VFX/MithrixCalldown/mithrixleap_001", tk2dSprite.Collection));


            Material mat = tk2dSprite.GetCurrentSpriteDef().material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2dSprite.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(104, 182, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1.55f);
            mat.SetFloat("_EmissivePower", 100);
            tk2dSprite.sprite.renderer.material = mat;

            DeadKingsDesparation.spriteIds1.Add(tk2dSprite.spriteId);
            gameObject2.SetActive(false);


            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[0]); //Mithrix Fall

            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[1]); //Mithrix Land
            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[2]);
            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[3]);
            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[4]);
            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[5]);
            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[6]);

            tk2dSprite.SetSprite(DeadKingsDesparation.spriteIds1[7]); //Mithrix Leap

            FakePrefab.MarkAsFakePrefab(gameObject2);
            UnityEngine.Object.DontDestroyOnLoad(gameObject2);
            DeadKingsDesparation.CalldownPrefab = gameObject2;
        }

        public static GameObject CalldownPrefab;
        public static List<int> spriteIds1 = new List<int>();

        private static GameObject FalseStrengthsPrefabFire;
        private static GameObject FalseStrengthsPrefabPoison;


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }


        public override void OnPreDrop(PlayerController user)
        {
            TrinketingAndBaubling = false;
            base.OnPreDrop(user);
        }


        public override bool CanBeUsed(PlayerController user)
        {
            if (user == null) { return false; }
            List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            return activeEnemies != null && this.TrinketingAndBaubling != true && user.IsInCombat;
        }



        public override void DoEffect(PlayerController user)
        {
            base.CanBeDropped = false;
            ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Combine(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
            AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Lockon_01", base.gameObject);
            this.TrinketingAndBaubling = true;
            this.m_PoisonImmunity = new DamageTypeModifier();
            this.m_PoisonImmunity.damageMultiplier = 0f;
            this.m_PoisonImmunity.damageType = CoreDamageTypes.Poison;
            user.healthHaver.damageTypeModifiers.Add(this.m_PoisonImmunity);
            this.m_FireImmunity = new DamageTypeModifier();
            this.m_FireImmunity.damageMultiplier = 0f;
            this.m_FireImmunity.damageType = CoreDamageTypes.Fire;
            user.healthHaver.damageTypeModifiers.Add(this.m_FireImmunity);

            user.OnRoomClearEvent += this.onRoomCleared;
            RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null)
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    this.AffectEnemy(activeEnemies[i], user);
                }
                if (user.PlayerHasActiveSynergy("Vermincide."))
                {
                    base.StartCoroutine(this.HandlePlaceDoll(user.CurrentRoom, user));
                }
            }
        }
        private IEnumerator HandlePlaceDoll(RoomHandler room, PlayerController player)
        {
            List<AIActor> activeEnemies1 = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies1 != null)
            {
                AIActor randomActiveEnemy1 = player.CurrentRoom.GetRandomActiveEnemy(true);
                Vector2 targetPoint = randomActiveEnemy1.sprite.WorldBottomCenter;
                {
                    GameObject fuck = UnityEngine.Object.Instantiate<GameObject>(DeadKingsDesparation.CalldownPrefab, targetPoint + new Vector2(0, 30), Quaternion.identity);
                    fuck.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(targetPoint + new Vector2(0f, 30f), tk2dBaseSprite.Anchor.LowerCenter);

                    AkSoundEngine.PostEvent("Play_BOSS_doormimic_land_01", base.gameObject);
                    fuck.GetComponent<tk2dBaseSprite>().SetSprite(DeadKingsDesparation.spriteIds1[0]);


                    float Time = 0.7f;
                    float elapsed = 0;
                    while (elapsed < Time)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = (float)elapsed / (float)Time;
                        Vector3 pos = Vector3.Lerp(targetPoint + new Vector2(0f, 30f), targetPoint, t);
                        fuck.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(pos, tk2dBaseSprite.Anchor.LowerCenter);
                        yield return null;
                    }

                    {
                        AkSoundEngine.PostEvent("Play_ENM_rock_blast_01", base.gameObject);
                        Exploder.DoDistortionWave(targetPoint, 6f, 0.6f, 0.2f, 0.1f);
                        if (randomActiveEnemy1 != null)
                        {
                            randomActiveEnemy1.healthHaver.ApplyDamage(250f, Vector2.zero, "Erasure", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

                        }
                        for (int q = 0; q < 5; q++)
                        {

                            fuck.GetComponent<tk2dBaseSprite>().SetSprite(DeadKingsDesparation.spriteIds1[q]);
                            Time = 0.35f;
                            elapsed = 0;
                            while (elapsed < Time)
                            {
                                elapsed += BraveTime.DeltaTime;
                                float t = (float)elapsed / (float)Time;
                                //Vector3 pos = Vector3.Lerp(targetPoint + new Vector2(0f, 30f), targetPoint, t);
                                fuck.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(targetPoint, tk2dBaseSprite.Anchor.LowerCenter);
                                yield return null;
                            }
                        }
                        fuck.GetComponent<tk2dBaseSprite>().SetSprite(DeadKingsDesparation.spriteIds1[6]);
                        Exploder.DoDistortionWave(targetPoint, 6f, 0.6f, 0.2f, 0.1f);
                        AkSoundEngine.PostEvent("Play_BOSS_doormimic_land_01", base.gameObject);
                        fuck.GetComponent<tk2dBaseSprite>().SetSprite(DeadKingsDesparation.spriteIds1[7]);

                        Time = 1.5f;
                        elapsed = 0;
                        while (elapsed < Time)
                        {
                            elapsed += BraveTime.DeltaTime;
                            float t = (float)elapsed / (float)Time;
                            Vector3 pos = Vector3.Lerp(targetPoint, targetPoint + new Vector2(0f, 50f), t);
                            fuck.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(pos, tk2dBaseSprite.Anchor.LowerCenter);
                            yield return null;
                        }
                    }
                    UnityEngine.Object.Destroy(fuck);
                }
            }
            yield break;
        }
        private void onRoomCleared(PlayerController player)
        {
            base.CanBeDropped = true;
            player.OnRoomClearEvent -= this.onRoomCleared;
            ETGMod.AIActor.OnPreStart = (Action<AIActor>)Delegate.Remove(ETGMod.AIActor.OnPreStart, new Action<AIActor>(this.AIActorMods));
            this.TrinketingAndBaubling = false;
            player.healthHaver.damageTypeModifiers.Remove(this.m_FireImmunity);
            player.healthHaver.damageTypeModifiers.Remove(this.m_PoisonImmunity);
            foreach(StatModifier stat in StatExtra)
            {
                player.ownerlessStatModifiers.Remove(stat);
                player.stats.RecalculateStats(player, true, true);
            }

        }
        public void AIActorMods(AIActor target)
        {
            if (this.TrinketingAndBaubling)
            {
                if (target && target.aiActor && target.aiActor.EnemyGuid != null && base.LastOwner != null)
                {this.AffectEnemy(target, base.LastOwner); }
            }
        }

        public void AffectEnemy(AIActor target, PlayerController user)
        {
            if (target.IsNormalEnemy || !target.IsHarmlessEnemy)
            {
                if (target != null && base.LastOwner != null)
                {
                    target.healthHaver.SetHealthMaximum(target.healthHaver.GetMaxHealth() * 0.8f);

                    if (base.LastOwner != null)
                    {
                        float StatBooster = target.healthHaver.GetMaxHealth() * 0.0003f;
                        StatModifier item = new StatModifier
                        {
                            statToBoost = PlayerStats.StatType.Damage,
                            amount = StatBooster,
                            modifyType = StatModifier.ModifyMethod.ADDITIVE
                        };
                        StatExtra.Add(item);

                        base.LastOwner.ownerlessStatModifiers.Add(item);
                        base.LastOwner.stats.RecalculateStats(user, true, true);
                    }

                    GameActor gameActor = target.gameActor;
                    gameActor.EffectResistances = new ActorEffectResistance[]
                    {
                    new ActorEffectResistance
                    {
                        resistAmount = 0f,
                        resistType = EffectResistanceType.Poison
                    },
                    new ActorEffectResistance
                    {
                        resistAmount = 0f,
                        resistType = EffectResistanceType.Fire
                    }
                    };

                    var pp = new DamageTypeModifier();
                    pp.damageMultiplier = 3f;
                    pp.damageType = CoreDamageTypes.Poison;
                    gameActor.healthHaver.damageTypeModifiers.Add(pp);

                    var ff = new DamageTypeModifier();
                    ff.damageMultiplier = 3f;
                    ff.damageType = CoreDamageTypes.Fire;
                    gameActor.healthHaver.damageTypeModifiers.Add(ff);

                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleSuckStrengths(target, user.sprite.WorldCenter,UnityEngine.Random.Range(0.5f, 1.5f), 0));
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleSuckStrengths(target, user.sprite.WorldCenter, UnityEngine.Random.Range(0.5f, 1.5f), 1));
                }

            }
        }
        private IEnumerator HandleSuckStrengths(AIActor target, Vector2 table, float DuartionForSteal, int UsesOneOrAnother)
        {
            if (base.LastOwner != null)
            {
                PlayerController player = GameManager.Instance.PrimaryPlayer;
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.5f));

                if (target == null) { yield break; }
                Exploder.DoDistortionWave(target.sprite.WorldCenter, 1f, 0.25f, 3, 0.066f);
                AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_01", base.gameObject);

                tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>(UsesOneOrAnother == 0 ? DeadKingsDesparation.FalseStrengthsPrefabFire : DeadKingsDesparation.FalseStrengthsPrefabPoison, target.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dSprite>();

                component.transform.parent = SpawnManager.Instance.VFX;
                GameObject gameObject2 = new GameObject("image parent");
                gameObject2.transform.position = component.WorldCenter;
                component.transform.parent = gameObject2.transform;

                Transform copySprite = gameObject2.transform;
                if (target == null) { yield break; }


                Vector3 startPosition = target.transform.position;
                float elapsed = 0f;
                float duration = DuartionForSteal;
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    if (player && copySprite && player != null)
                    {
                        Vector3 position = player.sprite.WorldCenter;
                        float t = elapsed / duration * (elapsed / duration);
                        copySprite.position = Vector3.Lerp(startPosition, position, t);
                        copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                        copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                        position = default(Vector3);
                    }
                    yield return null;
                }
                if (player == null && copySprite.gameObject != null)
                {
                    UnityEngine.Object.Destroy(copySprite.gameObject);
                    yield break;
                }
                if (copySprite.gameObject != null)
                {
                    UnityEngine.Object.Destroy(copySprite.gameObject);
                    yield break;
                }
                yield break;
            }
            else
            {
                yield break;
            }
        }
       


        private DamageTypeModifier m_PoisonImmunity;
        private DamageTypeModifier m_FireImmunity;

        public bool TrinketingAndBaubling;

        private List<StatModifier> StatExtra = new List<StatModifier>();

    }
}


