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

        { BlockShape.Square_2x2, new Vector2Int[]
            {
                new Vector2Int(0,  0), new Vector2Int(1,  0),
                new Vector2Int(0, -1), new Vector2Int(1, -1)
            }
        },

        { BlockShape.Square_3x3, new Vector2Int[]
            {
                new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1),
                new Vector2Int(-1,  0), new Vector2Int(0,  0), new Vector2Int(1,  0),
                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            }
        },

        { BlockShape.RectangleVertical_2x3, new Vector2Int[]
            {
                new Vector2Int(0,  1), new Vector2Int(1,  1),
                new Vector2Int(0,  0), new Vector2Int(1,  0),
                new Vector2Int(0, -1), new Vector2Int(1, -1)
            }
        },

        { BlockShape.RectangleHorizontal_2x3, new Vector2Int[]
            {
                new Vector2Int(-1,  0), new Vector2Int(0,  0), new Vector2Int(1,  0),
                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            }
        },

        { BlockShape.Lshape_5_rotation_1, new Vector2Int[]
            {
                new Vector2Int(0,  2),
                new Vector2Int(0,  1),
                new Vector2Int(0,  0),
                new Vector2Int(1,  0),
                new Vector2Int(2,  0)
            }
        },

        { BlockShape.Lshape_5_rotation_2, new Vector2Int[]
            {
                new Vector2Int(2,   0),
                new Vector2Int(1,   0),
                new Vector2Int(0,   0),
                new Vector2Int(0,  -1),
                new Vector2Int(0,  -2)
            }
        },

        { BlockShape.Lshape_5_rotation_3, new Vector2Int[]
            {
                new Vector2Int(0,  -2),
                new Vector2Int(0,  -1),
                new Vector2Int(0,   0),
                new Vector2Int(-1,  0),
                new Vector2Int(-2,  0)
            }
        },

        { BlockShape.Lshape_5_rotation_4, new Vector2Int[]
            {
                new Vector2Int(-2,  0),
                new Vector2Int(-1,  0),
                new Vector2Int(0,   0),
                new Vector2Int(0,   1),
                new Vector2Int(0,   2)
            }
        },

        { BlockShape.Lshape_3_rotation_1, new Vector2Int[]
            {
                new Vector2Int(0,  1),
                new Vector2Int(0,  0),
                new Vector2Int(1,  0)
            }
        },

        { BlockShape.Lshape_3_rotation_2, new Vector2Int[]
            {
                new Vector2Int(1,   0),
                new Vector2Int(0,   0),
                new Vector2Int(0,  -1)
            }
        },

        { BlockShape.Lshape_3_rotation_3, new Vector2Int[]
            {
                new Vector2Int(0,  -1),
                new Vector2Int(0,   0),
                new Vector2Int(-1,  0)
            }
        },

        { BlockShape.Lshape_3_rotation_4, new Vector2Int[]
            {
                new Vector2Int(-1,  0),
                new Vector2Int(0,   0),
                new Vector2Int(0,   1)
            }
        },
    };
}
