using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Display3DModel
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public CheckBox checkAutoposition;
        public Window1()
        {
            InitializeComponent();
        }
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void copybtn_Click(object sender, RoutedEventArgs e)
        {
			//if (this.copyDialog.ShowDialog(Main.main) == DialogResult.Cancel)
			//{
			//	return;
			//}
			//int num = (int)this.copyDialog.numericCopies.Value;
			//List<TopoGroup> list = new List<TopoGroup>();
			//foreach (TopoGroup topoGroup in this.host.Scene.Groups)
			//{
			//	if (topoGroup.Selected)
			//	{
			//		list.Add(topoGroup);
			//	}
			//}
			//foreach (TopoGroup topoGroup2 in list)
			//{
			//	for (int i = 0; i < num; i++)
			//	{
			//		topoGroup2.CopyGroup();
			//	}
			//}
			//if (this.copyDialog.checkAutoposition.Checked)
			//{
			//	this.host.Scene.Autoposition();
			//}
			//this.host.Scene.FireSceneChanged();
			//this.host.ThreeDView.Camera.FitObjects();
			//this.host.ObjectPlacementChanged = true;
		}
    }
}
