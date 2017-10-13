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
using Encog.Neural.Networks.Training.Propagation.Resilient;
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
        static void Main(string[] args)
        {
            string sciezkaPliku = @"F:\Downloads\data.train.csv";
            Dane doAnalizy = new Dane();
            doAnalizy.Wczytaj(sciezkaPliku);
            BasicNetwork siec = UtworzSiec();
            IMLDataSet dataSet = UczSiec(siec, doAnalizy);
            
            for(int i = 0; i < dataSet.Count; i++)
            {
                IMLData wynik = siec.Compute(dataSet[i].Input);
                doAnalizy.wynikoweKlasy[i] = wynik[0];
            }
            
            doAnalizy.EksportujDoPliku(@"F:\Downloads\wyniki.csv");
        }

        //Wypisuje co 50 wynik w konsoli
        static void WypiszWKonsoli(Dane dane)
        {
            for (int i = 0; i < dane.punktyDanych.GetLength(0); i++)
            {
                if (i % 50 == 0)
                {
                    Console.WriteLine("Wejscia: X={0:0.00}, Y={1:0.00}, wynik: {2:0.00}, oczekiwany: {3:0.0}", dane.punktyDanych[i][0], dane.punktyDanych[i][1], dane.wynikoweKlasy[i]*2+1.0, dane.klasyPunktow[i][0]*2+1.0);
                }
            }
        }

        static IMLDataSet UczSiec(BasicNetwork siec, Dane doNauki)
        {
            IMLDataSet dataSet = new BasicMLDataSet(doNauki.punktyDanych, doNauki.klasyPunktow);
            IMLTrain train = new Backpropagation(siec, dataSet);
            int iter = 1;

            do
            {
                train.Iteration();
                Console.WriteLine("Iteracja #{0} Blad {1:0.0000}", iter, train.Error);
                iter++;
            } while (train.Error >= 0.02 && iter < 5000);

            train.FinishTraining();
            return dataSet;
        }

        static BasicNetwork UtworzSiec()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 5));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }
}
