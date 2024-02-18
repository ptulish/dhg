using Newtonsoft.Json;

namespace MyMTCG;

public class ServerRequests
{
    public static async Task<User> getUser(User user, string username)
    {
        using (HttpClient client = new HttpClient())
        {
            // Измените URL в соответствии с вашим маршрутом
            string apiUrl = $"http://localhost:5268/user/users/{username}";

            // Отправка GET-запроса
            var response = await client.GetAsync(apiUrl);

            // Проверка на успешный статус ответа
            if (response.IsSuccessStatusCode)
            {
                // Получение ответа в виде строки
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Десериализация JSON в объект User
                user = JsonConvert.DeserializeObject<User>(jsonResponse);

                // Используйте объект user по вашему усмотрению
                Console.WriteLine($"User ID: {user.user_id}, Username: {user.username}, Coins: {user.coins}");
            }
            else
            {
                // Если статус ответа не успешен, выводим код ошибки
                Console.WriteLine($"Response error code: {response.StatusCode}");
            }
        }

        return user;
    }

    public static async Task<User> loginUser(string? inputUsername, string? inputPassword)
    {
        User user = new User();
        using (HttpClient client = new HttpClient())
        {
            // Измените URL в соответствии с вашим маршрутом
            string apiUrl = "http://localhost:5268/user/seccions";

            // Подготовка параметров для отправки
            var parameters = new Dictionary<string, string>
            {
                { "username", inputUsername },
                { "password", inputPassword }
            };

            // Создание объекта FormUrlEncodedContent для передачи параметров
            var content = new FormUrlEncodedContent(parameters);

            // Отправка POST-запроса
            var response = await client.PostAsync(apiUrl, content);

            // Проверка на успешный статус ответа
            if (response.IsSuccessStatusCode)
            {
                // Получение ответа в виде строки
                string jsonResponse = await response.Content.ReadAsStringAsync();
                //user = JsonConvert.DeserializeObject<User>(jsonResponse);
                user = new User(jsonResponse);
                return user;
            }
        }

        return null;
    }

    public static async Task<int> registerUser(User newUser)
    {
        using (HttpClient client = new HttpClient())
        {
            // Измените URL в соответствии с вашим маршрутом
            string apiUrl = "http://localhost:5268/user/users";

            // Подготовка параметров для отправки
            var parameters = new Dictionary<string, string>
            {
                { "username", newUser.username},
                { "password", newUser.password },
            };

            // Создание объекта FormUrlEncodedContent для передачи параметров
            var content = new FormUrlEncodedContent(parameters);

            // Отправка POST-запроса
            var response = await client.PostAsync(apiUrl, content);

            // Проверка на успешный статус ответа
            if (response.IsSuccessStatusCode)
            {
                // Получение ответа в виде строки
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResponse); // Вывести ответ сервера (например, "User registration successful")
                return 9;
            }
            else
            {
                return 0;
            }
        }
    }

    public static async Task<List<Card>> getUserCards(int userId)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = $"http://localhost:5268/cards/cards/{userId}";

            // Отправка GET-запроса
            var response = await client.GetAsync(apiUrl);

            // Проверка на успешный статус ответа
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Получение ответа в виде строки
                List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(jsonResponse);

                // Now 'cards' contains the list of cards
                foreach (var card in cards)
                {
                    Console.WriteLine($"Card ID: {card.card_id}, Name: {card.name}, Category: {card.category}, Damage: {card.damage}, Spell: {card.spell}, Type: {card.type}");
                }
            }
            else
            {
                // Если статус ответа не успешен, выводим код ошибки
                Console.WriteLine($"Response error code: {response.StatusCode}");
            }
        }
        

        return new List<Card>();
    }

    public static async Task setDeck(List<Card> deck)
    {
        string json = JsonConvert.SerializeObject(deck, Formatting.Indented);
        Console.WriteLine(json);

        using (HttpClient httpClient = new HttpClient())
        {
            string apiUrl = "http://localhost:5268/deck";

        }
    }
}