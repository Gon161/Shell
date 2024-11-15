using Shell.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string a = Encriptado.EncryptAes("aEvfdssszhbr<<<<<<<<<<<fdbfvg<azgeageUS");
            Console.WriteLine(a);

            string b = Encriptado.DecryptAes(a);
            Console.WriteLine(b);

            Console.ReadKey();

        }
    }
}
