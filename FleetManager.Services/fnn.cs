using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Autofac;
using System.Security.Cryptography;
using System.Text;
using FleetManager.Shared.Core;
using FleetManager.Shared;
using MutticoFleet.Service.Interfaces;
using MutticoFleet.Service;
using FleetManager.Shared.Interfaces;
using System.Data;
using FleetManager.Shared.dto;
using FleetManager.Shared.Models;

using System.Data.SqlClient;
using System.Data.Entity;
using Dapper;
using FleetManager.DbBase;

namespace MutticoFleet.Service
{
    public static class fnn
    {
        public static em.db_Type DB_PROVIDER = em.db_Type.none;
        public static string DB_SCHEMA { get; set; }
        public static string reset_key { get; set; }
        static bool reset = false;
        static string exclude_tables = "_MigrationHistory";
        static List<string> _table_names = new List<string>();
        internal static bool zzzKeySet { get; set; }
        private static dto_logged_user _logged_user = null;

        public static bool isSeeding = false;
        internal static dto_logged_user LOGGED_USER
        {
            get
            {
                if (_logged_user == null)
                {
                    _logged_user = ObjectBase._container.Resolve<ILoggedInUserService>().GetLoggedInUser();
                }
                return _logged_user;
            }
        }
        public static string GetRebexLicenceKey()
        {
            
            return string.Empty;

        }
        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        internal static BaseContext GetDbContext()
        {
            if (!zzzKeySet)
            {
                zzzKeySet = true;
            }
            return (ObjectBase._container.Resolve<IDbContextHelper>()).GetContext();
        }
        internal static DateTime GetServerDate()
        {
            var _zone = TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time");
            var _date_time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _zone);
            DateTime _server_date = _date_time;
            return _server_date;
        }
        internal static long GetUnixTimeStamp()
        {
            return GetServerDate().ToUnixTimestamp();
        }
        internal static DbParameter GetDbParameters(string parameterName, object value)
        {
            return (ObjectBase._container.Resolve<IDbContextHelper>()).GetDbParameters(parameterName, value);
        }
        internal static DbConnection GetDbConnection()
        {
            return (ObjectBase._container.Resolve<IDbContextHelper>()).GetDBConnection();
        }
        public static String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }
        public static bool isAdmin(pc_userC _user)
        {

            bool _is_admin = false;
            if (_user != null)
            {
                
            }
            return _is_admin;
        }
        internal static void DeleteCreditor(int cr_account_id)
        {
            using (var _db = fnn.GetDbContext())
            {
                foreach (var _tb in new string[]
                {
                                FleetManager.DbBase.tableNames.creditor_deposit_tb,
                                FleetManager.DbBase.tableNames.creditor_invoice_payment_tb,
                                FleetManager.DbBase.tableNames.creditor_invoice_tb,
                                FleetManager.DbBase.tableNames.creditor_tb,
                                FleetManager.DbBase.tableNames.creditor_trans_statement_tb,
                               
                })
                {
                    var _result = DbHelper.DeleteRecordWithDeleteId(new DbHelperDeleteRecordC()
                    {
                        pk_col_name = "cr_account_id",
                        pk_id = cr_account_id,
                        table_name = _tb.ToDbSchemaTable()
                    }, _db);
                    _db.SaveChanges();

                }
                _db.SaveChanges();
            }
        }
        internal static void CreateCreditorStatementFromDeposit(FleetManager.Shared.Models.creditor_depositC _dep,BaseContext _db)
        {
            try
            {
                
                //

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
        }
        internal static void RunCreditorPayments(int cr_account_id)
        {
           
        }
        internal static void CreateCreditorStatementFromInvoice(FleetManager.Shared.Models.creditor_invoiceC _inv, BaseContext _db)
        {
            try
            {
               

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

        }
        internal static void DeleteCreditorInvoice(List<FleetManager.Shared.Models.vh_expense_transC> _exp_trans_list, BaseContext _db)
        {
            if (_exp_trans_list == null)
            {
                return;
            }
            try
            {
               

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
        }
        internal static void DeleteCreditorDeposit(FleetManager.Shared.Models.creditor_depositC _dep, BaseContext _db)
        {
            if (_dep == null)
            {
                return;
            }
            try
            {
               

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
        }
        internal static void CreateCreditorInvoice(List<FleetManager.Shared.Models.vh_expense_transC> _exp_trans_list)
        {
            if (_exp_trans_list == null)
            {
                return;
            }
            List<int> _creditor_ids = new List<int>();
            using (var _db = fnn.GetDbContext())
            {
                try
                {
                    FleetManager.Shared.Models.creditor_invoiceC _invoice = null;
                    FleetManager.Shared.Models.creditorC _creditor = null;
                    List<FleetManager.Shared.Models.creditor_invoiceC> _invoice_list = new List<creditor_invoiceC>();
                    string _sql = null;
                    int _type_id = 0;
                    foreach (var _e in _exp_trans_list)
                    {
                        _creditor = null;
                        switch (_e.exp_type_cat)
                        {
                            case em.vehicle_expense_categoryS.car_parts:
                                {
                                    _type_id = em.creditor_typeS.mechanic.ToInt32();
                                    if (_e.mechanic_id > 0)
                                    {
                                        _creditor = (from k in _db.CREDITORS_SET
                                                     where k.cr_owner_id == _e.mechanic_id & k.cr_account_type_id == _type_id & k.delete_id == 0
                                                     select k).FirstOrDefault();
                                    }
                                    break;
                                }
                            case em.vehicle_expense_categoryS.vehicle_hire:
                                {
                                    _type_id = em.creditor_typeS.vehicle_owner.ToInt32();
                                    if (_e.vh_owner_id > 0)
                                    {
                                        _creditor = (from k in _db.CREDITORS_SET
                                                     where k.cr_owner_id == _e.vh_owner_id & k.cr_account_type_id == _type_id & k.delete_id == 0
                                                     select k).FirstOrDefault();
                                    }
                                    break;
                                }
                            case em.vehicle_expense_categoryS.fuel:
                            case em.vehicle_expense_categoryS.general_service:
                                {
                                     _type_id = em.creditor_typeS.fuel_station.ToInt32();
                                    if (_e.fuel_station_id > 0)
                                    {
                                        _creditor = (from k in _db.CREDITORS_SET
                                                     where k.cr_owner_id == _e.fuel_station_id & k.cr_account_type_id == _type_id & k.delete_id == 0
                                                     select k).FirstOrDefault();
                                    }
                                    break;
                                }
                            case em.vehicle_expense_categoryS.tyres:
                                {
                                    _type_id = em.creditor_typeS.city_tyres.ToInt32();
                                    _creditor = (from k in _db.CREDITORS_SET
                                                 where k.cr_owner_id ==_e.exp_type_id  & k.cr_account_type_id == _type_id & k.delete_id == 0
                                                 select k).FirstOrDefault();

                                    break;
                                }
                            default:
                                {
                                    _type_id = em.creditor_typeS.mechanic.ToInt32();
                                    if (_e.mechanic_id > 0)
                                    {
                                        _creditor = (from k in _db.CREDITORS_SET
                                                     where k.cr_owner_id == _e.fuel_station_id & k.cr_account_type_id == _type_id & k.delete_id == 0
                                                     select k).FirstOrDefault();
                                    }
                                    break;
                                }
                        }
                        if (_creditor == null)
                        {
                            continue;
                        }
                        _invoice = null;
                        _invoice = new creditor_invoiceC()
                        {
                            amount_paid = 0,
                            created_by_user_id = _e.created_by_user_id,
                            expense_id = _e.expense_id,
                            exp_cat_id = _e.exp_type_cat_id,
                            exp_type_id = _e.exp_type_id,
                            exp_type_name = _e.exp_type_name,
                            fs_timestamp = fnn.GetUnixTimeStamp(),
                            invoice_amount = _e.expense_amount,
                            invoice_balance = _e.expense_amount,
                            vehicle_plate_no = _e.vehicle_plate_no,
                            invoice_fs_date = _e.expense_date,
                            invoice_fs_id = _e.expense_fs_id,
                            vehicle_id = _e.vehicle_id,
                            project_id = _e.project_id,
                            project_name = _e.project_name,
                            server_edate = fnn.GetServerDate(),
                            cr_account_id = _creditor.cr_account_id,
                            cr_account_type_id = _creditor.cr_account_type_id,
                            cr_account_name=_creditor.cr_account_name

                        };
                        _db.CREDITOR_INVOICES.Add(_invoice);
                        var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                        if (_retVal == null || _retVal.Value == true)
                        {
                            throw new Exception("You Have Entered A Duplicate Invoice Transaction");
                        }
                        _invoice_list.Add(_invoice);
                    }
                    if (_invoice_list.Count == 0) { return; }
                    foreach(var _inv in _invoice_list)
                    {
                        #region update_expense_db
                        _sql = string.Format("update {0} set cr_invoice_id=@v1,fs_timestamp=@v2 where expense_id=@v3 and delete_id=0",
                                             FleetManager.DbBase.tableNames.expense_trans_tb.ToDbSchemaTable());
                        _db.Database.Connection.Execute(_sql, new
                        {
                            v1 = _inv.cr_invoice_id,
                            v2 = fnn.GetUnixTimeStamp(),
                            v3=_inv.expense_id
                        },
                        _db.Database.GetDbTransaction()
                        );
                        _db.SaveChanges();
                        #endregion
                        CreateCreditorStatementFromInvoice(_inv, _db);
                    }
                    _db.SaveChanges();
                    
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
            }
            foreach(var _id in _creditor_ids)
            {
                fnn.RunCreditorPayments(_id);
            }
        }
        internal static FleetManager.Shared.Models.creditorC CreateCreditor(int cr_owner_id, string cr_owner_name, string phone_no, em.creditor_typeS cr_type, BaseContext _db)
        {
            FleetManager.Shared.Models.creditorC _creditor = new creditorC()
            {
                created_by_user_id = -1964,
                cr_account_name = cr_owner_name,
                cr_account_type_id = cr_type.ToInt32(),
                cr_owner_id = cr_owner_id,
                fs_timestamp = fnn.GetUnixTimeStamp(),
                server_edate = fnn.GetServerDate(),
                cr_phone_no = phone_no,
                
                
            };
            try
            {
                _db.CREDITORS_SET.Add(_creditor);
                var _retVal = _db.SaveChangesWithDuplicateKeyDetected();
                if (_retVal == null || _retVal.Value == true)
                {
                    return null;
                }
                string _sql = null;
              
                switch (cr_type)
                {
                    case em.creditor_typeS.fuel_station:
                        {
                         
                            _sql = string.Format("update {0} set cr_account_id=@v1,fs_timestamp=@v2 where fuel_station_id=@v3 and delete_id=0",  FleetManager.DbBase.tableNames.fuel_station_tb.ToDbSchemaTable());
                            _db.Database.Connection.Execute(_sql, new
                            {
                                v1 = _creditor.cr_account_id,
                                v2 = fnn.GetUnixTimeStamp(),
                                v3 = cr_owner_id
                            });
                            _db.SaveChanges();
                            break;
                        }
                    case em.creditor_typeS.mechanic:
                        {
                            
                            _sql = string.Format("update {0} set cr_account_id=@v1,fs_timestamp=@v2 where mechanic_id=@v3 and delete_id=0",
                                 FleetManager.DbBase.tableNames.mechanic_tb.ToDbSchemaTable());
                            _db.Database.Connection.Execute(_sql, new
                            {
                                v1 = _creditor.cr_account_id,
                                v2 = fnn.GetUnixTimeStamp(),
                                v3 = cr_owner_id
                            });
                            _db.SaveChanges();

                            break;
                        }
                    case em.creditor_typeS.vehicle_owner:
                        {
                           
                            _sql = string.Format("update {0} set cr_account_id=@v1,fs_timestamp=@v2 where vh_owner_id=@v3 and delete_id=0",
                               FleetManager.DbBase.tableNames.vehicle_owner_tb.ToDbSchemaTable());
                            _db.Database.Connection.Execute(_sql, new
                            {
                                v1 = _creditor.cr_account_id,
                                v2 = fnn.GetUnixTimeStamp(),
                                v3 = cr_owner_id
                            });
                            _db.SaveChanges();
                            break;
                        }
                }
            }
            catch (SqlException ex)
            {
                _creditor = null;
                LoggerX.LogException(ex);
            }
            catch (DbException ex)
            {
                _creditor = null;
                LoggerX.LogException(ex);
            }
            catch (Exception ex)
            {
                _creditor = null;
                LoggerX.LogException(ex);

            }
            return _creditor;
        }
        public static void SeedAdmin(string dev_admin_email,string company_admin_email, string admin_pwd, BaseContext context=null)
        {

            //check if table exists
            const int DEV_ADMIN_ID= -1964;
            const int ADMIN_ID = -1900;
            const int ADMIN_SECURITY_GP_ID = -100;
            string _sql = string.Empty;
            Dictionary<string, object> _data_dictionary = new Dictionary<string, object>();
            isSeeding = true;
            try
            {
            
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
            isSeeding = false;
        }
        public static bool ResetDb(BaseContext _db)
        {
            //string _sql = null;
            // where are the try catch blocks ??

            bool _db_reset = false;

           
            
            return _db_reset;
        }
    }
}