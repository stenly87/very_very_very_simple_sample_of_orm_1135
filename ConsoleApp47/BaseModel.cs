public abstract class BaseModel
{
    [MysqlColumn("id")]
    public int ID { get; set; }
    [MysqlColumn("create_hash")]
    public string Hash { get; set; }
}
