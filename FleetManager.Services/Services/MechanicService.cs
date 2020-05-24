using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;
using FleetManager.Shared.Core;
using MutticoFleet.Service;

namespace MutticoFleet.Services
{
    public class MechanicService : IMechanicService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
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
        public MechanicService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();
        }
        public string _table_name =  FleetManager.DbBase.tableNames.mechanic_tb;

        public Task<mechanicC> Add(dto_mechanic_newC _dto)
        {
            mechanicC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Mechanic Object Is Null");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.mechanic_name))
            {
                AddErrorMessage("Error", "Error", "Mechanic Name Is Missing");
                return Task.FromResult(_obj);
            }
            try
            {
               
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new mechanicC()
                    {
                        mechanic_name = _dto.mechanic_name.Trim(),
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                        
                    };
                    _db.MECHANICS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Mechanic Name");
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    fnn.CreateCreditor(_obj.mechanic_id, _obj.mechanic_name, _obj.phone_no, FleetManager.Shared.em.creditor_typeS.mechanic, _db);
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
            mechanicC _existing = null;
            bool _record_deleted = false;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _existing = _db.MECHANICS.Where(e => e.mechanic_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_existing == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Mechanic Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        if(_existing.mechanic_id==-100)
                        {
                            _record_deleted = false;
                            AddErrorMessage("Delete Error", "Delete Error", "Cannot Delete System Mechanic");
                            return Task.FromResult(_record_deleted);
                        }
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) }, DbHelper.GetDbSchema(), new string[] { "mechanic_id" }, id);
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
                                pk_col_name = "mechanic_id",
                                pk_id = _existing.mechanic_id,
                                table_name = _table_name.ToDbSchemaTable()
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
            if (_record_deleted)
            {
                Task.Run(() =>
                {
                    if (_existing.cr_account_id > 0)
                    {
                        fnn.DeleteCreditor(_existing.cr_account_id);
                    }
                });
            }
            return Task.FromResult(_record_deleted);
        }
        public Task<List<mechanicC>> GetAll(long fs_timestamp)
        {
            List<mechanicC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<mechanicC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<mechanicC>(_sql).ToList();
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
        public Task<mechanicC> Update(dto_mechanic_updateC _dto)
        {
            mechanicC _existing = null;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Mechanic Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.mechanic_id == 0)
            {
                AddErrorMessage("Error", "Error", "Mechanic Id Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.mechanic_name))
            {
                AddErrorMessage("Error", "Error", "Mechanic Name Is Missing");
                return Task.FromResult(_existing);
            }


            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.MECHANICS.Where(e => e.mechanic_id == _dto.mechanic_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Mechanic Object");
                        return Task.FromResult(_existing);
                    }
                    if(_existing.mechanic_id==-100)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Update Error", "Cannot Update System Mechanic");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.mechanic_name.ToLower() != _dto.mechanic_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "mechanic_name",
                            new_col_value = _dto.mechanic_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "mechanic_id",
                            pk_id = _dto.mechanic_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Mechanic Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.MECHANICS.Where(e => e.mechanic_id == _dto.mechanic_id & e.delete_id == 0).FirstOrDefault();
                           
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    if (_existing.cr_account_id > 0)
                    {
                        string _sql = string.Format("update {0} set cr_account_name=@v1,cr_phone_no=@v2,fs_timestamp=@v3 where cr_account_id=@v4 and delete_id=0",
                             FleetManager.DbBase.tableNames.creditor_tb.ToDbSchemaTable());
                        _trans.Context.Database.Connection.Execute(_sql, new
                        {
                            v1 = _existing.mechanic_name,
                            v2 = _existing.phone_no,
                            v3 = fnn.GetUnixTimeStamp(),
                            v4 = _existing.cr_account_id
                        },_trans.Context.Database.GetDbTransaction());
                        _trans.Context.SaveChanges();
                    }
                    _trans.Context.MECHANICS.AddOrUpdateExtension(_existing);
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
