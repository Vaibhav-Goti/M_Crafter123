using HelixToolkit.Wpf;
using Ozonscan;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Display3DModel
{
    /// <summary>
    /// Interaction logic for Machine_Setting.xaml
    /// </summary>
    public partial class Machine_Setting : UserControl
    {
        ModifyRegistry mr = new ModifyRegistry();

        private bool handle = true;

        public Machine_Setting()
        {
            InitializeComponent();

        }

        private void MachineSetting_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Read_MachineSetting();
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
            }
        }

        private void Read_MachineSetting()
        {
            MachinenameCoBox.SelectedItem = mr.Read("MachineName");
            PrinterBedTypeCoBox.SelectedItem = mr.Read("BedType");
            sizetxt.Text = mr.Read("Size");
            scaleXtxt.Text = mr.Read("ScaleX");
            scaleYtxt.Text = mr.Read("ScaleY");
            scaleZtxt.Text = mr.Read("ScaleZ");
        }

        private void MachinenameCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (MachinenameCoBox.SelectedItem != "0")
                mr.Write("MachineName", MachinenameCoBox.SelectedItem);
        }
        private void scaleXtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scaleXtxt.Text != "0")
                mr.Write("ScaleX", scaleXtxt.Text);


            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            ModelVisual3D modelVisual = mainWindow.GetModelVisual();

            double height;
            if (double.TryParse(scaleXtxt.Text, out height))
            {
                var scaleTransform = modelVisual.Transform as ScaleTransform3D;
                if (scaleTransform != null)
                {
                    scaleTransform.ScaleX = height / helixviewport3d.ActualWidth;

                }
            }
        }

        private void scaleYtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scaleYtxt.Text != "0")
                mr.Write("ScaleY", scaleYtxt.Text);


            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            ModelVisual3D modelVisual = mainWindow.GetModelVisual();

            double width;
            if (double.TryParse(scaleYtxt.Text, out width))
            {
                // Scale the STL model vertically based on the new height
                var scaleTransform = modelVisual.Transform as ScaleTransform3D;
                if (scaleTransform != null)
                {
                    scaleTransform.ScaleY = width / helixviewport3d.ActualHeight;
                }
            }
        }

        private void scaleZtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scaleZtxt.Text != "0")
                mr.Write("ScaleZ", scaleZtxt.Text);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            ModelVisual3D modelVisual = mainWindow.GetModelVisual();

            double size;
            if (double.TryParse(scaleZtxt.Text, out size))
            {
                // Scale the STL model vertically based on the new height
                var scaleTransform = modelVisual.Transform as ScaleTransform3D;
                if (scaleTransform != null)
                {
                    scaleTransform.ScaleZ = size / helixviewport3d.ActualHeight;
                }
            }
        }

        private void PrinterBedTypeCoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (PrinterBedTypeCoBox.SelectedItem != "0")
            System.Windows.Controls.ComboBox cmb = sender as System.Windows.Controls.ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
            mr.Write("BedType", PrinterBedTypeCoBox.SelectedItem);
        }

        private void Handle()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            switch (PrinterBedTypeCoBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Square":

                    helixviewport3d.Children.Remove(STL.vis);
                    break;

                case "Round":

                    AddCircleModel();
                    break;

            }
        }

        private void sizetxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sizetxt.Text != "0")
                mr.Write("Size", sizetxt.Text);
        }

     

        private void loadfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private GeometryModel3D GetCircleModel(double radius, Vector3D normal, Point3D center, int resolution)
        {
            var mod = new GeometryModel3D();
            var geo = new MeshGeometry3D();

            // Generate the circle in the XZ-plane
            // Add the center first
            geo.Positions.Add(new Point3D(-10, -10, -10));

            // Iterate from angle 0 to 2*PI
            double t = 2 * Math.PI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                geo.Positions.Add(new Point3D(radius * Math.Cos(t * i), 0, -radius * Math.Sin(t * i)));
            }

            // Add points to MeshGeoemtry3D
            for (int i = 0; i < resolution; i++)
            {
                var a = 0;
                var b = i + 1;
                var c = (i < (resolution - 1)) ? i + 2 : 1;

                geo.TriangleIndices.Add(a);
                geo.TriangleIndices.Add(b);
                geo.TriangleIndices.Add(c);
            }

            mod.Geometry = geo;

            // Create transforms
            var trn = new Transform3DGroup();
            // Up Vector (normal for XZ-plane)
            var up = new Vector3D(0, 1, 0);
            // Set normal length to 1
            normal.Normalize();
            var axis = Vector3D.CrossProduct(up, normal); // Cross product is rotation axis
            var angle = Vector3D.AngleBetween(up, normal); // Angle to rotate
            trn.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle)));
            trn.Children.Add(new TranslateTransform3D(new Vector3D(center.X, center.Y, center.Z)));
            mod.Transform = trn;
            return mod;
        }

        private void AddCircleModel()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            var mod = GetCircleModel(100, new Vector3D(0, 0, 100), new Point3D(0, -1, 0), 100);
            mod.Material = new DiffuseMaterial(System.Windows.Media.Brushes.LightGray);
            var vis = new ModelVisual3D() { Content = mod };
            helixviewport3d.Children.Add(vis);
            STL.vis = vis;
        }
    }
}
