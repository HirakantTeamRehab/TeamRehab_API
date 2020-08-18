using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace Team.Rehab.Repository
{
    public interface IRepository<TEntity>
    {
        // TO DO  Remove Find and SelectQuery Method as redundant are old repositories methods prior to unitOfWork
        TEntity Find(params object[] keyValues);
        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        IEnumerable<TEntity> GetFromSQL(string strSQL);
        IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] parameters);
    }
}
