using System;
using System.IO;
using System.Xml.Serialization;

namespace WaterCommunications.Localization
{
    public static class Localization
    {
        public static void serialize()
        {
            LocalizationErrors l = new LocalizationErrors();
            l.error101 = "Error 101\nIncorrect id, id must be integer. \nid = ";
            l.error102 = "Error 102\nIncorrect source id, source id must be integer.\nError for station with id = ";
            l.error103 = "Error 103\nIncorrect Qn, Qn must be positive double.\nError for station with id = ";
            l.error104 = "Error 104\nIncorrect L, L must be positive double.\nError for station with id = ";
            l.error105 = "Error 105\nIncorrect d, d must be positive double.\nError for station with id = ";
            XmlSerializer formatter = new XmlSerializer(typeof(LocalizationErrors));
            using (FileStream fs = new FileStream("LocalizationErrors.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, l);
            }
        }
        private static String GetLanguageName(Language language)
        {
            switch (language)
            {
                case Language.English: return "English";
                case Language.Russian: return "Russian";
                default: return "English";
            }
        }
        public static LocalizationErrors GetLocalizationErrors()
        {
            String path = (Environment.CurrentDirectory + @"\Localization\" + GetLanguageName((Language)Properties.Settings.Default.lanuage) + @"\LocalizationErrors.xml");
            LocalizationErrors result = new LocalizationErrors();
            XmlSerializer formatter = new XmlSerializer(typeof(LocalizationErrors));
            using (FileStream fs = new FileStream(@path, FileMode.OpenOrCreate))
            {
                result = (LocalizationErrors)formatter.Deserialize(fs);
            }
            return result;
        }
        public static LocalizationOutput GetLocalizationOutput()
        {
            String path = (Environment.CurrentDirectory + @"\Localization\" + GetLanguageName((Language)Properties.Settings.Default.lanuage) + @"\LocalizationOutput.xml");
            LocalizationOutput result = new LocalizationOutput();
            XmlSerializer formatter = new XmlSerializer(typeof(LocalizationOutput));
            using (FileStream fs = new FileStream(@path, FileMode.OpenOrCreate))
            {
                result = (LocalizationOutput)formatter.Deserialize(fs);
            }
            return result;
        }
        public static LocalizationUserInterface GetLocalizationUserInterface()
        {
            String path = (Environment.CurrentDirectory + @"\Localization\" + GetLanguageName((Language)Properties.Settings.Default.lanuage) + @"\LocalizationUserInterface.xml");
            LocalizationUserInterface result = new LocalizationUserInterface();
            XmlSerializer formatter = new XmlSerializer(typeof(LocalizationUserInterface));
            using (FileStream fs = new FileStream(@path, FileMode.OpenOrCreate))
            {
                result = (LocalizationUserInterface)formatter.Deserialize(fs);
            }
            return result;
        }
    }
}
