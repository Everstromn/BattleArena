using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { Creature, Spell, Building, Nuetral}
public enum CreatureType { Undead, ForestKin, Greenskin, Elemental}
public enum SpellType { Damage, Spawn, Buff, Debuff, Resource, Stun}

public enum StatType { Range, Movement, Attack, Health, Poison, Stun}

public enum Team { Blue, Red, None}

public enum BattlePhase { Upkeep, Draw, Orders, Action, Completion}