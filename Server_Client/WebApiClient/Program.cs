// See https://aka.ms/new-console-template for more information

using System.Net.Http;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("press any key cont...");
            Console.ReadLine();
            using (System.Net.Http.HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("http://localhost:5059/values");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(message);
                }
                else
                {
                    Console.WriteLine($"response error code: {response.StatusCode}");
                }
            }

            Console.ReadLine();

            Console.WriteLine("Hello, World!");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HttpRequestException: {ex.Message}");
            // Дополнительная обработка исключения, если необходимо
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IOException: {ex.Message}");
            // Дополнительная обработка исключения, если необходимо
        }
        
    }

}


