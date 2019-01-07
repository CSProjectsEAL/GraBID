using Serilog;
using Shared;
using System.Collections.Generic;

namespace Refiner
{
    public class DataSource
    {
        public string KeyOrigin { get; set; }
        public IList<DataHandler> DataHandlers {get;set;}

        public override string ToString()
        {
            return $"[{nameof(KeyOrigin)}={KeyOrigin}],[Number of {nameof(DataHandlers)}={DataHandlers.Count}]";
        }
    }
}
