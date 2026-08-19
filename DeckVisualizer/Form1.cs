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
        private const int windowWidth = 980;
        private const int windowHeight = 955;
        private const int deckPickerWindowWidth = 800;
        private const int deckPickerWindowHeight = 550;
        private FlowLayoutPanel[] p1Rows = new FlowLayoutPanel[3];
        private FlowLayoutPanel[] p2Rows = new FlowLayoutPanel[3];
        private FlowLayoutPanel controlPanel;
        private Button p1Deck1Menu, p1Deck2Menu, p1Deck3Menu;
        private Button p2Deck1Menu, p2Deck2Menu, p2Deck3Menu;

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            this.Size = new Size(windowWidth, windowHeight);
            this.MinimumSize = new Size(windowWidth, windowHeight);

            Panel customTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = Color.FromArgb(30, 30, 35)
            };

            Label lblTitle = new Label
            {
                Text = "Card Deck Layout Engine",
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Location = new Point(12, 5),
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
                Location = new Point(windowWidth - 45, 2),
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
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "decks.json");
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
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            mainLayout.ColumnStyles.Clear();
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 760));

            controlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Width = 210,
                Padding = new Padding(0, 15, 0, 15),
                Margin = new Padding(0),
                AutoScroll = false,
                BackColor = Color.FromArgb(35, 35, 40),
                WrapContents = false
            };

            Label lblP1Title = new Label { Text = "", Width = 200, ForeColor = Color.White, Font = new Font("Arial", 9, FontStyle.Bold), Margin = new Padding(0, 120, 0, 8), Anchor = AnchorStyles.None, TextAlign = ContentAlignment.MiddleCenter };
            controlPanel.Controls.Add(lblP1Title);

            Button btnP1D1 = new Button { Text = "Select P1 Deck 1...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };
            Button btnP1D2 = new Button { Text = "Select P1 Deck 2...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };
            Button btnP1D3 = new Button { Text = "Select P1 Deck 3...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };

            btnP1D1.Click += (s, e) => OpenVisualDeckPicker(p1Rows[0], "P1 Deck 1", btnP1D1);
            btnP1D2.Click += (s, e) => OpenVisualDeckPicker(p1Rows[1], "P1 Deck 2", btnP1D2);
            btnP1D3.Click += (s, e) => OpenVisualDeckPicker(p1Rows[2], "P1 Deck 3", btnP1D3);

            controlPanel.Controls.Add(btnP1D1);
            controlPanel.Controls.Add(btnP1D2);
            controlPanel.Controls.Add(btnP1D3);

            Button btnOpenEditor = new Button { Text = "Open Deck Editor", Width = 150, Height = 35, BackColor = Color.FromArgb(50, 65, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Arial", 9, FontStyle.Bold), Margin = new Padding(0, 175, 0, 140), Anchor = AnchorStyles.None };
            btnOpenEditor.FlatAppearance.BorderSize = 0;
            btnOpenEditor.Click += (s, e) => { using (EditorWindow popup = new EditorWindow(RefreshAllMenus)) { popup.ShowDialog(this); } };
            controlPanel.Controls.Add(btnOpenEditor);

            Label lblP2Title = new Label { Text = "", Width = 200, ForeColor = Color.White, Font = new Font("Arial", 9, FontStyle.Bold), Margin = new Padding(0, 0, 0, 0), Anchor = AnchorStyles.None, TextAlign = ContentAlignment.MiddleCenter };
            controlPanel.Controls.Add(lblP2Title);

            Button btnP2D1 = new Button { Text = "Select P2 Deck 1...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };
            Button btnP2D2 = new Button { Text = "Select P2 Deck 2...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };
            Button btnP2D3 = new Button { Text = "Select P2 Deck 3...", Width = 170, Height = 30, BackColor = Color.FromArgb(50, 50, 55), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.None };

            btnP2D1.Click += (s, e) => OpenVisualDeckPicker(p2Rows[0], "P2 Deck 1", btnP2D1);
            btnP2D2.Click += (s, e) => OpenVisualDeckPicker(p2Rows[1], "P2 Deck 2", btnP2D2);
            btnP2D3.Click += (s, e) => OpenVisualDeckPicker(p2Rows[2], "P2 Deck 3", btnP2D3);

            controlPanel.Controls.Add(btnP2D1);
            controlPanel.Controls.Add(btnP2D2);
            controlPanel.Controls.Add(btnP2D3);

            mainLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainLayout);
            mainLayout.BringToFront();

            TableLayoutPanel boardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 7, Margin = new Padding(0), Padding = new Padding(0), AutoScroll = true };

            boardLayout.RowStyles.Clear();
            for (int r = 0; r < 7; r++)
            {
                if (r == 3)
                {
                    boardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
                }
                else
                {
                    boardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
            }

            TableLayoutPanel horizontalDivider = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(50, 50, 55),
                Margin = new Padding(0),
                Padding = new Padding(0, 2, 0, 2),
                ColumnCount = 1,
                RowCount = 1
            };

            horizontalDivider.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            horizontalDivider.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Button btnResetBoard = new Button
            {
                Text = "RESET",
                Width = 60,
                Height = 16,
                BackColor = Color.FromArgb(110, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 5f, FontStyle.Bold),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0)
            };
            btnResetBoard.FlatAppearance.BorderSize = 0;
            btnResetBoard.Click += MasterResetButton_Click;
            btnResetBoard.Location = new Point(
                (760 - btnResetBoard.Width) / 2,
                (20 - btnResetBoard.Height) / 2
            );
            horizontalDivider.Controls.Add(btnResetBoard);
            boardLayout.Controls.Add(horizontalDivider, 0, 3);

            for (int i = 0; i < 3; i++)
            {
                p1Rows[i] = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Width = 735,
                    Height = (i == 0) ? 150 : 152,
                    AutoScroll = false,
                    BackColor = Color.FromArgb(40 + (i * 5), 40, 40),
                    Margin = new Padding(0),
                    Padding = new Padding(0, (i == 0) ? 2 : 0, 0, 0)
                };

                p2Rows[i] = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Width = 735,
                    Height = 152,
                    AutoScroll = false,
                    BackColor = Color.FromArgb(50 + (i * 5), 50, 50),
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };

                boardLayout.Controls.Add(p1Rows[i], 0, i);
                boardLayout.Controls.Add(p2Rows[i], 0, i + 4);

                InitializeRowCards(p1Rows[i], $"P1 Row {i + 1}");
                InitializeRowCards(p2Rows[i], $"P2 Row {i + 1}");
            }

            mainLayout.Controls.Add(controlPanel, 0, 0);
            mainLayout.Controls.Add(boardLayout, 1, 0);

            RefreshAllMenus();
        }

        private void OpenVisualDeckPicker(FlowLayoutPanel targetRowPanel, string slotNamePrefix, Button sourceButton)
        {
            LoadDecksFromDisk();

            if (loadedDecks.Count == 0)
            {
                MessageBox.Show("No custom decks found! Open the Deck Editor to build your first card deck profile.", "Empty Storage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (DeckPickerWindow picker = new DeckPickerWindow(loadedDecks))
            {
                if (picker.ShowDialog(this) == DialogResult.OK && picker.SelectedDeck != null)
                {
                    UpdateSingleRowDisplay(targetRowPanel, picker.SelectedDeck, slotNamePrefix);
                    sourceButton.Text = picker.SelectedDeck.DeckName;
                    sourceButton.BackColor = Color.FromArgb(45, 80, 70);
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
                    BackColor = Color.DimGray,
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
                        fullPath = Path.IsPathRooted(deck.ImagePaths[i])
                            ? deck.ImagePaths[i]
                            : Path.Combine(baseDir, deck.ImagePaths[i]);
                    }

                    if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                    {
                        pic.Image = Image.FromFile(fullPath);
                        pic.CardLabel = "";
                    }
                    else
                    {
                        pic.Image = null;
                        pic.CardLabel = deck != null ? $"{deck.DeckName}\nSlot {i + 1}\nMissing" : $"{labelPrefix}\nEmpty";
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
                        pic.CardLabel = $"P1 Row {r + 1}\nSlot {p1Rows[r].Controls.IndexOf(pic) + 1}";
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
                        pic.CardLabel = $"P2 Row {r + 1}\nSlot {p2Rows[r].Controls.IndexOf(pic) + 1}";
                        pic.Invalidate();
                    }
                }
            }

            foreach (Control ctrl in controlPanel.Controls)
            {
                if (ctrl is Button btn && btn.Text != "Open Deck Editor")
                {
                    btn.BackColor = Color.FromArgb(50, 50, 55);

                    if (btn == p1Deck1Menu) btn.Text = "Select P1 Deck 1...";
                    else if (btn == p1Deck2Menu) btn.Text = "Select P1 Deck 2...";
                    else if (btn == p1Deck3Menu) btn.Text = "Select P1 Deck 3...";
                    else if (btn == p2Deck1Menu) btn.Text = "Select P2 Deck 1...";
                    else if (btn == p2Deck2Menu) btn.Text = "Select P2 Deck 2...";
                    else if (btn == p2Deck3Menu) btn.Text = "Select P2 Deck 3...";
                }
            }
        }

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
        public class DeckEditorPanel : Panel
        {
            private TextBox txtDeckName;
            private string[] chosenPaths = new string[6];
            private Button[] slotButtons = new Button[6];
            private Action onDeckSavedCallback;

            public DeckEditorPanel(Action onSaveCallback)
            {
                this.onDeckSavedCallback = onSaveCallback;
                this.BackColor = Color.FromArgb(58, 61, 64);
                InitializeEditorUI();
            }

            private void InitializeEditorUI()
            {
                Label lblName = new Label { Text = "Deck Name:", Location = new Point(20, 20), Width = 90, ForeColor = Color.White, Font = new Font("Arial", 9, FontStyle.Bold) };
                txtDeckName = new TextBox
                {
                    Location = new Point(120, 18),
                    Width = 250,
                    BackColor = Color.FromArgb(40, 40, 45),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                this.Controls.Add(lblName);
                this.Controls.Add(txtDeckName);

                Button btnBatchSelect = new Button
                {
                    Text = "⚡ Batch Select 6 Images At Once",
                    Location = new Point(120, 52),
                    Width = 300,
                    Height = 32,
                    BackColor = Color.FromArgb(40, 75, 95),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 9, FontStyle.Bold)
                };
                btnBatchSelect.FlatAppearance.BorderSize = 0;
                btnBatchSelect.Click += BatchSelectImages_Click;
                this.Controls.Add(btnBatchSelect);

                for (int i = 0; i < 6; i++)
                {
                    int slotIndex = i;
                    Label lblSlot = new Label { Text = $"Card Slot {slotIndex + 1}:", Location = new Point(20, 100 + (i * 35)), Width = 90, ForeColor = Color.White, Font = new Font("Arial", 9, FontStyle.Regular) };

                    Button btnBrowse = new Button
                    {
                        Text = "Empty Slot...",
                        Location = new Point(120, 95 + (i * 35)),
                        Width = 300,
                        Height = 26,
                        TextAlign = ContentAlignment.MiddleLeft,
                        BackColor = Color.FromArgb(50, 50, 55),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnBrowse.FlatAppearance.BorderSize = 1;
                    btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);

                    btnBrowse.Click += (s, e) => BrowseSingleCardImage(slotIndex, btnBrowse);

                    slotButtons[slotIndex] = btnBrowse;
                    this.Controls.Add(lblSlot);
                    this.Controls.Add(btnBrowse);
                }

                Button btnSave = new Button
                {
                    Text = "Save Custom Deck Profile",
                    Location = new Point(120, 325),
                    Width = 200,
                    Height = 35,
                    BackColor = Color.FromArgb(45, 110, 75),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 9, FontStyle.Bold)
                };
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Click += SaveDeckButton_Click;
                this.Controls.Add(btnSave);
            }

            private void BatchSelectImages_Click(object sender, EventArgs e)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Multiselect = true;
                    ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
                    ofd.Title = "Select Exactly 6 Image Files for your Deck";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string[] files = ofd.FileNames;
                        Array.Sort(files);

                        if (files.Length > 6)
                        {
                            MessageBox.Show("You selected more than 6 images. Only the first 6 will be used.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        int fillLimit = Math.Min(files.Length, 6);
                        for (int i = 0; i < fillLimit; i++)
                        {
                            chosenPaths[i] = files[i];
                            slotButtons[i].Text = Path.GetFileName(files[i]);
                            slotButtons[i].BackColor = Color.FromArgb(40, 75, 95);
                        }
                    }
                }
            }

            private void BrowseSingleCardImage(int index, Button targetButton)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
                    ofd.Title = $"Select Image File for Card Slot {index + 1}";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        chosenPaths[index] = ofd.FileName;
                        targetButton.Text = Path.GetFileName(ofd.FileName);
                        targetButton.BackColor = Color.FromArgb(40, 75, 95);
                    }
                }
            }

            private void SaveDeckButton_Click(object sender, EventArgs e)
            {
                string enteredName = txtDeckName.Text.Trim();
                if (string.IsNullOrEmpty(enteredName))
                {
                    MessageBox.Show("Please type in a Deck Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                for (int i = 0; i < 6; i++)
                {
                    if (string.IsNullOrEmpty(chosenPaths[i]))
                    {
                        chosenPaths[i] = "";
                        continue;
                    }

                    if (chosenPaths[i].StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                    {
                        chosenPaths[i] = chosenPaths[i].Substring(baseDir.Length);
                    }
                }

                DeckConfig newDeck = new DeckConfig { DeckName = enteredName, ImagePaths = new List<string>(chosenPaths) };
                string jsonPath = Path.Combine(baseDir, "decks.json");
                AppSettings currentSettings = new AppSettings { AvailableDecks = new List<DeckConfig>() };

                if (File.Exists(jsonPath))
                {
                    try
                    {
                        string oldJson = File.ReadAllText(jsonPath);
                        var loaded = JsonSerializer.Deserialize<AppSettings>(oldJson);
                        if (loaded?.AvailableDecks != null) currentSettings = loaded;
                    }
                    catch { }
                }

                currentSettings.AvailableDecks.RemoveAll(d => d.DeckName.Equals(enteredName, StringComparison.OrdinalIgnoreCase));
                currentSettings.AvailableDecks.Add(newDeck);

                string updatedJson = JsonSerializer.Serialize(currentSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, updatedJson);
                MessageBox.Show($"'{enteredName}' has been saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtDeckName.Clear();
                for (int i = 0; i < 6; i++)
                {
                    chosenPaths[i] = null;
                    slotButtons[i].Text = "Browse Image File...";
                    slotButtons[i].BackColor = Color.FromArgb(50, 50, 55);
                }

                onDeckSavedCallback?.Invoke();
            }
        }

        public class DeckConfig
        {
            public string DeckName { get; set; } = "";
            public List<string> ImagePaths { get; set; } = new List<string>();
            public override string ToString() => DeckName;
        }

        public class AppSettings
        {
            public List<DeckConfig> AvailableDecks { get; set; } = new List<DeckConfig>();
        }

        public class EditorWindow : Form
        {
            public EditorWindow(Action onSaveCallback)
            {
                this.Text = "Deck Configuration Editor";
                this.Size = new Size(460, 420);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                this.BackColor = Color.FromArgb(58, 61, 64);

                DeckEditorPanel editorPanel = new DeckEditorPanel(onSaveCallback) { Dock = DockStyle.Fill };
                this.Controls.Add(editorPanel);
            }
        }

        public class DeckPickerWindow : Form
        {
            public DeckConfig SelectedDeck { get; private set; }

            public DeckPickerWindow(List<DeckConfig> decks)
            {             
                this.Text = "Select Card Deck Layout Profile";
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
}