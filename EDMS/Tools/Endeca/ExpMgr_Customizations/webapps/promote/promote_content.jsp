<HTML>
<HEAD>
<TITLE>TEST</TITLE>
</HEAD>
<BODY>
<%
String cmd = "D:\\Endeca\\apps\\SALT\\control\\promote_content.bat" ;
java.lang.Process pb = new java.lang.ProcessBuilder(cmd).start();

java.io.BufferedReader buf = new java.io.BufferedReader( new java.io.InputStreamReader( pb.getInputStream() ) ) ;

String line;
while ( ( line = buf.readLine() ) != null )
{
out.println(line);%><BR><%
}
%>
</BODY>
</HTML>
