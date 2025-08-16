using System.Text;
using NavigationAndLocations;


//Vibecoded frontend
namespace Gamebot
{
    public partial class BotFrontend : Form
    {
    private Button? startButton;
    private Button? pauseButton;
        private Label? statusLabel;
        private TextBox? debugTextBox;
        private CancellationTokenSource? cancellationTokenSource;
    private bool botRunning = false;
    private bool botPaused = false;
        
        // Image display controls
        private PictureBox? headerPictureBox;
        private PictureBox? menuPictureBox;
        private PictureBox? chatPictureBox;
        private System.Windows.Forms.Timer? imageUpdateTimer;
        
        // OCR text labels
        private Label? headerOcrLabel;
        private Label? menuOcrLabel;
        private Label? chatOcrLabel;

        public BotFrontend()
        {
            InitializeComponent();
            InitializeBotSystems();
        }

        private void InitializeBotSystems()
        {
            try
            {
                Console.WriteLine("Bot systems ready.");
                if (statusLabel != null)
                    statusLabel.Text = "Bot Status: Ready";
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                    statusLabel.Text = "Bot Status: Error - " + ex.Message;
                if (startButton != null)
                    startButton.Enabled = false;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Civ Bot Controller";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Start Button
            startButton = new Button();
            startButton.Text = "Start Bot";
            startButton.Size = new Size(100, 30);
            startButton.Location = new Point(20, 20);
            startButton.BackColor = Color.LightGreen;
            startButton.Font = new Font("Arial", 12);
            startButton.Click += StartButton_Click;

            // Pause/Resume Button
            pauseButton = new Button();
            pauseButton.Text = "Pause Bot";
            pauseButton.Size = new Size(100, 30);
            pauseButton.Location = new Point(140, 20);
            pauseButton.BackColor = Color.Gold;
            pauseButton.Font = new Font("Arial", 12);
            pauseButton.Enabled = false;
            pauseButton.Click += PauseButton_Click;

            // Status Label
            statusLabel = new Label();
            statusLabel.Text = "Bot Status: Stopped";
            statusLabel.Size = new Size(250, 20);
            statusLabel.Location = new Point(20, 70);
            statusLabel.Font = new Font("Arial", 12);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Debug TextBox
            debugTextBox = new TextBox();
            debugTextBox.Multiline = true;
            debugTextBox.ScrollBars = ScrollBars.Vertical;
            debugTextBox.Size = new Size(450, 280);
            debugTextBox.Location = new Point(20, 100);
            debugTextBox.ReadOnly = true;
            debugTextBox.Font = new Font("Consolas", 12);

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

            // Setup image update timer
            imageUpdateTimer = new System.Windows.Forms.Timer();
            imageUpdateTimer.Interval = 2000; // Update every 2 seconds
            imageUpdateTimer.Tick += UpdateImages;
            imageUpdateTimer.Start();

            // Add controls to form
            this.Controls.Add(startButton);
            this.Controls.Add(pauseButton);
            this.Controls.Add(statusLabel);
            this.Controls.Add(debugTextBox);
            
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



        private async void StartButton_Click(object? sender, EventArgs e)
        {
            if (botRunning) return;

            botRunning = true;
            botPaused = false;
            cancellationTokenSource = new CancellationTokenSource();

            if (startButton != null) startButton.Enabled = false;
            if (pauseButton != null)
            {
                pauseButton.Enabled = true;
                pauseButton.Text = "Pause Bot";
                pauseButton.BackColor = Color.Gold;
            }
            if (statusLabel != null) statusLabel.Text = "Bot Status: Running";

            try
            {
                await Task.Run(() => Program.Initilizebot(cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                // Bot was stopped normally
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bot error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BotStopped();
            }
        }


        // Pause/Resume logic
        private void PauseButton_Click(object? sender, EventArgs e)
        {
            if (!botRunning) return;

            botPaused = !botPaused;
            Program.pausebot = botPaused;

            if (pauseButton != null)
            {
                if (botPaused)
                {
                    pauseButton.Text = "Resume Bot";
                    pauseButton.BackColor = Color.LightGreen;
                    if (statusLabel != null) statusLabel.Text = "Bot Status: Paused";
                }
                else
                {
                    pauseButton.Text = "Pause Bot";
                    pauseButton.BackColor = Color.Gold;
                    if (statusLabel != null) statusLabel.Text = "Bot Status: Running";
                }
            }
        }

        private void BotStopped()
        {
            botRunning = false;
            botPaused = false;
            Program.pausebot = false;
            if (startButton != null) startButton.Enabled = true;
            if (pauseButton != null)
            {
                pauseButton.Enabled = false;
                pauseButton.Text = "Pause Bot";
                pauseButton.BackColor = Color.Gold;
            }
            if (statusLabel != null) statusLabel.Text = "Bot Status: Stopped";
        }

       

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (botRunning)
            {
                var result = MessageBox.Show("Bot is still running. Stop the bot and close?",
                                           "Confirm Close",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cancellationTokenSource?.Cancel();
                    Thread.Sleep(1000);
                }
                else
                {
                    e.Cancel = true; // Don't close the form
                    return;
                }
            }
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