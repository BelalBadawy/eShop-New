namespace eShop.Application.Features.Users.Models.Requests
{
    public class ChangeUserStatusRequest
    {
        public string UserId { get; set; }
        public bool ActivateOrDeactivate { get; set; }
    }
}
