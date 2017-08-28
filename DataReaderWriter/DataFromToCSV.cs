using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using WaterCommunications.Localization;

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
            if (maxCount == 0) throw new Exception(CurrentLocalization.localizationErrors.error106);         
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
                    if (!csv.TryGetField(0, out id)) throw new Exception(CurrentLocalization.localizationErrors.error101 + csv.GetField(0));                                      
                    if (!csv.TryGetField(1, out sourceId)) throw new Exception(CurrentLocalization.localizationErrors.error102 + id);                                       
                    if (!csv.TryGetField(2, out Qn) || Qn <= 0) throw new Exception(CurrentLocalization.localizationErrors.error103 + id);                                       
                    if (!csv.TryGetField(3, out L) || L <= 0)
                        throw new Exception(CurrentLocalization.localizationErrors.error104 + id);                                      
                    if (!csv.TryGetField(4, out d) || d <= 0) throw new Exception(CurrentLocalization.localizationErrors.error105 + id);
                    
                    stations.Add(new Station(id, sourceId, Qn, L, d));
                }
            }
            return stations;
        }
        public void WriteInFile(String path, Communications communications, bool overwrite)
        {
            List<Station> stations = communications.stations;
            LocalizationOutput output = CurrentLocalization.localizationOutput;
            LocalizationSystemOfUnits units = CurrentLocalization.localizationSystemOfUnits;

            using (var writer = new StreamWriter(@path, !overwrite, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField(output.mainInfo.title);
                csv.NextRecord();

                csv.WriteField(output.mainInfo.destination);
                csv.WriteField(output.mainInfo.source);
                csv.WriteField(output.mainInfo.length + ", " + units.km);
                csv.WriteField(output.mainInfo.optimalK);
                csv.NextRecord();

                for (int i = 1; i < stations.Count; i++)
                {
                    csv.WriteField(stations[i].id);
                    csv.WriteField(stations[i].sourceId);
                    csv.WriteField(Math.Round(stations[i].pipeLength, 2));
                    csv.WriteField(stations[i].k);
                    csv.NextRecord();
                }

                csv.WriteField(output.startDatas.title);
                csv.NextRecord();

                csv.WriteField(output.startDatas.idMain);
                csv.WriteField(stations[0].id);
                csv.NextRecord();
                csv.WriteField(output.startDatas.headMain);
                csv.WriteField(communications.h);
                csv.WriteField(units.m);
                csv.NextRecord();
                csv.WriteField(output.startDatas.headMin);
                csv.WriteField(communications.hMin);
                csv.WriteField(units.m);
                csv.NextRecord();
                csv.WriteField(output.startDatas.accidentPercent);
                csv.WriteField(communications.accidentPercent * 100);
                csv.WriteField(units.percent);
                csv.NextRecord();
                csv.WriteField(output.startDatas.lengthRepairSection);
                csv.WriteField(communications.repairSectionMinimumLength);
                csv.WriteField(units.km);
                csv.NextRecord();

            }
        }
        public void WriteInFile(String path, Communications communications, int station, bool overwrite)
        {
            List<Station> stations = communications.stations;
            LocalizationOutput output = CurrentLocalization.localizationOutput;
            LocalizationSystemOfUnits units = CurrentLocalization.localizationSystemOfUnits;

            using (var writer = new StreamWriter(@path, !overwrite))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField(output.allInfo.destination);                
                csv.WriteField(output.allInfo.source);
                csv.WriteField(output.allInfo.length + ", " + units.km);
                csv.WriteField(output.allInfo.optimalK);
                csv.NextRecord();

                csv.WriteField(stations[station].id);
                csv.WriteField(stations[station].sourceId);
                csv.WriteField(Math.Round(stations[station].pipeLength, 2));
                csv.WriteField(stations[station].k);
                csv.NextRecord();

                csv.WriteField(output.allInfo.destination);
                csv.WriteField(output.allInfo.source);
                csv.WriteField(output.allInfo.length + ", " + units.km);
                csv.WriteField(output.allInfo.diameter + ", " + units.mm);
                csv.WriteField(output.allInfo.fluidFlow + ", " + units.m3h);
                csv.WriteField(output.allInfo.head + ", " + units.m);
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
