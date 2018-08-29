/*
 * Copyright 2001, 2012, Oracle and/or its affiliates. All rights reserved.
 * Oracle and Java are registered trademarks of Oracle and/or its
 * affiliates. Other names may be trademarks of their respective owners.
 * UNIX is a registered trademark of The Open Group.
 *
 * This software and related documentation are provided under a license
 * agreement containing restrictions on use and disclosure and are
 * protected by intellectual property laws. Except as expressly permitted
 * in your license agreement or allowed by law, you may not use, copy,
 * reproduce, translate, broadcast, modify, license, transmit, distribute,
 * exhibit, perform, publish, or display any part, in any form, or by any
 * means. Reverse engineering, disassembly, or decompilation of this
 * software, unless required by law for interoperability, is prohibited.
 * The information contained herein is subject to change without notice
 * and is not warranted to be error-free. If you find any errors, please
 * report them to us in writing.
 * U.S. GOVERNMENT END USERS: Oracle programs, including any operating
 * system, integrated software, any programs installed on the hardware,
 * and/or documentation, delivered to U.S. Government end users are
 * "commercial computer software" pursuant to the applicable Federal
 * Acquisition Regulation and agency-specific supplemental regulations.
 * As such, use, duplication, disclosure, modification, and adaptation
 * of the programs, including any operating system, integrated software,
 * any programs installed on the hardware, and/or documentation, shall be
 * subject to license terms and license restrictions applicable to the
 * programs. No other rights are granted to the U.S. Government.
 * This software or hardware is developed for general use in a variety
 * of information management applications. It is not developed or
 * intended for use in any inherently dangerous applications, including
 * applications that may create a risk of personal injury. If you use
 * this software or hardware in dangerous applications, then you shall
 * be responsible to take all appropriate fail-safe, backup, redundancy,
 * and other measures to ensure its safe use. Oracle Corporation and its
 * affiliates disclaim any liability for any damages caused by use of this
 * software or hardware in dangerous applications.
 * This software or hardware and documentation may provide access to or
 * information on content, products, and services from third parties.
 * Oracle Corporation and its affiliates are not responsible for and
 * expressly disclaim all warranties of any kind with respect to
 * third-party content, products, and services. Oracle Corporation and
 * its affiliates will not be responsible for any loss, costs, or damages
 * incurred due to your access to or use of third-party content, products,
 * or services.
 */

package com.endeca.infront.refapp.navigation;

import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.servlet.http.HttpServletRequest;

import com.endeca.infront.cartridge.model.NavigationAction;
import com.endeca.infront.cartridge.model.RecordAction;
import com.endeca.infront.content.ContentException;
import com.endeca.infront.content.source.ContentLocator;
import com.endeca.infront.content.source.ContentSource;
import com.endeca.infront.navigation.url.ActionPathProvider;

/**
 * A basic implementation of {@link ActionPathProvider}.
 * @see #BasicActionPathProvider(ContentSource, HttpServletRequest, Map, Map)
 */
public class BasicActionPathProvider implements ActionPathProvider {
    
    private ContentLocator navigationActionLocator = null;
    private ContentLocator recordActionLocator = null;

    /**
     * Constructs a new <tt>ActionPathProvider</tt> which will generate
     * paths using the following procedure:
     * <ol>
     * <li>Use the given content source to resolve the given request,
     * producing the content URI for the current page</li>
     * <li>Iterate through the <tt>navigationActionUriMap</tt>.  For each 
     * entry: <ol>
     *    <li>Treating the entry key as a regular expression, see if it
     *    matches the current content URI.</li>
     *    <li>If so, replace all matches with the entry value to generate
     *    the destination content URI to provide for constructing 
     *    {@link NavigationAction} objects</li>
     *    <li>Otherwise, try the next entry</li></ol>
     * <li>If no entries matched, just use the source content URI</li>
     * <li>Repeat steps 2 and 3 using the <tt>recordActionUriMap</tt> to 
     * generate the paths to provide for constructing 
     * {@link RecordAction} objects.</li>
     * </ol>
     *
     * @param source The content source used to resolve content URIs
     * @param request The request from which to infer the source content path
     * @param navigationActionUriMap The regular expressions used to map the 
     * source content URI to the content URI to be provided for use in
     * constructing navigation actions
     * @param recordActionUriMap The regular expressions used to map the 
     * source content URI to the content URI to be provided for use in
     * constructing record actions
     */
    public BasicActionPathProvider(ContentSource source,
                                   HttpServletRequest request,
                                   Map<String, String> navigationActionUriMap,
                                   Map<String, String> recordActionUriMap)
            throws ContentException {
        ContentLocator originalLocator = source.resolveContent(request);
        
        if (originalLocator != null) {
            String uri = originalLocator.getContentUri();
            
            if (uri != null) {
                String navUri = attemptMatch(uri, navigationActionUriMap);
                String recordUri = attemptMatch(uri, recordActionUriMap);
                
                if (navUri == null)
                    navigationActionLocator = originalLocator;
                else
                    navigationActionLocator = source.resolveContent(navUri);
                
                if (recordUri == null)
                    recordActionLocator = originalLocator;
                else
                    recordActionLocator = source.resolveContent(recordUri);
                
            }
        }
    }
    
    /**
     * Iterates through the given map, treating each key/value pair as 
     * a regular expression and a replacement string, respectively.
     * <p/>
     * If a key matches the given URI, replace the matches with the
     * replacement string and return the result.  Otherwise, proceed
     * to the next pair.  If no key matches, return null.
     */
    protected String attemptMatch(String uri, Map<String, String> regexps) {
        for (Map.Entry<String, String> entry : regexps.entrySet()) {
            Matcher matcher = Pattern.compile(entry.getKey()).matcher(uri);
            
            if (matcher.matches())
                return matcher.replaceAll(entry.getValue());
        }
        
        return null;
    }

    public String getDefaultNavigationActionSiteRootPath() {
        return navigationActionLocator == null ? 
                null : navigationActionLocator.getSiteRootPath();
    }

    public String getDefaultNavigationActionContentPath() {
        return navigationActionLocator == null ? 
                null : navigationActionLocator.getContentPath();
    }

    public String getDefaultRecordActionSiteRootPath() {
        return recordActionLocator == null ? 
                null : recordActionLocator.getSiteRootPath();
    }

    public String getDefaultRecordActionContentPath() {
        return recordActionLocator == null ? 
                null : recordActionLocator.getContentPath();
    }
}
