<%--
  ~ Copyright 2001, 2012, Oracle and/or its affiliates. All rights reserved.
  ~ Oracle and Java are registered trademarks of Oracle and/or its
  ~ affiliates. Other names may be trademarks of their respective owners.
  ~ UNIX is a registered trademark of The Open Group.
  ~
  ~ This software and related documentation are provided under a license
  ~ agreement containing restrictions on use and disclosure and are
  ~ protected by intellectual property laws. Except as expressly permitted
  ~ in your license agreement or allowed by law, you may not use, copy,
  ~ reproduce, translate, broadcast, modify, license, transmit, distribute,
  ~ exhibit, perform, publish, or display any part, in any form, or by any
  ~ means. Reverse engineering, disassembly, or decompilation of this
  ~ software, unless required by law for interoperability, is prohibited.
  ~ The information contained herein is subject to change without notice
  ~ and is not warranted to be error-free. If you find any errors, please
  ~ report them to us in writing.
  ~ U.S. GOVERNMENT END USERS: Oracle programs, including any operating
  ~ system, integrated software, any programs installed on the hardware,
  ~ and/or documentation, delivered to U.S. Government end users are
  ~ "commercial computer software" pursuant to the applicable Federal
  ~ Acquisition Regulation and agency-specific supplemental regulations.
  ~ As such, use, duplication, disclosure, modification, and adaptation
  ~ of the programs, including any operating system, integrated software,
  ~ any programs installed on the hardware, and/or documentation, shall be
  ~ subject to license terms and license restrictions applicable to the
  ~ programs. No other rights are granted to the U.S. Government.
  ~ This software or hardware is developed for general use in a variety
  ~ of information management applications. It is not developed or
  ~ intended for use in any inherently dangerous applications, including
  ~ applications that may create a risk of personal injury. If you use
  ~ this software or hardware in dangerous applications, then you shall
  ~ be responsible to take all appropriate fail-safe, backup, redundancy,
  ~ and other measures to ensure its safe use. Oracle Corporation and its
  ~ affiliates disclaim any liability for any damages caused by use of this
  ~ software or hardware in dangerous applications.
  ~ This software or hardware and documentation may provide access to or
  ~ information on content, products, and services from third parties.
  ~ Oracle Corporation and its affiliates are not responsible for and
  ~ expressly disclaim all warranties of any kind with respect to
  ~ third-party content, products, and services. Oracle Corporation and
  ~ its affiliates will not be responsible for any loss, costs, or damages
  ~ incurred due to your access to or use of third-party content, products,
  ~ or services.
  --%>
<%@page language="java" pageEncoding="UTF-8" contentType="text/html;charset=UTF-8"%>
<%@page import="java.util.Enumeration" %>
<%@page import="java.util.ResourceBundle" %>
<%@page import="org.apache.sling.commons.json.JSONObject" %>

<%@include file="/WEB-INF/views/include.jsp"%>
<fmt:setBundle basename="EndecaBrowser" scope="request"/>
<fmt:setLocale value="${pageContext.request.locale}" scope="request" />

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<%
    String contextPath = request.getContextPath();

    // TODO: When we move the EndecaBrowser inside Sling, we can use the
    // tag handler that we use to do this everywhere else.  For now, just
    // iterate over the resources and construct the JSON here.
    ResourceBundle resources = ResourceBundle.getBundle("EndecaBrowser");
    JSONObject resourceBundle = new JSONObject();
    Enumeration<String> keys = resources.getKeys();

    // Add all the resources to the JSON object as properties...
    while (keys.hasMoreElements()) {
        String key = keys.nextElement();
        resourceBundle.put(key, resources.getString(key));
    }
%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=8,9" />
    <title><c:out value="${component.title}"/></title>
    <link rel="stylesheet" type="text/css" href="<c:url value="/css/jquery-ui-1.8.16.custom.css"/>" />
    <link rel="stylesheet" type="text/css" href="<c:url value="/css/app.css"/>" />
    <link rel="stylesheet" type="text/css" href="<c:url value="/css/endeca_browser.css"/>" />
    <link rel="stylesheet" type="text/css" href="<c:url value="/js/static/extjs/resources/css/ext-all-gray.css"/>" />
    <link rel="stylesheet" type="text/css" href="<c:url value="/js/static/endeca/css/endeca.css"/>" />
    <link rel="stylesheet" type="text/css" href="/stylesheets/main.css">
    <script type="text/javascript" src="<c:url value="/js/jquery-1.4.4.min.js"/>"></script>
    <script type="text/javascript"> $j = jQuery.noConflict(); </script>
    <script type="text/javascript" src="<c:url value="/js/static/extjs/ext-all.js"/>"></script>
    <script type="text/javascript" src="<c:url value="/js/application.js"/>"></script>
    <script type="text/javascript" >
        // Configure the Ext loader
        Ext.Loader.setConfig({
            enabled: true,
            disableCaching: false,
            paths: {
                'Ext' : '<%= contextPath %>/js/static/extjs/ext',
                'Eb'  : '<%= contextPath %>/js/app',
                'Endeca': '<%= contextPath %>/js/static/endeca/javascript'
            }
        });
        Ext.ns("Eb");
        Eb.Globals = {
            contextPath : '<%= contextPath %>',
            resourceBundle : Ext.create('Endeca.util.ResourceBundle',
                <%= resourceBundle %>
            )
        };
        EndecaBrowserState.setSpecAndRollupKey('<c:out value="${param.spec}" />', '<c:out value="${param.rollupKey}" />');
    </script>
    <script type="text/javascript" src="<c:url value="/js/app.js"/>"></script>
