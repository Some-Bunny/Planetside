using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    // Token: 0x02000016 RID: 22
    [ActionCategory(".NPCs")]
    [Tooltip("Opens a dialogue box and speaks one or more lines of dialogue. Also supports one set of player responses.\nOnly the first valid Dialogue Box action will be run for a given state.")]
    public class AdvancedDialogueBox : FsmStateAction
    {
        // Token: 0x060000F4 RID: 244 RVA: 0x0000D920 File Offset: 0x0000BB20
        public override void Reset()
        {
            this.condition = AdvancedDialogueBox.Condition.All;
            this.sequence = AdvancedDialogueBox.DialogueSequence.Default;
            this.persistentStringsToShow = 1;
            this.dialogue = new FsmString[]
            {
                new FsmString(string.Empty)
            };
            this.responses = null;
            this.events = null;
            this.skipWalkAwayEvent = false;
            this.forceCloseTime = 0f;
            this.zombieTime = 0f;
            this.SuppressDefaultAnims = false;
            this.OverrideTalkAnim = string.Empty;
            this.PlayBoxOnInteractingPlayer = false;
            this.AlternativeTalker = null;
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x0000D9CC File Offset: 0x0000BBCC
        public override string ErrorCheck()
        {
            string text = string.Empty;
            AIAnimator component = base.Owner.GetComponent<AIAnimator>();
            bool flag = !this.SuppressDefaultAnims.Value;
            if (flag)
            {
                bool flag2 = !component;
                if (flag2)
                {
                    text += "Owner must have an AIAnimator to manage animations to use default animations.";
                }
                bool flag3 = component && !component.HasDefaultAnimation;
                if (flag3)
                {
                    text += "AIAnimator must have a default (base or idle) animation to use default animations.";
                }
                bool flag4 = component && !component.HasDirectionalAnimation("talk");
                if (flag4)
                {
                    text += "AIAnimator must have a talk animation to use default animations.";
                }
            }
            bool flag5 = this.sequence == AdvancedDialogueBox.DialogueSequence.Mutliline && this.dialogue.Length != 1;
            if (flag5)
            {
                text += "Multiline only supports a single dialogue string.\n";
            }
            bool flag6 = this.sequence == AdvancedDialogueBox.DialogueSequence.Sequential && this.dialogue.Length != 1;
            if (flag6)
            {
                text += "Sequential only supports a single dialogue string.\n";
            }
            bool flag7 = this.sequence == AdvancedDialogueBox.DialogueSequence.SeqThenRepeatLast && this.dialogue.Length != 1;
            if (flag7)
            {
                text += "SeqThenRepeatLast only supports a single dialogue string.\n";
            }
            bool flag8 = this.sequence == AdvancedDialogueBox.DialogueSequence.SeqThenRemoveState && this.dialogue.Length != 1;
            if (flag8)
            {
                text += "SeqThenRemoveState only supports a single dialogue string.\n";
            }
            bool flag9 = this.sequence == AdvancedDialogueBox.DialogueSequence.PersistentSequential && this.dialogue.Length < 2;
            if (flag9)
            {
                text += "PersistentSequential needs at least one sequential dialogue string and one stopper string.\n";
            }
            bool flag10 = this.dialogue != null && this.dialogue.Length == 0;
            if (flag10)
            {
                text += "Dialogue strings must contain at least one line of dialogue.\n";
            }
            bool flag11 = this.forceCloseTime.Value > 0f && this.responses != null && this.responses.Length != 0;
            if (flag11)
            {
                text += "Force Close Timer will be ignored if there are dialogue responses.\n";
            }
            return text;
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x0000DBC0 File Offset: 0x0000BDC0
        public override void OnEnter()
        {
            this.m_talkDoer = base.Owner.GetComponent<TalkDoerLite>();
            bool flag = this.ShouldSkip();
            if (flag)
            {
                base.Finish();
            }
            else
            {
                this.m_dialogueState = AdvancedDialogueBox.DialogueState.ShowNextDialogue;
                bool flag2 = this.sequence != AdvancedDialogueBox.DialogueSequence.PersistentSequential;
                if (flag2)
                {
                    this.m_textIndex = 0;
                }
                this.m_forceCloseTimer = 0f;
                bool value = this.skipWalkAwayEvent.Value;
                if (value)
                {
                    this.m_talkDoer.AllowWalkAways = false;
                }
                bool flag3 = this.sequence == AdvancedDialogueBox.DialogueSequence.Default;
                if (flag3)
                {
                    this.m_numDialogues = this.dialogue.Length;
                }
                else
                {
                    bool flag4 = this.sequence == AdvancedDialogueBox.DialogueSequence.Mutliline;
                    if (flag4)
                    {
                        this.m_numDialogues = StringTableManager.GetNumStrings(this.dialogue[0].Value);
                    }
                    else
                    {
                        this.m_numDialogues = 1;
                    }
                }
                this.m_rawResponses = new string[this.responses.Length];
                for (int i = 0; i < this.responses.Length; i++)
                {
                    this.m_rawResponses[i] = StringTableManager.GetString(this.responses[i].Value);
                    this.m_rawResponses[i] = this.NPCReplacementPostprocessString(this.m_rawResponses[i]);
                }
            }
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x0000DCF8 File Offset: 0x0000BEF8
        public override void OnUpdate()
        {
            bool flag = this.m_dialogueState == AdvancedDialogueBox.DialogueState.ShowNextDialogue;
            if (flag)
            {
                this.NextDialogue();
                this.m_dialogueState = AdvancedDialogueBox.DialogueState.ShowingDialogue;
                bool flag2 = !this.SuppressDefaultAnims.Value;
                if (flag2)
                {
                    bool flag3 = this.AlternativeTalker != null;
                    if (flag3)
                    {
                        this.AlternativeTalker.aiAnimator.PlayUntilFinished(this.TalkAnimName, false, null, -1f, false);
                    }
                    else
                    {
                        this.m_talkDoer.aiAnimator.PlayUntilFinished(this.TalkAnimName, false, null, -1f, false);
                    }
                }
            }
            else
            {
                bool flag4 = this.m_dialogueState == AdvancedDialogueBox.DialogueState.ShowingDialogue;
                if (flag4)
                {
                    bool flag5 = false;
                    bool flag6 = this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
                    if (flag6)
                    {
                        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX);
                        bool flag7;
                        flag5 = instanceForPlayer.WasAdvanceDialoguePressed(out flag7);
                        bool flag8 = flag7;
                        if (flag8)
                        {
                            this.m_talkDoer.TalkingPlayer.SuppressThisClick = true;
                        }
                    }
                    bool flag9 = false;
                    bool flag10 = this.m_forceCloseTimer > 0f;
                    if (flag10)
                    {
                        this.m_forceCloseTimer -= BraveTime.DeltaTime;
                        flag9 = (this.m_forceCloseTimer <= 0f);
                    }
                    bool flag11 = flag5 || flag9;
                    if (flag11)
                    {
                        bool flag12 = TextBoxManager.TextBoxCanBeAdvanced(this.m_talkDoer.speakPoint) && !flag9;
                        if (flag12)
                        {
                            TextBoxManager.AdvanceTextBox(this.m_talkDoer.speakPoint);
                        }
                        else
                        {
                            bool flag13 = this.m_textIndex < this.m_numDialogues && this.sequence != AdvancedDialogueBox.DialogueSequence.PersistentSequential;
                            if (flag13)
                            {
                                bool flag14 = this.m_talkDoer.echo1 != null;
                                if (flag14)
                                {
                                    this.m_talkDoer.echo1.IsDoingForcedSpeech = false;
                                }
                                bool flag15 = this.m_talkDoer.echo2 != null;
                                if (flag15)
                                {
                                    this.m_talkDoer.echo2.IsDoingForcedSpeech = false;
                                }
                                this.m_dialogueState = AdvancedDialogueBox.DialogueState.ShowNextDialogue;
                            }
                            else
                            {
                                bool flag16 = this.responses.Length != 0;
                                if (flag16)
                                {
                                    this.m_dialogueState = AdvancedDialogueBox.DialogueState.ShowingResponses;
                                }
                                else
                                {
                                    bool flag17 = this.forceCloseTime.Value != 0f && this.zombieTime.Value != 0f;
                                    if (flag17)
                                    {
                                        float a = this.forceCloseTime.Value + this.zombieTime.Value;
                                        float b = 0.5f + TextBoxManager.GetEstimatedReadingTime(this.m_currentDialogueText) * TextBoxManager.ZombieBoxMultiplier;
                                        float num = Mathf.Max(a, b);
                                        this.m_talkDoer.SetZombieBoxTimer(Mathf.Max(num - this.forceCloseTime.Value, 0.1f), this.TalkAnimName);
                                    }
                                    else
                                    {
                                        bool flag18 = !this.SuppressDefaultAnims.Value;
                                        if (flag18)
                                        {
                                            bool flag19 = this.AlternativeTalker != null;
                                            if (flag19)
                                            {
                                                this.AlternativeTalker.aiAnimator.EndAnimationIf(this.TalkAnimName);
                                            }
                                            else
                                            {
                                                this.m_talkDoer.aiAnimator.EndAnimationIf(this.TalkAnimName);
                                            }
                                        }
                                    }
                                    base.Finish();
                                }
                            }
                        }
                    }
                    else
                    {
                        bool flag20 = this.responses.Length != 0 && this.m_textIndex == this.m_numDialogues && !TextBoxManager.TextBoxCanBeAdvanced(this.m_talkDoer.speakPoint);
                        if (flag20)
                        {
                            this.m_dialogueState = AdvancedDialogueBox.DialogueState.ShowingResponses;
                        }
                    }
                }
                else
                {
                    bool flag21 = this.m_dialogueState == AdvancedDialogueBox.DialogueState.ShowingResponses;
                    if (flag21)
                    {
                        this.ShowResponses();
                        this.m_dialogueState = AdvancedDialogueBox.DialogueState.WaitingForResponse;
                    }
                    else
                    {
                        bool flag22 = this.m_dialogueState == AdvancedDialogueBox.DialogueState.WaitingForResponse;
                        if (flag22)
                        {
                            int num2;
                            bool flag23 = !GameUIRoot.Instance.GetPlayerConversationResponse(out num2);
                            if (!flag23)
                            {
                                this.m_talkDoer.TalkingPlayer.ClearInputOverride("dialogueResponse");
                                this.m_talkDoer.CloseTextBox(true);
                                base.Finish();
                                bool flag24 = !this.SuppressDefaultAnims.Value;
                                if (flag24)
                                {
                                    bool flag25 = this.AlternativeTalker != null;
                                    if (flag25)
                                    {
                                        this.AlternativeTalker.aiAnimator.EndAnimationIf(this.TalkAnimName);
                                    }
                                    else
                                    {
                                        this.m_talkDoer.aiAnimator.EndAnimationIf(this.TalkAnimName);
                                    }
                                }
                                base.Fsm.Event(this.events[num2]);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000E154 File Offset: 0x0000C354
        public override void OnExit()
        {
            bool flag = this.m_talkDoer;
            if (flag)
            {
                this.m_talkDoer.CloseTextBox(false);
                bool flag2 = this.m_talkDoer.echo1 != null;
                if (flag2)
                {
                    this.m_talkDoer.echo1.IsDoingForcedSpeech = false;
                }
                bool flag3 = this.m_talkDoer.echo2 != null;
                if (flag3)
                {
                    this.m_talkDoer.echo2.IsDoingForcedSpeech = false;
                }
                bool value = this.skipWalkAwayEvent.Value;
                if (value)
                {
                    this.m_talkDoer.AllowWalkAways = true;
                }
            }
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x0000E1F0 File Offset: 0x0000C3F0
        private bool ShouldSkip()
        {
            bool flag = this.condition == AdvancedDialogueBox.Condition.FirstEncounterThisInstance;
            if (flag)
            {
                bool flag2 = this.m_talkDoer.NumTimesSpokenTo > 1;
                if (flag2)
                {
                    return true;
                }
            }
            else
            {
                bool flag3 = this.condition == AdvancedDialogueBox.Condition.FirstEverEncounter;
                if (flag3)
                {
                    EncounterTrackable component = base.Owner.GetComponent<EncounterTrackable>();
                    bool flag4 = component == null;
                    if (flag4)
                    {
                        return true;
                    }
                    bool flag5 = GameStatsManager.Instance.QueryEncounterable(component) > 1;
                    if (flag5)
                    {
                        return true;
                    }
                }
                else
                {
                    bool flag6 = this.condition == AdvancedDialogueBox.Condition.KeyboardAndMouse;
                    if (flag6)
                    {
                        bool flag7 = !BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX).IsKeyboardAndMouse(false);
                        if (flag7)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        bool flag8 = this.condition == AdvancedDialogueBox.Condition.Controller && BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX).IsKeyboardAndMouse(false);
                        if (flag8)
                        {
                            return true;
                        }
                    }
                }
            }
            for (int i = 0; i < base.State.Actions.Length; i++)
            {
                bool flag9 = base.State.Actions[i] == this;
                if (flag9)
                {
                    break;
                }
                bool flag10 = base.State.Actions[i] is AdvancedDialogueBox && base.State.Actions[i].Active;
                if (flag10)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x060000FA RID: 250 RVA: 0x0000E36C File Offset: 0x0000C56C
        private string NPCReplacementPostprocessString(string input)
        {
            FsmString fsmString = base.Fsm.Variables.GetFsmString("npcReplacementString");
            bool flag = fsmString != null && !string.IsNullOrEmpty(fsmString.Value);
            if (flag)
            {
                input = input.Replace("%NPCREPLACEMENT", fsmString.Value);
            }
            string text = "%NPCNUMBER1";
            int num = 1;
            while (input.Contains(text))
            {
                FsmInt fsmInt = base.Fsm.Variables.GetFsmInt("npcNumber" + num.ToString());
                bool flag2 = fsmInt != null;
                if (flag2)
                {
                    input = input.Replace(text, fsmInt.Value.ToString());
                }
                num++;
                text = "%NPCNUMBER" + num.ToString();
            }
            return input;
        }

        // Token: 0x060000FB RID: 251 RVA: 0x0000E43C File Offset: 0x0000C63C
        private void NextDialogue()
        {
            bool flag = this.m_textIndex > 0;
            if (flag)
            {
            }
            bool flag2 = this.m_textIndex == this.m_numDialogues - 1;
            string text = "ERROR ERROR";
            bool flag3 = this.m_textIndex < this.dialogue.Length && this.m_textIndex >= 0 && this.dialogue[this.m_textIndex].UsesVariable && !this.dialogue[this.m_textIndex].Value.StartsWith("#");
            if (flag3)
            {
                text = this.dialogue[this.m_textIndex].Value;
            }
            else
            {
                bool flag4 = this.sequence == AdvancedDialogueBox.DialogueSequence.Default;
                if (flag4)
                {
                    text = StringTableManager.GetString(this.dialogue[this.m_textIndex].Value);
                }
                else
                {
                    bool flag5 = this.sequence == AdvancedDialogueBox.DialogueSequence.Mutliline;
                    if (flag5)
                    {
                        text = StringTableManager.GetExactString(this.dialogue[0].Value, this.m_textIndex);
                    }
                    else
                    {
                        bool flag6 = this.sequence == AdvancedDialogueBox.DialogueSequence.SeqThenRemoveState;
                        if (flag6)
                        {
                            bool flag7;
                            text = StringTableManager.GetStringSequential(this.dialogue[0].Value, ref this.m_sequentialStringLastIndex, out flag7, false);
                            bool flag8 = flag7;
                            if (flag8)
                            {
                                BravePlayMakerUtility.DisconnectState(base.State);
                            }
                        }
                        else
                        {
                            bool flag9 = this.sequence == AdvancedDialogueBox.DialogueSequence.Sequential || this.sequence == AdvancedDialogueBox.DialogueSequence.SeqThenRepeatLast;
                            if (flag9)
                            {
                                bool repeatLast = this.sequence == AdvancedDialogueBox.DialogueSequence.SeqThenRepeatLast;
                                text = StringTableManager.GetStringSequential(this.dialogue[0].Value, ref this.m_sequentialStringLastIndex, repeatLast);
                            }
                            else
                            {
                                bool flag10 = this.sequence == AdvancedDialogueBox.DialogueSequence.PersistentSequential;
                                if (flag10)
                                {
                                    bool flag11 = this.m_textIndex < this.dialogue.Length - 1;
                                    if (flag11)
                                    {
                                        text = StringTableManager.GetStringPersistentSequential(this.dialogue[this.m_textIndex].Value);
                                    }
                                    else
                                    {
                                        text = StringTableManager.GetString(this.dialogue[this.m_textIndex].Value);
                                        flag2 = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool flag12 = text.Contains("$");
            if (flag12)
            {
                string[] array = text.Split(new char[]
                {
                    '$'
                });
                text = array[0];
                bool flag13 = array.Length > 1;
                if (flag13)
                {
                    int num = 1;
                    while (num < array.Length && num - 1 < this.m_rawResponses.Length)
                    {
                        this.m_rawResponses[num - 1] = array[num];
                        num++;
                    }
                }
            }
            else
            {
                bool flag14 = text.Contains("&");
                if (flag14)
                {
                    string[] array2 = text.Split(new char[]
                    {
                        '&'
                    });
                    text = array2[0];
                    bool flag15 = this.m_talkDoer.echo1 != null;
                    if (flag15)
                    {
                        this.m_talkDoer.echo1.ForceTimedSpeech(array2[1], this.echo1Delay.Value, this.echo1DisplayDuration.Value, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT);
                    }
                    bool flag16 = this.m_talkDoer.echo2 != null && array2.Length > 2;
                    if (flag16)
                    {
                        this.m_talkDoer.echo2.ForceTimedSpeech(array2[2], this.echo2Delay.Value, this.echo1DisplayDuration.Value, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
                    }
                }
            }
            text = this.NPCReplacementPostprocessString(text);
            this.m_currentDialogueText = text;
            this.ClearAlternativeTalkerFromPrevious();
            bool flag17 = this.AlternativeTalker != null;
            if (flag17)
            {
                this.AlternativeTalker.SuppressClear = true;
                TextBoxManager.ClearTextBox(this.m_talkDoer.speakPoint);
                TextBoxManager.ClearTextBox(this.m_talkDoer.TalkingPlayer.transform);
                TalkDoerLite talkDoer = this.m_talkDoer;
                Vector3 worldPosition = this.AlternativeTalker.speakPoint.position + new Vector3(0f, 0f, -5f);
                Transform speakPoint = this.AlternativeTalker.speakPoint;
                float duration = -1f;
                string text2 = text;
                bool instant = false;
                bool showContinueText = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
                talkDoer.ShowText(worldPosition, speakPoint, duration, text2, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText, this.IsThoughtBubble.Value, this.AlternativeTalker.audioCharacterSpeechTag);
            }
            else
            {
                bool value = this.PlayBoxOnInteractingPlayer.Value;
                if (value)
                {
                    TextBoxManager.ClearTextBox(this.m_talkDoer.speakPoint);
                    TalkDoerLite talkDoer2 = this.m_talkDoer;
                    Vector3 worldPosition2 = this.m_talkDoer.TalkingPlayer.CenterPosition.ToVector3ZUp(this.m_talkDoer.TalkingPlayer.CenterPosition.y) + new Vector3(0f, 1f, -5f);
                    Transform transform = this.m_talkDoer.TalkingPlayer.transform;
                    float duration2 = -1f;
                    string text3 = text;
                    bool instant2 = false;
                    bool showContinueText2 = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
                    talkDoer2.ShowText(worldPosition2, transform, duration2, text3, instant2, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText2, this.IsThoughtBubble.Value, this.m_talkDoer.TalkingPlayer.characterAudioSpeechTag);
                }
                else
                {
                    bool flag18 = this.m_talkDoer.TalkingPlayer;
                    if (flag18)
                    {
                        TextBoxManager.ClearTextBox(this.m_talkDoer.TalkingPlayer.transform);
                    }
                    TalkDoerLite talkDoer3 = this.m_talkDoer;
                    Vector3 worldPosition3 = this.m_talkDoer.speakPoint.position + new Vector3(0f, 0f, -5f);
                    Transform speakPoint2 = this.m_talkDoer.speakPoint;
                    float duration3 = -1f;
                    string text4 = text;
                    bool instant3 = false;
                    bool showContinueText3 = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
                    talkDoer3.ShowText(worldPosition3, speakPoint2, duration3, text4, instant3, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText3, this.IsThoughtBubble.Value, null);
                }
            }
            bool flag19 = flag2 && this.forceCloseTime.Value > 0f;
            if (flag19)
            {
                this.m_forceCloseTimer = this.forceCloseTime.Value;
            }
            bool flag20 = this.sequence == AdvancedDialogueBox.DialogueSequence.PersistentSequential;
            if (flag20)
            {
                this.m_persistentIndex++;
                bool flag21 = this.m_persistentIndex >= this.persistentStringsToShow.Value;
                if (flag21)
                {
                    this.m_persistentIndex = 0;
                    this.m_textIndex = Mathf.Min(this.m_textIndex + 1, this.dialogue.Length - 1);
                }
            }
            else
            {
                this.m_textIndex++;
            }
        }

        // Token: 0x060000FC RID: 252 RVA: 0x0000EAA0 File Offset: 0x0000CCA0
        private void ClearAlternativeTalkerFromPrevious()
        {
            FsmState previousActiveState = base.Fsm.PreviousActiveState;
            bool flag = previousActiveState != null && previousActiveState != base.State;
            if (flag)
            {
                for (int i = 0; i < previousActiveState.Actions.Length; i++)
                {
                    bool flag2 = previousActiveState.Actions[i] is AdvancedDialogueBox;
                    if (flag2)
                    {
                        AdvancedDialogueBox advancedDialogueBox = previousActiveState.Actions[i] as AdvancedDialogueBox;
                        bool flag3 = advancedDialogueBox.AlternativeTalker != null;
                        if (flag3)
                        {
                            advancedDialogueBox.AlternativeTalker.SuppressClear = false;
                            TextBoxManager.ClearTextBox(advancedDialogueBox.AlternativeTalker.speakPoint);
                        }
                    }
                }
            }
        }

        // Token: 0x060000FD RID: 253 RVA: 0x0000EB48 File Offset: 0x0000CD48
        private void ShowResponses()
        {
            bool flag = this.m_talkDoer.echo1 != null && this.forceCloseEchoTextBoxesOnResponcesShown.Value;
            if (flag)
            {
                this.m_talkDoer.echo1.IsDoingForcedSpeech = false;
            }
            bool flag2 = this.m_talkDoer.echo2 != null && this.forceCloseEchoTextBoxesOnResponcesShown.Value;
            if (flag2)
            {
                this.m_talkDoer.echo2.IsDoingForcedSpeech = false;
            }
            bool flag3 = this.responses.Length != 0;
            if (flag3)
            {
                this.m_talkDoer.TalkingPlayer.SetInputOverride("dialogueResponse");
                GameUIRoot.Instance.DisplayPlayerConversationOptions(this.m_talkDoer.TalkingPlayer, this.m_rawResponses);
            }
        }

        // Token: 0x060000FE RID: 254 RVA: 0x0000EC08 File Offset: 0x0000CE08
        private bool HasNextDialogue()
        {
            bool flag = this.sequence == AdvancedDialogueBox.DialogueSequence.PersistentSequential;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = this.m_textIndex < this.m_numDialogues - 1;
                if (flag2)
                {
                    result = true;
                }
                else
                {
                    for (int i = 0; i < base.State.Transitions.Length; i++)
                    {
                        bool flag3 = !string.IsNullOrEmpty(base.State.Transitions[i].ToState);
                        if (flag3)
                        {
                            FsmState state = base.Fsm.GetState(base.State.Transitions[i].ToState);
                            for (int j = 0; j < state.Actions.Length; j++)
                            {
                                FsmStateAction fsmStateAction = state.Actions[j];
                                bool flag4 = fsmStateAction is AdvancedDialogueBox;
                                if (flag4)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    result = false;
                }
            }
            return result;
        }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x060000FF RID: 255 RVA: 0x0000ECF4 File Offset: 0x0000CEF4
        private string TalkAnimName
        {
            get
            {
                return (!this.SuppressDefaultAnims.Value && !string.IsNullOrEmpty(this.OverrideTalkAnim.Value)) ? this.OverrideTalkAnim.Value : "talk";
            }
        }

        // Token: 0x0400015F RID: 351
        [Tooltip("Only show this dialogue box if this condition is met")]
        public AdvancedDialogueBox.Condition condition;

        // Token: 0x04000160 RID: 352
        [Tooltip("Handles the dialogue sequence.")]
        [ActionSection("Text")]
        public AdvancedDialogueBox.DialogueSequence sequence;

        // Token: 0x04000161 RID: 353
        [Tooltip("The number of persistent strings to show for each key before progressing to the next one.")]
        public FsmInt persistentStringsToShow = 1;

        // Token: 0x04000162 RID: 354
        [Tooltip("Dialogue strings for the NPC to say.")]
        public FsmString[] dialogue;

        // Token: 0x04000163 RID: 355
        [CompoundArray("Responses", "Text", "Event")]
        public FsmString[] responses;

        // Token: 0x04000164 RID: 356
        public FsmEvent[] events;

        // Token: 0x04000165 RID: 357
        [ActionSection("Echos")]
        [Tooltip("How much delay there is between when the first text box is shown and the second.")]
        public FsmFloat echo1Delay = 1f;

        // Token: 0x04000166 RID: 358
        [Tooltip("How much delay there is between when the first text box is shown and the third.")]
        public FsmFloat echo2Delay = 2f;

        // Token: 0x04000167 RID: 359
        [Tooltip("How long the second text box is displayed for.")]
        public FsmFloat echo1DisplayDuration = 4f;

        // Token: 0x04000168 RID: 360
        [Tooltip("How long the third text box is displayed for.")]
        public FsmFloat echo2DisplayDuration = 4f;

        // Token: 0x04000169 RID: 361
        public FsmBool forceCloseEchoTextBoxesOnResponcesShown = true;

        // Token: 0x0400016A RID: 362
        [Tooltip("If true, player distance will not cause the playerWalkedAway event to fire.")]
        [ActionSection("Advanced")]
        public FsmBool skipWalkAwayEvent;

        // Token: 0x0400016B RID: 363
        [Tooltip("If set, after this amount of time (seconds) the dialogue box will force close.")]
        public FsmFloat forceCloseTime;

        // Token: 0x0400016C RID: 364
        [Tooltip("If set, after the dialogue box closes it will remain up for this amount of time (seconds). Set to -1 to leave it up until something else overrides it.")]
        public FsmFloat zombieTime;

        // Token: 0x0400016D RID: 365
        [Tooltip("If true, don't use the default talk and idle animations.")]
        public FsmBool SuppressDefaultAnims;

        // Token: 0x0400016E RID: 366
        [Tooltip("If specified, use this animation instead of the default talk animation.")]
        public FsmString OverrideTalkAnim;

        // Token: 0x0400016F RID: 367
        [Tooltip("If marked, play the textbox over the player. Only for Pasts!")]
        public FsmBool PlayBoxOnInteractingPlayer;

        // Token: 0x04000170 RID: 368
        [Tooltip("Thot box")]
        public FsmBool IsThoughtBubble;

        // Token: 0x04000171 RID: 369
        [Tooltip("If used, play the textbox over this talk doer instead.")]
        public TalkDoerLite AlternativeTalker;

        // Token: 0x04000172 RID: 370
        private TalkDoerLite m_talkDoer;

        // Token: 0x04000173 RID: 371
        private AdvancedDialogueBox.DialogueState m_dialogueState;

        // Token: 0x04000174 RID: 372
        private int m_numDialogues;

        // Token: 0x04000175 RID: 373
        private int m_textIndex;

        // Token: 0x04000176 RID: 374
        private int m_persistentIndex;

        // Token: 0x04000177 RID: 375
        private float m_forceCloseTimer;

        // Token: 0x04000178 RID: 376
        private string[] m_rawResponses;

        // Token: 0x04000179 RID: 377
        private int m_sequentialStringLastIndex = -1;

        // Token: 0x0400017A RID: 378
        private string m_currentDialogueText;

        // Token: 0x0200015B RID: 347
        private enum DialogueState
        {
            // Token: 0x04000794 RID: 1940
            ShowNextDialogue,
            // Token: 0x04000795 RID: 1941
            ShowingDialogue,
            // Token: 0x04000796 RID: 1942
            ShowingResponses,
            // Token: 0x04000797 RID: 1943
            WaitingForResponse
        }

        // Token: 0x0200015C RID: 348
        public enum DialogueSequence
        {
            // Token: 0x04000799 RID: 1945
            Default,
            // Token: 0x0400079A RID: 1946
            Sequential,
            // Token: 0x0400079B RID: 1947
            SeqThenRepeatLast,
            // Token: 0x0400079C RID: 1948
            SeqThenRemoveState,
            // Token: 0x0400079D RID: 1949
            Mutliline,
            // Token: 0x0400079E RID: 1950
            PersistentSequential
        }

        // Token: 0x0200015D RID: 349
        public enum Condition
        {
            // Token: 0x040007A0 RID: 1952
            All,
            // Token: 0x040007A1 RID: 1953
            FirstEncounterThisInstance,
            // Token: 0x040007A2 RID: 1954
            FirstEverEncounter,
            // Token: 0x040007A3 RID: 1955
            KeyboardAndMouse = 100,
            // Token: 0x040007A4 RID: 1956
            Controller = 110
        }
    }
}
