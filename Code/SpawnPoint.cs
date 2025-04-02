using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ForkliftGame
{
public partial class SpawnPoint : Node2D
{

	private Vector2 _spawningPoint;
	public Vector2 SpawningPoint {
		get { return _spawningPoint; }
	}
	public List<Vector2> _spawnerList = new List<Vector2>();
	public List<Vector2> _loadingAreaSpawnerList = new List<Vector2>();
	public List<Vector2> _boxOnTruckList = new List<Vector2>();
	public int randomLoadingAreaIndex = 0;
	Vector2[,] levelZero = {
		{new Vector2(1367, -82), new Vector2(1367, -200), new Vector2(1367, -316)},
		{new Vector2(1367, -82), new Vector2(1367, -200), new Vector2(1367, -316)},
		{new Vector2(1367, -82), new Vector2(1367, -200), new Vector2(1367, -316)}
	};
	Vector2[,] levelOne = {
		{new Vector2(-87, 718), new Vector2(-203, 718), new Vector2(-331, 718)},
		{new Vector2(1418, -86), new Vector2(1418, -204), new Vector2(1418, -331)},
		{new Vector2(-90, 2072), new Vector2(-208, 2072), new Vector2(-335, 2072)}
	};
	Vector2[,] levelTwo = {
		{new Vector2(2565, 2324), new Vector2(2565, 2449), new Vector2(2565, 2582)},
		{new Vector2(-89, 1437), new Vector2(-211, 1437), new Vector2(-339, 1437)},
		{new Vector2(1370, -327), new Vector2(1370, -198), new Vector2(1370, -81)}
	};
	Vector2[,] levelThree = {
		{new Vector2(2647, 2316), new Vector2(2647, 2450), new Vector2(2647, 2577)},
		{new Vector2(1438, -90), new Vector2(1438, -217), new Vector2(1438, -351)},
		{new Vector2(-86, 2080), new Vector2(-216, 2080), new Vector2(-345, 2080)}
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{


		_spawningPoint = this.Position;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void fillSpawnerList(int level)
	{
		switch (level)
		{
			case 0:
			_spawnerList.Insert(0, new Vector2 (1406, 587));
			_spawnerList.Insert(0, new Vector2 (2076, 604));
			_spawnerList.Insert(0, new Vector2 (976, 407));
			_spawnerList.Insert(0, new Vector2 (124, 584));
			_spawnerList.Insert(0, new Vector2 (1743, 379));
			break;

			case 1:
			_spawnerList.Insert(0, new Vector2 (969, 120));
			_spawnerList.Insert(0, new Vector2 (171, 1113));
			_spawnerList.Insert(0, new Vector2 (2831, 2132));
			_spawnerList.Insert(0, new Vector2 (1371, 2096));
			_spawnerList.Insert(0, new Vector2 (2180, 1472));
			_spawnerList.Insert(0, new Vector2 (2653, 146));
			_spawnerList.Insert(0, new Vector2 (1713, 514));
			_spawnerList.Insert(0, new Vector2 (3003, 1071));
			_spawnerList.Insert(0, new Vector2 (1551, 1319));
			_spawnerList.Insert(0, new Vector2 (165, 161));
			break;

			case 2:
			_spawnerList.Insert(0, new Vector2 (965, 425));
			_spawnerList.Insert(0, new Vector2 (1770, 982));
			_spawnerList.Insert(0, new Vector2 (2165, 2114));
			_spawnerList.Insert(0, new Vector2 (795, 795));
			_spawnerList.Insert(0, new Vector2 (1358, 1162));
			_spawnerList.Insert(0, new Vector2 (2244, 115));
			_spawnerList.Insert(0, new Vector2 (577, 2090));
			_spawnerList.Insert(0, new Vector2 (2559, 1125));
			_spawnerList.Insert(0, new Vector2 (2172, 473));
			_spawnerList.Insert(0, new Vector2 (174, 718));
			break;

			case 3:
			_spawnerList.Insert(0, new Vector2 (213, 848));
			_spawnerList.Insert(0, new Vector2 (171, 1281));
			_spawnerList.Insert(0, new Vector2 (1083, 629));
			_spawnerList.Insert(0, new Vector2 (1495, 1195));
			_spawnerList.Insert(0, new Vector2 (1447, 1692));
			_spawnerList.Insert(0, new Vector2 (1831, 2076));
			_spawnerList.Insert(0, new Vector2 (2173, 426));
			_spawnerList.Insert(0, new Vector2 (2200, 752));
			_spawnerList.Insert(0, new Vector2 (2627, 1035));
			_spawnerList.Insert(0, new Vector2 (2985, 170));
			break;

		}
	}

	public void fillLoadingAreaSpawnerList(int level)
	{
		switch (level)
		{
			case 0:
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1346, 158));

			break;

			case 1:
			_loadingAreaSpawnerList.Insert(0, new Vector2 (173, 716));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1351, 161));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (159, 2073));

			break;

			case 2:
			_loadingAreaSpawnerList.Insert(0, new Vector2 (2555, 2079));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (162, 1442));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1379, 181));
			break;

			case 3:
			_loadingAreaSpawnerList.Insert(0, new Vector2 (2634, 2083));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1436, 155));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (164, 2082));



			break;
		}
	}

	public void fillTruckSpawnerList(int level, int randomLoadingAreaIndex)
	{
		switch (level)
		{
			case 0:
				_boxOnTruckList.Insert(0, levelZero[randomLoadingAreaIndex, 0]);
				_boxOnTruckList.Insert(0, levelZero[randomLoadingAreaIndex, 1]);
				_boxOnTruckList.Insert(0, levelZero[randomLoadingAreaIndex, 2]);

				break;

			case 1:
				_boxOnTruckList.Insert(0, levelOne[randomLoadingAreaIndex, 0]);
				_boxOnTruckList.Insert(0, levelOne[randomLoadingAreaIndex, 1]);
				_boxOnTruckList.Insert(0, levelOne[randomLoadingAreaIndex, 2]);
				break;

			case 2:
				_boxOnTruckList.Insert(0, levelTwo[randomLoadingAreaIndex, 0]);
				_boxOnTruckList.Insert(0, levelTwo[randomLoadingAreaIndex, 1]);
				_boxOnTruckList.Insert(0, levelTwo[randomLoadingAreaIndex, 2]);
				break;
		}
	}





	public void ClearSpawnerList()
	{

		_spawnerList.Clear();

	}

	public void ClearLoadingAreaSpawnerList()
	{
		_loadingAreaSpawnerList.Clear();
	}

	public Vector2 GetRandomPosition()
	{
		int randomIndex = GD.RandRange(0 ,_spawnerList.Count - 1);
		Vector2 spawnPoint = _spawnerList[randomIndex];
		_spawnerList.RemoveAt(randomIndex);
		return spawnPoint;
	}
	public Vector2 GetRandomLoadingAreaPosition()
	{
		randomLoadingAreaIndex = GD.RandRange(0 ,_loadingAreaSpawnerList.Count - 1);
		Vector2 spawnPoint = _loadingAreaSpawnerList[randomLoadingAreaIndex];
		_loadingAreaSpawnerList.RemoveAt(randomLoadingAreaIndex);
		return spawnPoint;
	}


}
}
