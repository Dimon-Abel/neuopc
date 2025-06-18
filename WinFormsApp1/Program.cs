namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            try
            {
                IntPtr pValues = IntPtr.Zero;
                var values = OpcCom.Da.Interop.GetItemValues(ref pValues, 100, true);
            }
            catch (Exception ex)
            {

            }

            Application.Run(new Form1());
        }
    }
}