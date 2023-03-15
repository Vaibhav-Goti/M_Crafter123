using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ozonscan; 
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Windows.Media.Animation;

namespace Display3DModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_PrintConnection();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_ObjLoad();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_PrintStartPause();
        private bool printingInProgress = false;

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_PrintStop();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_PrintEmergenctStop();

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Power(int iPower);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Scale(float fScale);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_Speed(int speed);

        [DllImport("SwellMain.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SV_LaserStatus();


        private List<string> _loadedObjectPaths = new List<string>();

        private bool isLeftButtonDoubleClicked = false;

        private Material originalMaterial;

        public ModifyRegistry mr;

        private readonly List<ModelVisual3D> models = new List<ModelVisual3D>();

        private ModelVisual3D selectedModel;

        private ModelVisual3D _modelVisual;

        private Color _selectedColor;

        private Model3D device;

        bool isHelixViewPort3DUp = true;

        private PerspectiveCamera camera;

        public static MainWindow main;

        public static string openfile;

        private bool connect_machine;

        public bool start_stop_machine = false;

        private int _size;

        private int count = 1;

        public int Size { get; private set; }
        public Vector3D V2 { get; private set; }
        public object Children { get; private set; }
        public Point _lastPosition { get; private set; }

        private Model3DGroup _modelGroup;

        private TranslateTransform3D _translate;

        private Point? _lastPoint;

        private UserControl1 objectPlacement;

        private Manual_Control manualControl;

        private Print_Setting PrintSetting;

        private Machine_Setting MachineSetting;

        private ViewModel viewModel;

        private bool _isDragging;

        private Model3D _draggedObject;

        private Point3D _dragStartPosition;

        private PointHitTestResult result;

        private Dictionary<string, GeometryModel3D> _objectDictionary = new Dictionary<string, GeometryModel3D>();

        private Dictionary<string, GeometryModel3D> _modelDictionary = new Dictionary<string, GeometryModel3D>();

        private GeometryModel3D _previouslySelectedModel;

        public MainWindow()
        {
            InitializeComponent();
            //viewPort3d.MouseMove += viewPort3d_MouseMove;
            STL.ModelDictionary = _modelDictionary;
            STL.PreviouslySelectedModel = _previouslySelectedModel;

            _selectedColor = Colors.Red;


            this.DataContext = this.viewModel = new ViewModel();

            // Create a model group to hold the Model3D object
            _modelGroup = new Model3DGroup();

            // Create a translate transform to move the model
            _translate = new TranslateTransform3D();
            GeometryModel3D geometry = new GeometryModel3D();
           
            // set the geometry properties here
            ModelVisual3D modelVisual = new ModelVisual3D();
            modelVisual.Content = geometry;
            Viewport3D viewport3d = new Viewport3D();
            GridLinesVisual3D gridLinesVisual3D = new GridLinesVisual3D();
            viewPort3d.Children.Add(gridLinesVisual3D);
            viewport3d.Children.Add(modelVisual);
            STL.gridLinesVisual3D = gridLinesVisual3D;

            //STL.LoadObject= modelVisual;

            ModelVisual3D modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = _modelGroup;


            viewPort3d.Children.Add(modelVisual3D);
            // Handle the MouseDown event to move the model
            viewPort3d.MouseDown += viewPort3d_MouseDown;

            viewPort3d.Measure(new Size(200, 200));

            Point3DCollection point3Ds_X = new Point3DCollection();
            point3Ds_X.Add(new Point3D(100, 100, -0));
            point3Ds_X.Add(new Point3D(100, 100, 100));
            LinesVisual3D X_axis = new LinesVisual3D() { Points = point3Ds_X, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_M = new Point3DCollection();
            point3Ds_M.Add(new Point3D(-100, 100, -0));
            point3Ds_M.Add(new Point3D(-100, 100, 100));
            LinesVisual3D M_axis = new LinesVisual3D() { Points = point3Ds_M, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_N = new Point3DCollection();
            point3Ds_N.Add(new Point3D(-100, -100, -0));
            point3Ds_N.Add(new Point3D(-100, -100, 100));
            LinesVisual3D N_axis = new LinesVisual3D() { Points = point3Ds_N, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_O = new Point3DCollection();
            point3Ds_O.Add(new Point3D(100, -100, -0));
            point3Ds_O.Add(new Point3D(100, -100, 100));
            LinesVisual3D O_axis = new LinesVisual3D() { Points = point3Ds_O, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_Y = new Point3DCollection();
            point3Ds_Y.Add(new Point3D(100, -100, 100));
            point3Ds_Y.Add(new Point3D(100, 100, 100));
            LinesVisual3D Y_axis = new LinesVisual3D() { Points = point3Ds_Y, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_Z = new Point3DCollection();
            point3Ds_Z.Add(new Point3D(-100, 100, 100));
            point3Ds_Z.Add(new Point3D(100, 100, 100));
            LinesVisual3D Z_axis = new LinesVisual3D() { Points = point3Ds_Z, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_YY = new Point3DCollection();
            point3Ds_YY.Add(new Point3D(-100, -100, 100));
            point3Ds_YY.Add(new Point3D(-100, 100, 100));
            LinesVisual3D YY_axis = new LinesVisual3D() { Points = point3Ds_YY, Thickness = 2, Color = Colors.Black };

            Point3DCollection point3Ds_ZZ = new Point3DCollection();
            point3Ds_ZZ.Add(new Point3D(-100, -100, 100));
            point3Ds_ZZ.Add(new Point3D(100, -100, 100));
            LinesVisual3D ZZ_axis = new LinesVisual3D() { Points = point3Ds_ZZ, Thickness = 2, Color = Colors.Black };


            viewPort3d.Children.Add(X_axis);
            STL.X_axis = X_axis;
            viewPort3d.Children.Add(Y_axis);
            STL.Y_axis = Y_axis;
            viewPort3d.Children.Add(Z_axis);
            STL.Z_axis = Z_axis;
            viewPort3d.Children.Add(M_axis);
            STL.M_axis = M_axis;
            viewPort3d.Children.Add(N_axis);
            STL.N_axis = N_axis;
            viewPort3d.Children.Add(O_axis);
            STL.O_axis = O_axis;
            viewPort3d.Children.Add(YY_axis);
            STL.YY_axis = YY_axis;
            viewPort3d.Children.Add(ZZ_axis);
            STL.ZZ_axis = ZZ_axis;

        }

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="model">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                //Import 3D model file
                ModelImporter import = new ModelImporter();
                //Load the 3D model file
                device = import.Load(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not file 3D model
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        private void Object_placement_Click(object sender, RoutedEventArgs e)
        {
            settingpanel.Children.Clear();
            UserControl1 newFormControl = new UserControl1();
            settingpanel.Children.Add(newFormControl);
        }

        private void Print_Setting_Click(object sender, RoutedEventArgs e)
        {
            settingpanel.Children.Clear();
            Print_Setting newFormControl = new Print_Setting();
            settingpanel.Children.Add(newFormControl);
        }

        private void Manual_Control_Click(object sender, RoutedEventArgs e)
        {
            settingpanel.Children.Clear();
            Manual_Control newFormControl = new Manual_Control();
            settingpanel.Children.Add(newFormControl);
        }

        private void Machine_Setting_Click(object sender, RoutedEventArgs e)
        {
            settingpanel.Children.Clear();
            Machine_Setting newFormControl = new Machine_Setting();
            settingpanel.Children.Add(newFormControl);
        }

        private void loadbtn_Click(object sender, RoutedEventArgs e)
        {
            if (printingInProgress == true)
            {
                MessageBox.Show("Printing is started. You cannot add objects...");
            }
            else
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".stl";
                dlg.Filter = "STL files (*.stl)|*.stl";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    // Load the STL file using the Helix Toolkit's STL reader
                    var reader = new StLReader();
                    _modelGroup = reader.Read(dlg.FileName);
                    _modelGroup.Transform = _translate;

                    // Set the Tag property of each GeometryModel3D to the file name
                    foreach (GeometryModel3D model in _modelGroup.Children)
                    {
                        model.SetValue(TagProperty, dlg.FileName);
                    }

                    // Create a ModelVisual3D object and add the _modelGroup to it
                    _modelVisual = new ModelVisual3D();
                    _modelVisual.Content = _modelGroup;

                    // Add the model to the viewport's 3D model collection
                    viewPort3d.Children.Add(_modelVisual);
                    STL.removemodel = _modelVisual;
                    settingpanel.Children.Clear();


                    // Add the object name and model to the dictionary
                    string fileName = System.IO.Path.GetFileName(dlg.FileName);
                    _modelDictionary.Add(fileName, _modelGroup.Children[_modelGroup.Children.Count - 1] as GeometryModel3D);

                    // Add the object name to the existing list box
                     myUserControl.objectListBox.Items.Add(fileName);

                    // Add the object name to the existing list box
                    //////objectListBox.Items.Add(System.IO.Path.GetFileName(dlg.FileName));


                    STL.filename = dlg.FileName;

                    // Save the loaded STL file with ZIP extension at the specified path
                    string stlFilePath = dlg.FileName;
                    string baseZipFilePath = @"C:\Sell\" + System.IO.Path.GetFileNameWithoutExtension(stlFilePath) + ".zip";
                    string zipFilePath = baseZipFilePath;
                    int suffix = 1;
                    while (System.IO.File.Exists(zipFilePath))
                    {
                        zipFilePath = baseZipFilePath.Replace(".zip", "_" + suffix + ".zip");
                        suffix++;
                    }
                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(@"C:\Sell"))
                    {
                        Directory.CreateDirectory(@"C:\Sell");
                    }
                    using (FileStream stlFile = new FileStream(stlFilePath, FileMode.Open))
                    {
                        using (FileStream zipFile = new FileStream(zipFilePath, FileMode.Create))
                        {
                            using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Create))
                            {
                                ZipArchiveEntry entry = zip.CreateEntry(Path.GetFileName(stlFilePath));
                                using (Stream entryStream = entry.Open())
                                {
                                    stlFile.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                    //// Call the DLL function
                    //int LoadResult = SV_ObjLoad();
                    //if (LoadResult != 1)
                    //{
                    //    STL.removemodel = _modelVisual;
                    //    System.Windows.MessageBox.Show("Object loading...");
                    //}
                    //else
                    //{
                    //    System.Windows.MessageBox.Show("Error loading object.");
                    //}

                    //System.Windows.MessageBox.Show("Object loaded and saved as ZIP file.");
                }
                else
                {
                    System.Windows.MessageBox.Show("Could not load object.");
                }

                // Add the UserControl1 instance to the settingpanel if it hasn't been added already
                if (!settingpanel.Children.Contains(myUserControl))
                {
                    settingpanel.Children.Add(myUserControl);
                }
            }
        }

        UserControl1 myUserControl = new UserControl1();
        private void connectbtn_Click(object sender, RoutedEventArgs e)
        {
            int result = SV_PrintConnection();

            if (result == 1 && count == 1)
            {
                // Printer is connected
                MessageBox.Show("Printer is connected");
                System.Drawing.Bitmap bmp = Properties.Resources.connect;
                ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                connectbtn.Background = new ImageBrush(imgSrc);
                count = 2;
            }
            else if (result == 0)
            {
                // Printer is not connected
                MessageBox.Show("Printer is not connected");
                System.Drawing.Bitmap bmp = Properties.Resources.disconnect;
                ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                connectbtn.Background = new ImageBrush(imgSrc);
                count = 1;
            }
            else
            {
                // Machine is disconnected
                MessageBox.Show("Machine is disconnected");
            }
        }

        private void startstopbtn_Click(object sender, RoutedEventArgs e)
        {
            int result = SV_PrintConnection();

            if (result == 1 && count == 1 && STL.setPower == true && STL.setScale == true && STL.setSpeed == true) // printer connected
            {
                // Check if printing is currently in progress
                if (!printingInProgress)
                {
                    // Start printing
                    SV_PrintStartPause();
                    printingInProgress = true;
                    MessageBox.Show("Stop Printing");

                    System.Drawing.Bitmap bmp = Properties.Resources.pause;
                    ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    startstopbtn.Background = new ImageBrush(imgSrc);
                    count = 2;
                }
                else if(result == 0) // printer disconnected
                {
                    // Stop printing 
                    SV_PrintStartPause();
                    printingInProgress = false;
                    MessageBox.Show("Start Printing");

                    System.Drawing.Bitmap bmp = Properties.Resources.Start;
                    ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    startstopbtn.Background = new ImageBrush(imgSrc);
                    count = 1;
                }
            }
            else
            {
                MessageBox.Show("No printers are installed on this machine.");
            }
        }

        private Viewport3D ConvertToViewport3D(HelixViewport3D helixViewport)
        {
            Viewport3D viewport = new Viewport3D();
            viewport.Camera = helixViewport.Camera;
            foreach (var child in helixViewport.Children)
            {
                if (child is ModelVisual3D modelVisual)
                {
                    if (modelVisual.Content is Model3D model)
                    {
                        ModelVisual3D newChild = new ModelVisual3D();
                        newChild.Content = model;
                        viewport.Children.Add(newChild);
                    }
                    else
                    {
                        // handle other types of modelVisual.Content if necessary
                    }
                }
                else
                {
                    // handle other types of Visual3D objects if necessary
                }
            }
            // Copy other relevant properties from the HelixViewport3D object to the Viewport3D object
            return viewport;
        }

        private void viewPort3d_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var hitResult = VisualTreeHelper.HitTest(viewPort3d, e.GetPosition(viewPort3d));
            //if (hitResult != null && hitResult.VisualHit is ModelVisual3D visual3D)
            //{
            //    selectedModel = visual3D;

            //    System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(100,200,200,40);
            //    var brush = new SolidColorBrush(selectedColor);
            //    Material material = MaterialHelper.CreateMaterial(brush);

            //    if (selectedModel != null && selectedModel.Content is Model3DGroup model3dgroup)
            //    {
            //        foreach (var model in model3dgroup.Children)
            //        {
            //            if (model is GeometryModel3D geometryModel)
            //            {
            //                geometryModel.Material = material;
            //                geometryModel.BackMaterial = material;
            //            }
            //        }
            //    }

            //    // Set the selected object to the one that was clicked on            
            //    STL.removemodel = selectedModel;
            //    STL.RotateSTLModel = selectedModel;
            //    myUserControl.SelectedModel = selectedModel;
            //}

            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                isLeftButtonDoubleClicked = true;
                var hitResult = VisualTreeHelper.HitTest(viewPort3d, e.GetPosition(viewPort3d));
                if (hitResult != null && hitResult.VisualHit is ModelVisual3D visual3D)
                {
                    selectedModel = STL.Selected_Model;
                    if (selectedModel != null)
                    {
                        // Reset the material of the previously selected object
                        var group = selectedModel.Content as Model3DGroup;
                        if (group != null)
                        {
                            foreach (var model in group.Children)
                            {
                                if (model is GeometryModel3D geometryModel)
                                {
                                    geometryModel.Material = STL.UpdateMaterial;
                                    geometryModel.BackMaterial = STL.UpdateMaterial;
                                }
                            }
                        }
                    }

                    selectedModel = visual3D;
                    STL.Selected_Model = selectedModel;

                    // Store the original material of the newly selected object
                    var group2 = selectedModel.Content as Model3DGroup;
                    if (group2 != null && group2.Children.Count > 0)
                    {
                        var model = group2.Children[0] as GeometryModel3D;
                        if (model != null)
                        {
                            STL.UpdateMaterial = model.Material;
                        }
                    }

                    System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromRgb(139, 139, 139);
                    var brush = new SolidColorBrush(selectedColor);
                    Material material = MaterialHelper.CreateMaterial(brush);

                    if (selectedModel != null && selectedModel.Content is Model3DGroup model3dgroup)
                    {
                        foreach (var model in model3dgroup.Children)
                        {
                            if (model is GeometryModel3D geometryModel)
                            {
                                geometryModel.Material = material;
                                geometryModel.BackMaterial = material;
                            }
                        }
                    }
                    if (STL.vis == selectedModel)
                    {
                        selectedModel = null;
                    }
                    if (STL.X_axis == selectedModel || STL.Y_axis == selectedModel || STL.ZZ_axis == selectedModel || STL.Z_axis == selectedModel || STL.M_axis == selectedModel || STL.N_axis == selectedModel || STL.O_axis == selectedModel || STL.YY_axis == selectedModel)
                    {
                        selectedModel = null;
                    }
                    if (STL.gridLinesVisual3D == selectedModel)
                    {
                        selectedModel = null;
                    }
                    // Set the selected object to the one that was clicked on            
                    STL.removemodel = selectedModel;
                    STL.RotateSTLModel = selectedModel;
                    myUserControl.SelectedModel = selectedModel;
               
            



            //// Find the index of the loaded object in the ListBox
            //int selectedIndex = -1;
            //string loadedObjectName = System.IO.Path.GetFileName(STL.filename);
            //for (int i = 0; i < myUserControl.objectListBox.Items.Count; i++)
            //{
            //    string listBoxItemName = myUserControl.objectListBox.Items[i].ToString();
            //    if (listBoxItemName == loadedObjectName)
            //    {
            //        selectedIndex = i;
            //        break;
            //    }
            //}

            //// Set the selected index of the ListBox to the index of the loaded object
            //if (selectedIndex != -1)
            //{
            //    myUserControl.objectListBox.SelectedIndex = selectedIndex;
            //    myUserControl.objectListBox.ScrollIntoView(selectedIndex);

            //    // Change the background color of the selected ListBox item
            //    var item = (ListBoxItem)myUserControl.objectListBox.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
            //    if (item != null)
            //    {
            //        item.Background = Brushes.DarkGray;
            //    }
            //}
                }
            }
        }

        private void viewPort3d_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //System.Windows.Point position = e.GetPosition(this); // this = viewPort3D
            //double deltaX = position.X - _lastPosition.X;
            //double deltaY = position.Y - _lastPosition.Y;
            //_translate.OffsetX += deltaX;
            //_translate.OffsetY -= deltaY;
            //_lastPosition = position;

            //Xvaluetxt.Text = "X: " + position.X;
            //Yvaluetxt.Text = " Y: " + position.Y;



            //if (isLeftButtonDoubleClicked && selectedModel != null && e.LeftButton == MouseButtonState.Pressed)
            //{
            //    if (selectedModel != null && e.LeftButton == MouseButtonState.Pressed)
            //    {
            //        // Convert the mouse position to a 3D point in the viewport's coordinate system
            //        var position = e.GetPosition(viewPort3d);
            //        var point3D = viewPort3d.FindNearestPoint(position);

            //        // Move the selected model to the new position
            //        var transform = selectedModel.Transform as TranslateTransform3D;
            //        if (transform == null)
            //        {
            //            transform = new TranslateTransform3D();
            //            selectedModel.Transform = transform;
            //        }
            //        if (point3D.HasValue)
            //        {
            //            transform.OffsetX = point3D.Value.X;
            //            transform.OffsetY = point3D.Value.Y;
            //            //transform.OffsetZ = point3D.Value.Z;
            //        }
            //    }
            //}




            if (isLeftButtonDoubleClicked && selectedModel != null && e.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedModel != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    // Convert the mouse position to a 3D point in the viewport's coordinate system
                    var position = e.GetPosition(viewPort3d);
                    var point3D = viewPort3d.FindNearestPoint(position);

                    // Apply rotation
                    var rotateTransform = STL.RotateMove;
                    if (rotateTransform == null)
                    {
                        rotateTransform = new RotateTransform3D();
                    }

                    // Apply translation
                    var translateTransform = selectedModel.Transform as TranslateTransform3D;
                    if (translateTransform == null)
                    {
                        translateTransform = new TranslateTransform3D();
                        selectedModel.Transform = translateTransform;
                    }

                    // Combine rotation and translation transforms
                    var transformGroup = new Transform3DGroup();
                    transformGroup.Children.Add(rotateTransform);
                    transformGroup.Children.Add(translateTransform);

                    // Apply the combined transform to the selected model
                    selectedModel.Transform = transformGroup;

                    if (point3D.HasValue)
                    {
                        translateTransform.OffsetX = point3D.Value.X;
                        translateTransform.OffsetY = point3D.Value.Y;
                        // translateTransform.OffsetZ = point3D.Value.Z;
                    }
                    //    }
                    //}
                }
            }
        }

        public ModelVisual3D GetModelVisual()
        {
            return _modelVisual;
        }

        public Model3DGroup GetModel3DGroup()
        {
            return _modelGroup;
        }

        public HelixViewport3D GetHelixViewport3D()
        {
            return viewPort3d;

        }

        private void viewPort3d_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isLeftButtonDoubleClicked = false;
            }
        }

        private void objectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Get the selected index of the ListBox
            //int selectedIndex = objectListBox.SelectedIndex;

            //// Check if the selected index is within the bounds of the _modelGroup.Children collection
            //if (selectedIndex >= 0 && selectedIndex < _modelGroup.Children.Count)
            //{
            //    // Get the selected GeometryModel3D from the _modelGroup.Children collection
            //    GeometryModel3D selectedModel = _modelGroup.Children[selectedIndex] as GeometryModel3D;

            //    // Set the material of the selected GeometryModel3D to a red color
            //    selectedModel.Material = new DiffuseMaterial(Brushes.Red);

            //    // Set the material of all other GeometryModel3D objects to their original color
            //    foreach (GeometryModel3D model in _modelGroup.Children)
            //    {
            //        if (model != selectedModel)
            //        {
            //            model.Material = model.BackMaterial;
            //        }
            //    }

            //    // Update the viewPort3d
            //    viewPort3d.UpdateLayout();
            //}


            
            int selectedIndex = myUserControl.objectListBox.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < _modelGroup.Children.Count)
            {
                GeometryModel3D selectedModel = _modelGroup.Children[selectedIndex] as GeometryModel3D;

                selectedModel.Material = new DiffuseMaterial(Brushes.Red);

                foreach (GeometryModel3D model in _modelGroup.Children)
                {
                    if (model != selectedModel)
                    {
                        model.Material = model.BackMaterial;
                    }
                }
                // Update the viewPort3d
                viewPort3d.UpdateLayout();
            }
        }

        private void stopbtn_Click(object sender, RoutedEventArgs e)
        {
            if (printingInProgress)
            {
                int result = SV_PrintStop();
                if (result == 0 && count == 1)
                {
                    System.Drawing.Bitmap bmp = Properties.Resources.Green_stop;
                    ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    stopbtn.Background = new ImageBrush(imgSrc);
                    count = 2;
                    // print stop
                    MessageBox.Show("Printing stopped successfully");
                    stopbtn.IsEnabled = false;
                }
                else
                {
                    System.Drawing.Bitmap bmp = Properties.Resources.Red_stop;
                    ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    stopbtn.Background = new ImageBrush(imgSrc);
                    count = 1;
                    // print not stop
                    MessageBox.Show("Printing could not be stopped");
                }
            }
            else
            {
                stopbtn.IsEnabled = false;
            }
        }

        private void emergencybtn_Click(object sender, RoutedEventArgs e)
        {
            if(printingInProgress == true)
            {
                int result = SV_PrintEmergenctStop();
                if (result == 0)
                {
                    MessageBox.Show("Printing Stopped Emergency");
                }
                else
                {
                    MessageBox.Show("Printing Could Not be stopped Emergency");
                }
            }
            else
            {
                _ = emergencybtn.IsEnabled == false;
            }
           
        }

        public void BlinkingImage(Image image,int length, double repetition)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(length)),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(repetition)
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, image);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Begin(image);
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Bitmap bmp = Properties.Resources.LaserBlink;
            ImageSource imgSrc = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            startstopbtn.Background = new ImageBrush(imgSrc);
        }

        private void Label1_Loaded(object sender, RoutedEventArgs e)
        {
            //int result = SV_LaserStatus();
            //if (result != 0)
            {
                string path = "https://scitechdaily.com/images/Laser-Beam-Concept.gif";
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(path));
                image.Width = 50; // set the size of the image as per your requirement
                image.Height = 50;
                Label1.Content = image;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = "https://scitechdaily.com/images/Laser-Beam-Concept.gif";
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(path));
            image.Width = 50; // set the size of the image as per your requirement
            image.Height = 50;
            Label1.Content = image;

            //// Create a new Bitmap object from the GIF file
            //System.Drawing.Bitmap image = new Bitmap(@"C:\Users\gotiv\OneDrive\Desktop\R.gif");

            //// Set the BackgroundImage property of the button to the image
            //b1.BackgroundImage = image;
        }

        private void Label1_Loaded_1(object sender, RoutedEventArgs e)
        {
            //int result = SV_LaserStatus();
            //if (result != 0)
            //{
                var blinkAnimation = (Storyboard)FindResource("BlinkAnimation");
                var image = (Image)Label1.Content;
                image.BeginStoryboard(blinkAnimation);
            //}
            //else
            //{
            //    MessageBox.Show("Laser is Off...");
            //}
        }

        private int _previousSelectedIndex = -1;
        private int _selectedModelIndex = -1;

        private GeometryModel3D _selectedModel;
        private void objectListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //int selectedIndex = objectListBox.SelectedIndex;
            //if (selectedIndex >= 0 && selectedIndex < objectListBox.Items.Count)
            //{
            //    string selectedObjectName = objectListBox.Items[selectedIndex].ToString();
            //    GeometryModel3D selectedModel;
            //    if (_modelDictionary.TryGetValue(selectedObjectName, out selectedModel))
            //    {
            //        // Set the material of the selected model to red
            //        selectedModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkGray));
            //    }
            //}
       
            //int selectedIndex = objectListBox.SelectedIndex;
            //if (selectedIndex >= 0 && selectedIndex < objectListBox.Items.Count)
            //{
            //    string selectedObjectName = objectListBox.Items[selectedIndex].ToString();
            //    GeometryModel3D selectedModel;
            //    if (_modelDictionary.TryGetValue(selectedObjectName, out selectedModel))
            //    {
            //        // Restore the color of the previously selected model
            //        if (_previouslySelectedModel != null)
            //        {
            //            _previouslySelectedModel.Material = _previouslySelectedModel.BackMaterial;
            //        }

            //        // Set the material of the newly selected model to dark gray
            //        selectedModel.Material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkGray));

            //        // Update the previously selected model
            //        _previouslySelectedModel = selectedModel;
            //    }
            //}
        }
        
    }
}
