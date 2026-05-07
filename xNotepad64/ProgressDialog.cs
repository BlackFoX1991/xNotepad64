namespace xNotepad64
{
    public partial class ProgressDialog : Form
    {
        private CancellationTokenSource? _cancellationTokenSource;

        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void Configure(string title)
        {
            Text = title;
            stageLabel.Text = title;
            detailLabel.Text = "Vorgang wird vorbereitet...";
            operationProgressBar.Style = ProgressBarStyle.Marquee;
            operationProgressBar.Value = 0;
            cancelButton.Enabled = true;
        }

        public void BindCancellation(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void ReportProgress(OperationProgress progress)
        {
            stageLabel.Text = string.IsNullOrWhiteSpace(progress.Stage) ? Text : progress.Stage;
            detailLabel.Text = string.IsNullOrWhiteSpace(progress.Detail) ? "Bitte warten..." : progress.Detail;

            if (progress.IsIndeterminate)
            {
                operationProgressBar.Style = ProgressBarStyle.Marquee;
                return;
            }

            if (operationProgressBar.Style != ProgressBarStyle.Continuous)
            {
                operationProgressBar.Style = ProgressBarStyle.Continuous;
            }

            operationProgressBar.Value = progress.Percent;
        }

        private void cancelButton_Click(object? sender, EventArgs e)
        {
            cancelButton.Enabled = false;
            detailLabel.Text = "Abbruch wird angefordert...";
            _cancellationTokenSource?.Cancel();
        }
    }
}
