<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" version="4.0" indent="yes"/>

  <!-- Header of the Analysis document -->
  <xsl:template name="header">
    <div id="header">
      <img src="images/InputLog-Logo.gif" alt="InputLog logo" class="logo"/>
      <br class="clear" />
    </div>
  </xsl:template>

  <!-- Displays meta and sessionidentication information in a table -->
  <xsl:template name="meta_sessionidentification_parameters">
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
      <colgroup>
        <col class="collapse" />
        <col />
      </colgroup>

      <xsl:if test="session/meta">
        <tr>
          <th align="left" colspan="2">Meta Information</th>
        </tr>
        <tr>
          <td>Logfile</td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='Logfile']/@value" />
          </td>
        </tr>
        <tr>
          <xsl:if test="session/meta/entry[@name='ConvertedFrom']/@value!=''">
            <td>Converted from</td>
            <td>
              <xsl:value-of select="session/meta/entry[@name='ConvertedFrom']/@value" />
            </td>
          </xsl:if>
        </tr>
        <tr>
          <td>Main Document</td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='MainDocument']/@value" />
          </td>
        </tr>
        <tr>
          <td>Log Creation</td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='LogCreationDate']/@value" />
          </td>
        </tr>
        <tr>
          <td>
            Log <abbr  title="A globally unique identifier or GUID is a special type of identifier used in software applications to provide a unique reference number. In other words, the Log GUID uniquely defines a logging session.">GUID</abbr >
          </td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='LogGUID']/@value" />
          </td>
        </tr>
        <tr>
          <td>
            <abbr title="The Inputlog version that performed the logging.">Logging Program Version Number</abbr>
          </td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='LogProgramVersion']/@value" />
          </td>
        </tr>
        <tr>
          <td>Analysis Creation</td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='AnalysisCreationDate']/@value" />
          </td>
        </tr>
        <tr>
          <td>
            Analysis <abbr  title="A globally unique identifier or GUID is a special type of identifier used in software applications to provide a unique reference number. In other words, the Analysis GUID uniquely defines an analysis.">GUID</abbr >
          </td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='AnalysisGUID']/@value" />
          </td>
        </tr>
        <tr>
          <td>
            <abbr title="The version number of the program that performed the analysis.">Analysis Program Version Number</abbr>
          </td>
          <td>
            <xsl:value-of select="session/meta/entry[@name='AnalysisProgramVersion']/@value" />
          </td>
        </tr>
      </xsl:if>

      <xsl:if test="session/sessionIdentification">
        <tr>
          <th align="left" colspan="2">Session Identification</th>
        </tr>
        <xsl:for-each select="session/sessionIdentification/entry">
          <xsl:if test="@value!=''">
            <tr>
              <td>
                <xsl:value-of select="@name"/>
              </td>
              <td>
                <xsl:value-of select="@value"/>
              </td>
            </tr>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>

      <xsl:for-each select="session/extraInfo">
        <tr>
          <th align="left" colspan="2">
            <xsl:value-of select="@title"/>
          </th>
        </tr>
        <xsl:for-each select="entry">
          <tr>
            <td>
              <xsl:value-of select="@name"/>
            </td>
            <td>
              <xsl:value-of select="@value"/>
            </td>
          </tr>
        </xsl:for-each>
      </xsl:for-each>

    </table>
    <hr></hr>
  </xsl:template>

  <xsl:template name="single_module">
    <xsl:param name="module"/>
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
      <xsl:if test="$module/@name">
        <tr>
          <td align="left" colspan="2" class="moduleHeader">
            <xsl:value-of select="$module/@name"/>
          </td>
        </tr>
      </xsl:if>

      <xsl:for-each select="$module/block">
        <xsl:if test="@name">
          <tr>
            <th align="left" width="35%">
              <xsl:value-of select="@name"/>
            </th>
            <td width="65%">
              <xsl:value-of select="@value"/>
            </td>
          </tr>
        </xsl:if>
        <xsl:call-template name="module_element" />
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="module_block_element">
    <xsl:for-each select="session/module">
      <xsl:call-template name="single_module">
        <xsl:with-param name="module" select="."/>
      </xsl:call-template>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="module_element">
    <xsl:for-each select="element">
      <xsl:variable name="translatedVal" select='translate(@value,",",".")' />
      <tr>
        <td width="35%">
          <xsl:value-of select="@name"/>
        </td>
        <td width="65%">
          <xsl:choose>
            <xsl:when test="floor($translatedVal) = $translatedVal">
              <xsl:value-of select="format-number($translatedVal,'0')"/>
            </xsl:when>
            <xsl:when test="number($translatedVal)">
              <xsl:value-of select="format-number($translatedVal,'#####0.000')"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="@value" />
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </tr>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>