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
using static DiplRad.Encryptor;

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
                this.openFileDialog2.Filter = " jpg files (*.jpg)|*.jpg| jpeg files (*.jpeg)|*.jpeg";
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
            browseImageTextBox.Text = openFileDialog2.FileName;
            picturePath = openFileDialog2.FileName;
        }

        private void SelectOutputPathClicked(object sender, EventArgs e)
        {
            
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                browseFolderTextBox.Text = folderBrowserDialog1.SelectedPath;
                outputPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void DecryptStegoImageClicked(object sender, EventArgs e)
        {
            if (picturePath != null && outputPath != null)
            {
                Thread t = new Thread(new ThreadStart(DecryptionWorker));
                t.Priority = ThreadPriority.Highest;
                t.Start();
            }
            else
            {
                resultOutputLabel.ForeColor = Color.Red;
                resultOutputLabel.Text = String.Format("Please provide all the required fields");
            }
        }

        private void DecryptionWorker()
        {
            try
            {
                encryptor.SetParentForm(this);
                IOPaths iOPath = new IOPaths(picturePath, null, outputPath);
                StegoOptions encryptionOptions = new StegoOptions(
                    loopMessage: false,
                    compressMessage: compressedMessageCheckBox.Checked,
                    encryptMessage: encryptMessageCheckBox.Checked,
                    lsbUsed: (int)lsbCountUsed.Value);

                outputDecryptedFilename = encryptor.DecryptPicture(iOPath, encryptionOptions);

                this.Invoke(new Action(() =>
                {
                    resultOutputLabel.ForeColor = Color.Green;
                    resultOutputLabel.Text = String.Format("Saved output txt as {0}", outputDecryptedFilename);
                    showEncryptedTextButton.Enabled = true;
                }));
            }
            catch (Exception e)
            {
                this.Invoke(new Action(() =>
                {
                    progressBar.Value = 0;
                    resultOutputLabel.ForeColor = Color.Red;
                    resultOutputLabel.Text = e.Message;
                    showEncryptedTextButton.Enabled = false;
                }));
            }
        }

        private void DecryptionForm_Load(object sender, EventArgs e)
        {
            if (encryptor is PITEncryptor)
            {
                lsbCountUsed.Enabled = false;
                lsbCountUsed.Value = 2;
                titleLabel.Text = "Using PIT encryption method";
            }
            else if (encryptor is LSBEncryptor)
            {
                lsbCountUsed.Enabled = true;
                titleLabel.Text = "Using LSB encryption method";
            }
            else
            {
                lsbCountUsed.Enabled = false;
                lsbCountUsed.Value = 1;
                titleLabel.Text = "Using JSTEG encryption method";
            }
        }

        private void ImageComparisonButton_Click(object sender, EventArgs e)
        {
            var t = new ImageComparison(this, picturePath, outputDecryptedFilename)
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            };

            this.Hide();
            t.Show();
        }

        private void OutputTextFormClicked(object sender, EventArgs e)
        {
            var t = new OutputTextForm(this, outputDecryptedFilename)
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            };

            this.Hide();
            t.Show();
        }
    }
}
