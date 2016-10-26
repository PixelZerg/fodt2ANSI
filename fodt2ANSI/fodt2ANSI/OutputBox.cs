using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fodt2ANSI
{
    public partial class OutputBox : Form
    {
        public OutputBox()
        {
            InitializeComponent();
        }

        public OutputBox(string text)
        {
            InitializeComponent();
            this.richTextBox1.Text = text;
        }
    }
}
