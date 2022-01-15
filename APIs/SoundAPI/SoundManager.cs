using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using Ionic.Zip;
using System.Runtime.InteropServices;

namespace SoundAPI
{
    /// <summary>
    /// Core class of SoundAPI. Manages custom switch groups and other sound manipulation stuff.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// Inits SoundAPI.
        /// </summary>
        public static void Init()
        {
            if (m_initialized)
            {
                return;
            }
            CustomSwitchDatas = new List<CustomSwitchData>();
            Switches = new Dictionary<GameObject, Dictionary<string, string>>();
            StopEvents = new List<string>();
            StopEventsMusic = new List<string>();
            StopEventsObjects = new List<string>();
            StopEventsWeapons = new List<string>();
            if(SetSwitchHook == null)
            {
                SetSwitchHook = new Hook(typeof(AkSoundEngine).GetMethod("SetSwitch", new Type[] { typeof(string), typeof(string), typeof(GameObject) }), typeof(SoundManager).GetMethod("SetSwitch", BindingFlags.NonPublic | BindingFlags.Static));
            }
            PostEventPlayingIdHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject), typeof(uint), typeof(AkCallbackManager.EventCallback), typeof(object), typeof(uint), typeof(AkExternalSourceInfo),
                    typeof(uint) }), typeof(SoundManager).GetMethod("PostEventPlayingId", BindingFlags.NonPublic | BindingFlags.Static));
            PostEventExternalSourcesHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject), typeof(uint), typeof(AkCallbackManager.EventCallback), typeof(object), typeof(uint),
                    typeof(AkExternalSourceInfo) }), typeof(SoundManager).GetMethod("PostEventExternalSources", BindingFlags.NonPublic | BindingFlags.Static));
            PostEventExternalsHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject), typeof(uint), typeof(AkCallbackManager.EventCallback), typeof(object), typeof(uint) }),
                typeof(SoundManager).GetMethod("PostEventExternals", BindingFlags.NonPublic | BindingFlags.Static));
            PostEventCallbackCookieHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject), typeof(uint), typeof(AkCallbackManager.EventCallback), typeof(object) }),
                typeof(SoundManager).GetMethod("PostEventCallbackCookie", BindingFlags.NonPublic | BindingFlags.Static));
            PostEventFlagsHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject), typeof(uint) }), typeof(SoundManager).GetMethod("PostEventFlags", BindingFlags.NonPublic | BindingFlags.Static));
            PostEventHook = new Hook(typeof(AkSoundEngine).GetMethod("PostEvent", new Type[] { typeof(string), typeof(GameObject) }), typeof(SoundManager).GetMethod("PostEvent", BindingFlags.NonPublic | BindingFlags.Static));
            m_initialized = true;
        }

        /// <summary>
        /// Unloads SoundAPI.
        /// </summary>
        public static void Unload()
        {
            if (!m_initialized)
            {
                return;
            }
            CustomSwitchDatas?.Clear();
            CustomSwitchDatas = null;
            StopEvents?.Clear();
            StopEventsMusic?.Clear();
            StopEventsObjects?.Clear();
            StopEventsWeapons?.Clear();
            StopEvents = null;
            StopEventsMusic = null;
            StopEventsObjects = null;
            StopEventsWeapons = null;
            PostEventPlayingIdHook?.Dispose();
            PostEventExternalSourcesHook?.Dispose();
            PostEventExternalsHook?.Dispose();
            PostEventCallbackCookieHook?.Dispose();
            PostEventFlagsHook?.Dispose();
            PostEventHook?.Dispose();
            m_initialized = false;
        }

        /// <summary>
        /// Loads a soundbank with the name of <paramref name="fileName"/> from the mod folder/zip.
        /// </summary>
        /// <param name="mod">The mod's <see cref="ETGModule"/>.</param>
        /// <param name="fileName">The name of the bank file. Doesn't need to include the .bnk at the end.</param>
        public static void LoadBankFromModFolderOrZip(this ETGModule mod, string fileName)
        {
            if (!fileName.EndsWith(".bnk"))
            {
                fileName += ".bnk";
            }
            int FilesLoaded = 0;
            if (File.Exists(mod.Metadata.Archive))
            {
                ZipFile ModZIP = ZipFile.Read(mod.Metadata.Archive);
                if (ModZIP != null && ModZIP.Entries.Count > 0)
                {
                    foreach (ZipEntry entry in ModZIP.Entries)
                    {
                        if (entry.FileName == fileName)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                entry.Extract(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                LoadSoundbankFromStream(ms, entry.FileName);
                                FilesLoaded++;
                                break;
                            }
                        }
                    }
                    if (FilesLoaded > 0) { return; }
                }
            }
            LoadFromPath(mod.Metadata.Directory, fileName);
        }

        /// <summary>
        /// Loads all soundbanks from the mod's folder/zip.
        /// </summary>
        /// <param name="mod">The mod's <see cref="ETGModule"/>.</param>
        public static void LoadBanksFromModFolderOrZip(this ETGModule mod)
        {
            int FilesLoaded = 0;
            if (File.Exists(mod.Metadata.Archive))
            {
                ZipFile ModZIP = ZipFile.Read(mod.Metadata.Archive);
                if (ModZIP != null && ModZIP.Entries.Count > 0)
                {
                    foreach (ZipEntry entry in ModZIP.Entries)
                    {
                        if (entry.FileName.EndsWith(".bnk"))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                entry.Extract(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                LoadSoundbankFromStream(ms, entry.FileName);
                                FilesLoaded++;
                            }
                        }
                    }
                    if (FilesLoaded > 0) { return; }
                }
            }
            AutoloadFromPath(mod.Metadata.Directory);
        }

        /// <summary>
        /// Loads a soundbank with the path of <paramref name="path"/> from the mod project. The soundbank file must be an Embedded Resource otherwise it will not load.
        /// </summary>
        /// <param name="path">The path to the soundbank.</param>
        public static void LoadBankFromModProject(string path)
        {
            path = path.Replace("/", ".").Replace("\\", ".");
            if (!path.EndsWith(".bnk"))
            {
                path += ".bnk";
            }
            Assembly assembly = Assembly.GetCallingAssembly();
            using(Stream s = assembly.GetManifestResourceStream(path))
            {
                if(s != null)
                {
                    string name = path.Substring(0, path.LastIndexOf('.'));
                    string actualname = path;
                    if (name.LastIndexOf('.') >= 0)
                    {
                        actualname = name.Substring(name.LastIndexOf('.') + 1);
                    }
                    LoadSoundbankFromStream(s, actualname);
                }
            }
        }
        
        /// <summary>
        /// Loads all soundbanks from the mod project. The soundbanks must all be Embedded Resources otherwise they won't load.
        /// </summary>
        public static void LoadBanksFromModProject()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach(string path in assembly.GetManifestResourceNames())
            {
                using (Stream s = assembly.GetManifestResourceStream(path))
                {
                    if(s != null && path.EndsWith(".bnk"))
                    {
                        string name = path.Substring(0, path.LastIndexOf('.'));
                        string actualname = path;
                        if (name.LastIndexOf('.') >= 0)
                        {
                            actualname = name.Substring(name.LastIndexOf('.') + 1);
                        }
                        LoadSoundbankFromStream(s, actualname);
                    }
                }
            }
        }

        private static void LoadFromPath(string path, string filename)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            if (Directory.Exists(path))
            {
                List<string> list = new List<string>(Directory.GetFiles(path, "*.bnk", SearchOption.AllDirectories));
                for (int i = 0; i < list.Count; i++)
                {
                    string text = list[i];
                    using (FileStream fileStream = File.OpenRead(text))
                    {
                        string actualname = Path.GetFileName(path);
                        if(actualname == filename)
                        {
                            LoadSoundbankFromStream(fileStream, actualname);
                            break;
                        }
                    }
                }
            }
        }

        private static void AutoloadFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            if (Directory.Exists(path))
            {
                List<string> list = new List<string>(Directory.GetFiles(path, "*.bnk", SearchOption.AllDirectories));
                for (int i = 0; i < list.Count; i++)
                {
                    string text = list[i];
                    using (FileStream fileStream = File.OpenRead(text))
                    {
                        string actualname = Path.GetFileName(path);
                        LoadSoundbankFromStream(fileStream, actualname);
                    }
                }
            }
        }

        private static byte[] StreamToByteArray(Stream input)
        {
            byte[] array = new byte[16384];
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = input.Read(array, 0, array.Length)) > 0) { memoryStream.Write(array, 0, count); }
                result = memoryStream.ToArray();
            }
            return result;
        }

        private static void LoadSoundbankFromStream(Stream stream, string name)
        {
            byte[] array = StreamToByteArray(stream);
            IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
            try
            {
                Marshal.Copy(array, 0, intPtr, array.Length);
                AKRESULT akresult = AkSoundEngine.LoadAndDecodeBankFromMemory(intPtr, (uint)array.Length, false, name, false, out uint num);
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
        }

        /// <summary>
        /// Adds a new <see cref="CustomSwitchData"/> to <see cref="CustomSwitchDatas"/>.
        /// </summary>
        /// <param name="switchGroup">The <see cref="CustomSwitchData"/>'s <see cref="CustomSwitchData.SwitchGroup"/>.</param>
        /// <param name="switchValue">The <see cref="CustomSwitchData"/>'s <see cref="CustomSwitchData.RequiredSwitch"/>.</param>
        /// <param name="originalEventName">The <see cref="CustomSwitchData"/>'s <see cref="CustomSwitchData.OriginalEventName"/>.</param>
        /// <param name="replacementEvents">The <see cref="CustomSwitchData"/>'s <see cref="CustomSwitchData.ReplacementEvents"/>.</param>
        /// <returns></returns>
        public static CustomSwitchData AddCustomSwitchData(string switchGroup, string switchValue, string originalEventName, params SwitchedEvent[] replacementEvents)
        {
            if(CustomSwitchDatas == null)
            {
                Init();
            }
            CustomSwitchData data = new CustomSwitchData { OriginalEventName = originalEventName, ReplacementEvents = new List<SwitchedEvent>(replacementEvents), RequiredSwitch = switchValue, SwitchGroup = switchGroup };
            CustomSwitchDatas.Add(data);
            return data;
        }

        /// <summary>
        /// Adds a custom audio stop event to the "Stop_SND_all" and depending on <paramref name="types"/> adds it to other global stop sound events.
        /// </summary>
        /// <param name="eventName">Name of the stop sound event.</param>
        /// <param name="types">Types of other global stop sound events that the event will be added to. <see cref="StopEventType.Music"/> - "Stop_MUS_All", <see cref="StopEventType.Weapon"/> - "Stop_WPN_All", <see cref="StopEventType.Object"/> - 
        /// "Stop_SND_OBJ"</param>
        public static void RegisterStopEvent(string eventName, params StopEventType[] types)
        {
            if(StopEvents == null)
            {
                Init();
            }
            StopEvents.Add(eventName);
            foreach(StopEventType type in types)
            {
                switch (type)
                {
                    case StopEventType.Music:
                        StopEventsMusic.Add(eventName);
                        break;
                    case StopEventType.Weapon:
                        StopEventsWeapons.Add(eventName);
                        break;
                    case StopEventType.Object:
                        StopEventsObjects.Add(eventName);
                        break;
                }
            }
        }

        private static uint PostEventPlayingId(Func<string, GameObject, uint, AkCallbackManager.EventCallback, object, uint, AkExternalSourceInfo, uint, uint> orig, string eventName, GameObject gameObject, uint flags, 
            AkCallbackManager.EventCallback callback, object cookie, uint externals, AkExternalSourceInfo externalSources, uint playingId)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g, flags, callback, cookie, externals, externalSources, playingId));
        }

        private static uint PostEventExternalSources(Func<string, GameObject, uint, AkCallbackManager.EventCallback, object, uint, AkExternalSourceInfo, uint> orig, string eventName, GameObject gameObject, uint flags,
            AkCallbackManager.EventCallback callback, object cookie, uint externals, AkExternalSourceInfo externalSources)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g, flags, callback, cookie, externals, externalSources));
        }

        private static uint PostEventExternals(Func<string, GameObject, uint, AkCallbackManager.EventCallback, object, uint, uint> orig, string eventName, GameObject gameObject, uint flags,
            AkCallbackManager.EventCallback callback, object cookie, uint externals)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g, flags, callback, cookie, externals));
        }

        private static uint PostEventCallbackCookie(Func<string, GameObject, uint, AkCallbackManager.EventCallback, object, uint> orig, string eventName, GameObject gameObject, uint flags,
            AkCallbackManager.EventCallback callback, object cookie)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g, flags, callback, cookie));
        }

        private static uint PostEventFlags(Func<string, GameObject, uint, uint> orig, string eventName, GameObject gameObject, uint flags)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g, flags));
        }

        private static uint PostEvent(Func<string, GameObject, uint> orig, string eventName, GameObject gameObject)
        {
            return ProcessEvent(eventName, gameObject, (string s, GameObject g) => orig(s, g));
        }

        private static uint ProcessEvent(string eventName, GameObject go, Func<string, GameObject, uint> orig)
        {
            if(go != null && !string.IsNullOrEmpty(eventName))
            {
                CustomSwitchData data = GetCustomSwitchData(go, eventName);
                if(data != null)
                {
                    Func<SwitchedEvent, GameObject, uint> playData = delegate (SwitchedEvent switched, GameObject go2)
                    {
                        bool returnValue = false;
                        if(switched.switchGroup != null && switched.switchValue != null)
                        {
                            SetSwitchOrig(switched.switchGroup, switched.switchValue, go2);
                            returnValue = true;
                        }
                        uint u = orig(switched.eventName, go2);
                        if (returnValue)
                        {
                            ReturnSwitch(switched.switchGroup, go2);
                        }
                        return u;
                    };
                    return data.Play(go, playData);
                }
                if(eventName.ToLower() == "stop_snd_all")
                {
                    foreach(string stop in StopEvents)
                    {
                        orig(stop, go);
                    }
                }
                if (eventName.ToLower() == "stop_mus_all" | eventName.ToLower() == "stop_mus_boss_theme")
                {
                    foreach (string stop in StopEventsMusic)
                    {
                        orig(stop, go);
                    }
                }
                if (eventName.ToLower() == "stop_wpn_all")
                {
                    foreach (string stop in StopEventsWeapons)
                    {
                        orig(stop, go);
                    }
                }
                if (eventName.ToLower() == "stop_snd_obj")
                {
                    foreach (string stop in StopEventsObjects)
                    {
                        orig(stop, go);
                    }
                }
            }
            return orig(eventName, go);
        }

        private static CustomSwitchData GetCustomSwitchData(GameObject go, string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return null;
            }
            if(go != null && Switches.ContainsKey(go) && Switches[go] != null)
            {
                foreach(CustomSwitchData data in CustomSwitchDatas)
                {
                    if(data.OriginalEventName.ToLower() == eventName.ToLower() && Switches[go].ContainsKey(data.SwitchGroup.ToLower()) && Switches[go][data.SwitchGroup.ToLower()] == data.RequiredSwitch.ToLower())
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        private delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        private delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        private delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        private delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

        private static AKRESULT SetSwitch(Func<string, string, GameObject, AKRESULT> orig, string switchGroup, string switchValue, GameObject gameObject)
        {
            if(gameObject != null && !origSetSwitch)
            {
                if (!Switches.ContainsKey(gameObject))
                {
                    Switches.Add(gameObject, new Dictionary<string, string> { { switchGroup.ToLower(), switchValue.ToLower() } });
                }
                else
                {
                    if(Switches[gameObject] == null)
                    {
                        Switches[gameObject] = new Dictionary<string, string> { { switchGroup.ToLower(), switchValue.ToLower() } };
                    }
                    else
                    {
                        if (!Switches[gameObject].ContainsKey(switchGroup.ToLower()))
                        {
                            Switches[gameObject].Add(switchGroup.ToLower(), switchValue.ToLower());
                        }
                        else
                        {
                            Switches[gameObject][switchGroup.ToLower()] = switchValue.ToLower();
                        }
                    }
                }
            }
            return orig(switchGroup, switchValue, gameObject);
        }

        private static void SetSwitchOrig(string switchGroup, string switchValue, GameObject go)
        {
            origSetSwitch = true;
            AkSoundEngine.SetSwitch(switchGroup, switchValue, go);
            origSetSwitch = false;
        }

        private static void ReturnSwitch(string switchGroup, GameObject go)
        {
            if(Switches.ContainsKey(go) && Switches[go] != null && Switches[go].ContainsKey(switchGroup))
            {
                origSetSwitch = true;
                AkSoundEngine.SetSwitch(switchGroup, Switches[go][switchGroup], go);
                origSetSwitch = false;
            }
        }

        /// <summary>
        /// List of added <see cref="CustomSwitchData"/>s.
        /// </summary>
        public static List<CustomSwitchData> CustomSwitchDatas;
        /// <summary>
        /// List of registered stop audio events that will be added to the "Stop_SND_All" event.
        /// </summary>
        public static List<string> StopEvents;
        /// <summary>
        /// List of registered stop audio events that will be added to the "Stop_MUS_All" event.
        /// </summary>
        public static List<string> StopEventsMusic;
        /// <summary>
        /// List of registered stop audio events that will be added to the "Stop_WPN_All" event.
        /// </summary>
        public static List<string> StopEventsObjects;
        /// <summary>
        /// List of registered stop audio events that will be added to the "Stop_SND_OBJ" event.
        /// </summary>
        public static List<string> StopEventsWeapons;
        private static Dictionary<GameObject, Dictionary<string, string>> Switches;
        private static Hook SetSwitchHook;
        private static Hook PostEventPlayingIdHook;
        private static Hook PostEventExternalSourcesHook;
        private static Hook PostEventExternalsHook;
        private static Hook PostEventCallbackCookieHook;
        private static Hook PostEventFlagsHook;
        private static Hook PostEventHook;
        private static bool m_initialized;
        private static bool origSetSwitch;
    }
}
