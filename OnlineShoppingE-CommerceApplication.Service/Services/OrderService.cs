using Microsoft.EntityFrameworkCore;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using System.Linq.Dynamic.Core;
using OnlineShoppingE_CommerceApplication.Provider.Extensions;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Runtime.InteropServices;

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
                if (cart.Count == 0)
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

    public async Task<OrderResponseDto> GetOrders(OrderQuery query, int customerId)
    {

        //OrderInfoDto orderDetail = new OrderInfoDto();
        //query.EndDate = query.EndDate?.Date.AddDays(1).AddSeconds(-1);
        //var orders = dbContext.Order.Where(x => x.CustomerId == customerId && (
        //query.StartDate != null ? (query.EndDate != null ? (x.CreatedAt >= query.StartDate && x.CreatedAt <= query.EndDate) : x.CreatedAt >= query.StartDate) : (query.EndDate != null ? x.CreatedAt <= query.EndDate : true)) && (query.Status != null ? x.Status == query.Status : true));
        //if (!orders.Any())
        //    return null;

        //if (query.OrderBy != null)
        //    orders = QueryableExtensions.OrderBy(orders, query.OrderBy);

        //orderDetail.TotalRecords = orders.Count();
        //if (query.IsPagination)
        //{
        //    int TotalPages = (int)Math.Ceiling(orders.Count() / (double)query.PageSize);

        //    var items = orders.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();

        //    orderDetail.OrderDetails = new List<OrderInfo>();
        //    foreach (var item in items)
        //    {
        //        orderDetail.OrderDetails.Add(new OrderInfo
        //        {
        //            Id = item.Id,
        //            OrderDate = item.CreatedAt.Date,
        //            OrderNo = item.OrderNo,
        //            PaymentMode = item.PaymentMode,
        //            ShippingAddress = item.ShippingAddress,
        //            Status = item.Status,
        //            TotalItems = item.TotalItems,
        //            TotalPrice = item.TotalPrice

        //        });
        //    }
        //}
        //else
        //{
        //    orderDetail.OrderDetails = new List<OrderInfo>();
        //    foreach (var item in orders)
        //    {
        //        orderDetail.OrderDetails.Add(new OrderInfo
        //        {
        //            Id = item.Id,
        //            OrderDate = item.CreatedAt.Date,
        //            OrderNo = item.OrderNo,
        //            PaymentMode = item.PaymentMode,
        //            ShippingAddress = item.ShippingAddress,
        //            Status = item.Status,
        //            TotalItems = item.TotalItems,
        //            TotalPrice = item.TotalPrice

        //        });
        //    }
        //}
        //return orderDetail;
        OrderResponseDto response = new OrderResponseDto();
        query.EndDate = query.EndDate?.Date.AddDays(1).AddSeconds(0);
        var orders = dbContext.Order.Include(x => x.Customer).Where(x => x.CustomerId == customerId && (query.StartDate != null ? (query.EndDate != null ? (x.CreatedAt >= query.StartDate && x.CreatedAt <= query.EndDate) : x.CreatedAt >= query.StartDate) : (query.EndDate != null ? x.CreatedAt <= query.EndDate : true)) && (query.Status != null ? x.Status == query.Status : true));
        if (!orders.Any())
            return null;
        if (query.OrderBy != null)
            orders = QueryableExtensions.OrderBy(orders, query.OrderBy);


        response.CustomerId = customerId;
        response.CustomerName = orders.FirstOrDefault().Customer.Name;
        response.Address = orders.FirstOrDefault().Customer.Address;
        response.TotalOrders = orders.Count();
        response.Orders = new List<OrderDetailDto>();
        if (query.IsPagination)
        {
            int TotalPages = (int)Math.Ceiling(orders.Count() / (double)query.PageSize);

            var items = orders.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();

            foreach (var item in items)
            {
                var data = dbContext.OrderDetail.Include(x => x.ProductVariant).ThenInclude(x => x.Product).Where(x => x.OrderId == item.Id);

                List<ItemDetail> itemDetails = new List<ItemDetail>();
                foreach (var detail in data)
                {
                    itemDetails.Add(new ItemDetail()
                    {
                        ProductName = detail.ProductVariant.Product.Name,
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                        Description = detail.ProductVariant.Product.Description,
                        ImagePath = detail.ProductVariant.Path.Split('|').First(),
                        Colour = dbContext.Colour.First(x => x.Id == detail.ProductVariant.ColourId).Name,
                        Size = dbContext.Size.First(x => x.Id == detail.ProductVariant.SizeId).Name
                    });
                }

                response.Orders.Add(new OrderDetailDto
                {
                    OrderId = item.Id,
                    OrderDate = item.CreatedAt,
                    OrderNo = item.OrderNo,
                    PaymentMode = item.PaymentMode,
                    ShippingAddress = item.ShippingAddress,
                    Status = item.Status,
                    TotalPrice = item.TotalPrice,
                    TotalItems = item.TotalItems,
                    ItemDetails = itemDetails
                });
            }
        }
        else
        {

            foreach (var item in orders)
            {
                var data = dbContext.OrderDetail.Include(x => x.ProductVariant).ThenInclude(x => x.Product).Where(x => x.OrderId == item.Id);

                List<ItemDetail> itemDetails = new List<ItemDetail>();
                foreach (var detail in data)
                {
                    itemDetails.Add(new ItemDetail()
                    {
                        ProductName = detail.ProductVariant.Product.Name,
                        Quantity = detail.Quantity,
                        Price = detail.Price,
                        Description = detail.ProductVariant.Product.Description,
                        ImagePath = detail.ProductVariant.Path.Split('|').First(),
                        Colour = dbContext.Colour.First(x => x.Id == detail.ProductVariant.ColourId).Name,
                        Size = dbContext.Size.First(x => x.Id == detail.ProductVariant.SizeId).Name
                    });
                }

                response.Orders.Add(new OrderDetailDto
                {
                    OrderId = item.Id,
                    OrderDate = item.CreatedAt,
                    OrderNo = item.OrderNo,
                    PaymentMode = item.PaymentMode,
                    ShippingAddress = item.ShippingAddress,
                    Status = item.Status,
                    TotalPrice = item.TotalPrice,
                    TotalItems = item.TotalItems,
                    ItemDetails = itemDetails
                });
            }
        }
        return response;
    }



