/** Class Info
 * DictionaryHelper by Guyutongxue
 * For managing directory of marks.
 * Released under MIT License.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace copyfile2
{
    class DictionaryHelper
    {
        delegate void DictInputDelegate(List<string> args);

        Dictionary<string, Guid> dict = new Dictionary<string, Guid>();

        Dictionary<string, DictInputDelegate> dictInput = new Dictionary<string, DictInputDelegate>();

        string path, fatherpath;

        public DictionaryHelper(string xmlfilepath)
        {
            path = xmlfilepath;
            string[] dirs = path.Split('\\');
            int end = dirs[dirs.Length - 1].Length;
            fatherpath = path.Remove(path.Length - end);

            dictInput.Add("help", InputHelp);
            dictInput.Add("ls", InputLs);
            dictInput.Add("mv", InputMv);
            dictInput.Add("rm", InputRm);
        }

        /// <summary>
        /// Do dictionary's oprtation.
        /// </summary>
        /// <param name="args">Dictionary Helper Arguments.</param>
        public void Operate(List<string> args)
        {
            string output = string.Empty;
            Dictionary<string, Guid> backup = dict;
            try
            {
                if (args.Count == 0)
                {
                    Console.WriteLine("Dictionary Helper: Need command. Type 'dict help' for more info.");
                    return;
                }
                if (dictInput.ContainsKey(args[0])) dictInput[args[0]](args);
                else Console.WriteLine($"Dictinoary Helper: Command '{args[0]}' not found.");
            }
            catch (Exception ex)
            {
                dict = backup;
                Console.WriteLine($"Dictionary Helper: Error occured: {ex.Message}");
            }
        }

        #region DictInputFuncs

        void InputHelp(List<string> args)
        {
            Console.WriteLine(
@"Dictionary Helper Commands:
ls	view the dictionary
mv [-r] <OLD ALIAS> <NEW ALIAS>
	rename a pair by alias.
	-r	use regular expression in <OLD ALIAS>.
rm [-r] <ALIAS> [<ALIAS2> ...]
	remove a pair.
	-r	use regular expression.
help	show this message.");
        }

        void InputLs(List<string> args)
        {
            if (dict.Count == 0)
            {
                Console.WriteLine("(No data)");
                return;
            }
            foreach (var i in dict)
            {
                Console.WriteLine($"{i.Key}\t{i.Value}");
            }
        }

        void InputMv(List<string> args)
        {

            if (args[1][0] == '-')
            {
                if (args[1][1] == 'r')
                {

                    List<string> target = RegexMatch(args[2]);
                    switch (target.Count)
                    {
                        case 0:
                            {
                                Console.WriteLine($"No match for regex '{args[2]}'.");
                                break;
                            }
                        case 1:
                            {
                                Move(target[0], args[3]);
                                break;
                            }
                        default:
                            {
                                Console.WriteLine($"Too many matches of '{args[2]}'. Here are them:");
                                foreach (string j in target) Console.WriteLine(j);
                                break;
                            }
                    }

                }
                else if (args[1] == "--") Move(args[2], args[3]);
                else
                {
                    Console.WriteLine($"Error: Unknown option'{args[1]}'.");
                    return;
                }
            }
            else Move(args[1], args[2]);
        }



        void InputRm(List<string> args)
        {
            if (args[1][0] == '-')
            {
                if (args[1][1] == 'r')
                {
                    for (int i = 2; i < args.Count; i++)
                    {
                        List<string> target = RegexMatch(args[i]);
                        if (target.Count == 0)
                        {
                            Console.WriteLine($"No match for regex '{args[i]}'.");
                            break;
                        }
                        foreach (string j in target) Remove(j);
                    }

                }
                else if (args[1] == "--") for (int i = 2; i < args.Count; i++) Remove(args[i]);
                else
                {
                    Console.WriteLine($"Error: Unknown option'{args[1]}'.");
                    return;
                }
            }
            else for (int i = 1; i < args.Count; i++) Remove(args[i]);
        }

        #endregion

        void Move(string src, string dst)
        {
            Guid value = dict[src];
            dict.Remove(src);
            dict.Add(dst, value);
            WriteToFile();
            Console.WriteLine($"({src},{value}) -> ({dst},{value})");
            if (Directory.Exists(fatherpath + src))
            {
                Directory.Move(fatherpath + src, fatherpath + dst);
                Console.WriteLine("Folder moved.");
            }
        }

        void Remove(string src)
        {
            Guid value = dict[src];
            dict.Remove(src);
            WriteToFile();
            Console.WriteLine($"({src},{value}) removed.");
        }

        List<string> RegexMatch(string pattern)
        {
            Regex regex = new Regex('^' + pattern + '$');
            List<string> result = new List<string>();
            foreach (string i in dict.Keys)
            {
                if (regex.IsMatch(i))
                {
                    result.Add(i);
                }
            }
            return result;
        }
        

        /// <summary>
        /// Write the Dictionary to XML.
        /// </summary>
        public void WriteToFile()
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
            doc.Save(path);
        }

        /// <summary>
        /// Read the Dictionary from XML.
        /// </summary>
        public void ReadFromFile()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
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

        /// <summary>
        /// Analyse a certain drive's mark.
        /// </summary>
        /// <param name="drive">Drive's root directory.</param>
        /// <returns>Drive's Alias.</returns>
        public string Analyse(char drive)
        {
            if (!CopyManager.IsMarked(drive))
            {
                Guid guid = Guid.NewGuid();
                string alias = guid.ToString().Substring(0, 8);
                CopyManager.AddMark(drive, guid.ToString());
                dict.Add(alias, guid);
                WriteToFile();

                ConsoleHelper.EventWriteLine($"New Disk. Init in ({alias},{guid.ToString()}).");
                return alias;
            }
            else
            {
                string mark = CopyManager.GetMark(drive);
                foreach (var i in dict)
                {
                    if (i.Value.ToString().Equals(mark))
                    {
                        ConsoleHelper.EventWriteLine($"Found Disk of ({i.Key},{i.Value}).");
                        return i.Key;
                    }
                }
                string alias = mark.Substring(0, 8);
                dict.Add(alias, new Guid(mark));
                WriteToFile();

                ConsoleHelper.EventWriteLine($"New Disk but has mark. Init in ({alias},{mark}).");
                return alias;
            }
        }
    }
}
