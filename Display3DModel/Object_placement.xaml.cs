using HelixToolkit.Wpf;
using Ozonscan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Display3DModel
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public ModelVisual3D SelectedModel { get; set; }

        private Dictionary<string, ModelVisual3D> _loadedModels = new Dictionary<string, ModelVisual3D>();

        ModifyRegistry mr = new ModifyRegistry();

        private Model3D device;

        private int count = 1;

        public GeometryModel3D PreviouslySelectedModel;

        public Dictionary<string, GeometryModel3D> ModelDictionary = new Dictionary<string, GeometryModel3D>();

        private SolidColorBrush selectedColor;

        private int previouslySelectedIndex = -1;

        private Material previouslySelectedMaterial = null;


        public UserControl1()
        {
            InitializeComponent();

            objectListBox.SelectionChanged += objectListBox_SelectionChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void movebtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            if (moveXtxt.Text != null && moveYtxt.Text != null)
            {

                if (STL.RotateSTLModel == null)
                {
                    System.Windows.MessageBox.Show("pl.select model");
                }
                else
                {
                    string coords = moveXtxt.Text;
                    string coordss = moveYtxt.Text;
                    Point point = new Point(int.Parse(coords), int.Parse(coordss));
                    var position = point;
                    // Convert the mouse position to a 3D point in the viewport's coordinate system
                    // var position = e.GetPosition(viewPort3d);
                    Point3D _point = new Point3D();
                    string[] pointsArray = moveXtxt.Text.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] pointsArray1 = moveYtxt.Text.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    point.X = int.Parse(pointsArray[0]);
                    point.Y = int.Parse(pointsArray1[0]);
                    var Point3D = helixviewport3d.FindNearestPoint(position);
                    // Move the selected model to the new position
                    var transform = STL.RotateSTLModel.Transform as TranslateTransform3D;
                    if (transform == null)
                    {
                        transform = new TranslateTransform3D();
                        STL.RotateSTLModel.Transform = transform;
                        transform.OffsetX = point.X;
                        transform.OffsetY = point.Y;
                    }
                    if (moveXtxt.Text != null && moveYtxt.Text != null)
                    {
                        transform = new TranslateTransform3D();
                        STL.RotateSTLModel.Transform = transform;
                        transform.OffsetX = point.X;
                        transform.OffsetY = point.Y;
                    }

                }
            }
        }

        private void Multiplybtn_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            ModelVisual3D modelVisual = mainWindow.GetModelVisual();
            Model3DGroup model3dgroup = mainWindow.GetModel3DGroup();
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            if (!string.IsNullOrEmpty(multiplyXYtxt.Text))
            {
                if (modelVisual != null)
                {
                    multiplyXtxt.Clear();
                    multiplyYtxt1.Clear();

                    // Parse the position value from the text box
                    double position = 0.0;
                    double.TryParse(multiplyXYtxt.Text, out position);

                    // Get the position of the original model
                    Point3D originalPosition = GetModelPosition(modelVisual);

                    // Add the X and Y position of the original model to the position value
                    double newX = originalPosition.X + position;
                    double newY = originalPosition.Y + position;

                    // Clone the model group and set its position on the X axis
                    Model3DGroup copiedModelGroupX = new Model3DGroup();
                    double halfHeight = model3dgroup.Bounds.SizeY / 2;
                    copiedModelGroupX.Children.Add(model3dgroup.Children[0].Clone());
                    Transform3DGroup translateTransformX = new Transform3DGroup();
                    translateTransformX.Children.Add(new TranslateTransform3D(newX, originalPosition.Y, 0));
                    copiedModelGroupX.Transform = translateTransformX;

                    // Clone the model group and set its position on the Y axis
                    Model3DGroup copiedModelGroupY = new Model3DGroup();
                    copiedModelGroupY.Children.Add(model3dgroup.Children[0].Clone());
                    Transform3DGroup translateTransformY = new Transform3DGroup();
                    translateTransformY.Children.Add(new TranslateTransform3D(originalPosition.X, newY, 0));
                    copiedModelGroupY.Transform = translateTransformY;

                    // Create new instances of the model visual and set their content to the copied model groups
                    ModelVisual3D copiedModelVisualX = new ModelVisual3D();
                    copiedModelVisualX.Content = copiedModelGroupX;
                    ModelVisual3D copiedModelVisualY = new ModelVisual3D();
                    copiedModelVisualY.Content = copiedModelGroupY;

                    // Add the copied model visuals to the view port
                    helixviewport3d.Children.Add(copiedModelVisualX);
                    helixviewport3d.Children.Add(copiedModelVisualY);
                }
            }
              
            else if (!string.IsNullOrEmpty(multiplyXtxt.Text))
            {
                if (modelVisual != null)
                {
                    multiplyXYtxt.Clear();
                    multiplyYtxt1.Clear();

                    // Parse the position value from the text box
                    double positionX = 0.0;
                    double.TryParse(multiplyXtxt.Text, out positionX);

                    // Get the position of the original model 
                    Point3D originalPosition = GetModelPosition(modelVisual);

                    // Clone the model group and set its position on the X axis
                    Model3DGroup copiedModelGroupX = new Model3DGroup();
                    copiedModelGroupX.Children.Add(model3dgroup.Children[0].Clone());
                    Transform3DGroup translateTransformX = new Transform3DGroup();
                    double halfHeight = model3dgroup.Bounds.SizeY / 2;
                    translateTransformX.Children.Add(new TranslateTransform3D(originalPosition.X + positionX, originalPosition.Y, originalPosition.Z));
                    copiedModelGroupX.Transform = translateTransformX;

                    // Create new instances of the model visual and set their content to the copied model groups
                    ModelVisual3D copiedModelVisualX = new ModelVisual3D();
                    copiedModelVisualX.Content = copiedModelGroupX;

                    // Add the copied model visuals to the view port
                    helixviewport3d.Children.Add(copiedModelVisualX);
                }
            }

            else if (!string.IsNullOrEmpty(multiplyYtxt1.Text))
            {
                if (modelVisual != null)
                {
                    multiplyXYtxt.Clear();
                    multiplyXtxt.Clear();

                    // Parse the position value from the text box
                    double position = 0.0;
                    double.TryParse(multiplyYtxt1.Text, out position);

                    // Get the position of the original model
                    Point3D originalPosition = GetModelPosition(modelVisual);

                    // Clone the model group and set its position on the Y axis
                    Model3DGroup copiedModelGroupY = new Model3DGroup();
                    copiedModelGroupY.Children.Add(model3dgroup.Children[0].Clone());
                    Transform3DGroup translateTransformY = new Transform3DGroup();
                    translateTransformY.Children.Add(new TranslateTransform3D(originalPosition.X, originalPosition.Y + position, 0));
                    copiedModelGroupY.Transform = translateTransformY;

                    // Create new instances of the model visual and set their content to the copied model groups
                    ModelVisual3D copiedModelVisualY = new ModelVisual3D();
                    copiedModelVisualY.Content = copiedModelGroupY;

                    // Add the copied model visuals to the view port
                    helixviewport3d.Children.Add(copiedModelVisualY);
                }
            }
            else
            {

            }
        }

        private Point3D GetModelPosition(ModelVisual3D modelVisual)
        {
            Model3D model = modelVisual.Content as Model3D;
            if (model != null)
            {
                Rect3D bounds = model.Bounds;
                return new Point3D(bounds.X + (bounds.SizeX / 2), bounds.Y + (bounds.SizeY / 2), bounds.Z + (bounds.SizeZ / 2));
            }
            return new Point3D();
        }

        private void part1color_Click(object sender, RoutedEventArgs e)
        {
            if (objectListBox.SelectedIndex != -1)
            {
                // Open a color dialog to select a color
                System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
                dlg.ShowDialog();

                // Check if an object is selected
                if (objectListBox.SelectedItem != null)
                {
                    System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    var brush = new SolidColorBrush(selectedColor);
                    (sender as System.Windows.Controls.Button).Background = brush;
                    Material material = MaterialHelper.CreateMaterial(brush);

                    // Restore the color of the previously selected object
                    if (previouslySelectedIndex >= 0 && previouslySelectedIndex != objectListBox.SelectedIndex)
                    {
                        if (previouslySelectedMaterial != null)
                        {
                            GeometryModel3D previouslySelectedModel;
                            if (STL.ModelDictionary.TryGetValue(objectListBox.Items[previouslySelectedIndex].ToString(), out previouslySelectedModel))
                            {
                                previouslySelectedModel.Material = previouslySelectedMaterial;
                                previouslySelectedModel.BackMaterial = previouslySelectedMaterial;
                            }
                        }
                    }

                    // Get the selected object
                    string selectedObjectName = objectListBox.SelectedItem.ToString();
                    GeometryModel3D selectedModel;
                    if (STL.ModelDictionary.TryGetValue(selectedObjectName, out selectedModel))
                    {
                        if (selectedModel is GeometryModel3D geometryModel)
                        {
                            geometryModel.Material = material;
                            geometryModel.BackMaterial = material;
                        }

                        STL.UpdateMaterial = material; // update original material
                    }

                    // Update the previously selected index and material
                    previouslySelectedIndex = objectListBox.SelectedIndex;
                    previouslySelectedMaterial = material;
                }
            }
            else
            {
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                    var brush = new SolidColorBrush(selectedColor);
                    (sender as System.Windows.Controls.Button).Background = brush;
                    Material material = MaterialHelper.CreateMaterial(brush);

                    if (STL.Selected_Model != null && STL.Selected_Model.Content is Model3DGroup model3dgroup)
                    {
                        foreach (var model in model3dgroup.Children)
                        {
                            if (model is GeometryModel3D geometryModel)
                            {
                                geometryModel.Material = material;
                                geometryModel.BackMaterial = material;
                            }
                        }
                        STL.UpdateMaterial = material; // update original material
                    }
                }
            }
        }

        public void AddItemToListBox()
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Content = STL.ObjectName;
            objectListBox.Items.Add(listBoxItem);
        }


        private void part1hidebtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the ModelVisual3D object from the main window

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            if (count == 1)
            {
                System.Drawing.Bitmap bmp = Properties.Resources.hideOBJ;
                ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                part1hidebtn.Background = new ImageBrush(imgSrc);
                helixviewport3d.Children.Remove(STL.removemodel);
                count = 2;
            }
            else
            {
                System.Drawing.Bitmap bmp = Properties.Resources.ShowOBJ;
                ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                part1hidebtn.Background = new ImageBrush(imgSrc);
                helixviewport3d.Children.Add(STL.removemodel);
                count = 1;
            }
        }

        private void Rotatebtn_Click(object sender, RoutedEventArgs e)
        {
            double x = Convert.ToDouble(rotateXtxt.Text);
            double y = Convert.ToDouble(rotateYtxt.Text);
            double z = Convert.ToDouble(rotateZtxt.Text);
            double angle = Convert.ToDouble(rotateAtxt.Text);

            // Retrieve the ModelVisual3D object from the main window
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            ModelVisual3D modelVisual = STL.RotateSTLModel;

            // Apply rotation
            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(x, y, z), angle));
            STL.RotateMove = rotate;

            // Apply translation
            //
            //
            // to the point of rotation
            Transform3DGroup group1 = new Transform3DGroup();
            group1.Children.Add(rotate);
            TranslateTransform3D translate1 = new TranslateTransform3D(modelVisual.Transform.Value.OffsetX, modelVisual.Transform.Value.OffsetY, -modelVisual.Transform.Value.OffsetZ);
            group1.Children.Add(translate1);

            // Apply translation back to the original position
            Transform3DGroup group2 = new Transform3DGroup();
            TranslateTransform3D translate2 = new TranslateTransform3D(modelVisual.Transform.Value.OffsetX, modelVisual.Transform.Value.OffsetY, modelVisual.Transform.Value.OffsetZ);
            group2.Children.Add(translate2);

            // Combine all transformations and apply to the ModelVisual3D object
            Transform3DGroup finalGroup = new Transform3DGroup();
            finalGroup.Children.Add(group1);
            finalGroup.Children.Add(group2);
            modelVisual.Transform = finalGroup;
        }

        private void rotateXtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void rotateYtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void rotateZtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void rotateAtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void moveXtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //var textBox = sender as System.Windows.Forms.TextBox;
            //e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void moveYtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {


        }

        private void multiplyCYtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void multiplyXtxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void multiplyYtxt1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9\\.]+");
        }

        private void part1deletebtn_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            helixviewport3d.Children.Remove(STL.removemodel);


            //// Retrieve the ModelVisual3D object from the main window
            //MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            //HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            //// Get the selected index from the list box
            //int selectedIndex = this.objectListBox.SelectedIndex;
            //if (selectedIndex >= 0)
            //{
            //    // Remove the model from the Helix viewport
            //    string selectedFileName = this.objectListBox.SelectedItem.ToString();
            //    if (_loadedModels.ContainsKey(selectedFileName))
            //    {
            //        ModelVisual3D selectedModel = _loadedModels[selectedFileName];
            //        helixviewport3d.Children.Remove(selectedModel);
            //        _loadedModels.Remove(selectedFileName);
            //    }

            //    // Remove the file name from the list box
            //    this.objectListBox.Items.RemoveAt(selectedIndex);
            //}
        }

        private void part2deletebtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the ModelVisual3D object from the main window
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            helixviewport3d.Children.Remove(STL.removemodel);
        }

        private void part3deletebtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the ModelVisual3D object from the main window
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            helixviewport3d.Children.Remove(STL.removemodel);
        }

        private void part4deletebtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the ModelVisual3D object from the main window
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            helixviewport3d.Children.Remove(STL.removemodel);
        }

        private void part5deletebtn_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the ModelVisual3D object from the main window
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();

            helixviewport3d.Children.Remove(STL.removemodel);
        }

        private void multiplyXYtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(multiplyXYtxt.Text))
            {
                multiplyXtxt.Text = "";
                multiplyYtxt1.Text = "";
            }
        }

        private void multiplyXtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(multiplyXtxt.Text))
            {
                multiplyXYtxt.Text = "";
                multiplyYtxt1.Text = "";
            }
        }

        private void multiplyYtxt1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(multiplyYtxt1.Text))
            {
                multiplyXYtxt.Text = "";
                multiplyXtxt.Text = "";
            }
        }

        private void ObjectPlacement_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Read_ObjectPlacement();
            }
            catch (Exception exxx)
            {
                MessageBox.Show(exxx.Message);
            }
        }

        private void Read_ObjectPlacement()
        {
            moveXtxt.Text = mr.Read("moveXtxt");
            moveYtxt.Text = mr.Read("moveYtxt");
            rotateXtxt.Text = mr.Read("RotateXtxt");
            rotateYtxt.Text = mr.Read("RotateYtxt");
            rotateZtxt.Text = mr.Read("RotateZtxt");
            rotateAtxt.Text = mr.Read("RotateAtxt");
        }

        private void moveXtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (moveXtxt.Text != "0")
                mr.Write("moveXtxt", moveXtxt.Text);
        }

        private void moveYtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (moveYtxt.Text != "0")
                mr.Write("moveYtxt", moveYtxt.Text);
        }

        private void rotateXtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rotateXtxt.Text != "0")
                mr.Write("RotateXtxt", rotateXtxt.Text);
        }

        private void rotateYtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rotateYtxt.Text != "0")
                mr.Write("RotateYtxt", rotateYtxt.Text);
        }

        private void rotateZtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rotateZtxt.Text != "0")
                mr.Write("RotateZtxt", rotateZtxt.Text);
        }

        private void rotateAtxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rotateAtxt.Text != "0")
                mr.Write("RotateAtxt", rotateAtxt.Text);
        }

        private void objectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int selectedIndex = objectListBox.SelectedIndex;
            //if (selectedIndex >= 0 && selectedIndex < objectListBox.Items.Count)
            //{
            //    // Get the name of the selected object
            //    string selectedObjectName = objectListBox.Items[selectedIndex].ToString();
            //    GeometryModel3D selectedModel;
            //    if (STL.ModelDictionary.TryGetValue(selectedObjectName, out selectedModel))
            //    {
            //        // Restore the color of the previously selected model
            //        if (previouslySelectedIndex >= 0 && previouslySelectedIndex != selectedIndex)
            //        {
            //            if (previouslySelectedMaterial != null)
            //            {
            //                GeometryModel3D previouslySelectedModel;
            //                if (STL.ModelDictionary.TryGetValue(objectListBox.Items[previouslySelectedIndex].ToString(), out previouslySelectedModel))
            //                {
            //                    previouslySelectedModel.Material = previouslySelectedMaterial;
            //                }
            //            }
            //        }

            //        // Set the material of the stored object to dark gray
            //        selectedModel.Material = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 139, 139)));
            //        STL.ListboxSelectedObject = selectedModel.Material;

            //        // Update the previously selected index and material
            //        previouslySelectedIndex = selectedIndex;
            //        previouslySelectedMaterial = selectedModel.BackMaterial;
            //    }
            //}

            int selectedIndex = objectListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < objectListBox.Items.Count)
            {
                // Get the name of the selected object
                string selectedObjectName = objectListBox.Items[selectedIndex].ToString();
                GeometryModel3D selectedModel;
                if (STL.ModelDictionary.TryGetValue(selectedObjectName, out selectedModel))
                {
                    // Restore the color of the previously selected model
                    if (previouslySelectedIndex >= 0 && previouslySelectedIndex != selectedIndex)
                    {
                        if (previouslySelectedMaterial != null)
                        {
                            GeometryModel3D previouslySelectedModel;
                            if (STL.ModelDictionary.TryGetValue(objectListBox.Items[previouslySelectedIndex].ToString(), out previouslySelectedModel))
                            {
                                previouslySelectedModel.Material = previouslySelectedMaterial;
                            }
                        }
                    }

                    // Set the material of the stored object to dark gray
                    selectedModel.Material = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 139, 139)));
                    STL.ListboxSelectedObject = selectedModel.Material;

                    // Update the previously selected index and material
                    previouslySelectedIndex = selectedIndex;
                    previouslySelectedMaterial = selectedModel.BackMaterial;
                }
                else
                {
                    // Update the previously selected index and material if the object is deleted
                    previouslySelectedIndex = -1;
                    previouslySelectedMaterial = null;
                }
            }
        }

        public class TaggedModelVisual3D : ModelVisual3D
        {
            public object Tag { get; set; }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            HelixViewport3D helixviewport3d = mainWindow.GetHelixViewport3D();
            //Model3DGroup model3dgroup = mainWindow.GetModel3DGroup();
            //ModelVisual3D modelVisual = mainWindow.GetModelVisual();
            //helixviewport3d.Children.Remove(STL.removemodel);


            int selectedIndex = objectListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < objectListBox.Items.Count)
            {
                // Get the name of the selected object
                string selectedObjectName = objectListBox.Items[selectedIndex].ToString();
                GeometryModel3D selectedModel;
                if (STL.ModelDictionary.TryGetValue(selectedObjectName, out selectedModel))
                {
                    // Remove the object from the viewport
                    var modelVisual = helixviewport3d.Children.FirstOrDefault(m => (m is ModelVisual3D) && ((ModelVisual3D)m).Content == selectedModel);
                    if (modelVisual != null)
                    {
                        helixviewport3d.Children.Remove(modelVisual);
                    }

                    // Remove the object from the listbox
                    objectListBox.Items.RemoveAt(selectedIndex);
                    STL.filename = null;

                    // Remove the object from the dictionary
                    STL.ModelDictionary.Remove(selectedObjectName);
                }
            }
        }

        private ModelVisual3D FindParentModelVisual3D(DependencyObject child)
        {
            while ((child != null) && !(child is ModelVisual3D))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as ModelVisual3D;
        }

        private GeometryModel3D _selectedModel;
    }
}
