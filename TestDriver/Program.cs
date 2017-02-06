using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestDriver
{

    class Program
    {
        public const string FPATH = @"C:\Users\Omar\Desktop\Text Documents\orders.txt";
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            //**TESTING**//
            //var order = "095b4e2e7938360c286b7c7800004073";
            //var ordertwo = "095b3f5b7938360c286b7c6900004073";
            //printExternalID(order);
            //printExternalID(ordertwo);
            //List<String> usedGUID = new List<String>();
            //usedGUID.Add("GJ89S");
            //Console.Write(usedGUID.Contains("GJ89S"));

            //Initialize Collisions Counter
            int collisions = 0;



            //Run & Display First Method
            Console.WriteLine("First GUID # Generator (Current Generator)");
            Console.WriteLine("------------------------------------------");
            collisions = runRNGCollisions(firstProgram);
            Console.WriteLine("Number of Collisions: " + collisions + '\n');

            //Run & Display Second Method
            Console.WriteLine("Second GUID # Generator (Time Seed)");
            Console.WriteLine("------------------------------------------");
            collisions = runRNGCollisions(secondProgram);
            Console.WriteLine("Number of Collisions: " + collisions + '\n');

            //Run & Display Third Method
            Console.WriteLine("Third GUID # Generator (Secure Seed)");
            Console.WriteLine("------------------------------------------");
            collisions = runRNGCollisions(thirdProgram);
            Console.WriteLine("Number of Collisions: " + collisions + '\n');

            //**INITIALIZATION-Only Done Once**//
            //correctFile();

            Console.WriteLine("\n\nDone");
            Console.ReadLine();
        }


        private static int runRNGCollisions(Func<String, String> programFunction)
        {
            int collisionCounter = 0;
            List<String> usedGUID = new List<String>();
            if (File.Exists(FPATH))
            {
                using (StreamReader sr = File.OpenText(FPATH))
                {
                    string tmp = "";
                    String createdGUID = "";
                    while((tmp = sr.ReadLine()) != null)
                    {
                        //Generate new GUID
                        createdGUID = programFunction(tmp);

                        //Determine if Collision Occured
                        if (usedGUID.Contains(createdGUID))
                        {
                            collisionCounter++;
                        }else
                        {
                            usedGUID.Add(createdGUID);
                        }
                    }

                    sr.Close();
                }
            }else
            {
                Console.WriteLine("Failed to Open File");
            }


            return collisionCounter;
        }






        //Current Random Algorithm
        private static String firstProgram(String order)
        {
            var orderId = "1";
            // Turns the 32 GUID into a 16 GUID + 1 + 3
            var letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //Console.WriteLine("Order: " + order);
            for (var i = 0; i < order.Length; i += 2)
            {
                var total = letters.IndexOf(order[i]);
                //Console.WriteLine("Index: " + i);
                total += letters.IndexOf(order[i + 1]);
                var ind = total % letters.Length;
                orderId += letters[ind];
            }



            orderId += order[order.Length - 3];
            orderId += order[order.Length - 2];
            orderId += order[order.Length - 1];

            return orderId;

            //**Testing**//
            //Console.WriteLine("Order: " + orderId);
        }


        //Current Random Algorithm Plus randomly Generate last 3 characters based on time seed
        private static String secondProgram(String order)
        {
            var orderId = "1";
            // Turns the 32 GUID into a 16 GUID + 1 + 3
            var letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //Console.WriteLine("Order: " + order);
            for (var i = 0; i < order.Length; i += 2)
            {
                var total = letters.IndexOf(order[i]);
                //Console.WriteLine("Index: " + i);
                total += letters.IndexOf(order[i + 1]);
                var ind = total % letters.Length;
                orderId += letters[ind];
            }



            orderId += order[rnd.Next(0, order.Length)];
            orderId += order[rnd.Next(0, order.Length)];
            orderId += order[rnd.Next(0, order.Length)];

            return orderId;

            //**Testing**//
            //Console.WriteLine("Order: " + orderId);
        }

        //Current Random Algorithm Plus randomly Generate last 3 characters based on secure seed
        private static String thirdProgram(String order)
        {
            var orderId = "1";
            // Turns the 32 GUID into a 16 GUID + 1 + 3
            var letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //Console.WriteLine("Order: " + order);
            for (var i = 0; i < order.Length; i += 2)
            {
                var total = letters.IndexOf(order[i]);
                //Console.WriteLine("Index: " + i);
                total += letters.IndexOf(order[i + 1]);
                var ind = total % letters.Length;
                orderId += letters[ind];
            }

            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                for (int i =0; i < 3; i++)
                {
                    byte[] rno = new byte[5];
                    rg.GetBytes(rno);
                    int randomValue = BitConverter.ToInt32(rno, 0);
                    randomValue = Math.Abs(randomValue);
                    orderId += order[randomValue % order.Length];
                }
            }

            return orderId;

            //**Test Code**//
            //Console.WriteLine("Order: " + orderId);
        }

        //Used to Rewrite cvs file to only store order id
        private static void correctFile()
        {
            if (File.Exists(FPATH))
            {
                System.IO.StreamWriter filex = new System.IO.StreamWriter(@"C:\Users\Omar\Desktop\Text Documents\ordersfixed2.txt");
                using (StreamReader sr = File.OpenText(FPATH))
                {
                    string tmp = "";
                    String createdGUID = "";
                    while ((tmp = sr.ReadLine()) != null)
                    {
                        //Only use first 32 Characters
                        tmp = tmp.Substring(0, 32);

                        //Write 32 Character GUID in new File
                        filex.WriteLine(tmp);
                        filex.Flush();
                    }

                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Failed to Open File");
            }

            Console.WriteLine("Done!");

        }
    }
}
