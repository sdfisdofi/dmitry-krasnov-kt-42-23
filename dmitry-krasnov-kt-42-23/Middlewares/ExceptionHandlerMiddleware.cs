using System.Net;

namespace dmitry_krasnov_kt_42_23.Middlewares
{
    // Middleware - промежуточный слой между клиентом и контроллером.
    // Каждый запрос проходит через него. Если где-то дальше (в контроллере, сервисе, БД)
    // возникло необработанное исключение, оно "всплывет" сюда и будет перехвачено
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Передаем запрос дальше по цепочке (следующему middleware / контроллеру)
                await _next(context);
            }
            catch (Exception exception)
            {
                // Записываем ошибку в лог-файл (NLog) вместе со всем стектрейсом
                _logger.LogError(exception, "Необработанное исключение: {Message}", exception.Message);

                var httpResponse = context.Response;
                httpResponse.ContentType = "application/json";

                var responseModel = new ResponseModel<object>
                {
                    Succeeded = false,
                    Message = exception.Message
                };

                switch (exception)
                {
                    default:
                        httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;

                        // Внутренняя ошибка есть не всегда (например у ошибок БД она обычно есть)
                        if (exception.InnerException != null)
                        {
                            responseModel.Errors = new List<string> { exception.InnerException.Message };
                        }
                        break;
                }

                await httpResponse.WriteAsJsonAsync(responseModel);
            }
        }
    }

    // Единый формат ответа об ошибке
    public class ResponseModel<T>
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }

        public ResponseModel()
        {
        }

        public ResponseModel(T data, string? message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        public ResponseModel(string message)
        {
            Succeeded = true;
            Message = message;
        }
    }
}
