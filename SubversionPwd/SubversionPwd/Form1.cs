using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SubversionPwd
{
    public partial class Form1 : Form
    {
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public Form1()
        {
            InitializeComponent();
            // Set the title
            this.Text = "Recover Subversion Password";
            // Define the border style of the form to a dialog box.
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Set the MaximizeBox to false to remove the maximize box.
            this.MaximizeBox = false;
            // Set the MinimizeBox to false to remove the minimize box.
            this.MinimizeBox = false;
            // Set the start position of the form to the center of the screen.
            this.StartPosition = FormStartPosition.CenterScreen;
            // Always on top
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("<(.*)>.*", RegexOptions.IgnoreCase);
            String path = "C:/Users/"+Environment.UserName+"/AppData/Roaming/Subversion/auth/svn.simple";
            string line = Environment.NewLine;

            foreach (String f in Directory.EnumerateFiles(path))
            {
                StreamReader sr = File.OpenText(f);
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string encrypted = sr.ReadLine();

                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string url = regex.Replace(sr.ReadLine(), "$1");

                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                string user = sr.ReadLine();

                sr.Close();

                string entropy = null;
                string description="";
                string decrypted="";

                try
                {
                    decrypted = DPAPI.Decrypt(encrypted, entropy, out description);
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {
                        description = ex.GetBaseException().Message;
                    }
                }

                String txt = "url=" + url + line;
                txt += "user=" + user + line;

                if (description != null)
                {
                    txt += "error=" + description + line;
                }
                else
                {
                    txt += "pwd=" + decrypted + line;
                }
                                
                textBox1.Text += txt+line;
                
            }
        }
    }
}
