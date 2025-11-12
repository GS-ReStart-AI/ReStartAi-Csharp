using ReStartAI.Infrastructure.Context;

namespace ReStartAI.Infrastructure.Repositories
{
    public class LogRepository
    {
        private readonly AppLogContext _context;

        public LogRepository(AppLogContext context)
        {
            _context = context;
        }

        public async Task AddAsync(string message)
        {
            _context.Logs.Add(new LogEntry { Message = message });
            await _context.SaveChangesAsync();
        }

        public IEnumerable<LogEntry> GetAll()
        {
            return _context.Logs.OrderByDescending(l => l.CreatedAt).ToList();
        }
    }
}