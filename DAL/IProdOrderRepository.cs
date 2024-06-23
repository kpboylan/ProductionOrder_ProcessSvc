using MES_ProcessSvc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ProcessSvc.DAL
{
    internal interface IProdOrderRepository
    {
        void AddProdOrder(ProdOrder prodOrder);
    }
}
