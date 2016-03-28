using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
    [
        SuppressMessage( "Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Vs" ),
        ComImport,
        Guid( "3634494C-492F-4F91-8009-4541234E4E99" ),
        InterfaceTypeAttribute( ComInterfaceType.InterfaceIsIUnknown )
    ]
    public interface IVsSingleFileGenerator
    {
        //
        // Retrieve default properties for the generator
        // [propget]   HRESULT DefaultExtension([out,retval] BSTR* pbstrDefaultExtension);
        //
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate" )]
        [return: MarshalAs( UnmanagedType.BStr )]
        string GetDefaultExtension();

        //
        // Generate the file
        // HRESULT Generate([in] LPCOLESTR wszInputFilePath,
        //					[in] BSTR bstrInputFileContents,
        //					[in] LPCOLESTR wszDefaultNamespace, 
        //					[out] BYTE**    rgbOutputFileContents,
        //					[out] ULONG*    pcbOutput,
        //					[in] IVsGeneratorProgress* pGenerateProgress);
        //

        [
            SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#" ),
            SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#" )
        ]
        void Generate(
            [MarshalAs( UnmanagedType.LPWStr )] string wszInputFilePath,
              [MarshalAs( UnmanagedType.BStr )] string bstrInputFileContents,
            [MarshalAs( UnmanagedType.LPWStr )] string wszDefaultNamespace,
                                                out IntPtr rgbOutputFileContents,
                [MarshalAs( UnmanagedType.U4 )] out int pcbOutput,
                                                IVsGeneratorProgress pGenerateProgress );
    }
}

/* eof */