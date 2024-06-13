using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;
using GungeonAPI;
using static Planetside.MultiActiveReloadManager;
using System.Runtime.Remoting.Messaging;

namespace Planetside
{
    public static class GlobalMessageRadio
    {
        public static void BroadcastMessage(string message)
        {
            if (OnMessageRecieved != null) 
            {
                for (int i = 0; i < RegisteredContainers.Count; i++)
                {
                    var mc = RegisteredContainers[i];
                    var mcG = mc.gameObject;

                    if (mcG == null)
                    {
                        OnMessageRecieved -= mc.recieverCall;
                        RegisteredContainers.Remove(mc);
                    }
                }
                for (int i = 0; i < RegisteredContainers.Count; i++)
                {
                    var mc = RegisteredContainers[i];
                    var mcG = mc.gameObject;
                    var mcMs = mc.messages;

                    if (mcG != null)
                    {
                        if (mcMs.Contains(message))
                        {
                            mc.recieverCall(mcG, message);
                        }
                    }                   
                }
            }
        }

        public static void RegisterObjectToRadio(GameObject obj, List<string> validMessages, Action<GameObject, string> reciever) 
        {
            var mc = new MessageContainer()
            {
                gameObject = obj,
                messages = validMessages,
                recieverCall = reciever
            };
            RegisteredContainers.Add(mc);
            OnMessageRecieved += mc.recieverCall;
        }
        private static List<MessageContainer> RegisteredContainers = new List<MessageContainer>();

        private static Action<GameObject, string> OnMessageRecieved; 
        
        public class MessageContainer 
        {
            public GameObject gameObject;
            public List<string> messages = new List<string>();
            public Action<GameObject, string> recieverCall;
        }
    }
    public class LocalMessageRadio : MonoBehaviour
    {
        public void BroadcastLocalMessage(string message)
        {
            if (OnMessageRecieved != null)
            {
                for (int i = 0; i < RegisteredContainers.Count; i++)
                {
                    var mc = RegisteredContainers[i];
                    var mcG = mc.gameObject;

                    if (mcG == null)
                    {
                        OnMessageRecieved -= mc.recieverCall;
                        RegisteredContainers.Remove(mc);
                    }
                }
                for (int i = 0; i < RegisteredContainers.Count; i++)
                {
                    var mc = RegisteredContainers[i];
                    var mcG = mc.gameObject;
                    var mcMs = mc.messages;

                    if (mcG != null)
                    {
                        if (mcMs.Contains(message))
                        {
                            mc.recieverCall(mcG, message);
                        }
                    }
                }

            }
        }

        public static void RegisterObjectToRadio(GameObject obj, List<string> validMessages, Action<GameObject, string> reciever)
        {
            var mc = new MessageContainer()
            {
                gameObject = obj,
                messages = validMessages,
                recieverCall = reciever
            };
            RegisteredContainers.Add(mc);
            OnMessageRecieved += mc.recieverCall;
        }
        private static List<MessageContainer> RegisteredContainers = new List<MessageContainer>();

        private static Action<GameObject, string> OnMessageRecieved;

        public class MessageContainer
        {
            public GameObject gameObject;
            public List<string> messages = new List<string>();
            public Action<GameObject, string> recieverCall;
        }
    }
}
