using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Autofac;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Data.Common;
using FleetManager.Shared.Core;
using MutticoFleet.Service.Interfaces;
using Service;
using System.IO;
using System.Net;

namespace MutticoFleet.Service
{
    internal static class Extensions
    {
        public static bool? SaveChangesWithDuplicateKeyDetected(this DbContext value)
        {

            try
            {
                if (value == null)
                {
                    return null;
                }
                
                value.SaveChanges();
                return false;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                return (ObjectBase._container.Resolve<IDbContextHelper>()).IsSaveDuplicateRecordError(ex);
                
            }
            
        }
        public static string ToEnglishCultureDateString(this DateTime value)
       {
           
            try
            {
                return value.Date.ToShortDateString().ToString(new CultureInfo("en-US"));
            }
            catch (Exception ed)
            {
                return DateTime.MinValue.ToShortTimeString();
            }
       }
        public static Int32 ToInt32(this Object value )
        {
           
            try
            {
                if (value == null)
                {
                    return 0;
                }
                return Convert.ToInt32(value);
            }
            catch (Exception ed)
            {
                return 0;
            }
        }
        public static string ToDbSchemaTable(this string value)
        {
            return DbHelper.GetTableSchemaName(value);
        }
        public static Int64 ToInt64(this Object value)
        {
           
            try
            {
                if (value == null)
                {
                    return 0;
                }
                return Convert.ToInt64(value);
            }
            catch (Exception ed)
            {
                return 0;
            }
        }
        public static UInt32 ToUInt32(this Object value)
        {
           
            return Convert.ToUInt32(value);
        }
        public static float ToFloat(this object value)
        {
           
            try
            {
                if (value == null)
                {
                    return 0.0f;
                }
                return Convert.ToSingle(value);
            }
            catch (Exception)
            {
                return 0.0f;
            }
        }
        public static string ToProperCase(this Object value)
       {
          
           if (value == null)
           {
               return null;
            }
           if (value.ToString() == string.Empty)
           {
               return string.Empty;
           }
           CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
           TextInfo textInfo = cultureInfo.TextInfo;
           return textInfo.ToTitleCase(value.ToString());

       }
        public static Decimal ToDecimal(this Object value)
       {
          
           return Convert.ToDecimal(value);
       }
        public static Int16 ToInt16(this Object value)
        {
           
            try
            {
                if (value == null)
                {
                    return 0;
                }
                return Convert.ToInt16(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string GetResponseBodyString(this WebResponse _resp)
        {
            string _body = string.Empty;
            using (StreamReader sr = new StreamReader(_resp.GetResponseStream()))
            {
                // read the response and process it
                _body = sr.ReadToEnd();
                //  Process(body);
            }
            return _body;
        }
        public static  byte ToByte(this Object value)
        {
           
            try
            {
                if (value == null)
                {
                    return 0;
                }
                return Convert.ToByte(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static float ToInches(this Object value)
       {
          
           try
           {
               float a = Convert.ToSingle(value);
               float b = 100f;
               return (a / b);
           }
           catch (Exception)
           {
               return 0;
           }
       }
        public static string ToNumberDisplayFormat(this object value)
        {
            try
            {
                return Convert.ToDecimal(value).ToString("##,#", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return "0";
                throw;
            }
        }
        public static byte ToBooleanByte(this bool value)
        {
           return (value == true) ? 1.ToByte() : 0.ToByte();
        }
        public static byte[] ToUTF8Bytes(this string value)
        {
           
            if (value == null) { return null; }
            Encoding en = Encoding.UTF8;
            return en.GetBytes(value);
        }
        public static int SingleUpdateCommand(this DbContext dbContext,string table_name, string[] columns, object[] row, short comp_cols)
        {
            try
            {
                int _affected_rows = 0;
                short cmp_cols = comp_cols;
                if (cmp_cols == 0)
                {
                    cmp_cols = 1;
                }
                int start_comp_position = columns.Length - cmp_cols;
                using (DbCommand cmd = dbContext.Database.Connection.CreateCommand())
                {
                    //build insert string
                    StringBuilder update_cmd = new StringBuilder();
                    update_cmd.Append("update " + table_name + " set ");
                    for (byte i = 0; i < columns.Length; i++)
                    {
                        if (i == start_comp_position)
                        {
                            #region add last columns
                            for (short k = 0; k < cmp_cols; k++)
                            {
                                if (k == 0)
                                {
                                    update_cmd.Append(" where ");
                                    update_cmd.Append(columns[i + k]);
                                    update_cmd.Append("=@" + columns[i + k]);
                                    cmd.Parameters.Add(fnn.GetDbParameters("@" + columns[i + k], null));
                                }
                                else
                                {
                                    update_cmd.Append(" and ");
                                    update_cmd.Append(columns[i + k]);
                                    update_cmd.Append("=@" + columns[i + k]);
                                    cmd.Parameters.Add(fnn.GetDbParameters("@" + columns[i + k], null));
                                }
                            }
                            #endregion
                            break;
                        }

                        update_cmd.Append(columns[i]);
                        update_cmd.Append("=@" + columns[i]);
                        cmd.Parameters.Add(fnn.GetDbParameters("@" + columns[i], null));
                        if (i == start_comp_position - 1) { continue; }
                        update_cmd.Append(",");
                    }

                    cmd.CommandText = update_cmd.ToString();
                    update_cmd = null;
                    object[] _row = row;
                    for (byte j = 0; j < cmd.Parameters.Count; j++)
                    {
                        cmd.Parameters[j].Value = _row[j];
                    }
                    _affected_rows = cmd.ExecuteNonQuery();
                    _row = null;
                }
                return _affected_rows;


            }
            catch (System.Exception ex)
            {

                return -1;
            }
        }

        


    }
}