using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercicioHttpClient
{
    public class Usuario
    {
        public Usuario(string nome,
                       string sobreNome,
                       string documento)
        {
            Nome = nome;
            SobreNome = sobreNome;
            Documento = documento;
        }

        public string Nome { get; private set; }
        public string SobreNome { get; private set; }
        public string Documento { get; private set; }
        public Endereco Endereco { get; private set; }

        public void AdicionarEndereco(Endereco endereco)
        {
            Endereco = endereco; 
        }

    }
}
