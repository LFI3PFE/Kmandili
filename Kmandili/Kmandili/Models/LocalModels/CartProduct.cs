﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kmandili.Models.LocalModels
{
    public class CartProduct
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }

        public void updateTotal()
        {
            Total = Quantity * Product.Price;
        }
    }
}
