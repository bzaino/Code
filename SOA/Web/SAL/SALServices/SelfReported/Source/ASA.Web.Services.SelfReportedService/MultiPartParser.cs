using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ASA.Web.Services.SelfReportedService
{
    public class MultipartParser
    {
        public MultipartParser(Stream stream)
        {   
            this.Parse(stream, Encoding.UTF8);
        }

        public MultipartParser(Stream stream, Encoding encoding)
        {
            this.Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            this.Success = false;

            // Read the stream into a byte array
            byte[] data = ToByteArray(stream);

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                // Look for Content-Type
                Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                Match contentTypeMatch = re.Match(content);

                // Look for filename
                re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                Match filenameMatch = re.Match(content);

                // Did we find the required values?
                if (contentTypeMatch.Success && filenameMatch.Success)
                {
                    // Set properties
                    this.ContentType = contentTypeMatch.Value.Trim();
                    this.Filename = filenameMatch.Value.Trim();
                   
		    //Check that filename contains "MyStudentData.txt"
                    if (this.Filename.Contains("MyStudentData.txt"))
                    {
                        // Get the start & end indexes of the file contents
                        int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                        byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                        int endIndex = IndexOf(data, delimiterBytes, startIndex);

                        int contentLength = endIndex - startIndex;

                        // Extract the file contents from the byte array
                        byte[] fileData = new byte[contentLength];
                        byte[] individualIdBytes = new byte[data.Length];
                        int fullLength = data.Length;
                        int indIdBlockLength = fullLength - endIndex;
                        Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);
                        Buffer.BlockCopy(data, endIndex, individualIdBytes, 0, indIdBlockLength);

                        Encoding enc = Encoding.ASCII;
                        var indIdBlockStr = enc.GetString(individualIdBytes);

                        var indIdStr = indIdBlockStr.Substring(indIdBlockStr.IndexOf("\r\n\r\n") + 4, 36);

                        this.individualId = indIdStr;
                        this.FileContents = fileData;
                        this.Success = true;
                    }
                }
            }
        }

        private int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            int index = 0;
            int startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf<byte>(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        private byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public bool Success
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public byte[] FileContents
        {
            get;
            private set;
        }

        public string individualId
        {
            get;
            private set;
        }
    }
}

  
