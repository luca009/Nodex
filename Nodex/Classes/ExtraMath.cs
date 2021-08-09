using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public static class ExtraMath
    {
        public static int Step(double value, int factor)
        {
            int nearestMultiple = (int)Math.Round(
                         value / factor,
                         MidpointRounding.AwayFromZero
                     ) * factor;
            return nearestMultiple;
        }

        public static int FloorStep(double value, int factor)
        {
            int nearestMultiple = (int)Math.Floor(
                         value / factor
                     ) * factor;
            return nearestMultiple;
        }

        public static int CeilingStep(double value, int factor)
        {
            int nearestMultiple = (int)Math.Ceiling(
                         value / factor
                     ) * factor;
            return nearestMultiple;
        }
    }
}
