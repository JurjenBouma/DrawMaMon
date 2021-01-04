using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DrawMaMon.Structs;
using System.IO;

namespace DrawMaMon
{
    class GameFiles
    {
        string m_filePath;
        string m_worldName;
        string m_nameSaveFile;

        TileMaps tileMaps;
        Map worldMap;
        const int m_regionLength = 100;

        public GameFiles(Texture2D emptyTileTex, Size windowSize, Size numTilesScreen,string filePath)
        {
            m_filePath = filePath;
            
            if (!Directory.Exists(m_filePath))
                Directory.CreateDirectory(m_filePath);

            tileMaps = new TileMaps(emptyTileTex,
                    windowSize, numTilesScreen);
        }
        public TileMaps GetTileMaps() { return tileMaps; }
        public Map GetMap() { return worldMap; }
        public void SetNameSaveFile(string name) { m_nameSaveFile = name; }
        public string GetNameSaveFile() { return m_nameSaveFile; }
        public string GetWorldName() { return m_worldName; }
        public void SetFilePath(string filePath) { m_filePath = filePath; }
        public string GetSaveFolder()
        {
            if (!Directory.Exists(m_filePath + "\\SavedGames"))
                Directory.CreateDirectory(m_filePath + "\\SavedGames");
            return m_filePath +  "\\SavedGames";
        }
        public string GetWorldFolder()
        {
            if (!Directory.Exists(m_filePath + "\\Worlds"))
                Directory.CreateDirectory(m_filePath + "\\Worlds");
            return m_filePath + "\\Worlds"; 
        }

