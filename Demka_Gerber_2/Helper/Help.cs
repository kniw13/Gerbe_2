using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demka_Gerber_2.Models;
namespace Demka_Gerber_2.Helper
{
    internal class Help
    {
        private static Gerber_Entitites _context;
        public static Gerber_Entitites GetContext()
        {
            if (_context == null)
            {
                _context = new Gerber_Entitites();
            }
            return _context;
        }
    }
}
