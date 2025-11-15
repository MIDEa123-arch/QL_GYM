using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerApp.Models;
namespace ManagerApp.Repositories
{
    public class SanPhamRepository
    {
        private readonly QL_PHONGGYM _context;

        public SanPhamRepository(QL_PHONGGYM context)
        {
            _context = context;
        }
    }
}