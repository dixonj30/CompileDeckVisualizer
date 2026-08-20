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
        public const int deckPickerWindowWidth = 800;
        public const int deckPickerWindowHeight = 550;

        public DeckConfig SelectedDeck { get; private set; }
        public Image SelectedDeckImage { get; private set; }

        public DeckPickerWindow(List<DeckConfig> decks)
        {
            this.Text = DeckPickerWindowTitle;
            this.Size = new Size(deckPickerWindowWidth, deckPickerWindowHeight);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(58, 61, 64);

            FlowLayoutPanel pickerGrid = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(15),
                Margin = new Padding(0)
            };


            foreach (var deck in decks)
            {
                Panel deckTile = new Panel
                {
                    Width = deckPickerWindowWidth / 5,
                    Height = deckPickerWindowHeight / 8,
                    Margin = new Padding(3),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };

                PictureBox imgCover = new PictureBox
                {
                    Location = new Point(-10, 0),
                    Size = new Size(deckPickerWindowWidth / 5, deckPickerWindowHeight / 8),
                    SizeMode = PictureBoxSizeMode.Normal,
                    BackColor = Color.Transparent
                };

                if (deck.ImagePaths != null && deck.ImagePaths.Count > 0 && !string.IsNullOrEmpty(deck.ImagePaths[0]))
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string fullPath = Path.IsPathRooted(deck.ImagePaths[0])
                        ? deck.ImagePaths[0]
                        : Path.Combine(baseDir, deck.ImagePaths[0]);

                    if (File.Exists(fullPath))
                    {
                        imgCover.Image = Image.FromFile(fullPath);
                    }
                }

                deckTile.Controls.Add(imgCover);

                Action selectAction = () =>
                {
                    this.SelectedDeck = deck;
                    this.SelectedDeckImage = imgCover.Image;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                deckTile.Click += (s, e) => selectAction();
                imgCover.Click += (s, e) => selectAction();
                pickerGrid.Controls.Add(deckTile);
            }

            this.Controls.Add(pickerGrid);
        }
    }
}
