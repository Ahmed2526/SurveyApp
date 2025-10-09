using DataAccessLayer.Data;
using DataAccessLayer.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            var data = await _dbSet.ToListAsync(cancellationToken);
            return data;
        }
        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var data = await _dbSet.FindAsync(id, cancellationToken);
            return data;
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int result = await _context.SaveChangesAsync(cancellationToken);
            return result;
        }

    }
}
