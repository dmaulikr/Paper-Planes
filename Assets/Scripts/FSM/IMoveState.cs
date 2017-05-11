﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveState : EntityState {
	Direction Direction
	{
		get;
		set;
	}
}