//public async Task<OrderDetailDto> GetById(int orderId)
//{
//    OrderDetailDto orderDetail = new OrderDetailDto();
//    var orders = dbContext.OrderDetail.Include(x => x.ProductVariant).Include(x => x.Order).ThenInclude(x => x.Customer).Where(x => x.OrderId == orderId).ToList();
//    if (orders == null)
//        return null;
//    orderDetail.CustomerId = orders.FirstOrDefault().Order.Customer.Id;
//    orderDetail.CustomerName = orders.FirstOrDefault().Order.Customer.Name;
//    orderDetail.OrderId = orderId;
//    orderDetail.ItemDetails = new List<ItemDetail>();
//    foreach (var order in orders)
//    {
//        orderDetail.ItemDetails.Add(new ItemDetail()
//        {
//            Id = order.Id,
//            Quantity = order.Quantity,
//            ImagePath = order.ProductVariant.Path.Split('|').First(),
//            Price = order.Price
//        });
//    }
//    return orderDetail;
//}
public async Task<InvoiceDto> GetInvoice(int customerId, int orderId)
{
    InvoiceDto invoice = new InvoiceDto();
    var orders = dbContext.OrderDetail.Include(x => x.ProductVariant).Include(x => x.Order).ThenInclude(x => x.Customer).Where(x => x.OrderId == orderId && x.Order.Status == Provider.Enums.OrderStatus.Delivered && x.Order.Customer.Id == customerId).ToList();
    if (orders.Count == 0)
        return null;
    invoice.CustomerName = orders.FirstOrDefault().Order.Customer.Name;
    invoice.CustomerAddress = orders.FirstOrDefault().Order.ShippingAddress;
    invoice.OrderNo = orders.FirstOrDefault().Order.OrderNo;
    invoice.OrderDate = orders.FirstOrDefault().Order.CreatedAt;
    invoice.Orders = new List<OrderInvoice>();
    foreach (var order in orders)
    {
        invoice.Orders.Add(new OrderInvoice()
        {
            Quantity = order.Quantity,
            Price = order.Price,
            ProductVariantId = order.ProductVariantId,
            Total = order.Quantity * order.Price,
            Size = dbContext.Size.FirstOrDefault(x => x.Id == order.ProductVariant.SizeId).Name,
            Colour = dbContext.Colour.FirstOrDefault(x => x.Id == order.ProductVariant.ColourId).Name,
            ProductName = dbContext.Product.FirstOrDefault(x => x.Id == order.ProductVariant.ProductId).Name
        });
    }
    invoice.SummaryTotal = orders.FirstOrDefault().Order.TotalPrice;
    invoice.SummaryTax = (invoice.SummaryTotal * 5) / 100;
    invoice.SummaryNetTotal = invoice.SummaryTotal + invoice.SummaryTax;
    return invoice;

}
public async Task<Tuple<byte[], string>> GenerateInvoice(int customerId, int orderId)
{
    var invoice = await GetInvoice(customerId, orderId);
    if (invoice == null)
        return null;
    var document = new PdfDocument();
    string htmlcontent = "<div style='width:100%; text-align:center'>";

    htmlcontent += "<h2> Invoice No:" + invoice.OrderNo + " & Invoice Date:" + invoice.OrderDate + "</h2>";
    htmlcontent += "<h3> Customer : " + invoice.CustomerName + "</h3>";
    htmlcontent += "<p>" + "Address : " + invoice.CustomerAddress + "</p>";
    htmlcontent += "<h3> Contact : 09796709306 & Email :admin@gmail.com </h3>";
    htmlcontent += "<div>";

    htmlcontent += "<table style ='width:100%; border: 1px solid #000'>";
    htmlcontent += "<thead style='font-weight:bold'>";
    htmlcontent += "<tr>";
    htmlcontent += "<td style='border:1px solid #000'> Product Variant Code </td>";
    htmlcontent += "<td style='border:1px solid #000'> Product Name </td>";
    htmlcontent += "<td style='border:1px solid #000'> Size </td>";
    htmlcontent += "<td style='border:1px solid #000'> Colour </td>";
    htmlcontent += "<td style='border:1px solid #000'>Qty</td>";
    htmlcontent += "<td style='border:1px solid #000'>Price</td >";
    htmlcontent += "<td style='border:1px solid #000'>Total</td>";
    htmlcontent += "</tr>";
    htmlcontent += "</thead >";

    htmlcontent += "<tbody>";
    invoice.Orders.ForEach(item =>
        {
            htmlcontent += "<tr>";
            htmlcontent += "<td>" + item.ProductVariantId + "</td>";
            htmlcontent += "<td>" + item.ProductName + "</td>";
            htmlcontent += "<td>" + item.Size + "</td >";
            htmlcontent += "<td>" + item.Colour + "</td>";
            htmlcontent += "<td>" + item.Quantity + "</td>";
            htmlcontent += "<td>" + item.Price + "</td>";
            htmlcontent += "<td> " + item.Total + "</td >";
            htmlcontent += "</tr>";
        });

    htmlcontent += "</tbody>";

    htmlcontent += "</table>";
    htmlcontent += "</div>";

    htmlcontent += "<div style='text-align:right'>";
    htmlcontent += "<h1> Summary Info </h1>";
    htmlcontent += "<table style='border:1px solid #000;float:right' >";
    htmlcontent += "<tr>";
    htmlcontent += "<td style='border:1px solid #000'> Summary Total </td>";
    htmlcontent += "<td style='border:1px solid #000'> Summary Tax </td>";
    htmlcontent += "<td style='border:1px solid #000'> Summary NetTotal </td>";
    htmlcontent += "</tr>";

    htmlcontent += "<tr>";
    htmlcontent += "<td style='border: 1px solid #000'> " + invoice.SummaryTotal + " </td>";
    htmlcontent += "<td style='border: 1px solid #000'>" + invoice.SummaryTax + "</td>";
    htmlcontent += "<td style='border: 1px solid #000'> " + invoice.SummaryNetTotal + "</td>";
    htmlcontent += "</tr>";

    htmlcontent += "</table>";
    htmlcontent += "</div>";

    htmlcontent += "</div>";
    PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);
    byte[]? response = null;
    using (MemoryStream stream = new MemoryStream())
    {
        document.Save(stream);
        response = stream.ToArray();
    }
    string Filename = "sample.pdf";
    return new Tuple<byte[], string>(response, Filename);
}



}
