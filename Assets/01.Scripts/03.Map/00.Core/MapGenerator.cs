using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class MapGenerator
{
    private Dictionary<Vector2Int, RoomData> roomMap = new();
    private int maxRooms;

    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private int tilePerCellX;
    private int tilePerCellY;
    private readonly Vector2Int minCellSize = new(2, 2);
    private readonly Vector2Int maxCellSize = new(3, 3);

    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // 최대 방 개수에 맞춰 맵 생성
    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();
        Vector2Int startPos = Vector2Int.zero;

        // 시작 방 생성(크기 고정)
        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

        // BFS 방식으로 주변 방 생성
        while (roomMap.Count < maxRooms && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (roomMap.ContainsKey(next)) continue;
                if (Random.value > 0.5f) continue;

                RoomData room = CreateRoom(RoomType.Combat, next, GetRandomCellSize());
                roomMap[next] = room;
                queue.Enqueue(next);

                if (roomMap.Count >= maxRooms) break;
            }
        }

        // 이웃 정보 및 문 위치 설정
        SetNeighborsAndDoors();

        // 장애물(기둥) 위치 생성
        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    // 방 생성 및 타일 크기 계산
    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        if (type == RoomType.Start)
            cellSize = new Vector2Int(2, 2);

        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        return new RoomData
        {
            type = type,
            cellPosition = cellPos,
            cellSize = cellSize,
            tileSize = tileSize
        };
    }

    // 셀 크기 랜덤 결정
    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    // 이웃 방향 및 문 위치 계산
    private void SetNeighborsAndDoors()
    {
        foreach (var room in roomMap.Values)
        {
            room.neighborDirs.Clear();
            room.doorLocalPositions.Clear();

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = room.cellPosition + dir;
                if (roomMap.TryGetValue(neighborPos, out var neighbor))
                {
                    room.neighborDirs.Add(dir);
                    var doorPos = GetDoorLocalPosition(room.tileSize, dir);
                    room.doorLocalPositions.Add(doorPos);
                }
            }
        }
    }

    // 문 위치 계산 (방 타일 크기 기준)
    private Vector2Int GetDoorLocalPosition(Vector2Int tileSize, Vector2Int dir)
    {
        int centerX = tileSize.x / 2;
        int centerY = tileSize.y / 2;

        return dir switch
        {
            var d when d == Vector2Int.up => new Vector2Int(centerX, tileSize.y - 1),
            var d when d == Vector2Int.down => new Vector2Int(centerX, 0),
            var d when d == Vector2Int.left => new Vector2Int(0, centerY),
            var d when d == Vector2Int.right => new Vector2Int(tileSize.x - 1, centerY),
            _ => new Vector2Int(centerX, centerY)
        };
    }

    // 장애물(기둥) 위치 랜덤 생성, 중복 및 문 위치 제외 처리
    private void GenerateObstacles(RoomData room)  // 기존 장애물 위치 초기화
    {
        room.obstaclePositions.Clear();

        int numObstacles = Random.Range(2, 5); // 2~4개 사이의 장애물 수를 무작위 생성
        Vector2Int center = new(room.tileSize.x / 2, room.tileSize.y / 2); // 방의 중앙 좌표 계산

        int attempts = 0;
        int maxAttempts = 30; // 무한 루프 방지용 제한 (충돌/조건 때문에 장애물 못 넣을 수 있으므로)

        while (room.obstaclePositions.Count < numObstacles && attempts < maxAttempts)
        {
            // 방 내부 (테두리 제외)에서 랜덤 좌표 생성
            Vector2Int pos = new Vector2Int(
                Random.Range(1, room.tileSize.x - 1),
                Random.Range(1, room.tileSize.y - 1)
            );

            // 중앙과 너무 가까우면 생략 (플레이어 스폰 영역 확보용)
            if (Vector2Int.Distance(pos, center) < 2f) { attempts++; continue; }

            // 문 위치 또는 문 근처(1.5칸 이내)면 생략 (출입문 막지 않도록)
            if (IsNearDoor(pos, room.doorLocalPositions)) { attempts++; continue; }

            // 이미 같은 위치에 장애물이 있으면 생략 (중복 제거)
            if (room.obstaclePositions.Contains(pos)) { attempts++; continue; }

            // 위 조건 다 통과하면 장애물 위치로 등록
            room.obstaclePositions.Add(pos);
            attempts++;
        }
    }

    // 문 주변 1.5칸 이내 거리면 true
    private bool IsNearDoor(Vector2Int pos, List<Vector2Int> doors)
    {
        foreach (var door in doors)
        {
            if (Vector2Int.Distance(pos, door) < 1.5f)
                return true;
        }
        return false;
    }
}
