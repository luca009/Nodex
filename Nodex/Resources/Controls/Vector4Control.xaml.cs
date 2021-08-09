using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace Nodex.Resources.Controls
{
    /// <summary>
    /// Interaction logic for Vector4Control.xaml
    /// </summary>
    public partial class Vector4Control : UserControl
    {
        private Vector4 _vector;
        public Vector4 Vector
        {
            get { return _vector; }
            set {
                _vector = value;
                UpdateControls();
            }
        }

        public Vector4Control()
        {
            InitializeComponent();
            //iupdownX = new IntegerUpDown(0, int.MaxValue, int.MinValue, 1);
            //iupdownY = new IntegerUpDown(0, int.MaxValue, int.MinValue, 1);
            //iupdownZ = new IntegerUpDown(0, int.MaxValue, int.MinValue, 1);
            //iupdownW = new IntegerUpDown(0, int.MaxValue, int.MinValue, 1);
        }

        private void any_ValueChanged(object sender, EventArgs e)
        {
            _vector.X = iupdownX.Value;
            _vector.Y = iupdownY.Value;
            _vector.Z = iupdownZ.Value;
            _vector.W = iupdownW.Value;
        }

        private void UpdateControls()
        {
            iupdownX.Value = (int)Vector.X;
            iupdownY.Value = (int)Vector.Y;
            iupdownZ.Value = (int)Vector.Z;
            iupdownW.Value = (int)Vector.W;
        }
    }
}
