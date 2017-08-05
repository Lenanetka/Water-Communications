using System;
using System.Collections.Generic;

namespace WaterCommunications
{
    class Communications
    {
        private static double m = 0.223;
        private static double A0 = 0;
        private static double A1 = 0.01344;
        private static double c = 1;
        private static double g = 9.81;
        private static double K = 1.1;

        public List<Station> stations;
        public double h
        {
            get
            {
                return stations[0].h;
            }
            set
            {
                stations[0].h = value;
            }
        }
        public double hMin;
        public double accidentPercent;

        public Communications(List<Station> stations)
        {
            this.stations = stations;
            connectStations();
            hasAllSources();
            hasCycle();
            hasDoublePipes();
        }    

        private void connectStations()
        {
            for(int i = 0; i < stations.Count; i++)
                for (int j = 0; j < stations.Count; j++)
                {
                    if(stations[j].sourceId == stations[i].id)
                    {
                        stations[j].source = i;
                        stations[i].addSubStation(j);
                    }
                }
        }

        private void hasAllSources()
        {
            foreach (Station s in stations) if (s.source == -1) throw new Exception("There are no source for station " + s.id);
        }

        private void hasCycle()
        {
            List<int> traversal = new List<int>();
            traversal.Add(0);

            int k = 0;
            while(k < traversal.Count)
            {
                for (int i = 0; i < k; i++)
                    if (traversal[i] == traversal[k]) throw new Exception("Your communication has cycle, started at station " + stations[traversal[k]].id);
                foreach (int s in stations[traversal[k]].subs) traversal.Add(s);
                k++;
            }
        }

        private void hasDoublePipes()
        {
            foreach(Station st in stations)
            {
                List<string> subsId = new List<string>();
                foreach(int s in st.subs)
                {
                    if(subsId.Contains(stations[s].id)) throw new Exception("You have more than 1 connection between station " + st.id + " and " + stations[s].id);
                    subsId.Add(stations[s].id);
                }
            }
        }     

        public void resetQf()
        {
            for (int i = 1; i < stations.Count; i++)
                stations[i].resetQf();
        }

        private void changeAccidentSubs(int station)
        {
            stations[station].Qf = stations[station].Qn * accidentPercent;
            foreach (int s in stations[station].subs) changeAccidentSubs(s);
        }

        private void calculateQf(int station)
        {          
            if(stations[station].accident == false)
            {
                stations[station].Qf = stations[station].Qn;
                foreach (int s in stations[station].subs)
                {
                    calculateQf(s);
                    stations[station].Qf -= (stations[s].Qn - stations[s].Qf);
                }
            }                           
        }

        private double calcilateHLost(double Q, double L, double d)
        {
            double v = (4 * Q * 1000) / (3.6 * Math.PI * d * d);
            double hLost = (A1 / (2 * g)) * (Math.Pow(A0 + c / v, m) / Math.Pow(d / 1000, m + 1)) * v * v * L * K * 1000;
            return hLost;
        }

        private void calculateH(int station)
        {
            if (station != 0)
            {
                if (stations[station].accident == false)
                    stations[station].h = stations[stations[station].source].h - calcilateHLost(stations[station].Qf/2, stations[station].pipeLength, stations[station].pipeDiameter);  
                else stations[station].h = stations[stations[station].source].h 
                        - calcilateHLost(stations[station].Qf/2, stations[station].pipeLength * (stations[station].k-1) / stations[station].k, stations[station].pipeDiameter)
                        - calcilateHLost(stations[station].Qf, stations[station].pipeLength / stations[station].k, stations[station].pipeDiameter);
            }
            foreach (int s in stations[station].subs) calculateH(s);
        }

        public void calculateOptimalK(int station)
        {
            stations[station].accident = true;
            stations[station].k = 0;

            do
            {
                //Сбросить значения объемов воды, которые мы высчитывали ранее. 
                resetQf();
                //Для каждого трубопровода ищем оптимальное количество участков для поддержание нужного напора. 
                //Увеличиваем число участков пока вся сеть не пройдет проверку.
                stations[station].k++;
                //Сначала определяем место аварии и для него и всех зависящих от него станций уменьшаем количество поступаемой воды до предельного минимума. 
                //Рекурсивный обход вглубь аварийной ветки дерева, уменьшаем объем воды, тут всё равно как обходить, лишь бы посетить все нужные станции в любом порядке. 
                changeAccidentSubs(station);
                //Затем пересчитываем насколько уменьшается количество подаваемой воды на остальных станциях. 
                //Рекурсивный обход вглубь, просчитанную ранее аварийную ветку не трогаем. 
                //Здесь важно сначала сделать рекурсивный вызов, так как объем воды вышестоящих источников зависит от количества воды в нижестоящих.
                calculateQf(0);
                //Следующий шаг это просчитать напор на каждой станции, начиная с источника. 
                //И снова рекурсивный обход вглубь, только теперь наоборот сначала считаем для источника, затем идем дальше, отнимая от напора потери в процессе перехода воды по трубам.
                calculateH(0);
                
            }
            //Проверяется проходит ли норматив минимального напора воды вся сеть.
            while (checkNorms() == false);

            stations[station].accident = false;
        }

        private bool checkNorms()
        {
            for (int i = 0; i < stations.Count; i++)
                if(stations[i].h < hMin) return false;
            return true;
        }
    }
}
