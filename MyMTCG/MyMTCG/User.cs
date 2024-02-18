using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Windows.Markup;
using Newtonsoft.Json;
using Npgsql;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MyMTCG;

public class User
{
    string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mydb";
    public int user_id { get; set; }
    public string? username { get; set; }
    public string password { get; set; }
    public int coins { get; set; }
    public int elo { get; set; }
    public int wins { get; set; }
    public int draws { get; set; }
    public int games { get; set; }
    [JsonPropertyName("deck")] public List<Card> deck { get; set; }

    [JsonPropertyName("myStack")]
    public List<Card> myStack { get; set; }
    

    public User(User user)
    {
        user_id = user.user_id;
        username = user.username;
        password = user.password;
        coins = user.coins;
        deck = user.deck;
        myStack = user.myStack;
        elo = user.elo;
        wins = user.wins;
        draws = user.draws;
        games = user.games;

        //user_id = DataBaseCommands.DBCreateUser(this);
    }

    public User(int user_id, string username, string password, int coins, List<Card> deck, List<Card> myStack, int elo,
        int wins, int draws, int games)
    {
        this.user_id = user_id;
        this.username = username;
        this.password = password;
        this.coins = coins;
        this.deck = deck;
        this.myStack = myStack;
        this.elo = elo;
        this.wins = wins;
        this.draws = draws;
        this.games = games;
    }

    public User(int register)
    {
        string? input;
        bool ok = false;
        Console.Write("Please choose your username: ");
        while (ok == false)
        {
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                username = input;
                ok = true;
            }
            else
            {
                Console.Write("Username cannot be empty. Please enter new username: ");
            }
        }

        ok = false;

