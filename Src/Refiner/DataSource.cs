using Serilog;
using Shared;
using System.Collections.Generic;

namespace Refiner
{
    public class DataSource
    {
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public IList<DataHandler> DataHandlers {get;set;}

        public override string ToString()
        {
            return $"[{nameof(Queue)}={Queue}],[{nameof(Exchange)}={Exchange}],[Number of {nameof(DataHandlers)}={DataHandlers.Count}]";
        }
    }
}
