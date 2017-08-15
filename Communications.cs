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
        public double repairSectionMinimumLength;
        public double hMin;
        public double accidentPercent;
        public Communications(List<Station> stations)
        {
            repairSectionMinimumLength = 0;
            this.stations = stations;           
            connectStations();                       
        }
        
        private void connectStations()
        {
            for (int i = 0; i < stations.Count; i++)
                for (int j = 0; j < stations.Count; j++)
                {
                    if (stations[j].sourceId == stations[i].id && i != j)
                    {
                        stations[j].source = i;
                        stations[i].addSubStation(j);
                    }
                }
        }
        private delegate bool Check(out string errorMessage);
        public void checkData()
        {
            hasCycle();

            checkNoCriticalError(hasCorrectHmin);
            checkNoCriticalError(hasAllSources);
            checkNoCriticalError(hasCorrectWaterVolume);
            checkNoCriticalError(hasDoublePipes);
        }
        private void hasCycle()
        {
            List<int> traversal = new List<int>();
            traversal.Add(0);

            int k = 0;
            while (k < traversal.Count)
            {
                for (int i = 0; i < k; i++)
                    if (traversal[i] == traversal[k]) throw new Exception("Error 204\nYour communication has cycle");
                foreach (int s in stations[traversal[k]].subs) traversal.Add(s);
                k++;
            }            
        }
        private void checkNoCriticalError(Check check)
        {
            String errorMessage;
            if (!check(out errorMessage))
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(errorMessage + "\nDo you want to continue calculating?", "Error", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.No)
                {
                    throw new Exception("Error 210\nCalculating was cancelled");
                }
            }
        }
        private bool hasCorrectHmin(out string errorMessage)
        {         
            if (stations[0].h < hMin)
            {
                errorMessage = "Error 201\nH < Hmin";
                return false;
            }
            errorMessage = "All is OK";
            return true;
        }
        private bool hasAllSources(out string errorMessage)
        {
            foreach (Station s in stations)
                if (s.source == -1)
                {
                    errorMessage = "Error 202\nThere are no source for station " + s.id;
                    return false;
                }
            errorMessage = "All is OK";
            return true;
        }
        private bool hasCorrectWaterVolume(out string errorMessage)
        {
            for (int i = 1; i < stations.Count; i++)
            {
                double Qsum = 0;
                foreach (int s in stations[i].subs) Qsum += stations[s].Qn;
                if (stations[i].Qn < Qsum)
                {
                    errorMessage = "Error 203\nVolume of water in pipes less than sum of all sub-stations for station " + stations[i].id;
                    return false;
                }               
            }
            errorMessage = "All is OK";
            return true;
        }       
        private bool hasDoublePipes(out string errorMessage)
        {
            foreach (Station st in stations)
            {
                List<int> subsId = new List<int>();
                foreach (int s in st.subs)
                {
                    if (subsId.Contains(stations[s].id))
                    {
                        errorMessage = "Error 205\nYou have more than 1 connection between station " + st.id + " and " + stations[s].id;
                        return false;
                    }
                    subsId.Add(stations[s].id);
                }
            }
            errorMessage = "All is OK";
            return true;
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
            Q /= 3600;
            d /= 1000;
            L *= 1000;

            double v = (4 * Q) / (Math.PI * d * d);
            double hLost = (A1 / (2 * g)) * (Math.Pow(A0 + c / v, m) / Math.Pow(d, m + 1)) * v * v * L * K;
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
        private void simulateAccident(int station)
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
        public void calculateOptimalK(int station)
        {
            stations[station].accident = true;
            stations[station].k = 0;

            do
            {
                simulateAccident(station);
            }
            while (checkNorms() == false && Math.Floor(stations[station].pipeLength / repairSectionMinimumLength) > stations[station].k);
          
            List<int> notCheck = getOutOfNormsStations();
            if(notCheck.Count > 0)
            {
                stations[station].k = 0;
                do
                {
                    simulateAccident(station);
                }
                while (checkNorms(notCheck) == false && Math.Floor(stations[station].pipeLength / repairSectionMinimumLength) > stations[station].k);
            }         
            stations[station].accident = false;
        }
        private List<int> getOutOfNormsStations()
        {
            List<int> outOfNormsStations = new List<int>();
            for (int i = 0; i < stations.Count; i++)
                if (stations[i].h < hMin) outOfNormsStations.Add(i);
            return outOfNormsStations;
        }
        private bool checkNorms()
        {
            for (int i = 0; i < stations.Count; i++)
                if(stations[i].h < hMin) return false;
            return true;
        }
        private bool checkNorms(List<int> notCheck)
        {
            for (int i = 0; i < stations.Count; i++)
                if (stations[i].h < hMin && !notCheck.Contains(i)) return false;
            return true;
        }
    }
}
