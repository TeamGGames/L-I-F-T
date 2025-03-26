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
			_spawnerList.Insert(0, new Vector2 (711, 1586));
			_spawnerList.Insert(0, new Vector2 (182, 151));
			_spawnerList.Insert(0, new Vector2 (1507, 1227));
			_spawnerList.Insert(0, new Vector2 (1684, 566));
			_spawnerList.Insert(0, new Vector2 (2620, 120));
			_spawnerList.Insert(0, new Vector2 (2843, 1049));
			_spawnerList.Insert(0, new Vector2 (2424, 2110));
			_spawnerList.Insert(0, new Vector2 (1725, 2151));
			break;

			case 1:
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

	public void ClearSpawnerList()
	{

		_spawnerList.Clear();

	}

	public Vector2 GetRandomPosition()
	{
		int randomIndex = GD.RandRange(0 ,_spawnerList.Count - 1);
		Vector2 spawnPoint = _spawnerList[randomIndex];
		_spawnerList.RemoveAt(randomIndex);
		return spawnPoint;
	}
}
}
