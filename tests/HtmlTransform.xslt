<?xml version="1.0" encoding="UTF-8" ?>
<!-- Got this from here: http://bazaar.launchpad.net/~charlie.poole/nunit-summary/trunk/view/head:/Transforms/DefaultTransform.xslt -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method='text'/>
  <xsl:template match="/">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="test-results">
    <xsl:text>&lt;b&gt;</xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text>&lt;/b&gt;&lt;br&gt;&lt;br&gt;&#xD;&#xA;&#xD;&#xA;</xsl:text>

    <xsl:text>&lt;b&gt;NUnit Version:&lt;/b&gt; </xsl:text>
    <xsl:value-of select="environment/@nunit-version"/>
    <xsl:text>&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;b&gt;Date:&lt;/b&gt; </xsl:text>
    <xsl:value-of select="@date"/>
    <xsl:text>&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;b&gt;Time:&lt;/b&gt; </xsl:text>
    <xsl:value-of select="@time"/>
    <xsl:text>&lt;br&gt;&lt;br&gt;&#xD;&#xA;&#xD;&#xA;</xsl:text>

    <xsl:text>&lt;b&gt;Runtime Environment -&lt;/b&gt;&lt;br&gt;&#xD;&#xA;</xsl:text>
    <xsl:text>&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;b&gt;OS Version:&lt;/b&gt; </xsl:text>
    <xsl:value-of select="environment/@os-version"/>
    <xsl:text>&lt;br&gt;&#xD;&#xA;</xsl:text>
    <xsl:text>&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;b&gt;CLR Version:&lt;/b&gt; </xsl:text>
    <xsl:value-of select="environment/@clr-version"/>
    <xsl:text>&lt;br&gt;&lt;br&gt;&#xD;&#xA;&#xD;&#xA;</xsl:text>

    <xsl:text>&lt;b&gt;Tests run: </xsl:text>
    <xsl:value-of select="@total"/>
    <xsl:choose>
      <xsl:when test ="substring(environment/@nunit-version,1,3)>='2.5'">
        <xsl:text>, Errors: </xsl:text>
        <xsl:value-of select="@errors"/>
        <xsl:text>, Failures: </xsl:text>
        <xsl:value-of select="@failures"/>
        <xsl:if test="@inconclusive">
          <!-- Introduced in 2.5.1 -->
          <xsl:text>, Inconclusive: </xsl:text>
          <xsl:value-of select="@inconclusive"/>
        </xsl:if>
        <xsl:text>, Time: </xsl:text>
        <xsl:value-of select="test-suite/@time"/>
        <xsl:text> seconds&lt;br&gt;</xsl:text>
        <xsl:text>&amp;nbsp;&amp;nbsp;&amp;nbsp;Not run: </xsl:text>
        <xsl:value-of select="@not-run"/>
        <xsl:text>, Invalid: </xsl:text>
        <xsl:value-of select="@invalid"/>
        <xsl:text>, Ignored: </xsl:text>
        <xsl:value-of select="@ignored"/>
        <xsl:text>, Skipped: </xsl:text>
        <xsl:value-of select="@skipped"/>
        <xsl:text>&lt;/b&gt;&lt;br&gt;&lt;br&gt;&#xD;&#xA;</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>, Failures: </xsl:text>
        <xsl:value-of select="@failures"/>
        <xsl:text>, Not run: </xsl:text>
        <xsl:value-of select="@not-run"/>
        <xsl:text>, Time: </xsl:text>
        <xsl:value-of select="test-suite/@time"/>
        <xsl:text> seconds&lt;/b&gt;&lt;br&gt;&lt;br&gt;</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>
</xsl:text>

    <xsl:if test="//test-case[failure]">
      <xsl:text>&lt;h4&gt;Failures:&lt;/h4&gt;&#xD;&#xA;</xsl:text>
      <xsl:text>&lt;ol&gt;&#xD;&#xA;</xsl:text>
      <xsl:apply-templates select="//test-case[failure]"/>
      <xsl:text>&lt;/ol&gt;&#xD;&#xA;</xsl:text>
    </xsl:if>

    <xsl:if test="//test-case[@executed='False']">
      <xsl:text>&lt;h4&gt;Tests not run:&lt;/h4&gt;&#xD;&#xA;</xsl:text>
      <xsl:text>&lt;ol&gt;&#xD;&#xA;</xsl:text>
      <xsl:apply-templates select="//test-case[@executed='False']"/>
      <xsl:text>&lt;/ol&gt;&#xD;&#xA;</xsl:text>
    </xsl:if>

    <xsl:text disable-output-escaping='yes'>&#xD;&#xA;</xsl:text>
    <xsl:text>&lt;hr&gt;&#xD;&#xA;</xsl:text>
  </xsl:template>

  <xsl:template match="test-case">
    <xsl:text>&lt;pre&gt;&#xD;&#xA;</xsl:text>
    <xsl:text>&lt;li&gt;</xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text> : </xsl:text>
    <xsl:value-of select="child::node()/message"/>
    <xsl:text disable-output-escaping='yes'>&#xD;&#xA;</xsl:text>
    <xsl:if test="failure">
      <xsl:value-of select="failure/stack-trace"/>
    </xsl:if>
    <xsl:text>&lt;/pre&gt;&#xD;&#xA;</xsl:text>
  </xsl:template>

</xsl:stylesheet>