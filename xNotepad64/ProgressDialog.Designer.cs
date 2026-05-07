namespace xNotepad64
{
    partial class ProgressDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            mainLayoutPanel = new TableLayoutPanel();
            stageLabel = new Label();
            detailLabel = new Label();
            operationProgressBar = new ProgressBar();
            footerPanel = new FlowLayoutPanel();
            cancelButton = new Button();
            mainLayoutPanel.SuspendLayout();
            footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            mainLayoutPanel.ColumnCount = 1;
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Controls.Add(stageLabel, 0, 0);
            mainLayoutPanel.Controls.Add(detailLabel, 0, 1);
            mainLayoutPanel.Controls.Add(operationProgressBar, 0, 2);
            mainLayoutPanel.Controls.Add(footerPanel, 0, 3);
            mainLayoutPanel.Dock = DockStyle.Fill;
            mainLayoutPanel.Location = new Point(0, 0);
            mainLayoutPanel.Name = "mainLayoutPanel";
            mainLayoutPanel.Padding = new Padding(16);
            mainLayoutPanel.RowCount = 4;
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayoutPanel.Size = new Size(430, 170);
            mainLayoutPanel.TabIndex = 0;
            // 
            // stageLabel
            // 
            stageLabel.AutoSize = true;
            stageLabel.Dock = DockStyle.Fill;
            stageLabel.Location = new Point(19, 16);
            stageLabel.Name = "stageLabel";
            stageLabel.Size = new Size(392, 28);
            stageLabel.TabIndex = 0;
            stageLabel.Text = "Vorgang läuft";
            stageLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // detailLabel
            // 
            detailLabel.AutoSize = true;
            detailLabel.Dock = DockStyle.Fill;
            detailLabel.Location = new Point(19, 44);
            detailLabel.Name = "detailLabel";
            detailLabel.Size = new Size(392, 28);
            detailLabel.TabIndex = 1;
            detailLabel.Text = "Bitte warten...";
            detailLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // operationProgressBar
            // 
            operationProgressBar.Dock = DockStyle.Fill;
            operationProgressBar.Location = new Point(19, 75);
            operationProgressBar.Name = "operationProgressBar";
            operationProgressBar.Size = new Size(392, 32);
            operationProgressBar.Style = ProgressBarStyle.Marquee;
            operationProgressBar.TabIndex = 2;
            // 
            // footerPanel
            // 
            footerPanel.Controls.Add(cancelButton);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.FlowDirection = FlowDirection.RightToLeft;
            footerPanel.Location = new Point(19, 113);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(392, 38);
            footerPanel.TabIndex = 3;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(314, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 29);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Abbruch";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // ProgressDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(430, 170);
            ControlBox = false;
            Controls.Add(mainLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Vorgang läuft";
            mainLayoutPanel.ResumeLayout(false);
            mainLayoutPanel.PerformLayout();
            footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayoutPanel;
        private Label stageLabel;
        private Label detailLabel;
        private ProgressBar operationProgressBar;
        private FlowLayoutPanel footerPanel;
        private Button cancelButton;
    }
}
