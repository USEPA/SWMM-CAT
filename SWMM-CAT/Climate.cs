using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace SWMM_CAT
{
    class Climate
    {
        // Data series for zedGraphControl1 on MainForm
        public static PointPairList tempDelta1;
        public static PointPairList tempDelta2;
        public static PointPairList tempDelta3;

        // Data series for zedGraphContol2 on MainForm
        public static PointPairList evapDelta1;
        public static PointPairList evapDelta2;
        public static PointPairList evapDelta3;

        // Data for zedGraphControl3 on MainForm
        public static PointPairList rainDelta1;
        public static PointPairList rainDelta2;
        public static PointPairList rainDelta3;

        // Data for zedGraphControl4 on MainForm
        public static PointPairList xrain1;
        public static PointPairList xrain2;
        // public static PointPairList xrain3;

        public static string[] monthLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public static string[] returnPeriods = { "5", "10", "15", "30", "50", "100" };

        public static string[] scenarioNames = { "None", "Hot/Dry", "Median", "Warm/Wet" };

        public static double evapUcf, tempUcf;

        // Updates the climate change adjustments displayed on the MainForm
        public static void UpdateAdjustments()
        {
            UpdateTempAdjustments();
            UpdateEvapAdjustments();
            UpdateRainAdjustments();
            UpdateExtremeAdjustments();
        }

        public static void SaveAdjustments(string inpFile, int climate_scenario, int extreme_storm_scenario,
            bool[] savedAdjustments, int returnPeriodIndex)
        {
            PointPairList values;
            string tempLine = "";
            string evapLine = "";
            string precipLine = "";

            GetUcfs(inpFile);
            // Temperature adjustments
            if (savedAdjustments[0])
            {
                switch (climate_scenario)
                {
                    case 1: values = tempDelta1; break;
                    case 2: values = tempDelta2; break;
                    case 3: values = tempDelta3; break;
                    default: return;
                }
                tempLine = "Temperature";
                for (int i = 0; i < 12; i++)
                {
                    tempLine = tempLine + "    " + String.Format("{0:0.0000}", values[i].Y * tempUcf);
                }
            }

            // Evaporation adjustments
            if (savedAdjustments[1])
            {
                switch (climate_scenario)
                {
                    case 1: values = evapDelta1; break;
                    case 2: values = evapDelta2; break;
                    case 3: values = evapDelta3; break;
                    default: return;
                }
                evapLine = "Evaporation";
                for (int i = 0; i < 12; i++)
                {
                    evapLine = evapLine + "    " + String.Format("{0:0.0000}", values[i].Y * evapUcf);
                }
            }

            // Rainfall adjustments
            if (savedAdjustments[2])
            {
                switch (climate_scenario)
                {
                    case 1: values = rainDelta1; break;
                    case 2: values = rainDelta2; break;
                    case 3: values = rainDelta3; break;
                    default: return;
                }
                precipLine = "Rainfall";
                for (int i = 0; i < 12; i++)
                {
                    double x = 1.0 + values[i].Y / 100.0;
                    precipLine = precipLine + "    " + String.Format("{0:0.000}", x);
                }
            }

            // Extreme rainfall adjustment
            if (savedAdjustments[3])
            {
                switch (extreme_storm_scenario)
                {
                    case 1: values = xrain1; break;
                    case 2: values = xrain2; break;
                    //case 3: values = xrain3; break;
                    default: return;
                }
                double x = 1.0 + values[returnPeriodIndex].Y / 100.0;
                precipLine = "Rainfall";
                for (int i = 0; i < 12; i++)
                {
                    precipLine = precipLine + "    " + String.Format("{0:0.000}", x);
                }
            }

            using (StreamWriter sw = File.AppendText(inpFile))
            {
                if (tempLine.Length > 0 || evapLine.Length > 0 || precipLine.Length > 0)
                {
                    sw.WriteLine("");
                    sw.WriteLine("[ADJUSTMENTS]");
                    if (tempLine.Length > 0) sw.WriteLine(tempLine);
                    if (evapLine.Length > 0) sw.WriteLine(evapLine);
                    if (precipLine.Length > 0) sw.WriteLine(precipLine);
                    sw.WriteLine("");
                }
            }
        }

        private static void UpdateTempAdjustments()
        {
            double[] y = new double[12];
            string tempTable;

            tempDelta1.Clear();
            tempDelta2.Clear();
            tempDelta3.Clear();

            if (MainForm.climateYear == 2035)
                tempTable = SWMM_CAT.Properties.Resources.Temp2035Hot;
            else
                tempTable = SWMM_CAT.Properties.Resources.Temp2060Hot;
            if (GetTableData(tempTable, MainForm.evapID, 12, ref y)) tempDelta1.Add(null, y);

            if (MainForm.climateYear == 2035)
                tempTable = SWMM_CAT.Properties.Resources.Temp2035Med;
            else
                tempTable = SWMM_CAT.Properties.Resources.Temp2060Med;
            if (GetTableData(tempTable, MainForm.evapID, 12, ref y)) tempDelta2.Add(null, y);

            if (MainForm.climateYear == 2035)
                tempTable = SWMM_CAT.Properties.Resources.Temp2035Wet;
            else
                tempTable = SWMM_CAT.Properties.Resources.Temp2060Wet;
            if (GetTableData(tempTable, MainForm.evapID, 12, ref y)) tempDelta3.Add(null, y);
        }

        private static void UpdateEvapAdjustments()
        {
            // Declare variables
            double[] x = new double[12];
            double[] y = new double[12];
            double[] e = new double[12];
            string evapTable;

            // Clear all evap adjustments
            evapDelta1.Clear();
            evapDelta2.Clear();
            evapDelta3.Clear();

            // Get historical monthly evap for current weather station
            evapTable = SWMM_CAT.Properties.Resources.Pmet_historical;
            if (GetTableData(evapTable, MainForm.evapID, 12, ref e) == false) return;

            // Get Hot/Dry adjustments 
            if (MainForm.climateYear == 2035)
                evapTable = SWMM_CAT.Properties.Resources.Pmet2035Hot;
            else
                evapTable = SWMM_CAT    .Properties.Resources.Pmet2060Hot;
            if (GetTableData(evapTable, MainForm.evapID, 12, ref y))
            {
                for (int i = 0; i < 12; i++) x[i] = y[i] - e[i];
                evapDelta1.Add(null, x);
            }

            // Get Median adjustments
            if (MainForm.climateYear == 2035)
                evapTable = SWMM_CAT.Properties.Resources.Pmet2035Med;
            else
                evapTable = SWMM_CAT.Properties.Resources.Pmet2060Med;
            if (GetTableData(evapTable, MainForm.evapID, 12, ref y))
            {
                for (int i = 0; i < 12; i++) x[i] = y[i] - e[i];
                evapDelta2.Add(null, x);
            }

            // Get Warm/Wet adjustments
            if (MainForm.climateYear == 2035)
                evapTable = SWMM_CAT.Properties.Resources.Pmet2035Wet;
            else
                evapTable = SWMM_CAT.Properties.Resources.Pmet2060Wet;
            if (GetTableData(evapTable, MainForm.evapID, 12, ref y))
            {
                for (int i = 0; i < 12; i++) x[i] = y[i] - e[i];
                evapDelta3.Add(null, x);
            }
        }

        private static void UpdateRainAdjustments()
        {
            double[] y = new double[12];
            string precTable;

            rainDelta1.Clear();
            rainDelta2.Clear();
            rainDelta3.Clear();

            if (MainForm.climateYear == 2035)
                precTable = SWMM_CAT.Properties.Resources.PREC2035Hot;
            else
                precTable = SWMM_CAT.Properties.Resources.PREC2060Hot;
            if (GetTableData(precTable, MainForm.precipID, 12, ref y)) rainDelta1.Add(null, y);

            if (MainForm.climateYear == 2035)
                precTable = SWMM_CAT.Properties.Resources.PREC2035Med;
            else
                precTable = SWMM_CAT.Properties.Resources.PREC2060Med;
            if (GetTableData(precTable, MainForm.precipID, 12, ref y)) rainDelta2.Add(null, y);

            if (MainForm.climateYear == 2035)
                precTable = SWMM_CAT.Properties.Resources.PREC2035Wet;
            else
                precTable = SWMM_CAT.Properties.Resources.PREC2060Wet;
            if (GetTableData(precTable, MainForm.precipID, 12, ref y)) rainDelta3.Add(null, y);
        }

        private static void UpdateExtremeAdjustments()
        {
            double[] x = new double[6];
            double[] y = new double[6];
            string precTable;

            xrain1.Clear();
            xrain2.Clear();
            //xrain3.Clear();

            precTable = SWMM_CAT.Properties.Resources.GEVdepth_historical;
            if (GetTableData(precTable, MainForm.stormID, 6, ref x) == false) return;

            if (MainForm.climateYear == 2035)
                precTable = SWMM_CAT.Properties.Resources.GEVdepth2035Stormy;
            else
                precTable = SWMM_CAT.Properties.Resources.GEVdepth2060Stormy;
            if (GetTableData(precTable, MainForm.stormID, 6, ref y))
            {
                for (int i = 0; i < 6; i++) y[i] = (y[i] - x[i]) / x[i] * 100.0;
                xrain1.Add(null, y);
            }

            if (MainForm.climateYear == 2035)
                precTable = SWMM_CAT.Properties.Resources.GEVdepth2035LessStormy;
            else
                precTable = SWMM_CAT.Properties.Resources.GEVdepth2035LessStormy;
            if (GetTableData(precTable, MainForm.stormID, 6, ref y))
            {
                for (int i = 0; i < 6; i++) y[i] = (y[i] - x[i]) / x[i] * 100.0;
                xrain2.Add(null, y);
            }

        }

        // Retrieves n-values from a climate adjustment data file
        private static bool GetTableData(string dataTable, string stationID, int n, ref double[] x)
        {
            string line;
            for (int i = 0; i < n; i++) x[i] = 0;
            try
            {
                using (StringReader sr = new StringReader(dataTable))
                {
                    // First line is a header
                    line = sr.ReadLine();
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) return false;
                        if (line.StartsWith(stationID))
                        {
                            string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                            if (values.Length > n) 
                                for (int i = 0; i < n; i++) x[i] = Double.Parse(values[i+1]);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "File Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        static void GetUcfs(string inpFile)
        {
            string line;
            evapUcf = 1.0;
            tempUcf = 9.0 / 5.0;
            try
            {
                using (StreamReader sr = File.OpenText(inpFile))
                {
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null) return;
                        line = line.Trim();
                        line = line.ToUpper();
                        if (line.StartsWith("FLOW_UNITS"))
                        {
                            string[] values = line.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);
                            if (values.Length < 2) return;
                            if (values[1] == "LPS" || values[1] == "CMS" || values[1] == "MLD")
                            {
                                evapUcf = 25.4;
                                tempUcf = 1.0;
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
