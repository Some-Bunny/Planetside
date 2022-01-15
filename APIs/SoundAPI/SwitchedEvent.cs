using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAPI
{
    /// <summary>
    /// An event paired with a switch.
    /// </summary>
    public class SwitchedEvent
    {
        /// <summary>
        /// Creates a new <see cref="SwitchedEvent"/>
        /// </summary>
        public SwitchedEvent()
        {
        }

        /// <summary>
        /// Creates a new <see cref="SwitchedEvent"/> with the <see cref="eventName"/> of <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The new <see cref="SwitchedEvent"/>'s <see cref="eventName"/>.</param>
        public SwitchedEvent(string s)
        {
            eventName = s;
            switchGroup = null;
            switchValue = null;
        }

        /// <summary>
        /// Creates a new <see cref="SwitchedEvent"/> with the <see cref="eventName"/> of <paramref name="eventName"/>, <see cref="switchGroup"/> of <paramref name="switchGroup"/> and <see cref="switchValue"/> of <paramref name="switchValue"/>.
        /// </summary>
        /// <param name="eventName">The new <see cref="SwitchedEvent"/>'s <see cref="eventName"/>.</param>
        /// <param name="switchGroup">The new <see cref="SwitchedEvent"/>'s <see cref="switchGroup"/>.</param>
        /// <param name="switchValue">The new <see cref="SwitchedEvent"/>'s <see cref="switchValue"/>.</param>
        public SwitchedEvent(string eventName, string switchGroup, string switchValue)
        {
            this.eventName = eventName;
            this.switchGroup = switchGroup;
            this.switchValue = switchValue;
        }

        /// <summary>
        /// Creates a new <see cref="SwitchedEvent"/> with the <see cref="eventName"/> of <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The new <see cref="SwitchedEvent"/>'s <see cref="eventName"/></param>
        public static implicit operator SwitchedEvent(string s)
        {
            return new SwitchedEvent(s);
        }

        /// <summary>
        /// The name of the event that this <see cref="SwitchedEvent"/> will play.
        /// </summary>
        public string eventName;
        /// <summary>
        /// The switch group related to this <see cref="SwitchedEvent"/>.
        /// </summary>
        public string switchGroup;
        /// <summary>
        /// The switch of the <see cref="switchGroup"/> that will be set temporarily when this plays.
        /// </summary>
        public string switchValue;
    }
}
