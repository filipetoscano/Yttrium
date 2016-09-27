using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Yttrium.Configurator
{
    /// <summary />
    public class Program
    {
        /// <summary />
        static void Main( string[] args )
        {
            /*
             * 
             */
            string configFile;

            if ( args.Length == 0 )
            {
                if ( File.Exists( "config.xml" ) == false )
                {
                    Console.WriteLine( "usage: yconfig [FILE]" );
                    return;
                }

                configFile = "config.xml";
            }
            else
            {
                configFile = args[ 0 ];

                if ( File.Exists( configFile ) == false )
                {
                    Console.Error.WriteLine( "error: file '{0}' not found.", configFile );
                    Environment.ExitCode = 100;
                    return;
                }
            }


            /*
             * 
             */
            XmlSerializer ser = new XmlSerializer( typeof( Description.config ) );
            Description.config config;

            try
            {
                using ( FileStream fs = File.OpenRead( configFile ) )
                {
                    config = (Description.config) ser.Deserialize( fs );
                }
            }
            catch ( InvalidOperationException ex )
            {
                Console.Error.WriteLine( "error: could not load input file" );
                Console.Error.WriteLine( ex.ToString() );
                Environment.ExitCode = 101;
                return;
            }


            /*
             * 
             */
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach ( var v in config.values )
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
                    break;
                }
            }


            /*
             * 
             */
            //foreach ( var kv in values )
            //    Console.WriteLine( "$({0})={1}", kv.Key, kv.Value );


            /*
             * 
             */
            foreach ( var f in config.files )
            {
                if ( File.Exists( f.from ) == false )
                {
                    Console.Error.WriteLine( "error: could not load file '{0}'.", f.to );
                    Environment.ExitCode = 101;
                    return;
                }

                if ( f.type == Description.fileType.auto )
                {
                    if ( f.to.EndsWith( ".xml", StringComparison.OrdinalIgnoreCase ) )
                        f.type = Description.fileType.xml;
                    else if ( f.to.EndsWith( ".json", StringComparison.OrdinalIgnoreCase ) )
                        f.type = Description.fileType.json;
                    else
                        f.type = Description.fileType.text;
                }

                Console.Write( f.from );

                try
                {
                    ReplaceFile( values, f );
                }
                catch ( Exception ex )
                {
                    Console.Error.WriteLine( " ERROR" );
                    Console.Error.WriteLine( ex.ToString() );
                    Environment.ExitCode = 102;
                    return;
                }

                Console.Write( " --> " );
                Console.WriteLine( f.to );
            }
        }


        /// <summary />
        private static void ReplaceFile( Dictionary<string, string> values, Description.file file )
        {
            #region Validations

            if ( values == null )
                throw new ArgumentNullException( nameof( values ) );

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            #endregion

            switch ( file.type )
            {
                case Description.fileType.json:
                    ReplaceJson( values, file.from, file.to );
                    break;

                case Description.fileType.text:
                    ReplaceText( values, file.from, file.to );
                    break;

                case Description.fileType.xml:
                    ReplaceXml( values, file.from, file.to );
                    break;
            }
        }


        /// <summary />
        private static void ReplaceJson( Dictionary<string, string> values, string from, string to )
        {
            string content = File.ReadAllText( from );
            JObject json = JObject.Parse( content );

            foreach ( JToken node in json.Descendants() )
            {
                if ( node.Type == JTokenType.String )
                {
                    JValue v = node as JValue;
                    v.Value = ReplaceString( values, node.Value<string>() );
                }
            }

            using ( TextWriter tw = new StreamWriter( File.OpenWrite( to ) ) )
            {
                JsonTextWriter jw = new JsonTextWriter( tw );
                jw.Formatting = Formatting.Indented;
                jw.Indentation = 4;
                jw.IndentChar = ' ';

                json.WriteTo( jw );
            }
        }


        /// <summary />
        private static void ReplaceText( Dictionary<string, string> values, string from, string to )
        {
            string content = File.ReadAllText( from );

            content = ReplaceString( values, content );

            File.WriteAllText( to, content );
        }


        /// <summary />
        private static void ReplaceXml( Dictionary<string, string> values, string from, string to )
        {
            XDocument doc = XDocument.Load( from );

            foreach ( var node in (IEnumerable) doc.XPathEvaluate( " //@* " ) )
            {
                XAttribute attr = node as XAttribute;
                attr.Value = ReplaceString( values, attr.Value );
            }

            foreach ( var node in (IEnumerable) doc.XPathEvaluate( " //text() " ) )
            {
                XText attr = node as XText;
                attr.Value = ReplaceString( values, attr.Value );
            }

            doc.Save( to );
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
                string k = "$(" + kv.Key + ")";
                string v = kv.Value;

                formatted = formatted.Replace( k, v );
            }

            return formatted;
        }
    }
}
