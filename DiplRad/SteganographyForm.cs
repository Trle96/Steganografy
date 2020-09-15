using System;
using System.Windows.Forms;

namespace DiplRad
{
    public abstract partial class SteganographyForm : Form
    {
        protected Encryptor encryptor;
        protected ProgressBar progressBar;

        public SteganographyForm(Encryptor encryptor)
        {
            this.encryptor = encryptor;
        }

        public void SetCurrentProgress(int progress)
        {
            progressBar.BeginInvoke(new Action(
                        () => {
                            progressBar.Value = progress;
                            progressBar.Value = progress - 1;
                            progressBar.Value = progress;
                        }));
        }

        protected void GoBackToPreviousMenu(object sender, EventArgs e)
        {
            MainScreen.Instance.Show();
            MainScreen.Instance.Location = this.Location;
            this.Dispose();
        }
    }
}
