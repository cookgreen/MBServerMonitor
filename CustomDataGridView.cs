using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MBServerMonitor
{
    public class CustomDataGridView : DataGridView
    {
        private Brush blackBrush;
        private Brush whiteBrush;
        private const string NO_DATA_DISPLAY_STR = "(No Data Display)";
        private Font fontNoDataDisplay;

        public CustomDataGridView() : base()
        {
            DoubleBuffered = true;

            blackBrush = new SolidBrush(Color.Black);
            whiteBrush = new SolidBrush(Color.White);

            fontNoDataDisplay = new Font(Font, FontStyle.Italic);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            var num = Rows.Count;
            if (num == 0)
            {
                //Draw one line - No data display.
                int height = RowTemplate.Height;

                int top = height;

                g.FillRectangle(whiteBrush, new Rectangle(0, top, Width, height));

                SizeF strSize = g.MeasureString(NO_DATA_DISPLAY_STR, Font);
                float strTop = (height - strSize.Height) / 2;
                float strLeft = (Width - strSize.Width) / 2;

                PointF pointF = new PointF(strLeft, top + strTop);

                g.DrawString(NO_DATA_DISPLAY_STR, fontNoDataDisplay, blackBrush, pointF);
            }
        }
    }
}
