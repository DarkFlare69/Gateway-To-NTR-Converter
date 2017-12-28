using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Gateway_To_NTR_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.button4.Click += new System.EventHandler(this.button4_Click);
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = theDialog.FileName;
                string[] filelines = File.ReadAllLines(filename);
                textBox1.Text = "";
                foreach (string line in filelines)
                {
                    textBox1.Text += line + System.Environment.NewLine;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) // conversions
        {
            textBox2.Text = "";
            String temp = null;
            string line = null;
            StringReader LineString = new StringReader(textBox1.Text);

            int Tabs = 0;

            bool loop = false;

            while (true)
            {
                line = LineString.ReadLine();
                if ((line == "") && (Tabs != 0))
                {
                    textBox2.Text += "}" + System.Environment.NewLine + System.Environment.NewLine;
                    Tabs--;
                }
                if (line == null)
                {
                    if (Tabs != 0)
                    {
                        textBox2.Text += "}" + System.Environment.NewLine + System.Environment.NewLine;
                        Tabs--;
                    }
                    break;
                }
                if (line.StartsWith("["))
                {
                    temp = line.Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero"); // add more exceptions
                    temp = temp.Replace("[", "void\t").Replace("]", "(void)" + System.Environment.NewLine + "{" + System.Environment.NewLine);
                    Tabs = 1;

                    textBox2.Text += temp;
                }
                if (line.StartsWith("0")) // 32 bit write
                {
                    var regex = new Regex(Regex.Escape("0"));
                    temp = regex.Replace(line, "WRITEU32(offset_+_0x", 1);
                    temp = temp.Replace(" ", ", 0x");
                    temp += ");";
                    string result = new String('\t', Tabs);
                    result += temp;
                    result += System.Environment.NewLine;
                    result = result.Replace("_", " ");
                    textBox2.Text += result;
                }
                if (line.StartsWith("1")) // 16 bit write
                {
                    var regex = new Regex(Regex.Escape("1"));
                    temp = regex.Replace(line, "WRITEU16(offset_+_0x", 1);
                    temp = temp.Replace(" ", ", 0x");
                    temp += ");";
                    string result = new String('\t', Tabs);
                    result += temp;
                    result += System.Environment.NewLine;
                    result = result.Replace("_", " ");
                    textBox2.Text += result;
                }
                if (line.StartsWith("2")) // 8 bit write
                {
                    var regex = new Regex(Regex.Escape("2"));
                    temp = regex.Replace(line, "WRITEU8(offset_+_0x", 1);
                    temp = temp.Replace(" ", ", 0x");
                    temp += ");";
                    string result = new String('\t', Tabs);
                    result += temp;
                    result += System.Environment.NewLine;
                    result = result.Replace("_", " ");
                    textBox2.Text += result;
                }
                if (line.StartsWith("DD000000 ")) // button activator
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DD000000 "));
                    temp = regex.Replace(line, "if (is_pressed(0x", 1);
                    result += temp;
                    result += "))" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("3")) // 32 bit greater Than
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") > 0x");
                    var regex = new Regex(Regex.Escape("3"));
                    temp = regex.Replace(line, "if (READU32(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("4")) // 32 bit less Than
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") < 0x");
                    var regex = new Regex(Regex.Escape("4"));
                    temp = regex.Replace(line, "if (READU32(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("5")) // 32 bit equal
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") == 0x");
                    var regex = new Regex(Regex.Escape("5"));
                    temp = regex.Replace(line, "if (READU32(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("6")) // 32 bit not equal
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") != 0x");
                    var regex = new Regex(Regex.Escape("6"));
                    temp = regex.Replace(line, "if (READU32(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("7")) // 16 bit greater
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") > 0x");
                    var regex = new Regex(Regex.Escape("7"));
                    temp = regex.Replace(line, "if (READU16(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("8")) // 16 bit less
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") < 0x");
                    var regex = new Regex(Regex.Escape("8"));
                    temp = regex.Replace(line, "if (READU16(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("9")) // 16 bit equal
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") == 0x");
                    var regex = new Regex(Regex.Escape("9"));
                    temp = regex.Replace(line, "if (READU16(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("A")) // 16 bit not equal
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ") != 0x");
                    var regex = new Regex(Regex.Escape("A"));
                    temp = regex.Replace(line, "if (READU16(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("B")) // pointer
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" 00000000", ");" + System.Environment.NewLine);
                    var regex = new Regex(Regex.Escape("B"));
                    temp = regex.Replace(line, "offset = READU32(offset + 0x", 1);
                    result += temp;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D3000000 ")) // set offset
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D3000000 "));
                    temp = regex.Replace(line, "offset = 0x", 1);
                    temp += ";" + System.Environment.NewLine;
                    result += temp;
                    textBox2.Text += result;
                }
                if (line.StartsWith("DC000000 ")) // add to offset
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DC000000 "));
                    temp = regex.Replace(line, "offset += 0x", 1);
                    temp += ";" + System.Environment.NewLine;
                    result += temp;
                    textBox2.Text += result;
                }
                if (line.StartsWith("C0000000")) // loop test
                {
                    loop = true;
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("C0000000 "));
                    temp = regex.Replace(line, "for (int i = 0; i < 0x", 1);
                    temp += "; i++)" + System.Environment.NewLine + result + "{" + System.Environment.NewLine;
                    Tabs++;
                    result += temp;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D0000000") && Tabs != 0) // terminator
                {
                    Tabs--;
                    string result = new String('\t', Tabs);
                    result += "}" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D1000000")) // execute loop again, work on later
                {
                    string result = new String('\t', Tabs);
                    result += "continue;" + System.Environment.NewLine;
                    textBox2.Text += result;
                }

                if (line.StartsWith("D2000000") && Tabs != 0) // terminator
                {
                    if (!loop) // do not reset offset during a loop
                    {
                        string result = new String('\t', Tabs);
                        result += "offset = 0;" + System.Environment.NewLine + result + "data = 0;" + System.Environment.NewLine;
                        textBox2.Text += result;
                    }
                    loop = false;
                    for (int i = 1; i < Tabs; i++)
                    {
                        string result = new String('\t', (Tabs - i));
                        result += "}" + System.Environment.NewLine;
                        textBox2.Text += result;
                    }
                    Tabs = 1;
                }
                if (line.StartsWith("D4000000")) // add to data register
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D4000000 "));
                    temp = regex.Replace(line, "data += 0x", 1);
                    result += temp + ";" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D5000000")) // set data register
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D5000000 "));
                    temp = regex.Replace(line, "data = 0x", 1);
                    result += temp + ";" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D6000000")) // write data register to memory 32 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D6000000 "));
                    temp = regex.Replace(line, "WRITEU32(offset + 0x", 1);
                    result += temp + ", data);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D7000000")) // write data register to memory 16 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D7000000 "));
                    temp = regex.Replace(line, "WRITEU16(offset + 0x", 1);
                    result += temp + ", data);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D8000000")) // write data register to memory 8 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D8000000 "));
                    temp = regex.Replace(line, "WRITEU8(offset + 0x", 1);
                    result += temp + ", data);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D9000000")) // load to data register 32 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D9000000 "));
                    temp = regex.Replace(line, "data = READU32(offset + 0x", 1);
                    result += temp + ");" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("DA000000")) // load to data register 16 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DA000000 "));
                    temp = regex.Replace(line, "data = READU16(offset + 0x", 1);
                    result += temp + ");" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("DB000000")) // load to data register 8 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DA000000 "));
                    temp = regex.Replace(line, "data = READU8(offset + 0x", 1);
                    result += temp + ");" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("E")) // memory patch, work on later
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("E"));
                    temp = regex.Replace(line, "unsigned int patch_address = 0x", 1);
                    String address = temp.Substring(0, (temp.Length - 9));
                    result += address + ";" + System.Environment.NewLine;
                    String value = line.Substring(9);
                    textBox2.Text += result;
                }
                if (line.StartsWith("F")) // memory copy
                {
                    line = line.Replace(" ", "), (void*)(offset)), 0x");
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("F"));
                    temp = regex.Replace(line, "memcpy((void *)(", 1);
                    result += temp + ");" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String readall = textBox2.Text;
            string line = null;
            StringReader LineString = new StringReader(readall);
            textBox2.Text = "";
            while (true)
            {
                line = LineString.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (line.Contains("0x0"))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        line = line.Replace("0x0", "0x");
                    }
                }
                if (line.Contains("0x)") || line.Contains("0x,") || line.Contains("0x;"))
                {
                    line = line.Replace("0x)", "0)").Replace("0x,", "0,").Replace("0x;", "0;");
                }
                textBox2.Text += line + System.Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                Clipboard.SetText(textBox2.Text);
                MessageBox.Show("Copied to Clipboard!");
            }
            else
            {
                MessageBox.Show("Output cannot be empty!");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String cheats_c = "#include \"cheats.h\"\n#include <stdbool.h>\n#include \"hid.h\"\n#include \"values.h\"\n#include <string.h>\n\nu32 offset = 0;\nu32 data = 0;\n\n";
            cheats_c += textBox2.Text;
            File.WriteAllText(System.Environment.CurrentDirectory + "/pluginMenu/Sources/cheats.c", cheats_c);
            String cheats_h = "#ifndef CHEATS_H\n#define CHEATS_H\n\n#include \"plugin.h\"\n\n";
            String create_menu_c = "#include \"cheats.h\"\n\n#define ENTRY_COUNT 300\n\ntypedef struct s_menu\n{\n\tint         count;\n\tint         status;\n\tu32         f[ENTRY_COUNT];\n\tu32         s[ENTRY_COUNT];\n\tint         a[ENTRY_COUNT];\n\tconst char  *t[ENTRY_COUNT];\n\tconst char  *n[ENTRY_COUNT];\n\tvoid        (*fp[ENTRY_COUNT])();\n}             t_menu;\n\n\ntypedef void    (*FuncPointer)(void);\nextern t_menu menu;\n\n\nvoid    new_super_unselectable_entry(char *str, FuncPointer function)\n{\n\tint index;\n\n\tindex = menu.count;\n\tif (index >= 300)\n\t\treturn;\n\tnew_unselectable_entry(str);\n\tmenu.f[index] |= BIT(0) | BIT(1);\n\tmenu.fp[index] = function;\n}\n\nvoid with_note_common(const char *name, const char *note, void (*cheatfunction)(void), int type)\n{\n\tint     index;\n\n\tif (type == 0)\n\t\tindex = new_entry((char *)name, cheatfunction);\n\telse if (type == 1)\n\t\tindex = new_radio_entry((char *)name, cheatfunction);\n\telse if (type == 2)\n\t\tindex = new_spoiler((char *)name);\n\telse return;\n\tset_note(note, index);\n}\n\ninline void new_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))\n{\n\twith_note_common(name, note, cheatfunction, 0);\n}\n\ninline void new_radio_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))\n{\n\twith_note_common(name, note, cheatfunction, 1);\n}\n\ninline void new_spoiler_with_note(const char *name, const char *note)\n{\n\twith_note_common(name, note, NULL, 2);\n}\n\nchar	*builder_name = \"" + textBox3.Text + "\";\n\n\t/* static const char * const code1 = \"This is a note for code1\";     #format: new_entry_with_note(\"Visible Code Name\", note_name, code_name); */\n\nvoid    my_menus(void)\n{\n";
            string line = null;
            StringReader LineString = new StringReader(textBox2.Text);
            while (true)
            {
                line = LineString.ReadLine();
                if (line == null)
                {
                    cheats_h += "\n#endif\n";
                    File.WriteAllText(System.Environment.CurrentDirectory + "/pluginMenu/Sources/cheats.h", cheats_h);

                    break;
                }
                if (line.StartsWith("void"))
                {
                    cheats_h += line + ";\n";
                }
            }
            LineString = new StringReader(textBox1.Text);
            while (true)
            {
                line = LineString.ReadLine();
                if (line == null)
                {
                    create_menu_c += "}\n";
                    File.WriteAllText(System.Environment.CurrentDirectory + "/pluginMenu/Sources/create_menu.c", create_menu_c);
                    break;
                }
                if (line.StartsWith("["))
                {
                    String originalTitle = line.Replace("[", "").Replace("]", "");
                    String newTitle = originalTitle.Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero");
                    String MenuEntry = "\tnew_entry(\"" + originalTitle + "\", " + newTitle + ");" + System.Environment.NewLine;
                    create_menu_c += MenuEntry;
                }
            }
            System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "/pluginMenu/build.bat");
        }

        private void nDSCodetypeReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://doc.kodewerx.org/hacking_nds.html");
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("All codes need to have a name, enclosed in [brackets]." + System.Environment.NewLine + System.Environment.NewLine + "If a code does not convert properly, please leave a comment on my YouTube video." + System.Environment.NewLine + System.Environment.NewLine + "The Code Checker will only check certain elements of your input to ensure the codes will convert properly. This assumes your input is still valid codes.", "Information");
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program by DarkFlare" + System.Environment.NewLine + "NTR CFW by cell9" + System.Environment.NewLine + "Blank Cheat Menu by Nanquitas", "Credits");
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void codeCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string line = null;
            StringReader LineString = new StringReader(textBox1.Text);

            int SpaceCount = 1;
            int BracketCount = 0;

            while (true)
            {
                line = LineString.ReadLine();
                if (line == null)
                {
                    if (BracketCount == SpaceCount)
                    {
                        MessageBox.Show("Your codes look good!", "Results");
                    }
                    else
                    {
                        MessageBox.Show("There appears to be a problem with your codes. Assuming you inputted actual codes, this is probably due to improper/missing titles or lack of space between codes. This also could be because you used a codetype that was not yet implemented.", "Results");
                    }
                    break;
                }
                if (line == "")
                {
                    SpaceCount++;
                }
                if (line.StartsWith("[") && line.Contains("]"))
                {
                    BracketCount++;
                }
                if (line.StartsWith("E"))
                {
                    BracketCount+= 500;
                }
            }
        }
    }
}
