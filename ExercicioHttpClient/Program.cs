using Polly;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExercicioHttpClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando consulta de endereço...");

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);

            var maxRetryAttemps = 5;
            var pauseBetweenFAilures = TimeSpan.FromSeconds(1);
            
            var jsonOptions = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                
            };

            var retryPolly = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(maxRetryAttemps, i => pauseBetweenFAilures);

            string[] myAdress = { "89051-300", "89010-000", "99999999", "12345678977", "89041-710" };

            //Posso fazer uma requisação com retry dessa forma também:
            //var response = await GetAsync("https://www.google.com.br", 5);
            //dessa forma no response vou ter o conteúdo para poder deserializar. Essa forma é usada se eu não quiser importar uma biblioteca como o polly, por exemplo. Ai
            // eu faço o polly de forma manual.


            foreach (var c in myAdress)
            {
                try
                {
                    await retryPolly.ExecuteAsync(async () =>
                    {
                        var res = await client.GetAsync("https://viacep.com.br/ws/" + c + "/json/");
                        //res.EnsureSuccessStatusCode();

                        if (res.IsSuccessStatusCode)
                        {
                            var jsonEntrada = await res.Content.ReadAsStringAsync();

                            if (jsonEntrada.Contains("erro"))
                            {
                                throw new Exception("CEP consultado é inválido!");
                            }

                            var adress = JsonConvert.DeserializeObject<Endereco>(jsonEntrada, jsonOptions);

                            Console.WriteLine("Json recebido:");
                            Console.WriteLine(jsonEntrada);

                            var user = new Usuario("Adriano", "Marchi", "065.345.219-23");
                            user.AdicionarEndereco(adress);

                            var jsonSaida = JsonConvert.SerializeObject(user, jsonOptions);
                            Console.WriteLine("Json a ser enviado:");
                            Console.WriteLine(jsonSaida);

                        }
                        else
                        {
                            switch ((int)res.StatusCode)
                            {
                                case 400:
                                    Console.WriteLine("Requisição inválida!");
                                    break;
                                case 404:
                                    Console.WriteLine("Página não encontrada!");
                                    break;
                                case 500:
                                    Console.WriteLine("Erro interno no servidor!");
                                    break;
                                default:
                                    Console.WriteLine("Não foi possível consultar o endereço: " + (int)res.StatusCode + " -> " + res.StatusCode);
                                    break;

                            }

                        }

                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao consultar endereço: " + ex.Message);
                }

            }
        }
        
        public static async Task<string> GetAsync(string url, int retryCount)
        {
            var client = new HttpClient();
            var response = string.Empty;
            var retry = false;
            var retryIndex = 0;

            do
            {
                Console.WriteLine("Fazendo a requisição {0} de 5", retryIndex);
                var res = await client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    retry = true;
                    retryIndex++;
                }

                response = await res.Content.ReadAsStringAsync();
                

            } while (retry && retryIndex < retryCount);

            return response;

        }
    }    

}




 

 
 
 
 
