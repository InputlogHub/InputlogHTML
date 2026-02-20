<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Fluency Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
          <style type="text/css">
              .style1 {
                  width: 20%;
              }
          </style>
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Fluency Logging File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
        <br />

        <xsl:for-each select="session/periods">
          | periods of <xsl:value-of select="."/> seconds
        </xsl:for-each>
        <xsl:for-each select="session/intervals">
          | <xsl:value-of select="."/> intervals
        </xsl:for-each>
        <br />
                      
        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
          <tr>
            <th align="left" colspan="5">Statistics</th>
          </tr>
          <tr>
            <td>Average Strokes per Minute</td>
            <td align="left" colspan="4" width="60%">
              <xsl:value-of select="session/module[1]/block[1]/element[1]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation <!--(Intervals)--></td>
            <td align="left" colspan="4" width="60%">
              <xsl:value-of select="session/module[1]/block[1]/element[2]/@value"/>
            </td>
          </tr>
          <!--<tr>
            <td>Standard Deviation (Overall)</td>
            <td align="left" colspan="4" width="60%">
              <xsl:value-of select="session/module[1]/block[1]/element[3]/@value"/>
            </td>
          </tr>-->
          <tr>
            <td>Total Length</td>
            <td align="left" colspan="4" width="60%">
              <xsl:value-of select="session/module[1]/block[1]/element[4]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Total Nr of Strokes</td>
            <td align="left" colspan="4" width="60%">
              <xsl:value-of select="session/module[1]/block[1]/element[5]/@value"/>
            </td>
          </tr>
          <xsl:if test="session/module[1]/block[1]/element[@name='Interval Length']/@value">
            <tr>
              <td>Interval Length</td>
              <td align="left" colspan="4" width="60%">
                <xsl:value-of select="session/module[1]/block[1]/element[@name='Interval Length']/@value"/>
              </td>
            </tr>
          </xsl:if>
        </table>

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
          <tr>
            <th align="left" colspan="5">Absolute Maximum</th>
          </tr>
          <tr>
            <td>Maximum</td>
            <td align="left" colspan="4">
                <xsl:value-of select="session/module[2]/block[1]/element[1]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Coefficient of Variation of % Abs.Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[2]/block[1]/element[2]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of  % Abs. Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[2]/block[1]/element[3]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of % Abs. Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[2]/block[1]/element[4]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation of % Abs. Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[2]/block[1]/element[5]/@value"/>
            </td>
          </tr>

          <tr>
            <xsl:choose>
              <xsl:when test="/session/module[1]/block[1]/element[@name='Interval Length']/@value">
                <th align="left" width="25%">Interval</th>
                <th align="left" width="25%">Total Strokes</th>
                <th align="left" width="25%">Strokes/min</th>
                <th align="left" width="25%">% Maximum</th>
              </xsl:when>
              <xsl:otherwise>
                <th align="left" width="20%">Interval</th>
                <th align="left" width="20%">Total Strokes</th>
                <th align="left" width="20%">Strokes/min</th>
                <th align="left" width="20%">% Maximum</th>
                <th align="left" width="20%">Length</th>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
          <xsl:for-each select="session/module[2]/block[2]/element">
            <xsl:variable name="vPos" select="position()" />
            <tr>
              <td>
                <xsl:value-of select="@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[2]/block[3]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[2]/block[4]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[2]/block[5]/element[$vPos]/@value"/>
              </td>
              <xsl:if test="not(/session/module[1]/block[1]/element[@name='Interval Length']/@value)">
                <td>
                  <xsl:value-of select="/session/module[2]/block[6]/element[$vPos]/@value"/>
                </td>
              </xsl:if>
            </tr>
          </xsl:for-each>
        </table>

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
          <tr>
            <th align="left" colspan="5">Task Maximum</th>
          </tr>
          <tr>
            <td>Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[1]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Period Size</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[2]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Window Size</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[3]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Maximum Location</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[4]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Coefficient of Variation of % Task Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[5]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of % Task Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[6]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of % Task Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[7]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation of % Task Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[8]/@value"/>
            </td>
          </tr>

          <tr>
            <td>Coefficient of Variation of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[9]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[10]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[11]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation  of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[12]/@value"/>
            </td>
          </tr>

          <tr>
            <td>Coefficient of Variation of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[13]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[14]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[15]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[16]/@value"/>
            </td>
          </tr>
          

          <tr>
            <xsl:choose>
              <xsl:when test="/session/module[1]/block[1]/element[@name='Interval Length']/@value">
                <th align="left" width="25%">Interval</th>
                <th align="left" width="25%">Total Strokes</th>
                <th align="left" width="25%">Strokes/min</th>
                <th align="left" width="25%">% Maximum</th>
              </xsl:when>
              <xsl:otherwise>
                <th align="left" width="20%">Interval</th>
                <th align="left" width="20%">Total Strokes</th>
                <th align="left" width="20%">Strokes/min</th>
                <th align="left" width="20%">% Maximum</th>
                <th align="left" width="20%">Length</th>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
          <xsl:for-each select="session/module[3]/block[2]/element">
            <xsl:variable name="vPos" select="position()" />
            <tr>
              <td>
                <xsl:value-of select="@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[3]/block[3]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[3]/block[4]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[3]/block[5]/element[$vPos]/@value"/>
              </td>
              <xsl:if test="not(/session/module[1]/block[1]/element[@name='Interval Length']/@value)">
                <td>
                  <xsl:value-of select="/session/module[3]/block[6]/element[$vPos]/@value"/>
                </td>
              </xsl:if>
            </tr>
          </xsl:for-each>
        </table>

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">
          <tr>
            <th align="left" colspan="5">Personal Maximum</th>
          </tr>
          <tr>
            <td>Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[4]/block[1]/element[1]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Coefficient of Variation of % Pers.Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[4]/block[1]/element[2]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of % Pers.Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[4]/block[1]/element[3]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of % Pers.Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[4]/block[1]/element[4]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation of % Pers.Maximum</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[4]/block[1]/element[5]/@value"/>
            </td>
          </tr>

          <tr>
            <td>Coefficient of Variation of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[9]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[10]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[11]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation  of Strokes/Interval</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[12]/@value"/>
            </td>
          </tr>

          <tr>
            <td>Coefficient of Variation of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[13]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Mean of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[14]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Median of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[15]/@value"/>
            </td>
          </tr>
          <tr>
            <td>Standard Deviation of Strokes/Min.</td>
            <td align="left" colspan="4">
              <xsl:value-of select="session/module[3]/block[1]/element[16]/@value"/>
            </td>
          </tr>
          
          <tr>
            <xsl:choose>
              <xsl:when test="/session/module[1]/block[1]/element[@name='Interval Length']/@value">
                <th align="left" width="25%">Interval</th>
                <th align="left" width="25%">Total Strokes</th>
                <th align="left" width="25%">Strokes/min</th>
                <th align="left" width="25%">% Maximum</th>
              </xsl:when>
              <xsl:otherwise>
                <th align="left" width="20%">Interval</th>
                <th align="left" width="20%">Total Strokes</th>
                <th align="left" width="20%">Strokes/min</th>
                <th align="left" class="style1">% Maximum</th>
                <th align="left" width="20%">Length</th>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
          <xsl:for-each select="session/module[4]/block[2]/element">
            <xsl:variable name="vPos" select="position()" />
            <tr>
              <td>
                <xsl:value-of select="@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[4]/block[3]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[4]/block[4]/element[$vPos]/@value"/>
              </td>
              <td>
                <xsl:value-of select="/session/module[4]/block[5]/element[$vPos]/@value"/>
              </td>
              <xsl:if test="not(/session/module[1]/block[1]/element[@name='Interval Length']/@value)">
                <td>
                  <xsl:value-of select="/session/module[4]/block[6]/element[$vPos]/@value"/>
                </td>
              </xsl:if>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>