using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundAPI
{
    /// <summary>
    /// Represents a custom switch.
    /// </summary>
    public class CustomSwitchData
    {
        /// <summary>
        /// Plays all of <see cref="ReplacementEvents"/> on <paramref name="go"/> using <paramref name="playFunc"/>.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> to play <see cref="ReplacementEvents"/> on.</param>
        /// <param name="playFunc">The <see cref="Func{SwitchedEvent, GameObject, UInt32}"/> to play <see cref="ReplacementEvents"/> with.</param>
        /// <returns></returns>
        public uint Play(GameObject go, Func<SwitchedEvent, GameObject, uint> playFunc)
        {
            uint? id = null;
            foreach(SwitchedEvent replacement in ReplacementEvents)
            {
                uint newId = playFunc(replacement, go);
                id = id ?? newId;
            }
            if (id == null)
            {
                id = 0u;
            }
            return id.Value;
        }

        /// <summary>
        /// Name of the original event.
        /// </summary>
        public string OriginalEventName;
        /// <summary>
        /// Switch group to check.
        /// </summary>
        public string SwitchGroup;
        /// <summary>
        /// Required switch in <see cref="SwitchGroup"/>.
        /// </summary>
        public string RequiredSwitch;
        /// <summary>
        /// List of <see cref="SwitchedEvent"/>s that will be played.
        /// </summary>
        public List<SwitchedEvent> ReplacementEvents;
    }
}
