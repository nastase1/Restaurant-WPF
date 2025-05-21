using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.ViewModels
{
    public class LowStockProductViewModel
    {
        public string NumePreparat { get; set; }
        public decimal StocTotalGramaj { get; set; }
        public string DisplayStoc => $"{StocTotalGramaj}g";
    }
}
