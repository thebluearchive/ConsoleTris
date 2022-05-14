using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public static class PieceHelpers
    {
        public static string GetBlockTypeColor(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.I:
                    return "#00FFFF";
                case BlockType.J:
                    return "#0000FF";
                case BlockType.L:
                    return "#FFAA00";
                case BlockType.O:
                    return "#FFFF00";
                case BlockType.S:
                    return "#00FF00";
                case BlockType.T:
                    return "#9900FF";
                case BlockType.Z:
                    return "#FF0000";
                case BlockType.Empty:
                    return "#000000";
            }

            // Program should never hit this line of code since BlockType is
            // non-nullable and we have checked every possible value for the
            // BlockType enum. However the compiler is unhappy unless I return
            // something or throw an error here. So I am choosing to throw an
            // error if this line of code is ever hit.
            throw new Exception("Unable to determine Block Type.");
        }
    }
}
