using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterCommunications
{
    interface IDataReaderWriter
    {
        Communications ReadFromFile(String path);
        //write in file info about optimal k for all stations
        void WriteInFile(String path, Communications communications);
        //write in file info about optimal k for 1 station and h, Q for all stations while accident is active
        void WriteInFile(String path, Communications communications, int station);        
    }
}
