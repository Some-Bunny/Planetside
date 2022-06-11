using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.RuntimeDetour;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace ChallengeAPI
{
    /// <summary>
    /// The core class of ChallengeAPI, it has the methods that can build challenges.
    /// </summary>
    public static class ChallengeBuilder
    {
        /// <summary>
        /// Initializes <see cref="ChallengeBuilder"/>, <see cref="ChallengeTools"/>, <see cref="ResourceGetter"/> and <see cref="FakePrefabHandler"/>.
        /// </summary>
        public static void Init()
        {
            if (m_initialized)
            {
                return;
            }
            FakePrefabHandler.Init();
            ResourceGetter.Init();
            m_isDebugMode = false;
            UIRootPrefab = ChallengeTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>();
            ChallengeManagerPrefab = ((GameObject)BraveResources.Load("Global Prefabs/_ChallengeManager", ".prefab")).GetComponent<ChallengeManager>();
            SuperChallengeManagerPrefab = ((GameObject)BraveResources.Load("Global Prefabs/_ChallengeMegaManager", ".prefab")).GetComponent<ChallengeManager>();
            dfLanguages = new Dictionary<StringTableManager.GungeonSupportedLanguages, TextAsset>();
            List<StringTableManager.GungeonSupportedLanguages> langs = new List<StringTableManager.GungeonSupportedLanguages>()
            {
                StringTableManager.GungeonSupportedLanguages.ENGLISH,
                StringTableManager.GungeonSupportedLanguages.FRENCH,
                StringTableManager.GungeonSupportedLanguages.SPANISH,
                StringTableManager.GungeonSupportedLanguages.ITALIAN,
                StringTableManager.GungeonSupportedLanguages.GERMAN,
                StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE,
                StringTableManager.GungeonSupportedLanguages.JAPANESE,
                StringTableManager.GungeonSupportedLanguages.KOREAN,
                StringTableManager.GungeonSupportedLanguages.RUSSIAN,
                StringTableManager.GungeonSupportedLanguages.POLISH,
                StringTableManager.GungeonSupportedLanguages.CHINESE
            };
            foreach (StringTableManager.GungeonSupportedLanguages language in langs)
            {
                string subDirectory = string.Empty;
                switch (language)
                {
                    case StringTableManager.GungeonSupportedLanguages.ENGLISH:
                        subDirectory = "english_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.FRENCH:
                        subDirectory = "french_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.SPANISH:
                        subDirectory = "spanish_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.ITALIAN:
                        subDirectory = "italian_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.GERMAN:
                        subDirectory = "german_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE:
                        subDirectory = "portuguese_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.JAPANESE:
                        subDirectory = "japanese_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.KOREAN:
                        subDirectory = "korean_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
                        subDirectory = "russian_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.POLISH:
                        subDirectory = "polish_items";
                        break;
                    case StringTableManager.GungeonSupportedLanguages.CHINESE:
                        subDirectory = "chinese_items";
                        break;
                }
                TextAsset ta = (TextAsset)BraveResources.Load("strings/" + subDirectory + "/ui", typeof(TextAsset), ".txt");
                dfLanguages.Add(language, ta);
            }
            UIRootManagerStringHolder = UIRootPrefab.Manager.GetComponent<dfLanguageManager>().gameObject.AddComponent<dfLanguageExtraStringHolder>();
            UIRootManagerStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
            dfLanguageExtraStringHolder instanceUIRootStringHolder = null;
            if (GameUIRoot.Instance != null)
            {
                instanceUIRootStringHolder = GameUIRoot.Instance.Manager.GetComponent<dfLanguageManager>().gameObject.AddComponent<dfLanguageExtraStringHolder>();
                instanceUIRootStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
            }
            foreach (TextAsset ta in dfLanguages.Values)
            {
                UIRootManagerStringHolder.extraStrings.Add(ta, new Dictionary<string, string>());
                if (instanceUIRootStringHolder != null)
                {
                    instanceUIRootStringHolder.extraStrings.Add(ta, new Dictionary<string, string>());
                }
            }
            if (instanceUIRootStringHolder != null)
            {
                dfLanguageManager instanceUIRootManager = instanceUIRootStringHolder.GetComponent<dfLanguageManager>();
                instanceUIRootManager.LoadLanguage(instanceUIRootManager.CurrentLanguage, true);
            }
            dfLanguageManagerBackupFileInfo = typeof(dfLanguageManager).GetField("backupDataFile", BindingFlags.NonPublic | BindingFlags.Instance);
            dfLanguageManagerStringsInfo = typeof(dfLanguageManager).GetField("strings", BindingFlags.NonPublic | BindingFlags.Instance);
            languageTextLoadHook = new Hook(
                typeof(dfLanguageManager).GetMethod("parseDataFile", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(ChallengeBuilder).GetMethod("LanguageTextLoadHook")
            );
            dfManagerStartHook = new Hook(
                typeof(dfGUIManager).GetMethod("Start", BindingFlags.Public | BindingFlags.Instance),
                typeof(ChallengeBuilder).GetMethod("DFManagerStartHook")
            );
            addedChallenges = new List<ChallengeDataEntry>();
            addedBossChallenges = new List<BossChallengeData>();
            m_initialized = true;
        }

        /// <summary>
        /// Unloads <see cref="ChallengeBuilder"/>, <see cref="ChallengeTools"/>, <see cref="ResourceGetter"/> and <see cref="FakePrefabHandler"/>.
        /// </summary>
        public static void Unload()
        {
            if (!m_initialized)
            {
                return;
            }
            FakePrefabHandler.Unload();
            ResourceGetter.Unload();
            m_isDebugMode = false;
            dfLanguages?.Clear();
            dfLanguages = null;
            if(UIRootPrefab.Manager.GetComponent<dfLanguageExtraStringHolder>() != null)
            {
                UnityEngine.Object.Destroy(UIRootPrefab.Manager.GetComponent<dfLanguageExtraStringHolder>());
                UIRootManagerStringHolder = null;
            }
            if (GameUIRoot.Instance != null && GameUIRoot.Instance.Manager.GetComponent<dfLanguageExtraStringHolder>() != null)
            {
                UnityEngine.Object.Destroy(GameUIRoot.Instance.Manager.GetComponent<dfLanguageExtraStringHolder>());
            }
            dfLanguageManager instanceUIRootManager = GameUIRoot.Instance.Manager.GetComponent<dfLanguageManager>();
            instanceUIRootManager.LoadLanguage(instanceUIRootManager.CurrentLanguage, true);
            if(addedChallenges != null)
            {
                foreach(ChallengeDataEntry challenge in addedChallenges)
                {
                    if (ChallengeManagerPrefab.PossibleChallenges.Contains(challenge))
                    {
                        ChallengeManagerPrefab.PossibleChallenges.Remove(challenge);
                    }
                    if (SuperChallengeManagerPrefab.PossibleChallenges.Contains(challenge))
                    {
                        SuperChallengeManagerPrefab.PossibleChallenges.Remove(challenge);
                    }
                    if (ChallengeManager.Instance.PossibleChallenges.Contains(challenge))
                    {
                        ChallengeManager.Instance.PossibleChallenges.Remove(challenge);
                    }
                }
                addedChallenges.Clear();
                addedChallenges = null;
            }
            if (addedBossChallenges != null)
            {
                foreach (BossChallengeData challenge in addedBossChallenges)
                {
                    if (ChallengeManagerPrefab.BossChallenges.Contains(challenge))
                    {
                        ChallengeManagerPrefab.BossChallenges.Remove(challenge);
                    }
                    if (SuperChallengeManagerPrefab.BossChallenges.Contains(challenge))
                    {
                        SuperChallengeManagerPrefab.BossChallenges.Remove(challenge);
                    }
                    if (ChallengeManager.Instance.BossChallenges.Contains(challenge))
                    {
                        ChallengeManager.Instance.BossChallenges.Remove(challenge);
                    }
                }
                addedBossChallenges.Clear();
                addedBossChallenges = null;
            }
            dfLanguageManagerBackupFileInfo = null;
            dfLanguageManagerStringsInfo = null;
            languageTextLoadHook?.Dispose();
            dfManagerStartHook?.Dispose();
            UIRootPrefab = null;
            ChallengeManagerPrefab = null;
            SuperChallengeManagerPrefab = null;
            m_initialized = false;
        }

        public static void DFManagerStartHook(Action<dfGUIManager> orig, dfGUIManager self)
        {
            orig(self);
            GameUIRoot root = self.GetComponent<GameUIRoot>();
            if (root != null && GameUIRoot.Instance != null && root == GameUIRoot.Instance && UIRootPrefab != null && root.name == UIRootPrefab.name && self.GetComponent<dfLanguageExtraStringHolder>() == null)
            {
                dfLanguageExtraStringHolder holder = self.gameObject.AddComponent<dfLanguageExtraStringHolder>();
                dfLanguageExtraStringHolder prefabHolder = UIRootPrefab.Manager.gameObject.GetComponent<dfLanguageExtraStringHolder>();
                holder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
                foreach (TextAsset ta in prefabHolder.extraStrings.Keys)
                {
                    if (!holder.extraStrings.ContainsKey(ta))
                    {
                        holder.extraStrings.Add(ta, new Dictionary<string, string>());
                    }
                    foreach (KeyValuePair<string, string> str in prefabHolder.extraStrings[ta])
                    {
                        if (!holder.extraStrings.ContainsKey(ta))
                        {
                            holder.extraStrings.Add(ta, new Dictionary<string, string>());
                        }
                        if (holder.extraStrings[ta] == null)
                        {
                            holder.extraStrings[ta] = new Dictionary<string, string>();
                        }
                        holder.extraStrings[ta].Add(str.Key, str.Value);
                    }
                }
                dfLanguageManager langManager = self.GetComponent<dfLanguageManager>();
                if (langManager != null)
                {
                    langManager.LoadLanguage(langManager.CurrentLanguage);
                }
            }
        }

        public static void LanguageTextLoadHook(Action<dfLanguageManager> orig, dfLanguageManager self)
        {
            orig(self);
            dfLanguageExtraStringHolder stringHolder = self.GetComponent<dfLanguageExtraStringHolder>();
            if (stringHolder != null)
            {
                TextAsset dataFile = self.DataFile;
                TextAsset backupDataFile = self.BackupDataFile();
                Dictionary<string, string> strings = self.GetStrings();
                Dictionary<string, string> extraStringsForDataFile;
                if (stringHolder.extraStrings.ContainsKey(dataFile))
                {
                    extraStringsForDataFile = stringHolder.extraStrings[dataFile];
                }
                else if (stringHolder.extraStrings.ContainsKey(backupDataFile))
                {
                    extraStringsForDataFile = stringHolder.extraStrings[backupDataFile];
                }
                else
                {
                    return;
                }
                foreach (KeyValuePair<string, string> pair in extraStringsForDataFile)
                {
                    strings[pair.Key] = pair.Value;
                }
                if (stringHolder.extraStrings.ContainsKey(backupDataFile))
                {
                    Dictionary<string, string> extraStringsForBackupDataFile = stringHolder.extraStrings[backupDataFile];
                    foreach (KeyValuePair<string, string> pair in extraStringsForBackupDataFile)
                    {
                        if (!strings.ContainsKey(pair.Key))
                        {
                            strings[pair.Key] = pair.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the first <see cref="ChallengeDataEntry"/> in <paramref name="manager"/> with the <see cref="ChallengeModifier"/> that has the type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The required <see cref="ChallengeModifier"/> type.</typeparam>
        /// <param name="manager">The challenge mode manager to find the <see cref="ChallengeDataEntry"/> in.</param>
        /// <returns>If the challenge was found, the found <see cref="ChallengeDataEntry"/>. <see langword="null"/> otherwise.</returns>
        public static ChallengeDataEntry FindChallenge<T>(this ChallengeManager manager) where T : ChallengeModifier
        {
            ChallengeDataEntry result = null;
            if(manager != null && manager.PossibleChallenges != null)
            {
                foreach (ChallengeDataEntry challenge in manager.PossibleChallenges)
                {
                    if (challenge != null && challenge.challenge != null && challenge.challenge is T)
                    {
                        result = challenge;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sets a string in the DF UI string container.
        /// </summary>
        /// <param name="key">The string key.</param>
        /// <param name="value">The string value.</param>
        public static void SetDFString(string key, string value)
        {
            if(UIRootManagerStringHolder == null)
            {
                Init();
            }
            if (UIRootManagerStringHolder.extraStrings == null)
            {
                UIRootManagerStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
            }
            if (!UIRootManagerStringHolder.extraStrings.ContainsKey(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]))
            {
                UIRootManagerStringHolder.extraStrings.Add(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH], new Dictionary<string, string> { { key, value } });
            }
            else if (UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] == null)
            {
                UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] = new Dictionary<string, string> { { key, value } };
            }
            else
            {
                UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]].Add(key, value);
            }
            if (GameUIRoot.Instance != null)
            {
                dfLanguageExtraStringHolder instanceUIRootManagerStringHolder = GameUIRoot.Instance.Manager.GetComponent<dfLanguageManager>().gameObject.GetOrAddComponent<dfLanguageExtraStringHolder>();
                if (instanceUIRootManagerStringHolder.extraStrings == null)
                {
                    instanceUIRootManagerStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
                }
                if (!instanceUIRootManagerStringHolder.extraStrings.ContainsKey(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]))
                {
                    instanceUIRootManagerStringHolder.extraStrings.Add(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH], new Dictionary<string, string> { { key, value } });
                }
                else if (instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] == null)
                {
                    instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] = new Dictionary<string, string> { { key, value } };
                }
                else
                {
                    instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]].Add(key, value);
                }
                instanceUIRootManagerStringHolder.GetComponent<dfLanguageManager>().LoadLanguage(instanceUIRootManagerStringHolder.GetComponent<dfLanguageManager>().CurrentLanguage);
            }
        }

        /// <summary>
        /// Adds a new <see cref="ChallengeFloorData"/> to <paramref name="manager"/>.
        /// </summary>
        /// <param name="manager">The challenge mode manager to add the <see cref="ChallengeFloorData"/> to</param>
        /// <param name="floorID">The <see cref="ChallengeFloorData"/>'s floor.</param>
        /// <param name="minChallengesPerRoom">Minimum amount of challenges that will appear in a room in the <paramref name="floorID"/> floor.</param>
        /// <param name="maxChallengesPerRoom">Maximum amount of challenges that will appear in a room in the <paramref name="floorID"/> floor.</param>
        /// <returns>The built <see cref="ChallengeFloorData"/></returns>
        public static ChallengeFloorData AddChallengeFloorData(this ChallengeManager manager, GlobalDungeonData.ValidTilesets floorID, int minChallengesPerRoom, int maxChallengesPerRoom)
        {
            ChallengeFloorData result = new ChallengeFloorData
            {
                floorID = floorID,
                minChallenges = minChallengesPerRoom,
                maxChallenges = maxChallengesPerRoom
            };
            ChallengeFloorData alreadyExists = null;
            foreach(ChallengeFloorData floorData in manager.FloorData)
            {
                if (floorData.floorID == floorID)
                {
                    alreadyExists = floorData;
                }
            }
            if (alreadyExists != null)
            {
                manager.FloorData.Remove(alreadyExists);
            }
            manager.FloorData.Add(result);
            return result;
        }

        /// <summary>
        /// Finds the first <see cref="BossChallengeData"/> in <paramref name="manager"/> with a <see cref="ChallengeModifier"/> that has the type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The required <see cref="ChallengeModifier"/> type.</typeparam>
        /// <param name="manager">The challenge mode manager to find the <see cref="BossChallengeData"/> in.</param>
        /// <returns>If the boss challenge was found, the found <see cref="BossChallengeData"/>. <see langword="null"/> otherwise.</returns>
        public static BossChallengeData FindBossChallenge<T>(this ChallengeManager manager) where T : ChallengeModifier
        {
            BossChallengeData result = null;
            if (manager != null && manager.PossibleChallenges != null)
            {
                foreach (BossChallengeData challenge in manager.BossChallenges)
                {
                    if (challenge != null)
                    {
                        foreach(ChallengeModifier modifier in challenge.Modifiers)
                        {
                            if(modifier != null && modifier is T)
                            {
                                result = challenge;
                                break;
                            }
                        }
                    }
                    if(result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the backup data file of <paramref name="manager"/>.
        /// </summary>
        /// <param name="manager">The <see cref="dfLanguageManager"/> to get the backup data file from.</param>
        /// <returns><paramref name="manager"/>'s backup data file.</returns>
        public static TextAsset BackupDataFile(this dfLanguageManager manager)
        {
            return (TextAsset)dfLanguageManagerBackupFileInfo.GetValue(manager);
        }
        
        /// <summary>
        /// Gets the string dictionary of <paramref name="manager"/>.
        /// </summary>
        /// <param name="manager">The <see cref="dfLanguageManager"/> to get the string dictionary from.</param>
        /// <returns><paramref name="manager"/>'s string dictionary.</returns>
        public static Dictionary<string, string> GetStrings(this dfLanguageManager manager)
        {
            return (Dictionary<string, string>)dfLanguageManagerStringsInfo.GetValue(manager);
        }

        /// <summary>
        /// Sets the string dictionary of <paramref name="manager"/> to <paramref name="strings"/>.
        /// </summary>
        /// <param name="manager">The <see cref="dfLanguageManager"/> the string dictionary of which will get set.</param>
        /// <param name="strings">The new string dictionary for <paramref name="manager"/></param>
        public static void SetStrings(this dfLanguageManager manager, Dictionary<string, string> strings)
        {
            dfLanguageManagerStringsInfo.SetValue(manager, strings);
        }

        /// <summary>
        /// Creates a new <see cref="BossChallengeData"/> and adds it to the list of boss challenges.
        /// </summary>
        /// <param name="name">The name of the boss challenge.</param>
        /// <param name="bosses">List of bosses that are valid for the challenge.</param>
        /// <param name="numChallengesToUse">The amount of challenges from the <paramref name="challenges"/> list it randomly selects.</param>
        /// <param name="challenges">The list of challenges from which it will randomly select the challenges.</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the boss challenge to normal challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the boss challenge to double challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="BossChallengeData"/>.</returns>
        public static BossChallengeData BuildBossChallenge(string name, List<AIActor> bosses, int numChallengesToUse, List<ChallengeDataEntry> challenges, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true)
        {
            return BuildBossChallenge(name, bosses, numChallengesToUse, challenges.Convert(delegate (ChallengeDataEntry entry) { return entry.challenge; }), addToNormalChallengeManager, addToMegaChallengeManager);
        }

        /// <summary>
        /// Creates a new <see cref="BossChallengeData"/> and adds it to the list of boss challenges.
        /// </summary>
        /// <param name="name">The name of the boss challenge.</param>
        /// <param name="bossGuids">List of boss guids that are valid for the challenge.</param>
        /// <param name="numChallengesToUse">The amount of challenges from the <paramref name="challenges"/> list it randomly selects.</param>
        /// <param name="challenges">The list of challenges from which it will randomly select the challenges.</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the boss challenge to normal challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the boss challenge to double challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="BossChallengeData"/>.</returns>
        public static BossChallengeData BuildBossChallenge(string name, List<string> bossGuids, int numChallengesToUse, List<ChallengeDataEntry> challenges, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true)
        {
            return BuildBossChallenge(name, bossGuids, numChallengesToUse, challenges.Convert(delegate (ChallengeDataEntry entry) { return entry.challenge; }), addToNormalChallengeManager, addToMegaChallengeManager);
        }


        /// <summary>
        /// Creates a new <see cref="BossChallengeData"/> and adds it to the list of boss challenges.
        /// </summary>
        /// <param name="name">The name of the boss challenge.</param>
        /// <param name="bosses">List of bosses that are valid for the challenge.</param>
        /// <param name="numChallengesToUse">The amount of challenges from the <paramref name="modifiers"/> list it randomly selects.</param>
        /// <param name="modifiers">The list of challenges from which it will randomly select the challenges.</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the boss challenge to normal challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the boss challenge to double challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="BossChallengeData"/>.</returns>
        public static BossChallengeData BuildBossChallenge(string name, List<AIActor> bosses, int numChallengesToUse, List<ChallengeModifier> modifiers, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true)
        {
            return BuildBossChallenge(name, bosses.Convert(delegate (AIActor enemy) { return enemy.EnemyGuid; }), numChallengesToUse, modifiers, addToNormalChallengeManager, addToMegaChallengeManager);
        }

        /// <summary>
        /// Creates a new <see cref="BossChallengeData"/> and adds it to the list of boss challenges.
        /// </summary>
        /// <param name="name">The name of the boss challenge.</param>
        /// <param name="bossGuids">List of boss guids that are valid for the challenge.</param>
        /// <param name="numChallengesToUse">The amount of challenges from the <paramref name="modifiers"/> list it randomly selects.</param>
        /// <param name="modifiers">The list of challenges from which it will randomly select the challenges.</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the boss challenge to normal challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the boss challenge to double challenge mode's boss challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="BossChallengeData"/>.</returns>
        public static BossChallengeData BuildBossChallenge(string name, List<string> bossGuids, int numChallengesToUse, List<ChallengeModifier> modifiers, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true)
        {
            if(addedBossChallenges == null)
            {
                Init();
            }
            BossChallengeData data = new BossChallengeData
            {
                Annotation = name,
                BossGuids = bossGuids.ToArray(),
                NumToSelect = numChallengesToUse,
                Modifiers = modifiers.ToArray()
            };
            if (addToNormalChallengeManager && addToMegaChallengeManager)
            {
                AddBossChallenge(data);
                if (ChallengeManager.Instance != null)
                {
                    ChallengeManager.Instance.AddBossChallengeToChallengeManager(data);
                }
            }
            else
            {
                if (addToNormalChallengeManager)
                {
                    ChallengeManagerPrefab.AddBossChallengeToChallengeManager(data);
                    if (ChallengeManager.Instance != null && ChallengeManager.Instance.ChallengeMode != ChallengeModeType.ChallengeMegaMode)
                    {
                        ChallengeManager.Instance.AddBossChallengeToChallengeManager(data);
                    }
                }
                else if (addToMegaChallengeManager)
                {
                    SuperChallengeManagerPrefab.AddBossChallengeToChallengeManager(data);
                    if (ChallengeManager.Instance != null && ChallengeManager.Instance.ChallengeMode == ChallengeModeType.ChallengeMegaMode)
                    {
                        ChallengeManager.Instance.AddBossChallengeToChallengeManager(data);
                    }
                }
            }
            addedBossChallenges.Add(data);
            return data;
        }

        /// <summary>
        /// Builds a <see cref="ChallengeDataEntry"/> and a <see cref="ChallengeModifier"/> of the type <typeparamref name="T"/> and adds them to the list of challenges.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ChallengeModifier"/> that will be created.</typeparam>
        /// <param name="challengeIcon">The <see cref="Texture2D"/> that will be used as the challenge's icon.</param>
        /// <param name="challengeName">The name of the challenge.</param>
        /// <param name="validInBossRooms">If <see langword="false"/>, the challenge won't appear in boss rooms.</param>
        /// <param name="mutuallyExclusive"><see cref="ChallengeModifier"/>s that won't be able to get paired with this challenge. If <see langword="null"/>, defaults to no challenges.</param>
        /// <param name="excludedTilesets">The floor that the challenge won't appear on. If <see langword="null"/>, the challenge will be able to appear on all floors.</param>
        /// <param name="floorsWithCustomSlotRequirement">A dictionary with the keys being floors and values being the required slots for those floors. If <see langword="null"/>, the challenge will have the default slot requirement for all floors (1)</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the challenge to normal challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the challenge to double challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="ChallengeDataEntry"/></returns>
        public static ChallengeDataEntry BuildChallenge<T>(Texture2D challengeIcon, string challengeName, bool validInBossRooms = true, List<ChallengeModifier> mutuallyExclusive = null, GlobalDungeonData.ValidTilesets? excludedTilesets = null, 
            Dictionary<GlobalDungeonData.ValidTilesets, int> floorsWithCustomSlotRequirement = null, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true) where T : ChallengeModifier
        {
            GameObject go = new GameObject(typeof(T).ToString());
            go.SetActive(false);
            go.MarkAsFakePrefab();
            UnityEngine.Object.DontDestroyOnLoad(go);
            return BuildChallenge<T>(go, challengeIcon, challengeName, validInBossRooms, mutuallyExclusive, excludedTilesets, floorsWithCustomSlotRequirement, addToNormalChallengeManager, addToMegaChallengeManager);
        }

        /// <summary>
        /// Builds a <see cref="ChallengeDataEntry"/> and a <see cref="ChallengeModifier"/> of the type <typeparamref name="T"/> and adds them to the list of challenges.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ChallengeModifier"/> that will be created.</typeparam>
        /// <param name="go">The <see cref="GameObject"/> that the <see cref="ChallengeModifier"/> component will get added to.</param>
        /// <param name="challengeIconPath">The filepath to your challenge icon image in your project.</param>
        /// <param name="challengeName">The name of the challenge.</param>
        /// <param name="validInBossRooms">If <see langword="false"/>, the challenge won't appear in boss rooms.</param>
        /// <param name="mutuallyExclusive"><see cref="ChallengeModifier"/>s that won't be able to get paired with this challenge. If <see langword="null"/>, defaults to no challenges.</param>
        /// <param name="excludedTilesets">The floor that the challenge won't appear on. If <see langword="null"/>, the challenge will be able to appear on all floors.</param>
        /// <param name="floorsWithCustomSlotRequirement">A dictionary with the keys being floors and values being the required slots for those floors. If <see langword="null"/>, the challenge will have the default slot requirement for all floors (1)</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the challenge to normal challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the challenge to double challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="ChallengeDataEntry"/></returns>
        public static ChallengeDataEntry BuildChallenge<T>(GameObject go, string challengeIconPath, string challengeName, bool validInBossRooms = true, List<ChallengeModifier> mutuallyExclusive = null, 
            GlobalDungeonData.ValidTilesets? excludedTilesets = null, Dictionary<GlobalDungeonData.ValidTilesets, int> floorsWithCustomSlotRequirement = null, bool addToNormalChallengeManager = true, 
            bool addToMegaChallengeManager = true) where T : ChallengeModifier
        {
            return BuildChallenge<T>(go, ResourceGetter.GetTextureFromResource(challengeIconPath), challengeName, validInBossRooms, mutuallyExclusive, excludedTilesets, floorsWithCustomSlotRequirement, addToNormalChallengeManager, addToMegaChallengeManager);
        }

        /// <summary>
        /// Builds a <see cref="ChallengeDataEntry"/> and a <see cref="ChallengeModifier"/> of the type <typeparamref name="T"/> and adds them to the list of challenges.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ChallengeModifier"/> that will be created.</typeparam>
        /// <param name="challengeIconPath">The filepath to your challenge icon image in your project.</param>
        /// <param name="challengeName">The name of the challenge.</param>
        /// <param name="validInBossRooms">If <see langword="false"/>, the challenge won't appear in boss rooms.</param>
        /// <param name="mutuallyExclusive"><see cref="ChallengeModifier"/>s that won't be able to get paired with this challenge. If <see langword="null"/>, defaults to no challenges.</param>
        /// <param name="excludedTilesets">The floor that the challenge won't appear on. If <see langword="null"/>, the challenge will be able to appear on all floors.</param>
        /// <param name="floorsWithCustomSlotRequirement">A dictionary with the keys being floors and values being the required slots for those floors. If <see langword="null"/>, the challenge will have the default slot requirement for all floors (1)</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the challenge to normal challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the challenge to double challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="ChallengeDataEntry"/></returns>
        public static ChallengeDataEntry BuildChallenge<T>(string challengeIconPath, string challengeName, bool validInBossRooms = true, List<ChallengeModifier> mutuallyExclusive = null, GlobalDungeonData.ValidTilesets? excludedTilesets = null, 
            Dictionary<GlobalDungeonData.ValidTilesets, int> floorsWithCustomSlotRequirement = null, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true) where T : ChallengeModifier
        {
            GameObject go = new GameObject(typeof(T).ToString());
            go.SetActive(false);
            go.MarkAsFakePrefab();
            UnityEngine.Object.DontDestroyOnLoad(go);
            return BuildChallenge<T>(go, ResourceGetter.GetTextureFromResource(challengeIconPath), challengeName, validInBossRooms, mutuallyExclusive, excludedTilesets, floorsWithCustomSlotRequirement, addToNormalChallengeManager, addToMegaChallengeManager);
        }

        /// <summary>
        /// Builds a <see cref="ChallengeDataEntry"/> and a <see cref="ChallengeModifier"/> of the type <typeparamref name="T"/> and adds them to the list of challenges.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ChallengeModifier"/> that will be created.</typeparam>
        /// <param name="go">The <see cref="GameObject"/> that the <see cref="ChallengeModifier"/> component will get added to.</param>
        /// <param name="challengeIcon">The <see cref="Texture2D"/> that will be used as the challenge's icon.</param>
        /// <param name="challengeName">The name of the challenge.</param>
        /// <param name="validInBossRooms">If <see langword="false"/>, the challenge won't appear in boss rooms.</param>
        /// <param name="mutuallyExclusive"><see cref="ChallengeModifier"/>s that won't be able to get paired with this challenge. If <see langword="null"/>, defaults to no challenges.</param>
        /// <param name="excludedTilesets">The floor that the challenge won't appear on. If <see langword="null"/>, the challenge will be able to appear on all floors.</param>
        /// <param name="floorsWithCustomSlotRequirement">A dictionary with the keys being floors and values being the required slots for those floors. If <see langword="null"/>, the challenge will have the default slot requirement for all floors (1)</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the challenge to normal challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the challenge to double challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        public static ChallengeDataEntry BuildChallenge<T>(GameObject go, Texture2D challengeIcon, string challengeName, bool validInBossRooms = true, List<ChallengeModifier> mutuallyExclusive = null, GlobalDungeonData.ValidTilesets? excludedTilesets = null, 
            Dictionary<GlobalDungeonData.ValidTilesets, int> floorsWithCustomSlotRequirement = null, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true) where T : ChallengeModifier
        {
            if(UIRootManagerStringHolder == null || UIRootPrefab == null || ChallengeManagerPrefab == null || SuperChallengeManagerPrefab == null || addedChallenges == null)
            {
                Init();
            }
            ChallengeDataEntry result = new ChallengeDataEntry
            {
                Annotation = typeof(T).ToString()
            };
            T modifier = go.AddComponent<T>();
            modifier.AtlasSpriteName = UIRootPrefab.Manager.DefaultAtlas.AddNewItemToAtlas(challengeIcon, result.Annotation).name;
            string stringKey = "#CH_" + challengeName.ToUpper().Replace(" ", "_");
            if(UIRootManagerStringHolder.extraStrings == null)
            {
                UIRootManagerStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
            }
            if (!UIRootManagerStringHolder.extraStrings.ContainsKey(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]))
            {
                UIRootManagerStringHolder.extraStrings.Add(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH], new Dictionary<string, string> { { stringKey, challengeName } });
            }
            else if(UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] == null)
            {
                UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] = new Dictionary<string, string> { { stringKey, challengeName } };
            }
            else
            {
                UIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]].Add(stringKey, challengeName);
            }
            if(GameUIRoot.Instance != null)
            {
                dfLanguageExtraStringHolder instanceUIRootManagerStringHolder = GameUIRoot.Instance.Manager.GetComponent<dfLanguageManager>().gameObject.GetOrAddComponent<dfLanguageExtraStringHolder>();
                if (instanceUIRootManagerStringHolder.extraStrings == null)
                {
                    instanceUIRootManagerStringHolder.extraStrings = new Dictionary<TextAsset, Dictionary<string, string>>();
                }
                if (!instanceUIRootManagerStringHolder.extraStrings.ContainsKey(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]))
                {
                    instanceUIRootManagerStringHolder.extraStrings.Add(dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH], new Dictionary<string, string> { { stringKey, challengeName } });
                }
                else if (instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] == null)
                {
                    instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]] = new Dictionary<string, string> { { stringKey, challengeName } };
                }
                else
                {
                    instanceUIRootManagerStringHolder.extraStrings[dfLanguages[StringTableManager.GungeonSupportedLanguages.ENGLISH]].Add(stringKey, challengeName);
                }
                instanceUIRootManagerStringHolder.GetComponent<dfLanguageManager>().LoadLanguage(instanceUIRootManagerStringHolder.GetComponent<dfLanguageManager>().CurrentLanguage);
            }
            modifier.DisplayName = stringKey;
            modifier.ValidInBossChambers = validInBossRooms;
            if (mutuallyExclusive != null)
            {
                modifier.MutuallyExclusive = mutuallyExclusive.ToList();
            }
            else
            {
                modifier.MutuallyExclusive = new List<ChallengeModifier>();
            }
            result.challenge = modifier;
            result.excludedTilesets = excludedTilesets ?? 0;
            if (floorsWithCustomSlotRequirement != null)
            {
                result.tilesetsWithCustomValues = floorsWithCustomSlotRequirement.Keys.ToList();
                result.CustomValues = floorsWithCustomSlotRequirement.Values.ToList();
            }
            else
            {
                result.tilesetsWithCustomValues = new List<GlobalDungeonData.ValidTilesets>();
                result.CustomValues = new List<int>();
            }
            if (addToNormalChallengeManager && addToMegaChallengeManager)
            {
                AddChallenge(result);
                if(ChallengeManager.Instance != null)
                {
                    ChallengeManager.Instance.AddChallengeToChallengeManager(result);
                }
            }
            else
            {
                if (addToNormalChallengeManager)
                {
                    ChallengeManagerPrefab.AddChallengeToChallengeManager(result);
                    if (ChallengeManager.Instance != null && ChallengeManager.Instance.ChallengeMode != ChallengeModeType.ChallengeMegaMode)
                    {
                        ChallengeManager.Instance.AddChallengeToChallengeManager(result);
                    }
                }
                else if (addToMegaChallengeManager)
                {
                    SuperChallengeManagerPrefab.AddChallengeToChallengeManager(result);
                    if (ChallengeManager.Instance != null && ChallengeManager.Instance.ChallengeMode == ChallengeModeType.ChallengeMegaMode)
                    {
                        ChallengeManager.Instance.AddChallengeToChallengeManager(result);
                    }
                }
            }
            addedChallenges.Add(result);
            return result;
        }

        /// <summary>
        /// Builds a <see cref="ChallengeDataEntry"/> without building the <see cref="ChallengeModifier"/>.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> with the <see cref="ChallengeModifier"/> component.</param>
        /// <param name="excludedTilesets">The floor that the challenge won't appear on. If <see langword="null"/>, the challenge will be able to appear on all floors.</param>
        /// <param name="floorsWithCustomSlotRequirement">A dictionary with the keys being floors and values being the required slots for those floors. If <see langword="null"/>, the challenge will have the default slot requirement for all floors (1)</param>
        /// <param name="addToNormalChallengeManager">If <see langword="true"/>, it will add the challenge to normal challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <param name="addToMegaChallengeManager">If <see langword="true"/>, it will add the challenge to double challenge mode's challenge pool. If <see langword="false"/>, it won't.</param>
        /// <returns>The built <see cref="ChallengeDataEntry"/>.</returns>
        public static ChallengeDataEntry BuildChallengeDataFromChallengeModifier(GameObject go, GlobalDungeonData.ValidTilesets? excludedTilesets = null, Dictionary<GlobalDungeonData.ValidTilesets, int> 
            floorsWithCustomSlotRequirement = null, bool addToNormalChallengeManager = true, bool addToMegaChallengeManager = true)
        {
            if(addedChallenges == null)
            {
                Init();
            }
            ChallengeDataEntry result = new ChallengeDataEntry
            {
                Annotation = go.GetComponent<ChallengeModifier>().GetType().ToString()
            };
            ChallengeModifier modifier = go.GetComponent<ChallengeModifier>();
            result.challenge = modifier;
            result.excludedTilesets = excludedTilesets ?? 0;
            if (floorsWithCustomSlotRequirement != null)
            {
                result.tilesetsWithCustomValues = floorsWithCustomSlotRequirement.Keys.ToList();
                result.CustomValues = floorsWithCustomSlotRequirement.Values.ToList();
            }
            else
            {
                result.tilesetsWithCustomValues = new List<GlobalDungeonData.ValidTilesets>();
                result.CustomValues = new List<int>();
            }
            if (addToNormalChallengeManager && addToMegaChallengeManager)
            {
                AddChallenge(result);
            }
            else
            {
                if (addToNormalChallengeManager)
                {
                    ChallengeManagerPrefab.AddChallengeToChallengeManager(result);
                }
                else if (addToMegaChallengeManager)
                {
                    SuperChallengeManagerPrefab.AddChallengeToChallengeManager(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Adds <paramref name="entry"/> to the challenge pools of all challenge modes.
        /// </summary>
        /// <param name="entry">The <see cref="ChallengeDataEntry"/> to add.</param>
        public static void AddChallenge(ChallengeDataEntry entry)
        {
            ChallengeManagerPrefab.AddChallengeToChallengeManager(entry);
            SuperChallengeManagerPrefab.AddChallengeToChallengeManager(entry);
        }

        /// <summary>
        /// Adds <paramref name="entry"/> to <paramref name="manager"/>'s challenge pool.
        /// </summary>
        /// <param name="manager">The challenge mode manager to which <paramref name="entry"/> will get added to.</param>
        /// <param name="entry">The <see cref="ChallengeDataEntry"/> to add.</param>
        public static void AddChallengeToChallengeManager(this ChallengeManager manager, ChallengeDataEntry entry)
        {
            if (m_isDebugMode)
            {
                manager.PossibleChallenges = new List<ChallengeDataEntry> { entry };
            }
            else
            {
                manager.PossibleChallenges.Add(entry);
            }
        }

        /// <summary>
        /// Adds <paramref name="entry"/> to the boss challenge pools of all challenge modes.
        /// </summary>
        /// <param name="entry">The <see cref="BossChallengeData"/> to add.</param>
        public static void AddBossChallenge(BossChallengeData entry)
        {
            ChallengeManagerPrefab.AddBossChallengeToChallengeManager(entry);
            SuperChallengeManagerPrefab.AddBossChallengeToChallengeManager(entry);
        }

        /// <summary>
        /// Adds <paramref name="entry"/> to <paramref name="manager"/>'s boss challenge pool.
        /// </summary>
        /// <param name="manager">The challenge mode manager to which <paramref name="entry"/> will get added to.</param>
        /// <param name="entry">The <see cref="BossChallengeData"/> to add.</param>
        public static void AddBossChallengeToChallengeManager(this ChallengeManager manager, BossChallengeData entry)
        {
            manager.BossChallenges.Add(entry);
        }

        /// <summary>
        /// Enables Debug Mode. When Debug Mode is enabled, the challenge mode will only have the challenge that was last added to it.
        /// </summary>
        public static void EnableDebugMode()
        {
            if (m_initialized)
            {
                m_isDebugMode = true;
            }
        }

        /// <summary>
        /// Disables Debug Mode. When Debug Mode is enabled, the challenge mode will only have the challenge that was last added to it.
        /// </summary>
        public static void DisableDebugMode()
        {
            if (m_initialized)
            {
                m_isDebugMode = false;
            }
        }

        private static bool m_initialized;
        private static bool m_isDebugMode;
        /// <summary>
        /// The prefab of <see cref="GameUIRoot"/>.
        /// </summary>
        public static GameUIRoot UIRootPrefab;
        /// <summary>
        /// The <see cref="dfLanguageExtraStringHolder"/> of <see cref="GameUIRoot"/>'s prefab.
        /// </summary>
        public static dfLanguageExtraStringHolder UIRootManagerStringHolder;
        /// <summary>
        /// The prefab of normal challenge mode's challenge manager.
        /// </summary>
        public static ChallengeManager ChallengeManagerPrefab;
        /// <summary>
        /// The prefab of double challenge mode's challenge manager.
        /// </summary>
        public static ChallengeManager SuperChallengeManagerPrefab;
        public static FieldInfo dfLanguageManagerBackupFileInfo;
        public static FieldInfo dfLanguageManagerStringsInfo;
        public static Hook languageTextLoadHook;
        public static Hook dfManagerStartHook;
        /// <summary>
        /// A dictionary with the keys being languages and the values being the <see cref="TextAsset"/>s for those languages.
        /// </summary>
        public static Dictionary<StringTableManager.GungeonSupportedLanguages, TextAsset> dfLanguages;
        /// <summary>
        /// The challenges that were added.
        /// </summary>
        public static List<ChallengeDataEntry> addedChallenges;
        /// <summary>
        /// The boss challenges that were added.
        /// </summary>
        public static List<BossChallengeData> addedBossChallenges;
    }
}
