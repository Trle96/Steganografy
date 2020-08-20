using System;
using System.Windows.Forms;

namespace DiplRad
{
    public partial class MainScreen : Form
    {
        private static MainScreen _instance = null;

        public static MainScreen Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new MainScreen();
                }

                return _instance;
            }
        }


        public MainScreen()
        {
            InitializeComponent();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {

        }

        private void LsbEncryptorClicked(object sender, EventArgs e)
        {
            var t = new EncryptionForm(new LSBEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }

        private void PITEncryptorClicked(object sender, EventArgs e)
        {
            var t = new EncryptionForm(new PITEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }

        private void JStegEncryptorClicked(object sender, EventArgs e)
        {
            var t = new EncryptionForm(new JstegEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }

        private void LsbDecryptorClicked(object sender, EventArgs e)
        {
            var t = new DecryptionForm(new LSBEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }

        private void PITDecryptorClicked(object sender, EventArgs e)
        {
            var t = new DecryptionForm(new PITEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }

        private void JStegDecryptorClicked(object sender, EventArgs e)
        {
            var t = new DecryptionForm(new JstegEncryptor());
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            t.Show();
            this.Hide();
        }
    }
}
