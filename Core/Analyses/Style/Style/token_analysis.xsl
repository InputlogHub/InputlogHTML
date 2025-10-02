<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />

  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
        <title>Token Analysis</title>
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Token Analysis File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />

        <!-- Write the SNotation string -->
        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="markup">
          <tr>
            <th>
              <h2>Reconstructed Text</h2>
            </th>
          </tr>
          <tr>
            <td class="col_text">
              <xsl:for-each select="session/reconstruct">
                <xsl:value-of select="current()" />
              </xsl:for-each>
            </td>
          </tr>
          <br />

          <tr>
            <th>
              <h2>S-Notation</h2>
            </th>
          </tr>
          <tr>
            <td class="col_text">
              <xsl:for-each select="session/markup/node()">
                <xsl:choose>
                  <xsl:when test="self::text()">
                    <xsl:value-of select="current()" />
                  </xsl:when>
                  <xsl:when test="name()='break'">
                    <span class="subscript">
                      <xsl:value-of select="current()" />
                    </span>
                  </xsl:when>
                  <xsl:otherwise>
                    <span class="superscript">
                      <xsl:value-of select="current()" />
                    </span>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:for-each>
            </td>
          </tr>
        </table>

        <!-- Write the bigram data table -->
        <xsl:call-template name="write_bigrams" />
      </body>
    </html>
  </xsl:template>

  <!-- Template to write the bigrams. -->
  <xsl:template name="write_bigrams">
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">

      <!-- Get column count -->
      <xsl:variable name="bigram_count" select="/session/number_bigrams" />

      <!-- Write table headers-->
      <thead>
        <!-- Standard headers-->
        <th>ID</th>
        <th>User</th>
        <th>Target</th>
        <th>Produced</th>
        <th>S-Notation</th>
        <th>Revisions</th>

        <!-- Variable headers-->
        <xsl:call-template name="write_headers" />
      </thead>

      <!-- Write table data -->
      <xsl:for-each select="/session/bigrams">
        <tr>
          <xsl:call-template name="write_bigram_row" />
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <!-- Write headers template.-->
  <xsl:template name="write_headers">
    <xsl:for-each select="/session/bigrams">
      <xsl:if test="position() = 1">
        <xsl:for-each select="bigram">
          <th>
            Digr_<xsl:value-of select="position()" />
          </th>
          <th>
            Pause_<xsl:value-of select="position()" />
          </th>
        </xsl:for-each>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!--Write bigram rows -->
  <xsl:template name="write_bigram_row">
    <td>
      <xsl:value-of select="position()" />
    </td>
    <td>
      <xsl:value-of select="name" />
    </td>
    <td>
      <xsl:value-of select="target" />
    </td>
    <td>
      <xsl:value-of select="produced" />
    </td>
    <td>
      <xsl:value-of select="s_notation" />
    </td>
    <td>
      <xsl:value-of select="revision" />
    </td>
    <xsl:for-each select="bigram">
      <td>
        <xsl:value-of select="digr" />
      </td>
      <td>
        <xsl:value-of select="pause" />
      </td>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>