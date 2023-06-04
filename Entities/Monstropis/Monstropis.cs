using Godot;
using System;
using System.Collections.Generic;

public class Monstropis : Entity
{

    public override void _Ready()
    {
        base._Ready();
        this.ATKCOOLDOWN = 2;

        this.maxHP = 135;

        this.flippableAnim = true;

        this.animPerBeat = new byte[] { 4 };
        this.atkFolder = "res://zbeubzbeub";



        DOWNATK = new List<List<short[]>>
        {
            new List<short[]>
            {
                new short[] {  0 , 1 , 30 , 0 , 1 , 0 , 0},
                new short[] {  0 , 2 , 30 , 1 , 0 , 0 , 0},
                new short[] { -1 , 1 , 30 , 1 , 2 , 0 , 0},
                new short[] { -1 , 2 , 30 , 2 , 0 , 0 , 0},
                new short[] {  1 , 1 , 30 , 1 , 3 , 0 , 0},
                new short[] {  1 , 2 , 30 , 3 , 0 , 0 , 0}
            }
        };
        LEFTATK = new List<List<short[]>>
        {
            new List<short[]>
            {
                new short[] { -1 , 0 , 30 , 0 , 1 , 0 , 0},
                new short[] { -2 , 0 , 30 , 1 , 0 , 0 , 0},
                new short[] { -1 ,-1 , 30 , 1 , 2 , 0 , 0},
                new short[] { -2 ,-1 , 30 , 2 , 0 , 0 , 0},
                new short[] { -1 , 1 , 30 , 1 , 3 , 0 , 0},
                new short[] { -2 , 1 , 30 , 3 , 0 , 0 , 0}
            }
        };
        RIGHTATK = new List<List<short[]>>
        {
            new List<short[]>
            {
                new short[] {  1 , 0 , 30 , 0 , 1 , 0 , 0},
                new short[] {  2 , 0 , 30 , 1 , 0 , 0 , 0},
                new short[] {  1 ,-1 , 30 , 1 , 2 , 0 , 0},
                new short[] {  2 ,-1 , 30 , 2 , 0 , 0 , 0},
                new short[] {  1 , 1 , 30 , 1 , 3 , 0 , 0},
                new short[] {  2 , 1 , 30 , 3 , 0 , 0 , 0}
            }
        };
        UPATK = new List<List<short[]>>
        {
            new List<short[]>
            {
                new short[] {  0 ,-1 , 30 , 0 , 1 , 0 , 0},
                new short[] {  0 ,-2 , 30 , 1 , 0 , 0 , 0},
                new short[] { -1 ,-1 , 30 , 1 , 2 , 0 , 0},
                new short[] { -1 ,-2 , 30 , 2 , 0 , 0 , 0},
                new short[] {  1 ,-1 , 30 , 1 , 3 , 0 , 0},
                new short[] {  1 ,-2 , 30 , 3 , 0 , 0 , 0}
            }
        };



    }
    public override void HitSomeone(short points)
    {
        base.HitSomeone(points);
        RestoreHealth(15);

    }

}
