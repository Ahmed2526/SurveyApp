using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace BussinessLogicLater.Service
{
    public class PollsService : IPollsService
    {
        private readonly IRepository<Poll> _pollsRepository;
        private readonly IMapper _mapper;

        public PollsService(IRepository<Poll> pollsRepository, IMapper mapper)
        {
            _pollsRepository = pollsRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<PollDto>>> GetAllAsync()
        {
            var polls = await _pollsRepository.GetAllAsync();
            var pollsDto = _mapper.Map<IEnumerable<PollDto>>(polls);

            var result = Result<IEnumerable<PollDto>>.Success(StatusCodes.Status200OK, pollsDto);
            return result;
        }

        public async Task<Result<PollDto>> GetByIdAsync(int id)
        {
            var poll = await _pollsRepository.GetByIdAsync(id);

            if (poll is null)
                return Result<PollDto>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            var pollDto = _mapper.Map<PollDto>(poll);
            return Result<PollDto>.Success(StatusCodes.Status200OK, pollDto);
        }

        //handle Audit logging
        public async Task<Result<PollDto>> CreateAsync(PollCreateDto dto)
        {
            var poll = _mapper.Map<Poll>(dto);
            await _pollsRepository.AddAsync(poll);
            await _pollsRepository.SaveChangesAsync();

            var polldto = _mapper.Map<PollDto>(poll);

            return Result<PollDto>.Success(StatusCodes.Status201Created, polldto);
        }

        //handle Audit logging
        public async Task<Result<bool>> UpdateAsync(int id, PollCreateDto dto)
        {
            var poll = await _pollsRepository.GetByIdAsync(id);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            poll.Title = dto.Title;
            poll.Summary = dto.Summary;
            poll.IsPublished = dto.IsPublished;
            poll.StartsAt = dto.StartsAt;
            poll.EndsAt = dto.EndsAt;

            _pollsRepository.Update(poll);
            await _pollsRepository.SaveChangesAsync();

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var poll = await _pollsRepository.GetByIdAsync(id);

            if (poll is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Poll not found" });

            _pollsRepository.Delete(poll);
            await _pollsRepository.SaveChangesAsync();

            return Result<bool>.Success(StatusCodes.Status200OK, true);
        }

    }
}
