﻿using System.Collections.Generic;

namespace Commerce.Engine
{
    public class OrderData
    {
        public string CustomerEmail { get; set; }
        public List<OrderLineItemData> LineItems { get; set; }
        public string CreditCard { get; set; }
        public string ExpirationDate { get; set; }
    }
}