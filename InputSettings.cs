using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Parameters(int mainStationId, double h, double hMin, double accidentPercent, double repairSectionMinimumLength)
        {
            this.mainStationId = mainStationId;
            this.h = h;
            this.hMin = hMin;
            this.accidentPercent = accidentPercent;
            this.repairSectionMinimumLength = repairSectionMinimumLength;
        }
    }
    public class InputSettings
    {
        public SaveLoadInfo saveLoadInfo;
        public Parameters parameters;     

        public InputSettings(SaveLoadInfo saveLoadInfo, Parameters parameters)
        {
            this.saveLoadInfo = saveLoadInfo;
            this.parameters = parameters;
            
        }
    }
}