        public void NewGame(GraphicsDevice device, Size numTilesScreen, int nLayers, Size mapSize, string saveFileName, string worldName)
        {
            bool newWorld = true;
            m_worldName = saveFileName;
            foreach (string folder in Directory.GetDirectories(GetWorldFolder()))
            {
                if (Directory.Exists(folder + "\\Maps"))
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);
                    if (folderInfo.Name == worldName)
                    {
                        newWorld = false;
                        m_worldName = worldName;
                    }
                }
            }
            if (newWorld)
                worldMap = new Map(mapSize.Width, mapSize.Height, device, tileMaps.TilePixSize(), numTilesScreen, nLayers);
            else
            {
                LoadTileMaps(device, nLayers);
                worldMap = LoadMap(device, numTilesScreen, tileMaps.TilePixSize(), nLayers);
            }
            m_nameSaveFile = saveFileName;
        }

        public void LoadGame(GraphicsDevice device,Player player, Size numTilesScreen, int nLayers,string saveName)
        {
            m_nameSaveFile = saveName;
            LoadEnvironment();
            LoadPlayer(player);
            LoadTileMaps(device, nLayers);
            worldMap = LoadMap(device,numTilesScreen,tileMaps.TilePixSize(),nLayers);
        }
    
        void LoadEnvironment()
        {
            if (!Directory.Exists(m_filePath + "\\SavedGames\\" + m_nameSaveFile))
                Directory.CreateDirectory(m_filePath + "\\SavedGames\\" + m_nameSaveFile);

            Stream iStream = new FileStream(m_filePath + "\\SavedGames\\" + m_nameSaveFile + "\\World.world", FileMode.Open);
            BinaryReader streamReader = new BinaryReader(iStream);
            m_worldName = streamReader.ReadString();
            iStream.Close();
        }

        void LoadPlayer(Player player)
        {
            if (!Directory.Exists(m_filePath + "\\SavedGames\\" + m_nameSaveFile))
                Directory.CreateDirectory(m_filePath + "\\SavedGames\\" + m_nameSaveFile);

            Stream iStream = new FileStream(m_filePath + "\\SavedGames\\" + m_nameSaveFile + "\\Player.player", FileMode.Open);
            BinaryReader streamReader = new BinaryReader(iStream);

            player.SetPosition(new Point(streamReader.ReadInt32(), streamReader.ReadInt32()));
            player.SetDisplayTile(new Point(streamReader.ReadInt32(),streamReader.ReadInt32()));
            iStream.Close();
        }

        void LoadTileMaps(GraphicsDevice device,int nLayers)
        {
            foreach (string file in Directory.GetFiles(m_filePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".png")
                {
                    Stream iStream = new FileStream(file,FileMode.Open);
                    Texture2D tileMap = Texture2D.FromStream(device, iStream);
                    tileMaps.AddTileMap(tileMap);
                    iStream.Close();
                }
            }
        }

        Map LoadMap(GraphicsDevice device, Size numScreenTiles, int tilePixSize, int nLayers)
        {
            if (!Directory.Exists(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Maps01"))
                Directory.CreateDirectory(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01");

            Stream iStreamHeader = new FileStream(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01\\" + "Map01.info", FileMode.Open);
            BinaryReader streamReaderHeader = new BinaryReader(iStreamHeader);


            int mapWidth = streamReaderHeader.ReadInt16();
            int mapHeight = streamReaderHeader.ReadInt16();
            iStreamHeader.Close();

            Map loadMap = new Map(mapWidth, mapHeight, device, tilePixSize, numScreenTiles, nLayers);

            float numRegionsX = mapWidth / (float)m_regionLength;
            float numRegionsY = mapHeight / (float)m_regionLength;

            for (int rX = 0; rX < numRegionsX; rX++)
            {
                int regionWidth = m_regionLength;
                if (rX == (int)numRegionsX)
                {
                    regionWidth = mapWidth % m_regionLength;
                }

                for (int rY = 0; rY < numRegionsY; rY++)
                {
                    int regionHeight = m_regionLength;
                    if (rY == (int)numRegionsY)
                    {
                        regionHeight =mapHeight % m_regionLength;
                    }

                    Stream iStreamRegions = new FileStream(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01\\" + rX.ToString() + "," + rY.ToString() + ".region", FileMode.Open);
                    BinaryReader streamReaderRegions = new BinaryReader(iStreamRegions);
                    for (int l = 0; l < nLayers; l++)
                    {
                        int totalTiles = 0;
                        while (totalTiles < regionWidth * regionHeight)
                        {
                            int equalTileCount = streamReaderRegions.ReadInt16();
                            int mapNum = streamReaderRegions.ReadByte() - 1;

                            for (int i = 0; i < equalTileCount; i++)
                            {
                                int x = (totalTiles + i) / regionHeight;
                                int y = (totalTiles + i) - x * regionHeight;
                                loadMap.SetTile(x + rX*m_regionLength, y + rY*m_regionLength, Point.Zero, mapNum, l);
                            }
                            totalTiles += equalTileCount;
                        }
                    }
                    for (int l = 0; l < nLayers; l++)
                    {
                        int totalTiles = 0;
                        while (totalTiles < regionWidth * regionHeight)
                        {
                            int equalTileCount = streamReaderRegions.ReadInt16();
                            Point tilePos = new Point(streamReaderRegions.ReadByte(), streamReaderRegions.ReadByte());

                            for (int i = 0; i < equalTileCount; i++)
                            {
                                int x = (totalTiles + i) / regionHeight;
                                int y = (totalTiles + i) - x * regionHeight;
                                int mapNum = loadMap.GetTile(x+rX * m_regionLength, y+ rY*m_regionLength).tileLayers[l].NumTileMap;
                                loadMap.SetTile(x + rX * m_regionLength, y + rY * m_regionLength, tilePos, mapNum, l);
                            }
                            totalTiles += equalTileCount;
                        }
                    }
                    iStreamRegions.Close();
                }
            }
        
            return loadMap;
        }

        public void SaveWorld(int nLayers)
        {
            SaveTileMaps();
            SaveMap(nLayers);
        }

        public void SaveGame(Player player)
        {
            SaveEnvironment();
            SavePlayer(player);
        }
        void SaveEnvironment()
        {
            if (!Directory.Exists(m_filePath + "\\SavedGames\\" + m_nameSaveFile))
                Directory.CreateDirectory(m_filePath + "\\SavedGames\\" + m_nameSaveFile);

            Stream oStream = new FileStream(m_filePath + "\\SavedGames\\" + m_nameSaveFile + "\\World.world", FileMode.Create);
            BinaryWriter streamWriter = new BinaryWriter(oStream);
            streamWriter.Write(m_worldName);
            oStream.Close();
        }

        void SavePlayer(Player player)
        {
            if (!Directory.Exists(m_filePath + "\\SavedGames\\" + m_nameSaveFile))
                Directory.CreateDirectory(m_filePath + "\\SavedGames\\" + m_nameSaveFile);

            Stream oStream = new FileStream(m_filePath + "\\SavedGames\\" + m_nameSaveFile + "\\Player.player", FileMode.Create);
            BinaryWriter streamWriter = new BinaryWriter(oStream);
            int tilePixSize = tileMaps.TilePixSize();
            streamWriter.Write(player.GetPosition().X / tilePixSize * tilePixSize);
            streamWriter.Write(player.GetPosition().Y / tilePixSize * tilePixSize);
            streamWriter.Write(1);
            streamWriter.Write(player.GetDisplayTile().Y);
            oStream.Close();
        }

        void SaveTileMaps()
        {
            if (!Directory.Exists(m_filePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
                Directory.CreateDirectory(m_filePath + "\\Worlds\\" + m_worldName + "\\TileMaps");
            foreach (string file in Directory.GetFiles(m_filePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
            {
                  FileInfo fileInfo = new FileInfo(file);
                  if (fileInfo.Name.Contains("tilemap"))
                  {
                      File.Delete(file);
                  }
            }
            for (int i = 0; i < tileMaps.GetTileMapCount(); i++)
            {
                Stream oStream = new FileStream(m_filePath + "\\Worlds\\" + m_worldName + "\\TileMaps\\tilemap" + i.ToString() + ".png", FileMode.Create);
                Texture2D tileMap = tileMaps.GetTileMap(i);
                tileMap.SaveAsPng(oStream, tileMap.Width, tileMap.Height);
                oStream.Close();
            }
        }

        void SaveMap(int nLayers)
        {
            if (!Directory.Exists(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Maps01"))
                Directory.CreateDirectory(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01");

            Stream oStreamHeader = new FileStream(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01\\" + "Map01.info", FileMode.Create);
            BinaryWriter streamWriterHeader = new BinaryWriter(oStreamHeader);

            streamWriterHeader.Write(Convert.ToInt16(worldMap.Width()));
            streamWriterHeader.Write(Convert.ToInt16(worldMap.Height()));
            streamWriterHeader.Close();

            float numRegionsX = worldMap.Width() / (float)m_regionLength;
            float numRegionsY = worldMap.Height() / (float)m_regionLength;

            for (int rX = 0; rX < numRegionsX; rX++)
            {
                int regionWidth = m_regionLength;
                if (rX == (int)numRegionsX)
                {
                    regionWidth = worldMap.Width()%m_regionLength;
                }

                for (int rY = 0; rY < numRegionsY; rY++)
                {
                    int regionHeight = m_regionLength;
                    if (rY == (int)numRegionsY)
                    {
                        regionHeight =worldMap.Height() % m_regionLength;
                    }

                    Stream oStreamRegions = new FileStream(m_filePath + "\\Worlds\\" + m_worldName + "\\Maps" + "\\Map01\\" + rX.ToString() + "," + rY.ToString() + ".region", FileMode.Create);
                    BinaryWriter streamWriterRegions = new BinaryWriter(oStreamRegions);
                    for (int l = 0; l < nLayers; l++)            
                    {
                        int equalMapNumCount = 0;
                        int prevMapNum = 1000;

                        for (int x = 0; x < regionWidth ; x++)
                        {
                            for (int y = 0; y < regionHeight; y++)
                            {
                                Tile mapTile = worldMap.GetTile(x + rX * m_regionLength, y + rY *m_regionLength);
                                if (prevMapNum == 1000)
                                {
                                    prevMapNum = mapTile.tileLayers[l].NumTileMap;
                                    equalMapNumCount = 1;
                                }
                                else if (mapTile.tileLayers[l].NumTileMap != prevMapNum)
                                {
                                    streamWriterRegions.Write(Convert.ToInt16(equalMapNumCount));
                                    streamWriterRegions.Write(Convert.ToByte(prevMapNum + 1));
                                    prevMapNum = mapTile.tileLayers[l].NumTileMap;
                                    equalMapNumCount = 1;
                                }
                                else if (mapTile.tileLayers[l].NumTileMap == prevMapNum)
                                {
                                    equalMapNumCount++;
                                }

                            }
                        }
                        streamWriterRegions.Write(Convert.ToInt16(equalMapNumCount));
                        streamWriterRegions.Write(Convert.ToByte(prevMapNum + 1));
                    }


                    for (int l = 0; l < nLayers; l++)
                    {
                        int equalMapNumCount = 0;
                        Point prevTilePosition = new Point(-1000, -1000);

                        for (int x = 0; x < regionWidth; x++)
                        {
                            for (int y = 0; y < regionHeight; y++)
                            {
                                Tile mapTile = worldMap.GetTile(x + rX * m_regionLength, y + rY * m_regionLength);
                                if (prevTilePosition.X == -1000)
                                {
                                    prevTilePosition = mapTile.tileLayers[l].Position;
                                    equalMapNumCount = 1;
                                }
                                else if (mapTile.tileLayers[l].Position != prevTilePosition)
                                {
                                    streamWriterRegions.Write(Convert.ToInt16(equalMapNumCount));
                                    streamWriterRegions.Write(Convert.ToByte(prevTilePosition.X));
                                    streamWriterRegions.Write(Convert.ToByte(prevTilePosition.Y));
                                    prevTilePosition = mapTile.tileLayers[l].Position;
                                    equalMapNumCount = 1;
                                }
                                else if (mapTile.tileLayers[l].Position == prevTilePosition)
                                {
                                    equalMapNumCount++;
                                }

                            }
                        }
                        streamWriterRegions.Write(Convert.ToInt16(equalMapNumCount));
                        streamWriterRegions.Write(Convert.ToByte(prevTilePosition.X));
                        streamWriterRegions.Write(Convert.ToByte(prevTilePosition.Y));
                    }
                    oStreamRegions.Close();
                }
            }
  
        }
    }
}
