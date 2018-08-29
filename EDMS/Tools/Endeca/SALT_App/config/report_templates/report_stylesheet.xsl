<?xml version="1.0"?>
<!--
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
  -->

<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html" encoding="UTF-8"/>

<!--
REPORT_STYLESHEET.XSL
This Style Sheet provides a sample rendering for Endeca Reports.
It includes four templates:

	- Report - Outline for entire report page
	- Report Group - Wraps a series of reports with a header and table
	- Number Report - Report with single value (Number of Requests, for example)
	- Roster Report - Report with multiple rows and values (Top 20 Searches, for example)
	- Message Report - Single table cell displaying message reports specified in config
-->



<!-- REPORT -->
<xsl:template match="reports">
	<html>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
	<title>Endeca Log Report - Sample Wine Application</title>
	<body link="blue" alink="blue" vlink="blue" leftmargin="20" topmargin="20">

	<!-- Report header, including title and image -->
	<a name="TOP"></a>
	<table border="0" cellpadding="0" cellspacing="0" width="500">
		<tr><td><font face="arial" style="font-size:14pt">Endeca Log Report<br></br>Sample Wine Application</font></td></tr>
		<tr><td height="10"></td></tr>
		<tr><td><font face="arial" style="font-size:9pt" color="gray">Report Generated on <xsl:value-of select="@gen_date"/></font></td></tr>
		<tr><td><font face="arial" style="font-size:9pt" color="gray">From <xsl:value-of select="@logfile"/>/*</font></td></tr>
		<tr><td><font face="arial" style="font-size:9pt" color="gray">First Log Entry Processed: <xsl:value-of select="@start_date"/></font></td></tr>
		<tr><td><font face="arial" style="font-size:9pt" color="gray">Last Log Entry Processed: <xsl:value-of select="@stop_date"/></font></td></tr>
	</table>
	<br></br>

	<!-- Anchor links for table of contents -->
	<table border="0" cellpadding="0" cellspacing="0">
	<xsl:for-each select="group_report[@id!='']">
		<tr><td><font face="arial" style="font-size:9pt"><a><xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute><xsl:value-of select="@title"/></a></font></td></tr>
	</xsl:for-each>
	</table>
	<br></br>

	<!-- Body of report -->
	<xsl:apply-templates/>

	</body>
	</html>
</xsl:template>



<!-- REPORT GROUP -->
<xsl:template match="group_report">

	<!-- Anchor target from table of contents -->
	<a><xsl:attribute name="name"><xsl:value-of select="@id"/></xsl:attribute></a>

	<!-- Top-level group (not nested within any groups) -->
	<xsl:if test="count(ancestor::group_report) = 0">
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr height="1"><td bgcolor="#CCCCCC"></td></tr>
			<tr height="10"><td></td></tr>

			<!-- Group title -->
			<tr><td>
			<table border="0" cellpadding="0" cellspacing="0" width="500">
				<tr>
				<td><font face="arial" style="font-size:12pt"><b><xsl:value-of select="@title"/></b></font></td>
				<td align="right"><a href="#TOP"><font face="arial" style="font-size:7pt">Back to Top</font></a></td>
				</tr>
			</table>
			</td>
			</tr>
			<tr height="10"><td></td></tr>

			<!-- Display reports (all elements except messages) -->
			<tr><td>
			<table border="0" cellpadding="0" cellspacing="0" width="500">
				<xsl:apply-templates select="*[not(self::passthrough_report)]"/>
			</table>
			</td></tr>
			<tr height="10"><td></td></tr>

			<!-- Display messages, if any -->
			<xsl:apply-templates select="passthrough_report"/>
		</table>
	</xsl:if>

	<!-- Second-level group (nested within another group) -->
	<xsl:if test="count(ancestor::group_report) = 1">

		<!-- Group title -->
		<tr><td colspan="2"><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="@title"/></font></td></tr>

		<!-- Display reports -->
		<xsl:apply-templates/>
		<tr height="15"><td></td></tr>
	</xsl:if>

</xsl:template>



<!-- NUMBER REPORT -->
<xsl:template match="number_report">

	<!-- Alternate background color depending on even-odd reports -->
	<xsl:variable name="shaded" select="count(preceding-sibling::number_report) mod 2 = 1"/>
	<tr><xsl:attribute name="bgcolor"><xsl:if test="not($shaded)">#FFFFFF</xsl:if><xsl:if test="$shaded">#EEEEEE</xsl:if></xsl:attribute>

	<!-- Report Title -->
	<td><font face="arial" style="font-size:9pt"><xsl:value-of select="title"/></font></td>

	<!-- Report Value -->
	<td align="right"><font face="arial" style="font-size:9pt" color="green"><xsl:value-of select="data"/></font></td>
	</tr>
</xsl:template>



<!-- ROSTER REPORT -->
<xsl:template match="roster_report">
	<tr><td colspan="2">
	<table border="0" cellspacing="0" cellpadding="0" width="100%">

		<!-- Report Title -->
		<tr><td colspan="2"><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="title"/></font></td></tr>
		<tr>

		<!-- First Column Header -->
		<td><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[1]"/></font></td>
		<!-- Optional Column Header (for "correction" in top_autocorrected_terms and "suggestion" in top_search_terms_dym_engaged) -->
		<xsl:if test = "column_headers/*[3] != ''" >
			<td><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[3]"/></font></td>
		</xsl:if>
		<!-- Optional Column Header (for "search key" in top_search_terms_w_results) -->
		<xsl:if test = "column_headers/*[4] != ''" >
			<td><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[4]"/></font></td>
		</xsl:if>
		<!-- Optional Column Header (for "search mode" in top_search_terms_w_results) -->
		<xsl:if test = "column_headers/*[5] != ''" >
			<td><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[5]"/></font></td>
		</xsl:if>
		<!-- Optional Column Header (for "# results" in top_search_terms_w_results) -->
		<xsl:if test = "column_headers/*[6] != ''" >
			<td align="right"><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[6]"/></font></td>
		</xsl:if>
		<!-- Last Column Header (often blank) -->
		<td align="right"><font face="arial" style="font-size:9pt" color="gray"><xsl:value-of select="column_headers/*[2]"/></font></td>
		</tr>

		<!-- Loop over report rows -->
		<xsl:for-each select="data_row">

			<!-- Alternate background color depending on even-odd reports -->
			<xsl:variable name="shaded" select="count(preceding-sibling::data_row) mod 2 = 1"/>
			<tr><xsl:attribute name="bgcolor"><xsl:if test="not($shaded)">#FFFFFF</xsl:if><xsl:if test="$shaded">#EEEEEE</xsl:if></xsl:attribute>

			<!-- If row_header available, simple two column report-->
			<xsl:if test = "row_header != ''" >

				<!-- Row header -->
				<td><font face="arial" style="font-size:9pt"><xsl:value-of select="row_header"/></font></td>
				<!-- Row data -->
				<td align="right"><font face="arial" style="font-size:9pt" color="green"><xsl:value-of select="data[1]"/></font></td>

			</xsl:if>

			<!-- If no row_header available, multi-column report-->
			<xsl:if test = "not(row_header != '')" >

				<!-- Row header -->
				<td><font face="arial" style="font-size:9pt"><xsl:value-of select="data[1]"/></font></td>
				<!-- Optional Column (for "correction" in top_autocorrected_terms and "suggestion" in top_search_terms_dym_engaged) -->
				<xsl:if test = "data[3] != ''" >
					<td width="100"><font face="arial" style="font-size:9pt" color="#999966"><xsl:value-of select="data[3]"/></font></td>
				</xsl:if>
				<!-- Optional Column (for "search key" in top_search_terms_w_results) -->
				<xsl:if test = "data[4] != ''" >
					<td><font face="arial" style="font-size:7pt" color="gray"><xsl:value-of select="data[4]"/></font></td>
				</xsl:if>
				<!-- Optional Column (for "search mode" in top_search_terms_w_results) -->
				<xsl:if test = "data[5] != ''" >
					<td><font face="arial" style="font-size:7pt" color="gray"><xsl:value-of select="data[5]"/></font></td>
				</xsl:if>
				<!-- Optional Column (for "# results" in top_search_terms_w_results) -->
				<xsl:if test = "data[6] != ''" >
					<td align="right" width="80"><font face="arial" style="font-size:9pt" color="#999966"><xsl:value-of select="data[6]"/></font></td>
				</xsl:if>
				<!-- Row data -->
				<td width="60" align="right"><font face="arial" style="font-size:9pt" color="green"><xsl:value-of select="data[2]"/></font></td>

			</xsl:if>
			</tr>
		</xsl:for-each>
	</table>
	</td></tr>
	<tr height="15" colspan="2"><td></td></tr>
</xsl:template>

<!-- MESSAGE REPORT -->
<xsl:template match="passthrough_report">
    <tr><td>
	<xsl:if test="count(ancestor::group_report) = 0">
		<table border="0" cellpadding="0" cellspacing="0" width="500">
			<!-- Display message text -->
			<tr><td><font face="arial" style="font-size:9pt" color="gray"><xsl:apply-templates/></font></td></tr>
		</table>
	</xsl:if>
	<xsl:if test="count(ancestor::group_report) = 1">
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<!-- Display message text -->
			<tr><td><font face="arial" style="font-size:9pt" color="gray"><xsl:apply-templates/></font></td></tr>
		</table>
	</xsl:if>	

	</td></tr>
	<tr height="20"><td></td></tr>
</xsl:template>

</xsl:stylesheet>
