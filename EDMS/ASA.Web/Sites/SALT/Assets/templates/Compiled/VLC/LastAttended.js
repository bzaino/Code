define(["dust", "Compiled/VLC/partial_specialContent", "Compiled/VLC/partial_newTab_buttons", "Compiled/VLC/partial_VLC_buttons"], function(dust) { (function(){dust.register("VLC/LastAttended",body_0);function body_0(chk,ctx){return chk.reference(ctx.getPath(false,["mainContent","0","records","0","attributes","body"]),ctx,"h").write("<form id=\"LastAttended\"><div class=\"row\"><div class=\"small-12 medium-4 columns\"><div class=\"VLCLabel\">Month</div><div class=\"styled-select\"><select id=\"month\"><option value=\"01\">01</option><option value=\"02\">02</option><option value=\"03\">03</option><option value=\"04\">04</option><option value=\"05\">05</option><option value=\"06\">06</option><option value=\"07\">07</option><option value=\"08\">08</option><option value=\"09\">09</option><option value=\"10\">10</option><option value=\"11\">11</option><option value=\"12\">12</option></select></div></div><div class=\"small-12 medium-4 columns\"><div class=\"VLCLabel\">Day</div><div class=\"styled-select\"><select id=\"day\"><option value=\"01\">01</option><option value=\"02\">02</option><option value=\"03\">03</option><option value=\"04\">04</option><option value=\"05\">05</option><option value=\"06\">06</option><option value=\"07\">07</option><option value=\"08\">08</option><option value=\"09\">09</option><option value=\"10\">10</option><option value=\"11\">11</option><option value=\"12\">12</option><option value=\"13\">13</option><option value=\"14\">14</option><option value=\"15\">15</option><option value=\"16\">16</option><option value=\"17\">17</option><option value=\"18\">18</option><option value=\"19\">19</option><option value=\"20\">20</option><option value=\"21\">21</option><option value=\"22\">22</option><option value=\"23\">23</option><option value=\"24\">24</option><option value=\"25\">25</option><option value=\"26\">26</option><option value=\"27\">27</option><option value=\"28\">28</option><option value=\"29\">29</option><option value=\"30\">30</option><option value=\"31\">31</option></select></div></div><div class=\"small-12 medium-4 columns\"><div class=\"VLCLabel\">Year</div><div class=\"styled-select\"><select id=\"year\">").helper("yearDropdown",ctx,{"block":body_1},{"range":"-20"}).write("</select></div></div></div></form><div><a href='/Navigator/#").reference(ctx.getPath(false,["mainContent","0","records","0","attributes","answer-1-page-name"]),ctx,"h").write("' class=\"button base-btn main-btn right saveDate answer\">").reference(ctx.getPath(false,["mainContent","0","records","0","attributes","answer-1-button-text"]),ctx,"h").write("</a><a id=\"backButton\" class=\"button base-btn main-btn back js-void-href\" href=\"\"> Back<a/></div> ").partial("VLC/partial_specialContent",ctx,null).partial("VLC/partial_newTab_buttons",ctx,null).partial("VLC/partial_VLC_buttons",ctx,null);}function body_1(chk,ctx){return chk.write(" ");}return body_0;})(); });