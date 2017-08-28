﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using WaterCommunications.Localization;

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
                if (!int.TryParse(ws.Row(i).Cell(1).GetValue<String>(), out id)) throw new Exception(CurrentLocalization.localizationErrors.error101 + ws.Row(i).Cell(1).GetValue<String>());
                if (!int.TryParse(ws.Row(i).Cell(2).GetValue<String>(), out sourceId)) throw new Exception(CurrentLocalization.localizationErrors.error102 + id);
                if (!double.TryParse(ws.Row(i).Cell(3).GetValue<String>(), out Qn) || Qn <= 0) throw new Exception(CurrentLocalization.localizationErrors.error103 + id);
                if (!double.TryParse(ws.Row(i).Cell(4).GetValue<String>(), out L) || L <= 0) throw new Exception(CurrentLocalization.localizationErrors.error104 + id);
                if (!double.TryParse(ws.Row(i).Cell(5).GetValue<String>(), out d) || d <= 0) throw new Exception(CurrentLocalization.localizationErrors.error105 + id);

                stations.Add(new Station(id, sourceId, Qn, L, d));
                i++;
            }

            return stations;
        }        
            public void WriteInFile(String path, Communications communications, bool overwrite)
        {
            LocalizationOutput output = CurrentLocalization.localizationOutput;
            LocalizationSystemOfUnits units = CurrentLocalization.localizationSystemOfUnits;

            XLWorkbook wb;
            IXLWorksheet ws;
            IXLWorksheet wsi;
            if (overwrite)
            {
                wb = new XLWorkbook();                
            }
            else
            {
                wb = new XLWorkbook(@path);
            }
            ws = wb.Worksheets.Add(output.mainInfo.title);
            wsi = wb.Worksheets.Add(output.startDatas.title);

            List<Station> stations = communications.stations;

            //Main information

            ws.Cell(1, 1).Value = output.mainInfo.destination;
            ws.Cell(1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(1, 2).Value = output.mainInfo.source;
            ws.Cell(1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(1, 3).Value = output.mainInfo.length + ", " + units.km;
            ws.Cell(1, 3).AsRange().AddToNamed("Titles");
            ws.Cell(1, 4).Value = output.mainInfo.optimalK;
            ws.Cell(1, 4).AsRange().AddToNamed("Titles");

            for (int i = 1; i < stations.Count; i++)
            {
                ws.Cell(i + 1, 1).Value = stations[i].id;
                ws.Cell(i + 1, 2).Value = stations[i].sourceId;
                ws.Cell(i + 1, 3).Value = Math.Round(stations[i].pipeLength, 2);
                ws.Cell(i + 1, 4).Value = stations[i].k;
            }

            //Start datas

            wsi.Cell(1, 1).Value = output.startDatas.idMain;
            wsi.Cell(1, 2).Value = stations[0].id;
            wsi.Cell(2, 1).Value = output.startDatas.headMain;
            wsi.Cell(2, 2).Value = communications.h;
            wsi.Cell(2, 3).Value = units.m;
            wsi.Cell(3, 1).Value = output.startDatas.headMin;
            wsi.Cell(3, 2).Value = communications.hMin;
            wsi.Cell(3, 3).Value = units.m;
            wsi.Cell(4, 1).Value = output.startDatas.accidentPercent;
            wsi.Cell(4, 2).Value = communications.accidentPercent * 100;
            wsi.Cell(4, 3).Value = units.percent;
            wsi.Cell(5, 1).Value = output.startDatas.lengthRepairSection;
            wsi.Cell(5, 2).Value = communications.repairSectionMinimumLength;
            wsi.Cell(5, 3).Value = units.km;

            //Style ws
            ws.Rows(2, stations.Count).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            var rangeDouble = ws.Range(ws.Cell(2, 3).Address, ws.Cell(stations.Count, 3).Address);
            rangeDouble.Style.NumberFormat.Format = "0.00";

            var range = ws.Range(ws.Cell(stations.Count, 1).Address, ws.Cell(stations.Count, 4).Address);
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.Black;

            ws.Columns().AdjustToContents();

            //Style wsi
            wsi.Columns().AdjustToContents();

            //Style common
            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                                             
            wb.SaveAs(@path);
        }
        public void WriteInFile(String path, Communications communications, int station, bool overwrite)
        {
            LocalizationOutput output = CurrentLocalization.localizationOutput;
            LocalizationSystemOfUnits units = CurrentLocalization.localizationSystemOfUnits;

            XLWorkbook wb;
            IXLWorksheet ws;          
            if (overwrite)
            {
                wb = new XLWorkbook();
                ws = wb.Worksheets.Add(output.allInfo.title);
            }
            else
            {
                wb = new XLWorkbook(@path);
                ws = wb.Worksheet(output.allInfo.title);
            }

            List<Station> stations = communications.stations;
            int n = ((stations.Count - 1) + 3 + 1) * (station - 1);

            ws.Cell(n + 1, 1).Value = output.allInfo.destination;
            ws.Cell(n + 1, 1).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 2).Value = output.allInfo.source;
            ws.Cell(n + 1, 2).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 3).Value = output.allInfo.length + ", " + units.km;
            ws.Cell(n + 1, 3).AsRange().AddToNamed("Titles");
            ws.Cell(n + 1, 4).Value = output.allInfo.optimalK;
            ws.Cell(n + 1, 4).AsRange().AddToNamed("Titles");

            ws.Cell(n + 2, 1).Value = stations[station].id;
            ws.Cell(n + 2, 2).Value = stations[station].sourceId;
            ws.Cell(n + 2, 3).Value = Math.Round(stations[station].pipeLength, 2);
            ws.Cell(n + 2, 3).Style.NumberFormat.Format = "0.00";
            ws.Cell(n + 2, 4).Value = stations[station].k;

            ws.Cell(n + 3, 1).Value = output.allInfo.destination;
            ws.Cell(n + 3, 1).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 2).Value = output.allInfo.source;
            ws.Cell(n + 3, 2).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 3).Value = output.allInfo.length + ", " + units.km;
            ws.Cell(n + 3, 3).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 4).Value = output.allInfo.diameter + ", " + units.mm;
            ws.Cell(n + 3, 4).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 5).Value = output.allInfo.fluidFlow + ", " + units.m3h;
            ws.Cell(n + 3, 5).AsRange().AddToNamed("Titles");
            ws.Cell(n + 3, 6).Value = output.allInfo.head + ", " + units.m;
            ws.Cell(n + 3, 6).AsRange().AddToNamed("Titles");

            for (int i = 1; i < stations.Count; i++)
            {
                ws.Cell(n + i + 3, 1).Value = stations[i].id;
                ws.Cell(n + i + 3, 2).Value = stations[i].sourceId;
                ws.Cell(n + i + 3, 3).Value = Math.Round(stations[i].pipeLength , 2);
                ws.Cell(n + i + 3, 4).Value = Math.Round(stations[i].pipeDiameter, 1);
                ws.Cell(n + i + 3, 5).Value = Math.Round(stations[i].Qf, 1);
                ws.Cell(n + i + 3, 6).Value = Math.Round(stations[i].h, 2);
                if (stations[i].h < communications.hMin) ws.Cell(n + i + 3, 6).Style.Fill.BackgroundColor = XLColor.PastelRed;
            }

            ws.Rows(n + 1 + 3, n + stations.Count - 1 + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            var rangeDouble = ws.Range(ws.Cell(n + 1 + 3, 3).Address, ws.Cell(n + stations.Count - 1 + 3, 3).Address);
            rangeDouble.Style.NumberFormat.Format = "0.00";
            rangeDouble = ws.Range(ws.Cell(n + 1 + 3, 6).Address, ws.Cell(n + stations.Count - 1 + 3, 6).Address);
            rangeDouble.Style.NumberFormat.Format = "0.00";
            rangeDouble = ws.Range(ws.Cell(n + 1 + 3, 4).Address, ws.Cell(n + stations.Count - 1 + 3, 5).Address);
            rangeDouble.Style.NumberFormat.Format = "0.0";

            var range = ws.Range(ws.Cell(n + stations.Count - 1 + 3, 1).Address, ws.Cell(n + stations.Count - 1 + 3, 6).Address);
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorderColor = XLColor.Black;

            ws.Columns().AdjustToContents();

            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                        
            wb.SaveAs(@path);
        }
    }
}
