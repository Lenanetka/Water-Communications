using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WaterCommunications.DataReaderWriter
{
    class DataFromToCSV: IDataReaderWriter
    {
        public DataFromToCSV()
        {

        }
       
        public List<Station> ReadFromFile(String path)
        {
            List<Station> stations = new List<Station>();
            using (var reader = new StreamReader(@path, Encoding.UTF8))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ";";
                //csv.Configuration.HasHeaderRecord = false;
                while (csv.Read())
                {
                    //header = csv.FieldHeaders;
                    int id, sourceId;
                    double Qn, L, d;

                    if (!csv.TryGetField(0, out id)) throw new Exception("Error 101\nIncorrect id, id must be integer. \nid = " + csv.GetField(0));                                      
                    if (!csv.TryGetField(1, out sourceId)) throw new Exception("Error 102\nIncorrect source id, source id must be integer.\nError for station with id = " + id);                                       
                    if (!csv.TryGetField(2, out Qn) || Qn <= 0) throw new Exception("Error 103\nIncorrect Qn, Qn must be positive double.\nError for station with id = " + id);                                       
                    if (!csv.TryGetField(3, out L) || L <= 0) throw new Exception("Error 104\nIncorrect L, L must be positive double.\nError for station with id = " + id);                                      
                    if (!csv.TryGetField(4, out d) || d <= 0) throw new Exception("Error 105\nIncorrect d, d must be positive double.\nError for station with id = " + id);
                    
                    stations.Add(new Station(id, sourceId, Qn, L, d));
                }
            }
            return stations;
        }
        public void WriteInFile(String path, List<Station> stations, bool overwrite)
        {
            using (var writer = new StreamWriter(@path, !overwrite, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField("Station");
                csv.WriteField("Source");
                csv.WriteField("Optimal k");
                csv.NextRecord();

                for (int i = 1; i < stations.Count; i++)
                {
                    csv.WriteField(stations[i].id);
                    csv.WriteField(stations[i].sourceId);
                    csv.WriteField(stations[i].k);
                    csv.NextRecord();
                }
            }
        }
        public void WriteInFile(String path, List<Station> stations, int station, bool overwrite)
        {
            using (var writer = new StreamWriter(@path, !overwrite))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField("Station");                
                csv.WriteField("Source");
                csv.WriteField("Optimal k");
                csv.NextRecord();

                csv.WriteField(stations[station].id);
                csv.WriteField(stations[station].sourceId);
                csv.WriteField(stations[station].k);
                csv.NextRecord();

                csv.WriteField("id");
                csv.WriteField("sourseId");
                csv.WriteField("L");
                csv.WriteField("d");
                csv.WriteField("Q");
                csv.WriteField("h");
                csv.NextRecord();

                for (int i = 1; i < stations.Count; i++)
                {                  
                    csv.WriteField(stations[i].id);
                    csv.WriteField(stations[i].sourceId);
                    csv.WriteField(String.Format("{0:0.00}", stations[i].pipeLength));
                    csv.WriteField(String.Format("{0:0.0}", stations[i].pipeDiameter));
                    csv.WriteField(String.Format("{0:0.0}", stations[i].Qf));
                    csv.WriteField(String.Format("{0:0.00}", stations[i].h));
                    csv.NextRecord();
                }

                csv.NextRecord();
            }
        }
    }
}
