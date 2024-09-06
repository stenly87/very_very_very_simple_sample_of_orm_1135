using MySqlConnector;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text;

namespace ConsoleApp47
{
    static class MysqlTools
    {
        static MySqlConnection mysql;
        

        public static List<T> SimpleSelectFromTable<T>(string whereSql = null)
        {
            List<T> result = new();
            var type = typeof(T); // получение доступа к метаданным типа
            var attribute = type.GetCustomAttributes(typeof(MysqlTableAttribute), false).FirstOrDefault() as MysqlTableAttribute;
            var props = type.GetProperties();
            string table = attribute.TableName;
            string sql = "select * from `" + table + "`" + whereSql;
            using (MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql))
            using (MySqlDataReader dr = mySqlCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    T row = (T)Activator.CreateInstance(typeof(T));
                    result.Add(row);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string column = dr.GetName(i);
                        foreach (var prop in props)
                        {
                            var attributeProp = prop.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                            if (attributeProp != null && attributeProp.ColumnName == column)
                            {
                                if (!dr.IsDBNull(i))
                                    prop.SetValue(row, dr.GetValue(i));
                                break;
                            }
                        }

                    }
                }
                return result;
            }
        }

        public static List<object> SelectObjectFromTable(Type type, string whereSql = null)
        {
            List<object> result = new();
            var attribute = type.GetCustomAttributes(typeof(MysqlTableAttribute), false).FirstOrDefault() as MysqlTableAttribute;
            var props = type.GetProperties();
            string table = attribute.TableName;
            string sql = "select * from `" + table + "`" + whereSql;
            using (MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql))
            using (MySqlDataReader dr = mySqlCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    object row = Activator.CreateInstance(type);
                    result.Add(row);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string column = dr.GetName(i);
                        foreach (var prop in props)
                        {
                            var attributeProp = prop.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                            if (attributeProp != null && attributeProp.ColumnName == column)
                            {
                                if (!dr.IsDBNull(i))
                                    prop.SetValue(row, dr.GetValue(i));
                                break;
                            }
                        }

                    }
                }
                return result;
            }
        }

        internal static bool Delete<T>(T row) where T : BaseModel
        {
            var type = row.GetType(); // получение доступа к метаданным типа
            var attribute = type.GetCustomAttributes(typeof(MysqlTableAttribute), false).FirstOrDefault() as MysqlTableAttribute;
            var props = type.GetProperties();
            string sql;
            foreach (var property in props)
            {
                var attributeProp = property.GetCustomAttributes(typeof(MysqlRelationAttribute), false).FirstOrDefault() as MysqlRelationAttribute;
                if (attributeProp != null)
                {
                    sql = $"delete from `{attributeProp.TableName}` where {attributeProp.SecKey} = {row.ID}";
                    using (var ms = new MySqlCommand(sql, mysql))
                        ms.ExecuteNonQuery();
                }
            }
            int count = 0;
            sql = $"delete from `{attribute.TableName}` where id = {row.ID}";
            using (var ms = new MySqlCommand(sql, mysql))
                count = ms.ExecuteNonQuery();


            return count != 0;
        }

        internal static List<object> Include<T>(T row, Type t, string property, out PropertyInfo listColumn) where T : BaseModel
        {
            var type = row.GetType(); // получение доступа к метаданным типа
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == property)
                {
                    listColumn = prop;
                    var attributeProp = prop.GetCustomAttributes(typeof(MysqlRelationAttribute), false).FirstOrDefault() as MysqlRelationAttribute;
                    if (attributeProp != null)
                    {
                        List<object> result = SelectObjectFromTable(t, $"where {attributeProp.SecKey} = {row.ID}");
                        return result;
                    }
                }
            }
            listColumn = null;
            return null;
        }

        internal static bool Insert<T>(T row) where T : BaseModel
        {
            var type = row.GetType(); // получение доступа к метаданным типа
            var attribute = type.GetCustomAttributes(typeof(MysqlTableAttribute), false).FirstOrDefault() as MysqlTableAttribute;
            var props = type.GetProperties();

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into `");
            sb.Append(attribute.TableName);
            sb.Append("` (");
            List<string> columns = new List<string>();
            foreach (var property in props)
            {
                var attributeProp = property.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                if (attributeProp != null)
                    columns.Add(attributeProp.ColumnName);
            }
            sb.Append(string.Join(",", columns));
            sb.Append(") values (");
            sb.Append(string.Join(",", columns.Select(s => "@" + s)));
            sb.Append(");");
            string sql = sb.ToString();
            int count = 0;
            row.Hash = Guid.NewGuid().ToString();
            using (var ms = new MySqlCommand(sql, mysql))
            {
                foreach (var property in props)
                {
                    var attributeProp = property.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                    if (attributeProp != null)
                        ms.Parameters.Add(new MySqlParameter(attributeProp.ColumnName, property.GetValue(row)));
                }
                count = ms.ExecuteNonQuery();
            }

            sql = "select id from `" + attribute.TableName + "` where create_hash = '" + row.Hash + "'";
            using (var ms = new MySqlCommand(sql, mysql))
            using (var dr = ms.ExecuteReader())
            {
                dr.Read();
                row.ID = dr.GetInt32(0);
            }
            return count != 0;
        }

        internal static T SelectRowFromTable<T>(int id)
        {
            var result = SimpleSelectFromTable<T>("where id = " + id);
            return result.FirstOrDefault();
        }

        internal static void SetConnection(MySqlConnection mySqlConnection)
        {
            MysqlTools.mysql = mySqlConnection;
        }

        internal static bool Update(BaseModel row)
        {
            var type = row.GetType(); // получение доступа к метаданным типа
            var attribute = type.GetCustomAttributes(typeof(MysqlTableAttribute), false).FirstOrDefault() as MysqlTableAttribute;
            var props = type.GetProperties();

            StringBuilder sb = new StringBuilder();
            sb.Append("update `");
            sb.Append(attribute.TableName);
            sb.Append("` set ");
            List<string> columns = new List<string>();
            foreach (var prop in props)
            {
                var attributeProp = prop.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                if (attributeProp != null)
                    columns.Add($"`{attributeProp.ColumnName}` = @{attributeProp.ColumnName}");
            }
            sb.Append(string.Join(',', columns));
            sb.Append(" where id = ");
            sb.Append(row.ID);

            string sql = sb.ToString();
            int count = 0;
            using (var ms = new MySqlCommand(sql, mysql))
            {
                foreach (var property in props)
                {
                    var attributeProp = property.GetCustomAttributes(typeof(MysqlColumnAttribute), false).FirstOrDefault() as MysqlColumnAttribute;
                    if (attributeProp != null)
                        ms.Parameters.Add(new MySqlParameter(attributeProp.ColumnName, property.GetValue(row)));
                }
                count = ms.ExecuteNonQuery();
            }
            return count != 0;
        }
    }
}
