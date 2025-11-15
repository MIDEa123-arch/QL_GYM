using ManagerApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ManagerApp.Repositories
{
    public class SanPhamRepository
    {
        private readonly QL_PHONGGYM _context;
        public SanPhamRepository(string connStr)
        {
            _context = new QL_PHONGGYM(connStr);
        }    
        public List<SANPHAM> GetAll()
        {
            return _context.SANPHAMs.ToList();
        }

        public SANPHAM GetById(int id)
        {
            return _context.SANPHAMs.Find(id);
        }
   
        public void Add(SANPHAM sp)
        {
            _context.SANPHAMs.Add(sp);
            _context.SaveChanges();
        }

        public void Update(SANPHAM sp)
        {
            _context.Entry(sp).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var sp = _context.SANPHAMs.Find(id);
            if (sp != null)
            {
                _context.SANPHAMs.Remove(sp);
                _context.SaveChanges();
            }
        }
    }
}
