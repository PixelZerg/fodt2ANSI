using System;
using System.Drawing;

namespace fodt2ANSI.Fodt
{
    public class FodtStyle
    {

        public enum Family
        {
            paragraph,
            text,
            unkown
        }

        public string StyleName = "";
        public Family StyleFamily = Family.unkown;
        public Color? Colour = null;
        public Color? BackgroundColour = null;

        public FodtStyle()
        {
        }

        public override string ToString()
        {
            return "FodtStyle =>" + Environment.NewLine
            + "\tStyleName: " + this.StyleName + Environment.NewLine
            + "\tStyleFamily: " + this.StyleFamily + Environment.NewLine
            + "\tColour: " + this.Colour.ToString() + Environment.NewLine
            + "\tBackgroundColour: " + this.BackgroundColour.ToString() + Environment.NewLine;
        }
    }
}

