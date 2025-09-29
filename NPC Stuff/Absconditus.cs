using Alexandria.NPCAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Planetside.StaticVFXStorage;

namespace Planetside.NPC_Stuff
{
    public class Absconditus
    {
        public static void Init()
        {



            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            GameObject talkPoint = PrefabBuilder.BuildObject("SpeechPoint");
            talkPoint.transform.position = new Vector3(1.5f, 2f);

            GameObject npcObject = PrefabBuilder.BuildObject("PSOG:Absconditus");
            npcObject.layer = 22;


            var sprite = npcObject.AddComponent<tk2dSprite>();
            sprite.SetSprite(StaticSpriteDefinitions.NPC_Sheet_Data, "absconditus_hidden_001");
            sprite.IsPerpendicular = false;
            talkPoint.transform.parent = npcObject.transform;

            ItemAPI.FakePrefab.MarkAsFakePrefab(npcObject);
            UnityEngine.Object.DontDestroyOnLoad(npcObject);
            npcObject.SetActive(value: true);


            sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 3f);
            mat.SetFloat("_EmissivePower", 50);
            sprite.renderer.material = mat;



            tk2dSpriteAnimator tk2dSpriteAnimator = npcObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimator.library = StaticSpriteDefinitions.NPC_Animation_Data;
            tk2dSpriteAnimator.defaultClipId = StaticSpriteDefinitions.NPC_Animation_Data.GetClipIdByName("abscond_hidden");

            var speculativeRigidbody = npcObject.CreateFastBody(new IntVector2(32, 32), new IntVector2(0, 0), CollisionLayer.PlayerBlocker);
            npcObject.CreateFastBody(new IntVector2(32, 32), new IntVector2(0, 0), CollisionLayer.PlayerHitBox);

            speculativeRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker));
            TalkDoerLite talkDoerLite = npcObject.AddComponent<TalkDoerLite>();
            talkDoerLite.placeableWidth = 4;
            talkDoerLite.placeableHeight = 3;
            talkDoerLite.difficulty = DungeonPlaceableBehaviour.PlaceableDifficulty.BASE;
            talkDoerLite.isPassable = true;

            talkDoerLite.usesOverrideInteractionRegion = false;
            talkDoerLite.overrideRegionOffset = Vector2.zero;
            talkDoerLite.overrideRegionDimensions = Vector2.zero;
            talkDoerLite.overrideInteractionRadius = -1f;
            talkDoerLite.PreventInteraction = false;
            talkDoerLite.AllowPlayerToPassEventually = true;
            talkDoerLite.speakPoint = talkPoint.transform;
            talkDoerLite.SpeaksGleepGlorpenese = false;
            talkDoerLite.audioCharacterSpeechTag = ShopAPI.ReturnVoiceBox(ShopAPI.VoiceBoxes.BRAT);
            talkDoerLite.playerApproachRadius = 5f;
            talkDoerLite.conversationBreakRadius = 5f;
            talkDoerLite.echo1 = null;
            talkDoerLite.echo2 = null;
            talkDoerLite.PreventCoopInteraction = false;
            talkDoerLite.IsPaletteSwapped = false;
            talkDoerLite.PaletteTexture = null;
            talkDoerLite.OutlineDepth = 0.5f;
            talkDoerLite.OutlineLuminanceCutoff = 0.05f;
            talkDoerLite.MovementSpeed = 3f;
            talkDoerLite.PathableTiles = CellTypes.FLOOR;
            UltraFortunesFavor ultraFortunesFavor = npcObject.AddComponent<UltraFortunesFavor>();
            ultraFortunesFavor.goopRadius = 2;
            ultraFortunesFavor.beamRadius = 2;
            ultraFortunesFavor.bulletRadius = 2;
            ultraFortunesFavor.bulletSpeedModifier = 0.8f;
            ultraFortunesFavor.vfxOffset = 0.625f;
            ultraFortunesFavor.sparkOctantVFX = assetBundle.LoadAsset<GameObject>("FortuneFavor_VFX_Spark");
            AIAnimator aIAnimator = ShopAPI.GenerateBlankAIAnimator(npcObject);
            aIAnimator.spriteAnimator = tk2dSpriteAnimator;
            aIAnimator.specRigidbody = speculativeRigidbody;


            aIAnimator.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.Single,
                Prefix = "idle",
                AnimNames = new string[1] { "abscond_hidden" },
                Flipped = new DirectionalAnimation.FlipType[1]
            };
            aIAnimator.TalkAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.Single,
                Prefix = "talk",
                AnimNames = new string[1] { "abscond_talk" },
                Flipped = new DirectionalAnimation.FlipType[1]
            };

            aIAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>()
            {
                new AIAnimator.NamedDirectionalAnimation()
                {
                    anim =  new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        Prefix = "abscond_idle",
                        AnimNames = new string[1] { "abscond_idle" },
                        Flipped = new DirectionalAnimation.FlipType[1]
                    },
                    name = "abscond_idle"
                },
                new AIAnimator.NamedDirectionalAnimation()
                {
                    anim =  new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        Prefix = "abscond_hide",
                        AnimNames = new string[1] { "abscond_hide" },
                        Flipped = new DirectionalAnimation.FlipType[1]
                    },
                    name = "abscond_hide"
                },
                new AIAnimator.NamedDirectionalAnimation()
                {
                    anim =  new DirectionalAnimation
                    {
                        Type = DirectionalAnimation.DirectionType.Single,
                        Prefix = "abscond_unhide",
                        AnimNames = new string[1] { "abscond_unhide" },
                        Flipped = new DirectionalAnimation.FlipType[1]
                    },
                    name = "abscond_unhide"
                },
            };

            npcObject.transform.parent = npcObject.transform;
            //npcObject.transform.localPosition = new Vector3(3.895f, -0.504f, -0.5f);
            //FsmInt fsmInt = PlayMakerExtensions.AddFsmInt(playMakerFSM, "npcNumber1", 15);
            //FsmFloat fsmFloat = PlayMakerExtensions.AddFsmFloat(playMakerFSM, "cost", 15f);
            
            

            ETGMod.Databases.Strings.Core.Set("#ABSCONDITUS_REUSE", "LEAVE.");




            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_TALK_1", "LEAVE, PLANAR BEING.");
            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_TALK_1", "GO.");
            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_TALK_1", "LEAVE.");


            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_ASK_1", "PLANAR BEING, LEAVE ME BE.");
            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_ASK_2", "MY FLESH IS OLD AND SOFT, BROKEN BY TIME.");
            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_ASK_3", "UNLESS YOU HAVE SOME VITALITY TO SPARE..?");
            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_ASK_4", "A TRADE, MAYBE?");

            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_CHOICHE", "SOME FLESH FOR A KEY..?");

            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_YEA", "Sure..?  <Give [sprite \"heart_big_idle_001\"]>");
            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_YEA_ARMOR", "OK.  <Give [sprite \"armor_money_icon_001\"]>");



            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_NAH", "No thanks..?");

            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_NOTHANKS", "SUIT... YOURSELF...");
            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_NOHEALTH", "TOO LITTLE... FLESH.");

            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_THANK_1", "THIS WILL DO.");
            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_THANK_1", "ANOTHER DAY...");
            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_THANK_1", "MY GRATITUDES...");


            ETGMod.Databases.Strings.Core.AddComplex("#ABSCONDUS_THANK_2", "THIS SHOULD BE USEFUL.");


            ETGMod.Databases.Strings.Core.Set("#ABSCONDUS_RAGE", "LEAVE ME BE.");

            //"[sprite \"armor_money_icon_001\"]"
            //[sprite \"heart_big_idle_001\"]

            //PlayMakerExtensions.AddTransition(greetState, "FINISHED", "Check If Given Item", true); //IF [Check If Given Item] is true, run state [FINISHED]

            //FINISHED is ran when an action is complete


            #region Interaction Logic
            PlayMakerFSM playMakerFSM = NpcAPI.CreateBlankPlayMakerFSM(npcObject, "Absconditus");
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "modeBegin", "Idle", false); //Start     [GOES TO GREET]
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "takePlayerDamage", "Damaged", true);

            FsmBool boolVariable = PlayMakerExtensions.AddFsmBool(playMakerFSM, "hasYapped", false);
            FsmBool boolVariableDrained = PlayMakerExtensions.AddFsmBool(playMakerFSM, "Drained", false);


            //PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "playerWalkedAway", "Idle", true);


            //

            FsmState fsmState_hidden = PlayMakerExtensions.AddState(playMakerFSM, "Idle", true, false);
            PlayMakerExtensions.AddAction(fsmState_hidden, new PlayBraveAnimation
            {
                GameObject = new FsmOwnerDefault
                {
                    OwnerOption = OwnerDefaultOption.UseOwner
                },
                animName = "abscond_hidden",
                mode = PlayBraveAnimation.PlayMode.UntilCancelled,
                duration = 0f,
                dontPlayIfPlaying = false,
                next = PlayBraveAnimation.NextMode.ReturnToPrevious,
                nextAnimName = "",
                waitTime = 0f,
                playOnOtherTalkDoerInRoom = false
            });


            PlayMakerExtensions.AddTransition(fsmState_hidden, "playerInteract", "Open", true); // during state [IDLE], if player interacts, runs [SET COST]
            
            
            FsmState fsmState = PlayMakerExtensions.AddState(playMakerFSM, "Open", false, false);

            PlayMakerExtensions.AddAction(fsmState, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });
            PlayMakerExtensions.AddAction(fsmState, new PlayBraveAnimation
            {
                GameObject = new FsmOwnerDefault
                {
                    OwnerOption = OwnerDefaultOption.UseOwner
                },
                animName = "abscond_unhide",
                mode = PlayBraveAnimation.PlayMode.UntilFinished,
                duration = 0f,
                dontPlayIfPlaying = false,
                next = PlayBraveAnimation.NextMode.NewAnimation,
                nextAnimName = "abscond_idle",
                waitTime = 0f,
                playOnOtherTalkDoerInRoom = false
            });




            PlayMakerExtensions.AddAction(fsmState, new BoolTest
            {
                boolVariable = boolVariableDrained,
                isFalse = new FsmEvent("go_on"),
                isTrue = new FsmEvent("away"),
                everyFrame = false
            });
            PlayMakerExtensions.AddTransition(fsmState, "go_on", "boolcheck1", true);
            PlayMakerExtensions.AddTransition(fsmState, "away", "isSucced", true);

            FsmState isSucced = PlayMakerExtensions.AddState(playMakerFSM, "isSucced", false, false);
            PlayMakerExtensions.AddAction(isSucced, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Unlocked,
                conversationType = BeginConversation.ConversationType.Passive,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });

            PlayMakerExtensions.AddAction(isSucced, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_THANK_1"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = true,
                forceCloseTime = 2f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(isSucced, "FINISHED", "CloseUp", true);


            FsmState fsmState_ = PlayMakerExtensions.AddState(playMakerFSM, "boolcheck1", false, false);
            PlayMakerExtensions.AddAction(fsmState_, new BoolTest
            {
                boolVariable = boolVariable,
                isFalse = new FsmEvent("Dialogue"),
                isTrue = new FsmEvent("Selection"),
                everyFrame = false
            });
            PlayMakerExtensions.AddTransition(fsmState_, "Dialogue", "Dialogue", true); // during state [IDLE], if player interacts, runs [SET COST]
            PlayMakerExtensions.AddTransition(fsmState_, "Selection", "Selection", true); // during state [IDLE], if player interacts, runs [SET COST]


            FsmState fsmState_Dialogue_1 = PlayMakerExtensions.AddState(playMakerFSM, "Dialogue", false, true);



            PlayMakerExtensions.AddAction(fsmState_Dialogue_1, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Locked,
                conversationType = BeginConversation.ConversationType.Normal,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero,
                
            });
            PlayMakerExtensions.AddAction(fsmState_Dialogue_1, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_ASK_1"
                    },
                    new FsmString
                    {
                        Value = "#ABSCONDUS_ASK_2"
                    },
                    new FsmString
                    {
                        Value = "#ABSCONDUS_ASK_3"
                    },
                    new FsmString
                    {
                        Value = "#ABSCONDUS_ASK_4"
                    },
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = false,
                forceCloseTime = 2.5f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(fsmState_Dialogue_1, "FINISHED", "Selection", true); // during state [IDLE], if player interacts, runs [SET COST]



            FsmState selection = PlayMakerExtensions.AddState(playMakerFSM, "Selection", false, false);



            PlayMakerExtensions.AddAction(selection, new SetBoolValue
            {
                boolValue = true,
                boolVariable = boolVariable,
                everyFrame = false,
            });

            PlayMakerExtensions.AddAction(selection, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Locked,
                conversationType = BeginConversation.ConversationType.Normal,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero,
            });

            PlayMakerExtensions.AddAction(selection, new BoolTestHealth
            {
                _true = new FsmEvent("armor"),
                _false = new FsmEvent("health"),
                everyFrame = false
            });
            PlayMakerExtensions.AddTransition(selection, "health", "finishUpTheJob_health", true); // during state [IDLE], if player interacts, runs [SET COST]
            PlayMakerExtensions.AddTransition(selection, "armor", "finishUpTheJob_armor", true); // during state [IDLE], if player interacts, runs [SET COST]





            FsmState Selection_Main_Health = PlayMakerExtensions.AddState(playMakerFSM, "finishUpTheJob_health", false, false);
            PlayMakerExtensions.AddAction(Selection_Main_Health, new AdvancedDialogueBox
            {
                condition = AdvancedDialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = AdvancedDialogueBox.DialogueSequence.Mutliline,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_CHOICHE"
                    }
                },
                responses = new FsmString[]
    {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_YEA"
                    },
                    new FsmString
                    {
                        Value = "#ABSCONDUS_NAH"
                    }
                },
                events = new FsmEvent[]
                {
                    new FsmEvent("accept"),
                    new FsmEvent("reject")
                },
                echo1Delay = 0.2f,
                echo1DisplayDuration = -1f,
                forceCloseEchoTextBoxesOnResponcesShown = false,
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(Selection_Main_Health, "accept", "CheckVitality", false);
            PlayMakerExtensions.AddTransition(Selection_Main_Health, "reject", "Reject", false);

            FsmState Selection_Main_Armor = PlayMakerExtensions.AddState(playMakerFSM, "finishUpTheJob_armor", false, false);
            PlayMakerExtensions.AddAction(Selection_Main_Armor, new AdvancedDialogueBox
            {
                condition = AdvancedDialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = AdvancedDialogueBox.DialogueSequence.Mutliline,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_CHOICHE"
                    }
                },
                responses = new FsmString[]
    {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_YEA_ARMOR"
                    },
                    new FsmString
                    {
                        Value = "#ABSCONDUS_NAH"
                    }
                },
                events = new FsmEvent[]
                {
                    new FsmEvent("accept"),
                    new FsmEvent("reject")
                },
                echo1Delay = 0.2f,
                echo1DisplayDuration = -1f,
                forceCloseEchoTextBoxesOnResponcesShown = false,
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(Selection_Main_Armor, "accept", "CheckVitality", false);
            PlayMakerExtensions.AddTransition(Selection_Main_Armor, "reject", "Reject", false);


            FsmState vibeCheck = PlayMakerExtensions.AddState(playMakerFSM, "CheckVitality", false, false);
            PlayMakerExtensions.AddAction(vibeCheck, new TakeHealth()
            { 
                success = new FsmEvent("doAwesome"),
                failure = new FsmEvent("nohp"),
                drained = new FsmEvent("isDrained"),
            });
            PlayMakerExtensions.AddTransition(vibeCheck, "doAwesome", "doAwesome", false);
            PlayMakerExtensions.AddTransition(vibeCheck, "nohp", "RejectTooLittle", false);
            PlayMakerExtensions.AddTransition(vibeCheck, "isDrained", "setisDrained", false);

            FsmState doAwesome = PlayMakerExtensions.AddState(playMakerFSM, "doAwesome", false, false);


            PlayMakerExtensions.AddAction(doAwesome, new Wait
            {
                time = 1.5f,
                finishEvent = new FsmEvent("done"),
                realTime = true
            });
            PlayMakerExtensions.AddTransition(doAwesome, "done", "finishUpTheJob", false);
            
            FsmState finishUpTheJob = PlayMakerExtensions.AddState(playMakerFSM, "finishUpTheJob", false, false);
            PlayMakerExtensions.AddAction(finishUpTheJob, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_THANK_2"
                    },
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = false,
                forceCloseTime = 3f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(finishUpTheJob, new RestartWhenFinished());

            PlayMakerExtensions.AddTransition(finishUpTheJob, "FINISHED", "CloseUp", true);




            FsmState setdrainedValue = PlayMakerExtensions.AddState(playMakerFSM, "setisDrained", false, false);
            PlayMakerExtensions.AddAction(setdrainedValue, new Wait
            {
                time = 1.5f,
                finishEvent = new FsmEvent("done"),
                realTime = true
            });
            PlayMakerExtensions.AddTransition(setdrainedValue, "done", "setdrainedValue_1", false);

            FsmState setdrainedValue_1 = PlayMakerExtensions.AddState(playMakerFSM, "setdrainedValue_1", false, false);

            PlayMakerExtensions.AddAction(setdrainedValue_1, new SetBoolValue
            {
                boolValue = true,
                boolVariable = boolVariableDrained,
                everyFrame = false,
            });
            PlayMakerExtensions.AddTransition(setdrainedValue_1, "FINISHED", "finishUpTheJob", false);


            FsmState tooLittleHP = PlayMakerExtensions.AddState(playMakerFSM, "RejectTooLittle", false, false);
            PlayMakerExtensions.AddAction(tooLittleHP, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Unlocked,
                conversationType = BeginConversation.ConversationType.Normal,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                
                CustomScreenBuffer = Vector2.zero,

            });
            PlayMakerExtensions.AddAction(tooLittleHP, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_NOHEALTH"
                    },
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = false,
                forceCloseTime = 2f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(tooLittleHP, "FINISHED", "CloseUp", true);



            FsmState nohealthLol = PlayMakerExtensions.AddState(playMakerFSM, "Reject", false, false);
            PlayMakerExtensions.AddAction(nohealthLol, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Unlocked,
                conversationType = BeginConversation.ConversationType.Normal,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero,

            });
            PlayMakerExtensions.AddAction(nohealthLol, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_NOTHANKS"
                    },
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = false,
                forceCloseTime = 3f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(nohealthLol, "FINISHED", "CloseUp", true); 





            FsmState healthminning = PlayMakerExtensions.AddState(playMakerFSM, "Reject", false, false);




            FsmState fsmState2 = PlayMakerExtensions.AddState(playMakerFSM, "CloseUp", false, true);
            PlayMakerExtensions.AddAction(fsmState2, new PlayBraveAnimation
            {
                GameObject = new FsmOwnerDefault
                {
                    OwnerOption = OwnerDefaultOption.UseOwner
                },
                animName = "abscond_hide",
                mode = PlayBraveAnimation.PlayMode.Duration,
                duration = 2f,
                dontPlayIfPlaying = false,
                next = PlayBraveAnimation.NextMode.NewAnimation,
                nextAnimName = "abscond_hidden",
                waitTime = 0f,
                playOnOtherTalkDoerInRoom = false
            });

            PlayMakerExtensions.AddAction(fsmState2, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });

            PlayMakerExtensions.AddTransition(fsmState2, "FINISHED", "Idle", true);


            FsmState fsmState_Damaged = PlayMakerExtensions.AddState(playMakerFSM, "Damaged", false, false);
            PlayMakerExtensions.AddAction(fsmState_Damaged, new BeginConversation
            {
                locked = BeginConversation.LockedConversation.Unlocked,
                conversationType = BeginConversation.ConversationType.Passive,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });

            PlayMakerExtensions.AddAction(fsmState_Damaged, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#ABSCONDUS_RAGE"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[]
                {
                    new FsmEvent("FINISHED")
                },
                skipWalkAwayEvent = true,
                forceCloseTime = 2f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(fsmState_Damaged, "FINISHED", "CloseUp", true);


            /*
            # region SETCOST
            FsmState fsmState2 = PlayMakerExtensions.AddState(playMakerFSM, "SET COST", false, true);
            PlayMakerExtensions.AddAction(fsmState2, new SetIntValue
            {
                intVariable = fsmInt,
                intValue = 20,
                everyFrame = false
            });
            PlayMakerExtensions.AddAction(fsmState2, new ModifyVariableByInflationRate
            {
                TargetVariable = fsmInt,
                AdditionalMultiplier = 1f
            });
            PlayMakerExtensions.AddAction(fsmState2, new ConvertIntToFloat
            {
                intVariable = fsmInt,
                floatVariable = fsmFloat,
                everyFrame = false
            });
            */
            //PlayMakerExtensions.AddTransition(fsmState2, "FINISHED", "Mode Switchboard", true);
            //#endregion



            #endregion
            /*
            #region Player Dialogue
            FsmState fsmState3 = PlayMakerExtensions.AddState(playMakerFSM, "Mode Switchboard", false, false);

            PlayMakerExtensions.AddAction(fsmState3, new ModeSwitchboard());
            PlayMakerExtensions.AddAction(greetState, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Unlocked,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });

            #endregion

            */
            /*

            FsmState fsmState5 = PlayMakerExtensions.AddState(playMakerFSM, "Check If Given Item", false, false);
            FsmState fsmState6 = PlayMakerExtensions.AddState(playMakerFSM, "Has Given Item", false, false);
            FsmState fsmState7 = PlayMakerExtensions.AddState(playMakerFSM, "Check Money", false, false);
            FsmState fsmState8 = PlayMakerExtensions.AddState(playMakerFSM, "Choice", false, false);
            FsmState fsmState9 = PlayMakerExtensions.AddState(playMakerFSM, "No Money", false, false);
            FsmState fsmState10 = PlayMakerExtensions.AddState(playMakerFSM, "Accept", false, false);
            FsmState fsmState11 = PlayMakerExtensions.AddState(playMakerFSM, "Reject", false, false);
            FsmState fsmState12 = PlayMakerExtensions.AddState(playMakerFSM, "DoUpgradeAnim", false, false);
            FsmState fsmState13 = PlayMakerExtensions.AddState(playMakerFSM, "DoUpgrade", false, true);
            FsmState fsmState14 = PlayMakerExtensions.AddState(playMakerFSM, "End Conversation", false, false);
            
            

            PlayMakerExtensions.AddTransition(fsmState5, "hasntGivenItem", "Choice", false);
            PlayMakerExtensions.AddTransition(fsmState5, "hasGivenItem", "Has Given Item", false);
            PlayMakerExtensions.AddTransition(fsmState7, "hasCoins", "Accept", false);
            PlayMakerExtensions.AddTransition(fsmState7, "hasNoCoins", "No Money", false);
            PlayMakerExtensions.AddTransition(fsmState8, "accept", "Check Money", false);
            PlayMakerExtensions.AddTransition(fsmState8, "reject", "Reject", false);
            
            
            PlayMakerExtensions.AddTransition(fsmState10, "FINISHED", "DoUpgradeAnim", true);
            PlayMakerExtensions.AddTransition(fsmState13, "FINISHED", "End Conversation", true);
            PlayMakerExtensions.AddTransition(fsmState9, "FINISHED", "End Conversation", true);
            PlayMakerExtensions.AddTransition(fsmState11, "FINISHED", "End Conversation", true);
            PlayMakerExtensions.AddTransition(fsmState6, "FINISHED", "End Conversation", true);

            PlayMakerExtensions.AddTransition(fsmState12, "done", "DoUpgrade", false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "RESTART", "Idle", true);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "playerWalkedAway", "Idle", true);




            PlayMakerExtensions.AddAction(fsmState5, new BoolTest
            {
                boolVariable = boolVariable,
                isFalse = new FsmEvent("hasntGivenItem"),
                isTrue = new FsmEvent("hasGivenItem"),
                everyFrame = false
            });


            PlayMakerExtensions.AddAction(fsmState7, new TestConsumable
            {
                consumableType = BravePlayMakerUtility.ConsumableType.Currency,
                value = fsmFloat,
                greaterThan = new FsmEvent("hasCoins"),
                greaterThanOrEqual = new FsmEvent("hasCoins"),
                equal = new FsmEvent("hasCoins"),
                lessThan = new FsmEvent("hasNoCoins"),
                lessThanOrEqual = null,
                everyFrame = false
            });
            */
            /*
            PlayMakerExtensions.AddAction(fsmState6, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_REUSE"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });

            PlayMakerExtensions.AddAction(fsmState9, new AdvancedDialogueBox
            {
                condition = AdvancedDialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = AdvancedDialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_NO_MONEY"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[0],
                echo1Delay = 0.2f,
                echo1DisplayDuration = -1f,
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            */

            //Options box
            /*
            PlayMakerExtensions.AddAction(fsmState8, new AdvancedDialogueBox
            {
                condition = AdvancedDialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = AdvancedDialogueBox.DialogueSequence.Mutliline,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_OFFER"
                    }
                },
                responses = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_YES"
                    },
                    new FsmString
                    {
                        Value = "#SPIDER_NO"
                    }
                },
                events = new FsmEvent[]
                {
                    new FsmEvent("accept"),
                    new FsmEvent("reject")
                },
                echo1Delay = 0.2f,
                echo1DisplayDuration = -1f,
                forceCloseEchoTextBoxesOnResponcesShown = false,
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            */
            //

            /*
            PlayMakerExtensions.AddAction(fsmState10, new DialogueBox
            {
                condition = DialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = DialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_ACCEPTED_OFFER"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            var t = LootTableTools.CreateLootTable();
            t.AddItemToPool(69);



            PlayMakerExtensions.AddAction(fsmState12, new SetBoolValue
            {
                boolValue = true,
                boolVariable = boolVariable,
                everyFrame = false
            });
            PlayMakerExtensions.AddAction(fsmState12, new Wait
            {
                time = 1f,
                finishEvent = new FsmEvent("done"),
                realTime = false
            });
            PlayMakerExtensions.AddAction(fsmState13, new SpawnPickup
            {
                mode = SpawnPickup.Mode.LootTable,
                pickupId = -1,
                lootTable = t,
                spawnLocation = SpawnPickup.SpawnLocation.GiveToPlayer,
                spawnOffset = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState13, new TakeConsumable
            {
                consumableType = BravePlayMakerUtility.ConsumableType.Currency,
                amount = fsmFloat,
                success = new FsmEvent("FINISHED"),
                failure = new FsmEvent("FINISHED")
            });
            PlayMakerExtensions.AddAction(fsmState11, new AdvancedDialogueBox
            {
                condition = AdvancedDialogueBox.Condition.All,
                persistentStringsToShow = 1,
                sequence = AdvancedDialogueBox.DialogueSequence.Default,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        Value = "#SPIDER_REJECTED_OFFER"
                    }
                },
                responses = new FsmString[0],
                events = new FsmEvent[0],
                echo1Delay = 0.2f,
                echo1DisplayDuration = -1f,
                skipWalkAwayEvent = false,
                forceCloseTime = 0f,
                zombieTime = 0f,
                SuppressDefaultAnims = true,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState14, new RestartWhenFinished());
            PlayMakerExtensions.AddAction(fsmState14, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });
            */
            playMakerFSM.Fsm.InitData();

            ETGModConsole.ModdedNPCs.Add("psog:absconditus", npcObject);
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("psog:absconditus", npcObject);

            displayer = new PixelDisplayer(new List<PixelDisplayer.PixelItem>()
            {
                new PixelDisplayer.PixelItem(4,0),
                new PixelDisplayer.PixelItem(3,1),new PixelDisplayer.PixelItem(4,1),new PixelDisplayer.PixelItem(5,1),
                new PixelDisplayer.PixelItem(2,2),new PixelDisplayer.PixelItem(3,2),new PixelDisplayer.PixelItem(4,2),new PixelDisplayer.PixelItem(5,2),new PixelDisplayer.PixelItem(6,2),
                new PixelDisplayer.PixelItem(1,3),new PixelDisplayer.PixelItem(2,3),new PixelDisplayer.PixelItem(3,3),new PixelDisplayer.PixelItem(4,3),new PixelDisplayer.PixelItem(5,3),new PixelDisplayer.PixelItem(6,3),new PixelDisplayer.PixelItem(7,3),
                new PixelDisplayer.PixelItem(0,4),new PixelDisplayer.PixelItem(1,4),new PixelDisplayer.PixelItem(2,4),new PixelDisplayer.PixelItem(3,4),new PixelDisplayer.PixelItem(4,4),new PixelDisplayer.PixelItem(5,4),new PixelDisplayer.PixelItem(6,4),new PixelDisplayer.PixelItem(7,4),
                new PixelDisplayer.PixelItem(1,5),new PixelDisplayer.PixelItem(2,5),new PixelDisplayer.PixelItem(3,5),new PixelDisplayer.PixelItem(4,5),new PixelDisplayer.PixelItem(5,5),new PixelDisplayer.PixelItem(6,5),new PixelDisplayer.PixelItem(7,5),new PixelDisplayer.PixelItem(10,5),
                new PixelDisplayer.PixelItem(2,6),new PixelDisplayer.PixelItem(3,6),new PixelDisplayer.PixelItem(4,6),new PixelDisplayer.PixelItem(5,6),new PixelDisplayer.PixelItem(6,6),new PixelDisplayer.PixelItem(7,6),new PixelDisplayer.PixelItem(9,6),new PixelDisplayer.PixelItem(10,6),
                new PixelDisplayer.PixelItem(3,7),new PixelDisplayer.PixelItem(4,7),new PixelDisplayer.PixelItem(5,7),new PixelDisplayer.PixelItem(6,7),new PixelDisplayer.PixelItem(7,7),new PixelDisplayer.PixelItem(8,7),new PixelDisplayer.PixelItem(9,7),new PixelDisplayer.PixelItem(10,7),
                new PixelDisplayer.PixelItem(7,8),new PixelDisplayer.PixelItem(8,8),new PixelDisplayer.PixelItem(9,8),new PixelDisplayer.PixelItem(11,8),new PixelDisplayer.PixelItem(12,8),new PixelDisplayer.PixelItem(13,8),
                new PixelDisplayer.PixelItem(8,9),new PixelDisplayer.PixelItem(9,9),new PixelDisplayer.PixelItem(10,9),new PixelDisplayer.PixelItem(11,9),new PixelDisplayer.PixelItem(12,9),new PixelDisplayer.PixelItem(13,9),
                new PixelDisplayer.PixelItem(9,10),new PixelDisplayer.PixelItem(10,10),new PixelDisplayer.PixelItem(11,10),new PixelDisplayer.PixelItem(12,10),
                new PixelDisplayer.PixelItem(10,11),new PixelDisplayer.PixelItem(11,11),
            }, false);
            displayer.TotalSpawnTime = 1;
            displayer.OnComplete += OF;

        }
        public static PixelDisplayer displayer;

        public static void OF()
        {
            var player = GameManager.Instance.GetActivePlayerClosestToPoint(displayer.pixelRows[0].Instance.transform.position);
            foreach (var entry in displayer.pixelRows)
            {
                GameManager.Instance.StartCoroutine(OnFinish(entry));
            }
            GameManager.Instance.StartCoroutine(OnFinishMain(player));
        }
        public static IEnumerator OnFinish(PixelDisplayer.PixelItem pixelItem)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.35f, 0.65f));
            float e = 0;
            Vector3 p = pixelItem.Instance.transform.position;
            Vector3 scale = pixelItem.Instance.transform.localScale;
            var player = GameManager.Instance.GetActivePlayerClosestToPoint(p).transform.position + new Vector3(0.5f, 0.5f);
            while (e < 1)
            {
                e += Time.deltaTime * 2;
                pixelItem.Instance.transform.position = Vector3.Lerp(p, player, e * e);
                pixelItem.Instance.transform.localScale = Vector3.Lerp(scale, Vector3.zero, e * e);
                yield return null;
            }
            UnityEngine.Object.Destroy(pixelItem.Instance);
            yield break;
        }
        public static IEnumerator OnFinishMain(PlayerController playerController)
        {
            yield return new WaitForSeconds(1.125f);
            AkSoundEngine.PostEvent("Play_OBJ_key_pickup_01", playerController.gameObject);
            playerController.carriedConsumables.KeyBullets++;
            var I = playerController.HasPerk(CorruptedWealth.CorruptedWealthID);
            if (I != null)
            {
                (I as CorruptedWealth).AmountOfCorruptKeys++;
            }
            yield break;
        }

        public class TakeHealth : FsmStateAction
        {
            int uses = 2;
            public override void Reset()
            {
                uses = 2;
                this.failure = null;
            }

            public override string ErrorCheck()
            {
                string text = string.Empty;
                return text;
            }

            public override void OnEnter()
            {
                TalkDoerLite component = base.Owner.GetComponent<TalkDoerLite>();
                PlayerController talkingPlayer = component.TalkingPlayer;

                if (talkingPlayer.ForceZeroHealthState)
                {

                    if (talkingPlayer.healthHaver.Armor > 1)
                    {
                        uses--;
                        if (uses <= 0)
                        {
                            base.Fsm.Event(this.drained);
                        }
                        displayer.StartPosition = base.Owner.transform.position + new Vector3(0, 2f);
                        displayer.Scale = 0.166f;
                        displayer.Initialise();

                        talkingPlayer.healthHaver.Armor -= 1;
                        base.Fsm.Event(this.success);
                    }
                    else
                    {
                        base.Fsm.Event(this.failure);
                    }
                }
                else
                {

                    if (talkingPlayer.healthHaver.currentHealth > 1)
                    {

                        //BravePlayMakerUtility.SetConsumableValue(talkingPlayer, this.consumableType, consumableValue - this.amount.Value);
                        uses--;
                        displayer.StartPosition = base.Owner.transform.position + new Vector3(0, 2f);
                        displayer.Scale = 0.166f;

                        displayer.Initialise();
                        if (uses <= 0)
                        {
                            base.Fsm.Event(this.drained);
                        }
                        talkingPlayer.healthHaver.currentHealth -= 1;

                        base.Fsm.Event(this.success);
                    }
                    else
                    {

                        base.Fsm.Event(this.failure);
                    }
                }

                base.Finish();
            }


            public FsmEvent success;

            public FsmEvent failure;

            public FsmEvent drained;

        }

        public class BoolTestHealth : FsmStateAction
        {
            public override void Reset()
            {

                this.everyFrame = false;
            }

            public override void OnEnter()
            {
                TalkDoerLite component = base.Owner.GetComponent<TalkDoerLite>();
                PlayerController talkingPlayer = component.TalkingPlayer;

                if (talkingPlayer.ForceZeroHealthState)
                {
                    base.Fsm.Event(this._true);
                }
                else
                {
                    base.Fsm.Event(this._false);
                }
            }


            [UIHint(UIHint.Variable)]
            [RequiredField]
            public FsmString Text;


            public FsmEvent _true;
            public FsmEvent _false;

            public bool everyFrame;
        }

    }
}
