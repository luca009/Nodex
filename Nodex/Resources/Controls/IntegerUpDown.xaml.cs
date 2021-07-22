using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for IntegerUpDown.xaml
    /// </summary>
    public partial class IntegerUpDown : UserControl
    {
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public short Step { get; set; }
        public delegate void ValueChangedHandler(object sender, EventArgs e);
        public event ValueChangedHandler ValueChanged;

        public IntegerUpDown()
        {
            InitializeComponent();
        }
        public IntegerUpDown(int value = 0, int maxValue = 100, int minValue = 0, short step = 1)
        {
            InitializeComponent();
            Value = value;
            MaxValue = maxValue;
            MinValue = minValue;
            tboxInput.Text = value.ToString();
            if (step > 0)
                Step = step;
            else
                Step = 1;
        }

        private void bUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
            if (Value > MaxValue)
                Value = MaxValue;
            tboxInput.Text = Value.ToString();
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void bDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
            if (Value < MinValue)
                Value = MinValue;
            tboxInput.Text = Value.ToString();
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void tboxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tboxInput.Text == Value.ToString())
                return;

            if (tboxInput.Text == "")
                tboxInput.Text = Value.ToString();

            Regex regexObj = new Regex(@"[^\d]");
            tboxInput.Text = regexObj.Replace(tboxInput.Text, "");
            long parsedNumber = long.Parse(tboxInput.Text);

            if (parsedNumber > MaxValue)
                Value = MaxValue;
            else if (parsedNumber < MinValue)
                Value = MinValue;
            else
                Value = int.Parse(tboxInput.Text);

            tboxInput.Text = Value.ToString();

            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void tboxInput_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Value += Step;
            else
                Value -= Step;
            if (Value > MaxValue)
                Value = MaxValue;
            else if (Value < MinValue)
                Value = MinValue;
            tboxInput.Text = Value.ToString();
        }
    }
}
