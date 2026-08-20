using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace DeckVisualizer
{
    public partial class Form1 : Form
    {
        private List<DeckConfig> loadedDecks = new List<DeckConfig>();
        private const int WindowWidth = 870;
        private const int WindowHeight = 955;
        private FlowLayoutPanel[] p1Rows = new FlowLayoutPanel[3];
        private FlowLayoutPanel[] p2Rows = new FlowLayoutPanel[3];
        private Button p1Deck1Menu, p1Deck2Menu, p1Deck3Menu;
        private Button p2Deck1Menu, p2Deck2Menu, p2Deck3Menu;

        private static readonly Color DefaultButtonColor = Color.FromArgb(45, 110, 75);
        private static readonly Color ActiveButtonColor = Color.FromArgb(40, 75, 95);
        private static readonly string[] ButtonDefaultLabels = new string[]
        {
            "Select P1\nDeck 1", "Select P1\nDeck 2", "Select P1\nDeck 3",
            "Select P2\nDeck 1", "Select P2\nDeck 2", "Select P2\nDeck 3"
        };

        private static readonly Color AppMainBackgroundColor = Color.FromArgb(35, 35, 40);
        private static readonly Color DefaultCardBackgroundColor = Color.DimGray;
        private static readonly string CardEmptyPlaceholderText = "Empty";
        private static readonly string CardMissingPlaceholderText = "Missing";

        private static readonly string AppTitleHeader = "Compile Deck Tracker";
        private static readonly string ResetButtonLabel = "RESET";
        private static readonly string EditorButtonLabel = "EDITOR";

        private static readonly string StorageEmptyMessage = "No custom decks found! Open the Deck Editor to build your first card deck profile.";
        private static readonly string StorageEmptyTitle = "Empty Storage";       

        public static readonly string DecksDatabaseFileName = "decks.json";
        public static readonly string ResourcesFolderName = "resources";
        public static readonly string ImageFileFilters = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            this.Size = new Size(WindowWidth, WindowHeight);
            this.MinimumSize = new Size(WindowWidth, WindowHeight);

            Panel customTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = Color.FromArgb(30, 30, 35)
            };

            Label lblTitle = new Label
            {
                Text = AppTitleHeader,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Location = new Point(WindowWidth / 2, 5),
                AutoSize = true
            };
            customTitleBar.Controls.Add(lblTitle);

            Button btnClose = new Button
            {
                Text = "X",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 55),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(35, 21),
                Location = new Point(WindowWidth - 45, 2),
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            customTitleBar.Controls.Add(btnClose);

            customTitleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    customTitleBar.Capture = false;
                    Message msg = Message.Create(this.Handle, 0XA1, new IntPtr(2), IntPtr.Zero);
                    this.DefWndProc(ref msg);
                }
            };

            this.Controls.Add(customTitleBar);

            LoadDecksFromDisk();
            SetupGameTab();
        }

        private void LoadDecksFromDisk()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DecksDatabaseFileName);
            loadedDecks = new List<DeckConfig>();

            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                    if (settings?.AvailableDecks != null)
                    {
                        loadedDecks = settings.AvailableDecks;
                    }
                }
                catch { }
            }
        }

        private void SetupGameTab()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = new Padding(0),
                Padding = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = AppMainBackgroundColor
            };
            mainLayout.ColumnStyles.Clear();
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 832));

            TableLayoutPanel boardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 7, Margin = new Padding(0), Padding = new Padding(0), AutoScroll = true, BackColor = AppMainBackgroundColor };
            boardLayout.RowStyles.Clear();
            for (int r = 0; r < 7; r++)
            {
                if (r == 3) boardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
                else boardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            TableLayoutPanel horizontalDivider = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(50, 50, 55),
                Margin = new Padding(0),
                Padding = new Padding(0, 2, 0, 2),
                ColumnCount = 2,
                RowCount = 1
            };
            horizontalDivider.ColumnStyles.Clear();
            horizontalDivider.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148));
            horizontalDivider.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            horizontalDivider.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Button btnOpenEditorRow = new Button
            {
                Text = EditorButtonLabel,
                Width = 60,
                Height = 16,
                BackColor = Color.FromArgb(50, 65, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 6f, FontStyle.Bold),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
                UseCompatibleTextRendering = true
            };
            btnOpenEditorRow.FlatAppearance.BorderSize = 0;
            btnOpenEditorRow.Click += (s, e) =>
            {
                using (EditorWindow popup = new EditorWindow(RefreshAllMenus))
                {
                    popup.ShowDialog(this);
                }
            };
            horizontalDivider.Controls.Add(btnOpenEditorRow, 0, 0);

            Button btnResetBoard = new Button
            {
                Text = ResetButtonLabel,
                Width = 60,
                Height = 16,
                BackColor = Color.FromArgb(110, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 6f, FontStyle.Bold),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 120, 0),
                UseCompatibleTextRendering = true
            };
            btnResetBoard.FlatAppearance.BorderSize = 0;
            btnResetBoard.Click += MasterResetButton_Click;
            horizontalDivider.Controls.Add(btnResetBoard, 1, 0);
            boardLayout.Controls.Add(horizontalDivider, 0, 3);
            boardLayout.SetColumnSpan(horizontalDivider, 2);

            p1Deck1Menu = new Button { Text = ButtonDefaultLabels[0], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 2, 4, 2) };
            p1Deck2Menu = new Button { Text = ButtonDefaultLabels[1], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 3, 4, 3) };
            p1Deck3Menu = new Button { Text = ButtonDefaultLabels[2], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 3, 4, 3) };

            p2Deck1Menu = new Button { Text = ButtonDefaultLabels[3], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 3, 4, 3) };
            p2Deck2Menu = new Button { Text = ButtonDefaultLabels[4], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 3, 4, 3) };
            p2Deck3Menu = new Button { Text = ButtonDefaultLabels[5], Width = 128, Height = 146, BackColor = Color.FromArgb(45, 110, 75), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None, Margin = new Padding(4, 3, 4, 3) };

            p1Deck1Menu.Click += (s, e) => OpenVisualDeckPicker(p1Rows[0], "P1 Deck 1", p1Deck1Menu);
            p1Deck2Menu.Click += (s, e) => OpenVisualDeckPicker(p1Rows[1], "P1 Deck 2", p1Deck2Menu);
            p1Deck3Menu.Click += (s, e) => OpenVisualDeckPicker(p1Rows[2], "P1 Deck 3", p1Deck3Menu);

            p2Deck1Menu.Click += (s, e) => OpenVisualDeckPicker(p2Rows[0], "P2 Deck 1", p2Deck1Menu);
            p2Deck2Menu.Click += (s, e) => OpenVisualDeckPicker(p2Rows[1], "P2 Deck 2", p2Deck2Menu);
            p2Deck3Menu.Click += (s, e) => OpenVisualDeckPicker(p2Rows[2], "P2 Deck 3", p2Deck3Menu);

            boardLayout.Controls.Add(p1Deck1Menu, 0, 0);
            boardLayout.Controls.Add(p1Deck2Menu, 0, 1);
            boardLayout.Controls.Add(p1Deck3Menu, 0, 2);

            boardLayout.Controls.Add(p2Deck1Menu, 0, 4);
            boardLayout.Controls.Add(p2Deck2Menu, 0, 5);
            boardLayout.Controls.Add(p2Deck3Menu, 0, 6);

            for (int i = 0; i < 3; i++)
            {
                p1Rows[i] = new FlowLayoutPanel { Dock = DockStyle.Top, Width = 735, Height = (i == 0) ? 150 : 152, AutoScroll = false, BackColor = Color.FromArgb(40 + (i * 5), 40, 40), Margin = new Padding(0), Padding = new Padding(0, (i == 0) ? 2 : 0, 0, 0) };
                p2Rows[i] = new FlowLayoutPanel { Dock = DockStyle.Top, Width = 735, Height = 152, AutoScroll = false, BackColor = Color.FromArgb(50 + (i * 5), 50, 50), Margin = new Padding(0), Padding = new Padding(0) };

                boardLayout.Controls.Add(p1Rows[i], 1, i);
                boardLayout.Controls.Add(p2Rows[i], 1, i + 4);

                InitializeRowCards(p1Rows[i], $"P1 Row {i + 1}");
                InitializeRowCards(p2Rows[i], $"P2 Row {i + 1}");
            }

            mainLayout.Controls.Add(boardLayout, 0, 0);
            mainLayout.SetColumnSpan(boardLayout, 2);

            this.Controls.Add(mainLayout);
            mainLayout.BringToFront();

            RefreshAllMenus();
        }

        private void OpenVisualDeckPicker(FlowLayoutPanel targetRowPanel, string slotNamePrefix, Button sourceButton)
        {
            LoadDecksFromDisk();

            if (loadedDecks.Count == 0)
            {
                MessageBox.Show(StorageEmptyMessage, StorageEmptyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (DeckPickerWindow picker = new DeckPickerWindow(loadedDecks))
            {
                if (picker.ShowDialog(this) == DialogResult.OK && picker.SelectedDeck != null)
                {
                    // Apply the selected deck to the grid row
                    UpdateSingleRowDisplay(targetRowPanel, picker.SelectedDeck, slotNamePrefix);
                    sourceButton.Text = "";

                    // HIGH-PERFORMANCE TITLE IMAGE PASSING ENGINE
                    if (picker.SelectedDeckImage != null)
                    {
                        if (sourceButton.Image != null) sourceButton.Image.Dispose();

                        // 1. Create a blank canvas matching your exact 128x146 button dimensions
                        Bitmap buttonCanvas = new Bitmap(sourceButton.Width, sourceButton.Height);

                        using (Graphics g = Graphics.FromImage(buttonCanvas))
                        {
                            g.Clear(Color.Transparent);

                            // FIXED: Paints the pre-loaded title image flush across the full button width and height with zero clipping!
                            g.DrawImage(picker.SelectedDeckImage, 0, 0, sourceButton.Width, sourceButton.Height);
                        }

                        // Mount the clean, flat graphic directly onto the button background canvas
                        sourceButton.Image = buttonCanvas;
                    }
                    else
                    {
                        sourceButton.BackColor = ActiveButtonColor;
                        sourceButton.Text = picker.SelectedDeck.DeckName;
                    }
                }
            }
        }

        private void RefreshAllMenus() { }

        private void InitializeRowCards(FlowLayoutPanel rowPanel, string labelPrefix)
        {
            rowPanel.Controls.Clear();

            for (int i = 0; i < 6; i++)
            {
                CardPictureBox pic = new CardPictureBox
                {
                    Width = 110,
                    Height = 146,
                    Margin = new Padding(6, 3, 6, 3),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = DefaultCardBackgroundColor,
                    CardLabel = $"{labelPrefix}\nSlot {i + 1}"
                };
                pic.MouseDown += Card_MouseDown;
                rowPanel.Controls.Add(pic);
            }
        }

        private void UpdateSingleRowDisplay(FlowLayoutPanel rowPanel, DeckConfig deck, string labelPrefix)
        {
            if (rowPanel.Controls.Count < 6) return;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < 6; i++)
            {
                CardPictureBox pic = rowPanel.Controls[i] as CardPictureBox;
                if (pic != null)
                {
                    string fullPath = "";
                    if (deck != null && deck.ImagePaths != null && i < deck.ImagePaths.Count && !string.IsNullOrEmpty(deck.ImagePaths[i]))
                    {
                        string cleanPath = deck.ImagePaths[i];

                        if (Path.IsPathRooted(cleanPath))
                        {
                            fullPath = cleanPath;
                        }
                        else
                        {
                            string testLocal = Path.Combine(baseDir, cleanPath);
                            string testLocalRes = Path.Combine(baseDir, ResourcesFolderName, cleanPath);
                            string testRootRes = Path.Combine(baseDir, "..", "..", "..", ResourcesFolderName, cleanPath);

                            if (File.Exists(testLocal)) fullPath = testLocal;
                            else if (File.Exists(testLocalRes)) fullPath = testLocalRes;
                            else if (File.Exists(testRootRes)) fullPath = testRootRes;
                            else fullPath = testLocal;
                        }
                    }

                    if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                    {
                        pic.Image = Image.FromFile(fullPath);
                        pic.CardLabel = "";
                    }
                    else
                    {
                        pic.Image = null;
                        pic.CardLabel = deck != null
                            ? $"{deck.DeckName}\nSlot {i + 1}\n{CardMissingPlaceholderText}"
                            : $"{labelPrefix}\n{CardEmptyPlaceholderText}";
                    }
                }
            }
        }

        private void Card_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is CardPictureBox pic)
            {
                var cycleList = CardOverlayState.OverlayCycleList;
                int currentIndex = cycleList.IndexOf(pic.CurrentOverlay);

                if (currentIndex == -1) currentIndex = 0;

                int maxStates = cycleList.Count;

                if (e.Button == MouseButtons.Left)
                {
                    currentIndex = (currentIndex + 1) % maxStates;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    currentIndex = (currentIndex - 1 + maxStates) % maxStates;
                }
                else
                {
                    return;
                }

                pic.CurrentOverlay = cycleList[currentIndex];

                pic.Invalidate();
            }
        }

        private void MasterResetButton_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < 3; r++)
            {
                foreach (Control c in p1Rows[r].Controls)
                {
                    if (c is CardPictureBox pic)
                    {
                        pic.Image = null;
                        pic.CurrentOverlay = CardOverlayState.OverlayCycleList[0];
                        pic.CardLabel = $"P1 Row {r + 1}\nSlot {p1Rows[r].Controls.IndexOf(pic) + 1}\n{CardEmptyPlaceholderText}";
                        pic.Invalidate();
                    }
                }
            }

            for (int r = 0; r < 3; r++)
            {
                foreach (Control c in p2Rows[r].Controls)
                {
                    if (c is CardPictureBox pic)
                    {
                        pic.Image = null;
                        pic.CurrentOverlay = CardOverlayState.OverlayCycleList[0];
                        pic.CardLabel = $"P2 Row {r + 1}\nSlot {p2Rows[r].Controls.IndexOf(pic) + 1}\n{CardEmptyPlaceholderText}";
                        pic.Invalidate();
                    }
                }
            }

            Button[] deckButtons = { p1Deck1Menu, p1Deck2Menu, p1Deck3Menu, p2Deck1Menu, p2Deck2Menu, p2Deck3Menu };

            for (int i = 0; i < deckButtons.Length; i++)
            {
                if (deckButtons[i] != null)
                {
                    if (deckButtons[i].Image != null)
                    {
                        deckButtons[i].Image.Dispose();
                        deckButtons[i].Image = null;
                    }

                    deckButtons[i].Text = ButtonDefaultLabels[i];
                    deckButtons[i].BackColor = DefaultButtonColor;
                }
            }
        }

        public class DeckConfig
        {
            public string DeckName { get; set; } = "";
            public List<string> ImagePaths { get; set; } = new List<string>();
            public string DeckTitleImage { get; set; } = "";
            public override string ToString() => DeckName;
        }

        public class AppSettings
        {
            public List<DeckConfig> AvailableDecks { get; set; } = new List<DeckConfig>();
        }

    }
}