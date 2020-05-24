using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface IExpenseTypeFieldService : IServiceCoreSingle<Models.vh_expense_type_fieldC>
    {
        Task<vh_expense_type_fieldC> AddExpenseTypeField(dto_expense_type_field_new _dto);
        Task<vh_expense_type_fieldC> UpdateExpenseTypeField(dto_expense_type_field_updateC dto);
    }
}
