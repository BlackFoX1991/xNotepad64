namespace xNotepad64
{
    partial class xMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(xMain));
            menuStrip1 = new MenuStrip();
            dateiToolStripMenuItem = new ToolStripMenuItem();
            neuToolStripMenuItem = new ToolStripMenuItem();
            öffnenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            speichernToolStripMenuItem = new ToolStripMenuItem();
            speichernunterToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            beendenToolStripMenuItem = new ToolStripMenuItem();
            bearbeitenToolStripMenuItem = new ToolStripMenuItem();
            rückgängigToolStripMenuItem = new ToolStripMenuItem();
            wiederholenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            ausschneidenToolStripMenuItem = new ToolStripMenuItem();
            kopierenToolStripMenuItem = new ToolStripMenuItem();
            einfügenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            allesauswählenToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            searchToolStripMenuItem = new ToolStripMenuItem();
            extrasToolStripMenuItem = new ToolStripMenuItem();
            optionenToolStripMenuItem = new ToolStripMenuItem();
            hilfeToolStripMenuItem = new ToolStripMenuItem();
            infoToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStrip1 = new ToolStrip();
            neuToolStripButton = new ToolStripButton();
            öffnenToolStripButton = new ToolStripButton();
            speichernToolStripButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            ausschneidenToolStripButton = new ToolStripButton();
            kopierenToolStripButton = new ToolStripButton();
            einfügenToolStripButton = new ToolStripButton();
            TextContent = new TextBox();
            splitContainer1 = new SplitContainer();
            lvwChunks = new ListView();
            ChunkCol = new ColumnHeader();
            SizeCol = new ColumnHeader();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { dateiToolStripMenuItem, bearbeitenToolStripMenuItem, extrasToolStripMenuItem, hilfeToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1062, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            dateiToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { neuToolStripMenuItem, öffnenToolStripMenuItem, toolStripSeparator, speichernToolStripMenuItem, speichernunterToolStripMenuItem, toolStripSeparator1, beendenToolStripMenuItem });
            dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            dateiToolStripMenuItem.Size = new Size(57, 24);
            dateiToolStripMenuItem.Text = "&Datei";
            // 
            // neuToolStripMenuItem
            // 
            neuToolStripMenuItem.Image = (Image)resources.GetObject("neuToolStripMenuItem.Image");
            neuToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            neuToolStripMenuItem.Name = "neuToolStripMenuItem";
            neuToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            neuToolStripMenuItem.Size = new Size(197, 24);
            neuToolStripMenuItem.Text = "&Neu";
            // 
            // öffnenToolStripMenuItem
            // 
            öffnenToolStripMenuItem.Image = (Image)resources.GetObject("öffnenToolStripMenuItem.Image");
            öffnenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            öffnenToolStripMenuItem.Name = "öffnenToolStripMenuItem";
            öffnenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            öffnenToolStripMenuItem.Size = new Size(197, 24);
            öffnenToolStripMenuItem.Text = "Ö&ffnen";
            öffnenToolStripMenuItem.Click += öffnenToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(194, 6);
            // 
            // speichernToolStripMenuItem
            // 
            speichernToolStripMenuItem.Image = (Image)resources.GetObject("speichernToolStripMenuItem.Image");
            speichernToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            speichernToolStripMenuItem.Name = "speichernToolStripMenuItem";
            speichernToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            speichernToolStripMenuItem.Size = new Size(197, 24);
            speichernToolStripMenuItem.Text = "&Speichern";
            // 
            // speichernunterToolStripMenuItem
            // 
            speichernunterToolStripMenuItem.Name = "speichernunterToolStripMenuItem";
            speichernunterToolStripMenuItem.Size = new Size(197, 24);
            speichernunterToolStripMenuItem.Text = "Speichern &unter";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(194, 6);
            // 
            // beendenToolStripMenuItem
            // 
            beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            beendenToolStripMenuItem.Size = new Size(197, 24);
            beendenToolStripMenuItem.Text = "&Beenden";
            // 
            // bearbeitenToolStripMenuItem
            // 
            bearbeitenToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rückgängigToolStripMenuItem, wiederholenToolStripMenuItem, toolStripSeparator3, ausschneidenToolStripMenuItem, kopierenToolStripMenuItem, einfügenToolStripMenuItem, toolStripSeparator4, allesauswählenToolStripMenuItem, toolStripMenuItem1, searchToolStripMenuItem });
            bearbeitenToolStripMenuItem.Name = "bearbeitenToolStripMenuItem";
            bearbeitenToolStripMenuItem.Size = new Size(93, 24);
            bearbeitenToolStripMenuItem.Text = "&Bearbeiten";
            // 
            // rückgängigToolStripMenuItem
            // 
            rückgängigToolStripMenuItem.Name = "rückgängigToolStripMenuItem";
            rückgängigToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            rückgängigToolStripMenuItem.Size = new Size(223, 24);
            rückgängigToolStripMenuItem.Text = "&Rückgängig";
            rückgängigToolStripMenuItem.Click += rückgängigToolStripMenuItem_Click;
            // 
            // wiederholenToolStripMenuItem
            // 
            wiederholenToolStripMenuItem.Name = "wiederholenToolStripMenuItem";
            wiederholenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            wiederholenToolStripMenuItem.Size = new Size(223, 24);
            wiederholenToolStripMenuItem.Text = "&Wiederholen";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(220, 6);
            // 
            // ausschneidenToolStripMenuItem
            // 
            ausschneidenToolStripMenuItem.Image = (Image)resources.GetObject("ausschneidenToolStripMenuItem.Image");
            ausschneidenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            ausschneidenToolStripMenuItem.Name = "ausschneidenToolStripMenuItem";
            ausschneidenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            ausschneidenToolStripMenuItem.Size = new Size(223, 24);
            ausschneidenToolStripMenuItem.Text = "Aussc&hneiden";
            // 
            // kopierenToolStripMenuItem
            // 
            kopierenToolStripMenuItem.Image = (Image)resources.GetObject("kopierenToolStripMenuItem.Image");
            kopierenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            kopierenToolStripMenuItem.Name = "kopierenToolStripMenuItem";
            kopierenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            kopierenToolStripMenuItem.Size = new Size(223, 24);
            kopierenToolStripMenuItem.Text = "&Kopieren";
            // 
            // einfügenToolStripMenuItem
            // 
            einfügenToolStripMenuItem.Image = (Image)resources.GetObject("einfügenToolStripMenuItem.Image");
            einfügenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            einfügenToolStripMenuItem.Name = "einfügenToolStripMenuItem";
            einfügenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            einfügenToolStripMenuItem.Size = new Size(223, 24);
            einfügenToolStripMenuItem.Text = "&Einfügen";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(220, 6);
            // 
            // allesauswählenToolStripMenuItem
            // 
            allesauswählenToolStripMenuItem.Name = "allesauswählenToolStripMenuItem";
            allesauswählenToolStripMenuItem.Size = new Size(223, 24);
            allesauswählenToolStripMenuItem.Text = "&Alles auswählen";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(220, 6);
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Image = (Image)resources.GetObject("searchToolStripMenuItem.Image");
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(223, 24);
            searchToolStripMenuItem.Text = "Suchen...";
            // 
            // extrasToolStripMenuItem
            // 
            extrasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionenToolStripMenuItem });
            extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            extrasToolStripMenuItem.Size = new Size(60, 24);
            extrasToolStripMenuItem.Text = "E&xtras";
            // 
            // optionenToolStripMenuItem
            // 
            optionenToolStripMenuItem.Image = (Image)resources.GetObject("optionenToolStripMenuItem.Image");
            optionenToolStripMenuItem.Name = "optionenToolStripMenuItem";
            optionenToolStripMenuItem.Size = new Size(140, 24);
            optionenToolStripMenuItem.Text = "&Optionen";
            // 
            // hilfeToolStripMenuItem
            // 
            hilfeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { infoToolStripMenuItem });
            hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            hilfeToolStripMenuItem.Size = new Size(53, 24);
            hilfeToolStripMenuItem.Text = "&Hilfe";
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.Image = (Image)resources.GetObject("infoToolStripMenuItem.Image");
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new Size(113, 24);
            infoToolStripMenuItem.Text = "Inf&o...";
            infoToolStripMenuItem.Click += infoToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 641);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1062, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { neuToolStripButton, öffnenToolStripButton, speichernToolStripButton, toolStripSeparator5, ausschneidenToolStripButton, kopierenToolStripButton, einfügenToolStripButton });
            toolStrip1.Location = new Point(0, 28);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1062, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // neuToolStripButton
            // 
            neuToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            neuToolStripButton.Image = (Image)resources.GetObject("neuToolStripButton.Image");
            neuToolStripButton.ImageTransparentColor = Color.Magenta;
            neuToolStripButton.Name = "neuToolStripButton";
            neuToolStripButton.Size = new Size(23, 22);
            neuToolStripButton.Text = "&Neu";
            // 
            // öffnenToolStripButton
            // 
            öffnenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            öffnenToolStripButton.Image = (Image)resources.GetObject("öffnenToolStripButton.Image");
            öffnenToolStripButton.ImageTransparentColor = Color.Magenta;
            öffnenToolStripButton.Name = "öffnenToolStripButton";
            öffnenToolStripButton.Size = new Size(23, 22);
            öffnenToolStripButton.Text = "Ö&ffnen";
            // 
            // speichernToolStripButton
            // 
            speichernToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            speichernToolStripButton.Image = (Image)resources.GetObject("speichernToolStripButton.Image");
            speichernToolStripButton.ImageTransparentColor = Color.Magenta;
            speichernToolStripButton.Name = "speichernToolStripButton";
            speichernToolStripButton.Size = new Size(23, 22);
            speichernToolStripButton.Text = "&Speichern";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // ausschneidenToolStripButton
            // 
            ausschneidenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ausschneidenToolStripButton.Image = (Image)resources.GetObject("ausschneidenToolStripButton.Image");
            ausschneidenToolStripButton.ImageTransparentColor = Color.Magenta;
            ausschneidenToolStripButton.Name = "ausschneidenToolStripButton";
            ausschneidenToolStripButton.Size = new Size(23, 22);
            ausschneidenToolStripButton.Text = "&Ausschneiden";
            // 
            // kopierenToolStripButton
            // 
            kopierenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            kopierenToolStripButton.Image = (Image)resources.GetObject("kopierenToolStripButton.Image");
            kopierenToolStripButton.ImageTransparentColor = Color.Magenta;
            kopierenToolStripButton.Name = "kopierenToolStripButton";
            kopierenToolStripButton.Size = new Size(23, 22);
            kopierenToolStripButton.Text = "&Kopieren";
            // 
            // einfügenToolStripButton
            // 
            einfügenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            einfügenToolStripButton.Image = (Image)resources.GetObject("einfügenToolStripButton.Image");
            einfügenToolStripButton.ImageTransparentColor = Color.Magenta;
            einfügenToolStripButton.Name = "einfügenToolStripButton";
            einfügenToolStripButton.Size = new Size(23, 22);
            einfügenToolStripButton.Text = "&Einfügen";
            // 
            // TextContent
            // 
            TextContent.Dock = DockStyle.Fill;
            TextContent.Location = new Point(0, 0);
            TextContent.Multiline = true;
            TextContent.Name = "TextContent";
            TextContent.ScrollBars = ScrollBars.Both;
            TextContent.Size = new Size(821, 588);
            TextContent.TabIndex = 3;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 53);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvwChunks);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(TextContent);
            splitContainer1.Size = new Size(1062, 588);
            splitContainer1.SplitterDistance = 237;
            splitContainer1.TabIndex = 4;
            // 
            // lvwChunks
            // 
            lvwChunks.Columns.AddRange(new ColumnHeader[] { ChunkCol, SizeCol });
            lvwChunks.Dock = DockStyle.Fill;
            lvwChunks.FullRowSelect = true;
            lvwChunks.GridLines = true;
            lvwChunks.Location = new Point(0, 0);
            lvwChunks.Name = "lvwChunks";
            lvwChunks.Size = new Size(237, 588);
            lvwChunks.TabIndex = 0;
            lvwChunks.UseCompatibleStateImageBehavior = false;
            lvwChunks.View = View.Details;
            lvwChunks.SelectedIndexChanged += lvwChunks_SelectedIndexChanged;
            // 
            // ChunkCol
            // 
            ChunkCol.Text = "Block";
            ChunkCol.Width = 120;
            // 
            // SizeCol
            // 
            SizeCol.Text = "Groesse";
            SizeCol.Width = 120;
            // 
            // xMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1062, 663);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "xMain";
            StartPosition = FormStartPosition.CenterScreen;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem dateiToolStripMenuItem;
        private ToolStripMenuItem neuToolStripMenuItem;
        private ToolStripMenuItem öffnenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem speichernToolStripMenuItem;
        private ToolStripMenuItem speichernunterToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem beendenToolStripMenuItem;
        private ToolStripMenuItem bearbeitenToolStripMenuItem;
        private ToolStripMenuItem rückgängigToolStripMenuItem;
        private ToolStripMenuItem wiederholenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem ausschneidenToolStripMenuItem;
        private ToolStripMenuItem kopierenToolStripMenuItem;
        private ToolStripMenuItem einfügenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem allesauswählenToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem extrasToolStripMenuItem;
        private ToolStripMenuItem optionenToolStripMenuItem;
        private ToolStripMenuItem hilfeToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStrip toolStrip1;
        private ToolStripButton neuToolStripButton;
        private ToolStripButton öffnenToolStripButton;
        private ToolStripButton speichernToolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton ausschneidenToolStripButton;
        private ToolStripButton kopierenToolStripButton;
        private ToolStripButton einfügenToolStripButton;
        private TextBox TextContent;
        private SplitContainer splitContainer1;
        private ListView lvwChunks;
        private ColumnHeader ChunkCol;
        private ColumnHeader SizeCol;
    }
}
