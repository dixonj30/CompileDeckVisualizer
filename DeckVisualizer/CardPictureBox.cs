using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckVisualizer
{
    public class CardPictureBox : PictureBox
    {
        public CardOverlayState CurrentOverlay { get; set; }
        public string CardLabel { get; set; }

        public CardPictureBox()
        {
            this.DoubleBuffered = true;
            this.CurrentOverlay = new CardOverlayState(Color.Transparent, "");
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (this.Image == null && !string.IsNullOrEmpty(CardLabel))
            {
                using (Font font = new Font("Arial", 9, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    pe.Graphics.DrawString(CardLabel, font, brush, this.ClientRectangle, sf);
                }
            }

            if (this.CurrentOverlay.WashColor != Color.Transparent)
            {
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (SolidBrush overlayBrush = new SolidBrush(this.CurrentOverlay.WashColor))
                {
                    pe.Graphics.FillRectangle(overlayBrush, this.ClientRectangle);
                }

                using (Font textFont = new Font("Impact", 13, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    Rectangle shadowRect = new Rectangle(this.ClientRectangle.X + 1, this.ClientRectangle.Y + 1, this.ClientRectangle.Width, this.ClientRectangle.Height);
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(180, Color.Black)))
                    {
                        pe.Graphics.DrawString(this.CurrentOverlay.Label, textFont, shadowBrush, shadowRect, sf);
                    }

                    pe.Graphics.DrawString(this.CurrentOverlay.Label, textFont, textBrush, this.ClientRectangle, sf);
                }
            }
        }
    }
}
