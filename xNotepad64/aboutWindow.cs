using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace xNotepad64
{
    public partial class aboutWindow : Form
    {
        public aboutWindow()
        {
            InitializeComponent();
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            Text = LocalizationManager.Get("about.form.title", "Info");
            label1.Text = LocalizationManager.Get("about.description", "© 2026 Artur Loewen\nxNotepad64 ist ein Editor zur effizienten Bearbeitung grosser Text-Dateien, die deutlich groesser als 4 GB sein koennen.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/BlackFoX1991/xNotepad64",
                UseShellExecute = true
            });
        }
    }
}
