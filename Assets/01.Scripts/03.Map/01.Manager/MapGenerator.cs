using MyGame.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    // 방 위치(cell 좌표)와 RoomData를 연결하는 맵
    private Dictionary<Vector2Int, RoomData> roomMap = new();

    private int maxRooms;           // 생성할 최대 방 개수
    private int tilePerCellX;       // 셀 한 칸당 가로 타일 개수
    private int tilePerCellY;       // 셀 한 칸당 세로 타일 개수

    // 최소/최대 셀 크기 (셀 단위, 즉 방 크기 범위)
    private readonly Vector2Int minCellSize = new(3, 3);
    private readonly Vector2Int maxCellSize = new(4, 4);

    // 상하좌우 4방향 벡터
    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // 문 근처 장애물 배치 방지를 위한 거리 임계값 (하드코딩 되어 있음)
    private const float doorProximityThreshold = 1.5f;

    // 생성자: 방 한 셀 당 타일 크기 설정
    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // 맵 생성 메인 함수
    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int startPos = Vector2Int.zero; // 시작 방 위치 (0,0)

        // 시작 방 생성 (type: Start, 크기 2x2 셀)
        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

        // BFS 스타일로 방을 확장하며 최대 maxRooms까지 생성
        while (roomMap.Count < maxRooms && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (roomMap.ContainsKey(next)) continue;      // 이미 방이 있으면 건너뜀
                if (Random.value > 0.5f) continue;            // 50% 확률로 방 생성 안함

                // 전투 방 생성, 크기는 랜덤 셀 크기
                RoomData room = CreateRoom(RoomType.Combat, next, GetRandomCellSize());
                roomMap[next] = room;
                queue.Enqueue(next);

                // 현재 방과 새로 생성된 방 연결(문 추가)
                ConnectRooms(roomMap[current], room, dir);

                if (roomMap.Count >= maxRooms) break;
            }
        }

        // 모든 방에 장애물 생성
        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    // 방 생성 함수: 타입, 셀 위치, 셀 크기를 받아 RoomData 생성
    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        // 타일 크기는 셀 크기에 타일 개수를 곱해서 계산
        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        RoomData room = new RoomData
        {
            type = type,
            cellPosition = cellPos,
            cellSize = cellSize,
            tileSize = tileSize
        };

        // 전투방이면 중앙에 적 스폰 포인트 추가
        if (type == RoomType.Combat)
        {
            Vector2 center = new Vector2(tileSize.x, tileSize.y) * 0.5f;
            room.enemySpawnPoints.Add(center);
        }

        return room;
    }

    // 최소/최대 셀 크기 사이에서 랜덤한 셀 크기 반환
    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    // 두 방을 연결하며 각 방에 문 정보 추가
    private void ConnectRooms(RoomData roomA, RoomData roomB, Vector2Int dir)
    {
        // 방 크기와 방향에 따른 문 위치 계산 함수 (로컬 좌표)
        Vector2Int GetDoorPos(Vector2Int size, Vector2Int d) => d switch
        {
            var v when v == Vector2Int.up => new Vector2Int(size.x / 2, size.y - 1),
            var v when v == Vector2Int.down => new Vector2Int(size.x / 2, 0),
            var v when v == Vector2Int.left => new Vector2Int(0, size.y / 2),
            var v when v == Vector2Int.right => new Vector2Int(size.x - 1, size.y / 2),
            _ => new Vector2Int(size.x / 2, size.y / 2)
        };

        // 방 A 문 정보 추가 (연결 방향 및 상대 방 위치 포함)
        roomA.doors.Add(new DoorData
        {
            localPosition = GetDoorPos(roomA.tileSize, dir),
            direction = dir,
            connectedRoomPos = roomB.cellPosition
        });

        // 방 B 문 정보 추가 (반대 방향, 연결된 방 A 위치 포함)
        roomB.doors.Add(new DoorData
        {
            localPosition = GetDoorPos(roomB.tileSize, -dir),  
            direction = -dir,
            connectedRoomPos = roomA.cellPosition
        });
    }

    // 장애물 배치 함수
    private void GenerateObstacles(RoomData room) 
    {
        room.obstaclePositions.Clear();

        // 시작/아이템/보스/상점 방은 장애물 배치 안함
        if (room.type is RoomType.Start or RoomType.Item or RoomType.Boss or RoomType.Shop)
            return;

        Vector2Int size = room.tileSize;
        List<Vector2Int> potentialPositions = new();

        // 장애물을 배치할 수 있는 후보 위치들을 방 크기의 30% 지점 기준으로 계산
        int offsetX = Mathf.Clamp(Mathf.RoundToInt(size.x * 0.3f), 2, size.x - 3);
        int offsetY = Mathf.Clamp(Mathf.RoundToInt(size.y * 0.3f), 2, size.y - 3);

        // 4개의 코너 주변 위치 후보 추가
        potentialPositions.Add(new Vector2Int(offsetX, offsetY));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, offsetY));
        potentialPositions.Add(new Vector2Int(offsetX, size.y - offsetY - 1));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, size.y - offsetY - 1));

        // 방이 충분히 크면 중앙 위치도 후보 추가
        if (size.x >= 20 && size.y >= 20)
            potentialPositions.Add(new Vector2Int(size.x / 2, size.y / 2));

        // 문 근처 위치는 후보에서 제거하여 장애물이 문 바로 옆에 안 생기게 함
        potentialPositions.RemoveAll(pos => IsNearDoor(pos, room.doors.Select(d => d.localPosition)));

        // 2~4개 장애물 개수 결정, 후보 위치 중 랜덤하게 선택하여 추가
        int count = Mathf.Min(Random.Range(2, 5), potentialPositions.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, potentialPositions.Count);
            room.obstaclePositions.Add(potentialPositions[index]);
            potentialPositions.RemoveAt(index);
        }
    }

    // 주어진 위치가 문 위치들과 가까운지 검사 (임계값 doorProximityThreshold 사용)
    private bool IsNearDoor(Vector2Int pos, IEnumerable<Vector2Int> doorPositions)
    {
        foreach (var door in doorPositions)
        {
            if (Vector2Int.Distance(pos, door) < doorProximityThreshold)
                return true;
        }
        return false;
    }
}
