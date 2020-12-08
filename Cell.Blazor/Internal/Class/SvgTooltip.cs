using Cell.Blazor._Core.Abstract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;

namespace Cell.Blazor.Internal.Class
{
    public class SvgTooltip : CellBaseComponent
    {
        private const double BASEFONT = 100.0;

        private const
#nullable disable
    string SPACE = " ";

        [Parameter]
        public string ID { get; set; }

        [Parameter]
        public string TextSize { get; set; }

        [Parameter]
        public string FontColor { get; set; }

        [Parameter]
        public string FontWeight { get; set; } = "Normal";

        [Parameter]
        public string FontFamily { get; set; } = "Segoe UI";

        [Parameter]
        public string FontStyle { get; set; } = "Normal";

        [Parameter]
        public double FontOpacity { get; set; } = 1.0;

        [Parameter]
        public string BorderColor { get; set; }

        [Parameter]
        public double BorderWidth { get; set; }

        [Parameter]
        public double X { get; set; }

        [Parameter]
        public double Y { get; set; }

        [Parameter]
        public double Height { get; set; }

        [Parameter]
        public double Width { get; set; }

        [Parameter]
        public bool EnableShadow { get; set; } = true;

        [Parameter]
        public string Fill { get; set; }

        [Parameter]
        public double Opacity { get; set; }

        [Parameter]
        public string Content { get; set; }

        [Parameter]
        public double LocationX { get; set; }

        [Parameter]
        public double LocationY { get; set; }

        [Parameter]
        public double RX { get; set; } = 2.0;

        [Parameter]
        public double RY { get; set; } = 2.0;

        [Parameter]
        public double MarginX { get; set; } = 5.0;

        [Parameter]
        public double MarginY { get; set; } = 5.0;

        [Parameter]
        public bool EnableRtl { get; set; }

        [Parameter]
        public bool IsInverted { get; set; }

        [Parameter]
        public bool RenderArrow { get; set; }

        [Parameter]
        public string ControlName { get; set; }

        [Parameter]
        public bool IsIE { get; set; }

        private float arrowPadding { get; set; } = 12f;

        private int padding { get; set; } = 5;

        private TooltipPath path { get; set; }

        private TextSetting textSetting { get; set; }

        private List<TextSpan> textCollection { get; set; }

        private SvgProperties svgProperties { get; set; }

        private SizeF elementSize { get; set; }

        private double tipRadius { get; set; } = 1.0;

        private double elementLeft { get; set; }

        private double elementTop { get; set; }

        private CultureInfo culture { get; set; } = CultureInfo.InvariantCulture;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.RenderTooltip();
        }

        private void RenderTooltip()
        {
            this.GetPathValues();
            this.RenderText();
            this.RenderTooltipElement();
        }

        private void GetPathValues()
        {
            this.path = new TooltipPath();
            this.path.Fill = !string.IsNullOrEmpty(this.Fill) ? this.Fill : "transparent";
            this.path.Opacity = this.Opacity;
            this.path.Stroke = !string.IsNullOrEmpty(this.BorderColor) ? this.BorderColor : "transparent";
        }

        private void RenderText()
        {
            double fontSize = !string.IsNullOrEmpty(this.TextSize) ? (this.TextSize.IndexOf("px", StringComparison.InvariantCulture) > -1 ? double.Parse(this.TextSize.Replace("px", string.Empty, StringComparison.InvariantCulture)) : double.Parse(this.TextSize)) : 13.0;
            bool flag1 = true;
            float num1 = 4f;
            this.textSetting = new TextSetting();
            this.textSetting.X = this.MarginX * 2.0;
            this.textSetting.Y = this.MarginY * 2.0 + (double)(this.padding * 2) + (this.MarginY == 2.0 ? 3.0 : 0.0);
            this.textSetting.Anchor = this.EnableRtl ? "end" : "start";
            double num2 = 22.0 / 13.0 * double.Parse(this.TextSize.Replace("px", string.Empty, StringComparison.InvariantCulture));
            double val1 = 0.0;
            double num3 = 0.0;
            this.textCollection = new List<TextSpan>();
            SizeF sizeF;
            foreach (string str in this.Content.Replace("</br>", "<br>", StringComparison.InvariantCulture).Replace("<br/>", "<br>", StringComparison.InvariantCulture).Split("<br>"))
            {
                double num4 = 0.0;
                bool flag2 = true;
                num3 += num2;
                foreach (string text in str.Replace(":", "<br>:<br>", StringComparison.InvariantCulture).Split("<br>"))
                {
                    TextSpan textSpan = new TextSpan();
                    if (!flag2 && string.IsNullOrWhiteSpace(text) || !string.IsNullOrEmpty(text.Trim()))
                    {
                        num4 += !string.IsNullOrWhiteSpace(text) ? (double)num1 : 0.0;
                        if (flag2 && !flag1)
                            textSpan = new TextSpan()
                            {
                                X = this.MarginX * 2.0,
                                Text = text,
                                DY = num2
                            };
                        else if (flag1 & flag2)
                            textSpan = new TextSpan()
                            {
                                X = this.MarginX * 2.0,
                                Text = text
                            };
                    }
                    if (text.Contains("<b>", StringComparison.InvariantCulture))
                    {
                        text = text.Replace("<b>", string.Empty, StringComparison.InvariantCulture).Replace("</b>", string.Empty, StringComparison.InvariantCulture);
                        textSpan.FontWeight = "bold";
                    }
                    flag2 = false;
                    textSpan.Text = text;
                    this.textCollection.Add(textSpan);
                    double num5 = num4;
                    sizeF = this.MeasureText(text, fontSize);
                    double width = (double)sizeF.Width;
                    num4 = num5 + width;
                    flag1 = false;
                }
                double val2 = num4 - (double)num1;
                val1 = Math.Max(val1, val2);
            }
            sizeF = new SizeF();
            sizeF.Height = (float)num3;
            sizeF.Width = (float)(val1 + (val1 > 0.0 ? 2.0 * this.MarginX : 0.0));
            this.elementSize = sizeF;
        }

