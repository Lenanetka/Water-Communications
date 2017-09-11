using System;
using System.Collections.Generic;
using WaterCommunications.Localization;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;

namespace WaterCommunications
{
    class Communications
    {      
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
        public double repairSectionMinimumLength { get; }
        public double hMin { get; }
        public double accidentPercent { get; }
        public double additionalHeadLoss { get; }
        public Pipe pipe { get; }
        public Communications(List<Station> stations, Parameters parameters, Pipe pipe)
        {
            this.stations = stations;
            stations.Insert(0, new Station(parameters.mainStationId));
            connectStations();
            h = parameters.h;
            hMin = parameters.hMin;
            accidentPercent = parameters.accidentPercent / 100;
            repairSectionMinimumLength = parameters.repairSectionMinimumLength;
            additionalHeadLoss = 1 + parameters.additionalHeadLoss / 100;
            this.pipe = pipe;
        }      
        private void connectStations()
        {
            for (int i = 0; i < stations.Count; i++)
                for (int j = 1; j < stations.Count; j++)
                {
                    if (stations[j].sourceId == stations[i].id && i != j)
                    {
                        stations[j].source = i;
                        stations[i].addSubStation(j);
                    }
                }
        }
        #region checkData       
        public void checkData()
        {
            checkError(hasCycle, true);
            checkError(hasAllSources, true);
            checkError(hasCorrectHmin, false);
            checkError(hasCorrectWaterVolume, false);
            checkError(hasDoublePipes, false);
        }       
        public event EventHandler<NonCriticalErrorEventArgs> NonCriticalError;
        protected virtual void OnNonCriticalError(NonCriticalErrorEventArgs e)
        {
            EventHandler<NonCriticalErrorEventArgs> handler = NonCriticalError;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private delegate void Check();
        private void checkError(Check check, bool critical)
        {
            try
            {
                check();
            }
            catch(Exception e)
            {
                if (critical) throw e;
                else OnNonCriticalError(new NonCriticalErrorEventArgs(e.Message + Languages.current.messageContinueCalculating));
            }
        }
        private void hasCycle()
        {
            List<int> traversal = new List<int>();
            traversal.Add(0);

            int k = 0;
            while (k < traversal.Count)
            {
                for (int i = 0; i < k; i++)
                    if (traversal[i] == traversal[k]) throw new Exception(Languages.current.error204);
                foreach (int s in stations[traversal[k]].subs) traversal.Add(s);
                k++;
            }
        }
        private void hasAllSources()
        {
            foreach (Station s in stations)
                if (s.source == -1) throw new Exception(Languages.current.error202 + s.id);
        }
        private void hasCorrectHmin()
        {         
            if (stations[0].h < hMin)
            {
                throw new Exception(Languages.current.error201);
            }
        }       
        private void hasCorrectWaterVolume()
        {
            for (int i = 1; i < stations.Count; i++)
            {
                double Qsum = 0;
                foreach (int s in stations[i].subs) Qsum += stations[s].Qn;
                if (stations[i].Qn < Qsum)
                {
                    throw new Exception(Languages.current.error203 + stations[i].id);
                }               
            }
        }       
        private void hasDoublePipes()
        {
            foreach (Station st in stations)
            {
                List<int> subsId = new List<int>();
                foreach (int s in st.subs)
                {
                    if (subsId.Contains(stations[s].id))
                    {
                        throw new Exception(Languages.current.error205 + st.id + " - " + stations[s].id);
                    }
                    subsId.Add(stations[s].id);
                }
            }
        }
        #endregion
        #region calculating
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

        private static double g = 9.81;
        private double calcilateHLost(double Q, double L, double d)
        {
            Q /= 3600;
            d /= 1000;
            L *= 1000;

            double v = (4 * Q) / (Math.PI * d * d);

            pipe.v = v;

            double m = pipe.m;
            double A0 = pipe.A0;
            double A1 = pipe.A1 / 1000;
            double c = pipe.C;

            double hLost = (A1 / (2 * g)) * (Math.Pow(A0 + c / v, m) / Math.Pow(d, m + 1)) * v * v * L * additionalHeadLoss;
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
        private static int kMax = 100;
        public void calculateOptimalK(int station)
        {
            stations[station].accident = true;
            stations[station].k = 0;

            do
            {
                simulateAccident(station);
            }
            while (checkNorms() == false && Math.Floor(stations[station].pipeLength / repairSectionMinimumLength) > stations[station].k && stations[station].k < kMax);
          
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
        #endregion
    }
}
