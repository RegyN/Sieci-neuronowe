using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog;
using Encog.Engine.Network.Activation;
using Jitbit.Utils;

namespace Sieci_Neuronowe_projekt_1
{
    class Program
    {
        static string folder = @"F:\Downloads\";
        static string sciezkaKlasyfikacjaTreningowe =       folder + @"data.train.csv";
        static string sciezkaKlasyfikacjaBledyTreningu =    folder + @"bledy.data.train.csv";
        static string sciezkaKlasyfikacjaTreningoweWyniki = folder + @"wyniki.data.train.csv";
        static string sciezkaKlasyfikacjaTestowe =          folder + @"data.test.csv";
        static string sciezkaKlasyfikacjaTestoweWyniki =    folder + @"wyniki.data.test.csv";
        static string sciezkaRegresjaTreningowe =           folder + @"data.xsq.train.csv";
        static string sciezkaRegresjaBledyTreningu =        folder + @"bledy.data.xsq.train.csv";
        static string sciezkaRegresjaTreningoweWyniki =     folder + @"wyniki.data.xsq.train.csv";
        static string sciezkaRegresjaTestowe =              folder + @"data.xsq.test.csv";
        static string sciezkaRegresjaTestoweWyniki =        folder + @"wyniki.data.xsq.test.csv";

        static void Main(string[] args)
        {
            //Console.WriteLine("Wcisnij ENTER by rozpoczac klasyfikacje:");
            //Console.ReadLine();
            //ZrobKlasyfikacje();
            //Console.WriteLine();
            Console.WriteLine("Wcisnij ENTER by rozpoczac regresje:");
            Console.ReadLine();
            ZrobRegresje();
        }

        private static void ZrobRegresje()
        {
            DaneRegresja doNauki = new DaneRegresja();
            doNauki.Wczytaj(sciezkaRegresjaTreningowe);

            BasicNetwork siec = UtworzSiecDoRegresji();
            IMLDataSet dataTrening = UczSiec(siec, doNauki);

            // testuje siec na danych treningowych
            for (int i = 0; i < dataTrening.Count; i++)
            {
                IMLData wynik = siec.Compute(dataTrening[i].Input);
                doNauki.otrzymaneY.Add(wynik[0]);
            }

            doNauki.EksportujDoPliku(sciezkaRegresjaTreningoweWyniki);

            // testuje siec na danych testowych
            DaneRegresja doTestow = new DaneRegresja();
            doTestow.Wczytaj(sciezkaRegresjaTestowe);
            IMLDataSet dataTest = new BasicMLDataSet(doTestow.wejscioweX.ToArray(), doTestow.oczekiwaneY.ToArray());
            for (int i = 0; i < dataTest.Count; i++)
            {
                IMLData wynik = siec.Compute(dataTest[i].Input);
                doTestow.otrzymaneY.Add(wynik[0]);
            }
            doTestow.EksportujDoPliku(sciezkaRegresjaTestoweWyniki);
        }

        private static void ZrobKlasyfikacje()
        {
            DaneKlasyfikacja doNauki = new DaneKlasyfikacja();
            doNauki.Wczytaj(sciezkaKlasyfikacjaTreningowe);

            BasicNetwork siec = UtworzSiecDoKlasyfikacji();
            IMLDataSet dataTrening = UczSiec(siec, doNauki);

            // testuje siec na danych treningowych
            for (int i = 0; i < dataTrening.Count; i++)
            {
                IMLData wynik = siec.Compute(dataTrening[i].Input);
                doNauki.klasyWy.Add(wynik[0]);
            }

            doNauki.EksportujDoPliku(sciezkaKlasyfikacjaTreningoweWyniki);

            // testuje siec na danych testowych
            DaneKlasyfikacja doTestow = new DaneKlasyfikacja();
            doTestow.Wczytaj(sciezkaKlasyfikacjaTestowe);
            IMLDataSet dataTest = new BasicMLDataSet(doTestow.punkty.ToArray(), doTestow.klasyWej.ToArray());
            for (int i = 0; i < dataTest.Count; i++)
            {
                IMLData wynik = siec.Compute(dataTest[i].Input);
                doTestow.klasyWy.Add(wynik[0]);
            }
            doTestow.EksportujDoPliku(sciezkaKlasyfikacjaTestoweWyniki);
        }

        static IMLDataSet UczSiec(BasicNetwork siec, DaneKlasyfikacja doNauki, double wspolczynnikNauki = 0.003, double bezwladnosc = 0.01) //Wspolczynniki domyslne wybrane tak zeby chociaz dzialalo jakkolwiek
        {
            IMLDataSet dataSet = new BasicMLDataSet(doNauki.punkty.ToArray(), doNauki.klasyWej.ToArray());
            List<double> bledyTreningu = new List<double>();
            IMLTrain train = new Backpropagation(siec, dataSet, wspolczynnikNauki, bezwladnosc);

            int iter = 1;
            int maxIter = 5000;
            do
            {
                train.Iteration();
                Console.WriteLine("Iteracja #{0} Blad {1:0.0000}", iter, train.Error);
                bledyTreningu.Add(train.Error);
                iter++;
            } while (train.Error >= 0.03 && iter < maxIter);

            EksportujBledyTreningu(sciezkaKlasyfikacjaBledyTreningu, bledyTreningu);
            train.FinishTraining();
            return dataSet;
        }

        static IMLDataSet UczSiec(BasicNetwork siec, DaneRegresja doNauki, double wspolczynnikNauki = 0.003, double bezwladnosc = 0.01) 
        {
            IMLDataSet dataSet = new BasicMLDataSet(doNauki.wejscioweX.ToArray(), doNauki.oczekiwaneY.ToArray());
            List<double> bledyTreningu = new List<double>();
            IMLTrain train = new Backpropagation(siec, dataSet, wspolczynnikNauki, bezwladnosc);

            int iter = 1;
            int maxIter = 5000;
            do
            {
                train.Iteration();
                Console.WriteLine("Iteracja #{0} Blad {1:0.0000}", iter, train.Error);
                bledyTreningu.Add(train.Error);
                iter++;
            } while (train.Error >= 0.001 && iter < maxIter);

            EksportujBledyTreningu(sciezkaRegresjaBledyTreningu, bledyTreningu);
            train.FinishTraining();
            return dataSet;
        }

        static void EksportujBledyTreningu(string sciezka, List<double> bledy)
        {
            var myExport = new CsvExport(",", false);

            for (int i = 0; i < bledy.Count; i++)
            {
                myExport.AddRow();
                myExport["iter"] = i;
                myExport["err"] = bledy[i];
            }

            myExport.ExportToFile(sciezka);
            Console.WriteLine();
            Console.WriteLine("Zapisano wyniki do w pliku {0}", sciezka);
            Console.WriteLine();
        }

        static BasicNetwork UtworzSiecDoKlasyfikacji()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));                    //Neurony wejsciowe
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5)); //Warstwa ukryta
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1)); //Wyjścia
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }

        static BasicNetwork UtworzSiecDoRegresji()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 1));                    //Neurony wejsciowe
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 20)); //Warstwa ukryta
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1)); //Wyjścia
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }
}
