using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PdfSplitterLib
{
    public class PdfChunk
    {
        //Properties
        //***************************************************************************

        public string chunkName;    
        public string chunkType;
        public int firstPage;
        public int lastPage;

        //C-tor
        //***************************************************************************

        public PdfChunk(int fPage, string name)
        {
            firstPage = fPage;
            chunkName = name;
        }

        //Methods
        //***************************************************************************

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return chunkName + " : PAGES " + firstPage + " - " + lastPage;
        }
    }
}
