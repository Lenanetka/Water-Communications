using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaterCommunications.Localization;

namespace WaterCommunications.DataReaderWriter
{
    class DataFromToCSV: IDataReaderWriter
    {
        public DataFromToCSV()
        {

        }
       
        public Communications ReadFromFile(String path)
        {

            Communications communications = null;

            List<Station> stations = new List<Station>();
            Station currentStation = new Station("0");
            stations.Add(currentStation);

            if (!File.Exists(@path))
                throw new FileNotFoundException("The file is not found in the specified location");
            using (var reader = new StreamReader(@path))
            {

                var csv = new CsvReader(reader);
                //csv.Configuration.Delimiter = ";";
                //csv.Configuration.HasHeaderRecord = false;

                while (csv.Read())
                {
                    //header = csv.FieldHeaders;

                    string id;
                    if (!csv.TryGetField(0, out id))
                    {
                        throw new Exception("Incorrect id, id must be integer. \nid = " + csv.GetField(0));
                    }
                    string sourceId;
                    if (!csv.TryGetField(1, out sourceId))
                    {
                        throw new Exception("Incorrect source id, source id must be integer.\nError for station with id = " + id);
                    }
                    double Qn;
                    if (!csv.TryGetField(2, out Qn) || Qn <= 0)
                    {
                        throw new Exception("Incorrect Qn, Qn must be positive double.\nError for station with id = " + id);
                    }
                    double L;
                    if (!csv.TryGetField(3, out L) || L <= 0)
                    {
                        throw new Exception("Incorrect L, L must be positive double.\nError for station with id = " + id);
                    }
                    double d;
                    if (!csv.TryGetField(4, out d) || d <= 0)
                    {
                        throw new Exception("Incorrect d, d must be positive double.\nError for station with id = " + id);
                    }

                    currentStation = new Station(id, sourceId, Qn, L, d);
                    stations.Add(currentStation);
                }
            }
            communications = new Communications(stations);

            return communications;
        }
        public void WriteInFile(String path, Communications communications, bool overwrite)
        {
            try
            {
                using (var writer = new StreamWriter(@path, !overwrite))
                {
                    var csv = new CsvWriter(writer);

                    csv.WriteField("Main information");
                    csv.NextRecord();

                    csv.WriteField("id");
                    csv.WriteField("sourseId");
                    csv.WriteField("optimal k");
                    csv.NextRecord();

                    for (int i = 1; i < communications.stations.Count; i++)
                    {
                        csv.WriteField(communications.stations[i].id);
                        csv.WriteField(communications.stations[i].sourceId);
                        csv.WriteField(communications.stations[i].k);
                        csv.NextRecord();
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error");
            }
        }
        public void WriteInFile(String path, Communications communications, int station, bool overwrite)
        {
            using (var writer = new StreamWriter(@path, !overwrite))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField("Station");                
                csv.WriteField("Source");
                csv.WriteField("Optimal k");
                csv.NextRecord();

                csv.WriteField(communications.stations[station].id);
                csv.WriteField(communications.stations[station].sourceId);
                csv.WriteField(communications.stations[station].k);
                csv.NextRecord();

                csv.WriteField("id");
                csv.WriteField("sourseId");
                csv.WriteField("L");
                csv.WriteField("d");
                csv.WriteField("Q");
                csv.WriteField("h");
                csv.NextRecord();

                for (int i = 1; i < communications.stations.Count; i++)
                {
                    csv.WriteField(communications.stations[i].id);
                    csv.WriteField(communications.stations[i].sourceId);
                    csv.WriteField(communications.stations[i].pipeLength);
                    csv.WriteField(communications.stations[i].pipeDiameter);
                    csv.WriteField(communications.stations[i].Qf);
                    csv.WriteField(communications.stations[i].h);
                    csv.NextRecord();
                }

                csv.NextRecord();
            }
        }
    }
}
