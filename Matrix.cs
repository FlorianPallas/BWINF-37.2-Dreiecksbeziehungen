using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aufgabe2
{
    public class TMatrix
    {
        public TMatrix(double _A, double _B, double _C, double _D)
        {
            A = _A;
            B = _B;
            C = _C;
            D = _D;
        }

        public double A;
        public double B;
        public double C;
        public double D;

        public Vector VektorMult(Vector V)
        {
            return new Vector(A * V.X + B * V.Y, C * V.X + D * V.Y);
        }

        public TMatrix Mult(double X)
        {
            return new TMatrix(A * X, B * X, C * X, D * X);
        }

        public double Determinante()
        {
            return A * D - B * C;
        }

        public TMatrix Invertiert()
        {
            TMatrix M = new TMatrix(D, -B, -C, A);
            return M.Mult(1 / Determinante());
        }

        public string Zeige()
        {
            return "A: " + A + " B: " + B + Environment.NewLine + "C: " + C + " D: " + D;
        }
    }
}
