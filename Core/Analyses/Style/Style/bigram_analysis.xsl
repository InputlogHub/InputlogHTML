<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template name="bigram_info_named">
    <xsl:param name="module" />
      <tr>
        <th align="left" width="11%">Bigram</th>
        <th align="left" width="11%">Count</th>
        <th align="left" width="11%">Mean</th>
        <th align="left" width="11%">Std Dev</th>
        <th align="left" width="11%">Median</th>
        <th align="left" width="11%">Minimum</th>
        <th align="left" width="11%">Maximum</th>
        <th align="left" width="12%">95&#37; Interval Low</th>
        <th align="left" width="12%">95&#37; Interval High</th>
      </tr>
      <xsl:for-each select="session/module[$module]/block[1]/element">
        <xsl:variable name="vPos" select="position()" />
        <tr>
          <td>
            <xsl:value-of select="@name"/>
          </td>
          <td>
            <xsl:value-of select="@value"/>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[2]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[3]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[4]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[5]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[6]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[7]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
          <td>
            <xsl:if test="@value > 0">
              <xsl:value-of select="/session/module[$module]/block[8]/element[$vPos]/@value"/>
            </xsl:if>
          </td>
        </tr>
      </xsl:for-each>
  </xsl:template>
  <xsl:template name="bigram_info_numbered">
    <xsl:param name="module" />
    <tr>
      <th align="left" width="11%">Bigram</th>
      <th align="left" width="11%">Count</th>
      <th align="left" width="11%">Mean</th>
      <th align="left" width="11%">Std Dev</th>
      <th align="left" width="11%">Median</th>
      <th align="left" width="11%">Minimum</th>
      <th align="left" width="11%">Maximum</th>
      <th align="left" width="12%">95&#37; Interval Low</th>
      <th align="left" width="12%">95&#37; Interval High</th>
    </tr>
    <xsl:for-each select="session/module[$module]/block[1]/element">
      <xsl:variable name="vPos" select="position()" />
      <tr>
        <td>
          <xsl:value-of select="$vPos" />.
          <xsl:value-of select="/session/module[$module]/block[1]/element[$vPos]/@value"/>
        </td>
        <td>
          <xsl:value-of select="/session/module[$module]/block[2]/element[$vPos]/@value"/>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[3]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[4]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[5]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[6]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[7]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[8]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
        <td>
          <xsl:if test="/session/module[$module]/block[2]/element[$vPos]/@value > 0">
            <xsl:value-of select="/session/module[$module]/block[9]/element[$vPos]/@value"/>
          </xsl:if>
        </td>
      </tr>
    </xsl:for-each>
  </xsl:template>
  <xsl:template match="/">
    <html>
      <head>
        <title>Bigram Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Bigram Logging File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
        <br />

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
          <tr class="section">
            <th align="left" colspan="9">Bigram Categories</th>
          </tr>
          <xsl:call-template name="bigram_info_named">
            <xsl:with-param name="module" select="2"/>
          </xsl:call-template>
          <tr class="section">
            <th align="left" colspan="9">Speed</th>
          </tr>
          <tr>
            <th align="left" colspan="9">Fastest</th>
          </tr>
          <xsl:call-template name="bigram_info_numbered">
            <xsl:with-param name="module" select="5"/>
          </xsl:call-template>
          <tr>
            <th align="left" width="12%">Slowest</th>
          </tr>
          <xsl:call-template name="bigram_info_numbered">
            <xsl:with-param name="module" select="6"/>
          </xsl:call-template>

        <tr class="section">
          <th align="left" colspan="9">Frequency</th>
        </tr>
        <tr>
        <th align="left" colspan="9">High Frequency</th>
        </tr>
        <xsl:call-template name="bigram_info_numbered">
          <xsl:with-param name="module" select="7"/>
        </xsl:call-template>
        <tr>
          <th align="left" width="12%">Low Frequency</th>
        </tr>
        <xsl:call-template name="bigram_info_numbered">
          <xsl:with-param name="module" select="8"/>
        </xsl:call-template>

          <tr class="section">
            <th align="left" colspan="9">Frequency (doc frequency * language frequency)</th>
          </tr>
          <tr>
            <th align="left" colspan="9">High Frequency</th>
          </tr>
          <xsl:call-template name="bigram_info_numbered">
            <xsl:with-param name="module" select="9"/>
          </xsl:call-template>
          <tr>
            <th align="left">Low Frequency</th>
          </tr>
          <xsl:call-template name="bigram_info_numbered">
            <xsl:with-param name="module" select="10"/>
          </xsl:call-template>
          
          <tr class="section">
            <th align="left" colspan="9">Alphabet Bigrams</th>
          </tr>
          <xsl:call-template name="bigram_info_named">
            <xsl:with-param name="module" select="3"/>
          </xsl:call-template>
          <tr class="section">
            <th align="left" colspan="9">Non-Alpha Bigrams</th>
          </tr>
          <xsl:call-template name="bigram_info_named">
            <xsl:with-param name="module" select="4"/>
          </xsl:call-template>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>