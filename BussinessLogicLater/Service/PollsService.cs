using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BussinessLogicLater.Service
{
    public class PollsService : IPollsService
    {
        private readonly IRepository<Poll> _pollsRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public PollsService(IRepository<Poll> pollsRepository, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _pollsRepository = pollsRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<IEnumerable<PollDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var polls = await _pollsRepository.GetAllAsync(cancellationToken);
            var pollsDto = _mapper.Map<IEnumerable<PollDto>>(polls);

            var result = Result<IEnumerable<PollDto>>.Success(StatusCodes.Status200OK, pollsDto);
            return result;
        }

        public async Task<Result<PollDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var poll = await _pollsRepository.GetByIdAsync(id, cancellationToken);

            if (poll is null)
                return Result<PollDto>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            var pollDto = _mapper.Map<PollDto>(poll);
            return Result<PollDto>.Success(StatusCodes.Status200OK, pollDto);
        }

        public async Task<Result<PollDto>> CreateAsync(PollCreateDto dto, CancellationToken cancellationToken)
        {
            var poll = _mapper.Map<Poll>(dto);

            var userId = _contextAccessor.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Result<PollDto>.Fail(StatusCodes.Status401Unauthorized, new[] { "unauthorized user" });

            poll.CreatedById = userId;

            await _pollsRepository.AddAsync(poll, cancellationToken);
            await _pollsRepository.SaveChangesAsync(cancellationToken);

            var polldto = _mapper.Map<PollDto>(poll);

            return Result<PollDto>.Success(StatusCodes.Status201Created, polldto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, PollCreateDto dto, CancellationToken cancellationToken)
        {
            var poll = await _pollsRepository.GetByIdAsync(id, cancellationToken);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            var userId = _contextAccessor.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "unauthorized user" });

            poll.Title = dto.Title;
            poll.Summary = dto.Summary;
            poll.IsPublished = dto.IsPublished;
            poll.StartsAt = dto.StartsAt;
            poll.EndsAt = dto.EndsAt;
            poll.UpdatedById = userId;
            poll.UpdatedOn = DateTime.Now;

            _pollsRepository.Update(poll);
            await _pollsRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var poll = await _pollsRepository.GetByIdAsync(id, cancellationToken);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            _pollsRepository.Delete(poll);
            await _pollsRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

    }
}
