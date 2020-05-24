using FleetManager.Shared.Models;
using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
   public interface IExpenseTypeFieldItemService : IServiceCoreSingle<Models.vh_expense_type_field_d_itemC>
    {
        Task<vh_expense_type_field_d_itemC> AddExpenseTypeFieldItem(dto.dto_expense_type_field_item_newC _dto);
        Task<vh_expense_type_field_d_itemC> UpdateExpenseTypeFieldItem(dto.dto_expense_type_field_item_updateC _dto);
    }
}
