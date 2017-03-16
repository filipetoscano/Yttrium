using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yttrium.Base64
{
    class Program
    {
        static void Main( string[] args )
        {
            /*
             * 
             */
            if ( args.Length != 3 )
            {
                Console.Error.WriteLine( "err: ybase64 [ encode | decode ] INPUT OUTPUT" );
                Environment.ExitCode = 1;
                return;
            }

            string command = args[ 0 ];
            string input = args[ 1 ];
            string output = args[ 2 ];


            /*
             * 
             */
            if ( command != "encode" && command != "decode" )
            {
                Console.Error.WriteLine( "err: command '{0}' not supported.", command );
                Environment.ExitCode = 2;
                return;
            }

            if ( File.Exists( input ) == false )
            {
                Console.Error.WriteLine( "err: file '{0}' not found.", input );
                Environment.ExitCode = 3;
                return;
            }


            /*
             * 
             */
            if ( command == "encode" )
            {
                byte[] inputContent = File.ReadAllBytes( input );
                string b64 = Convert.ToBase64String( inputContent );
                File.WriteAllText( output, b64 );
            }
            else
            {
                string b64 = File.ReadAllText( input );
                byte[] outputContent = Convert.FromBase64String( b64 );
                File.WriteAllBytes( output, outputContent );
            }
        }
    }
}
