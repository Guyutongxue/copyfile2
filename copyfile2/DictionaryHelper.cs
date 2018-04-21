using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace copyfile2
{
    class DictionaryHelper
    {

        public Dictionary<string, Guid> dict = new Dictionary<string, Guid>();

        public DictionaryHelper()
        {

        }

        public void Add(string key, Guid value)
        {
            dict.Add(key, value);
        }

        public string Operate(string[] cmds)
        {
            string output = string.Empty;
            try
            {
                switch (cmds[1])
                {
                    case "help":
                        {
                            output = @"
DictionaryHelper Commands:
view: View the dictionary
mv <OLD ALIAS> <NEW ALIAS>: Rename a pair by alias.
rm <ALIAS>: Remove a pair.
help: Print this message.";
                            break;
                        }
                    case "view":
                        {

                            foreach (var i in dict)
                            {
                                output += $"{i.Key}\t{i.Value}\n";
                            }
                            output = output == string.Empty ? "(No data)" : output.Remove(output.Length - 1);
                            break;
                        }
                    case "mv":
                        {
                            Guid value = dict[cmds[2]];
                            output += $"({cmds[2]},{value})";
                            dict.Remove(cmds[2]);
                            dict.Add(cmds[3], value);
                            output += $" -> ({cmds[3]},{value})";
                            break;
                        }
                    case "rm":
                        {
                            Guid value = dict[cmds[2]];
                            dict.Remove(cmds[2]);
                            output = $"({cmds[2]},{value}) removed.";
                            break;
                        }
                    default:
                        {
                            output = $"Dictionary Helper: Command {cmds[1]} not found.";
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                output = $"Dictionary Helper: Error occured: {ex.Message}";
            }
            return output;
        }

        public void WriteToFile(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = doc.CreateElement("cfmks");
            doc.AppendChild(root);
            foreach (var vp in dict)
            {
                XmlNode node = doc.CreateElement("cfmk");
                XmlElement eleAlias = doc.CreateElement("alias");
                eleAlias.InnerText = vp.Key;
                node.AppendChild(eleAlias);
                XmlElement eleGuid = doc.CreateElement("guid");
                eleGuid.InnerText = vp.Value.ToString();
                node.AppendChild(eleGuid);
                root.AppendChild(node);
            }
            doc.Save(filepath);
        }

        public void ReadFromFile(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filepath);
                XmlElement root = doc.DocumentElement;
                XmlNodeList list = root.SelectNodes("cfmk");
                for (int i = 0; i < list.Count; i++)
                {
                    dict.Add(list.Item(i).SelectSingleNode("./alias").InnerText, new Guid(list.Item(i).SelectSingleNode("./guid").InnerText));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when reading dictionary: {ex.Message}");
            }
        }
    }
}
