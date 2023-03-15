using Ozonscan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Display3DModel
{
    /// <summary>
    /// Interaction logic for Print_Setting.xaml
    /// </summary>
    public partial class Print_Setting : UserControl
    {

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float SV_LayerHeight();
        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_NoofLayer();
        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_PrintTime();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Power(int iPower);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Scale(float fScale);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Speed(int OnSpeed, int OffSpeed);


        private bool isPowerSet = false;
        private bool isScaleSet = false;
        private bool isSpeedSet = false;


        ModifyRegistry mr = new ModifyRegistry();

        public Print_Setting()

        {
            InitializeComponent();
            SetLabelText();
        }

        private void SetLabelText()
        {
            // layer height
            float LayerHeight = SV_LayerHeight();
            layerheightvaluelbl.Content = LayerHeight.ToString();
            
            // no of layer
            int NoOfLayer = SV_NoofLayer();
            nooflayersvaluelbl.Content = NoOfLayer.ToString();

            //
            int PrintTime = SV_PrintTime();
            printtimevaluelbl.Content = PrintTime.ToString();
        }

        private void settingbtn_Click(object sender, RoutedEventArgs e)
        {
            //PowerSetting w = new PowerSetting();
            //w.Show();
        }

        private void PrintSetting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Read_Printsetting();
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
            }
        }

        private void Read_Printsetting()
        {
            Powertxt.Text = mr.Read("Powertxt");
            scaletxt.Text = mr.Read("Scaletxt");
            speedOntxt.Text = mr.Read("SpeedONtxt");
            speedOfftxt.Text = mr.Read("SpeedOFFtxt");
        }

        private void Powertxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Powertxt.Text != "0")
                mr.Write("Powertxt", Powertxt.Text);
        }

        private void scaletxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scaletxt.Text != "0")
                mr.Write("Scaletxt", scaletxt.Text);
        }

        private void speedOntxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (speedOntxt.Text != "0")
                mr.Write("SpeedONtxt", speedOntxt.Text);
        }

        private void speedOfftxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (speedOfftxt.Text != "0")
                mr.Write("SpeedOFFtxt", speedOfftxt.Text);
        }

        private void powerOKbtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Powertxt.Text, out int iPower))
            {
                int result = SV_Power(iPower);
                MessageBox.Show("Result: " + result.ToString());
                isPowerSet = true;
                STL.setPower = isPowerSet;
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }
        }

        private void ScaleOKbtn_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(Powertxt.Text, out float fScale))
            {
                float result = SV_Scale(fScale);
                MessageBox.Show("Result: " + result.ToString());
                isScaleSet = true;
                STL.setScale = isScaleSet;
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }
        }

        private void speedOKbtn_Click(object sender, RoutedEventArgs e)
        {
            int onSpeed, offSpeed;
            if (int.TryParse(speedOntxt.Text, out onSpeed) && int.TryParse(speedOfftxt.Text, out offSpeed))
            {
                int result = SV_Speed(onSpeed, offSpeed);
                MessageBox.Show("Result: " + result.ToString());
                isSpeedSet = true;
                STL.setSpeed = isSpeedSet;
            }
            else
            {
                MessageBox.Show("invalid input...");
            }
        }
    }
}
