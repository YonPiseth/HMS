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
                        catch (Exception)
                        {
                            MessageBox.Show("Error showing splash screen", "Splash Screen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                break; // Exit on unexpected error
            }
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show("An unhandled application error occurred: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

}
