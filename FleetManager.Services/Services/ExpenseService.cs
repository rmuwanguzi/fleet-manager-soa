using Dapper;
using MutticoFleet.Service;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.Models;
using Newtonsoft.Json;
using Service;
using FleetManager.Shared.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

using System.Threading.Tasks;
using FleetManager.Shared;

namespace MutticoFleet.Services
{
    public class ExpenseService : IExpenseService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        private string _table_name =  FleetManager.DbBase.tableNames.expense_trans_tb;
        public ExpenseService(IMessageDialog dialog, ILoggedInUserService l_user)
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

       
        public Task<bool> Delete(int id)
        {
            bool _record_deleted = false;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_expense_trans))
            {
               
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var expense = _db.EXPENSE_TRANSACTIONS.Where(e => e.expense_id == id & e.delete_id == 0).FirstOrDefault();
                    if (expense == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Expense Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                        {
                            pk_col_name = "expense_id",
                            pk_id = expense.expense_id,
                            table_name = DbHelper.GetTableSchemaName(_table_name)
                        }, _db);
                        if (_result == null || _result == false)
                        {
                            AddErrorMessage("Delete Error", "Delete Error", "Error Encountered While Trying To Delete Record");
                            _record_deleted = false;
                        }
                        else
                        {
                            _record_deleted = true;
                            _db.SaveChanges();
                            //delete extra_trans_field
                            string _sql = string.Format(" update {0} set delete_id = @v1, delete_fs_date = @v2, fs_timestamp = @v3, delete_user_id = @v4, delete_fs_id = @v5 where exp_type_id = {1} and delete_id = 0 ");
                            _db.Database.Connection.Execute(_sql, new
                            {
                                v1 = 0,
                                v2 = fnn.GetServerDate(),
                                v3 = fnn.GetUnixTimeStamp(),
                                v4 = m_logged_user.user_id
                            }, _db.Database.GetDbTransaction());
                            _db.SaveChanges();
                            return Task.FromResult(_record_deleted);
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
            return Task.FromResult(_record_deleted);
        }
        public Task<List<vh_expense_transC>> GetAll(long fs_timestamp)
        {
            List<vh_expense_transC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
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
        public Task<vh_expense_transC> GetSingle(int id)
        {
            vh_expense_transC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where expense_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable());
                    _item = _db.Query<vh_expense_transC>(_sql).FirstOrDefault();
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
            return Task.FromResult(_item);
        }
        public Task<List<vh_expense_transC>> GetExpensesByExpenseType(dto_fs_rangeC _dto)
        {
            List<vh_expense_transC> _list = null;
            if(_dto == null)
            {

                AddErrorMessage("Error", "Error", "Expense Object Can Not Be Null");
                return Task.FromResult(_list);
            }
            if(_dto.filter_id == 0)
            {
                AddErrorMessage("Error", "Error", "Filter Id Can Not Be Null");
                return Task.FromResult(_list);
            }
          
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (_dto.fs_time_stamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where exp_type_id = {1} and fs_timestamp > {2} and fs_timestamp < {3}  and delete_id = 0",
                                                            _table_name.ToDbSchemaTable(), _dto.filter_id, _dto.start_fs_id, _dto.end_fs_id);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
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
        public Task<List<vh_expense_transC>> GetAllExpensesRange(dto_fs_rangeC _dto)
        {
            List<vh_expense_transC> _list = null;
            if (_dto == null)
            {

                AddErrorMessage("Error", "Error", "Expense Object Can Not Be Null");
                return Task.FromResult(_list);
            }
            
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (_dto.fs_time_stamp == 0)
                    {
                        _sql = string.Format("select * from {0} where  (expense_fs_id >={1} and  expense_fs_id <= {2}) and delete_id = 0",
                                                            _table_name.ToDbSchemaTable(), _dto.start_fs_id, _dto.end_fs_id);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where (expense_fs_id >={1} and  expense_fs_id <= {2}) and fs_timestamp >{3}",
                                                            _table_name.ToDbSchemaTable(), _dto.start_fs_id, _dto.end_fs_id, _dto.fs_time_stamp);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
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
        public Task<List<vh_expense_transC>> GetAllExpensesByMonthPartition(dto_month_partitionC _dto)
        {

            List<vh_expense_transC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (_dto.fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where m_partition_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable(), _dto.m_partition_id);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where m_partition_id = {1} and fs_timestamp > {2}", _table_name.ToDbSchemaTable(), _dto.m_partition_id, _dto.fs_timestamp);
                        _list = _db.Query<vh_expense_transC>(_sql).ToList();
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
        public Task<List<vh_expense_transC>> AddExpense(dto_expense_trans_voucher_newC _dto)
        {
            List<vh_expense_transC> _list = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_vehicle_expense))
            {
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_list);
            }

            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "Expense Object Can Not Be Null");
                return Task.FromResult(_list);
            }
            if (_dto.vehicle_id > 0)
            {
                if (_dto.driver_id == 0)
                {
                    AddErrorMessage("Error", "Error", "No Driver Attached ");
                    return Task.FromResult(_list);
                }
            }
            if (_dto.expense_date == null)
            {
                AddErrorMessage("Error", "Error", "Expense Date Is Missing");
                return Task.FromResult(_list);
            }
            if(string.IsNullOrEmpty(_dto.voucher_no))
            {
                AddErrorMessage("Error", "Error", "Voucher No  Cannot Be Empty");
                return Task.FromResult(_list);
            }
            try
            {
                string _sql = null;
                using (var _trans = new ZUpdateContext())
                {
                    var _driver = _trans.Context.DRIVERS.Where(e => e.driver_id == _dto.driver_id & e.delete_id == 0).FirstOrDefault();
                    var _vehicle = _trans.Context.VEHICLES.Where(e => e.vehicle_id == _dto.vehicle_id & e.delete_id == 0).FirstOrDefault();
                    if (_vehicle == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Vehicle Object");
                        _trans.RollBack();
                        return Task.FromResult(_list);
                    }
                    if (_vehicle.vehicle_id > 0)
                    {
                        if (_driver == null & _vehicle.is_hired_id < 2)
                        {
                            AddErrorMessage("Error", "Error", "Unable To Find driver Object");
                            _trans.RollBack();
                            return Task.FromResult(_list);
                        }
                    }
                    vh_expense_trans_voucherC _voucher = new vh_expense_trans_voucherC()
                    {
                        driver_id = _dto.driver_id,
                        driver_name = _driver == null ? null : _driver.driver_name.Trim(),
                        expense_date = _dto.expense_date.Value,
                        expense_fs_id = fn.GetFSID(_dto.expense_date.Value),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        server_edate = fnn.GetServerDate(),
                        vehicle_id = _dto.vehicle_id,
                        vehicle_plate_no = _vehicle.vehicle_plate_no.Trim(),
                        created_by_user_id = m_logged_user.user_id,
                        m_partition_id = string.Format("{0}{1}", _dto.expense_date.Value.Year, _dto.expense_date.Value.Month).ToInt32(),
                        voucher_no = _dto.voucher_no,
                        user_id = m_logged_user.user_id,
                        user_name = m_logged_user.user_name,
                        voucher_total_amount = _dto.expense_trans_collection.Sum(g => g.expense_amount),
                        


                    };
                    _trans.Context.EXPENSE_TRANSACTIONS_VOUCHERS.Add(_voucher);
                    var _retVal = _trans.Context.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Expense Transaction");
                        _list = null;
                        _trans.RollBack();
                        return Task.FromResult(_list);
                    }
                    vh_expense_typeC _exp_type_obj = null;
                    _list = new List<vh_expense_transC>();
                    List<vh_expense_trans_fieldC> extra_field_list = new List<vh_expense_trans_fieldC>();
                            

                    foreach (var _e in _dto.expense_trans_collection)
                    {
                       _exp_type_obj= _trans.Context.EXPENSE_TYPES.Where(e => e.exp_type_id == _e.exp_type_id & e.delete_id == 0).FirstOrDefault();
                        if (_vehicle.vehicle_id > 0)
                        {
                            switch (_exp_type_obj.exp_cat)
                            {
                                case em.vehicle_expense_categoryS.fuel:
                                case em.vehicle_expense_categoryS.vehicle_hire:
                                    {
                                        _sql = string.Format("select count(expense_id) from {0} where expense_fs_id=@v1 and " +
                                            "vehicle_id=@v2 and exp_type_id=@v3 and delete_id=0",
                                              FleetManager.DbBase.tableNames.expense_trans_tb.ToDbSchemaTable());
                                        object _db_obj_result = _trans.Context.Database.Connection.ExecuteScalar(_sql,
                                             new
                                             {
                                                 v1 = fn.GetFSID(_voucher.expense_date.Value),
                                                 v2 = _e.vehicle_id,
                                                 v3 = _exp_type_obj.exp_type_id
                                             }, _trans.Context.Database.GetDbTransaction());
                                        if (_db_obj_result != null && _db_obj_result.ToInt32() > 0)
                                        {
                                            if(_exp_type_obj.exp_cat==em.vehicle_expense_categoryS.vehicle_hire)
                                            {
                                                AddErrorMessage("Duplicate Vehicle Hire", "Duplicate Vehicle Hire", "You Have Already Made a Vehicle Hire Entry For This Vehicle On The Selected Date");
                                            }
                                            else
                                            {
                                                AddErrorMessage("Duplicate Fuel Entry", "Duplicate Fuel Entry", "You Have Already Made a Fuel Entry For This Vehicle On The Selected Date");
                                            }
                                            _list = null;
                                            _trans.RollBack();
                                            return Task.FromResult(_list);
                                        }

                                        break;
                                    }
                            }
                        }
                        var _exp_trans = new vh_expense_transC()
                        {
                            exp_type_id = _e.exp_type_id,
                            exp_type_name = _exp_type_obj.exp_type_name,
                            driver_id = _dto.driver_id,
                            driver_name = _driver == null ? null : _driver.driver_name,
                            expense_amount = _e.expense_amount,
                            expense_date = _voucher.expense_date.Value,
                            expense_details = _e.expense_details,
                            expense_fs_id = fn.GetFSID(_voucher.expense_date.Value),
                            exp_type_cat_id = _exp_type_obj.exp_cat_id,
                            fs_timestamp = fnn.GetUnixTimeStamp(),
                            server_edate = fnn.GetServerDate(),
                            vehicle_id = _dto.vehicle_id,
                            vehicle_cat_id = _vehicle.vehicle_cat_id,
                            vehicle_plate_no = _vehicle.vehicle_plate_no.Trim(),
                            created_by_user_id = m_logged_user.user_id,
                            repair_action_type_id = _e.repair_action_type_id,
                            repair_action_type_name = _e.repair_action_type_name,
                            m_partition_id = string.Format("{0}{1}", _dto.expense_date.Value.Year, _dto.expense_date.Value.Month).ToInt32(),
                            user_id = m_logged_user.user_id,
                            user_name = m_logged_user.user_name,
                            voucher_id = _voucher.voucher_id,
                            voucher_no = _voucher.voucher_no,
                            project_id = _e.project_id,
                            project_name = _e.project_name,
                            team_leader_id = _e.team_leader_id,
                            team_leader_name = _e.team_leader_name,
                            vh_owner_id = _e.vh_owner_id,
                            vh_owner_name = _e.vh_owner_name,
                            fuel_station_id=_e.fuel_station_id,
                            fuel_station_name=_e.fuel_station_name,
                            mechanic_id=_e.mechanic_id,
                            mechanic_name=_e.mechanic_name,
                           
                        };
                        if (_vehicle.is_hired_id == 2 | _vehicle.vehicle_id == -111)
                        {
                            _exp_trans.vehicle_cat_id = _e.vehicle_cat_id;
                            _exp_trans.vehicle_plate_no = _e.vehicle_plate_no;
                        }
                        _trans.Context.EXPENSE_TRANSACTIONS.Add(_exp_trans);
                        _trans.Context.SaveChanges();
                        _list.Add(_exp_trans);
                        if (_e.extra_fields_collection != null && _e.extra_fields_collection.Count > 0)
                        {
                            vh_expense_trans_fieldC _ex_field_obj = null;
                            foreach (var k in _e.extra_fields_collection)
                            {
                                _ex_field_obj = new vh_expense_trans_fieldC()
                                {
                                    created_by_user_id = m_logged_user.user_id,
                                    expense_id = _exp_trans.expense_id,
                                    server_edate = fnn.GetServerDate(),
                                    fs_timestamp = fnn.GetUnixTimeStamp(),
                                    exp_type_id = _exp_trans.exp_type_id,
                                    et_field_id = k.et_field_id,
                                    et_field_item_id = k.et_field_item_id,
                                    field_name = k.field_name,
                                    field_value = k.field_value,
                                    voucher_id=_voucher.voucher_id
                                };
                                _trans.Context.EXPENSE_TRANSACTIONS_FIELDS.Add(_ex_field_obj);
                                _trans.Context.SaveChanges();
                                extra_field_list.Add(_ex_field_obj);
                            }
                            string _json_data = JsonConvert.SerializeObject(extra_field_list);
                            _sql = string.Format("update {0} set json_extra_fields_info = @v1," +
                                " fs_timestamp = @v2 where expense_id = {1} and delete_id = 0 ",
                                _table_name.ToDbSchemaTable(), _exp_trans.expense_id);
                            _trans.Context.Database.Connection.Execute(_sql, new
                            {
                                v1 = _json_data,
                                v2 = fnn.GetUnixTimeStamp(),
                            }, _trans.Context.Database.GetDbTransaction());
                            _exp_trans.json_extra_fields_info = _json_data;
                            _trans.Context.SaveChanges();
                        }
                     }
                    _trans.Context.SaveChanges();
                    _trans.Commit();
      
                }

            }
            catch (SqlException ex)
            {
                _list = null;
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                _list = null;
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                _list = null;
                LoggerX.LogException(ex);

            }
            if(_list!=null)
            {
                Task.Run(() =>
                {
                    fnn.CreateCreditorInvoice(_list);
                   
                });
            }
            return Task.FromResult(_list);
        }
        public Task<bool> DeleteExpenseVoucher(int voucher_id)
        {
            bool _record_deleted = false;
            vh_expense_trans_voucherC _voucher = null;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _voucher = _db.EXPENSE_TRANSACTIONS_VOUCHERS.Where(e => e.voucher_id == voucher_id & e.delete_id == 0).FirstOrDefault();
                    if (_voucher == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Expense Voucher Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                        {
                            pk_col_name = "voucher_id",
                            pk_id = _voucher.voucher_id,
                            table_name = DbHelper.GetTableSchemaName( FleetManager.DbBase.tableNames.expense_trans_voucher_tb)
                        }, _db);
                        if (_result == null || _result == false)
                        {
                            AddErrorMessage("Delete Error", "Delete Error", "Error Encountered While Trying To Delete Record");
                            _record_deleted = false;
                        }
                        else
                        {
                            _record_deleted = true;
                            _db.SaveChanges();

                        }
                    }
                }
                if (_record_deleted)
                {
                    Task.Run(() =>
                    {
                        using (var _db = fnn.GetDbContext())
                        {
                            var _expense_trans_list = (from k in _db.EXPENSE_TRANSACTIONS
                                                       where k.voucher_id == _voucher.voucher_id & k.delete_id == 0
                                                       select k).ToList();
                            foreach (var _e in _expense_trans_list)
                            {
                                var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                                {
                                    pk_col_name = "expense_id",
                                    pk_id = _e.expense_id,
                                    table_name = DbHelper.GetTableSchemaName( FleetManager.DbBase.tableNames.expense_trans_tb)
                                }, _db);
                                if (_result == null || _result == false)
                                {
                                    throw new Exception("Error Encountered Trying To Delete Expense Object");
                                }
                                else
                                {
                                    _db.SaveChanges();

                                }
                            }

                            var _expense_trans_field_list = (from k in _db.EXPENSE_TRANSACTIONS_FIELDS
                                                             where k.voucher_id == _voucher.voucher_id & k.delete_id == 0
                                                             select k).ToList();

                            foreach (var _e in _expense_trans_field_list)
                            {
                                var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                                {
                                    pk_col_name = "un_id",
                                    pk_id = _e.un_id,
                                    table_name = DbHelper.GetTableSchemaName( FleetManager.DbBase.tableNames.expense_trans_field_tb)
                                }, _db);
                                if (_result == null || _result == false)
                                {
                                    throw new Exception("Error Encountered Trying To Delete Expense Object");
                                }
                                else
                                {
                                    _db.SaveChanges();

                                }
                            }
                            fnn.DeleteCreditorInvoice(_expense_trans_list, _db);

                            _db.SaveChanges();
                        }

                    });
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
            return Task.FromResult(_record_deleted);
        }
        public Task<List<vh_expense_transC>> GetAllRecentVehicleExpenses(dto_vehicle_expense_recentC _dto)
        {
            List<vh_expense_transC> _list = null;
            if (_dto == null)
            {
                 AddErrorMessage("Error", "Error", "DTO Object Can Not Be Null");
                return Task.FromResult(_list);
            }
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select TOP 15 * from {0} where vehicle_id={1} and exp_type_id={2} and delete_id = 0 order by expense_fs_id desc",
                                                            _table_name.ToDbSchemaTable(), _dto.vehicle_id, _dto.exp_type_id);
                    _list = _db.Query<vh_expense_transC>(_sql).ToList();

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
    }
}
