using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioApi.Models
{
    public class Operacao
    {
        public int Id { get; set; }

        public string Side { get; set; }

        public Decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Symbol { get; set; }
    }
}
