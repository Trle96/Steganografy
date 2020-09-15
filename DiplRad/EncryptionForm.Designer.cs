namespace DiplRad
{
    partial class EncryptionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.browseTxtFilesButton = new System.Windows.Forms.Button();
            this.BrowseTxtFilesTextBox = new System.Windows.Forms.TextBox();
            this.BrowseImagesTextBox = new System.Windows.Forms.TextBox();
            this.browseImagesButton = new System.Windows.Forms.Button();
            this.browseFoldersButton = new System.Windows.Forms.Button();
            this.BrowseFoldersTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectTxtFileLabel = new System.Windows.Forms.Label();
            this.BrowseImagesLabel = new System.Windows.Forms.Label();
            this.BrowseFoldersLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.showImageComparisonButton = new System.Windows.Forms.Button();
            this.showHistogramComparisonButton = new System.Windows.Forms.Button();
            this.createStegoImageButton = new System.Windows.Forms.Button();
            this.lsbCountUsed = new System.Windows.Forms.NumericUpDown();
            this.lsbCountUsedLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.encryptImageLabel = new System.Windows.Forms.Label();
            this.encryptImageCheckBox = new System.Windows.Forms.CheckBox();
            this.operationResultLabel = new System.Windows.Forms.Label();
            this.advancedOptions = new System.Windows.Forms.GroupBox();
            this.loopMessageRadioButton = new System.Windows.Forms.RadioButton();
            this.compressMessageRadioButton = new System.Windows.Forms.RadioButton();
            this.noneRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.lsbCountUsed)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            this.openFileDialog1.Title = "Select input txt";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "png files (*.png)|*.png| bmp files (*.bmp)|*.bmp";
            this.openFileDialog2.Title = "Select cover image";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // TitleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(10, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(200, 30);
            this.titleLabel.TabIndex = 18;
            this.titleLabel.Text = "titleLabel";
            // 
            // SelectTxtFileLabel
            // 
            this.SelectTxtFileLabel.AutoSize = true;
            this.SelectTxtFileLabel.Font = new System.Drawing.Font("Calibri", 10F);
            this.SelectTxtFileLabel.Location = new System.Drawing.Point(10, 50);
            this.SelectTxtFileLabel.Name = "SelectTxtFileLabel";
            this.SelectTxtFileLabel.Size = new System.Drawing.Size(200, 15);
            this.SelectTxtFileLabel.TabIndex = 7;
            this.SelectTxtFileLabel.Text = "Select txt file that contains stego message:";
            // 
            // BrowseTxtFilesTextBox
            // 
            this.BrowseTxtFilesTextBox.Location = new System.Drawing.Point(10, 70);
            this.BrowseTxtFilesTextBox.Name = "BrowseTxtFilesTextBox";
            this.BrowseTxtFilesTextBox.ReadOnly = true;
            this.BrowseTxtFilesTextBox.Size = new System.Drawing.Size(300, 20);
            this.BrowseTxtFilesTextBox.TabIndex = 2;
            // 
            // BrowseTxtFilesButton
            // 
            this.browseTxtFilesButton.Location = new System.Drawing.Point(315, 70);
            this.browseTxtFilesButton.Name = "browseTxtFilesButton";
            this.browseTxtFilesButton.Size = new System.Drawing.Size(75, 20);
            this.browseTxtFilesButton.TabIndex = 1;
            this.browseTxtFilesButton.Text = "Browse files";
            this.browseTxtFilesButton.UseVisualStyleBackColor = true;
            this.browseTxtFilesButton.Click += new System.EventHandler(this.BrowseTxtFilesButton_Click);
            // 
            // BrowseImagesLabel
            // 
            this.BrowseImagesLabel.AutoSize = true;
            this.BrowseImagesLabel.Font = new System.Drawing.Font("Calibri", 10F);
            this.BrowseImagesLabel.Location = new System.Drawing.Point(10, 100);
            this.BrowseImagesLabel.Name = "BrowseImagesLabel";
            this.BrowseImagesLabel.Size = new System.Drawing.Size(200, 15);
            this.BrowseImagesLabel.TabIndex = 8;
            this.BrowseImagesLabel.Text = "Select a cover picture for steganography:";
            // 
            // BrowseImagesButton
            // 
            this.browseImagesButton.Location = new System.Drawing.Point(315, 120);
            this.browseImagesButton.Name = "BrowseImagesButton";
            this.browseImagesButton.Size = new System.Drawing.Size(75, 20);
            this.browseImagesButton.TabIndex = 4;
            this.browseImagesButton.Text = "Browse files";
            this.browseImagesButton.UseVisualStyleBackColor = true;
            this.browseImagesButton.Click += new System.EventHandler(this.BrowseImagesButton_Click);
            // 
            // BrowseImagesTextBox
            // 
            this.BrowseImagesTextBox.Location = new System.Drawing.Point(10, 120);
            this.BrowseImagesTextBox.Name = "BrowseImagesTextBox";
            this.BrowseImagesTextBox.ReadOnly = true;
            this.BrowseImagesTextBox.Size = new System.Drawing.Size(300, 20);
            this.BrowseImagesTextBox.TabIndex = 3;
            // 
            // BrowseFoldersLabel
            // 
            this.BrowseFoldersLabel.AutoSize = true;
            this.BrowseFoldersLabel.Font = new System.Drawing.Font("Calibri", 10F);
            this.BrowseFoldersLabel.Location = new System.Drawing.Point(10, 150);
            this.BrowseFoldersLabel.Name = "BrowseFoldersLabel";
            this.BrowseFoldersLabel.Size = new System.Drawing.Size(200, 15);
            this.BrowseFoldersLabel.TabIndex = 9;
            this.BrowseFoldersLabel.Text = "Select an output path:";
            // 
            // BrowseFoldersButton
            // 
            this.browseFoldersButton.Location = new System.Drawing.Point(315, 170);
            this.browseFoldersButton.Name = "BrowseFoldersButton";
            this.browseFoldersButton.Size = new System.Drawing.Size(75, 20);
            this.browseFoldersButton.TabIndex = 6;
            this.browseFoldersButton.Text = "Browse files";
            this.browseFoldersButton.UseVisualStyleBackColor = true;
            this.browseFoldersButton.Click += new System.EventHandler(this.BrowseFoldersButton_Click);
            // 
            // BrowseFoldersTextBox
            // 
            this.BrowseFoldersTextBox.Location = new System.Drawing.Point(10, 170);
            this.BrowseFoldersTextBox.Name = "BrowseFoldersTextBox";
            this.BrowseFoldersTextBox.ReadOnly = true;
            this.BrowseFoldersTextBox.Size = new System.Drawing.Size(300, 20);
            this.BrowseFoldersTextBox.TabIndex = 5;
            // 
            // AdvancedOptions
            // 
            this.advancedOptions.Controls.Add(this.lsbCountUsedLabel);
            this.advancedOptions.Controls.Add(this.lsbCountUsed);
            this.advancedOptions.Controls.Add(this.encryptImageLabel);
            this.advancedOptions.Controls.Add(this.encryptImageCheckBox);
            this.advancedOptions.Controls.Add(this.loopMessageRadioButton);
            this.advancedOptions.Controls.Add(this.compressMessageRadioButton);
            this.advancedOptions.Controls.Add(this.noneRadioButton);
            this.advancedOptions.Location = new System.Drawing.Point(10, 200);
            this.advancedOptions.Name = "AdvancedOptions";
            this.advancedOptions.Size = new System.Drawing.Size(375, 160);
            this.advancedOptions.TabIndex = 2;
            this.advancedOptions.TabStop = false;
            this.advancedOptions.Text = "Advanced Options";
            // 
            // lsbCountUsed
            // 
            this.lsbCountUsed.Location = new System.Drawing.Point(10, 25);
            this.lsbCountUsed.Font = new System.Drawing.Font("Arial", 8F);
            this.lsbCountUsed.Maximum = 7;
            this.lsbCountUsed.Minimum = 1;
            this.lsbCountUsed.Name = "lsbCountUsed";
            this.lsbCountUsed.ReadOnly = true;
            this.lsbCountUsed.Size = new System.Drawing.Size(50, 5);
            this.lsbCountUsed.TabIndex = 16;
            this.lsbCountUsed.Value = 1;
            // 
            // lsbCountUsedLabel
            // 
            this.lsbCountUsedLabel.AutoSize = true;
            this.lsbCountUsedLabel.Font = new System.Drawing.Font("Arial", 8F);
            this.lsbCountUsedLabel.Location = new System.Drawing.Point(60, 27);
            this.lsbCountUsedLabel.Name = "lsbCountUsedLabel";
            this.lsbCountUsedLabel.Size = new System.Drawing.Size(400, 10);
            this.lsbCountUsedLabel.TabIndex = 17;
            this.lsbCountUsedLabel.Text = "Number of least significant bits that will be used";
            // 
            // NoneRadioButton
            // 
            this.noneRadioButton.AutoSize = true;
            this.noneRadioButton.Location = new System.Drawing.Point(10, 60);
            this.noneRadioButton.Font = new System.Drawing.Font("Arial", 8F);
            this.noneRadioButton.Name = "NoneRadioButton";
            this.noneRadioButton.TabIndex = 0;
            this.noneRadioButton.TabStop = true;
            this.noneRadioButton.Text = "None";
            this.noneRadioButton.UseVisualStyleBackColor = true;
            this.noneRadioButton.Checked = true;
            // 
            // LoopMessageRadioButton
            // 
            this.loopMessageRadioButton.AutoSize = true;
            this.loopMessageRadioButton.Location = new System.Drawing.Point(10, 80);
            this.loopMessageRadioButton.Font = new System.Drawing.Font("Arial", 8F);
            this.loopMessageRadioButton.Name = "LoopMessageRadioButton";
            this.loopMessageRadioButton.TabIndex = 0;
            this.loopMessageRadioButton.TabStop = true;
            this.loopMessageRadioButton.Text = "Loop message across whole image";
            this.loopMessageRadioButton.UseVisualStyleBackColor = true;
            // 
            // CompressMessageRadioButton
            // 
            this.compressMessageRadioButton.AutoSize = true;
            this.compressMessageRadioButton.Location = new System.Drawing.Point(10, 100);
            this.compressMessageRadioButton.Font = new System.Drawing.Font("Arial", 8F);
            this.compressMessageRadioButton.Name = "CompressMessageRadioButton";
            this.compressMessageRadioButton.TabIndex = 1;
            this.compressMessageRadioButton.TabStop = true;
            this.compressMessageRadioButton.Text = "Compress message before embedding";
            this.compressMessageRadioButton.UseVisualStyleBackColor = true;
            // 
            // EncryptImageLabelCheckBox
            // 
            this.encryptImageCheckBox.Location = new System.Drawing.Point(10, 134);
            this.encryptImageCheckBox.Name = "EncryptImageLabelCheckBox";
            this.encryptImageCheckBox.Size = new System.Drawing.Size(75, 20);
            this.encryptImageCheckBox.TabIndex = 16;
            this.encryptImageCheckBox.Checked = false;
            // 
            // EncryptImageLabel
            // 
            this.encryptImageLabel.AutoSize = true;
            this.encryptImageLabel.Font = new System.Drawing.Font("Arial", 8F);
            this.encryptImageLabel.Location = new System.Drawing.Point(25, 136);
            this.encryptImageLabel.Name = "EncryptImageLabel";
            this.encryptImageLabel.Size = new System.Drawing.Size(400, 20);
            this.encryptImageLabel.TabIndex = 17;
            this.encryptImageLabel.Text = "Check if you want to encrypt message before embedding";
            // 
            // showImageComparisonButton
            // 
            this.showImageComparisonButton.Enabled = false;
            this.showImageComparisonButton.Location = new System.Drawing.Point(40, 380);
            this.showImageComparisonButton.Name = "showImageComparisonButton";
            this.showImageComparisonButton.Size = new System.Drawing.Size(100, 50);
            this.showImageComparisonButton.TabIndex = 12;
            this.showImageComparisonButton.Text = "Show image comparison";
            this.showImageComparisonButton.UseVisualStyleBackColor = true;
            this.showImageComparisonButton.Click += new System.EventHandler(this.ShowImageComparisonButton_Click);
            // 
            // showHistogramComparisonButton
            // 
            this.showHistogramComparisonButton.Enabled = false;
            this.showHistogramComparisonButton.Location = new System.Drawing.Point(150, 380);
            this.showHistogramComparisonButton.Name = "showHistogramComparisonButton";
            this.showHistogramComparisonButton.Size = new System.Drawing.Size(100, 50);
            this.showHistogramComparisonButton.TabIndex = 13;
            this.showHistogramComparisonButton.Text = "Show histogram comparison";
            this.showHistogramComparisonButton.UseVisualStyleBackColor = true;
            this.showHistogramComparisonButton.Click += new System.EventHandler(this.ShowHistogramComparisonButton_Click);
            // 
            // createStegoImageButton
            // 
            this.createStegoImageButton.Location = new System.Drawing.Point(260, 380);
            this.createStegoImageButton.Name = "createStegoImageButton";
            this.createStegoImageButton.Size = new System.Drawing.Size(100, 50);
            this.createStegoImageButton.TabIndex = 14;
            this.createStegoImageButton.Text = "Create a Stego Image";
            this.createStegoImageButton.UseVisualStyleBackColor = true;
            this.createStegoImageButton.Click += new System.EventHandler(this.CreateStegoImageButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar.Location = new System.Drawing.Point(10, 450);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(375, 20);
            this.progressBar.TabIndex = 10;
            // 
            // operationResultLabel
            // 
            this.operationResultLabel.AutoSize = false;
            this.operationResultLabel.ForeColor = System.Drawing.Color.Green;
            this.operationResultLabel.Location = new System.Drawing.Point(10, 480);
            this.operationResultLabel.Name = "operationResultLabel";
            this.operationResultLabel.Size = new System.Drawing.Size(375, 60);
            this.operationResultLabel.TabIndex = 15;
            this.operationResultLabel.Font = new System.Drawing.Font("Calibri", 7F);
            // 
            // EncryptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 540); 
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.operationResultLabel);
            this.Controls.Add(this.createStegoImageButton);
            this.Controls.Add(this.showHistogramComparisonButton);
            this.Controls.Add(this.showImageComparisonButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.BrowseFoldersLabel);
            this.Controls.Add(this.BrowseImagesLabel);
            this.Controls.Add(this.SelectTxtFileLabel);
            this.Controls.Add(this.browseFoldersButton);
            this.Controls.Add(this.BrowseFoldersTextBox);
            this.Controls.Add(this.browseImagesButton);
            this.Controls.Add(this.BrowseImagesTextBox);
            this.Controls.Add(this.BrowseTxtFilesTextBox);
            this.Controls.Add(this.browseTxtFilesButton);
            this.Controls.Add(this.advancedOptions);
            this.advancedOptions.ResumeLayout(false);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "EncryptionForm";
            this.Text = "Encryption Form";
            this.Load += new System.EventHandler(this.EncryptionForm_Load);
            this.Disposed += new System.EventHandler(this.GoBackToPreviousMenu);
            ((System.ComponentModel.ISupportInitialize)(this.lsbCountUsed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button browseTxtFilesButton;
        private System.Windows.Forms.TextBox BrowseTxtFilesTextBox;
        private System.Windows.Forms.TextBox BrowseImagesTextBox;
        private System.Windows.Forms.Button browseImagesButton;
        private System.Windows.Forms.Button browseFoldersButton;
        private System.Windows.Forms.TextBox BrowseFoldersTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label SelectTxtFileLabel;
        private System.Windows.Forms.Label BrowseImagesLabel;
        private System.Windows.Forms.Label BrowseFoldersLabel;
        private System.Windows.Forms.Button showImageComparisonButton;
        private System.Windows.Forms.Button showHistogramComparisonButton;
        private System.Windows.Forms.Button createStegoImageButton;
        private System.Windows.Forms.Label operationResultLabel;
        private System.Windows.Forms.NumericUpDown lsbCountUsed;
        private System.Windows.Forms.Label lsbCountUsedLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label encryptImageLabel;
        private System.Windows.Forms.CheckBox encryptImageCheckBox;
        private System.Windows.Forms.GroupBox advancedOptions;
        private System.Windows.Forms.RadioButton loopMessageRadioButton;
        private System.Windows.Forms.RadioButton compressMessageRadioButton;
        private System.Windows.Forms.RadioButton noneRadioButton;
    }
}