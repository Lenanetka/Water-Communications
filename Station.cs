using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCommunications
{
    class Station
    {
        public int id;

        //id of source
        public int sourceId;
        //list numbers of source and subs
        public int source;
        public List<int> subs;

        //lenght of pipes subs this station (kilometers)
        public double pipeLength;
        //diameter of pipes subs this station (milimeters)
        public double pipeDiameter;
        //volume of water in the pipes here, standart (cubic meters)
        public double Qn;
        //volume of water in the pipes here, in fact (cubic meters per hour)
        public double Qf;
        //hydraulic head (meters)
        public double h;
        //number of emergency areas
        public int k;
        public bool accident;

        //constructor for main station
        public Station(int id, double h)
        {
            this.id = id;           

            sourceId = -1;
            source = -1;
            subs = new List<int>();

            pipeLength = -1;
            pipeDiameter = -1;
            Qn = 0;
            Qf = 0;
            this.h = h;
            k = 1;
            accident = false;
        }
        //constructor for station with known source
        public Station(int id, int sourceId, double pipeLength, double pipeDiameter, double Qn)
        {
            this.id = id;

            this.sourceId = sourceId;
            source = -1;
            subs = new List<int>();

            this.pipeLength = pipeLength;
            this.pipeDiameter = pipeDiameter;
            this.Qn = Qn;
            Qf = Qn;
            h = 0;
            k = 1;
            accident = false;
        }
        public void resetQf()
        {
            Qf = Qn;
        }
        public void addSubStation(int sub)
        {
            if (!subs.Contains(sub)) subs.Add(sub);
        }
    }
}
