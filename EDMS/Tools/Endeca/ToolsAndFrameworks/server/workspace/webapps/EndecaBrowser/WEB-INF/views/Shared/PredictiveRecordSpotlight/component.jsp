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
<%@page import="java.util.*"%>
<%@include file="/WEB-INF/views/include.jsp"%>

<c:if test="${not empty component.records}">
    <div class="PredictiveRecordSpotlight">
        <div class="spotLightHeaderText floatLeft">
            <fmt:message key="selected_record"/>
        </div>
        <div>
            <c:forEach var="record" items="${component.records}" varStatus="status">
                <c:set var="imgThumbnail" value="${util:escapeHtml(record.attributes[param.imgURL][0])}"/>
                <c:set var="rollupFieldVal" value="${util:escapeJavaScript(record.attributes[param.rollupKey][0])}"/>
                <c:set var="spec" value="${util:escapeJavaScript(record.attributes[param.spec][0])}"/>
                <c:set var="properties" value="${param.properties}"/>
                <c:set var="recordIdKey" value="${param.recordIdKey}"/>
                
                <div class="selectedRecord">
                    <c:choose>
                        <c:when test="${not empty imgThumbnail}">
                            <c:set var="thumbnail" value="${imgThumbnail}" />
                        </c:when>
                        <c:otherwise>
                            <c:url var="noImage" value="/images/no_image_75x75.gif" />
                            <c:set var="thumbnail" value="${noImage}" />
                        </c:otherwise>
                    </c:choose>
                    <!-- Converts the displayed part of the record into javascript where it is picked up by the ExtJS components -->
                    <script type="text/javascript" >
                        var recordId = '<c:out value="${util:escapeJavaScript(record.attributes[recordIdKey][0])}" />';
                        var recordIndex = EndecaBrowserState.getRecordIndex(recordId);
                        var record = {
                            <c:forEach items="${fn:split(properties,':')}" var="prop" varStatus="propStatus">
                                    'field${propStatus.index}': '<c:out value="${util:escapeJavaScript(record.attributes[prop][0])}" />',
                            </c:forEach>
                            '__Endeca_Browser_id' : recordId,
                            '__Endeca_Browser_spec': '<c:out value="${spec}" />',
                            '__Endeca_Browser_rollupVal': '<c:out value="${rollupFieldVal}" />',
                            '__Endeca_Browser_thumbnail': '<c:out value="${util:escapeJavaScript(thumbnail)}" />',
                            '__Endeca_Browser_index': recordIndex === -1 ? EndecaBrowserState.getRecords().length + 1 : recordIndex + 1,
                            '__Endeca_Browser_valid': true
                        };
                        //Replace the dummy records that we added in the TwoColumnNavigationPage with the records in the spotlight
                        EndecaBrowserState.replaceRecord(recordIndex, record);
                    </script>
               </div>
            </c:forEach>
        </div>
    </div>
</c:if>
