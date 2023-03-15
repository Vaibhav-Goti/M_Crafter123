using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Display3DModel
{
    public static class STL
    {
        public static ModelVisual3D removemodel;

        public static ModelVisual3D RotateObject;

        public static ModelVisual3D Rotate;

        public static ModelVisual3D RotateSTLModel;

        public static ModelVisual3D Selected_Model;

        public static Material ListboxSelectedObject;

        public static ModelVisual3D loadeModel;

        public static ModelVisual3D vis;

        public static RotateTransform3D RotateMove;

        public static Model3DGroup LoadObject;

        public static Material UpdateMaterial;

        public static string filename;

        public static string OpenFile;

        public static string ObjectName;

        public static bool setSpeed;

        public static bool setScale;

        public static bool setPower;

        public static GeometryModel3D PreviouslySelectedModel;

        public static Dictionary<string, GeometryModel3D> ModelDictionary;

        internal static Visual3D previouslySelectedMaterial;

        public static GridLinesVisual3D gridLinesVisual3D;

        public static LinesVisual3D X_axis;

        public static LinesVisual3D M_axis;

        public static LinesVisual3D N_axis;

        public static LinesVisual3D O_axis;

        public static LinesVisual3D Y_axis;

        public static LinesVisual3D Z_axis;

        public static LinesVisual3D YY_axis;

        public static LinesVisual3D ZZ_axis;
    }
}
