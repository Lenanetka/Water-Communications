using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCommunications.DataReaderWriter
{
    interface IDataReaderWriter
    {
        List<Station> ReadFromFile(String path);
        void WriteInFile(String path, List<Station> stations, bool overwrite);
        void WriteInFile(String path, List<Station> stations, int station, bool overwrite);
    }
}
