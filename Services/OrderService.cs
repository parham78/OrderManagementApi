using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly OrderManagementDbContext _context;

    public OrderService(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<OrderResponseDto>> GetAll(
        int page,
        int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var query = _context.Orders
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,

                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .ToListAsync();

        return new PagedResultDto<OrderResponseDto>
        {
            Items = orders,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }

    public async Task<OrderResponseDto> GetById(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,

                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        return order;
    }

    public async Task<OrderResponseDto> Create(
        CreateOrderRequestDto dto)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            throw new CustomerNotFoundException(
                $"Customer {dto.CustomerId} was not found.");
        }

        var productIds = dto.Items
            .Select(i => i.ProductId)
            .ToList();

        if (productIds.Distinct().Count() != productIds.Count)
        {
            throw new BadRequestException(
                "The same product cannot appear twice in an order.");
        }

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var itemDto in dto.Items)
        {
            if (!products.ContainsKey(itemDto.ProductId))
            {
                throw new ProductNotFoundException(
                    $"Product {itemDto.ProductId} was not found.");
            }
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        decimal totalPrice = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = products[itemDto.ProductId];

            if (product.Stock < itemDto.Quantity)
            {
                throw new InsufficientStockException(
                    $"Not enough stock for product {product.Name}.");
            }

            product.Stock -= itemDto.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);

            totalPrice +=
                itemDto.Quantity * product.Price;
        }

        order.TotalPrice = totalPrice;

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "One or more products were modified by another request.");
        }

        return await GetById(order.Id);
    }

    public async Task<OrderResponseDto> Update(
        int id,
        UpdateOrderRequestDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            throw new CustomerNotFoundException(
                $"Customer {dto.CustomerId} was not found.");
        }

        var newProductIds = dto.Items
            .Select(i => i.ProductId)
            .ToList();

        if (newProductIds.Distinct().Count() != newProductIds.Count)
        {
            throw new BadRequestException(
                "The same product cannot appear twice in an order.");
        }

        // We need both:
        // old products -> restore their stock
        // new products -> subtract new quantities
        var allProductIds = order.OrderItems
            .Select(oi => oi.ProductId)
            .Concat(newProductIds)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p => allProductIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validate products from the old order.
        foreach (var oldItem in order.OrderItems)
        {
            if (!products.ContainsKey(oldItem.ProductId))
            {
                throw new ProductNotFoundException(
                    $"Original product {oldItem.ProductId} was not found.");
            }
        }

        // Validate products in the new request.
        foreach (var itemDto in dto.Items)
        {
            if (!products.ContainsKey(itemDto.ProductId))
            {
                throw new ProductNotFoundException(
                    $"Product {itemDto.ProductId} was not found.");
            }
        }

        // Restore stock from the old order first.
        foreach (var oldItem in order.OrderItems)
        {
            var oldProduct = products[oldItem.ProductId];

            oldProduct.Stock += oldItem.Quantity;
        }

        var oldItems = order.OrderItems.ToList();

        _context.OrderItems.RemoveRange(oldItems);

        order.OrderItems.Clear();

        decimal newTotalPrice = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = products[itemDto.ProductId];

            if (product.Stock < itemDto.Quantity)
            {
                throw new InsufficientStockException(
                    $"Not enough stock for product {product.Name}.");
            }

            product.Stock -= itemDto.Quantity;

            var newOrderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(newOrderItem);

            newTotalPrice +=
                itemDto.Quantity * product.Price;
        }

        order.CustomerId = dto.CustomerId;
        order.TotalPrice = newTotalPrice;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "One or more products were modified by another request.");
        }

        return await GetById(order.Id);
    }

    public async Task Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        var productIds = order.OrderItems
            .Select(oi => oi.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in order.OrderItems)
        {
            var product = products.GetValueOrDefault(
                item.ProductId);

            if (product is null)
            {
                throw new ProductNotFoundException(
                    $"Product {item.ProductId} was not found.");
            }

            product.Stock += item.Quantity;
        }

        _context.Orders.Remove(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "One or more products were modified by another request.");
        }
    }
}