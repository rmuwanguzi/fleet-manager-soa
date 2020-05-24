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
    public class ProjectService : IProjectService
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
        public ProjectService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();
        }
        public string _table_name =  FleetManager.DbBase.tableNames.project_tb;

        public Task<projectC> Add(dto_project_newC _dto)
        {
            projectC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Project Object Is Null");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.project_name))
            {
                AddErrorMessage("Error", "Error", "Project Name Is Missing");
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new projectC()
                    {
                        project_name = _dto.project_name.Trim(),
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                    };
                    _db.PROJECTS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Project  Name");
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
            projectC _obj = null;
            bool _record_deleted = false;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = _db.PROJECTS.Where(e => e.project_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_obj == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Project Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) },
                            DbHelper.GetDbSchema(), new string[] { "project_id" }, id);
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
                                pk_col_name = "project_id",
                                pk_id = _obj.project_id,
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
        public Task<List<projectC>> GetAll(long fs_timestamp)
        {
            List<projectC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<projectC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<projectC>(_sql).ToList();
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
        public Task<projectC> Update(dto_project_updateC _dto)
        {
            projectC _existing = null;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Driver Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.project_id == 0)
            {
                AddErrorMessage("Error", "Error", "Project Id Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.project_name))
            {
                AddErrorMessage("Error", "Error", "Project Name Is Missing");
                return Task.FromResult(_existing);
            }
          
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.PROJECTS.Where(e => e.project_id == _dto.project_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Project Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.project_name.ToLower() != _dto.project_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "project_name",
                            new_col_value = _dto.project_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "project_id",
                            pk_id = _dto.project_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Project Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.PROJECTS.Where(e => e.project_id == _dto.project_id & e.delete_id == 0).FirstOrDefault();
                            
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _trans.Context.PROJECTS.AddOrUpdateExtension(_existing);
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

        public Task<assign_project_fuel_stationC> AssignFuelStationToProject(dto_project_assign_fuelstationC _dto)
        {
            projectC _project = null;
            fuel_stationC _fuel_station = null;
            assign_project_fuel_stationC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "DTO Is Null!");
                _project = null;
                return Task.FromResult(_obj);
            }
            if (_dto.project_id == 0)
            {
                AddErrorMessage("Error", "Error", "Project Id Is Missing");
                return Task.FromResult(_obj);
            }
            if (_dto.fuel_station_id == 0)
            {
                AddErrorMessage("Error", "Error", "Fuel Station Id Is Missing");
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _fuel_station = (from k in _db.FUEL_STATIONS
                                where k.fuel_station_id == _dto.fuel_station_id & k.delete_id == 0
                                select k).FirstOrDefault();
                    if (_fuel_station == null)
                    {
                        AddErrorMessage("Error", "Error", "No Associated Fuel Station Found");
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    _project = (from k in _db.PROJECTS
                                     where k.project_id == _dto.project_id & k.delete_id == 0
                                     select k).FirstOrDefault();
                    if (_project == null)
                    {
                        AddErrorMessage("Error", "Error", "No Associated Project Found");
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    _obj = new assign_project_fuel_stationC()
                    {
                        project_id = _project.project_id,
                        fuel_station_id = _fuel_station.fuel_station_id,
                        created_by_user_id = m_logged_user.user_id,
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        server_edate = fnn.GetServerDate()
                    };
                    _db.ASSIGN_PROJECT_FUEL_STATIONS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Fuel Station Assignment");
                        _obj = null;
                        return Task.FromResult(_obj);
                    }
                    var _sql = string.Format("update {0} set no_of_fuel_stations=(no_of_fuel_stations + 1),fs_timestamp={1} where project_id={2} and delete_id=0",
                       _table_name.ToDbSchemaTable(), fnn.GetUnixTimeStamp(), _project.project_id);
                    _db.Database.Connection.Execute(_sql);
                    _db.SaveChanges();
                    //


                }
            }
            catch (SqlException ex)
            {
                _obj = null;
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                _obj = null;
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                _obj = null;
                LoggerX.LogException(ex);
            }
            return Task.FromResult(_obj);
        }

        public Task<bool> RemoveFuelStationFromProject(dto_project_remove_fuelstationC _dto)
        {
            bool _is_removed = false;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "DTO Is Null!");
                _is_removed = false;
                return Task.FromResult(_is_removed);
            }
            if (_dto.project_id == 0)
            {
                AddErrorMessage("Error", "Error", "Project Id Is Missing");
                _is_removed = false;
                return Task.FromResult(_is_removed);
            }
            if (_dto.fuel_station_id == 0)
            {
                AddErrorMessage("Error", "Error", "Fuel Station Id Is Missing");
                _is_removed = false;
                return Task.FromResult(_is_removed);
            }
            assign_project_fuel_stationC _existing = null;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _existing = (from k in _db.ASSIGN_PROJECT_FUEL_STATIONS
                                     where k.fuel_station_id == _dto.fuel_station_id & k.project_id == _dto.project_id & k.delete_id==0
                                     select k).FirstOrDefault();
                    if (_existing == null)
                    {
                        AddErrorMessage("Error", "Error", "No Associated Assigned Object Found");
                        _is_removed = false;
                        return Task.FromResult(_is_removed);
                    }
                    var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                    {
                        pk_col_name = "un_id",
                        pk_id = _existing.un_id,
                        table_name =  FleetManager.DbBase.tableNames.assign_fuelstation_project_tb.ToDbSchemaTable()
                    }, _db);
                    if (_result == null || _result == false)
                    {
                        AddErrorMessage("Delete Error", "Delete Error", "Error Encountered While Trying To Delete Record");
                        _is_removed = false;
                        return Task.FromResult(_is_removed);
                    }
                    else
                    {
                        var _sql = string.Format("update {0} set no_of_fuel_stations=(no_of_fuel_stations - 1),fs_timestamp={1} where project_id={2} and delete_id=0",
                       _table_name.ToDbSchemaTable(), fnn.GetUnixTimeStamp(), _existing.project_id);
                        _db.Database.Connection.Execute(_sql);
                        _is_removed = true;
                        _db.SaveChanges();
                        return Task.FromResult(_is_removed);
                    }
                    



                }
            }
            catch (SqlException ex)
            {
                _is_removed = false;
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                _is_removed = false;
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                _is_removed = false;
                LoggerX.LogException(ex);
            }
            return Task.FromResult(_is_removed);
        }

        public Task<List<assign_project_fuel_stationC>> GetFuelStationsAssignedToProjects(long fs_timestamp)
        {
            List<assign_project_fuel_stationC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0",  FleetManager.DbBase.tableNames.assign_fuelstation_project_tb.ToDbSchemaTable());
                        _list = _db.Query<assign_project_fuel_stationC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}",  FleetManager.DbBase.tableNames.assign_fuelstation_project_tb.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<assign_project_fuel_stationC>(_sql).ToList();
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
    }
}
