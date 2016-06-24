using System;
using System.IO;
using System.Drawing;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Newtonsoft.Json;

namespace PdfSplitterLib
{
    public class PdfChunker
    {
        //Properties
        //88888888888888888888888888888888888888888888888888888888888888888888888
        
        protected FileInfo SrcPdf;                    //The PDF to chunk
        protected PdfChunksCol Chunks;                //Collection of chunks.

        //Count Properties
        private int totalPages;
        public int TotalPages
        {
            get { return totalPages; }
        }
        
        private int qrCount;          //QR Code seperator page count.
        public int QrCount
        {
            get { return qrCount; }
        }

        private int contentCount;     //Content page count
        public int ContentCount
        {
            get { return contentCount; }
        }

        private int ripCount;         //Ripped chunck count
        public int RipCount
        {
            get { return ripCount; }
        }

        //C-Tors
        //88888888888888888888888888888888888888888888888888888888888888888888888

        public PdfChunker(FileInfo src)
        {
            SrcPdf = src;
            Chunks = new PdfChunksCol();

            //Zero out counts
            qrCount = 0;
            contentCount = 0;
            ripCount = 0;  
        }

        //Methods
        //88888888888888888888888888888888888888888888888888888888888888888888888

        /// <summary>
        /// Divide the source pdf by QR code seperatorrs into chunks that
        /// can be ripped and saved from the source.
        /// </summary>
        public void FindPdfChunks()
        {
            GhostscriptVersionInfo Gvi;                 //Ghostscript info object
            GhostscriptRasterizer Rasterizer = null;    //Rasterizer
            Bitmap pageImg; //Image of one PDF page.
            string docType; //Value of document-type property in QR code json.

            //Only process PDFs
            if (SrcIsPdf())
            {
                try
                {
                    //TEST_OUTPUT
                    //Console.WriteLine("BASE DIRECTORY{0}{1}", AppDomain.CurrentDomain.BaseDirectory + "gsdll64.dll", Environment.NewLine);

                    //Init GhostScript and Rasterize
                    Gvi = new GhostscriptVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gsdll64.dll"));
                    Rasterizer = new GhostscriptRasterizer();
                    Rasterizer.Open(SrcPdf.FullName, Gvi, false); //Raserize the source PDF.

                    totalPages = Rasterizer.PageCount; //Total pages

                    //Loop from page 1 to page n-1 (No need to read the last page).
                    for (int i = 1; i < Rasterizer.PageCount; i++)
                    {
                        pageImg = (Bitmap)Rasterizer.GetPage(72, 72, i);
                        docType = getDocTypeStr(QrReader.ReadQr(pageImg));

                        //Assert that page has a doc type
                        if (!isBlankOrNullStr(docType))
                        {
                            qrCount++; //Increment found qr code count.

                            //Console.WriteLine("DOC TYPE: {0}{1}", docType, Environment.NewLine); //TEST OUTPUT

                            //Create chunk with src page # for first page (The one after QR page) and name (srcFileName_docType_qr#).
                            initChunk(i + 1, Path.GetFileNameWithoutExtension(SrcPdf.Name) + "_" + docType + "_" + qrCount);

                            updatePreviousChunk(i - 1); //Update the previous chunks last page.
                        }
                        else
                        {
                            contentCount++; //Content page count.
                        }
                    }
                    //END LOOP

                    contentCount++; //Loop ends before the last page which is assumed to be content.

                    //Assert that we have some chuncks
                    if (Chunks.Count > 0)
                    {
                        //Set last page of the last chunk to the last page of the source document.
                        Chunks[Chunks.Count - 1].lastPage = Rasterizer.PageCount;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to complete chunk search. " + ex.Message);
                }
                finally
                {
                    //Dispose raterizer resource.
                    if (Rasterizer != null)
                    {
                        Rasterizer.Close();
                        Rasterizer.Dispose();
                    }
                }
            }
            //End outer if
        }

        /// <summary>
        /// Rip chucks from source pdf.
        /// </summary>
        /// <returns></returns>
        public void RipChucks()
        {
            //Assert that we have some chuncks.
            if (Chunks.Count > 0)
            {
                //Loop through chunks and rip from source.
                foreach (PdfChunk Chunk in Chunks)
                {
                    //Rip chunk to an output pdf file
                    PdfMaker.RipPdf(Chunk.firstPage, Chunk.lastPage, SrcPdf.FullName,
                        Path.Combine(SrcPdf.DirectoryName, Chunk.chunkName + ".pdf"));

                    ripCount++; //Count rips.
                }
            }
        }

        /// <summary>
        /// Assert that the source files extesion is .pdf
        /// </summary>
        /// <returns>True if .pdf</returns>
        public bool SrcIsPdf()
        {
            string ext = ""; //Source file's extension
            bool isPdf = false; //True if source doc had .pdf extension

            if(SrcPdf != null)
            {
                ext = SrcPdf.Extension.ToLower(); //use lowercase

                isPdf = (ext == ".pdf");
            }

            return isPdf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fPage"></param>
        /// <param name="name"></param>
        public void initChunk(int fPage, string name)
        {
            //Add new chunk with a first source page number and name.
            Chunks.Add(new PdfChunk(fPage, name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lPage"></param>
        public void updatePreviousChunk(int lPage)
        {
            int previousChunk = Chunks.Count - 2; //The second to last chunk index

            if(previousChunk >= 0 && lPage > 0)
            {
                Chunks[previousChunk].lastPage = lPage;
            }
        }

        /// <summary>
        /// Deserialize json containing a document-type property into
        /// a DocType object and get the string value.
        /// </summary>
        /// <param name="jsonStr">Json string with document-type property.</param>
        /// <returns>Returns the value of document-type or empty string if unable to find it.</returns>
        public string getDocTypeStr(string jsonStr)
        {
            DocType Dt;
            string type;

            try
            {
                Dt = JsonConvert.DeserializeObject<DocType>(jsonStr);
                type = Dt.documentType;
            }
            catch(Exception)
            {
                type = String.Empty;
            }

            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool isBlankOrNullStr(string str)
        {
            if (str != null && str != String.Empty)
                return false;
            else
                return true;
        }

        //Test Methods
        //88888888888888888888888888888888888888888888888888888888888888888888888

        /// <summary>
        /// Print all PDF chunk properties to the console.
        /// </summary>
        public void printChunks()
        {
            foreach(PdfChunk Chunk in Chunks)
            {
                Console.WriteLine(Chunk.ToString());
            }
        }

        /// <summary>
        /// Print rip job summary to the console
        /// </summary>
        public void printRipSummary()
        {
            Console.WriteLine("Total Source Pages: {0}{1}", totalPages, Environment.NewLine);
            Console.WriteLine("Total QR Code Pages: {0}{1}", qrCount, Environment.NewLine);
            Console.WriteLine("Total Content Pages: {0}{1}", contentCount, Environment.NewLine);
            Console.WriteLine("Total Ripped Chunks: {0}{1}", ripCount, Environment.NewLine);
        }
    }
}
