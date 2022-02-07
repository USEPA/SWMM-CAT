using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ZedGraph;

namespace SWMM_CAT
{
    class Graphs
    {
        // Curve colors
        static Color[] colors = { Color.SteelBlue, Color.RosyBrown, Color.PowderBlue, Color.Wheat };
        //{ Color.Blue, Color.Green, Color.Cyan, Color.Lime };

        static void SetCommonProperties(GraphPane myPane)
        {
            myPane.IsFontsScaled = false;
            myPane.Title.FontSpec.Size = 12;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.Scale.FontSpec.IsAntiAlias = false;
            myPane.YAxis.Scale.FontSpec.IsAntiAlias = false;
            myPane.YAxis.Scale.FontSpec.Size = 11;
            myPane.XAxis.Scale.FontSpec.Size = 11;
            myPane.XAxis.Title.FontSpec.Size = 11;
            myPane.XAxis.Title.FontSpec.IsBold = false;
            myPane.Legend.IsVisible = true;
            myPane.Legend.Border.IsVisible = false;
            myPane.Legend.Position = LegendPos.TopCenter;
            myPane.Legend.FontSpec.Size = 11;
            myPane.Legend.FontSpec.IsAntiAlias = false;

            // Apply a gradient fill to the chart area
            //myPane.Chart.Fill = new Fill(Color.FromArgb(220, 220, 255), Color.White, 45);
            myPane.Chart.Fill = new Fill(Color.AliceBlue);
        }

        public static void SetSymbolProperties(LineItem myCurve, Color c)
        {
            myCurve.Symbol.Size = 10;
            myCurve.Symbol.Fill.Type = FillType.Solid;
            myCurve.Symbol.Fill.Color = c;
            myCurve.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            myCurve.Line.IsAntiAlias = true;
        }

        public static void CreateMonthlyTempPlot(ZedGraphControl zgc)
        {
            // Display point values
            zgc.PointValueFormat = "F4";
            zgc.IsShowPointValues = true;

            // Get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            SetCommonProperties(myPane);

            // Set the graph's titles
            myPane.Title.Text = MainForm.timePeriodTitle + MainForm.tempPlotTitle;
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";

            // Create data arrays for monthly evap adjustments
            Climate.tempDelta1 = new PointPairList();
            Climate.tempDelta2 = new PointPairList();
            Climate.tempDelta3 = new PointPairList();

            // Generate curves for each data array
            LineItem myCurve1 = myPane.AddCurve("Hot/Dry", Climate.tempDelta1,
                Color.Black, SymbolType.TriangleDown);
            LineItem myCurve2 = myPane.AddCurve("Central", Climate.tempDelta2,
                Color.Black, SymbolType.Diamond);
            LineItem myCurve3 = myPane.AddCurve("Warm/Wet", Climate.tempDelta3,
                Color.Black, SymbolType.Triangle);
            SetSymbolProperties(myCurve1, Color.Red);
            SetSymbolProperties(myCurve2, Color.Gray);
            SetSymbolProperties(myCurve3, Color.Cyan);

            // Assign labels to the X-axis
            myPane.XAxis.Scale.TextLabels = Climate.monthLabels;
            myPane.XAxis.Type = AxisType.Text;

            // Tell ZedGraph to refigure the axes since the data have changed
            zgc.AxisChange();
        }

        public static void CreateMonthlyEvapPlot(ZedGraphControl zgc)
        {
            // Display point values
            zgc.PointValueFormat = "F4";
            zgc.IsShowPointValues = true;

            // Get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            SetCommonProperties(myPane);

            // Set the graph's titles
            myPane.Title.Text = MainForm.timePeriodTitle + MainForm.evapPlotTitle;
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";

            // Create data arrays for monthly evap adjustments
            Climate.evapDelta1 = new PointPairList();
            Climate.evapDelta2 = new PointPairList();
            Climate.evapDelta3 = new PointPairList();

            // Generate curves for each data array
            LineItem myCurve1 = myPane.AddCurve("Hot/Dry", Climate.evapDelta1,
                Color.Black, SymbolType.TriangleDown);
            LineItem myCurve2 = myPane.AddCurve("Central", Climate.evapDelta2,
                Color.Black, SymbolType.Diamond);
            LineItem myCurve3 = myPane.AddCurve("Warm/Wet", Climate.evapDelta3,
                Color.Black, SymbolType.Triangle);
            SetSymbolProperties(myCurve1, Color.Red);
            SetSymbolProperties(myCurve2, Color.Gray);
            SetSymbolProperties(myCurve3, Color.Cyan);

            // Assign labels to the X-axis
            myPane.XAxis.Scale.TextLabels = Climate.monthLabels;
            myPane.XAxis.Type = AxisType.Text;

            // Tell ZedGraph to refigure the axes since the data have changed
            zgc.AxisChange();
        }

