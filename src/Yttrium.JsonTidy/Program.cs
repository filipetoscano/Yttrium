using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yttrium.JsonTidy
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

                JObject jo;

                using ( TextReader tr = new StreamReader( file ) )
                {
                    using ( JsonReader jr = new JsonTextReader( tr ) )
                    {
                        try
                        {
                            jo = JObject.Load( jr );
                        }
                        catch ( Exception ex )
                        {
                            hadError = true;
                            Console.Error.WriteLine( "error: " + ex.ToString() );
                            continue;
                        }
                    }
                }

                using ( TextWriter tw = new StreamWriter( file ) )
                {
                    using ( JsonTextWriter jw = new JsonTextWriter( tw ) )
                    {
                        jw.Formatting = Formatting.Indented;
                        jw.IndentChar = ' ';
                        jw.Indentation = 4;

                        try
                        {
                            jo.WriteTo( jw );
                        }
                        catch ( Exception ex )
                        {
                            hadError = true;
                            Console.Error.WriteLine( "error: " + ex.ToString() );
                            continue;
                        }
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
