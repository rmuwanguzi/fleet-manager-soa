using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;
using FleetManager.Shared.Interfaces;
using Service;
using FleetManager.Shared.dto;
using FleetManager.Shared.Core;
using MutticoFleet.Service;
using FleetManager.Shared.Models;
using FleetManager.Shared;

namespace MutticoFleet.Services
{
    public class VehicleCatService : IVehicleCatService
    {
        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        private string _table_name =  FleetManager.DbBase.tableNames.vehicle_cat_tb;
        public VehicleCatService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
           // _table_name = _table_name.ToDbSchemaTable();
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

        public Task<vh_categoryC> AddVehicleCategory(dto_vehicle_category_newC _dto)
        {
            vh_categoryC _obj = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_vehicle_category))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_obj);
            }

            if (string.IsNullOrEmpty(_dto.vehicle_cat_name))
            {
                AddErrorMessage("Insert Error", "Save Error", "Vehicle Category Name Is Null");
                _obj = null;
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new vh_categoryC()
                    {
                        vehicle_cat_name = _dto.vehicle_cat_name.Trim().ToProperCase(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        server_edate = fnn.GetServerDate(),
                        created_by_user_id = m_logged_user.user_id
                        };
                    _db.VEHICLE_CATEGORIES.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Cake Category Name");
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
            vh_categoryC _obj = null;
            bool _record_deleted = false;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_vehicle_category))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }

            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = _db.VEHICLE_CATEGORIES.Where(e => e.vehicle_cat_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_obj == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Vehicle Category Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { _table_name.ToDbSchemaTable() },
                            DbHelper.GetDbSchema(), new string[] { "vehicle_cat_id" }, id);
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
                                pk_col_name = "vehicle_cat_id",
                                pk_id = _obj.vehicle_cat_id,
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
        public Task<List<vh_categoryC>> GetAll(long fs_timestamp)
        {
            List<vh_categoryC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vh_categoryC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vh_categoryC>(_sql).ToList();
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
        public Task<vh_categoryC> GetSingle(int id)
        {
            vh_categoryC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where vehicle_cat_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable());
                    _item = _db.Query<vh_categoryC>(_sql).FirstOrDefault();
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
        public Task<vh_categoryC> UpdateVehicleCategory(dto_vehicle_category_updateC   _dto)
        {
            vh_categoryC _existing = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_edit_vehicle_category))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_existing);
            }

            if (string.IsNullOrEmpty(_dto.vehicle_cat_name))
            {
                AddErrorMessage("Update Error", "Save Error", "Vehicle Category Name Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.VEHICLE_CATEGORIES.Where(e => e.vehicle_cat_id == _dto.vehicle_cat_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Vehicle Category Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.vehicle_cat_name.ToLower() != _dto.vehicle_cat_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "vehicle_cat_name",
                            new_col_value = _dto.vehicle_cat_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "vehicle_cat_id",
                            pk_id = _dto.vehicle_cat_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Vechile Category Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.VEHICLE_CATEGORIES.Where(e => e.vehicle_cat_id == _dto.vehicle_cat_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _trans.Context.VEHICLE_CATEGORIES.AddOrUpdateExtension(_existing);
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
