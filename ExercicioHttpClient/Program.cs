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
            var jsonOptions = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            try
            {
                var res = await client.GetAsync("https://viacep.com.br/ws/89041710/json/");

                if (res.IsSuccessStatusCode)
                {
                    var jsonEntrada = await res.Content.ReadAsStringAsync();
                    var endereco = JsonConvert. DeserializeObject<Endereco>(jsonEntrada,jsonOptions);  

                    Console.WriteLine("Json recebido:");
                    Console.WriteLine(jsonEntrada);

                    var usuario = new Usuario("Adriano","Marchi","065.345.219-23");
                    usuario.AdicionarEndereco(endereco);

                    var jsonSaida = JsonConvert.SerializeObject(usuario, jsonOptions);
                    Console.WriteLine("Json a ser enviado:");
                    Console.WriteLine(jsonSaida);

                }
                else
                {
                    Console.WriteLine("Não foi possível consultar o endereço: " + res.StatusCode);
                }

            }catch (Exception ex)
            {
                Console.WriteLine("Erro ao consultar endereço: " + ex.Message);
            }

        }
    }
}
