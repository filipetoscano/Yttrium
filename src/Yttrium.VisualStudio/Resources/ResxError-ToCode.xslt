﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:p="urn:platinum/actor"
    xmlns:fn="urn:eo-util"
    exclude-result-prefixes="msxsl p fn">

    <xsl:output method="text" indent="no" />

    <xsl:param name="ToolVersion" />
    <xsl:param name="FileName" />
    <xsl:param name="FullFileName" />
    <xsl:param name="Namespace" />


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ p:errors/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" p:errors ">
        <xsl:text>// autogenerated: do NOT edit manually

namespace </xsl:text>
        <xsl:value-of select=" $Namespace " />
        <xsl:text>
{
    /// &lt;summary /&gt;
    public static class ER
    {</xsl:text>

        <xsl:apply-templates select=" p:error " />

        <xsl:text>
    }
}

/* eof */
</xsl:text>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ p:error/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" p:error ">
        <xsl:if test=" position() > 1 ">
            <xsl:text>
</xsl:text>
        </xsl:if>

        <xsl:text>
        /// &lt;summary&gt;</xsl:text>

        <xsl:value-of select=" fn:ToWrap( p:description, '        /// ', 70 ) "/>

        <xsl:text>
        /// Actor: </xsl:text>
        <xsl:choose>
            <xsl:when test=" starts-with( @actor, '~' ) ">
                <xsl:value-of select=" /p:errors/p:actor/@base " />
                <xsl:value-of select=" substring( @actor, 2 ) " />
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select=" @actor " />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:text>, Code: </xsl:text>
        <xsl:value-of select=" @code " />

        <xsl:text>
        /// &lt;/summary&gt;</xsl:text>

        <xsl:text>
        public const string </xsl:text>
        <xsl:value-of select=" @id "/>
        <xsl:text> = "</xsl:text>
        <xsl:value-of select=" @id "/>
        <xsl:text>";</xsl:text>
    </xsl:template>

</xsl:stylesheet>
