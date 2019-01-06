using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Shared;

namespace Refiner
{
    public class TransactionDataProcessor : DataProcessor
    {

        public dynamic ExtractDataArray(dynamic dataJObject)
        {
            return dataJObject.data;
        }
        

        public override dynamic Process(dynamic data)
        {
            JArray processedData = new JArray();

            foreach (dynamic obj in data)
            {
                dynamic entry = new JObject();
                //processedData = data; 
                // Clean data by means of extracting only what is needed.
                // Restructure data for ease of access and understability when creating queries
                entry.Reseller = new JObject();
                entry.User = new JObject();
                entry.Subscription = new JObject();
                entry.Bill = new JObject();

                // Correct structure of usergroups, to an enum array with user group classifiers
                if (obj.userGroup != null) { 
                    string userGroupStr = obj.userGroup;
                    string[] userGroups =  userGroupStr.Split(", ");
                }
                //entry.UserGroups = new JArray(userGroups);

                // Extracting values and placing them in the correct objects.
                entry.Reseller.Id = obj.resellerId;
                entry.Reseller.Name = obj.resellerName;
                entry.Reseller.Discount = obj.discount;
                
                entry.User.AccountId = obj.accountId;
                
                entry.Name = obj.subscriptionName;
                
                entry.Type = obj.type;
                entry.State = obj.state;
                entry.Price = obj.price;
                entry.OnExpiry = obj.onExpiry;
                entry.StartDate = obj.startDate;
                entry.ExpiryDate = obj.expiryDate;

                entry.BillingDate = obj.billingDate;
                entry.DiscountedPrice = obj.discountedPrice;
                entry.CurrencyName = obj.currencyName;
                entry.CurrencyISO = obj.code;

                processedData.Add(entry);
            }
            return processedData;
        }
    }
}