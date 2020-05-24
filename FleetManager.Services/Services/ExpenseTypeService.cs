using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetManager.Shared.dto;
using Service;
using FleetManager.Shared.Core;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;
using MutticoFleet.Service;
using FleetManager.Shared.Models;
using FleetManager.Shared;

namespace MutticoFleet.Services
{
    public class ExpenseTypeService : IExpenseTypeService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        private string _table_name =  FleetManager.DbBase.tableNames.exp_type_tb;
        public ExpenseTypeService(IMessageDialog dialog, ILoggedInUserService l_user)
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

        public Task<vh_expense_typeC> Add(dto_expense_type_newC _dto)
        {
            vh_expense_typeC  _obj = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_expense_type_field))
            {
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_obj);
            }

            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Expense Object Is Null");
                return Task.FromResult(_obj);
            }
            if (_dto.exp_cat_id  == 0)
            {
                AddErrorMessage("Error", "Error", "No Expense Category Selected");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.exp_type_name))
            {
                AddErrorMessage("Error", "Error", "Expense Type Name Is Missing");
                return Task.FromResult(_obj);
            }
           
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new vh_expense_typeC()
                    { 

                    exp_cat_id = _dto.exp_cat_id,
                    exp_type_name = _dto.exp_type_name.Trim(),
                    server_edate = fnn.GetServerDate(),
                    fs_timestamp = fnn.GetUnixTimeStamp(),
                    created_by_user_id = m_logged_user.user_id,
                    
                    };
                    _db.EXPENSE_TYPES.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Expense Type  Name");
                        _obj = null;
                        return Task.FromResult(_obj);
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
            return Task.FromResult(_obj);
        }
        public Task<bool> Delete(int id)
        {
            bool _record_deleted = false;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_expense_type))
            {
                _record_deleted = false;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var exp_type = _db.EXPENSE_TYPES.Where(e => e.exp_type_id == id & e.delete_id == 0).FirstOrDefault();
                    if (exp_type == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Expense Type Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        if (exp_type.exp_type_id < 0)
                        {
                            AddErrorMessage("Update Error", "Update Error", "Cannot Delete A System Defined Expense Type");
                            _record_deleted = false;
                            return Task.FromResult(_record_deleted);
                        }

                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) }, DbHelper.GetDbSchema(), new string[] { "exp_type_id" }, id);
                        if (_has_dependency == null || _has_dependency == true)
                        {
                            AddErrorMessage("Delete Error", "Delete Error", "Unable To Delete Record Because It Has System Dependencies.");
                            _record_deleted = false;
                            return Task.FromResult(_record_deleted);
                        }
                        else
                        {
                            var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                            {
                                pk_col_name = "exp_type_id",
                                pk_id = exp_type.exp_type_id,
                                table_name = DbHelper.GetTableSchemaName(_table_name)
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
                                return Task.FromResult(_record_deleted);
                            }
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
        public Task<List<vh_expense_typeC>> GetAll(long fs_timestamp)
        {
            List<vh_expense_typeC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_expense_typeC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vh_expense_typeC>(_sql).ToList();
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
        public Task<vh_expense_typeC> GetSingle(int id)
        {
            vh_expense_typeC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where exp_type_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable());
                    _item = _db.Query<vh_expense_typeC>(_sql).FirstOrDefault();
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
        public Task<vh_expense_typeC> Update(dto_expense_type_updateC _dto)
        {
          
            vh_expense_typeC _existing = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_edit_expense_type))
            {
                _existing = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_existing);
            }

            if (string.IsNullOrEmpty(_dto.exp_type_name))
            {
                AddErrorMessage("Update Error", "Save Error", "Expense Type  Name Is Null!");              
                return Task.FromResult(_existing);
            }
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.EXPENSE_TYPES.Where(e => e.exp_type_id == _dto.exp_type_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Expense Type Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.exp_type_id < 0)
                    {
                        AddErrorMessage("Update Error", "Update Error", "Cannot Update A System Defined Expense Type");
                        return Task.FromResult(_existing);
                    }

                    if (_existing.exp_type_name.ToLower() != _dto.exp_type_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "exp_type_name",
                            new_col_value = _dto.exp_type_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "exp_type_id",
                            pk_id = _dto.exp_type_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Expense Type Name Already Exists");
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.EXPENSE_TYPES.Where(e => e.exp_type_id == _dto.exp_type_id & e.delete_id == 0).FirstOrDefault();
                           
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _trans.Context.EXPENSE_TYPES.AddOrUpdateExtension(_existing);
                    _trans.Context.SaveChanges();
                    _trans.Commit();
                    return Task.FromResult(_existing);
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
            return Task.FromResult(_existing);
        }
    }
}
