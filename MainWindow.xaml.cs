using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace Aufgabe2
{
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // KLASSE MainWindow
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class MainWindow : Window
    {
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // KONSTRUKTOR
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public MainWindow()
        {
            InitializeComponent();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // VARIABLEN
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private TDreieck[] Dreiecke;
        List<TBehaelter> Behaelter;
        private double Scale;
        private double KleinstesY;
        private double GroesstesY;
        private double KleinstesX;
        private double GroesstesX;
        private const double Rand = 100;
        private double OffsetX = 0;
        private double OffsetY = 0;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // METHODEN
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void Zeichne()
        {
            ZeichenCanvas.Children.Clear();
            SetzeSkalierung();
            ZeichneBehaelter();
            ZeichneLinie();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void ZeichneLinie()
        {
            if(Behaelter != null)
            {
                if(Behaelter.Count > 1)
                {
                    Point Pos1 = PunktZuCanvas(Behaelter[0].TempPos);
                    Point Pos2 = PunktZuCanvas(Behaelter[Behaelter.Count - 1].TempPos);

                    Line L = new Line()
                    {
                        X1 = Pos1.X,
                        X2 = Pos2.X,
                        Y1 = Pos1.Y + 10,
                        Y2 = Pos2.Y + 10,
                        Stroke = Brushes.Red,
                        StrokeThickness = 5
                    };

                    ZeichenCanvas.Children.Add(L);
                }
                else
                {
                    Ellipse E = new Ellipse()
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Red
                    };

                    Point Pos = PunktZuCanvas(Behaelter[0].TempPos);

                    ZeichenCanvas.Children.Add(E);

                    Canvas.SetTop(E, Pos.Y - 5);
                    Canvas.SetLeft(E, Pos.X - 5);
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void SetzeSkalierung()
        {
            if(Behaelter != null)
            {
                KleinstesY = double.PositiveInfinity;
                GroesstesY = double.NegativeInfinity;
                KleinstesX = double.PositiveInfinity;
                GroesstesX = double.NegativeInfinity;

                foreach (TBehaelter B in Behaelter)
                {
                    foreach(TDreieck D in B.Dreiecke)
                    {
                        Point[] Ecken = new Point[3] { D.A, D.B, D.C };
                        foreach(Point P in Ecken)
                        {
                            if (P.Y > GroesstesY) { GroesstesY = P.Y; }
                            if (P.Y < KleinstesY) { KleinstesY = P.Y; }
                            if (P.X > GroesstesX) { GroesstesX = P.X; }
                            if (P.X < KleinstesX) { KleinstesX = P.X; }
                        }
                    }
                }

                double ScaleX = ZeichenCanvas.ActualWidth / (GroesstesX - KleinstesX + Rand);
                double ScaleY = ZeichenCanvas.ActualHeight / (GroesstesY - KleinstesY + Rand);

                Scale = Math.Min(ScaleX, ScaleY);

                OffsetX = ZeichenCanvas.ActualWidth - (GroesstesX - KleinstesX) * Scale;
                OffsetY = ZeichenCanvas.ActualHeight - (GroesstesY - KleinstesY) * Scale;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void ZeichneDreiecke(TDreieck[] _Dreiecke)
        {
            if(_Dreiecke == null) { return; }
            foreach(TDreieck D in _Dreiecke)
            {
                Polygon P = D.Dreieck;

                P.Fill = Brushes.DarkGray;
                if(D.Gespiegelt)
                {
                    P.Fill = Brushes.Gray;
                }

                ZeichenCanvas.Children.Add(PolygonZuCanvas(P));
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void ZeichneBehaelter()
        {
            if (Behaelter == null) { return; }
            foreach (TBehaelter B in Behaelter)
            {
                ZeichneDreiecke(B.Dreiecke.ToArray());
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private Polygon PolygonZuCanvas(Polygon P)
        {
            for (int I = 0; I < P.Points.Count; I++)
            {
                P.Points[I] = PunktZuCanvas(P.Points[I]);
            }

            return P;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private Point PunktZuCanvas(Point P)
        {
            return new Point((P.X - KleinstesX) * Scale + OffsetX / 2, ZeichenCanvas.ActualHeight - P.Y * Scale - OffsetY / 2);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++pai+

        private void DateiEinlesen(string Datei)
        {
            string[] Zeilen = Datei.Split('\n');
            string[] StartpunktStrings = Zeilen[Zeilen.Length - 1].Split(' ');

            int DreieckAnzahl;
            TDreieck[] _Dreiecke;

            try
            {
                DreieckAnzahl = int.Parse(Zeilen[0]);
                _Dreiecke = new TDreieck[DreieckAnzahl];

                for (int I = 0; I < DreieckAnzahl; I++)
                {
                    string[] HindernisString = Zeilen[I + 1].Split(' ');
                    int PunktAnzahl = int.Parse(HindernisString[0]);
                    Point[] Punkte = new Point[PunktAnzahl];

                    for (int J = 0; J < PunktAnzahl; J++)
                    {
                        double X = double.Parse(HindernisString[J * 2 + 1]);
                        double Y = double.Parse(HindernisString[J * 2 + 2]);

                        Punkte[J] = new Point(X, Y);
                    }

                    if (Punkte.Length > 3)
                    {
                        throw new Exception("Mehr Punkte als erwartet!");
                    }

                    _Dreiecke[I] = new TDreieck(Punkte[0], Punkte[1], Punkte[2]);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Die Datei konnte nicht korrekt eingelesen werden!" + Environment.NewLine + Ex.Message);
                return;
            }

            Dreiecke = _Dreiecke.ToArray();

            MenuItemSaveAs.IsEnabled = true;
            Berechnen();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void Berechnen()
        {

            // Alle Dreiecke in Liste zuordnen & Behaelterliste erstellen
            List<TDreieck> DreieckeUnzugeteilt = Dreiecke.ToList();
            List<TBehaelter> _Behaelter = new List<TBehaelter>();

            // Dreiecke nach KLEINSTER Kante ordnen | Worst Case: O(N)=N² Operation
            DreieckeUnzugeteilt = OrdneNachKleinsterStrecke(DreieckeUnzugeteilt);

            // Gesamtwinkel bestimmen
            double GesamtWinkel = 0.0;
            foreach (TDreieck D in DreieckeUnzugeteilt)
            {
                GesamtWinkel += D.WA;
            }

            // Handeln nach Gesamtwinkel
            if (GesamtWinkel < 180.0)
            {
                // 1 Behaelter ausreichend
                TBehaelter B = new TBehaelter();
                B.BehaelterTyp = TBehaelter.TBehaelterTyp.Startbehaelter;

                _Behaelter.Add(B);

                while (DreieckeUnzugeteilt.Count > 0)
                {
                    TDreieck D = DreieckeUnzugeteilt[DreieckeUnzugeteilt.Count - 1];
                    _Behaelter[0].DreieckHinzufügen(D);
                    DreieckeUnzugeteilt.RemoveAt(DreieckeUnzugeteilt.Count - 1);
                }
            }
            else
            {
                // 2 oder mehr Behaelter notwendig?
                if (BerechnenMittelbehaelterNoetig(Dreiecke.ToList()))
                {
                    // Start-, Mittel- und Endbehälter
                    TBehaelter BStart = new TBehaelter();
                    TBehaelter BEnde = new TBehaelter();
                    BStart.BehaelterTyp = TBehaelter.TBehaelterTyp.Startbehaelter;
                    BEnde.BehaelterTyp = TBehaelter.TBehaelterTyp.Endbehaelter;

                    _Behaelter.Add(BStart);
                    _Behaelter.Add(BEnde);

                    // Mittelbehälterzahl (ungefähr - im worst case sind es mehr)
                    int BehaelterZahlMindestens = (int)Math.Ceiling(GesamtWinkel / 180.0);
                    int MittelbehaelterZahlMindestens = BehaelterZahlMindestens - 2;

                    if(MittelbehaelterZahlMindestens > 0)
                    {
                        for(int I = 0; I < MittelbehaelterZahlMindestens; I++)
                        {
                            TBehaelter BMittel = new TBehaelter();
                            BMittel.BehaelterTyp = TBehaelter.TBehaelterTyp.Mittelbehaelter;

                            _Behaelter.Insert(1,BMittel);
                        }
                    }

                    // Abwechselnd Start- und Endbehälter auf 90° auffüllen (Dreiecke mit größter Strecke zuerst)
                    bool InStartcontainer = true;
                    double WinkelStart = 0.0;
                    double WinkelEnd = 0.0;
                    while ((WinkelStart < 90.0) && (WinkelEnd < 90.0) && DreieckeUnzugeteilt.Count > 0)
                    {
                        TDreieck D = DreieckeUnzugeteilt[DreieckeUnzugeteilt.Count - 1];

                        if (InStartcontainer)
                        {
                            WinkelStart += D.WA;

                            if (WinkelStart > 90.0)
                            {
                                continue;
                            }

                            BStart.DreieckHinzufügen(D);
                        }
                        else
                        {
                            WinkelEnd += D.WA;

                            if (WinkelEnd > 90.0)
                            {
                                continue;
                            }

                            BEnde.DreieckHinzufügen(D);
                        }

                        DreieckeUnzugeteilt.RemoveAt(DreieckeUnzugeteilt.Count - 1);

                        // Flag umkehren
                        InStartcontainer = !InStartcontainer;
                    }

                    // Temporären Winkel setzen
                    foreach (TBehaelter B in _Behaelter)
                    {
                        B.TempWinkel = 0.0;
                    }

                    // Start- und Endbehälter sind bereits teilweise gefüllt
                    BStart.TempWinkel = BStart.GefuellterWinkel();
                    BEnde.TempWinkel = BEnde.GefuellterWinkel();

                    // Restliche Dreiecke auch unter Mittelbehältern zuteilen
                    int Index = 0;
                    int Fehlzaehler = 0;
                    while (DreieckeUnzugeteilt.Count > 0)
                    {
                        TBehaelter B = _Behaelter[Index];
                        TDreieck D = DreieckeUnzugeteilt[DreieckeUnzugeteilt.Count - 1];

                        // Dreieck hinzufügen wenn noch Platz ist
                        B.TempWinkel += D.WA;

                        if (B.TempWinkel < 180.0)
                        {
                            // Dreieck hinzufügen
                            B.DreieckHinzufügen(D);

                            DreieckeUnzugeteilt.RemoveAt(DreieckeUnzugeteilt.Count - 1);

                            Fehlzaehler = 0;
                        }
                        else
                        {
                            Fehlzaehler++;
                        }

                        // Falls die Behälter nicht ausreichen wird ein neuer hinzugefügt
                        if(Fehlzaehler >= _Behaelter.Count)
                        {
                            TBehaelter BNeu = new TBehaelter();
                            BNeu.BehaelterTyp = TBehaelter.TBehaelterTyp.Mittelbehaelter;
                            BNeu.TempWinkel = 0;

                            _Behaelter.Insert(1,BNeu);
                        }

                        // Nächste Iteration
                        Index++;
                        if (Index >= _Behaelter.Count)
                        {
                            Index = 0;
                        }
                    }
                }
                else
                {
                    // Start- und Endbehälter
                    TBehaelter BStart = new TBehaelter();
                    TBehaelter BEnde = new TBehaelter();
                    BStart.BehaelterTyp = TBehaelter.TBehaelterTyp.Startbehaelter;
                    BEnde.BehaelterTyp = TBehaelter.TBehaelterTyp.Endbehaelter;

                    _Behaelter.Add(BStart);
                    _Behaelter.Add(BEnde);

                    // Abwechselnd Dreiecke zu Start- und Endbehälter hinzufügen (Dreiecke mit größter Strecke zuerst)
                    bool InStartcontainer = true;
                    while (DreieckeUnzugeteilt.Count > 0)
                    {
                        TDreieck D = DreieckeUnzugeteilt[DreieckeUnzugeteilt.Count - 1];

                        if (InStartcontainer)
                        {
                            BStart.DreieckHinzufügen(D);
                        }
                        else
                        {
                            BEnde.DreieckHinzufügen(D);
                        }

                        DreieckeUnzugeteilt.RemoveAt(DreieckeUnzugeteilt.Count - 1);

                        // Flag umkehren
                        InStartcontainer = !InStartcontainer;
                    }
                }
            }

            // Dreiecke in allen Behältern korrekt orientieren
            foreach (TBehaelter B in _Behaelter)
            {
                B.DreieckeAnordnen();
            }

            TBehaelter[] BesteAnordnung;
            double BesteStrecke;
            if(_Behaelter.Count > 3) // Ab 3 sind Permuatation möglich (2 Behälter werden abgezogen)
            {
                // Permuatationen ohne Start- und Endbehälter
                List<TBehaelter> PermuatationsBehaelter = new List<TBehaelter>(_Behaelter);
                PermuatationsBehaelter.RemoveAt(0);
                PermuatationsBehaelter.RemoveAt(PermuatationsBehaelter.Count - 1);

                // Alle möglichen Kombinationen bestimmen
                var PermutationenRoh = BehaelterPermutationen(PermuatationsBehaelter, PermuatationsBehaelter.Count).ToArray();

                List<TBehaelter[]> Permutationen = new List<TBehaelter[]>();

                double[] Ergebnisse = new double[PermutationenRoh.Length];
                for (int I = 0; I < PermutationenRoh.Length; I++)
                {
                    List<TBehaelter> PermutationenUnvollstaendig = PermutationenRoh[I].ToList();
                    PermutationenUnvollstaendig.Insert(0, _Behaelter[0]);
                    PermutationenUnvollstaendig.Add(_Behaelter[_Behaelter.Count - 1]);

                    TBehaelter[] PermutationenVollstaendig = PermutationenUnvollstaendig.ToArray();

                    Permutationen.Add(PermutationenVollstaendig);

                    TBerechnung Berechnung = new TBerechnung(PermutationenVollstaendig);
                    Ergebnisse[I] = Berechnung.Berechnen();
                }

                int BestePermuatationIndex = -1;
                for (int I = 0; I < Ergebnisse.Length; I++)
                {
                    if (BestePermuatationIndex == -1 || Ergebnisse[I] < Ergebnisse[BestePermuatationIndex])
                    {
                        BestePermuatationIndex = I;
                    }
                }

                // Beste Permutation anzeigen
                TBehaelter[] BestePermutation = Permutationen[BestePermuatationIndex].ToArray();
                BestePermutation[0].Verschieben(new Vector(-(BestePermutation[0].TempPos.X - 200), 0));

                TBerechnung BerechnungBeste = new TBerechnung(BestePermutation);
                BerechnungBeste.Berechnen();

                BesteAnordnung = BestePermutation;
                BesteStrecke = Ergebnisse[BestePermuatationIndex];
            }
            else
            {
                TBerechnung Berechnung = new TBerechnung(_Behaelter.ToArray());
                BesteStrecke = Berechnung.Berechnen();
                BesteAnordnung = _Behaelter.ToArray();
            }

            // Ausgabe
            StatusBarItemInfo.Content = "Distanz: " + (int)Math.Round(BesteStrecke) + "m";

            // Behälter setzen & Zeichnen
            Behaelter = _Behaelter;
            Zeichne();
        }

        private bool BerechnenMittelbehaelterNoetig(List<TDreieck> _Dreiecke)
        {
            double B1Winkel = 0.0, B2Winkel = 0.0;
            List<TDreieck> DreieckeWinkeltest = _Dreiecke.ToList();
            while (DreieckeWinkeltest.Count > 0)
            {
                TDreieck D = DreieckeWinkeltest[DreieckeWinkeltest.Count - 1];

                if (B1Winkel < 180.0)
                {
                    B1Winkel += D.WA;

                    if (B1Winkel < 180.0)
                    {
                        DreieckeWinkeltest.RemoveAt(DreieckeWinkeltest.Count - 1);
                    }
                }
                else if (B2Winkel < 180.0)
                {
                    B2Winkel += D.WA;

                    if (B2Winkel < 180.0)
                    {
                        DreieckeWinkeltest.RemoveAt(DreieckeWinkeltest.Count - 1);
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IEnumerable<IEnumerable<TBehaelter>> BehaelterPermutationen(IEnumerable<TBehaelter> list, int length)
        {
            if (length == 1) return list.Select(t => new TBehaelter[] { t });
            return BehaelterPermutationen(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new TBehaelter[] { t2 }));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private List<TDreieck> OrdneNachKleinsterStrecke(List<TDreieck> Liste)
        {
            return Liste.OrderBy(D => D.LaengsteStrecke).ToList();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void OeffnenDialog()
        {
            OpenFileDialog Dialog = new OpenFileDialog()
            {
                Filter = "\"Dreiecksbeziehungen\"-Datei (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                FilterIndex = 0
            };

            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            FileStream Datei = new FileStream(Dialog.FileName, FileMode.Open);
            string DateiString = String.Empty;

            try
            {
                StreamReader Reader = new StreamReader(Datei);
                DateiString = Reader.ReadToEnd();
            }
            finally
            {
                Datei.Close();
            }

            DateiEinlesen(DateiString);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void SpeichernDialog()
        {
            SaveFileDialog Dialog = new SaveFileDialog()
            {
                Filter = "\"Dreiecksbeziehungen\"-Datei (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                FilterIndex = 0
            };

            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            Speichern(Dialog.FileName);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void Speichern(string FileName)
        {
            MessageBoxResult Result = MessageBox.Show("Sollen die Koordinaten auf Ganzzahlen gerundet werden?", "Koordinaten Rundung", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            bool Rundung;
            if(Result == MessageBoxResult.Cancel)
            {
                return;
            }
            else if(Result == MessageBoxResult.Yes)
            {
                Rundung = true;
            }
            else
            {
                Rundung = false;
            }

            FileStream Datei = new FileStream(FileName, FileMode.Create);

            try
            {
                StreamWriter Writer = new StreamWriter(Datei);

                Writer.WriteLine(Dreiecke.Length.ToString());

                foreach(TBehaelter B in Behaelter)
                {
                    foreach (TDreieck D in B.Dreiecke)
                    {
                        if (Rundung)
                        {
                            int AX, AY, BX, BY, CX, CY;

                            AX = (int)Math.Round(D.A.X);
                            AY = (int)Math.Round(D.A.Y);
                            BX = (int)Math.Round(D.B.X);
                            BY = (int)Math.Round(D.B.Y);
                            CX = (int)Math.Round(D.C.X);
                            CY = (int)Math.Round(D.C.Y);

                            Writer.WriteLine("3 " + AX + " " + AY + " " + BX + " " + BY + " " + CX + " " + CY);
                        }
                        else
                        {
                            Writer.WriteLine("3 " + D.A.X + " " + D.A.Y + " " + D.B.X + " " + D.B.Y + " " + D.C.X + " " + D.C.Y);
                        }
                    }
                }

                Writer.Flush();
                Datei.Flush();
            }
            finally
            {
                Datei.Close();
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // EVENT HANDLER
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Zeichne();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Zeichne();
        }

        private void MenuItemBeenden_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemOeffnen_Click(object sender, RoutedEventArgs e)
        {
            OeffnenDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Dreiecke[0].Drehen(45);
            Zeichne();
        }

        private void MenuItemSpiegeln_Click(object sender, RoutedEventArgs e)
        {
            Dreiecke[0].Spiegeln();
            Zeichne();
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SpeichernDialog();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
}
