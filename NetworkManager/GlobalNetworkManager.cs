﻿using System;
using System.IO;
using System.Net.Sockets;

namespace NetworkManager
{
    public static class GlobalNetworkManager
    {
        public static ClientNetworkManager clientNetworkManager { get; }

        public static ServerNetworkManager ServerNetworkManager { get; }
        

        internal static NetworkStream networkStream { get; set; }

        internal static StreamWriter streamWriter { get; set; }

        internal static StreamReader streamReader { get; set; }

        internal static BinaryWriter binaryWriter { get; set; }

        internal static BinaryReader binaryReader { get; set; }

        const int DEFAULT_BUFFER_SIZE = 4046;   // to use


        /// <summary>
        /// Constructor, initiate clientNetworkManager and ServerNetworkManager
        /// </summary>
        static GlobalNetworkManager()
        {
            clientNetworkManager = new ClientNetworkManager();
            ServerNetworkManager = new ServerNetworkManager();
        }



        #region Read
        
        /// <summary>
        /// Read a byte array of 'count' size from the network stream into the given buffer
        /// </summary>
        /// <param name="buffer">Buffer to write the byte array into</param>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>Read lenght</returns>
        public static int ReadBytesIntoBuffer(byte[] buffer, int count)
            => binaryReader.Read(buffer, 0, count);


        /// <summary>
        /// Read one line from the network stream
        /// </summary>
        /// <returns>String</returns>
        public static string ReadLine() => streamReader.ReadLine();

        #endregion Read


        #region Write

        /// <summary>
        /// Read all the given ReadStream content and write it to the network stream as byte arrays, then flush
        /// </summary>
        /// <param name="stream">Stream to process, must be a readable stream</param>
        /// <param name="bufferSize">Buffer size that will be used to send data</param>
        public static void ReadStreamAndWriteToNetworkStream(Stream stream, int bufferSize)
        {
            if (!stream.CanRead)
            {
                // The stream can't be read : invalid argument
                throw new ArgumentException();
            }

            var buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                binaryWriter.Write(buffer, 0, bytesRead);
                binaryWriter.Flush();
            }
        }


        /// <summary>
        /// Read all the network ReadStream content and write it to the given stream stream
        /// </summary>
        /// <param name="stream">Stream to process, must be a writable stream</param>
        /// <param name="bufferSize">Buffer size that will be used to receive data</param>
        /// <param name="dataSize">Size of the data to write</param>
        public static void ReadNetworkStreamAndWriteToStream(Stream stream, int bufferSize, int dataSize)
        {
            if (!stream.CanWrite)
            {
                // The stream can't write : invalid argument
                throw new ArgumentException();
            }

            var buffer = new byte[bufferSize];
            int bytesRead;
            var bytesWritten = 0;
            while ((bytesRead = ReadBytesIntoBuffer(buffer, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;

                // The file has been totally written
                if (bytesWritten == dataSize)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// Write the given byte array to the network stream, then flush
        /// </summary>
        /// <param name="data">Byte array to write</param>
        public static void WriteByteArray(byte[] data)
        {
            binaryWriter.Write(data);
            binaryWriter.Flush();
        }


        /// <summary>
        /// Write the given string to the network stream, then flush
        /// </summary>
        /// <param name="data">String to write</param>
        public static void WriteLine(string data)
        {
            streamWriter.WriteLine(data);
            streamWriter.Flush();
        }


        /// <summary>
        /// Write a 'Hello !' message to the network stream and flush
        /// </summary>
        public static void SayHello() => WriteLine("Hello !");

        #endregion Write


        /// <summary>
        /// Close the StreamReaders, StreamWriters and the network stream
        /// </summary>
        internal static void Cleanup()
        {
            streamReader.Close();
            streamWriter.Close();
            binaryReader.Close();
            binaryWriter.Close();
            networkStream.Close();
        }
    }
}