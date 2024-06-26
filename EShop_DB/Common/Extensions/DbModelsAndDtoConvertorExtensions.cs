using EShop_DB.Models.MainModels;
using EShop_DB.Models.SecondaryModels;
using SharedLibrary.Models.DtoModels.MainModels;
using SharedLibrary.Models.DtoModels.SecondaryModels;
using SharedLibrary.Models.Enums;

namespace EShop_DB.Common.Extensions;

public static class DbModelsAndDtoConvertorExtensions
{
    #region User
    
    public static UserDTO ToUserDto(this User user)
    {
        return new UserDTO(
            user.Name,
            user.LastName,
            user.PhoneNumber,
            user.Email,
            patronymic: user.Patronymic,
            passwordHash: user.PasswordHash,
            salt: user.Salt
        )
        {
            UserDtoId = user.UserId,
            
            RoleDtoId = user.RoleId,
            RoleDto = user.Role?.ToRoleDto(),
            
            SellerDtoId = user.SellerId,
            SellerDto = user.Seller?.ToSellerDto(), 
            
            OrdersDto = user.Orders?.Select(o => o.ToOrderDto()).ToList(),
                
            RecipientDto = user.Recipient?.ToRecipientDto(),
            DeliveryAddressDto = user.DeliveryAddress?.ToDeliveryAddressDto(),
        };
    }
    public static User ToUser(this UserDTO userDto)
    {
        return new User(
            userDto.Name,
            userDto.LastName,
            userDto.PasswordHash ?? "",
            userDto.Salt ?? "",
            userDto.PhoneNumber,
            userDto.Email,
            userDto.Patronymic
        )
        {
            UserId = userDto.UserDtoId,
            
            RoleId = userDto.RoleDtoId,
            Role = userDto.RoleDto?.ToRole(),
            
            SellerId = userDto.SellerDtoId,
            Seller = userDto.SellerDto?.ToSeller(),
            
            Orders = userDto.OrdersDto?.Select(o => o.ToOrder()).ToList(),
            
            Recipient = userDto.RecipientDto?.ToRecipient(),
            DeliveryAddress = userDto.DeliveryAddressDto?.ToDeliveryAddress()
        };
    }
    
    #endregion
    
    #region Order
    
    public static OrderDTO ToOrderDto(this Order order)
    {
        return new OrderDTO()
        {
            OrderDtoId = order.OrderId,
            AnonymousToken = order.AnonymousToken,
            
            UserDtoId = order.UserId,
            UserDto = order.User?.ToUserDto(),
            
            OrderEventsDto = order.OrderEvents?.Select(oi => oi.ToOrderEventDto()).ToList(),
            OrderItemsDto = order.OrderItems?.Select(oi => oi.ToOrderItemDto()).ToList(),
            
            RecipientDto = order.Recipient?.ToRecipientDto(),
            DeliveryAddressDto = order.DeliveryAddress?.ToDeliveryAddressDto()
        };
    }
    public static Order ToOrder(this OrderDTO orderDto)
    {
        return new Order()
        {
            OrderId = orderDto.OrderDtoId,
            
            AnonymousToken = orderDto.AnonymousToken,
            
            UserId = orderDto.UserDtoId,
            User = orderDto.UserDto?.ToUser(),
            
            OrderEvents = orderDto.OrderEventsDto?.Select(oe => oe.ToOrderEvent()).ToList(),
            OrderItems = orderDto.OrderItemsDto?.Select(oi => oi.ToOrderItem()).ToList(),
            
            Recipient = orderDto.RecipientDto?.ToRecipient(),
            DeliveryAddress = orderDto.DeliveryAddressDto?.ToDeliveryAddress()
        };
    }
    
    #endregion
    
    #region OrderItem
    
    public static OrderItemDTO ToOrderItemDto(this OrderItem orderItem)
    {
        return new OrderItemDTO(orderItem.OrderId, orderItem.ProductId, orderItem.Quantity)
        {
            OrderItemDtoId = orderItem.OrderItemId,
            
            OrderDto = orderItem.Order?.ToOrderDto(),
            ProductDto = orderItem.Product?.ToProductDto(),
        };
    }
    public static OrderItem ToOrderItem(this OrderItemDTO orderItemDto)
    {
        return new OrderItem(orderItemDto.OrderDtoId, orderItemDto.ProductDtoId, orderItemDto.Quantity)
        {
            OrderItemId = orderItemDto.OrderItemDtoId,
            
            Order = orderItemDto.OrderDto?.ToOrder(),
            Product = orderItemDto.ProductDto?.ToProduct(),
        };
    }
    
    #endregion
    
    #region OrderEvent
    
    public static OrderEventDTO ToOrderEventDto(this OrderEvent orderEvent)
    {
        return new OrderEventDTO(orderEvent.OrderId, orderEvent.Stage)
        {
            OrderEventDtoId = orderEvent.OrderEventId,
            
            EventTime = orderEvent.EventTime,
            OrderDto = orderEvent.Order?.ToOrderDto(),
        };
    }
    public static OrderEvent ToOrderEvent(this OrderEventDTO orderEventDto)
    {
        return new OrderEvent(orderEventDto.OrderDtoId, orderEventDto.Stage)
        {
            OrderEventId = orderEventDto.OrderEventDtoId,
            
            EventTime = orderEventDto.EventTime,
            Order = orderEventDto.OrderDto?.ToOrder(),
        };
    }
    
