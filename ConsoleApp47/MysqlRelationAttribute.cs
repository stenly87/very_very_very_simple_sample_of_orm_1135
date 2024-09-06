
internal class MysqlRelationAttribute : Attribute
{
    public MysqlRelationAttribute(string table, string secKey)
    {
        TableName = table;
        SecKey = secKey;
    }

    public string TableName { get; }
    public string SecKey { get; }
}