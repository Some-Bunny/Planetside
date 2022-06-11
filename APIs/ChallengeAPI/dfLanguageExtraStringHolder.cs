using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ChallengeAPI
{
    /// <summary>
    /// A component that holds extra strings for <see cref="dfLanguageManager"/>s.
    /// </summary>
    public class dfLanguageExtraStringHolder : BraveBehaviour
    {
        /// <summary>
        /// The extra strings for <see cref="dfLanguageManager"/>s.
        /// </summary>
        public Dictionary<TextAsset, Dictionary<string, string>> extraStrings;
    }
}
