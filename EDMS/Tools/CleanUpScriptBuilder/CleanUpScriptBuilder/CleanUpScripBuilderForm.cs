using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CleanUpScriptBuilder
{
    public partial class CleanUpScriptBuilderForm : Form
    {
        private string _rtaNumber = "";
        private List<string> _parsedData = new List<string>();
        private string _template = "";
        public string outputPath = "";
        public string filePattern = "";
        public string destFile = "";

        public CleanUpScriptBuilderForm()
        {
            InitializeComponent();
            this.submitButton.Click += new EventHandler(Submit_Click);
            this.openFileButton.Click += new EventHandler(openFileButton_Click);
            this.outputDirButton.Click += new EventHandler(outputDirButton_Click);
            this.sqlTemplateButton.Click += new EventHandler(sqlTemplateButton_Click);
        }

        void sqlTemplateButton_Click(object sender, EventArgs e)
        {
            String startupPath = Application.StartupPath;
            int lastIndex = startupPath.LastIndexOf("\\bin\\");
            String realPath = startupPath.Substring(0, lastIndex) + "\\template"; ;
            this.templateOpenFileDialog.InitialDirectory = realPath;


            DialogResult result = this.templateOpenFileDialog.ShowDialog();
            StreamReader reader = null;
            if (DialogResult.OK == result)
            {
                this.sqlTemplateBox.Text = this.templateOpenFileDialog.FileName;
                try
                {
                    reader = new StreamReader(this.templateOpenFileDialog.OpenFile());
                    this._template = reader.ReadToEnd();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }
        void openFileButton_Click(object sender, EventArgs e)
        {
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            idFileOpenFileDialog.InitialDirectory = DesktopPath;
            idFileOpenFileDialog.Filter = "Comma Delimited Files|*.csv";

            
            DialogResult result = idFileOpenFileDialog.ShowDialog();
            StreamReader reader = null;
            if (result == DialogResult.OK)
            {
                this.fileNameBox.Text = idFileOpenFileDialog.FileName;
                try
                {
                    reader = new StreamReader(idFileOpenFileDialog.OpenFile());
                    string line;
                    string[] row;
                    int rowOffSet = 0;

                    //Adjusts the offset to accomodate newsletter files
                    if (idFileOpenFileDialog.FileName.Contains("SMC"))
                    {
                        this.NewsetterCheckBox.Checked = true;
                        rowOffSet = 1;
                    }
                    else
                    {
                        this.NewsetterCheckBox.Checked = false;
                        this.EmailCheckBox.Checked = true;
                    }
                    int lineCount = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        if (lineCount == 0)
                        {
                            lineCount++;
                            continue;
                        }
                        //remove double quotes if there

                        string line2 = row[rowOffSet];
                        row[rowOffSet] = line2.Replace("\"", string.Empty);
                        _parsedData.Add(row[rowOffSet]);
                        lineCount++;
                        if (lineCount % 100 == 0)
                        {
                            this.lineCountLabel.Text = lineCount.ToString();
                        }
                    }
                    this.lineCountLabel.Text = lineCount.ToString();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            
        }
        void outputDirButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.selectOutputDirDialog.ShowDialog();

            if (DialogResult.OK == result)
            {
                this.outputDirBox.Text = this.selectOutputDirDialog.SelectedPath;
                outputPath = this.selectOutputDirDialog.SelectedPath;
            }
        }

        void Submit_Click(object sender, EventArgs e)
        {
            this._rtaNumber = this.rtaNumberBox.Text.Trim();
            
            #region Debugging Output
            this.debugBox.AppendText("RTA Number: " + this._rtaNumber + "\n");
            this.debugBox.AppendText("ID Count: " + this._parsedData.Count.ToString() + "\n");
            this.debugBox.AppendText("First ID: " + this._parsedData[0] + "\n");
            this.debugBox.AppendText("Output Dir: " + this.outputDirBox.Text + "\n");
            #endregion

            string sqlBase = (string)this._template.Replace("xxRTA_NUMxx", this._rtaNumber);

            int count = 0;
            int fileCount = 1;
            string idList = "";
            int idsPerFile = 20000;


            // Display the ProgressBar control.
            pBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
            pBar1.ForeColor = Color.Green;

            
            if(this.idsPerFileBox.Text == null || ! int.TryParse(this.idsPerFileBox.Text, out idsPerFile)) {
                idsPerFile = 20000;
            }

            int expFileCount = this._parsedData.Count / idsPerFile;
            // Set Maximum to the total number of files to copy.
            pBar1.Maximum = expFileCount;
            pBar1.Refresh();
            if (this._parsedData.Count % idsPerFile != 0)
            {
                expFileCount++;
            }

            this.statusLabel.Text = "Processing File " + fileCount.ToString() + " of " + expFileCount.ToString();
            this.statusLabel.Refresh();
            int processedCount = 0;
            foreach(string id in this._parsedData) {
                count++;
                processedCount++;
                int total = this._parsedData.Count;
                idList += "'" + id + "'";
                if (count < idsPerFile)
                {
                    if (processedCount != total)
                    {
                        idList += ",\n";
                    }
                }
                else
                {
                    pBar1.Refresh();
                    pBar1.PerformStep();
                    pBar1.Refresh();
                    writeFile(sqlBase, fileCount, idList);


                    //Clean Up
                    fileCount++; 
                    count = 0;
                    idList = "";
                    this.statusLabel.Text = "Processing File " + fileCount.ToString() + " of " + expFileCount.ToString();
                    this.statusLabel.Refresh();
                    continue;
                }
            }

            if (count != 0)
            {
                writeFile(sqlBase, fileCount, idList);
            }
            if (this.MergeFilesBox.Checked == true)
            {
                string fileName = @"RTA_" + this._rtaNumber + "_MRM_MassCommunicationCleanup_";
                mergeFiles(outputPath, filePattern, fileName);
            }
            pBar1.ForeColor = Color.Red;
            pBar1.Refresh();
            this.statusLabel.Text = "Processing Complete for " + fileCount.ToString() + "files";
        }

        private void writeFile(string sqlBase, int fileCount, string idList)
        {
            string sql = sqlBase.Replace("xxID_LISTxx", idList);
            string fileName = @"RTA_" + this._rtaNumber + "_MRM_MassCommunicationCleanup_" + fileCount.ToString("000") + ".sql";
            sql = sql.Replace("xxSCRIPT_NAMExx", fileName);
            string path = this.outputDirBox.Text + @"\" + fileName;

            if (File.Exists(path))
            {
                if (this.fileReplaceCheckBox.Checked)
                {
                    File.Delete(path);
                }
                else
                {
                    int copy = 1;
                    path += "." + copy.ToString();
                    while (File.Exists(path))
                    {
                        copy++;
                        path += "." + copy.ToString();
                    }
                }
            }
            StreamWriter outputfile = new StreamWriter(path);
            outputfile.Write(sql);
            outputfile.Close();
            this.debugBox.AppendText("New SQL File Created @ " + path + "\n");
        }
        private void mergeFiles(string dirPath, string filePattern, string fileName)
        {
            filePattern = fileName + "*";
            destFile = outputPath + "\\" + fileName + DateTime.Today.ToString("yyyyMMdd") + ".sql";
            string[] fileAry = Directory.GetFiles(dirPath, filePattern);

            Console.WriteLine("Total File Count : " + fileAry.Length);

            using (TextWriter tw = new StreamWriter(destFile, true))
            {
                foreach (string filePath in fileAry)
                {
                    using (TextReader tr = new StreamReader(filePath))
                    {
                        tw.WriteLine(tr.ReadToEnd());
                        tr.Close();
                        tr.Dispose();
                    }
                    Console.WriteLine("File Processed : " + filePath);
                }

                tw.Close();
                tw.Dispose();
            }
  
        }
        private void IDFile_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void idsPerFileBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileReplaceCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NewsetterCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void templateOpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void MergeFilesBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void EmailCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

    }
}
