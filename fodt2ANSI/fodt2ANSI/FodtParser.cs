using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using fodt2ANSI.Fodt;
using System.Text.RegularExpressions;

namespace fodt2ANSI
{
    public class FodtParser
    {
        public static Regex rgxSpaceElem = new Regex("<text:s text:c=\"\\d+?\"([^>]|)+?\\/>");

        public XDocument doc = null;
        public List<FodtStyle> styles = new List<FodtStyle>();

        public FodtParser(XDocument doc)
        {
            this.doc = doc;
        }

        public void ParseStyles()
        {
            try
            {
                foreach (XElement styleNode in doc.Descendants().Where(p => p.Name.LocalName == "automatic-styles").First()
                                       .Descendants().Where(p => p.Name.LocalName == "style").ToList())
                {
                    FodtStyle style = new FodtStyle();

                    try
                    {
                        style.StyleName = styleNode.Attributes().Where(x => x.Name.LocalName == "name").First().Value;
                    }
                    catch
                    {
                    }

                    string family = "";
                    try
                    {
                        family = styleNode.Attributes().Where(x => x.Name.LocalName == "family").First().Value;
                    }
                    catch
                    {
                    }
                    style.StyleFamily = (family == "text" || family == "paragraph") ? ((family == "text") ? FodtStyle.Family.text : FodtStyle.Family.paragraph) : FodtStyle.Family.unkown;

                    try
                    {
                        XElement propertiesNode = styleNode.Descendants().Where(p => p.Name.LocalName == "text-properties").First();

                        try
                        {
                            style.Colour = System.Drawing.ColorTranslator.FromHtml(propertiesNode.Attributes().Where(x => x.Name.LocalName == "color").First().Value);
                        }
                        catch
                        {
                        }

                        try
                        {
                            style.BackgroundColour = System.Drawing.ColorTranslator.FromHtml(propertiesNode.Attributes().Where(x => x.Name.LocalName == "background-color").First().Value);
                        }
                        catch
                        {
                        }
                    }
                    catch
                    {
                    }

                    //Console.WriteLine(style);
                    styles.Add(style);
                }
            }
            catch
            {
            }
        }

        public List<string> BuildANSI()
        {
            List<string> ret = new List<string>();
            XElement moo = doc.Elements().Where(p => p.Name.LocalName == "document").First()
                .Elements().Where(p => p.Name.LocalName == "body").First()
                .Elements().Where(p => p.Name.LocalName == "text").First();
            foreach (XElement p in moo.Elements().Where(p=>p.Name.LocalName == "p"))
            {
                FodtStyle pStyle = new FodtStyle();
                try
                {
                    pStyle = GetStyleByName(p.Attributes().Where(x => x.Name.LocalName == "style-name").First().Value);
                }
                catch { }
                string text = "";
                int na = 0;
                foreach (var node in p.DescendantNodes())
                {
                    if (na > 0)
                    {
                        //if (node.NodeType != XmlNodeType.Text)
                        //{
                        //    Console.ForegroundColor = ConsoleColor.Red;
                        //}
                        //if (node.ToString().EndsWith("/>"))
                        //{
                        //    Console.ForegroundColor = ConsoleColor.Cyan;
                        //}
                        //Console.WriteLine(node);
                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine("--------------------------------------------------------");
                        //Console.ResetColor();
                        text+=BuildText(node, pStyle);
                    }
                    na++;
                }
                //Console.WriteLine(text);


                //Console.WriteLine(pXml);
                //Console.WriteLine(pStyle);
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("////////////////////////////////////////////////////////////////////////////////////////////////////");
                //Console.ResetColor();
                ret.Add(text);
            }
            return ret;
        }

        public string BuildText(XNode node, FodtStyle inheritStyle)
        {
            //if (node.NodeType != XmlNodeType.Text)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //}
            //if (node.ToString().EndsWith("/>"))
            //{
            //    Console.ForegroundColor = ConsoleColor.Cyan;
            //}
            //Console.WriteLine(node);
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("--------------------------------------------------------");
            //Console.ResetColor();
            if (node.NodeType == XmlNodeType.Text)
            {
                return inheritStyle.GetANSI() + this.BashEscapeString(node.ToString())+inheritStyle.GetANSI();
            }
            else
            {
                if (node.ToString().EndsWith("/>"))
                {
                    try
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(' ', Int32.Parse(((XElement)node).Attributes().Where(x => x.Name.LocalName == "c").First().Value));
                        return inheritStyle.GetANSI() + sb.ToString()/*+inheritStyle.GetANSI()*/;
                    }
                    catch { }
                }
                else
                {
                    string ret = inheritStyle.GetANSI();
                    FodtStyle pStyle = new FodtStyle();
                    try
                    {
                        pStyle = GetStyleByName(((XElement)node).Attributes().Where(x => x.Name.LocalName == "style-name").First().Value);
                    }
                    catch { }
                    int na = 0;
                    foreach (var nodeNode in ((XElement)node).DescendantNodes())
                    {
                        if (na > 0)
                        {
                            ret += BuildText(nodeNode, pStyle);

                        }
                        na++;
                    }
                    ret += inheritStyle.GetANSI();
                }
            }
            return "";
        }

        public string BashEscapeString(string s)
        {
            return s.Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\v", "\\v")
                .Replace("\b", "\\b")
                .Replace("\a", "\\a")
                .Replace("\"", "\\\"")
                .Replace("$", "\\$");
        }

        public string GetInnerXml(XElement parent)
        {
            var reader = parent.CreateReader();
            reader.MoveToContent();

            return reader.ReadInnerXml();
        }

        public FodtStyle GetStyleByName(string name)
        {
            return styles.Where(x => x.StyleName == name).First();
        }
    }
}

