namespace CleanUpScriptBuilder
{
    partial class CleanUpScriptBuilderForm
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
            this.rtaNumberBox = new System.Windows.Forms.TextBox();
            this.rtaLabel = new System.Windows.Forms.Label();
            this.idFileOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.submitButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.fileNameBox = new System.Windows.Forms.TextBox();
            this.lineCountLabel = new System.Windows.Forms.Label();
            this.lineCountTitleLabel = new System.Windows.Forms.Label();
            this.outputDirBox = new System.Windows.Forms.TextBox();
            this.outputDirButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.selectOutputDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.debugBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sqlTemplateBox = new System.Windows.Forms.TextBox();
            this.sqlTemplateButton = new System.Windows.Forms.Button();
            this.templateOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileReplaceCheckBox = new System.Windows.Forms.CheckBox();
            this.idsPerFileBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.NewsetterCheckBox = new System.Windows.Forms.CheckBox();
            this.MergeFilesBox = new System.Windows.Forms.CheckBox();
            this.EmailCheckBox = new System.Windows.Forms.CheckBox();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // rtaNumberBox
            // 
            this.rtaNumberBox.Location = new System.Drawing.Point(88, 18);
            this.rtaNumberBox.Name = "rtaNumberBox";
            this.rtaNumberBox.Size = new System.Drawing.Size(75, 20);
            this.rtaNumberBox.TabIndex = 0;
            // 
            // rtaLabel
            // 
            this.rtaLabel.AutoSize = true;
            this.rtaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtaLabel.Location = new System.Drawing.Point(9, 18);
            this.rtaLabel.Name = "rtaLabel";
            this.rtaLabel.Size = new System.Drawing.Size(72, 13);
            this.rtaLabel.TabIndex = 1;
            this.rtaLabel.Text = "RTA Number:";
            // 
            // idFileOpenFileDialog
            // 
            this.idFileOpenFileDialog.InitialDirectory = "Desktop";
            this.idFileOpenFileDialog.Title = "Select ID File";
            this.idFileOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.IDFile_FileOk);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(16, 360);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.TabIndex = 2;
            this.submitButton.Text = "Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(88, 49);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(75, 23);
            this.openFileButton.TabIndex = 3;
            this.openFileButton.Text = "Browse";
            this.openFileButton.UseVisualStyleBackColor = true;
            // 
            // fileNameBox
            // 
            this.fileNameBox.Location = new System.Drawing.Point(282, 49);
            this.fileNameBox.Name = "fileNameBox";
            this.fileNameBox.Size = new System.Drawing.Size(320, 20);
            this.fileNameBox.TabIndex = 4;
            // 
            // lineCountLabel
            // 
            this.lineCountLabel.AutoSize = true;
            this.lineCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineCountLabel.ForeColor = System.Drawing.Color.Blue;
            this.lineCountLabel.Location = new System.Drawing.Point(231, 52);
            this.lineCountLabel.Name = "lineCountLabel";
            this.lineCountLabel.Size = new System.Drawing.Size(14, 15);
            this.lineCountLabel.TabIndex = 6;
            this.lineCountLabel.Text = "0";
            // 
            // lineCountTitleLabel
            // 
            this.lineCountTitleLabel.AutoSize = true;
            this.lineCountTitleLabel.Location = new System.Drawing.Point(176, 54);
            this.lineCountTitleLabel.Name = "lineCountTitleLabel";
            this.lineCountTitleLabel.Size = new System.Drawing.Size(49, 13);
            this.lineCountTitleLabel.TabIndex = 7;
            this.lineCountTitleLabel.Text = "ID Count";
            // 
            // outputDirBox
            // 
            this.outputDirBox.Location = new System.Drawing.Point(179, 80);
            this.outputDirBox.Name = "outputDirBox";
            this.outputDirBox.Size = new System.Drawing.Size(423, 20);
            this.outputDirBox.TabIndex = 8;
            // 
            // outputDirButton
            // 
            this.outputDirButton.Location = new System.Drawing.Point(88, 80);
            this.outputDirButton.Name = "outputDirButton";
            this.outputDirButton.Size = new System.Drawing.Size(75, 23);
            this.outputDirButton.TabIndex = 9;
            this.outputDirButton.Text = "Browse";
            this.outputDirButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "ID File (*.csv):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Output Dir:";
            // 
            // selectOutputDirDialog
            // 
            this.selectOutputDirDialog.HelpRequest += new System.EventHandler(this.folderBrowserDialog1_HelpRequest);
            // 
            // debugBox
            // 
            this.debugBox.Location = new System.Drawing.Point(114, 169);
            this.debugBox.Multiline = true;
            this.debugBox.Name = "debugBox";
            this.debugBox.Size = new System.Drawing.Size(488, 214);
            this.debugBox.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "SQL Template:";
            // 
            // sqlTemplateBox
            // 
            this.sqlTemplateBox.Location = new System.Drawing.Point(179, 109);
            this.sqlTemplateBox.Name = "sqlTemplateBox";
            this.sqlTemplateBox.Size = new System.Drawing.Size(423, 20);
            this.sqlTemplateBox.TabIndex = 14;
            // 
            // sqlTemplateButton
            // 
            this.sqlTemplateButton.Location = new System.Drawing.Point(88, 107);
            this.sqlTemplateButton.Name = "sqlTemplateButton";
            this.sqlTemplateButton.Size = new System.Drawing.Size(75, 23);
            this.sqlTemplateButton.TabIndex = 15;
            this.sqlTemplateButton.Text = "Browse";
            this.sqlTemplateButton.UseVisualStyleBackColor = true;
            // 
            // templateOpenFileDialog
            // 
            this.templateOpenFileDialog.Title = "Select SQL Template location";
            this.templateOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.templateOpenFileDialog_FileOk);
            // 
            // fileReplaceCheckBox
            // 
            this.fileReplaceCheckBox.AutoSize = true;
            this.fileReplaceCheckBox.Location = new System.Drawing.Point(12, 182);
            this.fileReplaceCheckBox.Name = "fileReplaceCheckBox";
            this.fileReplaceCheckBox.Size = new System.Drawing.Size(96, 17);
            this.fileReplaceCheckBox.TabIndex = 16;
            this.fileReplaceCheckBox.Text = "Replace Files?";
            this.fileReplaceCheckBox.UseVisualStyleBackColor = true;
            // 
            // idsPerFileBox
            // 
            this.idsPerFileBox.Location = new System.Drawing.Point(88, 140);
            this.idsPerFileBox.Name = "idsPerFileBox";
            this.idsPerFileBox.Size = new System.Drawing.Size(75, 20);
            this.idsPerFileBox.TabIndex = 17;
            this.idsPerFileBox.Text = "20000";
            this.idsPerFileBox.TextChanged += new System.EventHandler(this.idsPerFileBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "IDs Per File:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(176, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Current Status: ";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.DarkGreen;
            this.statusLabel.Location = new System.Drawing.Point(288, 16);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(73, 15);
            this.statusLabel.TabIndex = 20;
            this.statusLabel.Text = "Setting Up...";
            // 
            // NewsetterCheckBox
            // 
            this.NewsetterCheckBox.AutoCheck = false;
            this.NewsetterCheckBox.AutoSize = true;
            this.NewsetterCheckBox.Location = new System.Drawing.Point(13, 269);
            this.NewsetterCheckBox.Name = "NewsetterCheckBox";
            this.NewsetterCheckBox.Size = new System.Drawing.Size(74, 17);
            this.NewsetterCheckBox.TabIndex = 21;
            this.NewsetterCheckBox.Text = "Newsetter";
            this.NewsetterCheckBox.UseVisualStyleBackColor = true;
            this.NewsetterCheckBox.CheckedChanged += new System.EventHandler(this.NewsetterCheckBox_CheckedChanged);
            // 
            // MergeFilesBox
            // 
            this.MergeFilesBox.AutoSize = true;
            this.MergeFilesBox.Location = new System.Drawing.Point(12, 224);
            this.MergeFilesBox.Name = "MergeFilesBox";
            this.MergeFilesBox.Size = new System.Drawing.Size(86, 17);
            this.MergeFilesBox.TabIndex = 22;
            this.MergeFilesBox.Text = "Merge Files?";
            this.MergeFilesBox.UseVisualStyleBackColor = true;
            this.MergeFilesBox.CheckedChanged += new System.EventHandler(this.MergeFilesBox_CheckedChanged);
            // 
            // EmailCheckBox
            // 
            this.EmailCheckBox.AutoCheck = false;
            this.EmailCheckBox.AutoSize = true;
            this.EmailCheckBox.Location = new System.Drawing.Point(12, 318);
            this.EmailCheckBox.Name = "EmailCheckBox";
            this.EmailCheckBox.Size = new System.Drawing.Size(101, 17);
            this.EmailCheckBox.TabIndex = 23;
            this.EmailCheckBox.Text = "Email Activation";
            this.EmailCheckBox.UseVisualStyleBackColor = true;
            this.EmailCheckBox.CheckedChanged += new System.EventHandler(this.EmailCheckBox_CheckedChanged);
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(493, 15);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(100, 23);
            this.pBar1.TabIndex = 24;
            // 
            // CleanUpScriptBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 405);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.EmailCheckBox);
            this.Controls.Add(this.MergeFilesBox);
            this.Controls.Add(this.NewsetterCheckBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.idsPerFileBox);
            this.Controls.Add(this.fileReplaceCheckBox);
            this.Controls.Add(this.sqlTemplateButton);
            this.Controls.Add(this.sqlTemplateBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.debugBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputDirButton);
            this.Controls.Add(this.outputDirBox);
            this.Controls.Add(this.lineCountTitleLabel);
            this.Controls.Add(this.lineCountLabel);
            this.Controls.Add(this.fileNameBox);
            this.Controls.Add(this.openFileButton);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.rtaLabel);
            this.Controls.Add(this.rtaNumberBox);
            this.Name = "CleanUpScriptBuilderForm";
            this.Text = "CleanUp Script Builder V 2.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox rtaNumberBox;
        private System.Windows.Forms.Label rtaLabel;
        private System.Windows.Forms.OpenFileDialog idFileOpenFileDialog;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.TextBox fileNameBox;
        private System.Windows.Forms.Label lineCountLabel;
        private System.Windows.Forms.Label lineCountTitleLabel;
        private System.Windows.Forms.TextBox outputDirBox;
        private System.Windows.Forms.Button outputDirButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog selectOutputDirDialog;
        private System.Windows.Forms.TextBox debugBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sqlTemplateBox;
        private System.Windows.Forms.Button sqlTemplateButton;
        private System.Windows.Forms.OpenFileDialog templateOpenFileDialog;
        private System.Windows.Forms.CheckBox fileReplaceCheckBox;
        private System.Windows.Forms.TextBox idsPerFileBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.CheckBox NewsetterCheckBox;
        private System.Windows.Forms.CheckBox MergeFilesBox;
        private System.Windows.Forms.CheckBox EmailCheckBox;
        private System.Windows.Forms.ProgressBar pBar1;
    }
}

