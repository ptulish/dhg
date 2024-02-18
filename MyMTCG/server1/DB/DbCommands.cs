using Microsoft.CSharp.RuntimeBinder;
using MyServer.Classes;
using Newtonsoft.Json;
using Npgsql;

namespace server1.DB;

public class DbCommands
{
    private string connectionString;
    private NpgsqlConnection connection;
    public DbCommands(ConnectionToDB connectionToDb)
    {
        this.connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";
        this.connection = connectionToDb.GetConnection();
    }
    public User getUser(string username)
    {
        // Параметризованный SQL-запрос для поиска пользователя по логину
        string selectUserQuery = "SELECT * FROM users WHERE username = @username";
        User newUser = null;
        using (NpgsqlCommand command = new NpgsqlCommand(cmdText: selectUserQuery, connection: connection))
        {
            // Добавление параметра для поиска
            command.Parameters.AddWithValue(parameterName: "@username", value: username);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                // Если пользователь найден
                if (reader.Read())
                {
                    newUser = new User(User_id: (int)reader[name: "user_id"],
                        username: reader[name: "username"].ToString(),
                        password: reader[name: "password"].ToString(), coins: (int)reader[name: "coins"],
                        elo: (int)reader[name: "elo"], draws: (int)reader[name: "draws"],
                        wins: (int)reader[name: "wins"], games: (int)reader[name: "games"]);
                    Console.WriteLine(value: $"User ID: {reader[name: "user_id"]}");
                    Console.WriteLine(value: $"Username: {reader[name: "username"]}");
                    Console.WriteLine(value: $"Password: {reader[name: "password"]}");
                    Console.WriteLine(value: $"Coins: {reader[name: "coins"]}");
                    Console.WriteLine(value: $"ELO: {reader[name: "elo"]}");
                    Console.WriteLine(value: $"Wins: {reader[name: "wins"]}");
                    Console.WriteLine(value: $"Draws: {reader[name: "draws"]}");
                    Console.WriteLine(value: $"Games: {reader[name: "games"]}");
                }
                else
                {
                    Console.WriteLine(value: "Пользователь не найден.");
                    
                }
            }
        }

