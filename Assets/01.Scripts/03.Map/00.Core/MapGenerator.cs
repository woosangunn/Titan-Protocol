using UnityEngine;
using System.Collections.Generic;
using MyGame.Map;

public class MapGenerator
{
    // 생성된 방들을 저장하는 맵 (key: 셀 좌표, value: 방 데이터)
    private Dictionary<Vector2Int, RoomData> roomMap = new();

    // 생성할 최대 방 수
    private int maxRooms;

    // 방 생성 방향 (상, 하, 좌, 우)
    private static readonly List<Vector2Int> directions = new()
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // 셀 하나당 타일 수 (크기 조절용)
    private int tilePerCellX;
    private int tilePerCellY;

    // 셀 크기의 최소/최대 범위
    private readonly Vector2Int minCellSize = new(3, 3);
    private readonly Vector2Int maxCellSize = new(4, 4); // 더 큰 방을 만들기 위해 확장 가능

    // 생성자: 셀 하나의 타일 수 지정
    public MapGenerator(int roomUnitSize)
    {
        tilePerCellX = roomUnitSize;
        tilePerCellY = roomUnitSize;
    }

    // 전체 맵 생성 함수 (최대 방 수만큼 생성)
    public Dictionary<Vector2Int, RoomData> GenerateMap(int maxRooms)
    {
        this.maxRooms = maxRooms;
        roomMap.Clear();

        Queue<Vector2Int> queue = new();             // BFS 처리를 위한 큐
        Vector2Int startPos = Vector2Int.zero;       // 시작 방 위치

        // 시작 방 생성 (크기 고정)
        RoomData startRoom = CreateRoom(RoomType.Start, startPos, new Vector2Int(2, 2));
        roomMap[startPos] = startRoom;
        queue.Enqueue(startPos);

        // BFS 방식으로 인접한 방 생성
        while (roomMap.Count < maxRooms && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                // 이미 생성된 위치는 스킵
                if (roomMap.ContainsKey(next)) continue;

                // 랜덤 확률로 방 생성 여부 결정
                if (Random.value > 0.5f) continue;

                // 새로운 전투 방 생성
                RoomData room = CreateRoom(RoomType.Combat, next, GetRandomCellSize());
                roomMap[next] = room;
                queue.Enqueue(next);

                // 최대 방 수 도달 시 중단
                if (roomMap.Count >= maxRooms) break;
            }
        }

        // 각 방의 이웃 방향과 문 위치 계산
        SetNeighborsAndDoors();

        // 각 방에 장애물 위치 생성
        foreach (var room in roomMap.Values)
        {
            GenerateObstacles(room);
        }

        return roomMap;
    }

    // 방 생성 함수 (타일 크기 계산 포함)
    private RoomData CreateRoom(RoomType type, Vector2Int cellPos, Vector2Int cellSize)
    {
        // 셀 크기를 기반으로 실제 타일 크기 계산
        Vector2Int tileSize = new(cellSize.x * tilePerCellX, cellSize.y * tilePerCellY);

        return new RoomData
        {
            type = type,
            cellPosition = cellPos,     // 맵 좌표
            cellSize = cellSize,        // 셀 크기 (기준 단위)
            tileSize = tileSize         // 실제 타일 크기
        };
    }

    // 셀 크기를 랜덤으로 결정 (min~max 범위 내)
    private Vector2Int GetRandomCellSize()
    {
        int w = Random.Range(minCellSize.x, maxCellSize.x + 1);
        int h = Random.Range(minCellSize.y, maxCellSize.y + 1);
        return new Vector2Int(w, h);
    }

    // 방 간 연결 정보 및 문 위치 설정
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
                    // 이웃 방향 등록
                    room.neighborDirs.Add(dir);

                    // 해당 방향에 문 위치 추가
                    var doorPos = GetDoorLocalPosition(room.tileSize, dir);
                    room.doorLocalPositions.Add(doorPos);
                }
            }
        }
    }

    // 문 위치 계산 함수 (방의 타일 크기와 방향을 기준으로 중앙 문 위치 설정)
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
            _ => new Vector2Int(centerX, centerY) // 기본값 (중앙)
        };
    }

    // 방 내부에 기둥 장애물 생성 (문과 겹치지 않도록)
    private void GenerateObstacles(RoomData room)
    {
        room.obstaclePositions.Clear();

        // 특수 방은 기둥 생성 제외
        if (room.type == RoomType.Start || room.type == RoomType.Item || room.type == RoomType.Boss || room.type == RoomType.Shop)
            return;

        Vector2Int size = room.tileSize;
        List<Vector2Int> potentialPositions = new();

        // 1. 네 구석에 기둥 후보 위치 추가 (30% 위치 기준)
        int offsetX = Mathf.RoundToInt(size.x * 0.3f);
        int offsetY = Mathf.RoundToInt(size.y * 0.3f);

        // 너무 모서리에 붙지 않도록 보정
        offsetX = Mathf.Clamp(offsetX, 2, size.x - 3);
        offsetY = Mathf.Clamp(offsetY, 2, size.y - 3);

        // 대칭 위치로 기둥 배치
        potentialPositions.Add(new Vector2Int(offsetX, offsetY));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, offsetY));
        potentialPositions.Add(new Vector2Int(offsetX, size.y - offsetY - 1));
        potentialPositions.Add(new Vector2Int(size.x - offsetX - 1, size.y - offsetY - 1));

        // 2. 큰 방이면 중앙 기둥도 추가
        if (size.x >= 20 && size.y >= 20)
            potentialPositions.Add(new Vector2Int(size.x / 2, size.y / 2));

        // 3. 문 위치와 가까운 후보 제거 (1.5칸 이내)
        potentialPositions.RemoveAll(pos => IsNearDoor(pos, room.doorLocalPositions));

        // 4. 최대 2~4개 랜덤 선택하여 기둥 확정
        int count = Mathf.Min(Random.Range(2, 5), potentialPositions.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, potentialPositions.Count);
            room.obstaclePositions.Add(potentialPositions[index]);
            potentialPositions.RemoveAt(index); // 중복 제거
        }
    }

    // 기둥이 문과 너무 가까운지 판단 (거리 < 1.5면 true)
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
