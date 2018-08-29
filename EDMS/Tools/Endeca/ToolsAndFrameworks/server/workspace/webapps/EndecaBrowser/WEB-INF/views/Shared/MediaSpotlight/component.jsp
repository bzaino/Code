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
<%@page import="java.util.Date"%>
<%@include file="/WEB-INF/views/include.jsp"%>

<c:if test="${not empty component.records}">
    <div class="RecordSpotlight">
        <div class="spotLightHeaderText floatLeft">
            <fmt:message key="selected_media_item"/>
        </div>
        
        <div>
            <c:forEach var="record" items="${component.records}" varStatus="status">
                <c:set var="name" value="${util:escapeJavaScript(record.attributes['media.name'][0])}"/>
                <c:set var="size" value="${util:escapeJavaScript(record.attributes['media.size'][0])}"/>
                <c:set var="width" value="${util:escapeJavaScript(record.attributes['image.width'][0])}"/>
                <c:set var="height" value="${util:escapeJavaScript(record.attributes['image.height'][0])}"/>
                <c:set var="dimensions" value="${width}x${height} px"/>
                <c:set var="lastModified" value="${util:escapeJavaScript(record.attributes['media.last_modification_date'][0])}"/>
                <c:set var="author" value="${util:escapeJavaScript(record.attributes['media.author'][0])}"/>
                <c:set var="mimeType" value="${util:escapeJavaScript(record.attributes['media.mime_type'][0])}"/>
                <c:set var="spec" value="${util:escapeJavaScript(record.attributes['record.id'][0])}"/>
                <c:set var="recordId" value="${util:escapeJavaScript(record.attributes[param.recordIdKey][0])}"/>
                        
                <div class="selectedRecord">
                    <div class="floatRight">
                        <input id="removeRecord" type="image" src="<c:url value="/images/iconDelete.png"/>" 
                            name="removeRecord" 
                            onclick="javascript:removeParam('selectedRecords')"></input>
                    </div>
                    
                    <div class="mediaInfoTable">
                        <div class="mediaInfoRow">
                            <div class="title"><fmt:message key="media_item"/></div>
                            <div><c:out value="${name}" /></div>
                        </div>
                        <div class="mediaInfoRow">
                            <div class="title"><fmt:message key="file_size"/></div>
                            <div><c:out value="${size} bytes" /></div>
                        </div>
                        <div class="mediaInfoRow">
                            <div class="title"><fmt:message key="image_dimensions"/></div>
                            <div><c:out value="${dimensions}" /></div>
                        </div>
                        <div class="mediaInfoRow">
                            <jsp:useBean id="dateValue" class="java.util.Date" />   
                            <jsp:setProperty name="dateValue" property="time" value="${lastModified}" />
                            <div class="title"><fmt:message key="last_modified"/></div>
                            <div><fmt:formatDate value="${dateValue}" type="both" timeStyle="short" /></div>
                        </div>
                        <div class="mediaInfoRow">
                            <div class="title"><fmt:message key="author"/></div>
                            <div><c:out value="${author}" /></div>
                        </div>
                    </div>
                    <!-- Converts the displayed part of the record into javascript where it is picked up by the ExtJS components -->
                    <script type="text/javascript" >
                        var record = {
                            '__Endeca_Browser_id': '<c:out value="${recordId}" />',
                            '__Endeca_Browser_spec': '<c:out value="${spec}" />',
                            'selectedMediaURI': '<c:out value="${util:escapeJavaScript(param.selectedMediaURI)}" />',
                            'selectedMediaWidth': '<c:out value="${util:escapeJavaScript(param.selectedMediaWidth)}" />',
                            'selectedMediaHeight': '<c:out value="${util:escapeJavaScript(param.selectedMediaHeight)}" />',
                            'selectedMediaSize': '<c:out value="${util:escapeJavaScript(param.selectedMediaSize)}" />',
                            'selectedMediaSrcKey': '<c:out value="${util:escapeJavaScript(param.selectedMediaSrcKey)}" />',
                            'mediaType': '<c:out value="${mimeType}" />'
                        };
                        EndecaBrowserState.setMedia(record);
                    </script>
                </div>
            </c:forEach>
        </div>
    </div>
</c:if>
