using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using col = System.Drawing.Color;

namespace fodt2ANSI.Fodt
{
    public class BashColour
    {
        public static int ClosestBash(System.Drawing.Color c)
        {
            int code = 0;
            int nearestcode = 0;
            double nearestcloseness = int.MaxValue;
            foreach (col col in list)
            {
                double closeness = (Math.Abs(col.R - c.R)) + (Math.Abs(col.B - c.B)) + (Math.Abs(col.G - c.G));
                if (closeness == 0d)
                {
                    return code;
                }
                if (closeness < nearestcloseness)
                {
                    nearestcloseness = closeness;
                    nearestcode = code;
                }
                code++;
            }
            return nearestcode;
        }

        public BashColour()
        {
            if (list.Count <= 0) serialize();
        }
        public BashColour(int _code, int _charno, bool _background = false)
        {
            if (list.Count <= 0) serialize();
            //lineno = _lineno;
            code = _code;
            background = _background;
            charno = _charno;
        }
        public static List<System.Drawing.Color> list = new List<System.Drawing.Color>();
        private static int[] additions = new int[] { 0, 95, 40, 40, 40, 10 };
        public static void serialize(bool force = false)
        {
            if (list.Count > 0 && !force)
            {
                return;
            }
            list.AddRange(new System.Drawing.Color[] { col.FromArgb(0,0,0),
            col.FromArgb(191,0,0),col.FromArgb(0,191,0),col.FromArgb(191,191,0),
            col.FromArgb(0,0,191),col.FromArgb(191,0,191),col.FromArgb(0,191,191),
            col.FromArgb(191,191,191),col.FromArgb(64,64,64),col.FromArgb(225,64,64),

            col.FromArgb(64,225,64),col.FromArgb(225,225,64),col.FromArgb(96,96,225)
        ,col.FromArgb(225,64,225),col.FromArgb(64,225,225),col.FromArgb(225,225,225)});
            int r = 0;
            for (int rindex = 0; rindex < 6; rindex++)
            {
                r += additions[rindex];
                int g = 0;
                for (int gindex = 0; gindex < 6; gindex++)
                {
                    g += additions[gindex];
                    int b = 0;
                    for (int bindex = 0; bindex < 6; bindex++)
                    {
                        b += additions[bindex];
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
                        list.Add(c);
                    }
                }
            }
        }
        //public int lineno = 0;
        public int charno = 0;
        public int code = 0;
        public bool background = false;
        public override string ToString()
        {
            string ret = @"\e[";
            if (background) ret += "48";
            else ret += "38";
            ret += ";5;" + code + "m";
            return ret;
        }
        public System.Drawing.Color ToColor()
        {
            int no = code;
            again:
            try
            {
                return list[no];
            }
            catch
            {
                no -= list.Count;
                goto again;
            }
        }
    }
}
