using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using Service;
using FleetManager.Shared.Core;
using MutticoFleet.Service;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;
using FleetManager.Shared;

namespace MutticoFleet.Services
{
    public class ExpenseTypeFieldItemService : IExpenseTypeFieldItemService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        private string table_name =  FleetManager.DbBase.tableNames.exp_type_field_item_tb;

        public ExpenseTypeFieldItemService(IMessageDialog dialog, ILoggedInUserService l_user)
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

        public Task<vh_expense_type_field_d_itemC> AddExpenseTypeFieldItem(dto_expense_type_field_item_newC _dto)
        {
            vh_expense_type_field_d_itemC _obj = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_expense_type_field_item))
            {
                _obj = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_obj);
            }
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "Field Item Is Null");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.et_field_item_name))
            {
                AddErrorMessage("Error", "Error", "Field Item Name Is Missing");
                return Task.FromResult(_obj);
            }
            if (_dto.exp_type_id == 0)
            {
                AddErrorMessage("Error", "Error", "Expense Type Not Selected");
                return Task.FromResult(_obj);
            }
            if (_dto.et_field_id == 0)
            {
                AddErrorMessage("Error", "Error", "Extra Field Id Is MIssing");
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var exp_type = _db.EXPENSE_TYPES.Where(e => e.exp_type_id == _dto.exp_type_id & e.delete_id == 0).FirstOrDefault();
                    var field = _db.EXPENSE_TYPE_FIELDS.Where(e => e.et_field_id == _dto.et_field_id & e.delete_id == 0).FirstOrDefault();
                    if (exp_type == null)
                    {
                        AddErrorMessage("Error", "Error", "Expense Type Object Can Not Be Found");
                        return Task.FromResult(_obj);
                    }
                    if (field == null)
                    {
                        AddErrorMessage("Error", "Error", "Expense Extra Feild Object Can Not Be Found");
                        return Task.FromResult(_obj);
                    }
                    _obj = new vh_expense_type_field_d_itemC()
                    {
                        et_field_item_name = _dto.et_field_item_name.Trim(),
                        exp_type_id = _dto.exp_type_id,
                        et_field_id = _dto.et_field_id,
                        et_field_name = field.et_field_name.Trim(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        server_edate = fnn.GetServerDate(),
                        created_by_user_id = m_logged_user.user_id
                    };
                    _db.EXPENSE_FIELD_TYPE_ITEMS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Field Item Name");
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
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_expense_type_field_item))
            {
                
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var exp_type = _db.EXPENSE_FIELD_TYPE_ITEMS.Where(e => e.et_field_item_id == id & e.delete_id == 0).FirstOrDefault();
                    if (exp_type == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Expense Type Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(table_name) },
                            DbHelper.GetDbSchema(), new string[] { "et_field_item_id" }, id);
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
                                pk_col_name = "et_field_item_id",
                                pk_id = exp_type.et_field_item_id,
                                table_name = DbHelper.GetTableSchemaName(table_name)
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
        public Task<List<vh_expense_type_field_d_itemC>> GetAll(long fs_timestamp)
        {
            List<vh_expense_type_field_d_itemC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_expense_type_field_d_itemC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vh_expense_type_field_d_itemC>(_sql).ToList();
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
        public Task<vh_expense_type_field_d_itemC> GetSingle(int id)
        {
            vh_expense_type_field_d_itemC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where et_field_item_id = {1} and delete_id = 0", table_name.ToDbSchemaTable(), id);
                    _item = _db.Query<vh_expense_type_field_d_itemC>(_sql).FirstOrDefault();
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
        public Task<vh_expense_type_field_d_itemC> UpdateExpenseTypeFieldItem(dto_expense_type_field_item_updateC _dto)
        {
            vh_expense_type_field_d_itemC _existing = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_expense_type_field_item))
            {
                _existing = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_existing);
            }
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error",  "Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.et_field_item_id == 0)
            {
                AddErrorMessage("Update Error", "Save Error", "Expense Type Field Item Id Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.et_field_item_name))
            {
                AddErrorMessage("Error", "Error", "Expense Type Field Name Is Missing");
                return Task.FromResult(_existing);
            }
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.EXPENSE_FIELD_TYPE_ITEMS.Where(e => e.et_field_item_id == _dto.et_field_item_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Vehicle Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.et_field_item_name.ToLower() != _dto.et_field_item_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "et_field_item_name",
                            new_col_value = _dto.et_field_item_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(table_name),
                            pk_col_name = "et_field_item_id",
                            pk_id = _dto.et_field_item_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Expense Type Field Item Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.EXPENSE_FIELD_TYPE_ITEMS.Where(e => e.et_field_item_id == _dto.et_field_item_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }

                    SimpleMapper.PropertyMap(_dto, _existing);
                   // _existing.fs_timestamp = fnn.GetUnixTimeStamp();
                    _trans.Context.EXPENSE_FIELD_TYPE_ITEMS.AddOrUpdateExtension(_existing);
                    _trans.Context.SaveChanges();
                    _trans.Commit();
                    return Task.FromResult(_existing);

                }
            }
            catch (SqlException ex)
            {
                _existing = null;
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                _existing = null;
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                _existing = null;
                LoggerX.LogException(ex);

            }
            return Task.FromResult(_existing);
        }
    }
}
