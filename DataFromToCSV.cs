using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCommunications
{
    class DataFromToCSV: IDataReaderWriter
    {
        public DataFromToCSV()
        {
        }
        public Communications ReadFromFile(String path)
        {
            //для источника данные из полей программы
            return null;
        }
        public void WriteInFile(String path, Communications communications)
        {

        }
        public void WriteInFile(String path, Communications communications, int station)
        {

        }
    }
}
