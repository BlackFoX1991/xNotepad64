namespace xNotepad64
{
    public partial class ProgressDialog : Form
    {
        private CancellationTokenSource? _cancellationTokenSource;

        public ProgressDialog()
        {
            InitializeComponent();
            ApplyLocalization();
        }

        public void ApplyLocalization()
        {
            Text = LocalizationManager.Get("progress.default_title", "Vorgang laeuft");
            stageLabel.Text = LocalizationManager.Get("progress.default_title", "Vorgang laeuft");
            detailLabel.Text = LocalizationManager.Get("progress.default_detail", "Vorgang wird vorbereitet...");
            cancelButton.Text = LocalizationManager.Get("progress.cancel", "Abbruch");
        }

        public void Configure(string title)
        {
            Text = title;
            stageLabel.Text = title;
            detailLabel.Text = LocalizationManager.Get("progress.default_detail", "Vorgang wird vorbereitet...");
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
            detailLabel.Text = string.IsNullOrWhiteSpace(progress.Detail)
                ? LocalizationManager.Get("progress.please_wait", "Bitte warten...")
                : progress.Detail;

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
            detailLabel.Text = LocalizationManager.Get("progress.cancelling", "Abbruch wird angefordert...");
            _cancellationTokenSource?.Cancel();
        }
    }
}
