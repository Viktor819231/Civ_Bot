using System.Text;
using NavigationAndLocations;


//Vibecoded frontend
namespace Gamebot
{
    public partial class BotFrontend : Form
    {
        private Label? statusLabel;
        private TextBox? debugTextBox;
        private CheckBox? debugModeCheckBox;
        
        // Stats labels
        private Label? statsConnectionsLabel;
        private Label? statsRestartsLabel;
        private Label? statsRelobbiesLabel;
        
        // Mouse coordinates tracking
        private Label? mouseCoordinatesLabel;
        private System.Windows.Forms.Timer? mouseTrackingTimer;
        
        // Image display controls
        private PictureBox? headerPictureBox;
        private PictureBox? menuPictureBox;
        private PictureBox? chatPictureBox;
        private PictureBox? creditScreenPictureBox;
        private System.Windows.Forms.Timer? imageUpdateTimer;
        
        // OCR text labels
        private Label? headerOcrLabel;
        private Label? menuOcrLabel;
        private Label? chatOcrLabel;
        private Label? creditScreenOcrLabel;

        public BotFrontend()
        {
            InitializeComponent();
            InitializeBotSystems();
        }

        private void InitializeBotSystems()
        {
            try
            {
                Console.WriteLine("Display frontend initialized.");
                if (statusLabel != null)
                    statusLabel.Text = "Status: Ready";
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                    statusLabel.Text = "Status: Error - " + ex.Message;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Civ Bot - Info Display";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Debug Mode Checkbox
            debugModeCheckBox = new CheckBox();
            debugModeCheckBox.Text = "Debug Mode";
            debugModeCheckBox.Size = new Size(150, 30);
            debugModeCheckBox.Location = new Point(20, 20);
            debugModeCheckBox.Font = new Font("Arial", 12);
            debugModeCheckBox.CheckedChanged += DebugModeCheckBox_CheckedChanged;

            // Status Label
            statusLabel = new Label();
            statusLabel.Text = "Status: Monitoring";
            statusLabel.Size = new Size(300, 20);
            statusLabel.Location = new Point(20, 60);
            statusLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Debug TextBox
            debugTextBox = new TextBox();
            debugTextBox.Multiline = true;
            debugTextBox.ScrollBars = ScrollBars.Vertical;
            debugTextBox.Size = new Size(450, 300);
            debugTextBox.Location = new Point(20, 90);
            debugTextBox.ReadOnly = true;
            debugTextBox.Font = new Font("Consolas", 10);
            debugTextBox.Visible = true; // Always visible

            // Stats Labels
            var statsHeaderLabel = new Label();
            statsHeaderLabel.Text = "Bot Statistics";
            statsHeaderLabel.Size = new Size(200, 25);
            statsHeaderLabel.Location = new Point(20, 400);
            statsHeaderLabel.Font = new Font("Arial", 14, FontStyle.Bold);

            statsConnectionsLabel = new Label();
            statsConnectionsLabel.Text = "Connections: 0";
            statsConnectionsLabel.Size = new Size(200, 20);
            statsConnectionsLabel.Location = new Point(20, 430);
            statsConnectionsLabel.Font = new Font("Arial", 11);

            statsRestartsLabel = new Label();
            statsRestartsLabel.Text = "Restarts: 0";
            statsRestartsLabel.Size = new Size(200, 20);
            statsRestartsLabel.Location = new Point(20, 455);
            statsRestartsLabel.Font = new Font("Arial", 11);

            statsRelobbiesLabel = new Label();
            statsRelobbiesLabel.Text = "Relobbies: 0";
            statsRelobbiesLabel.Size = new Size(200, 20);
            statsRelobbiesLabel.Location = new Point(20, 480);
            statsRelobbiesLabel.Font = new Font("Arial", 11);

            // Mouse Coordinates Display
            mouseCoordinatesLabel = new Label();
            mouseCoordinatesLabel.Text = "Mouse: Screen (0, 0) | Window (0, 0)";
            mouseCoordinatesLabel.Size = new Size(450, 30);
            mouseCoordinatesLabel.Location = new Point(20, 515);
            mouseCoordinatesLabel.Font = new Font("Consolas", 11, FontStyle.Bold);
            mouseCoordinatesLabel.ForeColor = Color.DarkBlue;
            mouseCoordinatesLabel.BackColor = Color.LightCyan;
            mouseCoordinatesLabel.BorderStyle = BorderStyle.FixedSingle;
            mouseCoordinatesLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Mouse tracking timer - updates 20 times per second
            mouseTrackingTimer = new System.Windows.Forms.Timer();
            mouseTrackingTimer.Interval = 50;
            mouseTrackingTimer.Tick += UpdateMouseCoordinates;
            mouseTrackingTimer.Start();

            // === IMAGE BOXES SECTION ===
            // Header Image Box
            var headerLabel = new Label();
            headerLabel.Text = "Header";
            headerLabel.Size = new Size(400, 20);
            headerLabel.Location = new Point(500, 20);
            headerLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            headerLabel.TextAlign = ContentAlignment.MiddleCenter;

            var headerPictureBox = new PictureBox();
            headerPictureBox.Size = new Size(400, 60);
            headerPictureBox.Location = new Point(500, 40);
            headerPictureBox.BorderStyle = BorderStyle.FixedSingle;
            headerPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.headerPictureBox = headerPictureBox;
            LoadImageSafely(headerPictureBox, "Header.png");

            var headerOcrLabel = new Label();
            headerOcrLabel.Text = "OCR: (waiting...)";
            headerOcrLabel.Size = new Size(400, 40);
            headerOcrLabel.Location = new Point(500, 105);
            headerOcrLabel.BackColor = Color.LightYellow;
            headerOcrLabel.BorderStyle = BorderStyle.FixedSingle;
            headerOcrLabel.Font = new Font("Arial", 14);
            headerOcrLabel.TextAlign = ContentAlignment.TopLeft;
            this.headerOcrLabel = headerOcrLabel;

            // Menu Image Box
            var menuLabel = new Label();
            menuLabel.Text = "Menu";
            menuLabel.Size = new Size(400, 20);
            menuLabel.Location = new Point(500, 160);
            menuLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            menuLabel.TextAlign = ContentAlignment.MiddleCenter;

            var menuPictureBox = new PictureBox();
            menuPictureBox.Size = new Size(400, 60);
            menuPictureBox.Location = new Point(500, 180);
            menuPictureBox.BorderStyle = BorderStyle.FixedSingle;
            menuPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.menuPictureBox = menuPictureBox;
            LoadImageSafely(menuPictureBox, "ManuSS.png");

            var menuOcrLabel = new Label();
            menuOcrLabel.Text = "OCR: (waiting...)";
            menuOcrLabel.Size = new Size(400, 40);
            menuOcrLabel.Location = new Point(500, 245);
            menuOcrLabel.BackColor = Color.LightYellow;
            menuOcrLabel.BorderStyle = BorderStyle.FixedSingle;
            menuOcrLabel.Font = new Font("Arial", 14);
            menuOcrLabel.TextAlign = ContentAlignment.TopLeft;
            this.menuOcrLabel = menuOcrLabel;

            // Chat Image Box
            var chatLabel = new Label();
            chatLabel.Text = "Chat";
            chatLabel.Size = new Size(400, 20);
            chatLabel.Location = new Point(500, 300);
            chatLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            chatLabel.TextAlign = ContentAlignment.MiddleCenter;

            var chatPictureBox = new PictureBox();
            chatPictureBox.Size = new Size(400, 60);
            chatPictureBox.Location = new Point(500, 320);
            chatPictureBox.BorderStyle = BorderStyle.FixedSingle;
            chatPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.chatPictureBox = chatPictureBox;
            LoadImageSafely(chatPictureBox, "ChatSS.png");

            var chatOcrLabel = new Label();
            chatOcrLabel.Text = "OCR: (waiting...)";
            chatOcrLabel.Size = new Size(400, 40);
            chatOcrLabel.Location = new Point(500, 385);
            chatOcrLabel.BackColor = Color.LightYellow;
            chatOcrLabel.BorderStyle = BorderStyle.FixedSingle;
            chatOcrLabel.Font = new Font("Arial", 14);
            chatOcrLabel.TextAlign = ContentAlignment.TopLeft;
            this.chatOcrLabel = chatOcrLabel;

            // Credit Screen Image Box
            var creditScreenLabel = new Label();
            creditScreenLabel.Text = "Credit Screen";
            creditScreenLabel.Size = new Size(400, 20);
            creditScreenLabel.Location = new Point(500, 440);
            creditScreenLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            creditScreenLabel.TextAlign = ContentAlignment.MiddleCenter;

            var creditScreenPictureBox = new PictureBox();
            creditScreenPictureBox.Size = new Size(400, 60);
            creditScreenPictureBox.Location = new Point(500, 460);
            creditScreenPictureBox.BorderStyle = BorderStyle.FixedSingle;
            creditScreenPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.creditScreenPictureBox = creditScreenPictureBox;
            LoadImageSafely(creditScreenPictureBox, "CreditScreenSS.png");

            var creditScreenOcrLabel = new Label();
            creditScreenOcrLabel.Text = "OCR: (waiting...)";
            creditScreenOcrLabel.Size = new Size(400, 40);
            creditScreenOcrLabel.Location = new Point(500, 525);
            creditScreenOcrLabel.BackColor = Color.LightYellow;
            creditScreenOcrLabel.BorderStyle = BorderStyle.FixedSingle;
            creditScreenOcrLabel.Font = new Font("Arial", 14);
            creditScreenOcrLabel.TextAlign = ContentAlignment.TopLeft;
            this.creditScreenOcrLabel = creditScreenOcrLabel;

            // Setup image update timer
            imageUpdateTimer = new System.Windows.Forms.Timer();
            imageUpdateTimer.Interval = 2000; // Update every 2 seconds
            imageUpdateTimer.Tick += UpdateImages;
            imageUpdateTimer.Start();

            // Add controls to form
            this.Controls.Add(debugModeCheckBox);
            this.Controls.Add(statusLabel);
            this.Controls.Add(debugTextBox);
            this.Controls.Add(statsHeaderLabel);
            this.Controls.Add(statsConnectionsLabel);
            this.Controls.Add(statsRestartsLabel);
            this.Controls.Add(statsRelobbiesLabel);
            this.Controls.Add(mouseCoordinatesLabel);
            
            // Add image controls
            this.Controls.Add(headerLabel);
            this.Controls.Add(headerPictureBox);
            this.Controls.Add(headerOcrLabel);
            this.Controls.Add(menuLabel);
            this.Controls.Add(menuPictureBox);
            this.Controls.Add(menuOcrLabel);
            this.Controls.Add(chatLabel);
            this.Controls.Add(chatPictureBox);
            this.Controls.Add(chatOcrLabel);
            this.Controls.Add(creditScreenLabel);
            this.Controls.Add(creditScreenPictureBox);
            this.Controls.Add(creditScreenOcrLabel);

            // Redirect console output to debug textbox
            Console.SetOut(new TextBoxWriter(debugTextBox));
        }

        private void LoadImageSafely(PictureBox pictureBox, string imageName)
        {
            try
            {
                string imagePath = Path.Combine(AppContext.BaseDirectory, imageName);
                if (File.Exists(imagePath))
                {
                    pictureBox.Image = Image.FromFile(imagePath);
                }
                else
                {
                    // Create a placeholder if image not found
                    var placeholder = new Bitmap(pictureBox.Width, pictureBox.Height);
                    using (Graphics g = Graphics.FromImage(placeholder))
                    {
                        g.Clear(Color.LightGray);
                        g.DrawString($"Missing:\n{imageName}", new Font("Arial", 8), Brushes.Black, 5, 5);
                    }
                    pictureBox.Image = placeholder;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load image {imageName}: {ex.Message}");
                var errorImage = new Bitmap(pictureBox.Width, pictureBox.Height);
                using (Graphics g = Graphics.FromImage(errorImage))
                {
                    g.Clear(Color.Red);
                    g.DrawString("Error", new Font("Arial", 10), Brushes.White, 10, 10);
                }
                pictureBox.Image = errorImage;
            }
        }

        private void UpdateImages(object? sender, EventArgs e)
        {
            try
            {
                // Update stats
                if (statsConnectionsLabel != null)
                    statsConnectionsLabel.Text = $"Connections: {BotStats.TotalConnections}";
                if (statsRestartsLabel != null)
                    statsRestartsLabel.Text = $"Restarts: {BotStats.TotalRestarts}";
                if (statsRelobbiesLabel != null)
                    statsRelobbiesLabel.Text = $"Relobbies: {BotStats.TotalRelobbies}";

                // Only update if images exist (meaning OCR has run)
                if (headerPictureBox != null && headerOcrLabel != null)
                {
                    RefreshPictureBox(headerPictureBox, "Header.png");
                    UpdateOcrText(headerOcrLabel, CivTextBox.HeaderText);
                }
                if (menuPictureBox != null && menuOcrLabel != null)
                {
                    RefreshPictureBox(menuPictureBox, "ManuSS.png");
                    UpdateOcrText(menuOcrLabel, CivTextBox.MenuText);
                }
                if (chatPictureBox != null && chatOcrLabel != null)
                {
                    RefreshPictureBox(chatPictureBox, "ChatSS.png");
                    UpdateOcrText(chatOcrLabel, CivTextBox.ChatText);
                }
                if (creditScreenPictureBox != null && creditScreenOcrLabel != null)
                {
                    RefreshPictureBox(creditScreenPictureBox, "CreditScreenSS.png");
                    UpdateOcrText(creditScreenOcrLabel, CivTextBox.CreditScreen);
                }
            }
            catch (Exception)
            {
                // Don't spam console with update errors
                // Console.WriteLine($"Image update error");
            }
        }

        private void UpdateOcrText(Label ocrLabel, CivTextBox textBox)
        {
            try
            {
                string imagePath = Path.Combine(AppContext.BaseDirectory, textBox.filename);
                if (File.Exists(imagePath))
                {
                    string ocrText = OCR.ImgToText.TextReader(imagePath);
                    string cleanText = ocrText.Trim().Replace("\n", " ").Replace("\r", "");
                    if (cleanText.Length > 100) cleanText = cleanText.Substring(0, 100) + "...";
                    ocrLabel.Text = $"OCR: {cleanText}";
                }
            }
            catch (Exception ex)
            {
                ocrLabel.Text = $"OCR Error: {ex.Message}";
            }
        }

        private void RefreshPictureBox(PictureBox pictureBox, string imageName)
        {
            string imagePath = Path.Combine(AppContext.BaseDirectory, imageName);
            if (File.Exists(imagePath))
            {
                try
                {
                    // Dispose old image to free memory
                    var oldImage = pictureBox.Image;
                    
                    // Load new image
                    using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pictureBox.Image = Image.FromStream(fs);
                    }
                    
                    // Clean up old image
                    if (oldImage != null && oldImage != pictureBox.Image)
                    {
                        oldImage.Dispose();
                    }
                }
                catch
                {
                    // If loading fails, keep the old image
                }
            }
        }



        private void UpdateMouseCoordinates(object? sender, EventArgs e)
        {
            if (mouseCoordinatesLabel != null)
            {
                try
                {
                    var screenPos = MouseControl.GetMousePosition();
                    
                    // Get window coordinates if Civ window handle exists
                    if (Program.CivWindowHandle != IntPtr.Zero)
                    {
                        var windowPos = MouseControl.GetMousePositionInWindow(Program.CivWindowHandle);
                        mouseCoordinatesLabel.Text = $"Mouse: Screen ({screenPos.X}, {screenPos.Y}) | Window ({windowPos.X}, {windowPos.Y})";
                    }
                    else
                    {
                        mouseCoordinatesLabel.Text = $"Mouse: Screen ({screenPos.X}, {screenPos.Y}) | Window: N/A";
                    }
                }
                catch
                {
                    mouseCoordinatesLabel.Text = "Mouse: Error reading coordinates";
                }
            }
        }

        private void DebugModeCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (debugTextBox != null && debugModeCheckBox != null)
            {
                debugTextBox.Visible = debugModeCheckBox.Checked;
                if (statusLabel != null)
                {
                    statusLabel.Text = debugModeCheckBox.Checked ? 
                        "Status: Monitoring (Debug ON)" : 
                        "Status: Monitoring";
                }
            }
        }

       

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up resources
            imageUpdateTimer?.Stop();
            imageUpdateTimer?.Dispose();
            
