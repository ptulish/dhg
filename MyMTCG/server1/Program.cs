using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using MyServer.Classes;
using Newtonsoft.Json.Linq;
using server1.DB;
using JsonSerializer = System.Text.Json.JsonSerializer;

class Program
{
    private static ConcurrentDictionary<string, TcpClient>
        activeClients = new ConcurrentDictionary<string, TcpClient>();

    private static ConnectionToDB connectionToDb = new ConnectionToDB();

    static void Main()
    {
        connectionToDb.OpenConnection();
        connectionToDb.initializeTables();

        DbCommands dbCommands = new DbCommands(connectionToDb);

        string url = "http://localhost:2345/";
        // HttpListener httpListener = new HttpListener();
        // httpListener.Prefixes.Add(url);

        var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2345);
        TcpListener listener = new(ipEndPoint);

        try
        {
            listener.Start();
            //httpListener.Start();
            Console.WriteLine($"Сервер запущен по адресу {url}");

            List<string> tokList = new List<string>();
            while (true)
            {

                TcpClient client = listener.AcceptTcpClient();
                Thread thread = new Thread(() => HandleRequest(client));
                thread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }

    static void HandleRequest(TcpClient tcpClient)
    {
        User user = null;
        NetworkStream networkStream = tcpClient.GetStream();
        DbCommands dbCommands = new DbCommands(connectionToDb);
        // Create a new instance of Random class
        Random random = new Random();

        // Generate a random integer
        int authenticationClient = random.Next();

        activeClients.TryAdd(authenticationClient.ToString(), tcpClient);


        using (BinaryReader binaryReader = new BinaryReader(networkStream))
        using (BinaryWriter binaryWriter = new BinaryWriter(networkStream))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            StringBuilder allData = new StringBuilder();

            while (networkStream.DataAvailable && (bytesRead = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                allData.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }

            Console.WriteLine($"Received data: {allData.ToString()}");

            // Получаем метод и путь запроса
            string[] infotmation;
            infotmation = getInofrmation(allData.ToString());
            int contentLength;
            string content = string.Empty;
            if (infotmation[1] == "")
            {
                contentLength = 0;
                content = string.Empty;
            }
            else
            {
                contentLength = Convert.ToInt32(infotmation[1]);
                content = infotmation[2];
            }
            string[] method = infotmation[0].Split(" ");
            string path = method[1];
            string httpMethod = method[0];
            string auththenticationUser = infotmation[3];
            if (user != null)
            {
                auththenticationUser = user.user_id + "-" + authenticationClient;
            }
            else
            {
                auththenticationUser = " -" + authenticationClient;
            }

            // Логика обработки запроса в зависимости от метода и пути
            if (httpMethod == "GET")
            {
                HandleGetRequest(path, binaryWriter, content, auththenticationUser, dbCommands, user);
            }
            else if (httpMethod == "POST")
            {
                HandlePostRequest(path, binaryWriter, content, auththenticationUser, dbCommands, user);
            }
            else if (httpMethod == "PUT")
            {
                HandlePutRequest(path, binaryWriter, content, auththenticationUser, dbCommands, user);
            }
            else if (httpMethod == "DELETE")
            {

            }
            // Добавьте обработку других методов (PUT, DELETE) при необходимости

            //auththenticationUser = user.user_id + "-" + authenticationClient;

        }

        // Закрываем поток
        networkStream.Close();
        tcpClient.Close();
    }

    public static string[] getInofrmation(string buffer)
    {
        string command = string.Empty;
        string length = string.Empty;
        string content = string.Empty;
        int i = 0;
        // Разделение HTTP-запроса на строки
        string[] lines = buffer.Split('\n');
        string auth = string.Empty;

        foreach (string line in lines)
        {
            if (line.StartsWith("Content-Length"))
            {
                string[] cntLng = line.Split(":");
                length = cntLng[1].Trim();

            }

            if (line.StartsWith("Autorization: "))
            {
                auth = line.Split(" ")[2];
            }
        }

        for (int j = 10; j < lines.Length; j++)
        {
            content += lines[j];
        }

        return new string[] { lines[0].Trim(), length, content, auth };
    }

    private static void HandlePostRequest(string path, BinaryWriter binaryWriter, string requestBody, string auth,
        DbCommands dbCommands, User user)
    {
        string[] pathStrings = path.Split('/', '?', '{', '}');
        
        switch (pathStrings[1])
        {
            case "users":
            {
                if (pathStrings.Length == 2)
                {
                    try
                    {
                        UserCred userCred = JsonSerializer.Deserialize<UserCred>(requestBody);
                        if (dbCommands.registerNewUSer(userCred.Username, userCred.Password) == -5)
                        {
                            string responseBody =
                                "HTTP/1.1 409 ERORR\r\nContent-Type: text/plain\r\n\r\nUser with same username already registered";
                            byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                            binaryWriter.Write(buffer);
                            binaryWriter.Flush();
                        }
                        else
                        {

                            string responseBody =
                                "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nData successfully retrieved";
                            byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                            binaryWriter.Write(buffer);
                            binaryWriter.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                break;
            }
            case "sessions":{UserCred userCred = JsonSerializer.Deserialize<UserCred>(requestBody);

                int result = dbCommands.IsValidUser(userCred.Username, userCred.Password, user);
                if (result < 0)
                {
                    string responseBody =
                        "HTTP/1.1 404 ERORR\r\nContent-Type: text/plain\r\n\r\nInvalid username/password provided";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                    return;
                }
                else
                {
                    string responseBody = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nUser succuesfully logged in";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                }
                dbCommands.authenticationLogin(Convert.ToInt32(auth.Split('-')[1]), userCred.Username);

                break;
            }
            case "packages":
            {
                string responseBody;
                byte[] buffer;

                if (pathStrings.Length == 3)
                {
                    Package package = new Package();
                    string username = pathStrings[2].Split('=', '?')[1];
                    int fromDB = package.createPackage(dbCommands, username);

                    if (fromDB == -1)
                    {
                        responseBody =
                            "HTTP/1.1 409 ERORR\r\nContent-Type: text/plain\r\n\r\nAt least one card in the packages already exists";
                        buffer = Encoding.UTF8.GetBytes(responseBody);
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();
                        return;
                    }
                    else if (fromDB == -2)
                    {
                        responseBody =
                            "HTTP/1.1 403 ERORR\r\nContent-Type: text/plain\r\n\r\nUser is not admin";
                        buffer = Encoding.UTF8.GetBytes(responseBody);
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();
                        return;
                    }
                    else
                    {
                        string responsecontent = JsonSerializer.Serialize<List<Card>>(package.PackageList);
                    
                    
                        responseBody =
                            "HTTP/1.1 201 OK\r\nContent-Type: application/json\r\n\r\nPackage and cards successfully created" + responsecontent;
                        buffer = Encoding.UTF8.GetBytes(responseBody);
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();

                    }
                    
                }
                break;
            }
            case "transactions":
            {
                string[] cred = pathStrings[3].Split("&");
                string username = cred[0].Split('=')[1];
                string password = cred[1].Split('=')[1];
                
                JObject jsonObject = JObject.Parse(requestBody);
                string package_id = jsonObject["package_id"]?.ToString();

                int result = dbCommands.buyPackage(username, Convert.ToInt32(package_id));

                if (result == -1)
                {
                    var responseBody = "HTTP/1.1 403 OK\r\nContent-Type: text/plain\r\n\r\nNot enough money for buying a card package";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                } else if (result == -2)
                {
                    var responseBody = "HTTP/1.1 404 OK\r\nContent-Type: text/plain\r\n\r\nPackage not found";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                }
                else if (result > 0)
                {
                    var responseBody = "HTTP/1.1 403 OK\r\nContent-Type: text/plain\r\n\r\nNot enough money for buying a card package";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                }
                
                break;
            }
            case "battles":
            {
                break;
            }
            case "tradings":
            {
                break;
            }
        }
    }

    private static void HandleGetRequest(string path, BinaryWriter binaryWriter, string requestBody, string auth,
        DbCommands dbCommands, User user)
    {
        string[] pathStrings = path.Split('/', '?', '{', '}');
        pathStrings = pathStrings.Select(s => s.Replace("%7D", "}")).ToArray();
        pathStrings = pathStrings.Select(s => s.Replace("%7B", "{")).ToArray();

        switch (pathStrings[1])
        {
            case "users":
            {
                try
                {
                    // Ищем позиции открывающей и закрывающей фигурных скобок
                    int startIndex = pathStrings[2].IndexOf('{') + 1;
                    int endIndex = pathStrings[2].IndexOf('}');

                    // Извлекаем подстроку между фигурными скобками
                    string username = pathStrings[2].Substring(startIndex, endIndex - startIndex);


                    user = dbCommands.getUser(username);
                    if (user == null)
                    {
                        string responseBody =
                            "HTTP/1.1 404 ERORR\r\nContent-Type: text/plain\r\n\r\nUser not found";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();
                    }
                    else
                    {
                        string content = JsonSerializer.Serialize<User>(user);
                        string responseBody =
                            "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\n\r\nData successfully retrieved" +
                            content;
                        byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                break;
            }
            case "cards":
            {
                
                break;
            }
        }
       
    }
    
    private static void HandleDeleteRequest(string path, HttpListenerResponse response, string requestBody)
    {
        throw new NotImplementedException();
    }

    private static void HandlePutRequest(string path, BinaryWriter binaryWriter, string requestBody, string auth, DbCommands dbCommands, User user)
    {
        string[] pathStrings = path.Split('/', '?', '{', '}');
        try
        {
            pathStrings = pathStrings.Select(s => s.Replace("%7D", "}")).ToArray();
            pathStrings = pathStrings.Select(s => s.Replace("%7B", "{")).ToArray();


            if (pathStrings[1] == "users")
            {
                // Ищем позиции открывающей и закрывающей фигурных скобок
                int startIndex = pathStrings[2].IndexOf('{') + 1;
                int endIndex = pathStrings[2].IndexOf('}');

                // Извлекаем подстроку между фигурными скобками
                string username = pathStrings[2].Substring(startIndex, endIndex - startIndex);
                JObject jsonObject = JObject.Parse(requestBody);
                string usernameToChange = jsonObject["Username"]?.ToString();
                string passwordToChange = jsonObject["Password"]?.ToString();
                int result = 0;
                if (usernameToChange != null) 
                {
                    result = dbCommands.changeUsername(username, usernameToChange);
                } 
                else if (passwordToChange != null)
                {
                    result = dbCommands.changePassword(username, passwordToChange);
                }
                if (result == -5)
                {
                    string responseBody =
                        "HTTP/1.1 410 ERORR\r\nContent-Type: text/plain\r\n\r\nUsername already in use";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                } 
                else if (result == 0)
                {
                    string responseBody =
                        "HTTP/1.1 404 ERORR\r\nContent-Type: text/plain\r\n\r\nUser not found";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                } else if (result == 1) 
                {
                    string responseBody =
                        "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nUser succuesfully updated";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                }
            }
            else if (pathStrings[1] == "deck")
            {
            
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

    }
}


public class UserCred
{
    public string Username { get; set; }
    public string Password { get; set; }

}