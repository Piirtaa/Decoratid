using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Communication.Implementations.Sockets
{
    /// <summary>
    /// helper class for this socket implementation
    /// </summary>
    internal static class ProtocolUtil
    {
        internal const string SIZE_PREFIX_DELIM = "|";

        internal static void ProtocolWrite(NetworkStream stream, string data)
        {
            
            //prepend the length of the data to the message, and convert to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(data.Length.ToString() + SIZE_PREFIX_DELIM + data );

            // send message
            stream.Write(bytes, 0, bytes.Length);
        }

        internal static string ProtocolRead(NetworkStream stream)
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

                //read the existing buffer
                string bufferString;

                //test if we're complete - Skip Out Filter # 1
                //we have a positive message length and we've read to this length
                if (messageLength > -1 && bytesRead == messageLength)
                {
                    //read the buffer and return
                    bufferString = Encoding.ASCII.GetString(buffer);

                    //remove the length prefix
                    bufferString = bufferString.Remove(0, bufferString.IndexOf(SIZE_PREFIX_DELIM)+1);
                    return bufferString;
                }

                //test if we've gotten the message length yet. 
                //if so, just continue reading as we've already set the buffer up
                if (messageLength > -1)
                    continue;

                //try to parse the message length out
                bufferString = Encoding.ASCII.GetString(buffer);
                var prefixIDX = bufferString.IndexOf(SIZE_PREFIX_DELIM, 0);
                if (prefixIDX > 0)
                {
                    //parse the length
                    var prefix = bufferString.Substring(0, prefixIDX);
                    messageLength = int.Parse(prefix);

                    //allocate new buffer space
                    byte[] newBuffer = new byte[messageLength];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    buffer = newBuffer;

                    continue;
                }
                
                //looks like we have a very long message as we don't have the message length prefix
                //we'll have to expand our buffer if we've used it up

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (bytesRead == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        break;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[bytesRead] = (byte)nextByte;
                    buffer = newBuffer;
                    bytesRead++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[bytesRead];
            Array.Copy(buffer, ret, bytesRead);

            var rv = Encoding.ASCII.GetString(ret);
            //remove the length prefix
            rv = rv.Remove(0, rv.IndexOf(SIZE_PREFIX_DELIM));

            return rv;
        }
    }
}
