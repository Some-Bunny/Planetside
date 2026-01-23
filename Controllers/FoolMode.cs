using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using SaveAPI;
using HarmonyLib;
using AmmonomiconAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Brave.BulletScript;


namespace Planetside.Controllers
{
    public class FoolMode
    {
        public static void StartUp()
        {
            var theDay = DateTime.Today;

            if (CrossGameDataStorage.CrossGameStorage.FoolModeDay == 0)
            {
                var _random = new System.Random();

                int month = UnityEngine.Random.Range(1, 13);
                var noOfDaysInMonth = DateTime.DaysInMonth(2026, month);
                var day = _random.Next(1, noOfDaysInMonth);


                CrossGameDataStorage.CrossGameStorage.FoolModeDay = day;
                CrossGameDataStorage.CrossGameStorage.FoolModeMonth = month;
                CrossGameDataStorage.UpdateConfiguration();
            }
            if (theDay.Date.Day == 1 && theDay.Date.Month == 4)
            {
                FoolModeActive = true;
            }
            else if (theDay.Date.Month == 4 && theDay.Date.Day > 1 && theDay.Date.Day < 8)
            {
                FoolModeActive = UnityEngine.Random.value < 0.1f;
            }
            else if (theDay.Date.Day == CrossGameDataStorage.CrossGameStorage.FoolModeDay && theDay.Date.Month == CrossGameDataStorage.CrossGameStorage.FoolModeMonth)
            {
                FoolModeActive = true;
            }
            if (FoolModeActive)
            {
                Items.Clone.quality = PickupObject.ItemQuality.EXCLUDED;
                GameManager.Instance.StartCoroutine(WaitingGame());
                var HandleOpenAmmonomiconDeath = new Hook(
                        typeof(TalkingGunModifier).GetMethod("HandleDelayedTalk", BindingFlags.Instance | BindingFlags.NonPublic),
                        typeof(FoolMode).GetMethod("HandleDelayedTalkMod", BindingFlags.Static | BindingFlags.Public));
            }


        }

        private static IEnumerator WaitingGame()
        {
            float t = UnityEngine.Random.Range(1200, 3600);
            yield return new WaitForSeconds(t);
            AkSoundEngine.PostEvent("Play_CannonBlast", GameManager.Instance.gameObject);
            yield return null;
        }

        private static string[] Shittalking = new string[]
        {
            "You smell like moldy grapes.",
            "You are incredibly tossable.",
            "You're inedible. Blegh.",
            "Nose-breather.",
            "Mario doesn't like you.",
            "You're gonna get a C on your homework.",
            "Can you pass the controller to someone else?",
            "You left the stove on, nerd.",
            "Your gameplay was peer-reviewed. It sucks.",
            "Your only two brain cells fight for third place.",
            "I bet you eat half-grilled cheese sandwiches.",
            "You're still playing Gungeon?",
            "You should eat an onion.",
            "Buffoon."
        };

        public static IEnumerator HandleDelayedTalkMod(Func<TalkingGunModifier, PlayerController, IEnumerator> orig, TalkingGunModifier self, PlayerController playerController)
        {
            yield return new WaitForSeconds(1f);
            if (!playerController.IsInCombat && self.gameObject.activeSelf)
            {
                TextBoxManager.ShowTextBox(self.talkPoint.position + Vector3.zero, self.talkPoint, 4f, Shittalking[UnityEngine.Random.Range(0, Shittalking.Length)], string.Empty, false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);
            }
            yield break;
        }


        private static bool FoolModeActive = false;
        public static bool isFoolish {  get { return FoolModeActive; } }

    }
}
