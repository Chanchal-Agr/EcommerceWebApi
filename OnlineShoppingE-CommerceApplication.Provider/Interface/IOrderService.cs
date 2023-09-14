using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IOrderService
    {
        Task<int> Post(OrderDto orderDto,int customerId);
        Task<bool> UpdateStatus(int orderId, OrderStatus status);
        Task<OrderInfoDto> GetOrders(OrderQuery query);
        Task<OrderDetailDto> GetById(int orderId);  
    }
}

