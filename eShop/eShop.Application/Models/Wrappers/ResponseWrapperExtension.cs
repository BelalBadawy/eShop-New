using System.Text.Json;
using System.Text.Json.Serialization;

namespace eShop.Application.Models
{
    public static class ResponseWrapperExtension
    {
        public static async Task<ResponseWrapper<T>> ToResponse<T>(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper<T>>(responseAsString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

            return responseObject;
        }

        public static async Task<ResponseWrapper> ToResponse(this HttpResponseMessage responseMessage)
        {
            var responseAsString = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(responseAsString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

            return responseObject;
        }
    }
}
