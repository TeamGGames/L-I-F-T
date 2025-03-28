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
		{new Vector2(1429, -295), new Vector2(1429, -189), new Vector2(1429, -77)},
		{new Vector2(1429, -295), new Vector2(1429, -189), new Vector2(1429, -77)},
		{new Vector2(1429, -295), new Vector2(1429, -189), new Vector2(1429, -77)}
	};
	Vector2[,] levelOne = {
		{new Vector2(-162, 764), new Vector2(-270, 764), new Vector2(-432, 764)},
		{new Vector2(1382, -151), new Vector2(1382, -268), new Vector2(1382, -394)},
		{new Vector2(-172, 2129), new Vector2(-322, 2129), new Vector2(-500, 2129)}
	};
	Vector2[,] levelTwo = {
		{new Vector2(2510, 2392), new Vector2(2510, 2504), new Vector2(2510, 2660)},
		{new Vector2(-187, 1397), new Vector2(-284, 1397), new Vector2(-451, 1397)},
		{new Vector2(1394, -155), new Vector2(1394, -267), new Vector2(1394, -412)}
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
			_spawnerList.Insert(0, new Vector2 (711, 1586));
			_spawnerList.Insert(0, new Vector2 (182, 151));
			_spawnerList.Insert(0, new Vector2 (1507, 1227));
			_spawnerList.Insert(0, new Vector2 (1684, 566));
			_spawnerList.Insert(0, new Vector2 (2620, 120));
			_spawnerList.Insert(0, new Vector2 (2843, 1049));
			_spawnerList.Insert(0, new Vector2 (2424, 2110));
			_spawnerList.Insert(0, new Vector2 (1725, 2151));
			break;

			case 2:
			_spawnerList.Insert(0, new Vector2 (156, 723));
			_spawnerList.Insert(0, new Vector2 (140, 2072));
			_spawnerList.Insert(0, new Vector2 (1387, 1739));
			_spawnerList.Insert(0, new Vector2 (2238, 773));
			_spawnerList.Insert(0, new Vector2 (2990, 1767));
			_spawnerList.Insert(0, new Vector2 (2923, 154));
			_spawnerList.Insert(0, new Vector2 (1772, 468));
			_spawnerList.Insert(0, new Vector2 (559, 2062));
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
			_loadingAreaSpawnerList.Insert(0, new Vector2 (159, 2073));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1351, 161));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (173, 716));
			break;

			case 2:
			_loadingAreaSpawnerList.Insert(0, new Vector2 (1346, 158));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (162, 1442));
			_loadingAreaSpawnerList.Insert(0, new Vector2 (2555, 2079));
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
