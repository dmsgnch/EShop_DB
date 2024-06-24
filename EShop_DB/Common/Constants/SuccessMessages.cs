namespace EShop_DB.Common.Constants;

public static class SuccessMessages
{
    public static class UniversalResponse
    {
        public static string Updated(string entity) => $"{entity} has been successfully updated";
        public static string Deleted(string entity) => $"{entity} has been successfully deleted";
        public static string Created(string entity) => $"{entity} has been successfully created";
    }
}