﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:fn="urn:eo-util"
    exclude-result-prefixes="msxsl fn">

    <xsl:output method="text" indent="no" />

    <xsl:param name="ToolVersion" />
    <xsl:param name="FileName" />
    <xsl:param name="FullFileName" />
    <xsl:param name="Namespace" />


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ tools/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" tools ">
        <xsl:text>// autogenerated: do NOT edit manually
using CustomToolGenerator;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace </xsl:text>
        <xsl:value-of select=" $Namespace " />
        <xsl:text>
{</xsl:text>

        <xsl:apply-templates select=" add ">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>

        <xsl:text>
}

/* eof */
</xsl:text>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ add/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" add ">
        <xsl:if test=" position() > 1 ">
            <xsl:text>
</xsl:text>
        </xsl:if>

        <!-- Attributes -->
        <xsl:text>
</xsl:text>
        <xsl:text>    [ComVisible( true )]</xsl:text>

        <xsl:text>
</xsl:text>
        <xsl:text>    [Guid( "</xsl:text>
        <xsl:value-of select=" @guid " />
        <xsl:text>" )]</xsl:text>

        <!-- Class decl -->
        <xsl:text>
    </xsl:text>
        <xsl:text>public partial class </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text> : BaseTool</xsl:text>

        <xsl:text>
    {</xsl:text>


        <!-- Register -->
        <xsl:text>
        [ComRegisterFunction]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "t" )]
        private static void RegisterClass( Type t )
        {
            Guid category = Registration.CSharpCategoryGuid;
            Type type = typeof( </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text> );
            string desc = "</xsl:text>
        <xsl:value-of select=" description "/>
        <xsl:text>";

            Registration.RegisterCustomTool( "</xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>", category, type, desc );
        }</xsl:text>

        <!-- Unregister -->
        <xsl:text>
</xsl:text>
        <xsl:text>
        [ComUnregisterFunction]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "t" )]
        private static void UnregisterClass( Type t )
        {
            Guid category = Registration.CSharpCategoryGuid;
            Type type = typeof( </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text> );

            Registration.UnregisterCustomTool( "</xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text>", category, type );
        }</xsl:text>

        <!-- End -->
        <xsl:text>
    }</xsl:text>

    </xsl:template>


</xsl:stylesheet>
