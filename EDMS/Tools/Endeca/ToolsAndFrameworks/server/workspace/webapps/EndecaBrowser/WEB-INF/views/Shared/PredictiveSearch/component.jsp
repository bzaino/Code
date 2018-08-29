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

<%@include file="/WEB-INF/views/include.jsp"%>
<%@page language="java" pageEncoding="UTF-8" contentType="text/html;charset=UTF-8"%>

<div class="PredictiveList">
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
<div class="predictiveListHeader">
    <div class="predictiveListTitle floatLeft">
        <c:choose>
            <c:when test="${param.page eq 'record' }" >
                <fmt:message key="records"/>
            </c:when>
            <c:when test="${not empty param.page and param.page eq 'nav' }" >
                <fmt:message key="filtered_state"/>
            </c:when>
        </c:choose>
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
    <c:choose>
        <c:when test="${component.displaySortOptions}">
            <fmt:message key="sort_by"/>
            <select id="sortSelector" onchange="location = this.options[this.selectedIndex].value">
                <c:forEach var="sort" items="${component.precomputedSorts}">
                    <c:set var="ascLabel" value="${sort.ascendingAction.label}" />
                    <c:set var="descLabel" value="${sort.descendingAction.label}" />
                    <option value="${util:getUrlForAction(sort.ascendingAction)}"
                            <c:choose>
                                <c:when test="${sort.ascendingAction.selected}">
                                    selected="true"
                                </c:when>
                            </c:choose>
                            >
                        <c:choose>
                            <c:when test="${ascLabel eq ''}">
                                <fmt:message key="default_sort" />
                            </c:when>
                            <c:otherwise>
                                <fmt:message key="ascending_sort">
                                    <fmt:param value="${ascLabel}"/>
                                </fmt:message>
                            </c:otherwise>
                        </c:choose>
                    </option>
                    <c:choose>
                        <c:when test="${ascLabel ne ''}">
                            <option value="${util:getUrlForAction(sort.descendingAction)}"
                                    <c:choose>
                                        <c:when test="${sort.descendingAction.selected}">
                                            selected="true"
                                        </c:when>
                                    </c:choose>
                                    >
                                <c:choose>
                                    <c:when test="${descLabel eq ''}">
                                        <fmt:message key="default_sort" />
                                    </c:when>
                                    <c:otherwise>
                                        <fmt:message key="descending_sort">
                                            <fmt:param value="${descLabel}"/>
                                        </fmt:message>
                                    </c:otherwise>
                                </c:choose>
                            </option>
                        </c:when>
                    </c:choose>
                </c:forEach>
            </select>
        </c:when>
    </c:choose>
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

<div>
    <c:forEach var="record" varStatus="count" items="${component.records}">
        <c:set var="selectedRecords" value="${util:escapeJavaScript(param.selectedRecords)}"/>
        <c:set var="imgThumbnail" value="${util:escapeHtml(record.attributes[param.imgURL][0])}"/>
        <c:set var="rollupFieldVal" value="${util:escapeJavaScript(record.attributes[param.rollupKey])}"/>
        <c:set var="spec" value="${util:escapeJavaScript(record.attributes[param.spec][0])}"/>
        <c:set var="properties" value="${param.properties}"/>
        <c:set var="select" value="${param.recordSelect}"/>
        <c:set var="recordId" value="${util:escapeJavaScript(record.attributes[param.recordIdKey][0])}"/>

        <div class="record">
            <div class="checkbox">
                <c:if test="${param.page eq 'record'}">
                    <c:set var="recordSelected" value="${fn:contains(selectedRecords,recordId)}" />
                    <c:choose>
                        <c:when test="${select eq 'multi'}">
                            <input type="checkbox"
                        </c:when>
                        <c:otherwise>
                            <input type="radio"
                        </c:otherwise>
                    </c:choose>
                    <c:choose>
                        <c:when test="${recordSelected}">
                            checked=true
                        </c:when>
                    </c:choose>
                    <c:choose>

                        <c:when test="${select eq 'multi'}">
                            onclick="toggle_${count.index}(false)"
                        </c:when>
                        <c:when test="${!recordSelected}">
                            onclick="toggle_${count.index}(true)"
                        </c:when>
                    </c:choose>
                    name="selectedRecord"
                    id="select_record_${util:escapeHtml(recordId)}"
                    />
                </c:if>
            </div>
            <div class="thumbnail floatLeft">
                <c:choose>
                    <c:when test="${not empty imgThumbnail}">
                        <c:set var="thumbnail" value="${imgThumbnail}" />
                    </c:when>
                    <c:otherwise>
                        <c:url var="noImage" value="/images/no_image_75x75.gif" />
                        <c:set var="thumbnail" value="${noImage}" />
                    </c:otherwise>
                </c:choose>
                <img class="pic" src="${thumbnail}" />
            </div>
            <script type="text/javascript" >
                function toggle_${count.index}(singleSelect) {
                    var record = {
                        <c:forEach items="${fn:split(properties,':')}" var="prop" varStatus="propStatus">
                        'field${propStatus.index}' : '<c:out value="${util:escapeJavaScript(record.attributes[prop])}" />',
                        </c:forEach>
                        '__Endeca_Browser_id' : '<c:out value="${recordId}"/>',
                        '__Endeca_Browser_spec': '<c:out value="${spec}" />',
                        '__Endeca_Browser_rollupVal': '<c:out value="${rollupFieldVal}" />',
                        '__Endeca_Browser_thumbnail': '<c:out value="${util:escapeJavaScript(thumbnail)}" />',
                        '__Endeca_Browser_valid': true
                    };
                    toggleRecord(record, singleSelect);
                }
            </script>
            <div class="basicProductInfo">
                <div>
                    <fmt:message key="record_id"/>
                    <c:out value="${spec}"/>
                </div>
                <c:forEach items="${fn:split(properties,':')}" var="prop">
                    <div>
                            ${record.attributes[prop]}
                    </div>
                </c:forEach>
                <div class="MoreDetails">
                    <a href="javascript:toggle('show_more_${count.index}','show_more_text_${count.index}');">
                                    <span id="show_more_text_${count.index}">
                                        <fmt:message key="eb.records.available.show_more"/>
                                    </span>
                    </a>
                </div>
                <div id="show_more_${count.index}" class="allProperties">
                    <table>
                        <c:forEach var="currentRecord" items="${record.attributes}">
                            <c:forEach var="attrValue" items="${currentRecord.value}">
                                <tr>
                                    <td class="name"><c:out value="${currentRecord.key}"></c:out></td>
                                    <td><c:out value="${attrValue}"></c:out></td>
                                </tr>
                            </c:forEach>
                        </c:forEach>
                    </table>
                </div>
            </div>
        </div>
    </c:forEach>
</div>
</c:otherwise>
</c:choose>
</div>