        private void RenderTooltipElement()
        {
            bool isTop = false;
            bool isLeft = false;
            bool isBottom = false;
            float num1 = 0.0f;
            float num2 = 0.0f;
            PointF arrowLocation = PointF.Empty;
            PointF tipLocation = PointF.Empty;
            RectangleF tooltipLocation = this.GetTooltipLocation(out arrowLocation, out tipLocation);
            if (!this.IsInverted)
            {
                isTop = (double)tooltipLocation.Y < this.LocationY;
                isBottom = !isTop;
                num2 = isTop ? 0.0f : this.arrowPadding;
            }
            else
            {
                isLeft = (double)tooltipLocation.X < this.LocationX;
                num1 = isLeft ? 0.0f : this.arrowPadding;
            }
            double num3 = this.BorderWidth / 2.0;
            RectangleF rect = new RectangleF()
            {
                X = (float)num3 + num1,
                Y = (float)num3 + num2,
                Width = tooltipLocation.Width - (float)num3,
                Height = tooltipLocation.Height - (float)num3
            };
            this.elementLeft = (double)tooltipLocation.X;
            this.elementTop = (double)tooltipLocation.Y;
            this.svgProperties = new SvgProperties();
            this.svgProperties.Width = (double)tooltipLocation.Width + this.BorderWidth + (!this.IsInverted ? 0.0 : (double)this.arrowPadding) + 5.0;
            this.svgProperties.Height = (double)tooltipLocation.Height + this.BorderWidth + (this.IsInverted ? 0.0 : (double)this.arrowPadding) + 5.0;
            this.path.Direction = this.FindDirection(rect, isTop, isBottom, isLeft, arrowLocation, tipLocation, this.tipRadius);
            this.textSetting.Transform = this.ChangeText(isBottom, !isLeft && !isTop && !isBottom);
            if (this.RenderArrow)
                return;
            if (isBottom)
            {
                this.elementTop += 4.0;
            }
            else
            {
                if (!isTop)
                    return;
                this.elementTop += 8.0;
            }
        }

