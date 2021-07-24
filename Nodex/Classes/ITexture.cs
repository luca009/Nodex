using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Nodex.Classes
{
    public interface ITexture
    {
        double Evaluate(double x, double y);
        double Evaluate(double x, double y, double z);
        double Evaluate(double x, double y, double z, double w);
    }
}
