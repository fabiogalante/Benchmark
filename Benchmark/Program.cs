using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    class Program
    {



        static void Main(string[] args)
        {

            Console.WriteLine(
                "--------------BENCHMARK - WebRequest (Fundos) x HpptClient x WebClient  x RestSharp------------------------");


            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 3; ++i)
            {
                CallGetWebRequest();
                CallPostWebRequest();
            }

            stopWatch.Stop();
            var webRequesttValue = stopWatch.Elapsed;
            Console.WriteLine($"{webRequesttValue} ====> WebRequest (API FUNDOS)");



            stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 3; ++i)
            {
                CallGetWebClient();
                CallPostWebClient();
            }

            stopWatch.Stop();
            var webClientValue = stopWatch.Elapsed;
            Console.WriteLine($"{webClientValue} ====> WebClient ");




            stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 3; ++i)
            {
                CallGetHttpClient();
                CallPostHttpClient();
            }

            stopWatch.Stop();
            var httpClientValue = stopWatch.Elapsed;
            Console.WriteLine($"{httpClientValue} ====> HttpClient (Pagamento)");






            stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 3; ++i)
            {
                CallGeRestSharp();
                CallPostRestSharp();
            }

            stopWatch.Stop();
            var webRestSharp = stopWatch.Elapsed;

            Console.WriteLine($"{webRestSharp} ====> RestSharp ");

            Console.ReadKey();
        }


        
        #region WebRequest




        private static string CallPostWebRequest()
        {
            const string apiUrl = "http://10.167.0.203:6450/api/v2/Rentabilidade/calcula-rentabilidade-historica";

            var encoding = new UTF8Encoding(false);
            var webRequest = WebRequest.Create(apiUrl);
            webRequest.Timeout = 900000;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";



            var data = encoding.GetBytes("[24433,582589]");
            webRequest.ContentLength = data.Length;

            using (Stream requestStream = webRequest.GetRequestStream())
                requestStream.Write(data, 0, data.Length);

            using (var webResponse = webRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    return streamReader.ReadToEnd();
            }
        }


        private static string CallGetWebRequest()
        {
            var request =
                (HttpWebRequest) WebRequest.Create("http://10.167.0.203:6450/api/v2/Objetivo/consulta/000226725");

            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }


        #endregion

        #region WebClient



        //Usar WebClient é um pouco mais lento que HttpWebRequest, mas essa desvantagem traz alguns benefícios como precisar de menos código e ser mais fácil de usar.
        //    Assim para fazer uma requisição precisamos apenas de duas linhas de código ao invés de cinco usadas na classe HttpWebRequest:Request:

        private static string CallPostWebClient()
        {
            var apiUrl = "http://10.167.0.203:6450/api/v2/Rentabilidade/calcula-rentabilidade-historica";


            string json;
            using (var client = new WebClient
                {Headers = {["Content-type"] = "application/json"}, Encoding = Encoding.UTF8})
            {
                json = client.UploadString(apiUrl, "[24433,582589]");
            }

            return json;
        }

        private static string CallGetWebClient()
        {
            using (var client = new WebClient
                {Headers = {["Content-type"] = "application/json"}, Encoding = Encoding.UTF8})
            {
                return client.DownloadString("http://10.167.0.203:6450/api/v2/Objetivo/consulta/000226725");
            }
        }


        #endregion

        #region HttpClient

        //A classe HttpClient fornece poderosas funcionalidades e uma sintaxe melhor para os novos recursos da threading como
        //    dar suporte a await, além de habilitar o download em threads com melhor verificação e validação de código.



        private static void CallGetHttpClient()
        {
            Task<HttpResponseMessage> response;
            using (var httpClient = new HttpClient())
            {
                response = httpClient.GetAsync("http://10.167.0.203:6450/api/v2/Objetivo/consulta/000226725");
                response.Wait();
            }

            var result = response.Result;
            var readTask = result.Content.ReadAsStringAsync().Result;

        }



        private static void CallPostHttpClient()
        {
            Task<HttpResponseMessage> responseTask;
            using (var httpClient = new HttpClient())
            {
                HttpContent content = new StringContent("[24433,582589]", Encoding.UTF8, "application/json");
                responseTask = httpClient.PostAsync(
                        "http://10.167.0.203:6450/api/v2/Rentabilidade/calcula-rentabilidade-historica", content);
                responseTask.Wait();
            }


            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync().Result;
        }




        #endregion

        #region RestSharp


        private static string CallGeRestSharp()
        {

            var client = new RestClient("http://10.167.0.203:6450/api/v2/Objetivo/consulta/000226725");
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            return response.Content;

        }

        private static void CallPostRestSharp()
        {
            var client = new RestClient("http://10.167.0.203:6450/api/v2/Rentabilidade/calcula-rentabilidade-historica");
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json; charset=utf-8", "[24433,582589]", ParameterType.RequestBody);
            var response = client.Execute(request);
        }

        #endregion

    }
}
