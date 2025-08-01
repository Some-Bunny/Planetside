using Gungeon;
using NpcApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetside
{
    public class AlphabetController
    {

        private static bool isInited = false;

        public static void InitialiseAlphabet()
        {
            if (isInited == true) { return; }
            isInited = true;
            GenerateLetter("letter_01", "PSOGRune_A", "a");
            GenerateLetter("letter_02", "PSOGRune_B", "b");
            GenerateLetter("letter_03", "PSOGRune_C", "c");
            GenerateLetter("letter_04", "PSOGRune_D", "d");
            GenerateLetter("letter_05", "PSOGRune_E", "e");
            GenerateLetter("letter_06", "PSOGRune_F", "f");
            GenerateLetter("letter_07", "PSOGRune_G", "g");
            GenerateLetter("letter_08", "PSOGRune_H", "h");
            GenerateLetter("letter_09", "PSOGRune_I", "i");
            GenerateLetter("letter_10", "PSOGRune_J", "j");
            GenerateLetter("letter_11", "PSOGRune_K", "k");
            GenerateLetter("letter_12", "PSOGRune_L", "l");
            GenerateLetter("letter_13", "PSOGRune_M", "m");
            GenerateLetter("letter_14", "PSOGRune_N", "n");
            GenerateLetter("letter_15", "PSOGRune_O", "o");
            GenerateLetter("letter_16", "PSOGRune_P", "p");
            GenerateLetter("letter_17", "PSOGRune_Q", "q");
            GenerateLetter("letter_18", "PSOGRune_R", "r");
            GenerateLetter("letter_19", "PSOGRune_S", "s");
            GenerateLetter("letter_20", "PSOGRune_T", "t");
            GenerateLetter("letter_21", "PSOGRune_U", "u");
            GenerateLetter("letter_22", "PSOGRune_V", "v");
            GenerateLetter("letter_23", "PSOGRune_W", "w");
            GenerateLetter("letter_24", "PSOGRune_X", "x");
            GenerateLetter("letter_25", "PSOGRune_Y", "y");
            GenerateLetter("letter_26", "PSOGRune_Z", "z");
        }

        public static string ConvertString(string text)
        {
            if (isInited == false) { InitialiseAlphabet(); }

            string newText = string.Empty;
            foreach (char letter in text)
            {
                if (referenceKeys.ContainsKey(letter.ToString().ToLower())) 
                {
                    newText += referenceKeys[letter.ToString().ToLower()];
                }
                else
                {
                    newText += letter.ToString();
                }
            }
            return newText;
        }
        public static void GenerateLetter(string assetName, string name, string correspondingLetter)
        {
            var t = Alexandria.CharacterAPI.ToolsCharApi.AddNewItemToAtlas(GameUIRoot.Instance.ConversationBar.portraitSprite.Atlas, PlanetsideModule.SpriteCollectionAssets.LoadAsset<Texture2D>(assetName), name).name;

            var SetLetter = "[sprite \"" + t + "\"]";
            referenceKeys.Add(correspondingLetter, SetLetter);
        }
        public static Dictionary<string, string> referenceKeys = new Dictionary<string, string>();
    }
}