        public static void CreateMonthlyRainPlot(ZedGraphControl zgc)
        {
            // Display point values
            zgc.PointValueFormat = "F2";
            zgc.IsShowPointValues = true;

            // Get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            SetCommonProperties(myPane);

            // Set the graph's titles
            myPane.Title.Text = MainForm.timePeriodTitle + MainForm.rainPlotTitle;
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "";

            // Create data arrays for monthly rainfall adjustments
            Climate.rainDelta1 = new PointPairList();
            Climate.rainDelta2 = new PointPairList();
            Climate.rainDelta3 = new PointPairList();

            // Generate curves for each data array
            LineItem myCurve1 = myPane.AddCurve("Hot/Dry", Climate.rainDelta1,
                Color.Black, SymbolType.TriangleDown);
            LineItem myCurve2 = myPane.AddCurve("Central", Climate.rainDelta2,
                Color.Black, SymbolType.Diamond);
            LineItem myCurve3 = myPane.AddCurve("Warm/Wet", Climate.rainDelta3,
                Color.Black, SymbolType.Triangle);
            SetSymbolProperties(myCurve1, Color.Red);
            SetSymbolProperties(myCurve2, Color.Gray);
            SetSymbolProperties(myCurve3, Color.Cyan);

            // Assign labels to the X-axis
            myPane.XAxis.Scale.TextLabels = Climate.monthLabels;
            myPane.XAxis.Type = AxisType.Text;

            // Tell ZedGraph to refigure the axes since the data have changed
            zgc.AxisChange();
        }

        public static void CreateXrainPlot(ZedGraphControl zgc)
        {
            // Display point values
            zgc.PointValueFormat = "F2";
            zgc.IsShowPointValues = true;

            // Get a reference to the GraphPane
            GraphPane myPane = zgc.GraphPane;
            SetCommonProperties(myPane);

            // Set the graph's titles
            myPane.Title.Text = MainForm.timePeriodTitle + MainForm.xrainPlotTitle;
            myPane.XAxis.Title.Text = "Return Period (years)";
            myPane.YAxis.Title.Text = "";

            // Create data arrays for annual max. rainfall adjustments
            Climate.xrain1 = new PointPairList();
            Climate.xrain2 = new PointPairList();
            //Climate.xrain3 = new PointPairList();

            // Generate curves for each data array
            LineItem myCurve1 = myPane.AddCurve("Stormy", Climate.xrain1,
                Color.Black, SymbolType.Triangle);
            LineItem myCurve2 = myPane.AddCurve("Less Stormy", Climate.xrain2,
                Color.Black, SymbolType.TriangleDown);
            //LineItem myCurve3 = myPane.AddCurve("Warm/Wet", Climate.xrain3,
            //    Color.Black, SymbolType.TriangleDown);
            SetSymbolProperties(myCurve1, Color.Red);
            SetSymbolProperties(myCurve2, Color.Cyan);
            //SetSymbolProperties(myCurve3, Color.Cyan);

            // Assign labels to the X-axis
            myPane.XAxis.Scale.TextLabels = Climate.returnPeriods;
            myPane.XAxis.Type = AxisType.Text;

            // Tell ZedGraph to refigure the axes since the data have changed
            zgc.AxisChange();
        }

        public static void SetBorderVisible(ZedGraphControl zgc, bool isVisible)
        {
            zgc.GraphPane.Border.IsVisible = isVisible;
            zgc.AxisChange();
        }

        public static void Refresh(ZedGraphControl zgc, string title)
        {
            zgc.GraphPane.Title.Text = title;
            zgc.Visible = true;
            zgc.AxisChange();
            zgc.Refresh();
        }

        public static void Clear(ZedGraphControl zgc)
        {
            foreach (CurveItem c in zgc.GraphPane.CurveList)
            {
                c.Clear();
            }
            zgc.Refresh();
        }
    }
}
