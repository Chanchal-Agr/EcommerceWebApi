﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class StockQuery : QueryBase
    {
        public int ProductVariantId { get; set; }
    }
}