    #endregion
    
    #region Product
    
    public static ProductDTO ToProductDto(this Product product)
    {
        return new ProductDTO(
            product.Name,
            product.Description,
            product.PricePerUnit,
            product.WeightInGrams,
            product.SellerId,
            product.ImageUrl)
        {
            InStock = product.InStock,
            
            ProductDtoId = product.ProductId,
            SellerDto = product.Seller?.ToSellerDto(),
            
            OrderItemsDto = product.OrderItems?.Select(oi => oi.ToOrderItemDto()).ToList(),
        };
    }
    public static Product ToProduct(this ProductDTO productDto)
    {
        return new Product(
            productDto.Name,
            productDto.Description,
            productDto.PricePerUnit,
            productDto.WeightInGrams,
            productDto.InStock ?? -1,
            productDto.SellerDtoId,
            productDto.ImageUrl)
        {
            ProductId = productDto.ProductDtoId,
            Seller = productDto.SellerDto?.ToSeller(),
            
            OrderItems = productDto.OrderItemsDto?.Select(oi => oi.ToOrderItem()).ToList(),
        };
    }
    
    #endregion
    
    #region Role
    
    public static RoleDTO ToRoleDto(this Role role)
    {
        return new RoleDTO(role.RoleTag)
        {
            RoleDtoId = role.RoleId,
            
            UsersDto = role.Users?.Select(u => u.ToUserDto()).ToList() ?? new(),
        };
    }
    public static Role ToRole(this RoleDTO roleDto)
    {
        return new Role(roleDto.RoleTag)
        {
            RoleId = roleDto.RoleDtoId,
            
            Users = roleDto.UsersDto?.Select(u => u.ToUser()).ToList() ?? new(),
        };
    }
    
    #endregion
    
    #region Seller
    
    public static SellerDTO ToSellerDto(this Seller seller)
    {
        return new SellerDTO(
            seller.CompanyName,
            seller.ContactNumber,
            seller.EmailAddress,
            seller.CompanyDescription,
            seller.ImageUrl,
            seller.AdditionNumber)
        {
            SellerDtoId = seller.SellerId,
            
            ProductsDto = seller.Products?.Select(pr => pr.ToProductDto()).ToList(),
            UsersDto = seller.Users?.Select(u => u.ToUserDto()).ToList(),
        };
    }
    public static Seller ToSeller(this SellerDTO sellerDto)
    {
        return new Seller(
            sellerDto.CompanyName,
            sellerDto.ContactNumber,
            sellerDto.EmailAddress,
            sellerDto.CompanyDescription,
            sellerDto.ImageUrl,
            sellerDto.AdditionNumber)
        {
            SellerId = sellerDto.SellerDtoId,
            
            Products = sellerDto.ProductsDto?.Select(pr => pr.ToProduct()).ToList(),
            Users = sellerDto.UsersDto?.Select(u => u.ToUser()).ToList(),
        };
    }
    
    #endregion
    
    #region DeliveryAddress
    
    public static DeliveryAddressDTO ToDeliveryAddressDto(this DeliveryAddress deliveryAddress)
    {
        return new DeliveryAddressDTO(
            deliveryAddress.City,
            deliveryAddress.Street,
            deliveryAddress.House,
            deliveryAddress.Apartment,
            deliveryAddress.Floor,
            deliveryAddress.UserId,
            deliveryAddress.OrderId)
        {
            DeliveryAddressDtoId = deliveryAddress.DeliveryAddressId,
            
            UserDto = deliveryAddress.User?.ToUserDto(),
            OrderDto = deliveryAddress.Order?.ToOrderDto(),
        };
    }
    public static DeliveryAddress ToDeliveryAddress(this DeliveryAddressDTO deliveryAddressDto)
    {
        return new DeliveryAddress(
            deliveryAddressDto.City,
            deliveryAddressDto.Street,
            deliveryAddressDto.House,
            deliveryAddressDto.Apartment,
            deliveryAddressDto.Floor,
            deliveryAddressDto.UserDtoId,
            deliveryAddressDto.OrderDtoId)
        {
            DeliveryAddressId = deliveryAddressDto.DeliveryAddressDtoId,
            
            User = deliveryAddressDto.UserDto?.ToUser(),
            Order = deliveryAddressDto.OrderDto?.ToOrder(),
        };
    }
    
    #endregion
    
    #region Recipient
    
    public static RecipientDTO ToRecipientDto(this Recipient recipient)
    {
        return new RecipientDTO(
            recipient.Name,
            recipient.LastName,
            recipient.PhoneNumber,
            recipient.Patronymic,
            recipient.UserId,
            recipient.OrderId)
        {
            RecipientDtoId = recipient.RecipientId,
            
            UserDto = recipient.User?.ToUserDto(),
            OrderDto = recipient.Order?.ToOrderDto(),
        };
    }
    public static Recipient ToRecipient(this RecipientDTO recipientDto)
    {
        return new Recipient(
            recipientDto.Name,
            recipientDto.LastName,
            recipientDto.PhoneNumber,
            recipientDto.Patronymic,
            recipientDto.UserDtoId,
            recipientDto.OrderDtoId)
        {
            RecipientId = recipientDto.RecipientDtoId,
            
            User = recipientDto.UserDto?.ToUser(),
            Order = recipientDto.OrderDto?.ToOrder(),
        };
    }
    
    #endregion
}