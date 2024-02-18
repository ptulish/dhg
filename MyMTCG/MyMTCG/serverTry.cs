using System.Net.Sockets;
using System.Text;

namespace MyMTCG;

public class serverTry
{
    public static async void start()
    {
        Console.Write("Enter your name: ");
        string clientName = Console.ReadLine();

        TcpClient client = new TcpClient("127.0.0.1", 12345);

        try
        {
            NetworkStream stream = client.GetStream();

            // Отправляем серверу имя клиента
            byte[] nameBytes = Encoding.ASCII.GetBytes($"Name:{clientName}\n");
            await stream.WriteAsync(nameBytes, 0, nameBytes.Length);

            _ = Task.Run(async () => await ReceiveMessagesAsync(client));

            while (true)
            {
                // Отправляем сообщение серверу
                Console.Write("Enter message: ");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.ASCII.GetBytes($"Message:{message}\n");
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    static async Task ReceiveMessagesAsync(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[2048];

            while (true)
            {
                
                // Читаем сообщения от сервера и других клиентов
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                    break;

                string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                int currentLine = Console.CursorTop;

                Console.SetCursorPosition(0, currentLine);
                Console.Write(new string(' ', Console.WindowWidth));

                // Возвращаем курсор на начало
                Console.SetCursorPosition(0, currentLine);
                Console.WriteLine($"Received: {receivedMessage}");
                
                Console.WriteLine("Enter your message: ");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }
    // public static void start()
    // {
    //     string serverIpAddress = "127.0.0.1"; // Замените на IP вашего сервера
    //     int serverPort = 12345;
    //
    //     try
    //     {            
    //         TcpClient client = new TcpClient(serverIpAddress, serverPort);
    //
    //         NetworkStream stream = client.GetStream();
    //
    //         Console.WriteLine("Connected to the server!");
    //         while (true)
    //         {
    //             
    //             // Отправляем сообщение на сервер
    //             string message = Console.ReadLine();
    //             
    //             byte[] data = Encoding.ASCII.GetBytes(message);
    //             stream.Write(data, 0, data.Length);
    //
    //             // Читаем ответ от сервера
    //             byte[] buffer = new byte[1024];
    //             int bytesRead = stream.Read(buffer, 0, buffer.Length);
    //             string serverResponse = Encoding.ASCII.GetString(buffer, 0, bytesRead);
    //             Console.WriteLine($"Server response: {serverResponse}");
    //         }
    //         // Закрываем соединение
    //         stream.Close();
    //         client.Close();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }
    // }
}