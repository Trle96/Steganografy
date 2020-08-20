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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            messagePath = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            textBox2.Text = openFileDialog2.FileName;
            picturePath = openFileDialog2.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
                outputPath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (picturePath != null && messagePath != null && outputPath != null)
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
            outputEncryptedFilename = encryptor.EncryptPicture(picturePath, messagePath, outputPath);
            label1.ForeColor = Color.Green;

            if (encryptor is JstegEncryptor)
            {
                outputCompressedFilename = ((JstegEncryptor)encryptor).CompressPicture(picturePath, encodeMessage: false);
                this.Invoke(new Action(() =>
                {
                    label1.Text = String.Format("Saved encrypted picture as {0}, Saved compressed picture as {1}", outputEncryptedFilename, outputCompressedFilename);
                    button5.Enabled = true;
                    button6.Enabled = true;
                }));
            }
            else
            {
                this.Invoke(new Action(() => {
                    label1.Text = String.Format("Saved picture as {0}", outputEncryptedFilename);
                    button5.Enabled = true;
                    button6.Enabled = true;
                }));
            }
        }

        private void EncryptionForm_Load(object sender, EventArgs e)
        {
            if (encryptor is PITEncryptor)
            {
                numericUpDown1.Enabled = false;
                numericUpDown1.Value = 2;
                label6.Text = "Using PIT encryption method";
            }
            else
            {
                numericUpDown1.Enabled = true;
                numericUpDown1.Value = 1;
                if (encryptor is LSBEncryptor)
                {
                    label6.Text = "Using LSB encryption method";
                }
                else
                {
                    label6.Text = "Using JSTEG encryption method";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var t = new ImageComparison(this, picturePath, outputEncryptedFilename);
            t.StartPosition = FormStartPosition.Manual;
            t.Location = this.Location;
            this.Hide();
            t.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (encryptor is JstegEncryptor)
            {
                var t = new JStegHistogramComparison(this, outputCompressedFilename, outputEncryptedFilename, encryptor);
                t.StartPosition = FormStartPosition.Manual;
                t.Location = this.Location;
                this.Hide();
                t.Show();
            }
            else
            {
                var t = new HistogramComparison(this, picturePath, outputEncryptedFilename, encryptor);
                t.StartPosition = FormStartPosition.Manual;
                t.Location = this.Location;
                this.Hide();
                t.Show();
            }
        }
    }
}
