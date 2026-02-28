using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using HMS.Repositories;
using HMS.Services;
using HMS.Models;

namespace HMS;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Configure Services
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

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

                        // Resolve MainForm with dependencies
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var mainForm = new MainForm(loginForm.UserRole, loginForm.UserID, 
                                scope.ServiceProvider);
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

    private static void ConfigureServices(IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepository<Doctor>, DoctorRepository>();
        services.AddScoped<IRepository<Patient>, PatientRepository>();
        services.AddScoped<IRepository<Supplier>, SupplierRepository>();
        // Add other generic repositories...

        // Services
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        // Add other domain services...

        // Controls
        services.AddTransient<DoctorControl>();
        services.AddTransient<PatientControl>();
        services.AddTransient<AppointmentControl>();
        services.AddTransient<RoomControl>();
        services.AddTransient<InvoiceControl>();
        services.AddTransient<DiseaseControl>();
        services.AddTransient<SupplierControl>();
        services.AddTransient<MedicineControl>();
        services.AddTransient<BillingControl>();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show("An unhandled application error occurred: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

}
