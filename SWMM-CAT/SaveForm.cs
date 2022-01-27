using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SWMM_CAT
{
    public partial class SaveForm : Form
    {
        public int climate_scenario;
        public int extreme_storm_scenario;
        public string inpFile;

        public SaveForm()
        {
            InitializeComponent();
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            inpFile = MainForm.inpFile;
            climate_scenario = MainForm.climate_scenario;
            extreme_storm_scenario = MainForm.extreme_storm_scenario;
            textBox1.Text = inpFile;
            comboBox1.SelectedIndex = 0;
            if (inpFile.Length > 0)
            {
                textBox1.Enabled = false;
                button1.Visible = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) checkBox4.Checked = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) checkBox3.Checked = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(inpFile))
            {
                MessageBox.Show(this, "The designated SWMM file does not exist.", "File Name Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            bool[] savedAdjustments = new bool[4];
            int returnPeriodIndex = comboBox1.SelectedIndex;
            savedAdjustments[0] = checkBox1.Checked;
            savedAdjustments[1] = checkBox2.Checked;
            savedAdjustments[2] = checkBox3.Checked;
            savedAdjustments[3] = checkBox4.Checked;
            Climate.SaveAdjustments(inpFile, climate_scenario, extreme_storm_scenario, savedAdjustments, returnPeriodIndex);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a SWMM Input File";
            openFileDialog1.Filter = "SWMM input files (*.inp)|*.inp|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = textBox1.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                inpFile = openFileDialog1.FileName;
                textBox1.Text = inpFile;
            }
        }
    }
}
