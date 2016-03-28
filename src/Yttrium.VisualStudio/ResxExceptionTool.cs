using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Yttrium.VisualStudio
{
    public partial class ResxExceptionTool
    {
        protected override string DoGenerateCode( string fileContent )
        {
            /*
             * #1. Load XML document
             */
            XmlDocument doc = X.Load( fileContent, "ResxException-Schema.xsd" );


            /*
             * #2. Build arguments
             */
            FileInfo inputFile = new FileInfo( this.FileName );
            string rawName = inputFile.Name.Substring( 0, inputFile.Name.Length - inputFile.Extension.Length );

            XsltArgumentList args = new XsltArgumentList();
            args.AddParam( "ToolVersion", "", Assembly.GetExecutingAssembly().GetName( false ).Version.ToString( 4 ) );
            args.AddParam( "FileName", "", rawName );
            args.AddParam( "FullFileName", "", inputFile.FullName );
            args.AddParam( "Namespace", "", this.FileNameSpace );

            args.AddExtensionObject( "urn:eo-util", new XsltExtensionObject() );


            /*
             * #3. Load transformation
             */
            XslCompiledTransform xslt = X.LoadXslt( "ResxException-ToCode.xslt" );


            /*
             * #4. Apply transformation
             */
            return X.ToText( xslt, args, doc );
        }
    }
}

/* eof */