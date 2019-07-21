using System;
using System.Text;

namespace DiscordMinesweeper
{

    class MinesweeperMap
    {
        private readonly int[,] map;

        private static readonly string[] emojis =
        {
            "||:bomb:||",
            "||:zero:||",
            "||:one:||",
            "||:two:||",
            "||:three:||",
            "||:four:||",
            "||:five:||",
            "||:six:||",
            "||:seven:||",
            "||:eight:||",
            "||:nine:||"
       };

        private void SetBombs(int mines)
        {
            Random rand = new Random();
            int map_x;
            int map_y;

            while (mines != 0)
            {
                map_x = rand.Next(0, map.GetLength(0));
                map_y = rand.Next(0, map.GetLength(1));
                if (map[map_x, map_y] != -1)
                {
                    map[map_x, map_y] = -1;
                    mines -= 1;
                }
            };
        }

        private int IsValidAndNotBomb(int x, int y, int max_x, int max_y)
        {
            if (x < 0)
                return 0;
            if (y < 0)
                return 0;
            if (x > max_x)
                return 0;
            if (y > max_y)
                return 0;
            if (map[x, y] == -1)
                return 1;
            return 0;
        }

        private int CountBombs(int x, int y, int max_x, int max_y)
        {
            int i = 0;

            if (map[x, y] == -1)
                return -1;
            i += IsValidAndNotBomb(x - 1, y - 1, max_x, max_y); i += IsValidAndNotBomb(x - 1, y - 0, max_x, max_y); i += IsValidAndNotBomb(x - 1, y + 1, max_x, max_y);
            i += IsValidAndNotBomb(x - 0, y - 1, max_x, max_y); /*                                              */; i += IsValidAndNotBomb(x - 0, y + 1, max_x, max_y);
            i += IsValidAndNotBomb(x + 1, y - 1, max_x, max_y); i += IsValidAndNotBomb(x + 1, y - 0, max_x, max_y); i += IsValidAndNotBomb(x + 1, y + 1, max_x, max_y);
            return i;
        }
        public MinesweeperMap(int[,] map, int mines)
        {
            this.map = map;
            int rows = this.map.GetLength(0);
            int cols = this.map.GetLength(1);

            this.SetBombs(mines);
            for (int i = 0; i < rows; i += 1)
            {
                for (int j = 0; j < cols; j += 1)
                {
                    this.map[i, j] = CountBombs(i, j, rows - 1, cols - 1);
                }
            }
        }

        public string ToDiscordEmoji()
        {
            StringBuilder sb = new StringBuilder(String.Format("Here! Have a grid\n"));
            int rows = this.map.GetLength(0);
            int cols = this.map.GetLength(1);

            for (int i = 0; i < rows; i += 1)
            {
                for (int j = 0; j < cols; j += 1)
                {
                    sb.Append(emojis[map[i, j] + 1]);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            StringBuilder myString = new StringBuilder("My map:\n");

            for (int i = 0; i < rows; i += 1)
            {
                for (int j = 0; j < cols; j += 1)
                {
                    if (map[i, j] == -1)
                        myString.Append("B");
                    else
                        myString.Append(map[i, j].ToString());
                }
                myString.Append("\n");
            }
            return myString.ToString();
        }
    }

    static class MapGenerator
    {
        static private int[,] AllocMap(int x, int y)
        {
            int[,] map = new int[x, y];

            for (int i = 0; i < x; i += 1)
            {
                for (int j = 0; j < y; j += 1)
                {
                    map[i, j] = 0;
                }
            }
            return map;
        }

        static public MinesweeperMap GenerateMap(int x, int y, int mines)
        {
            int[,] map = AllocMap(x, y);

            return new MinesweeperMap(map, mines);
        }
    }
}
