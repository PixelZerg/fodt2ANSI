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
                FodtStyle pStyle = GetStyleByName(p.Attributes().Where(x => x.Name.LocalName == "style-name").First().Value);
                string xml = GetInnerXml(p);

                //explode spaces
                var spaces = rgxSpaceElem.Matches(xml);
                int off = 0;
                foreach (Match space in spaces)
                {
                    try
                    {
                        int no = Int32.Parse(Parsing.GetBetween(space.Value, "\"", "\""));
                        xml = xml.Remove(space.Index + off, space.Length);
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append(' ', no);
                        xml = xml.Insert(space.Index + off, sb.ToString());
                        off -= space.Length - no;
                    }
                    catch
                    {
                    }
                }

                Console.WriteLine(xml);
                Console.WriteLine(pStyle);
                Console.WriteLine("------------------------------");
            }
            return ret;
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

