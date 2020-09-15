using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static DiplRad.Encryptor;

namespace DiplRad
{
    public partial class EncryptionForm : SteganographyForm
    {
        private string picturePath = null;
        private string messagePath = null;
        private string outputPath = null;
        private string outputEncryptedFilename = null;
        private string outputCompressedFilename = null;

        public EncryptionForm(Encryptor encryptor) : base(encryptor)
        {
            InitializeComponent();
        }

        private void BrowseTxtFilesButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            BrowseTxtFilesTextBox.Text = openFileDialog1.FileName;
            messagePath = openFileDialog1.FileName;
        }

        private void BrowseImagesButton_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            BrowseImagesTextBox.Text = openFileDialog2.FileName;
            picturePath = openFileDialog2.FileName;
        }

        private void BrowseFoldersButton_Click(object sender, EventArgs e)
        {
            
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                BrowseFoldersTextBox.Text = folderBrowserDialog1.SelectedPath;
                outputPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void CreateStegoImageButton_Click(object sender, EventArgs e)
        {
            if (picturePath != null && messagePath != null && outputPath != null)
            {
                Thread t = new Thread(new ThreadStart(EncryptionWorker))
                {
                    Priority = ThreadPriority.Highest
                };
                t.Start();
            }
            else
            {
                operationResultLabel.ForeColor = Color.Red;
                operationResultLabel.Text = String.Format("Please provide all the required fields");
            }
        }

        private void EncryptionWorker()
        {
            encryptor.SetParentForm(this);

            try
            {
                IOPaths iOPath = new IOPaths(picturePath, messagePath, outputPath);
                StegoOptions stegoOptions = new StegoOptions(
                    loopMessageRadioButton.Checked,
                    compressMessageRadioButton.Checked,
                    encryptImageCheckBox.Checked,
                    (int)lsbCountUsed.Value);

                outputEncryptedFilename = encryptor.EncryptPicture(iOPath, stegoOptions);

                if (encryptor is JstegEncryptor jstegEncryptor)
                {
                    outputCompressedFilename = jstegEncryptor.CompressPicture(iOPath, encodeMessage: false);
                    this.Invoke(new Action(() =>
                    {
                        operationResultLabel.ForeColor = Color.Green;
                        operationResultLabel.Text = String.Format("Saved encrypted picture as {0}\nSaved compressed picture as {1}", outputEncryptedFilename, outputCompressedFilename);
                        showImageComparisonButton.Enabled = true;
                        showHistogramComparisonButton.Enabled = true;
                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        this.operationResultLabel.ForeColor = Color.Green;
                        operationResultLabel.Text = String.Format("Saved picture as {0}", outputEncryptedFilename);
                        showImageComparisonButton.Enabled = true;
                        showHistogramComparisonButton.Enabled = true;
                    }));
                }
            }
            catch(Exception e)
            {
                this.Invoke(new Action(() =>
                {
                    progressBar.Value = 0;
                    operationResultLabel.ForeColor = Color.Red;
                    operationResultLabel.Text = e.Message;
                    showImageComparisonButton.Enabled = false;
                    showHistogramComparisonButton.Enabled = false;
                }));
            }
        }

        private void EncryptionForm_Load(object sender, EventArgs e)
        {
            if (encryptor is PITEncryptor)
            {
                loopMessageRadioButton.Enabled = false;
                lsbCountUsed.Enabled = false;
                lsbCountUsed.Value = 2;
                titleLabel.Text = "Using PIT encryption method";
            }
            else if (encryptor is LSBEncryptor)
            {

                loopMessageRadioButton.Enabled = true;
                lsbCountUsed.Enabled = true;
                titleLabel.Text = "Using LSB encryption method";
            }
            else
            {
                loopMessageRadioButton.Enabled = true;
                lsbCountUsed.Enabled = false;
                lsbCountUsed.Value = 1;
                titleLabel.Text = "Using JSTEG encryption method";
            }
        }

        private void ShowImageComparisonButton_Click(object sender, EventArgs e)
        {
            var t = new ImageComparison(this, picturePath, outputEncryptedFilename)
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            };
            this.Hide();
            t.Show();
        }

        private void ShowHistogramComparisonButton_Click(object sender, EventArgs e)
        {
            if (encryptor is JstegEncryptor)
            {
                var t = new JStegHistogramComparison(this, outputCompressedFilename, outputEncryptedFilename, encryptor)
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = this.Location
                };
                this.Hide();
                t.Show();
            }
            else
            {
                var t = new HistogramComparison(this, picturePath, outputEncryptedFilename, encryptor)
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = this.Location
                };
                this.Hide();
                t.Show();
            }
        }
    }
}
