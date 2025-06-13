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
                    using (var mainForm = new MainForm(loginForm.UserRole, loginForm.UserID))
                    {
                        Application.Run(mainForm);
                    }
                }
                else
                {
                    break; // Exit the application if login is cancelled
                }
            }
        }
    }    
}