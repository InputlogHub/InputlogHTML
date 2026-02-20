<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>Copytask Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css"/>
      </head>
      <body>
        <xsl:call-template name="header" />
        <h1>Copytask Analysis</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
        <br />

        <!-- Transform each module to a table -->
        <xsl:apply-templates />
      </body>
    </html>
  </xsl:template>

  <xsl:template match="group_title">
    <h1>
      <xsl:value-of select="text()"/>
    </h1>
  </xsl:template>

  <!--Transforms modules to tables-->
  <xsl:template match="module">
    <h2>
      <xsl:value-of select="@name"/>
    </h2>
    <table cellspacing="0" cellpadding="5" border="1" width="100%" class="content">
      <xsl:apply-templates select="block" />
    </table>
  </xsl:template>

  <!--Transform a block to a table row. If the block is the header block, make it
  a table header instead-->
  <xsl:template match="block[@name='header']">
    <tr style="text-align:right">
      <xsl:for-each select="element">
        <xsl:choose>
          <!--If the name is group, the first column, we make it wider-->
          <xsl:when test="@name='group'">
            <th width="20%" >
              <xsl:value-of select="@value"/>
            </th>
          </xsl:when>

          <!--If the name is not group we make the width smaller-->
          <xsl:when test="@name!='group'">
            <th width="8%" style="text-align:right">
              <xsl:value-of select="@value"/>
            </th>
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
    </tr>
  </xsl:template>

  <!--Transform a block to a table row. Headers are treated differently-->
  <xsl:template match="block[not(@name='header')]">
    <tr>
      <xsl:for-each select="element">
        <xsl:choose>
          <xsl:when test="@name='value'">
            <td style="text-align:left">
              <span style="padding-left:5%">
                <xsl:value-of select="@value"/>
              </span>
            </td>
          </xsl:when>
          <xsl:when test="@name!='value'">
            <td style="text-align:right">
              <xsl:value-of select="@value"/>
            </td>
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
    </tr>
  </xsl:template>
</xsl:stylesheet>