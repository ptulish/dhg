using Npgsql;

namespace MyMTCG;

public static class DataBaseCommands
{
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";

    public static bool DBLogin(string login, string password)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Параметризованный SQL-запрос для поиска пользователя по логину
            string selectUserQuery = "SELECT * FROM users WHERE username = @username";

            using (NpgsqlCommand command = new NpgsqlCommand(selectUserQuery, connection))
            {
                // Добавление параметра для поиска
                command.Parameters.AddWithValue("@username", login);

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
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль. Вход запрещен.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Пользователь не найден.");
                    }
                }
            }


            return true;
        }
    }

    public static int DBCreateUser(User user)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery =
                "INSERT INTO users (username, password, coins, ELO, wins, draws, games) VALUES (@username, @password, @coins, @ELO, @wins, @draws, @games) RETURNING user_id";



            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                // Добавление параметров
                command.Parameters.AddWithValue("@username", user.username);
                command.Parameters.AddWithValue("@password", user.password);
                command.Parameters.AddWithValue("@ELO", user.elo);
                command.Parameters.AddWithValue("@coins", user.coins);
                command.Parameters.AddWithValue("@wins", user.wins);
                command.Parameters.AddWithValue("@draws", user.draws);
                command.Parameters.AddWithValue("@games", user.games);

                return (int)command.ExecuteScalar();
            }
        }

    }

    public static void DBSetDeck(List<Card> MyStack, int intInput, int user_id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string insertQuery = "INSERT INTO decks (user_id, card_id) VALUES (@value1, @value2)";
            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("value2", MyStack[intInput - 1].card_id);
                command.Parameters.AddWithValue("value1", user_id);

                command.ExecuteNonQuery();
            }

            string deleteQuery = "DELETE FROM stacks WHERE card_id = @card_id";
            using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("card_id", MyStack[intInput - 1].card_id);

                command.ExecuteNonQuery();
            }
        }
    }

    public static void DBCreateTables()
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Создание таблицы
            string createUsersTableQuery = "CREATE TABLE IF NOT EXISTS users (" +
                                           "user_id SERIAL PRIMARY KEY," +
                                           "username VARCHAR(255)," +
                                           "password VARCHAR(255)," +
                                           "coins INTEGER," +
                                           "ELO INTEGER," +
                                           "wins INTEGER," +
                                           "draws INTEGER," +
                                           "games INTEGER)";
            using (NpgsqlCommand command = new NpgsqlCommand(createUsersTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            // Создание таблицы cards (предположим, что у вас есть таблица cards)
            string createCardsTableQuery = "CREATE TABLE IF NOT EXISTS cards (" +
                                           "card_id SERIAL PRIMARY KEY," +
                                           "card_name VARCHAR(255)," +
                                           "card_category VARCHAR(255)," +
                                           "card_Type VARCHAR(255)," +
                                           "card_damage VARCHAR(255))";
            using (NpgsqlCommand command = new NpgsqlCommand(createCardsTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            // Создание таблицы decks
            string createDecksTableQuery = "CREATE TABLE IF NOT EXISTS decks (" +
                                           "deck_id SERIAL PRIMARY KEY," +
                                           "user_id INTEGER REFERENCES users(user_id)," +
                                           "card_id INTEGER REFERENCES cards(card_id))";
            using (NpgsqlCommand command = new NpgsqlCommand(createDecksTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            // Создание таблицы stacks
            string createStacksTableQuery = "CREATE TABLE IF NOT EXISTS stacks (" +
                                            "stack_id SERIAL PRIMARY KEY," +
                                            "user_id INTEGER REFERENCES users(user_id)," +
                                            "card_id INTEGER REFERENCES cards(card_id))";
            using (NpgsqlCommand command = new NpgsqlCommand(createStacksTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

    }

    public static void DBChangeDeck(int cardIdFromStack, int cardIdFromDeck, int userId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string deleteQueryStack = "DELETE FROM stacks WHERE card_id = @card_id";
            using (NpgsqlCommand command = new NpgsqlCommand(deleteQueryStack, connection))
            {
                command.Parameters.AddWithValue("card_id", cardIdFromStack);

                command.ExecuteNonQuery();
            }
            string deleteQueryDeck = "DELETE FROM decks WHERE card_id = @card_id";
            using (NpgsqlCommand command = new NpgsqlCommand(deleteQueryDeck, connection))
            {
                command.Parameters.AddWithValue("card_id", cardIdFromDeck);

                command.ExecuteNonQuery();
            }
            string insertQueryDeck = "INSERT INTO decks (card_id, user_id) VALUES (@value1, @value2)";
            using (NpgsqlCommand command = new NpgsqlCommand(insertQueryDeck, connection))
            {
                command.Parameters.AddWithValue("value1", cardIdFromStack);
                command.Parameters.AddWithValue("value2", userId);

                command.ExecuteNonQuery();
            }
            string insertQueryStack = "INSERT INTO stacks (card_id, user_id) VALUES (@value1, @value2)";
            using (NpgsqlCommand command = new NpgsqlCommand(insertQueryStack, connection))
            {
                command.Parameters.AddWithValue("value1", cardIdFromDeck);
                command.Parameters.AddWithValue("value2", userId);

                command.ExecuteNonQuery();
            }
        }
    }

    public static void DBBuyPackage(int user_id, int card_id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery = "INSERT INTO stacks (user_id, card_id) VALUES ( @userId, @cardID)";

            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                // Добавление параметров
                command.Parameters.AddWithValue("@userId", user_id);
                command.Parameters.AddWithValue("@cardID", card_id);

                // Выполнение запроса
                command.ExecuteNonQuery();
            }
        }
    }

    public static void DBChangeUsername(string input, int user_id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string updateQuery = "UPDATE users SET username = @newValue WHERE user_id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("newValue", input);
                command.Parameters.AddWithValue("id", user_id);

                command.ExecuteNonQuery();
            }
        }
    }
    
    public static void DBCangePassword(string input, int userId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string updateQuery = "UPDATE users SET password = @newValue WHERE user_id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("newValue", input);
                command.Parameters.AddWithValue("id", userId);

                command.ExecuteNonQuery();
            }
        }
    }
}
    