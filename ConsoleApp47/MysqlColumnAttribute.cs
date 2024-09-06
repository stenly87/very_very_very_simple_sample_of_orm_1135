
public class MysqlColumnAttribute : Attribute
{
    public string ColumnName { get; set; }

    public MysqlColumnAttribute(string column)
    {
        ColumnName = column;
    }
}