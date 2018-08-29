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
<%@page import="java.lang.Math"%>
<%@page import="com.endeca.infront.cartridge.support.ResourceUtil"%>
<%@include file="/WEB-INF/views/include.jsp"%>


<div class="ResultsList">
    <c:choose>
        <c:when test="${empty component.records}">
            <div class="zeroResults">
                <fmt:message key="zero_results"/>
            </div>
            <div class="zeroResultsAdvice">
                <fmt:message key="zero_results_advice"/>
            </div>
        </c:when>
        <c:otherwise>
            <div class="resultsListHeader">
                <div class="resultsListTitle floatLeft">
                    <fmt:message key="media" />
                </div>
                <div class="pagination">
                    <div>
                        <c:if test="${(component.totalNumRecs / component.recsPerPage > 1)}">
                            <%@include file="ResultsSetPagination.jsp"%>
                        </c:if>
                    </div>
                    <div class="paginationInfo floatRight">
                        <c:choose>
                            <c:when test="${component.totalNumRecs == 1}">
                                <fmt:message key="n_items">
                                    <fmt:param value="1"/>
                                </fmt:message>
                            </c:when>
                            <c:otherwise>
                                <fmt:message key="record_paging_info">
                                    <fmt:param value="${component.firstRecNum}"/>
                                    <fmt:param value="${component.lastRecNum}"/>
                                    <fmt:param value="${component.totalNumRecs}"/>
                                </fmt:message>
                            </c:otherwise>
                        </c:choose>
                    </div>
                </div>
                <div class="clearBoth"></div>
            </div>
            
            <div class="recordsTableHeader">
                <div class="floatRight">
                    <span><fmt:message key="display"/></span> 
                    <select
                        onchange="location = this.options[this.selectedIndex].value">
                        <c:forEach var="i" begin="1" end="10" step="1" varStatus="status">
                            <c:url var="firstPageURL"
                                value="${util:getUrlForAction(util:changeRecordsPerPage(component.pagingActionTemplate, 0, i*10))}" />
                            <option value="${firstPageURL}"
                                <c:if test="${i*10 == component.recsPerPage}">
                                    selected="true"
                                </c:if>>
                                <fmt:message key="records_per_page">
                                    <fmt:param value="${i*10}"/>
                                </fmt:message>
                            </option>
                        </c:forEach>
                    </select>
                </div>
                <div class="clearBoth"></div>
            </div>
            
            <div class="mediaTable">
                <div class="mediaHeader">
                    <div>
                        <fmt:message key="media_item" />
                    </div>
                    <div>
                        <fmt:message key="file_size" />
                    </div>
                    <div>
                        <fmt:message key="image_dimensions" />
                    </div>
                    <div>
                        <fmt:message key="last_modified" />
                    </div>
                    <div>
                        <fmt:message key="author" />
                    </div>
                </div>
                <div class="mediaTableBody">
                    <c:set var="selectedRecords" value="${param.selectedRecords}"/>
                    
                    <c:forEach var="record" items="${component.records}" varStatus="status">
                        <c:set var="recordId" value="${util:escapeJavaScript(record.attributes['record.id'][0])}"/>
                        <c:set var="name" value="${util:escapeJavaScript(record.attributes['media.name'][0])}"/>
                        <c:set var="size" value="${util:escapeJavaScript(record.attributes['media.size'][0])}"/>
                        <c:set var="width" value="${util:escapeJavaScript(record.attributes['image.width'][0])}"/>
                        <c:set var="height" value="${util:escapeJavaScript(record.attributes['image.height'][0])}"/>
                        <c:set var="dimensions" value="${width}x${height} px"/>
                        <c:set var="lastModified" value="${util:escapeJavaScript(record.attributes['media.last_modification_date'][0])}"/>
                        <c:set var="author" value="${util:escapeJavaScript(record.attributes['media.author'][0])}"/>
                        <c:set var="path" value="${util:escapeJavaScript(record.attributes['media.path'][0])}"/>
                        <c:set var="repository" value="${util:escapeJavaScript(record.attributes['media.repository_id'][0])}"/>
                        <c:set var="fullImagePath" value="${util:escapeHtml(param[repository])}${util:escapeHtml(path)}" />

                        <div class="mediaTableRow">
                            <div class="mediaItem">
                                <span style="display:block">
                                    <input type="radio" name="selectedRecord" value="${recordId}"
                                        <c:choose>
                                            <c:when test="${selectedRecords eq recordId}">
                                                checked=true
                                            </c:when>
                                            <c:otherwise>
                                                onclick="insertParams(
                                                    ['selectedRecords', 
                                                    'selectedMediaURI', 
                                                    'selectedMediaWidth', 
                                                    'selectedMediaHeight', 
                                                    'selectedMediaSize', 
                                                    'selectedMediaSrcKey'],
                                                    ['${recordId}', 
                                                    '${path}', 
                                                    '${width}', 
                                                    '${height}', 
                                                    '${size}', 
                                                    '${repository}'])"
                                            </c:otherwise>
                                        </c:choose>
                                    />
                                    
                                    <c:out value="${name}"/>
                                </span>
                                <c:choose>
                                    <c:when test="${not empty path}">
                                        <c:url var="image" value="${fullImagePath}" />
                                    </c:when>
                                    <c:otherwise>
                                        <c:url var="noImage" value="/images/no_image_75x75.gif" />
                                        <c:set var="image" value="${noImage}" />
                                    </c:otherwise>
                                </c:choose>
                                
                                <%-- Math to calculate the correct image size --%>
                                <%
                                Object widthObj = pageContext.getAttribute("width");
                                Object heightObj = pageContext.getAttribute("height");
                                float height = Float.parseFloat(heightObj.toString());
                                float width = Float.parseFloat(widthObj.toString());
                                
                                if (Math.max(height, width) > 75) {
                                	pageContext.setAttribute("imgWidth", 
                                            height > width ? (75 / height) * width : 75);
                                    pageContext.setAttribute("imgHeight", 
                                    		height > width ? 75 : (75 / width) * height);
                                }else{
                                	pageContext.setAttribute("imgWidth", width);
                                    pageContext.setAttribute("imgHeight", height);
                                }
                                
                                if (Math.max(height, width) > 320) {
                                    pageContext.setAttribute("imgWidthLarge", 
                                            height > width ? (320 / height) * width : 320);
                                    pageContext.setAttribute("imgHeightLarge", 
                                            height > width ? 320 : (320 / width) * height);
                                }else{
                                    pageContext.setAttribute("imgWidthLarge", width);
                                    pageContext.setAttribute("imgHeightLarge", height);
                                }
                                
                                %>
                                
                                <a class="imgThumbnail" href="#">
                                    <img class="small" src="${image}" width="${imgWidth}" height="${imgHeight}" />
                                    
                                    <div class="previewImage">
                                        <img class="large" src="${image}" height="${imgHeightLarge}"
                                            width="${imgWidthLarge}" />
                                    </div>
                                </a>
                            </div>
                            <div>
                                <span><c:out value="${size}"/> bytes</span>
                            </div>
                            <div>
                                <span><c:out value="${dimensions}"/></span>
                            </div>
                            <div>
                                <span>
                                    <jsp:useBean id="dateValue" class="java.util.Date" />   
                                    <jsp:setProperty name="dateValue" property="time" value="${lastModified}" />
                                    <fmt:formatDate value="${dateValue}" type="both" timeStyle="short" />
                                </span>
                            </div>
                            <div>
                                <span><c:out value="${author}"/></span>
                            </div>
                        </div>
                    </c:forEach>
            </div>
        </c:otherwise>
    </c:choose>
</div>
