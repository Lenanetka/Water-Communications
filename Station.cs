using System;
using System.Collections.Generic;

namespace WaterCommunications
{
    class Station
    {
        public string id;

        //id of source
        public string sourceId;
        //list numbers of source and subs
        public int source;
        public List<int> subs;

        public double Qn;
        //volume of water in the pipes here, in fact (cubic meters per hour)
        public double Qf;
        //hydraulic head (meters)
        //lenght of pipes subs this station (kilometers)
        public double pipeLength;
        //diameter of pipes subs this station (milimeters)
        public double pipeDiameter;
        //volume of water in the pipes here, standart (cubic meters)        
        public double h;
        //number of emergency areas
        public int k;
        public bool accident;

        //constructor for main station
        public Station(string id)
        {
            this.id = id;           

            subs = new List<int>();
            h = -1;

            accident = false;
        }
        //constructor for station with known source
        public Station(string id, string sourceId, double Qn, double pipeLength, double pipeDiameter)
        {
            this.id = id;

            this.sourceId = sourceId;
            source = -1;
            subs = new List<int>();

            this.Qn = Qn;
            Qf = Qn;
            this.pipeLength = pipeLength;
            this.pipeDiameter = pipeDiameter;          
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
