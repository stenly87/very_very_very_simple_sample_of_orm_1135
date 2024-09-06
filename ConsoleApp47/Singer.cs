[MysqlTable("singer")]
public class Singer : BaseModel
{
    [MysqlColumn("firstname")]
    public string FirstName { get; set; }
    [MysqlColumn("lastname")]
    public string LastName  { get; set; }

    [MysqlRelation("music", "id_singer")]
    public List<Music> Musics { get; set; }

    [MysqlRelation("yellow_press", "id_singer")]
    public List<Press> PressArticles { get; set; }
}