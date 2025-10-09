using BussinessLogicLater.IService;
using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BussinessLogicLater.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;

        public QuestionService(IQuestionRepository Questionrepository, IMapper mapper, ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _questionRepository = Questionrepository;
            _mapper = mapper;
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken)
        {
            var data = await _questionRepository.GetAllWithIncludeAsync(PollId, cancellationToken, nameof(Question.Answers));

            var response = _mapper.Map<List<QuestionResponse>>(data);

            var result = Result<IEnumerable<QuestionResponse>>.Success(StatusCodes.Status200OK, response);

            return result;
        }

        public async Task<Result<QuestionResponse>> GetByIdAsync(int PollId, int id, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdWithIncludeAsync(PollId, id, cancellationToken, nameof(Question.Answers));

            if (question is null)
                return Result<QuestionResponse>.Fail(StatusCodes.Status404NotFound, new[] { "Question not found" });

            var response = _mapper.Map<QuestionResponse>(question);

            return Result<QuestionResponse>.Success(StatusCodes.Status200OK, response);
        }

        public async Task<Result<QuestionResponse>> CreateAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken)
        {
            var questionExists = await _questionRepository.CheckUniqueQuestionInPollForCreate(PollId, request.Content, cancellationToken);

            if (!questionExists)
                return Result<QuestionResponse>
                    .Fail(StatusCodes.Status400BadRequest, new[] { "Question already exists within the same poll" });

            var duplicateAnswers = _questionRepository.CheckUniqueAnswers(request.Answers);

            if (duplicateAnswers)
                return Result<QuestionResponse>
                    .Fail(StatusCodes.Status400BadRequest, new[] { "Duplicate Answer" });

            var question = _mapper.Map<Question>(request);

            var userId = _contextAccessor.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Result<QuestionResponse>.Fail(StatusCodes.Status401Unauthorized, new[] { "unauthorized user" });

            question.CreatedById = userId;
            question.PollId = PollId;

            await _questionRepository.AddAsync(question, cancellationToken);
            await _questionRepository.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<QuestionResponse>(question);

            return Result<QuestionResponse>.Success(StatusCodes.Status201Created, result);
        }

        public async Task<Result<bool>> UpdateAsync(int PollId, int QuestionId, QuestionRequest request, CancellationToken cancellationToken)
        {
            //Handle duplicate question in poll
            var isDuplicateQuestion = await _questionRepository
                .CheckUniqueQuestionInPollForUpdate(PollId, QuestionId, request.Content, cancellationToken);

            if (isDuplicateQuestion)
                return Result<bool>
                   .Fail(StatusCodes.Status400BadRequest, new[] { "Duplicate Question" });

            var duplicateAnswers = _questionRepository.CheckUniqueAnswers(request.Answers);

            if (duplicateAnswers)
                return Result<bool>
                    .Fail(StatusCodes.Status400BadRequest, new[] { "Duplicate Answer" });

            var question = await _questionRepository
                .GetByIdWithIncludeAsync(PollId, QuestionId, cancellationToken, nameof(Question.Answers));

            if (question == null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Question not found" });

            question.Content = request.Content.Trim();
            question.IsActive = request.IsActive;

            // request.Answers is a list of strings (the new answers)
            var existingAnswers = question.Answers.ToList();
            var requestAnswersNormalized = request.Answers
                .Select(a => a.Trim().ToLower())
                .ToList();

            // 1️ Remove answers that no longer exist in the request
            foreach (var answer in existingAnswers)
            {
                if (!requestAnswersNormalized.Contains(answer.Content.Trim().ToLower()))
                {
                    question.Answers.Remove(answer);
                }
            }

            // 2️ Add new answers that don't exist in the current question
            foreach (var answerText in request.Answers)
            {
                var normalized = answerText.Trim().ToLower();
                if (!existingAnswers.Any(a => a.Content.Trim().ToLower() == normalized))
                {
                    question.Answers.Add(new Answer { Content = answerText });
                }
            }

            //Handle user
            var userId = _contextAccessor.HttpContext!
               .User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Result<bool>.Fail(StatusCodes.Status401Unauthorized, new[] { "unauthorized user" });

            question.UpdatedById = userId;
            question.UpdatedOn = DateTime.UtcNow;

            _questionRepository.Update(question);
            await _questionRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status202Accepted, true);

        }

        public async Task<Result<bool>> DeleteAsync(int PollId, int QuestionId, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdWithIncludeAsync(PollId, QuestionId, cancellationToken, nameof(Question.Answers));

            if (question is null)
                return Result<bool>.Fail(StatusCodes.Status404NotFound, new[] { "Question not found" });

            _questionRepository.Delete(question);
            await _questionRepository.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(StatusCodes.Status204NoContent, true);
        }
    }
}
