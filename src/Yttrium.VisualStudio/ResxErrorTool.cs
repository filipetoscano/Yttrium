using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Yttrium.VisualStudio
{
    public partial class ResxErrorTool
    {
        protected override string DoGenerateCode( string fileContent )
        {
            /*
             * #1. Load XML document
             */
            XmlDocument doc = X.Load( fileContent, "ResxError-Schema.xsd" );


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
            XslCompiledTransform xsl1 = X.LoadXslt( "ResxError-ToCode.xslt" );
            XslCompiledTransform xsl2 = X.LoadXslt( "ResxError-ToResx.xslt" );


            /*
             * #4. Apply transformation
             */
            string cs = X.ToText( xsl1, args, doc );
            string resx = X.ToXml( xsl2, args, doc );


            /*
             * #5. Write the contents of the ER.resx file
             */
            string resxPath = Path.Combine( inputFile.DirectoryName, "ER.resx" );
            File.WriteAllText( resxPath, resx, Encoding.UTF8 );

            return cs;
        }
    }
}

/* eof */