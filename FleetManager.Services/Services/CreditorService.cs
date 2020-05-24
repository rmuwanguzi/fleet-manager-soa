using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MutticoFleet.Service;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.Models;

namespace MutticoFleet.Services
{
    public class CreditorService : FleetManager.Shared.Interfaces.ICreditorService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        
        public CreditorService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();

        }
        private void AddErrorMessage(string error_key, string _title, string _error_message)
        {
            if (!string.IsNullOrEmpty(controller_key))
            {
                _dialog.ErrorMessage(error_key, _error_message, controller_key);
            }
            else
            {
                _dialog.ErrorMessage(_error_message, "Save Error");
            }
        }
        public Task<creditor_depositC> AddCreditorDeposit(dto_creditor_depositC _dto)
        {
            creditor_depositC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Deposit Object Is Null");
                return Task.FromResult(_obj);
            }
            if (_dto.cr_account_id <= 0)
            {
                AddErrorMessage("Error", "Error", "Invalid Creditor Account ID");
                return Task.FromResult(_obj);
            }
            if(_dto.deposit_date==null)
            {
                AddErrorMessage("Error", "Error", "Deposit Date Missing");
                return Task.FromResult(_obj);
            }
            if(_dto.amount_deposited<=0)
            {
                AddErrorMessage("Error", "Error", "Invalid Deposited Amount");
                return Task.FromResult(_obj);
            }
            if(string.IsNullOrEmpty(_dto.pay_ref_no))
            {
                AddErrorMessage("Error","Error", "Payment Ref No Is Missing");
                return Task.FromResult(_obj);
            }

            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var _creditor = _db.CREDITORS_SET.Where(e => e.cr_account_id == _dto.cr_account_id & e.delete_id == 0).FirstOrDefault();
                    if (_creditor == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Creditor Object");
                        return Task.FromResult(_obj);
                    }
                    _obj = new creditor_depositC()
                    {
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                        cr_account_id = _creditor.cr_account_id,
                        guid_key = _dto.pay_ref_no,
                        amount_deposited = _dto.amount_deposited,
                        cr_account_name = _creditor.cr_account_name,
                        cr_account_type_id = _creditor.cr_account_type_id,
                        deposit_date = _dto.deposit_date.Value,
                        deposit_fs_id = fn.GetFSID(_dto.deposit_date.Value),
                        deposit_details = _dto.deposit_details,
                        m_partition_id = string.Format("{0}{1}", _dto.deposit_date.Value.Year, _dto.deposit_date.Value.Month).ToInt32(),
                        balance = _dto.amount_deposited,
                        pay_ref_no=_dto.pay_ref_no,
                        cr_payment_mode_id=_dto.cr_payment_mode_id,
                        

                    };
                    _db.CREDITOR_DEPOSITS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        switch(_obj.cr_payment_mode)
                        {
                            case FleetManager.Shared.em.creditor_payment_mode.cash:
                                {
                                    AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Already Entered This Cash Voucher Payment");
                                    break;
                                }
                            case FleetManager.Shared.em.creditor_payment_mode.cheque:
                                {
                                    AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Already Entered This Cheque Payment");
                                    break;
                                }
                            default:
                                {
                                    AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Already This Deposit/Payment");
                                    break;
                                }
                        }
                       
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    _creditor.last_pay_date = _obj.deposit_date;
                    _creditor.fs_timestamp = fnn.GetUnixTimeStamp();
                    _creditor.last_cr_deposit_id = _obj.cr_deposit_id;
                    _db.CREDITORS_SET.AddOrUpdateExtension(_creditor);
                    _db.SaveChanges();
                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            if (_obj != null)
            {
                Task.Run(() =>
                {
                    using (var _db = fnn.GetDbContext())
                    {
                        fnn.CreateCreditorStatementFromDeposit(_obj, _db);
                        _db.SaveChanges();
                    }
                    fnn.RunCreditorPayments(_obj.cr_account_id);

                });
                
            }
            return Task.FromResult(_obj);
        }

        public Task<bool> DeleteCreditorDeposit(int cr_deposit_id)
        {
            bool _record_deleted = false;
            FleetManager.Shared.Models.creditor_depositC _deposit = null;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _deposit = _db.CREDITOR_DEPOSITS.Where(e => e.cr_deposit_id == cr_deposit_id & e.delete_id == 0).FirstOrDefault();
                    if (_deposit == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Deposit Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                        {
                            pk_col_name = "cr_deposit_id",
                            pk_id = _deposit.cr_deposit_id,
                            table_name = DbHelper.GetTableSchemaName( FleetManager.DbBase.tableNames.creditor_deposit_tb)
                        }, _db);
                        if (_result == null || _result == false)
                        {
                            AddErrorMessage("Delete Error", "Delete Error", "Error Encountered While Trying To Delete Record");
                            _record_deleted = false;
                            return Task.FromResult(_record_deleted);
                        }
                        else
                        {
                            _record_deleted = true;
                            _db.SaveChanges();
                            
                        }
                        var _creditor = (from c in _db.CREDITORS_SET
                                         where c.cr_account_id == _deposit.cr_account_id & c.delete_id == 0
                                         select c).FirstOrDefault();
                        if (_creditor != null)
                        {
                            var _last_deposit = (from k in _db.CREDITOR_DEPOSITS
                                                 where k.delete_id == 0
                                                 orderby k.deposit_fs_id descending, k.cr_deposit_id
                                                 select k).FirstOrDefault();
                            if (_last_deposit != null)
                            {
                                _creditor.last_cr_deposit_id = _last_deposit.cr_deposit_id;
                                _creditor.last_pay_date = _last_deposit.deposit_date;
                            }
                            else
                            {
                                _creditor.last_cr_deposit_id = 0;
                                _creditor.last_pay_date = null;
                            }
                            _creditor.fs_timestamp = fnn.GetUnixTimeStamp();
                            //
                            _db.CREDITORS_SET.AddOrUpdateExtension(_creditor);
                            _db.SaveChanges();
                        }


                    }
                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            if(_record_deleted)
            {
                Task.Run(() =>
                {
                    using (var _db = fnn.GetDbContext())
                    {
                        //var _creditor = (from k in _db.CREDITORS_SET
                        //                 where k.cr_account_id == _deposit.cr_account_id & k.delete_id == 0
                        //                 select k).FirstOrDefault();
                        //if (_creditor != null)
                        //{
                        //    _deposit = (from k in _db.CREDITOR_DEPOSITS
                        //                where k.cr_account_id == _creditor.cr_account_id & k.delete_id == 0
                        //                orderby k.deposit_fs_id descending, k.cr_deposit_id
                        //                select k).FirstOrDefault();
                        //}
                        fnn.DeleteCreditorDeposit(_deposit, _db);
                        _db.SaveChanges();
                    }

                });
            }
            return Task.FromResult(_record_deleted);
        }

        public Task<creditorC> EditCreditorOpeningCrBalance(dto_creditor_update_opening_cr_balanceC _dto)
        {
            FleetManager.Shared.Models.creditorC _creditor = null;
           
            if (_dto == null)
            {

                AddErrorMessage("Error", "Error", "Method Not Implemented");
                
            }
            return Task.FromResult(_creditor);
        }

        public Task<List<creditor_depositC>> GetAllCreditorDepositsByMonthPartition(dto_month_partitionC _dto)
        {
            List<creditor_depositC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (_dto.fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where m_partition_id = {1} and delete_id = 0", FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(), _dto.m_partition_id);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where m_partition_id = {1} and fs_timestamp > {2}",  FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(), _dto.m_partition_id, _dto.fs_timestamp);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }

                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_list);
        }

        public Task<List<creditor_depositC>> GetAllCreditorDepositsByRange(dto_fs_rangeC _dto)
        {
            List<creditor_depositC> _list = null;
            if (_dto == null)
            {

                AddErrorMessage("Error", "Error", "DTO Can Not Be Null");
                return Task.FromResult(_list);
            }

            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (_dto.fs_time_stamp == 0)
                    {
                        _sql = string.Format("select * from {0} where  (deposit_fs_id>={1} and  deposit_fs_id <= {2}) and delete_id = 0",
                                                             FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(), _dto.start_fs_id, _dto.end_fs_id);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where (deposit_fs_id >={1} and  deposit_fs_id <= {2}) and fs_timestamp >{3}",
                                                             FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(), _dto.start_fs_id, _dto.end_fs_id, _dto.fs_time_stamp);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }

                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_list);
        }

        public Task<List<creditorC>> GetAllCreditors(long fs_timestamp)
        {
            List<creditorC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0",  FleetManager.DbBase.tableNames.creditor_tb.ToDbSchemaTable());
                        _list = _db.Query<creditorC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}",  FleetManager.DbBase.tableNames.creditor_tb.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<creditorC>(_sql).ToList();
                    }

                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_list);
        }

        public Task<List<creditor_depositC>> GetCreditorDepositsByCreditorId(int cr_account_id, long fs_timestamp)
        {
            List<creditor_depositC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where cr_account_id={1} and delete_id = 0", 
                             FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(), cr_account_id);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where cr_account_id={1} and fs_timestamp > {2}",
                             FleetManager.DbBase.tableNames.creditor_deposit_tb.ToDbSchemaTable(),cr_account_id, fs_timestamp);
                        _list = _db.Query<creditor_depositC>(_sql).ToList();
                    }

                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_list);
        }

        public Task<creditorC> GetCreditorSingle(int cr_account_id)
        {
            creditorC _obj = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select TOP(1) * from {0} where cr_account_id={1} and delete_id = 0",  FleetManager.DbBase.tableNames.creditor_tb.ToDbSchemaTable(), cr_account_id);
                    _obj = _db.Query<creditorC>(_sql).FirstOrDefault();

                }
            }
            catch (SqlException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_obj);
        }

        public Task<List<creditorC>> GetAllCreditorsBB(long fs_timestamp, long kapoge, string vinka)
        {
            throw new NotImplementedException();
        }
    }
}
