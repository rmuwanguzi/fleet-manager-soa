using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DriverService : IDriverService
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
        public DriverService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();
        }
        public string _table_name =  FleetManager.DbBase.tableNames.driver_tb;
        public Task<vh_driverC> Add(dto_driver_newC _dto)
        {
            vh_driverC _obj = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_vehicle_driver))
            {
                _obj = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_obj);
            }
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Driver Object Is Null");
                return Task.FromResult(_obj);
            }
           if (string.IsNullOrEmpty(_dto.driver_name))
            {
                AddErrorMessage("Error", "Error", "Driver Name Is Missing");
                return Task.FromResult(_obj);
            }
           try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new vh_driverC()
                    {
                        driver_name = _dto.driver_name.Trim(),
                        server_edate = fnn.GetServerDate(),
                        driver_phone_no = _dto.driver_phone_no,
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                        renew_expiry_date = _dto.renew_expiry_date,
                        renew_issue_date = _dto.renew_issue_date,
                        permit_no = _dto.permit_no
                    };
                    _db.DRIVERS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Name");
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
            vh_driverC  _obj = null;
            bool _record_deleted = false;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_vehicle_driver))
            {
                _obj = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = _db.DRIVERS.Where(e => e.driver_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_obj == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Driver Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) }, 
                            DbHelper.GetDbSchema(), new string[] { "driver_id" }, id);
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
                                pk_col_name = "driver_id",
                                pk_id = _obj.driver_id,
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
        public Task<List<vh_driverC>> GetAll(long fs_timestamp)
        {
            List<vh_driverC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_driverC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vh_driverC>(_sql).ToList();
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
        public Task<vh_driverC> GetSingle(int id)
        {
            vh_driverC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where vh_driver_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable());
                    _item = _db.Query<vh_driverC>(_sql).FirstOrDefault();
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
        public Task<vh_driverC> Update(dto_driver_updateC _dto)
        {
            vh_driverC _existing = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_edit_driver_info))
            {
                _existing = null;
                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_existing);
            }
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Driver Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.driver_id == 0)
            {
                AddErrorMessage("Error", "Error", "Driver Id Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.driver_name))
            {
                AddErrorMessage("Update Error", "Save Error", "Driver Name Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.DRIVERS.Where(e => e.driver_id == _dto.driver_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Driver Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.driver_name.ToLower() != _dto.driver_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "driver_name",
                            new_col_value = _dto.driver_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "driver_id",
                            pk_id = _dto.driver_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Driver Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.DRIVERS.Where(e => e.driver_id == _dto.driver_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _existing.fs_timestamp = fnn.GetUnixTimeStamp();
                    _trans.Context.DRIVERS.AddOrUpdateExtension(_existing);
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
