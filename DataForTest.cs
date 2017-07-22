using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WaterCommunications
{
    class DataForTest: IDataReaderWriter
    {
        public DataForTest()
        {

        }
        public Communications ReadFromFile(String path)
        {
            return ReadFromFile();
        }
        public Communications ReadFromFile()
        {
            List<Station> stations = new List<Station>();
            Station currentStation = new Station(0, 40);
            stations.Add(currentStation);
            currentStation = new Station(2, 0, 2, 200, 200);
            stations.Add(currentStation);
            currentStation = new Station(3, 2, 2, 200, 100);
            stations.Add(currentStation);

            Communications communications = new Communications(stations);
            communications.h = 40;
            communications.hMin = 30;
            communications.accidentPercent = 0.7;
            return communications;
        }
        public void WriteInFile(String path, Communications communications)
        {
            String result = "\n";
            for (int i = 1; i < communications.stations.Count; i++)
            {
                result += "Station " + (communications.stations[i].id + " from " + communications.stations[i].sourceId +
                    "(L=" + communications.stations[i].pipeLength + " km d=" + communications.stations[i].pipeDiameter + " mm):" +
                    " optimal k=" + communications.stations[i].k + " .\n");
            }
            MessageBox.Show(result, "Common result");
        }
        public void WriteInFile(String path, Communications communications, int station)
        {
            String result = "\nStation " + communications.stations[station].id + ": optimal k=" + communications.stations[station].k + "\n";
            for (int i = 1; i < communications.stations.Count; i++)
            {
                result += (communications.stations[i].id + " from " + communications.stations[i].sourceId +
                    "(L=" + communications.stations[i].pipeLength + " km d=" + communications.stations[i].pipeDiameter + " mm):" +
                    " h=" + communications.stations[i].h + " m" +
                    " Q=" + communications.stations[i].Qf + " m^3/h\n");
            }
            MessageBox.Show(result, "Result for station " + communications.stations[station].id);
        }       
    }
}
