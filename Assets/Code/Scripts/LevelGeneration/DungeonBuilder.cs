﻿using System.Collections.Generic;
using System.Linq;
public class DungeonBuilder
{
    
    List<RoomNode> nodes = new List<RoomNode>();
    private int dungeonWidth;
    private int dungeonLength;

    public DungeonBuilder(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }



    public List<Node> buildRoomsAndCorridors(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomModifier, float roomTopModifier, int roomOffset, int corridorWidth, int tileSize)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        nodes = bsp.getSplitNodes(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = Helper.getLeaves(bsp.RootNode);

        RoomsBuilder roomGenerator = new RoomsBuilder();
        List<RoomNode> rooms = roomGenerator.buildRooms(roomSpaces, roomBottomModifier, roomTopModifier, roomOffset, tileSize);

        CorridorsBuilder corridorGenerator = new CorridorsBuilder();
        var corridors = corridorGenerator.buildCorridors(nodes, corridorWidth);
        
        return new List<Node>(rooms).Concat(corridors).ToList();
    }
}