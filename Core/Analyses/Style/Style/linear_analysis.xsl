<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Linear Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Linear Logging File</h1>
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
            <th align="left" width="20%">Interval</th>
            <th align="left" width="80%">Output</th>
          </tr>
          <xsl:for-each select="session/Period">
            <tr>
              <td valign="top">
                <A>
                  <xsl:attribute name="name">
                    <xsl:value-of select="@periodTime"/>
                  </xsl:attribute>
                  <xsl:attribute name="href">
                    <xsl:value-of select="@link"/>
                  </xsl:attribute>
                  <!-- If-else because the changes in the LinearAnalysisFile changed the startTime element 
                  from name to the period_id element. So this if-else checks to see if the analysis xml is 
                  still using the old tag name or the new one and selects accordingly.
                  -->
                  <!--<xsl:value-of select="@startTime"/> -->
                  <xsl:choose>
                    <xsl:when test="@startTime"><xsl:value-of select="@startTime"/></xsl:when>
                    <xsl:otherwise><xsl:value-of select="@period_id"/></xsl:otherwise>
                  </xsl:choose>
                </A>
              </td>
              <td>
                <xsl:for-each select="PeriodEvent">
                  <xsl:choose>
                    <xsl:when test="starts-with(@value,'[')">
                      <span style="color:#888">
                        <xsl:value-of select="@value"/>
                      </span>
                    </xsl:when>
                    <xsl:when test="starts-with(@value,'{')">
                      <span style="color:#44F">
                        <xsl:value-of select="@value"/>
                      </span>
                    </xsl:when>
                    <xsl:otherwise>
                      <span style="color:inherit">
                        <xsl:value-of select="@value"/>
                      </span>
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:for-each>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>