using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entensions;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class PollRepository :Repository<Poll>, IPollRepository
    {
        public PollRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<PagedResult<Poll>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken)
        {
            var query = _context.Polls.AsNoTracking();

            var result = await query.ToPagedResultAsync(filterRequest.PageNumber, filterRequest.PageSize, cancellationToken);

            return result;
        }
    }
}
