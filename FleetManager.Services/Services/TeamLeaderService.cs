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
    public class TeamLeaderService : ITeamLeaderService
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
        public TeamLeaderService(IMessageDialog dialog, ILoggedInUserService l_user)
        {
            _dialog = dialog;
            m_logged_user = l_user.GetLoggedInUser();
        }
        public string _table_name =  FleetManager.DbBase.tableNames.team_leader_tb;

        public Task<team_leaderC> Add(dto_team_leader_newC _dto)
        {
            team_leaderC _obj = null;
            if (_dto == null)
            {
                AddErrorMessage("Error", "Error", "New Team Leader Object Is Null");
                return Task.FromResult(_obj);
            }
            if (string.IsNullOrEmpty(_dto.team_leader_name))
            {
                AddErrorMessage("Error", "Error", "Team Leader Name Is Missing");
                return Task.FromResult(_obj);
            }
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = new team_leaderC()
                    {
                        team_leader_name  = _dto.team_leader_name.Trim(),
                        server_edate = fnn.GetServerDate(),
                        fs_timestamp = fnn.GetUnixTimeStamp(),
                        created_by_user_id = m_logged_user.user_id,
                    };
                    _db.TEAM_LEADERS.Add(_obj);
                    var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                    if (_retVal == null || _retVal.Value == true)
                    {
                        AddErrorMessage("Duplicate Key Error", "Duplicate Key Error", "You Have Entered A Duplicate Team Leader Name");
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
            team_leaderC  _obj = null;
            bool _record_deleted = false;
            try
            {
                using (var _db = fnn.GetDbContext())
                {
                    _obj = _db.TEAM_LEADERS.Where(e => e.team_leader_id == id & e.delete_id == 0).SingleOrDefault();
                    if (_obj == null)
                    {
                        _record_deleted = false;
                        AddErrorMessage("Delete Error", "Delete Error", "Could Not Find Team Leader Object");
                        return Task.FromResult(_record_deleted);
                    }
                    else
                    {
                        var _has_dependency = DbHelper.HasDbDependencies(_db.Database, new string[] { DbHelper.GetTableSchemaName(_table_name) }, DbHelper.GetDbSchema(), new string[] { "team_leader_id" }, id);
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
                                pk_col_name = "team_leader_id",
                                pk_id = _obj.team_leader_id,
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
        public Task<List<team_leaderC>> GetAll(long fs_timestamp)
        {
            List<team_leaderC> _list = null;
            string _sql = null;
            try
            {
                using (var _db = fnn.GetDbConnection())
                {
                    if (fs_timestamp == 0)
                    {
                        _sql = string.Format("select * from {0} where delete_id = 0", _table_name.ToDbSchemaTable());
                        _list = _db.Query<team_leaderC>(_sql).ToList();
                    }
                    else
                    {
                        _sql = string.Format("select * from {0} where fs_timestamp > {1}", _table_name.ToDbSchemaTable(), fs_timestamp);
                        _list = _db.Query<team_leaderC>(_sql).ToList();
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
        public Task<team_leaderC> Update(dto_team_leader_updateC _dto)
        {
            team_leaderC _existing = null;
            if (_dto == null)
            {
                AddErrorMessage("Update Error", "Save Error", "Dto Object Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            if (_dto.team_leader_id  == 0)
            {
                AddErrorMessage("Error", "Error", "Team Leader Id Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.team_leader_name))
            {
                AddErrorMessage("Error", "Error", "Team Leader Name Is Missing");
                return Task.FromResult(_existing);
            }
            if (string.IsNullOrEmpty(_dto.team_leader_name))
            {
                AddErrorMessage("Update Error", "Save Error", "Team Leader Name Is Null!");
                _existing = null;
                return Task.FromResult(_existing);
            }
            try
            {
                using (var _trans = new ZUpdateContext())
                {
                    _existing = _trans.Context.TEAM_LEADERS.Where(e => e.team_leader_id == _dto.team_leader_id & e.delete_id == 0).FirstOrDefault();
                    if (_existing == null)
                    {
                        _existing = null;
                        AddErrorMessage("Update Error", "Save Error", "Unable To Find Vehicle Category Object");
                        return Task.FromResult(_existing);
                    }
                    if (_existing.team_leader_name.ToLower() != _dto.team_leader_name.ToLower())
                    {
                        var _ret = DbHelper.UpdatePrimaryKeyColumn(new DbHelperPrimarykeyUpdateC
                        {
                            col_to_update = "team_leader_name",
                            new_col_value = _dto.team_leader_name.Trim().ToProperCase(),
                            table_name = DbHelper.GetTableSchemaName(_table_name),
                            pk_col_name = "team_leader_id",
                            pk_id = _dto.team_leader_id
                        }, _trans.Context);
                        if (_ret == null || _ret.Value == false)
                        {
                            AddErrorMessage("Error", "Update Error", "Team Leader Name Already Exists");
                            _existing = null;
                            _trans.RollBack();
                            return Task.FromResult(_existing);
                        }
                        else
                        {
                            _trans.Context.SaveChanges();
                            _existing = _trans.Context.TEAM_LEADERS.Where(e => e.team_leader_id == _dto.team_leader_id & e.delete_id == 0).FirstOrDefault();
                        }
                    }
                    SimpleMapper.PropertyMap(_dto, _existing);
                    _trans.Context.TEAM_LEADERS.AddOrUpdateExtension(_existing);
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
