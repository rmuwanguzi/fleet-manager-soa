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
   public  interface IExpenseService : IServiceCoreSingle<vh_expense_transC>
    {
        Task<List<vh_expense_transC>> AddExpense(dto_expense_trans_voucher_newC _dto);
        Task<List<vh_expense_transC>> GetAllExpensesRange(dto_fs_rangeC _dto);
        Task<List<vh_expense_transC>> GetAllExpensesByMonthPartition(dto_month_partitionC _dto);
        Task<List<vh_expense_transC>> GetAllRecentVehicleExpenses(dto_vehicle_expense_recentC _dto);
        Task<bool> DeleteExpenseVoucher(int voucher_id);
    }
}
