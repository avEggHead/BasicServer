using System.Net;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace BasicServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:3721/";

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);

            listener.Start();
            Console.WriteLine($"Listening for requests at {url}");

            ThreadPool.QueueUserWorkItem(HandleRequests, listener);

            Console.ReadLine();

            listener.Stop();
        }

        static void HandleRequests(object state)
        {
            HttpListener listener = (HttpListener)state;

            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();

                ProcessRequest(context);
            }
        }

        static void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseString = string.Empty;

            switch (request.HttpMethod.ToUpper())
            {
                case "GET":
                    Console.WriteLine("GET request received at: " + DateTime.Now);
                    Console.WriteLine("From: " + request.RemoteEndPoint);
                    Console.WriteLine("URL: " + request.RawUrl);
                    responseString = "Hello, this is a simple C# HTTP server! you called get";
                    break;
                case "POST":
                    Console.WriteLine("POST request received at: " + DateTime.Now);
                    Console.WriteLine("From: " + request.RemoteEndPoint);
                    Console.WriteLine("URL: " + request.RawUrl);
                    string requestBody;
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                    var body = JsonConvert.DeserializeObject<SampleObject>(requestBody);
                    responseString = $"Hello, this is a simple C# HTTP server! you called post. \n here's the body: \n  {requestBody} \n here's the body as an object {body}";


                    break;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close(); 
        }
    }
}