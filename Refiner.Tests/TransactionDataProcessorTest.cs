using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Refiner.Tests
{
    public class TransactionDataProcessorTest
    {
        [Test]
        public void CanProcessData()
        {
            // Positive cases
            string jsonInputCase1 = "{data:[{\"hello\" : 1}]}";
            string jsonInputCase2 = "{data:[{\"userGroup\" : \"one,two\"}]}";

            string jsonInputCase3 = "{\r\n  \"status\": {\r\n    \"code\": 200,\r\n    \"text\": \"\",\r\n    \"timeStamp\": \"2018-10-24T05:56:30+00:00\"\r\n  },\r\n  \"data\": [\r\n    {\r\n      \"success\": true,\r\n      \"transactions\": [\r\n        {\r\n          \"id\": 1,\r\n          \"type\": \"STARTUP\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:27:16\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"ACTIVE\",\r\n          \"site_Id\": 816899,\r\n          \"accountId\": 1,\r\n          \"expire\": \"2018-09-25 06:27:16\",\r\n          \"startDate\": \"2017-09-25 06:27:16\",\r\n          \"billingDate\": \"2018-09-25 06:27:16\",\r\n          \"onExpiry\": \"RENEW\",\r\n          \"apiId\": \"professional\",\r\n          \"resellerId\": 86,\r\n          \"userGroup\": \"professional,site_n,social,site_n,social\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        },\r\n        {\r\n          \"id\": 2,\r\n          \"type\": \"RENEWAL\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:27:16\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"ACTIVE\",\r\n          \"site_Id\": 816899,\r\n          \"accountId\": 1,\r\n          \"expire\": \"2018-09-25 06:27:16\",\r\n          \"startDate\": \"2017-09-25 06:27:16\",\r\n          \"billingDate\": \"2018-09-25 06:27:16\",\r\n          \"onExpiry\": \"RENEW\",\r\n          \"apiId\": \"professional\",\r\n          \"resellerId\": 86,\r\n          \"userGroup\": \"professional,site_n,social,site_n,social\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        },\r\n        {\r\n          \"id\": 3,\r\n          \"type\": \"STARTUP\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:28:20\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"TERMINATED\",\r\n          \"site_Id\": 816901,\r\n          \"accountId\": 3,\r\n          \"expire\": \"2018-09-25 06:28:20\",\r\n          \"startDate\": \"2017-09-25 06:28:20\",\r\n          \"billingDate\": \"2018-09-25 06:28:20\",\r\n          \"onExpiry\": \"RENEW\",\r\n          \"apiId\": \"professional\",\r\n          \"resellerId\": 86,\r\n          \"userGroup\": \"professional,site_n,social,site_n,social\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        },\r\n        {\r\n          \"id\": 4,\r\n          \"type\": \"RENEWAL\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:28:20\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"TERMINATED\",\r\n          \"site_Id\": 816901,\r\n          \"accountId\": 3,\r\n          \"expire\": \"2018-09-25 06:28:20\",\r\n          \"startDate\": \"2017-09-25 06:28:20\",\r\n          \"billingDate\": \"2018-09-25 06:28:20\",\r\n          \"onExpiry\": \"RENEW\",\r\n          \"apiId\": \"professional\",\r\n          \"resellerId\": 86,\r\n          \"userGroup\": \"professional,site_n,social,site_n,social\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        },\r\n        {\r\n          \"id\": 5,\r\n          \"type\": \"STARTUP\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:47:09\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"TERMINATED\",\r\n          \"site_Id\": 816902,\r\n          \"accountId\": 4,\r\n          \"expire\": \"2017-11-25 06:47:09\",\r\n          \"startDate\": \"2017-09-25 06:47:09\",\r\n          \"billingDate\": \"2017-11-25 06:47:09\",\r\n          \"onExpiry\": \"TERMINATE\",\r\n          \"apiId\": \"trial\",\r\n          \"resellerId\": 191,\r\n          \"userGroup\": \"trial\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        }      ],\r\n      \"meta\": {\r\n        \"count\": 263,\r\n        \"lastId\": 263\r\n      }\r\n    }\r\n  ]\r\n}";
            string expectedCase1 = "[{\r\n \t\"id\": 1,\r\n \t\"type\": \"STARTUP\",\r\n \t\"user\": \"Cron\",\r\n \t\"timestamp\": \"2017-09-25 06:27:16\",\r\n \t\"price\": \"0.00\",\r\n \t\"discount\": \"0.00\",\r\n \t\"state\": \"ACTIVE\",\r\n \t\"site_Id\": 816899,\r\n \t\"accountId\": 1,\r\n \t\"expire\": \"2018-09-25 06:27:16\",\r\n \t\"startDate\": \"2017-09-25 06:27:16\",\r\n \t\"billingDate\": \"2018-09-25 06:27:16\",\r\n \t\"onExpiry\": \"RENEW\",\r\n \t\"apiId\": \"professional\",\r\n \t\"resellerId\": 86,\r\n \t\"userGroup\": \"professional,site_n,social,site_n,social\",\r\n \t\"code\": \"EUR\",\r\n \t\"currencyName\": \"Euro\"\r\n }]";

        
            //Processor need to adhere to interface segregation principle, to allow for processors to be stored in same collection.
            TransactionDataProcessor processor = new TransactionDataProcessor();
            dynamic data1 = DataProcessor.ConvertDataToDynamicObject(jsonInputCase3);
          
         
           // Assert.AreEqual("{data:[{\"hello\" : 1}]}", jsonInputCase1);
            dynamic extractedData = processor.ExtractDataArray(data1);
            JArray datastr = processor.Process(extractedData);
            
            JToken ob = processor.Process(extractedData);
            //JsonConvert.DeserializeObject<List<RootObject>>(string);
            //Assert.AreEqual("", ob.ToString());
            // JArray output = processor.Process(data1);
        }

        [Test]
        public void CanValidateProcessedData()
        {
            // With the intention of verifying the integrity of the data.
            // Positive cases
            string jsonInputCase1 = "\"status\": {\r\n    \"code\": 200,\r\n    \"text\": \"\",\r\n    \"timeStamp\": \"2018-10-24T05:56:30+00:00\"\r\n  },\r\n  \"data\": [\r\n    {\r\n      \"success\": true,\r\n      \"transactions\": [\r\n        {\r\n          \"id\": 1,\r\n          \"type\": \"STARTUP\",\r\n          \"user\": \"Cron\",\r\n          \"timestamp\": \"2017-09-25 06:27:16\",\r\n          \"price\": \"0.00\",\r\n          \"discount\": \"0.00\",\r\n          \"state\": \"ACTIVE\",\r\n          \"site_Id\": 816899,\r\n          \"accountId\": 1,\r\n          \"expire\": \"2018-09-25 06:27:16\",\r\n          \"startDate\": \"2017-09-25 06:27:16\",\r\n          \"billingDate\": \"2018-09-25 06:27:16\",\r\n          \"onExpiry\": \"RENEW\",\r\n          \"apiId\": \"professional\",\r\n          \"resellerId\": 86,\r\n          \"userGroup\": \"professional,site_n,social,site_n,social\",\r\n          \"code\": \"EUR\",\r\n          \"currencyName\": \"Euro\"\r\n        }      ],\r\n      \"meta\": {\r\n        \"count\": 263,\r\n        \"lastId\": 263\r\n      }\r\n    }\r\n  ]\r\n}";
            // Negative cases
            //Processor need to adhere to interface segregation principle, to allow for abstraction to point where they can be stored in the same collection.
        }
    }
}
