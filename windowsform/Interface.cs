using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using Gamebot;

namespace winform
{
    public partial class MainForm : Form
    {
        private TextBox statusTextBox;
        private Panel mainPanel;          // Main bot control panel
        private Panel scriptPanel;        // Script selection panel
        private Panel localizationPanel;  // Localization panel
        
        // Import CreateRoundRectRgn from GDI32.dll
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        // Import ReleaseCapture and SendMessage for form dragging
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form setup - grey/purple theme with wider dimensions
            this.MinimumSize = new Size(950, 650);       // Increased from 800x600
            this.ClientSize = new Size(950, 650);        // Increased from 800x600
            this.Text = "Bot Control Panel";
            this.BackColor = Color.FromArgb(245, 243, 248); // Lighter grey with subtle purple tint
            this.Font = new Font("Segoe UI", 10F);
            
            // Custom form appearance - more rounded look
            this.FormBorderStyle = FormBorderStyle.None; // Remove default border for custom styling
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Apply rounded corners using the custom form extension method
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            
            // Add custom title bar with close button
            Panel titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(100, 90, 130)  // Darker purple for title bar
            };
            
            // First add the close button to the title bar
            Button closeButton = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(100, 90, 130),
                FlatAppearance = { BorderSize = 0 }
            };
            closeButton.Click += (s, e) => Application.Exit();
            titleBar.Controls.Add(closeButton);

            // Then create a centered title label
            Label titleLabel = new Label
            {
                Text = "Bot Control Panel",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 12F),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter  // This centers the text
            };
            titleBar.Controls.Add(titleLabel);

            this.Controls.Add(titleBar);
            
            // Create panels for different "pages" with proper padding
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = true,  // Start with main panel visible
                Padding = new Padding(0, 40, 0, 0) // Account for title bar
            };
            
            scriptPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false, // Start with script panel hidden
                Padding = new Padding(0, 40, 0, 0) // Account for title bar
            };
            
            localizationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                Padding = new Padding(0, 40, 0, 0)
            };
            
            // Add panels to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(scriptPanel);
            this.Controls.Add(localizationPanel);
            
            // Set up the MAIN PANEL contents
            SetupMainPanel();
            
            // Set up the SCRIPT PANEL contents
            SetupScriptPanel();
            
            // Set up the LOCALIZATION PANEL contents
            SetupLocalizationPanel();
            
            // Enable form dragging
            titleBar.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); } };
        }

        private void SetupMainPanel()
        {
            // Status text box (initially hidden)
            statusTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Visible = false,
                Location = new Point(50, 50),
                Size = new Size(300, 80),
                BackColor = Color.FromArgb(248, 248, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F)
            };
            mainPanel.Controls.Add(statusTextBox);
            
            // Big square button on left
            var startButton = new RoundedButton
            {
                Text = "Start Bot",
                Location = new Point(50, 150),
                Size = new Size(300, 300),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                BackColor = Color.FromArgb(90, 80, 120), // Dark purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 15
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.Click += StartButton_Click;
            mainPanel.Controls.Add(startButton);
            
            // Choose script button below the main button
            var chooseScriptButton = new RoundedButton
            {
                Text = "Choose Script",
                Location = new Point(50, 470),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(130, 120, 160), // Medium purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10
            };
            chooseScriptButton.FlatAppearance.BorderSize = 0;
            chooseScriptButton.Click += ChooseScriptButton_Click; // Add this handler
            mainPanel.Controls.Add(chooseScriptButton);

            // Right side - 3 buttons with same width as square button
            int rightButtonX = 450;
            int rightButtonWidth = 300;
            int rightButtonHeight = 80;
            int spacing = 30;
            int startY = 150;
            
            // Function to create consistent right buttons
            RoundedButton CreateRightButton(string text, int yPos)
            {
                var btn = new RoundedButton
                {
                    Text = text,
                    Location = new Point(rightButtonX, yPos),
                    Size = new Size(rightButtonWidth, rightButtonHeight),
                    BackColor = Color.FromArgb(110, 100, 140), // Medium-dark purple
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11F),
                    FlatStyle = FlatStyle.Flat,
                    BorderRadius = 10
                };
                btn.FlatAppearance.BorderSize = 0;
                return btn;
            }
            
            // First right button
            var rightButton1 = CreateRightButton("Create New Localization Picture", startY);
            rightButton1.Click += LocalizationButton_Click;
            mainPanel.Controls.Add(rightButton1);
            
            // Second right button
            var rightButton2 = CreateRightButton("Option 2", startY + rightButtonHeight + spacing);
            mainPanel.Controls.Add(rightButton2);
            
            // Third right button
            var rightButton3 = CreateRightButton("Option 3", startY + 2 * (rightButtonHeight + spacing));
            mainPanel.Controls.Add(rightButton3);
        }

        private void SetupScriptPanel()
        {
            // Header/title
            var titleLabel = new Label
            {
                Text = "Select Script",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 80, 120),
                Location = new Point(50, 30),
                AutoSize = true
            };
            scriptPanel.Controls.Add(titleLabel);
            
            // Script dropdown (ComboBox)
            var scriptComboBox = new ComboBox
            {
                Location = new Point(50, 80),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F)
            };
            
            // Add default script
            scriptComboBox.Items.Add("Default Bot Script");
            scriptComboBox.SelectedIndex = 0;
            scriptComboBox.SelectedIndexChanged += ScriptComboBox_SelectedIndexChanged;
            scriptPanel.Controls.Add(scriptComboBox);
            
            // Script actions display area
            var scriptActionsTextBox = new TextBox
            {
                Location = new Point(50, 120),
                Size = new Size(350, 350),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 248, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                ScrollBars = ScrollBars.Vertical,  // Add this line for scrolling
                Text = "1. Navigate: Setup Multiplayer\r\n2. CreatelobbyWithconfig\r\n3. NavigateTo(STAGING ROOM)\r\n4. ADVERTISE(ad1)\r\n5. ConfirmLocation(STAGING ROOM)\r\n6. ScanForChatMsg\r\n7. ReactToChatMsg(protocal)\r\n8. SLEEP(5000)\r\n9. ADVERTISE(ad2)\r\n7. Loop(4-6)"
            };
            scriptPanel.Controls.Add(scriptActionsTextBox);
            
            // Grid of 6 buttons on the right (2x3 grid)
            int buttonWidth = 140;
            int buttonHeight = 80;  // Made slightly smaller height
            int buttonSpacing = 20;
            int startX = 450;
            int startY = 120;
            
            // Create a 2x3 grid of action buttons (2 columns, 3 rows)
            for (int row = 0; row < 3; row++)  // Changed to 3 rows
            {
                for (int col = 0; col < 2; col++)  // Changed to 2 columns
                {
                    int index = row * 2 + col + 1;  // Updated index calculation
                    var actionButton = new RoundedButton
                    {
                        Text = $"Action {index}",
                        Location = new Point(
                            startX + col * (buttonWidth + buttonSpacing),
                            startY + row * (buttonHeight + buttonSpacing)
                        ),
                        Size = new Size(buttonWidth, buttonHeight),
                        BackColor = Color.FromArgb(110, 100, 140),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10F),
                        FlatStyle = FlatStyle.Flat,
                        BorderRadius = 10
                    };
                    actionButton.FlatAppearance.BorderSize = 0;
                    actionButton.Tag = index; // Store button index for later reference
                    actionButton.Click += ActionButton_Click;
                    scriptPanel.Controls.Add(actionButton);
                }
            }
            
            // Back button to return to main panel
            var backButton = new RoundedButton
            {
                Text = "Back",
                Location = new Point(50, 520),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(130, 120, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += BackButton_Click;
            scriptPanel.Controls.Add(backButton);
            
            // Create New Script button - aligned with first column of grid
            var createNewButton = new RoundedButton
            {
                Text = "Create New",
                Location = new Point(startX, 520), // Use same startX as grid
                Size = new Size(buttonWidth, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(90, 80, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10
            };
            createNewButton.FlatAppearance.BorderSize = 0;
            createNewButton.Click += CreateNewScript_Click;
            scriptPanel.Controls.Add(createNewButton);

            // Delete Script button - aligned with second column of grid
            var deleteScriptButton = new RoundedButton
            {
                Text = "Delete Script",
                Location = new Point(startX + buttonWidth + buttonSpacing, 520), // Second column
                Size = new Size(buttonWidth, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(140, 90, 110), // Reddish purple
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10,
                Tag = "deleteButton" // Tag for easy lookup
            };
            deleteScriptButton.FlatAppearance.BorderSize = 0;
            deleteScriptButton.Click += DeleteScript_Click;
            // Initially disable if default script is selected
            deleteScriptButton.Enabled = scriptComboBox.SelectedItem.ToString() != "Default Bot Script";
            scriptPanel.Controls.Add(deleteScriptButton);
        }

        private void SetupLocalizationPanel()
        {
            // Header/title - CENTERED
            var titleLabel = new Label
            {
                Text = "Create New Localization Picture",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 80, 120),
                AutoSize = true,
                // Center horizontally
                Location = new Point((this.ClientSize.Width - 350) / 2, 30)
            };
            localizationPanel.Controls.Add(titleLabel);
            
            // Image display box in the middle - MADE WIDER
            var pictureBox = new PictureBox
            {
                Location = new Point(250, 80),
                Size = new Size(600, 300),  // Increased width from 450 to 600
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(235, 235, 240),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            localizationPanel.Controls.Add(pictureBox);
            
            // Text recognition result box (initially invisible)
            var imgToTextLabel = new Label
            {
                Text = "Image to Text Result:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(250, 390),
                AutoSize = true,
                Visible = false,
                Tag = "recognitionControl"
            };
            localizationPanel.Controls.Add(imgToTextLabel);
            
            var recognizedTextBox = new TextBox
            {
                Location = new Point(250, 420),
                Size = new Size(600, 80),  // Match the width of pictureBox
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 248, 250),
                Visible = false,
                Tag = "recognitionControl"
            };
            localizationPanel.Controls.Add(recognizedTextBox);
            
            // Coordinate input boxes - IMPROVED SPACING
            string[] coordLabels = { "Left X:", "Right X:", "Top Y:", "Bottom Y:" };
            var coordTextBoxes = new TextBox[4];
            
            for (int i = 0; i < 4; i++)
            {
                var label = new Label
                {
                    Text = coordLabels[i],
                    Location = new Point(50, 100 + i * 60),
                    Size = new Size(65, 25),  // Fixed width for consistent layout
                    Font = new Font("Segoe UI", 10F),
                    TextAlign = ContentAlignment.MiddleRight  // Right-align text
                };
                localizationPanel.Controls.Add(label);
                
                coordTextBoxes[i] = new TextBox
                {
                    Location = new Point(125, 100 + i * 60),  // Aligned Y with label, increased X position
                    Size = new Size(80, 25),
                    Font = new Font("Segoe UI", 10F),
                    BorderStyle = BorderStyle.FixedSingle
                };
                localizationPanel.Controls.Add(coordTextBoxes[i]);
            }
            
            // Try recognize text button
            var recognizeButton = new RoundedButton
            {
                Text = "Try Recognize Text in Box",
                Location = new Point(50, 350),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(110, 100, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10
            };
            recognizeButton.FlatAppearance.BorderSize = 0;
            recognizeButton.Click += RecognizeText_Click;
            localizationPanel.Controls.Add(recognizeButton);
            
            // Save localization object button - HIDDEN INITIALLY
            var saveButton = new RoundedButton
            {
                Text = "Save Localization Object",
                Location = new Point(450, 520),
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(90, 80, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10,
                Visible = false,                   // Initially hidden
                Tag = "recognitionControl"         // Tag to show with other recognition controls
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveLocalization_Click;
            localizationPanel.Controls.Add(saveButton);
            
            // Back button
            var backButton = new RoundedButton
            {
                Text = "Back",
                Location = new Point(50, 520),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(130, 120, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                BorderRadius = 10
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += LocalizationBackButton_Click;
            localizationPanel.Controls.Add(backButton);
        }

        // Event handlers
        private void StartButton_Click(object sender, EventArgs e)
        {
            // Show the status box when start button is clicked
            statusTextBox.Visible = true;
            statusTextBox.Text = "Bot starting...";
            
            // Run your bot logic on a background thread
            Task.Run(() => Program.SetupNewLobby("Test"));
        }
        
        private void ChooseScriptButton_Click(object sender, EventArgs e)
        {
            // Switch to script selection panel
            mainPanel.Visible = false;
            scriptPanel.Visible = true;
        }
        
        private void BackButton_Click(object sender, EventArgs e)
        {
            // Switch back to main panel
            scriptPanel.Visible = false;
            mainPanel.Visible = true;
        }
        
        private void ScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // In the future, update the script actions display based on selection
            ComboBox comboBox = (ComboBox)sender;
            string selectedScript = comboBox.SelectedItem.ToString();
            
            // Find the script actions textbox and update it
            var textBox = scriptPanel.Controls.OfType<TextBox>().FirstOrDefault();
            if (textBox != null)
            {
                textBox.Text = $"Selected: {selectedScript}\r\n1. Start game\r\n2. Create lobby\r\n3. Set name to FFACIV.com\r\n4. Wait for players\r\n5. Start game when ready";
            }
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            // Placeholder for action button functionality
            RoundedButton button = (RoundedButton)sender;
            int actionIndex = (int)button.Tag;
            
            MessageBox.Show($"Action {actionIndex} clicked - functionality will be implemented later");
        }

        private void CreateNewScript_Click(object sender, EventArgs e)
        {
            // Show an input dialog for the new script name
            using (var inputForm = new Form())
            {
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.Text = "Create New Script";
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                
                var label = new Label() { Left = 20, Top = 20, Text = "Script Name:" };
                var textBox = new TextBox() { Left = 20, Top = 50, Width = 350 };
                
                var okButton = new Button() 
                { 
                    Text = "Create", 
                    Left = 240, 
                    Top = 80, 
                    Width = 60,
                    DialogResult = DialogResult.OK
                };
                
                var cancelButton = new Button() 
                { 
                    Text = "Cancel", 
                    Left = 310, 
                    Top = 80, 
                    Width = 60,
                    DialogResult = DialogResult.Cancel
                };
                
                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(okButton);
                inputForm.Controls.Add(cancelButton);
                
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;
                
                DialogResult result = inputForm.ShowDialog();
                
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    string newScriptName = textBox.Text;
                    
                    // Add the new script to the dropdown
                    ComboBox scriptComboBox = scriptPanel.Controls.OfType<ComboBox>().FirstOrDefault();
                    if (scriptComboBox != null)
                    {
                        scriptComboBox.Items.Add(newScriptName);
                        scriptComboBox.SelectedItem = newScriptName;
                    }
                }
            }
        }

        // Update the select script handler to work with ComboBox instead of ListBox
        private void SelectScript_Click(object sender, EventArgs e)
        {
            // Get the selected script and switch back
            var comboBox = scriptPanel.Controls.OfType<ComboBox>().FirstOrDefault();
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string selectedScript = comboBox.SelectedItem.ToString();
                statusTextBox.Text = $"Selected script: {selectedScript}";
                statusTextBox.Visible = true;
            }
            
            // Switch back to main panel
            scriptPanel.Visible = false;
            mainPanel.Visible = true;
        }

        // Add this new event handler
        private void DeleteScript_Click(object sender, EventArgs e)
        {
            var comboBox = scriptPanel.Controls.OfType<ComboBox>().FirstOrDefault();
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string scriptToDelete = comboBox.SelectedItem.ToString();
                
                // Confirm deletion
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete the script '{scriptToDelete}'?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.Yes)
                {
                    int selectedIndex = comboBox.SelectedIndex;
                    comboBox.Items.RemoveAt(selectedIndex);
                    
                    // Select another item
                    if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = Math.Max(0, selectedIndex - 1);
                    }
                }
            }
        }

        private void LocalizationButton_Click(object sender, EventArgs e)
        {
            mainPanel.Visible = false;
            localizationPanel.Visible = true;
        }

        private void LocalizationBackButton_Click(object sender, EventArgs e)
        {
            localizationPanel.Visible = false;
            mainPanel.Visible = true;
        }

        private void RecognizeText_Click(object sender, EventArgs e)
        {
            // Get coordinates from textboxes
            bool valid = true;
            int[] coords = new int[4];
            
            for (int i = 0; i < 4; i++)
            {
                var textBox = localizationPanel.Controls.OfType<TextBox>()
                    .Where(tb => tb.Location.X == 120 && tb.Location.Y == 97 + i * 60)
                    .FirstOrDefault();
                    
                if (textBox != null && int.TryParse(textBox.Text, out coords[i]))
                {
                    // Valid coordinate
                }
                else
                {
                    valid = false;
                    break;
                }
            }
            
            if (valid)
            {
                // Show the recognition controls
                foreach (Control c in localizationPanel.Controls)
                {
                    if (c.Tag != null && c.Tag.ToString() == "recognitionControl")
                    {
                        c.Visible = true;
                    }
                }
                
                // For demo purposes, just show a sample text
                var textBox = localizationPanel.Controls.OfType<TextBox>()
                    .Where(tb => tb.Multiline == true && tb.ReadOnly == true)
                    .FirstOrDefault();
                    
                if (textBox != null)
                {
                    textBox.Text = "Sample recognized text from the selected region.";
                }
                
                // In a real implementation, you would:
                // 1. Get the image from the PictureBox
                // 2. Crop it using the coordinates
                // 3. Run OCR on the cropped region
                // 4. Display the result
            }
            else
            {
                MessageBox.Show("Please enter valid coordinates for all four bounds.", 
                                "Invalid Coordinates", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        private void SaveLocalization_Click(object sender, EventArgs e)
        {
            // Demo implementation
            MessageBox.Show("Localization object saved successfully!", 
                           "Success", 
                           MessageBoxButtons.OK, 
                           MessageBoxIcon.Information);
        }
    }

    // Custom rounded button implementation
    public class RoundedButton : Button
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int BorderRadius { get; set; } = 10;
        
        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.FromArgb(110, 100, 140);
            this.ForeColor = Color.White;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, BorderRadius, BorderRadius, 180, 90);
            path.AddArc(Width - BorderRadius, 0, BorderRadius, BorderRadius, 270, 90);
            path.AddArc(Width - BorderRadius, Height - BorderRadius, BorderRadius, BorderRadius, 0, 90);
            path.AddArc(0, Height - BorderRadius, BorderRadius, BorderRadius, 90, 90);
            path.CloseAllFigures();
            
            this.Region = new Region(path);
            
            base.OnPaint(e);
        }
    }
}