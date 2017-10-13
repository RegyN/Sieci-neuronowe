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
        public double[][] punktyDanych;
        public double[][] klasyPunktow;

        public void Wczytaj(string sciezkaPliku)
        {
            try
            {
                using (TextFieldParser czytnikCsv = new TextFieldParser(sciezkaPliku))
                {
                    punktyDanych = new double[3000][];
                    klasyPunktow = new double[3000][];
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

            foreach (var item in punktyDanych)
            {

            }
            myExport.AddRow();
            myExport["Region"] = "Los Angeles, USA";
            myExport["Sales"] = 100000;
            myExport["Date Opened"] = new DateTime(2003, 12, 31);

            myExport.AddRow();
            myExport["Region"] = "Canberra \"in\" Australia";
            myExport["Sales"] = 50000;
            myExport["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);

            myExport.ExportToFile(@"F:\Downloads\Somefile.csv");
        }
    }
}
