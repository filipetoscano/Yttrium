using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Yttrium.VisualStudio
{
    public partial class XsltTool : BaseTool
    {
        protected override string DoGenerateCode( string fileContent )
        {
            #region Validation

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            #endregion

            /*
             * 
             */
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml( fileContent );
            }
            catch ( XmlException ex )
            {
                return ErrorEmit( "Error: File is not a valid Xml document", ex );
            }

            XPathNavigator xpNav = doc.DocumentElement.CreateNavigator();


            /*
             * 
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "xsi", "http://www.w3.org/2001/XMLSchema-instance" );

            XmlAttribute schemaAttr = (XmlAttribute) doc.SelectSingleNode( " /*[ @xsi:schemaLocation ] ", manager );

            if ( schemaAttr != null )
            {
            }



            /*
             * 
             */
            XmlProcessingInstruction pi = (XmlProcessingInstruction) doc.SelectSingleNode( " processing-instruction( \"codebehind\" ) " );

            if ( pi == null )
                return ErrorEmit( "Processing instruction <?codebehind?> not found" );


            /*
             * 
             */
            Regex regex = new Regex( "transformation=['\"](?<xslt>.*?)['\"]" );

            Match m = regex.Match( pi.Value );

            if ( m.Success == false )
                return ErrorEmit( "Processing instruction <?codebehind?> does not contain @transformation attribute" );


            /*
             * 
             */
            FileInfo inputFile = new FileInfo( this.FileName );

            string xsltRaw = m.Groups[ "xslt" ].Value;
            string xslt = Path.Combine( inputFile.DirectoryName, xsltRaw );
            string rawName = inputFile.Name.Substring( 0, inputFile.Name.Length - inputFile.Extension.Length );


            /*
             * 
             */
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam( "ToolVersion", "", Assembly.GetExecutingAssembly().GetName( false ).Version.ToString( 4 ) );
            args.AddParam( "FileName", "", rawName );
            args.AddParam( "FullFileName", "", inputFile.FullName );
            args.AddParam( "Namespace", "", this.FileNameSpace );

            args.AddExtensionObject( "urn:eo-util", new XsltExtensionObject() );


            /*
             * 
             */
            XsltSettings settings = new XsltSettings( true, true );
            XmlResolver resolver = new XmlUrlResolver();

            XslCompiledTransform xsl = new XslCompiledTransform();

            using ( XmlReader xr = XmlReader.Create( xslt ) )
            {
                try
                {
                    xsl.Load( xr, settings, resolver );
                }
                catch ( XsltCompileException ex )
                {
                    return ErrorEmit( "Error loading transformation", ex );
                }
                catch ( XmlException ex )
                {
                    return ErrorEmit( "Error loading transformation", ex );
                }
            }


            /*
             * 
             */
            StringBuilder sb = new StringBuilder();

            using ( TextWriter writer = new StringWriter( sb, CultureInfo.InvariantCulture ) )
            {
                try
                {
                    xsl.Transform( xpNav, args, writer );
                }
                catch ( XmlException ex )
                {
                    return ErrorEmit( "Error during transformation", ex );
                }
                catch ( XsltException ex )
                {
                    return ErrorEmit( "Error during transformation", ex );
                }
            }

            return sb.ToString();
        }
    }
}

/* eof */