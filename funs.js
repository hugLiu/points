// IE8 cacl�������
$(".left-tab-content").css("height", "100%").css("height", "-=52px");

//ƥ������ 
var substr = "ii(0000)ffr".match(/\((\S*)\)/);

//�ַ���תxml����
var strXml = "<xml></xml>";
var xml = $.parseXML(strXml);
var value = xml.getElementsByTagName('BOT') && xml.getElementsByTagName('BOT').length > 0
    ? xml.getElementsByTagName('BOT')[0].getAttribute("LayerIds") : "";
