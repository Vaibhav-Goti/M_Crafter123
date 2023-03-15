using RepetierHostExtender.interfaces;
using RepetierHostExtender.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Display3DModel
{
    public class Trans
    {
		public Trans(string folder, IHost _host)
		{
			Trans.host = _host;
			if (Trans.reg == null)
			{
				Trans.reg = Trans.host.GetRegistryFolder("");
			}
			this.translations = new SortedList<string, Translation>();
			this.AddFolder(folder);
			if (this.active == null)
			{
				this.active = this.english;
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000BE2A File Offset: 0x0000A02A
		public static Trans Init(IRegMemoryFolder _reg)
		{
			if (Trans.trans != null)
			{
				return Trans.trans;
			}
			Trans.reg = _reg;
			//Trans.trans = new Trans(Path.Combine(Application.StartupPath, "data", "translations"), null);
			return Trans.trans;
        }

		// Token: 0x0600016E RID: 366 RVA: 0x0000BE64 File Offset: 0x0000A064
		public void AddFolder(string folder)
		{
			string[] files = Directory.GetFiles(folder, "*.xml");
			string twoLetterISOLanguageName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
			string @string = Trans.reg.GetString("lastLanguage", twoLetterISOLanguageName + ".xml");
			foreach (string text in files)
			{
				try
				{
					string name = new FileInfo(text).Name;
					Translation translation = new Translation(text, name);
					int num = this.translations.IndexOfKey(translation.language);
					if (num < 0)
					{
						if (name == "en.xml")
						{
							this.english = translation;
						}
						if (name == @string)
						{
							this.active = translation;
						}
						this.translations.Add(translation.language, translation);
					}
					else
					{
						this.translations.Values[num].MergeTranslation(translation);
					}
				}
				catch
				{
				}
			}
			Trans.trans = this;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000BF60 File Offset: 0x0000A160
		public static void SelectByCode(string code)
		{
			foreach (Translation translation in Trans.trans.translations.Values)
			{
				if (translation.code == code)
				{
					Trans.trans.selectLanguage(translation);
					break;
				}
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		public void selectLanguage(Translation t)
		{
			this.active = t;
			Trans.reg.SetString("lastLanguage", t.fileshort);
			Trans.host.TriggerLanguageChanged();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000BFF4 File Offset: 0x0000A1F4
		public static string T(string id)
		{
			string text = null;
			if (Trans.trans.active != null && Trans.trans.active.trans.ContainsKey(id))
			{
				text = Trans.trans.active.trans[id];
			}
			if (text != null)
			{
				return text;
			}
			if (Trans.trans.english != null && Trans.trans.english.trans.ContainsKey(id))
			{
				text = Trans.trans.english.trans[id];
			}
			if (text != null)
			{
				return text;
			}
			return id;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000C080 File Offset: 0x0000A280
		public static string T1(string id, string v1)
		{
			return Trans.T(id).Replace("$1", v1);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000C093 File Offset: 0x0000A293
		public static string T2(string id, string v1, string v2)
		{
			return Trans.T(id).Replace("$1", v1).Replace("$2", v2);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000C0B1 File Offset: 0x0000A2B1
		public static string T3(string id, string v1, string v2, string v3)
		{
			return Trans.T(id).Replace("$1", v1).Replace("$2", v2).Replace("$3", v3);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000C0DA File Offset: 0x0000A2DA
		public static string T4(string id, string v1, string v2, string v3, string v4)
		{
			return Trans.T(id).Replace("$1", v1).Replace("$2", v2).Replace("$3", v3).Replace("$4", v4);
		}

		// Token: 0x040000AD RID: 173
		public Translation english;

		// Token: 0x040000AE RID: 174
		public Translation active;

		// Token: 0x040000AF RID: 175
		public SortedList<string, Translation> translations;

		// Token: 0x040000B0 RID: 176
		public static Trans trans;

		// Token: 0x040000B1 RID: 177
		public static IHost host;

		// Token: 0x040000B2 RID: 178
		public static IRegMemoryFolder reg;
	}
}
