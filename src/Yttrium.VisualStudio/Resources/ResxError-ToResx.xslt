<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:p="urn:platinum/actor"
    xmlns:fn="urn:eo-util"
    exclude-result-prefixes="msxsl p fn">

    <xsl:output method="xml" indent="yes" />

    <xsl:param name="ToolVersion" />
    <xsl:param name="FileName" />
    <xsl:param name="FullFileName" />
    <xsl:param name="Namespace" />


    <xsl:template match=" p:errors ">
        <root>
            <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
                <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
                <xsd:element name="root" msdata:IsDataSet="true">
                    <xsd:complexType>
                        <xsd:choice maxOccurs="unbounded">
                            <xsd:element name="metadata">
                                <xsd:complexType>
                                    <xsd:sequence>
                                        <xsd:element name="value" type="xsd:string" minOccurs="0" />
                                    </xsd:sequence>
                                    <xsd:attribute name="name" use="required" type="xsd:string" />
                                    <xsd:attribute name="type" type="xsd:string" />
                                    <xsd:attribute name="mimetype" type="xsd:string" />
                                    <xsd:attribute ref="xml:space" />
                                </xsd:complexType>
                            </xsd:element>
                            <xsd:element name="assembly">
                                <xsd:complexType>
                                    <xsd:attribute name="alias" type="xsd:string" />
                                    <xsd:attribute name="name" type="xsd:string" />
                                </xsd:complexType>
                            </xsd:element>
                            <xsd:element name="data">
                                <xsd:complexType>
                                    <xsd:sequence>
                                        <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                                        <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
                                    </xsd:sequence>
                                    <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
                                    <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
                                    <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
                                    <xsd:attribute ref="xml:space" />
                                </xsd:complexType>
                            </xsd:element>
                            <xsd:element name="resheader">
                                <xsd:complexType>
                                    <xsd:sequence>
                                        <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                                    </xsd:sequence>
                                    <xsd:attribute name="name" type="xsd:string" use="required" />
                                </xsd:complexType>
                            </xsd:element>
                        </xsd:choice>
                    </xsd:complexType>
                </xsd:element>
            </xsd:schema>
            <resheader name="resmimetype">
                <value>text/microsoft-resx</value>
            </resheader>
            <resheader name="version">
                <value>2.0</value>
            </resheader>
            <resheader name="reader">
                <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </resheader>
            <resheader name="writer">
                <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </resheader>

            <xsl:apply-templates select=" p:error " />
        </root>
    </xsl:template>


    <xsl:template match=" p:error ">
        <data name="{ @id }_Actor">
            <value>
                <xsl:choose>
                    <xsl:when test=" starts-with( @actor, '~' ) ">
                        <xsl:value-of select=" /p:errors/p:actor/@base " />
                        <xsl:value-of select=" substring( @actor, 2 ) " />
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select=" @actor " />
                    </xsl:otherwise>
                </xsl:choose>
            </value>
        </data>
        <data name="{ @id }_Code">
            <value>
                <xsl:value-of select=" @code " />
            </value>
        </data>
        <data name="{ @id }_Description">
            <value>
                <xsl:value-of select=" p:description " />
            </value>
        </data>
    </xsl:template>

</xsl:stylesheet>
