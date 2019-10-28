using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Aufgabe2
{
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // KLASSE TDreieck
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class TDreieck
    {
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // KONSTRUKTOR
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public TDreieck(Point _A, Point _B, Point _C)
        {
            Gespiegelt = false;

            // Erstmalig Punkte setzen
            SetzePunkte(_A, _B, _C);
            BerechneWinkel();

            // Punkte setzen sodass an Punkt A der kleinsten Winkel anliegt
            SetzePunkteNachWinkel();

            // Winkel & Strecken neuberechnen
            BerechneWinkel();
            BerechneStrecken();
        }

        private void SetzePunkteNachWinkel()
        {
            // Punkt A auf Punkt mit kleinstem Winkel setzen
            if(WB < WA && WB < WC)
            {
                // Punkt B hat kleinsten Winkel
                SetzePunkte(B, A, C);
            }
            else if(WC < WA && WC < WB)
            {
                // Punkt C hat kleinsten Winkel
                SetzePunkte(C, A, B);
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // VARIABLEN
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public Point A, B, C;           // Punkte A, B, C
        public double AB, BC, CA;       // Strecken AB, BC, CA
        public double WA, WB, WC;       // Winkel in A, B, C
        public bool Gespiegelt;         // Flag für vertauschte Punkte nach dem Spiegeln
        public double LaengsteStrecke;  // Längste Strecke des Dreiecks

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public Polygon Dreieck
        {
            get
            {
                Polygon P = new Polygon();
                P.Points.Add(A);
                P.Points.Add(B);
                P.Points.Add(C);
                return P;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // METHODEN
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public void SpiegelnStartbehaelter()
        {
            // Muss gespiegelt werden?
            if (AB > CA) { Spiegeln(); }
        }

        public void SpiegelnEndbehaelter()
        {
            // Muss gespiegelt werden?
            if (CA > AB) { Spiegeln(); }
        }

        public void Verschieben(Vector Verschiebung)
        {
            A.X += Verschiebung.X;
            A.Y += Verschiebung.Y;
            B.X += Verschiebung.X;
            B.Y += Verschiebung.Y;
            C.X += Verschiebung.X;
            C.Y += Verschiebung.Y;
        }

        public void Drehen(double Winkel)
        {
            // Gradmaß in Bogemaß umrechnen
            Winkel *= Math.PI / 180;

            // Punkt speichern
            Point A_Alt = A;

            // Zum Ursprung verschieben
            Point Verschiebung = A;
            Verschiebung.X *= -1;
            Verschiebung.Y *= -1;
            Verschieben(new Vector(Verschiebung.X, Verschiebung.Y));

            // Drehen mit Drehmatrix
            double MA = Math.Cos(Winkel);
            double MB = -Math.Sin(Winkel);
            double MC = Math.Sin(Winkel);
            double MD = Math.Cos(Winkel);

            TMatrix M = new TMatrix(MA, MB, MC, MD);

            Vector VA = M.VektorMult(new Vector(A.X, A.Y));
            Vector VB = M.VektorMult(new Vector(B.X, B.Y));
            Vector VC = M.VektorMult(new Vector(C.X, C.Y));

            A = new Point(VA.X, VA.Y);
            B = new Point(VB.X, VB.Y);
            C = new Point(VC.X, VC.Y);

            // Zurück verschieben
            Verschieben(new Vector(A_Alt.X, A_Alt.Y));
        }

        public void Spiegeln()
        {
            Point CAlt = C;
            Point BAlt = B;

            double XDistAB = A.X - B.X;
            double XDistAC = A.X - C.X;

            B = new Point(A.X + XDistAC, CAlt.Y);
            C = new Point(A.X + XDistAB, BAlt.Y);

            BerechneStrecken();
            BerechneWinkel();

            Gespiegelt = !Gespiegelt;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void SetzePunkte(Point _A, Point _B, Point _C)
        {
            // Punkt A ist immer Punkt A
            A = _A;

            // Determinante bestimmen
            Vector AB = _B - _A;
            Vector AC = _C - _A;

            double Determinante = Vector.Determinant(AB, AC);
            if(Determinante == 0)
            {
                MessageBox.Show("Invalides Dreieck");
                return;
            }

            // Bestimmen ob B und C getauscht werden müssen
            if (Determinante < 0)
            {
                B = _C;
                C = _B;
            }
            else
            {
                B = _B;
                C = _C;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void BerechneStrecken()
        {
            // Vektoren bestimmen
            Vector _AB = B - A;
            Vector _BC = C - B;
            Vector _CA = A - C;

            // Betrag der Vektoren berechnen
            AB = _AB.Length;
            BC = _BC.Length;
            CA = _CA.Length;

            // Längste Strecke setzen
            LaengsteStrecke = AB;
            if(CA > LaengsteStrecke)
            {
                LaengsteStrecke = CA;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void BerechneWinkel()
        {
            // Vektoren bestimmen
            Vector _AB = B - A;
            Vector _AC = C - A;

            Vector _BA = A - B;
            Vector _BC = C - B;

            Vector _CA = A - C;
            Vector _CB = B - C;

            // Dazwischenliegenden Winkel berechnen
            WA = Vector.AngleBetween(_AB, _AC);
            WB = Vector.AngleBetween(_BC, _BA);
            WC = Vector.AngleBetween(_CA, _CB);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
}