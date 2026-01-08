using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//Just some vibecode for the Debug help when mouse clicks screen so i can see where it tried to click
namespace Gamebot
{
    /// <summary>
    /// Shows a small red dot on screen to indicate where a click occurred
    /// </summary>
    public class ClickIndicator : Form
    {
        // Win32 API for making window click-through
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        private System.Windows.Forms.Timer? disposalTimer;
        private const int DOT_SIZE = 12; // Size of the red dot
        private const int DISPLAY_DURATION = 2000; // How long to show (milliseconds)
        
        // Keep track of active indicators
        private static List<ClickIndicator> activeIndicators = new List<ClickIndicator>();
        private static object lockObject = new object();

        public ClickIndicator(int screenX, int screenY)
        {
            // Form setup - no borders, always on top, small circular dot
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(DOT_SIZE, DOT_SIZE);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(screenX - DOT_SIZE / 2, screenY - DOT_SIZE / 2);
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Red;
            
            // Make it circular
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, DOT_SIZE, DOT_SIZE);
            this.Region = new Region(path);

            // Set up timer to auto-close after duration
            disposalTimer = new System.Windows.Forms.Timer();
            disposalTimer.Interval = DISPLAY_DURATION;
            disposalTimer.Tick += (sender, e) => 
            {
                disposalTimer?.Stop();
                disposalTimer?.Dispose();
                RemoveFromActiveList();
                this.Close();
            };
            disposalTimer.Start();
            
            // Add to active indicators list
            lock (lockObject)
            {
                activeIndicators.Add(this);
            }
        }
        
        private void RemoveFromActiveList()
        {
            lock (lockObject)
            {
                activeIndicators.Remove(this);
            }
        }
        
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Make the window click-through (transparent to mouse events)
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw a solid red circle
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillEllipse(brush, 0, 0, DOT_SIZE - 1, DOT_SIZE - 1);
            }
            // Add a small white border for visibility
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawEllipse(pen, 1, 1, DOT_SIZE - 3, DOT_SIZE - 3);
            }
        }

        /// <summary>
        /// Closes all currently active click indicators
        /// </summary>
        public static void CloseAllIndicators()
        {
            lock (lockObject)
            {
                // Create a copy to avoid modification during iteration
                var indicatorsToClose = new List<ClickIndicator>(activeIndicators);
                activeIndicators.Clear();
                
                foreach (var indicator in indicatorsToClose)
                {
                    try
                    {
                        if (indicator != null && !indicator.IsDisposed)
                        {
                            if (indicator.InvokeRequired)
                            {
                                indicator.Invoke(new Action(() => indicator.Close()));
                            }
                            else
                            {
                                indicator.Close();
                            }
                        }
                    }
                    catch
                    {
                        // Silently ignore errors closing indicators
                    }
                }
            }
        }

        /// <summary>
        /// Shows a click indicator at the specified screen coordinates if debug mode is enabled
        /// </summary>
        public static void ShowClickIndicator(int screenX, int screenY)
        {
            // Only show if debug mode is enabled
            if (Program.settings.DebugMode)
            {
                try
                {
                    // Must create on UI thread
                    if (Application.OpenForms.Count > 0)
                    {
                        var mainForm = Application.OpenForms[0];
                        if (mainForm.InvokeRequired)
                        {
                            mainForm.Invoke(new Action(() =>
                            {
                                ClickIndicator indicator = new ClickIndicator(screenX, screenY);
                                indicator.Show();
                            }));
                        }
                        else
                        {
                            ClickIndicator indicator = new ClickIndicator(screenX, screenY);
                            indicator.Show();
                        }
                    }
                }
                catch
                {
                    // Silently fail if we can't show the indicator
                }
            }
        }
    }
}
