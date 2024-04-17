namespace Payment.bKash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IPayment paymentService;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IPayment paymentService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            this.paymentService = paymentService;
        }


        [HttpPost("/api/GrantToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GrantToken()
        {
            // Create HttpClient instance from the factory
            var httpClient = _httpClientFactory.CreateClient();

            // API endpoint
            var apiUrl = "https://tokenized.pay.bka.sh/v1.2.0-beta/tokenized/checkout/token/grant";

            // Request body
            var requestBody = new
            {
                app_key = _configuration["Bkash:app_key"],
                app_secret = _configuration["Bkash:app_secret"]
            };

            // Adding headers
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Add("username", _configuration["Bkash:username"]);
            httpClient.DefaultRequestHeaders.Add("password", _configuration["Bkash:password"]);

            // Send POST request and retrieve response
            var response = await httpClient.PostAsJsonAsync(apiUrl, requestBody);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read and return response content
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            else
            {
                // Return error status code and reason phrase
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }



        [HttpPost("/api/CreatePayment")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePayment(PaymentCreateVm model)
        {
            // API endpoint
            string apiUrl = "https://tokenized.pay.bka.sh/v1.2.0-beta/tokenized/checkout/create";

            string successUrl = _configuration["Bkash:success_url"];

            // Request body
            string requestBody = @$"{{
                ""mode"": ""0011"",
                ""payerReference"": ""{model.senderBkashNo}"",
                ""callbackURL"": ""{successUrl}"",
                ""amount"": ""{model.amount}"",
                ""currency"": ""BDT"",
                ""intent"": ""sale"",
                ""merchantInvoiceNumber"": ""TR43543543""
            }}";

            // Adding headers
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", model.token);
            httpClient.DefaultRequestHeaders.Add("X-App-Key", _configuration["Bkash:app_key"]);

            // Send POST request
            var response = await httpClient.PostAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var res = JsonSerializer.Deserialize<BkashPaymentResponseViewModel>(responseContent);


                    var data = new PaymentLog
                    {
                        Id = 0,
                        paymentID = res.paymentID,
                        status = "Pending",
                        trxNo = "TR43543543",
                        amount = model.amount,
                        token = model.token
                    };

                    var result = await paymentService.SavePaymentLogs(data);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }

                // Read and return response content

                return Ok(responseContent);
            }
            else
            {
                // Return error status code and reason phrase
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }


        [HttpGet("/api/ExecutePayment")]
        [AllowAnonymous]
        public async Task<string> ExecutePayment(string paymentId)
        {
            var paymentData = await paymentService.GetPaymentLogByPaymentId(paymentId);


            // API endpoint
            string apiUrl = "https://tokenized.pay.bka.sh/v1.2.0-beta/tokenized/checkout/execute";

            // Request body
            string requestBody = $"{{ \"paymentID\": \"{paymentId}\" }}";

            // Adding headers
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", paymentData.token);
            httpClient.DefaultRequestHeaders.Add("X-App-Key", _configuration["Bkash:app_key"]);

            // Send POST request
            var response = await httpClient.PostAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {

                // Read and return response content
                string responseContent = await response.Content.ReadAsStringAsync();

                paymentData.status = "Success";

                await paymentService.SavePaymentLogs(paymentData);


                return paymentData.status;
            }
            else
            {
                // Throw an exception if the request failed
                throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }



        [HttpGet("/api/GetPaymentInfoByPaymentId")]
        public async Task<IActionResult> GetPaymentInfoByPaymentId(string paymentId)
        {
            var data = await paymentService.GetPaymentLogByPaymentId(paymentId);

            return Ok(data);
        }


        
        [HttpGet("/api/payment_success")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSuccess(string paymentID)
        {
            var data = await ExecutePayment(paymentID);

            if (data == "Success")
            {
                return Ok(new { Message = "Payment Success", PaymentId = paymentID });
            }
            else
            {
                return Ok(new { Message = "Payment Failed", PaymentId = paymentID });
            }
        }
    }
}

