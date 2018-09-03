using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RedisDemo
{
    public class Program
    {
        public static void Main()
        {
            DemoRedisNativeClient();
            Console.WriteLine("------------------------------");
            DemoRedisClient();
            Console.WriteLine("------------------------------");
            DemoRedisTypedClient();
            Console.WriteLine("------------------------------");
            DemoRedisTransaction();
            Console.WriteLine("------------------------------");
            //DemoPublishSubscribe();
            Console.WriteLine("------------------------------");

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

        private static void DemoRedisTypedClient()
        {
            long lastCustomerId;

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<Customer> customerClient = redisClient.GetTypedClient<Customer>();
                Customer customer = new Customer
                {
                    Id = customerClient.GetNextSequence(),
                    Name = "Frank",
                    Address = "123 Main Street",
                    Orders = new List<Order>
                    {
                        new Order {OrderNumber = "123", OrderTotal = 100.00m},
                        new Order {OrderNumber = "123", OrderTotal = 100.00m}
                    }
                };
                Customer savedCustomer = customerClient.Store(customer);
                lastCustomerId = savedCustomer.Id;
            }

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<Customer> customerClient = redisClient.GetTypedClient<Customer>();
                Customer customer = customerClient.GetById(lastCustomerId);
                Console.WriteLine($"Customer = {customer.Id}, {customer.Name}");
            }
        }

        private static void DemoRedisTransaction()
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                const string key = "abc";
                IRedisTransaction transaction = redisClient.CreateTransaction();
                transaction.QueueCommand(c => c.Set(key, 1));
                transaction.QueueCommand(c => c.Increment(key, 1));
                transaction.Commit();

                int result = redisClient.Get<int>(key);
                Console.WriteLine($"Result = {result}");
            }
        }

        private static void DemoPublishSubscribe()
        {
            using (IRedisClient redisClient = new RedisClient())
            {
                redisClient.PublishMessage("debug", "Published message");
            }

            using (IRedisClient redisClient = new RedisClient())
            {
                IRedisSubscription subscription = redisClient.CreateSubscription();
                subscription.OnMessage = (c, m) => Console.WriteLine($"Message from channel {c} = {m}");
                // Console waits here
                subscription.SubscribeToChannels("news");
            }
        }
    }
}
