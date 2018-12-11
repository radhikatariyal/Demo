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

namespace Patient.Demographics.Common.Excel
{
    public static class ExcelCategory
    {
        public static byte[] WriteExcelCategoryFileSheet(DataTable datatable)
        {
            var ms = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet1 = excelPackage.Workbook.Worksheets.Add("Category Mapping");
                worksheet1.Cells["A1"].LoadFromDataTable(datatable, true);
                worksheet1.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet1.Cells.AutoFitColumns();
                int i = 1;
                if (datatable.Rows.Count == 0)
                {
                    foreach (DataColumn Column in datatable.Columns)
                    {
                        worksheet1.Cells[1, i].Value = Column.ColumnName;
                        i++;
                    }
                }

                using (ExcelRange range = worksheet1.Cells[1, 1, 1, datatable.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#538DD5");
                    range.Style.Fill.BackgroundColor.SetColor(colFromHex);
                    range.Style.Font.Color.SetColor(Color.White);
                }
                excelPackage.Save();
                return ms.ToArray();
            }
        }

        public static byte[] WriteExcelFreightGroupListSheet(DataTable datatable)
        {
            var ms = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet1 = excelPackage.Workbook.Worksheets.Add("Freight Group Items");
                worksheet1.Cells["A1"].LoadFromDataTable(datatable, true);
                worksheet1.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                worksheet1.Cells.AutoFitColumns();
                int i = 1;
                if (datatable.Rows.Count == 0)
                {
                    foreach (DataColumn Column in datatable.Columns)
                    {
                        worksheet1.Cells[1, i].Value = Column.ColumnName;
                        i++;
                    }
                }

                using (ExcelRange range = worksheet1.Cells[1, 1, 1, datatable.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
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