        return newUser;

    }
    public List<Card> getCardsFromUser(int userId)
    {
        List<Card> cards = new List<Card>();
        string selectUserCardsQuery =
            "SELECT cards.* FROM stacks JOIN cards ON stacks.card_id = cards.card_id WHERE stacks.user_id = @user_id;";
        using (NpgsqlCommand command = new NpgsqlCommand(cmdText: selectUserCardsQuery, connection: connection))
        {
            command.Parameters.AddWithValue(parameterName: "@user_id", value: userId);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                List<Card> cardsIds = new List<Card>();


                // Если пользователь найден
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Прочитываем значения из результата запроса
                        int card_id = reader.GetInt32(reader.GetOrdinal("card_id"));
                        string Name = reader.GetString(reader.GetOrdinal("card_name"));
                        string Category = reader.GetString(reader.GetOrdinal("card_category"));
                        int Damage = Convert.ToInt32(reader.GetString(reader.GetOrdinal("card_damage")));
                        bool Spell;
                        if (!string.IsNullOrEmpty(Category = "Spell"))
                        {
                            Spell = true;
                        }
                        else
                        {
                            Spell = false;
                        }

                        string Type = reader.GetString(reader.GetOrdinal("card_type"));

                        // Создаем объект Card и добавляем его в список
                        Card card = new Card(card_id, Name, Category, Damage, Spell, Type);
                        cards.Add(card);
                    }
                }
                else
                {
                    // Если результат запроса пуст, выполните соответствующие действия
                    Console.WriteLine("No rows found.");
                }

            }
        }

        return cards;
    }
    public int registerNewUSer(string username, string password)
    {
        try
        {
            bool isAdmin = username == "admin";

            User returnUser = new User(username, password);
            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery =
                "INSERT INTO users (username, password, coins, ELO, wins, draws, games, admin) VALUES (@username, @password, @coins, @ELO, @wins, @draws, @games, @admin) RETURNING user_id";
        
            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                // Добавление параметров
                command.Parameters.AddWithValue("@username", returnUser.Username);
                command.Parameters.AddWithValue("@password", returnUser.Password);
                command.Parameters.AddWithValue("@ELO", returnUser.Elo);
                command.Parameters.AddWithValue("@coins", returnUser.Coins);
                command.Parameters.AddWithValue("@wins", returnUser.Wins);
                command.Parameters.AddWithValue("@draws", returnUser.Draws);
                command.Parameters.AddWithValue("@games", returnUser.Games);
                command.Parameters.AddWithValue("admin", isAdmin);

                return (int)command.ExecuteScalar();
            }
        }
        catch (Exception e)
        {
            int returnValue = 0;
            Console.WriteLine(e.Data["SqlState"].ToString());

            if (e.Data["SqlState"].ToString() == 23505.ToString())
            {
                returnValue = -5;
            }

            return returnValue;
        }
    }
    public int IsValidUser(string username, string password, User user)
    {
        string selectUserQuery = "SELECT * FROM users WHERE username = @username";

        using (NpgsqlCommand command = new NpgsqlCommand(selectUserQuery, connection))
        {
            // Добавление параметра для поиска
            command.Parameters.AddWithValue("@username", username);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string storedPassword = reader["password"].ToString(); 

                    // Сравнение введенного пароля с хешем из базы данных
                    if (storedPassword == password)
                    {
                        Console.WriteLine("Пароль верный. Вход разрешен.");
                        user = new User(User_id: (int)reader[name: "user_id"],
                            username: reader[name: "username"].ToString(),
                            password: reader[name: "password"].ToString(), coins: (int)reader[name: "coins"],
                            elo: (int)reader[name: "elo"], draws: (int)reader[name: "draws"],
                            wins: (int)reader[name: "wins"], games: (int)reader[name: "games"]);
                        return 0;
                    }
                    Console.WriteLine("Неверный пароль. Вход запрещен.");
                    return -1;
                }
                Console.WriteLine("Пользователь не найден.");
                return -2;
            }
        }
    }
    public void authenticationLogin(int auth, string username)
    {
        try
        {
            DateTime dateTime = DateTime.Now;
            string insertQuery =
                "INSERT INTO tokens (username, token, login, logout) VALUES (@username, @auth, @current_date, NULL);";
            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@auth", auth);
                command.Parameters.AddWithValue("@current_date", dateTime);
                command.ExecuteNonQuery();
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

    }
    public int changeUsername(string usernameBefore, string usernameToChange)
    {
        int result = 0;

        try
        {
            string updateQuery = "UPDATE users SET username = @usernameToChange WHERE username = @usernameBefore;";
            using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(updateQuery, connection))
            {
                npgsqlCommand.Parameters.AddWithValue("@usernameBefore", usernameBefore);
                npgsqlCommand.Parameters.AddWithValue("@usernameToChange", usernameToChange);
                result = npgsqlCommand.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            if (e.Data["SqlState"].ToString() == 23505.ToString())
            {
                return -5;
            }

            throw;
        }

        return result;
    }
    public int changePassword(string username, string passwordToChange)
    {
        int result = 0;

        try
        {
            string updateQuery = "UPDATE users SET password = @passwordToChange WHERE username = @username;";
            using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(updateQuery, connection))
            {
                npgsqlCommand.Parameters.AddWithValue("@passwordToChange", passwordToChange);
                npgsqlCommand.Parameters.AddWithValue("@username", username);
                result = npgsqlCommand.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            if (e.Data["SqlState"].ToString() == 23505.ToString())
            {
                return -5;
            }

            throw;
        }

        return result;
    }
    public int addCardToDb(string name, string category, string type, int damage)
    {
        try
        {
            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery =
                "INSERT INTO cards (card_name, card_category, card_type, card_damage) VALUES (@name, @category, @type, @damage) RETURNING card_id";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                // Добавление параметров
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@damage", damage);
            
                return (int)command.ExecuteScalar();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    public int addPackageToDb(List<Card> packageList)
    {
        List<int> Ids = new List<int>();
        foreach (var card in packageList)
        {
            Ids.Add(card.card_id);
        }
        
        try
        {
            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery =
                "INSERT INTO packages (card1_id, card2_id, card3_id, card4_id, card5_id, inStore) VALUES (@card1, @card2, @card3, @card4, @card5, @inStore) RETURNING package_id";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                int i = 1;
                foreach (var card in packageList)
                {
                    command.Parameters.AddWithValue("@card" + i, card.card_id);
                    i++;
                }

                command.Parameters.AddWithValue("@inStore", true);
            
                return (int)command.ExecuteScalar();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
        
    }
    public int buyPackage(string username, int package_id)
    {
        try
        {
            int card1_id;
            int card2_id;
            int card3_id;
            int card4_id;
            int card5_id;
            int coins = getUser(username).Coins;
            int user_id = getUser(username).user_id;
            if (coins < 5)
            {
                return - 1;                
            }
            string selectQuery = "SELECT * from packages WHERE package_id = @package_id";
            using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(selectQuery, connection))
            {
                npgsqlCommand.Parameters.AddWithValue("@package_id", package_id);
                using (NpgsqlDataReader reader = npgsqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        card1_id = Convert.ToInt32(reader[name: "card1_id"]);
                        card2_id = Convert.ToInt32(reader[name: "card2_id"]);
                        card3_id = Convert.ToInt32(reader[name: "card3_id"]);
                        card4_id = Convert.ToInt32(reader[name: "card4_id"]);
                        card5_id = Convert.ToInt32(reader[name: "card5_id"]);
                    }
                    else
                    {
                        return -2;
                    }
                }
            }

            List<int> cardIds = new List<int>(){ card1_id, card2_id, card3_id, card4_id, card5_id};
            int result = 0;
            foreach (var card_id in cardIds)
            {
                string insertQuery =
                    "INSERT INTO stacks (user_id, card_id) VALUES (@user_id, @card_id) RETURNING stack_id";
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@user_id", user_id);
                    command.Parameters.AddWithValue("@card_id", card_id);

                     result += (int)command.ExecuteScalar();
                }
            }
            
            string updateQuery = "UPDATE users SET coins = @coins WHERE username = @username;";
            using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(updateQuery, connection))
            {
                npgsqlCommand.Parameters.AddWithValue("@coins", coins - 5);
                npgsqlCommand.Parameters.AddWithValue("@username", username);
                npgsqlCommand.ExecuteNonQuery();
            }

            string updatePackageQuery = "UPDATE packages SET inStore = false WHERE package_id = @package_id;";
            using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(updatePackageQuery, connection))
            {
                npgsqlCommand.Parameters.AddWithValue("@package_id", package_id);
                npgsqlCommand.ExecuteNonQuery();
            }
            
            return result;


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool isAdminUser(string username)
    {
        bool returnValue = false;
        try
        {
            string selectUserQuery = "SELECT * FROM users WHERE username = @username";
            using (NpgsqlCommand command = new NpgsqlCommand(cmdText: selectUserQuery, connection: connection))
            {
                // Добавление параметра для поиска
                command.Parameters.AddWithValue(parameterName: "@username", value: username);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    // Если пользователь найден
                    if (reader.Read())
                    {
                        Console.WriteLine(value: $"admin: {reader[name: "admin"]}");
                        if (reader["admin"] == "f")
                        {
                            returnValue = false;
                        }
                        else
                        {
                            returnValue = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine(value: "Пользователь не найден.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return returnValue;
    }
}