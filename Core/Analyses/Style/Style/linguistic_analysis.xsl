<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="common.xsl" />
  <xsl:template match="/">
    <html>
      <head>
        <title>Linguistic Analysis</title>
        <link rel="stylesheet" type="text/css" href="Style/common.css" />
      </head>
      <body>
        <!-- Header and sessionidentification -->
        <xsl:call-template name="header" />
        <h1>Linguistic Analysis File</h1>
        <xsl:call-template name="meta_sessionidentification_parameters" />
        
        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="markup">
          <tbody>
            <tr>
              <th>
                <h2>Final Product(from document)</h2>
              </th>
            </tr>
            <tr>
              <td class="col_text">
                <xsl:for-each select="session/final">
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
            <br />

            <tr>
              <th>
                <h2>W-Notation</h2>
              </th>
            </tr>
            <tr>
              <td class="col_text">
                <xsl:for-each select="session/wnotation">
                  <xsl:value-of select="current()" />
                </xsl:for-each>
              </td>
            </tr>
            <br />

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
                <h2>Inserts in context</h2>
              </th>
            </tr>
            <tr>
              <td class="col_text">
                <xsl:for-each select="session/inserts">
                  <xsl:value-of select="current()" />
                </xsl:for-each>
              </td>
            </tr>
            <br />
            <tr>
              <th>
                <h2>Deletions in context</h2>
              </th>
            </tr>
            <tr>
              <td class="col_text">
                <xsl:for-each select="session/deletions">
                  <xsl:value-of select="current()" />
                </xsl:for-each>
              </td>
            </tr>
          </tbody>
        </table>
        <br />

        <table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">

          <thead>
            <colgroup>
              <col align="center" />
            </colgroup>
            <tr>
              <!-- Data from Inputlog-->
              <th>Revisions</th>
              <th>S-Notation</th>
              <th>#Chars</th>
              <th>Token</th>
              <th>Start WordID</th>
              <th>End WordID</th>
              <th>Start WordTime</th>
              <th>End WordTime</th>
              <th>BfrWord-2</th>
              <th>BfrWord-1</th>
              <th>Btwn Word</th>
              <th>Word Prod</th>
              <th>Within Word</th>
              <th>AftWord+1</th>
              <!-- Data from linguistic server -->
              <th>Token</th>
              <th>PoSA</th>
              <th>PoSB</th>
              <th>PoS-Prob</th>
              <th>Lemma</th>
              <th>Lemma-Prob</th>
              <th>ChunkA</th>
              <th>ChunkB</th>
              <th>NE</th>
              <th>NE-Prob</th>
              <th>LogFreq</th>
              <th>RelFreq</th>
              <th>Syll</th>
            </tr>
          </thead>

          <tbody>
            <xsl:for-each select="session/linguisticProcess/infoTable">
              <tr>
                <!-- 0 Revisions -->
                <td>
                  <xsl:value-of select="Revisions" />
                </td>
                <!-- 1 S-Notation-->
                <td>
                  <xsl:value-of select="S-Notation" />
                </td>
                <!-- 2 Chars Produced-->
                <td>
                  <xsl:value-of select="CharsProduced" />
                </td>
                <!-- 3 Reconstruction -->
                <td>
                  <xsl:value-of select="Token1" />
                </td>

                <!-- 4 StartID -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(StartID) != 0 or position() = 1">
                      <xsl:value-of select="StartID"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 5 EndID -->
                <td align="right">

                  <xsl:choose>
                    <xsl:when test="number(EndID) != 0 or position() = 1">
                      <xsl:value-of select="EndID"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 6 Start Time -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(StartTime) != 0 or position() = 1">
                      <xsl:value-of select="StartTime"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 7 End Time -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(EndTime) != 0 or position() = 1">
                      <xsl:value-of select="EndTime"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 8 Before Word Pause Time - 2 -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(BeforeWord2) != 0 or position() = 1">
                      <xsl:value-of select="BeforeWord2"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 9 Before Word Pause Time -1 -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(BeforeWord1) != 0 or position() = 1">
                      <xsl:value-of select="BeforeWord1"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 10 Between Word Pause Time -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(BetweenPause) != 0 or position() = 1">
                      <xsl:value-of select="BetweenPause"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>              
                <!-- 11 Word Production Time -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(Production) != 0 or position() = 1">
                      <xsl:value-of select="Production"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 12 Word Pause Time -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(WordPause) != 0 or position() = 1">
                      <xsl:value-of select="WordPause"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>
                <!-- 13 Pause Time after word -->
                <td align="right">
                  <xsl:choose>
                    <xsl:when test="number(AfterWordPause) != 0 or position() = 1">
                      <xsl:value-of select="AfterWordPause"/>
                    </xsl:when>
                    <xsl:otherwise>-</xsl:otherwise>
                  </xsl:choose>
                </td>

                <!-- 14 Linguistic data -->
                <!-- Token -->
                <td>
                  <xsl:value-of select="Token2" />
                </td>
                <!-- 15 Part of Speech A-->
                <td>
                  <xsl:value-of select="PoSA" />
                </td>
                <!-- 16 Part of Speech B-->
                <td>
                  <xsl:value-of select="PoSB" />
                </td>
                <!-- 17 Part of Speech Probability -->
                <td>
                  <xsl:value-of select= 'format-number(PoS-Prob, "0.00" )'/>
                </td>
                <!-- 18 Lemmatizer -->
                <td>
                  <xsl:value-of select="Lemma" />
                </td>
                <!-- 19 Lemmatizer Probability -->
                <td>
                  <xsl:value-of select='format-number(Lemma-Prob, "0.00" )' />
                </td>
                <!-- 20 Chunks A-->
                <td>
                  <xsl:value-of select="ChunkA" />
                </td>
                <!-- 21 Chunks B -->
                <td>
                  <xsl:value-of select="ChunkB" />
                </td>
                <!-- 22 Named Entity -->
                <td>
                  <xsl:value-of select="NE" />
                </td>
                <!-- 23 Named Entity Probability -->
                <td>
                  <xsl:value-of select= 'format-number(NE-Prob, "0.00" )' />
                </td>
                <!-- 24 The Log of the Word Frequency -->
                <td align="right">
                  <xsl:value-of select='format-number(LogFreq,"0" )' />
                </td>
                <!-- 25 Relative Word Frequency -->
                <td align="right">
                  <xsl:value-of select='format-number(RelFreq, "0.000" )' />
                </td>
                <!-- 26 Syllables -->
                <td>
                  <xsl:value-of select="Syllable" />
                </td>

              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>