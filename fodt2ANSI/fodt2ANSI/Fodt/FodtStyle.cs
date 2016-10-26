using System;
using System.Drawing;

namespace fodt2ANSI.Fodt
{
    public class FodtStyle
    {
        public static bool useTrueColour = false;

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

        public string GetANSI()
        {
            if (useTrueColour)
            {
                return ((this.BackgroundColour != null) ? "\\x1b[48;2;" + this.BackgroundColour.Value.R + ";" + this.BackgroundColour.Value.G + ";" + this.BackgroundColour.Value.B + "m" : "")
                    + ((this.Colour != null) ? "\\x1b[38;2;" + this.Colour.Value.R + ";" + this.Colour.Value.G + ";" + this.Colour.Value.B + "m" : "");
            }
            else
            {

                return ((this.BackgroundColour != null) ? "\\x1b[48;5;" + BashColour.ClosestBash(this.BackgroundColour.Value) + "m" : "")
                    + ((this.Colour != null) ? "\\x1b[38;5;" + BashColour.ClosestBash(this.Colour.Value) + "m" : "");
            }
        }
    }
}

