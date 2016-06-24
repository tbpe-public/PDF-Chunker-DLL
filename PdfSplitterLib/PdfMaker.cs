using System;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace PdfSplitterLib
{
    /// <summary>
    /// PDF creater tool that uses the iTestSharp DLL.
    /// </summary>
    public static class PdfMaker
    {
        /// <summary>
        /// Create a PDF from a subset of source PDF pages specified in a page range.
        /// </summary>
        /// <param name="FirstPage">first source page</param>
        /// <param name="LastPage">last source page</param>
        /// <param name="srcPath">the source file</param>
        /// <param name="outputPath">New PDF with the specefied pages.</param>
        /// <returns>True if all goes well</returns>
        public static bool RipPdf(int FirstPage, int LastPage, string srcPath, string outputPath)
        {
            bool result = true;

            PdfReader reader = null;
            Document document = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            // Intialize a new PdfReader instance with the contents of the source Pdf file:
            using (reader = new PdfReader(srcPath))
            {

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                using (document = new Document(reader.GetPageSizeWithRotation(FirstPage)))
                {

                    // Initialize an instance of the PdfCopyClass with the source 
                    // document and an output file stream:
                    pdfCopyProvider = new PdfCopy(document,
                        new System.IO.FileStream(outputPath, System.IO.FileMode.Create));

                    document.Open();

                    // Walk the specified range and add the page copies to the output file:
                    for (int i = FirstPage; i <= LastPage; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }

                }//Dispose of document
            }//Dispose of reader

            return result;
        }
    }
}
