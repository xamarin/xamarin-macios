<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" indent="yes" />
  <xsl:template match="/">
    <xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;&#10;</xsl:text>
    <html>
    <head>
    <title>Test results</title>
    <style type="text/css">

      .strong {
      font-weight: bold;
      }

      .smllabel {
      width:110px;
      font-weight: bold;
      }

      td.left {
      text-align: left;
      }

      td.right {
      text-align: right;
      }

      li {
      font-family: monospace
      }
      .collapsed
      {
      border:0;
      border-collapse: collapse;
      }

    </style>
    </head>
    <body>
    <xsl:apply-templates/>
    </body>
    </html>
  </xsl:template>

  <xsl:template match="test-run">

    <!-- Command Line -->
    <h4>Command Line</h4>
    <pre>
      <xsl:value-of select="command-line"/>
    </pre>
    
    <!-- Runtime Environment -->
    <h4>Runtime Environment</h4>

    <table id="runtime">
      <tr>
        <td class="smllabel right">OS Version:</td>
        <td class="left">
          <xsl:value-of select="test-suite/environment/@os-version[1]"/>
        </td>
      </tr>
      <tr>
        <td class="smllabel right">CLR Version:</td>
        <td class="left">
          <xsl:value-of select="@clr-version"/>
        </td>
      </tr>
      <tr>
        <td colspan="2">
          <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
        </td>
      </tr>
      <tr>
        <td class="smllabel right">NUnit Version:</td>
        <td class="left">
          <xsl:value-of select="@engine-version"/>
        </td>
      </tr>
    </table>

    <!-- Test Files -->
    <div id="testfiles">
      <h4>Test Files</h4>
      <xsl:if test="count(test-suite[@type='Assembly']) > 0">
        <ol>
          <xsl:for-each select="test-suite[@type='Assembly']">
            <li>
              <xsl:value-of select="@fullname"/>
            </li>
          </xsl:for-each>
        </ol>
      </xsl:if>
    </div>

    <!-- Successes -->
    <xsl:if test="//test-case[@result='Passed']">
      <details>
      <summary>Successes</summary>
      <ol>
        <xsl:apply-templates select="//test-case[@result='Passed']"/>
      </ol>
    </details>
    </xsl:if>

    <!-- Tests Not Run -->
    <xsl:if test="//test-case[@result='Skipped']">
      <details>
        <summary>Tests Not Run</summary>
      <ol>
        <xsl:apply-templates select="//test-case[@result='Skipped']"/>
      </ol>
    </details>
    </xsl:if>

    <!-- Errors and Failures -->
    <xsl:if test="//test-case[failure]">
      <details>
      <summary>Errors and Failures</summary>
      <ol>
        <xsl:apply-templates select="//test-case[failure]"/>
      </ol>
    </details>
    </xsl:if>

    <!-- Run Settings (gets first one found) -->
    <xsl:variable name="settings" select ="//settings[1]" />

    <h4>Run Settings</h4>
    <ul>
      <li>
        DefaultTimeout: <xsl:value-of select="$settings/setting[@name='DefaultTimeout']/@value"/>
      </li>
      <li>
        WorkDirectory: <xsl:value-of select="$settings/setting[@name='WorkDirectory']/@value"/>
      </li>
      <li>
        ImageRuntimeVersion: <xsl:value-of select="$settings/setting[@name='ImageRuntimeVersion']/@value"/>
      </li>
      <li>
        ImageTargetFrameworkName: <xsl:value-of select="$settings/setting[@name='ImageTargetFrameworkName']/@value"/>
      </li>
      <li>
        ImageRequiresX86: <xsl:value-of select="$settings/setting[@name='ImageRequiresX86']/@value"/>
      </li>
      <li>
        ImageRequiresDefaultAppDomainAssemblyResolver: <xsl:value-of select="$settings/setting[@name='ImageRequiresDefaultAppDomainAssemblyResolver']/@value"/>
      </li>
      <li>
        NumberOfTestWorkers: <xsl:value-of select="$settings/setting[@name='NumberOfTestWorkers']/@value"/>
      </li>
    </ul>

    <h4>Test Run Summary</h4>
    <table id="summary" class="collapsed">
      <tr>
        <td class="smllabel right">Overall result:</td>
        <td class="left">
          <xsl:value-of select="@result"/>
        </td>
      </tr>
      <tr>
        <td class="smllabel right">Test Count:</td>
        <td class="left">
          <xsl:value-of select="@total"/>, Passed: <xsl:value-of select="@passed"/>, Failed: <xsl:value-of select="@failed"/>, Inconclusive: <xsl:value-of select="@inconclusive"/>, Skipped: <xsl:value-of select="@skipped"/>
        </td>
      </tr>

      <!-- [Optional] - Failed Test Summary -->
      <xsl:if test="@failed > 0">
        <xsl:variable name="failedTotal" select="count(//test-case[@result='Failed' and not(@label)])" />
        <xsl:variable name="errorsTotal" select="count(//test-case[@result='Failed' and @label='Error'])" />
        <xsl:variable name="invalidTotal" select="count(//test-case[@result='Failed' and @label='Invalid'])" />
        <tr>
          <td class="smllabel right">Failed Tests: </td>
          <td class="left">
            Failures: <xsl:value-of select="$failedTotal"/>, Errors: <xsl:value-of select="$errorsTotal"/>, Invalid: <xsl:value-of select="$invalidTotal"/>
          </td>
        </tr>
      </xsl:if>

      <!-- [Optional] - Skipped Test Summary -->
      <xsl:if test="@skipped > 0">
        <xsl:variable name="ignoredTotal" select="count(//test-case[@result='Skipped' and @label='Ignored'])" />
        <xsl:variable name="explicitTotal" select="count(//test-case[@result='Skipped' and @label='Explicit'])" />
        <xsl:variable name="otherTotal" select="count(//test-case[@result='Skipped' and not(@label='Explicit' or @label='Ignored')])" />
        <tr>
          <td class="smllabel right">Skipped Tests: </td>
          <td class="left">
            Ignored: <xsl:value-of select="$ignoredTotal"/>, Explicit: <xsl:value-of select="$explicitTotal"/>, Other: <xsl:value-of select="$otherTotal"/>
          </td>
        </tr>
      </xsl:if>

      <!-- Times -->
      <tr>
        <td class="smllabel right">Start time: </td>
        <td class="left">
          <xsl:value-of select="@start-time"/>
        </td>
      </tr>
      <tr>
        <td class="smllabel right">End time: </td>
        <td class="left">
          <xsl:value-of select="@end-time"/>
        </td>
      </tr>
      <tr>
        <td class="smllabel right">Duration: </td>
        <td class="left">
          <xsl:value-of select="format-number(@duration,'0.000')"/> seconds
        </td>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="test-case">
    <!-- Determine type of test-case for formatting -->
    <xsl:variable name="type">
      <xsl:choose>
        <xsl:when test="@result='Skipped'">
          <xsl:choose>
            <xsl:when test="@label='Ignored' or @label='Explicit'">
              <xsl:value-of select="@label"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'Other'"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:when test="@result='Failed'">
          <xsl:choose>
            <xsl:when test="@label='Error' or @label='Invalid'">
              <xsl:value-of select="@label"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'Failed'"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:when test="@result='Passed'">
           <xsl:value-of select="'Success'"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="'Unknown'"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <!-- Show details of test-cases either skipped or errored -->
    <li>
        <xsl:value-of select="concat($type,' : ', @fullname)" />
        <br/>
      <pre>
        <xsl:value-of select="child::node()/message"/>

        <xsl:choose>
          <xsl:when test="$type='Failed'">
            <br/>
          </xsl:when>
          <xsl:when test="$type='Error'">
            <br/>
          </xsl:when>
          <xsl:when test="$type='Passed'">
            <br/>
          </xsl:when>
        </xsl:choose>

        <!-- Stack trace for failures -->
        <xsl:if test="failure and ($type='Failed' or $type='Error')">
          <xsl:value-of select="failure/stack-trace"/>
        </xsl:if>
      </pre>

      <xsl:if test="child::node()/attachment">
        <ul>
        <xsl:for-each select="child::node()/attachment">
           <li> <a href="file://{filePath}"><xsl:value-of select="description"/></a></li>
        </xsl:for-each>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>