using FleetManager.Shared.Models;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
   public interface IExpenseTypeService : IServiceCore<vh_expense_typeC, dto_expense_type_newC, dto_expense_type_updateC>
    {
             
    }
}
