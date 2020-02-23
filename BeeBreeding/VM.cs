using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeeBreeding
{
    class VM : INotifyPropertyChanged
    {
        private int originTile = 1;
        public int OriginTile
        {
            get { return originTile; }
            set { originTile = value; OnChange(); }
        }

        private int destinationTile = 1;
        public int DestinationTile
        {
            get { return destinationTile; }
            set { destinationTile = value; OnChange(); }
        }

        public BindingList<UserSelection> UserInput { get; set; } = new BindingList<UserSelection>();

        public BindingList<Output> Output { get; set; } = new BindingList<Output>();

        readonly List<CoOrds> tilePosition = new List<CoOrds>();
        int l_x = 0, l_y = 0, t_x = 0, t_y = 0, tileDistance = 0, tile_l = 0, tile_t = 0;
        string ShortestPath = "", Distance = "";
        readonly CoOrds refTile = new CoOrds(0, 0, 0);

        enum Movement { UP, UP_RIGHT, UP_LEFT, DOWN, DOWN_RIGHT, DOWN_LEFT };

        public VM()
        {
            int TileNum = 1;
            int X = 0;
            int Y = 0;
            int tileNumRef = 0;
            for (int ringNum = 0; ringNum < 60;)
            {
                switch (ringNum)
                {
                    case 0:
                        tilePosition.Add(new CoOrds(TileNum, X, Y));
                        TileNum++;
                        break;
                    default:
                        TileNum = 2 + (6 * tileNumRef);
                        tileNumRef += ringNum;
                        X = ringNum;
                        Y = 0 - ringNum;
                        int halfRingSize = ringNum * 3;
                        int sequence = 0;
                        for (int num = 0; num < halfRingSize; num++)
                        {
                            if (sequence < (1 * ringNum))
                            {
                                X--;
                                Y--;
                                tilePosition.Add(new CoOrds(TileNum, X, Y));
                                int inverseTileNum = TileNum + halfRingSize;
                                int inverseX = X * -1;
                                int inverseY = Y * -1;
                                tilePosition.Add(new CoOrds(inverseTileNum, inverseX, inverseY));
                                TileNum++;
                                sequence++;
                            }
                            else if (sequence < (2 * ringNum))
                            {
                                X--;
                                Y++;
                                tilePosition.Add(new CoOrds(TileNum, X, Y));
                                int inverseTileNum = TileNum + halfRingSize;
                                int inverseX = X * -1;
                                int inverseY = Y * -1;
                                tilePosition.Add(new CoOrds(inverseTileNum, inverseX, inverseY));
                                TileNum++;
                                sequence++;
                            }
                            else
                            {
                                X = -ringNum;
                                Y += 2;
                                tilePosition.Add(new CoOrds(TileNum, X, Y));
                                int inverseTileNum = TileNum + halfRingSize;
                                int inverseX = X * -1;
                                int inverseY = Y * -1;
                                tilePosition.Add(new CoOrds(inverseTileNum, inverseX, inverseY));
                                TileNum++;
                                sequence++;
                            }
                        }
                        break;
                }
                ringNum++;
            }
        }

        public void ShowUserInput()
        {
            UserInput.Add(new UserSelection(OriginTile, DestinationTile));
        }

        public void BeginPath()
        {
            Output.Clear();
            foreach (UserSelection us in UserInput)
            {
                ShortestPath = "    ";
                tileDistance = 0;
                foreach (CoOrds e in tilePosition)
                {
                    if (us.A == e.TileNum)
                    {
                        tile_l = e.TileNum;
                        l_x = e.X;
                        l_y = e.Y;
                    }
                }
                foreach (CoOrds e in tilePosition)
                {
                    if (us.B == e.TileNum)
                    {
                        tile_t = e.TileNum;
                        t_x = e.X;
                        t_y = e.Y;
                    }
                }
                CheckCoOrds(l_x, l_y, t_x, t_y);
            }
        }

        public void CheckCoOrds(int l_x, int l_y, int t_x, int t_y)
        {
            if (l_x == t_x && l_y == t_y)
            {
                ShortestPath += tile_t.ToString();
                Distance = "The distance between cells " + tile_l.ToString() + " and " + tile_t.ToString() + " is " + tileDistance.ToString();
                Output.Add(new Output(Distance, ShortestPath));
            }
            else
            {
                foreach (CoOrds a in tilePosition)
                {
                    if (l_x == a.X && l_y == a.Y)
                    {
                        refTile.TileNum = a.TileNum;
                        refTile.X = a.X;
                        refTile.Y = a.Y;
                        ShortestPath += a.TileNum.ToString() + " - ";
                        tileDistance += 1;
                        GetNextTile(refTile);
                    }
                }
            }
        }

        public void GetNextTile(CoOrds refTile)
        {
            if (l_x == t_x)                                              // Move up or down only   
                if (l_y < t_y)
                    GetTileCoOrds(refTile, (int)Movement.UP);            // TileNum above
                else
                    GetTileCoOrds(refTile, (int)Movement.DOWN);          // TileNum below 
            else if (l_x > t_x)                                          // Move left
                if (l_y > t_y)
                    GetTileCoOrds(refTile, (int)Movement.DOWN_LEFT);
                else
                    GetTileCoOrds(refTile, (int)Movement.UP_LEFT);
            else if (l_y > t_y)                                          // Move Right
                GetTileCoOrds(refTile, (int)Movement.DOWN_RIGHT);
            else
                GetTileCoOrds(refTile, (int)Movement.UP_RIGHT);
        }

        public void GetTileCoOrds(CoOrds refTile, int move)
        {
            int X = refTile.X;
            int Y = refTile.Y;
            Movement path = (Movement)Enum.Parse(typeof(Movement), move.ToString());
            switch (path)
            {
                case Movement.UP:
                    Y += 2;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
                case Movement.UP_RIGHT:
                    X += 1;
                    Y += 1;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
                case Movement.UP_LEFT:
                    X -= 1;
                    Y += 1;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
                case Movement.DOWN:
                    Y -= 2;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
                case Movement.DOWN_RIGHT:
                    X += 1;
                    Y -= 1;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
                case Movement.DOWN_LEFT:
                    X -= 1;
                    Y -= 1;
                    foreach (CoOrds e in tilePosition)
                    {
                        if (e.X == X && e.Y == Y)
                        {
                            l_x = e.X;
                            l_y = e.Y;
                            CheckCoOrds(l_x, l_y, t_x, t_y);
                        }
                    }
                    break;
            }
        }

        public void ResetData()
        {
            OriginTile = 1;
            DestinationTile = 1;
            ShortestPath = "";
            Distance = "";
            l_x = 0;
            l_y = 0;
            t_x = 0;
            t_y = 0;
            tile_l = 0;
            tile_t = 0;
            tileDistance = 0;
            UserInput.Clear();
            Output.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChange([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}