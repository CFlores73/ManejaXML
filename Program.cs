using ManejaXML;
using System.Xml;

class Program
{
    public static void Main(String[] args)
    {
        XmlLINQ xmlLINQ = new XmlLINQ();
        xmlLINQ.ProcesarXML(@"C:/Temp/response1095.xml");
    }
}