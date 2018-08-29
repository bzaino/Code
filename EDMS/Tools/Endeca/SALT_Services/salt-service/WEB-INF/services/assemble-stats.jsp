<%@page import="com.endeca.infront.serialization.XmlSerializer"%>
<%@page import="com.endeca.infront.serialization.JsonSerializer"%>
<%@page
    import="org.springframework.web.context.support.WebApplicationContextUtils"%>
<%@page import="com.endeca.infront.assembler.Assembler"%>
<%@page
    import="com.endeca.infront.assembler.perf.support.StatsPageContentItem"%>
<%@page import="com.endeca.infront.assembler.ContentItem"%>
<%@page import="com.endeca.infront.assembler.AssemblerFactory"%>
<%@page import="org.springframework.web.context.WebApplicationContext"%>
<%@page language="java" contentType="text/html; charset=UTF-8"%>
<%--
*
* Copyright (C) 2011 Endeca Technologies, Inc.
*
* The use of the source code in this file is subject to the ENDECA
* TECHNOLOGIES, INC. SOFTWARE TOOLS LICENSE AGREEMENT. The full text of the
* license agreement can be found in the ENDECA INFORMATION ACCESS PLATFORM
* THIRD-PARTY SOFTWARE USAGE AND LICENSES document included with this software
* distribution.
*
--%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="discover" tagdir="/WEB-INF/tags/discover"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%
    // Construct a content include to query the JCR
    // for content, given the path info of the request
    String url = request.getRequestURI();
    ContentItem contentItem = new StatsPageContentItem(
        url.contains("statsreset"));

    // Get the AssemblerFactory from the spring context and create an Assembler
    WebApplicationContext webappCtx = WebApplicationContextUtils
        .getRequiredWebApplicationContext(application);
    AssemblerFactory assemblerFactory = (AssemblerFactory) webappCtx
        .getBean("assemblerFactory");
			
    Assembler assembler = assemblerFactory.createAssembler();
	
    // Assemble the content
    ContentItem responseContentItem = assembler.assemble(contentItem);

    // If the response contains an error, send an error
    Object error = responseContentItem.get("@error");
    if (error != null) {
        if (((String) error).contains("FileNotFound")) {
            response.sendError(HttpServletResponse.SC_NOT_FOUND,
                (String) error);
        } else {
            response.sendError(HttpServletResponse.SC_INTERNAL_SERVER_ERROR,
                (String) error);
        }
    } else {
        // Determine the output format and write to response
        String format = request.getParameter("format");
        if ("json".equals(format)) {
            response.setHeader("content-type", "application/json");
            new JsonSerializer(response.getWriter())
                .write(responseContentItem);
        } else if ("xml".equals(format)) {
            response.setHeader("content-type", "application/xml");
            new XmlSerializer(response.getWriter())
                .write(responseContentItem);
        } else {
            request.setAttribute("component", responseContentItem);
%>
<c:set var="errorMessage" scope="request" value="${component['@error']}" />
<c:choose>
    <c:when test="${not empty errorMessage}">
        <c:out value="Unable to display component due to error \" ${errorMessage}\"." />
    </c:when>
    <c:otherwise>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en"
    lang="en">
<head>
<script language="javascript"
    src="<c:url value="/js/jquery-1.4.4.min.js"/>"></script>
<script language="javascript"
    src="<c:url value="/js/jquery.cookie.js"/>"></script>
<script language="javascript">
	$j = jQuery.noConflict();
	$j(function() {
		$j("a").click(
				function(event) {
					var id = event.target.id;
					//ignore ids that start with link_ (those are real links)
					var comp = "link_";
					if (id.slice(0, comp.length) == comp) {
						return;
					}
					event.preventDefault();
					var divId = "div_" + id;
					var hideDivId = "hide_div_" + event.target.id;
					if (event.target.innerHTML == "show") {
						event.target.innerHTML = "hide";
						$j("#" + divId).addClass("showBlock").removeClass(
								"hiddenBlock");
						$j("#" + hideDivId).addClass("hiddenBlock")
								.removeClass("showBlock");
					} else {
						event.target.innerHTML = "show";
						$j("#" + divId).addClass("hiddenBlock").removeClass(
								"showBlock");
						$j("#" + hideDivId).addClass("showBlock").removeClass(
								"hiddenBlock");
					}
				});
	});
</script>
<title><c:out value="${component.title}" /></title>
<meta name="keywords" content="${component.metaKeywords}" />
<meta name="description"
    content="${responseContentItem.metaDescription}" />
<link rel="StyleSheet" href="<c:url value="/css/stats_page.css"/>" />
</head>
<body>
    <c:choose>
        <c:when test="${component.reset == 'true'}">
            <b>Reset Successful! </b>
            <a id="link_reset" href="stats">back to stats</a>
            <p />
        </c:when>
        <c:otherwise>
            <jsp:useBean id="dateValue" class="java.util.Date" />
            <div class="StatsPage">
                <table>
                    <tr>
                        <th colspan="2">System Stats</th>
                    </tr>
                    <tr>
                        <td>View As</td>
                        <td><a id="link_xml" href="?format=xml">xml</a>
                            or <a id="link_json" href="?format=json">json</a>
                        </td>
                    </tr>
                    <tr>
                        <th>Stat</th>
                        <th>Value</th>
                    </tr>
                    <tr>
                        <td>Last Reset Time</td>
                        <td><jsp:setProperty name="dateValue"
                                property="time"
                                value="${component.systemStats.startTime}" />
                            <fmt:formatDate value="${dateValue}"
                                pattern="MM dd yyyy HH:mm:ss:SSS z" />
                            <a id="link_reset" href="statsreset">reset</a>
                        </td>
                    </tr>
                    <tr>
                        <td>Number of Requests</td>
                        <td><discover:dispNumber
                                value="${component.systemStats.numRequests}" />
                        </td>
                    </tr>
                    <tr>
                        <td>System Throughput</td>
                        <td><discover:dispNumber
                                value="${component.systemStats.requestsPerSecond}" />
                            req/s</td>
                    </tr>
                    <tr>
                        <td>Average Request Time</td>
                        <td><discover:statsTime
                                value="${component.systemStats.averageRequestTime}" />
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">Stats Legend</th>
                    </tr>
                    <tr>
                        <td>Count</td>
                        <td>The amount of times an event is called
                            within a single request</td>
                    </tr>
                    <tr>
                        <td>Total Time</td>
                        <td>Total time between start and end of
                            event (includes child events)</td>
                    </tr>
                    <tr>
                        <td>Local Time</td>
                        <td>Total Time - time spent in child events
                            (events called between the start and end of
                            the current event)</td>
                    </tr>
                    <tr>
                        <th>Per Request Stats</th>
                        <th>Statistics for each event are collected
                            and aggregated per request: <a
                            id="statsRequest" href="">show</a>
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2" class="innerTable">
                            <div id="hide_div_statsRequest"
                                class="showBlock">...</div>
                            <div id="div_statsRequest"
                                class="hiddenBlock">
                                <c:choose>
                                    <c:when
                                        test="${fn:length(component.perRequestStats) != 0}">
                                        <table class="innerTable">
                                            <tr>
                                                <th>Event</th>
                                                <th>Stat</th>
                                                <th>Total</th>
                                                <th>Min</th>
                                                <th>Max</th>
                                                <th>Average</th>
                                            </tr>
                                            <c:forEach var="prop"
                                                items="${component.perRequestStats}">
                                                <tr>
                                                    <td>${prop['@name']}</td>
                                                    <td>Count</td>
                                                    <td><discover:dispNumber
                                                            value="${prop.countPerRequest.total}" />
                                                    </td>
                                                    <td><discover:dispNumber
                                                            value="${prop.countPerRequest.min}" />
                                                    </td>
                                                    <td><discover:dispNumber
                                                            value="${prop.countPerRequest.max}" />
                                                    </td>
                                                    <td><discover:dispNumber
                                                            value="${prop.countPerRequest.average}" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Used in <b>${prop.n}
                                                            requests</b></td>
                                                    <td>Total Time</td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.total}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.min}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.max}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.average}" />
                                                    </td>
                                                </tr>
                                                <tr class="divider">
                                                    <td />
                                                    <td>Local Time</td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.total}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.min}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.max}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.average}" />
                                                    </td>
                                                </tr>
                                            </c:forEach>
                                        </table>
                                    </c:when>
                                    <c:otherwise>
                                        No Per Request Stats to display...
                                    </c:otherwise>
                                </c:choose>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>Per Event Stats</th>
                        <th>Statistics for each individual event
                            (independent of request): <a id="statsEvent"
                            href="">show</a></th>
                    </tr>
                    <tr>
                        <td colspan="2" class="innerTable">
                            <div id="hide_div_statsEvent"
                                class="showBlock">...</div>
                            <div id="div_statsEvent" class="hiddenBlock">
                                <c:choose>
                                    <c:when
                                        test="${fn:length(component.perEventStats) != 0}">
                                        <a name="statsEvent"></a>
                                        <table class="innerTable">
                                            <tr>
                                                <th>Event</th>
                                                <th>Stat</th>
                                                <th>Total</th>
                                                <th>Min</th>
                                                <th>Max</th>
                                                <th>Average</th>
                                            </tr>
                                            <c:forEach var="prop"
                                                items="${component.perEventStats}">
                                                <tr>
                                                    <td>${prop['@name']}</td>
                                                    <td>Total Time</td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.total}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.min}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.max}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime.average}" />
                                                    </td>
                                                </tr>
                                                <tr class="divider">
                                                    <td>called <b>${prop.n}
                                                            times</b></td>
                                                    <td>Local Time</td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.total}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.min}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.max}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.selfTime.average}" />
                                                    </td>
                                                </tr>
                                            </c:forEach>
                                        </table>
                                    </c:when>
                                    <c:otherwise>No Per Event Stats to display...</c:otherwise>
                                </c:choose>
                            </div></td>
                    </tr>
                    <tr>
                        <th>Worst Queries</th>
                        <th>The 20 (or less) queries which took the
                            most Total Time: <a id="worst" href="">show</a>
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2" class="innerTable">
                            <div id="hide_div_worst" class="showBlock">...</div>
                            <div id="div_worst" class="hiddenBlock">
                                <c:choose>
                                    <c:when
                                        test="${fn:length(component.worstQueries) != 0}">
                                        <table class="innerTable">
                                            <tr>
                                                <th>Time</th>
                                                <th>URL</th>
                                                <th>Total Time</th>
                                                <th>Local Time</th>
                                                <th>Details</th>
                                            </tr>
                                            <c:forEach var="prop"
                                                items="${component.worstQueries}">
                                                <jsp:setProperty
                                                    name="dateValue"
                                                    property="time"
                                                    value="${prop.timestamp}" />
                                                <c:set var="id"
                                                    value="${prop.timestamp}_${prop.totalTime}" />
                                                <tr>
                                                    <td><fmt:formatDate
                                                            value="${dateValue}"
                                                            pattern="MM dd yyyy HH:mm:ss:SSS z" />
                                                    </td>
                                                    <td>${prop.url}</td>
                                                    <td><discover:statsTime
                                                            value="${prop.totalTime}" />
                                                    </td>
                                                    <td><discover:statsTime
                                                            value="${prop.localTime}" />
                                                    </td>
                                                    <td><a
                                                        id="${id}"
                                                        href="">show</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="5"
                                                        class="innerTable">
                                                        <div
                                                            id="hide_div_${id}"
                                                            class="showBlock">...</div>
                                                        <div
                                                            id="div_${id}"
                                                            class="hiddenBlock">
                                                            <table
                                                                class="innerTable">
                                                                <tr>
                                                                    <th>Event</th>
                                                                    <th>Stat</th>
                                                                    <th>Min</th>
                                                                    <th>Max</th>
                                                                    <th>Total</th>
                                                                </tr>
                                                                <c:forEach
                                                                    var="perf"
                                                                    items="${prop.perfSummary}">
                                                                    <tr>
                                                                        <td>${perf['@name']}</td>
                                                                        <td>Total
                                                                            Time</td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.all.min}" />
                                                                        </td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.all.max}" />
                                                                        </td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.all.total}" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr
                                                                        class="divider">
                                                                        <td>Called
                                                                            <b>${perf.count}
                                                                                times</b>
                                                                        </td>
                                                                        <td>Local
                                                                            Time</td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.local.min}" />
                                                                        </td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.local.max}" />
                                                                        </td>
                                                                        <td><discover:statsTime
                                                                                value="${perf.local.total}" />
                                                                        </td>
                                                                    </tr>
                                                                </c:forEach>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </c:forEach>
                                        </table>
                                    </c:when>
                                    <c:otherwise>No Per Event Stats to display...</c:otherwise>
                                </c:choose>
                            </div></td>
                    </tr>
                </table>
            </div>
        </c:otherwise>
    </c:choose>
</body>
        </html>
    </c:otherwise>
</c:choose>
<%
    }
    }
%>
