using MongoDB.Bson.Serialization.Attributes;
using Refiner.TransactionItems;

namespace Refiner.Items
{
    public class TransactionItem
    {
        [BsonId]
        public int accountID { get; set; }
        public UserItem User { get; set; }
        public ResellerItem Reseller { get; set;}
        public string[] UserGroups { get; set; }
        public string SubscriptionName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public float Price { get; set; }
        public string OnExpiry { get; set; }
        public string StartDate { get; set; }
        public string ExpiryDate { get; set; }
    }
}