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
    class DaneKlasyfikacja
    {
        //Dane wejściowe do trenowania sieci
        public List<double[]> punkty = new List<double[]>();
        public List<double[]> klasyWej = new List<double[]>();

        //Wyniki pracy sieci
        public List<double> klasyWy = new List<double>();

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
                        punkty.Add(new double[2]
                        {
                            Convert.ToDouble(fieldData[0], culture),
                            Convert.ToDouble(fieldData[1], culture)
                        });
                        if(liczbaKolumnWPliku>=3)       // To znaczy że plik ma informacje o oczekiwanej klasie
                        { 
                            klasyWej.Add(new double[1]
                            {
                                Convert.ToDouble(fieldData[2], culture)/2 - 0.5
                            });
                        }
                        else
                        {
                            klasyWej.Add(new double[1]
                            {
                                -1.0
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

            for (int i=0; i<punkty.Count; i++)
            {
                myExport.AddRow();
                myExport["x"] = punkty[i][0];
                myExport["y"] = punkty[i][1];
                if(klasyWej[i][0]>=0.0)
                    myExport["cls"] = Convert.ToInt32(klasyWej[i][0]*2+1.0);
                if (klasyWy[i] <= 0.33)
                    myExport["res"] = 1;
                else if (klasyWy[i] <= 0.66)
                    myExport["res"] = 2;
                else 
                    myExport["res"] = 3;
            }

            myExport.ExportToFile(sciezka);
            Console.WriteLine();
            Console.WriteLine("Zapisano wyniki do w pliku {0}", sciezka);
            Console.WriteLine();
        }

        //Wypisuje co 50 wynik w konsoli
        public void WypiszWKonsoli()
        {
            for (int i = 0; i < punkty.Count; i++)
            {
                if (i % 50 == 0)
                {
                    Console.WriteLine("Wejscia: X={0:0.00}, Y={1:0.00}, wynik: {2:0.00}, oczekiwany: {3:0.0}", punkty[i][0], punkty[i][1], klasyWy[i] * 2 + 1.0, klasyWej[i][0] * 2 + 1.0);
                }
            }
        }
    }
}
