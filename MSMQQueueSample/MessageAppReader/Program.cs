using MessageApp1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MessageAppReader
{
    class Program
    {
        private static string Queue = @".\Private$\Lib-Queue";

        static void Main(string[] args)
        {
            MessageQueue messageQueue = null;
            if (MessageQueue.Exists(Queue))
            {
                messageQueue = new MessageQueue(Queue);

                try
                {
                    while (true)
                    {
                        try
                        {
                            /*Check if there exist any message in queue.*/
                            messageQueue.Peek(TimeSpan.FromSeconds(2));
                        }
                        catch (MessageQueueException)
                        {
                            break;
                        }

                        var message = messageQueue.Receive();

                        /*We didnt use any custom serializer and were using Binary serializer so specify that here so that the message can be deserialized back*/
                        message.Formatter = new BinaryMessageFormatter();
                        var messageEncoded = message.Body.ToString();

                        /*We did a custom serialization. So deserialize the message content which we have received.*/
                        object Json = DeserializeObject(messageEncoded);

                        /*We also applied json serialization. So deserialize.*/
                        var customMessage = JsonConvert.DeserializeObject<CustomMessage>(Json.ToString());
                        Console.WriteLine(Json);
                    }
                }
                finally
                {
                    messageQueue.Dispose();
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }

        public static object DeserializeObject(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}
