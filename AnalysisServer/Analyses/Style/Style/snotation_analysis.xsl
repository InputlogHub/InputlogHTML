<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match= "/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
        <title>S-Notation Analysis</title>
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>S-Notation File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="markup">
          <tr>
            <td class="col_text">
              <xsl:for-each select="session/markup/node()">
                <xsl:choose>
                  <xsl:when test="self::text()">
                    <xsl:value-of select="current()"/>
                  </xsl:when>
                  <xsl:when test="name()='break'">
                    <span class="subscript">
                      <xsl:value-of select="current()"/>
                    </span>
                  </xsl:when>
                  <xsl:otherwise>
                    <span class="superscript">
                      <xsl:value-of select="current()"/>
                    </span>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:for-each>
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
