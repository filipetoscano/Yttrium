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
                if ( File.Exists( f.to ) == false )
                {
                    Console.Error.WriteLine( "error: could not load file '{0}'.", f.to );
                    Environment.ExitCode = 101;
                    return;
                }

                Description.fileType fileType = f.type;

                if ( fileType == Description.fileType.auto )
                {
                    if ( f.to.EndsWith( ".xml", StringComparison.OrdinalIgnoreCase ) )
                        fileType = Description.fileType.xml;
                    else if ( f.to.EndsWith( ".js", StringComparison.OrdinalIgnoreCase ) )
                        fileType = Description.fileType.json;
                    else
                        fileType = Description.fileType.text;
                }

                Console.Write( f.from );

                switch ( fileType )
                {
                    case Description.fileType.json:
                        ReplaceJson( values, f.from, f.to );
                        break;

                    case Description.fileType.text:
                        ReplaceText( values, f.from, f.to );
                        break;

                    case Description.fileType.xml:
                        ReplaceXml( values, f.from, f.to );
                        break;
                }

                Console.Write( " --> " );
                Console.WriteLine( f.to );
            }
        }


        /// <summary />
        private static void ReplaceJson( Dictionary<string, string> values, string from, string to )
        {
            throw new NotImplementedException();
        }


        /// <summary />
        private static void ReplaceText( Dictionary<string, string> values, string from, string to )
        {
            string content = File.ReadAllText( from );

            content = ReplaceAll( values, content );

            File.WriteAllText( to, content );
        }


        /// <summary />
        private static void ReplaceXml( Dictionary<string, string> values, string from, string to )
        {
            XDocument doc = XDocument.Load( from );

            foreach ( var node in (IEnumerable) doc.XPathEvaluate( " //@* " ) )
            {
                XAttribute attr = node as XAttribute;
                attr.Value = ReplaceAll( values, attr.Value );
            }

            foreach ( var node in (IEnumerable) doc.XPathEvaluate( " //text() " ) )
            {
                XText attr = node as XText;
                attr.Value = ReplaceAll( values, attr.Value );
            }

            doc.Save( to );
        }


        /// <summary />
        private static string ReplaceAll( Dictionary<string, string> values, string value )
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
