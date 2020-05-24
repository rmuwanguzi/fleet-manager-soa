using FleetManager.Shared.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface ICreditorService : IService
    {
        Task<List<Models.creditorC>> GetAllCreditors(long fs_timestamp);
        Task<Models.creditorC> EditCreditorOpeningCrBalance(dto.dto_creditor_update_opening_cr_balanceC _dto);
        Task<Models.creditor_depositC> AddCreditorDeposit(dto.dto_creditor_depositC _dto);
        Task<bool> DeleteCreditorDeposit(int cr_deposit_id);
        Task<List<Models.creditor_depositC>> GetAllCreditorDepositsByRange(dto_fs_rangeC _dto);
        Task<List<Models.creditor_depositC>> GetAllCreditorDepositsByMonthPartition(dto_month_partitionC _dto);
        Task<List<Models.creditor_depositC>> GetCreditorDepositsByCreditorId(int cr_account_id, long fs_timestamp);
        Task<Models.creditorC> GetCreditorSingle(int cr_account_id);

        Task<List<Models.creditorC>> GetAllCreditorsBB(long fs_timestamp,long kapoge, string vinka);
    }
}
