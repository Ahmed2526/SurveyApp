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
        private readonly IPollRepository _pollRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private const string _cachePrefix = "availablePolls";
        public PollsService(IPollRepository pollsRepository, IMapper mapper, IHttpContextAccessor contextAccessor, ICacheService cacheService)
        {
            _pollRepository = pollsRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _cacheService = cacheService;
        }

        public async Task<Result<PagedResult<PollDto>>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken)
        {
            var cacheKey = GenerateCacheKey(filterRequest);

            var cachedPolls = await _cacheService.GetAsync<PagedResult<PollDto>>(cacheKey, cancellationToken);

            if (cachedPolls is not null)
                return Result<PagedResult<PollDto>>.Success(StatusCodes.Status200OK, cachedPolls);


            var polls = await _pollRepository.GetAllAsync(filterRequest, cancellationToken);

            var pollsDto = _mapper.Map<PagedResult<PollDto>>(polls);

            if (pollsDto.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pollsDto, cancellationToken);
            }

            var result = Result<PagedResult<PollDto>>.Success(StatusCodes.Status200OK, pollsDto);
            return result;
        }

        public async Task<Result<PollDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var poll = await _pollRepository.GetByIdAsync(id, cancellationToken);

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

            await _pollRepository.AddAsync(poll, cancellationToken);
            await _pollRepository.SaveChangesAsync(cancellationToken);

            //Remove cached data
            var cacheKey = _cachePrefix;
            await _cacheService.RemoveByPrefixAsync(cacheKey, cancellationToken);

            var polldto = _mapper.Map<PollDto>(poll);

            return Result<PollDto>.Success(StatusCodes.Status201Created, polldto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, PollCreateDto dto, CancellationToken cancellationToken)
        {
            var poll = await _pollRepository.GetByIdAsync(id, cancellationToken);

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

            _pollRepository.Update(poll);
            await _pollRepository.SaveChangesAsync(cancellationToken);

            //Remove cached data
            var cacheKey = _cachePrefix;
            await _cacheService.RemoveByPrefixAsync(cacheKey, cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var poll = await _pollRepository.GetByIdAsync(id, cancellationToken);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            _pollRepository.Delete(poll);
            await _pollRepository.SaveChangesAsync(cancellationToken);

            //Remove cached data
            var cacheKey = _cachePrefix;
            await _cacheService.RemoveByPrefixAsync(cacheKey, cancellationToken);

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        private string GenerateCacheKey(FilterRequest filterRequest)
            => $"{_cachePrefix}:page:{filterRequest.PageNumber}:size:{filterRequest.PageSize}";

    }
}
