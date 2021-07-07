using Nodex.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Nodex.Classes
{
    public static class DynamicLine
    {
        public static void RecalculateConnection(this Line line, Point point1, Point point2)
        {
            if (line == null || point1 == null || point2 == null)
                return;
            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;
        }
        public static void RecalculateConnection(this Line line, NodeOutputControl nodeOutputControl, Point point)
        {
            if (line == null || nodeOutputControl == null || point == null)
                return;
            Ellipse nodeOutputControlEllipse = (Ellipse)((Grid)nodeOutputControl.Content).Children[1];
            Point nodeOutputControlRelativePoint = nodeOutputControlEllipse.TranslatePoint(new Point(nodeOutputControlEllipse.Width / 2, nodeOutputControlEllipse.Height / 2), nodeOutputControl.parentCanvas);
            
            line.X1 = nodeOutputControlRelativePoint.X;
            line.Y1 = nodeOutputControlRelativePoint.Y;
            line.X2 = point.X;
            line.Y2 = point.Y;
        }
        public static void RecalculateConnection(this Line line, Point point, NodeInputControl nodeInputControl)
        {
            if (line == null || point == null || nodeInputControl == null)
                return;
            Ellipse nodeInputControlEllipse = (Ellipse)((Grid)nodeInputControl.Content).Children[1];
            Point nodeInputControlRelativePoint = nodeInputControlEllipse.TranslatePoint(new Point(nodeInputControlEllipse.Width / 2, nodeInputControlEllipse.Height / 2), nodeInputControl.parentCanvas);
            
            line.X1 = point.X;
            line.Y1 = point.Y;
            line.X2 = nodeInputControlRelativePoint.X;
            line.Y2 = nodeInputControlRelativePoint.Y;
        }
        public static void RecalculateConnection(this Line line, NodeOutputControl nodeOutputControl, NodeInputControl nodeInputControl)
        {
            if (line == null || nodeOutputControl == null || nodeInputControl == null)
                return;
            //Get the two connecting ellipses' centers
            Ellipse nodeInputControlEllipse = (Ellipse)((Grid)nodeInputControl.Content).Children[0];
            Ellipse nodeOutputControlEllipse = (Ellipse)((Grid)nodeOutputControl.Content).Children[1];
            Point nodeInputControlRelativePoint = nodeInputControlEllipse.TranslatePoint(new Point(nodeInputControlEllipse.Width / 2, nodeInputControlEllipse.Height / 2), nodeInputControl.parentCanvas);
            Point nodeOutputControlRelativePoint = nodeOutputControlEllipse.TranslatePoint(new Point(nodeOutputControlEllipse.Width / 2, nodeOutputControlEllipse.Height / 2), nodeOutputControl.parentCanvas);
            
            line.X1 = nodeOutputControlRelativePoint.X;
            line.Y1 = nodeOutputControlRelativePoint.Y;
            line.X2 = nodeInputControlRelativePoint.X;
            line.Y2 = nodeInputControlRelativePoint.Y;

            line.UpdateLayout();
        }
    }
}
