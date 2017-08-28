using System;
using System.IO;
using System.Xml.Serialization;

namespace WaterCommunications.Localization
{
    public static class CurrentLocalization
    {
        public static String GetLanguageName(int language)
        {
            switch (language)
            {
                case 0: return "English";
                case 1: return "Russian";
                default: return "English";
            }
        }
        private static T GetLocalization<T>(String fileName)
        {
            String path = (Environment.CurrentDirectory + @"\Localization\" + GetLanguageName(Properties.Settings.Default.lanuage) + @"\" + @fileName + @".xml");
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(fs);
            }
        }
        public static LocalizationErrors localizationErrors
        {
            get
            {
                return GetLocalization<LocalizationErrors>("LocalizationErrors");
            }
            set { }
        }
        public static LocalizationOutput localizationOutput
        {
            get
            {
                return GetLocalization<LocalizationOutput>("LocalizationOutput");
            }
            set { }
        }
        public static LocalizationSystemOfUnits localizationSystemOfUnits
        {
            get
            {
                return GetLocalization<LocalizationSystemOfUnits>("LocalizationSystemOfUnits");
            }
            set { }
        }
        public static LocalizationUserInterface localizationUserInterface
        {
            get
            {
                return GetLocalization<LocalizationUserInterface>("LocalizationUserInterface");
            }
            set { }
        }
    }
}
