using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Yttrium.DbManifest.Properties;

namespace Yttrium.DbManifest
{
    class Program
    {
        [SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times" )]
        static void Main( string[] args )
        {
            /*
             *
             */
            CommandLine cl = new CommandLine();

            if ( cl.Parse( args ) == false )
            {
                Environment.Exit( 1001 );
                return;
            }

            if ( cl.Help == true )
            {
                cl.HelpShow();
                Environment.Exit( 1002 );
                return;
            }


            /*
             * 
             */
            DataSet ds = new DataSet();
            ds.Locale = CultureInfo.InvariantCulture;

            using ( SqlConnection conn = new SqlConnection( cl.ConnectionString ) )
            {
                try
                {
                    conn.Open();
                }
                catch ( DbException ex )
                {
                    Console.Error.WriteLine( "FAIL! Could not connect to database:" );
                    Console.Error.WriteLine( ex.Message );
                    Environment.Exit( 2001 );
                    return;
                }

                using ( SqlCommand cmd = new SqlCommand( "dbo.DbManifest", conn ) )
                {
                    SqlDataAdapter da = new SqlDataAdapter( cmd );

                    try
                    {
                        DateTime before = DateTime.UtcNow;

                        if ( cl.Verbose == true )
                            Console.Write( "call dbmanifest... " );

                        da.Fill( ds );

                        if ( cl.Verbose == true )
                            Console.WriteLine( "done, {0}", DateTime.UtcNow - before );
                    }
                    catch ( DbException ex )
                    {
                        Console.Error.WriteLine( "FAIL! Could not execute command dbo.DbManifest:" );
                        Console.Error.WriteLine( ex.Message );
                        Environment.Exit( 2002 );
                        return;
                    }
                }
            }


            /*
             * Name the data set and data tables, so that the XSLT make
             * more sense -- since they can use names rather than positions.
             */
            ds.DataSetName = "DbManifest";
            ds.Tables[ 0 ].TableName = "Database";
            ds.Tables[ 1 ].TableName = "Table";
            ds.Tables[ 2 ].TableName = "TableColumn";
            ds.Tables[ 3 ].TableName = "Index";
            ds.Tables[ 4 ].TableName = "IndexColumn";
            ds.Tables[ 5 ].TableName = "View";
            ds.Tables[ 6 ].TableName = "ViewColumn";
            ds.Tables[ 7 ].TableName = "Procedure";
            ds.Tables[ 8 ].TableName = "ProcedureParameter";
            ds.Tables[ 9 ].TableName = "Function";
            ds.Tables[ 10 ].TableName = "FunctionParameter";


            /*
             * 
             */
            XmlDocument doc = new XmlDocument();
            doc.LoadXml( ds.GetXml() );


            /*
             * 
             */
            XslCompiledTransform xslt = LoadTransform( Resources.ToManifest );
            StringBuilder manifest = new StringBuilder();

            using ( XmlWriter xw = XmlWriter.Create( manifest ) )
            {
                XsltArgumentList xargs = new XsltArgumentList();
                xargs.AddParam( "IncludeComments", "", cl.IncludeComments ? "true" : "false" );

                xslt.Transform( doc, xargs, xw );
            }

            XmlDocument man = new XmlDocument();
            man.LoadXml( manifest.ToString() );


            /*
             * 
             */
            bool outputToFile = string.IsNullOrEmpty( cl.OutputFileName ) == false;

            if ( outputToFile == true )
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Encoding = Encoding.UTF8;
                xws.Indent = true;
                xws.IndentChars = "    ";

                using ( XmlWriter xw = XmlWriter.Create( cl.OutputFileName, xws ) )
                {
                    man.WriteTo( xw );
                }
            }
            else
            {
                XslCompiledTransform toConsole = LoadTransform( Resources.ToConsole );
                StringBuilder output = new StringBuilder();

                using ( StringWriter writer = new StringWriter( output ) )
                {
                    XsltArgumentList xargs = new XsltArgumentList();
                    xargs.AddExtensionObject( "urn:eo", new XslExtensionObject() );

                    toConsole.Transform( man, xargs, writer );
                }

                Console.WriteLine( output.ToString() );
            }


            /*
             * Only export procedures 
             */
            if ( outputToFile == true && cl.ExportCode == true )
            {
                ExportCode( ds.Tables[ "View" ].Rows, Resources.SkelView );
                ExportCode( ds.Tables[ "Procedure" ].Rows, Resources.SkelProcedure );
                ExportCode( ds.Tables[ "Function" ].Rows, Resources.SkelFunction );
            }

            return;
        }


        private static void ExportCode( DataRowCollection rows, string template )
        {
            #region Validations

            if ( rows == null )
                throw new ArgumentNullException( "rows" );

            if ( template == null )
                throw new ArgumentNullException( "template" );

            #endregion

            foreach ( DataRow row in rows )
            {
                string name = (string) row[ "name" ];
                string defn = (string) row[ "definition" ];

                Console.WriteLine( name + "..." );

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat( Resources.SkelProcedure, name, defn.Trim() );

                File.WriteAllText( name + ".sql", sb.ToString() );
            }
        }


        private static XslCompiledTransform LoadTransform( string content )
        {
            XslCompiledTransform xslt = new XslCompiledTransform();

            using ( StringReader sr = new StringReader( content ) )
            {
                var xp = new XPathDocument( sr );
                var xs = new XsltSettings( true, true );
                var xr = new XmlUrlResolver();

                xslt.Load( xp, xs, xr );
            }

            return xslt;
        }
    }
}

/* eof */