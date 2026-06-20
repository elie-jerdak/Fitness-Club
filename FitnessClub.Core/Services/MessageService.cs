using FitnessClub_Test.Core.Interfaces;
using FitnessClub_Test.Core.NewModels;
using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub_Test.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly FitnessClubDbContext _context;

        public MessageService(FitnessClubDbContext context)
        {
            _context = context;
        }

        public async Task SubmitMessageAsync(MessageDto dto)
        {
            var message = new Message
            {
                Title = dto.Title,
                Content = dto.Content,
                Type = dto.Type,
                Status = "Open",
                Time = DateTime.UtcNow,
                IsDeleted = false,
                UserId = dto.UserId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesByUserAsync(int userId, int page, int pageSize)
        {
            var query = _context.Messages
                .Where(m => m.UserId == userId && !m.IsDeleted)
                .OrderByDescending(m => m.Time);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .Select(m => new MessageDto
                                   {
                                       Title = m.Title,
                                       Content = m.Content,
                                       Type = m.Type,
                                       Time = m.Time
                                   }).ToListAsync();

            return new PaginatedResult<MessageDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
