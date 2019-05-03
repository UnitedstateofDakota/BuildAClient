using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            const int MAXBUF = 256;
            const string IP = "127.0.0.1";
            const int PORT = 8080;

            IPAddress addr = IPAddress.Parse(IP);
            TcpListener listener = new TcpListener(addr, PORT);
            listener.Start();
            Console.WriteLine("Server started.");
            while(true)
            {
                // This is what you have to do to get what is sent to you across the port
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                NetworkStream clientstream = client.GetStream();
                byte []buffer = new byte[MAXBUF];
                int iLen = clientstream.Read(buffer, 0, MAXBUF);
                string request = Encoding.ASCII.GetString(buffer);
                request = request.Substring(0, iLen);
                request = request.Trim();

                // This is basically the equialent of logging
                Console.WriteLine($"{DateTime.Now} Received: {request}");

                // This might be done in a separate thread in real life
                string response = ExecuteCommand(request);

                // More logging
                Console.WriteLine($"{DateTime.Now} Responding: {response}");

                // Send this back to the client
                clientstream.Write(Encoding.ASCII.GetBytes(response), 
                    0, 
                    Encoding.ASCII.GetByteCount(response));

                // You should always close the new port so as not to leak ports
                client.Close();
            }
        }

        static string ExecuteCommand(string request)
        {
            string response = "response";
            int iIndexSpace = request.IndexOf(' ');
            string sCommand = "";
            string sOperand = "";

            if (iIndexSpace > 0)
            {
                sCommand = request.Substring(0, iIndexSpace);
                sOperand = request.Substring(iIndexSpace + 1);

                // Now, cut off anything after the space
                iIndexSpace = sOperand.IndexOf(' ');
                if (iIndexSpace > 0)
                {
                    sOperand = sOperand.Substring(0, iIndexSpace);
                }

                switch (sCommand)
                {
                    case "GET":
                        try
                        {
                            response = File.ReadAllText(sOperand);
                        }
                        catch (Exception err)
                        {
                            response = err.Message;
                        }
                        break;

                    default:
                        response = "*** Unknown Command ***";
                        break;
                }
            }



            return (response);
        }
    }
}
