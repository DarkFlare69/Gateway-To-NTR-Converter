﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace Gateway_To_NTR_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool library;

        private void button4_Click(object sender, EventArgs e)
        {
            this.button4.Click += new System.EventHandler(this.button4_Click);
            OpenFileDialog dialogue = new OpenFileDialog();
            dialogue.Title = "Open Text File";
            dialogue.Filter = "TXT files|*.txt";
            if (dialogue.ShowDialog() == DialogResult.OK)
            {
                string filename = dialogue.FileName;
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
            library = true;
            textBox2.Text = "";
            String temp = "";
            String value = "";
            String total = "";
            String line = "";
            String current = "";
            StringReader LineString = new StringReader(textBox1.Text);
            int Tabs = 0;
            int j = 0;
            int e_code = 0;
            bool loop = false;
            while (true)
            {
                j++;
                String iteration = j.ToString();
                line = LineString.ReadLine();
                if (line == "" && Tabs != 0)
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
                if (e_code > 0) // patch codes (0xE)
                {
                    line = line.Replace(" ", "");

                    String start = "0x" + line.Remove(8, 8);
                    String end = "0x" + line.Substring(8);

                    int tmp = Convert.ToInt32(start, 16);
                    var reversedBytes = System.Net.IPAddress.NetworkToHostOrder(tmp);
                    start = reversedBytes.ToString("X").PadLeft(8, '0');

                    tmp = Convert.ToInt32(end, 16);
                    reversedBytes = System.Net.IPAddress.NetworkToHostOrder(tmp);
                    end = reversedBytes.ToString("X").PadLeft(8, '0');

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < start.Length; i++)
                    {
                        if (i % 2 == 0)
                            sb.Append(", 0x");
                        sb.Append(start[i]);
                    }
                    start = sb.ToString();

                    sb = new StringBuilder();
                    for (int i = 0; i < end.Length; i++)
                    {
                        if (i % 2 == 0)
                            sb.Append(", 0x");
                        sb.Append(end[i]);
                    }
                    end = sb.ToString();

                    if (!total.Contains("static const u8"))
                    {
                        current = iteration;
                        total = new String('\t', Tabs) + "static const u8 buffer" + current + "[] = {";
                    }
                    if (e_code != 0)
                    {
                        total += start;
                        e_code--;
                    }
                    if (e_code != 0)
                    {
                        total += end;
                        e_code--;
                    }
                    if (e_code < 1)
                    {
                        total += " };" + System.Environment.NewLine;
                        total = total.Replace("{,", "{");
                        textBox2.Text += total;
                        textBox2.Text += new String('\t', Tabs) + "memcpy((void *)(patch_address + offset), buffer" + current + ", " + value + ");" + System.Environment.NewLine;
                        total = "";
                    }
                    continue;
                }
                if (line.StartsWith("["))
                {
                    temp = line.Replace(",", "_").Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero"); // add more exceptions
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
                if (line.StartsWith("C0000000")) // loop
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
                /*if (line.StartsWith("D1000000") && loop) // execute loop again, work on later
                {
                    string result = new String('\t', Tabs);
                    result += "continue;" + System.Environment.NewLine;
                    textBox2.Text += result;
                    for (int i = 1; i < Tabs; i++)
                    {
                        result = new String('\t', (Tabs - i));
                        result += "}" + System.Environment.NewLine;
                        textBox2.Text += result;
                    }
                    Tabs = 1;
                }*/
                if (line.StartsWith("D1000000") && loop) // execute loop again
                {
                    string result = new String('\t', Tabs);
                    result += "continue;" + System.Environment.NewLine;
                    textBox2.Text += result;
                    result = new String('\t', (Tabs - 1));
                    result += "}" + System.Environment.NewLine;
                    textBox2.Text += result;
                    Tabs--;
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
                    var regex = new Regex(Regex.Escape("DB000000 "));
                    temp = regex.Replace(line, "data = READU8(offset + 0x", 1);
                    result += temp + ");" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("E")) // memory patch
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("E"));
                    temp = regex.Replace(line, "patch_address = 0x", 1);
                    String address = temp.Substring(0, (temp.Length - 9));
                    result += address + ";" + System.Environment.NewLine;
                    value = line.Substring(9);
                    value = "0x" + value;
                    e_code = Convert.ToInt32(value, 16);
                    e_code /= 4;
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
                if (line.Contains("0x)") || line.Contains("0x,") || line.Contains("0x;") || line.Contains("0x "))
                {
                    line = line.Replace("0x)", "0)").Replace("0x,", "0,").Replace("0x;", "0;").Replace("0x ", "0 ");
                }
                textBox2.Text += line + System.Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                Clipboard.SetText(textBox2.Text);
                MessageBox.Show("Copied to Clipboard!", "Copied");
            }
            else
            {
                MessageBox.Show("Output cannot be empty!", "Error");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String originalTitle = "";
            String newTitle = "";
            String create_menu_c = "";
            String notes = "";
            String cheats_c = "#include \"cheats.h\"\n#include <stdbool.h>\n#include \"hid.h\"\n#include \"values.h\"\n#include <string.h>\n\nu32 offset = 0;\nu32 data = 0;\nu32 patch_address = 0;\n\n";
            cheats_c += textBox2.Text;
            File.WriteAllText(System.Environment.CurrentDirectory + "/pluginMenu/Sources/cheats.c", cheats_c);
            String cheats_h = "#ifndef CHEATS_H\n#define CHEATS_H\n\n#include \"plugin.h\"\n\n";
            string line = null;
            StringReader LineString = new StringReader(textBox2.Text);
            if (!library)
            {
                string dirpath = Directory.GetCurrentDirectory();
                if (dirpath.Contains(" "))
                {
                    MessageBox.Show("The CTRPF builder cannot run when a base directory contains a space in it's name." + System.Environment.NewLine + System.Environment.NewLine + "Your current directory is:" + System.Environment.NewLine + dirpath, "Error");
                    return;
                }
                cheats_c = "#include \"cheats.hpp\"\nu32 offset = 0;\nu32 data32 = 0;\nu8 data8 = 0;\nu16 data16 = 0;\nu32 patch_address = 0;\nu16 cmp16 = 0;\nu32 cmp32 = 0;\n\nnamespace CTRPluginFramework\n{\n\n";
                cheats_c += textBox2.Text + "}";
                File.WriteAllText(System.Environment.CurrentDirectory + "/ctrpfMenu/Sources/cheats.cpp", cheats_c);
                cheats_h = "#ifndef CHEATS_H\n#define CHEATS_H\n\n#include \"CTRPluginFramework.hpp\"\n#include \"Helpers.hpp\"\n#include \"Unicode.h\"\n\nnamespace CTRPluginFramework\n{\n";
                while (true)
                {
                    line = LineString.ReadLine();
                    if (line == null)
                    {
                        cheats_h += "}\n\n#endif\n";
                        String version = textBox5.Text.Replace(".", ", ");
                        create_menu_c = "#include \"cheats.hpp\"\n\nnamespace CTRPluginFramework\n{\n\tvoid\tPatchProcess(FwkSettings &settings) { }\n\n\tint\tmain()\n\t{\n\t\tPluginMenu *menu = new PluginMenu(\"" + textBox4.Text + "\", " + version + ");\n\t\tmenu->SynchronizeWithFrame(true);\n\t\tMenuFolder *folder = nullptr, *subfolder = nullptr, *subsubfolder = nullptr;\n\n";
                        File.WriteAllText(System.Environment.CurrentDirectory + "/ctrpfMenu/Includes/cheats.hpp", cheats_h);

                        break;
                    }
                    if (line.StartsWith("void"))
                    {
                        cheats_h += "\t" + line + ";\n";
                    }
                }
                LineString = new StringReader(textBox1.Text);
                int i = 0;
                int j = 0;
                String currentFolder = "menu";
                List<String> folderNames = new List<String>();
                folderNames.Add("menu");
                while (true) // only thing left. folders dont work. neither do notes. but everything else does
                {
                    line = LineString.ReadLine();
                    if (line == null)
                    {
                        create_menu_c = create_menu_c.Replace("menu->SynchronizeWithFrame(true);", "menu->SynchronizeWithFrame(true);" + System.Environment.NewLine + System.Environment.NewLine + notes);
                        create_menu_c += "\n\t\tmenu->Run();\n\t\treturn 0;\n\t}\n}\n";
                        File.WriteAllText(System.Environment.CurrentDirectory + "/ctrpfMenu/Sources/main.cpp", create_menu_c);
                        break;
                    }
                    if (line.StartsWith("["))
                    {
                        originalTitle = line.Replace("[", "").Replace("]", "");
                        newTitle = originalTitle.Replace(",", "_").Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero");
                        String MenuEntry = "\t\t" + currentFolder + "->Append(new MenuEntry(\"" + originalTitle + "\", " + newTitle + ")); " + System.Environment.NewLine;
                        create_menu_c += MenuEntry;
                    }
                    if (line.StartsWith("+"))
                    {
                        i++;
                        j++;
                        currentFolder = "folder" + i.ToString();
                        folderNames.Add(currentFolder);
                        line = line.Replace("+", "\t\t" + currentFolder + " = new MenuFolder(\"") + "\");" + System.Environment.NewLine;
                        create_menu_c += line;
                    }
                    if (line.StartsWith("-"))
                    {
                        line = "\t\t" + folderNames[j - 1] + "->Append(" + folderNames[j] + ");" + System.Environment.NewLine;
                        j--;
                        create_menu_c += line;
                    }
                    if (line.StartsWith("*"))
                    {
                        notes += "\t\tstd::string " + newTitle + "_note = \"" + line.Replace("*", "") + "\";" + System.Environment.NewLine;
                        create_menu_c = create_menu_c.Replace("Append(new MenuEntry(\"" + originalTitle + "\", " + newTitle + "))", "Append(new MenuEntry(\"" + originalTitle + "\", " + newTitle + ", " + newTitle + "_note))");
                    }
                }
                System.Diagnostics.Process.Start(System.Environment.CurrentDirectory + "/ctrpfMenu/build.bat");
                return;
            }
            while (true)
            {
                line = LineString.ReadLine();
                if (line == null)
                {
                    cheats_h += "\n#endif\n";
                    create_menu_c = "#include \"cheats.h\"\n\n#define ENTRY_COUNT 300\n\ntypedef struct s_menu\n{\n\tint         count;\n\tint         status;\n\tu32         f[ENTRY_COUNT];\n\tu32         s[ENTRY_COUNT];\n\tint         a[ENTRY_COUNT];\n\tconst char  *t[ENTRY_COUNT];\n\tconst char  *n[ENTRY_COUNT];\n\tvoid        (*fp[ENTRY_COUNT])();\n}             t_menu;\n\n\ntypedef void    (*FuncPointer)(void);\nextern t_menu menu;\n\n\nvoid    new_super_unselectable_entry(char *str, FuncPointer function)\n{\n\tint index;\n\n\tindex = menu.count;\n\tif (index >= 300)\n\t\treturn;\n\tnew_unselectable_entry(str);\n\tmenu.f[index] |= BIT(0) | BIT(1);\n\tmenu.fp[index] = function;\n}\n\nvoid with_note_common(const char *name, const char *note, void (*cheatfunction)(void), int type)\n{\n\tint     index;\n\n\tif (type == 0)\n\t\tindex = new_entry((char *)name, cheatfunction);\n\telse if (type == 1)\n\t\tindex = new_radio_entry((char *)name, cheatfunction);\n\telse if (type == 2)\n\t\tindex = new_spoiler((char *)name);\n\telse return;\n\tset_note(note, index);\n}\n\nvoid new_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))\n{\n\twith_note_common(name, note, cheatfunction, 0);\n}\n\nvoid new_radio_entry_with_note(const char *name, const char *note, void (*cheatfunction)(void))\n{\n\twith_note_common(name, note, cheatfunction, 1);\n}\n\nvoid new_spoiler_with_note(const char *name, const char *note)\n{\n\twith_note_common(name, note, NULL, 2);\n}\n\nchar	*builder_name = \"" + textBox3.Text + "\";\n\n\nvoid\tmy_menus(void)\n{\n";
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
                    create_menu_c = create_menu_c.Replace("char	*builder_name = \"" + textBox3.Text + "\";\n\n", "char	*builder_name = \"" + textBox3.Text + "\";\n\n" + notes);
                    create_menu_c += "}\n";
                    File.WriteAllText(System.Environment.CurrentDirectory + "/pluginMenu/Sources/create_menu.c", create_menu_c);
                    break;
                }
                if (line.StartsWith("["))
                {
                    originalTitle = line.Replace("[", "").Replace("]", "");
                    newTitle = originalTitle.Replace(",", "_").Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero");
                    String MenuEntry = "\tnew_entry(\"" + originalTitle + "\", " + newTitle + ");" + System.Environment.NewLine;
                    create_menu_c += MenuEntry;
                }
                if (line.StartsWith("+"))
                {
                    line = line.Replace("+", "\tnew_spoiler(\"") + "\");" + System.Environment.NewLine;
                    create_menu_c += line;
                }
                if (line.StartsWith("-"))
                {
                    line = "\texit_spoiler();" + System.Environment.NewLine;
                    create_menu_c += line;
                }
                if (line.StartsWith("*"))
                {
                    notes += "\tstatic const char * const " + newTitle + "_note = \"" + line.Replace("*", "") + "\";" + System.Environment.NewLine;
                    create_menu_c = create_menu_c.Replace("new_entry(\"" + originalTitle + "\", ", "new_entry_with_note(\"" + originalTitle + "\", " + newTitle + "_note, ");
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
            MessageBox.Show("Version 2.0.5" + System.Environment.NewLine + System.Environment.NewLine +  "All codes need to have a name, enclosed in [brackets]." + System.Environment.NewLine + System.Environment.NewLine + "Menu Formatting:\n\t+Create a spoiler with this name (plus)\n\t-Close most recent spoiler (minus)\n\t*Note for the above code (asterisk; must be one line)" + System.Environment.NewLine + System.Environment.NewLine + "If a code does not convert properly, please leave a comment on my YouTube video." + System.Environment.NewLine + System.Environment.NewLine + "The Code Checker will only check certain elements of your input to ensure the codes will convert properly. This assumes your input is still valid codes.", "Information");
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program by DarkFlare" + System.Environment.NewLine + "NTR CFW by cell9" + System.Environment.NewLine + "Blank Cheat Menu by Nanquitas", "Credits");
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void codeCheckerToolStripMenuItem_Click(object sender, EventArgs e) // improve this
        {
            String line = "";
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
                        MessageBox.Show("There appears to be a problem with your codes. Assuming you inputted actual codes, this is probably due to improper/missing titles or lack of space between codes.", "Results");
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
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/DarkFlare69/Gateway-To-NTR-Converter/raw/master/Gateway%20To%20NTR%20Converter/bin/Debug/Gateway%20To%20NTR%20Converter.exe", "Gateway To NTR Converter-latest.exe");
                MessageBox.Show("The latest update has just been downloaded! You can close this application and delete it. Run the 'Gateway To NTR Converter-latest.exe' file alongside this one.", "Finished");
            }
        }

        public String  ReturnButtonActivator(String input)
        {
            int button = Convert.ToInt32("0x" + input, 16);
            String final = "if (Controller::IsKeyDown(Key::";
            int count = 0;
            int[] buttons = { 0x800, 0x400, 0x200, 0x100, 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };
            String[] buttonNames = { "Y", "X", "L", "R", "Down", "Up", "Left", "Right", "Start", "Select", "B", "A" };
            for (int i = 0; i < 12; i++)
            {
                if (buttons[i] <= button)
                {
                    count++;
                    button -= buttons[i];
                    if (count > 1)
                    {
                        final = final.Replace("IsKeyDown(Key::", "IsKeysDown(");
                        final += " + " + buttonNames[i];
                    }
                    else
                    {
                        final += buttonNames[i];
                    }
                }
            }
            return (final + "))");
        }

        private void button6_Click(object sender, EventArgs e) // conversions for ctrpf
        {
            library = false;
            textBox2.Text = "";
            String temp = "";
            String value = "";
            String total = "";
            String line = "";
            String current = "";
            StringReader LineString = new StringReader(textBox1.Text);
            int Tabs = 0;
            int j = 0;
            int e_code = 0;
            bool loop = false;
            int register = 0;
            while (true)
            {
                j++;
                String iteration = j.ToString();
                line = LineString.ReadLine();
                if (line == "" && Tabs != 0)
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
                if (e_code > 0) // patch codes (0xE)
                {
                    line = line.Replace(" ", "");

                    String start = "0x" + line.Remove(8, 8);
                    String end = "0x" + line.Substring(8);

                    int tmp = Convert.ToInt32(start, 16);
                    var reversedBytes = System.Net.IPAddress.NetworkToHostOrder(tmp);
                    start = reversedBytes.ToString("X").PadLeft(8, '0');

                    tmp = Convert.ToInt32(end, 16);
                    reversedBytes = System.Net.IPAddress.NetworkToHostOrder(tmp);
                    end = reversedBytes.ToString("X").PadLeft(8, '0');

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < start.Length; i++)
                    {
                        if (i % 2 == 0)
                            sb.Append(", 0x");
                        sb.Append(start[i]);
                    }
                    start = sb.ToString();

                    sb = new StringBuilder();
                    for (int i = 0; i < end.Length; i++)
                    {
                        if (i % 2 == 0)
                            sb.Append(", 0x");
                        sb.Append(end[i]);
                    }
                    end = sb.ToString();

                    if (!total.Contains("static const u8"))
                    {
                        current = iteration;
                        total = new String('\t', Tabs) + "static const u8 buffer" + current + "[] = {";
                    }
                    if (e_code != 0)
                    {
                        total += start;
                        e_code--;
                    }
                    if (e_code != 0)
                    {
                        total += end;
                        e_code--;
                    }
                    if (e_code < 1)
                    {
                        total += " };" + System.Environment.NewLine;
                        total = total.Replace("{,", "{");
                        textBox2.Text += total;
                        textBox2.Text += new String('\t', Tabs) + "memcpy((void *)(patch_address + offset), buffer" + current + ", " + value + ");" + System.Environment.NewLine;
                        total = "";
                    }
                    continue;
                }
                if (line.StartsWith("["))
                {
                    temp = line.Replace(",", "_").Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("+", "_").Replace("+", "_").Replace("-", "_").Replace(".", "_").Replace("1", "One").Replace("2", "Two").Replace("3", "Three").Replace("4", "Four").Replace("5", "Five").Replace("6", "Six").Replace("7", "Seven").Replace("8", "Eight").Replace("9", "Nine").Replace("0", "Zero"); // add more exceptions
                    temp = temp.Replace("[", "void\t").Replace("]", "(MenuEntry *entry)" + System.Environment.NewLine + "{" + System.Environment.NewLine);
                    Tabs = 1;
                    textBox2.Text += temp;
                }
                if (line.StartsWith("0")) // 32 bit write
                {
                    var regex = new Regex(Regex.Escape("0"));
                    temp = regex.Replace(line, "Process::Write32(offset_+_0x", 1);
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
                    temp = regex.Replace(line, "Process::Write16(offset_+_0x", 1);
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
                    temp = regex.Replace(line, "Process::Write8(offset_+_0x", 1);
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
                    line = line.Replace("DD000000 00000", "");
                    line = result + ReturnButtonActivator(line);
                    Tabs++;
                    line += System.Environment.NewLine + result + "{" + System.Environment.NewLine;
                    textBox2.Text += line;
                }
                if (line.StartsWith("3")) // 32 bit greater Than
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ", cmp32) && cmp32 > 0x");
                    var regex = new Regex(Regex.Escape("3"));
                    temp = regex.Replace(line, "if (Process::Read32(offset + 0x", 1);
                    result += temp;
                    result += ")" + System.Environment.NewLine;
                    result += new String('\t', Tabs);
                    result += "\t{" + System.Environment.NewLine;
                    Tabs++;
                    textBox2.Text += result;
                }
                if (line.StartsWith("4")) // 32 bit less Than
                {
                    string result = new String('\t', Tabs);
                    line = line.Replace(" ", ", cmp32) && cmp32 < 0x");
                    var regex = new Regex(Regex.Escape("4"));
                    temp = regex.Replace(line, "if (Process::Read32(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp32) && cmp32 == 0x");
                    var regex = new Regex(Regex.Escape("5"));
                    temp = regex.Replace(line, "if (Process::Read32(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp32) && cmp32 != 0x");
                    var regex = new Regex(Regex.Escape("6"));
                    temp = regex.Replace(line, "if (Process::Read32(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp16) && cmp16 > 0x");
                    var regex = new Regex(Regex.Escape("7"));
                    temp = regex.Replace(line, "if (Process::Read16(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp16) && cmp16 < 0x");
                    var regex = new Regex(Regex.Escape("8"));
                    temp = regex.Replace(line, "if (Process::Read16(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp16) && cmp16 == 0x");
                    var regex = new Regex(Regex.Escape("9"));
                    temp = regex.Replace(line, "if (Process::Read16(offset + 0x", 1);
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
                    line = line.Replace(" ", ", cmp16) && cmp16 != 0x");
                    var regex = new Regex(Regex.Escape("A"));
                    temp = regex.Replace(line, "if (Process::Read16(offset + 0x", 1);
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
                    line = line.Replace(" 00000000", ", offset);" + System.Environment.NewLine);
                    var regex = new Regex(Regex.Escape("B"));
                    temp = regex.Replace(line, "Process::Read32(offset + 0x", 1);
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
                /*if (line.StartsWith("D1000000")) // execute loop again, work on later
                {
                    string result = new String('\t', Tabs);
                    result += "continue;" + System.Environment.NewLine;
                    textBox2.Text += result;
                    for (int i = 1; i < Tabs; i++)
                    {
                        result = new String('\t', (Tabs - i));
                        result += "}" + System.Environment.NewLine;
                        textBox2.Text += result;
                    }
                    Tabs = 1;
                }*/
                if (line.StartsWith("D1000000") && loop) // execute loop again
                {
                    string result = new String('\t', Tabs);
                    result += "continue;" + System.Environment.NewLine;
                    textBox2.Text += result;
                    result = new String('\t', (Tabs - 1));
                    result += "}" + System.Environment.NewLine;
                    textBox2.Text += result;
                    Tabs--;
                }
                if (line.StartsWith("D2000000") && Tabs != 0) // terminator
                {
                    if (!loop) // do not reset offset during a loop
                    {
                        string result = new String('\t', Tabs);
                        result += "offset = 0;" + System.Environment.NewLine + result + "data32 = 0;" + System.Environment.NewLine + result + "data16 = 0;" + System.Environment.NewLine + result + "data8 = 0;" + System.Environment.NewLine;
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
                    temp = regex.Replace(line, "data" + register.ToString() + " += 0x", 1);
                    result += temp + ";" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D5000000")) // set data register
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D5000000 "));
                    temp = regex.Replace(line, "data32 = 0x", 1);
                    result += temp + ";" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D6000000")) // write data register to memory 32 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D6000000 "));
                    temp = regex.Replace(line, "Process::Write32(offset + 0x", 1);
                    result += temp + ", data32);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D7000000")) // write data register to memory 16 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D7000000 "));
                    temp = regex.Replace(line, "Process::Write16(offset + 0x", 1);
                    result += temp + ", data16);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D8000000")) // write data register to memory 8 bit
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D8000000 "));
                    temp = regex.Replace(line, "Process::Write8(offset + 0x", 1);
                    result += temp + ", data8);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("D9000000")) // load to data register 32 bit
                {
                    register = 32;
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("D9000000 "));
                    temp = regex.Replace(line, "Process::Read32(offset + 0x", 1);
                    result += temp + ", data32);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("DA000000")) // load to data register 16 bit
                {
                    register = 16;
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DA000000 "));
                    temp = regex.Replace(line, "Process::Read16(offset + 0x", 1);
                    result += temp + ", data16);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("DB000000")) // load to data register 8 bit
                {
                    register = 8;
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("DB000000 "));
                    temp = regex.Replace(line, "Process::Read8(offset + 0x", 1);
                    result += temp + ", data8);" + System.Environment.NewLine;
                    textBox2.Text += result;
                }
                if (line.StartsWith("E")) // memory patch
                {
                    string result = new String('\t', Tabs);
                    var regex = new Regex(Regex.Escape("E"));
                    temp = regex.Replace(line, "patch_address = 0x", 1);
                    String address = temp.Substring(0, (temp.Length - 9));
                    result += address + ";" + System.Environment.NewLine;
                    value = line.Substring(9);
                    value = "0x" + value;
                    e_code = Convert.ToInt32(value, 16);
                    e_code /= 4;
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
    }
}
