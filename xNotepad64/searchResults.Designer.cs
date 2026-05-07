namespace xNotepad64
{
    partial class searchResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(searchResults));
            mainLayoutPanel = new TableLayoutPanel();
            searchPanel = new FlowLayoutPanel();
            searchTextBox = new TextBox();
            matchCaseCheckBox = new CheckBox();
            wholeWordCheckBox = new CheckBox();
            interpretEscapesCheckBox = new CheckBox();
            searchButton = new Button();
            summaryLabel = new Label();
            resultsListView = new ListView();
            chunkColumn = new ColumnHeader();
            positionColumn = new ColumnHeader();
            previewColumn = new ColumnHeader();
            mainLayoutPanel.SuspendLayout();
            searchPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 1;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(searchPanel, 0, 0);
            mainLayoutPanel.Controls.Add(summaryLabel, 0, 1);
            mainLayoutPanel.Controls.Add(resultsListView, 0, 2);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.RowCount = 3;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Size = new Size(840, 420);
            mainLayoutPanel.TabIndex = 0;
            // 
            // searchPanel
            // 
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(matchCaseCheckBox);
            searchPanel.Controls.Add(wholeWordCheckBox);
            searchPanel.Controls.Add(interpretEscapesCheckBox);
            searchPanel.Controls.Add(searchButton);
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Location = new Point(3, 3);
            searchPanel.Name = "searchPanel";
            searchPanel.Padding = new Padding(12, 12, 12, 8);
            searchPanel.Size = new Size(834, 78);
            searchPanel.TabIndex = 0;
            // 
            // searchTextBox
            // 
            searchTextBox.Location = new Point(15, 15);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(240, 27);
            searchTextBox.TabIndex = 0;
            // 
            // matchCaseCheckBox
            // 
            matchCaseCheckBox.AutoSize = true;
            matchCaseCheckBox.Location = new Point(261, 15);
            matchCaseCheckBox.Name = "matchCaseCheckBox";
            matchCaseCheckBox.Size = new Size(168, 24);
            matchCaseCheckBox.TabIndex = 1;
            matchCaseCheckBox.Text = "Gross/Klein beachten";
            matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // wholeWordCheckBox
            // 
            wholeWordCheckBox.AutoSize = true;
            wholeWordCheckBox.Location = new Point(435, 15);
            wholeWordCheckBox.Name = "wholeWordCheckBox";
            wholeWordCheckBox.Size = new Size(111, 24);
            wholeWordCheckBox.TabIndex = 2;
            wholeWordCheckBox.Text = "Whole Word";
            wholeWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // interpretEscapesCheckBox
            // 
            interpretEscapesCheckBox.AutoSize = true;
            interpretEscapesCheckBox.Location = new Point(552, 15);
            interpretEscapesCheckBox.Name = "interpretEscapesCheckBox";
            interpretEscapesCheckBox.Size = new Size(130, 24);
            interpretEscapesCheckBox.TabIndex = 3;
            interpretEscapesCheckBox.Text = "Escapes deuten";
            interpretEscapesCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchButton
            // 
            searchButton.Location = new Point(688, 15);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(110, 29);
            searchButton.TabIndex = 4;
            searchButton.Text = "Suchen";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // summaryLabel
            // 
            summaryLabel.AutoSize = true;
            summaryLabel.Dock = DockStyle.Fill;
            summaryLabel.Location = new Point(12, 84);
            summaryLabel.Margin = new Padding(12, 0, 12, 0);
            summaryLabel.Name = "summaryLabel";
            summaryLabel.Size = new Size(816, 28);
            summaryLabel.TabIndex = 1;
            summaryLabel.Text = "Keine Treffer geladen.";
            summaryLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resultsListView
            // 
            resultsListView.Columns.AddRange(new ColumnHeader[] { chunkColumn, positionColumn, previewColumn });
            resultsListView.Dock = DockStyle.Fill;
            resultsListView.FullRowSelect = true;
            resultsListView.GridLines = true;
            resultsListView.Location = new Point(12, 124);
            resultsListView.Margin = new Padding(12);
            resultsListView.Name = "resultsListView";
            resultsListView.Size = new Size(816, 284);
            resultsListView.TabIndex = 2;
            resultsListView.UseCompatibleStateImageBehavior = false;
            resultsListView.View = View.Details;
            // 
            // chunkColumn
            // 
            chunkColumn.Text = "Chunk";
            chunkColumn.Width = 120;
            // 
            // positionColumn
            // 
            positionColumn.Text = "Position";
            positionColumn.Width = 120;
            // 
            // previewColumn
            // 
            previewColumn.Text = "Vorschau";
            previewColumn.Width = 540;
            // 
            // searchResults
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 420);
            Controls.Add(mainLayoutPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "searchResults";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Suche";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private FlowLayoutPanel searchPanel;
        private TextBox searchTextBox;
        private CheckBox matchCaseCheckBox;
        private CheckBox wholeWordCheckBox;
        private CheckBox interpretEscapesCheckBox;
        private Button searchButton;
        private Label summaryLabel;
        private ListView resultsListView;
        private ColumnHeader chunkColumn;
        private ColumnHeader positionColumn;
        private ColumnHeader previewColumn;
    }
}
