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

        while (true)
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
                        MessageBox.Show("Error showing splash screen: " + splashEx.Message, "Splash Screen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Exit if splash screen fails
                    }

                    try
                    {
                        using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserID))
                        {
                            Application.Run(mainForm);
                            if (mainForm.IsLogout)
                                continue; // Restart login after logout
                            else
                                break; // Exit if main form closed without logout
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error starting main form: " + ex.Message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    break; // Exit if login is cancelled
                }
            }
        }
    }    
}