using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MTCGServer;

public class TcpServer
{
    public async void startServer()
    {
        int serverPort = 12345;

        TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), serverPort);
        server.Start();

        Console.WriteLine($"Server is listening on port {serverPort}");

        try
        {
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                // Запускаем новый поток для обработки клиента
                Thread thread = new Thread(HandleClientAsync);
                thread.Start(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            server.Stop();
        }
    }

    static void HandleClientAsync(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;

        try
        {
            Console.WriteLine($"Client connected: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

            NetworkStream stream = client.GetStream();

            // Ваш асинхронный код обработки клиента
            // Пример чтения данных от клиента
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received from client: {clientMessage}");

            // Пример отправки данных клиенту
            string responseMessage = "Hello, client!";
            byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
            stream.Write(responseData, 0, responseData.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
        // public void startServer()
        // {
        //     TcpListener? server;
        //     try
        //     {
        //         // Set the TcpListener on port 13000.
        //         Int32 port = 10001;
        //         IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        //
        //         // TcpListener server = new TcpListener(port);
        //         server = new TcpListener(localAddr, port);
        //
        //         // Start listening for client requests.
        //         server.Start();
        //
        //         // Buffer for reading data
        //         Byte[] bytes = new Byte[4096];
        //         string? data = null;
        //
        //         List<string>? Tokens = new List<string>();
        //
        //         while (true)
        //         {
        //             Console.Write("Waiting for a connection... ");
        //         
        //             TcpClient client = server.AcceptTcpClient();
        //             Console.WriteLine(client.Client.Connected);
        //             
        //             //Thread thread = new Thread(() => RequestHandle(client, bytes, data, Tokens, Lobbylist));
        //             //thread.Start();
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }
        // }
    }
}