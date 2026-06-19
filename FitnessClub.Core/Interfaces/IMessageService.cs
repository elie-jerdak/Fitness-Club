using FitnessClub_Test.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessClub_Test.Core.Interfaces
{
    public interface IMessageService
    {
        Task SubmitMessageAsync(MessageDto dto);
        Task<PaginatedResult<MessageDto>> GetMessagesByUserAsync(int userId, int page, int pageSize);
    }
}
