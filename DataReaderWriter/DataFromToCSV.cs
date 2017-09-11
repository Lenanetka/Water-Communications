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
            List<char> separators = new List<char> { ',', ';' };
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
            if (maxCount == 0) throw new Exception(Languages.current.error106);
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
                    if (!csv.TryGetField(0, out id)) throw new Exception(Languages.current.error101 + csv.GetField(0));                                      
                    if (!csv.TryGetField(1, out sourceId)) throw new Exception(Languages.current.error102 + id);                                       
                    if (!csv.TryGetField(2, out Qn) || Qn <= 0) throw new Exception(Languages.current.error103 + id);                                       
                    if (!csv.TryGetField(3, out L) || L <= 0)
                        throw new Exception(Languages.current.error104 + id);                                      
                    if (!csv.TryGetField(4, out d) || d <= 0) throw new Exception(Languages.current.error105 + id);
                    
                    stations.Add(new Station(id, sourceId, Qn, L, d));
                }
            }
            return stations;
        }
        public List<Pipe> loadPipeMaterials(String path)
        {
            List<Pipe> pipes = new List<Pipe>();
            using (var reader = new StreamReader(@path, Encoding.UTF8))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.Delimiter = detectDelimiter(path);

                //csv.Configuration.HasHeaderRecord = false;
                while (csv.Read())
                {
                    //header = csv.FieldHeaders;
                    String name;
                    int id;
                    double m, A0, A1, C, VLimit, mLimit, A0Limit, A1Limit, CLimit;
                    if (!csv.TryGetField(0, out name) || !csv.TryGetField(1, out id) || !csv.TryGetField(2, out m) || !csv.TryGetField(3, out A0) || !csv.TryGetField(4, out A1) || !csv.TryGetField(5, out C)) throw new Exception(Languages.current.error107);
                    if (!csv.TryGetField(6, out VLimit) || VLimit <= 0)
                    {
                        pipes.Add(new Pipe(name, id, m, A0, A1, C));
                    }
                    else
                    {
                        if (!csv.TryGetField(7, out mLimit) || !csv.TryGetField(8, out A0Limit) || !csv.TryGetField(9, out A1Limit) || !csv.TryGetField(10, out CLimit)) throw new Exception(Languages.current.error107);
                        pipes.Add(new Pipe(name, id, m, A0, A1, C, VLimit, mLimit, A0Limit, A1Limit, CLimit));
                    }
                }
            }
            return pipes;
        }
        public void WriteInFile(String path, Communications communications, bool overwrite)
        {
            List<Station> stations = communications.stations;
            Languages output = Languages.current;

            using (var writer = new StreamWriter(@path, !overwrite, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField(output.outputTitleMain);
                csv.NextRecord();

                csv.WriteField(output.outputDestination);
                csv.WriteField(output.outputSource);
                csv.WriteField(output.outputLength + ", " + output.km);
                csv.WriteField(output.outputOptimalK);
                csv.NextRecord();

                for (int i = 1; i < stations.Count; i++)
                {
                    csv.WriteField(stations[i].id);
                    csv.WriteField(stations[i].sourceId);
                    csv.WriteField(Math.Round(stations[i].pipeLength, 2));
                    csv.WriteField(stations[i].k);
                    csv.NextRecord();
                }

                csv.WriteField(output.outputTitleSettings);
                csv.NextRecord();

                csv.WriteField(output.outputIdMain);
                csv.WriteField(stations[0].id);
                csv.NextRecord();
                csv.WriteField(output.outputHeadMain);
                csv.WriteField(communications.h);
                csv.WriteField(output.m);
                csv.NextRecord();
                csv.WriteField(output.outputHeadMin);
                csv.WriteField(communications.hMin);
                csv.WriteField(output.m);
                csv.NextRecord();
                csv.WriteField(output.outputAccidentPercent);
                csv.WriteField(communications.accidentPercent * 100);
                csv.WriteField(output.percent);
                csv.NextRecord();
                csv.WriteField(output.outputLengthRepairSection);
                csv.WriteField(communications.repairSectionMinimumLength);
                csv.WriteField(output.km);
                csv.NextRecord();
                csv.WriteField(output.outputPipeMaterial);
                csv.WriteField(communications.pipe.name);
                csv.NextRecord();
                csv.WriteField(output.outputAdditionalHeadLoss);
                csv.WriteField(communications.additionalHeadLoss * 100 - 100);
                csv.WriteField(output.percent);
                csv.NextRecord();
            }
        }
        public void WriteInFile(String path, Communications communications, int station, bool overwrite)
        {
            List<Station> stations = communications.stations;
            Languages output = Languages.current;

            using (var writer = new StreamWriter(@path, !overwrite))
            {
                var csv = new CsvWriter(writer);

                csv.WriteField(output.outputDestination);                
                csv.WriteField(output.outputSource);
                csv.WriteField(output.outputLength + ", " + output.km);
                csv.WriteField(output.outputOptimalK);
                csv.NextRecord();

                csv.WriteField(stations[station].id);
                csv.WriteField(stations[station].sourceId);
                csv.WriteField(Math.Round(stations[station].pipeLength, 2));
                csv.WriteField(stations[station].k);
                csv.NextRecord();

                csv.WriteField(output.outputDestination);
                csv.WriteField(output.outputSource);
                csv.WriteField(output.outputLength + ", " + output.km);
                csv.WriteField(output.outputDiameter + ", " + output.mm);
                csv.WriteField(output.outputFluidFlow + ", " + output.m3h);
                csv.WriteField(output.outputHead + ", " + output.m);
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
