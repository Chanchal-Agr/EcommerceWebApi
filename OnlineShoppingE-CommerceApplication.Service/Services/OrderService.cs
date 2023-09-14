using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System.Linq.Dynamic.Core;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineShoppingE_CommerceApplication.Service.Services;
public class OrderService : IOrderService
{
    private readonly OnlineShoppingDbContext dbContext;
    public OrderService(OnlineShoppingDbContext dbContext)
    {
        this.dbContext = dbContext;

    }
    public async Task<int> Post(OrderDto orderDto, int customerId)
    {
        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                var cart = dbContext.Cart.Include(x => x.Customer).Include(p => p.ProductVariant).Where(x => x.CustomerId == customerId).ToList();
                if (cart == null)
                    return 0;
                else
                {
                    Provider.Entities.Order order = new Provider.Entities.Order();
                    List<OrderDetail> orderDetails = new List<OrderDetail>();
                    //Random rnd = new Random();
                    order.OrderNo = "DEKJ" + new Random().Next(100000, 999999);
                    order.CustomerId = customerId;
                    order.PaymentMode = orderDto.PaymentMode;
                    order.ShippingAddress = (orderDto.ShippingAddress != null) ? orderDto.ShippingAddress : cart.FirstOrDefault().Customer.Address;
                    order.TotalItems = cart.Count;
                    foreach (var item in cart)
                    {
                        var stocks = dbContext.Stock.Where(x => x.ProductVariantId == item.ProductVariantId && x.IsActive == true);
                        orderDetails.Add(new OrderDetail
                        {
                            ProductVariantId = item.ProductVariantId,
                            Quantity = item.Quantity,
                            Price = stocks.Max(s => s.SellingPrice)
                        });
                        if (item.Quantity < stocks.FirstOrDefault().StockToSale)
                        {
                            stocks.FirstOrDefault().StockToSale -= item.Quantity;
                            dbContext.SaveChanges();
                        }
                        else if (item.Quantity == stocks.FirstOrDefault().StockToSale)
                        {
                            stocks.FirstOrDefault().StockToSale = 0;
                            stocks.FirstOrDefault().IsActive = false;
                            dbContext.SaveChanges();
                        }
                        else//item.quantity > firststock
                        {
                            foreach (var stock in stocks)
                            {
                                if (item.Quantity > 0)
                                {
                                    if (item.Quantity > stock.StockToSale)
                                    {
                                        item.Quantity -= stock.StockToSale;
                                        stock.StockToSale = 0;
                                        stock.IsActive = false;
                                    }
                                    else if (item.Quantity < stock.StockToSale)
                                    {
                                        stock.StockToSale -= item.Quantity;
                                        item.Quantity = 0;
                                    }
                                    else
                                    {
                                        stock.StockToSale = item.Quantity = 0;
                                        stock.IsActive = false;
                                    }
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    order.TotalPrice = orderDetails.Sum(x => x.Quantity * x.Price);
                    dbContext.Order.Add(order);
                    dbContext.SaveChanges();
                    orderDetails.ForEach(x => x.OrderId = order.Id);
                    dbContext.AddRange(orderDetails);
                    dbContext.RemoveRange(cart);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    return order.Id;
                }
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

    }

    public async Task<bool> UpdateStatus(int orderId, OrderStatus status)
    {
        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                Provider.Entities.Order? order = await dbContext.Order.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order != null)
                {
                    order.UpdatedAt = DateTime.Now;
                    order.Status = status;
                    IQueryable<Provider.Entities.OrderDetail> orderDetails = dbContext.OrderDetail.Where(x => x.OrderId == orderId);
                    if (status == OrderStatus.Delivered)
                    {
                        foreach (var orderDetail in orderDetails)
                        {
                            var stocks = dbContext.Stock.Where(x => x.ProductVariantId == orderDetail.ProductVariantId);
                            if (orderDetail.Quantity <= stocks.FirstOrDefault().AvailableQuantity)
                            {
                                stocks.FirstOrDefault().AvailableQuantity -= orderDetail.Quantity;
                                dbContext.SaveChanges();
                            }
                            else//orderdetail.quantity > firststock
                            {
                                int quantity = orderDetail.Quantity;
                                foreach (var stock in stocks)
                                {
                                    if (quantity > 0)
                                    {
                                        if (quantity > stock.AvailableQuantity)
                                        {
                                            quantity -= stock.AvailableQuantity;
                                            stock.AvailableQuantity = 0;
                                        }
                                        else
                                        {
                                            stock.AvailableQuantity -= quantity;
                                            quantity = 0;
                                        }
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        if (status == OrderStatus.Cancelled)
                        {
                            foreach (var orderDetail in orderDetails)
                            {
                                var stocks = dbContext.Stock.Where(x => x.ProductVariantId == orderDetail.ProductVariantId);
                                if (orderDetail.Quantity <= stocks.FirstOrDefault().AvailableQuantity - stocks.FirstOrDefault().StockToSale)
                                {
                                    stocks.FirstOrDefault().StockToSale += orderDetail.Quantity;
                                    stocks.FirstOrDefault().IsActive = true;
                                    dbContext.SaveChanges();
                                }
                                else//orderdetail.quantity > firststock
                                {
                                    int quantity = orderDetail.Quantity;
                                    foreach (var stock in stocks)
                                    {
                                        if (quantity > 0)
                                        {
                                            if (quantity > stock.AvailableQuantity)
                                            {
                                                stock.StockToSale += stock.AvailableQuantity;
                                                stock.IsActive = true;
                                                quantity -= stock.AvailableQuantity;
                                            }
                                            else
                                            {
                                                stock.StockToSale += quantity;
                                                stock.IsActive = true;
                                                quantity = 0;
                                            }
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                                dbContext.SaveChanges();
                            }

                        }
                    }
                    transaction.Commit();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }

    public async Task<OrderInfoDto> GetOrders(OrderQuery query)
    {

        OrderInfoDto orderDetail = new OrderInfoDto();
        query.EndDate = query.EndDate?.Date.AddDays(1).AddSeconds(-1);
        var orders = dbContext.Order.Where(x => x.CustomerId == query.CustomerId && (
        query.StartDate != null ? (query.EndDate != null ? (x.CreatedAt >= query.StartDate && x.CreatedAt <= query.EndDate) : x.CreatedAt >= query.StartDate) : (query.EndDate != null ? x.CreatedAt <= query.EndDate : true)) && (query.Status != null ? x.Status == query.Status : true));
        if (!orders.Any())
            return null;

        if (query.OrderBy != null)
            orders = QueryableExtensions.OrderBy(orders, query.OrderBy);

        orderDetail.TotalRecords = orders.Count();
        if (query.IsPagination)
        {
            int TotalPages = (int)Math.Ceiling(orders.Count() / (double)query.PageSize);

            var items = orders.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();

            orderDetail.OrderDetails = new List<OrderInfo>();
            foreach (var item in items)
            {
                orderDetail.OrderDetails.Add(new OrderInfo
                {
                    Id = item.Id,
                    OrderDate = item.CreatedAt.Date,
                    OrderNo = item.OrderNo,
                    PaymentMode = item.PaymentMode,
                    ShippingAddress = item.ShippingAddress,
                    Status = item.Status,
                    TotalItems = item.TotalItems,
                    TotalPrice = item.TotalPrice

                });
            }
        }
        else
        {
            orderDetail.OrderDetails = new List<OrderInfo>();
            foreach (var item in orders)
            {
                orderDetail.OrderDetails.Add(new OrderInfo
                {
                    Id = item.Id,
                    OrderDate = item.CreatedAt.Date,
                    OrderNo = item.OrderNo,
                    PaymentMode = item.PaymentMode,
                    ShippingAddress = item.ShippingAddress,
                    Status = item.Status,
                    TotalItems = item.TotalItems,
                    TotalPrice = item.TotalPrice

                });
            }
        }
        return orderDetail;
    }
    public async Task<OrderDetailDto> GetById(int orderId)
    {
        OrderDetailDto orderDetail = new OrderDetailDto();
        var orders = dbContext.OrderDetail.Include(x => x.ProductVariant).Include(x => x.Order).ThenInclude(x => x.Customer).Where(x => x.OrderId == orderId).ToList();
        if (orders == null)
            return null;

        orderDetail.CustomerId = orders.FirstOrDefault().Order.Customer.Id;
        orderDetail.CustomerName = orders.FirstOrDefault().Order.Customer.Name;
        orderDetail.OrderId = orderId;

        foreach (var order in orders)
        {
            orderDetail.ItemDetails = new List<ItemDetail>();
            orderDetail.ItemDetails.Add(new ItemDetail()
            {
                Id = order.Id,
                Quantity = order.Quantity,
                ImagePath = order.ProductVariant.Path.Split('|').First(),
                Price = order.Price
            });
        }
        return orderDetail;
    }
}
