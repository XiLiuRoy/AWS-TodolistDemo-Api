using System.Collections.Generic;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TodolistDemo
{
    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }

        public static readonly string Table = "TodoList";
        public static readonly string QueryParameterId = "Id";
        public static Dictionary<string, string> LambdaHeader = new Dictionary<string, string>
        {
            {
                "Content-Type",
                "application/json"
            },
            {
                "Access-Control-Allow-Origin",
                "*"
            },
            {
                "Cache-Control",
                "no-store, no-cache, must-revalidate, proxy-revalidate"
            }
        };
    }
}
