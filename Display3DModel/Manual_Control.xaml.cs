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
    /// Interaction logic for Manual_Control.xaml
    /// </summary>
    public partial class Manual_Control : UserControl
    {
        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_SetOutput(int iIndex, int iStatus);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_LaserOn();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_LaserOff();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_XMove(float fX, float fSpeed);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_ZMove(float fZ, float fSpeed);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_CXMove(float fCX, float fSpeed);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_CYMove(float fCY, float fSpeed);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Power(int iPower);

        ModifyRegistry mr = new ModifyRegistry();

        private bool isPowerSet = false; // a variable to keep track of whether the power has been set or not

        public Manual_Control()
        {
            InitializeComponent();
            // Assign unique indices to the checkboxes
            Chk1.Tag = 0;
            Chk2.Tag = 1;
            Chk3.Tag = 2;
            Chk4.Tag = 3;
            Chk5.Tag = 4;
            Chk6.Tag = 5;
            Chk7.Tag = 6;
            Chk8.Tag = 7;
        }

        private void ManualControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Read_ManualControl();
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
            }
        }

        private void Read_ManualControl()
        {
            xvaluetxt.Text = mr.Read("Xvalue");
            Xspeedtxt.Text = mr.Read("Xspeed");
            Zvaluetxt.Text = mr.Read("Zvalue");
            Zspeedtxt.Text = mr.Read("Zspeed");
            CXvaluetxt.Text = mr.Read("CXvalue");
            CXspeedtxt.Text = mr.Read("CXspeed");
            CYvaluetxt.Text = mr.Read("CYvalue");
            CYspeedtxt.Text = mr.Read("CYspeed");
            Powersettxt.Text = mr.Read("PowerSet");
        }

        private void xvaluetxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (xvaluetxt.Text != "0")
                mr.Write("Xvalue", xvaluetxt.Text);
        }

        private void Xspeedtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Xspeedtxt.Text != "0")
                mr.Write("Xspeed", Xspeedtxt.Text);
        }

        private void Zvaluetxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Zvaluetxt.Text != "0")
                mr.Write("Zvalue", Zvaluetxt.Text);
        }

        private void Zspeedtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Zspeedtxt.Text != "0")
                mr.Write("Zspeed", Zspeedtxt.Text);
        }

        private void CXvaluetxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CXvaluetxt.Text != "0")
                mr.Write("CXvalue", CXvaluetxt.Text);
        }

        private void CXspeedtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CXspeedtxt.Text != "0")
                mr.Write("CXspeed", CXspeedtxt.Text);
        }

        private void CYvaluetxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CYvaluetxt.Text != "0")
                mr.Write("CYvalue", CYvaluetxt.Text);
        }

        private void CYspeedtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CYspeedtxt.Text != "0")
                mr.Write("CYspeed", CYspeedtxt.Text);
        }

        private void Powersettxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Powersettxt.Text != "0")
                mr.Write("PowerSet", Powersettxt.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckBox[] checkBoxes = { Chk1, Chk2, Chk3, Chk4, Chk5, Chk6, Chk7, Chk8 };

            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if ((bool)(checkBoxes[i].IsChecked = true))
                {
                    // Checkbox is checked, set its status to 1 using the DLL function
                    SV_SetOutput(i + 1, 1);
                }
                else
                {
                    // Checkbox is unchecked, set its status to 0 using the DLL function
                    SV_SetOutput(i + 1, 0);
                }
            }
        }

        private void ONbtn_Click(object sender, RoutedEventArgs e)
        {
            int result = SV_LaserOn();
            if (result == 0)
            {
                MessageBox.Show("Laser is On...");
            }
        }

        private void OFFbtn_Click(object sender, RoutedEventArgs e)
        {
            int result = SV_LaserOff();
            if (result == 0)
            {
                MessageBox.Show("Laser Is Off...");
            }
        }

        private void Powersetbtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Powersettxt.Text, out int iPower))
            {
                int result = SV_Power(iPower);
                if (result == 0)
                {
                    isPowerSet = true; // set the flag to indicate that power has been set
                    MessageBox.Show("Power Set Successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to set power!");
                }
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }
        }

        private async void Xrightbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                // Call the DLL function with the positive value
                int result = SV_XMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                // Disable the minus button for 3 seconds
                Xleftbtn.IsEnabled = false;
                setXOKbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false; 
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                Xleftbtn.IsEnabled = true;
                setXOKbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void Xleftbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox and negate it
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                value = -value;
                // Call the DLL function with the negative value
                int result = SV_XMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                Xrightbtn.IsEnabled = false;
                setXOKbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                Xrightbtn.IsEnabled = true;
                setXOKbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void Zrightbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                // Call the DLL function with the positive value
                int result = SV_ZMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                // Disable the minus button for 3 seconds
               
                Zleftbtn.IsEnabled = false;
                setXOKbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                Zleftbtn.IsEnabled = true;
                setXOKbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                Xleftbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void Zleftbtn_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                value = -value;
                // Call the DLL function with the negative value
                int result = SV_ZMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");


               
                setXOKbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

               
                setXOKbtn.IsEnabled = true;
                Xleftbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void CXrightbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                // Call the DLL function with the positive value
                int result = SV_CXMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                // Disable the minus button for 3 seconds
                
                
                
                setXOKbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                Xleftbtn.IsEnabled = true;
                setXOKbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }


        private async void CXleftbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox and negate it
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                value = -value;
                // Call the DLL function with the negative value
                int result = SV_CXMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");


                setXOKbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                setXOKbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                Xleftbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void CYrightbtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the value from the textbox
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                // Call the DLL function with the positive value
                int result = SV_CYMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                // Disable the minus button for 3 seconds
                setXOKbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYleftbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                setXOKbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                Xleftbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYleftbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private async void CYleftbtn_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(xvaluetxt.Text, out float value))
            {
                value = -value;
                // Call the DLL function with the negative value
                int result = SV_ZMove(value, 0.0f);
                // Do something with the result
                Console.WriteLine($"DLL function returned {result}");

                setXOKbtn.IsEnabled = false;
                Xrightbtn.IsEnabled = false;
                Xleftbtn.IsEnabled = false;
                setzOKbtn.IsEnabled = false;
                Zrightbtn.IsEnabled = false;
                Zleftbtn.IsEnabled = false;
                CXrightbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;
                CYrightbtn.IsEnabled = false;
                CXleftbtn.IsEnabled = false;
                setCXOKbtn.IsEnabled = false;

                await Task.Delay(3000);

                setXOKbtn.IsEnabled = true;
                Xrightbtn.IsEnabled = true;
                Xleftbtn.IsEnabled = true;
                setzOKbtn.IsEnabled = true;
                Zrightbtn.IsEnabled = true;
                Zleftbtn.IsEnabled = true;
                CXrightbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
                CYrightbtn.IsEnabled = true;
                CXleftbtn.IsEnabled = true;
                setCXOKbtn.IsEnabled = true;
            }
        }

        private void setXOKbtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the value from the TextSpeed textbox
            string speedText = Xspeedtxt.Text;

            // Convert the string value to float
            if (float.TryParse(speedText, out float speed))
            {
                // Call the DLL function with the speed argument
                int result = SV_XMove(0.0f, speed);
                // Check the result if needed
            }
            else
            {
                MessageBox.Show("Invalid speed value entered!");
            }
        }

        private void setzOKbtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the value from the TextSpeed textbox
            string speedText = Zspeedtxt.Text;

            // Convert the string value to float
            if (float.TryParse(speedText, out float speed))
            {
                // Call the DLL function with the speed argument
                int result = SV_ZMove(0.0f, speed);
                // Check the result if needed
            }
            else
            {
                MessageBox.Show("Invalid speed value entered!");
            }
        }

        private void setCXOKbtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the value from the TextSpeed textbox
            string speedText = CXspeedtxt.Text;

            // Convert the string value to float
            if (float.TryParse(speedText, out float speed))
            {
                // Call the DLL function with the speed argument
                int result = SV_CXMove(0.0f, speed);
                // Check the result if needed
            }
            else
            {
                MessageBox.Show("Invalid speed value entered!");
            }
        }
        private void setCYOKbtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the value from the TextSpeed textbox
            string speedText = CYspeedtxt.Text;

            // Convert the string value to float
            if (float.TryParse(speedText, out float speed))
            {
                // Call the DLL function with the speed argument
                int result = SV_CYMove(0.0f, speed);
                // Check the result if needed
            }
            else
            {
                MessageBox.Show("Invalid speed value entered!");
            }
        }

    }
}
