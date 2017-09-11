using System;

namespace WaterCommunications
{
    public class SaveLoadInfo
    {
        public String pathLoad;
        public String pathSave;
        public bool onlyMainInfo;
        public SaveLoadInfo(String pathLoad, String pathSave, bool onlyMainInfo)
        {
            this.pathLoad = pathLoad;
            this.pathSave = pathSave;
            this.onlyMainInfo = onlyMainInfo;
        }
    }
    public class Parameters
    {
        public int mainStationId;
        public double h;
        public double hMin;
        public double accidentPercent;
        public double repairSectionMinimumLength;
        public double additionalHeadLoss;
        public Parameters(int mainStationId, double h, double hMin, double accidentPercent, double repairSectionMinimumLength, double additionalHeadLoss)
        {
            this.mainStationId = mainStationId;
            this.h = h;
            this.hMin = hMin;
            this.accidentPercent = accidentPercent;
            this.repairSectionMinimumLength = repairSectionMinimumLength;
            this.additionalHeadLoss = additionalHeadLoss;
        }
    }
    public class Pipe
    {
        public String name;
        public int id;

        private double _m;
        private double _A0;
        private double _A1;
        private double _C;

        private double _v;
        private double vLimit;

        private double _mLimit;
        private double _A0Limit;
        private double _A1Limit;
        private double _CLimit;

        public double v
        {
            get
            {
                return _v;
            }
            set
            {
                if (vLimit == -1) _v = -2;
                else if (value < 0) _v = 0;
                else _v = value;
            }
        }
        public double m
        {
            get
            {
                if (v < vLimit) return _m;
                else return _mLimit;
            }
        }
        public double A0
        {
            get
            {
                if (v < vLimit) return _A0;
                else return _A0Limit;
            }
        }
        public double A1
        {
            get
            {
                if (v < vLimit) return _A1;
                else return _A1Limit;
            }
        }
        public double C
        {
            get
            {
                if (v < vLimit) return _C;
                else return _CLimit;
            }
        }

        public Pipe(String name, int id, double m, double A0, double A1, double C)
        {
            this.name = name;
            this.id = id;
            _m = m;
            _A0 = A0;
            _A1 = A1;
            _C = C;
            vLimit = -1;
        }
        public Pipe(String name, int id, double m, double A0, double A1, double C, double vLimit, double mLimit, double A0Limit, double A1Limit, double CLimit)
        {
            this.name = name;
            this.id = id;
            _m = m;
            _A0 = A0;
            _A1 = A1;
            _C = C;
            this.vLimit = vLimit;
            _mLimit = mLimit;
            _A0Limit = A0Limit;
            _A1Limit = A1Limit;
            _CLimit = CLimit;
        }
        public override string ToString()
        {
            return name;
        }
    }
    public class InputSettings
    {
        public SaveLoadInfo saveLoadInfo;
        public Parameters parameters;
        public Pipe pipe;

        public InputSettings(SaveLoadInfo saveLoadInfo, Parameters parameters, Pipe pipe)
        {
            this.saveLoadInfo = saveLoadInfo;
            this.parameters = parameters;
            this.pipe = pipe;
        }
    }
}
