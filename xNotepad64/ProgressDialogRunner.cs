namespace xNotepad64
{
    public static class ProgressDialogRunner
    {
        public static async Task RunAsync(IWin32Window? owner, string title, Func<IProgress<OperationProgress>, CancellationToken, Task> operation)
        {
            await RunAsync<object?>(
                owner,
                title,
                async (progress, cancellationToken) =>
                {
                    await operation(progress, cancellationToken);
                    return null;
                });
        }

        public static async Task<T> RunAsync<T>(IWin32Window? owner, string title, Func<IProgress<OperationProgress>, CancellationToken, Task<T>> operation)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            using var dialog = new ProgressDialog();
            dialog.Configure(title);
            dialog.BindCancellation(cancellationTokenSource);

            var progress = new Progress<OperationProgress>(dialog.ReportProgress);
            Task<T> operationTask = operation(progress, cancellationTokenSource.Token);

            bool dialogShown = false;
            Task delayTask = Task.Delay(200);
            if (await Task.WhenAny(operationTask, delayTask) != operationTask)
            {
                dialogShown = true;

                if (owner is Form ownerForm)
                {
                    dialog.Show(ownerForm);
                }
                else
                {
                    dialog.Show();
                }
            }

            try
            {
                return await operationTask;
            }
            finally
            {
                if (dialogShown && !dialog.IsDisposed)
                {
                    dialog.Close();
                }
            }
        }
    }
}
