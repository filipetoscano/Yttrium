using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Yttrium.VisualStudio
{
    public partial class XsdTool
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
            FileInfo inputFile = new FileInfo( this.FileName );
            string rawName = inputFile.Name.Substring( 0, inputFile.Name.Length - inputFile.Extension.Length );



            /*
             * 
             */
            string tempPath = Path.GetTempPath().TrimEnd( '\\' );
            string tempFile = Path.Combine( tempPath, rawName + ".cs" );


            /*
             * 
             */
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( CultureInfo.InvariantCulture, " \"{0}\" ", inputFile.FullName );
            sb.AppendFormat( CultureInfo.InvariantCulture, " /nologo " );
            sb.AppendFormat( CultureInfo.InvariantCulture, " /classes " );
            sb.AppendFormat( CultureInfo.InvariantCulture, " /language:CS " );
            sb.AppendFormat( CultureInfo.InvariantCulture, " /namespace:\"{0}\" ", this.FileNameSpace );
            sb.AppendFormat( CultureInfo.InvariantCulture, " /out:\"{0}\" ", tempPath );


            /*
             * 
             */
            VisualStudioTool tool = VisualStudioSdk.ToolGet( "xsd.exe" );

            if ( tool.Found == false )
            {
                return ErrorEmit( "xsd.exe not found in any location", string.Join( "\n", tool.Locations ) );
            }


            /*
             * 
             */
            ProcessStartInfo psinfo = new ProcessStartInfo();
            psinfo.FileName = tool.Path;
            psinfo.LoadUserProfile = false;
            psinfo.CreateNoWindow = true;
            psinfo.RedirectStandardOutput = true;
            psinfo.RedirectStandardError = true;
            psinfo.UseShellExecute = false;
            psinfo.WorkingDirectory = Path.GetTempPath();
            psinfo.Arguments = sb.ToString();

            using ( Process p = new Process() )
            {
                p.StartInfo = psinfo;
                p.Start();
                p.WaitForExit();


                /*
                 * 
                 */
                if ( p.ExitCode != 0 )
                {
                    string er = p.StandardError.ReadToEnd();

                    return ErrorEmit( er, "executing xsd.exe command" );
                }
            }


            /*
             * 
             */
            FileInfo tempFileInfo = new FileInfo( tempFile );

            if ( tempFileInfo.Exists == false )
                return "// Tool failed: temporary file not found!";

            string content = File.ReadAllText( tempFile );


            /*
             * 
             */
            XsdToolProcessingInstruction pi = LoadProcessingInstruction( inputFile.FullName );


            /*
             * 
             */
            StringBuilder ob = new StringBuilder( content );

            if ( pi.Minify == true )
            {
                // Namespaces
                ob.Replace( "    using System.Xml.Serialization;",
                           @"    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;" );

                // System
                ob.Replace( "System.SerializableAttribute()", "Serializable" );

                // System.CodeDom.Compiler
                ob.Replace( "System.CodeDom.Compiler.GeneratedCodeAttribute", "GeneratedCode" );

                // System.ComponentModel
                ob.Replace( "System.ComponentModel.DesignerCategoryAttribute", "DesignerCategory" );

                // System.Diagnostics
                ob.Replace( "System.Diagnostics.DebuggerStepThroughAttribute()", "DebuggerStepThrough" );

                // System.Xml.Serialization
                ob.Replace( "System.Xml.Serialization.XmlArrayItemAttribute", "XmlArrayItem" );
                ob.Replace( "System.Xml.Serialization.XmlAttributeAttribute", "XmlAttribute" );
                ob.Replace( "System.Xml.Serialization.XmlElementAttribute", "XmlElement" );
                ob.Replace( "System.Xml.Serialization.XmlRootAttribute", "XmlRoot" );
                ob.Replace( "System.Xml.Serialization.XmlTypeAttribute", "XmlType" );
            }

            if ( pi.Scope == "internal" )
            {
                ob.Replace( "public partial class", "internal partial class" );
            }


            return ob.ToString();
        }


        private static XsdToolProcessingInstruction LoadProcessingInstruction( string xsd )
        {
            /*
             * Defaults
             */
            XsdToolProcessingInstruction pi = new XsdToolProcessingInstruction();
            pi.Minify = true;
            pi.Scope = "public";

            
            /*
             * 
             */
            XmlDocument doc = new XmlDocument();
            doc.Load( xsd );

            XmlProcessingInstruction xmlpi = (XmlProcessingInstruction) doc.SelectSingleNode( " processing-instruction('xsdTool') " );

            if ( xmlpi == null )
                return pi;

            string[] values = xmlpi.Value.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

            foreach ( string v in values )
            {
                string[] p = v.Split( '=' );

                if ( p.Length < 2 )
                    continue;

                switch ( p[ 0 ] )
                {
                    case "scope":
                        pi.Scope = p[ 1 ];
                        break;

                    case "minify":
                        if ( p[ 1 ].ToLowerInvariant() == "true" )
                            pi.Minify = true;

                        if ( p[ 1 ].ToLowerInvariant() == "false" )
                            pi.Minify = false;
                        break;

                    default:
                        break;
                }
            }

            return pi;
        }


        /// <summary>
        /// Values defined in the xsdTool processing instruction, added to the
        /// top of the XSD document.
        /// </summary>
        internal class XsdToolProcessingInstruction
        {
            internal string Scope;
            internal bool Minify;
        }
    }
}

/* eof */