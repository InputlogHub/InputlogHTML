<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:include href="common.xsl" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="Style/common.css" />
				<title>General Analysis</title>
				<style>
					.windowTitle div{
					border: 1px solid #E9EEF3;
					padding:2px 5px;
					margin-left: 5px;
					background-color: #ffffdd;
					position:absolute;
					display:none;
					}
					.pauseLocation {
					display:none;
					}
					.positionFull {
					display:none;
					}
					.doclengthFull {
					display:none;
					}
				</style>

				<script src="Scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
				<script>
					$(document).ready(function(){
					// window title
					$(".windowTitle a").hover(function(){
					var div = $(this).next("div");
					if (div.is(":hidden")){
					div.show();
					} else{
					div.hide();
					}
					});

					// pause location displayed value
					var currentLocationDisplay = "full";
					$(".pauseLocationDisplay").click(function(e){
					if (currentLocationDisplay == "full"){
					$(".pauseLocationFull").hide();
					$(".pauseLocation").show();
					currentLocationDisplay = "number";
					} else {
					$(".pauseLocation").hide();
					$(".pauseLocationFull").show();
					currentLocationDisplay = "full";
					}
					e.preventDefault();
					});

					// position displayed value
					var currentPositionDisplay = "normal";
					$(".positionDisplay").click(function(e){
					if (currentPositionDisplay == "full"){
					$(".positionFull").hide();
					$(".position").show();
					currentPositionDisplay = "normal";
					} else {
					$(".position").hide();
					$(".positionFull").show();
					currentPositionDisplay = "full";
					}
					e.preventDefault();
					});

					// document length displayed value
					var currentLengthDisplay = "normal";
					$(".doclengthDisplay").click(function(e){
					if (currentLengthDisplay == "full"){
					$(".doclengthFull").hide();
					$(".doclength").show();
					currentLengthDisplay = "normal";
					} else {
					$(".doclength").hide();
					$(".doclengthFull").show();
					currentLengthDisplay = "full";
					}
					e.preventDefault();
					});

					});
				</script>
			</head>
			<body>

				<!-- Header and sessionidentification -->
				<xsl:call-template name="header" />
				<h1>General Analysis</h1>
				<xsl:call-template name="meta_sessionidentification_parameters" />
				<br/>


				<table cellspacing="0" cellpadding="5" border="0" width="100%" class="content">

					<thead>
						<colgroup>
							<col align="center" span="10" />
							<col align="center" />
							<col align="center" span="2" />
						</colgroup>

						<tr>
							<th>#Id</th>
							<th>Event Type</th>
							<th>Output</th>
							<th class="positionFull">
								<a href="#" class="positionDisplay" >Position Full</a>
							</th>
							<th class="position">
								<a href="#" class="positionDisplay" >Position</a>
							</th>
							<th class="doclengthFull">
								<a href="#" class="doclengthDisplay" >DocLength Full</a>
							</th>
							<th class="doclength">
								<a href="#" class="doclengthDisplay" >DocLength</a>
							</th>
							<th>Character Production</th>
							<th>StartTime</th>
							<th>StartClock</th>
							<th>EndTime</th>
							<th>EndClock</th>
							<th>ActionTime</th>
							<th>PauseTime</th>
							<th class="pauseLocationFull">
								<a href="#" class="pauseLocationDisplay" >PauseLocation</a>
							</th>
							<th class="pauseLocation">
								<a href="#" class="pauseLocationDisplay" >PauseLocation</a>
							</th>
							<!-- Deprecated
              <th>Rangelength</th>
              -->
              <th>IntervalFixedSize</th>
              <th>IntervalFixedNumber</th>
              
							<th>X</th>
							<th>Y</th>
              
							<xsl:if test="session/IncludeRevisions/@Value = 'True'">
								<th>Revision Nr</th>
								<th>Revision Pos</th>
								<th>Revision Type</th>
							</xsl:if>
							<xsl:if test="/session/IncludeEyetracking/@Value = 'True'">
								<!-- All the other eyetracking headers here -->
								<th>Fixation Index</th>
								<th>Saccade Index</th>
								<th>Gaze Type</th>
								<th>Gaze Event Duration</th>
								<th>Avg GzePntX ADCSpx</th>
								<th>Avg GzePntY ADCSpx</th>
								<th>Avg Pupil Left</th>
								<th>Avg Pupil Right</th>
								<th>Avg Validity Left</th>
								<th>Avg Validity Right</th>
								<th>Offscreen Time</th>
								<th>Nr Samples</th>
								<th>Nr Valid Samples</th>
								<th>Min GazePntX ADCSpx</th>
								<th>Min GazePntX MCSpx</th>
								<th>Min GazePntY ADCSpx</th>
								<th>Min GazePntY MCSpx</th>
								<th>Max GazePntX ADCSpx</th>
								<th>Max GazePntX MCSpx</th>
								<th>Max GazePntY ADCSpx</th>
								<th>Max GazePntY MCSpx</th>
								<th>Max DistX</th>
								<th>Max DistY</th>
								<th>DistX</th>
								<th>DistY</th>
								<th>Start GazePntX ADCSpx</th>
								<th>Start GazePntY ADCSpx</th>
								<th>End GazePntX ADCSpx</th>
								<th>End GazePntY ADCSpx</th>
								<th>CumAbs DistX</th>
								<th>CumAbs DistX Left</th>
								<th>CumAbs DistX Right</th>
								<th>CumAbs DistY</th>
								<th>CumAbs DistY Down</th>
								<th>CumAbs DistY Up</th>
								<th>Max EyeDist Left</th>
								<th>Min EyeDist Left</th>
								<th>Max EyeDist Right</th>
								<th>Min EyeDist Right</th>
								<th>Max EyePos LeftX</th>
								<th>Min EyePos LeftX</th>
								<th>Max EyePos LeftY</th>
								<th>Min EyePos LeftY</th>
								<th>Max EyePos LeftZ</th>
								<th>Min EyePos LeftZ</th>
								<th>Max EyePos RightX</th>
								<th>Min EyePos RightX</th>
								<th>Max EyePos RightY</th>
								<th>Min EyePos RightY</th>
								<th>Max EyePos RightZ</th>
								<th>Min EyePos RightZ</th>
								<th>EyeTr StartTime</th>
								<th>EyeTr EndTime</th>
								<th>EyeTr Duration</th>
								<th>EyeTr StartTstmp</th>
								<th>Media Name</th>
								<th>Media PosX ADCSpx</th>
								<th>Media PosY ADCSpx</th>
								<th>Media H</th>
								<th>Media W</th>
								<th>Sgm Name</th>
								<th>Sgm Start</th>
								<th>Sgm End</th>
								<th>Sgm Dur</th>
								<th>Scn Name</th>
								<th>Scn Start</th>
								<th>Scn End</th>
								<th>Scn Dur</th>
								<th>Mouse Ev</th>
								<th>#Mouse Ev</th>
								<th>Keyb Ev</th>
								<th>#Keyb Ev</th>
								<th>Studio Ev</th>
								<th>Studio EvVal</th>
								<th>Ext Ev</th>
								<th>Ext EvVal</th>
								<!-- The aoi table headers -->
								<xsl:if test="/session/IncludeAOIs/@Value = 'True'">
									<xsl:for-each select="session/aoiNames/aoiName">
										<th>
											<xsl:value-of select="." />
										</th>
									</xsl:for-each>
								</xsl:if>
							</xsl:if>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select="session/event">
							<tr>
								<!-- ID -->
								<td>
									<xsl:value-of select="id"/>
								</td>
								<!-- Type -->
								<td>
									<xsl:value-of select="type"/>
								</td>

                <!-- Ouput -->
                <xsl:choose>
                  <xsl:when test="Type = 'focus'">
                    <td class="windowTitle" nowrap="nowrap">
                      <a href="#">[show window title]</a>
                      <div>
                        <xsl:value-of select="output"/>
                      </div>
                      <noscript>
                        <xsl:value-of select="output"/>
                      </noscript>
                    </td>
                  </xsl:when>
                  <xsl:otherwise>
                    <td class="windowTitle">
                      <xsl:if test="resource">
                        <a>
                          <xsl:attribute name="href">
                            <xsl:value-of select="concat('file://', resource)" />
                          </xsl:attribute>
                          <xsl:value-of select="output"/>
                        </a>
                      </xsl:if>
                      <xsl:if test="not(resource)">
                        <xsl:value-of select="output"/>
                      </xsl:if>
                    </td>
                  </xsl:otherwise>
                </xsl:choose>

								<!-- Position -->
								<td class="position" align="right">
									<xsl:value-of select="position"/>
								</td>
								<td class="positionFull" align="right">
									<xsl:value-of select="positionFull"/>
								</td>

								<!-- DocLength -->
								<td class="doclength" align="right">
									<xsl:value-of select="doclength"/>
								</td>
								<td class="doclengthFull" align="right">
									<xsl:value-of select="doclengthFull"/>
								</td>

								<!-- Character Production -->
								<td align="right">
									<xsl:value-of select="charProduction"/>
								</td>

								<!-- StartTime -->
								<td align="right">
									<xsl:value-of select="startTime"/>
								</td>

								<!-- StartClock -->
								<td align="center">
									<xsl:value-of select="substring-before(startClock,'.')"/>
								</td>

								<!-- EndTime -->
								<td align="right">
									<xsl:value-of select="endTime"/>
								</td>

								<!-- EndClock -->
								<td align="center">
									<xsl:value-of select="substring-before(endClock,'.')"/>
								</td>

								<!-- ActionTime -->
								<td align="right">
									<xsl:value-of select="actionTime"/>
								</td>

								<!-- PauseTime -->
								<td align="right">
									<xsl:value-of select="pauseTime"/>
								</td>

								<!-- Pause Location -->
								<td class="pauseLocation" align="center">
									<xsl:value-of select="pauseLocation"/>
								</td>
								<td class="pauseLocationFull" align="center" nowrap="nowrap">
									<xsl:value-of select="pauseLocationFull"/>
								</td>

								<!-- Range Length - Deprecated
                <td align="right">
                  <xsl:value-of select="rangeLength"/>
                </td>
                -->
                
								<!-- Intervals -->
                <td align="right">
                  <xsl:value-of select="intervalSize"/>
                </td>
                 <td align="right">
                  <xsl:value-of select="intervalNumber"/>
                </td>
                
								<!-- Mouse Position Y -->
								<td align="center">
									<xsl:value-of select="x"/>
								</td>

								<!-- Mouse Position X -->
								<td align="center">
									<xsl:value-of select="y"/>
								</td>
                  
								<xsl:if test="/session/IncludeRevisions/@Value = 'True'">
									<!-- Revision Info -->
									<td align="center">
										<xsl:value-of select="RevisionInfo/RevisionNumber" />
									</td>
									<td align="center">
										<xsl:value-of select="RevisionInfo/RevisionPos" />
									</td>
									<td align="center">
										<xsl:value-of select="RevisionInfo/RevisionType" />
									</td>
								</xsl:if>

								<xsl:if test="/session/IncludeEyetracking/@Value = 'True'">
									<!-- All the other eyetracking headers here -->
									<td align="center">
										<xsl:value-of select="fixationIndex" />
									</td>
									<td align="center">
										<xsl:value-of select="saccadeIndex" />
									</td>
									<td align="center">
										<xsl:value-of select="gazeEventType" />
									</td>
									<td align="center">
										<xsl:value-of select="gazeEventDuration" />
									</td>
									<td align="center">
										<xsl:value-of select="averageGazePointX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="averageGazePointY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="averagePupilLeft" />
									</td>
									<td align="center">
										<xsl:value-of select="averagePupilRight" />
									</td>
									<td align="center">
										<xsl:value-of select="averageValidityLeft" />
									</td>
									<td align="center">
										<xsl:value-of select="averageValidityRight" />
									</td>
									<td align="center">
										<xsl:value-of select="offscreenTime" />
									</td>
									<td align="center">
										<xsl:value-of select="nrOfSamples" />
									</td>
									<td align="center">
										<xsl:value-of select="nrOfValidSamples" />
									</td>
									<td align="center">
										<xsl:value-of select="minGazePointX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="minGazePointX_MCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="minGazePointY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="minGazePointY_MCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="maxGazePointX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="maxGazePointX_MCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="maxGazePointY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="maxGazePointY_MCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="maxDistanceX" />
									</td>
									<td align="center">
										<xsl:value-of select="maxDistanceY" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceX" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceY" />
									</td>
									<td align="center">
										<xsl:value-of select="startGazePointX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="startGazePointY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="endGazePointX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="endGazePointY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceX" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceX_Left" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceX_Right" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceY" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceY_Down" />
									</td>
									<td align="center">
										<xsl:value-of select="cumAbsDistanceY_Up" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceLeft_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceLeft_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceRight_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="distanceRight_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftX_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftX_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftY_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftY_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftZ_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosLeftZ_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightX_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightX_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightY_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightY_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightZ_Max" />
									</td>
									<td align="center">
										<xsl:value-of select="eyePosRightZ_Min" />
									</td>
									<td align="center">
										<xsl:value-of select="et_startTime" />
									</td>
									<td align="center">
										<xsl:value-of select="et_endTime" />
									</td>
									<td align="center">
										<xsl:value-of select="et_duration" />
									</td>
									<td align="center">
										<xsl:value-of select="et_fullTimestamp" />
									</td>
									<td align="center">
										<xsl:value-of select="mediaName" />
									</td>
									<td align="center">
										<xsl:value-of select="mediaPosX_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="mediaPosY_ADCSpx" />
									</td>
									<td align="center">
										<xsl:value-of select="mediaHeight" />
									</td>
									<td align="center">
										<xsl:value-of select="mediaWidth" />
									</td>
									<td align="center">
										<xsl:value-of select="segmentName" />
									</td>
									<td align="center">
										<xsl:value-of select="segmentStart" />
									</td>
									<td align="center">
										<xsl:value-of select="segmentEnd" />
									</td>
									<td align="center">
										<xsl:value-of select="segmentDuration" />
									</td>
									<td align="center">
										<xsl:value-of select="sceneName" />
									</td>
									<td align="center">
										<xsl:value-of select="sceneStart" />
									</td>
									<td align="center">
										<xsl:value-of select="sceneEnd" />
									</td>
									<td align="center">
										<xsl:value-of select="sceneDuration" />
									</td>
									<td align="center">
										<xsl:value-of select="mouseEvents" />
									</td>
									<td align="center">
										<xsl:value-of select="mouseNumberOfEvents" />
									</td>
									<td align="center">
										<xsl:value-of select="keyboardEvents" />
									</td>
									<td align="center">
										<xsl:value-of select="keyboardNumberOfEvents" />
									</td>
									<td align="center">
										<xsl:value-of select="studioEvent" />
									</td>
									<td align="center">
										<xsl:value-of select="studioEventValue" />
									</td>
									<td align="center">
										<xsl:value-of select="externalEvent" />
									</td>
									<td align="center">
										<xsl:value-of select="externalEventValue" />
									</td>
									<!-- The aoi table headers -->
									<xsl:if test="/session/IncludeAOIs/@Value = 'True'">
										<xsl:for-each select="aoiHits/aoi">
											<td align="center">
												<xsl:value-of select="." />
											</td>
										</xsl:for-each>
									</xsl:if>
								</xsl:if>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>