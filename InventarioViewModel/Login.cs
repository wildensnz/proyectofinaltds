using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioViewModel
{
    public class Login
    {
        public string user
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }       
        public string password
        {
            get;
            set;
        }

        public bool rememberLogin
        {
            get;
            set;
        }
        public string returnUrl
        {
            get;
            set;
        }
    }
}
