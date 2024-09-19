<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:testIdGenerator="urn:hash-generator">
  <xsl:output cdata-section-elements="message stack-trace"/>

  <xsl:template match="/">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="assemblies">
    <test-run name="Test results" fullname="Test results" inconclusive="0" asserts="0" id="2"> 
      <!-- test-run attributes -->
      <xsl:attribute name="testcasecount">
        <xsl:value-of select="sum(assembly/@total)"/>
      </xsl:attribute>
      <xsl:attribute name="total">
        <xsl:value-of select="sum(assembly/@total)"/>
      </xsl:attribute>
      <xsl:attribute name="passed">
        <xsl:value-of select="sum(assembly/@passed)"/>
      </xsl:attribute>
      <xsl:attribute name="failed">
        <xsl:value-of select="sum(assembly/@failed)"/>
      </xsl:attribute>
      <xsl:attribute name="skipped">
        <xsl:value-of select="sum(assembly/@skipped)"/>
      </xsl:attribute>
      <xsl:attribute name="time">
        <xsl:value-of select="sum(assembly/@time)"/>
      </xsl:attribute>
      <xsl:attribute name="run-date">
        <xsl:value-of select="assembly[1]/@run-date"/>
      </xsl:attribute>
      <xsl:attribute name="start-time">
        <xsl:value-of select="assembly[1]/@start-time"/>
      </xsl:attribute>
      <xsl:attribute name="result">
        <xsl:if test="sum(assembly/@failed) > 0">Failed</xsl:if>
        <xsl:if test="sum(assembly/@failed) = 0">Passed</xsl:if>
      </xsl:attribute>
      <!-- end of the test-run attributes -->
      <environment os-version="unknown" platform="unknown" cwd="unknown" machine-name="unknown" user="unknown" user-domain="unknown">
        <xsl:attribute name="nunit-version">
          <xsl:value-of select="assembly[1]/@test-framework"/>
        </xsl:attribute>
        <xsl:attribute name="clr-version">
          <xsl:value-of select="assembly[1]/@environment"/>
        </xsl:attribute>
      </environment>
      <!-- failure element if any -->
      <xsl:if test="sum(assembly/@failed) > 0">
        <failure>
          <message><![CDATA[Child test failed]]></message>
        </failure>
      </xsl:if>
      <!-- end enviroment --> 
      <!-- Add all the test-suite="Assembly" that maps to <assembly> in xunit -->
      <xsl:apply-templates select="assembly"/>
    </test-run>
  </xsl:template>

  <!-- Map Assembly to test-suite name="Assembly" -->
  <xsl:template match="assembly">
    <test-suite type="Assembly">
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="testcasecount">
        <xsl:value-of select="@total"/>
      </xsl:attribute>
      <xsl:attribute name="total">
        <xsl:value-of select="@total"/>
      </xsl:attribute>
      <xsl:attribute name="passed">
        <xsl:value-of select="@passed"/>
      </xsl:attribute>
      <xsl:attribute name="failed">
        <xsl:value-of select="@failed"/>
      </xsl:attribute>
      <xsl:attribute name="skipped">
        <xsl:value-of select="@skipped"/>
      </xsl:attribute>
      <xsl:attribute name="result">
        <xsl:if test="@failed > 0">Failed</xsl:if>
        <xsl:if test="@failed = 0">Passed</xsl:if>
      </xsl:attribute>
      <xsl:attribute name="time">
        <xsl:value-of select="@time"/>
      </xsl:attribute>
      <!-- failure element if any -->
       <xsl:if test="@failed > 0">
        <failure>
          <message><![CDATA[Child test failed]]></message>
        </failure>
      </xsl:if>
      <xsl:apply-templates select="collection"/>
    </test-suite>
  </xsl:template>

  <!-- Map colection to test-case name="TestFixture" -->
  <xsl:template match="collection">
    <xsl:param name="hash_source" select="@name"/>
    <test-suite type="TestFixture" inconclusive="0">
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:value-of select="testIdGenerator:GenerateHash($hash_source)"/>
      </xsl:attribute>
      <xsl:attribute name="testcasecount">
        <xsl:value-of select="@total"/>
      </xsl:attribute>
      <xsl:attribute name="total">
        <xsl:value-of select="@total"/>
      </xsl:attribute>
      <xsl:attribute name="result">
        <xsl:if test="@failed > 0">Failed</xsl:if>
        <xsl:if test="@failed = 0">Passed</xsl:if>
      </xsl:attribute>
      <xsl:attribute name="time">
        <xsl:value-of select="@time"/>
      </xsl:attribute>
      <xsl:attribute name="passed">
        <xsl:value-of select="@passed"/>
      </xsl:attribute>
      <xsl:attribute name="failed">
        <xsl:value-of select="@failed"/>
      </xsl:attribute>
      <xsl:attribute name="skipped">
        <xsl:value-of select="@skipped"/>
      </xsl:attribute>

      <!-- failure element if any -->
       <xsl:if test="@failed > 0">
        <failure>
          <message><![CDATA[Child test failed]]></message>
        </failure>
      </xsl:if>
      <xsl:apply-templates select="test"/>
    </test-suite>
  </xsl:template>

  <!-- Map test to test-case -->

  <xsl:template match="test">
    <xsl:param name="hash_source" select="@name"/>
    <test-case>
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="fullname">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:value-of select="testIdGenerator:GenerateHash($hash_source)"/>
      </xsl:attribute>
      <xsl:attribute name="result">
        <xsl:if test="@result='Fail'">Failed</xsl:if>
        <xsl:if test="@result='Pass'">Passed</xsl:if>
        <xsl:if test="@result='Skip'">Skipped</xsl:if>
      </xsl:attribute>
      <xsl:if test="@time">
        <xsl:attribute name="time">
          <xsl:value-of select="@time"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates select="traits"/>
      <xsl:if test="reason">
        <reason>
          <message>
            <xsl:apply-templates select="reason"/>
          </message>
        </reason>
      </xsl:if>
      <xsl:apply-templates select="failure"/>
    </test-case>
  </xsl:template>

  <!-- Map traits to properties -->
  <xsl:template match="traits">
    <properties>
      <xsl:apply-templates select="trait"/>
    </properties>
  </xsl:template>

  <!-- Map a single trait to a property -->
  <xsl:template match="trait">
    <property>
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="value">
        <xsl:value-of select="@value"/>
      </xsl:attribute>
    </property>
  </xsl:template>

  <!-- failures are the same -->
  <xsl:template match="failure">
    <failure>
      <xsl:copy-of select="node()"/>
    </failure>
  </xsl:template>

</xsl:stylesheet>