        private string FindDirection(
          RectangleF rect,
          bool isTop,
          bool isBottom,
          bool isLeft,
          PointF arrowLocation,
          PointF tipLocation,
          double tipRadius)
        {
            string empty = string.Empty;
            double x = (double)rect.X;
            double y = (double)rect.Y;
            double num1 = (double)rect.X + (double)rect.Width;
            double num2 = (double)rect.Y + (double)rect.Height;
            tipRadius = tipRadius != 0.0 ? tipRadius : 0.0;
            string str1;
            if (isTop)
            {
                string[] strArray1 = new string[25];
                strArray1[0] = empty;
                strArray1[1] = "M ";
                strArray1[2] = x.ToString((IFormatProvider)this.culture);
                strArray1[3] = " ";
                double num3 = y + this.RY;
                strArray1[4] = num3.ToString((IFormatProvider)this.culture);
                strArray1[5] = " Q ";
                strArray1[6] = x.ToString((IFormatProvider)this.culture);
                strArray1[7] = " ";
                strArray1[8] = y.ToString((IFormatProvider)this.culture);
                strArray1[9] = " ";
                num3 = x + this.RX;
                strArray1[10] = num3.ToString((IFormatProvider)this.culture);
                strArray1[11] = " ";
                strArray1[12] = y.ToString((IFormatProvider)this.culture);
                strArray1[13] = "  L  ";
                num3 = num1 - this.RX;
                strArray1[14] = num3.ToString((IFormatProvider)this.culture);
                strArray1[15] = " ";
                strArray1[16] = y.ToString((IFormatProvider)this.culture);
                strArray1[17] = " Q ";
                strArray1[18] = num1.ToString((IFormatProvider)this.culture);
                strArray1[19] = " ";
                strArray1[20] = y.ToString((IFormatProvider)this.culture);
                strArray1[21] = " ";
                strArray1[22] = num1.ToString((IFormatProvider)this.culture);
                strArray1[23] = " ";
                num3 = y + this.RY;
                strArray1[24] = num3.ToString((IFormatProvider)this.culture);
                string str2 = string.Concat(strArray1);
                string[] strArray2 = new string[13];
                strArray2[0] = str2;
                strArray2[1] = " L ";
                strArray2[2] = num1.ToString((IFormatProvider)this.culture);
                strArray2[3] = " ";
                num3 = num2 - this.RY;
                strArray2[4] = num3.ToString((IFormatProvider)this.culture);
                strArray2[5] = " Q ";
                strArray2[6] = num1.ToString((IFormatProvider)this.culture);
                strArray2[7] = " ";
                strArray2[8] = num2.ToString((IFormatProvider)this.culture);
                strArray2[9] = " ";
                num3 = num1 - this.RX;
                strArray2[10] = num3.ToString((IFormatProvider)this.culture);
                strArray2[11] = " ";
                strArray2[12] = num2.ToString((IFormatProvider)this.culture);
                string str3 = string.Concat(strArray2) + " L " + (arrowLocation.X + this.arrowPadding / 2f).ToString((IFormatProvider)this.culture) + " " + num2.ToString((IFormatProvider)this.culture);
                if (this.RenderArrow)
                {
                    string[] strArray3 = new string[5]
                    {
            str3,
            " L ",
            null,
            null,
            null
                    };
                    num3 = (double)tipLocation.X + tipRadius;
                    strArray3[2] = num3.ToString((IFormatProvider)this.culture);
                    strArray3[3] = " ";
                    num3 = num2 + (double)this.arrowPadding - tipRadius;
                    strArray3[4] = num3.ToString((IFormatProvider)this.culture);
                    string str4 = string.Concat(strArray3);
                    string[] strArray4 = new string[9];
                    strArray4[0] = str4;
                    strArray4[1] = " Q ";
                    strArray4[2] = tipLocation.X.ToString((IFormatProvider)this.culture);
                    strArray4[3] = " ";
                    num3 = num2 + (double)this.arrowPadding;
                    strArray4[4] = num3.ToString((IFormatProvider)this.culture);
                    strArray4[5] = " ";
                    num3 = (double)tipLocation.X - tipRadius;
                    strArray4[6] = num3.ToString((IFormatProvider)this.culture);
                    strArray4[7] = " ";
                    num3 = num2 + (double)this.arrowPadding - tipRadius;
                    strArray4[8] = num3.ToString((IFormatProvider)this.culture);
                    str3 = string.Concat(strArray4);
                }
                if ((double)arrowLocation.X - (double)this.arrowPadding / 2.0 > x)
                {
                    string[] strArray3 = new string[18];
                    strArray3[0] = str3;
                    strArray3[1] = " L ";
                    strArray3[2] = (arrowLocation.X - this.arrowPadding / 2f).ToString((IFormatProvider)this.culture);
                    strArray3[3] = " ";
                    strArray3[4] = num2.ToString((IFormatProvider)this.culture);
                    strArray3[5] = " L ";
                    num3 = x + this.RX;
                    strArray3[6] = num3.ToString((IFormatProvider)this.culture);
                    strArray3[7] = " ";
                    strArray3[8] = num2.ToString((IFormatProvider)this.culture);
                    strArray3[9] = " Q ";
                    strArray3[10] = x.ToString((IFormatProvider)this.culture);
                    strArray3[11] = " ";
                    strArray3[12] = num2.ToString((IFormatProvider)this.culture);
                    strArray3[13] = " ";
                    strArray3[14] = x.ToString((IFormatProvider)this.culture);
                    strArray3[15] = " ";
                    num3 = num2 - this.RY;
                    strArray3[16] = num3.ToString((IFormatProvider)this.culture);
                    strArray3[17] = " z";
                    str1 = string.Concat(strArray3);
                }
                else
                {
                    double num4 = this.RenderArrow ? num2 + this.RY : num2 + this.RY - 2.0;
                    str1 = str3 + " L " + x.ToString((IFormatProvider)this.culture) + " " + num4.ToString((IFormatProvider)this.culture) + " z";
                }
            }
            else if (isBottom)
            {
                string[] strArray1 = new string[13];
                strArray1[0] = empty;
                strArray1[1] = "M ";
                strArray1[2] = x.ToString((IFormatProvider)this.culture);
                strArray1[3] = " ";
                double num3 = y + this.RY;
                strArray1[4] = num3.ToString((IFormatProvider)this.culture);
                strArray1[5] = " Q ";
                strArray1[6] = x.ToString((IFormatProvider)this.culture);
                strArray1[7] = " ";
                strArray1[8] = y.ToString((IFormatProvider)this.culture);
                strArray1[9] = " ";
                num3 = x + this.RX;
                strArray1[10] = num3.ToString((IFormatProvider)this.culture);
                strArray1[11] = " ";
                strArray1[12] = y.ToString((IFormatProvider)this.culture);
                string str2 = string.Concat(strArray1);
                if (this.RenderArrow)
                {
                    string[] strArray2 = new string[5]
                    {
            str2 + " L " + (arrowLocation.X - this.arrowPadding / 2f).ToString((IFormatProvider) this.culture) + " " + y.ToString((IFormatProvider) this.culture),
            " L ",
            null,
            null,
            null
                    };
                    num3 = (double)tipLocation.X - tipRadius;
                    strArray2[2] = num3.ToString((IFormatProvider)this.culture);
                    strArray2[3] = " ";
                    num3 = (double)arrowLocation.Y + tipRadius;
                    strArray2[4] = num3.ToString((IFormatProvider)this.culture);
                    string[] strArray3 = new string[9]
                    {
            string.Concat(strArray2),
            " Q ",
            tipLocation.X.ToString((IFormatProvider) this.culture),
            " ",
            arrowLocation.Y.ToString((IFormatProvider) this.culture),
            " ",
            null,
            null,
            null
                    };
                    num3 = (double)tipLocation.X + tipRadius;
                    strArray3[6] = num3.ToString((IFormatProvider)this.culture);
                    strArray3[7] = " ";
                    num3 = (double)arrowLocation.Y + tipRadius;
                    strArray3[8] = num3.ToString((IFormatProvider)this.culture);
                    str2 = string.Concat(strArray3);
                }
                string[] strArray4 = new string[17];
                strArray4[0] = str2;
                strArray4[1] = " L ";
                strArray4[2] = (arrowLocation.X + this.arrowPadding / 2f).ToString((IFormatProvider)this.culture);
                strArray4[3] = " ";
                strArray4[4] = y.ToString((IFormatProvider)this.culture);
                strArray4[5] = " L ";
                num3 = num1 - this.RX;
                strArray4[6] = num3.ToString((IFormatProvider)this.culture);
                strArray4[7] = " ";
                strArray4[8] = y.ToString((IFormatProvider)this.culture);
                strArray4[9] = " Q ";
                strArray4[10] = num1.ToString((IFormatProvider)this.culture);
                strArray4[11] = " ";
                strArray4[12] = y.ToString((IFormatProvider)this.culture);
                strArray4[13] = " ";
                strArray4[14] = num1.ToString((IFormatProvider)this.culture);
                strArray4[15] = " ";
                num3 = y + this.RY;
                strArray4[16] = num3.ToString((IFormatProvider)this.culture);
                string str3 = string.Concat(strArray4);
                string[] strArray5 = new string[26];
                strArray5[0] = str3;
                strArray5[1] = " L ";
                strArray5[2] = num1.ToString((IFormatProvider)this.culture);
                strArray5[3] = " ";
                num3 = num2 - this.RY;
                strArray5[4] = num3.ToString((IFormatProvider)this.culture);
                strArray5[5] = " Q ";
                strArray5[6] = num1.ToString((IFormatProvider)this.culture);
                strArray5[7] = " ";
                strArray5[8] = num2.ToString((IFormatProvider)this.culture);
                strArray5[9] = " ";
                num3 = num1 - this.RX;
                strArray5[10] = num3.ToString((IFormatProvider)this.culture);
                strArray5[11] = " ";
                strArray5[12] = num2.ToString((IFormatProvider)this.culture);
                strArray5[13] = " L ";
                num3 = x + this.RX;
                strArray5[14] = num3.ToString((IFormatProvider)this.culture);
                strArray5[15] = " ";
                strArray5[16] = num2.ToString((IFormatProvider)this.culture);
                strArray5[17] = " Q ";
                strArray5[18] = x.ToString((IFormatProvider)this.culture);
                strArray5[19] = " ";
                strArray5[20] = num2.ToString((IFormatProvider)this.culture);
                strArray5[21] = " ";
                strArray5[22] = x.ToString((IFormatProvider)this.culture);
                strArray5[23] = " ";
                num3 = num2 - this.RY;
                strArray5[24] = num3.ToString((IFormatProvider)this.culture);
                strArray5[25] = " z";
                str1 = string.Concat(strArray5);
            }
            else if (isLeft)
            {
                string[] strArray1 = new string[13];
                strArray1[0] = empty;
                strArray1[1] = "M ";
                strArray1[2] = x.ToString((IFormatProvider)this.culture);
                strArray1[3] = " ";
                double num3 = y + this.RY;
                strArray1[4] = num3.ToString((IFormatProvider)this.culture);
                strArray1[5] = " Q ";
                strArray1[6] = x.ToString((IFormatProvider)this.culture);
                strArray1[7] = " ";
                strArray1[8] = y.ToString((IFormatProvider)this.culture);
                strArray1[9] = " ";
                num3 = x + this.RX;
                strArray1[10] = num3.ToString((IFormatProvider)this.culture);
                strArray1[11] = " ";
                strArray1[12] = y.ToString((IFormatProvider)this.culture);
                string str2 = string.Concat(strArray1);
                string[] strArray2 = new string[17];
                strArray2[0] = str2;
                strArray2[1] = " L ";
                num3 = num1 - this.RX;
                strArray2[2] = num3.ToString((IFormatProvider)this.culture);
                strArray2[3] = " ";
                strArray2[4] = y.ToString((IFormatProvider)this.culture);
                strArray2[5] = " Q ";
                strArray2[6] = num1.ToString((IFormatProvider)this.culture);
                strArray2[7] = " ";
                strArray2[8] = y.ToString((IFormatProvider)this.culture);
                strArray2[9] = " ";
                strArray2[10] = num1.ToString((IFormatProvider)this.culture);
                strArray2[11] = " ";
                num3 = y + this.RY;
                strArray2[12] = num3.ToString((IFormatProvider)this.culture);
                strArray2[13] = " L ";
                strArray2[14] = num1.ToString((IFormatProvider)this.culture);
                strArray2[15] = " ";
                strArray2[16] = (arrowLocation.Y - this.arrowPadding / 2f).ToString((IFormatProvider)this.culture);
                string[] strArray3 = new string[5]
                {
          string.Concat(strArray2),
          " L ",
          null,
          null,
          null
                };
                num3 = num1 + (double)this.arrowPadding - tipRadius;
                strArray3[2] = num3.ToString((IFormatProvider)this.culture);
                strArray3[3] = " ";
                num3 = (double)tipLocation.Y - tipRadius;
                strArray3[4] = num3.ToString((IFormatProvider)this.culture);
                string str3 = string.Concat(strArray3);
                string[] strArray4 = new string[9];
                strArray4[0] = str3;
                strArray4[1] = " Q ";
                num3 = num1 + (double)this.arrowPadding;
                strArray4[2] = num3.ToString((IFormatProvider)this.culture);
                strArray4[3] = " ";
                strArray4[4] = tipLocation.Y.ToString((IFormatProvider)this.culture);
                strArray4[5] = " ";
                num3 = num1 + (double)this.arrowPadding - tipRadius;
                strArray4[6] = num3.ToString((IFormatProvider)this.culture);
                strArray4[7] = " ";
                num3 = (double)tipLocation.Y + tipRadius;
                strArray4[8] = num3.ToString((IFormatProvider)this.culture);
                string str4 = string.Concat(strArray4);
                string[] strArray5 = new string[17];
                strArray5[0] = str4;
                strArray5[1] = " L ";
                strArray5[2] = num1.ToString((IFormatProvider)this.culture);
                strArray5[3] = " ";
                strArray5[4] = (arrowLocation.Y + this.arrowPadding / 2f).ToString((IFormatProvider)this.culture);
                strArray5[5] = " L ";
                strArray5[6] = num1.ToString((IFormatProvider)this.culture);
                strArray5[7] = " ";
                num3 = num2 - this.RY;
                strArray5[8] = num3.ToString((IFormatProvider)this.culture);
                strArray5[9] = " Q ";
                strArray5[10] = num1.ToString((IFormatProvider)this.culture);
                strArray5[11] = " ";
                strArray5[12] = num2.ToString((IFormatProvider)this.culture);
                strArray5[13] = " ";
                num3 = num1 - this.RX;
                strArray5[14] = num3.ToString((IFormatProvider)this.culture);
                strArray5[15] = " ";
                strArray5[16] = num2.ToString((IFormatProvider)this.culture);
                string str5 = string.Concat(strArray5);
                string[] strArray6 = new string[14];
                strArray6[0] = str5;
                strArray6[1] = " L ";
                num3 = x + this.RX;
                strArray6[2] = num3.ToString((IFormatProvider)this.culture);
                strArray6[3] = " ";
                strArray6[4] = num2.ToString((IFormatProvider)this.culture);
                strArray6[5] = " Q ";
                strArray6[6] = x.ToString((IFormatProvider)this.culture);
                strArray6[7] = " ";
                strArray6[8] = num2.ToString((IFormatProvider)this.culture);
                strArray6[9] = " ";
                strArray6[10] = x.ToString((IFormatProvider)this.culture);
                strArray6[11] = " ";
                num3 = num2 - this.RY;
                strArray6[12] = num3.ToString((IFormatProvider)this.culture);
                strArray6[13] = " z";
                str1 = string.Concat(strArray6);
            }
            else
            {
                string[] strArray1 = new string[17]
                {
          empty,
          "M ",
          (x + this.RX).ToString((IFormatProvider) this.culture),
          " ",
          y.ToString((IFormatProvider) this.culture),
          " Q ",
          x.ToString((IFormatProvider) this.culture),
          " ",
          y.ToString((IFormatProvider) this.culture),
          " ",
          x.ToString((IFormatProvider) this.culture),
          " ",
          (y + this.RY).ToString((IFormatProvider) this.culture),
          " L ",
          x.ToString((IFormatProvider) this.culture),
          " ",
          null
                };
                float num3 = arrowLocation.Y - this.arrowPadding / 2f;
                strArray1[16] = num3.ToString((IFormatProvider)this.culture);
                string[] strArray2 = new string[5]
                {
          string.Concat(strArray1),
          " L ",
          null,
          null,
          null
                };
                double num4 = x - (double)this.arrowPadding + tipRadius;
                strArray2[2] = num4.ToString((IFormatProvider)this.culture);
                strArray2[3] = " ";
                num4 = (double)tipLocation.Y - tipRadius;
                strArray2[4] = num4.ToString((IFormatProvider)this.culture);
                string str2 = string.Concat(strArray2);
                string[] strArray3 = new string[9];
                strArray3[0] = str2;
                strArray3[1] = " Q ";
                num4 = x - (double)this.arrowPadding;
                strArray3[2] = num4.ToString((IFormatProvider)this.culture);
                strArray3[3] = " ";
                num3 = tipLocation.Y;
                strArray3[4] = num3.ToString((IFormatProvider)this.culture);
                strArray3[5] = " ";
                double num5 = x - (double)this.arrowPadding + tipRadius;
                strArray3[6] = num5.ToString((IFormatProvider)this.culture);
                strArray3[7] = " ";
                num5 = (double)tipLocation.Y + tipRadius;
                strArray3[8] = num5.ToString((IFormatProvider)this.culture);
                str1 = string.Concat(strArray3) + " L " + x.ToString((IFormatProvider)this.culture) + " " + (arrowLocation.Y + this.arrowPadding / 2f).ToString((IFormatProvider)this.culture) + " L " + x.ToString((IFormatProvider)this.culture) + " " + (num2 - this.RY).ToString((IFormatProvider)this.culture) + " Q " + x.ToString((IFormatProvider)this.culture) + " " + num2.ToString((IFormatProvider)this.culture) + " " + (x + this.RX).ToString((IFormatProvider)this.culture) + " " + num2.ToString((IFormatProvider)this.culture) + " L " + (num1 - this.RX).ToString((IFormatProvider)this.culture) + " " + num2.ToString((IFormatProvider)this.culture) + " Q " + num1.ToString((IFormatProvider)this.culture) + " " + num2.ToString((IFormatProvider)this.culture) + " " + num1.ToString((IFormatProvider)this.culture) + " " + (num2 - this.RY).ToString((IFormatProvider)this.culture) + " L " + num1.ToString((IFormatProvider)this.culture) + " " + (y + this.RY).ToString((IFormatProvider)this.culture) + " Q " + num1.ToString((IFormatProvider)this.culture) + " " + y.ToString((IFormatProvider)this.culture) + " " + (num1 - this.RX).ToString((IFormatProvider)this.culture) + " " + y.ToString((IFormatProvider)this.culture) + " z";
            }
            return str1;
        }

