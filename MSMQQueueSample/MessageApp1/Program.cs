using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MessageApp1
{
    class Program
    {
        private static string Queue = @".\Private$\Lib-Queue";

        static void Main(string[] args)
        {
            MessageQueue messageQueue = null;
            Console.Write("Type your message; Prepend with ~ if of high priority: ");
            var message = Console.ReadLine();

            if (MessageQueue.Exists(Queue)) {
                messageQueue = new MessageQueue(Queue);
            }
            else {
                messageQueue = MessageQueue.Create(Queue);
            }

            // set as persistant message. This will be brought back to queue if in case machine restarted.
            messageQueue.DefaultPropertiesToSend.Recoverable = true;

            /* here we want to have a custom serializer. But we are keeping it simple by using a default formatter and serializing it seperately.
                    you could specify a csutom serializer by providing a class which implements IMessageFormatter and assign it to message.Formatter
                     */
            messageQueue.Formatter = new BinaryMessageFormatter();

            string category = "Low";
            if (message.StartsWith("~"))
            {
                category = "High";

                /*Set priority. Queue will process message based on the priority.*/
                messageQueue.DefaultPropertiesToSend.Priority = MessagePriority.High;
            }

            CustomMessage msmqMessage = new CustomMessage { Category = category, Priority = category, Message = message.TrimStart('~') };

            try
            {
                /*Here we do some custom serialization and send it to queue.*/
                messageQueue.Send(SerializeObject(JsonConvert.SerializeObject(msmqMessage)), "Custom Label");

            }
            finally
            {
                messageQueue.Dispose();
            }
        }
        
        public static string SerializeObject(object o)
        {
            if (!o.GetType().IsSerializable)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, o);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
