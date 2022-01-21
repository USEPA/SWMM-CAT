using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CREATClimateData;

namespace CreatReader
{
    public partial class Form1 : Form
    {
        public String CREATPath = "C:\\Program Files (x86)\\CREAT 2.0";
        public CREATClimateData.ClimateData climateData;

        public String station;
        public float latitude;
        public float longitude;
        public int year;

        public String[] stations;

        public StreamWriter swHot;
        public StreamWriter swMed;
        public StreamWriter swWet;

        public Form1()
        {
            InitializeComponent();
            climateData = new CREATClimateData.ClimateData();
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String path = Application.StartupPath + "\\evap.txt";
            label1.Text = "";
            textBox1.Clear();
            textBox1.AppendText("\nReading station locations from " + path + "\n");
            stations = File.ReadAllLines(path);
            textBox1.AppendText(stations.Length.ToString() + " stations read.\n");
            textBox1.AppendText("\nReading data for 2035.");
            readCreatData(2035);
            textBox1.AppendText("\nReading data for 2060.");
            readCreatData(2060);
            textBox1.AppendText("Reading CREAT completed.");
        }

        private void readCreatData(int year)
        {
            int nStations = stations.Length;
            int nReads = 0;

            swHot = new StreamWriter("Temp" + year.ToString() + "Hot.txt");
            swMed = new StreamWriter("Temp" + year.ToString() + "Med.txt");
            swWet = new StreamWriter("Temp" + year.ToString() + "Wet.txt");
            String header = "Station \tJan \tFeb \tMar \tApr \tMay \tJun \tJul \tAug \tSep \tOct \tNov \tDec";
            try
            {
                swHot.WriteLine(header);
                swMed.WriteLine(header);
                swWet.WriteLine(header);

                for (int i = 0; i < nStations; i++)
                {
                    string[] words = stations[i].Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length < 3) continue;
                    try
                    {
                        station = words[0];
                        latitude = float.Parse(words[1]);
                        longitude = float.Parse(words[2]);
                        if (readCreatLocation(year)) nReads = nReads + 1;
                        label1.Text = nReads.ToString() + " stations processed.";
                        Application.DoEvents();
                    }
                    catch (Exception e)
                    {
                        textBox1.AppendText(e.Message + "\n");
                        continue;
                    }
                }
            }
            finally
            {
                swWet.Close();
                swMed.Close();
                swHot.Close();
            }

            textBox1.AppendText(nReads.ToString() + " stations processed.\n");
        }

        private bool readCreatLocation(int year)
        {
            String tempLine;
            try
            {
                CREATClimateData.TempPrecipData tempData;
                tempData = climateData.GetCREATTemp(CREATPath, latitude, longitude, year);
                tempLine = getTempLine(tempData.ModelHot);
                swHot.WriteLine(tempLine);
                tempLine = getTempLine(tempData.ModelMedium);
                swMed.WriteLine(tempLine);
                tempLine = getTempLine(tempData.ModelWet);
                swWet.WriteLine(tempLine);
            }
            catch (Exception e)
            {
                textBox1.AppendText(e.Message + "\n");
                return false;
            }
            return true;
        }

        private String getTempLine(TempPrecipData.MonthValues values)
        {
            StringBuilder tempLine = new StringBuilder();
            tempLine.Append(station);
            tempLine.Append("\t");
            tempLine.Append(values.Jan);
            tempLine.Append("\t");
            tempLine.Append(values.Feb);
            tempLine.Append("\t");
            tempLine.Append(values.Mar);
            tempLine.Append("\t");
            tempLine.Append(values.Apr);
            tempLine.Append("\t");
            tempLine.Append(values.May);
            tempLine.Append("\t");
            tempLine.Append(values.Jun);
            tempLine.Append("\t");
            tempLine.Append(values.Jul);
            tempLine.Append("\t");
            tempLine.Append(values.Aug);
            tempLine.Append("\t");
            tempLine.Append(values.Sep);
            tempLine.Append("\t");
            tempLine.Append(values.Oct);
            tempLine.Append("\t");
            tempLine.Append(values.Nov);
            tempLine.Append("\t");
            tempLine.Append(values.Dec);
            return tempLine.ToString();
        }
    }
}