        private RectangleF GetTooltipLocation(
          out PointF arrowLocation,
          out PointF tipLocation)
        {
            arrowLocation = new PointF(0.0f, 0.0f);
            tipLocation = new PointF(0.0f, 0.0f);
            PointF pointF1 = new PointF()
            {
                X = (float)this.LocationX,
                Y = (float)this.LocationY
            };
            double num1 = (double)this.elementSize.Width + 2.0 * this.MarginX;
            double num2 = (double)this.elementSize.Height + 2.0 * this.MarginY;
            double x = this.X;
            double y = this.Y;
            PointF pointF2;
            if (!this.IsInverted)
            {
                pointF2 = new PointF()
                {
                    X = pointF1.X - this.elementSize.Width / 2f - (float)this.padding,
                    Y = pointF1.Y - this.elementSize.Height - (float)(2 * this.padding) - this.arrowPadding
                };
                arrowLocation.X = tipLocation.X = (float)(num1 / 2.0);
                if ((double)pointF2.Y < y)
                    pointF2.Y = this.LocationY < 0.0 ? 0.0f : (float)this.LocationY;
                if ((double)pointF2.Y + num2 + (double)this.arrowPadding > y + this.Height)
                    pointF2.Y = (this.LocationY > this.Height ? (float)this.Height : (float)this.LocationY) - this.elementSize.Height - (float)(2 * this.padding) - this.arrowPadding;
                tipLocation.X = (float)num1 / 2f;
                if ((double)pointF2.X < x)
                {
                    arrowLocation.X -= (float)x - pointF2.X;
                    tipLocation.X -= (float)x - pointF2.X;
                    pointF2.X = (float)x;
                }
                if ((double)pointF2.X + num1 > x + this.Width)
                {
                    arrowLocation.X += (float)((double)pointF2.X + num1 - (x + this.Width));
                    tipLocation.X += (float)((double)pointF2.X + num1 - (x + this.Width));
                    pointF2.X -= (float)((double)pointF2.X + num1 - (x + this.Width));
                }
                if ((double)arrowLocation.X + (double)this.arrowPadding / 2.0 > num1 - this.RX)
                {
                    arrowLocation.X = (float)(num1 - this.RX - (double)this.arrowPadding / 2.0);
                    tipLocation.X = (float)num1;
                    this.tipRadius = 0.0;
                }
                if ((double)arrowLocation.X - (double)this.arrowPadding / 2.0 < this.RX)
                {
                    arrowLocation.X = (float)(this.RX + (double)this.arrowPadding / 2.0);
                    tipLocation.X = 0.0f;
                    this.tipRadius = 0.0;
                }
            }
            else
            {
                pointF2 = new PointF()
                {
                    X = pointF1.X,
                    Y = pointF1.Y - this.elementSize.Height / 2f - (float)this.padding
                };
                arrowLocation.Y = tipLocation.Y = (float)(num2 / 2.0);
                if ((double)pointF2.X + num1 + (double)this.arrowPadding > x + this.Width)
                    pointF2.X = (float)((this.LocationX > this.Width ? this.Width : this.LocationX) - (num1 + (double)this.arrowPadding));
                if ((double)pointF2.X < x)
                    pointF2.X = this.LocationX < 0.0 ? 0.0f : (float)this.LocationX;
                if ((double)pointF2.Y <= y)
                {
                    arrowLocation.Y -= (float)y - pointF2.Y;
                    pointF2.Y -= (float)y - pointF2.Y;
                    pointF2.Y = (float)y;
                }
                if ((double)pointF2.Y + num2 >= y + this.Height)
                {
                    arrowLocation.Y += (float)((double)pointF2.Y + num2 - (y + this.Height));
                    tipLocation.Y += (float)((double)pointF2.Y + num2 - (y + this.Height));
                    pointF2.Y -= (float)((double)pointF2.Y + num2 - (y + this.Height));
                }
                if ((double)arrowLocation.Y + (double)this.arrowPadding / 2.0 > num2 - this.RY)
                {
                    arrowLocation.Y = (float)(num2 - this.RY - (double)this.arrowPadding / 2.0);
                    tipLocation.Y = (float)num2;
                    this.tipRadius = 0.0;
                }
                if ((double)arrowLocation.Y - (double)this.arrowPadding / 2.0 < this.RY)
                {
                    arrowLocation.Y = (float)(this.RY + (double)this.arrowPadding / 2.0);
                    tipLocation.Y = 0.0f;
                    this.tipRadius = 0.0;
                }
            }
            return new RectangleF()
            {
                X = this.ControlName == "LinearGauge" ? (float)this.X : pointF2.X,
                Y = this.ControlName == "LinearGauge" ? (float)this.Y : pointF2.Y,
                Width = (float)num1,
                Height = (float)num2
            };
        }

