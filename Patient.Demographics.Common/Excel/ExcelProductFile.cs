using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Demographics.Common
{
    public static class ExcelProductFile
    {
        public static byte[] WriteExcelProductFileSheet(DataTable productFile, DataTable productRules)
        {
            var ms = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet1 = excelPackage.Workbook.Worksheets.Add("Product File");
                worksheet1.Cells["A1"].LoadFromDataTable(productFile, true);
                worksheet1.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet1.Cells.AutoFitColumns();
                int i = 1;
                if (productFile.Rows.Count == 0)
                {
                    foreach (DataColumn Column in productFile.Columns)
                    {
                        worksheet1.Cells[1, i].Value = Column.ColumnName;
                        i++;
                    }
                }

                using (ExcelRange range = worksheet1.Cells[1, 1, 1, productFile.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#538DD5");
                    range.Style.Fill.BackgroundColor.SetColor(colFromHex);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                ExcelWorksheet worksheet2 = excelPackage.Workbook.Worksheets.Add("Product File Rules");
                worksheet2.Cells["A1"].LoadFromDataTable(productRules, true);
                worksheet2.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet2.Cells.AutoFitColumns();
                int j = 1;
                if (productRules.Rows.Count == 0)
                {
                    foreach (DataColumn Column in productRules.Columns)
                    {
                        worksheet2.Cells[1, j].Value = Column.ColumnName;
                        j++;
                    }
                }
                using (ExcelRange range = worksheet2.Cells[1, 1, 1, productRules.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#538DD5");
                    range.Style.Fill.BackgroundColor.SetColor(colFromHex);
                    range.Style.Font.Color.SetColor(Color.White);
                }
                excelPackage.Save();
                return ms.ToArray();
            }
        }
    }
}
