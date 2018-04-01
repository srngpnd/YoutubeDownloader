using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Stopwatch sw;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog.

            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)

            {

                label4.Text = folderDlg.SelectedPath;

                Environment.SpecialFolder root = folderDlg.RootFolder;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool validator = false;
            label11.Text = "";
            label12.Text = "";
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                label11.Text = "Mandatory";
                validator = true;
            }

            if (string.IsNullOrWhiteSpace(label4.Text))
            {
                label12.Text = "Mandatory";
                validator = true;
            }

            if(validator)
            {
                return;
            }

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(textBox1.Text);

            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == Convert.ToInt32(comboBox1.SelectedItem.ToString()));

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */
            var videoDownloader = new VideoDownloader(video, Path.Combine(label4.Text, string.IsNullOrWhiteSpace(textBox2.Text) ? video.Title + video.VideoExtension : textBox2.Text + video.VideoExtension));
            
            // Register the ProgressChanged event and print the current progress
            videoDownloader.DownloadProgressChanged += (s, args) => valueChange(args.ProgressPercentage);
            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();
        }

        public void valueChange(double a)
        {
            progressBar1.Value = (int)a;
            int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
            progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2-10,progressBar1.Height / 2-7 ));
        }
    }
}
