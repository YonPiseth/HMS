using System;
using System.Windows.Forms;

namespace HMS;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Add a global exception handler for unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        while (true)
        {
            try
            {
                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // If login is successful, show splash screen, then run the main form
                        try
                        {
                            using (var splashForm = new SplashForm())
                            {
                                splashForm.ShowDialog();
                            }
                        }
                        catch (Exception splashEx)
                        {
                            LogException(splashEx, "Error showing splash screen");
                            MessageBox.Show("Error showing splash screen: " + splashEx.Message, "Splash Screen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Exit if splash screen fails
                        }

                        using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserID))
                        {
                            Application.Run(mainForm);
                            if (mainForm.IsLogout)
                                continue; // Restart login after logout
                            else
                                break; // Exit if main form closed without logout
                        }
                    }
                    else
                    {
                        break; // Exit if login is cancelled
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "Error in main application loop");
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break; // Exit on unexpected error
            }
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        LogException(ex, "Unhandled Exception");
        MessageBox.Show("An unhandled application error occurred: " + ex.Message + "\n\nDetails logged to error.log", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void LogException(Exception ex, string context)
    {
        try
        {
            string logFilePath = "error.log";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
            {
                writer.WriteLine("Date: " + DateTime.Now.ToString());
                writer.WriteLine("Context: " + context);
                writer.WriteLine("Message: " + ex.Message);
                writer.WriteLine("Stack Trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    writer.WriteLine("Inner Exception Message: " + ex.InnerException.Message);
                    writer.WriteLine("Inner Exception Stack Trace: " + ex.InnerException.StackTrace);
                }
                writer.WriteLine(new string('-', 50));
            }
        }
        catch (Exception logEx)
        {
            MessageBox.Show("Error writing to log file: " + logEx.Message, "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
