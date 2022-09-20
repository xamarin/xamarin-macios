<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    exclude-result-prefixes="xs"
    version="1.0">
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>
    <!-- Do not copy those elements with an empty Include -->
    <xsl:template match="//*[@Include[not(string())]]" />
    <!-- Do not copy those elements with an empty version -->
    <xsl:template match="//*[@Version[not(string())]]" />
</xsl:stylesheet>
