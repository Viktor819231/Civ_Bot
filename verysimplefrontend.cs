using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NavigationAndLocations;
using OCR;

//Vibecoded frontend
namespace Gamebot
{
    public partial class BotFrontend : Form
    {
        private Button? startButton;
        private Button? stopButton;
        private Label? statusLabel;
        private TextBox? debugTextBox;
        private CancellationTokenSource? cancellationTokenSource;
        private bool botRunning = false;

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
                Console.WriteLine($"Failed to initialize bot systems: {ex.Message}");
                if (statusLabel != null)
                    statusLabel.Text = "Bot Status: Error - " + ex.Message;
                if (startButton != null)
                    startButton.Enabled = false;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Civ Bot Controller";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Start Button
            startButton = new Button();
            startButton.Text = "Start Bot";
            startButton.Size = new Size(100, 30);
            startButton.Location = new Point(20, 20);
            startButton.BackColor = Color.LightGreen;
            startButton.Click += StartButton_Click;

            // Stop Button
            stopButton = new Button();
            stopButton.Text = "Stop Bot";
            stopButton.Size = new Size(100, 30);
            stopButton.Location = new Point(140, 20);
            stopButton.BackColor = Color.LightCoral;
            stopButton.Enabled = false;
            stopButton.Click += StopButton_Click;

            // Status Label
            statusLabel = new Label();
            statusLabel.Text = "Bot Status: Stopped";
            statusLabel.Size = new Size(250, 20);
            statusLabel.Location = new Point(20, 70);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Debug TextBox
            debugTextBox = new TextBox();
            debugTextBox.Multiline = true;
            debugTextBox.ScrollBars = ScrollBars.Vertical;
            debugTextBox.Size = new Size(450, 280);
            debugTextBox.Location = new Point(20, 100);
            debugTextBox.ReadOnly = true;
            debugTextBox.Font = new Font("Consolas", 8);

            // Add controls to form
            this.Controls.Add(startButton);
            this.Controls.Add(stopButton);
            this.Controls.Add(statusLabel);
            this.Controls.Add(debugTextBox);

            // Redirect console output to debug textbox
            Console.SetOut(new TextBoxWriter(debugTextBox));
        }



        private async void StartButton_Click(object? sender, EventArgs e)
        {
            if (botRunning) return;

            botRunning = true;
            cancellationTokenSource = new CancellationTokenSource();

            if (startButton != null) startButton.Enabled = false;
            if (stopButton != null) stopButton.Enabled = true;
            if (statusLabel != null) statusLabel.Text = "Bot Status: Running";

            try
            {
                await Task.Run(() => RunBotLoop(cancellationTokenSource.Token));
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

        private void StopButton_Click(object? sender, EventArgs e)
        {
            if (!botRunning) return;

            cancellationTokenSource?.Cancel();
            BotStopped();
        }

        private void BotStopped()
        {
            botRunning = false;
            if (startButton != null) startButton.Enabled = true;
            if (stopButton != null) stopButton.Enabled = false;
            if (statusLabel != null) statusLabel.Text = "Bot Status: Stopped";
        }

        private void RunBotLoop(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("=== BOT STARTING ===");
                Console.WriteLine($"Base Directory: {AppContext.BaseDirectory}");
                Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
                Console.WriteLine($"Process MainModule: {System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName}");
                Console.WriteLine($"Settings loaded: {Program.settings.LobbyName}");

                // Test AHK scripts
                Console.WriteLine("Testing AHK scripts...");
                CivBot.Sleep(3000);
                try
                {
                    CivBot.MoveMouseTo(CivButton.outoftheway);
                    Console.WriteLine("✓ AHK mouse movement successful");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"✗ AHK failed: {e.Message}");
                }

                // Test Tesseract OCR
                Console.WriteLine("Testing Tesseract OCR...");

                try
                {
                    Console.WriteLine("Starting OCR test...");
                    Console.WriteLine($"MenuText rectangle: {CivTextBox.MenuText.GetRectanglePictureBox()}");
                    Console.WriteLine($"MenuText filename: '{CivTextBox.MenuText.filename}'");

                    string testText = ImgToText.TextAt(CivTextBox.MenuText.GetRectanglePictureBox(), CivTextBox.MenuText.filename);
                    Console.WriteLine($"✓ OCR successful, text: '{testText.Trim()}'");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"✗ OCR failed: {e.Message}");
                    if (e.InnerException != null)
                        Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {e.StackTrace}");
                }

                // Run the main bot function with cancellation support
                Console.WriteLine("Starting main bot logic...");
                Program.RunBareBonesBot(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("=== BOT STOPPED BY USER ===");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== BOT ERROR ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (botRunning)
            {
                // Show confirmation dialog
                var result = MessageBox.Show("Bot is still running. Stop the bot and close?",
                                           "Confirm Close",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cancellationTokenSource?.Cancel();

                    // Wait a moment for graceful shutdown
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