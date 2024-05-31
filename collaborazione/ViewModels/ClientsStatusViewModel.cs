using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class ClientsStatusViewModel
    {
        public int TotalClients { get; set; }
        
        public double TotalCommission { get; set; }//totalCommition 40 from 2 clients(if commetion is not 0)
        public int TotalCommissionOfClients { get; set; }

        public double TotalCommissionPaid { get; set; }//commissionPaid 300 of 5 clients(where progress==4)
        public int TotalCommissionPaidOfClients { get; set; }

        public double TotalProfitBalance { get; set; }//totalProfit 500 (TotalCommission-TotalCommissionPaid=this)
    }
}
