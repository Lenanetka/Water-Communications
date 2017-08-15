using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace WaterCommunications.DataReaderWriter
{
    class DataFromToCSV: IDataReaderWriter
    {
        public DataFromToCSV()
        {

        }
       private String detectDelimiter(String path)
        {           
            List<char> separators = new List<char> {',', ';'};
            List<int> separatorsCount = new List<int> { 0, 0 };

            int character;
            int row = 0;
            bool quoted = false;
            bool firstChar = true;


            using (var reader = new StreamReader(@path, Encoding.UTF8))
            {
                while (!reader.EndOfStream && row < 3)
                {
                    character = reader.Read();

                    switch (character)
                    {
                        case '"':
                            if (quoted)
                            {
                                if (reader.Peek() != '"')
                                    // Value is quoted and current character is " and next character is not ".
                                    quoted = false;
                                else
                                    reader.Read();
                                // Value is quoted and current and next characters are "" - read (skip) peeked 
                                // qoute.
                            }
                            else
                            {
                                if (firstChar)
                                    // Set value as quoted only if this quote is the first char in the value.
                                    quoted = true;
                            }
                            break;
                        case '\n':
                            if (!quoted)
                            {
                                firstChar = true;
                                row++;
                                continue;
                            }
                            break;
                        default:
                            if (!quoted)
                            {
                                if (separators.Contains((char)character))
                                {
                                    ++separatorsCount[separators.IndexOf((char)character)];
                                    firstChar = true;
                                    continue;
                                }
                            }
                            break;
                    }
                    if (firstChar)
                        firstChar = false;
                }
            }
            int maxCount = separatorsCount.Max();
            if (maxCount == 0) throw new Exception("Error 106\nCannot detect delimiter");         
            return separators[separatorsCount.IndexOf(maxCount)].ToString();
        }
        public List<Station> ReadFromFile(String path)
        {
            List<Station> stations = new List<Station>();
            using (var reader = new StreamReader(@path, Encoding.UTF8))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.Delimiter = detectDelimiter(path);

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
        public void WriteInFile(String path, Communications communications, bool overwrite)
        {
            List<Station> stations = communications.stations;
            using (var writer = new StreamWriter(@path, !overwrite, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField("Main information");
                csv.NextRecord();

                csv.WriteField("Destination");
                csv.WriteField("Source");
                csv.WriteField("Length");
                csv.WriteField("Optimal k");
                csv.NextRecord();

                for (int i = 1; i < stations.Count; i++)
                {
                    csv.WriteField(stations[i].id);
                    csv.WriteField(stations[i].sourceId);
                    csv.WriteField(Math.Round(stations[i].pipeLength, 2));
                    csv.WriteField(stations[i].k);
                    csv.NextRecord();
                }

                csv.WriteField("Start datas");
                csv.NextRecord();

                csv.WriteField("Main station id");
                csv.WriteField(stations[0].id);
                csv.NextRecord();
                csv.WriteField("H");
                csv.WriteField(communications.h);
                csv.NextRecord();
                csv.WriteField("Hmin");
                csv.WriteField(communications.hMin);
                csv.NextRecord();
                csv.WriteField("Accident percent of Q");
                csv.WriteField(communications.accidentPercent * 100);
                csv.NextRecord();
                csv.WriteField("Minimum length of repair section");
                csv.WriteField(communications.repairSectionMinimumLength);
                csv.NextRecord();

            }
        }
        public void WriteInFile(String path, Communications communications, int station, bool overwrite)
        {
            List<Station> stations = communications.stations;

            using (var writer = new StreamWriter(@path, !overwrite))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField("Destination");                
                csv.WriteField("Source");
                csv.WriteField("Length");
                csv.WriteField("Optimal k");
                csv.NextRecord();

                csv.WriteField(stations[station].id);
                csv.WriteField(stations[station].sourceId);
                csv.WriteField(Math.Round(stations[station].pipeLength, 2));
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
                    csv.WriteField(Math.Round(stations[i].pipeLength, 2));
                    csv.WriteField(Math.Round(stations[i].pipeDiameter, 1));
                    csv.WriteField(Math.Round(stations[i].Qf, 1));
                    csv.WriteField(Math.Round(stations[i].h, 2));
                    if(stations[i].h < communications.hMin) csv.WriteField("*");
                    csv.NextRecord();
                }

                csv.NextRecord();
            }
        }
    }
}