        Console.Write("Please choose Your password: ");
        while (ok == false)
        {
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                password = input;
                ok = true;
            }
            else
            {
                Console.Write("Password cannot be empty. Please enter new password: ");
            }
        }

        ok = false;
        coins = 20;
        elo = 100;
        wins = 0;
        draws = 0;
        games = 0;
        deck = new List<Card>();
        myStack = new List<Card>();
    }

    public User()
    {
        
    }

    public User(string json)
    {
        // Десериализация JSON-строки в объект User
        User? userFromJson = System.Text.Json.JsonSerializer.Deserialize<User>(json);
        Console.Write(json);
        
        // Инициализация свойств текущего объекта на основе свойств объекта, полученного из JSON
        user_id = userFromJson.user_id;
        username = userFromJson.username;
        password = userFromJson.password;
        coins = userFromJson.coins;
        elo = userFromJson.elo;
        wins = userFromJson.wins;
        draws = userFromJson.draws;
        games = userFromJson.games;
        deck = userFromJson.deck;
        myStack = userFromJson.myStack;
        //Console.WriteLine($"{json["user_id"]}");
    }

    public User(string hello, int i)
    {
        username = "admin123";
        password = "Password";
        coins = 20;
        deck = new List<Card>();
        myStack = new List<Card>();
        elo = 100;
        wins = 0;
        draws = 0;
        games = 0;

        user_id = DataBaseCommands.DBCreateUser(this);
    }

    public void PrintUser()
    {
        Console.Write("\n Your profile:" +
                      "\n Username: " + username + " " +
                      "\n Coins: " + coins + "" +
                      "\n ELO: " + elo + "" +
                      "\n Games played: " + games + "" +
                      "\n WIns: " + wins + "" +
                      "\n Draws: " + draws + "" +
                      "\n Enter d to see your deck, s for stack and q for main menu: ");
        string? input;
        bool ok = false;
        while (!ok)
        {
            input = Console.ReadLine();
            if (input == "d")
            {
                PrintCards(deck, "Deck");
                Console.Write("\n Enter d to see your deck, s for stack and q for main menu: ");
            }
            else if (input == "s")
            {
                PrintCards(myStack, "Stack");
                Console.Write("\n Enter d to see your deck, s for stack and q for main menu: ");
            }
            else if (input == "q")
            {
                return;
            }
            else
            {
                Console.Write(" Wrong input, please use d, s or q: ");
            }
        }

    }

    private void PrintCards(List<Card>? cards, string type)
    {
        int i = 1;
        if (cards != null && cards.Count != 0)
            foreach (var card in cards)
            {
                Console.WriteLine("#" + i + " Card name: " + card.name + ", category: " + card.category + ", type: " +
                                  card.type + ", damage: " + card.damage + " ");
                i++;
            }
        else
        {
            Console.Write(" You do not have any cards in your " + type);
        }
    }

    public void SetDeck()
    {
        int i;
        if (this.deck.Count != 0)
        {
            Console.WriteLine("Here is your deck");
            PrintCards(deck, "deck");
        }

        Console.WriteLine("Here are Cards in your stack: ");

        for (int j = 0; j < 4; j++)
        {
            i = 1;
            foreach (var card in myStack)
            {
                Console.WriteLine("Card #" + i + ": " + card.name);
                i++;
            }

            Console.Write("Which cards do you want to add to your Stack, or q for main menu? ");
            bool ok = false;
            while (ok == false)
            {
                string input = Console.ReadLine();
                if (input == "q")
                {
                    Console.WriteLine("Here is your deck: ");
                    PrintCards(deck, "deck");
                    return;
                }

                int intInput;
                int.TryParse(input, out intInput);

                if (intInput > myStack.Count)
                {
                    Console.Write("You dont have a card with this number, please enter another: ");
                }
                else
                {
                    //DataBaseCommands.DBSetDeck(myStack, intInput, user_id);
                    deck.Add(myStack[intInput - 1]);
                    myStack.RemoveAt(intInput - 1);
                    ok = true;
                }
            }
        }

        ServerRequests.setDeck(deck);
    }

    public void ChangeDeck(User user)
    {
        string input;
        int fromStack = 0;
        int fromDeck = 0;
        Console.WriteLine("Here is your Deck: ");
        PrintCards(deck, "Deck");
        if (deck.Count == 0)
        {
            Console.WriteLine("You have to set deck at first.");
            user.SetDeck();
            return;
        }

        while (true)
        {
            Console.WriteLine("Here is your Stack: ");
            PrintCards(myStack, "stack");


            bool ok = false;
            while (!ok)
            {
                Console.Write("Take a card from Stack: ");
                input = Console.ReadLine();
                int.TryParse(input, out fromStack);
                Console.Write("Card taken: " + myStack[fromStack - 1].name + " is it right? y for yes and n for no: ");
                input = Console.ReadLine();
                if (input == "y")
                {
                    ok = true;
                }
            }

            ok = false;

            while (!ok)
            {
                Console.Write("Take a card from deck: ");
                input = Console.ReadLine();
                int.TryParse(input, out fromDeck);
                Console.Write("Card taken: " + deck[fromDeck - 1].name + " is it right? y for yes and n for no: ");
                input = Console.ReadLine();
                if (input == "y")
                {
                    ok = true;
                }
            }

            DataBaseCommands.DBChangeDeck(myStack[fromStack - 1].card_id, deck[fromDeck - 1].card_id, this.user_id);

            (deck[fromDeck - 1], myStack[fromStack - 1]) = (myStack[fromStack - 1], deck[fromDeck - 1]);
            Console.WriteLine("Now your Deck is: ");
            PrintCards(deck, "deck");
            Console.WriteLine("And your Stack is: ");
            PrintCards(myStack, "stack");
            Console.Write("Do you want to make another changes? ");
            input = Console.ReadLine();
            while (true)
            {
                if (input == "n")
                {
                    return;
                }
                else if (input == "y")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong input use y for yes and n for no.");
                }
            }
        }
    }

    public static async Task<User> login()
    {
        string inputUsername;
        string inputPassword;

        Console.Write("Please enter your username: ");
        inputUsername = Console.ReadLine();
        Console.Write("Please enter your password: ");
        inputPassword = Console.ReadLine();

        User user = await ServerRequests.loginUser(inputUsername, inputPassword);

        return user;
    }
    
    public static async Task<User> register()
    {
        User newUser = new User(1);
        
        var response = await ServerRequests.registerUser(newUser);

        newUser.user_id = response; 
        return newUser;
    }
    
    public User ChangeUsername()
    {
        bool ok = false;
        string? input = null;
        Console.Write("Choose a new username: ");
        while (ok == false)
        {
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                username = input;
                ok = true;
            }
            else
            {
                Console.Write("Username cannot be empty. Please enter new username: ");
            }
        }
        DataBaseCommands.DBChangeUsername(input, user_id);
        return this;
    }
    public User ChangePassword()
    {
        bool ok = false;
        string? input;
        Console.Write("Your old password: ");
        while (ok == false)
        {
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input) && input == password)
            {
                Console.WriteLine("Please choose your new password: ");
                while (ok == false)
                {
                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        password = input;
                        ok = true;
                        DataBaseCommands.DBCangePassword(input, user_id);
                    }
                    else
                    {
                        Console.Write("Password cannot be empty. Please enter new password: ");
                    }
                }
            }
            else if (input == "q")
            {
                return this;
            }
            else
            {
                Console.Write("Wrong Password please repeat, or q for quit: ");
            }
        }
        return this;
    }
    //TODO: add to database

    public void BuyPackage()
    {   
        for (int i = 0; i < 5; i++)
        {
            Card card = new Card();
            myStack.Add(card);
            Console.WriteLine("New card: name: " + card.name + " category: " + card.category  + " type: " + card.type + " damage: " + card.damage + " ");
            DataBaseCommands.DBBuyPackage(user_id, card.card_id);
        }

        coins -= 5;
        
        
    }
}