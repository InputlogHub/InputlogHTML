<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
        <title>Summary Analysis</title>
      </head>
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Summary Logging File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />

        <!-- Analysis results -->
        <xsl:call-template name="module_block_element" />

      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>