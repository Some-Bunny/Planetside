using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.NPCAPI;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



namespace Planetside
{

    public class Spider
    {

        public static void InitSpider()
        {
            //Augments.Init();
            GenericLootTable lootTable = LootUtility.CreateLootTable(null, null);
            foreach (int poID in AlexandriaTags.GetAllItemsIdsWithTag("cyborg_gift"))
            {
                lootTable.AddItemToPool(poID, 1f);
            }
            GameObject gameObject = new GameObject();// = NpcInit.InitLittleShit();
            List<string> list = new List<string>
            {
                "cybord_spider_npc_idle_001.png",
                "cybord_spider_npc_idle_002.png",
                "cybord_spider_npc_idle_003.png",
                "cybord_spider_npc_idle_004.png",
                "cybord_spider_npc_idle_005.png",
                "cybord_spider_npc_idle_006.png",
                "cybord_spider_npc_idle_007.png",
                "cybord_spider_npc_idle_008.png",
                "cybord_spider_npc_idle_009.png",
                "cybord_spider_npc_idle_010.png",
                "cybord_spider_npc_idle_011.png",
                "cybord_spider_npc_idle_012.png",
                "cybord_spider_npc_idle_013.png",
                "cybord_spider_npc_idle_014.png",
                "cybord_spider_npc_idle_015.png",
                "cybord_spider_npc_idle_016.png",
                "cybord_spider_npc_idle_017.png",
                "cybord_spider_npc_idle_018.png",
                "cybord_spider_npc_idle_019.png",
                "cybord_spider_npc_idle_020.png",
                "cybord_spider_npc_idle_021.png",
                "cybord_spider_npc_idle_022.png"
            };
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = "BotsMod/sprites/Npcs/Spider/" + list[i];
            }
            ETGMod.Databases.Strings.Core.AddComplex("#SPIDER_OFFER", "We offer augmentations... &for both weapons and their users.");
            ETGMod.Databases.Strings.Core.AddComplex("#SPIDER_OFFER", "Would you be interesting... &in purchasing one?");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_ACCEPTED_OFFER", "Very well.");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_REJECTED_OFFER", "Your mistake. &Dumb fuck.");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_REUSE", "We are sorry but that is all... &we can offer you.");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_YES", "Sure? <Pay %NPCNUMBER1%CURRENCY_SYMBOL>");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_NO", "Nah");
            ETGMod.Databases.Strings.Core.Set("#SPIDER_NO_MONEY", "This is not a charity... &you will need more than that!");
            GameObject gameObject2 = ShopAPI.SetUpNPC("spider", "bot", list, 16, list, 16, new Vector3(1.1435f, 2.7875f, -1.31f), Vector2.zero, ShopAPI.VoiceBoxes.BRAT, 2f, null, null);
            gameObject2.GetComponent<TalkDoerLite>().echo1 = gameObject.GetComponent<TalkDoerLite>();
            gameObject.transform.parent = gameObject2.transform;
            gameObject.transform.localPosition = new Vector3(3.895f, -0.504f, -0.5f);
            
            PlayMakerFSM playMakerFSM = NpcAPI.CreateBlankPlayMakerFSM(gameObject2, "Spider");
            FsmInt fsmInt = PlayMakerExtensions.AddFsmInt(playMakerFSM, "npcNumber1", 15);
            FsmFloat fsmFloat = PlayMakerExtensions.AddFsmFloat(playMakerFSM, "cost", 15f);
            FsmBool boolVariable = PlayMakerExtensions.AddFsmBool(playMakerFSM, "givenItem", false);

            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "hasCoins", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "hasNoCoins", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "accept", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "reject", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "done", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "hasntGivenItem", false);
            PlayMakerExtensions.AddEvent(playMakerFSM.fsm, "hasGivenItem", false);
            FsmState fsmState = PlayMakerExtensions.AddState(playMakerFSM, "Idle", true, false);
            FsmState fsmState2 = PlayMakerExtensions.AddState(playMakerFSM, "SET COST", false, true);
            FsmState fsmState3 = PlayMakerExtensions.AddState(playMakerFSM, "Mode Switchboard", false, false);
            FsmState fsmState4 = PlayMakerExtensions.AddState(playMakerFSM, "Greet", false, false);
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
            PlayMakerExtensions.AddTransition(fsmState, "playerInteract", "SET COST", true);
            PlayMakerExtensions.AddTransition(fsmState2, "FINISHED", "Mode Switchboard", true);
            PlayMakerExtensions.AddTransition(fsmState4, "FINISHED", "Check If Given Item", true);
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
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "modeBegin", "Greet", false);
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
                animName = "idle",
                mode = PlayBraveAnimation.PlayMode.UntilCancelled,
                duration = 0f,
                dontPlayIfPlaying = false,
                next = PlayBraveAnimation.NextMode.ReturnToPrevious,
                nextAnimName = "",
                waitTime = 0f,
                playOnOtherTalkDoerInRoom = false
            });
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
            PlayMakerExtensions.AddAction(fsmState3, new ModeSwitchboard());
            PlayMakerExtensions.AddAction(fsmState4, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
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
                lootTable = lootTable,
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
            playMakerFSM.Fsm.InitData();
            Material material = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            material.SetTexture("_MainTex", ETGMod.GetAnySprite(gameObject2.GetComponent<tk2dSprite>()).renderer.material.mainTexture);
            material.SetColor("_EmissiveColor", new Color32(byte.MaxValue, 69, 245, byte.MaxValue));
            material.SetFloat("_EmissiveColorPower", 1.55f);
            material.SetFloat("_EmissivePower", 55f);
            ETGMod.GetAnySprite(gameObject2.GetComponent<tk2dSprite>()).usesOverrideMaterial = true;
            ETGMod.GetAnySprite(gameObject2.GetComponent<tk2dSprite>()).renderer.material = new Material(material);
            ETGMod.GetAnySprite(gameObject2.GetComponent<tk2dSprite>()).renderer.material.SetFloat("_EmissivePower", 55f);
            tk2dSpriteAnimationClip clipByName = gameObject2.gameObject.GetComponentInChildren<tk2dSpriteAnimator>().Library.GetClipByName("idle");
            //clipByName.AddEmissiveLerpEventByFrame(0, 0.6875f, 3f);
            //clipByName.AddEmissiveLerpEventByFrame(11, 0.6875f, 55f);
            ETGModConsole.ModdedNPCs.Add("spider", gameObject2);
        }
    }

}
