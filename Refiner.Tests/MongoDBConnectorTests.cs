using NUnit.Framework;
using Refiner;

namespace Tests
{
    public class MongoDBConnectorTests
    {
        [Test]
        public void CanSaveAndRetrieveJSONCollection()
        {
            IDBFacade db = new MongoDBConnector("mongodb://localhost:27017");
            string jsonCollection = "{\"employees\":[{\"firstName\":\"John\", \"lastName\":\"Doe\"},{\"firstName\":\"Anna\", \"lastName\":\"Smith\"},{\"firstName\":\"Peter\",\"lastName\":\"Jones\"}]}";
            db.SaveCollection(jsonCollection, "Test", "refinedCache");
            string fetchedData = db.FetchCollection("Test", "refinedCache");
            Assert.AreEqual(jsonCollection, fetchedData); 
        }
    }
}