using System;
using System.IO;
using System.Text;

namespace Yttrium.FindReplace
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
                Console.Error.WriteLine( "err: yfr file find replace" );
                Environment.ExitCode = 1;
                return;
            }

            string file = args[ 0 ];
            string find = args[ 1 ];
            string repl = args[ 2 ];



            /*
             * 
             */
            if ( File.Exists( file ) == false )
            {
                Console.Error.WriteLine( "err: file '{0}' does not exist", file );
                Environment.ExitCode = 1;
                return;
            }

            string content;

            try
            {
                content = File.ReadAllText( file );
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine( "err: failed to load from '{0}'.", file );
                Console.Error.WriteLine( ex.ToString() );

                Environment.ExitCode = 2;
                return;
            }


            /*
             * 
             */
            content = content.Replace( find, repl );


            /*
             * 
             */
            try
            {
                File.WriteAllText( file, content, Encoding.UTF8 );
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine( "err: failed to write to '{0}'.", file );
                Console.Error.WriteLine( ex.ToString() );

                Environment.ExitCode = 3;
                return;
            }


            Environment.ExitCode = 0;
            return;
        }
    }
}
