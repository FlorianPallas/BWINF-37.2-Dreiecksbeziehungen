using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aufgabe2
{
    public class TBehaelter
    {
        public enum TBehaelterTyp { Startbehaelter, Mittelbehaelter, Endbehaelter };
        public List<TDreieck> Dreiecke;
        public Point TempPos;
        public TBehaelterTyp BehaelterTyp;
        public double TempWinkel;

        public TBehaelter()
        {
            Dreiecke = new List<TDreieck>();
        }

        public void DreieckHinzufügen(TDreieck D)
        {
            if(BehaelterTyp == TBehaelterTyp.Mittelbehaelter)
            {
                DreieckHinzufügenMittelbehaelter(D);
                return;
            }

            D.Verschieben(TempPos - new Point(D.A.X, D.A.Y));
            Dreiecke.Add(D);
        }

        private bool AnordnungsFlag = true;
        public void DreieckHinzufügenMittelbehaelter(TDreieck D)
        {
            D.Verschieben(TempPos - new Point(D.A.X, D.A.Y));

            if (AnordnungsFlag)
            {
                Dreiecke.Insert(0, D);
            }
            else
            {
                Dreiecke.Add(D);
            }

            AnordnungsFlag = !AnordnungsFlag;
        }

        public void DreieckeAnordnen()
        {
            // Spiegeln
            switch (BehaelterTyp)
            {
                case TBehaelterTyp.Startbehaelter:
                    {
                        foreach(TDreieck D in Dreiecke)
                        {
                            D.SpiegelnStartbehaelter();
                        }

                        AnordnenStartbehaelter();
                        break;
                    }

                case TBehaelterTyp.Mittelbehaelter:
                    {
                        SpiegelnMittelbehaelter();
                        AnordnenMittelbehaelter();
                        break;
                    }

                case TBehaelterTyp.Endbehaelter:
                    {
                        foreach (TDreieck D in Dreiecke)
                        {
                            D.SpiegelnEndbehaelter();
                        }

                        AnordnenEndbehaelter();
                        break;
                    }
            }
        }

        private void SpiegelnMittelbehaelter()
        {
            int DreieckeLinks = (int)(Math.Round((decimal)Dreiecke.Count / 2));
            int DreieckeRechts = Dreiecke.Count - DreieckeLinks;

            for (int I = 0; I < DreieckeLinks; I++)
            {
                Dreiecke[I].SpiegelnStartbehaelter();
            }

            for (int I = 0; I < DreieckeRechts; I++)
            {
                int Index = DreieckeLinks + I;
                Dreiecke[I].SpiegelnEndbehaelter();
            }
        }

        public void Verschieben(Vector Verschiebung)
        {
            TempPos += Verschiebung;
            foreach(TDreieck D in Dreiecke)
            {
                D.Verschieben(Verschiebung);
            }
        }

        private void AnordnenStartbehaelter()
        {
            const double Epsilon = 10E-10;

            Vector AnlegGerade = new Vector(-1, 0); // Horizontale Gerade als Start

            foreach (TDreieck D in Dreiecke)
            {
                // Winkel zum Anlegen berechnen
                Vector AC = D.C - D.A;
                double WinkelZuGerade = Vector.AngleBetween(AnlegGerade, AC);

                D.Drehen(-WinkelZuGerade);

                AC = D.C - D.A;
                WinkelZuGerade = Vector.AngleBetween(AnlegGerade, AC);

                if (WinkelZuGerade > Epsilon)
                {
                    D.Drehen(-WinkelZuGerade);
                }

                Vector AB = D.B - D.A;
                AnlegGerade = AB;
            }
        }

        private void AnordnenEndbehaelter()
        {
            const double Epsilon = 10E-10;

            Vector AnlegGerade = new Vector(1, 0); // Horizontale Gerade als Start

            foreach (TDreieck D in Dreiecke)
            {
                // Winkel zum Anlegen berechnen
                Vector AB = D.B - D.A;
                double WinkelZuGerade = Vector.AngleBetween(AnlegGerade, AB);

                D.Drehen(-WinkelZuGerade);

                AB = D.B - D.A;

                WinkelZuGerade = Vector.AngleBetween(AnlegGerade, AB);

                if (WinkelZuGerade > Epsilon)
                {
                    D.Drehen(-WinkelZuGerade);
                }

                Vector AC = D.C - D.A;
                AnlegGerade = AC;
            }
        }

        private void AnordnenMittelbehaelter()
        {
            AnordnenStartbehaelter();

            double FreierWinkel = 180.0 - GefuellterWinkel();

            foreach(TDreieck D in Dreiecke)
            {
                D.Drehen(-(FreierWinkel / 2));
            }
        }

        public double GefuellterWinkel()
        {
            double Winkel = 0.0;
            foreach(TDreieck D in Dreiecke)
            {
                Winkel += D.WA;
            }

            return Winkel;
        }
    }
}
