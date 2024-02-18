// See https://aka.ms/new-console-template for more information

using MTCGServer;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyServer.Classes;

User user = new User("1", "1");
Console.WriteLine(user.user_id);

// class Program
// {
//     static async Task Main()
//     {
//         int serverPort = 12345;
//
//         TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), serverPort);
//         server.Start();
//
//         Console.WriteLine($"Server is listening on port {serverPort}");
//
//         
//
//         try
//         {
//             while (true)
//             {
//                 TcpClient client = await server.AcceptTcpClientAsync();
//
//                 // Запускаем новый поток для обработки клиента
//                 Thread thread = new Thread(async () => await HandleClientAsync(client));
//                 thread.Start();
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error: {ex.Message}");
//         }
//         finally
//         {
//             server.Stop();
//         }
//     }
//
//     static async Task HandleClientAsync(TcpClient client)
//     {
//         try
//         {
//             Console.WriteLine($"Client connected: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");
//
//             NetworkStream stream = client.GetStream();
//
//             // Асинхронный код обработки клиента
//             byte[] buffer = new byte[1024];
//             int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//             string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
//             Console.WriteLine($"Received from client: {clientMessage}");
//
//             // Асинхронный пример отправки данных клиенту
//             string responseMessage = "Hello, client!";
//             byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
//             await stream.WriteAsync(responseData, 0, responseData.Length);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error handling client: {ex.Message}");
//         }
//         finally
//         {
//             client.Close();
//         }
//     }
// }



