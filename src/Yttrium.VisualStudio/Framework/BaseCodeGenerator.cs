using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
    /// <summary>
    /// A managed wrapper for VS's concept of an IVsSingleFileGenerator which is
    /// a custom tool invoked during the build which can take any file as an input
    /// and provide a compilable code file as output.
    /// </summary>
    [
        SuppressMessage( "Microsoft.Interoperability", "CA1409:ComVisibleTypesShouldBeCreatable" ),
        Guid( "1fb5efcf-d56f-48cf-bc09-087dcd4a1688" ),
        ComVisible( true )
    ]
    public abstract class BaseCodeGenerator : IVsSingleFileGenerator
    {
        private IVsGeneratorProgress codeGeneratorProgress;

        /// <summary>
        /// interface to the VS shell object we use to tell our
        /// progress while we are generating.
        /// </summary>
        internal IVsGeneratorProgress CodeGeneratorProgress
        {
            get
            {
                return codeGeneratorProgress;
            }
        }

        /// <summary>
        /// gets the default extension for this generator
        /// </summary>
        /// <returns>string with the default extension for this generator</returns>
        public abstract string GetDefaultExtension();

        /// <summary>
        /// the method that does the actual work of generating code given the input
        /// file.
        /// </summary>
        /// <param name="baseNamespace">namespace of input file</param>
        /// <param name="inputFileName">input file name</param>
        /// <param name="inputFileContent">file contents as a string</param>
        /// <returns>the generated code file as a byte-array</returns>
        protected abstract byte[] GenerateCode( string baseNamespace, string inputFileName, string inputFileContent );

        /// <summary>
        /// method that will communicate an error via the shell callback mechanism.
        /// </summary>
        /// <param name="warning">true if this is a warning</param>
        /// <param name="level">level or severity</param>
        /// <param name="message">text displayed to the user</param>
        /// <param name="line">line number of error/warning</param>
        /// <param name="column">column number of error/warning</param>
        protected virtual void GeneratorErrorCallback( bool warning, int level, string message, int line, int column )
        {
            IVsGeneratorProgress progress = CodeGeneratorProgress;
            if ( progress != null )
            {
                progress.GeneratorError( warning, level, message, line, column );
            }
        }

        /// <summary>
        /// main method that the VS shell calls to do the generation
        /// </summary>
        /// <param name="wszInputFilePath">path to the input file</param>
        /// <param name="bstrInputFileContents">contents of the input file as a string (shell handles UTF-8 to Unicode & those types of conversions)</param>
        /// <param name="wszDefaultNamespace">default namespace for the generated code file</param>
        /// <param name="rgbOutputFileContents">byte-array of output file contents</param>
        /// <param name="pcbOutput">count of bytes in the output byte-array</param>
        /// <param name="pGenerateProgress">interface to send progress updates to the shell</param>
        public void Generate( string wszInputFilePath,
                             string bstrInputFileContents,
                             string wszDefaultNamespace,
                             out IntPtr rgbOutputFileContents,
                             out int pcbOutput,
                             IVsGeneratorProgress pGenerateProgress )
        {

            if ( bstrInputFileContents == null )
                throw new ArgumentNullException( "bstrInputFileContents" );

            codeGeneratorProgress = pGenerateProgress;

            byte[] bytes = GenerateCode( wszDefaultNamespace, wszInputFilePath, bstrInputFileContents );

            if ( bytes == null )
            {
                rgbOutputFileContents = IntPtr.Zero;
                pcbOutput = 0;
            }
            else
            {
                pcbOutput = bytes.Length;
                rgbOutputFileContents = Marshal.AllocCoTaskMem( pcbOutput );
                Marshal.Copy( bytes, 0, rgbOutputFileContents, pcbOutput );
            }
        }

        /// <summary>
        /// method to return a byte-array given a Stream
        /// </summary>
        /// <param name="stream">stream to convert to a byte-array</param>
        /// <returns>the stream's contents as a byte-array</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic" )]
        protected byte[] StreamToBytes( Stream stream )
        {
            if ( stream == null )
            {
                return new byte[] { };
            }

            if ( stream.Length == 0 )
            {
                return new byte[] { };
            }

            long position = stream.Position;
            stream.Position = 0;
            byte[] bytes = new byte[ (int) stream.Length ];
            stream.Read( bytes, 0, bytes.Length );
            stream.Position = position;

            return bytes;
        }
    }
}

/* eof */