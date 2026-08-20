using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeckVisualizer.Form1;

namespace DeckVisualizer
{
    public class DeckPickerWindow : Form
    {
        private static readonly string DeckPickerWindowTitle = "Select Card Deck Layout Profile";
        public const int DeckPickerWindowWidth = 800;
        public const int DeckPickerWindowHeight = 550;

        public DeckConfig SelectedDeck { get; private set; }
        public Image SelectedDeckImage { get; private set; }

        public DeckPickerWindow(List<DeckConfig> decks)
        {
            this.Text = DeckPickerWindowTitle;
            this.Size = new Size(DeckPickerWindowWidth, DeckPickerWindowHeight);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(58, 61, 64);

            FlowLayoutPanel pickerGrid = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                Margin = new Padding(0)
            };


            foreach (var deck in decks)
            {
                Panel deckTile = new Panel
                {
                    Width = 128,
                    Height = 146,
                    Margin = new Padding(12),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };

                PictureBox imgCover = new PictureBox
                {
                    Location = new Point(0, 0),
                    Size = new Size(128, 146),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };

                if (!string.IsNullOrEmpty(deck.DeckTitleImage))
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string fullPath = Path.IsPathRooted(deck.DeckTitleImage) ? deck.DeckTitleImage : Path.Combine(baseDir, deck.DeckTitleImage);

                    if (File.Exists(fullPath))
                    {
                        imgCover.Image = Image.FromFile(fullPath);
                    }
                }

                Label lblDeckName = new Label
                {
                    Text = deck.DeckName,
                    Location = new Point(0, 116),
                    Size = new Size(128, 30),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopCenter,
                    BackColor = Color.FromArgb(140, 20, 20, 25)
                };

                imgCover.Controls.Add(lblDeckName);
                deckTile.Controls.Add(imgCover);
                lblDeckName.BringToFront();


                Action selectAction = () =>
                {
                    this.SelectedDeck = deck;
                    this.SelectedDeckImage = imgCover.Image;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                deckTile.Click += (s, e) => selectAction();
                imgCover.Click += (s, e) => selectAction();
                lblDeckName.Click += (s, e) => selectAction();

                pickerGrid.Controls.Add(deckTile);
            }

            this.Controls.Add(pickerGrid);
        }
    }
}
