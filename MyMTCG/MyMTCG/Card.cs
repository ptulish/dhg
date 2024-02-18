using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Npgsql;

namespace MyMTCG;

public class Card
{
    string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";

    public int card_id { get; set; }
    public string name { get; set; }
    public string category { get; set; }
    public int damage { get; set; }

    public bool spell
    {
        get;
        set;
    }

    public string type { get; set; }
    
    public Card(int i)
    {
        
    }
    
    public Card()
    {
        Random random = new Random();

        // Генерируем случайное число в заданном диапазоне
        int randomNumber = random.Next(2);
        if (randomNumber == 0)
        {
            category = "Monster";
            spell = false;
        }
        else
        {
            category = "Spell";
            spell = true;
        }

        randomNumber = random.Next(5);
        switch (randomNumber)
        {
            case 0:
                type = "Water";
                break;
            case 1:
                type = "Fire";
                break;
            default:
                type = "Normal";
                break;
        }

        if (category == "Spell")
        {
            switch (type)
            {
                case "Water":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Aqua Cascade";
                            damage = random.Next(0, 51);
                            break;
                        case 1:
                            name = "Hydro Vortex";
                            damage = random.Next(10, 61);
                            break;
                        case 2:
                            name = "Nimbus Torrent";
                            damage = random.Next(5, 56);
                            break;
                        case 3:
                            name = "Aqueduct Surge";
                            damage = random.Next(15, 66);
                            break;
                        case 4:
                            name = "Mystic Deluge";
                            damage = random.Next(20, 71);
                            break;
                        case 5:
                            name = "Ripple Wave";
                            damage = random.Next(8, 59);
                            break;
                        case 6:
                            name = "Tsunami Veil";
                            damage = random.Next(25, 76);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Fire":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Inferno Blaze";
                            damage = random.Next(5, 55);
                            break;
                        case 1:
                            name = "Pyroburst";
                            damage = random.Next(15, 65);
                            break;
                        case 2:
                            name = "Ember Surge";
                            damage = random.Next(8, 59);
                            break;
                        case 3: 
                            name = "Flamewave Torrent";
                            damage = random.Next(20, 70);
                            break;
                        case 4:
                            name = "Ingition Burst";
                            damage = random.Next(25, 76);
                            break;
                        case 5:
                            name = "Scorching Cyclone";
                            damage = random.Next(12, 63);
                            break;
                        case 6:
                            name = "Firestorm Fury";
                            damage = random.Next(30, 80);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Normal":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Elemental Strike";
                            damage = random.Next(3, 48);
                            break;
                        case 1:
                            name = "Nature's Wrath";
                            damage = random.Next(12, 58);
                            break;
                        case 2:
                            name = "Terra Surge";
                            damage = random.Next(7, 53);
                            break;
                        case 3:
                            name = "Gale Slash";
                            damage = random.Next(15, 61);
                            break;
                        case 4:
                            name = "Aurora Burst";
                            damage = random.Next(18, 64);
                            break;
                        case 5:
                            name = "Quake Pulse";
                            damage = random.Next(10, 56);
                            break;
                        case 6:
                            name = "Radiant Surge";
                            damage = random.Next(20, 70);
                            break;
                        default:
                            break;
                    }
                    break;
                default:

                    break;
            }
        }

        if (category == "Monster")
        {
            switch (type)
            {
                case "Water":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Water Goblin";
                            damage = random.Next(20, 40);
                            break;
                        case 1:
                            name = "Aqua Specter";
                            damage = random.Next(15, 87);
                            break;
                        case 2:
                            name = "Tidal Leviathan";
                            damage = random.Next(30, 77);
                            break;
                        case 3:
                            name = "Frostbite Kraken";
                            damage = random.Next(18, 55);
                            break;
                        case 4:
                            name = "Ripple Behemoth";
                            damage = random.Next(40, 80);
                            break;
                        case 5:
                            name = "Torrential Naga";
                            damage = random.Next(25, 70);
                            break;
                        case 6:
                            name = "Quicksilver Hydra";
                            damage = random.Next(30, 68);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Fire":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Fire Elves";
                            damage = random.Next(37, 65);
                            break;
                        case 1:
                            name = "Ember Dragon";
                            damage = random.Next(25, 50);
                            break;
                        case 2:
                            name = "Inferno Phoenix";
                            damage = random.Next(20, 46);
                            break;
                        case 3:
                            name = "Flame Imp";
                            damage = random.Next(30, 56);
                            break;
                        case 4:
                            name = "Ignition Hound";
                            damage = random.Next(28, 64);
                            break;
                        case 5: 
                            name = "Pyro Wyvern";
                            damage = random.Next(23, 49);
                            break;
                        case 6:
                            name = "Scorching Salamander";
                            damage = random.Next(25, 48);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Normal":
                    randomNumber = random.Next(7);
                    switch (randomNumber)
                    {
                        case 0:
                            name = "Earth Knight";
                            damage = random.Next(15, 56);
                            break;
                        case 1:
                            name = "Sylvan Wizzard";
                            damage = random.Next(30, 80);
                            break;
                        case 2:
                            name = "Stone Ork";
                            damage = random.Next(20, 61);
                            break;
                        case 3:
                            name = "Wind Sylph";
                            damage = random.Next(13, 43);
                            break;
                        case 4:
                            name = "Light Pixie";
                            damage = random.Next(18, 48);
                            break;
                        case 5:
                            name = "Quake Elemental";
                            damage = random.Next(12, 42);
                            break;
                        case 6:
                            name = "Celestial Unicorn";
                            damage = random.Next(25, 66);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            // Параметризованный SQL-запрос для вставки данных в таблицу decks
            string insertQuery = "INSERT INTO cards (card_name, card_category, card_type, card_damage) VALUES (@name, @category, @type, @damage) RETURNING card_id";
            
            using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
            {
                // Добавление параметров
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@damage", damage);

                // Выполнение запроса
                //command.ExecuteNonQuery();
                int insertedCardId = (int)command.ExecuteScalar();
                card_id = insertedCardId;
        
                // Теперь insertedCardId содержит значение card_id, которое было сгенерировано при вставке
                Console.WriteLine($"Вставлена запись с card_id: {insertedCardId}");
            }

            
        }
    }
    
}