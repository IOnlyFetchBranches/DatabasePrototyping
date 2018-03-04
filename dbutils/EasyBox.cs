using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace dbutils
{
    /// <summary>
    /// Contains methods for easily handling and displaying exceptions.
    /// </summary>
    public abstract class EasyBox
    {
        /// <summary>
        /// This method will take in an exception, display it in a nice manner.
        /// It will also auto log using Logger.
        /// </summary>
        /// <param name="exception">The exception</param>
        public static void ShowError(Exception exception)
        {
            MessageBox.Show("The program has encountered an error and needs to exit.\n" + "Reason: " + exception.Message + "\n" +
                            "You can find more details about this in the log file!", "Something Bad Happened...",
                MessageBoxButton.OK, MessageBoxImage.Error);

            bool hasInner = exception.InnerException != null;
            Logger.LogG("Caught Error: " + exception.GetType() + "\nMessage" + exception.Message +"\nDetails:" +exception.StackTrace);

            if (hasInner)
            {
                Logger.LogG("Caught Inner Error: " + exception.InnerException.GetType()
                    + "\nMessage" + exception.InnerException.Message + "\nDetails:" + exception.InnerException.StackTrace);
            }
            

        }
    }
}
