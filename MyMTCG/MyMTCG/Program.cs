// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;
using MyMTCG;
using Newtonsoft.Json;
using Npgsql;

serverTry.start();
//
//     string serverAddress = "http://localhost:12345/users";
//
//     await SendGetRequest(serverAddress);
//     await SendPostRequest(serverAddress);
//
// static async Task SendGetRequest(string url)
// {
//     using (HttpClient client = new HttpClient())
//     {
//         HttpResponseMessage response = await client.GetAsync(url);
//
//         if (response.IsSuccessStatusCode)
//         {
//             string responseBody = await response.Content.ReadAsStringAsync();
//             Console.WriteLine($"GET Response from server: {responseBody}");
//         }
//         else
//         {
//             Console.WriteLine($"Error sending GET request. Status Code: {response.StatusCode}");
//         }
//     }
// }
//
// static async Task SendPostRequest(string url)
// {
//     using (HttpClient client = new HttpClient())
//     {
//         
//         string data = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
//         var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
//
//         HttpResponseMessage response = await client.PostAsync(url, content);
//
//         if (response.IsSuccessStatusCode)
//         {
//             string responseBody = await response.Content.ReadAsStringAsync();
//             Console.WriteLine($"POST Response from server: {responseBody}");
//         }
//         else
//         {
//             Console.WriteLine($"Error sending POST request. Status Code: {response.StatusCode}");
//         }
//     }
// }

// DataBaseCommands.DBCreateTables();
// bool ok = false;
// User user = new User();
// Console.Write("Welcome to Monter Traiding Card Game!");
// //while (true)
// {
//     while (!ok)
//     {
//         Console.WriteLine("\nEnter l for login and r for register: ");
//         string? input;
//         input = Console.ReadLine();
//         if (input == "l")
//         {
//             user = await User.login();
//             if (user == null)
//             {
//                 Console.WriteLine("Wrong username or password please try again");
//             }
//             else
//             {
//                 user.PrintUser();
//                 break;
//             }
//             
//         } 
//         else if (input == "r")
//         {
//             user = await User.register();
//         } 
//         else if (input == "q")
//         {
//             return;
//         }
//         else
//         {
//             Console.WriteLine("Please enter l for login, r for register or q for quit");
//             input = Console.ReadLine();
//         }
//     }
// }
// User user1 = new User("je", 1);
// user1.BuyPackage();
// // user.ChangePassword();
//user.BuyPackage();
// user.SetDeck();
// // user.BuyPackage();
// // user.ChangeDeck();
// user = await ServerRequests.getUser(user, "admin");
// user.PrintUser();


// try
// {
//     using (System.Net.Http.HttpClient client = new HttpClient())
//     {
//         var response = await client.GetAsync($"http://localhost:5268/user/users/{user.Username}");
//         response.EnsureSuccessStatusCode();
//         if (response.IsSuccessStatusCode)
//         {
//             string message = await response.Content.ReadAsStringAsync();
//             Console.WriteLine(message);
//             Console.ReadLine();
//
//         }
//         else
//         {
//             Console.WriteLine($"response error code: {response.StatusCode}");
//         }
//     }
//
//     Console.ReadLine();
//
//     Console.WriteLine("Hello, World!");
// }
// catch (HttpRequestException ex)
// {
//     Console.WriteLine($"HttpRequestException: {ex.Message}");
//     // Дополнительная обработка исключения, если необходимо
// }
// catch (IOException ex)
// {
//     Console.WriteLine($"IOException: {ex.Message}");
//     // Дополнительная обработка исключения, если необходимо
// }