using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using TodolistDemo.Domain;
using TodolistDemo.ViewModel;

namespace TodolistDemo.LambdaFunctions
{
    public class GetTodo
    {
        private readonly IDynamoDBContext _dbContext;

        public GetTodo()
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(TodoItem)] = new Amazon.Util.TypeMapping(typeof(TodoItem), Functions.Table);
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var todoItemsScan = _dbContext.ScanAsync<TodoItem>(null);
            var pageItems = await todoItemsScan.GetNextSetAsync();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(pageItems.Select(x=>new TodoItemViewModel(x))),
                Headers = Functions.LambdaHeader
            };

            return response;
        }
    }
}
