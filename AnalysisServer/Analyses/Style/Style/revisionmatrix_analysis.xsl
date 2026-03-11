<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Revision Matrix</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
      </head>
      
      <body>

        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Revision Matrix Notation</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
       
        <table cellspacing="0" cellpadding="5" border="0" class="content">         
          <tr>
            <th>
              <h2>Summary</h2>
            </th>
          </tr>
            <tr align="right">
              <th>Type</th>
              <th>#Revisions</th>
              <th>Edits</th>
              <th>Duration</th>
              <th>Length</th>
              <th>Chars</th>
              <th>Chars without space</th>
              <th>Words</th>
            </tr>
          <xsl:for-each select="session/module[1]/block">            
            <tr align="right">
              <td>          
                <xsl:value-of select="@name"/>               
              </td>
              <xsl:for-each select="element">
                <td>
                  <xsl:value-of select="@value"/>
                </td>
              </xsl:for-each>
            </tr>
          </xsl:for-each>
          <tr align="right">
            <th>R-Bursts</th>
          </tr>
          
            <xsl:for-each select="session/module[2]/block[1]/element">            
            <tr align="right">
              <td>          
                <xsl:value-of select="@name"/>               
              </td>
                <td>
                  <xsl:value-of select="@value"/>
                </td>
            </tr>
          </xsl:for-each>
          
        <tr>
          <th>
            <h2>Revisions</h2>
          </th>
        </tr>
          <tr align="right">
              <th>#Revision</th>
              <th>Type</th>
              <th>Content</th>
              <th>Edits</th>
              <th>Start</th>
              <th>End</th>
              <th>Duration</th>
              <th>BeginPos</th>
              <th>EndPos</th>
              <th>Length</th>
              <th>Chars</th>
              <th>Chars without space</th>
              <th>Words</th>
            </tr>
            <xsl:for-each select="session/revision">
              <tr >
                <!-- #Revision -->
                  <td align="center">
                  <xsl:value-of select="revisionNumber"/>
                </td>
                <!-- Type-->
                <td>
                  <xsl:value-of select="type"/>
                </td>
                <!-- Content -->
                <td class="col_text">
                  <xsl:value-of select="content"/>
                </td>
                <!-- Edits -->
                <td align="right">
                  <xsl:value-of select="edits"/>
                </td>
                <!-- Start -->
                <td align="right">
                  <xsl:value-of select="start"/>
                </td>
                <!-- End -->
                <td align="right">
                  <xsl:value-of select="end"/>
                </td>
                <!-- Duration -->
                <td align="center">
                  <xsl:value-of select="duration"/>
                </td>
                <!-- BeginPos -->
                <td align="right">
                  <xsl:value-of select="beginPos"/>
                </td>
                <!-- EndPos-->
                <td align="right">
                  <xsl:value-of select="endPos"/>
                </td>
                <!-- Length -->
                <td align="right">
                  <xsl:value-of select="length"/>
                </td>
                <!-- Characters -->
                <td align="right">
                  <xsl:value-of select="chars"/>
                </td>
                <!-- Characters without spaces -->
                <td align="right">
                  <xsl:value-of select="charWithoutSpace"/>
                </td>
                <!-- Words -->
                <td align="right">
                  <xsl:value-of select="words"/>
                </td>
              </tr>             
            </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>