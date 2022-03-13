using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace SynergyAPI
{
    /// <summary>
    /// Advanced version of <see cref="StringDB"/> that has the synergy table and allows setting the complex values of it's tables.
    /// </summary>
    public class AdvancedStringDB
    {
        /// <summary>
        /// Current language of the game. Can be both get and set.
        /// </summary>
        public StringTableManager.GungeonSupportedLanguages CurrentLanguage
        {
            get
            {
                return GameManager.Options.CurrentLanguage;
            }
            set
            {
                StringTableManager.SetNewLanguage(value, true);
            }
        }

        /// <summary>
        /// Unloads this <see cref="AdvancedStringDB"/>.
        /// </summary>
        public void Unload()
        {
            ETGMod.Databases.Strings.OnLanguageChanged -= LanguageChanged;
            StringTableManager.SetNewLanguage(StringTableManager.CurrentLanguage, true);
        }

        /// <summary>
        /// Creates a new <see cref="AdvancedStringDB"/>.
        /// </summary>
        public AdvancedStringDB()
        {
            ETGMod.Databases.Strings.OnLanguageChanged += LanguageChanged;
            Core = new AdvancedStringDBTable(() => StringTableManager.CoreTable);
            Items = new AdvancedStringDBTable(() => StringTableManager.ItemTable);
            Enemies = new AdvancedStringDBTable(() => StringTableManager.EnemyTable);
            Intro = new AdvancedStringDBTable(() => StringTableManager.IntroTable);
            Synergies = new AdvancedStringDBTable(() => SynergyTable);
        }

        /// <summary>
        /// This method is called when the language is changed. It makes all of added strings be kept.
        /// </summary>
        /// <param name="newLang"></param>
        public void LanguageChanged(StringTableManager.GungeonSupportedLanguages newLang)
        {
            Core.LanguageChanged();
            Items.LanguageChanged();
            Enemies.LanguageChanged();
            Intro.LanguageChanged();
            Synergies.LanguageChanged();
            Action<StringTableManager.GungeonSupportedLanguages> onLanguageChanged = OnLanguageChanged;
            if (onLanguageChanged == null)
            {
                return;
            }
            onLanguageChanged(newLang);
        }

        /// <summary>
        /// The string table for synergies.
        /// </summary>
        public static Dictionary<string, StringTableManager.StringCollection> SynergyTable
        {
            get
            {
                StringTableManager.GetSynergyString("ThisExistsOnlyToLoadTables");
                return (Dictionary<string, StringTableManager.StringCollection>)SynergyBuilder.m_synergyTable.GetValue(null);
            }
        }

        /// <summary>
        /// The core string table.
        /// </summary>
        public readonly AdvancedStringDBTable Core;
        /// <summary>
        /// The item string table that has items names, descriptions and short descriptions.
        /// </summary>
        public readonly AdvancedStringDBTable Items;
        /// <summary>
        /// The enemy string table that has enemy names, descriptions and short descriptions.
        /// </summary>
        public readonly AdvancedStringDBTable Enemies;
        /// <summary>
        /// The intro string table that has strings related to the intro.
        /// </summary>
        public readonly AdvancedStringDBTable Intro;
        /// <summary>
        /// The synergy string table that has synergy names.
        /// </summary>
        public readonly AdvancedStringDBTable Synergies;
        /// <summary>
        /// The event that will happen when the game language changes.
        /// </summary>
        public Action<StringTableManager.GungeonSupportedLanguages> OnLanguageChanged;
    }
}
