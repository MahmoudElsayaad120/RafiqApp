using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models.OrderModels;
using Persistence;
using Services.Abstractions;
using Services.Specifications;
using Shared.OrderModels;

namespace Services
{
    public class OrderService(
        IMapper mapper,
        IUnitOfWork unitOfWork
        ) : IOrderService
    {
        public async Task<OrderResultDto> CreateOrderAsync(OrderRequestDto orderRequest, string userEmail)
        {
           var address = mapper.Map<address>(orderRequest.ShipToAddress);

            var orderItems = mapper.Map<ICollection<OrderItem>>(orderRequest.OrderItems);


            var order = new Order(userEmail,address, orderItems);
            await unitOfWork.GetRepository<Order, Guid>().AddAsyns(order);
            var count = await unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                return mapper.Map<OrderResultDto>(order);
            }
            var result = mapper .Map<OrderResultDto>(order);
            return result;

        }

        public async Task<OrderResultDto> GetOrderByIdAsync(Guid id)
        {
            var spec = new OrderSpecifications(id);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAsyns(spec);

           var result = mapper.Map<OrderResultDto>(order);
            
            return result;
        }

        public async Task<IEnumerable<OrderResultDto>> GetOrdersByUserEmailAsync(string userEmail)
        {
            var spec = new OrderSpecifications(userEmail);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetAllAsyns(spec);

            var result = mapper.Map<IEnumerable<OrderResultDto>>(order);

            return result;
        }
    }
}
