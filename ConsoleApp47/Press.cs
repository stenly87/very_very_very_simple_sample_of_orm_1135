[MysqlTable("yellow_press")]
public class Press : BaseModel
{
    [MysqlColumn("title_article")]
    public string Title { get; set; }
    [MysqlColumn("description")]
    public string Description { get; set; }
    [MysqlColumn("id_singer")]
    public int IDSinger { get; set; }
}