using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MinegamesAntiCheat
{
    public class Communication
    {
        public static bool SendMessageToServer(string Message, string ServerIPAddress, int Port, ProtocolType Protocol)
        {
            IPAddress Server = IPAddress.Parse(ServerIPAddress);
            IPEndPoint Endpoint = new IPEndPoint(Server, Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
            try
            {
                socket.Connect(Endpoint);
                socket.Send(Encoding.UTF8.GetBytes(Message));
                return true;
            }
            catch
            {
                socket.Close();
                return false;
            }
        }

        public static bool SendMessageToServer_Encrypted(string Message, string ServerIPAddress, int Port, ProtocolType Protocol, string PublicKey, int RSAKeySize, bool OAEP_Padding)
        {
            IPAddress Server = IPAddress.Parse(ServerIPAddress);
            IPEndPoint Endpoint = new IPEndPoint(Server, Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
            try
            {
                RSACryptoServiceProvider RSAEncrypt = new RSACryptoServiceProvider(RSAKeySize);
                RSAEncrypt.FromXmlString(PublicKey);
                string EncryptedString = Convert.ToBase64String(RSAEncrypt.Encrypt(Encoding.UTF8.GetBytes(Message), OAEP_Padding));
                socket.Connect(Endpoint);
                socket.Send(Encoding.UTF8.GetBytes(EncryptedString));
                return true;
            }
            catch
            {
                socket.Close();
                return false;
            }
        }

        public static string GetMessageFromServer(string ServerIPAddress, int Port, ProtocolType Protocol)
        {
            IPAddress Server = IPAddress.Parse(ServerIPAddress);
            EndPoint Endpoint = new IPEndPoint(Server, Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
            try
            {
                byte[] Buffer = Encoding.UTF8.GetBytes(string.Empty);
                socket.ReceiveFrom(Buffer, ref Endpoint);
                return Encoding.UTF8.GetString(Buffer);
            }
            catch
            {
                socket.Close();
                return null;
            }
        }

        public static string GetEncryptedMessageFromServer(string ServerIPAddress, int Port, ProtocolType Protocol, string PrivateKey, int RSAKeySize, bool OAEP_Padding)
        {
            IPAddress Server = IPAddress.Parse(ServerIPAddress);
            EndPoint Endpoint = new IPEndPoint(Server, Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
            try
            {
                byte[] Buffer = Encoding.UTF8.GetBytes(string.Empty);
                socket.ReceiveFrom(Buffer, ref Endpoint);
                RSACryptoServiceProvider RSADecrypt = new RSACryptoServiceProvider(RSAKeySize);
                RSADecrypt.FromXmlString(PrivateKey);
                return Encoding.UTF8.GetString(RSADecrypt.Decrypt(Convert.FromBase64String(Encoding.UTF8.GetString(Buffer)), OAEP_Padding));
            }
            catch
            {
                socket.Close();
                return null;
            }
        }
    }
}
