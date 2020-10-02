using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CameraToDVD
{
    public partial class VideoInfoForm : Form
    {
        public VideoInfoForm()
        {
            InitializeComponent();
        }

        public string Info;

        private void VideoInfoForm_Load(object sender, EventArgs e)
        {
            txtInfo.Text = Info;
        }

        private void VideoInfoForm_Shown(object sender, EventArgs e)
        {
            txtInfo.Select(Info.Length, 0);
        }

        private void VideoInfoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
