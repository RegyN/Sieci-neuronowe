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

            Console.WriteLine("Liczba linii:" + doAnalizy.punktyDanych.GetLength(0));
            Console.ReadLine();

            IMLDataSet dataSet = new BasicMLDataSet(doAnalizy.punktyDanych, doAnalizy.klasyPunktow);
            IMLTrain train = new Backpropagation(siec, dataSet);
            int iter = 1;

            do
            {
                train.Iteration();
                Console.WriteLine("Iteracja #{0} Blad {1:0.0000}", iter, train.Error);
                iter++;
            } while (train.Error>=0.02 && iter<100000);

            train.FinishTraining();

            // test the neural network
            Console.ReadLine();
            Console.WriteLine("Wyniki:");

            //Wyświetl co 50 wynik
            int i = 0;
            foreach (IMLDataPair pair in dataSet)
            {
                i++;
                if (i >= 50)
                {
                    IMLData wynik = siec.Compute(pair.Input);
                    Console.WriteLine("Wejscia: X={0:0.00} , Y={1:0.00} , wynik: {2:0.00}, oczekiwany: {3:0.0}", pair.Input[0] , pair.Input[1] , wynik[0] , pair.Ideal[0]);
                    i = 0;
                }
            }
            Console.ReadLine();

            doAnalizy.EksportujDoPliku(@"F:\Downloads\wyniki.csv");
        }

        

        static BasicNetwork UtworzSiec()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 25));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            return network;
        }
    }
}
