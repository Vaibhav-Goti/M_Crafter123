using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Ozonscan
{
	// Token: 0x0200003F RID: 63
	public class ModifyRegistry
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600029F RID: 671 RVA: 0x0002E340 File Offset: 0x0002C540
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x00002993 File Offset: 0x00000B93
		public bool ShowError
		{
			get
			{
				return this.showError;
			}
			set
			{
				this.showError = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0002E358 File Offset: 0x0002C558
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0000299D File Offset: 0x00000B9D
		public string SubKey
		{
			get
			{
				return this.subKey;
			}
			set
			{
				this.subKey = value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0002E370 File Offset: 0x0002C570
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x000029A7 File Offset: 0x00000BA7
		public RegistryKey BaseRegistryKey
		{
			get
			{
				return this.baseRegistryKey;
			}
			set
			{
				this.baseRegistryKey = value;
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0002E388 File Offset: 0x0002C588
		public string Read(string KeyName)
		{
			RegistryKey registryKey = this.baseRegistryKey;
			RegistryKey registryKey2 = registryKey.OpenSubKey(this.subKey);
			bool flag = registryKey2 == null;
			string result;
			if (flag)
			{
				MessageBox.Show("Reading registry " + KeyName.ToUpper());
				result = null;
			}
			else
			{
				try
				{
					result = (string)registryKey2.GetValue(KeyName.ToUpper());
				}
				catch (Exception e)
				{
					this.ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0002E414 File Offset: 0x0002C614
		public bool Write(string KeyName, object Value)
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.baseRegistryKey;
				RegistryKey registryKey2 = registryKey.CreateSubKey(this.subKey);
				registryKey2.SetValue(KeyName.ToUpper(), Value);
				result = true;
			}
			catch (Exception e)
			{
				this.ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
				result = false;
			}
			return result;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0002E478 File Offset: 0x0002C678
		public bool DeleteKey(string KeyName)
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.baseRegistryKey;
				RegistryKey registryKey2 = registryKey.CreateSubKey(this.subKey);
				bool flag = registryKey2 == null;
				if (flag)
				{
					result = true;
				}
				else
				{
					registryKey2.DeleteValue(KeyName);
					result = true;
				}
			}
			catch (Exception e)
			{
				this.ShowErrorMessage(e, "Deleting SubKey " + this.subKey);
				result = false;
			}
			return result;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0002E4E4 File Offset: 0x0002C6E4
		public bool DeleteSubKeyTree()
		{
			bool result;
			try
			{
				RegistryKey registryKey = this.baseRegistryKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(this.subKey);
				bool flag = registryKey2 != null;
				if (flag)
				{
					registryKey.DeleteSubKeyTree(this.subKey);
				}
				result = true;
			}
			catch (Exception e)
			{
				this.ShowErrorMessage(e, "Deleting SubKey " + this.subKey);
				result = false;
			}
			return result;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0002E554 File Offset: 0x0002C754
		public int SubKeyCount()
		{
			int result;
			try
			{
				RegistryKey registryKey = this.baseRegistryKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(this.subKey);
				bool flag = registryKey2 != null;
				if (flag)
				{
					result = registryKey2.SubKeyCount;
				}
				else
				{
					result = 0;
				}
			}
			catch (Exception e)
			{
				this.ShowErrorMessage(e, "Retriving subkeys of " + this.subKey);
				result = 0;
			}
			return result;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0002E5C0 File Offset: 0x0002C7C0
		public int ValueCount()
		{
			int result;
			try
			{
				RegistryKey registryKey = this.baseRegistryKey;
				RegistryKey registryKey2 = registryKey.OpenSubKey(this.subKey);
				bool flag = registryKey2 != null;
				if (flag)
				{
					result = registryKey2.ValueCount;
				}
				else
				{
					result = 0;
				}
			}
			catch (Exception e)
			{
				this.ShowErrorMessage(e, "Retriving keys of " + this.subKey);
				result = 0;
			}
			return result;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0002E62C File Offset: 0x0002C82C
		private void ShowErrorMessage(Exception e, string Title)
		{
			bool flag = this.showError;
			if (flag)
			{
				MessageBox.Show(e.ToString(), Title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x040002F2 RID: 754
		private bool showError = false;

		// Token: 0x040002F3 RID: 755
		private string subKey = "SOFTWARE\\" + Application.ProductName.ToUpper();

		// Token: 0x040002F4 RID: 756
		private RegistryKey baseRegistryKey = Registry.CurrentUser;
	}
}
