<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:output method="html" indent="yes" />

  <xsl:template name="statistics_header">
    <thead>
      <colgroup>
        <col align="left" span="1" />
        <col align="right" span="4" />
      </colgroup>

      <tr>
        <th align="left">Window Title</th>
        <th align="right">Total Time (s)</th>
        <th align="right">Total Time (relative)</th>
        <th align="right">Total Keystrokes</th>
        <th align="right">Total Keystrokes (relative)</th>
      </tr>
    </thead>
  </xsl:template>

  <xsl:template name="transition_header">
    <thead>
      <colgroup>
        <col align="left" span="3" />
      </colgroup>

      <tr>
        <th align="left">From Window Title</th>
        <th align="left">To Window Title</th>
        <th align="right">Count</th>
      </tr>
    </thead>
  </xsl:template>
  <xsl:template name="window_statistics_ORIGINAL">
    <xsl:param name="param" />
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
      <xsl:call-template name="statistics_header" />
      <tbody>
        <xsl:for-each select="session/module[1]/block[1]/element">
          <xsl:variable name="vPos" select="position()" />
          <tr>
            <td class="col_text">
              <xsl:value-of select="@name" />
            </td>
            <td align="right">
              <xsl:variable name="totalTime" select="/session/module[1]/block[1]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal1" select='translate($totalTime,",",".")' />
              <xsl:value-of select="$totalTime" />
            </td>
            <td align="right">
              <xsl:variable name="totalTimeRelative" select="/session/module[1]/block[2]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal" select='translate($totalTimeRelative,",",".")' />
              <xsl:value-of select="format-number($translatedVal,'0.###')" />
            </td>
            <td align="right">
              <xsl:value-of select="/session/module[1]/block[3]/element[$vPos]/@value" />
            </td>
            <td align="right">
              <xsl:variable name="totalKeypressesRelative" select="/session/module[1]/block[4]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal2" select='translate($totalKeypressesRelative,",",".")' />
              <xsl:value-of select="format-number($translatedVal2,'0.###')" />
            </td>
          </tr>
        </xsl:for-each>

        <tr class="window_summary">
          <td>Total</td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[1]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'0.###')" />
          </td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[2]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'#.000')" />
          </td>
          <td align="right">
            <xsl:value-of select="/session/module[1]/block[5]/element[3]/@value" />
          </td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[4]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'#.000')" />
          </td>
        </tr>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template name="window_statistics">
    <xsl:param name="param" />
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
      <xsl:call-template name="statistics_header" />
      <tbody>
        <xsl:for-each select="$param/block[1]/element">
          <xsl:variable name="vPos" select="position()" />
          <tr>
            <td class="col_text">
              <xsl:value-of select="@name" />
            </td>
            <td align="right">
              <xsl:variable name="totalTime" select="$param/block[1]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal1" select='translate($totalTime,",",".")' />
              <xsl:value-of select="$totalTime" />
            </td>
            <td align="right">
              <xsl:variable name="totalTimeRelative" select="$param/block[2]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal" select='translate($totalTimeRelative,",",".")' />
              <xsl:value-of select="format-number($translatedVal,'0.###')" />
            </td>
            <td align="right">
              <xsl:value-of select="$param/block[3]/element[$vPos]/@value" />
            </td>
            <td align="right">
              <xsl:variable name="totalKeypressesRelative" select="$param/block[4]/element[$vPos]/@value" />
              <xsl:variable name="translatedVal2" select='translate($totalKeypressesRelative,",",".")' />
              <xsl:value-of select="format-number($translatedVal2,'0.###')" />
            </td>
          </tr>
        </xsl:for-each>

        <tr class="window_summary">
          <td>Total</td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[1]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'0.###')" />
          </td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[2]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'#.000')" />
          </td>
          <td align="right">
            <xsl:value-of select="/session/module[1]/block[5]/element[3]/@value" />
          </td>
          <td align="right">
            <xsl:variable name="translatedVal3"
                          select='translate(/session/module[1]/block[5]/element[4]/@value,",",".")' />
            <xsl:value-of select="format-number($translatedVal3,'#.000')" />
          </td>
        </tr>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template name="transition_statistics">
    <xsl:param name="param" />
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
      <xsl:call-template name="transition_header" />
      <tbody>
        <xsl:for-each select="$param/block[1]/element">
          <xsl:variable name="vPos" select="position()" />
          <tr>
            <td class="col_text">
              <xsl:value-of select="$param/block[1]/element[$vPos]/@value" />
            </td>
            <td class="col_text">
              <xsl:value-of select="$param/block[2]/element[$vPos]/@value" />
            </td>
            <td align="right">
              <xsl:value-of select="$param/block[3]/element[$vPos]/@value" />
            </td>
          </tr>
        </xsl:for-each>
        <tr class="window_summary">
          <td>Total</td>
          <td></td>
          <td align="right">
            <xsl:value-of select="$param/@totalTransitions" />
          </td>
        </tr>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="/session/module[contains(@name, 'Interval') and contains(@name, 'Window Transition Statistics')]">
    <h1>
      <xsl:value-of select="@name"/>
    </h1>
    <xsl:call-template name="transition_statistics">
      <xsl:with-param name="param" select="."/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="/session/module[contains(@name, 'Interval') and contains(@name, 'Window Statistics')]">
    <h1>
      <xsl:value-of select="@name"/>
    </h1>
    <xsl:call-template name="window_statistics">
      <xsl:with-param name="param" select="."/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="/">

    <html>
      <head>
        <title>Source Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
      </head>

      <body>
        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />

        <h1>Source Analysis</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
        <br />

        <h1>Window Statistics</h1>
        <xsl:call-template name="window_statistics">
          <xsl:with-param name="param" select="/session/module[1]"/>
        </xsl:call-template>

        <h1>Window Transition Statistics</h1>
        <xsl:call-template name="transition_statistics">
          <xsl:with-param name="param" select="/session/module[2]"/>
        </xsl:call-template>

        <xsl:if test="/session/module[3]">
          <h1>Interval Statistics</h1>
          <xsl:call-template name="single_module">
            <xsl:with-param name="module" select="/session/module[3]"/>
          </xsl:call-template>
          <xsl:apply-templates select="/session/module[position() > 3]"/>
        </xsl:if>

        <xsl:if test="session/visualRepresentationPath">
          <h1>Visual Representation</h1>
          <img src="{session/visualRepresentationPath}" alt="Visual representation of the Source Analysis" />
        </xsl:if>

      </body>
    </html>

  </xsl:template>
</xsl:stylesheet>