using Npgsql;

namespace MyServer.DB;

public class TableInit
{
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";
    
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

            Console.WriteLine("Tables checked");
        }

    }
}