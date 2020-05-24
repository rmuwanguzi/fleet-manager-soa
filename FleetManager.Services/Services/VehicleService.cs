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
    public class VehicleService : IVehicleService
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
        public VehicleService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();
        }
        public string _table_name =  FleetManager.DbBase.tableNames.vehicle_tb;

        public Task<vehicleC> Add(dto_vehicle_newC _dto)
        {
            vehicleC _obj = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_create_vehicle))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_obj);
            }

            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Vehicle Object Is Null");
                return Task.FromResult(_obj);
            }
            if(_dto.vehicle_cat_id  == 0)
            {
                AddErrorMessage("Error", "Error", "No Vehicle Category Selected");
                return Task.FromResult(_obj);
            }
            if(string.IsNullOrEmpty(_dto.vehicle_plate_no))
            {
                AddErrorMessage("Error", "Error", "Plate Number Is Missing");
                return Task.FromResult(_obj);
            }
           
            try{
                using (var _db = fnn.GetDbContext())
                {
                    //var _driver = _db.DRIVERS.Where(e => e.driver_id == _dto.driver_id & e.delete_id == 0).FirstOrDefault();
                    //if (_driver == null)
                    //{
                    //    AddErrorMessage("Error", "Error", "Unable To Find Driver Object");
                    //    return Task.FromResult(_obj);
                    //}
                    _obj = new vehicleC()
                    {
                        is_hired_id = _dto.is_hired_id,
                        driver_id = _dto.driver_id,
                        vehicle_cat_id = _dto.vehicle_cat_id,
                        vehicle_plate_no = _dto.vehicle_plate_no.Trim(),
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                        hire_amount = _dto.hire_amount,
                        vh_owner_id = _dto.vh_owner_id,
                        vh_purpose_type_id=em.vehicle_purpose_typeS.field_work.ToInt32()
                    };
                    _db.VEHICLES.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Plate Number");
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
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_delete_vehicle))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_record_deleted);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    var vehicle = _db.VEHICLES.Where(e => e.vehicle_id == id & e.delete_id == 0).FirstOrDefault();
                    if (vehicle == null)
                    {
                        AddErrorMessage("Error", "Error", "Unable To Find Vehicle Object");
                        _record_deleted = false;
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { _table_name.ToDbSchemaTable() },
                            DbHelper.GetDbSchema(), new string[] { "vehicle_id" }, id);
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
                                pk_col_name = "vehicle_id",
                                pk_id = vehicle.vehicle_id,
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
        public Task<List<vehicleC>> GetAll(long fs_timestamp)
        {
            List<vehicleC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<vehicleC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<vehicleC>(_sql).ToList();
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
        public Task<vehicleC> GetSingle(int id)
        {
            vehicleC _item = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    _sql = string.Format("select * from {0} where vehicle_id = {1} and delete_id = 0", _table_name.ToDbSchemaTable());
                    _item = _db.Query<vehicleC>(_sql).FirstOrDefault();
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
        public Task<vehicleC> Update(dto_vehicle_updateC _dto)
        {
            vehicleC _existing = null;
            if (fnn.LOGGED_USER.DoesNotHaveRight(em.user_rights_types.can_edit_vehicle))
            {

                AddErrorMessage("Limited Rights Error", "Rights Error", "You Can Not Perform This Operation");
                return Task.FromResult(_existing);
            }
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Vehicle Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.vehicle_id == 0)
            {
                AddErrorMessage("Update Error", "Save Error", "Vehicle Id Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            //if(_dto.driver_id == 0)
            //{
            //    AddErrorMessage("Error", "Error", "Driver Id Is Missing");
            //    return Task.FromResult(_existing);
            //}
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.VEHICLES.Where(e => e.vehicle_id == _dto.vehicle_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Vehicle Object");
                        return Task.FromResult(_existing);
                    }
                    if(_existing.vehicle_plate_no.ToLower()!=_dto.vehicle_plate_no.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "vehicle_plate_no",
                            new_col_value = _dto.vehicle_plate_no.Trim().ToUpper(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "vehicle_id",
                            pk_id = _dto.vehicle_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Vehicle Plate No Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.VEHICLES.Where(e => e.vehicle_id == _dto.vehicle_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _trans.Context.VEHICLES.AddOrUpdateExtension(_existing);
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
