namespace EShop_DB.Common.Constants;

public static class ErrorMessages
{
    public static class Universal
    {
        public static string AlreadyExistsEmail(string entity) => $"{entity} with the same email is already exist";

        public static string AlreadyExistsId(string entity, Guid id) => $"{entity} with the same id ({id}) is already exist";
        
        public static string NotFoundWithId(string entity, Guid id) => $"{entity} with id = \"{id}\" was not found";
    }

    public static class Seller
    {
        public const string AlreadyExistsEmailOrName = "Seller with the same email or name is already exist";
    }

    public static class Recipient
    {
        public const string AlreadyExistsPhone = "Recipient with the same phone number is already exist";
    }

    public static class Product
    {
        public const string AlreadyExistsNameSeller = "A product with this name already exists from this seller";
    }
}