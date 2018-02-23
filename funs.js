// IE8 cacl替代方案
$(".left-tab-content").css("height", "100%").css("height", "-=52px");

//匹配括号 
var substr = "ii(0000)ffr".match(/\((\S*)\)/);

//字符串转xml对象
var strXml = "<xml></xml>";
var xml = $.parseXML(strXml);
var value = xml.getElementsByTagName('BOT') && xml.getElementsByTagName('BOT').length > 0
    ? xml.getElementsByTagName('BOT')[0].getAttribute("LayerIds") : "";
