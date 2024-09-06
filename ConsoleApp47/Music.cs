[MysqlTable("music")]
public class Music : BaseModel
{
    [MysqlColumn("title")]
    public string Title { get; set; }
    [MysqlColumn("id_singer")]
    public int IDSinger { get; set; }
}
