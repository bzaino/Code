<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="xml" version="1.0"
encoding="UTF-8" indent="yes" cdata-section-elements="PVAL"/>
<xsl:strip-space elements="*"/> 


<xsl:template match="/percPage">
<RECORDS>
<RECORD>
	<xsl:variable name="primaryKey">
		<xsl:value-of select="@id" />
	</xsl:variable>
	<PROP NAME="PRIMARY KEY">
		<PVAL><xsl:value-of select="substring-after($primaryKey, '-')" /></PVAL>
	</PROP>
	<PROP NAME="Item_Version">
		<PVAL><xsl:value-of select="substring-before($primaryKey, '-')" /></PVAL>
	</PROP>
<xsl:apply-templates/>
</RECORD>
</RECORDS>
</xsl:template>

<xsl:template match="/percPage/content">
	<xsl:for-each select="region/widget">
		<xsl:if test="not(@name = '')">
			<xsl:variable name="normalizeData">
				<xsl:value-of select="normalize-space(.)" />
			</xsl:variable>
			<xsl:variable name="firstPass">		
				<xsl:call-template name="strip-leadingDivTag"> 
					<xsl:with-param name="text" select="$normalizeData"/> 
				</xsl:call-template>
			</xsl:variable>
			<xsl:variable name="secondPass">
				<xsl:call-template name="strip-trailingDivTag">
					<xsl:with-param name="text" select="$firstPass"/>
				</xsl:call-template>
			</xsl:variable>
			<PROP NAME="{@name}">
				<PVAL>
					<xsl:value-of select="($secondPass)" />
				</PVAL>
          </PROP>
        </xsl:if>
		<xsl:if test="@name = 'body'">
			<PROP NAME="Word-Count">
				<PVAL>
					<xsl:value-of select="string-length(normalize-space(.)) - string-length(translate(normalize-space(.),' ','')) -6" />
				</PVAL>
			</PROP>
		</xsl:if>
	</xsl:for-each>
</xsl:template>

<xsl:template match="/percPage/metadata/*">
	<xsl:choose>
          <xsl:when test="self::tags">
			<xsl:for-each select="page_tags">
				<PROP NAME="{local-name()}">
					<PVAL><xsl:value-of select="."/></PVAL>
				</PROP>
			</xsl:for-each>
          </xsl:when>
		  <xsl:when test="self::categories">
			<xsl:for-each select="page_categories">
				<PROP NAME="{local-name()}">
					<PVAL><xsl:value-of select="."/></PVAL>
				</PROP>
			</xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
				<xsl:choose>
					<xsl:when test="(local-name() = 'page_summary')">
						<xsl:variable name="normalizeData">
							<xsl:value-of select="normalize-space(.)" />
						</xsl:variable>
						<xsl:variable name="firstPass">		
							<xsl:call-template name="strip-leadingDivTag"> 
								<xsl:with-param name="text" select="$normalizeData"/> 
							</xsl:call-template>
						</xsl:variable>
						<xsl:variable name="secondPass">
							<xsl:call-template name="strip-trailingDivTag">
								<xsl:with-param name="text" select="$firstPass"/>
							</xsl:call-template>
						</xsl:variable>
						<PROP NAME="{local-name()}">
							<PVAL>
								<xsl:value-of select="($secondPass)" />
							</PVAL>
						</PROP>
					</xsl:when>
					<xsl:otherwise>
						<PROP NAME="{local-name()}">
							<PVAL><xsl:value-of select="."/></PVAL>
						</PROP>
					</xsl:otherwise>
				</xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
</xsl:template>

<!-- This will remove all html tags in a string -->
<!--
<xsl:template name="remove-html">
    <xsl:param name="text"/>
    <xsl:choose>
        <xsl:when test="contains($text, '&lt;')">
            <xsl:value-of select="substring-before($text, '&lt;')"/>
            <xsl:call-template name="remove-html">
                    <xsl:with-param name="text" select="substring-after($text, '&gt;')"/>
            </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
            <xsl:value-of select="$text"/>
        </xsl:otherwise>
    </xsl:choose>
</xsl:template>
-->

<xsl:template name="strip-leadingDivTag"> 
    <xsl:param name="text"/> 
    <xsl:choose> 
        <xsl:when test="contains($text, '&lt;div')"> 
            <xsl:value-of select="substring-before($text, '&lt;div')"/> 
            <xsl:call-template name="strip-leadingDivTag"> 
                    <xsl:with-param name="text" select="substring-after($text, '&gt;')"/> 
            </xsl:call-template> 
        </xsl:when> 
        <xsl:otherwise> 
            <xsl:value-of select="$text"/> 
        </xsl:otherwise> 
    </xsl:choose> 
</xsl:template>
  
<xsl:template name="strip-trailingDivTag"> 
    <xsl:param name="text"/> 
    <xsl:choose> 
        <xsl:when test="contains($text, '&lt;/div&gt;')"> 
            <xsl:value-of select="substring-before($text, '&lt;/div&gt;')"/> 
            <xsl:call-template name="strip-trailingDivTag"> 
                    <xsl:with-param name="text" select="substring-after($text, '&lt;/div&gt;')"/> 
            </xsl:call-template> 
        </xsl:when> 
        <xsl:otherwise> 
            <xsl:value-of select="$text"/> 
        </xsl:otherwise> 
    </xsl:choose> 
</xsl:template>
</xsl:stylesheet> 

