namespace eShop.Application.Features.Users.Models.Requests
{
    public class UpdateUserRequest
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
