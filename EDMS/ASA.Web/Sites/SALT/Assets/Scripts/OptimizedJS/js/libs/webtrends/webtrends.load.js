// WebTrends SmartSource Data Collector Tag v10.2.55
// Copyright (c) 2013 Webtrends Inc.  All rights reserved.
// Tag Builder Version: 4.1.0.42
// Created: 2013.01.22
window.webtrendsAsyncInit=function(){
    var dcs=new Webtrends.dcs().init({
        dcsid:"dcsxx1gz6wz5bd051hwv5wjc4_1i1g",
        domain:"statse.webtrendslive.com",
        timezone:-5,
        i18n:true,
        offsite:true,
        download:true,
        downloadtypes:"xls,doc,pdf,txt,csv,zip,docx,xlsx,rar,gzip",
        anchor:true,
        onsitedoms:"saltmoney.org",
        fpcdom:".saltmoney.org",
        plugins:{
            hm:{src:"//s.webtrends.com/js/webtrends.hm.js"}
        }
        }).track();
};
(function(){
    var s=document.createElement("script"); s.async=true; s.src="/assets/scripts/js/libs/webtrends/webtrends.min.js";    
    var s2=document.getElementsByTagName("script")[0]; s2.parentNode.insertBefore(s,s2);
}());
