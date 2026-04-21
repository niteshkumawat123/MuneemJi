using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using iTextSharp.text;

namespace MUNEEMJI.PdfServices.Common
{
    public class ConfigControls
    {
        public ConfigControls()
        {

        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, iTextSharp.text.BaseColor ForeColour)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, iTextSharp.text.Font.FontFamily.HELVETICA, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour);

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, iTextSharp.text.BaseColor ForeColour, iTextSharp.text.Font bFont)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, iTextSharp.text.Font.FontFamily.HELVETICA, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour, bFont);

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, iTextSharp.text.BaseColor ForeColour, int FixedHeight)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, iTextSharp.text.Font.FontFamily.HELVETICA, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour);
            cell.FixedHeight = FixedHeight;
            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, iTextSharp.text.Font.FontFamily CellFontFamily, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, iTextSharp.text.BaseColor ForeColour, iTextSharp.text.Font bFont)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(value, bFont));
            cell.HorizontalAlignment = HorizontalAlignment;

            if (VerticalAlignment == 0)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_TOP;
            if (VerticalAlignment == 1)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_MIDDLE;
            if (VerticalAlignment == 2)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_BOTTOM;

            cell.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Black);
            cell.Colspan = ColSpan;
            cell.Rowspan = RowSpan;
            cell.PaddingTop = PaddingTop;
            cell.PaddingBottom = PaddingBottom;
            cell.Border = iTextSharp.text.Rectangle.BOX;

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, iTextSharp.text.Font.FontFamily CellFontFamily, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, iTextSharp.text.BaseColor ForeColour)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(value, new iTextSharp.text.Font(CellFontFamily, FontSize, FontStyle, ForeColour)));
            cell.HorizontalAlignment = HorizontalAlignment;

            if (VerticalAlignment == 0)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_TOP;
            if (VerticalAlignment == 1)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_MIDDLE;
            if (VerticalAlignment == 2)
                cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_BOTTOM;

            cell.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Black);
            cell.Colspan = ColSpan;
            cell.Rowspan = RowSpan;
            cell.PaddingTop = PaddingTop;
            cell.PaddingBottom = PaddingBottom;
            cell.Border = iTextSharp.text.Rectangle.BOX;
            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, bool BorderLeft, bool BorderRight, bool BorderTop, bool BorderBottom, iTextSharp.text.BaseColor ForeColour)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour);
            if (!BorderLeft)
                cell.BorderWidthLeft = 0f;
            if (!BorderRight)
                cell.BorderWidthRight = 0f;
            if (!BorderTop)
                cell.BorderWidthTop = 0f;
            if (!BorderBottom)
                cell.BorderWidthBottom = 0f;


            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, bool BorderLeft, bool BorderRight, bool BorderTop, bool BorderBottom, iTextSharp.text.BaseColor ForeColour, iTextSharp.text.Font bFont)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour, bFont);

            if (!BorderLeft)
                cell.BorderWidthLeft = 0f;
            if (!BorderRight)
                cell.BorderWidthRight = 0f;
            if (!BorderTop)
                cell.BorderWidthTop = 0f;
            if (!BorderBottom)
                cell.BorderWidthBottom = 0f;

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, iTextSharp.text.Font.FontFamily CellFontFamily, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, bool BorderLeft, bool BorderRight, bool BorderTop, bool BorderBottom, iTextSharp.text.BaseColor ForeColour)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, CellFontFamily, FontSize, FontStyle, PaddingTop, PaddingBottom, ForeColour);
            if (!BorderLeft)
                cell.BorderWidthLeft = 0f;
            if (!BorderRight)
                cell.BorderWidthRight = 0f;
            if (!BorderTop)
                cell.BorderWidthTop = 0f;
            if (!BorderBottom)
                cell.BorderWidthBottom = 0f;

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, bool BorderLeft, bool BorderRight, bool BorderTop, bool BorderBottom, iTextSharp.text.BaseColor BackgroundColor, iTextSharp.text.BaseColor ForeColor)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, FontSize, FontStyle, PaddingTop, PaddingBottom, BorderLeft, BorderRight, BorderTop, BorderBottom, ForeColor);
            if (BackgroundColor != null)
                cell.BackgroundColor = BackgroundColor;

            return cell;
        }

        public static iTextSharp.text.pdf.PdfPCell GetPdfTableCell(string value, int HorizontalAlignment, int VerticalAlignment, int ColSpan, int RowSpan, float FontSize, int FontStyle, float PaddingTop, float PaddingBottom, bool BorderLeft, bool BorderRight, bool BorderTop, bool BorderBottom, iTextSharp.text.BaseColor BackgroundColor, iTextSharp.text.BaseColor ForeColor, iTextSharp.text.FontFactory Font)
        {
            iTextSharp.text.pdf.PdfPCell cell = GetPdfTableCell(value, HorizontalAlignment, VerticalAlignment, ColSpan, RowSpan, FontSize, FontStyle, PaddingTop, PaddingBottom, BorderLeft, BorderRight, BorderTop, BorderBottom, ForeColor);
            cell.BackgroundColor = BackgroundColor;

            return cell;
        }



        public static string ConvertAmountToWords(decimal number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + ConvertAmountToWords(Math.Abs(number));

            string words = "";

            int intPart = (int)number;
            int decimalPart = (int)((number - intPart) * 100);
            if (decimalPart > 0)
            {
                words = $"{ConvertToWords(intPart)} rupees and {ConvertToWords(decimalPart)} paise";
            }
            else
            {
                words = $"{ConvertToWords(intPart)} rupees";
            }

            return words.First().ToString().ToUpper() + words.Substring(1);
        }

        public static string ConvertToWords(int number)
        {
            if (number == 0)
                return "";

            if (number < 0)
                return "minus " + ConvertToWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ConvertToWords(number / 10000000) + " crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertToWords(number / 100000) + " lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}
