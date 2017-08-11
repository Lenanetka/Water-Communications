using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;

namespace WaterCommunications.DataReaderWriter
{
    class DataFromToXML: IDataReaderWriter
    {
        public DataFromToXML()
        {

        }

        public List<Station> ReadFromFile(String path)
        {
           throw new NotImplementedException();
        }
        public void WriteInFile(String path, List<Station> stations, bool overwrite)
        {
            var wb = new XLWorkbook();
            if(!overwrite) wb = new XLWorkbook(@path);

            var ws = wb.Worksheets.Add("Main information");
            
            ws.Cell(1, 1).Value = "id";
            ws.Cell(1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(1, 2).Value = "sourseId";
            ws.Cell(1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(1, 3).Value = "optimal k";
            ws.Cell(1, 3).AsRange().AddToNamed("Titles");

            for (int i = 1; i < stations.Count; i++)
            {
                ws.Cell(i + 1, 1).Value = stations[i].id;
                ws.Cell(i + 1, 2).Value = stations[i].sourceId;
                ws.Cell(i + 1, 3).Value = stations[i].k;
            }

            // Prepare the style for the titles
            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.Cyan;

            // Format all titles in one shot
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            ws.Columns().AdjustToContents();

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

            int n = ((stations.Count - 1) + 3) * (station - 1);

            ws.Cell(n + 1, 1).Value = "Station";
            ws.Cell(n + 1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 2).Value = "Source";
            ws.Cell(n + 1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 3).Value = "Optimal k";
            ws.Cell(n + 1, 3).AsRange().AddToNamed("Titles");

            ws.Cell(n + 2, 1).Value = stations[station].id;
            ws.Cell(n + 2, 1).AsRange().AddToNamed("Cells");
            ws.Cell(n + 2, 2).Value = stations[station].sourceId;
            ws.Cell(n + 2, 2).AsRange().AddToNamed("Cells");
            ws.Cell(n + 2, 3).Value = stations[station].k;
            ws.Cell(n + 2, 3).AsRange().AddToNamed("Cells");

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
                ws.Cell(n + i + 3, 1).AsRange().AddToNamed("Cells");
                ws.Cell(n + i + 3, 2).Value = stations[i].sourceId;
                ws.Cell(n + i + 3, 2).AsRange().AddToNamed("Cells");
                ws.Cell(n + i + 3, 3).Value = String.Format("{0:0.00}", stations[i].pipeLength);
                ws.Cell(n + i + 3, 3).AsRange().AddToNamed("Cells");
                ws.Cell(n + i + 3, 4).Value = String.Format("{0:0.0}", stations[i].pipeDiameter);
                ws.Cell(n + i + 3, 4).AsRange().AddToNamed("Cells");
                ws.Cell(n + i + 3, 5).Value = String.Format("{0:0.0}", stations[i].Qf);
                ws.Cell(n + i + 3, 5).AsRange().AddToNamed("Cells");
                ws.Cell(n + i + 3, 6).Value = String.Format("{0:0.00}", stations[i].h);
                ws.Cell(n + i + 3, 6).AsRange().AddToNamed("Cells");
            }

            // Prepare the style for the titles
            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;

            // Prepare the style for the titles
            var cellsStyle = wb.Style;
            cellsStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cellsStyle.Fill.BackgroundColor = XLColor.NoColor;

            // Format all titles in one shot
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
            wb.NamedRanges.NamedRange("Cells").Ranges.Style = cellsStyle;

            ws.Columns().AdjustToContents();

            wb.SaveAs(@path);
        }
    }
}
