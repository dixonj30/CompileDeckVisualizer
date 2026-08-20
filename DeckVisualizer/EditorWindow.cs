using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DeckVisualizer.Form1;

namespace DeckVisualizer
{
    public class EditorWindow : Form
    {
        private static readonly string EditorTitle = "Deck Configuration Editor";

        public EditorWindow(Action onSaveCallback)
        {
            this.Text = EditorTitle;
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

    public class DeckEditorPanel : Panel
    {
        private static readonly string SaveSuccessTitle = "Success";
        private static readonly string SaveTitle = "Save Custom Deck Profile";
        private static readonly string EditorEmptyWarning = "Please type in a Deck Name first.";
        private static readonly string EditorEmptyTitle = "Error";
        private static readonly string EditorEmptySlot = "Empty Slot...";
        private static readonly string EditorBatchSelectButton = "⚡ Batch Select 6 Images At Once";
        private static readonly string BatchSelectTitle = "Select Exactly 6 Image Files for your Deck";
        private static readonly string BatchLimitNotice = "You selected more than 6 images. Only the first 6 will be used.";
        private static readonly string BatchLimitTitle = "Notice";
        private static readonly string SingleSelectTitle = "Select Image File for Card Slot";

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
                Text = EditorBatchSelectButton,
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
                    Text = EditorEmptySlot,
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
                Text = SaveTitle,
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
                ofd.Filter = ImageFileFilters;
                ofd.Title = BatchSelectTitle;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] files = ofd.FileNames;
                    Array.Sort(files);

                    if (files.Length > 6)
                    {
                        MessageBox.Show(BatchLimitNotice, BatchLimitTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                ofd.Filter = ImageFileFilters;
                ofd.Title = $"{SingleSelectTitle} {index + 1}";

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
                MessageBox.Show(EditorEmptyWarning, EditorEmptyTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            string jsonPath = Path.Combine(baseDir, DecksDatabaseFileName);
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
            MessageBox.Show($"'{enteredName}' has been saved!", SaveSuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
}
