using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Yttrium.Scaffold
{
    /// <summary />
    public class Program
    {
        static void Main( string[] args )
        {
            /*
             * #1. Any arguments?
             */
            if ( args.Length != 1 )
            {
                Console.WriteLine( "err: yscaffold [ DIRECTORY | ARCHIVE ]" );
                Environment.ExitCode = 100;
                return;
            }


            /*
             * #2. 
             */
            string arg = args[ 0 ];

            string scaffoldTempl;
            string directoryTempl;

            if ( Directory.Exists( arg ) == true )
            {
                scaffoldTempl = Path.Combine( arg, "yscaffold.xml" );

                if ( File.Exists( scaffoldTempl ) == false )
                {
                    Console.WriteLine( "err: directory does not contain 'yscaffold.xml' file." );
                    Environment.ExitCode = 101;
                    return;
                }

                directoryTempl = arg;
            }
            else if ( File.Exists( arg ) == true )
            {
                if ( Path.GetFileName( arg ) == "yscaffold.xml" )
                {
                    scaffoldTempl = arg;
                    directoryTempl = Path.GetDirectoryName( arg );
                }
                else
                {
                    Console.WriteLine( "err: scaffolding from archive not yet implemented!" );
                    Environment.ExitCode = 102;
                    return;
                }
            }
            else
            {
                Console.WriteLine( "err: scaffold template not found at '{0}'.", arg );
                Environment.ExitCode = 103;
                return;
            }


            /*
             * 
             */
            Console.WriteLine( scaffoldTempl );
            Console.WriteLine( directoryTempl );


            /*
             * 
             */
            XmlSerializer ser = new XmlSerializer( typeof( Description.config ) );
            Description.config config;

            try
            {
                using ( FileStream fs = File.OpenRead( scaffoldTempl ) )
                {
                    config = (Description.config) ser.Deserialize( fs );
                }
            }
            catch ( InvalidOperationException ex )
            {
                Console.Error.WriteLine( "error: could not load input file" );
                Console.Error.WriteLine( ex.ToString() );
                Environment.ExitCode = 104;
                return;
            }


            /*
             * 
             */
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach ( var v in config.variables )
            {
                while ( true )
                {
                    Console.Write( v.text );

                    if ( v.@default != null )
                        Console.Write( " [{0}]", v.@default );

                    Console.Write( ": " );

                    string value = Console.ReadLine();

                    if ( string.IsNullOrWhiteSpace( value ) == true && v.@default != null )
                        value = v.@default;

                    if ( string.IsNullOrWhiteSpace( value ) == true && v.required == true )
                        continue;


                    values.Add( v.name, value );

                    if ( v.name.ToLowerInvariant() != v.name )
                        values.Add( v.name.ToLowerInvariant(), value.ToLowerInvariant() );

                    if ( v.name.ToUpperInvariant() != v.name )
                        values.Add( v.name.ToLowerInvariant(), value.ToUpperInvariant() );

                    break;
                }
            }

            if ( config.values.date?.Length >= 0 )
            {
                foreach ( var v in config.values.date )
                {

                }
            }

            if ( config.values.guid?.Length >= 0 )
            {
                foreach ( var v in config.values.guid )
                {
                    values.Add( v.name, Guid.NewGuid().ToString().ToLowerInvariant() );
                }
            }


        }
    }
}
