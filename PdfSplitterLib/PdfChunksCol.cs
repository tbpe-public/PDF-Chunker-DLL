using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PdfSplitterLib
{
    /// <summary>
    /// An indexed collection of PdfChunk.
    /// </summary>
    public class PdfChunksCol : CollectionBase
    {
        /////////////////////////////////
        //FIELDS
        /////////////////////////////////

        //Indexer
        public PdfChunk this[int ChunkIndex]
        {
            get { return (PdfChunk)List[ChunkIndex]; }
            set { List[ChunkIndex] = value; }
        }

        /////////////////////////////////
        //METHODS
        /////////////////////////////////

        /// <summary>
        /// Add a PDF chunk to the collection
        /// </summary>
        /// <param name="Chunk">PdfChunk</param>
        public void Add(PdfChunk Chunk)
        {
            List.Add(Chunk);
        }

        /// <summary>
        /// Remove a PDF chunk from the collection
        /// </summary>
        /// <param name="Chunk">File object derived from TLFFile</param>
        public void Remove(PdfChunk Chunk)
        {
            List.Remove(Chunk);
        }

        /// <summary>
        /// Remove all Chunks from the collection
        /// </summary>
        public void RemoveAll()
        {
            foreach (PdfChunk Chunk in this.List)
                this.Remove(Chunk);
        }
    }
}
