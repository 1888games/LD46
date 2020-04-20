using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{

	public int Productivity = 0;
	public int Level = 0;
	public bool Active = false;

	public List<Desk> Desks;

	public Game () {

		Productivity = 0;
		Level = 0;	
		Desks = new List<Desk> ();

	}
   
}
