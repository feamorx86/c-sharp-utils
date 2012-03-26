using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Windows.Forms;
 
    [AttributeUsage(AttributeTargets.Class)]
    public class CanSQLSync : Attribute
    {
        public string TableName;
        public CanSQLSync(string TableName)
        {
            this.TableName = TableName;
        }

        public CanSQLSync()
        {
            this.TableName = "";
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SQLField : Attribute
    {
        public string Table_FieldName;
        //public string 
        public SQLField()
        {
            Table_FieldName = "";
        }

        public SQLField(string ColumName)
        {
            Table_FieldName = ColumName;
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SQLField_NoSELECT : Attribute
    {
        public SQLField_NoSELECT()
        {

        }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SQLField_NoINSERT : Attribute
    {
        public SQLField_NoINSERT()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SQLField_NoUPDATE : Attribute
    {
        public SQLField_NoUPDATE()
        {

        }
    }

    public class SQLUtil
    {
        public static string DateTime_Format = "yyyy-MM-dd hh:mm:ss";
        public static string Date_Format = "yyyy-MM-dd";

        public static string Get_SELECT_ALL(Type SQLAbleType)
        {
            string result = "SELECT ";
            object[] attr = SQLAbleType.GetCustomAttributes(typeof(CanSQLSync),true);
            if (attr.Length != 1) return null;

            CanSQLSync tb = attr[0] as CanSQLSync;
            string TableName;
            if (string.IsNullOrEmpty(tb.TableName))
                TableName = SQLAbleType.Name;
            else
                TableName = tb.TableName;

            FieldInfo[] fields = SQLAbleType.GetFields();
            Type atr_type=typeof(SQLField);
            Type no_attr_type = typeof(SQLField_NoSELECT);

            SQLField f;
            bool was_fields=false;
            foreach (FieldInfo i in fields)
            {
                attr = i.GetCustomAttributes(no_attr_type, true);
                if (attr.Length > 0) continue;

                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length >0)
                {
                    was_fields = true;
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        result = result + " "+i.Name + ",";
                    else
                        result = result + " "+f.Table_FieldName+",";
                }
            }

            if (!was_fields) return null;
            //remove last char ','            
            return result.Remove(result.Length - 1)+" FROM "+TableName+";";            
        }

        public static string Get_SELECT_ALL(string TableName, List<String> Fields)
        {
            if (Fields.Count==0)
                return null;

            string result = "";

            for (int i = 0; i < Fields.Count-1;i++ )
            {
                result = result + Fields[i] + ", ";
            }

            return "SELECT " + result + Fields[Fields.Count - 1] + " FROM " + TableName;
        }

        public static string ObjectToValidSqlValue(Object obj)
        {
            //simply type
            Type t = obj.GetType();
            if (t==typeof(Int16) || t==typeof(Int32) || t==typeof(Int64) || 
                t==typeof(byte) || 
                t==typeof(UInt16) || t==typeof(UInt32) || t==typeof(UInt64))
                return obj.ToString();

            if (t == typeof(DateTime))
                return "'" + Convert.ToDateTime(obj).ToString(Date_Format) + "'";

            if (t == typeof(string))
                return "'" + Convert.ToString(obj)+ "'";


            return "'" + obj.ToString() + "'";
        }

        public static string Get_INSERT(Object Value)
        {
            Type SQLAbleType = Value.GetType();
            string result = "INSERT INTO ";
            object[] attr = SQLAbleType.GetCustomAttributes(typeof(CanSQLSync), true);

            if (attr.Length != 1) return null;

            CanSQLSync tb = attr[0] as CanSQLSync;
            
            if (string.IsNullOrEmpty(tb.TableName))
                result=result + SQLAbleType.Name;
            else
                result = result + tb.TableName;

            FieldInfo[] fields = SQLAbleType.GetFields();

            Type atr_type = typeof(SQLField);
            Type no_atr_type = typeof(SQLField_NoINSERT);

            SQLField f;
            string Cols=" ( ",Vals=" VALUES ( ";
            bool WasFields=false;
            foreach (FieldInfo i in fields)
            {
                attr = i.GetCustomAttributes(no_atr_type, true);
                if (attr.Length > 0) continue;

                //if (Value.GetType() == typeof(DateTime))return null;
                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length > 0)
                {
                    WasFields = true;
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        Cols=Cols+" "+i.Name + ",";
                    else
                        Cols = Cols +" "+ f.Table_FieldName + ",";

                    Vals = Vals +" "+ObjectToValidSqlValue(i.GetValue(Value)) + ",";
                }
            }
            if (!WasFields) return null;
            //last char ','            
            return result + Cols.Remove(Cols.Length - 1) + " )" + Vals.Remove(Vals.Length - 1) + " ) ;";
        }

        public static string Get_UPDATE(Object Value,string WhereStr)
        {
            if (string.IsNullOrEmpty(WhereStr)) return null;
            Type SQLAbleType = Value.GetType();
            string result = "UPDATE ";
            object[] attr = SQLAbleType.GetCustomAttributes(typeof(CanSQLSync), true);

            if (attr.Length != 1) return null;

            CanSQLSync tb = attr[0] as CanSQLSync;

            if (string.IsNullOrEmpty(tb.TableName))
                result = result + SQLAbleType.Name;
            else
                result = result + tb.TableName;

            result = result + " SET ";

            FieldInfo[] fields = SQLAbleType.GetFields();
            Type atr_type = typeof(SQLField);
            Type no_atr_type = typeof(SQLField_NoUPDATE);

            SQLField f;
            string Vals = "";
            bool WasFields = false;
            foreach (FieldInfo i in fields)
            {
                attr = i.GetCustomAttributes(no_atr_type, true);
                if (attr.Length > 0) continue;
                //if (Value.GetType() == typeof(DateTime))return null;
                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length > 0)
                {
                    WasFields = true;
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        Vals = Vals + " "+i.Name + " = ";
                    else
                        Vals = Vals +" "+ f.Table_FieldName + " = ";

                    Vals = Vals + ObjectToValidSqlValue(i.GetValue(Value)) + ",";
                }
            }
            if (!WasFields) return null;
            //last char ','            
            return result + Vals.Remove(Vals.Length - 1) + " WHERE "+WhereStr+" ;";
        }
        /*
        public static Object ReadObject(DbDataReader r, Type SQLAbleType)
        {
            if (r == null || r.IsClosed || !r.Read()) return null;

            
            object[] attr = SQLAbleType.GetCustomAttributes(typeof(CanSQLSync), true);
            if (attr.Length != 1) return null;

            Object Value = Activator.CreateInstance(SQLAbleType);

            CanSQLSync tb = attr[0] as CanSQLSync;

            FieldInfo[] fields = SQLAbleType.GetFields();
            Type atr_type = typeof(SQLField);
            Type no_atr_type = typeof(SQLField_NoSELECT);

            SQLField f;
            bool was_fields = false;
            string FName;
            foreach (FieldInfo i in fields)
            {
                attr = i.GetCustomAttributes(no_atr_type, true);
                if (attr.Length > 0)
                    continue;

                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length > 0)
                {
                    was_fields = true;
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        FName = i.Name;
                    else
                        FName = f.Table_FieldName;
                    i.SetValue(Value, Convert.ChangeType(r[FName], i.FieldType));
                }
            }
            if (!was_fields) return null;
            //remove last char ','            
            return Value;  
        }
        */

        public static db_result GetObjects(DbConnection con, TypeSqlableInfo info, out List<Object> resutl, ref string err)
        {
            resutl = null;

            #region  test connection

            if (con == null)
            {
                err = "Connection is null, Need to create connection";
                return db_result.worng_data;
            }

            if (info == null || info.TypeInstance == null || info.Fields.Count == 0)
            {
                err = "Wrong SqalableType Info";
                return db_result.worng_data;
            }

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch (System.Exception ex)
                {
                    if (con != null)
                        con.Close();

                    err = "Open connection Error: " + ex.Message;

                    return db_result.db_error;
                }
            }
            #endregion

            DbCommand cmd = null;
            DbDataReader r = null;

            Object Value;

            #region  DB Get Data
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = info.Get_SELECT();

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    err = "SELECT query is Empty, maybe Wrong type description";
                    return db_result.worng_data;
                }

                r = cmd.ExecuteReader();

                resutl = new List<Object>();

                while (r.Read())
                {
                    Value = Activator.CreateInstance(info.TypeInstance);

                    foreach (SqlableFieldInfo f in info.Fields)
                    {
                        if (f.IsSelect)
                        {
                            f.field.SetValue(Value,Convert.ChangeType(r[f.Num], f.field.FieldType));
                        }
                    }

                    resutl.Add(Value);
                }

                r.Close();

            }
            catch (System.Exception ex)
            {
                if (r != null)
                    r.Close();
                err = "DB Error: " + ex.Message;
                return db_result.db_error;
            }
            #endregion

            return db_result.ok;
        }

        public static db_result GetObjects<T>(DbConnection con, TypeSqlableInfo info, out List<T> resutl, ref string err)
        {
            resutl = null;

            #region  test connection

            if (con == null)
            {
                err = "Connection is null, Need to create connection";
                return db_result.worng_data;
            }

            if (info == null || info.TypeInstance == null || info.Fields.Count == 0)
            {
                err = "Wrong SqalableType Info";
                return db_result.worng_data;
            }

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch (System.Exception ex)
                {
                    if (con != null)
                        con.Close();

                    err = "Open connection Error: " + ex.Message;

                    return db_result.db_error;
                }
            }
            #endregion

            DbCommand cmd = null;
            DbDataReader r = null;

            T Value;

            #region  DB Get Data
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = info.Get_SELECT();

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    err = "SELECT query is Empty, maybe Wrong type description";
                    return db_result.worng_data;
                }

                r = cmd.ExecuteReader();

                resutl = new List<T>();

                while (r.Read())
                {
                    Value = (T)Activator.CreateInstance(info.TypeInstance);

                    foreach (SqlableFieldInfo f in info.Fields)
                    {
                        if (f.IsSelect)
                        {
                            f.field.SetValue(Value, Convert.ChangeType(r[f.Num], f.field.FieldType));
                        }
                    }

                    resutl.Add(Value);
                }

                r.Close();

            }
            catch (System.Exception ex)
            {
                if (r != null)
                    r.Close();
                err = "DB Error: " + ex.Message;
                return db_result.db_error;
            }
            #endregion

            return db_result.ok;
        }

        public static db_result GetObjects(DbConnection con, TypeSqlableInfo info, out List<Object> resutl)
        {
            resutl = null;

            #region  test connection

            if (con == null)
            {
               Log.LogError("GetObjects: Connection is null, Need to create connection");
                return db_result.worng_data;
            }

            if (info == null || info.TypeInstance == null || info.Fields.Count == 0)
            {
                Log.LogError("GetObjects:Wrong SqalableType Info");
                return db_result.worng_data;
            }

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch (System.Exception ex)
                {
                    if (con != null)
                        con.Close();

                    Log.LogError("GetObjects:Open connection Error: " + ex.Message);

                    return db_result.db_error;
                }
            }
            #endregion

            DbCommand cmd = null;
            DbDataReader r = null;

            Object Value;

            #region  DB Get Data
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = info.Get_SELECT();

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    Log.LogError("GetObjects:SELECT query is Empty, maybe Wrong type description");
                    return db_result.worng_data;
                }

                r = cmd.ExecuteReader();

                resutl = new List<Object>();

                while (r.Read())
                {
                    Value = Activator.CreateInstance(info.TypeInstance);

                    foreach (SqlableFieldInfo f in info.Fields)
                    {
                        if (f.IsSelect)
                        {
                            f.field.SetValue(Value, Convert.ChangeType(r[f.Num], f.field.FieldType));
                        }
                    }

                    resutl.Add(Value);
                }

                r.Close();

            }
            catch (System.Exception ex)
            {
                if (r != null)
                    r.Close();
                Log.LogError("GetObjects:DB Error: " + ex.Message);
                return db_result.db_error;
            }
            #endregion

            return db_result.ok;
        }

        public static db_result SafeInsert(DbConnection Connection, TypeSqlableInfo info, Object Value, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err="Insert: Can`t Begin Command, error: Can`t open connection.";
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    err="Insert: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd=null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_INSERT(Value);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Insert: db_error, changed!=1";
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Insert: db error=" + ex.Message;
                return db_result.db_error;
            }
            return db_result.ok;
        }

        public static db_result SafeInsert(DbConnection Connection, TypeSqlableInfo info, Object Value)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Insert: Can`t Begin Command, error: Can`t open connection.");
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Insert: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_INSERT(Value);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Insert: db_error, changed!=1");
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Insert: db error=" + ex.Message);
                return db_result.db_error;
            }
            return db_result.ok;
        }

        public static db_result SafeUpdate(DbConnection Connection, TypeSqlableInfo info, Object Value, string Where, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err = "Update: Can`t Begin Command, error: Can`t open connection.";
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    err = "Update: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_UPDATE(Value,Where);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Update: db_error, changed!=1";
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Update: db error=" + ex.Message;
                return db_result.db_error;
            }
            return db_result.ok;
        }

        public static db_result SafeUpdate(DbConnection Connection, TypeSqlableInfo info, Object Value, string Where)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Update: Can`t Begin Command, error: Can`t open connection.");
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Update: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_UPDATE(Value, Where);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Update: db_error, changed!=1");
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Update: db error=" + ex.Message);
                return db_result.db_error;
            }
            return db_result.ok;
        }


        public static db_result SafeQuery(DbConnection Connection, string Query, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err = "Query: Can`t Begin Command, error: Can`t open connection.";
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    err = "Query: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = Query;

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Query: db_error, changed!=1";
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Query: db error=" + ex.Message;
                return db_result.db_error;
            }
            return db_result.ok;
        }

        public static db_result SafeQuery(DbConnection Connection, string Query)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Query: Can`t Begin Command, error: Can`t open connection.");
                        return db_result.db_error;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Query: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return db_result.db_error;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = Query;

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Query: db_error, changed!=1");
                    return db_result.db_error;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Query: db error=" + ex.Message);
                return db_result.db_error;
            }
            return db_result.ok;
        }

        public static bool TryGetObjects(DbConnection con, TypeSqlableInfo info, out List<Object> resutl, ref string err)
        {
            resutl = null;

            #region  test connection

            if (con == null)
            {
                err = "Connection is null, Need to create connection";
                return false;
            }

            if (info == null || info.TypeInstance == null || info.Fields.Count == 0)
            {
                err = "Wrong SqalableType Info";
                return false;
            }

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch (System.Exception ex)
                {
                    if (con != null)
                        con.Close();

                    err = "Open connection Error: " + ex.Message;

                    return false;
                }
            }
            #endregion

            DbCommand cmd = null;
            DbDataReader r = null;

            Object Value;

            #region  DB Get Data
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = info.Get_SELECT();

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    err = "SELECT query is Empty, maybe Wrong type description";
                    return false;
                }

                r = cmd.ExecuteReader();

                resutl = new List<Object>();

                while (r.Read())
                {
                    Value = Activator.CreateInstance(info.TypeInstance);

                    foreach (SqlableFieldInfo f in info.Fields)
                    {
                        if (f.IsSelect)
                        {
                            f.field.SetValue(Value, Convert.ChangeType(r[f.Num], f.field.FieldType));
                        }
                    }

                    resutl.Add(Value);
                }

                r.Close();

            }
            catch (System.Exception ex)
            {
                if (r != null)
                    r.Close();
                err = "DB Error: " + ex.Message;
                return false;
            }
            #endregion

            return true;
        }

        public static bool TryGetObjects(DbConnection con, TypeSqlableInfo info, out List<Object> resutl)
        {
            resutl = null;

            #region  test connection

            if (con == null)
            {
                Log.LogError("GetObjects: Connection is null, Need to create connection");
                return false;
            }

            if (info == null || info.TypeInstance == null || info.Fields.Count == 0)
            {
                Log.LogError("GetObjects:Wrong SqalableType Info");
                return false;
            }

            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch (System.Exception ex)
                {
                    if (con != null)
                        con.Close();

                    Log.LogError("GetObjects:Open connection Error: " + ex.Message);

                    return false;
                }
            }
            #endregion

            DbCommand cmd = null;
            DbDataReader r = null;

            Object Value;

            #region  DB Get Data
            try
            {
                cmd = con.CreateCommand();

                cmd.CommandText = info.Get_SELECT();

                if (string.IsNullOrEmpty(cmd.CommandText))
                {
                    Log.LogError("GetObjects:SELECT query is Empty, maybe Wrong type description");
                    return false;
                }

                r = cmd.ExecuteReader();

                resutl = new List<Object>();

                while (r.Read())
                {
                    Value = Activator.CreateInstance(info.TypeInstance);

                    foreach (SqlableFieldInfo f in info.Fields)
                    {
                        if (f.IsSelect)
                        {
                            f.field.SetValue(Value, Convert.ChangeType(r[f.Num], f.field.FieldType));
                        }
                    }

                    resutl.Add(Value);
                }

                r.Close();

            }
            catch (System.Exception ex)
            {
                if (r != null)
                    r.Close();
                Log.LogError("GetObjects:DB Error: " + ex.Message);
                return false;
            }
            #endregion

            return true;
        }

        public static bool TrySafeInsert(DbConnection Connection, TypeSqlableInfo info, Object Value, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err = "Insert: Can`t Begin Command, error: Can`t open connection.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    err = "Insert: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_INSERT(Value);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Insert: db_error, changed!=1";
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Insert: db error=" + ex.Message;
                return false;
            }
            return true;
        }

        public static bool TrySafeInsert(DbConnection Connection, TypeSqlableInfo info, Object Value)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Insert: Can`t Begin Command, error: Can`t open connection.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Insert: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_INSERT(Value);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Insert: db_error, changed!=1");
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Insert: db error=" + ex.Message);
                return false;
            }
            return true;
        }

        public static bool TrySafeInsert(SQLiteConnection Connection, TypeSqlableInfo info, Object Value,ref int AddedID)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Insert: Can`t Begin Command, error: Can`t open connection.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Insert: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return false;
                }
            }

            SQLiteTransaction tr = null;
            SQLiteCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_INSERT(Value);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Insert: db_error, changed!=1");
                    return false;
                }
                AddedID = (int)tr.Connection.LastInsertRowId;
                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Insert: db error=" + ex.Message);
                return false;
            }
            return true;
        }

        public static bool TrySafeUpdate(DbConnection Connection, TypeSqlableInfo info, Object Value, string Where, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err = "Update: Can`t Begin Command, error: Can`t open connection.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    err = "Update: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_UPDATE(Value, Where);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Update: db_error, changed!=1";
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Update: db error=" + ex.Message;
                return false;
            }
            return true;
        }

        public static bool TrySafeUpdate(DbConnection Connection, TypeSqlableInfo info, Object Value, string Where)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Update: Can`t Begin Command, error: Can`t open connection.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Update: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = info.Get_UPDATE(Value, Where);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Update: db_error, changed!=1");
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Update: db error=" + ex.Message);
                return false;
            }
            return true;
        }


        public static bool TrySafeQuery(DbConnection Connection, string Query, ref string err)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        err = "Query: Can`t Begin Command, error: Can`t open connection.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    err = "Query: Can`t Begin Command, error:" + ex.Message;
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = Query;

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    err = "Query: db_error, changed!=1";
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                err = "Query: db error=" + ex.Message;
                return false;
            }
            return true;
        }

        public static bool TrySafeQuery(DbConnection Connection, string Query)
        {
            //Can begin command
            if (Connection.State != ConnectionState.Open)
            {
                try
                {
                    Connection.Open();
                    if (Connection.State != ConnectionState.Open)
                    {
                        Log.LogError("Query: Can`t Begin Command, error: Can`t open connection.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("Query: Can`t Begin Command, error:" + ex.Message);
                    Connection.Close();
                    return false;
                }
            }

            DbTransaction tr = null;
            DbCommand cmd = null;

            try
            {
                tr = Connection.BeginTransaction();

                cmd = tr.Connection.CreateCommand();
                cmd.CommandText = Query;

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tr.Rollback();
                    Log.LogError("Query: db_error, changed!=1");
                    return false;
                }

                tr.Commit();
            }
            catch (System.Exception ex)
            {
                if (tr != null) tr.Rollback();
                Log.LogError("Query: db error=" + ex.Message);
                return false;
            }
            return true;
        }
    }

    public class SqlableFieldInfo
    {
        public int Num;
        public FieldInfo field;
        public string Name;
        public bool IsSelect;
        public bool IsUpdate;
        public bool IsInsert;

        public SqlableFieldInfo(int num,FieldInfo f,string f_Name)
        {   
            Num = num;
            field = f;
            Name = f_Name;

            IsSelect = true;
            IsUpdate = true;
            IsInsert = true;
        }
    }

    public class TypeSqlableInfo
    {
        public Type TypeInstance;
        public List<SqlableFieldInfo> Fields;

        public string TableName;

        public TypeSqlableInfo()
        {
            TypeInstance = null;
            Fields = new List<SqlableFieldInfo>();
        }

        public TypeSqlableInfo(Type Sqlable)
        {
            TypeInstance = Sqlable;
            Fields = new List<SqlableFieldInfo>();
        }

        public static TypeSqlableInfo BuildInfo(Type SqlableType)
        {
            TypeSqlableInfo result = null;

            object[] attr = SqlableType.GetCustomAttributes(typeof(CanSQLSync), true);

            if (attr.Length <= 0)
                return null;

            CanSQLSync tb = attr[0] as CanSQLSync;

            FieldInfo[] t_fields = SqlableType.GetFields();

            Type atr_type = typeof(SQLField);
            Type no_select = typeof(SQLField_NoSELECT);
            Type no_update = typeof(SQLField_NoUPDATE);
            Type no_insert = typeof(SQLField_NoINSERT);

            SQLField f;

            SqlableFieldInfo fi;

            result = new TypeSqlableInfo();

            if (string.IsNullOrEmpty(tb.TableName))
                result.TableName = SqlableType.Name;
            else
                result.TableName = tb.TableName;

            result.TypeInstance = SqlableType;

            int num = 0;
            foreach (FieldInfo i in t_fields)
            {
                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length > 0)
                {
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        fi = new SqlableFieldInfo(num, i, i.Name);
                    else
                        fi = new SqlableFieldInfo(num, i, f.Table_FieldName);
                    result.Fields.Add(fi);
                    num++;

                    attr = i.GetCustomAttributes(no_select, true);
                    if (attr.Length > 0)
                        fi.IsSelect = false;
                    attr = i.GetCustomAttributes(no_update, true);
                    if (attr.Length > 0)
                        fi.IsUpdate = false;
                    attr = i.GetCustomAttributes(no_insert, true);
                    if (attr.Length > 0)
                        fi.IsInsert = false;
                }
            }

            if (result.Fields.Count == 0)
            {
                return null;
            }

            return result;
        }

        public bool BuildMe()
        {
            Fields.Clear();
            if (TypeInstance == null) return false;

            object[] attr = TypeInstance.GetCustomAttributes(typeof(CanSQLSync), true);

            if (attr.Length <= 0)
                return false;

            CanSQLSync tb = attr[0] as CanSQLSync;

            if (string.IsNullOrEmpty(tb.TableName))
                TableName = TypeInstance.Name;
            else
                TableName = tb.TableName;

            FieldInfo[] t_fields = TypeInstance.GetFields();

            Type atr_type = typeof(SQLField);
            Type no_select = typeof(SQLField_NoSELECT);
            Type no_update = typeof(SQLField_NoUPDATE);
            Type no_insert = typeof(SQLField_NoINSERT);

            SQLField f;

            SqlableFieldInfo fi;

            int num = 0;
            foreach (FieldInfo i in t_fields)
            {
                attr = i.GetCustomAttributes(atr_type, true);
                if (attr.Length > 0)
                {
                    f = attr[0] as SQLField;
                    if (string.IsNullOrEmpty(f.Table_FieldName))
                        fi = new SqlableFieldInfo(num, i, i.Name);
                    else
                        fi = new SqlableFieldInfo(num, i, f.Table_FieldName);
                    Fields.Add(fi);
                    num++;

                    attr = i.GetCustomAttributes(no_select, true);
                    if (attr.Length > 0)
                        fi.IsSelect = false;
                    attr = i.GetCustomAttributes(no_update, true);
                    if (attr.Length > 0)
                        fi.IsUpdate = false;
                    attr = i.GetCustomAttributes(no_insert, true);
                    if (attr.Length > 0)
                        fi.IsInsert = false;
                }
            }

            if (Fields.Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool BuildMe(Type ForType)
        {
            TypeInstance=ForType;
            return BuildMe();
        }

        public string Get_SELECT()
        {
            if (Fields.Count==0)
                return "";

            string result = "";

            foreach(SqlableFieldInfo f in Fields)
            {
                if (f.IsSelect)
                    result = result +f.Name + ", ";
            }
            
            if (string.IsNullOrEmpty(result)) return "";
            
            return "SELECT " + result.Substring(0,result.Length - 2) + " FROM " + TableName + " ;";
        }

        public string Get_INSERT(Object Value)
        {
            if (Value == null) return "";

            string result = "INSERT INTO "+TableName;

            string Cols = " ( ", Vals = " VALUES ( ";
            bool WasFields = false;
            foreach (SqlableFieldInfo i in Fields)
            {
                if (i.IsInsert)
                {
                    WasFields = true;
                    Cols = Cols + i.Name + ", ";
                    Vals = Vals + SQLUtil.ObjectToValidSqlValue(i.field.GetValue(Value)) + ", ";
                }
            }
            if (!WasFields) return "";
            //last char ', '
            return result + Cols.Substring(0,Cols.Length - 2) + " )" + Vals.Substring(0,Vals.Length - 2) + " ) ;";
        }

        public string Get_UPDATE(Object Value, string WhereStr)
        {
            if (Value==null) return "";
            string result = "UPDATE "+TableName+" SET ";

            string Vals = "";
            bool WasFields = false;

            foreach (SqlableFieldInfo i in Fields)
            {
                if (i.IsUpdate)
                {
                    WasFields = true;
                    Vals = Vals + " " + i.Name + " = " + SQLUtil.ObjectToValidSqlValue(i.field.GetValue(Value)) + ", ";
                }
            }
            if (!WasFields) return "";
            //last char ','            
            return result + Vals.Substring(0,Vals.Length - 2) + " WHERE " + WhereStr + " ;";
        }
    }

    public enum db_result
    {
        ok=0,
        worng_data,
        db_error,
        exist,
        not_exist
    }

    public class XmlUtils
    {
        public static XmlNode GetNodeByName(XmlNode parent, string NodeName)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.ChildNodes.Count; i++)
            {
                if (parent.ChildNodes[i].Name == NodeName)
                    return parent.ChildNodes[i];
            }
            return null;
        }

    }

    public class ListViewStrComparer : System.Collections.IComparer
    {
        public int CurrColum = 0;
        public int OldColum = -2;
        public delegate int ColumCompareDelegate(object A, Object B, int Coulum);
        public ColumCompareDelegate Comparer;

        public ListViewStrComparer(int StartColum, ColumCompareDelegate cmp_Delegate)
        {
            CurrColum = StartColum;
            OldColum = CurrColum - 1;
            Comparer = cmp_Delegate;
        }

        public int Compare(Object x, Object y)
        {
            if (CurrColum!=OldColum)
                return  Comparer(((ListViewItem)x).Tag,((ListViewItem)y).Tag,CurrColum);
            else
                return  Comparer(((ListViewItem)y).Tag,((ListViewItem)x).Tag,CurrColum);
        }
    }
