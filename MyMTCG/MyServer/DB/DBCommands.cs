using System.Data;
using System.Diagnostics;
using MyServer.Classes;
using Npgsql;

namespace MyServer.DB;

public class DBCommands
{
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";

    public static User getUser(string username)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString: connectionString))
        {
            connection.Open();

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
        
    }
    internal static User IsValidUser(string username, string password)
    {
        User newUser;
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Параметризованный SQL-запрос для поиска пользователя по логину
            string selectUserQuery = "SELECT * FROM users WHERE username = @username";

            using (NpgsqlCommand command = new NpgsqlCommand(selectUserQuery, connection))
            {
                // Добавление параметра для поиска
                command.Parameters.AddWithValue("@username", username);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    // Если пользователь найден
                    if (reader.Read())
                    {
                        // Получение хеша пароля из базы данных
                        string storedPassword =
                            reader["password"]
                                .ToString(); // Предположим, что пароль хранится в виде строки в базе данных

                        // Сравнение введенного пароля с хешем из базы данных
                        if (storedPassword == password)
                        {
                            Console.WriteLine("Пароль верный. Вход разрешен.");
                            newUser = new User(User_id: (int)reader[name: "user_id"],
                                username: reader[name: "username"].ToString(),
                                password: reader[name: "password"].ToString(), coins: (int)reader[name: "coins"],
                                elo: (int)reader[name: "elo"], draws: (int)reader[name: "draws"],
                                wins: (int)reader[name: "wins"], games: (int)reader[name: "games"]);
                            return newUser;
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль. Вход запрещен.");
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Пользователь не найден.");
                    }
                }
            }
        }
        
        // Реализуйте здесь свою логику для проверки учетных данных
        // Например, сравнение с данными в базе данных
        // Это просто пример, реальная логика будет зависеть от вашей системы
        return null;
    }

    public static List<Card> getCardsFromUser(int userId)
    {
        List<Card> cards = new List<Card>();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string selectUserCardsQuery = "SELECT cards.* FROM stacks JOIN cards ON stacks.card_id = cards.card_id WHERE stacks.user_id = @user_id;";
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

        }

        return cards;
    }


    public static DataTable GetCardsByUserId(int userId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("get_cards_by_user_id", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("user_id_param", userId);

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }

    public static int registerNewUSer(string username, string password)
    {
        User returnUser = new User(username, password);
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery =
                "INSERT INTO users (username, password, coins, ELO, wins, draws, games) VALUES (@username, @password, @coins, @ELO, @wins, @draws, @games) RETURNING user_id";



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

                return (int)command.ExecuteScalar();
            }
        }
        
    }
}