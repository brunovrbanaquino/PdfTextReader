﻿using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveTableText : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _tables;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var page = parserTable.PageTables;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveTableText requires IdentifyTables");
            }
            
            this._tables = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._tables == null)
            {
                PdfReaderException.AlwaysThrow("RemoveTableText requires IdentifyTables");
            }

            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                bool insideTable = false;

                foreach(var table in _tables)
                {
                    if( Block.HasOverlap(table, block) )
                    {
                        insideTable = true;
                        break;
                    }
                }

                if( !insideTable )
                {
                    result.Add(block);
                }
            }

            return result;
        }
    }
}
