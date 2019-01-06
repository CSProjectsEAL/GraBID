using System;
using System.Collections.Generic;
using System.Text;

namespace Refiner.Items
{
    public class UserItem
    {
        public int AccountId { get; set; }
        public string BillingDate { get; set; }
        public double DiscountedPrice { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyISO { get; set; }
    }
}
