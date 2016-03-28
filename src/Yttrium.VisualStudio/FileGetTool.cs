using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Yttrium.VisualStudio
{
    public partial class FileGetTool
    {
        protected override string DoGenerateCode( string fileContent )
        {
            /*
             * #1. Load XML document
             */
            XmlDocument doc = X.Load( fileContent, "FileGet-Schema.xsd" );

            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "fg", "urn:yttrium/vs/fileget" );


            /*
             * #2. Base URL
             */
            string baseUrl = "";

            XmlAttribute baseAttr = (XmlAttribute) doc.SelectSingleNode( " /fg:download/fg:base/@href ", manager );

            if ( baseAttr != null )
            {
                baseUrl = baseAttr.Value;

                if ( baseUrl.EndsWith( "/", StringComparison.Ordinal ) == false )
                    baseUrl = baseUrl + "/";
            }




            /*
             * #3.
             */
            StringBuilder sb = new StringBuilder();
            sb.Append( "// " );
            sb.AppendLine( DateTime.UtcNow.ToString( "s" ) );

            foreach ( XmlElement fileElem in doc.SelectNodes( " /fg:download/fg:file ", manager ) )
            {
                string href = fileElem.Attributes[ "href" ].Value;

                if ( href.StartsWith( "~/", StringComparison.Ordinal ) == true )
                    href = baseUrl + href.Substring( 2 );

                Uri fromUri = new Uri( href );


                /*
                 * 
                 */
                string toFile;

                if ( fileElem.HasAttribute( "as" ) == true )
                    toFile = fileElem.Attributes[ "as" ].Value;
                else
                    toFile = Path.GetFileName( fromUri.AbsolutePath );

                string toPath = Path.Combine( Path.GetDirectoryName( this.FileName ), toFile );


                /*
                 * 
                 */
                using ( WebClient wc = new WebClient() )
                {
                    wc.DownloadFile( fromUri, toPath );
                }


                /*
                 * 
                 */
                string hash;

                using ( var md5 = MD5.Create() )
                {
                    using ( var stream = File.OpenRead( toPath ) )
                    {
                        hash = BitConverter.ToString( md5.ComputeHash( stream ) ).Replace( "-", "" ).ToLowerInvariant();
                    }
                }

                sb.AppendFormat( "// {0} {1,-30} {2}{3}", hash, toFile, fromUri, Environment.NewLine );
            }

            Console.WriteLine( sb.ToString() );
            return sb.ToString();
        }
    }
}

/* eof */