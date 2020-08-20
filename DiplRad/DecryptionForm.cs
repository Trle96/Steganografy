using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplRad
{
    public partial class DecryptionForm : SteganographyForm
    {
        private string picturePath = null;
        private string outputPath = null;
        private string outputDecryptedFilename = null;

        public DecryptionForm(Encryptor encryptor) : base(encryptor)
        {
            InitializeComponent();
            if (encryptor is JstegEncryptor)
            {
                this.openFileDialog2.Filter = "jpeg files (*.jpeg)|*.jepg| jpg files (*.jpg)|*.jpg";
            }
            else
            {
                this.openFileDialog2.Filter = "png files (*.png)|*.png| bmp files (*.bmp)|*.bmp";
            }
        }

        private void OpenSelectPictureFileDialogClicked(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            textBox2.Text = openFileDialog2.FileName;
            picturePath = openFileDialog2.FileName;
        }

        private void SelectOutputPathClicked(object sender, EventArgs e)
        {
            
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
                outputPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void DecryptStegoImageClicked(object sender, EventArgs e)
        {
            if (picturePath != null && outputPath != null)
            {
                if (numericUpDown1.Enabled)
                {
                    encryptor.SetLSBCount((int)numericUpDown1.Value);
                }

                Thread t = new Thread(new ThreadStart(Test));
                t.Priority = ThreadPriority.Highest;
                t.Start();
            }
            else
            {
                label1.ForeColor = Color.Red;
                label1.Text = String.Format("Please provide all the required fields");
            }
        }

        private void Test()
        {
            encryptor.SetParentForm(this);
            outputDecryptedFilename = encryptor.DecryptPicture(picturePath, outputPath);
            label1.ForeColor = Color.Green;

            this.Invoke(new Action(() => {
                label1.Text = String.Format("Saved output txt as {0}", outputDecryptedFilename);
                button6.Enabled = true;
            }));
        }

        private void EncryptionForm_Load(object sender, EventArgs e)
        {
            if (encryptor is PITEncryptor)
            {
                numericUpDown1.Enabled = false;
                numericUpDown1.Value = 2;
                label6.Text = "Using PIT decryption method";
            }
            else
            {
                numericUpDown1.Enabled = true;
                numericUpDown1.Value = 1;
                if (encryptor is LSBEncryptor)
                {
                    label6.Text = "Using LSB decryption method";
                }
                else
                {
                    label6.Text = "Using JSTEG decryption method";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var t = new ImageComparison(this, picturePath, outputDecryptedFilename);
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            this.Hide();
            t.Show();
        }

        private void OutputTextFormClicked(object sender, EventArgs e)
        {
            var t = new OutputTextForm(this, outputDecryptedFilename);
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            this.Hide();
            t.Show();
        }
    }
}
