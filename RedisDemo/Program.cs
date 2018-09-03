using System;
using System.Text;
using ServiceStack.Redis;

namespace RedisDemo
{
    public class Program
    {
        public static void Main()
        {
            DemoRedisNativeClient();
            DemoRedisClient();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void DemoRedisNativeClient()
        {
            const string key = "urn:messages:1";

            using (IRedisNativeClient redisNativeClient = new RedisClient())
            {
                redisNativeClient.Set(key, Encoding.UTF8.GetBytes("Hello World"));
            }

            using (IRedisNativeClient redisNativeClient = new RedisClient())
            {
                byte[] bytes = redisNativeClient.Get(key);
                string result = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"Message = {result}");
            }
        }

        private static void DemoRedisClient()
        {
            const string customerListKey = "urn:customerList";

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisList customerList = redisClient.Lists[customerListKey];
                customerList.Clear();
                customerList.Add("Joe");
                customerList.Add("Mary");
                customerList.Add("Bob");
                customerList.Add("Frank");
            }

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisList customerList = redisClient.Lists[customerListKey];

                foreach (string customerName in customerList)
                {
                    Console.WriteLine($"Customer: {customerName}");
                }
            }
        }
    }
}
