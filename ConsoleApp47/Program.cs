using ConsoleApp47;
using MySqlConnector;

MySqlConnection mySqlConnection;
MySqlConnectionStringBuilder sb =
    new MySqlConnectionStringBuilder();
sb.Server = "192.168.200.13";
sb.UserID = "student";
sb.Password = "student";
sb.Database = "1135_new_2024";
sb.CharacterSet = "utf8mb4";
sb.ConnectionTimeout = 1;
//Console.WriteLine(sb.ToString());
mySqlConnection = new MySqlConnection(sb.ToString());

try
{
    mySqlConnection.Open();
}
catch (MySqlException ex)
{
    Console.WriteLine($"{ex.Number}: {ex.Message}");
    return;
}

MysqlTools.SetConnection(mySqlConnection);

DataBase dataBase = new DataBase();

/*
var newSinger = new Singer { FirstName = "Алла", LastName = "Пугачева" };
if (newSinger.Insert(mySqlConnection))
{
    var newMusic = new Music { IDSinger = newSinger.ID, Title = "Миллион белых роз" };
    if (newMusic.Insert(mySqlConnection))
        Console.WriteLine("Новая песенка добавлена успешно");
}*/

//new Music { ID = 4, IDSinger = 6, Title = "Белые розы" }.Update();

var singer1 = dataBase.GetSingerByID(6).Include<Music>("Musics");

var singer2 = dataBase.GetSingerByID(7).
    Include<Music>("Musics").
    Include<Press>("PressArticles");

var musics  = dataBase.GetMusic();
var singers = dataBase.GetSingers();

foreach (var singer in singers)
    Console.WriteLine($"{singer.ID} {singer.LastName} {singer.FirstName}");

foreach (var music in musics)
    Console.WriteLine($"{music.ID} {music.Title} {music.IDSinger}");


/*
Dictionary<int, Singer> singerData = new();

string sql = "select `music`.id AS 'id_music', title, `music`.id_singer, firstname, lastname from music join singer on music.id_singer = singer.id";
using (MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection))
using (MySqlDataReader dr = mySqlCommand.ExecuteReader())
{
    while (dr.Read())
    {
        int idSinger = dr.GetInt32("id_singer");
        if (!singerData.ContainsKey(idSinger))
        {
            string fName = dr.GetString("firstname");
            string lName = dr.GetString("lastname");
            singerData.Add(idSinger, new Singer
            {
                FirstName = fName,
                LastName = lName,
                ID = idSinger,
                Musics = new List<Music>()
            });
        }
        int idMusic = dr.GetInt32("id_music");
        string title = dr.GetString("title");
        singerData[idSinger].Musics.Add(new Music
        {
            ID = idMusic,
            Title = title
        });
    }
}



foreach (var key in singerData.Keys)
{
    Console.WriteLine($"{singerData[key].ID} {singerData[key].LastName} {singerData[key].FirstName}");
    foreach (var music in singerData[key].Musics)
        Console.WriteLine(music.Title);
}

Console.WriteLine("Непременно нужно добавить еще исполнителя или музыку к нему");
Console.WriteLine("Введите индекс певца ртом для продолжения или пустую строку для создания нового певца ртом");
string userAction = Console.ReadLine();
if (!int.TryParse(userAction, out int index))
{
    Console.WriteLine("Как фамилия певца?");
    string lNewName = Console.ReadLine();
    Console.WriteLine("Как имя певца?");
    string fNewName = Console.ReadLine();
    string hash = Guid.NewGuid().ToString();

    sql = "insert into `singer` values (0, @firstname, @lastname, @create_hash)";
    using (var ms = new MySqlCommand(sql, mySqlConnection))
    {
        ms.Parameters.Add(new MySqlParameter("firstname", fNewName));
        ms.Parameters.Add(new MySqlParameter("lastname", lNewName));
        ms.Parameters.Add(new MySqlParameter("create_hash", hash));
        ms.ExecuteNonQuery();
    }

    sql = "select id from singer where create_hash = '" + hash + "'";
    using (var ms = new MySqlCommand(sql, mySqlConnection))
    using (var dr =  ms.ExecuteReader())    
    {
        dr.Read();
        index = dr.GetInt32(0);
    }
    // дополнительный запрос, который еще больше уменьшает вероятность
    // что у двух записей будет одинаковый хэш
    sql = "update singer set create_hash = '' where create_hash = '" + hash + "'";
    using (var ms = new MySqlCommand(sql, mySqlConnection))
        ms.ExecuteNonQuery();
}


Console.WriteLine("Как называется песня певца ртом?");
string newTitle = Console.ReadLine();

sql = "insert into `music` values (0, @title, @id_singer, '')";
using (var ms = new MySqlCommand(sql, mySqlConnection))
{
    ms.Parameters.Add(new MySqlParameter("title", newTitle));
    ms.Parameters.Add(new MySqlParameter("id_singer", index));
    ms.ExecuteNonQuery();
}*/

mySqlConnection.Close();