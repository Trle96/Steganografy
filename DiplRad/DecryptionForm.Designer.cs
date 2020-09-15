namespace DiplRad
{
    partial class DecryptionForm
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
            this.browseImageTextBox = new System.Windows.Forms.TextBox();
            this.browseImageButton = new System.Windows.Forms.Button();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.browseFolderTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.browseImageLabel = new System.Windows.Forms.Label();
            this.browseFolderLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.decryptStegoImageButton = new System.Windows.Forms.Button();
            this.resultOutputLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.showEncryptedTextButton = new System.Windows.Forms.Button();
            this.encryptMessageLabel = new System.Windows.Forms.Label();
            this.encryptMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.advancedOptions = new System.Windows.Forms.GroupBox();
            this.compressedMessageLabel = new System.Windows.Forms.Label();
            this.compressedMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.lsbCountUsed = new System.Windows.Forms.NumericUpDown();
            this.lsbCountUsedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.lsbCountUsed)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Title = "Select cover image";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(10, 10);
            this.titleLabel.Name = "TitleLabel";
            this.titleLabel.Size = new System.Drawing.Size(200, 30);
            this.titleLabel.TabIndex = 18;
            this.titleLabel.Text = "TitleLabel";
            // 
            // BrowseImageLabel
            // 
            this.browseImageLabel.AutoSize = true;
            this.browseImageLabel.Font = new System.Drawing.Font("Calibri", 10F);
            this.browseImageLabel.Location = new System.Drawing.Point(10, 50);
            this.browseImageLabel.Name = "BrowseImageLabel";
            this.browseImageLabel.Size = new System.Drawing.Size(200, 15);
            this.browseImageLabel.TabIndex = 8;
            this.browseImageLabel.Text = "Select a picture you want to decrypt";
            // 
            // BrowseImageTextBox
            // 
            this.browseImageTextBox.Location = new System.Drawing.Point(10, 70);
            this.browseImageTextBox.Name = "BrowseImageTextBox";
            this.browseImageTextBox.ReadOnly = true;
            this.browseImageTextBox.Size = new System.Drawing.Size(300, 20);
            this.browseImageTextBox.TabIndex = 3;
            // 
            // BrowseImageButton
            // 
            this.browseImageButton.Location = new System.Drawing.Point(315, 70);
            this.browseImageButton.Name = "BrowseImageButton";
            this.browseImageButton.Size = new System.Drawing.Size(75, 20);
            this.browseImageButton.TabIndex = 4;
            this.browseImageButton.Text = "Browse files";
            this.browseImageButton.UseVisualStyleBackColor = true;
            this.browseImageButton.Click += new System.EventHandler(this.OpenSelectPictureFileDialogClicked);
            // 
            // BrowseFolderLabel
            // 
            this.browseFolderLabel.AutoSize = true;
            this.browseFolderLabel.Font = new System.Drawing.Font("Calibri", 10F);
            this.browseFolderLabel.Location = new System.Drawing.Point(10, 100);
            this.browseFolderLabel.Name = "BrowseFolderLabel";
            this.browseFolderLabel.Size = new System.Drawing.Size(200, 15);
            this.browseFolderLabel.TabIndex = 9;
            this.browseFolderLabel.Text = "Select an output path";
            // 
            // BrowseFolderTextBox
            // 
            this.browseFolderTextBox.Location = new System.Drawing.Point(10, 120);
            this.browseFolderTextBox.Name = "BrowseFolderTextBox";
            this.browseFolderTextBox.ReadOnly = true;
            this.browseFolderTextBox.Size = new System.Drawing.Size(300, 20);
            this.browseFolderTextBox.TabIndex = 5;
            // 
            // BrowseFolderButton
            // 
            this.browseFolderButton.Location = new System.Drawing.Point(315, 120);
            this.browseFolderButton.Name = "BrowseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(75, 20);
            this.browseFolderButton.TabIndex = 6;
            this.browseFolderButton.Text = "Browse files";
            this.browseFolderButton.UseVisualStyleBackColor = true;
            this.browseFolderButton.Click += new System.EventHandler(this.SelectOutputPathClicked);
            // 
            // AdvancedOptions
            // 
            this.advancedOptions.Controls.Add(this.lsbCountUsedLabel);
            this.advancedOptions.Controls.Add(this.lsbCountUsed);
            this.advancedOptions.Controls.Add(this.encryptMessageLabel);
            this.advancedOptions.Controls.Add(this.encryptMessageCheckBox);
            this.advancedOptions.Controls.Add(this.compressedMessageLabel);
            this.advancedOptions.Controls.Add(this.compressedMessageCheckBox);
            this.advancedOptions.Location = new System.Drawing.Point(10, 150);
            this.advancedOptions.Name = "AdvancedOptions";
            this.advancedOptions.Size = new System.Drawing.Size(375, 110);
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
            // CompressedMessageCheckBox
            // 
            this.compressedMessageCheckBox.Location = new System.Drawing.Point(10, 54);
            this.compressedMessageCheckBox.Name = "CompressedMessageCheckBox";
            this.compressedMessageCheckBox.Size = new System.Drawing.Size(75, 20);
            this.compressedMessageCheckBox.TabIndex = 16;
            this.compressedMessageCheckBox.Checked = false;
            // 
            // CompressedMessageLabel
            // 
            this.compressedMessageLabel.AutoSize = true;
            this.compressedMessageLabel.Font = new System.Drawing.Font("Arial", 8F);
            this.compressedMessageLabel.Location = new System.Drawing.Point(25, 56);
            this.compressedMessageLabel.Name = "CompressedMessageLabel";
            this.compressedMessageLabel.Size = new System.Drawing.Size(400, 20);
            this.compressedMessageLabel.TabIndex = 17;
            this.compressedMessageLabel.Text = "Check if message is compressed before embedding";
            // 
            // EncryptMessageCheckBox
            // 
            this.encryptMessageCheckBox.Location = new System.Drawing.Point(10, 84);
            this.encryptMessageCheckBox.Name = "EncryptMessageCheckBox";
            this.encryptMessageCheckBox.Size = new System.Drawing.Size(75, 20);
            this.encryptMessageCheckBox.TabIndex = 16;
            this.encryptMessageCheckBox.Checked = false;
            // 
            // EncryptMessageLabel
            // 
            this.encryptMessageLabel.AutoSize = true;
            this.encryptMessageLabel.Font = new System.Drawing.Font("Arial", 8F);
            this.encryptMessageLabel.Location = new System.Drawing.Point(25, 86);
            this.encryptMessageLabel.Name = "EncryptMessageLabel";
            this.encryptMessageLabel.Size = new System.Drawing.Size(400, 20);
            this.encryptMessageLabel.TabIndex = 17;
            this.encryptMessageLabel.Text = "Check if message is encrypted before embedding";
            // 
            // DecryptStegoImageButton
            // 
            this.decryptStegoImageButton.Location = new System.Drawing.Point(95, 280);
            this.decryptStegoImageButton.Name = "DecryptStegoImageButton";
            this.decryptStegoImageButton.Size = new System.Drawing.Size(100, 50);
            this.decryptStegoImageButton.TabIndex = 14;
            this.decryptStegoImageButton.Text = "Decrypt Stego Image";
            this.decryptStegoImageButton.UseVisualStyleBackColor = true;
            this.decryptStegoImageButton.Click += new System.EventHandler(this.DecryptStegoImageClicked);
            // 
            // ShowEncryptedTextButton
            // 
            this.showEncryptedTextButton.Enabled = false;
            this.showEncryptedTextButton.Location = new System.Drawing.Point(205, 280);
            this.showEncryptedTextButton.Name = "ShowEncryptedTextButton";
            this.showEncryptedTextButton.Size = new System.Drawing.Size(100, 50);
            this.showEncryptedTextButton.TabIndex = 13;
            this.showEncryptedTextButton.Text = "Show encrypted text";
            this.showEncryptedTextButton.UseVisualStyleBackColor = true;
            this.showEncryptedTextButton.Click += new System.EventHandler(this.OutputTextFormClicked);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(10, 350);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(375, 20);
            this.progressBar.TabIndex = 10;
            // 
            // ResultOutputLabel
            // 
            this.resultOutputLabel.AutoSize = false;
            this.resultOutputLabel.ForeColor = System.Drawing.Color.Green;
            this.resultOutputLabel.Location = new System.Drawing.Point(10, 380);
            this.resultOutputLabel.Name = "ResultOutputLabel";
            this.resultOutputLabel.Size = new System.Drawing.Size(300, 20);
            this.resultOutputLabel.TabIndex = 15;
            this.resultOutputLabel.Font = new System.Drawing.Font("Calibri", 7F);
            // 
            // DecryptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 420);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.resultOutputLabel);
            this.Controls.Add(this.decryptStegoImageButton);
            this.Controls.Add(this.showEncryptedTextButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.browseFolderLabel);
            this.Controls.Add(this.browseImageLabel);
            this.Controls.Add(this.browseFolderButton);
            this.Controls.Add(this.browseFolderTextBox);
            this.Controls.Add(this.browseImageButton);
            this.Controls.Add(this.browseImageTextBox);
            this.Controls.Add(this.advancedOptions);
            this.advancedOptions.ResumeLayout(false);
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "DecryptionForm";
            this.Text = "Decryption Form";
            this.Load += new System.EventHandler(this.DecryptionForm_Load);
            this.Disposed += new System.EventHandler(this.GoBackToPreviousMenu);
            ((System.ComponentModel.ISupportInitialize)(this.lsbCountUsed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        #endregion
        private System.Windows.Forms.TextBox browseImageTextBox;
        private System.Windows.Forms.Button browseImageButton;
        private System.Windows.Forms.Button browseFolderButton;
        private System.Windows.Forms.TextBox browseFolderTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label browseImageLabel;
        private System.Windows.Forms.Label browseFolderLabel;
        private System.Windows.Forms.Button decryptStegoImageButton;
        private System.Windows.Forms.Label resultOutputLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button showEncryptedTextButton;
        private System.Windows.Forms.Label compressedMessageLabel;
        private System.Windows.Forms.CheckBox compressedMessageCheckBox;
        private System.Windows.Forms.Label encryptMessageLabel;
        private System.Windows.Forms.CheckBox encryptMessageCheckBox;
        private System.Windows.Forms.GroupBox advancedOptions;
        private System.Windows.Forms.NumericUpDown lsbCountUsed;
        private System.Windows.Forms.Label lsbCountUsedLabel;
    }
}