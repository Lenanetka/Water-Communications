using System;
using System.Collections.Generic;

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
        public Station(int id)
        {
            this.id = id;
            sourceId = -2;
            source = -2;
            subs = new List<int>();
            Qn = -1;
            Qf = -1;
            pipeLength = -2;
            pipeDiameter = -2;
            h = -1;
            k = -2;
            accident = false;
        }
        //constructor for station with known source
        public Station(int id, int sourceId, double Qn, double pipeLength, double pipeDiameter)
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
