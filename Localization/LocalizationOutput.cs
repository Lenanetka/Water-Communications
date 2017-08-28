using System;

namespace WaterCommunications.Localization
{
    [Serializable]
    public class LocalizationMainInfo
    {
        public LocalizationMainInfo()
        {

        }
        public String title;
        public String destination;
        public String source;
        public String length;
        public String optimalK;
    }
    [Serializable]
    public class LocalizationStartDatas
    {
        public LocalizationStartDatas()
        {

        }
        public String title;
        public String idMain;
        public String headMain;
        public String headMin;
        public String accidentPercent;
        public String lengthRepairSection;
    }
    [Serializable]
    public class LocalizationAllInfo
    {
        public LocalizationAllInfo()
        {

        }
        public String title;
        public String destination;
        public String source;
        public String length;
        public String optimalK;
        public String diameter;
        public String fluidFlow;
        public String head;
    }

    [Serializable]
    public class LocalizationOutput
    {
        public LocalizationOutput()
        {
            mainInfo = new LocalizationMainInfo();
            startDatas = new LocalizationStartDatas();
            allInfo = new LocalizationAllInfo();
        }
        public LocalizationMainInfo mainInfo;
        public LocalizationStartDatas startDatas;
        public LocalizationAllInfo allInfo;
    }
}