            mouseTrackingTimer?.Stop();
            mouseTrackingTimer?.Dispose();
            
            // Write final stats and close logger
            Logger.Close();
            
            base.OnFormClosing(e);
        }
    }

    // Custom TextWriter to redirect Console output to TextBox
    public class TextBoxWriter : TextWriter
    {
        private TextBox? textBox;

        public TextBoxWriter(TextBox? textBox)
        {
            this.textBox = textBox;
        }

        public override void Write(char value)
        {
            if (textBox != null && textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() =>
                {
                    textBox.AppendText(value.ToString());
                    textBox.ScrollToCaret();
                }));
            }
            else if (textBox != null)
            {
                textBox.AppendText(value.ToString());
                textBox.ScrollToCaret();
            }
            // Also log to file
            Logger.LogToFile(value.ToString());
        }

        public override void Write(string? value)
        {
            if (value != null && textBox != null && textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() =>
                {
                    textBox.AppendText(value);
                    textBox.ScrollToCaret();
                }));
            }
            else if (value != null && textBox != null)
            {
                textBox.AppendText(value);
                textBox.ScrollToCaret();
            }
            // Also log to file
            if (value != null)
                Logger.LogToFile(value);
        }

        public override void WriteLine(string? value)
        {
            if (value != null && textBox != null && textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() =>
                {
                    textBox.AppendText(value + Environment.NewLine);
                    textBox.ScrollToCaret();
                }));
            }
            else if (value != null && textBox != null)
            {
                textBox.AppendText(value + Environment.NewLine);
                textBox.ScrollToCaret();
            }
            // Also log to file
            Logger.LogLineToFile(value);
        }

        public override Encoding Encoding => Encoding.UTF8;
    }

    // Frontend launcher - called from Program.cs
    public static class BotFrontendLauncher
    {
        public static void StartFrontend()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BotFrontend());
        }
    }
}
