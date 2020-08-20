using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplRad
{
    public partial class SteganographyForm : Form
    {
        protected Encryptor encryptor;
        protected ProgressBar progressBar;

        public SteganographyForm(Encryptor encryptor)
        {
            this.encryptor = encryptor;
        }

        public void SetCurrentProgress(int progress)
        {
            this.Invoke(new Action(() => progressBar.Value = progress));
        }

        protected void GoBackToPreviousMenu(object sender, EventArgs e)
        {
            MainScreen.Instance.Show();
            MainScreen.Instance.Location = this.Location;
            this.Dispose();
        }
    }
}
