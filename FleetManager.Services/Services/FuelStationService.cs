using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FleetManager.Shared;
using FleetManager.Shared.Core;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.Models;

namespace MutticoFleet.Service
{
    public class FuelStationService : FleetManager.Shared.Interfaces.IFuelStationService
    {

        public string controller_key { get; set; }
        private IMessageDialog _dialog;
        private dto_logged_user m_logged_user { get; set; }
        private string _table_name =  FleetManager.DbBase.tableNames.fuel_station_tb;
        public FuelStationService(IMessageDialog dialog, ILoggedInUserService l_user)
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
        public Task<fuel_stationC> Add(dto_fuel_station_newC _dto)
        {
            fuel_stationC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Fuel Station Is Null");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.fuel_station_name))
            {
                AddErrorMessage("Error", "Error", "Fuel Station Name Is Missing");
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new fuel_stationC()
                    {
                        fuel_station_name = _dto.fuel_station_name.Trim(),
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                    };
                    _db.FUEL_STATIONS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Fuel Station Name");
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    fnn.CreateCreditor(_obj.fuel_station_id, _obj.fuel_station_name, string.Empty, em.creditor_typeS.fuel_station, _db);

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
            fuel_stationC _existing = null;
            bool _record_deleted = false;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _existing = _db.FUEL_STATIONS.Where(e => e.fuel_station_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_existing == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Fuel Station Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) },
                            DbHelper.GetDbSchema(), new string[] { "fuel_station_id" }, id);
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
                                pk_col_name = "fuel_station_id",
                                pk_id = _existing.fuel_station_id,
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
            if(_record_deleted)
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

        public Task<List<fuel_stationC>> GetAll(long fs_timestamp)
        {
            List<fuel_stationC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<fuel_stationC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<fuel_stationC>(_sql).ToList();
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

        public Task<fuel_stationC> Update(dto_fuel_station_updateC _dto)
        {
            fuel_stationC _existing = null;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Fuel Station Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.fuel_station_id == 0)
            {
                AddErrorMessage("Error", "Error", "Fuel Station Id Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.fuel_station_name))
            {
                AddErrorMessage("Error", "Error", "Fuel Station Name Is Missing");
                return Task.FromResult(_existing);
            }
           
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.FUEL_STATIONS.Where(e => e.fuel_station_id == _dto.fuel_station_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Fuel Station Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.fuel_station_name.ToLower() != _dto.fuel_station_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "fuel_station_name",
                            new_col_value = _dto.fuel_station_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "fuel_station_id",
                            pk_id = _dto.fuel_station_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Fuel Station Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.FUEL_STATIONS.Where(e => e.fuel_station_id == _dto.fuel_station_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    if (_existing.cr_account_id > 0)
                    {
                        string _sql = string.Format("update {0} set cr_account_name=@v1,cr_phone_no=@v2,fs_timestamp=@v3 where cr_account_id=@v4 and delete_id=0",
                             FleetManager.DbBase.tableNames.creditor_tb.ToDbSchemaTable());
                        _trans.Context.Database.Connection.Execute(_sql, new
                        {
                            v1 = _existing.fuel_station_name,
                            v2 = string.Empty,
                            v3 = fnn.GetUnixTimeStamp(),
                            v4 = _existing.cr_account_id
                        }, _trans.Context.Database.GetDbTransaction());
                        _trans.Context.SaveChanges();
                    }

                    _trans.Context.FUEL_STATIONS.AddOrUpdateExtension(_existing);
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
