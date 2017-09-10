using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ib.mbank
{
    public class Transaction
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
