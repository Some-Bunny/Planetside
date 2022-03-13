using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyAPI
{
    public sealed class AdvancedStringDBTable
    {
        /// <summary>
        /// The strings for this table.
        /// </summary>
        public Dictionary<string, StringTableManager.StringCollection> Table
        {
            get
            {
                Dictionary<string, StringTableManager.StringCollection> result;
                if ((result = _CachedTable) == null)
                {
                    result = (_CachedTable = _GetTable());
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets a string from this table using <paramref name="key"/> as the key.
        /// </summary>
        /// <param name="key">The key for the string.</param>
        /// <returns>The string.</returns>
        public StringTableManager.StringCollection this[string key]
        {
            get
            {
                return Table[key];
            }
            set
            {
                Table[key] = value;
                int num = _ChangeKeys.IndexOf(key);
                if (num > 0)
                {
                    _ChangeValues[num] = value;
                }
                else
                {
                    _ChangeKeys.Add(key);
                    _ChangeValues.Add(value);
                }
                JournalEntry.ReloadDataSemaphore++;
            }
        }

        /// <summary>
        /// Creates a new <see cref="AdvancedStringDBTable"/>.
        /// </summary>
        /// <param name="_getTable">Method for getting the strings for the table.</param>
        public AdvancedStringDBTable(Func<Dictionary<string, StringTableManager.StringCollection>> _getTable)
        {
            _ChangeKeys = new List<string>();
            _ChangeValues = new List<StringTableManager.StringCollection>();
            _GetTable = _getTable;
        }

        /// <summary>
        /// Returns <see langword="true"/> if this string table contains <paramref name="key"/> in it's list of keys, returns <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><see langword="true"/> if this string table contains <paramref name="key"/> in it's list of keys, <see langword="false"/> otherwise.</returns>
        public bool ContainsKey(string key)
        {
            return Table.ContainsKey(key);
        }

        /// <summary>
        /// Sets <paramref name="key"/>'s value to <paramref name="value"/> in this string table.
        /// </summary>
        /// <param name="key">The key for the string to set.</param>
        /// <param name="value">The new value for the string.</param>
        public void Set(string key, string value)
        {
            StringTableManager.StringCollection stringCollection = new StringTableManager.SimpleStringCollection();
            stringCollection.AddString(value, 1f);
            if (Table.ContainsKey(key))
            {
                Table[key] = stringCollection;
            }
            else
            {
                Table.Add(key, stringCollection);
            }
            int num = _ChangeKeys.IndexOf(key);
            if (num > 0)
            {
                _ChangeValues[num] = stringCollection;
            }
            else
            {
                _ChangeKeys.Add(key);
                _ChangeValues.Add(stringCollection);
            }
            JournalEntry.ReloadDataSemaphore++;
        }

        /// <summary>
        /// Sets <paramref name="key"/>'s value to <paramref name="values"/> in this string table.
        /// </summary>
        /// <param name="key">The key for the strings to set.</param>
        /// <param name="values">The new values for the string.</param>
        public void SetComplex(string key, params string[] values)
        {
            StringTableManager.StringCollection stringCollection = new StringTableManager.ComplexStringCollection();
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];
                stringCollection.AddString(value, 1f);
            }
            Table[key] = stringCollection;
            int num = _ChangeKeys.IndexOf(key);
            if (num > 0)
            {
                _ChangeValues[num] = stringCollection;
            }
            else
            {
                _ChangeKeys.Add(key);
                _ChangeValues.Add(stringCollection);
            }
            JournalEntry.ReloadDataSemaphore++;
        }

        /// <summary>
        /// Sets <paramref name="key"/>'s value to <paramref name="values"/> and makes sets their weights to <paramref name="weights"/> in this string table.
        /// </summary>
        /// <param name="key">The key for the strings to set.</param>
        /// <param name="values">The new values for the string.</param>
        /// <param name="weights">The weights for the new values.</param>
        public void SetComplex(string key, List<string> values, List<float> weights)
        {
            StringTableManager.StringCollection stringCollection = new StringTableManager.ComplexStringCollection();
            for (int i = 0; i < values.Count; i++)
            {
                string value = values[i];
                float weight = weights[i];
                stringCollection.AddString(value, weight);
            }
            Table[key] = stringCollection;
            int num = _ChangeKeys.IndexOf(key);
            if (num > 0)
            {
                _ChangeValues[num] = stringCollection;
            }
            else
            {
                _ChangeKeys.Add(key);
                _ChangeValues.Add(stringCollection);
            }
            JournalEntry.ReloadDataSemaphore++;
        }

        /// <summary>
        /// Gets a string using <paramref name="key"/>
        /// </summary>
        /// <param name="key">The key for the string.</param>
        /// <returns>The found string.</returns>
        public string Get(string key)
        {
            return StringTableManager.GetString(key);
        }

        /// <summary>
        /// Makes all the new/changed strings not be lost when changing the game's language.
        /// </summary>
        public void LanguageChanged()
        {
            _CachedTable = null;
            Dictionary<string, StringTableManager.StringCollection> table = Table;
            for (int i = 0; i < _ChangeKeys.Count; i++)
            {
                table[_ChangeKeys[i]] = _ChangeValues[i];
            }
        }
        private readonly Func<Dictionary<string, StringTableManager.StringCollection>> _GetTable;
        private Dictionary<string, StringTableManager.StringCollection> _CachedTable;
        private readonly List<string> _ChangeKeys;
        private readonly List<StringTableManager.StringCollection> _ChangeValues;
    }
}
