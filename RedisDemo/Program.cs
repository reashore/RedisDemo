using System;
using System.Text;
using ServiceStack.Redis;

namespace RedisDemo
{
    public class Program
    {
        public static void Main()
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

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
