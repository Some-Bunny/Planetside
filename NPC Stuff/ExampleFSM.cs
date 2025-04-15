using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.NPCAPI;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace BotsMod.NPCs
{
    // Token: 0x0200012B RID: 299
    internal class ExampleFsms
    {
        // Token: 0x060006E2 RID: 1762 RVA: 0x00072B00 File Offset: 0x00070D00
        private void Init()
        {
            GameObject gameObject = new GameObject();
            PlayMakerFSM playMakerFSM = NpcAPI.CreateBlankPlayMakerFSM(gameObject, "Shopkeep");
            FsmState fsmState = PlayMakerExtensions.AddState(playMakerFSM, "Idle", true, false);
            FsmState fsmState2 = PlayMakerExtensions.AddState(playMakerFSM, "Check Coop", false, false);
            FsmState fsmState3 = PlayMakerExtensions.AddState(playMakerFSM, "Rebuke Coop", false, false);
            FsmState fsmState4 = PlayMakerExtensions.AddState(playMakerFSM, "Mode Switchboard", false, false);
            FsmState fsmState5 = PlayMakerExtensions.AddState(playMakerFSM, "End State 5", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "RESTART", "Idle", true);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "playerWalkedAway", "Idle", true);
            PlayMakerExtensions.AddTransition(fsmState, "playerInteract", "Check Coop", true);
            PlayMakerExtensions.AddAction(fsmState, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });
            PlayMakerExtensions.AddTransition(fsmState2, "isCoop", "Rebuke Coop", false);
            PlayMakerExtensions.AddTransition(fsmState2, "FINISHED", "Mode Switchboard", true);
            PlayMakerExtensions.AddAction(fsmState2, new CharacterClassSwitch
            {
                compareTo = new PlayableCharacters[]
                {
                    
                },
                sendEvent = new FsmEvent[]
                {
                    new FsmEvent("isCoop")
                }
            });
            PlayMakerExtensions.AddTransition(fsmState3, "FINISHED", "End State 5", true);
            PlayMakerExtensions.AddAction(fsmState3, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState3, new DialogueBox
            {
                condition = 0,
                sequence = 0,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#COOP_REBUKE"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.25f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState5, new RestartWhenFinished());
            PlayMakerExtensions.AddAction(fsmState4, new ModeSwitchboard());
            FsmState fsmState6 = PlayMakerExtensions.AddState(playMakerFSM, "Purchased Something", false, false);
            FsmState fsmState7 = PlayMakerExtensions.AddState(playMakerFSM, "Failed Purchase", false, false);
            FsmState fsmState8 = PlayMakerExtensions.AddState(playMakerFSM, "End Conversation 2", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "succeedPurchase", "Purchased Something", false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "failedPurchase", "Failed Purchase", false);
            PlayMakerExtensions.AddTransition(fsmState6, "FINISHED", "End Conversation 2", true);
            PlayMakerExtensions.AddAction(fsmState6, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState6, new DialogueBox
            {
                condition = 0,
                sequence = 0,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#EXAMPLE_SHOP_PURCHASED"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.5f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(fsmState7, "FINISHED", "End Conversation 2", true);
            PlayMakerExtensions.AddAction(fsmState7, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState7, new DialogueBox
            {
                condition = 0,
                sequence = 0,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#EXAMPLE_PURCHASE_FAILED"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.5f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState8, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });
            PlayMakerExtensions.AddAction(fsmState8, new RestartWhenFinished());
            FsmState fsmState9 = PlayMakerExtensions.AddState(playMakerFSM, "Entered Room", false, false);
            FsmState fsmState10 = PlayMakerExtensions.AddState(playMakerFSM, "Basic Conversation", false, false);
            FsmState fsmState11 = PlayMakerExtensions.AddState(playMakerFSM, "End Conversation", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "playerEnteredRoom", "Entered Room", true);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "modeBegin", "Basic Conversation", false);
            PlayMakerExtensions.AddTransition(fsmState9, "FINISHED", "End Conversation", true);
            PlayMakerExtensions.AddAction(fsmState9, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState9, new DialogueBox
            {
                condition = 0,
                sequence = 0,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#EXAMPLE_INTRO"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.5f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddTransition(fsmState10, "FINISHED", "End Conversation", true);
            PlayMakerExtensions.AddAction(fsmState10, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState10, new DialogueBox
            {
                condition = 0,
                sequence = DialogueBox.DialogueSequence.SeqThenRepeatLast,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#EXAMPLE_RUNBASEDMULTILINE_GENERIC"
                    },
                    new FsmString
                    {
                        value = "#EXAMPLE_RUNBASEDMULTILINE_STOPPER"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.5f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState11, new EndConversation
            {
                killZombieTextBoxes = false,
                doNotLerpCamera = false,
                suppressReinteractDelay = false,
                suppressFurtherInteraction = false
            });
            PlayMakerExtensions.AddAction(fsmState11, new RestartWhenFinished());
            FsmState fsmState12 = PlayMakerExtensions.AddState(playMakerFSM, "Go away thief!", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "modeCaughtStealing", "Go away thief!", false);
            PlayMakerExtensions.AddAction(fsmState12, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState12, new DialogueBox
            {
                condition = 0,
                sequence = 0,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#SHOP_GENERIC_NO_SALE_LABEL"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 1f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState12, new RestartWhenFinished());
            FsmState fsmState13 = PlayMakerExtensions.AddState(playMakerFSM, "Caught Stealing", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "caughtStealing", "Caught Stealing", false);
            PlayMakerExtensions.AddAction(fsmState13, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState13, new DialogueBox
            {
                condition = 0,
                sequence = DialogueBox.DialogueSequence.SeqThenRepeatLast,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#SUBSHOP_GENERIC_CAUGHT_STEALING"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 1f,
                zombieTime = 3f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerExtensions.AddAction(fsmState13, new RestartWhenFinished());
            FsmState fsmState14 = PlayMakerExtensions.AddState(playMakerFSM, "ouch", false, false);
            PlayMakerExtensions.AddGlobalTransition(playMakerFSM.Fsm, "takePlayerDamage", "ouch", true);
            PlayMakerExtensions.AddAction(fsmState14, new BeginConversation
            {
                conversationType = BeginConversation.ConversationType.Normal,
                locked = BeginConversation.LockedConversation.Default,
                overrideNpcScreenHeight = -1f,
                UsesCustomScreenBuffer = false,
                CustomScreenBuffer = Vector2.zero
            });
            PlayMakerExtensions.AddAction(fsmState14, new DialogueBox
            {
                condition = 0,
                sequence = DialogueBox.DialogueSequence.SeqThenRepeatLast,
                persistentStringsToShow = 1,
                dialogue = new FsmString[]
                {
                    new FsmString
                    {
                        value = "#EXAMPLE_TAKEPLAYERDAMAGE"
                    }
                },
                responses = new FsmString[0],
                skipWalkAwayEvent = false,
                forceCloseTime = 0.5f,
                zombieTime = 2f,
                SuppressDefaultAnims = false,
                OverrideTalkAnim = "",
                PlayBoxOnInteractingPlayer = false,
                IsThoughtBubble = false,
                AlternativeTalker = null
            });
            PlayMakerFSM playMakerFSM2 = NpcAPI.CreateBlankPlayMakerFSM(gameObject, "VampireLike");
        }
    }
}
