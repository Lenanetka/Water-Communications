using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCommunications.Localization
{
    [Serializable]
    public class LocalizationMainPage
    {
        public LocalizationMainPage()
        {

        }
        public String tiMain;
        public String tiGraph;
        public String labelLoad;
        public String labelSave;
        public String tooltipLoadSave;
        public String labelSourceId;
        public String labelH;
        public String LabelHMin;
        public String LabelAccidentPercent;
        public String LabelRepairSectionMinimumLength;
        public String cbOnlyMainInfo;
        public String bCalculate;
    }
    [Serializable]
    public class LocalizationMenu
    {
        public LocalizationMenu()
        {

        }
        public String file;
        public String file_open;
        public String file_calculate;
        public String file_exit;

        public String settings;
        public String settings_language;
        public String settings_language_english;
        public String settings_language_russian;

        public String help;
        public String help_about;
    }
    [Serializable]
    public class LocalizationMessages
    {
        public LocalizationMessages()
        {

        }
        public String ok;
        public String yes;
        public String no;
        public String finishCalculating;
        public String finishCalculatingTitle;
        public String continueCalculating;
    }
    [Serializable]
    public class LocalizationUserInterface
    {       
        public LocalizationUserInterface()
        {
            mainPage = new LocalizationMainPage();
            menu = new LocalizationMenu();
            messages = new LocalizationMessages();
        }
        public LocalizationMainPage mainPage;
        public LocalizationMenu menu;
        public LocalizationMessages messages;
    }
}
