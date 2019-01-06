using Newtonsoft.Json.Linq;

namespace Refiner
{
    public abstract class DataProcessor : IDataProcessor
    {
        public static dynamic ConvertDataToDynamicObject(string envelopeData)
        {
            return JObject.Parse(envelopeData);
        }
        public abstract dynamic Process(dynamic envelopeData);
    }
}
