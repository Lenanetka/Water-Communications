using ClosedXML.Excel;
using System;
using System.Collections.Generic;

namespace WaterCommunications.DataReaderWriter
{
    class DataFromToXLSX : IDataReaderWriter
    {
        public DataFromToXLSX()
        {

        }

        public List<Station> ReadFromFile(String path)
        {
            List<Station> stations = new List<Station>();
            var wb = new XLWorkbook(@path);
            var ws = wb.Worksheet(1);

            int i = 2;
            while (!ws.Row(i).IsEmpty())
            {
                //header = csv.FieldHeaders;
                int id, sourceId;
                double Qn, L, d;
                if (!int.TryParse(ws.Row(i).Cell(1).GetValue<String>(), out id)) throw new Exception("Error 101\nIncorrect id, id must be integer. \nid = " + ws.Row(i).Cell(1).GetValue<String>());
                if (!int.TryParse(ws.Row(i).Cell(2).GetValue<String>(), out sourceId)) throw new Exception("Error 102\nIncorrect source id, source id must be integer.\nError for station with id = " + id);
                if (!double.TryParse(ws.Row(i).Cell(3).GetValue<String>(), out Qn) || Qn <= 0) throw new Exception("Error 103\nIncorrect Qn, Qn must be positive double.\nError for station with id = " + id);
                if (!double.TryParse(ws.Row(i).Cell(4).GetValue<String>(), out L) || L <= 0) throw new Exception("Error 104\nIncorrect L, L must be positive double.\nError for station with id = " + id);
                if (!double.TryParse(ws.Row(i).Cell(5).GetValue<String>(), out d) || d <= 0) throw new Exception("Error 105\nIncorrect d, d must be positive double.\nError for station with id = " + id);

                stations.Add(new Station(id, sourceId, Qn, L, d));
                i++;
            }

            return stations;
        }
        public void WriteInFile(String path, List<Station> stations, bool overwrite)
        {
            var wb = new XLWorkbook();
            if (!overwrite) wb = new XLWorkbook(@path);

            var ws = wb.Worksheets.Add("Main information");

            ws.Cell(1, 1).Value = "Station";
            ws.Cell(1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(1, 2).Value = "Source";
            ws.Cell(1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(1, 3).Value = "Optimal k";
            ws.Cell(1, 3).AsRange().AddToNamed("Titles");

            for (int i = 1; i < stations.Count; i++)
            {
                ws.Cell(i + 1, 1).Value = stations[i].id;
                ws.Cell(i + 1, 2).Value = stations[i].sourceId;
                ws.Cell(i + 1, 3).Value = stations[i].k;
            }

            ws.Rows(2, stations.Count).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            ws.Columns().AdjustToContents();

            var range = ws.Range(ws.Cell(stations.Count, 1).Address, ws.Cell(stations.Count, 3).Address);
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.Black;

            wb.SaveAs(@path);
        }
        public void WriteInFile(String path, List<Station> stations, int station, bool overwrite)
        {
            XLWorkbook wb;
            IXLWorksheet ws;

            if (overwrite)
            {
                wb = new XLWorkbook();
                ws = wb.Worksheets.Add("All information");
            }
            else
            {
                wb = new XLWorkbook(@path);
                ws = wb.Worksheet(1);
            }

            int n = ((stations.Count - 1) + 3 + 1) * (station - 1);

            ws.Cell(n + 1, 1).Value = "Station";
            ws.Cell(n + 1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 2).Value = "Source";
            ws.Cell(n + 1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 3).Value = "Optimal k";
            ws.Cell(n + 1, 3).AsRange().AddToNamed("Titles");

            ws.Cell(n + 2, 1).Value = stations[station].id;
            ws.Cell(n + 2, 2).Value = stations[station].sourceId;
            ws.Cell(n + 2, 3).Value = stations[station].k;

            ws.Cell(n + 3, 1).Value = "id";
            ws.Cell(n + 3, 1).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 2).Value = "sourseId";
            ws.Cell(n + 3, 2).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 3).Value = "L";
            ws.Cell(n + 3, 3).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 4).Value = "d";
            ws.Cell(n + 3, 4).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 5).Value = "Q";
            ws.Cell(n + 3, 5).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 6).Value = "h";
            ws.Cell(n + 3, 6).AsRange().AddToNamed("Titles");

            for (int i = 1; i < stations.Count; i++)
            {
                ws.Cell(n + i + 3, 1).Value = stations[i].id;
                ws.Cell(n + i + 3, 2).Value = stations[i].sourceId;
                ws.Cell(n + i + 3, 3).Value = String.Format("{0:0.00}", stations[i].pipeLength);
                ws.Cell(n + i + 3, 4).Value = String.Format("{0:0.0}", stations[i].pipeDiameter);
                ws.Cell(n + i + 3, 5).Value = String.Format("{0:0.0}", stations[i].Qf);
                ws.Cell(n + i + 3, 6).Value = String.Format("{0:0.00}", stations[i].h);
            }

            ws.Rows(n + 1 + 3, n + stations.Count - 1 + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            ws.Columns().AdjustToContents();

            var range = ws.Range(ws.Cell(n + stations.Count - 1 + 3, 1).Address, ws.Cell(n + stations.Count - 1 + 3, 6).Address);
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.Black;

            wb.SaveAs(@path);
        }
    }
}
