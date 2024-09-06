using MySqlConnector;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text;

namespace ConsoleApp47
{
    public static class BaseModelExtension
    {
        public static bool Insert(this BaseModel model)
        {
            return MysqlTools.Insert(model);
        }

        public static bool Delete(this BaseModel model)
        {
            return MysqlTools.Delete(model);
        }

        public static bool Update(this BaseModel model)
        {
            return MysqlTools.Update(model);
        }

        public static BaseModel Include<T>(this BaseModel model, string property)
        {
            var objects = MysqlTools.Include(model, typeof(T), property, out PropertyInfo listColumn);
            if (objects == null)
                return model;
            var castResult = objects.Select(s => (T)s).ToList();
            listColumn.SetValue(model, castResult);
            return model;
        }
    }
}