        private string ChangeText(bool isBottom, bool isRight)
        {
            string str = string.Empty;
            if (isBottom)
                str = "translate(0," + this.arrowPadding.ToString((IFormatProvider)this.culture) + ")";
            else if (isRight)
                str = "translate(" + this.arrowPadding.ToString((IFormatProvider)this.culture) + ")";
            return str;
        }

        internal SizeF MeasureText(string text, double fontSize)
        {
            SizeF sizeF = new SizeF();
            double num = 0.0;
            double val2 = 0.0;
            for (int index = 0; index < text.Length; ++index)
            {
                SizeF charSize = SvgTooltip.GetCharSize(text[index]);
                num += (double)charSize.Width;
                val2 = Math.Max((double)charSize.Height, val2);
            }
            return new SizeF()
            {
                Width = (float)(num * fontSize / 100.0),
                Height = (float)(val2 * fontSize / 100.0)
            };
        }

        private static SizeF GetCharSize(char character)
        {
            double num = 75.0;
            new FontInfo().Chars.TryGetValue(character, out num);
            return new SizeF()
            {
                Width = (float)(num * 100.0 / 16.0),
                Height = 130f
            };
        }

        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "id", this.ID + "Tooltip");
            __builder.AddAttribute(2, "style", "pointer-events:none; position:absolute; left:" + this.elementLeft.ToString((IFormatProvider)this.culture) + "px; top:" + this.elementTop.ToString((IFormatProvider)this.culture) + "px;");
            __builder.OpenElement(3, "svg");
            __builder.AddAttribute(4, "id", this.ID + "Tooltip_svg");
            __builder.AddAttribute(5, "opacity", this.Opacity.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(6, "height", this.svgProperties.Height.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(7, "width", this.svgProperties.Width.ToString((IFormatProvider)this.culture));
            __builder.OpenElement(8, "g");
            __builder.AddAttribute(9, "id", this.ID + "Tooltip_group");
            __builder.AddAttribute(10, "transform", "translate(0,0)");
            __builder.OpenElement(11, "path");
            __builder.AddAttribute(12, "id", this.ID + "Tooltip_path");
            __builder.AddAttribute(13, "d", this.path.Direction);
            __builder.AddAttribute(14, "stroke", this.path.Stroke);
            __builder.AddAttribute(15, "stroke-width", this.BorderWidth.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(16, "opacity", this.path.Opacity.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(17, "fill", this.path.Fill);
            __builder.AddAttribute(18, "filter", this.IsIE ? string.Empty : "url(#" + this.ID + "Tooltip_shadow)");
            __builder.CloseElement();
            __builder.AddMarkupContent(19, "\r\n            ");
            __builder.OpenElement(20, "text");
            __builder.AddAttribute(21, "id", this.ID + "Tooltip_text");
            __builder.AddAttribute(22, "x", this.textSetting.X.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(23, "y", this.textSetting.Y.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(24, "transform", this.textSetting.Transform);
            __builder.AddAttribute(25, "text-anchor", this.textSetting.Anchor);
            __builder.AddAttribute(26, "font-family", this.FontFamily);
            __builder.AddAttribute(27, "font-style", this.FontStyle);
            __builder.AddAttribute(28, "font-size", this.TextSize.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(29, "opacity", this.FontOpacity.ToString((IFormatProvider)this.culture));
            __builder.AddAttribute(30, "font-weight", this.FontWeight);
            double num;
            for (int index = 0; index < this.textCollection.Count; ++index)
            {
                if (!double.IsNaN(this.textCollection[index].X))
                {
                    if (!double.IsNaN(this.textCollection[index].DY))
                    {
                        __builder.OpenElement(31, "tspan");
                        RenderTreeBuilder renderTreeBuilder1 = __builder;
                        num = this.textCollection[index].X;
                        string str1 = num.ToString((IFormatProvider)this.culture);
                        renderTreeBuilder1.AddAttribute(32, "x", str1);
                        RenderTreeBuilder renderTreeBuilder2 = __builder;
                        num = this.textCollection[index].DY;
                        string str2 = num.ToString((IFormatProvider)this.culture);
                        renderTreeBuilder2.AddAttribute(33, "dy", str2);
                        __builder.AddAttribute(34, "fill", this.FontColor);
                        __builder.AddAttribute(35, "font-weight", this.textCollection[index].FontWeight);
                        __builder.AddContent(36, this.textCollection[index].Text);
                        __builder.CloseElement();
                    }
                    else
                    {
                        __builder.OpenElement(37, "tspan");
                        RenderTreeBuilder renderTreeBuilder = __builder;
                        num = this.textCollection[index].X;
                        string str = num.ToString((IFormatProvider)this.culture);
                        renderTreeBuilder.AddAttribute(38, "x", str);
                        __builder.AddAttribute(39, "fill", this.FontColor);
                        __builder.AddAttribute(40, "font-weight", this.textCollection[index].FontWeight);
                        __builder.AddContent(41, this.textCollection[index].Text);
                        __builder.CloseElement();
                    }
                }
                else
                {
                    __builder.OpenElement(42, "tspan");
                    __builder.AddAttribute(43, "fill", this.FontColor);
                    __builder.AddAttribute(44, "font-weight", this.textCollection[index].FontWeight);
                    __builder.AddContent(45, this.textCollection[index].Text);
                    __builder.CloseElement();
                }
            }
            __builder.CloseElement();
            if (this.EnableShadow)
            {
                __builder.OpenElement(46, "defs");
                __builder.AddAttribute(47, "id", this.ID + "TooltipSVG_tooltip_definition");
                __builder.OpenElement(48, "filter");
                __builder.AddAttribute(49, "id", this.ID + "Tooltip_shadow");
                __builder.AddAttribute(50, "height", "130%");
                __builder.AddMarkupContent(51, "<feGaussianBlur in=\"SourceAlpha\" stdDeviation=\"3\"></feGaussianBlur>\r\n                        <feOffset dx=\"3\" dy=\"3\" result=\"offsetblur\"></feOffset>\r\n                        ");
                __builder.AddMarkupContent(52, "<feComponentTransfer><feFuncA type=\"linear\" slope=\"0.5\"></feFuncA></feComponentTransfer>\r\n                        ");
                __builder.AddMarkupContent(53, "<feMerge><feMergeNode></feMergeNode>\r\n                            <feMergeNode in=\"SourceGraphic\"></feMergeNode></feMerge>");
                __builder.CloseElement();
                __builder.CloseElement();
            }
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
        }
    }
}