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
        void WriteInFile(String path, Communications communications, bool overwrite);
        void WriteInFile(String path, Communications communications, int station, bool overwrite);
    }
}
