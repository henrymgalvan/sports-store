using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private ApplicationDbContext _context;

        public IEnumerable<Order> Orders => _context.Orders.Include(p => p.Lines).ThenInclude(l => l.Product);

        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public void SaveOrder(Order order)
        {
            _context.AttachRange(order.Lines.Select(p => p.Product));
            if (order.OrderId == 0)
            {
                _context.Orders.Add(order);
            }
            _context.SaveChanges();
        }
    }
}
