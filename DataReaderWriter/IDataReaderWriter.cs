using System;

namespace WaterCommunications.DataReaderWriter
{
    interface IDataReaderWriter
    {
        Communications ReadFromFile(String path);
        //write in file info about optimal k for all stations
        void WriteInFile(String path, Communications communications, bool overwrite);
        //write in file info about optimal k for 1 station and h, Q for all stations while accident is active
        void WriteInFile(String path, Communications communications, int station, bool overwrite);        
    }
}
