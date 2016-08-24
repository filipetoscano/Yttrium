using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Yttrium.XmlTidy
{
    public static class Program
    {
        static void Main( string[] args )
        {
            /*
             * Validations
             */
            if ( args == null || args.Length == 0 )
            {
                Console.Out.WriteLine( "usage: xmltidy [FILES]" );
                Environment.ExitCode = 1000;
                return;
            }


            /*
             * 
             */
            bool hadError = false;


            foreach ( string file in args )
            {
                if ( File.Exists( file ) == false )
                {
                    hadError = true;
                    Console.Error.WriteLine( "error: file '{0}' does not exist.", file );
                    continue;
                }


                /*
                 * 
                 */
                XmlDocument doc = new XmlDocument();

                try
                {
                    doc.Load( file );
                }
                catch ( Exception ex )
                {
                    hadError = true;
                    Console.Error.WriteLine( "error: exception loading file '{0}'.", file );
                    Console.Error.WriteLine( ex.ToString() );
                    continue;
                }


                /*
                 * 
                 */
                using ( XmlTextWriter xw = new XmlTextWriter( file, Encoding.UTF8 ) )
                {
                    xw.Formatting = Formatting.Indented;
                    xw.Indentation = 4;
                    xw.IndentChar = ' ';

                    try
                    {
                        doc.Save( xw );
                    }
                    catch ( Exception ex )
                    {
                        hadError = true;
                        Console.Error.WriteLine( "error: exception saving to file '{0}'.", file );
                        Console.Error.WriteLine( ex.ToString() );
                        continue;
                    }
                }
            }


            /*
             * 
             */
            if ( hadError == true )
                Environment.ExitCode = 2000;
            else
                Environment.ExitCode = 0;
        }
    }
}
