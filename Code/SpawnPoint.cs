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
			_spawnerList.Insert(0, new Vector2 (429, 107));
			_spawnerList.Insert(0, new Vector2 (436, 652));
			_spawnerList.Insert(0, new Vector2 (127, 1158));
			_spawnerList.Insert(0, new Vector2 (1290, 724));
			_spawnerList.Insert(0, new Vector2 (2075, 156));
			break;

			case 1:
			_spawnerList.Insert(0, new Vector2 (429, 107));
			_spawnerList.Insert(0, new Vector2 (436, 652));
			_spawnerList.Insert(0, new Vector2 (127, 1158));
			_spawnerList.Insert(0, new Vector2 (1290, 724));
			_spawnerList.Insert(0, new Vector2 (2075, 156));
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
