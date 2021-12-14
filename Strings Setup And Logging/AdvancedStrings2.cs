using System;
using System.Collections.Generic;

namespace Planetside
{
	// Token: 0x02000026 RID: 38
	public sealed class AdvancedStringDBTable
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600010E RID: 270 RVA: 0x0000BC00 File Offset: 0x00009E00
		public Dictionary<string, StringTableManager.StringCollection> Table
		{
			get
			{
				Dictionary<string, StringTableManager.StringCollection> result;
				bool flag = (result = this._CachedTable) == null;
				if (flag)
				{
					result = (this._CachedTable = this._GetTable());
				}
				return result;
			}
		}

		// Token: 0x1700000F RID: 15
		public StringTableManager.StringCollection this[string key]
		{
			get
			{
				return this.Table[key];
			}
			set
			{
				this.Table[key] = value;
				int num = this._ChangeKeys.IndexOf(key);
				bool flag = num > 0;
				if (flag)
				{
					this._ChangeValues[num] = value;
				}
				else
				{
					this._ChangeKeys.Add(key);
					this._ChangeValues.Add(value);
				}
				JournalEntry.ReloadDataSemaphore++;
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000024BA File Offset: 0x000006BA
		internal AdvancedStringDBTable(Func<Dictionary<string, StringTableManager.StringCollection>> _getTable)
		{
			this._ChangeKeys = new List<string>();
			this._ChangeValues = new List<StringTableManager.StringCollection>();
			this._GetTable = _getTable;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000BCC8 File Offset: 0x00009EC8
		public bool ContainsKey(string key)
		{
			return this.Table.ContainsKey(key);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000BCE8 File Offset: 0x00009EE8
		public void Set(string key, string value)
		{
			StringTableManager.StringCollection stringCollection = new StringTableManager.SimpleStringCollection();
			stringCollection.AddString(value, 1f);
			bool flag = this.Table.ContainsKey(key);
			if (flag)
			{
				this.Table[key] = stringCollection;
			}
			else
			{
				this.Table.Add(key, stringCollection);
			}
			int num = this._ChangeKeys.IndexOf(key);
			bool flag2 = num > 0;
			if (flag2)
			{
				this._ChangeValues[num] = stringCollection;
			}
			else
			{
				this._ChangeKeys.Add(key);
				this._ChangeValues.Add(stringCollection);
			}
			JournalEntry.ReloadDataSemaphore++;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000BD8C File Offset: 0x00009F8C
		public void SetComplex(string key, List<string> values, List<float> weights)
		{
			StringTableManager.StringCollection stringCollection = new StringTableManager.ComplexStringCollection();
			for (int i = 0; i < values.Count; i++)
			{
				string text = values[i];
				float num = weights[i];
				stringCollection.AddString(text, num);
			}
			this.Table[key] = stringCollection;
			int num2 = this._ChangeKeys.IndexOf(key);
			bool flag = num2 > 0;
			if (flag)
			{
				this._ChangeValues[num2] = stringCollection;
			}
			else
			{
				this._ChangeKeys.Add(key);
				this._ChangeValues.Add(stringCollection);
			}
			JournalEntry.ReloadDataSemaphore++;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000BE34 File Offset: 0x0000A034
		public string Get(string key)
		{
			return StringTableManager.GetString(key);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000BE4C File Offset: 0x0000A04C
		public void LanguageChanged()
		{
			this._CachedTable = null;
			Dictionary<string, StringTableManager.StringCollection> table = this.Table;
			for (int i = 0; i < this._ChangeKeys.Count; i++)
			{
				table[this._ChangeKeys[i]] = this._ChangeValues[i];
			}
		}

		// Token: 0x040000C8 RID: 200
		private readonly Func<Dictionary<string, StringTableManager.StringCollection>> _GetTable;

		// Token: 0x040000C9 RID: 201
		private Dictionary<string, StringTableManager.StringCollection> _CachedTable;

		// Token: 0x040000CA RID: 202
		private readonly List<string> _ChangeKeys;

		// Token: 0x040000CB RID: 203
		private readonly List<StringTableManager.StringCollection> _ChangeValues;
	}
}