using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Yttrium.Scaffold.Description;

namespace Yttrium.Scaffold
{
    /// <summary />
    public class Program
    {
        /// <summary />
        public static void Main( string[] args )
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
             * #2. Argument validation.
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

                FileInfo finfo = new FileInfo( scaffoldTempl );

                scaffoldTempl = finfo.FullName;
                directoryTempl = finfo.DirectoryName;
            }
            else if ( File.Exists( arg ) == true )
            {
                if ( Path.GetFileName( arg ) == "yscaffold.xml" )
                {
                    FileInfo finfo = new FileInfo( arg );

                    scaffoldTempl = finfo.FullName;
                    directoryTempl = finfo.DirectoryName;
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
             * #3. Deserialize scaffold meta
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
             * #4. Build list of values from:
             *     - Prompted values;
             *     - Generated values.
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

                    values.Add( config.placeholders, v.name, value );
                    break;
                }
            }

            if ( config.values.date?.Length >= 0 )
            {
                foreach ( var v in config.values.date )
                {
                    string key = v.name;
                    string val = DateTime.Now.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture );

                    values.Add( config.placeholders, key, val );
                }
            }

            if ( config.values.guid?.Length >= 0 )
            {
                foreach ( var v in config.values.guid )
                {
                    string key = v.name;
                    string val = Guid.NewGuid().ToString().ToLowerInvariant();

                    values.Add( config.placeholders, key, val );

                    if ( v.find != null )
                        values.Add( v.find, val );
                }
            }

            //foreach ( var k in values.Keys )
            //    Console.WriteLine( "{0}={1}", k, values[ k ] );


            /*
             * #5. Mirror 'directoryTempl' onto current working directory.
             */
            var ctx = new ExecutionContext();
            ctx.Values = values;
            ctx.FromRoot = new DirectoryInfo( directoryTempl );
            ctx.ToRoot = new DirectoryInfo( Environment.CurrentDirectory );

            Mirror( ctx, ctx.FromRoot, ctx.ToRoot );
        }



        private static void Mirror( ExecutionContext ctx, DirectoryInfo fromDirectory, DirectoryInfo toDirectory )
        {
            #region Validations

            if ( ctx == null )
                throw new ArgumentNullException( nameof( ctx ) );

            if ( fromDirectory == null )
                throw new ArgumentNullException( nameof( fromDirectory ) );

            if ( toDirectory == null )
                throw new ArgumentNullException( nameof( toDirectory ) );

            #endregion


            /*
             * Mirror files in current directory
             */
            foreach ( var file in fromDirectory.GetFiles() )
            {
                string fromFile = file.FullName;
                string toFile = Path.Combine( toDirectory.FullName, ReplaceString( ctx.Values, file.Name ) );

                if ( file.IsBinaryFile() == true )
                {
                    Console.WriteLine( "  {0} *", ctx.Relative( toFile ) );
                    File.Copy( fromFile, toFile );
                }
                else
                {
                    Console.WriteLine( "  {0}", ctx.Relative( toFile ) );
                    FileReplaceAndCopy( ctx.Values, fromFile, toFile );
                }
            }


            /*
             * Recursively walk!
             */
            foreach ( var dir in fromDirectory.GetDirectories() )
            {
                if ( dir.Name == ".git" )
                    continue;

                if ( dir.Name == ".vs" )
                    continue;

                if ( dir.Name == "bin" && fromDirectory.FullName != ctx.FromRoot.FullName )
                    continue;

                if ( dir.Name == "packages" && fromDirectory.FullName == ctx.FromRoot.FullName )
                    continue;

                if ( dir.Name == "obj" )
                    continue;

                if ( dir.Name == "pkg" )
                    continue;

                string to = Path.Combine( toDirectory.FullName, ReplaceString( ctx.Values, dir.Name ) );
                var toDir = Directory.CreateDirectory( to );

                Console.WriteLine( "D {0}", ctx.Relative( toDir.FullName ) );
                Mirror( ctx, dir, toDir );
            }
        }


        /// <summary />
        private static void FileReplaceAndCopy( Dictionary<string, string> values, string from, string to )
        {
            #region Validations

            if ( values == null )
                throw new ArgumentNullException( nameof( values ) );

            if ( from == null )
                throw new ArgumentNullException( nameof( from ) );

            if ( to == null )
                throw new ArgumentNullException( nameof( to ) );

            #endregion

            string content;

            content = File.ReadAllText( from, Encoding.UTF8 );

            content = ReplaceString( values, content );

            File.WriteAllText( to, content, Encoding.UTF8 );
        }


        /// <summary />
        private static string ReplaceString( Dictionary<string, string> values, string value )
        {
            #region Validations

            if ( values == null )
                throw new ArgumentNullException( nameof( values ) );

            #endregion

            if ( value == null )
                return null;

            string formatted = value;

            foreach ( var kv in values )
            {
                string k = kv.Key;
                string v = kv.Value;

                formatted = formatted.Replace( k, v );
            }

            return formatted;
        }
    }
}
