using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbutils.Outdated
{
    /// <summary>
    /// This class stores the Connection Strings for all of us
    /// You can get a connection string to store here by connecting to the DB with Server Explorer.
    /// Once connected, hit properties and copy/paste the connection string to a readonly property here.
    /// </summary>
    public static class ConnectionStrings
    {

        public static string Marcus =>
            "Data Source=DESKTOP-OQ6MOPB\\SQLEXPRESS;Initial Catalog=WallyWorld;Integrated Security=True";
        public static string Shawn => "Data Source=AMD8-PC;Initial Catalog=WallyWorld;Integrated Security=True";

        public static string Prince =>
            "Data Source=PRINCE\\SQLEXPRESS;Initial Catalog=WallyWorld;Integrated Security=True";
    }
}
