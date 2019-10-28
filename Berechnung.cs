using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aufgabe2
{
    public class TBerechnung
    {
        public TBehaelter[] Behaelter;

        public TBerechnung(TBehaelter[] _Behaelter)
        {
            Behaelter = new TBehaelter[_Behaelter.Length];
            Array.Copy(_Behaelter, Behaelter, _Behaelter.Length);
        }

        public double Berechnen()
        {
            // Behälter verschieben
            for (int I = 0; I < Behaelter.Length - 1; I++)
            {
                BehaelterAnnaehern(Behaelter[I], Behaelter[I + 1]);
            }

            double EntfernungFinal;
            if (Behaelter.Length < 2)
            {
                EntfernungFinal = 0;
            }
            else
            {
                EntfernungFinal = Behaelter[Behaelter.Length - 1].TempPos.X - Behaelter[0].TempPos.X;
            }

            return EntfernungFinal;
        }

        private void BehaelterAnnaehern(TBehaelter B1, TBehaelter B2)
        {
            const double Distanz = 1000;
            const double StrahlLaenge = 10 * Distanz;

            Vector B2Verschub = new Vector((B1.TempPos.X + Distanz) - B2.TempPos.X, 0 - B2.TempPos.Y);
            B2.Verschieben(B2Verschub);

            List<Vector> Results1 = new List<Vector>();
            foreach (TDreieck D1 in B1.Dreiecke)
            {
                Point[] Eckpunkte = new Point[] { D1.A, D1.B, D1.C };
                foreach (Point Eckpunkt in Eckpunkte)
                {
                    foreach (TDreieck D2 in B2.Dreiecke)
                    {
                        Point B = Eckpunkt + new Vector(StrahlLaenge, 0);
                        DreieckKollision(Eckpunkt, B, D2.A, D2.B, ref Results1);
                        DreieckKollision(Eckpunkt, B, D2.B, D2.C, ref Results1);
                        DreieckKollision(Eckpunkt, B, D2.C, D2.A, ref Results1);
                    }
                }
            }

            List<Vector> Results2 = new List<Vector>();
            foreach (TDreieck D2 in B2.Dreiecke)
            {
                Point[] Eckpunkte = new Point[] { D2.A, D2.B, D2.C };
                foreach (Point Eckpunkt in Eckpunkte)
                {
                    foreach (TDreieck D1 in B1.Dreiecke)
                    {
                        Point B = Eckpunkt + new Vector(-StrahlLaenge, 0);
                        DreieckKollision(Eckpunkt, B, D1.A, D1.B, ref Results2);
                        DreieckKollision(Eckpunkt, B, D1.B, D1.C, ref Results2);
                        DreieckKollision(Eckpunkt, B, D1.C, D1.A, ref Results2);
                    }
                }
            }

            List<double> Distanzen1 = new List<double>();
            foreach (Vector V in Results1)
            {
                Distanzen1.Add(V.Y * StrahlLaenge);
            }

            foreach (Vector V in Results2)
            {
                Distanzen1.Add(V.Y * StrahlLaenge);
            }

            double KleinsteDistanz = double.PositiveInfinity;
            foreach (double D in Distanzen1)
            {
                if (D < KleinsteDistanz) { KleinsteDistanz = D; }
            }

            B2.Verschieben(new Vector(-KleinsteDistanz, 0));
        }

        private void DreieckKollision(Point A, Point B, Point P, Point Q, ref List<Vector> Results)
        {
            Vector ST;
            if (Kollision(A, B, P, Q, out ST))
            {
                Results.Add(ST);
            }
        }

        private bool Kollision(Point A, Point B, Point P, Point Q, out Vector ST)
        {
            Vector BA = new Vector(A.X - B.X, A.Y - B.Y);
            Vector PQ = new Vector(Q.X - P.X, Q.Y - P.Y);
            Vector PA = new Vector(A.X - P.X, A.Y - P.Y);

            TMatrix M = new TMatrix(PQ.X, BA.X, PQ.Y, BA.Y);

            if (M.Determinante() == 0.0)
            {
                ST = new Vector();
                return false;
            }

            Vector X = M.Invertiert().VektorMult(PA);

            double s = X.X;
            double t = X.Y;

            ST = new Vector(s, t);

            const double Eps = 1E-12;

            if ((s < 0.0) || (t <= Eps) || (s > 1.0) || (t >= 1.0 - Eps)) { return false; }

            return true;
        }
    }
}
