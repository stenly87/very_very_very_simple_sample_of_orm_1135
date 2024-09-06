
public class MysqlTableAttribute : Attribute
{
    public MysqlTableAttribute(string table)
    {
        TableName = table;
    }

    public string TableName { get; }
}