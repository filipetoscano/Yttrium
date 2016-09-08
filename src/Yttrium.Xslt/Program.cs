using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Yttrium.Xslt
{
    class Program
    {
        static void Main( string[] args )
        {
            /*
             *
             */
            CommandLine cl = new CommandLine();

            if ( cl.Parse( args ) == false )
                Environment.Exit( 1001 );

            if ( cl.Help == true )
            {
                cl.HelpShow();
                Environment.Exit( 1002 );
            }


            /*
             *
             */
            XmlDocument idoc = new XmlDocument();
            idoc.Load( cl.InputFile );

            XmlResolver resolver = new XmlUrlResolver();

            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load( cl.TransformFile, settings, resolver );

            if ( xslt.OutputSettings.OutputMethod == XmlOutputMethod.Xml )
            {
                using ( XmlWriter xw = XmlWriter.Create( cl.OutputFile, xslt.OutputSettings ) )
                {
                    XsltArgumentList xargs = new XsltArgumentList();

                    XmlReader xr = XmlReader.Create( cl.InputFile );
                    xslt.Transform( xr, xargs, xw, resolver );
                }
            }
            else
            {
                using ( TextWriter tw = new StreamWriter( File.OpenWrite( cl.OutputFile ) ) )
                {
                    XsltArgumentList xargs = new XsltArgumentList();

                    XmlReader xr = XmlReader.Create( cl.InputFile );
                    xslt.Transform( xr, xargs, tw );
                }
            }
        }
    }
}
