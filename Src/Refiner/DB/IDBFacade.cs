using Newtonsoft.Json.Linq;

namespace Refiner
{
    public interface IDBFacade
    {
        void SaveCollection(string collection, string collectionName, string databaseName);
        string FetchCollection(string collectionName, string databaseName);
        string FetchCollection(string collectionName, string databaseName, string fromId);
        string LatestID(string collectionName, string databaseName);
    }
}
