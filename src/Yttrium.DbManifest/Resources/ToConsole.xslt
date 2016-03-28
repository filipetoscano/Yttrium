<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:m="urn:yttrium:dbmanifest"
    xmlns:eo="urn:eo"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

    <xsl:output method="text" indent="yes" />


    <xsl:template match=" m:manifest ">
        <xsl:text>DB </xsl:text>
        <xsl:value-of select=" eo:pad( m:database/@name, 25 ) " />
        <xsl:text xml:space="preserve"> </xsl:text>
        <xsl:value-of select=" m:database/@collation " />

        <xsl:if test=" m:database/m:fulltext/@enabled = 'true' ">
            <xsl:text>&#10;</xsl:text>
            <xsl:text>   full text = enabled</xsl:text>
        </xsl:if>

        <xsl:if test=" m:database/m:broker/@enabled = 'true' ">
            <xsl:text>&#10;</xsl:text>
            <xsl:text>   broker    = enabled</xsl:text>
        </xsl:if>

        <xsl:apply-templates select=" m:table " />
        <xsl:apply-templates select=" m:view " />
        <xsl:apply-templates select=" m:procedure " />
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Table / View
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" m:table | m:view ">
        <xsl:text>&#10;</xsl:text>
        <xsl:text>&#10;</xsl:text>

        <xsl:if test=" local-name(.) = 'table' ">
            <xsl:text>T</xsl:text>
        </xsl:if>
        <xsl:if test=" local-name(.) = 'view' ">
            <xsl:text>V</xsl:text>
        </xsl:if>

        <xsl:text xml:space="preserve">  </xsl:text>
        <xsl:value-of select=" @name " />

        <xsl:for-each select=" m:column ">
            <xsl:text>&#10;</xsl:text>
            <xsl:text xml:space="preserve">   </xsl:text>
            <xsl:value-of select=" eo:pad( @name, 25 ) " />
            <xsl:text xml:space="preserve"> </xsl:text>
            <xsl:value-of select=" eo:pad( @type, 17 ) " />
            <xsl:text xml:space="preserve"> </xsl:text>
            <xsl:choose>
                <xsl:when test=" @optional = 'true' ">
                    <xsl:text>Y</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text xml:space="preserve"> </xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:for-each>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Procedure
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" m:procedure | m:function ">
        <xsl:text>&#10;&#13;</xsl:text>

        <xsl:choose>
            <xsl:when test=" local-name(.) = 'procedure' ">
                <xsl:text>P  </xsl:text>
            </xsl:when>
            <xsl:when test=" @type = 'scalar' ">
                <xsl:text>FN </xsl:text>
            </xsl:when>
            <xsl:when test=" @type = 'inline-table-valued' ">
                <xsl:text>IF </xsl:text>
            </xsl:when>
            <xsl:when test=" @type = 'table-valued' ">
                <xsl:text>TF </xsl:text>
            </xsl:when>
            <xsl:when test=" @type = 'clr-scalar' ">
                <xsl:text>FS </xsl:text>
            </xsl:when>
            <xsl:when test=" @type = 'clr-table-valued' ">
                <xsl:text>FT </xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:text>?  </xsl:text>
            </xsl:otherwise>
        </xsl:choose>
        
        <xsl:value-of select=" @name " />

        <xsl:for-each select=" m:param ">
            <xsl:text>&#10;</xsl:text>
            <xsl:text xml:space="preserve"> </xsl:text>
            <xsl:choose>
                <xsl:when test=" @direction = 'output' ">
                    <xsl:text xml:space="preserve">*</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text xml:space="preserve"> </xsl:text>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:text xml:space="preserve"> </xsl:text>
            <xsl:value-of select=" eo:pad( @name, 25 ) " />
            <xsl:text xml:space="preserve"> </xsl:text>
            <xsl:value-of select=" @type " />
        </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>
