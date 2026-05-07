namespace xNotepad64
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            mainLayoutPanel = new TableLayoutPanel();
            contentPanel = new Panel();
            fontGroupBox = new GroupBox();
            fontPreviewLabel = new Label();
            fontSummaryLabel = new Label();
            fontStyleValueLabel = new Label();
            fontSizeValueLabel = new Label();
            fontNameValueLabel = new Label();
            fontStyleCaptionLabel = new Label();
            fontSizeCaptionLabel = new Label();
            fontNameCaptionLabel = new Label();
            chooseFontButton = new Button();
            languageGroupBox = new GroupBox();
            languageDescriptionValueLabel = new Label();
            languageDescriptionCaptionLabel = new Label();
            languageAuthorValueLabel = new Label();
            languageAuthorCaptionLabel = new Label();
            languageVersionValueLabel = new Label();
            languageVersionCaptionLabel = new Label();
            languageCultureValueLabel = new Label();
            languageCultureCaptionLabel = new Label();
            languageComboBox = new ComboBox();
            languageLabel = new Label();
            infoGroupBox = new GroupBox();
            gibValueLabel = new Label();
            mibValueLabel = new Label();
            bytesValueLabel = new Label();
            gibCaptionLabel = new Label();
            mibCaptionLabel = new Label();
            bytesCaptionLabel = new Label();
            allowAbruptChunkCutoffCheckBox = new CheckBox();
            hintLabel = new Label();
            chunkSizeNumericUpDown = new NumericUpDown();
            chunkSizeLabel = new Label();
            footerPanel = new FlowLayoutPanel();
            btnSave = new Button();
            btnCancel = new Button();
            mainLayoutPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            fontGroupBox.SuspendLayout();
            infoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chunkSizeNumericUpDown).BeginInit();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 1;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(contentPanel, 0, 0);
            mainLayoutPanel.Controls.Add(footerPanel, 0, 1);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 2;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            mainLayoutPanel.Size = new Size(620, 620);
            mainLayoutPanel.TabIndex = 0;
            // 
            // contentPanel
            // 
            contentPanel.Controls.Add(fontGroupBox);
            contentPanel.Controls.Add(languageGroupBox);
            contentPanel.Controls.Add(infoGroupBox);
            contentPanel.Controls.Add(allowAbruptChunkCutoffCheckBox);
            contentPanel.Controls.Add(hintLabel);
            contentPanel.Controls.Add(chunkSizeNumericUpDown);
            contentPanel.Controls.Add(chunkSizeLabel);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(3, 3);
            contentPanel.Name = "contentPanel";
            contentPanel.Padding = new Padding(18);
            contentPanel.Size = new Size(614, 546);
            contentPanel.TabIndex = 0;
            // 
            // fontGroupBox
            // 
            fontGroupBox.Controls.Add(fontPreviewLabel);
            fontGroupBox.Controls.Add(fontSummaryLabel);
            fontGroupBox.Controls.Add(fontStyleValueLabel);
            fontGroupBox.Controls.Add(fontSizeValueLabel);
            fontGroupBox.Controls.Add(fontNameValueLabel);
            fontGroupBox.Controls.Add(fontStyleCaptionLabel);
            fontGroupBox.Controls.Add(fontSizeCaptionLabel);
            fontGroupBox.Controls.Add(fontNameCaptionLabel);
            fontGroupBox.Controls.Add(chooseFontButton);
            fontGroupBox.Location = new Point(22, 367);
            fontGroupBox.Name = "fontGroupBox";
            fontGroupBox.Size = new Size(570, 165);
            fontGroupBox.TabIndex = 4;
            fontGroupBox.TabStop = false;
            fontGroupBox.Text = "Editor-Schrift";
            // 
            // fontPreviewLabel
            // 
            fontPreviewLabel.BorderStyle = BorderStyle.FixedSingle;
            fontPreviewLabel.Location = new Point(17, 112);
            fontPreviewLabel.Name = "fontPreviewLabel";
            fontPreviewLabel.Size = new Size(532, 34);
            fontPreviewLabel.TabIndex = 8;
            fontPreviewLabel.Text = "Beispiel";
            fontPreviewLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fontSummaryLabel
            // 
            fontSummaryLabel.AutoSize = true;
            fontSummaryLabel.Location = new Point(17, 86);
            fontSummaryLabel.Name = "fontSummaryLabel";
            fontSummaryLabel.Size = new Size(180, 20);
            fontSummaryLabel.TabIndex = 7;
            fontSummaryLabel.Text = "Schrift-Zusammenfassung";
            // 
            // fontStyleValueLabel
            // 
            fontStyleValueLabel.AutoSize = true;
            fontStyleValueLabel.Location = new Point(370, 50);
            fontStyleValueLabel.Name = "fontStyleValueLabel";
            fontStyleValueLabel.Size = new Size(60, 20);
            fontStyleValueLabel.TabIndex = 6;
            fontStyleValueLabel.Text = "Regular";
            // 
            // fontSizeValueLabel
            // 
            fontSizeValueLabel.AutoSize = true;
            fontSizeValueLabel.Location = new Point(370, 25);
            fontSizeValueLabel.Name = "fontSizeValueLabel";
            fontSizeValueLabel.Size = new Size(43, 20);
            fontSizeValueLabel.TabIndex = 5;
            fontSizeValueLabel.Text = "10 pt";
            // 
            // fontNameValueLabel
            // 
            fontNameValueLabel.AutoSize = true;
            fontNameValueLabel.Location = new Point(85, 25);
            fontNameValueLabel.Name = "fontNameValueLabel";
            fontNameValueLabel.Size = new Size(111, 20);
            fontNameValueLabel.TabIndex = 4;
            fontNameValueLabel.Text = "Cascadia Mono";
            // 
            // fontStyleCaptionLabel
            // 
            fontStyleCaptionLabel.AutoSize = true;
            fontStyleCaptionLabel.Location = new Point(304, 50);
            fontStyleCaptionLabel.Name = "fontStyleCaptionLabel";
            fontStyleCaptionLabel.Size = new Size(33, 20);
            fontStyleCaptionLabel.TabIndex = 3;
            fontStyleCaptionLabel.Text = "Stil:";
            // 
            // fontSizeCaptionLabel
            // 
            fontSizeCaptionLabel.AutoSize = true;
            fontSizeCaptionLabel.Location = new Point(304, 25);
            fontSizeCaptionLabel.Name = "fontSizeCaptionLabel";
            fontSizeCaptionLabel.Size = new Size(39, 20);
            fontSizeCaptionLabel.TabIndex = 2;
            fontSizeCaptionLabel.Text = "Size:";
            // 
            // fontNameCaptionLabel
            // 
            fontNameCaptionLabel.AutoSize = true;
            fontNameCaptionLabel.Location = new Point(17, 25);
            fontNameCaptionLabel.Name = "fontNameCaptionLabel";
            fontNameCaptionLabel.Size = new Size(41, 20);
            fontNameCaptionLabel.TabIndex = 1;
            fontNameCaptionLabel.Text = "Font:";
            // 
            // chooseFontButton
            // 
            chooseFontButton.Location = new Point(17, 50);
            chooseFontButton.Name = "chooseFontButton";
            chooseFontButton.Size = new Size(130, 29);
            chooseFontButton.TabIndex = 0;
            chooseFontButton.Text = "Schrift waehlen";
            chooseFontButton.UseVisualStyleBackColor = true;
            // 
            // languageGroupBox
            // 
            languageGroupBox.Controls.Add(languageDescriptionValueLabel);
            languageGroupBox.Controls.Add(languageDescriptionCaptionLabel);
            languageGroupBox.Controls.Add(languageAuthorValueLabel);
            languageGroupBox.Controls.Add(languageAuthorCaptionLabel);
            languageGroupBox.Controls.Add(languageVersionValueLabel);
            languageGroupBox.Controls.Add(languageVersionCaptionLabel);
            languageGroupBox.Controls.Add(languageCultureValueLabel);
            languageGroupBox.Controls.Add(languageCultureCaptionLabel);
            languageGroupBox.Controls.Add(languageComboBox);
            languageGroupBox.Controls.Add(languageLabel);
            languageGroupBox.Location = new Point(22, 228);
            languageGroupBox.Name = "languageGroupBox";
            languageGroupBox.Size = new Size(570, 133);
            languageGroupBox.TabIndex = 6;
            languageGroupBox.TabStop = false;
            languageGroupBox.Text = "Sprache";
            // 
            // languageDescriptionValueLabel
            // 
            languageDescriptionValueLabel.Location = new Point(114, 90);
            languageDescriptionValueLabel.Name = "languageDescriptionValueLabel";
            languageDescriptionValueLabel.Size = new Size(435, 34);
            languageDescriptionValueLabel.TabIndex = 9;
            languageDescriptionValueLabel.Text = "-";
            // 
            // languageDescriptionCaptionLabel
            // 
            languageDescriptionCaptionLabel.AutoSize = true;
            languageDescriptionCaptionLabel.Location = new Point(17, 90);
            languageDescriptionCaptionLabel.Name = "languageDescriptionCaptionLabel";
            languageDescriptionCaptionLabel.Size = new Size(93, 20);
            languageDescriptionCaptionLabel.TabIndex = 8;
            languageDescriptionCaptionLabel.Text = "Beschreibung:";
            // 
            // languageAuthorValueLabel
            // 
            languageAuthorValueLabel.AutoSize = true;
            languageAuthorValueLabel.Location = new Point(388, 60);
            languageAuthorValueLabel.Name = "languageAuthorValueLabel";
            languageAuthorValueLabel.Size = new Size(12, 20);
            languageAuthorValueLabel.TabIndex = 7;
            languageAuthorValueLabel.Text = "-";
            // 
            // languageAuthorCaptionLabel
            // 
            languageAuthorCaptionLabel.AutoSize = true;
            languageAuthorCaptionLabel.Location = new Point(304, 60);
            languageAuthorCaptionLabel.Name = "languageAuthorCaptionLabel";
            languageAuthorCaptionLabel.Size = new Size(49, 20);
            languageAuthorCaptionLabel.TabIndex = 6;
            languageAuthorCaptionLabel.Text = "Autor:";
            // 
            // languageVersionValueLabel
            // 
            languageVersionValueLabel.AutoSize = true;
            languageVersionValueLabel.Location = new Point(114, 60);
            languageVersionValueLabel.Name = "languageVersionValueLabel";
            languageVersionValueLabel.Size = new Size(12, 20);
            languageVersionValueLabel.TabIndex = 5;
            languageVersionValueLabel.Text = "-";
            // 
            // languageVersionCaptionLabel
            // 
            languageVersionCaptionLabel.AutoSize = true;
            languageVersionCaptionLabel.Location = new Point(17, 60);
            languageVersionCaptionLabel.Name = "languageVersionCaptionLabel";
            languageVersionCaptionLabel.Size = new Size(60, 20);
            languageVersionCaptionLabel.TabIndex = 4;
            languageVersionCaptionLabel.Text = "Version:";
            // 
            // languageCultureValueLabel
            // 
            languageCultureValueLabel.AutoSize = true;
            languageCultureValueLabel.Location = new Point(388, 30);
            languageCultureValueLabel.Name = "languageCultureValueLabel";
            languageCultureValueLabel.Size = new Size(12, 20);
            languageCultureValueLabel.TabIndex = 3;
            languageCultureValueLabel.Text = "-";
            // 
            // languageCultureCaptionLabel
            // 
            languageCultureCaptionLabel.AutoSize = true;
            languageCultureCaptionLabel.Location = new Point(304, 30);
            languageCultureCaptionLabel.Name = "languageCultureCaptionLabel";
            languageCultureCaptionLabel.Size = new Size(49, 20);
            languageCultureCaptionLabel.TabIndex = 2;
            languageCultureCaptionLabel.Text = "Kultur:";
            // 
            // languageComboBox
            // 
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.FormattingEnabled = true;
            languageComboBox.Location = new Point(114, 27);
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Size = new Size(170, 28);
            languageComboBox.TabIndex = 1;
            // 
            // languageLabel
            // 
            languageLabel.AutoSize = true;
            languageLabel.Location = new Point(17, 30);
            languageLabel.Name = "languageLabel";
            languageLabel.Size = new Size(76, 20);
            languageLabel.TabIndex = 0;
            languageLabel.Text = "UI-Sprache:";
            // 
            // infoGroupBox
            // 
            infoGroupBox.Controls.Add(gibValueLabel);
            infoGroupBox.Controls.Add(mibValueLabel);
            infoGroupBox.Controls.Add(bytesValueLabel);
            infoGroupBox.Controls.Add(gibCaptionLabel);
            infoGroupBox.Controls.Add(mibCaptionLabel);
            infoGroupBox.Controls.Add(bytesCaptionLabel);
            infoGroupBox.Location = new Point(22, 88);
            infoGroupBox.Name = "infoGroupBox";
            infoGroupBox.Size = new Size(570, 95);
            infoGroupBox.TabIndex = 3;
            infoGroupBox.TabStop = false;
            infoGroupBox.Text = "Aktuelle Chunk-Groesse";
            // 
            // gibValueLabel
            // 
            gibValueLabel.AutoSize = true;
            gibValueLabel.Location = new Point(193, 65);
            gibValueLabel.Name = "gibValueLabel";
            gibValueLabel.Size = new Size(71, 20);
            gibValueLabel.TabIndex = 5;
            gibValueLabel.Text = "0.000 GiB";
            // 
            // mibValueLabel
            // 
            mibValueLabel.AutoSize = true;
            mibValueLabel.Location = new Point(193, 43);
            mibValueLabel.Name = "mibValueLabel";
            mibValueLabel.Size = new Size(47, 20);
            mibValueLabel.TabIndex = 4;
            mibValueLabel.Text = "0 MiB";
            // 
            // bytesValueLabel
            // 
            bytesValueLabel.AutoSize = true;
            bytesValueLabel.Location = new Point(193, 21);
            bytesValueLabel.Name = "bytesValueLabel";
            bytesValueLabel.Size = new Size(56, 20);
            bytesValueLabel.TabIndex = 3;
            bytesValueLabel.Text = "0 Bytes";
            // 
            // gibCaptionLabel
            // 
            gibCaptionLabel.AutoSize = true;
            gibCaptionLabel.Location = new Point(17, 65);
            gibCaptionLabel.Name = "gibCaptionLabel";
            gibCaptionLabel.Size = new Size(91, 20);
            gibCaptionLabel.TabIndex = 2;
            gibCaptionLabel.Text = "GiB (binaer):";
            // 
            // mibCaptionLabel
            // 
            mibCaptionLabel.AutoSize = true;
            mibCaptionLabel.Location = new Point(17, 43);
            mibCaptionLabel.Name = "mibCaptionLabel";
            mibCaptionLabel.Size = new Size(94, 20);
            mibCaptionLabel.TabIndex = 1;
            mibCaptionLabel.Text = "MiB (binaer):";
            // 
            // bytesCaptionLabel
            // 
            bytesCaptionLabel.AutoSize = true;
            bytesCaptionLabel.Location = new Point(17, 21);
            bytesCaptionLabel.Name = "bytesCaptionLabel";
            bytesCaptionLabel.Size = new Size(47, 20);
            bytesCaptionLabel.TabIndex = 0;
            bytesCaptionLabel.Text = "Bytes:";
            // 
            // allowAbruptChunkCutoffCheckBox
            // 
            allowAbruptChunkCutoffCheckBox.AutoSize = true;
            allowAbruptChunkCutoffCheckBox.Location = new Point(22, 58);
            allowAbruptChunkCutoffCheckBox.Name = "allowAbruptChunkCutoffCheckBox";
            allowAbruptChunkCutoffCheckBox.Size = new Size(432, 24);
            allowAbruptChunkCutoffCheckBox.TabIndex = 5;
            allowAbruptChunkCutoffCheckBox.Text = "Konsequentes abschneiden von Wörtern und Zahlen erlauben";
            allowAbruptChunkCutoffCheckBox.UseVisualStyleBackColor = true;
            // 
            // hintLabel
            // 
            hintLabel.Location = new Point(22, 189);
            hintLabel.Name = "hintLabel";
            hintLabel.Size = new Size(570, 36);
            hintLabel.TabIndex = 2;
            hintLabel.Text = "Hinweis";
            // 
            // chunkSizeNumericUpDown
            // 
            chunkSizeNumericUpDown.Location = new Point(251, 27);
            chunkSizeNumericUpDown.Name = "chunkSizeNumericUpDown";
            chunkSizeNumericUpDown.Size = new Size(121, 27);
            chunkSizeNumericUpDown.TabIndex = 1;
            chunkSizeNumericUpDown.ThousandsSeparator = true;
            // 
            // chunkSizeLabel
            // 
            chunkSizeLabel.AutoSize = true;
            chunkSizeLabel.Location = new Point(22, 29);
            chunkSizeLabel.Name = "chunkSizeLabel";
            chunkSizeLabel.Size = new Size(216, 20);
            chunkSizeLabel.TabIndex = 0;
            chunkSizeLabel.Text = "Maximale Chunk-Groesse (MiB)";
            // 
            // footerPanel
            // 
            footerPanel.BackColor = Color.FromArgb(240, 240, 240);
            footerPanel.Controls.Add(btnSave);
            footerPanel.Controls.Add(btnCancel);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.FlowDirection = FlowDirection.RightToLeft;
            footerPanel.Location = new Point(0, 552);
            footerPanel.Margin = new Padding(0);
            footerPanel.Name = "footerPanel";
            footerPanel.Padding = new Padding(10);
            footerPanel.Size = new Size(620, 68);
            footerPanel.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(502, 13);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(95, 34);
            btnSave.TabIndex = 0;
            btnSave.Text = "Speichern";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(401, 13);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(95, 34);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Abbrechen";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 620);
            Controls.Add(mainLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Options";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Optionen";
            mainLayoutPanel.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            contentPanel.PerformLayout();
            fontGroupBox.ResumeLayout(false);
            fontGroupBox.PerformLayout();
            infoGroupBox.ResumeLayout(false);
            infoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chunkSizeNumericUpDown).EndInit();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private Panel contentPanel;
        private FlowLayoutPanel footerPanel;
        private Button btnSave;
        private Button btnCancel;
        private Label chunkSizeLabel;
        private NumericUpDown chunkSizeNumericUpDown;
        private Label hintLabel;
        private GroupBox infoGroupBox;
        private Label bytesCaptionLabel;
        private Label gibValueLabel;
        private Label mibValueLabel;
        private Label bytesValueLabel;
        private Label gibCaptionLabel;
        private Label mibCaptionLabel;
        private CheckBox allowAbruptChunkCutoffCheckBox;
        private GroupBox fontGroupBox;
        private Button chooseFontButton;
        private Label fontStyleValueLabel;
        private Label fontSizeValueLabel;
        private Label fontNameValueLabel;
        private Label fontStyleCaptionLabel;
        private Label fontSizeCaptionLabel;
        private Label fontNameCaptionLabel;
        private Label fontSummaryLabel;
        private Label fontPreviewLabel;
        private GroupBox languageGroupBox;
        private Label languageDescriptionValueLabel;
        private Label languageDescriptionCaptionLabel;
        private Label languageAuthorValueLabel;
        private Label languageAuthorCaptionLabel;
        private Label languageVersionValueLabel;
        private Label languageVersionCaptionLabel;
        private Label languageCultureValueLabel;
        private Label languageCultureCaptionLabel;
        private ComboBox languageComboBox;
        private Label languageLabel;
    }
}

