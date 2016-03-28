<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

    <xsl:output method="xml" indent="yes" />

    <xsl:param name="IncludeComments" select=" 'false' " />


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Manifest root
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" DbManifest ">
        <manifest xmlns="urn:yttrium:dbmanifest">
            <database name="{ Database/name }" collation="{ Database/collation_name }">
                <xsl:if test=" Database/is_fulltext_enabled = 'true' ">
                    <fulltext enabled="true" />
                </xsl:if>

                <xsl:if test=" Database/is_broker_enabled = 'true' ">
                    <broker enabled="true" />
                </xsl:if>
            </database>

            <xsl:apply-templates select=" Table ">
                <xsl:sort select=" name " />
            </xsl:apply-templates>

            <xsl:apply-templates select=" View ">
                <xsl:sort select=" name " />
            </xsl:apply-templates>

            <xsl:apply-templates select=" Procedure ">
                <xsl:sort select=" name " />
            </xsl:apply-templates>

            <xsl:apply-templates select=" Function ">
                <xsl:sort select=" name " />
            </xsl:apply-templates>
        </manifest>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Table
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" Table ">
        <table name="{ name/text() }" xmlns="urn:yttrium:dbmanifest">
            <xsl:apply-templates select=" ../TableColumn[ object_id/text() = current()/object_id/text() ] " />
            <xsl:apply-templates select=" ../Index[ object_id/text() = current()/object_id/text() ] " />
        </table>
    </xsl:template>

    <xsl:template match=" TableColumn | ViewColumn ">
        <column name="{ name/text() }" xmlns="urn:yttrium:dbmanifest">
            <xsl:attribute name="type">
                <xsl:call-template name="dbType" />
            </xsl:attribute>

            <xsl:if test=" is_nullable/text() = 'true' ">
                <xsl:attribute name="optional">true</xsl:attribute>
            </xsl:if>

            <xsl:if test=" collation_name and collation_name != ../Database/collation_name ">
                <xsl:attribute name="collation">
                    <xsl:value-of select=" collation_name/text() "/>
                </xsl:attribute>
            </xsl:if>
        </column>
    </xsl:template>

    <xsl:template match=" Index ">
        <xsl:variable name="type">
            <xsl:choose>
                <xsl:when test=" is_primary_key = 'true' ">
                    <xsl:text>primaryKey</xsl:text>
                </xsl:when>

                <xsl:when test=" is_unique_constraint = 'true' ">
                    <xsl:text>unique</xsl:text>
                </xsl:when>

                <xsl:otherwise>
                    <xsl:text>index</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>

        <xsl:element name="{ $type }" namespace="urn:yttrium:dbmanifest">
            <xsl:attribute name="name">
                <xsl:value-of select=" name " />
            </xsl:attribute>

            <xsl:if test=" type = 1 ">
                <xsl:attribute name="clustered">
                    <xsl:text>true</xsl:text>
                </xsl:attribute>
            </xsl:if>

            <xsl:apply-templates select=" ../IndexColumn[     object_id/text() = current()/object_id/text()
                                                          and index_id/text() = current()/index_id/text() ] " />
        </xsl:element>
    </xsl:template>

    <xsl:template match=" IndexColumn ">
        <column name="{ name/text() }" xmlns="urn:yttrium:dbmanifest">
            <xsl:if test=" is_descending_key = 'true' ">
                <xsl:attribute name="order">
                    <xsl:text>desc</xsl:text>
                </xsl:attribute>
            </xsl:if>
        </column>
    </xsl:template>

    <xsl:template match=" IndexColumn[ is_included_column = 'true' ] ">
        <include name="{ name/text() }" xmlns="urn:yttrium:dbmanifest" />
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ View
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" View ">
        <view name="{ name/text() }" checksum="{ checksum }" xmlns="urn:yttrium:dbmanifest">
            <xsl:apply-templates select=" ../ViewColumn[ object_id/text() = current()/object_id/text() ] " />
        </view>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Procedure
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" Procedure ">
        <procedure name="{ name }" checksum="{ checksum }" xmlns="urn:yttrium:dbmanifest">
            <xsl:apply-templates select=" ../ProcedureParameter[ object_id/text() = current()/object_id/text() ] " />
        </procedure>
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Function
    ~   
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" Function ">
        <function name="{ name }" checksum="{ checksum }" xmlns="urn:yttrium:dbmanifest">
            <xsl:attribute name="type">
                <xsl:choose>
                    <xsl:when test=" type = 'FN' ">
                        <xsl:text>scalar</xsl:text>
                    </xsl:when>
                    <xsl:when test=" type = 'IF' ">
                        <xsl:text>inline-table-valued</xsl:text>
                    </xsl:when>
                    <xsl:when test=" type = 'TF' ">
                        <xsl:text>table-valued</xsl:text>
                    </xsl:when>
                    <xsl:when test=" type = 'FS' ">
                        <xsl:text>clr-scalar</xsl:text>
                    </xsl:when>
                    <xsl:when test=" type = 'FT' ">
                        <xsl:text>clr-table-valued</xsl:text>
                    </xsl:when>
                </xsl:choose>
            </xsl:attribute>

            <xsl:apply-templates select=" ../FunctionParameter[ object_id/text() = current()/object_id/text() ] " />
        </function>
    </xsl:template>


    <xsl:template match=" ProcedureParameter | FunctionParameter ">
        <param name="{ name/text() }" xmlns="urn:yttrium:dbmanifest">
            <xsl:attribute name="type">
                <xsl:call-template name="dbType" />
            </xsl:attribute>

            <xsl:if test=" is_output/text() = 'true' ">
                <xsl:attribute name="direction">
                    <xsl:text>output</xsl:text>
                </xsl:attribute>
            </xsl:if>

            <xsl:if test=" collation_name ">
                <xsl:attribute name="collation">
                    <xsl:value-of select=" collation_name/text() "/>
                </xsl:attribute>
            </xsl:if>
        </param>
    </xsl:template>



    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Shared
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template name="dbType">
        <xsl:choose>
            <xsl:when test=" system_type = 'varchar' or system_type = 'char' ">
                <xsl:value-of select=" system_type " />
                <xsl:text>(</xsl:text>
                <xsl:choose>
                    <xsl:when test=" max_length = '-1' ">
                        <xsl:text>max</xsl:text>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select=" max_length " />
                    </xsl:otherwise>
                </xsl:choose>
                <xsl:text>)</xsl:text>
            </xsl:when>

            <xsl:when test=" system_type = 'nvarchar' or system_type = 'nchar' ">
                <xsl:value-of select=" system_type " />
                <xsl:text>(</xsl:text>
                <xsl:choose>
                    <xsl:when test=" max_length = '-1' ">
                        <xsl:text>max</xsl:text>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select=" number( max_length ) div 2 " />
                    </xsl:otherwise>
                </xsl:choose>
                <xsl:text>)</xsl:text>
            </xsl:when>

            <xsl:when test=" system_type = 'decimal' or system_type = 'numeric' ">
                <xsl:value-of select=" system_type "/>
                <xsl:text>(</xsl:text>
                <xsl:value-of select=" precision "/>
                <xsl:text>,</xsl:text>
                <xsl:value-of select=" scale "/>
                <xsl:text>)</xsl:text>
            </xsl:when>

            <xsl:otherwise>
                <xsl:value-of select=" system_type "/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
