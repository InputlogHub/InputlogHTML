<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:include href="common.xsl" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="Style/common.css" />
				<title>General Eyetrack Analysis</title>
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
				<h1>General Eyetrack Analysis</h1>
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
							<th>X</th>
							<th>Y</th>
							<xsl:if test="session/IncludeRevisions/@Value = 'True'">
								<th>Revision Nr</th>
								<th>Revision Pos</th>
								<th>Revision Type</th>
							</xsl:if>
							<xsl:if test="/session/IncludeEyetracking/@Value = 'True'">
								<!-- All the other eyetracking headers here -->
								<th>[FIX] Event Indices</th>
								<th>[FIX]#GazeEvents</th>
								<th>[FIX]Gaze Event Duration</th>
								<th>[FIX]Avg GzePntX ADCSpx</th>
								<th>[FIX]Avg GzePntY ADCSpx</th>
								<th>[FIX]Avg Pupil Left</th>
								<th>[FIX]Avg Pupil Right</th>
								<th>[FIX]Avg Validity Left</th>
								<th>[FIX]Avg Validity Right</th>
								<th>[FIX]Offscreen Time</th>
								<th>[FIX]Nr Samples</th>
								<th>[FIX]Nr Valid Samples</th>
								<th>[FIX]Min GazePntX ADCSpx</th>
								<th>[FIX]Min GazePntX MCSpx</th>
								<th>[FIX]Min GazePntY ADCSpx</th>
								<th>[FIX]Min GazePntY MCSpx</th>
								<th>[FIX]Max GazePntX ADCSpx</th>
								<th>[FIX]Max GazePntX MCSpx</th>
								<th>[FIX]Max GazePntY ADCSpx</th>
								<th>[FIX]Max GazePntY MCSpx</th>
								<th>[FIX]Max MaxDistX</th>
								<th>[FIX]Min MaxDistX</th>
								<th>[FIX]Avg MaxDistX</th>
								<th>[FIX]Max MaxDistY</th>
								<th>[FIX]Min MaxDistY</th>
								<th>[FIX]Avg MaxDistY</th>
								<th>[FIX]Max DistX</th>
								<th>[FIX]Min DistX</th>
								<th>[FIX]Avg DistX</th>
								<th>[FIX]Max DistY</th>
								<th>[FIX]Min DistY</th>
								<th>[FIX]Avg DistY</th>
								<th>[FIX]Max Start GazePntX ADCSpx</th>
								<th>[FIX]Max Start GazePntY ADCSpx</th>
								<th>[FIX]Min Start GazePntX ADCSpx</th>
								<th>[FIX]Min Start GazePntY ADCSpx</th>
								<th>[FIX]Max End GazePntX ADCSpx</th>
								<th>[FIX]Max End GazePntY ADCSpx</th>
								<th>[FIX]Min End GazePntX ADCSpx</th>
								<th>[FIX]Min End GazePntY ADCSpx</th>
								<th>[FIX]CumAbs DistX</th>
								<th>[FIX]CumAbs DistX Left</th>
								<th>[FIX]CumAbs DistX Right</th>
								<th>[FIX]CumAbs DistY</th>
								<th>[FIX]CumAbs DistY Down</th>
								<th>[FIX]CumAbs DistY Up</th>
								<th>[FIX]Max EyeDist Left</th>
								<th>[FIX]Min EyeDist Left</th>
								<th>[FIX]Max EyeDist Right</th>
								<th>[FIX]Min EyeDist Right</th>
								<th>[FIX]Max EyePos LeftX</th>
								<th>[FIX]Min EyePos LeftX</th>
								<th>[FIX]Max EyePos LeftY</th>
								<th>[FIX]Min EyePos LeftY</th>
								<th>[FIX]Max EyePos LeftZ</th>
								<th>[FIX]Min EyePos LeftZ</th>
								<th>[FIX]Max EyePos RightX</th>
								<th>[FIX]Min EyePos RightX</th>
								<th>[FIX]Max EyePos RightY</th>
								<th>[FIX]Min EyePos RightY</th>
								<th>[FIX]Max EyePos RightZ</th>
								<th>[FIX]Min EyePos RightZ</th>
								<th>[FIX]Mouse Ev</th>
								<th>[FIX]#Mouse Ev</th>
								<th>[FIX]Keyb Ev</th>
								<th>[FIX]#Keyb Ev</th>
								<th>[FIX]Studio Ev</th>
								<th>[FIX]Studio EvVal</th>
								<th>[FIX]Ext Ev</th>
								<th>[FIX]Ext EvVal</th>
								<th>[SAC] Event Indices</th>
								<th>[SAC]#GazeEvents</th>
								<th>[SAC]Gaze Event Duration</th>
								<th>[SAC]Avg GzePntX ADCSpx</th>
								<th>[SAC]Avg GzePntY ADCSpx</th>
								<th>[SAC]Avg Pupil Left</th>
								<th>[SAC]Avg Pupil Right</th>
								<th>[SAC]Avg Validity Left</th>
								<th>[SAC]Avg Validity Right</th>
								<th>[SAC]Offscreen Time</th>
								<th>[SAC]Nr Samples</th>
								<th>[SAC]Nr Valid Samples</th>
								<th>[SAC]Min GazePntX ADCSpx</th>
								<th>[SAC]Min GazePntX MCSpx</th>
								<th>[SAC]Min GazePntY ADCSpx</th>
								<th>[SAC]Min GazePntY MCSpx</th>
								<th>[SAC]Max GazePntX ADCSpx</th>
								<th>[SAC]Max GazePntX MCSpx</th>
								<th>[SAC]Max GazePntY ADCSpx</th>
								<th>[SAC]Max GazePntY MCSpx</th>
								<th>[SAC]Max MaxDistX</th>
								<th>[SAC]Min MaxDistX</th>
								<th>[SAC]Avg MaxDistX</th>
								<th>[SAC]Max MaxDistY</th>
								<th>[SAC]Min MaxDistY</th>
								<th>[SAC]Avg MaxDistY</th>
								<th>[SAC]Max DistX</th>
								<th>[SAC]Min DistX</th>
								<th>[SAC]Avg DistX</th>
								<th>[SAC]Max DistY</th>
								<th>[SAC]Min DistY</th>
								<th>[SAC]Avg DistY</th>
								<th>[SAC]Max Start GazePntX ADCSpx</th>
								<th>[SAC]Max Start GazePntY ADCSpx</th>
								<th>[SAC]Min Start GazePntX ADCSpx</th>
								<th>[SAC]Min Start GazePntY ADCSpx</th>
								<th>[SAC]Max End GazePntX ADCSpx</th>
								<th>[SAC]Max End GazePntY ADCSpx</th>
								<th>[SAC]Min End GazePntX ADCSpx</th>
								<th>[SAC]Min End GazePntY ADCSpx</th>
								<th>[SAC]CumAbs DistX</th>
								<th>[SAC]CumAbs DistX Left</th>
								<th>[SAC]CumAbs DistX Right</th>
								<th>[SAC]CumAbs DistY</th>
								<th>[SAC]CumAbs DistY Down</th>
								<th>[SAC]CumAbs DistY Up</th>
								<th>[SAC]Max EyeDist Left</th>
								<th>[SAC]Min EyeDist Left</th>
								<th>[SAC]Max EyeDist Right</th>
								<th>[SAC]Min EyeDist Right</th>
								<th>[SAC]Max EyePos LeftX</th>
								<th>[SAC]Min EyePos LeftX</th>
								<th>[SAC]Max EyePos LeftY</th>
								<th>[SAC]Min EyePos LeftY</th>
								<th>[SAC]Max EyePos LeftZ</th>
								<th>[SAC]Min EyePos LeftZ</th>
								<th>[SAC]Max EyePos RightX</th>
								<th>[SAC]Min EyePos RightX</th>
								<th>[SAC]Max EyePos RightY</th>
								<th>[SAC]Min EyePos RightY</th>
								<th>[SAC]Max EyePos RightZ</th>
								<th>[SAC]Min EyePos RightZ</th>
								<th>[SAC]Mouse Ev</th>
								<th>[SAC]#Mouse Ev</th>
								<th>[SAC]Keyb Ev</th>
								<th>[SAC]#Keyb Ev</th>
								<th>[SAC]Studio Ev</th>
								<th>[SAC]Studio EvVal</th>
								<th>[SAC]Ext Ev</th>
								<th>[SAC]Ext EvVal</th>
								<th>[UNC]#GazeEvents</th>
								<th>[UNC]Gaze Event Duration</th>
								<th>[UNC]Avg GzePntX ADCSpx</th>
								<th>[UNC]Avg GzePntY ADCSpx</th>
								<th>[UNC]Avg Pupil Left</th>
								<th>[UNC]Avg Pupil Right</th>
								<th>[UNC]Avg Validity Left</th>
								<th>[UNC]Avg Validity Right</th>
								<th>[UNC]Offscreen Time</th>
								<th>[UNC]Nr Samples</th>
								<th>[UNC]Nr Valid Samples</th>
								<th>[UNC]Min GazePntX ADCSpx</th>
								<th>[UNC]Min GazePntX MCSpx</th>
								<th>[UNC]Min GazePntY ADCSpx</th>
								<th>[UNC]Min GazePntY MCSpx</th>
								<th>[UNC]Max GazePntX ADCSpx</th>
								<th>[UNC]Max GazePntX MCSpx</th>
								<th>[UNC]Max GazePntY ADCSpx</th>
								<th>[UNC]Max GazePntY MCSpx</th>
								<th>[UNC]Max MaxDistX</th>
								<th>[UNC]Min MaxDistX</th>
								<th>[UNC]Avg MaxDistX</th>
								<th>[UNC]Max MaxDistY</th>
								<th>[UNC]Min MaxDistY</th>
								<th>[UNC]Avg MaxDistY</th>
								<th>[UNC]Max DistX</th>
								<th>[UNC]Min DistX</th>
								<th>[UNC]Avg DistX</th>
								<th>[UNC]Max DistY</th>
								<th>[UNC]Min DistY</th>
								<th>[UNC]Avg DistY</th>
								<th>[UNC]Max Start GazePntX ADCSpx</th>
								<th>[UNC]Max Start GazePntY ADCSpx</th>
								<th>[UNC]Min Start GazePntX ADCSpx</th>
								<th>[UNC]Min Start GazePntY ADCSpx</th>
								<th>[UNC]Max End GazePntX ADCSpx</th>
								<th>[UNC]Max End GazePntY ADCSpx</th>
								<th>[UNC]Min End GazePntX ADCSpx</th>
								<th>[UNC]Min End GazePntY ADCSpx</th>
								<th>[UNC]CumAbs DistX</th>
								<th>[UNC]CumAbs DistX Left</th>
								<th>[UNC]CumAbs DistX Right</th>
								<th>[UNC]CumAbs DistY</th>
								<th>[UNC]CumAbs DistY Down</th>
								<th>[UNC]CumAbs DistY Up</th>
								<th>[UNC]Max EyeDist Left</th>
								<th>[UNC]Min EyeDist Left</th>
								<th>[UNC]Max EyeDist Right</th>
								<th>[UNC]Min EyeDist Right</th>
								<th>[UNC]Max EyePos LeftX</th>
								<th>[UNC]Min EyePos LeftX</th>
								<th>[UNC]Max EyePos LeftY</th>
								<th>[UNC]Min EyePos LeftY</th>
								<th>[UNC]Max EyePos LeftZ</th>
								<th>[UNC]Min EyePos LeftZ</th>
								<th>[UNC]Max EyePos RightX</th>
								<th>[UNC]Min EyePos RightX</th>
								<th>[UNC]Max EyePos RightY</th>
								<th>[UNC]Min EyePos RightY</th>
								<th>[UNC]Max EyePos RightZ</th>
								<th>[UNC]Min EyePos RightZ</th>
								<th>[UNC]Mouse Ev</th>
								<th>[UNC]#Mouse Ev</th>
								<th>[UNC]Keyb Ev</th>
								<th>[UNC]#Keyb Ev</th>
								<th>[UNC]Studio Ev</th>
								<th>[UNC]Studio EvVal</th>
								<th>[UNC]Ext Ev</th>
								<th>[UNC]Ext EvVal</th> 
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
										<xsl:value-of select="FIX_gazeEventType_nrOf" />
									</td>
									<td align="center">
										<xsl:value-of select="FIX_gazeEventDuration" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averageGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averageGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averagePupilLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averagePupilRight" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averageValidityLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_averageValidityRight" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_offscreenTime" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_nrOfSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_nrOfValidSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_avgMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_avgMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_avgDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_avgDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_maxEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_minEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceX_Left" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceX_Right" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceY_Down" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_cumAbsDistanceY_Up" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_distanceLeft_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_distanceLeft_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_distanceRight_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_distanceRight_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosLeftZ_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_eyePosRightZ_Min" />
									</td>
									<td align="left">
										<xsl:value-of select="FIX_mouseEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_mouseNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_keyboardEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_keyboardNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_studioEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_studioEventValue" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_externalEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="FIX_externalEventValue" />
									</td>
									<td align="center">
										<xsl:value-of select="saccadeIndex" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_gazeEventType_nrOf" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_gazeEventDuration" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averageGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averageGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averagePupilLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averagePupilRight" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averageValidityLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_averageValidityRight" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_offscreenTime" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_nrOfSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_nrOfValidSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_avgMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_avgMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_avgDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_avgDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_maxEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_minEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceX_Left" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceX_Right" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceY_Down" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_cumAbsDistanceY_Up" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_distanceLeft_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_distanceLeft_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_distanceRight_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_distanceRight_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosLeftZ_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_eyePosRightZ_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_mouseEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_mouseNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_keyboardEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_keyboardNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_studioEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_studioEventValue" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_externalEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="SAC_externalEventValue" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_gazeEventType_nrOf" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_gazeEventDuration" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averageGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averageGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averagePupilLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averagePupilRight" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averageValidityLeft" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_averageValidityRight" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_offscreenTime" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_nrOfSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_nrOfValidSamples" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxGazePointX_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxGazePointX_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxGazePointY_ADCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxGazePointY_MCSpx" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_avgMaxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_avgMaxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_avgDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_avgDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minStartGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minStartGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_maxEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minEndGazePointX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_minEndGazePointY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceX" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceX_Left" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceX_Right" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceY" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceY_Down" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_cumAbsDistanceY_Up" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_distanceLeft_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_distanceLeft_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_distanceRight_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_distanceRight_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosLeftZ_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightX_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightX_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightY_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightY_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightZ_Max" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_eyePosRightZ_Min" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_mouseEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_mouseNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_keyboardEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_keyboardNumberOfEvents" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_studioEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_studioEventValue" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_externalEvent" />
									</td>
									<td align="right">
										<xsl:value-of select="UNC_externalEventValue" />
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