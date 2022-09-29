using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ZedGraph;

namespace SWMM_CAT
{
    public partial class MainForm : Form
    {
        public static double lat;
        public static double lng;
        public static string inpFile;
        public static string precipID;
        public static string stormID;
        public static string evapID;
        public static string tempID;
        public static int climate_scenario;
        public static int extreme_storm_scenario;
        public static int climateYear;
        public static bool idFound;

        public static string tempPlotTitle = "Change in Monthly Temperature (deg. C)";
        public static string evapPlotTitle = "Change in Monthly Evaporation Rate (in/day)";
        public static string rainPlotTitle = "Percentage Change in Monthly Rainfall";
        public static string xrainPlotTitle = "Percentage Change in 24-Hour Design Storm";
        public static string timePeriodTitle;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            inpFile = "";
            if (args.Length > 1)
            {
                inpFile = args[1];
                if (!File.Exists(inpFile)) inpFile = "";
            }
            linkLabel1.Enabled = false;
            climateYear = 2035;
            timePeriodTitle = "Near Term ";
            idFound = false;
            Graphs.CreateMonthlyTempPlot(zedGraphControl1);
            Graphs.CreateMonthlyEvapPlot(zedGraphControl2);
            Graphs.CreateMonthlyRainPlot(zedGraphControl3);
            Graphs.CreateXrainPlot(zedGraphControl4);
            helpTextBox.Text = SWMM_CAT.Properties.Resources.help;
            locationBox.Focus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (radioButton1.Checked) climate_scenario = 1;
            else if (radioButton2.Checked) climate_scenario = 2;
            else climate_scenario = 3;

            if (radioButton7.Checked) extreme_storm_scenario = 1;
            else if (radioButton8.Checked) extreme_storm_scenario = 2;
            else extreme_storm_scenario = 0;

            SaveForm saveForm = new SaveForm();
            if (saveForm.ShowDialog() == DialogResult.OK) Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void locationBox_Enter(object sender, EventArgs e)
        {
            locationBox.SelectAll();
        }

        private void locationBox_Click(object sender, EventArgs e)
        {
            locationBox.SelectAll();
        }

        private void locationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) GetClimateAdjustments(locationBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetClimateAdjustments(locationBox.Text);
        }

        // Switches time period for display of CMIP3 projections
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                climateYear = 2035;
                timePeriodTitle = "Near Term ";
            }
            else
            {
                climateYear = 2060;
                timePeriodTitle = "Far Term ";
            }
            FindClimateAdjustments();
        }

        private void GetClimateAdjustments(string location)
        {
            ClearGraphs();
            idFound = false;
            linkLabel1.Enabled = false;
            if (FindLatLng(location) == false) return;
            if (FindPrecipID() == false) return;
            if (FindStormID() == false) return;
            if (FindEvapID() == false) return;
            idFound = true;
            FindClimateAdjustments();
            linkLabel1.Enabled = true;
        }

        private bool FindLatLng(string location)
        {
            string[] values = location.Split(new Char[] { ' ', ',', ':', '\t' },
                                             StringSplitOptions.RemoveEmptyEntries);
            if (values.Length == 1)
            {
                if (ZipToLatLng(values[0]) == false) return false;
                locationBox.Text = lat.ToString() + "," + lng.ToString();
                return true;
            }
            else if (values.Length == 2)
            {
                try
                {
                    lat = Double.Parse(values[0]);
                    lng = Double.Parse(values[1]);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lat-Long Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            MessageBox.Show("Cannot parse lat-long location.", "Lat-Long Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private bool ZipToLatLng(string zipcode)
        {
            if (zipcode.Length != 5)
            {
                MessageBox.Show("Zip code is not 5 digits.",
                     "Zip Code Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string line;
            try
            {
                using (StringReader sr = new StringReader(SWMM_CAT.Properties.Resources.zipcode))
                {
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        if (line.StartsWith(zipcode))
                        {
                            string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                            if (values.Length > 2)
                            {
                                lat = Double.Parse(values[1]);
                                lng = Double.Parse(values[2]);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Zip Code Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            MessageBox.Show("Cannot find any lat-long coordinates for this zip code.",
                "Zip Code Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private bool FindPrecipID()
        {
            double lat1, lng1, dist, dx, dy;
            string line;
            try
            {
                double minDist = 1.0e20;
                using (StringReader sr = new StringReader(SWMM_CAT.Properties.Resources.precip))
                {
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length < 3) continue;
                        if (!Double.TryParse(values[1], out lat1)) continue;
                        if (!Double.TryParse(values[2], out lng1)) continue;
                        dx = lng - lng1;
                        dy = lat - lat1;
                        dist = (dx * dx) + (dy * dy);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            precipID = values[0];
                        }
                    }
                }
                // Precip stations all have 6 characters padded in front with 0's.
                while (precipID.Length < 6) precipID = "0" + precipID;
                tempID = precipID;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Read Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool FindStormID()
        {
            double lat1, lng1, dist, dx, dy;
            string line;
            try
            {
                double minDist = 1.0e20;
                using (StringReader sr = new StringReader(SWMM_CAT.Properties.Resources.storm))
                {
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length < 3) continue;
                        if (!Double.TryParse(values[1], out lat1)) continue;
                        if (!Double.TryParse(values[2], out lng1)) continue;
                        dx = lng - lng1;
                        dy = lat - lat1;
                        dist = (dx * dx) + (dy * dy);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            stormID = values[0];
                        }
                    }
                }
                // Precip stations all have 6 characters padded in front with 0's.
                while (stormID.Length < 6) stormID = "0" + stormID;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Read Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool FindEvapID()
        {
            double lat1, lng1, dist, dx, dy;
            string line;
            try
            {
                double minDist = 1.0e20;
                using (StringReader sr = new StringReader(SWMM_CAT.Properties.Resources.evap))
                {
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length < 3) continue;
                        if (!Double.TryParse(values[1], out lat1)) continue;
                        if (!Double.TryParse(values[2], out lng1)) continue;
                        dx = lng - lng1;
                        dy = lat - lat1;
                        dist = (dx * dx) + (dy * dy);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            evapID = values[0];
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Read Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClearGraphs()
        {
            Graphs.Clear(zedGraphControl1);
            Graphs.Clear(zedGraphControl2);
            Graphs.Clear(zedGraphControl3);
            Graphs.Clear(zedGraphControl4);
        }

        private void FindClimateAdjustments()
        {
            string title;
            if (idFound) Climate.UpdateAdjustments();
            title = timePeriodTitle + tempPlotTitle;
            Graphs.Refresh(zedGraphControl1, title);
            title = timePeriodTitle + evapPlotTitle;
            Graphs.Refresh(zedGraphControl2, title);
            title = timePeriodTitle + rainPlotTitle;
            Graphs.Refresh(zedGraphControl3, title);
            title = timePeriodTitle + xrainPlotTitle;
            Graphs.Refresh(zedGraphControl4, title);
        }

	private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
