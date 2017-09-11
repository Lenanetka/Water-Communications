using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WaterCommunications.Localization
{
    [Serializable]
    public class Languages
    {
        public Languages()
        {
        }
        public String bCalculate { get; set; }
        public String bBrowsePathTooltip { get; set; }
        public String bOk { get; set; }
        public String bYes { get; set; }
        public String bNo { get; set; }

        public String cbOnlyMainInfo { get; set; }

        public String labelLoad { get; set; }
        public String labelSave { get; set; }
        public String labelSourceId { get; set; }
        public String labelH { get; set; }
        public String labelHMin { get; set; }
        public String labelAccidentPercent { get; set; }
        public String labelRepairSectionMinimumLength { get; set; }
        public String labelPipeMaterial { get; set; }
        public String labelAdditionalHeadLoss { get; set; }

        public List<String> cbPipeMaterial { get; set; }

        public String mFile { get; set; }
        public String mOpen { get; set; }
        public String mCalculate { get; set; }
        public String mExit { get; set; }
        public String mSettings { get; set; }
        public String mLanguage { get; set; }
        public String mHelp { get; set; }
        public String mAbout { get; set; }

        public String tiMain { get; set; }
        public String tiGraph { get; set; }

        public String messageFinishCalculating { get; set; }
        public String messageTitleFinishCalculating { get; set; }
        public String messageContinueCalculating { get; set; }

        public String error { get; set; }
        public String error101 { get; set; }
        public String error102 { get; set; }
        public String error103 { get; set; }
        public String error104 { get; set; }
        public String error105 { get; set; }
        public String error106 { get; set; }
        public String error107 { get; set; }
        public String error201 { get; set; }
        public String error202 { get; set; }
        public String error203 { get; set; }
        public String error204 { get; set; }
        public String error205 { get; set; }

        public String outputTitleMain { get; set; }
        public String outputTitleSettings { get; set; }
        public String outputTitleAll { get; set; }
        public String outputDestination { get; set; }
        public String outputSource { get; set; }
        public String outputLength { get; set; }
        public String outputOptimalK { get; set; }
        public String outputDiameter { get; set; }
        public String outputFluidFlow { get; set; }
        public String outputHead { get; set; }
        public String outputIdMain { get; set; }
        public String outputHeadMain { get; set; }
        public String outputHeadMin { get; set; }
        public String outputAccidentPercent { get; set; }
        public String outputLengthRepairSection { get; set; }
        public String outputPipeMaterial { get; set; }
        public String outputAdditionalHeadLoss { get; set; }

        public String percent { get; set; }
        public String m { get; set; }
        public String km { get; set; }
        public String mm { get; set; }
        public String m3h { get; set; }
        
        public static Languages current
        {
            get
            {
                String path = (Environment.CurrentDirectory + @"\Localization\" + @Properties.Settings.Default.language + @".xml");
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var xmlSerializer = new XmlSerializer(typeof(Languages));
                    return (Languages)xmlSerializer.Deserialize(fs);
                }
            }
        }
        public static List<String> list()
        {
            string path = (Environment.CurrentDirectory + @"\Localization\");
            var result = new List<String>();
            foreach (var file in new DirectoryInfo(path).GetFiles("*.xml")) result.Add(Path.GetFileNameWithoutExtension(file.Name));
            return result;
        }
    }
}
