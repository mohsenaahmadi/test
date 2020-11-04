using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string OutPut = "start transaction;\n";
        private void button1_Click(object sender, EventArgs e)
        {
            bool readheader = false;
            string outs = OutPut;
            string outs2 = OutPut;
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = false;
            od.Filter = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml|All files(*.*)|*.*";
            string path = string.Empty;
            List<string> st = new List<string>();
            if (od.ShowDialog() == DialogResult.OK)
            {
                path = od.FileName;
                button2.Enabled = true;
            }
            using (StreamReader CSV = new StreamReader(path))
            {
                while (!CSV.EndOfStream)
                {
                    st.Add(CSV.ReadLine());
                }
            }
            for (int i = 1; i < st.Count; i++)
            {
                string[] temp = st[i].Split(',');
                 for (int k = 0; k < 3; k++)
                {
                    if (k == 0)
                    {
                        outs += "add bus \"eService Object Generator\" \"" + ConvertToRegisteredName(temp[0]) + "\" \""+temp[7].ToString() + "\" vault \"eService Administration\" policy \"eService Object Generator\" \"eService Safety Vault\" \""+ temp[1].ToString() + "\" \"eService Retry Delay\" \""+ temp[2].ToString() +"\" \"eService Retry Count\" \"" + temp[3].ToString() + "\" \"eService Processing Time Limit\" \"" + temp[4].ToString()+"\" \"eService Name Prefix\" \"" + temp[5].ToString() + "\";\n";
                    }
                    else if (k == 1)
                    {
                        outs += "add bus \"eService Number Generator\" \"" + ConvertToRegisteredName(temp[0]) + "\" \"" + temp[7].ToString() + "\" vault \"eService Administration\" policy \"eService Object Generator\" \"eService Next Number\" \""+ temp[6].ToString() + "\";\n";
                    }
                    else
                    {
                        outs += "add connection \"eService Number Generator\" from \"eService Object Generator\" \"" + ConvertToRegisteredName(temp[0]) + "\" \"" + temp[7].ToString() + "\" to \"eService Number Generator\" \"" + ConvertToRegisteredName(temp[0]) +"\" \"" + temp[7].ToString() + "\" ;\n";
                    }
                }
                outs2 += "modify bus \"eService Number Generator\" \"" + ConvertToRegisteredName(temp[0]) + "\" \""+ temp[7].ToString()+"\" \"eService Next Number\" 000001;\n";
            }
            #region Last Query
            /*int Dev = 0, Porto = 0, type = 0, revis = 0, name = 0 ;
            bool readheader = false;
            for (int j = 0; j < st.Count; j++)
            {
                if (st[j] != "")
                {
                   List<string> s = new List<string>();
                   foreach (string u in st[j].Split(','))
                       s.Add(u);
                    if (!readheader)
                    {
                        Dev = s.IndexOf("Development Cost");
                        Porto = s.IndexOf("Portotype Cost");
                        name = s.IndexOf("Name");
                        type = s.IndexOf("Type");
                        revis = s.IndexOf("Revision");
                        readheader = true;

                    }
                    else
                    {
                        int sum = int.Parse(s[Dev]) + int.Parse(s[Porto]);
                        outs += format(s[type], s[name], s[revis], sum, true)+"\n";
                        if (sum>100)
                            outs += format(s[type], s[name], s[revis], 1, false);
                        else
                            outs += format(s[type], s[name], s[revis], 0, false);

                    }
                    outs += "\n";
                }
                else
                    continue;
                    }
                */
            #endregion


            OutPut = outs + "commit transaction;\n" + "|" + outs2 + "commit transaction;\n";

        }
        private string ConvertToRegisteredName(string internalName)
        {
            return "type_"+internalName.Trim().Replace(" ",string.Empty);
        }
        private string format(string type, string name, string revision, int sum , bool DoE)
        {
            string output = "modify bus";
            output += "\""+type+"\""+" ";
            output += "\"" +name+ "\""+ " ";
            output += "\"" + revision + "\"" + " ";
            if (DoE)
            {
                output += "description "+ "\"Total Cost is: \""   +" "+sum.ToString()+";";
            }
            else
            {
                if (sum == 1)
                {
                    output += "\"End Item\"" + " YES" + ";";
                }
                else 
                    output += "\"End Item\"" + " NO" + ";";
            }
            return output;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string[] y = OutPut.Split('|');
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            for(int i = 0; i < 2; i ++)
            { 
            SaveFileDialog save = new SaveFileDialog();
            if(i==0)
                    save.FileName = "create.txt";
            else
                    save.FileName = "modify.txt";
                save.Filter = "Text File | *.txt";
                // If the file name is not an empty string open it for saving.  
                if (save.FileName != "")
                {
                    // Saves the Image via a FileStream created by the OpenFile method.  
                    // Saves the Image in the appropriate ImageFormat based upon the  
                    // File type selected in the dialog box.  
                    // NOTE that the FilterIndex property is one-based.  
                    if (save.ShowDialog() == DialogResult.OK)

                    {
                        StreamWriter writer = new StreamWriter(save.OpenFile());
                        if (i == 0)
                            writer.Write(y[0]);
                        else
                            writer.Write(y[1]);
                        writer.Dispose();
                        writer.Close();

                    }
                }
            }
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }
    }
}
