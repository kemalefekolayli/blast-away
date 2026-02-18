using UnityEngine;
using System.Collections.Generic;

public static class BlockShapeData
{
    // Her BlockShape için tile offset listesi.
    // (0,0) = merkez tile, diğerleri ona göre konumlanır.
    public static readonly Dictionary<BlockShape, Vector2Int[]> Offsets =
        new Dictionary<BlockShape, Vector2Int[]>
    {
        { BlockShape.Single, new Vector2Int[]
            {
                new Vector2Int(0, 0)
            }
        },

        { BlockShape.LongHorizontalLine_2, new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0)
            }
        },

        { BlockShape.LongVerticalLine_2, new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, -1)
            }
        },

        { BlockShape.LongHorizontalLine_3, new Vector2Int[]
            {
                new Vector2Int(-1, 0),
                new Vector2Int(0,  0),
                new Vector2Int(1,  0)
            }
        },

        { BlockShape.LongVerticalLine_3, new Vector2Int[]
            {
                new Vector2Int(0,  1),
                new Vector2Int(0,  0),
                new Vector2Int(0, -1)
            }
        },

        { BlockShape.LongHorizontalLine_4, new Vector2Int[]
            {
                new Vector2Int(-1, 0),
                new Vector2Int(0,  0),
                new Vector2Int(1,  0),
                new Vector2Int(2,  0)
            }
        },

        { BlockShape.LongVerticalLine_4, new Vector2Int[]
            {
                new Vector2Int(0,  1),
                new Vector2Int(0,  0),
                new Vector2Int(0, -1),
                new Vector2Int(0, -2)
            }
        },
    };
}