</head>
<body>
<c:if test="${not empty fn:trim(param.selectedRecords)}">
    <div>
        <c:set var="recordsInQuery" value="${fn:split(fn:trim(param.selectedRecords), ',')}" />
        <c:set var="properties" value="${param.properties}"/>
        <!--
        Add all records that are in the query string to the array
        so we don't lose records that are no longer in the mdex
        -->
        <script type="text/javascript" >
            var rec, record;

            <c:forEach var="record" items="${recordsInQuery}" >
            rec = EndecaBrowserState.getRecord('<c:out value="${record}" />');
            if(rec === null){
                record = {
                    <c:forEach items="${fn:split(properties,':')}" var="prop" varStatus="propStatus">
                    'field${propStatus.index}': '<fmt:message key="no_data_available"/>',
                    </c:forEach>
                    '__Endeca_Browser_id' : '<c:out value="${util:escapeJavaScript(record)}" />',
                    '__Endeca_Browser_spec': '<c:out value="${util:escapeJavaScript(record)}" />',
                    '__Endeca_Browser_rollupVal': '',
                    '__Endeca_Browser_thumbnail': '',
                    '__Endeca_Browser_index': EndecaBrowserState.getRecords().length + 1,
                    '__Endeca_Browser_valid': false
                };
                EndecaBrowserState.addRecord(record);
            }
            </c:forEach>
        </script>
    </div>
</c:if>
<!-- x-hide-display is used to prevent flickering when html is rendered in the ExtJS viewport... -->
<div class="x-hide-display" id="EntirePageContent">
    <div class="PageHeader">
        <div id="PageTypeMenu">
            <h4 id="pageSelectorLabel">
                <fmt:message key="${component.headerMessageKey}"/>
            </h4>

            <c:if test="${component.toggleableRecordSelection}">
                <div class="pageTypeSelection">
                    <input type="radio" name="page" id="navSelector"
                            <c:choose>
                                <c:when test="${empty param.page or param.page eq 'nav'}">
                                    checked=true
                                </c:when>
                                <c:otherwise>
                                    onclick="location.search = queryStringWithoutParam(location.search.substr(1), ['selectedRecords']).
                                                                replace('page=record', this.value);"
                                </c:otherwise>
                            </c:choose>
                           value='page=nav' />
                    <label for="navSelector" class="selectorLabel"><fmt:message key="navigation_state_selection"/></label>

                    <input type="radio" name="page" id="recordSelector"
                            <c:choose>
                                <c:when test="${param.page eq 'record' }">
                                    checked=true
                                </c:when>
                                <c:otherwise>
                                    onclick="location.search = queryStringWithoutParam(location.search.substr(1), ['selectedRecords']).
                                                                replace('page=nav', this.value);"
                                </c:otherwise>
                            </c:choose>
                           value='page=record' />
                    <label for="recordSelector" class="selectorLabel"><fmt:message key="record_selection"/></label>
                </div>
            </c:if>
        </div>
        <div id="SelectedRecords">
            <c:forEach var="element" items="${component.spotlight}">
                <discover:include component="${element}"/>
            </c:forEach>
        </div>
    </div>

    <div class="MainContent" id="MainContent">
        <div class="LeftColumn">
            <div>
                <c:forEach var="element" items="${component.filteredState}">
                    <discover:include component="${element}"/>
                </c:forEach>
            </div>
            <div>
                <c:forEach var="element" items="${component.leftColumn}">
                    <discover:include component="${element}"/>
                </c:forEach>
            </div>
        </div>

        <div class="MainColumn">
            <div>
                <c:forEach var="element" items="${component.header}">
                    <discover:include component="${element}"/>
                </c:forEach>
            </div>
            <div>
                <c:forEach var="element" items="${component.main}">
                    <c:set var="errorMessage" value="${element['@error']}"/>
                    <c:choose>
                        <c:when test="${!empty(errorMessage) && fn:contains(errorMessage, 'com.endeca.navigation.ENEConnectionException')}">
                            <div class="errorMessage">
                                <fmt:message key="host_connection_error">
                                    <fmt:param value="${param.mdexHost}"/>
                                    <fmt:param value="${param.mdexPort}"/>
                                </fmt:message>
                            </div>
                        </c:when>
                        <c:otherwise>
                            <discover:include component="${element}"/>
                        </c:otherwise>
                    </c:choose>
                </c:forEach>
            </div>
        </div>
    </div>
</div>
</body>
</html>
