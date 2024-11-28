using Bank.Data.Entities;
using Bank.InfrastructureBases;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private DbSet<Message> _messages;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _messages = context.Set<Message>();
        }
    }
}
