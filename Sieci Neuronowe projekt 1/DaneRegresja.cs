using Jitbit.Utils;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieci_Neuronowe_projekt_1
{
    class DaneRegresja
    {
        //Dane wejściowe
        public List<double[]> wejscioweX = new List<double[]>();

        // Wyniki oczekiwane
        public List<double[]> oczekiwaneY = new List<double[]>();

        //Wyniki otrzymane
        public List<double> otrzymaneY = new List<double>();

        public void Wczytaj(string sciezkaPliku)
        {
            try
            {
                using (TextFieldParser czytnikCsv = new TextFieldParser(sciezkaPliku))
                {
                    czytnikCsv.SetDelimiters(new string[] { "," });
                    czytnikCsv.HasFieldsEnclosedInQuotes = true;
                    int liczbaKolumnWPliku = czytnikCsv.ReadFields().GetLength(0);

                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");  // Potrzebne żeby kropka była separatorem dziesiętnym
                    int i = 0;
                    while (!czytnikCsv.EndOfData)
                    {
                        string[] fieldData = czytnikCsv.ReadFields();
                        wejscioweX.Add(new double[1]
                        {
                            Convert.ToDouble(fieldData[0], culture)
                        });
                        if (liczbaKolumnWPliku >= 2)       // To znaczy że plik ma informacje o oczekiwanym wyniku
                        {
                            oczekiwaneY.Add(new double[1]
                            {
                                Convert.ToDouble(fieldData[1], culture)/30 // Dziele na 30 zeby bylo od 0 do 1
                            });
                        }
                        else
                        {
                            oczekiwaneY.Add(new double[1]
                            {
                                -100.0
                            });
                        }
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

            for (int i = 0; i < wejscioweX.Count; i++)
            {
                myExport.AddRow();
                myExport["X"] = wejscioweX[i][0];
                if (this.oczekiwaneY[i][0] >= -100.0)
                    myExport["oczY"] = oczekiwaneY[i][0]*30;
                myExport["wynY"] = otrzymaneY[i]*30;
            }

            myExport.ExportToFile(sciezka);
            Console.WriteLine();
            Console.WriteLine("Zapisano wyniki do w pliku {0}", sciezka);
            Console.WriteLine();
        }
    }
}
