using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Yttrium.VisualStudio
{
    internal static class X
    {
        internal static XmlDocument Load( string xml )
        {
            return Load( xml, null );
        }


        internal static XmlDocument Load( string xml, string schema )
        {
            #region Validations

            if ( xml == null )
                throw new ArgumentNullException( "xml" );

            #endregion

            /*
             * 
             */
            List<string> messages = new List<string>();

            XmlReaderSettings xrs = new XmlReaderSettings();

            if ( schema != null )
            {
                XmlSchemaSet ss = new XmlSchemaSet();
                ss.Add( X.LoadSchema( schema ) );
                ss.ValidationEventHandler += ( sender, e ) => { messages.Add( e.Message ); };

                xrs.ValidationType = ValidationType.Schema;
                xrs.ValidationEventHandler += ( sender, e ) => { messages.Add( e.Message ); };
                xrs.Schemas.Add( ss );
                xrs.Schemas.Compile();
            }


            /*
             * 
             */
            XmlDocument doc = new XmlDocument();

            StringReader sr = new StringReader( xml );
            XmlTextReader xtr = new XmlTextReader( sr );

            using ( XmlReader reader = XmlReader.Create( xtr, xrs ) )
            {
                try
                {
                    doc.Load( reader );
                }
                catch ( Exception ex )
                {
                    throw new ToolException( "Failed to load XML document.", ex );
                }
            }

            if ( messages.Count > 0 )
            {
                ToolException ctx = new ToolException( string.Join( "\n", messages ) );
                throw new ToolException( "XML does not comply against XSD schema.", ctx );
            }

            return doc;
        }


        internal static XmlSchema LoadSchema( string name )
        {
            #region Validations

            if ( name == null )
                throw new ArgumentNullException( "name" );

            #endregion

            string fullName = "Yttrium.VisualStudio.Resources." + name;

            XmlSchema schema;

            using ( Stream stream = typeof( X ).Assembly.GetManifestResourceStream( fullName ) )
            {
                schema = XmlSchema.Read( stream, null );
            }

            return schema;
        }


        internal static XslCompiledTransform LoadXslt( string name )
        {
            #region Validations

            if ( name == null )
                throw new ArgumentNullException( "name" );

            #endregion

            string fullName = "Yttrium.VisualStudio.Resources." + name;

            XsltSettings settings = new XsltSettings( true, true );
            XmlResolver resolver = new XmlUrlResolver();
            XslCompiledTransform xsl = new XslCompiledTransform();

            Stream xsltStream = typeof( X ).Assembly.GetManifestResourceStream( fullName );

            using ( XmlReader xr = XmlReader.Create( xsltStream ) )
            {
                xsl.Load( xr, settings, resolver );
            }

            return xsl;
        }


        internal static string ToText( XslCompiledTransform xslt, XsltArgumentList args, XmlDocument xdoc )
        {
            #region Validations

            if ( xslt == null )
                throw new ArgumentNullException( "xslt" );

            if ( xdoc == null )
                throw new ArgumentNullException( "xdoc" );

            #endregion

            XPathNavigator xpNav = xdoc.DocumentElement.CreateNavigator();
            StringBuilder sb = new StringBuilder();

            using ( TextWriter writer = new StringWriter( sb, CultureInfo.InvariantCulture ) )
            {
                try
                {
                    xslt.Transform( xpNav, args, writer );
                }
                catch ( Exception ex )
                {
                    throw new ToolException( "Failed to apply XSLT transformation", ex );
                }
            }

            return sb.ToString();
        }


        internal static string ToXml( XslCompiledTransform xslt, XsltArgumentList args, XmlDocument xdoc )
        {
            #region Validations

            if ( xslt == null )
                throw new ArgumentNullException( "xslt" );

            if ( xdoc == null )
                throw new ArgumentNullException( "xdoc" );

            #endregion

            XPathNavigator xpNav = xdoc.DocumentElement.CreateNavigator();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( @"<?xml version=""1.0"" encoding=""utf-8""?>" );

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "    ";
            xws.OmitXmlDeclaration = true;

            using ( XmlWriter xw = XmlWriter.Create( sb, xws ) )
            {
                try
                {
                    xslt.Transform( xpNav, args, xw );
                }
                catch ( Exception ex )
                {
                    throw new ToolException( "Failed to apply XSLT transformation", ex );
                }
            }

            return sb.ToString();
        }
    }
}

/* eof */