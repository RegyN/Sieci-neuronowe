using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Jitbit.Utils;

namespace Sieci_Neuronowe_projekt_1
{
    class Dane
    {
        //Dane wejściowe do trenowania sieci
        public double[][] punktyDanych;
        public double[][] klasyPunktow;

        //Wyniki pracy sieci
        public double[] wynikoweKlasy;

        public void Wczytaj(string sciezkaPliku)
        {
            try
            {
                using (TextFieldParser czytnikCsv = new TextFieldParser(sciezkaPliku))
                {
                    punktyDanych = new double[3000][];
                    klasyPunktow = new double[3000][];
                    wynikoweKlasy = new double[3000];
                    czytnikCsv.SetDelimiters(new string[] { "," });
                    czytnikCsv.HasFieldsEnclosedInQuotes = true;
                    czytnikCsv.ReadFields();

                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");  // Potrzebne żeby kropka była separatorem dziesiętnym
                    int i = 0;
                    while (!czytnikCsv.EndOfData)
                    {
                        string[] fieldData = czytnikCsv.ReadFields();

                        punktyDanych[i] = new double[]
                        {
                            Convert.ToDouble(fieldData[0], culture),
                            Convert.ToDouble(fieldData[1], culture)
                        };
                        klasyPunktow[i] = new double[]
                        {
                            Convert.ToDouble(fieldData[2], culture)/2 - 0.5
                        };
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        public void EksportujDoPliku(string sciezka)
        {
            var myExport = new CsvExport(",", false);

            for (int i=0; i<3000; i++)
            {
                myExport.AddRow();
                myExport["x"] = punktyDanych[i][0];
                myExport["y"] = punktyDanych[i][1];
                myExport["cls"] = Convert.ToInt32(klasyPunktow[i][0]*2+1.0);
                if (wynikoweKlasy[i] <= 0.33)
                    myExport["res"] = 1;
                else if (wynikoweKlasy[i] <= 0.66)
                    myExport["res"] = 2;
                else 
                    myExport["res"] = 3;
            }

            myExport.ExportToFile(sciezka);
            Console.WriteLine();
            Console.WriteLine("Zapisano wyniki do w pliku {0}", sciezka);
            Console.WriteLine();
        }
    }
}
