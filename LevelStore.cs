using Newtonsoft.Json;
using System.IO;

namespace monoGameCP
{

public class LevelStore{


    public LevelStore(){

    }

    public void storeLevelAsJSON(System.Collections.Generic.List<TileObject> listIn,string filename){

          var json = JsonConvert.SerializeObject(listIn);
          var path=@"C:\\Users\\Lowry\\monoGameCP\\"+filename;
          File.WriteAllText(path,json);
    }

     /* public void writeXMLElement(){
          TextWriter
              textWriter= new XmlTextWriter("C:\\Users\\Lowry\\monoGameCP\\level.xml", null) ;
             textWriter.WriteStartDocument();  
               textWriter.WriteComment("First Comment XmlTextWriter Sample Example");  
            textWriter.WriteComment("myXmlFile.xml in root dir");  
            // Write first element  
            textWriter.WriteStartElement("Tile");  
            textWriter.WriteStartElement("r", "RECORD", "urn:record");  
            // Write next element  
            textWriter.WriteStartElement("Name", "");  
            textWriter.WriteString("Student");  
            textWriter.WriteEndElement();  
            // Write one more element  
            textWriter.WriteStartElement("Address", "");  
            textWriter.WriteString("Colony");  
            textWriter.WriteEndElement();  
            // WriteChars  
            char[] ch = new char[3];  
            ch[0] = 'a';  
            ch[1] = 'r';  
            ch[2] = 'c';  
            textWriter.WriteStartElement("Char");  
            textWriter.WriteChars(ch, 0, ch.Length);  
            textWriter.WriteEndElement();  
            // Ends the document.  
            textWriter.WriteEndDocument();  
            // close writer  
            textWriter.Close();  
        }*/

}


}