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
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public PollsService(IRepository<Poll> pollsRepository, IMapper mapper, IHttpContextAccessor contextAccessor, ICacheService cacheService)
        {
            _pollsRepository = pollsRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _cacheService = cacheService;
        }

        public async Task<Result<IEnumerable<PollDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cacheKey = "polls:all";

            var cachedPolls = await _cacheService.GetAsync<IEnumerable<PollDto>>(cacheKey, cancellationToken);

            if (cachedPolls is not null)
                return Result<IEnumerable<PollDto>>.Success(StatusCodes.Status200OK, cachedPolls);


            var polls = await _pollsRepository.GetAllAsync(cancellationToken);
            var pollsDto = _mapper.Map<IEnumerable<PollDto>>(polls);

            if (pollsDto.Any())
            {
                await _cacheService.SetAsync(cacheKey, pollsDto, cancellationToken);
            }

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

            //Remove cached data
            var cacheKey = "polls:all";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

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

            //Remove cached data
            var cacheKey = "polls:all";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var poll = await _pollsRepository.GetByIdAsync(id, cancellationToken);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            _pollsRepository.Delete(poll);
            await _pollsRepository.SaveChangesAsync(cancellationToken);

            //Remove cached data
            var cacheKey = "polls:all";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

    }
}
