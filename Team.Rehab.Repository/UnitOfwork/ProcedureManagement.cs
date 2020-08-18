using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.Rehab.DataModel;

namespace Team.Rehab.Repository.UnitOfwork
{
    public class ProcedureManagement : IDisposable, IProcedureManagement
    {
        private RehabEntities _context;
        public void Save()
        {
            _context.SaveChanges();
        }
        public ProcedureManagement()
        {
            _context = new RehabEntities();
        }
        private bool disposed = false;
        public List<SP_ValidateUser_Result> GetUser(string UserName, string Password)
        {
            var abc = _context.SP_ValidateUser(UserName, Password);
            return _context.SP_ValidateUser(UserName, Password).ToList<SP_ValidateUser_Result>();
        }
      
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //  Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
