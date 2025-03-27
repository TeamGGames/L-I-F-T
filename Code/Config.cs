using Godot;
using System;

namespace ForkliftGame;
/// <summary>
/// Config file for input control for the whole project.
/// </summary>
public static class Config
{
    public const string GrabAction = "grab";
    public const string ReleaseAction = "release";
    public const string GearOne = "gearOne";
    public const string GearTwo = "gearTwo";
    public const string GearThree = "gearThree";
    public const string TurnLeftAction = "left";
    public const string TurnRightAction = "right";
    public const string ReverseAction = "reverse";
    public const string ForwardAction = "forward";
    public const string Level0 = "res://Levels/LEVEL0.tscn";
    public const string Level1 = "res://Levels/LEVEL1.tscn";
    public const string Level2 = "res://Levels/LEVEL2.tscn";
    public const string HighScoreFile = "user://highscore.csv";
    public const string SaveFolder = "Save";
    public const string QuickSaveFile = "quicksave.json";

}