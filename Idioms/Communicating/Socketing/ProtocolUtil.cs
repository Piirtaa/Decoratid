using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Communicating.Socketing
{
    /// <summary>
    /// helper class that conforms stream reading/writing to a protocol
    /// </summary>
    internal static class ProtocolUtil
    {
        internal const string SIZE_PREFIX_DELIM = "|";

        internal static void Write(NetworkStream stream, string data)
        {
            //prepend the length of the data to the message, and convert to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(data.Length.ToString() + SIZE_PREFIX_DELIM + data);

            // send message
            stream.Write(bytes, 0, bytes.Length);
        }

        internal static string Read(NetworkStream stream)
        {
            //start with an initially small 50 byte buffer.  
            //This should be sufficient to read the message length, from which we then read the entire message exactly
            byte[] buffer = new byte[50];

            //indexes we track
            int bytesRead = 0;
            int chunkRead;

            //length of the message as parsed from the message prefix
            int messageLength = -1;

            //spin 
            while ((chunkRead = stream.Read(buffer, bytesRead, buffer.Length - bytesRead)) > 0)
            {
                //update the read index
                bytesRead += chunkRead;

                //read the buffer
                string bufferString = Encoding.ASCII.GetString(buffer);

                //if we don't have a message length let's get one.  this will run on first read only
                if (messageLength == -1)
                {
                    var lengthIdx = bufferString.IndexOf(SIZE_PREFIX_DELIM);
                    if (lengthIdx == -1)
                    {
                        //wow, long message.  50 chars not enough to get length
                        //expand buffer and try again
                        byte[] newBuffer = new byte[buffer.Length * 2];
                        Array.Copy(buffer, newBuffer, buffer.Length);
                        buffer = newBuffer;

                        //try again
                        continue;
                    }
                    else
                    {
                        messageLength = bufferString.Substring(0, lengthIdx).ConvertToInt();

                        //is the message smaller than the buffer? if so, we're done reading
                        if ((messageLength + messageLength.ToString().Length + 1) <= buffer.Length)
                            break;

                        //now that we have message length allocate new, exact, buffer space
                        byte[] newBuffer2 = new byte[messageLength + messageLength.ToString().Length + 1];
                        Array.Copy(buffer, newBuffer2, buffer.Length);
                        buffer = newBuffer2;
                    }
                }

                //oky dokey. we have length and proper buffer allocation.  lets see if we need to get more data
                if (messageLength + messageLength.ToString().Length + 1 == bytesRead)
                {
                    break;
                }
            }

            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[bytesRead];
            Array.Copy(buffer, ret, bytesRead);

            var rv = Encoding.ASCII.GetString(ret);

            //remove the length prefix
            rv = rv.Remove(0, rv.IndexOf(SIZE_PREFIX_DELIM) + 1);

            return rv;
        }
    }
}
