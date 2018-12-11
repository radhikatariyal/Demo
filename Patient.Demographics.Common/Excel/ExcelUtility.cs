using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OfficeOpenXml.Style;
using System.IO;

namespace Patient.Demographics.Common
{
    public static class ExcelUtility
    {
        public static void WriteExcelSuccessErrorSheet(string fileName, DataTable successSheet, DataTable errorSheet)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet1 = excelPackage.Workbook.Worksheets.Add("Success");
                worksheet1.Cells["A1"].LoadFromDataTable(successSheet, true);
                worksheet1.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet1.Cells.AutoFitColumns();

                using (ExcelRange range = worksheet1.Cells["A1:XFD1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.CadetBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                ExcelWorksheet worksheet2 = excelPackage.Workbook.Worksheets.Add("Error");
                worksheet2.Cells["A1"].LoadFromDataTable(errorSheet, true);
                worksheet2.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet2.Cells.AutoFitColumns();

                using (ExcelRange range = worksheet2.Cells["A1:XFD1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.CadetBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                if (File.Exists(fileName))
                    File.Delete(fileName);

                FileStream fileStream = File.Create(fileName);
                fileStream.Close();

                File.WriteAllBytes(fileName, excelPackage.GetAsByteArray());
            }
        }
    }
}
